using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Attributes;

readonly struct SupportedOSPlatformData {
	public ApplePlatform Platform { get; }
	public Version Version { get; }

	internal SupportedOSPlatformData (string platformName)
	{
		(Platform, Version) = platformName.GetPlatformAndVersion ();
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out SupportedOSPlatformData? data)
	{
		data = default;
		string platformName;

		var count = attributeData.ConstructorArguments.Length;
		switch (count) {
		case 1:
			platformName = (string) attributeData.ConstructorArguments [0].Value!;
			break;
		default:
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new(platformName);
			return true;
		}

		foreach (var (name, value) in attributeData.NamedArguments) {
			switch (name) {
			case "PlatformName":
				platformName = (string?) value.Value!;
				break;
			default:
				data = null;
				return false;
			}
		}

		data = new(platformName);
		return true;
	}
}
