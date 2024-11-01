using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Extensions;

static class TypeSymbolExtensions {

	public static Dictionary<string, List<AttributeData>> GetAttributeData (this ISymbol symbol)
	{
		var boundAttributes = symbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			// return an empty dictionary if there are no attributes
			return new ();
		}

		var attributes = new Dictionary<string, List<AttributeData>> ();
		foreach (var attributeData in boundAttributes) {
			var attrName = attributeData.AttributeClass?.ToDisplayString ();
			if (string.IsNullOrEmpty (attrName))
				continue;
			if (!attributes.TryGetValue (attrName, out var attributeDataList)) {
				attributeDataList = new List<AttributeData> ();
				attributes.Add (attrName, attributeDataList);
			}
			attributeDataList.Add (attributeData);
		}

		return attributes;
	}
}
