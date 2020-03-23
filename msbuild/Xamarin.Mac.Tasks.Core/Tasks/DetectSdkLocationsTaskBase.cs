using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Localization.MSBuild;

namespace Xamarin.Mac.Tasks
{
	public class DetectSdkLocationsTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		// This is also an input
		[Output]
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
		public string SdkVersion {
			get; set;
		}

		[Output]
		public bool IsXcode8 {
			get; set;
		}

		#endregion Outputs

		public override bool Execute ()
		{
			if (EnsureAppleSdkRoot ())
				EnsureSdkPath ();
			EnsureXamarinSdkRoot ();

			IsXcode8 = AppleSdkSettings.XcodeVersion.Major >= 8;

			return !Log.HasLoggedErrors;
		}

		void EnsureSdkPath ()
		{
			var sdkVersion = MacOSXSdkVersion.GetDefault (MacOSXSdks.Native);
			if (!MacOSXSdks.Native.SdkIsInstalled (sdkVersion)) {
				Log.LogError (MSBStrings.E0083);
				return;
			}

			SdkVersion = sdkVersion.ToString ();

			SdkRoot = MacOSXSdks.Native.GetSdkPath (sdkVersion);
			if (string.IsNullOrEmpty (SdkRoot))
				Log.LogError (MSBStrings.E0084, SdkVersion, SdkRoot);

			SdkUsrPath = DirExists ("SDK usr directory", Path.Combine (MacOSXSdks.Native.DeveloperRoot, "usr"));
			if (string.IsNullOrEmpty (SdkUsrPath))
				Log.LogError (MSBStrings.E0085, SdkVersion, SdkRoot);

			SdkBinPath = DirExists ("SDK bin directory", Path.Combine (SdkUsrPath, "bin"));
			if (string.IsNullOrEmpty (SdkBinPath))
				Log.LogError (MSBStrings.E0032);
		}

		bool EnsureAppleSdkRoot ()
		{
			if (!MacOSXSdks.Native.IsInstalled) {
				string ideSdkPath;
				if (string.IsNullOrEmpty(SessionId))
					// SessionId is only and always defined on windows.
					// We can't check 'Environment.OSVersion.Platform' since the base tasks are always executed on the Mac.
					ideSdkPath = "(Projects > SDK Locations > Apple > Apple SDK)";
				else
					ideSdkPath = "(Tools > Options > Xamarin > iOS Settings > Apple SDK)";
				Log.LogError (MSBStrings.E0044, AppleSdkSettings.InvalidDeveloperRoot, ideSdkPath);
				return false;
			}
			Log.LogMessage(MessageImportance.Low, "DeveloperRoot: {0}", MacOSXSdks.Native.DeveloperRoot);
			Log.LogMessage(MessageImportance.Low, "GetPlatformPath: {0}", MacOSXSdks.Native.GetPlatformPath());

			SdkDevPath = MacOSXSdks.Native.DeveloperRoot;
			if (string.IsNullOrEmpty(SdkDevPath)) {
				Log.LogError(MSBStrings.E0086);
				return false;
			}
			return true;
		}

		void EnsureXamarinSdkRoot ()
		{
			if (string.IsNullOrEmpty (XamarinSdkRoot))
				XamarinSdkRoot = MacOSXSdks.XamMac.FrameworkDirectory;

			if (string.IsNullOrEmpty (XamarinSdkRoot) || !Directory.Exists (XamarinSdkRoot))
				Log.LogError (MSBStrings.E0087);
		}

		string DirExists (string checkingFor, params string[] paths)
		{
			try {
				if (paths.Any (p => string.IsNullOrEmpty (p)))
					return null;

				var path = Path.GetFullPath (Path.Combine (paths));
				Log.LogMessage (MessageImportance.Low, MSBStrings.M0047, checkingFor, path);
				return Directory.Exists (path) ? path : null;
			} catch {
				return null;
			}
		}
	}
}
