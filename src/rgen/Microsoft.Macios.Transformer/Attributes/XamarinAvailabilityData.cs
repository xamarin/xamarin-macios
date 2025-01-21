// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Attributes;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer.Attributes;

static class XamarinAvailabilityData {

	static readonly IReadOnlyDictionary<string, ApplePlatform> supportedAttributes = new Dictionary<string, ApplePlatform> {
		{ AttributesNames.iOSAttribute, ApplePlatform.iOS },
		{ AttributesNames.TVAttribute, ApplePlatform.TVOS },
		{ AttributesNames.MacAttribute, ApplePlatform.MacOSX },
		{ AttributesNames.MacCatalystAttribute, ApplePlatform.MacCatalyst },
	};

	static readonly IReadOnlyDictionary<string, ApplePlatform> unsupportedAttributes = new Dictionary<string, ApplePlatform> {
		{ AttributesNames.NoiOSAttribute, ApplePlatform.iOS },
		{ AttributesNames.NoTVAttribute, ApplePlatform.TVOS },
		{ AttributesNames.NoMacAttribute, ApplePlatform.MacOSX },
		{ AttributesNames.NoMacCatalystAttribute, ApplePlatform.MacCatalyst },
	};
	
	public static bool TryParseSupportedOSData (string attributeName, AttributeData attributeData,
		[NotNullWhen (true)] out SupportedOSPlatformData? data)
	{
		data = null;
		if (!supportedAttributes.TryGetValue (attributeName, out var platform)) {
			// not a supported attribute
			return false;
		}
		
		var count = attributeData.ConstructorArguments.Length;
		int major = 0;
		int? minor = null;
		int? subminor = null;
		
		// custom marshal directive values

		switch (count) {
		case 1:
			major = (byte) attributeData.ConstructorArguments [0].Value!;
			break;
		case 2:
			major = (byte) attributeData.ConstructorArguments [0].Value!; 
			minor = (byte) attributeData.ConstructorArguments [1].Value!;
			break;
		case 3:
			major = (byte) attributeData.ConstructorArguments [0].Value!; 
			minor = (byte) attributeData.ConstructorArguments [1].Value!;
			subminor = (byte) attributeData.ConstructorArguments [2].Value!;
			break;
		default:
			// 0 should not be an option..
			return false;
		}

		data = (major, minor, subminor) switch {
			(_, null, null) => new(platform, new($"{major}")),
			(_, not null, null) => new(platform, new(major, minor.Value)),
			(_,not null, not null) => new(platform, new(major, minor.Value, subminor.Value)),
			_ => throw new ArgumentOutOfRangeException ("Could not parse the version")
		};
		return true;
	}

	public static bool TryParseUnsupportedOSData (string attributeName, AttributeData attributeData,
		[NotNullWhen (true)] out UnsupportedOSPlatformData? data)
	{
		data = null;
		if (!unsupportedAttributes.TryGetValue (attributeName, out var platform)) {
			// not a supported attribute
			return false;
		}

		data = new UnsupportedOSPlatformData (platform);
		return true;
	}
}
