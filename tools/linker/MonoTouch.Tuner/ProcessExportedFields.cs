using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;

using Xamarin.Linker;

namespace MonoTouch.Tuner {

	//
	// We do not want to list symbols for properties that are linked away.
	// This poses a minor challenge, because the [Field] attribute is linked away
	// before properties are marked, so by the time we know which properties are linked
	// away and which aren't, we don't know the symbol to keep anymore.
	// 
	// So we have a pre-processing step that collect all the properties with a
	// Field attribute, and store corresponding symbol as an annotation.
	//
	// Then at the end of the linker process (ListExportedSymbols step)
	// we lookup that annotation.
	//

	public class ProcessExportedFields : BaseStep {
		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			if (!assembly.MainModule.HasTypeReference (Namespaces.Foundation + ".FieldAttribute"))
				return;

			if (!assembly.MainModule.HasTypes)
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

			if (type.HasProperties) {
				foreach (var property in type.Properties)
					ProcessProperty (property);
			}
		}

		void ProcessProperty (PropertyDefinition property)
		{
			if (!property.HasCustomAttributes)
				return;

			var symbol = GetFieldSymbol (property);
			if (symbol is null)
				return;

			Annotations.GetCustomAnnotations ("ExportedFields").Add (property, symbol);
		}

		internal static string GetFieldSymbol (PropertyDefinition property)
		{
			if (!property.HasCustomAttributes)
				return null;

			foreach (CustomAttribute attrib in property.CustomAttributes) {
				var declaringType = attrib.Constructor.DeclaringType.Resolve ();

				if (!declaringType.Is (Namespaces.Foundation, "FieldAttribute"))
					continue;

				if (attrib.ConstructorArguments.Count != 2)
					continue;

				var libraryName = (string) attrib.ConstructorArguments [1].Value;
				if (libraryName != "__Internal")
					continue;

				return (string) attrib.ConstructorArguments [0].Value;
			}

			return null;
		}
	}
}
