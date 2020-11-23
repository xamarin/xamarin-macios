using System;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
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
				case ApplePlatform.MacCatalyst:
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
				case ApplePlatform.MacCatalyst:
					return "MacCatalyst";
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

		protected async System.Threading.Tasks.Task<Execution> ExecuteAsync (string fileName, IList<string> arguments, string sdkDevPath, Dictionary<string, string> environment = null, bool mergeOutput = true, bool showErrorIfFailure = true)
		{
			// Create a new dictionary if we're given one, to make sure we don't change the caller's dictionary.
			var launchEnvironment = environment == null ? new Dictionary<string, string> () : new Dictionary<string, string> (environment);
			if (!string.IsNullOrEmpty (sdkDevPath))
				launchEnvironment ["DEVELOPER_DIR"] = sdkDevPath;

			Log.LogMessage (MessageImportance.Normal, MSBStrings.M0001, fileName, StringUtils.FormatArguments (arguments));
			var rv = await Execution.RunAsync (fileName, arguments, environment: launchEnvironment, mergeOutput: mergeOutput);
			Log.LogMessage (rv.ExitCode == 0 ? MessageImportance.Low : MessageImportance.High, MSBStrings.M0002, fileName, rv.ExitCode);

			// Show the output
			var output = rv.StandardOutput.ToString ();
			if (!mergeOutput) {
				if (output.Length > 0)
					output += Environment.NewLine;
				output += rv.StandardError.ToString ();
			}
			if (output.Length > 0)
				Log.LogMessage (rv.ExitCode == 0 ? MessageImportance.Low : MessageImportance.Normal, output);

			if (showErrorIfFailure && rv.ExitCode != 0)
				Log.LogError (MSBStrings.E0117, /* {0} exited with code {1} */ fileName == "xcrun" ? arguments [0] : fileName, rv.ExitCode);

			return rv;
		}
	}
}

