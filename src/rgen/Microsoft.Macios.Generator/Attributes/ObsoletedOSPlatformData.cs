using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Attributes;

readonly struct ObsoletedOSPlatformData {
	public ApplePlatform Platform { get; }
	public Version Version { get; }
	public string? Message { get; } = default;

	public string? Url { get; }

	internal ObsoletedOSPlatformData (string platformName)
	{
		(Platform, Version) = platformName.GetPlatformAndVersion ();
	}

	internal ObsoletedOSPlatformData (string platformName, string? message = null, string? url = null) : this (platformName)
	{
		Message = message;
		Url = url;
	}

	public static bool TryParse (AttributeData attributeData,
		[NotNullWhen (true)] out ObsoletedOSPlatformData? data)
	{
		data = default;
		string platformName;
		string? message = null;
		string? url = null;

		var count = attributeData.ConstructorArguments.Length;
		switch (count) {
		case 1:
			platformName = (string) attributeData.ConstructorArguments [0].Value!;
			break;
		case 2:
			platformName = (string) attributeData.ConstructorArguments [0].Value!;
			message = (string) attributeData.ConstructorArguments [1].Value!;
			break;
		default: // there is not 3 args constructor with a url
			return false;
		}

		if (attributeData.NamedArguments.Length == 0) {
			data = new (platformName, message, url);
			return true;
		}

		foreach (var (name, value) in attributeData.NamedArguments) {
			switch (name) {
			case "Url":
				url = (string?) value.Value!;
				break;
			case "TypeId": // ignore the type id
				break;
			default:
				data = null;
				return false;
			}
		}

		data = new (platformName, message, url);
		return true;
	}
}
