using System;
using System.Diagnostics.CodeAnalysis;
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
}
