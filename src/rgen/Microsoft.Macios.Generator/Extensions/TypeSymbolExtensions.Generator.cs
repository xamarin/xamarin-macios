// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;

namespace Microsoft.Macios.Generator.Extensions;

static partial class TypeSymbolExtensions {

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

	public static bool IsSmartEnum (this ITypeSymbol symbol)
	{
		// a type is a smart enum if its type is a enum one AND it was decorated with the
		// binding type attribute
		return symbol.TypeKind == TypeKind.Enum
			   && symbol.HasAttribute (AttributesNames.BindingAttribute);
	}

	public static BindingTypeData GetBindingData (this ISymbol symbol)
	{
		var boundAttributes = symbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			// no attrs in the symbol, therefore the symbol is supported in all platforms
			return default;
		}

		// we are looking for the basic BindingAttribute attr
		foreach (var attributeData in boundAttributes) {
			var attrName = attributeData.AttributeClass?.ToDisplayString ();
			if (string.IsNullOrEmpty (attrName) || attrName != AttributesNames.BindingAttribute)
				continue;
			if (BindingTypeData.TryParse (attributeData, out var bindingData)) {
				return bindingData.Value;
			}
		}

		return default;
	}

	public static BindingTypeData<T> GetBindingData<T> (this ISymbol symbol) where T : Enum
	{
		var boundAttributes = symbol.GetAttributes ();
		if (boundAttributes.Length == 0) {
			// no attrs in the symbol, therefore the symbol is supported in all platforms
			return default;
		}

		var targetAttrName = AttributesNames.GetBindingTypeAttributeName<T> ();
		foreach (var attributeData in boundAttributes) {
			var attrName = attributeData.AttributeClass?.ToDisplayString ();
			if (string.IsNullOrEmpty (attrName) || attrName != targetAttrName)
				continue;
			if (BindingTypeData<T>.TryParse (attributeData, out var bindingData)) {
				return bindingData.Value;
			}
		}

		return default;
	}

	/// <summary>
	/// Retrieve the data of an export attribute on a symbol.
	/// </summary>
	/// <param name="symbol">The tagged symbol.</param>
	/// <typeparam name="T">Enum type used in the attribute.</typeparam>
	/// <returns>The data of the export attribute if present or null if it was not found.</returns>
	/// <remarks>If the passed enum is unknown or not supported as an enum for the export attribute, null will be
	/// returned.</remarks>
	public static ExportData<T>? GetExportData<T> (this ISymbol symbol) where T : Enum
		=> GetAttribute<ExportData<T>> (symbol, AttributesNames.GetExportAttributeName<T>, ExportData<T>.TryParse);

	/// <summary>
	/// Retrieve the data of a field attribute on a symbol.
	/// </summary>
	/// <param name="symbol">The tagged symbol.</param>
	/// <typeparam name="T">Enum type used in the attribute.</typeparam>
	/// <returns>The data of the export attribute if present or null if it was not found.</returns>
	/// <remarks>If the passed enum is unknown or not supported as an enum for the field attribute, null will be
	/// returned.</remarks>
	public static FieldData<T>? GetFieldData<T> (this ISymbol symbol) where T : Enum
		=> GetAttribute<FieldData<T>> (symbol, AttributesNames.GetFieldAttributeName<T>, FieldData<T>.TryParse);
}
