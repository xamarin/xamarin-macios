// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using Microsoft.Macios.Generator.Availability;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator.IO;

static class TabbedStringBuilderAvailability {

	public static TabbedWriter<StringWriter> AppendMemberAvailability (this TabbedWriter<StringWriter> self, in SymbolAvailability allPlatformsAvailability)
	{
		foreach (var availability in allPlatformsAvailability.PlatformAvailabilities) {
			var platformName = availability.Platform.AsString ().ToLower ();
			if (availability.SupportedVersion is not null) {
				var versionStr = (PlatformAvailability.IsDefaultVersion (availability.SupportedVersion))
					? string.Empty
					: availability.SupportedVersion.ToString ();
				self.WriteLine ($"[SupportedOSPlatform (\"{platformName}{versionStr}\")]");
			}

			// loop over the unsupported versions of the platform 
			foreach (var (version, message) in availability.UnsupportedVersions) {
				var versionStr = (PlatformAvailability.IsDefaultVersion (version)) ? string.Empty : version.ToString ();
				if (message is null) {
					self.WriteLine ($"[UnsupportedOSPlatform (\"{platformName}{versionStr}\")]");
				} else {
					self.WriteLine ($"[UnsupportedOSPlatform (\"{platformName}{versionStr}\", \"{message}\")]");
				}
			}

			// loop over the obsolete versions of the platform 
			foreach (var (version, obsoleteInfo) in availability.ObsoletedVersions) {
				var versionStr = (PlatformAvailability.IsDefaultVersion (version)) ? string.Empty : version.ToString ();

				switch (obsoleteInfo) {
				case (null, null):
					self.WriteLine ($"[ObsoletedOSPlatform (\"{platformName}{versionStr}\")]");
					break;
				case (not null, null):
					self.WriteLine ($"[ObsoletedOSPlatform (\"{platformName}{versionStr}\", \"{obsoleteInfo.Message}\")]");
					break;
				case (null, not null):
					self.WriteLine ($"[ObsoletedOSPlatform (\"{platformName}{versionStr}\", Url=\"{obsoleteInfo.Url}\")]");
					break;
				case (not null, not null):
					self.WriteLine (
						$"[ObsoletedOSPlatform (\"{platformName}{versionStr}\", \"{obsoleteInfo.Message}\", Url=\"{obsoleteInfo.Url}\")]");
					break;
				}
			}
		}

		return self;
	}
}
