using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using ObjCBindings;

namespace Microsoft.Macios.Generator.Extensions;

static class FieldSymbolExtensions {
	public static FieldData<EnumValue>? GetFieldData (this IFieldSymbol fieldSymbol)
	{
		var attributes = fieldSymbol.GetAttributeData ();
		if (attributes.Count == 0)
			return null;

		// Get all the FieldAttribute, parse it and add the data to the result
		if (!attributes.TryGetValue (AttributesNames.EnumFieldAttribute, out var fieldAttrDataList) ||
			fieldAttrDataList.Count != 1)
			return null;

		var fieldAttrData = fieldAttrDataList [0];
		var fieldSyntax = fieldAttrData.ApplicationSyntaxReference?.GetSyntax ();
		if (fieldSyntax is null)
			return null;

		if (FieldData<EnumValue>.TryParse (fieldAttrData, out var fieldData))
			return fieldData.Value;

		return null;
	}
}
