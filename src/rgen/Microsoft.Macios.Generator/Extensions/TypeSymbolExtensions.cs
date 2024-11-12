using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;

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

	/// <summary>
	/// Return the list of parent symbols in ascending order.
	/// </summary>
	/// <param name="symbol">The symbol whose parents to retrieve.</param>
	/// <returns>Array with all the parent symbols until we reach the containing namespace.</returns>
	public static ImmutableArray<ISymbol> GetParents (this ISymbol symbol)
	{
		var result = new List<ISymbol> ();
		var current = symbol.ContainingSymbol;
		if (current is null)
			return [];

		while (current is not INamespaceSymbol) {
			result.Add (current);
			current = current.ContainingSymbol;
		}

		return [.. result];
	}

	static SymbolAvailability GetAvailabilityForSymbol (this ISymbol symbol)
	{
		//get the attribute of the symbol and look for the Supported and Unsupported attributes and
		// add the different platforms to the result hashsets
		var builder = SymbolAvailability.CreateBuilder ();
		var boundAttributes = symbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			// no attrs in the symbol, therefore the symbol is supported in all platforms
			return builder.ToImmutable ();
		}

		foreach (var attributeData in boundAttributes) {
			var attrName = attributeData.AttributeClass?.ToDisplayString ();
			if (string.IsNullOrEmpty (attrName))
				continue;
			// we only care in this case about the support and unsupported attrs, ignore any other
			switch (attrName) {
			case AttributesNames.SupportedOSPlatformAttribute:
				if (SupportedOSPlatformData.TryParse (attributeData, out var supportedPlatform)) {
					builder.Add (supportedPlatform.Value);
				}

				break;
			case AttributesNames.UnsupportedOSPlatformAttribute:
				if (UnsupportedOSPlatformData.TryParse (attributeData, out var unsupportedPlatform)) {
					builder.Add (unsupportedPlatform.Value);
				}

				break;
			case AttributesNames.ObsoletedOSPlatformAttribute:
				if (ObsoletedOSPlatformData.TryParse (attributeData, out var obsoletedOsPlatform)) {
					builder.Add (obsoletedOsPlatform.Value);
				}

				break;
			default:
				continue;
			}
		}

		return builder.ToImmutable ();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="symbol"></param>
	/// <returns></returns>
	public static SymbolAvailability GetSupportedPlatforms (this ISymbol symbol)
	{
		var availability = GetAvailabilityForSymbol (symbol);
		// get the parents and return the merge
		foreach (var parent in GetParents (symbol)) {
			availability = availability.MergeWithParent (GetAvailabilityForSymbol (parent));
		}
		return availability;
	}
}
