using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.Mac.Tasks
{
	public class DetectSdkLocations : Task
	{
		#region Inputs

		// This is also an input
		[Output]
		public string SdkVersion {
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

		#endregion Outputs

		public override bool Execute ()
		{
			Log.LogTaskName ("DetectSdkLocations");
			Log.LogTaskProperty ("SdkVersion", SdkVersion);
			Log.LogTaskProperty ("XamarinSdkRoot", XamarinSdkRoot);

			EnsureAppleSdkRoot ();
			EnsureXamarinSdkRoot ();
			EnsureSdkPath ();

			return !Log.HasLoggedErrors;
		}

		void EnsureSdkPath ()
		{
			MacOSXSdkVersion requestedSdkVersion;
			if (string.IsNullOrEmpty (SdkVersion)) {
				requestedSdkVersion = MacOSXSdkVersion.UseDefault;
			} else if (!MacOSXSdkVersion.TryParse (SdkVersion, out requestedSdkVersion)) {
				Log.LogError ("Could not parse the SDK version '{0}'", SdkVersion);
				return;
			}

			var sdkVersion = requestedSdkVersion.ResolveIfDefault (MacOSXSdks.Native);
			if (!MacOSXSdks.Native.SdkIsInstalled (sdkVersion)) {
				sdkVersion = MacOSXSdks.Native.GetClosestInstalledSdk (sdkVersion);

				if (sdkVersion.IsUseDefault || !MacOSXSdks.Native.SdkIsInstalled (sdkVersion)) {
					if (requestedSdkVersion.IsUseDefault) {
						Log.LogError ("The Apple MacOSX SDK is not installed.");
					} else {
						Log.LogError ("The MacOSX SDK version '{0}' is not installed, and no newer version was found.", requestedSdkVersion.ToString ());
					}
					return;
				}
				Log.LogWarning ("The MacOSX SDK version '{0}' is not installed. Using newer version '{1}' instead'.", requestedSdkVersion, sdkVersion);
			}

			SdkVersion = sdkVersion.ToString ();

			SdkRoot = MacOSXSdks.Native.GetSdkPath (sdkVersion);
			if (string.IsNullOrEmpty (SdkRoot))
				Log.LogError ("Could not locate the MacOSX '{0}' SDK at path '{1}'", SdkVersion, SdkRoot);

			SdkUsrPath = DirExists ("SDK usr directory", Path.Combine (MacOSXSdks.Native.DeveloperRoot, "usr"));
			if (string.IsNullOrEmpty (SdkUsrPath))
				Log.LogError ("Could not locate the MacOSX '{0}' SDK usr path at '{1}'", SdkVersion, SdkRoot);

			SdkBinPath = DirExists ("SDK bin directory", Path.Combine (SdkUsrPath, "bin"));
			if (string.IsNullOrEmpty (SdkBinPath))
				Log.LogError ("Could not locate SDK bin directory");
		}

		void EnsureAppleSdkRoot ()
		{
			if (!MacOSXSdks.Native.IsInstalled) {
				Log.LogError ("  Could not find valid a usable Xcode app bundle");
			} else {
				Log.LogMessage (MessageImportance.Low, "  DeveloperRoot: {0}", MacOSXSdks.Native.DeveloperRoot);
				Log.LogMessage (MessageImportance.Low, "  GetPlatformPath: {0}", MacOSXSdks.Native.GetPlatformPath ());

				SdkDevPath = MacOSXSdks.Native.DeveloperRoot;
				if (string.IsNullOrEmpty (SdkDevPath))
					Log.LogError ("  Could not find valid a usable Xcode developer path");
			}
		}

		void EnsureXamarinSdkRoot ()
		{
			if (string.IsNullOrEmpty (XamarinSdkRoot))
				XamarinSdkRoot = MacOSXSdks.XamMac.FrameworkDirectory;

			if (string.IsNullOrEmpty (XamarinSdkRoot) || !Directory.Exists (XamarinSdkRoot))
				Log.LogError ("  Could not find 'Xamarin.Mac'");
		}

		string DirExists (string checkingFor, params string[] paths)
		{
			try {
				if (paths.Any (p => string.IsNullOrEmpty (p)))
					return null;

				var path = Path.GetFullPath (Path.Combine (paths));
				Log.LogMessage (MessageImportance.Low, "  Searching for '{0}' in '{1}'", checkingFor, path);
				return Directory.Exists (path) ? path : null;
			} catch {
				return null;
			}
		}
	}
}
