// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Macios.Generator.Availability;
using Xamarin.Utils;

namespace Microsoft.Macios.Generator;

partial class TabbedStringBuilder {

	public TabbedStringBuilder AppendMemberAvailability (in SymbolAvailability allPlatformsAvailability)
	{
		foreach (var availability in allPlatformsAvailability.PlatformAvailabilities) {
			var platformName = availability.Platform.AsString ().ToLower ();
			if (availability.SupportedVersion is not null) {
				var versionStr = (PlatformAvailability.IsDefaultVersion (availability.SupportedVersion))
					? string.Empty
					: availability.SupportedVersion.ToString ();
				AppendLine ($"[SupportedOSPlatform (\"{platformName}{versionStr}\")]");
			}

			// loop over the unsupported versions of the platform 
			foreach (var (version, message) in availability.UnsupportedVersions) {
				var versionStr = (PlatformAvailability.IsDefaultVersion (version)) ? string.Empty : version.ToString ();
				if (message is null) {
					AppendLine ($"[UnsupportedOSPlatform (\"{platformName}{versionStr}\")]");
				} else {
					AppendLine ($"[UnsupportedOSPlatform (\"{platformName}{versionStr}\", \"{message}\")]");
				}
			}

			// loop over the obsolete versions of the platform 
			foreach (var (version, obsoleteInfo) in availability.ObsoletedVersions) {
				var versionStr = (PlatformAvailability.IsDefaultVersion (version)) ? string.Empty : version.ToString ();

				switch (obsoleteInfo) {
				case (null, null):
					AppendLine ($"[ObsoletedOSPlatform (\"{platformName}{versionStr}\")]");
					break;
				case (not null, null):
					AppendLine ($"[ObsoletedOSPlatform (\"{platformName}{versionStr}\", \"{obsoleteInfo.Message}\")]");
					break;
				case (null, not null):
					AppendLine ($"[ObsoletedOSPlatform (\"{platformName}{versionStr}\", Url=\"{obsoleteInfo.Url}\")]");
					break;
				case (not null, not null):
					AppendLine (
						$"[ObsoletedOSPlatform (\"{platformName}{versionStr}\", \"{obsoleteInfo.Message}\", Url=\"{obsoleteInfo.Url}\")]");
					break;
				}
			}
		}

		return this;
	}
}
