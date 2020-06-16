using System;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {
	public abstract class GetMinimumOSVersionTaskBase : XamarinTask {
		public string AppManifest { get; set; }

		[Required]
		public string SdkVersion { get; set; }

		[Output]
		public string MinimumOSVersion { get; set; }

		public override bool Execute ()
		{
			PDictionary plist = null;

			if (!string.IsNullOrEmpty (AppManifest)) {
				try {
					plist = PDictionary.FromFile (AppManifest);
				} catch (Exception ex) {
					Log.LogError (null, null, null, AppManifest, 0, 0, 0, 0, MSBStrings.E0010, AppManifest, ex.Message);
					return false;
				}
			}

			var minimumOSVersionInManifest = plist?.Get<PString> (PlatformFrameworkHelper.GetMinimumOSVersionKey (Platform))?.Value;
			if (string.IsNullOrEmpty (minimumOSVersionInManifest)) {
				MinimumOSVersion = SdkVersion;
			} else if (!IAppleSdkVersion_Extensions.TryParse (minimumOSVersionInManifest, out var _)) {
				Log.LogError (null, null, null, AppManifest, 0, 0, 0, 0, MSBStrings.E0011, minimumOSVersionInManifest);
				return false;
			} else {
				MinimumOSVersion = minimumOSVersionInManifest;
			}

			return true;
		}
	}
}

