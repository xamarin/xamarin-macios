using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class DetectSdkLocationsTaskBase : Task
	{
#region Inputs

		public string SessionId { get; set; }

		// This is also an input
		[Output]
		public string SdkVersion {
			get; set;
		}

		[Required]
		public string TargetFrameworkIdentifier {
			get; set; 
		}
	
		public string TargetArchitectures {
			get; set;
		}

		public string XamarinSdkRoot {
			get; set;
		}

#endregion Inputs

#region Outputs

		[Output]
		public string SdkRoot {
			get; set;
		}

		[Output]
		public string SdkBinPath {
			get; set;
		}

		[Output]
		public string SdkDevPath {
			get; set;
		}

		[Output]
		public string SdkUsrPath {
			get; set;
		}

		[Output]
		public string SdkPlatform {
			get; set; 
		}

		[Output]
		public bool SdkIsSimulator {
			get; set;
		}

		[Output]
		public bool IsXcode8 {
			get; set;
		}

#endregion Outputs

		public PlatformFramework Framework {
			get { return PlatformFrameworkHelper.GetFramework (TargetFrameworkIdentifier); }
		}

		public AppleSdk CurrentSdk {
			get {
				return IPhoneSdks.GetSdk (Framework);
			}
		}

		public override bool Execute ()
		{
			AppleSdkSettings.Init ();
			IPhoneSdks.Reload ();

			TargetArchitecture architectures;
			if (string.IsNullOrEmpty (TargetArchitectures) || !Enum.TryParse (TargetArchitectures, out architectures))
				architectures = TargetArchitecture.Default;

			SdkIsSimulator = (architectures & (TargetArchitecture.i386 | TargetArchitecture.x86_64)) != 0;

			IsXcode8 = AppleSdkSettings.XcodeVersion.Major >= 8;

			if (EnsureAppleSdkRoot ()) {
				switch (Framework) {
				case PlatformFramework.iOS:
					EnsureiOSSdkPath ();
					break;
				case PlatformFramework.TVOS:
					EnsureTVOSSdkPath ();
					break;
				case PlatformFramework.WatchOS:
					EnsureWatchSdkPath ();
					break;
				default:
					throw new InvalidOperationException (string.Format ("Invalid framework: {0}", Framework));
				}
			}
			EnsureXamarinSdkRoot ();

			return !Log.HasLoggedErrors;
		}

		bool IsWatchFramework {
			get { return TargetFrameworkIdentifier == "Xamarin.WatchOS"; }
		}

		bool IsTVOSFramework {
			get { return TargetFrameworkIdentifier == "Xamarin.TVOS"; }
		}

		void EnsureTVOSSdkPath ()
		{
			var currentSdk = IPhoneSdks.GetSdk (Framework);
			IPhoneSdkVersion requestedSdkVersion;

			if (string.IsNullOrEmpty (SdkVersion)) {
				requestedSdkVersion = IPhoneSdkVersion.UseDefault;
			} else if (!IPhoneSdkVersion.TryParse (SdkVersion, out requestedSdkVersion)) {
				Log.LogError ("Could not parse the SDK version '{0}'", SdkVersion);
				return;
			}

			var sdkVersion = requestedSdkVersion.ResolveIfDefault (currentSdk, SdkIsSimulator);
			if (!currentSdk.SdkIsInstalled (sdkVersion, SdkIsSimulator)) {
				sdkVersion = currentSdk.GetClosestInstalledSdk (sdkVersion, SdkIsSimulator);

				if (sdkVersion.IsUseDefault || !currentSdk.SdkIsInstalled (sdkVersion, SdkIsSimulator)) {
					if (requestedSdkVersion.IsUseDefault) {
						Log.LogError ("The Apple TVOS SDK is not installed.");
					} else {
						Log.LogError ("The TVOS SDK version '{0}' is not installed, and no newer version was found.", requestedSdkVersion.ToString ());
					}
					return;
				}
				Log.LogWarning ("The TVOS SDK version '{0}' is not installed. Using newer version '{1}' instead'.", requestedSdkVersion, sdkVersion);
			}

			SdkVersion = sdkVersion.ToString ();

			var platformDir = currentSdk.GetPlatformPath (SdkIsSimulator);
			if (string.IsNullOrEmpty (platformDir) || !Directory.Exists (platformDir))
				Log.LogError ("Could not locate the TVOS platform directory at path '{0}'", platformDir);

			SdkRoot = currentSdk.GetSdkPath (sdkVersion, SdkIsSimulator);
			if (string.IsNullOrEmpty (SdkRoot) || !Directory.Exists (SdkRoot))
				Log.LogError ("Could not locate the TVOS '{0}' SDK at path '{1}'", SdkVersion, SdkRoot);

			SdkUsrPath = DirExists ("SDK Usr directory", Path.Combine (currentSdk.DeveloperRoot, "usr"));
			if (string.IsNullOrEmpty (SdkUsrPath))
				Log.LogError ("Could not locate the TVOS '{0}' SDK usr path at '{1}'", SdkVersion, SdkRoot);

			SdkBinPath = DirExists ("SDK bin directory", Path.Combine (SdkUsrPath, "bin"));
			if (string.IsNullOrEmpty (SdkBinPath))
				Log.LogError ("Could not locate SDK bin directory");

			SdkPlatform = SdkIsSimulator ? "AppleTVSimulator" : "AppleTVOS";
		}

		void EnsureWatchSdkPath ()
		{
			var currentSDK = IPhoneSdks.GetSdk (Framework);
			IPhoneSdkVersion requestedSdkVersion;

			if (string.IsNullOrEmpty (SdkVersion)) {
				requestedSdkVersion = IPhoneSdkVersion.UseDefault;
			} else if (!IPhoneSdkVersion.TryParse (SdkVersion, out requestedSdkVersion)) {
				Log.LogError ("Could not parse the SDK version '{0}'", SdkVersion);
				return;
			}

			var sdkVersion = requestedSdkVersion.ResolveIfDefault (currentSDK, SdkIsSimulator);
			if (!currentSDK.SdkIsInstalled (sdkVersion, SdkIsSimulator)) {
				sdkVersion = currentSDK.GetClosestInstalledSdk (sdkVersion, SdkIsSimulator);

				if (sdkVersion.IsUseDefault || !currentSDK.SdkIsInstalled (sdkVersion, SdkIsSimulator)) {
					if (requestedSdkVersion.IsUseDefault) {
						Log.LogError ("The Apple Watch SDK is not installed.");
					} else {
						Log.LogError ("The Watch SDK version '{0}' is not installed, and no newer version was found.", requestedSdkVersion.ToString ());
					}
					return;
				}
				Log.LogWarning ("The Watch SDK version '{0}' is not installed. Using newer version '{1}' instead'.", requestedSdkVersion, sdkVersion);
			}

			SdkVersion = sdkVersion.ToString ();

			var platformDir = currentSDK.GetPlatformPath (SdkIsSimulator);
			if (string.IsNullOrEmpty (platformDir) || !Directory.Exists (platformDir))
				Log.LogError ("Could not locate the WatchOS platform directory at path '{0}'", platformDir);

			SdkRoot = currentSDK.GetSdkPath (sdkVersion, SdkIsSimulator);
			if (string.IsNullOrEmpty (SdkRoot) || !Directory.Exists (SdkRoot))
				Log.LogError ("Could not locate the WatchOS '{0}' SDK at path '{1}'", SdkVersion, SdkRoot);
			
			SdkUsrPath = DirExists ("SDK Usr directory", Path.Combine (currentSDK.DeveloperRoot, "usr"));
			if (string.IsNullOrEmpty (SdkUsrPath))
				Log.LogError ("Could not locate the WatchOS '{0}' SDK usr path at '{1}'", SdkVersion, SdkRoot);
				
			SdkBinPath = DirExists ("SDK bin directory", Path.Combine (SdkUsrPath, "bin"));
			if (string.IsNullOrEmpty (SdkBinPath))
				Log.LogError ("Could not locate SDK bin directory");

			SdkPlatform = SdkIsSimulator ? "WatchSimulator" : "WatchOS";
		}

		void EnsureiOSSdkPath ()
		{
			var currentSDK = IPhoneSdks.GetSdk (Framework);
			IPhoneSdkVersion requestedSdkVersion;

			if (string.IsNullOrEmpty (SdkVersion)) {
				requestedSdkVersion = IPhoneSdkVersion.UseDefault;
			} else if (!IPhoneSdkVersion.TryParse (SdkVersion, out requestedSdkVersion)) {
				Log.LogError ("Could not parse the SDK version '{0}'", SdkVersion);
				return;
			}

			var sdkVersion = requestedSdkVersion.ResolveIfDefault (currentSDK, SdkIsSimulator);
			if (!currentSDK.SdkIsInstalled (sdkVersion, SdkIsSimulator)) {
				sdkVersion = currentSDK.GetClosestInstalledSdk (sdkVersion, SdkIsSimulator);

				if (sdkVersion.IsUseDefault || !currentSDK.SdkIsInstalled (sdkVersion, SdkIsSimulator)) {
					if (requestedSdkVersion.IsUseDefault) {
						Log.LogError ("The Apple iOS SDK is not installed.");
					} else {
						Log.LogError ("The iOS SDK version '{0}' is not installed, and no newer version was found.", requestedSdkVersion.ToString ());
					}
					return;
				}
				Log.LogWarning ("The iOS SDK version '{0}' is not installed. Using newer version '{1}' instead'.", requestedSdkVersion, sdkVersion);
			}

			SdkVersion = sdkVersion.ToString ();

			var platformDir = currentSDK.GetPlatformPath (SdkIsSimulator);
			if (string.IsNullOrEmpty (platformDir) || !Directory.Exists (platformDir))
				Log.LogError ("Could not locate the iOS platform directory at path '{0}'", platformDir);

			SdkRoot = currentSDK.GetSdkPath (sdkVersion, SdkIsSimulator);
			if (string.IsNullOrEmpty (SdkRoot) || !Directory.Exists (SdkRoot))
				Log.LogError ("Could not locate the iOS '{0}' SDK at path '{1}'", SdkVersion, SdkRoot);

			// Note: Developer/Platforms/iPhoneOS.platform/Developer/usr is a physical directory, but
			// Developer/Platforms/iPhoneSimulator.platform/Developer/bin has always been a symlink
			// to Developer/bin and starting with Xcode 7 Beta 2, the usr symlink no longer exists.
			SdkUsrPath = DirExists ("SDK Usr directory", Path.Combine (platformDir, "Developer", "usr"));
			if (string.IsNullOrEmpty (SdkUsrPath)) {
				SdkUsrPath = DirExists ("SDK Usr directory", Path.Combine (currentSDK.DeveloperRoot, "usr"));
				if (string.IsNullOrEmpty (SdkUsrPath))
					Log.LogError ("Could not locate the iOS '{0}' SDK usr path at '{1}'", SdkVersion, SdkRoot);
			}

			SdkBinPath = DirExists ("SDK bin directory", Path.Combine (SdkUsrPath, "bin"));
			if (string.IsNullOrEmpty (SdkBinPath))
				Log.LogError ("Could not locate SDK bin directory");

			SdkPlatform = SdkIsSimulator ? "iPhoneSimulator" : "iPhoneOS";
		}

		bool EnsureAppleSdkRoot ()
		{
			if (!CurrentSdk.IsInstalled) {
				string ideSdkPath;
				if (string.IsNullOrEmpty(SessionId))
					// SessionId is only and always defined on windows.
					// We can't check 'Environment.OSVersion.Platform' since the base tasks are always executed on the Mac.
					ideSdkPath = "(Project > SDK Locations > Apple > Apple SDK)";
				else
					ideSdkPath = "(Tools > Options > Xamarin > iOS Settings > Apple SDK)";
				Log.LogError ("Could not find a valid Xcode app bundle at '{0}'. Please update your Apple SDK location in Visual Studio's preferences {1}.", AppleSdkSettings.InvalidDeveloperRoot, ideSdkPath);
				return false;
			}
			Log.LogMessage (MessageImportance.Low, "DeveloperRoot: {0}", CurrentSdk.DeveloperRoot);
			Log.LogMessage (MessageImportance.Low, "DevicePlatform: {0}", CurrentSdk.DevicePlatform);
			Log.LogMessage (MessageImportance.Low, "GetPlatformPath: {0}", CurrentSdk.GetPlatformPath (false));

			SdkDevPath = CurrentSdk.DeveloperRoot;
			if (string.IsNullOrEmpty (SdkDevPath)) {
				Log.LogError ("Could not find valid a usable Xcode developer path");
				return false;
			}
			return true;
		}

		void EnsureXamarinSdkRoot ()
		{
			if (string.IsNullOrEmpty (XamarinSdkRoot))
				XamarinSdkRoot = IPhoneSdks.MonoTouch.SdkDir;

			if (string.IsNullOrEmpty (XamarinSdkRoot) || !Directory.Exists (XamarinSdkRoot))
				Log.LogError ("Could not find 'Xamarin.iOS'");
		}

		string DirExists (string checkingFor, string path)
		{
			try {
				if (path == null)
					return null;

				path = Path.GetFullPath (path);

				Log.LogMessage (MessageImportance.Low, "Searching for '{0}' in '{1}'", checkingFor, path);
				return Directory.Exists (path) ? path : null;
			} catch {
				return null;
			}
		}
	}
}
