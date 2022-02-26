using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Build.Framework;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Utils;
using Xamarin.Localization.MSBuild;

namespace Xamarin.iOS.Tasks
{
	public abstract class CompileAppManifestTaskCore : CompileAppManifestTaskBase
	{
		IPhoneDeviceType supportedDevices;
		AppleSdkVersion sdkVersion;

		bool IsIOS { get { return Platform == ApplePlatform.iOS; } }

		protected override bool Compile (PDictionary plist)
		{
			var currentSDK = Sdks.GetAppleSdk (Platform);

			sdkVersion = AppleSdkVersion.Parse (DefaultSdkVersion);
			if (!currentSDK.SdkIsInstalled (sdkVersion, SdkIsSimulator)) {
				Log.LogError (null, null, null, null, 0, 0, 0, 0, MSBStrings.E0013, Platform, sdkVersion);
				return false;
			}

			supportedDevices = plist.GetUIDeviceFamily ();

			var sdkSettings = currentSDK.GetSdkSettings (sdkVersion, SdkIsSimulator);
			var dtSettings = currentSDK.GetAppleDTSettings ();

			SetValue (plist, ManifestKeys.BuildMachineOSBuild, dtSettings.BuildMachineOSBuild);
			// We have an issue here, this is for consideration by the platform:
			// CFLocaleCopyCurrent(), used in the mono code to get the current locale (locale.c line 421), will return the value of the application's CFBundleDevelopmentRegion Info.plist key if all of the following conditions are true:
			// 
			// * CFBundleDevelopmentRegion is present in the Info.plist
			// * The CFBundleDevelopmentRegion language is in the list of preferred languages on the iOS device, but isn't the first one
			// * There are no localized resources (i.e. no .lproj directory) in the app for the first preferred locale
			//
			// This differs from iOS 10 where the presence of the CFBundleDevelopmentRegion key had no effect. Commenting this line out, ensures that CurrentCulture is correct and behaves like the iOS 10 version.
			// plist.SetIfNotPresent (ManifestKeys.CFBundleDevelopmentRegion, "en");

			if (IsIOS) {
				var executable = plist.GetCFBundleExecutable ();
				if (executable.EndsWith (".app", StringComparison.Ordinal))
					LogAppManifestError (MSBStrings.E0014, executable);
			}

			if (!string.IsNullOrEmpty (ResourceRules))
				plist.SetIfNotPresent (ManifestKeys.CFBundleResourceSpecification, Path.GetFileName (ResourceRules));
			if (!plist.ContainsKey (ManifestKeys.CFBundleSupportedPlatforms))
				plist[ManifestKeys.CFBundleSupportedPlatforms] = new PArray { SdkPlatform };

			string dtCompiler = null;
			string dtPlatformBuild = null;
			string dtSDKBuild = null;
			string dtPlatformName = null;
			string dtPlatformVersion = null;
			string dtXcode = null;
			string dtXcodeBuild = null;

			if (!SdkIsSimulator) {
				dtCompiler = sdkSettings.DTCompiler;
				dtPlatformBuild = dtSettings.DTPlatformBuild;
				dtSDKBuild = sdkSettings.DTSDKBuild;
			}

			dtPlatformName = SdkPlatform.ToLowerInvariant ();
			if (!SdkIsSimulator)
				dtPlatformVersion = dtSettings.DTPlatformVersion;

			var dtSDKName = sdkSettings.CanonicalName;
			// older sdksettings didn't have a canonicalname for sim
			if (SdkIsSimulator && string.IsNullOrEmpty (dtSDKName)) {
				var deviceSdkSettings = currentSDK.GetSdkSettings (sdkVersion, false);
				dtSDKName = deviceSdkSettings.AlternateSDK;
			}

			if (!SdkIsSimulator) {
				dtXcode = AppleSdkSettings.DTXcode;
				dtXcodeBuild = dtSettings.DTXcodeBuild;
			}

			SetValueIfNotNull (plist, "DTCompiler", dtCompiler);
			SetValueIfNotNull (plist, "DTPlatformBuild", dtPlatformBuild);
			SetValueIfNotNull (plist, "DTSDKBuild", dtSDKBuild);
			plist.SetIfNotPresent ("DTPlatformName", dtPlatformName);
			SetValueIfNotNull (plist, "DTPlatformVersion", dtPlatformVersion);
			SetValue (plist, "DTSDKName", dtSDKName);
			SetValueIfNotNull (plist, "DTXcode", dtXcode);
			SetValueIfNotNull (plist, "DTXcodeBuild", dtXcodeBuild);

			SetDeviceFamily (plist);

			if (IsWatchExtension) {
				// Note: Only watchOS1 Extensions target Xamarin.iOS
				if (Platform == ApplePlatform.iOS) {
					PObject value;

					if (!plist.TryGetValue (ManifestKeys.UIRequiredDeviceCapabilities, out value)) {
						var capabilities = new PArray ();
						capabilities.Add (new PString ("watch-companion"));

						plist.Add (ManifestKeys.UIRequiredDeviceCapabilities, capabilities);
					} else if (value is PDictionary) {
						var capabilities = (PDictionary) value;

						if (!capabilities.ContainsKey ("watch-companion"))
							capabilities.Add ("watch-companion", new PBoolean (true));
					} else {
						var capabilities = (PArray) value;
						bool exists = false;

						foreach (var capability in capabilities.OfType<PString> ()) {
							if (capability.Value != "watch-companion")
								continue;

							exists = true;
							break;
						}

						if (!exists)
							capabilities.Add (new PString ("watch-companion"));
					}
				}

				if (Debug)
					SetAppTransportSecurity (plist);
			}

			SetRequiredArchitectures (plist);

			if (IsIOS)
				Validation (plist);

			return !Log.HasLoggedErrors;
		}

		void SetValueIfNotNull (PDictionary dict, string key, string value)
		{
			if (value == null)
				return;
			SetValue (dict, key, value);
		}

		void SetRequiredArchitectures (PDictionary plist)
		{
			PObject capabilities;

			if (plist.TryGetValue (ManifestKeys.UIRequiredDeviceCapabilities, out capabilities)) {
				if (capabilities is PArray) {
					var architectureValues = new HashSet<string> (new[] { "armv6", "armv7", "arm64" });
					var array = (PArray) capabilities;

					// Remove any architecture values
					for (int i = 0; i < array.Count; i++) {
						var value = array[i] as PString;

						if (value == null || !architectureValues.Contains (value.Value))
							continue;

						array.RemoveAt (i);
					}

					// If-and-only-if the TargetArchitecture is a single architecture, set it as a required device capability
					switch (architectures) {
					case TargetArchitecture.ARM64:
						array.Add (new PString ("arm64"));
						break;
					case TargetArchitecture.ARMv7:
						array.Add (new PString ("armv7"));
						break;
					}
				} else if (capabilities is PDictionary) {
					var dict = (PDictionary) capabilities;

					switch (architectures) {
					case TargetArchitecture.ARM64:
						dict["arm64"] = new PBoolean (true);
						dict.Remove ("armv6");
						dict.Remove ("armv7");
						break;
					case TargetArchitecture.ARMv7:
						dict["armv7"] = new PBoolean (true);
						dict.Remove ("armv6");
						dict.Remove ("arm64");
						break;
					default:
						dict.Remove ("armv6");
						dict.Remove ("armv7");
						dict.Remove ("arm64");
						break;
					}
				}
			} else {
				var array = new PArray ();

				// If-and-only-if the TargetArchitecture is a single architecture, set it as a required device capability
				switch (architectures) {
				case TargetArchitecture.ARM64:
					array.Add (new PString ("arm64"));
					break;
				case TargetArchitecture.ARMv7:
					array.Add (new PString ("armv7"));
					break;
				}

				if (array.Count > 0)
					plist.Add (ManifestKeys.UIRequiredDeviceCapabilities, array);
			}
		}

		void SetDeviceFamily (PDictionary plist)
		{
			switch (Platform) {
			case ApplePlatform.iOS:
				SetIOSDeviceFamily (plist);
				break;
			case ApplePlatform.WatchOS:
				plist.SetUIDeviceFamily (IPhoneDeviceType.Watch);
				break;
			case ApplePlatform.TVOS:
				plist.SetUIDeviceFamily (IPhoneDeviceType.TV);
				break;
			}
		}

		void SetIOSDeviceFamily (PDictionary plist)
		{
			if (IsWatchApp) {
				if (SdkIsSimulator) {
					plist.SetUIDeviceFamily (IPhoneDeviceType.IPhone | IPhoneDeviceType.Watch);
				} else {
					plist.SetUIDeviceFamily (IPhoneDeviceType.Watch);
				}
			} else {
				if (!IsAppExtension)
					plist.SetIfNotPresent (ManifestKeys.LSRequiresIPhoneOS, true);

				if (supportedDevices == IPhoneDeviceType.NotSet)
					plist.SetUIDeviceFamily (IPhoneDeviceType.IPhone);
			}
		}

		void SetAppTransportSecurity (PDictionary plist)
		{
			// Debugging over http has a couple of gotchas:
			// * We can't use https, because that requires a valid server certificate,
			//   which we can't ensure.
			//   It would also require a hostname for the mac, which it might not have either.
			// * NSAppTransportSecurity/NSExceptionDomains does not allow exceptions based
			//   on IP address (only hostname).
			// * Which means the only way to make sure watchOS allows connections from 
			//   the app on device to the mac is to disable App Transport Security altogether.
			// Good news: watchOS 3 will apparently not apply ATS when connecting
			// directly to IP addresses, which means we won't have to do this at all
			// (sometime in the future).

			PDictionary ats;

			if (!plist.TryGetValue (ManifestKeys.NSAppTransportSecurity, out ats))
				plist.Add (ManifestKeys.NSAppTransportSecurity, ats = new PDictionary ());

			if (ats.GetBoolean (ManifestKeys.NSAllowsArbitraryLoads)) {
				Log.LogMessage (MessageImportance.Low, MSBStrings.M0017);
			} else {
				Log.LogMessage (MessageImportance.Low, MSBStrings.M0018);
				ats.SetBooleanOrRemove (ManifestKeys.NSAllowsArbitraryLoads, true);
			}
		}

		void Validation (PDictionary plist)
		{
			if (!Validate)
				return;

			var supportsIPhone = (supportedDevices & IPhoneDeviceType.IPhone) != 0
			                     || supportedDevices == IPhoneDeviceType.NotSet;
			var supportsIPad = (supportedDevices & IPhoneDeviceType.IPad) != 0;

			// Validation...
			if (!IsAppExtension && sdkVersion >= AppleSdkVersion.V3_2) {
				IPhoneOrientation orientation;

				if (supportsIPhone) {
					orientation = plist.GetUISupportedInterfaceOrientations (false);
					if (orientation == IPhoneOrientation.None) {
						LogAppManifestWarning (MSBStrings.W0019);
					} else if (!orientation.IsValidPair ()) {
						LogAppManifestWarning (MSBStrings.W0020);
					}
				}

				if (supportsIPad) {
					orientation = plist.GetUISupportedInterfaceOrientations (true);
					if (orientation == IPhoneOrientation.None) {
						LogAppManifestWarning (MSBStrings.W0021);
					} else if (!orientation.IsValidPair ()) {
						LogAppManifestWarning (MSBStrings.W0022);
					}
				}
			}
		}
	}
}
