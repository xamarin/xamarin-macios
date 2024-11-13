using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Attributes;

/// <summary>
/// Represents the data found in a ObsoletedOSPlatformAttribute.
/// </summary>
readonly struct ObsoletedOSPlatformData {
	/// <summary>
	/// Obsoleted platform.
	/// </summary>
	public ApplePlatform Platform { get; }
	
	/// <summary>
	/// Version in which the symbol was obsoleted. The default new Version () value will be
	/// used when the symbol has been obsoleted in all version.
	/// </summary>
	public Version Version { get; }
	
	/// <summary>
	/// Optional obsoleted message to report to the user.
	/// </summary>
	public string? Message { get; } = default;

	/// <summary>
	/// Optional url that points to the documentation.
	/// </summary>
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

	/// <summary>
	/// Try to parse the attribute data to retrieve the information of an ObsoletedOSPlatformAttribute.
	/// </summary>
	/// <param name="attributeData">The attribute data to be parsed.</param>
	/// <param name="data">The parsed data. Null if we could not parse the attribute data.</param>
	/// <returns>True if the data was parsed.</returns>
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
