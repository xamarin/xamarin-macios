using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

using Xamarin.Utils;

#nullable enable

namespace Xamarin.Utils {
	internal static class OSPlatformAttributeExtensions {
#if NET
		public static bool TryParse (string? platformName, [NotNullWhen (true)] out string? platform, out Version? version)
#else
		public static bool TryParse (string? platformName, out string? platform, out Version? version)
#endif
		{
			platform = null;
			version = null;

			if (string.IsNullOrEmpty (platformName))
				return false;

			var versionIndex = -1;
#if NET
			for (var i = 0; i < platformName.Length; i++) {
#else
			for (var i = 0; i < platformName!.Length; i++) {
#endif
				if (platformName [i] >= '0' && platformName [i] <= '9') {
					versionIndex = i;
					break;
				}
			}
			string supportedPlatform;
			string? supportedVersion = null;

			if (versionIndex == -1) {
				supportedPlatform = platformName;
			} else {
				supportedPlatform = platformName.Substring (0, versionIndex);
				supportedVersion = platformName.Substring (versionIndex);
			}

			platform = supportedPlatform.ToLowerInvariant ();

			if (string.IsNullOrEmpty (supportedVersion))
				return true;

			return Version.TryParse (supportedVersion, out version);
		}

#if NET
		public static bool TryParse (string? platformName, [NotNullWhen (true)] out ApplePlatform? platform, out Version? version)
#else
		public static bool TryParse (string? platformName, out ApplePlatform? platform, out Version? version)
#endif
		{
			platform = null;

			if (!TryParse (platformName, out string? supportedPlatform, out version))
				return false;

#if NET
			return TryGetApplePlatform (supportedPlatform, out platform);
#else
			return TryGetApplePlatform (supportedPlatform!, out platform);
#endif
		}

#if NET
		public static bool TryParse (this OSPlatformAttribute self, [NotNullWhen (true)] out string? platform, out Version? version)
		{
			if (self is null)
				throw new ArgumentNullException (nameof (self));

			return TryParse (self.PlatformName, out platform, out version);
		}

		public static bool TryParse (this OSPlatformAttribute self, [NotNullWhen (true)] out ApplePlatform? platform, out Version? version)
		{
			platform = null;

			if (!TryParse (self, out string? supportedPlatform, out version))
				return false;

			return TryGetApplePlatform (supportedPlatform, out platform);
		}
#endif

#if NET
		static bool TryGetApplePlatform (string supportedPlatform, [NotNullWhen (true)] out ApplePlatform? platform)
#else
		static bool TryGetApplePlatform (string supportedPlatform, out ApplePlatform? platform)
#endif
		{
			switch (supportedPlatform) {
			case "ios":
				platform = ApplePlatform.iOS;
				break;
			case "tvos":
				platform = ApplePlatform.TVOS;
				break;
			case "macos":
				platform = ApplePlatform.MacOSX;
				break;
			case "maccatalyst":
				platform = ApplePlatform.MacCatalyst;
				break;
			case "watchos":
				platform = ApplePlatform.WatchOS;
				break;
			default:
				platform = null;
				return false;
			}
			return true;
		}
	}
}
