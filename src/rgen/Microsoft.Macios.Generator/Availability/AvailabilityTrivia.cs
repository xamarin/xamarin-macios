// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Availability;

readonly struct AvailabilityTrivia {

	/// <summary>
	/// Retrieve the trivia to add before the symbol declaration.
	/// </summary>
	public string? Start { get; }
	
	/// <summary>
	/// Retrieve the trivia to add after the symbol declaration.
	/// </summary>
	public string? End { get; }

	public AvailabilityTrivia (SymbolAvailability availability)
	{
		// trivia is calculated based on the availability of the symbol in each platform 
		// we will check each of the platforms and decide for the shorts #if possible
		var supportedPlatforms = new HashSet<string> ();
		var unsupportedPlatforms = new HashSet<string> ();
		foreach (var platformAvailability in availability.PlatformAvailabilities) {
			var platformDefine = platformAvailability.Platform.ToPlatformDefine ();
			if (platformDefine is null)
				continue;
			
			if (platformAvailability.IsSupported)
				supportedPlatforms.Add (platformDefine);
			else
				unsupportedPlatforms.Add (platformDefine);
		}
		
		// if all platforms are supported, we don't need any trivia
		if (unsupportedPlatforms.Count == 0) {
			// all platforms are supported
			Start = null;
			End = null;
			return;
		}

		if (supportedPlatforms.Count > unsupportedPlatforms.Count) {
			// we have more supported platforms than unsupported ones
			// we will use #if to exclude the unsupported platforms
			Start = $"#if !{string.Join (" && !", unsupportedPlatforms)}";
			End = "#endif";
		} else {
			// we have more unsupported platforms than supported ones
			// we will use #if to include the supported platforms
			Start = $"#if {string.Join (" || ", supportedPlatforms)}";
			End = "#endif";	
		} 
	}	
}
