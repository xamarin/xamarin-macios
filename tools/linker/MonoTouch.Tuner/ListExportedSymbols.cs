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
					DerivedLinkContext.RequiredSymbols.AddFunction (pinfo.EntryPoint).AddMember (method);
					break;

				case "System.Net.Security.Native":
#if NET
					if (DerivedLinkContext.App.Platform == ApplePlatform.TVOS)
						break; // tvOS does not ship with System.Net.Security.Native due to https://github.com/dotnet/runtime/issues/45535
					goto case "System.Native";
#endif
				case "System.Native":
				case "System.Security.Cryptography.Native.Apple":
					DerivedLinkContext.RequireMonoNative = true;
					DerivedLinkContext.RequiredSymbols.AddFunction (pinfo.EntryPoint).AddMember (method);
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
