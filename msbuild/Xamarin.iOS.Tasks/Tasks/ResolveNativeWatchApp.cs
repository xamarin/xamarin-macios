using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;
using Xamarin.Utils;
using Xamarin.Messaging.Build.Client;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.iOS.Tasks {
	public class ResolveNativeWatchApp : XamarinTask, ICancelableTask {
		#region Inputs

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SdkVersion { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string NativeWatchApp { get; set; }

		#endregion

		bool IsWatchFramework {
			get {
				return Platform == ApplePlatform.WatchOS;
			}
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var currentSdk = Sdks.GetSdk (TargetFrameworkMoniker);
			AppleSdkVersion version;
			string sdk_path;

			if (IsWatchFramework) {
				if (!AppleSdkVersion.TryParse (SdkVersion, out version)) {
					Log.LogError (MSBStrings.E0066, SdkVersion);
					return false;
				}

				version = currentSdk.GetClosestInstalledSdk (version, false);
				sdk_path = currentSdk.GetSdkPath (version, SdkIsSimulator);
			} else {
				if (AppleSdkSettings.XcodeVersion.Major >= 10) {
					Log.LogError (MSBStrings.E0067);
					return false;
				}
				if (!(AppleSdkSettings.XcodeVersion.Major > 6 || (AppleSdkSettings.XcodeVersion.Major == 6 && AppleSdkSettings.XcodeVersion.Minor >= 2))) {
					Log.LogError (MSBStrings.E0068);
					return false;
				}

				if (!AppleSdkVersion.TryParse (SdkVersion, out version)) {
					Log.LogError (MSBStrings.E0066, SdkVersion);
					return false;
				}

				if (version < AppleSdkVersion.V8_2) {
					Log.LogError (MSBStrings.E0069, version);
					return false;
				}

				version = currentSdk.GetClosestInstalledSdk (version, false);
				sdk_path = currentSdk.GetSdkPath (version, SdkIsSimulator);
			}

			NativeWatchApp = Path.Combine (sdk_path, "Library", "Application Support", "WatchKit", "WK");
			if (File.Exists (NativeWatchApp))
				return true;

			Log.LogError (MSBStrings.E0070);

			return false;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
