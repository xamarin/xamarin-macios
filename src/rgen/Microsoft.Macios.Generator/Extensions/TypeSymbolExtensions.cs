using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Microsoft.Macios.Generator.Extensions;

static class TypeSymbolExtensions {

	public static Dictionary<string, AttributeData> GetAttributeData (this ISymbol symbol)
	{
		var boundAttributes = symbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			// return an empty dictionary if there are no attributes
			return new ();
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

	/// <summary>
	/// Returns if a symbol is a valid exported symbol. That means that we are dealing with a symbol that
	/// is either a method or a property that is partial and virtual.
	/// </summary>
	/// <param name="symbol">The symbol under check.</param>
	/// <returns>True for method or properties that are partial and virtual.</returns>
	public static bool IsValidExportedSymbol (this ISymbol symbol)
	{
		// export symbols can only be methods, properties that are partial and virtual
		if (symbol is IMethodSymbol methodSymbol) {
			return methodSymbol is { IsPartialDefinition: true, IsVirtual: true };
		}

		// properties as special, we need to look at their get/set method
		if (symbol is IPropertySymbol propertySymbol) {
			return propertySymbol is { IsVirtual: true, IsPartialDefinition: true };
		}

		return false;
	}

}
