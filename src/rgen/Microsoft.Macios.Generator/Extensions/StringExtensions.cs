// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.Extensions;

static class StringExtensions {

	public static bool IsValidIdentifier ([NotNullWhen (true)] this string? self)
	{
		if (self is null)
			return false;
		var kind = SyntaxFacts.GetKeywordKind (self);
		return !SyntaxFacts.IsKeywordKind (kind) && SyntaxFacts.IsValidIdentifier (self);
	}

	public static (ApplePlatform Platform, Version Version) GetPlatformAndVersion (this string self)
	{
		var platform = self switch {
			_ when self.StartsWith ("ios") => ApplePlatform.iOS,
			_ when self.StartsWith ("tvos") => ApplePlatform.TVOS,
			_ when self.StartsWith ("maccatalyst") => ApplePlatform.MacCatalyst,
			_ when self.StartsWith ("macos") => ApplePlatform.MacOSX,
			_ => ApplePlatform.None
		};
		if (platform == ApplePlatform.None)
			return (platform, new Version ());
		return Version.TryParse (self [platform.AsString ().Length..], out var newVersion) ?
			(platform, newVersion) : (platform, new Version ());
	}

	public static string GetSelectorFieldName (this string self, bool inlineSelectors = false)
	{
		// calculate the name of the handle to use for the selector
		StringBuilder sb = new StringBuilder ();
		bool up = true;
		sb.Append ("sel");

		foreach (char c in self) {
			if (up && c != ':') {
				sb.Append (Char.ToUpper (c));
				up = false;
			} else if (c == ':') {
				// Selectors can differ only by colons.
				// Example 'mountDeveloperDiskImageWithError:' and 'mountDeveloperDiskImage:WithError:' (from Xamarin.Hosting)
				// This also showed up in a bug report: http://bugzilla.xamarin.com/show_bug.cgi?id=2626
				// So make sure we create a different name for those in C# as well, otherwise the generated code won't build.
				up = true;
				sb.Append ('_');
			} else
				sb.Append (c);
		}

		if (!inlineSelectors)
			sb.Append ("XHandle");
		return sb.ToString ();
	}
}
