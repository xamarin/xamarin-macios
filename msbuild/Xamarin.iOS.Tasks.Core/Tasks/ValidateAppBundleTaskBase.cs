using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class ValidateAppBundleTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string AppBundlePath { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string TargetFrameworkIdentifier { get; set; }

		#endregion

		public PlatformFramework Framework {
			get { return PlatformFrameworkHelper.GetFramework (TargetFrameworkIdentifier); }
		}

		void ValidateAppExtension (string path, string mainBundleIdentifier, string mainShortVersionString, string mainVersion)
		{
			var name = Path.GetFileNameWithoutExtension (path);
			var info = Path.Combine (path, "Info.plist");
			if (!File.Exists (info)) {
				Log.LogError ("The App Extension '{0}' does not contain an Info.plist", name);
				return;
			}

			var plist = PDictionary.FromFile (info);

			var bundleIdentifier = plist.GetCFBundleIdentifier ();
			if (string.IsNullOrEmpty (bundleIdentifier)) {
				Log.LogError ("The App Extension '{0}' does not specify a CFBundleIdentifier.", name);
				return;
			}

			// The filename of the extension path is the extension's bundle identifier, which turns out ugly
			// in error messages. Try to get something more friendly-looking.
			name = plist.GetCFBundleDisplayName () ?? name;

			var executable = plist.GetCFBundleExecutable ();
			if (string.IsNullOrEmpty (executable))
				Log.LogError ("The App Extension '{0}' does not specify a CFBundleExecutable", name);

			if (!bundleIdentifier.StartsWith (mainBundleIdentifier + ".", StringComparison.Ordinal))
				Log.LogError ("The App Extension '{0}' has an invalid CFBundleIdentifier ({1}), it does not begin with the main app bundle's CFBundleIdentifier ({2}).", name, bundleIdentifier, mainBundleIdentifier);

			if (bundleIdentifier.EndsWith (".key", StringComparison.Ordinal))
				Log.LogError ("The App Extension '{0}' has a CFBundleIdentifier ({1}) that ends with the illegal suffix \".key\".", name, bundleIdentifier);

			var shortVersionString = plist.GetCFBundleShortVersionString ();
			if (string.IsNullOrEmpty (shortVersionString))
				Log.LogWarning ("The App Extension '{0}' does not specify a CFBundleShortVersionString", name);

			if (shortVersionString != mainShortVersionString)
				Log.LogWarning ("The App Extension '{0}' has a CFBundleShortVersionString ({1}) that does not match the main app bundle's CFBundleShortVersionString ({2})", name, shortVersionString, mainShortVersionString);

			var version = plist.GetCFBundleVersion ();
			if (string.IsNullOrEmpty (version))
				Log.LogWarning ("The App Extension '{0}' does not specify a CFBundleVersion", name);

			if (version != mainVersion)
				Log.LogWarning ("The App Extension '{0}' has a CFBundleVersion ({1}) that does not match the main app bundle's CFBundleVersion ({2})", name, version, mainVersion);

			var extension = plist.Get<PDictionary> ("NSExtension");
			if (extension == null) {
				Log.LogError ("The App Extension '{0}' has an invalid Info.plist: it does not contain an NSExtension dictionary.", name);
				return;
			}

			var extensionPointIdentifier = extension.GetString ("NSExtensionPointIdentifier").Value;

			if (string.IsNullOrEmpty (extensionPointIdentifier)) {
				Log.LogError ("The App Extension '{0}' has an invalid Info.plist: the NSExtension dictionary does not contain an NSExtensionPointIdentifier value.", name);
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
			case "com.apple.AudioUnit-UI": // iOS
			case "com.apple.tv-services": // tvOS
			case "com.apple.broadcast-services": // iOS+tvOS
			case "com.apple.callkit.call-directory": // iOS
			case "com.apple.message-payload-provider": // iOS
			case "com.apple.intents-service": // iOS
			case "com.apple.intents-ui-service": // iOS
			case "com.apple.usernotifications.content-extension": // iOS
			case "com.apple.usernotifications.service": // iOS
				break;
			case "com.apple.watchkit": // iOS8.2
				var attributes = extension.Get<PDictionary> ("NSExtensionAttributes");

				if (attributes == null) {
					Log.LogError ("The WatchKit Extension '{0}' has an invalid Info.plist: The NSExtension dictionary does not contain an NSExtensionAttributes dictionary.", name);
					return;
				}

				var wkAppBundleIdentifier = attributes.GetString ("WKAppBundleIdentifier").Value;
				var apps = Directory.GetDirectories (path, "*.app");
				if (apps.Length == 0) {
					Log.LogError ("The WatchKit Extension '{0}' does not contain any watch apps.", name);
				} else if (apps.Length > 1) {
					Log.LogError ("The WatchKit Extension '{0}' contain more than one watch apps.", name);
				} else {
					PObject requiredDeviceCapabilities;

					if (plist.TryGetValue ("UIRequiredDeviceCapabilities", out requiredDeviceCapabilities)) {
						var requiredDeviceCapabilitiesDictionary = requiredDeviceCapabilities as PDictionary;
						var requiredDeviceCapabilitiesArray = requiredDeviceCapabilities as PArray;

						if (requiredDeviceCapabilitiesDictionary != null) {
							PBoolean watchCompanion;

							if (!requiredDeviceCapabilitiesDictionary.TryGetValue ("watch-companion", out watchCompanion) || !watchCompanion.Value)
								Log.LogError ("The WatchKit Extension '{0}' has an invalid Info.plist: the UIRequiredDeviceCapabilities dictionary must contain the 'watch-companion' capability with a value of 'true'.", name);
						} else if (requiredDeviceCapabilitiesArray != null) {
							if (!requiredDeviceCapabilitiesArray.OfType<PString> ().Any (x => x.Value == "watch-companion"))
								Log.LogError ("The WatchKit Extension '{0}' has an invalid Info.plist: the UIRequiredDeviceCapabilities array must contain the 'watch-companion' capability.", name);
						} else {
							Log.LogError ("The WatchKit Extension '{0}' has an invalid Info.plist: the UIRequiredDeviceCapabilities key must be present and contain the 'watch-companion' capability.", name);
						}
					} else {
						Log.LogError ("The WatchKit Extension '{0}' has an invalid Info.plist: the UIRequiredDeviceCapabilities key must be present and contain the 'watch-companion' capability.", name);
					}

					ValidateWatchOS1App (apps[0], name, mainBundleIdentifier, wkAppBundleIdentifier);
				}
				break;
			default:
				Log.LogWarning ("The App Extension '{0}' has an unrecognized NSExtensionPointIdentifier value ('{1}').", name, extensionPointIdentifier);
				break;
			}
		}

		void ValidateWatchApp (string path, string mainBundleIdentifier, string mainShortVersionString, string mainVersion)
		{
			var name = Path.GetFileNameWithoutExtension (path);
			var info = Path.Combine (path, "Info.plist");
			if (!File.Exists (info)) {
				Log.LogError ("The Watch App '{0}' does not contain an Info.plist", name);
				return;
			}

			var plist = PDictionary.FromFile (info);
			var bundleIdentifier = plist.GetCFBundleIdentifier ();
			if (string.IsNullOrEmpty (bundleIdentifier)) {
				Log.LogError ("The Watch App '{0}' does not specify a CFBundleIdentifier.", name);
				return;
			}

			if (!bundleIdentifier.StartsWith (mainBundleIdentifier + ".", StringComparison.Ordinal))
				Log.LogError ("The Watch App '{0}' has an invalid CFBundleIdentifier ({1}), it does not begin with the main app bundle's CFBundleIdentifier ({2}).", name, bundleIdentifier, mainBundleIdentifier);

			var shortVersionString = plist.GetCFBundleShortVersionString ();
			if (string.IsNullOrEmpty (shortVersionString))
				Log.LogWarning ("The Watch App '{0}' does not specify a CFBundleShortVersionString", name);

			if (shortVersionString != mainShortVersionString)
				Log.LogWarning ("The Watch App '{0}' has a CFBundleShortVersionString ({1}) that does not match the main app bundle's CFBundleShortVersionString ({2})", name, shortVersionString, mainShortVersionString);

			var version = plist.GetCFBundleVersion ();
			if (string.IsNullOrEmpty (version))
				Log.LogWarning ("The Watch App '{0}' does not specify a CFBundleVersion", name);

			if (version != mainVersion)
				Log.LogWarning ("The Watch App '{0}' has a CFBundleVersion ({1}) that does not match the main app bundle's CFBundleVersion ({2})", name, version, mainVersion);

			var watchDeviceFamily = plist.GetUIDeviceFamily ();
			if (watchDeviceFamily != IPhoneDeviceType.Watch)
				Log.LogError ("The Watch App '{0}' does not have a valid UIDeviceFamily value. Expected 'Watch (4)' but found '{1} ({2})'.", name, watchDeviceFamily.ToString (), (int) watchDeviceFamily);

			var watchExecutable = plist.GetCFBundleExecutable ();
			if (string.IsNullOrEmpty (watchExecutable))
				Log.LogError ("The Watch App '{0}' does not specify a CFBundleExecutable", name);

			var wkCompanionAppBundleIdentifier = plist.GetString ("WKCompanionAppBundleIdentifier").Value;
			if (wkCompanionAppBundleIdentifier != mainBundleIdentifier)
				Log.LogError ("The Watch App '{0}' has an invalid WKCompanionAppBundleIdentifier value ('{1}'), it does not match the main app bundle's CFBundleIdentifier ('{2}').", name, wkCompanionAppBundleIdentifier, mainBundleIdentifier);

			PBoolean watchKitApp;
			if (!plist.TryGetValue ("WKWatchKitApp", out watchKitApp) || !watchKitApp.Value)
				Log.LogError ("The Watch App '{0}' has an invalid Info.plist: The WKWatchKitApp key must be present and have a value of 'true'.", name);

			if (plist.ContainsKey ("LSRequiresIPhoneOS"))
				Log.LogError ("The Watch App '{0}' has an invalid Info.plist: the LSRequiresIPhoneOS key must not be present.", name);

			var pluginsDir = Path.Combine (path, "PlugIns");
			if (!Directory.Exists (pluginsDir)) {
				Log.LogError ("The Watch App '{0}' does not contain any Watch Extensions.", name);
				return;
			}

			int count = 0;
			foreach (var plugin in Directory.EnumerateDirectories (pluginsDir, "*.appex")) {
				ValidateWatchExtension (plugin, bundleIdentifier, shortVersionString, version);
				count++;
			}

			if (count == 0)
				Log.LogError ("The Watch App '{0}' does not contan a Watch Extension.", name);
		}

		void ValidateWatchExtension (string path, string watchAppBundleIdentifier, string mainShortVersionString, string mainVersion)
		{
			var name = Path.GetFileNameWithoutExtension (path);
			var info = Path.Combine (path, "Info.plist");
			if (!File.Exists (info)) {
				Log.LogError ("The Watch Extension '{0}' does not contain an Info.plist", name);
				return;
			}

			var plist = PDictionary.FromFile (info);

			var bundleIdentifier = plist.GetCFBundleIdentifier ();
			if (string.IsNullOrEmpty (bundleIdentifier)) {
				Log.LogError ("The Watch Extension '{0}' does not specify a CFBundleIdentifier.", name);
				return;
			}

			// The filename of the extension path is the extension's bundle identifier, which turns out ugly
			// in error messages. Try to get something more friendly-looking.
			name = plist.GetCFBundleDisplayName () ?? name;

			var executable = plist.GetCFBundleExecutable ();
			if (string.IsNullOrEmpty (executable))
				Log.LogError ("The Watch Extension '{0}' does not specify a CFBundleExecutable", name);

			if (!bundleIdentifier.StartsWith (watchAppBundleIdentifier + ".", StringComparison.Ordinal))
				Log.LogError ("The Watch Extension '{0}' has an invalid CFBundleIdentifier ({1}), it does not begin with the main app bundle's CFBundleIdentifier ({2}).", name, bundleIdentifier, watchAppBundleIdentifier);

			if (bundleIdentifier.EndsWith (".key", StringComparison.Ordinal))
				Log.LogError ("The Watch Extension '{0}' has a CFBundleIdentifier ({1}) that ends with the illegal suffix \".key\".", name, bundleIdentifier);

			var shortVersionString = plist.GetCFBundleShortVersionString ();
			if (string.IsNullOrEmpty (shortVersionString))
				Log.LogWarning ("The Watch Extension '{0}' does not specify a CFBundleShortVersionString", name);

			if (shortVersionString != mainShortVersionString)
				Log.LogWarning ("The Watch Extension '{0}' has a CFBundleShortVersionString ({1}) that does not match the main app bundle's CFBundleShortVersionString ({2})", name, shortVersionString, mainShortVersionString);

			var version = plist.GetCFBundleVersion ();
			if (string.IsNullOrEmpty (version))
				Log.LogWarning ("The Watch Extension '{0}' does not specify a CFBundleVersion", name);

			if (version != mainVersion)
				Log.LogWarning ("The Watch Extension '{0}' has a CFBundleVersion ({1}) that does not match the main app bundle's CFBundleVersion ({2})", name, version, mainVersion);

			var extension = plist.Get<PDictionary> ("NSExtension");
			if (extension == null) {
				Log.LogError ("The Watch Extension '{0}' has an invalid Info.plist: it does not contain an NSExtension dictionary.", name);
				return;
			}

			var extensionPointIdentifier = extension.Get<PString> ("NSExtensionPointIdentifier");
			if (extensionPointIdentifier != null) {
				if (extensionPointIdentifier.Value != "com.apple.watchkit")
					Log.LogError ("The Watch Extension '{0}' has an invalid Info.plist: the NSExtensionPointIdentifier must be \"com.apple.watchkit\".", name);
			} else {
				Log.LogError ("The Watch Extension '{0}' has an invalid Info.plist: the NSExtension dictionary must contain an NSExtensionPointIdentifier.", name);
			}

			PDictionary attributes;
			if (!extension.TryGetValue ("NSExtensionAttributes", out attributes)) {
				Log.LogError ("The Watch Extension '{0}' has an invalid Info.plist: the NSExtension dictionary must contain NSExtensionAttributes.", name);
				return;
			}

			var appBundleIdentifier = attributes.Get<PString> ("WKAppBundleIdentifier");
			if (appBundleIdentifier != null) {
				if (appBundleIdentifier.Value != watchAppBundleIdentifier)
					Log.LogError ("The Watch Extension '{0}' has an invalid WKAppBundleIdentifier value ('{1}'), it does not match the parent Watch App bundle's CFBundleIdentifier ('{2}').", name, appBundleIdentifier.Value, watchAppBundleIdentifier);
			} else {
				Log.LogError ("The Watch Extension '{0}' has an invalid Info.plist: the NSExtensionAttributes dictionary must contain a WKAppBundleIdentifier.", name);
			}
		}

		void ValidateWatchOS1App (string path, string extensionName, string mainBundleIdentifier, string wkAppBundleIdentifier)
		{
			var name = Path.GetFileNameWithoutExtension (path);
			var info = Path.Combine (path, "Info.plist");
			if (!File.Exists (info)) {
				Log.LogError ("The Watch App '{0}' does not contain an Info.plist", name);
				return;
			}

			var plist = PDictionary.FromFile (info);
			var bundleIdentifier = plist.GetCFBundleIdentifier ();
			if (string.IsNullOrEmpty (bundleIdentifier)) {
				Log.LogError ("The Watch App '{0}' does not specify a CFBundleIdentifier.", name);
				return;
			}

			var deviceFamily = plist.GetUIDeviceFamily ();
			IPhoneDeviceType expectedDeviceFamily;
			string expectedDeviceFamilyString;
			if (SdkIsSimulator) {
				expectedDeviceFamily = IPhoneDeviceType.Watch | IPhoneDeviceType.IPhone;
				expectedDeviceFamilyString = "IPhone, Watch (1, 4)";
			} else {
				expectedDeviceFamily = IPhoneDeviceType.Watch;
				expectedDeviceFamilyString = "Watch (4)";
			}

			if (deviceFamily != expectedDeviceFamily)
				Log.LogError ("The Watch App '{0}' does not have a valid UIDeviceFamily value. Expected '{1}' but found '{2} ({3})'.", name, expectedDeviceFamilyString, deviceFamily.ToString (), (int) deviceFamily);

			var executable = plist.GetCFBundleExecutable ();
			if (string.IsNullOrEmpty (executable))
				Log.LogError ("The Watch App '{0}' does not specify a CFBundleExecutable", name);

			if (bundleIdentifier != wkAppBundleIdentifier)
				Log.LogError ("The WatchKit Extension '{0}' has an invalid WKAppBundleIdentifier value ('{1}'), it does not match the Watch App's CFBundleIdentifier ('{2}').", extensionName, wkAppBundleIdentifier, bundleIdentifier);

			var companionAppBundleIdentifier = plist.Get<PString> ("WKCompanionAppBundleIdentifier");
			if (companionAppBundleIdentifier != null) {
				if (companionAppBundleIdentifier.Value != mainBundleIdentifier)
					Log.LogError ("The Watch App '{0}' has an invalid WKCompanionAppBundleIdentifier value ('{1}'), it does not match the main app bundle's CFBundleIdentifier ('{2}').", name, companionAppBundleIdentifier.Value, mainBundleIdentifier);
			} else {
				Log.LogError ("The Watch App '{0}' has an invalid Info.plist: the WKCompanionAppBundleIdentifier must exist and must match the main app bundle's CFBundleIdentifier.", name);
			}

			if (plist.ContainsKey ("LSRequiresIPhoneOS"))
				Log.LogError ("The Watch App '{0}' has an invalid Info.plist: the LSRequiresIPhoneOS key must not be present.", name);
		}

		public override bool Execute ()
		{
			Log.LogTaskName ("ValidateAppBundle");
			Log.LogTaskProperty ("AppBundlePath", Path.GetFullPath (AppBundlePath));
			Log.LogTaskProperty ("SdkIsSimulator", SdkIsSimulator);
			Log.LogTaskProperty ("TargetFrameworkIdentifier", TargetFrameworkIdentifier);

			var mainInfoPath = Path.Combine (AppBundlePath, "Info.plist");
			if (!File.Exists (mainInfoPath)) {
				Log.LogError ("The app bundle {0} does not contain an Info.plist.", AppBundlePath);
				return false;
			}

			var plist = PDictionary.FromFile (mainInfoPath);

			var bundleIdentifier = plist.GetCFBundleIdentifier ();
			if (string.IsNullOrEmpty (bundleIdentifier)) {
				Log.LogError ("{0} does not specify a CFBundleIdentifier.", mainInfoPath);
				return false;
			}

			var executable = plist.GetCFBundleExecutable ();
			if (string.IsNullOrEmpty (executable))
				Log.LogError ("{0} does not specify a CFBundleExecutable", mainInfoPath);

			var supportedPlatforms = plist.GetArray (ManifestKeys.CFBundleSupportedPlatforms);
			var platform = string.Empty;
			if (supportedPlatforms == null || supportedPlatforms.Count == 0) {
				Log.LogError ("{0} does not specify a CFBundleSupportedPlatforms.", mainInfoPath);
			} else {
				platform = (PString) supportedPlatforms[0];
			}

			// Validate UIDeviceFamily
			var deviceTypes = plist.GetUIDeviceFamily ();
			var deviceFamilies = deviceTypes.ToDeviceFamily ();
			AppleDeviceFamily[] validFamilies = null;

			switch (Framework) {
			case PlatformFramework.iOS:
				validFamilies = new AppleDeviceFamily[] {
					AppleDeviceFamily.IPhone,
					AppleDeviceFamily.IPad,
					AppleDeviceFamily.Watch
				};
				break;
			case PlatformFramework.WatchOS:
				validFamilies = new AppleDeviceFamily[] { AppleDeviceFamily.Watch };
				break;
			case PlatformFramework.TVOS:
				validFamilies = new AppleDeviceFamily[] { AppleDeviceFamily.TV };
				break;
			default:
				Log.LogError ("Invalid framework: {0}", Framework);
				break;
			}

			if (validFamilies != null) {
				if (validFamilies.Length == 0) {
					Log.LogError ("{0} does not specify a UIDeviceFamily.", mainInfoPath);
				} else {
					foreach (var family in deviceFamilies) {
						if (Array.IndexOf (validFamilies, family) == -1) {
							Log.LogError ("{0} is invalid: the UIDeviceFamily key must contain a value for '{1}'.", mainInfoPath, family);
						}
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
