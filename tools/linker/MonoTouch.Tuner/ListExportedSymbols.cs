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

#nullable enable

namespace Xamarin.Linker.Steps {
#if ASSEMBLY_PREPARER
	public class ListExportedSymbols : AssemblyModifierStep {
		protected override string Name { get; } = "List Exported Symbols";
		protected override int ErrorCode { get; } = 2510;
#else
	public class ListExportedSymbols : BaseStep {
#endif
		PInvokeWrapperGenerator? state;

		PInvokeWrapperGenerator? State {
			get {
				if (state is null && DerivedLinkContext.App.RequiresPInvokeWrappers) {
					Configuration.PInvokeWrapperGenerationState = new PInvokeWrapperGenerator () {
						App = DerivedLinkContext.App,
						SourcePath = Path.Combine (Configuration.CacheDirectory, "pinvokes.mm"),
						HeaderPath = Path.Combine (Configuration.CacheDirectory, "pinvokes.h"),
						Registrar = DerivedLinkContext.StaticRegistrar,
					};
					state = Configuration.PInvokeWrapperGenerationState;
				}
				return state;
			}
		}

#if ASSEMBLY_PREPARER
		protected override void TryEndProcess ()
#else
		protected override void EndProcess ()
#endif
		{
			if (state?.Started == true) {
				// The generator is 'started' by the linker, which means it may not
				// be started if the linker was not executed due to re-using cached results.
				state.End ();
			}
#if !ASSEMBLY_PREPARER
			base.EndProcess ();
#endif
		}

#if !ASSEMBLY_PREPARER
		public LinkerConfiguration Configuration {
			get {
				return LinkerConfiguration.GetInstance (Context);
			}
		}

		public DerivedLinkContext DerivedLinkContext {
			get {
				return Configuration.DerivedLinkContext;
			}
		}
#endif

		public ListExportedSymbols ()
		{
		}

#if ASSEMBLY_PREPARER
		protected override bool ModifyAssembly (AssemblyDefinition assembly)
		{
			if (Annotations.GetAction (assembly) == AssemblyAction.Delete)
				return false;

			if (!assembly.MainModule.HasTypes)
				return false;

			if (!HasSymbols (assembly))
				return false;

			// The base class recurses into all the types (including nested types) and
			// saves the assembly through the AppBundleRewriter if we modified it (i.e.
			// if ProcessType/ProcessMethod returned true).
			return base.ModifyAssembly (assembly);
		}

		protected override bool ProcessType (TypeDefinition type)
		{
			// The base class takes care of recursing into nested types.
			var modified = ProcessMethods (type);
			AddRequiredObjectiveCType (type);
			return modified;
		}
#else
		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			base.ProcessAssembly (assembly);

			if (Annotations.GetAction (assembly) == AssemblyAction.Delete)
				return;

			if (!assembly.MainModule.HasTypes)
				return;

			if (!HasSymbols (assembly))
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
#endif

		static bool HasSymbols (AssemblyDefinition assembly)
		{
			if (assembly.MainModule.HasModuleReferences)
				return true;
			if (assembly.MainModule.HasTypeReference (Namespaces.Foundation + ".FieldAttribute"))
				return true;
			return false;
		}

		void AddRequiredObjectiveCType (TypeDefinition type)
		{
			if (TryGetRequiredObjectiveCType (DerivedLinkContext, type, out var exportedName))
				DerivedLinkContext.RequiredSymbols.AddObjectiveCClass (exportedName).AddMember (type);
		}

		// Returns true if the specified type represents an Objective-C class that should be referenced as a required symbol, so that the native linker doesn't link it away.
		public static bool TryGetRequiredObjectiveCType (DerivedLinkContext derivedLinkContext, TypeDefinition type, [NotNullWhen (true)] out string? exportedName)
		{
			exportedName = null;

			// The product assembly only has one type we may need to keep: XamarinSwiftFunctions
			if (derivedLinkContext.LinkerConfiguration.Profile.IsProductAssembly (type.Module.Assembly)) {
				switch (type.Name) {
				case "XamarinSwiftFunctions":
					break;
				default:
					return false;
				}
			}

			var staticRegistrar = derivedLinkContext.StaticRegistrar;
			if (staticRegistrar is null)
				return false;

			if (!staticRegistrar.TryGetExportedTypeName (type, out exportedName))
				return false;

			if (derivedLinkContext.App.RequireLinkWithAttributeForObjectiveCClassSearch) {
				var has_linkwith_attributes = false;
				if (derivedLinkContext.App.Assemblies.TryGetValue (type.Module.Assembly, out var asm))
					has_linkwith_attributes = asm.HasLinkWithAttributes;
				if (!has_linkwith_attributes)
					return false;
			}

			return true;
		}

		// Whether we need to collect [Field] symbols referenced via Dlfcn here (instead of relying on the
		// inlined 'xamarin_Dlfcn_*_Native' P/Invokes that a post-trim scan would otherwise pick up). This is
		// the case when InlineDlfcnMethodsStep didn't inline the call sites for the assembly:
		// * when inlining is disabled globally, or
		// * in a Hot Reload compatible build, for reloadable assemblies (AssemblyAction != Link), which
		//   InlineDlfcnMethodsStep intentionally leaves byte-unmodified.
		// InlineDlfcnMethodsStep runs in a separate process (the "prepare" pass)
		// whose collected symbols are discarded before the "post-process" pass runs GenerateReferencesStep,
		// so collecting here (in the post-process pass) is what actually keeps the symbol alive.
		bool ShouldCollectFieldSymbols (MethodDefinition method)
		{
			if (!Configuration.InlineDlfcnMethodsEnabled)
				return true;
			if (Configuration.HotReloadCompatibleBuild && Annotations.GetAction (method.Module.Assembly) != AssemblyAction.Link)
				return true;
			return false;
		}

#if ASSEMBLY_PREPARER
		protected override bool ProcessMethod (MethodDefinition method)
#else
		bool ProcessMethod (MethodDefinition method)
#endif
		{
			var modified = false;

			if (method.IsPInvokeImpl && method.HasPInvokeInfo && method.PInvokeInfo is not null) {
				var pinfo = method.PInvokeInfo;
				bool addPInvokeSymbol = false;

				if (State is not null) {
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

				// Create a list of all the libraries from Mono that we'll link with
				// We add 4 different variations for each library:
				// * with and without a "lib" prefix
				// * with and without the ".dylib" extension
				var app = LinkerConfiguration.GetInstance (Context).Application;
				var monoLibraryVariationsEnumerable = app.MonoLibraries.
					Where (v => v.EndsWith (".dylib", StringComparison.OrdinalIgnoreCase) || v.EndsWith (".a", StringComparison.OrdinalIgnoreCase)).
					Select (v => Path.GetFileNameWithoutExtension (v)).
					Select (v => v.StartsWith ("lib", StringComparison.OrdinalIgnoreCase) ? v.Substring (3) : v);
				var monoLibraryVariations = new HashSet<string> (monoLibraryVariationsEnumerable);
				monoLibraryVariations.Add ("System.Globalization.Native"); // System.Private.CoreLib has P/Invokes pointing to libSystem.Globalization.Native, but they're actually in libmonosgen-2.0
				monoLibraryVariations.UnionWith (monoLibraryVariations.Select (v => "lib" + v).ToArray ());
				monoLibraryVariations.UnionWith (monoLibraryVariations.Select (v => v + ".dylib").ToArray ());
				// If the P/Invoke points to any of those libraries, then we add it as a P/Invoke symbol.
				if (monoLibraryVariations.Contains (pinfo.Module.Name))
					addPInvokeSymbol = true;

				switch (pinfo.Module.Name) {
				case "__Internal":
					if (Configuration.Application.XamarinRuntime == XamarinRuntime.NativeAOT) {
						// For NativeAOT builds, don't add inlined dlfcn P/Invoke wrappers as
						// required symbols: only the surviving ones will have native code generated,
						// so force-referencing all of them causes linker errors for symbols that
						// NativeAOT trimmed away. For non-NativeAOT builds, the wrappers are resolved
						// via dlsym and need the -u flags to be exported from the binary.
						if (Configuration.InlineDlfcnMethodsEnabled && pinfo.EntryPoint.StartsWith (InlineDlfcnMethodsStep.PInvokePrefix, StringComparison.Ordinal))
							break;
						// Same goes for inlined Class.GetHandle calls.
						if (Configuration.InlineClassGetHandle != InlineClassGetHandleMode.Disabled && pinfo.EntryPoint.StartsWith (InlineClassGetHandleStep.PInvokePrefix, StringComparison.Ordinal))
							break;
					}
					DerivedLinkContext.App.Log (4, "Adding native reference to {0} in {1} because it's referenced by {2} in {3}.", pinfo.EntryPoint, pinfo.Module.Name, method.FullName, method.Module.Name);
					DerivedLinkContext.RequiredSymbols.AddFunction (pinfo.EntryPoint).AddMember (method);
					break;

				default:
					if (!addPInvokeSymbol)
						DerivedLinkContext.App.Log (4, "Did not add native reference to {0} in {1} referenced by {2} in {3}.", pinfo.EntryPoint, pinfo.Module.Name, method.FullName, method.Module.Name);
					break;
				}

				if (addPInvokeSymbol) {
					DerivedLinkContext.App.Log (4, "Adding native reference to {0} in {1} because it's referenced by {2} in {3}.", pinfo.EntryPoint, pinfo.Module.Name, method.FullName, method.Module.Name);
					DerivedLinkContext.RequireMonoNative = true;
					if (DerivedLinkContext.App.Platform != ApplePlatform.MacOSX &&
						DerivedLinkContext.App.LibMonoNativeLinkMode == AssemblyBuildTarget.StaticObject) {
						DerivedLinkContext.RequiredSymbols.AddFunction (pinfo.EntryPoint).AddMember (method);
					}
				}
			}

			if (method.IsPropertyMethod () && ShouldCollectFieldSymbols (method)) {
				var property = method.GetProperty ();
				// The Field attribute may have been linked away, but we've stored it in an annotation.
				if (property is not null && Annotations.GetCustomAnnotations ("ExportedFields").TryGetValue (property, out var symbol) && symbol is string symbolStr) {
					DerivedLinkContext.RequiredSymbols.AddField (symbolStr).AddMember (property);
				}
			}

			return modified;
		}
	}
}
