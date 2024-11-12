using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Attributes;

readonly struct UnsupportedOSPlatformData {
	public ApplePlatform Platform { get; }
	public Version Version { get; }
	public string? Message { get; } = default;

	internal UnsupportedOSPlatformData (string platformName)
	{
		(Platform, Version) = platformName.GetPlatformAndVersion ();
	}

	internal UnsupportedOSPlatformData (string platformName, string? message = null) : this (platformName)
	{
		Message = message;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out UnsupportedOSPlatformData? data)
	{
		data = default;
		string platformName;
		string? message = null;

		var count = attributeData.ConstructorArguments.Length;
		switch (count) {
		case 1:
			platformName = (string) attributeData.ConstructorArguments [0].Value!;
			break;
		case 2:
			platformName = (string) attributeData.ConstructorArguments [0].Value!;
			message = (string) attributeData.ConstructorArguments [1].Value!;
			break;
		default:
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (platformName, message);
			return true;
		}

		// could be that the user used the named params because he did not know better
		foreach (var (name, value) in attributeData.NamedArguments) {
			switch (name) {
			case "Message":
				message = (string?) value.Value!;
				break;
			case "PlatformName":
				platformName = (string) value.Value!;
				break;
			case "TypeId": // ignore the type id
				break;
			default:
				data = null;
				return false;
			}
		}
		data = new (platformName, message);
		return true;
	}

}
