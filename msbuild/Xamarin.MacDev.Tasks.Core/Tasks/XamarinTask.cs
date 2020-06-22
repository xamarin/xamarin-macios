using System;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	// This is the same as XamarinToolTask, except that it subclasses Task instead.
	public abstract class XamarinTask : Task {

		public string SessionId { get; set; }

		public string TargetFrameworkMoniker { get; set; }

		void VerifyTargetFrameworkMoniker ()
		{
			if (!string.IsNullOrEmpty (TargetFrameworkMoniker))
				return;
			Log.LogError ($"The task {GetType ().Name} requires TargetFrameworkMoniker to be set.");
		}

		public string Product {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return "Xamarin.iOS";
				case ApplePlatform.MacOSX:
					return "Xamarin.Mac";
				default:
					throw new InvalidOperationException ($"Invalid platform: {Platform}");
				}
			}
		}

		ApplePlatform? platform;
		public ApplePlatform Platform {
			get {
				if (!platform.HasValue) {
					VerifyTargetFrameworkMoniker ();
					platform = PlatformFrameworkHelper.GetFramework (TargetFrameworkMoniker);
				}
				return platform.Value;
			}
		}

		TargetFramework? target_framework;
		public TargetFramework TargetFramework {
			get {
				if (!target_framework.HasValue) {
					VerifyTargetFrameworkMoniker ();
					target_framework = TargetFramework.Parse (TargetFrameworkMoniker);
				}
				return target_framework.Value;
			}
		}

		public string PlatformName {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
					return "iOS";
				case ApplePlatform.TVOS:
					return "tvOS";
				case ApplePlatform.WatchOS:
					return "watchOS";
				case ApplePlatform.MacOSX:
					return "macOS";
				default:
					throw new InvalidOperationException ($"Invalid platform: {Platform}");
				}
			}
		}

		protected string GetSdkPlatform (bool isSimulator)
		{
			switch (Platform) {
			case ApplePlatform.iOS:
				return isSimulator ? "iPhoneSimulator" : "iPhoneOS";
			case ApplePlatform.TVOS:
				return isSimulator ? "AppleTVSimulator" : "AppleTVOS";
			case ApplePlatform.WatchOS:
				return isSimulator ? "WatchSimulator" : "WatchOS";
			case ApplePlatform.MacOSX:
				return "MacOSX";
			default:
				throw new InvalidOperationException ($"Invalid platform: {Platform}");
			}
		}

		protected async System.Threading.Tasks.Task<Execution> ExecuteAsync (string fileName, IList<string> arguments, string sdkDevPath)
		{
			var environment = new Dictionary<string, string> () {
				{  "DEVELOPER_DIR", sdkDevPath },
			};
			Log.LogMessage (MessageImportance.Low, $"Executing '{fileName} {StringUtils.FormatArguments (arguments)}'");
			var rv = await Execution.RunAsync ("xcrun", arguments, environment: environment, mergeOutput: true);
			if (rv.ExitCode != 0) {
				Log.LogMessage (MessageImportance.Normal, rv.StandardOutput.ToString ());
				Log.LogError ($"Executing '{fileName} {StringUtils.FormatArguments (arguments)}' failed with exit code {rv.ExitCode}. Please review build log for more information.");
			} else {
				Log.LogMessage (MessageImportance.Low, rv.StandardOutput.ToString ());
			}
			return rv;
		}
	}
}

