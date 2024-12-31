using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;

namespace Microsoft.Macios.Generator.Extensions;

static class TypeSymbolExtensions {
	/// <summary>
	/// Retrieve a dictionary with the attribute data of all the attributes attached to a symbol. Because
	/// an attribute can appear more than once, the valus are a collection of attribute data.
	/// </summary>
	/// <param name="symbol">The symbol whose attributes we want to retrieve.</param>
	/// <returns>A dictionary with the attribute names as keys and the related attribute data.</returns>
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

	/// <summary>
	/// Return the symbol availability WITHOUT taking into account the parent symbols availability.
	/// </summary>
	/// <param name="symbol">The symbols whose availability attributes we want to retrieve.</param>
	/// <returns>The symbol availability WITHOUT taking into account the parent symbols.</returns>
	/// <remarks>This is a helper method, you probably don't want to use it.</remarks>
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
	/// Returns the symbol availability taking into account the parent symbols availability.
	///
	/// That means that the attributes used on the current symbol are merged with the attributes used
	/// in all the symbol parents following the correct child-parent order.
	/// </summary>
	/// <param name="symbol">The symbol whose availability we want to retrieve.</param>
	/// <returns>A symbol availability structure for the symbol.</returns>
	public static SymbolAvailability GetSupportedPlatforms (this ISymbol symbol)
	{
		var availability = GetAvailabilityForSymbol (symbol);
		// get the parents and return the merge
		foreach (var parent in GetParents (symbol)) {
			availability = availability.MergeWithParent (GetAvailabilityForSymbol (parent));
		}
		return availability;
	}

	/// <summary>
	/// Retrieve the data of an export attribute on a symbol.
	/// </summary>
	/// <param name="symbol">The tagged symbol.</param>
	/// <typeparam name="T">Enum type used in the attribute.</typeparam>
	/// <returns>The data of the export attribute if present or null if it was not found.</returns>
	/// <remarks>If the passed enum is unknown or not supproted as an enum for the export attribute, null will be
	/// returned.</remarks>
	public static ExportData<T>? GetExportData<T> (this ISymbol symbol) where T : Enum
	{
		var attributes = symbol.GetAttributeData ();
		if (attributes.Count == 0)
			return null;

		// retrieve the name of the attribute based on the flag
		var attrName = AttributesNames.GetFieldAttributeName<T> ();
		if (attrName is null)
			return null;
		if (!attributes.TryGetValue (attrName, out var exportAttrDataList) ||
			exportAttrDataList.Count != 1)
			return null;

		var exportAttrData = exportAttrDataList [0];
		var fieldSyntax = exportAttrData.ApplicationSyntaxReference?.GetSyntax ();
		if (fieldSyntax is null)
			return null;

		if (ExportData<T>.TryParse (exportAttrData, out var exportData))
			return exportData.Value;
		return null;
	}

}
