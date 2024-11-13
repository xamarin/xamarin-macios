using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Attributes;

/// <summary>
/// Represents the data found in a SupportedOSPlatformAttribute.
/// </summary>
readonly struct SupportedOSPlatformData {
	/// <summary>
	/// Supported platform.
	/// </summary>
	public ApplePlatform Platform { get; }
	
	/// <summary>
	/// Version in which the symbol is supported. The default new Version () value will be
	/// used when the symbol has been obsoleted in all version.
	/// </summary>
	public Version Version { get; }

	internal SupportedOSPlatformData (string platformName)
	{
		(Platform, Version) = platformName.GetPlatformAndVersion ();
	}

	/// <summary>
	/// Try to parse the attribute data to retrieve the information of an SupportedOSPlatformAttribute.
	/// </summary>
	/// <param name="attributeData">The attribute data to be parsed.</param>
	/// <param name="data">The parsed data. Null if we could not parse the attribute data.</param>
	/// <returns>True if the data was parsed.</returns>
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
			data = new (platformName);
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

		data = new (platformName);
		return true;
	}
}
