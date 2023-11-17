using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Utils;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.iOS.Tasks {
	public abstract class ValidateAppBundleTaskBase : XamarinTask {
		#region Inputs

		[Required]
		public string AppBundlePath { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		#endregion

		void ValidateAppExtension (string path, string mainBundleIdentifier, string mainShortVersionString, string mainVersion)
		{
			var name = Path.GetFileNameWithoutExtension (path);
			var info = Path.Combine (path, "Info.plist");
			if (!File.Exists (info)) {
				Log.LogError (7003, path, MSBStrings.E7003, name);
				return;
			}

			var plist = PDictionary.FromFile (info);

			var bundleIdentifier = plist.GetCFBundleIdentifier ();
			if (string.IsNullOrEmpty (bundleIdentifier)) {
				Log.LogError (7004, info, MSBStrings.E7004, name);
				return;
			}

			// The filename of the extension path is the extension's bundle identifier, which turns out ugly
			// in error messages. Try to get something more friendly-looking.
			name = plist.GetCFBundleDisplayName () ?? name;

			var executable = plist.GetCFBundleExecutable ();
			if (string.IsNullOrEmpty (executable))
				Log.LogError (7005, info, MSBStrings.E7005, name);

			if (!bundleIdentifier.StartsWith (mainBundleIdentifier + ".", StringComparison.Ordinal))
				Log.LogError (7006, info, MSBStrings.E7006, name, bundleIdentifier, mainBundleIdentifier);

			if (bundleIdentifier.EndsWith (".key", StringComparison.Ordinal))
				Log.LogError (7007, info, MSBStrings.E7007, name, bundleIdentifier);

			var shortVersionString = plist.GetCFBundleShortVersionString ();
			if (string.IsNullOrEmpty (shortVersionString))
				Log.LogError (7008, info, MSBStrings.E7008, name);

			if (shortVersionString != mainShortVersionString)
				Log.LogWarning (MSBStrings.W0071, name, shortVersionString, mainShortVersionString);

			var version = plist.GetCFBundleVersion ();
			if (string.IsNullOrEmpty (version))
				Log.LogWarning (MSBStrings.W0072, name);

			if (version != mainVersion)
				Log.LogWarning (MSBStrings.W0073, name, version, mainVersion);

			var extension = plist.Get<PDictionary> ("NSExtension");
			if (extension is null) {
				Log.LogError (7009, info, MSBStrings.E7009, name);
				return;
			}

			var extensionPointIdentifier = extension.GetString ("NSExtensionPointIdentifier").Value;

			if (string.IsNullOrEmpty (extensionPointIdentifier)) {
				Log.LogError (7010, info, MSBStrings.E7010, name);
				return;
			}

			// https://developer.apple.com/library/prerelease/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/SystemExtensionKeys.html#//apple_ref/doc/uid/TP40014212-SW9
			switch (extensionPointIdentifier) {
			case "com.apple.ui-services": // iOS+OSX
			case "com.apple.services": // iOS
			case "com.apple.keyboard-service": // iOS
			case "com.apple.fileprovider-ui": // iOS
			case "com.apple.fileprovider-nonui": // iOS
			case "com.apple.FinderSync": // OSX
			case "com.apple.photo-editing": // iOS
			case "com.apple.share-services": // iOS+OSX
			case "com.apple.widget-extension": // iOS+OSX
			case "com.apple.Safari.content-blocker": // iOS
			case "com.apple.Safari.sharedlinks-service": // iOS
			case "com.apple.spotlight.index": // iOS
			case "com.apple.AudioUnit": // iOS
			case "com.apple.AudioUnit-UI": // iOS
			case "com.apple.tv-services": // tvOS
			case "com.apple.broadcast-services": // iOS+tvOS
			case "com.apple.callkit.call-directory": // iOS
			case "com.apple.message-payload-provider": // iOS
			case "com.apple.intents-service": // iOS
			case "com.apple.intents-ui-service": // iOS
			case "com.apple.usernotifications.content-extension": // iOS
			case "com.apple.usernotifications.service": // iOS
			case "com.apple.networkextension.packet-tunnel": // iOS+OSX
			case "com.apple.authentication-services-credential-provider-ui": // iOS
				break;
			case "com.apple.watchkit": // iOS8.2
				Log.LogError (7069, info, MSBStrings.E7069 /* Xamarin.iOS 14+ does not support watchOS 1 apps. Please migrate your project to watchOS 2+. */);
				break;
			default:
				Log.LogWarning (MSBStrings.W0074, name, extensionPointIdentifier);
				break;
			}
		}

		void ValidateWatchApp (string path, string mainBundleIdentifier, string mainShortVersionString, string mainVersion)
		{
			var name = Path.GetFileNameWithoutExtension (path);
			var info = Path.Combine (path, "Info.plist");
			var isSingleProject = false;
			if (!File.Exists (info)) {
				Log.LogError (7014, path, MSBStrings.E7014, name);
				return;
			}

			var plist = PDictionary.FromFile (info);
			var bundleIdentifier = plist.GetCFBundleIdentifier ();
			if (string.IsNullOrEmpty (bundleIdentifier)) {
				Log.LogError (7015, info, MSBStrings.E7015, name);
				return;
			}

			if (!bundleIdentifier.StartsWith (mainBundleIdentifier + ".", StringComparison.Ordinal))
				Log.LogError (7016, info, MSBStrings.E7016, name, bundleIdentifier, mainBundleIdentifier);

			var shortVersionString = plist.GetCFBundleShortVersionString ();
			if (string.IsNullOrEmpty (shortVersionString))
				Log.LogWarning (MSBStrings.W0075, name);

			if (shortVersionString != mainShortVersionString)
				Log.LogWarning (MSBStrings.W0076, name, shortVersionString, mainShortVersionString);

			var version = plist.GetCFBundleVersion ();
			if (string.IsNullOrEmpty (version))
				Log.LogWarning (MSBStrings.W0077, name);

			if (version != mainVersion)
				Log.LogWarning (MSBStrings.W0078, name, version, mainVersion);

			var watchDeviceFamily = plist.GetUIDeviceFamily ();
			if (watchDeviceFamily != IPhoneDeviceType.Watch)
				Log.LogError (7017, info, MSBStrings.E7017, name, watchDeviceFamily.ToString (), (int) watchDeviceFamily);

			var watchExecutable = plist.GetCFBundleExecutable ();
			if (string.IsNullOrEmpty (watchExecutable))
				Log.LogError (7018, info, MSBStrings.E7018, name);

			var wkCompanionAppBundleIdentifier = plist.GetString ("WKCompanionAppBundleIdentifier").Value;
			if (wkCompanionAppBundleIdentifier != mainBundleIdentifier)
				Log.LogError (7019, info, MSBStrings.E7019, name, wkCompanionAppBundleIdentifier, mainBundleIdentifier);

			PBoolean watchKitApp;
			if (plist.TryGetValue ("WKWatchKitApp", out watchKitApp)) {
				if (!watchKitApp.Value)
					Log.LogError (7020, info, MSBStrings.E7020, name);
			} else {
				isSingleProject = true;
			}

			if (plist.ContainsKey ("LSRequiresIPhoneOS"))
				Log.LogError (7021, info, MSBStrings.E7021, name);

			var pluginsDir = Path.Combine (path, "PlugIns");
			if (!Directory.Exists (pluginsDir) && !isSingleProject) {
				Log.LogError (7022, path, MSBStrings.E7022, name);
				return;
			}

			if (!isSingleProject) {
				int count = 0;
				foreach (var plugin in Directory.EnumerateDirectories (pluginsDir, "*.appex")) {
					ValidateWatchExtension (plugin, bundleIdentifier, shortVersionString, version);
					count++;
				}

				if (count == 0)
					Log.LogError (7022, pluginsDir, MSBStrings.E7022_A, name);
			}
		}

		void ValidateWatchExtension (string path, string watchAppBundleIdentifier, string mainShortVersionString, string mainVersion)
		{
			var name = Path.GetFileNameWithoutExtension (path);
			var info = Path.Combine (path, "Info.plist");
			if (!File.Exists (info)) {
				Log.LogError (7023, path, MSBStrings.E7023, name);
				return;
			}

			var plist = PDictionary.FromFile (info);

			var bundleIdentifier = plist.GetCFBundleIdentifier ();
			if (string.IsNullOrEmpty (bundleIdentifier)) {
				Log.LogError (7024, info, MSBStrings.E7024, name);
				return;
			}

			// The filename of the extension path is the extension's bundle identifier, which turns out ugly
			// in error messages. Try to get something more friendly-looking.
			name = plist.GetCFBundleDisplayName () ?? name;

			var executable = plist.GetCFBundleExecutable ();
			if (string.IsNullOrEmpty (executable))
				Log.LogError (7025, info, MSBStrings.E7025, name);

			if (!bundleIdentifier.StartsWith (watchAppBundleIdentifier + ".", StringComparison.Ordinal))
				Log.LogError (7026, info, MSBStrings.E7026, name, bundleIdentifier, watchAppBundleIdentifier);

			if (bundleIdentifier.EndsWith (".key", StringComparison.Ordinal))
				Log.LogError (7027, info, MSBStrings.E7027, name, bundleIdentifier);

			var shortVersionString = plist.GetCFBundleShortVersionString ();
			if (string.IsNullOrEmpty (shortVersionString))
				Log.LogWarning (MSBStrings.W0079, name);

			if (shortVersionString != mainShortVersionString)
				Log.LogWarning (MSBStrings.W0080, name, shortVersionString, mainShortVersionString);

			var version = plist.GetCFBundleVersion ();
			if (string.IsNullOrEmpty (version))
				Log.LogWarning (MSBStrings.W0081, name);

			if (version != mainVersion)
				Log.LogWarning (MSBStrings.W0082, name, version, mainVersion);

			var extension = plist.Get<PDictionary> ("NSExtension");
			if (extension is null) {
				Log.LogError (7028, info, MSBStrings.E7028, name);
				return;
			}

			var extensionPointIdentifier = extension.Get<PString> ("NSExtensionPointIdentifier");
			if (extensionPointIdentifier is not null) {
				if (extensionPointIdentifier.Value != "com.apple.watchkit")
					Log.LogError (7029, info, MSBStrings.E7029, name);
			} else {
				Log.LogError (7029, info, MSBStrings.E7029_A, name);
			}

			PDictionary attributes;
			if (!extension.TryGetValue ("NSExtensionAttributes", out attributes)) {
				Log.LogError (7030, info, MSBStrings.E7030, name);
				return;
			}

			var appBundleIdentifier = attributes.Get<PString> ("WKAppBundleIdentifier");
			if (appBundleIdentifier is not null) {
				if (appBundleIdentifier.Value != watchAppBundleIdentifier)
					Log.LogError (7031, info, MSBStrings.E7031, name, appBundleIdentifier.Value, watchAppBundleIdentifier);
			} else {
				Log.LogError (7031, info, MSBStrings.E7031_A, name);
			}

			PObject requiredDeviceCapabilities;

			if (plist.TryGetValue ("UIRequiredDeviceCapabilities", out requiredDeviceCapabilities)) {
				var requiredDeviceCapabilitiesDictionary = requiredDeviceCapabilities as PDictionary;
				var requiredDeviceCapabilitiesArray = requiredDeviceCapabilities as PArray;

				if (requiredDeviceCapabilitiesDictionary is not null) {
					PBoolean watchCompanion;

					if (requiredDeviceCapabilitiesDictionary.TryGetValue ("watch-companion", out watchCompanion))
						Log.LogError (7032, info, MSBStrings.E7032, name);
				} else if (requiredDeviceCapabilitiesArray is not null) {
					if (requiredDeviceCapabilitiesArray.OfType<PString> ().Any (x => x.Value == "watch-companion"))
						Log.LogError (7032, info, MSBStrings.E7032_A, name);
				}
			}
		}

		public override bool Execute ()
		{
			var mainInfoPath = PlatformFrameworkHelper.GetAppManifestPath (Platform, AppBundlePath);
			if (!File.Exists (mainInfoPath)) {
				Log.LogError (7040, AppBundlePath, MSBStrings.E7040, AppBundlePath);
				return false;
			}

			var plist = PDictionary.FromFile (mainInfoPath);

			var bundleIdentifier = plist.GetCFBundleIdentifier ();
			if (string.IsNullOrEmpty (bundleIdentifier)) {
				Log.LogError (7041, mainInfoPath, MSBStrings.E7041, mainInfoPath);
				return false;
			}

			var executable = plist.GetCFBundleExecutable ();
			if (string.IsNullOrEmpty (executable))
				Log.LogError (7042, mainInfoPath, MSBStrings.E7042, mainInfoPath);

			var supportedPlatforms = plist.GetArray (ManifestKeys.CFBundleSupportedPlatforms);
			var platform = string.Empty;
			if (supportedPlatforms is null || supportedPlatforms.Count == 0) {
				Log.LogError (7043, mainInfoPath, MSBStrings.E7043, mainInfoPath);
			} else {
				platform = (PString) supportedPlatforms [0];
			}

			// Validate UIDeviceFamily
			var deviceTypes = plist.GetUIDeviceFamily ();
			var deviceFamilies = deviceTypes.ToDeviceFamily ();
			AppleDeviceFamily [] validFamilies = null;
			AppleDeviceFamily [] requiredFamilies = null;

			switch (Platform) {
			case ApplePlatform.MacCatalyst:
				requiredFamilies = new AppleDeviceFamily [] {
					AppleDeviceFamily.IPad,
				};
				goto case ApplePlatform.iOS;
			case ApplePlatform.iOS:
				validFamilies = new AppleDeviceFamily [] {
					AppleDeviceFamily.IPhone,
					AppleDeviceFamily.IPad,
				};
				break;
			case ApplePlatform.WatchOS:
				validFamilies = new AppleDeviceFamily [] { AppleDeviceFamily.Watch };
				break;
			case ApplePlatform.TVOS:
				validFamilies = new AppleDeviceFamily [] { AppleDeviceFamily.TV };
				break;
			default:
				Log.LogError ("Invalid platform: {0}", Platform);
				break;
			}

			if (validFamilies is not null) {
				if (validFamilies.Length == 0) {
					Log.LogError (7044, mainInfoPath, MSBStrings.E7044, mainInfoPath);
				} else {
					foreach (var family in deviceFamilies) {
						if (Array.IndexOf (validFamilies, family) == -1) {
							Log.LogError (7044, mainInfoPath, MSBStrings.E7044_A, mainInfoPath, family);
						}
					}
				}
			}

			if (requiredFamilies is not null) {
				foreach (var family in requiredFamilies) {
					if (!deviceFamilies.Contains (family)) {
						Log.LogError (7044, mainInfoPath, MSBStrings.E7044_A, mainInfoPath, family);
					}
				}
			}

			var mainShortVersionString = plist.GetCFBundleShortVersionString ();
			var mainVersion = plist.GetCFBundleVersion ();

			if (Directory.Exists (Path.Combine (AppBundlePath, "PlugIns"))) {
				foreach (var plugin in Directory.GetDirectories (Path.Combine (AppBundlePath, "PlugIns"), "*.appex"))
					ValidateAppExtension (plugin, bundleIdentifier, mainShortVersionString, mainVersion);
			}

			if (Directory.Exists (Path.Combine (AppBundlePath, "Watch"))) {
				foreach (var watchApp in Directory.GetDirectories (Path.Combine (AppBundlePath, "Watch"), "*.app"))
					ValidateWatchApp (watchApp, bundleIdentifier, mainShortVersionString, mainVersion);
			}

			return !Log.HasLoggedErrors;
		}
	}
}
