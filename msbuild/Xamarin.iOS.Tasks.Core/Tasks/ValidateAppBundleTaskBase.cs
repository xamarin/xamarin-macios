using System;
using System.IO;

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

			var mainInfo = PDictionary.FromFile (mainInfoPath);

			var mainBundleIdentifier = mainInfo.GetCFBundleIdentifier ();
			if (string.IsNullOrEmpty (mainBundleIdentifier)) {
				Log.LogError ("{0} does not specify a CFBundleIdentifier.", mainInfoPath);
				return false;
			}

			var mainExecutable = mainInfo.GetCFBundleExecutable ();
			if (string.IsNullOrEmpty (mainExecutable))
				Log.LogError ("{0} does not specify a CFBundleExecutable", mainInfoPath);

			var mainBundleSupportedPlatforms = mainInfo.GetArray (ManifestKeys.CFBundleSupportedPlatforms);
			var mainPlatform = string.Empty;
			if (mainBundleSupportedPlatforms == null || mainBundleSupportedPlatforms.Count == 0) {
				Log.LogError ("{0} does not specify a CFBundleSupportedPlatforms.", mainInfoPath);
			} else {
				mainPlatform = (PString) mainBundleSupportedPlatforms [0];
			}

			// Validate UIDeviceFamily
			var deviceTypes = mainInfo.GetUIDeviceFamily ();
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
							Log.LogError ("{0} specifies an invalid UIDeviceFamily: {1}.", mainInfoPath, family);
						}
					}
				}
			}

			if (Directory.Exists (Path.Combine (AppBundlePath, "PlugIns"))) {
				var mainShortVersionString = mainInfo.GetCFBundleShortVersionString ();
				var mainVersion = mainInfo.GetCFBundleVersion ();

				foreach (var plugin in Directory.GetDirectories (Path.Combine (AppBundlePath, "PlugIns"))) {
					var extensionName = Path.GetFileNameWithoutExtension (plugin);
					var extensionInfoPath = Path.Combine (plugin, "Info.plist");
					if (!File.Exists (extensionInfoPath)) {
						Log.LogError ("The App Extension {0} does not contain an Info.plist", extensionName);
						continue;
					}

					var extensionInfo = PDictionary.FromFile (extensionInfoPath);

					var extensionBundleIdentifier = extensionInfo.GetCFBundleIdentifier ();
					if (string.IsNullOrEmpty (extensionBundleIdentifier)) {
						Log.LogError ("The App Extension {0} does not specify a CFBundleIdentifier.", extensionName);
						continue;
					}
					// The filename of the extension path is the extension's bundle identifier, which turns out ugly
					// in error messages. Try to get something more friendly-looking.
					extensionName = extensionInfo.GetCFBundleDisplayName () ?? extensionName;

					var extensionExecutable = extensionInfo.GetCFBundleExecutable ();
					if (string.IsNullOrEmpty (extensionExecutable))
						Log.LogError ("The App Extension {0} does not specify a CFBundleExecutable", extensionName);

					if (!extensionBundleIdentifier.StartsWith (mainBundleIdentifier + ".", StringComparison.Ordinal))
						Log.LogError ("The App Extension {0} has an invalid CFBundleIdentifier ({1}), it does not begin with the main app bundle's CFBundleIdentifier ({2}).", extensionName, extensionBundleIdentifier, mainBundleIdentifier);

					if (extensionBundleIdentifier.EndsWith (".key", StringComparison.Ordinal))
						Log.LogError ("The App Extension {0} has a CFBundleIdentifier ({1}) that ends with the illegal suffix \".key\".", extensionName, extensionBundleIdentifier);

					var extensionShortVersionString = extensionInfo.GetCFBundleShortVersionString ();
					if (string.IsNullOrEmpty (extensionShortVersionString))
						Log.LogWarning ("The App Extension {0} does not specify a CFBundleShortVersionString", extensionName);

					if (extensionShortVersionString != mainShortVersionString)
						Log.LogWarning("The App Extension {0} has a CFBundleShortVersionString ({1}) that does not match the main app bundle's CFBundleShortVersionString ({2})", extensionName, extensionShortVersionString, mainShortVersionString);

					var extensionVersion = extensionInfo.GetCFBundleVersion ();
					if (string.IsNullOrEmpty (extensionVersion))
						Log.LogWarning ("The App Extension {0} does not specify a CFBundleVersion", extensionName);

					if (extensionVersion != mainVersion)
						Log.LogWarning ("The App Extension {0} has a CFBundleVersion ({1}) that does not match the main app bundle's CFBundleVersion ({2})", extensionName, extensionVersion, mainVersion);

					var dictNSExtension = extensionInfo.Get<PDictionary> ("NSExtension");
					if (dictNSExtension == null) {
						Log.LogError ("The App Extension '{0}' has an invalid Info.plist: it does not contain an NSExtension dictionary.", extensionName);
					} else {
						var nsExtensionAttributes = dictNSExtension.Get<PDictionary> ("NSExtensionAttributes");
						var nsExtensionPointIdentifier = dictNSExtension.GetString ("NSExtensionPointIdentifier").Value;

						if (string.IsNullOrEmpty (nsExtensionPointIdentifier)) {
							Log.LogError ("The App Extension '{0}' has an invalid Info.plist: the NSExtension dictionary does not contain an NSExtensionPointIdentifier value.", extensionName);
						} else {
							// https://developer.apple.com/library/prerelease/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/SystemExtensionKeys.html#//apple_ref/doc/uid/TP40014212-SW9
							switch (nsExtensionPointIdentifier) {
							case "com.apple.ui-services": // iOS+OSX
							case "com.apple.services": // iOS
							case "com.apple.keyboard-service": // iOS
							case "com.apple.fileprovider-ui": // iOS
							case "com.apple.fileprovider-nonui": // iOS
							case "com.apple.FinderSync": // OSX
							case "com.apple.photo-editing": // iOS
							case "com.apple.share-services": // iOS+OSX
							case "com.apple.widget-extension": // iOS+OSX
								break;
							case "com.apple.watchkit": // iOS8.2
								if (nsExtensionAttributes == null) {
									Log.LogError ("The WatchKit Extension '{0}' has an invalid Info.plist: The NSExtension dictionary does not contain an NSExtensionAttributes dictionary.", extensionName);
								} else {
									var wkAppBundleIdentifier = nsExtensionAttributes.GetString ("WKAppBundleIdentifier").Value;
									var apps = Directory.GetDirectories (plugin, "*.app");
									if (apps.Length == 0) {
										Log.LogError ("The WatchKit Extension '{0}' does not contain any watch apps.", extensionName);
									} else if (apps.Length > 1) {
										Log.LogError ("The WatchKit Extension '{0}' contain more than one watch apps.", extensionName);
									} else {
										var watchName = Path.GetFileNameWithoutExtension (apps [0]);
										var watchInfoPath = Path.Combine (apps [0], "Info.plist");
										if (!File.Exists (watchInfoPath)) {
											Log.LogError ("The Watch App '{0}' does not contain an Info.plist", watchName);
											continue;
										}
										var watchInfo = PDictionary.FromFile (watchInfoPath);
										var watchBundleIdentifier = watchInfo.GetCFBundleIdentifier ();
										if (string.IsNullOrEmpty (watchBundleIdentifier)) {
											Log.LogError ("The Watch App '{0}' does not specify a CFBundleIdentifier.", watchName);
											continue;
										}

										var watchDeviceFamily = watchInfo.GetUIDeviceFamily ();
										IPhoneDeviceType expectedDeviceFamily;
										string expectedDeviceFamilyString;
										if (SdkIsSimulator) {
											expectedDeviceFamily = IPhoneDeviceType.Watch | IPhoneDeviceType.IPhone;
											expectedDeviceFamilyString = "IPhone, Watch (1, 4)";
										} else {
											expectedDeviceFamily = IPhoneDeviceType.Watch;
											expectedDeviceFamilyString = "Watch (4)";
										}
										if (watchDeviceFamily != expectedDeviceFamily)
											Log.LogError ("The Watch App '{0}' does not have a valid UIDeviceFamily value. Expected '{1}' found '{2} ({3})'.", watchName, expectedDeviceFamilyString, watchDeviceFamily.ToString (), (int) (watchDeviceFamily));

										var watchExecutable = watchInfo.GetCFBundleExecutable ();
										if (string.IsNullOrEmpty (watchExecutable))
											Log.LogError ("The Watch App '{0}' does not specify a CFBundleExecutable", watchName);
											
										if (watchBundleIdentifier != wkAppBundleIdentifier)
											Log.LogError ("The WatchKit Extension '{0}' has an invalid WKAppBundleIdentifier value ('{1}'), it does not match the Watch App's CFBundleIdentifier ('{2}').", extensionName, wkAppBundleIdentifier, watchBundleIdentifier);

										var wkCompanionAppBundleIdentifier = watchInfo.GetString ("WKCompanionAppBundleIdentifier").Value;
										if (wkCompanionAppBundleIdentifier != mainBundleIdentifier)
											Log.LogError ("The Watch App '{0}' has an invalid WKCompanionAppBundleIdentifier value ('{1}'), it does not match the main app bundle's CFBundleIdentifier ('{2}').", watchName, wkCompanionAppBundleIdentifier, mainBundleIdentifier);
											
										if (watchInfo.ContainsKey ("LSRequiresIPhoneOS"))
											Log.LogError ("The Watch App '{0}' has an invalid Info.plist: the LSRequiresIPhoneOS key must not be present.");

										var watchUIDeviceFamilies = watchInfo.GetArray ("UIDeviceFamily");
										if (watchUIDeviceFamilies != null) {
											var found = false;
											foreach (PNumber number in watchUIDeviceFamilies) {
												if (number != null && number.Value == 4) {
													found = true;
													break;
												}
											}
											if (!found)
												Log.LogError ("The Watch App '{0}' has an invalid Info.plist: the list of supported UIDeviceFamily values must include '4'.");
										}
									}
								}
								break;
							default:
								Log.LogWarning ("The App Extension '{0}' has an unrecognized NSExtensionPointIdentifier value ('{1}').", extensionName, nsExtensionPointIdentifier);
								break;
							}
						}
					}
				}
			}

			return !Log.HasLoggedErrors;
		}
	}
}
