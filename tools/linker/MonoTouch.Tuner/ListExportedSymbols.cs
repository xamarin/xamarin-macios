using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;

using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Tuner;
using Xamarin.Utils;

namespace Xamarin.Linker.Steps {
	public class ListExportedSymbols : BaseStep {
		PInvokeWrapperGenerator state;
#if !NET
		bool skip_sdk_assemblies;
#endif

		PInvokeWrapperGenerator State {
			get {
#if NET
				if (state is null && DerivedLinkContext.App.RequiresPInvokeWrappers) {
					Configuration.PInvokeWrapperGenerationState = new PInvokeWrapperGenerator () {
						App = DerivedLinkContext.App,
						SourcePath = Path.Combine (Configuration.CacheDirectory, "pinvokes.mm"),
						HeaderPath = Path.Combine (Configuration.CacheDirectory, "pinvokes.h"),
						Registrar = DerivedLinkContext.StaticRegistrar,
					};
					state = Configuration.PInvokeWrapperGenerationState;
				}
#endif
				return state;
			}
		}

#if NET
		protected override void EndProcess ()
		{
			if (state?.Started == true) {
				// The generator is 'started' by the linker, which means it may not
				// be started if the linker was not executed due to re-using cached results.
				state.End ();
			}
			base.EndProcess ();
		}
#endif

#if NET
		public LinkerConfiguration Configuration {
			get {
				return LinkerConfiguration.GetInstance (Context);
			}
		}
#endif

		public DerivedLinkContext DerivedLinkContext {
			get {
#if NET
				return Configuration.DerivedLinkContext;
#else
				return (DerivedLinkContext) Context;
#endif
			}
		}

#if NET
		public ListExportedSymbols ()
		{
		}
#else
		internal ListExportedSymbols (PInvokeWrapperGenerator state, bool skip_sdk_assemblies = false)
		{
			this.state = state;
			this.skip_sdk_assemblies = skip_sdk_assemblies;
		}
#endif

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			base.ProcessAssembly (assembly);

			if (Annotations.GetAction (assembly) == AssemblyAction.Delete)
				return;

#if !NET
			if (skip_sdk_assemblies && Profile.IsSdkAssembly (assembly))
				return;
#endif

			if (!assembly.MainModule.HasTypes)
				return;

			var hasSymbols = false;
			if (assembly.MainModule.HasModuleReferences) {
				hasSymbols = true;
			} else if (assembly.MainModule.HasTypeReference (Namespaces.Foundation + ".FieldAttribute")) {
				hasSymbols = true;
			}
			if (!hasSymbols)
				return;

			var modified = false;
			foreach (var type in assembly.MainModule.Types)
				modified |= ProcessType (type);

			// Make sure the linker saves any changes in the assembly.
			if (modified) {
				var action = Context.Annotations.GetAction (assembly);
				if (action == AssemblyAction.Copy)
					Context.Annotations.SetAction (assembly, AssemblyAction.Save);
			}
		}

		bool ProcessType (TypeDefinition type)
		{
			var modified = false;
			if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes)
					modified |= ProcessType (nested);
			}

			if (type.HasMethods) {
				foreach (var method in type.Methods)
					modified |= ProcessMethod (method);
			}

			AddRequiredObjectiveCType (type);

			return modified;
		}

		void AddRequiredObjectiveCType (TypeDefinition type)
		{
			var registerAttribute = DerivedLinkContext.StaticRegistrar?.GetRegisterAttribute (type);
			if (registerAttribute == null)
				return;

			if (!registerAttribute.IsWrapper)
				return;

			if (DerivedLinkContext.StaticRegistrar.HasProtocolAttribute (type))
				return;

			Assembly asm;
			bool has_linkwith_attributes = false;
			if (DerivedLinkContext.Target.Assemblies.TryGetValue (type.Module.Assembly, out asm))
				has_linkwith_attributes = asm.HasLinkWithAttributes;

			if (has_linkwith_attributes) {
				var exportedName = DerivedLinkContext.StaticRegistrar.GetExportedTypeName (type, registerAttribute);
				DerivedLinkContext.RequiredSymbols.AddObjectiveCClass (exportedName).AddMember (type);
			}
		}

		bool ProcessMethod (MethodDefinition method)
		{
			var modified = false;

			if (method.IsPInvokeImpl && method.HasPInvokeInfo && method.PInvokeInfo != null) {
				var pinfo = method.PInvokeInfo;
				bool addPInvokeSymbol = false;

				if (State != null) {
					switch (pinfo.EntryPoint) {
					case "objc_msgSend":
					case "objc_msgSendSuper":
					case "objc_msgSend_stret":
					case "objc_msgSendSuper_stret":
					case "objc_msgSend_fpret":
						State.ProcessMethod (method);
						modified = true;
						break;
					default:
						break;
					}
				}

#if NET
				// Create a list of all the libraries from Mono that we'll link with
				// We add 4 different variations for each library:
				// * with and without a "lib" prefix
				// * with and without the ".dylib" extension
				var app = LinkerConfiguration.GetInstance (Context).Application;
				var monoLibraryVariations = app.MonoLibraries.
					Where (v => v.EndsWith (".dylib", StringComparison.OrdinalIgnoreCase) || v.EndsWith (".a", StringComparison.OrdinalIgnoreCase)).
					Select (v => Path.GetFileNameWithoutExtension (v)).
					Select (v => v.StartsWith ("lib", StringComparison.OrdinalIgnoreCase) ? v.Substring (3) : v).ToHashSet ();
#if !__MACOS__
				monoLibraryVariations.Add ("System.Globalization.Native"); // System.Private.CoreLib has P/Invokes pointing to libSystem.Globalization.Native, but they're actually in libmonosgen-2.0
#endif
				monoLibraryVariations.UnionWith (monoLibraryVariations.Select (v => "lib" + v).ToArray ());
				monoLibraryVariations.UnionWith (monoLibraryVariations.Select (v => v + ".dylib").ToArray ());
				// If the P/Invoke points to any of those libraries, then we add it as a P/Invoke symbol.
				if (monoLibraryVariations.Contains (pinfo.Module.Name))
					addPInvokeSymbol = true;
#endif

				switch (pinfo.Module.Name) {
				case "__Internal":
					Driver.Log (4, "Adding native reference to {0} in {1} because it's referenced by {2} in {3}.", pinfo.EntryPoint, pinfo.Module.Name, method.FullName, method.Module.Name);
					DerivedLinkContext.RequiredSymbols.AddFunction (pinfo.EntryPoint).AddMember (method);
					break;

#if !NET
				case "System.Net.Security.Native":
				case "System.Security.Cryptography.Native.Apple":
				case "System.Native":
					addPInvokeSymbol = true;
					break;
#endif

				default:
					if (!addPInvokeSymbol)
						Driver.Log (4, "Did not add native reference to {0} in {1} referenced by {2} in {3}.", pinfo.EntryPoint, pinfo.Module.Name, method.FullName, method.Module.Name);
					break;
				}

				if (addPInvokeSymbol) {
					Driver.Log (4, "Adding native reference to {0} in {1} because it's referenced by {2} in {3}.", pinfo.EntryPoint, pinfo.Module.Name, method.FullName, method.Module.Name);
					DerivedLinkContext.RequireMonoNative = true;
					if (DerivedLinkContext.App.Platform != ApplePlatform.MacOSX &&
						DerivedLinkContext.App.LibMonoNativeLinkMode == AssemblyBuildTarget.StaticObject) {
						DerivedLinkContext.RequiredSymbols.AddFunction (pinfo.EntryPoint).AddMember (method);
					}
				}
			}

			if (method.IsPropertyMethod ()) {
				var property = method.GetProperty ();
				object symbol;
				// The Field attribute may have been linked away, but we've stored it in an annotation.
				if (property != null && Annotations.GetCustomAnnotations ("ExportedFields").TryGetValue (property, out symbol)) {
					DerivedLinkContext.RequiredSymbols.AddField ((string) symbol).AddMember (property);
				}
			}

			return modified;
		}
	}
}
