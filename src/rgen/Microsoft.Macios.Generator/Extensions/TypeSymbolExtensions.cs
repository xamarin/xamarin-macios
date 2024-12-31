using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
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

	/// <summary>
	/// Returns if a type is blittable or not.
	/// </summary>
	/// <param name="symbol"></param>
	/// <returns></returns>
	/// <seealso cref="https://learn.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types"/>
	public static bool IsBlittable (this ITypeSymbol symbol)
	{
		while (true) {
			// per the documentation, the following system types are bittable
			switch (symbol.SpecialType) {
			case SpecialType.System_Byte:
			case SpecialType.System_SByte:
			case SpecialType.System_Int16:
			case SpecialType.System_UInt16:
			case SpecialType.System_Int32:
			case SpecialType.System_UInt32:
			case SpecialType.System_Int64:
			case SpecialType.System_UInt64:
			case SpecialType.System_Single:
			case SpecialType.System_Double:
			case SpecialType.System_IntPtr:
			case SpecialType.System_UIntPtr:
				return true;
			case SpecialType.System_Array:
				// if we are dealing with an array, an array of bittable args is bittable
				symbol = symbol.ContainingType;
				continue;
			}

			if (symbol is IArrayTypeSymbol arrayTypeSymbol) {
				symbol = arrayTypeSymbol.ElementType;
				continue;
			}

			// if we are dealing with a structure, we have to check the layout type and all its childre
			if (symbol.TypeKind == TypeKind.Struct) {
				// Check for StructLayout attribute with LayoutKind.Sequential
				var layoutAttribute = symbol.GetAttributes ()
					.FirstOrDefault (attr => attr.AttributeClass?.ToString () == typeof (StructLayoutAttribute).FullName);

				if (layoutAttribute is not null) {
					var layoutKind = (LayoutKind) layoutAttribute.ConstructorArguments [0].Value!;
					if (layoutKind == LayoutKind.Auto) {
						return false;
					}
				} else {
					return false;
				}

				// Recursively check all fields of the struct
				foreach (var member in symbol.GetMembers ().OfType<IFieldSymbol> ()) {
					if (!member.Type.IsBlittable ()) {
						return false;
					}
				}

				return true;
			}

			// any other types are not blittable
			return false;
		}
	}
}
