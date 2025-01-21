// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Transformer.Attributes;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Extensions;

static partial class TypeSymbolExtensions {

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
		if (boundAttributes.Length == 0) {
			// no attrs in the symbol, therefore the symbol is supported in all platforms
			return builder.ToImmutable ();
		}

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
			builder.Add (new SupportedOSPlatformData(platform, new Version()));
		}

		return builder.ToImmutable ();
	}

	public static bool IsSmartEnum (this ITypeSymbol symbol)
	{
		throw new NotImplementedException ();
	}
}
