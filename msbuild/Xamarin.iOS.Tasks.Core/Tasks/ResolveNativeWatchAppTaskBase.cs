using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class ResolveNativeWatchAppTaskBase : Task
	{

		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SdkVersion { get; set; }

		[Required]
		public string TargetFrameworkIdentifier { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string NativeWatchApp { get; set; }

		#endregion

		bool IsWatchFramework {
			get {
				return TargetFrameworkIdentifier == "Xamarin.WatchOS";
			}
		}

		public override bool Execute ()
		{
			var currentSdk = IPhoneSdks.GetSdk (TargetFrameworkIdentifier);
			IPhoneSdkVersion version;
			string sdk_path;

			if (IsWatchFramework) {
				if (!IPhoneSdkVersion.TryParse (SdkVersion, out version)) {
					Log.LogError ("Failed to parse SdkVersion '{0}'.", SdkVersion);
					return false;
				}

				version = currentSdk.GetClosestInstalledSdk (version, false);
				sdk_path = currentSdk.GetSdkPath (version, SdkIsSimulator);
			} else {
				if (AppleSdkSettings.XcodeVersion.Major >= 10) {
					Log.LogError ("Xcode 10 does not support watchOS 1 apps. Either upgrade to watchOS 2 apps, or use an earlier version of Xcode.");
					return false;
				}
				if (!(AppleSdkSettings.XcodeVersion.Major > 6 || (AppleSdkSettings.XcodeVersion.Major == 6 && AppleSdkSettings.XcodeVersion.Minor >= 2))) {
					Log.LogError ("An installation of Xcode >= 6.2 is required to build WatchKit applications.");
					return false;
				}

				if (!IPhoneSdkVersion.TryParse (SdkVersion, out version)) {
					Log.LogError ("Failed to parse SdkVersion '{0}'.", SdkVersion);
					return false;
				}

				if (version < IPhoneSdkVersion.V8_2) {
					Log.LogError ("iOS {0} does not support WatchKit.", version);
					return false;
				}

				version = currentSdk.GetClosestInstalledSdk (version, false);
				sdk_path = currentSdk.GetSdkPath (version, SdkIsSimulator);
			}

			NativeWatchApp = Path.Combine (sdk_path, "Library", "Application Support", "WatchKit", "WK");
			if (File.Exists (NativeWatchApp))
				return true;

			Log.LogError ("Failed to locate the WatchKit launcher in the Xcode app bundle.");

			return false;
		}
	}
}
