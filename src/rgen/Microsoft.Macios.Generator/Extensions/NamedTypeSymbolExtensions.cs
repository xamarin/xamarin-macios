using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using ObjCBindings;

namespace Microsoft.Macios.Generator.Extensions;

using ExportedConstructor = ExportedMember<IMethodSymbol, ExportData<Constructor>>;
using ExportedField = ExportedMember<IPropertySymbol, FieldData<Field>>;
using ExportedMethod = ExportedMember<IMethodSymbol, ExportData<Method>>;
using ExportedProperty = ExportedMember<IPropertySymbol, ExportData<Property>>;

static class NamedTypeSymbolExtensions {
	public static bool TryGetEnumFields (this INamedTypeSymbol enumSymbol,
		[NotNullWhen (true)] out ImmutableArray<(IFieldSymbol Symbol, FieldData<EnumValue> FieldData)>? fields,
		[NotNullWhen (false)] out ImmutableArray<Diagnostic>? diagnostics)
	{
		fields = null;
		diagnostics = null;

		// we can only return fields for enums
		if (enumSymbol.TypeKind != TypeKind.Enum) {
			diagnostics = [
				Diagnostic.Create (BindingSourceGeneratorGenerator.RBI0000,
					enumSymbol.Locations [0], enumSymbol.ToDisplayString ().Trim ())
			];
			return false;
		}

		// because we are dealing with an enum, we need to get all the fields from the symbol but we need to
		// keep the order in which they are defined in the source code.

		var fieldBucket =
			ImmutableArray.CreateBuilder<(IFieldSymbol Symbol, FieldData<EnumValue> FieldData)> ();

		var members = enumSymbol.GetMembers ().OfType<IFieldSymbol> ().ToArray ();
		foreach (var fieldSymbol in members) {
			var attributes = fieldSymbol.GetAttributeData ();
			if (attributes.Count == 0)
				continue;

			// Get all the FieldAttribute, parse it and add the data to the result
			if (attributes.TryGetValue (AttributesNames.EnumFieldAttribute, out var fieldAttrData)) {
				var fieldSyntax = fieldAttrData.ApplicationSyntaxReference?.GetSyntax ();
				if (fieldSyntax is null)
					continue;

				if (FieldData<EnumValue>.TryParse (fieldSyntax, fieldAttrData, out var fieldData)) {
					fieldBucket.Add ((Symbol: fieldSymbol, FieldData: fieldData));
				} else {
					// TODO: diagnostics
				}
			}
		}

		fields = fieldBucket.ToImmutable ();
		return true;
	}

	public static bool TryGetBindingData (this INamedTypeSymbol self,
		[NotNullWhen (true)] out BindingTypeData? bindingData)
	{
		// retrieve the attribute data of the object and try to get the binding data
		bindingData = null;
		var attributes = self.GetAttributeData ();
		if (attributes.TryGetValue (AttributesNames.BindingAttribute, out var bindingTypeAttrData)) {
			var bindingTypeSyntax = bindingTypeAttrData.ApplicationSyntaxReference?.GetSyntax ();
			if (bindingTypeSyntax is null)
				return false;

			return BindingTypeData.TryParse (bindingTypeSyntax, bindingTypeAttrData, out bindingData);
		}

		return false;
	}

	public static bool TryGetExportedMembers (this INamedTypeSymbol self,
		[NotNullWhen (true)] out ExportedMembers? exported)
	{
		exported = null;
		// we can only return exported members for classes
		if (self.TypeKind != TypeKind.Class)
			return false;

		var constructors = ImmutableArray.CreateBuilder<ExportedConstructor> ();
		var fields = ImmutableArray.CreateBuilder<ExportedField> ();
		var methods = ImmutableArray.CreateBuilder<ExportedMethod> ();
		var properties = ImmutableArray.CreateBuilder<ExportedProperty> ();

		// return only those members that are methods, have the export attribute and have been marked as constructors
		var members = self.GetMembers ();
		foreach (var symbol in members) {
			// ignore those symbols that are not a method or property that is partial and virtual
			if (!symbol.IsValidExportedSymbol ())
				continue;

			// try to get the export data for the current symbol, if there is none it means that the
			// export attribute was not added to the symbol
			var attributes = symbol.GetAttributeData ();
			// the member can be of 3 types:
			// 1. A constructor
			// 2. A field
			// 3. A method
			// 4. A property
			string? attrName = null;
			AttributeData? exportAttrData = null;
			foreach (var name in AttributesNames.MethodAttributes) {
				if (attributes.TryGetValue (name, out exportAttrData)) {
					attrName = name;
					break;
				}
			}

			if (exportAttrData is null)
				continue;

			var exportSyntax = exportAttrData.ApplicationSyntaxReference?.GetSyntax ();
			if (exportSyntax is null)
				// could not get the syntax node or we could not find a valid export attribute
				continue;
			// parse the export data based on the attribute name
			switch (attrName) {
			case AttributesNames.ConstructorAttribute:
				if (ExportData<Constructor>.TryParse (exportSyntax, exportAttrData, out var constructorData)
				    && symbol is IMethodSymbol methoSymbol) {
					constructors.Add (new(methoSymbol, constructorData));
				}

				break;
			case AttributesNames.FieldAttribute:
				if (FieldData<Field>.TryParse (exportSyntax, exportAttrData, out var fieldData)
				    && symbol is IPropertySymbol fieldSymbol) {
					fields.Add (new(fieldSymbol, fieldData));
				}

				break;
			case AttributesNames.MethodAttribute:
				if (ExportData<Method>.TryParse (exportSyntax, exportAttrData, out var methodData)
				    && symbol is IMethodSymbol methodSymbol) {
					methods.Add (new(methodSymbol, methodData));
				}

				break;
			case AttributesNames.PropertyAttribute:
				if (ExportData<Property>.TryParse (exportSyntax, exportAttrData, out var propertyData)
				    && symbol is IPropertySymbol propertySymbol) {
					properties.Add (new(propertySymbol, propertyData));
				}

				break;
			}
		}

		exported = new(constructors.ToImmutable (), fields.ToImmutable (), methods.ToImmutable (),
			properties.ToImmutable ());
		return true;
	}

	public static bool HasAttribute (this INamedTypeSymbol symbol, string attributeName)
	{
		var attributes = symbol.GetAttributeData ();
		return attributes.ContainsKey (attributeName);
	}

	public static bool IsSmartEnum (this INamedTypeSymbol symbol)
	{
		// we are looking to make sure that the symbol is a IEnum symbol and that it contains the
		// binding type attribute. Else it is not
		return symbol.TypeKind == TypeKind.Enum && symbol.HasAttribute (AttributesNames.BindingAttribute);
	}
}
