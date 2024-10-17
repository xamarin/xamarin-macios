using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Extensions;

static class TypeSymbolExtensions {
	public static bool IsSmartEnum (this ITypeSymbol symbol)
	{
		var boundAttributes = symbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			return false;
		}

		// do not use LINQ here, we need to check if the attribute is present
		foreach (var attributeData in boundAttributes) {
			if (attributeData.AttributeClass?.ToDisplayString () == AttributesNames.BindingAttribute)
				return true;
		}
		return false;
	}

	public static string GetSmartEnumType (this ITypeSymbol symbol)
	{
		// TODO: look into the backing type of the smart enum
		return "NSString";
	}

	public static Dictionary<string, AttributeData> GetAttributeData (this ISymbol symbol)
	{
		var boundAttributes = symbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			// return an empty dictionary if there are no attributes
			return new();
		}

		var attributes = new Dictionary<string, AttributeData> ();
		foreach (var attributeData in boundAttributes) {
			var attrName = attributeData.AttributeClass?.ToDisplayString ();
			if (string.IsNullOrEmpty (attrName))
				continue;
			if (!attributes.TryAdd (attrName, attributeData)) {
				// TODO: diagnostics
			}
		}

		return attributes;
	}
}
