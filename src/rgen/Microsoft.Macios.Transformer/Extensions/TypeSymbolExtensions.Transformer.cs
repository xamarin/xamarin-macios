// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Transformer;
using Microsoft.Macios.Transformer.Attributes;
using Microsoft.Macios.Transformer.Extensions;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Extensions;

static partial class TypeSymbolExtensions {

	/// <summary>
	/// List of supported platforms by the transformer.
	/// </summary>
	static readonly ImmutableArray<ApplePlatform> allSupportedPlatforms = [
		ApplePlatform.iOS,
		ApplePlatform.TVOS,
		ApplePlatform.MacOSX,
		ApplePlatform.MacCatalyst
	];

	/// <summary>
	/// Return the symbol availability WITHOUT taking into account the parent symbols availability.
	/// </summary>
	/// <param name="symbol">The symbols whose availability attributes we want to retrieve.</param>
	/// <returns>The symbol availability WITHOUT taking into account the parent symbols.</returns>
	/// <remarks>This is a helper method, you probably don't want to use it.</remarks>
	internal static SymbolAvailability GetAvailabilityForSymbol (this ISymbol symbol)
	{
		//get the attribute of the symbol and look for the Supported and Unsupported attributes and
		// add the different platforms to the result hashsets
		var builder = SymbolAvailability.CreateBuilder ();
		var boundAttributes = symbol.GetAttributes ();

		foreach (var attributeData in boundAttributes) {
			var attrName = attributeData.AttributeClass?.ToDisplayString ();
			if (string.IsNullOrEmpty (attrName))
				continue;

			if (XamarinAvailabilityData.TryParseSupportedOSData (attrName, attributeData, out var availabilityData)) {
				builder.Add (availabilityData.Value);
			}

			if (XamarinAvailabilityData.TryParseUnsupportedOSData (attrName, attributeData,
					out var unsupportedOsPlatformData)) {
				builder.Add (unsupportedOsPlatformData.Value);
			}
		}

		// if a platform was not ignore or had a specific version, then it is supported, loop over what we got
		// and add the missing platforms with the default version
		var supportedPlatforms = builder.PlatformAvailabilities.ToArray ()
			.Select (a => a.Platform);

		// add data to all not added platforms
		foreach (var platform in allSupportedPlatforms.Except (supportedPlatforms)) {
			builder.Add (new SupportedOSPlatformData (platform, new Version ()));
		}

		return builder.ToImmutable ();
	}

	/// <summary>
	/// Return if a symbol represents a smart enum in the old Xamarin bidings.
	/// </summary>
	/// <param name="symbol">The symbol under query.</param>
	/// <returns>True if the symbol represents a smart enum.</returns>
	public static bool IsSmartEnum (this ITypeSymbol symbol)
	{
		// smart enums in the classic bindings are a little more complicated to detect since we need
		// to find AT LEAST one enum field that contains the Field attribute.
		if (symbol.TypeKind != TypeKind.Enum)
			return false;

		foreach (var member in symbol.GetMembers ()) {
			if (member is not IFieldSymbol field || !field.IsConst)
				continue;

			// try to get the Field attribute from the current member, if we found it, then we have a smart enum
			var attributeData = field.GetAttributeData ();
			if (attributeData.HasFieldAttribute ())
				return true;
		}

		return false;
	}
	
	/// <summary>
	/// Returns the BaseTypeAttribute data that was used for a given symbol.
	/// </summary>
	/// <param name="symbol">The symbol under query.</param>
	/// <returns>The BaseTypeAttribute data if it was found, null otherwise.</returns>
	public static BaseTypeData? GetBaseTypeData (this ISymbol symbol)
		=> GetAttribute<BaseTypeData> (symbol, AttributesNames.BaseTypeAttribute, BaseTypeData.TryParse);
}
