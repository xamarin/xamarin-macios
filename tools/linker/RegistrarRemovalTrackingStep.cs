using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;

using Xamarin.Bundler;
using Xamarin.Linker;
#if !NET
using Mono.Linker.Steps;
using Mono.Tuner;
using Xamarin.Tuner;
#endif

namespace MonoTouch.Tuner {

#if NET
	public class RegistrarRemovalTrackingStep : ConfigurationAwareStep {

		protected override string Name { get; } = "RegistrarRemovalTracking";
		protected override int ErrorCode { get; } = 2380;

		int WarnCode => ErrorCode + 7;

		Profile Profile => new Profile (Configuration);

		Optimizations Optimizations => Configuration.Application.Optimizations;

		string PlatformAssemblyName => Configuration.PlatformAssembly;

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			Process (assembly);
		}
#else
	public class RegistrarRemovalTrackingStep : BaseStep {

		Optimizations Optimizations => ((DerivedLinkContext) Context).App.Optimizations;

		string PlatformAssemblyName => ((MobileProfile) Profile.Current).ProductAssembly;

		int WarnCode => 2107; // for compatibility

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			Process (assembly);
			base.ProcessAssembly (assembly);
		}
#endif
		AssemblyDefinition PlatformAssembly;

		bool dynamic_registration_support_required;

		void Process (AssemblyDefinition assembly)
		{
			if (Optimizations.RemoveDynamicRegistrar != false)
				dynamic_registration_support_required |= RequiresDynamicRegistrar (assembly, Optimizations.RemoveDynamicRegistrar == true);
		}

		// If certain conditions are met, we can optimize away the code for the dynamic registrar.
		bool RequiresDynamicRegistrar (AssemblyDefinition assembly, bool warnIfRequired)
		{
			// We know that the SDK assemblies we ship don't use the methods we're looking for.
			if (Profile.IsSdkAssembly (assembly))
				return false;

			// The product assembly itself is safe as long as it's linked
			if (Profile.IsProductAssembly (assembly)) {
				if (Annotations.GetAction (assembly) != AssemblyAction.Link)
					return false;
				PlatformAssembly = assembly;
			}

			// Can't touch the forbidden fruit in the product assembly unless there's a reference to it
			var hasProductReference = false;
			foreach (var ar in assembly.MainModule.AssemblyReferences) {
				if (!Profile.IsProductAssembly (ar.Name))
					continue;
				hasProductReference = true;
				break;
			}
			if (!hasProductReference)
				return false;

			// Check if the assembly references any methods that require the dynamic registrar
			var productAssemblyName = PlatformAssemblyName;
			var requires = false;
			foreach (var mr in assembly.MainModule.GetMemberReferences ()) {
				if (mr.DeclaringType is null || string.IsNullOrEmpty (mr.DeclaringType.Namespace))
					continue;

				var scope = mr.DeclaringType.Scope;
				var name = string.Empty;
				switch (scope.MetadataScopeType) {
				case MetadataScopeType.ModuleDefinition:
					name = ((ModuleDefinition) scope).Assembly.Name.Name;
					break;
				default:
					name = scope.Name;
					break;
				}
				if (name != productAssemblyName)
					continue;

				switch (mr.DeclaringType.Namespace) {
				case "ObjCRuntime":
					switch (mr.DeclaringType.Name) {
					case "Runtime":
						switch (mr.Name) {
						case "ConnectMethod":
							// Req 1: Nobody must call Runtime.ConnectMethod.
							if (warnIfRequired)
								Warn (assembly, mr);
							requires = true;
							break;
						case "RegisterAssembly":
							// Req 3: Nobody must call Runtime.RegisterAssembly
							if (warnIfRequired)
								Warn (assembly, mr);
							requires = true;
							break;
						}
						break;
					case "BlockLiteral":
						// Req 2:
						// * Nobody must call BlockLiteral.SetupBlock[Unsafe].
						// * Nobody must call BlockLiteral..ctor (void*, object, Type, string)
						//
						// Fortunately the linker is able to rewrite:
						//
						// * Calls to SetupBlock[Unsafe] to call SetupBlockImpl
						// * Calls to .ctor(void*, object, Type, string) to call .ctor(void*, object, string)
						//
						// and these overloads don't need the dynamic registrar, which means we only have
						// to look in assemblies that aren't linked.
						if (Annotations.GetAction (assembly) == AssemblyAction.Link && Optimizations.OptimizeBlockLiteralSetupBlock == true)
							break;

						switch (mr.Name) {
						case "SetupBlock":
						case "SetupBlockUnsafe":
							if (warnIfRequired)
								Warn (assembly, mr);

							requires = true;
							break;
						case ".ctor":
							var md = mr.Resolve () as MethodDefinition;
#if NET
							requires |= Xamarin.Linker.OptimizeGeneratedCodeHandler.IsBlockLiteralCtor_Type_String (md);
#else
							requires |= Xamarin.Linker.OptimizeGeneratedCodeSubStep.IsBlockLiteralCtor_Type_String (md);
#endif
							if (requires && warnIfRequired)
								Warn (assembly, mr);
							break;
						}
						break;
					case "TypeConverter":
						switch (mr.Name) {
						case "ToManaged":
							// Req 4: Nobody must call TypeConverter.ToManaged
							if (warnIfRequired)
								Warn (assembly, mr);
							requires = true;
							break;
						}
						break;
					}
					break;
				}
			}

			return requires;
		}

		void Warn (AssemblyDefinition assembly, MemberReference mr)
		{
			ErrorHelper.Warning (WarnCode, Errors.MM2107, assembly.Name.Name, mr.DeclaringType.FullName, mr.Name, string.Join (", ", ((MethodReference) mr).Parameters.Select ((v) => v.ParameterType.FullName)));
		}

#if NET
		protected override void TryEndProcess ()
		{
#else
		protected override void EndProcess ()
		{
			base.EndProcess ();
#endif
			if (!Optimizations.RemoveDynamicRegistrar.HasValue) {
				// If dynamic registration is not required, and removal of the dynamic registrar hasn't already
				// been disabled, then we can remove it!
				Optimizations.RemoveDynamicRegistrar = !dynamic_registration_support_required;
			}

			Driver.Log (4, "Optimization dynamic registrar removal: {0}", Optimizations.RemoveDynamicRegistrar.Value ? "enabled" : "disabled");

			if (Optimizations.RemoveDynamicRegistrar.Value) {
				// ILLink will optimize `Runtime.Initialize` based on `DynamicRegistrationSupported` returning a constant (`true`)
				// and this will runs before we have the chance to set it to `false` in `CoreOptimizedGeneratedCode` so we instead
				// do the change here so the linker can do this without further ado
				// note: it does not matter for _legacy_ so we apply the change (to earlier) to minimize the difference between them
				if (PlatformAssembly is not null) {
					var method = PlatformAssembly.MainModule.GetType ("ObjCRuntime.Runtime").Methods.First ((n) => n.Name == "get_DynamicRegistrationSupported");
					// Rewrite to return 'false'
					var instr = method.Body.Instructions;
					instr.Clear ();
					instr.Add (Instruction.Create (OpCodes.Ldc_I4_0));
					instr.Add (Instruction.Create (OpCodes.Ret));
				}
			}
#if MTOUCH
			var app = (Context as DerivedLinkContext).App;
			if (app.IsCodeShared) {
				foreach (var appex in app.AppExtensions) {
					if (!appex.IsCodeShared)
						continue;
					appex.Optimizations.RemoveDynamicRegistrar = app.Optimizations.RemoveDynamicRegistrar;
				}
			}
#endif
		}
	}
}
