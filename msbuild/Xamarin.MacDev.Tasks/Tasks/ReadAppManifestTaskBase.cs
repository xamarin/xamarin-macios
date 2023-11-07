#nullable enable

using System;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public abstract class ReadAppManifestTaskBase : XamarinTask {
		public ITaskItem? AppManifest { get; set; }

		[Output]
		public string? CLKComplicationGroup { get; set; }

		[Output]
		public string? CFBundleExecutable { get; set; }

		[Output]
		public string? CFBundleDisplayName { get; set; }

		[Output]
		public string? CFBundleIdentifier { get; set; }

		[Output]
		public string? CFBundleVersion { get; set; }

		[Output]
		public string? MinimumOSVersion { get; set; }

		[Output]
		public string? NSExtensionPointIdentifier { get; set; }

		[Output]
		public string? UIDeviceFamily { get; set; }

		[Output]
		public bool WKWatchKitApp { get; set; }

		[Output]
		public string? XSAppIconAssets { get; set; }

		[Output]
		public string? XSLaunchImageAssets { get; set; }

		public override bool Execute ()
		{
			PDictionary? plist = null;

			if (!string.IsNullOrEmpty (AppManifest?.ItemSpec)) {
				try {
					plist = PDictionary.FromFile (AppManifest!.ItemSpec);
				} catch (Exception ex) {
					Log.LogError (null, null, null, AppManifest!.ItemSpec, 0, 0, 0, 0, MSBStrings.E0010, AppManifest.ItemSpec, ex.Message);
					return false;
				}
			}

			CFBundleExecutable = plist.GetCFBundleExecutable ();
			CFBundleDisplayName = plist?.GetCFBundleDisplayName ();
			CFBundleIdentifier = plist?.GetCFBundleIdentifier ();
			CFBundleVersion = plist?.GetCFBundleVersion ();
			CLKComplicationGroup = plist?.Get<PString> (ManifestKeys.CLKComplicationGroup)?.Value;

			MinimumOSVersion = plist?.Get<PString> (PlatformFrameworkHelper.GetMinimumOSVersionKey (Platform))?.Value;
			if (Platform == ApplePlatform.MacCatalyst) {
				// The minimum version in the Info.plist is the macOS version. However, the rest of our tooling
				// expects the iOS version, so expose that.
				if (!MacCatalystSupport.TryGetiOSVersion (Sdks.GetAppleSdk (Platform).GetSdkPath (), MinimumOSVersion!, out var convertedVersion, out var knownMacOSVersions))
					Log.LogError (MSBStrings.E0187, MinimumOSVersion, string.Join (", ", knownMacOSVersions));
				MinimumOSVersion = convertedVersion;
			}

			NSExtensionPointIdentifier = plist?.GetNSExtensionPointIdentifier ();
			UIDeviceFamily = plist?.GetUIDeviceFamily ().ToString ();
			WKWatchKitApp = plist?.GetWKWatchKitApp () == true;
			XSAppIconAssets = plist?.Get<PString> (ManifestKeys.XSAppIconAssets)?.Value;
			XSLaunchImageAssets = plist?.Get<PString> (ManifestKeys.XSLaunchImageAssets)?.Value;

			return !Log.HasLoggedErrors;
		}
	}
}
