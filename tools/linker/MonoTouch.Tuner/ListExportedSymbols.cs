using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;
using Xamarin.Linker;

#if MONOMAC
using DerivedLinkContext = MonoMac.Tuner.MonoMacLinkContext;
#else
using DerivedLinkContext = MonoTouch.Tuner.MonoTouchLinkContext;
#endif

namespace MonoTouch.Tuner
{
	public class ListExportedSymbols : BaseStep
	{
		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			base.ProcessAssembly (assembly);

			if (Context.Annotations.GetAction (assembly) == AssemblyAction.Delete)
				return;

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
		}

		void ProcessMethod (MethodDefinition method)
		{
			if (method.IsPInvokeImpl && method.HasPInvokeInfo && method.PInvokeInfo.Module.Name == "__Internal")
				((DerivedLinkContext) Context).RequiredSymbols[method.PInvokeInfo.EntryPoint] = method;

			if (MarkStep.IsPropertyMethod (method)) {
				var property = MarkStep.GetProperty (method);
				object symbol;
				// The Field attribute may have been linked away, but we've stored it in an annotation.
				if (property != null && Context.Annotations.GetCustomAnnotations ("ExportedFields").TryGetValue (property, out symbol)) {
					((DerivedLinkContext) Context).RequiredSymbols[(string) symbol] = property;
				}
			}
		}
	}
}
