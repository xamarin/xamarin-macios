using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class CompileAppManifestTaskBase : Xamarin.MacDev.Tasks.CompileAppManifestTaskBase
	{
		[Required]
		public string DefaultSdkVersion { get; set; }

		[Required]
		public bool IsWatchApp { get; set; }

		[Required]
		public bool IsWatchExtension { get; set; }

		[Required]
		public string SdkPlatform { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string TargetFrameworkIdentifier { get; set; }

		[Required]
		public bool Debug { get; set; }

		public string DebugIPAddresses { get; set; }

		public string ResourceRules { get; set; }

		public PlatformFramework Framework {
			get { return PlatformFrameworkHelper.GetFramework (TargetFrameworkIdentifier); }
		}

		IPhoneDeviceType supportedDevices;
		IPhoneSdkVersion minimumOSVersion;
		IPhoneSdkVersion sdkVersion;

		bool IsIOS;

		public override bool Execute ()
		{
			PDictionary plist;

			Log.LogTaskName ("CompileAppManifest");
			Log.LogTaskProperty ("AppBundleName", AppBundleName);
			Log.LogTaskProperty ("AppBundleDir", AppBundleDir);
			Log.LogTaskProperty ("AppManifest", AppManifest);
			Log.LogTaskProperty ("Architecture", Architecture);
			Log.LogTaskProperty ("AssemblyName", AssemblyName);
			Log.LogTaskProperty ("BundleIdentifier", BundleIdentifier);
			Log.LogTaskProperty ("Debug", Debug);
			Log.LogTaskProperty ("DebugIPAddresses", DebugIPAddresses);
			Log.LogTaskProperty ("DefaultSdkVersion", DefaultSdkVersion);
			Log.LogTaskProperty ("IsAppExtension", IsAppExtension);
			Log.LogTaskProperty ("IsWatchApp", IsWatchApp);
			Log.LogTaskProperty ("IsWatchExtension", IsWatchExtension);
			Log.LogTaskProperty ("PartialAppManifests", PartialAppManifests);
			Log.LogTaskProperty ("ResourceRules", ResourceRules);
			Log.LogTaskProperty ("SdkPlatform", SdkPlatform);
			Log.LogTaskProperty ("SdkIsSimulator", SdkIsSimulator);
			Log.LogTaskProperty ("TargetFrameworkIdentifier", TargetFrameworkIdentifier);

			try {
				plist = PDictionary.FromFile (AppManifest);
			} catch (Exception ex) {
				LogAppManifestError ("Error loading '{0}': {1}", AppManifest, ex.Message);
				return false;
			}

			sdkVersion = IPhoneSdkVersion.Parse (DefaultSdkVersion);
			var text = plist.GetMinimumOSVersion ();
			if (string.IsNullOrEmpty (text)) {
				minimumOSVersion = sdkVersion;
			} else if (!IPhoneSdkVersion.TryParse (text, out minimumOSVersion)) {
				LogAppManifestError ("Could not parse MinimumOSVersion value '{0}'", text);
				return false;
			}

			switch (Framework) {
			case PlatformFramework.iOS:
				IsIOS = true;
				break;
			case PlatformFramework.WatchOS:
				break;
			case PlatformFramework.TVOS:
				break;
			default:
				throw new InvalidOperationException (string.Format ("Invalid framework: {0}", Framework));
			}

			return Compile (plist);
		}

		bool Compile (PDictionary plist)
		{
			var currentSDK = IPhoneSdks.GetSdk (Framework);

			if (!currentSDK.SdkIsInstalled (sdkVersion, SdkIsSimulator)) {
				Log.LogError (null, null, null, null, 0, 0, 0, 0, "The {0} SDK for '{1}' is not installed.", Framework, sdkVersion);
				return false;
			}

			supportedDevices = plist.GetUIDeviceFamily ();

			if (!IsWatchApp) {
				var version = IPhoneSdks.MonoTouch.ExtendedVersion;
				// This key is our supported way of determining if an app
				// was built with Xamarin, so it needs to be present in all apps.

				var dict = new PDictionary ();
				dict.Add ("Version", new PString (string.Format ("{0} ({1}: {2})", version.Version, version.Branch, version.Hash)));
				plist.Add ("com.xamarin.ios", dict);
			}

			var sdkSettings = currentSDK.GetSdkSettings (sdkVersion, SdkIsSimulator);
			var dtSettings = currentSDK.GetDTSettings ();

			SetValue (plist, ManifestKeys.BuildMachineOSBuild, dtSettings.BuildMachineOSBuild);
			plist.SetIfNotPresent (ManifestKeys.CFBundleDevelopmentRegion, "en");

			plist.SetIfNotPresent (ManifestKeys.CFBundleExecutable, AssemblyName);
			if (IsIOS) {
				var executable = plist.GetCFBundleExecutable ();
				if (executable.EndsWith (".app", StringComparison.Ordinal))
					LogAppManifestError ("The executable (CFBundleExecutable) name ({0}) cannot end with '.app', because iOS may fail to launch the app.", executable);
			}

			if (IsIOS) {
				if (minimumOSVersion < IPhoneSdkVersion.V5_0 && plist.GetUIMainStoryboardFile (true) != null)
					LogAppManifestError ("Applications using a storyboard as the Main Interface must have a deployment target greater than 5.0");

				if (!plist.ContainsKey (ManifestKeys.CFBundleName))
					plist [ManifestKeys.CFBundleName] = plist.ContainsKey (ManifestKeys.CFBundleDisplayName) ? plist.GetString (ManifestKeys.CFBundleDisplayName).Clone () : new PString (AppBundleName);
			} else {
				plist.SetIfNotPresent (ManifestKeys.CFBundleName, AppBundleName);
			}

			plist.SetIfNotPresent (ManifestKeys.CFBundleIdentifier, BundleIdentifier);
			plist.SetIfNotPresent (ManifestKeys.CFBundleInfoDictionaryVersion, "6.0");
			plist.SetIfNotPresent (ManifestKeys.CFBundlePackageType, IsAppExtension ? "XPC!" : "APPL");
			if (!string.IsNullOrEmpty (ResourceRules))
				plist.SetIfNotPresent (ManifestKeys.CFBundleResourceSpecification, Path.GetFileName (ResourceRules));
			plist.SetIfNotPresent (ManifestKeys.CFBundleSignature, "????");
			if (!plist.ContainsKey (ManifestKeys.CFBundleSupportedPlatforms))
				plist[ManifestKeys.CFBundleSupportedPlatforms] = new PArray { SdkPlatform };
			plist.SetIfNotPresent (ManifestKeys.CFBundleVersion, "1.0");
			plist.SetIfNotPresent (ManifestKeys.CFBundleShortVersionString, plist.GetCFBundleVersion ());

			if (!SdkIsSimulator) {
				SetValue (plist, "DTCompiler", sdkSettings.DTCompiler);
				SetValue (plist, "DTPlatformBuild", dtSettings.DTPlatformBuild);
				SetValue (plist, "DTSDKBuild", sdkSettings.DTSDKBuild);
			}

			plist.SetIfNotPresent ("DTPlatformName", SdkPlatform.ToLowerInvariant ());
			if (!SdkIsSimulator)
				SetValue (plist, "DTPlatformVersion", dtSettings.DTPlatformVersion);

			var sdkName = sdkSettings.CanonicalName;
			// older sdksettings didn't have a canonicalname for sim
			if (SdkIsSimulator && string.IsNullOrEmpty (sdkName)) {
				var deviceSdkSettings = currentSDK.GetSdkSettings (sdkVersion, false);
				sdkName = deviceSdkSettings.AlternateSDK;
			}
			SetValue (plist, "DTSDKName", sdkName);

			if (!SdkIsSimulator) {
				SetValue (plist, "DTXcode", AppleSdkSettings.DTXcode);
				SetValue (plist, "DTXcodeBuild", dtSettings.DTXcodeBuild);
			}

			SetDeviceFamily (plist);

			plist.SetIfNotPresent (ManifestKeys.MinimumOSVersion, minimumOSVersion.ToString ());

			if (IsWatchExtension) {
				// Note: Only watchOS1 Extensions target Xamarin.iOS
				if (Framework == PlatformFramework.iOS) {
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

			// Remove any Xamarin Studio specific keys
			plist.Remove (ManifestKeys.XSLaunchImageAssets);
			plist.Remove (ManifestKeys.XSAppIconAssets);

			// Merge with any partial plists generated by the Asset Catalog compiler...
			MergePartialPlistTemplates (plist);

			if (IsIOS)
				Validation (plist);

			CompiledAppManifest = new TaskItem (Path.Combine (AppBundleDir, "Info.plist"));
			plist.Save (CompiledAppManifest.ItemSpec, true, true);

			return !Log.HasLoggedErrors;
		}

		void SetDeviceFamily (PDictionary plist)
		{
			switch (Framework) {
			case PlatformFramework.iOS:
				SetIOSDeviceFamily (plist);
				break;
			case PlatformFramework.WatchOS:
				plist.SetUIDeviceFamily (IPhoneDeviceType.Watch);
				break;
			case PlatformFramework.TVOS:
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

				if (minimumOSVersion >= IPhoneSdkVersion.V3_2 && supportedDevices == IPhoneDeviceType.NotSet)
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
				Log.LogMessage (MessageImportance.Low, "All http loads are already allowed.");
			} else {
				Log.LogMessage (MessageImportance.Low, "Allowed arbitrary HTTP loads to support debugging.");
				ats.SetBooleanOrRemove (ManifestKeys.NSAllowsArbitraryLoads, true);
			}
		}

		void Validation (PDictionary plist)
		{
			var supportsIPhone = (supportedDevices & IPhoneDeviceType.IPhone) != 0
			                     || supportedDevices == IPhoneDeviceType.NotSet;
			var supportsIPad = (supportedDevices & IPhoneDeviceType.IPad) != 0;

			// Validation...
			if (!IsAppExtension && sdkVersion >= IPhoneSdkVersion.V3_2) {
				IPhoneOrientation orientation;

				if (supportsIPhone) {
					orientation = plist.GetUISupportedInterfaceOrientations (false);
					if (orientation == IPhoneOrientation.None) {
						LogAppManifestWarning ("Supported iPhone orientations have not been set");
					} else if (!orientation.IsValidPair ()) {
						LogAppManifestWarning ("Supported iPhone orientations are not matched pairs");
					}
				}

				if (supportsIPad) {
					orientation = plist.GetUISupportedInterfaceOrientations (true);
					if (orientation == IPhoneOrientation.None) {
						LogAppManifestWarning ("Supported iPad orientations have not been set");
					} else if (!orientation.IsValidPair ()) {
						LogAppManifestWarning ("Supported iPad orientations are not matched pairs");
					}
				}
			}
		}
	}
}
