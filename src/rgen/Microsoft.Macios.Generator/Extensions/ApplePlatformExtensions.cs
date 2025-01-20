// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Extensions;

public static class ApplePlatformExtensions {
	
	/// <summary>
	/// Return the platform define for the given ApplePlatform for use in #if directives.
	/// </summary>
	/// <param name="self">Apple platform.</param>
	/// <returns>the platform define</returns>
	public static string? ToPlatformDefine (this ApplePlatform self) => self switch {
		ApplePlatform.iOS => "IOS",
		ApplePlatform.TVOS => "TVOS",
		ApplePlatform.MacOSX => "MONOMAC",
		ApplePlatform.MacCatalyst => "__MACCATALYST__",
		_ => null
	};
}
