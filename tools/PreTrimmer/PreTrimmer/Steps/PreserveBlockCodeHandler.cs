using System;
using System.Linq;

using Mono.Cecil;

using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;

using Xamarin.Bundler;

#nullable enable

namespace Xamarin.Linker.Steps {
	public class PreserveBlockCodeHandler : ConfigurationAwareStep {
		protected override string Name { get; } = "Preserve Block Code";
		protected override int ErrorCode { get; } = 2240;

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			foreach (var type in assembly.Modules.SelectMany (v => v.Types)) {
				ProcessTypes (type);
			}
		}

		void ProcessTypes (TypeDefinition type)
		{
			ProcessType (type);
			foreach (var nestedType in type.NestedTypes) {
				ProcessTypes (nestedType);
			}
		}

		void ProcessType (TypeDefinition type)
		{
			if (!type.HasFields || !type.IsAbstract || !type.IsSealed || !type.IsNested)
				return;
			if (type.Fields.Count != 1)
				return;

			// The type is also nested inside ObjCRuntime.Trampolines class)
			var nestingType = type.DeclaringType;
			if (!nestingType.Is ("ObjCRuntime", "Trampolines"))
				return;

			// The class has a readonly field named 'Handler'
			var field = type.Fields [0];
			if (!field.IsInitOnly)
				return;
			if (field.Name != "Handler")
				return;

			// The class has a parameterless 'Invoke' method with a 'MonoPInvokeCallback' attribute
			if (!type.HasMethods)
				return;
			var method = type.Methods.SingleOrDefault (v => {
				if (v.Name != "Invoke")
					return false;
				if (!v.HasParameters)
					return false;
				if (!v.HasCustomAttributes)
					return false;
				if (!v.CustomAttributes.Any (v => v.AttributeType.Name == "MonoPInvokeCallbackAttribute"))
					return false;
				return true;
			});

			if (method is null)
				return;

			// The type was used, so preserve the method and field
			Context.Annotations.AddPreservedMethod (type, method);
			Context.Annotations.AddPreservedField (type, field);
		}
	}
}
