using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;

using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Tuner;
using Xamarin.Utils;

namespace Xamarin.Linker.Steps
{
	public class ListExportedSymbols : BaseStep
	{
		PInvokeWrapperGenerator state;
		bool skip_sdk_assemblies;

		public DerivedLinkContext DerivedLinkContext {
			get {
#if NET
				return LinkerConfiguration.GetInstance (Context).DerivedLinkContext;
#else
				return (DerivedLinkContext) Context;
#endif
			}
		}

		internal ListExportedSymbols (PInvokeWrapperGenerator state, bool skip_sdk_assemblies = false)
		{
			this.state = state;
			this.skip_sdk_assemblies = skip_sdk_assemblies;
		}

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

			foreach (var type in assembly.MainModule.Types)
				ProcessType (type);
		}

		void ProcessType (TypeDefinition type)
		{
			if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes)
					ProcessType (nested);
			}

			if (type.HasMethods) {
				foreach (var method in type.Methods)
					ProcessMethod (method);
			}

			AddRequiredObjectiveCType (type);
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

		void ProcessMethod (MethodDefinition method)
		{
			if (method.IsPInvokeImpl && method.HasPInvokeInfo && method.PInvokeInfo != null) {
				var pinfo = method.PInvokeInfo;

				if (state != null) {
					switch (pinfo.EntryPoint) {
					case "objc_msgSend":
					case "objc_msgSendSuper":
					case "objc_msgSend_stret":
					case "objc_msgSendSuper_stret":
					case "objc_msgSend_fpret":
						state.ProcessMethod (method);
						break;
					default:
						break;
					}
				}

				switch (pinfo.Module.Name) {
				case "__Internal":
					Driver.Log (4, "Adding native reference to {0} in {1} because it's referenced by {2} in {3}.", pinfo.EntryPoint, pinfo.Module.Name, method.FullName, method.Module.Name);
					DerivedLinkContext.RequiredSymbols.AddFunction (pinfo.EntryPoint).AddMember (method);
					break;

				case "libSystem.Net.Security.Native":
				case "System.Net.Security.Native":
#if NET
					if (DerivedLinkContext.App.Platform == ApplePlatform.TVOS) {
						Driver.Log (4, "Did not add native reference to {0} in {1} referenced by {2} in {3}.", pinfo.EntryPoint, pinfo.Module.Name, method.FullName, method.Module.Name);
						break; // tvOS does not ship with System.Net.Security.Native due to https://github.com/dotnet/runtime/issues/45535
					}

					if (DerivedLinkContext.App.Platform == ApplePlatform.MacOSX) {
						Driver.Log (4, "Did not add native reference to {0} in {1} referenced by {2} in {3}.", pinfo.EntryPoint, pinfo.Module.Name, method.FullName, method.Module.Name);
						break; // The macOS version of the BCL has several references to native methods supposedly in libSystem.Net.Security.Native that aren't there, so skip it.
					}

					goto case "System.Native";
#endif
				case "libSystem.Native":
				case "System.Native":
#if NET
					if (DerivedLinkContext.App.Platform == ApplePlatform.MacOSX) {
						Driver.Log (4, "Did not add native reference to {0} in {1} referenced by {2} in {3}.", pinfo.EntryPoint, pinfo.Module.Name, method.FullName, method.Module.Name);
						break; // The macOS version of the BCL has several references to native methods supposedly in libSystem.Native that aren't there, so skip it.
					}
					goto case "System.Security.Cryptography.Native.Apple";
#endif
				case "libSystem.Security.Cryptography.Native.Apple":
				case "System.Security.Cryptography.Native.Apple":
					Driver.Log (4, "Adding native reference to {0} in {1} because it's referenced by {2} in {3}.", pinfo.EntryPoint, pinfo.Module.Name, method.FullName, method.Module.Name);
					DerivedLinkContext.RequireMonoNative = true;
					DerivedLinkContext.RequiredSymbols.AddFunction (pinfo.EntryPoint).AddMember (method);
					break;
				default:
					Driver.Log (4, "Did not add native reference to {0} in {1} referenced by {2} in {3}.", pinfo.EntryPoint, pinfo.Module.Name, method.FullName, method.Module.Name);
					break;
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
		}
	}
}
