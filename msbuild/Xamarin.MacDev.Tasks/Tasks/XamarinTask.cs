using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
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
				if (IsDotNet)
					return "Microsoft." + PlatformName;

				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
				case ApplePlatform.MacCatalyst:
					return "Xamarin.iOS";
				case ApplePlatform.MacOSX:
					return "Xamarin.Mac";
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
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

		public bool IsDotNet {
			get { return TargetFramework.IsDotNet; }
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
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		public string DotNetVersion {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
					return DotNetVersionConstants.Microsoft_iOS_Version;
				case ApplePlatform.MacCatalyst:
					return DotNetVersionConstants.Microsoft_MacCatalyst_Version;
				case ApplePlatform.MacOSX:
					return DotNetVersionConstants.Microsoft_macOS_Version;
				case ApplePlatform.TVOS:
					return DotNetVersionConstants.Microsoft_tvOS_Version;
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		protected string GetSdkPlatform (bool isSimulator)
		{
			return PlatformFrameworkHelper.GetSdkPlatform (Platform, isSimulator);
		}

		protected async System.Threading.Tasks.Task<Execution> ExecuteAsync (string fileName, IList<string> arguments, string sdkDevPath = null, Dictionary<string, string> environment = null, bool mergeOutput = true, bool showErrorIfFailure = true, string workingDirectory = null)
		{
			// Create a new dictionary if we're given one, to make sure we don't change the caller's dictionary.
			var launchEnvironment = environment == null ? new Dictionary<string, string> () : new Dictionary<string, string> (environment);
			if (!string.IsNullOrEmpty (sdkDevPath))
				launchEnvironment ["DEVELOPER_DIR"] = sdkDevPath;

			Log.LogMessage (MessageImportance.Normal, MSBStrings.M0001, fileName, StringUtils.FormatArguments (arguments));
			var rv = await Execution.RunAsync (fileName, arguments, environment: launchEnvironment, mergeOutput: mergeOutput, workingDirectory: workingDirectory);
			Log.LogMessage (rv.ExitCode == 0 ? MessageImportance.Low : MessageImportance.High, MSBStrings.M0002, fileName, rv.ExitCode);

			// Show the output
			var output = rv.StandardOutput.ToString ();
			if (!mergeOutput) {
				if (output.Length > 0)
					output += Environment.NewLine;
				output += rv.StandardError.ToString ();
			}
			if (output.Length > 0) {
				var importance = MessageImportance.Low;
				if (rv.ExitCode != 0)
					importance = showErrorIfFailure ? MessageImportance.High : MessageImportance.Normal;
				Log.LogMessage (importance, output);
			}

			if (showErrorIfFailure && rv.ExitCode != 0) {
				var stderr = rv.StandardError.ToString ().Trim ();
				if (stderr.Length > 1024)
					stderr = stderr.Substring (0, 1024);
				if (string.IsNullOrEmpty (stderr)) {
					Log.LogError (MSBStrings.E0117, /* {0} exited with code {1} */ fileName == "xcrun" ? arguments [0] : fileName, rv.ExitCode);
				} else {
					Log.LogError (MSBStrings.E0118, /* {0} exited with code {1}:\n{2} */ fileName == "xcrun" ? arguments [0] : fileName, rv.ExitCode, stderr);
				}
			}

			return rv;
		}

		public bool ShouldExecuteRemotely () => this.ShouldExecuteRemotely (SessionId);

		protected void FileCopierReportErrorCallback (int code, string format, params object [] arguments)
		{
			Log.LogError (format, arguments);
		}

		protected void FileCopierLogCallback (int min_verbosity, string format, params object [] arguments)
		{
			MessageImportance importance;
			if (min_verbosity <= 0) {
				importance = MessageImportance.High;
			} else if (min_verbosity <= 1) {
				importance = MessageImportance.Normal;
			} else {
				importance = MessageImportance.Low;
			}
			Log.LogMessage (importance, format, arguments);
		}

		protected string GetNonEmptyStringOrFallback (ITaskItem item, string metadataName, string fallbackValue, string fallbackName = null, bool required = false)
		{
			return GetNonEmptyStringOrFallback (item, metadataName, out var _, fallbackValue, fallbackName, required);
		}

		protected string GetNonEmptyStringOrFallback (ITaskItem item, string metadataName, out bool foundInMetadata, string fallbackValue, string fallbackName = null, bool required = false)
		{
			var metadataValue = item.GetMetadata (metadataName);
			if (!string.IsNullOrEmpty (metadataValue)) {
				foundInMetadata = true;
				return metadataValue;
			}
			if (required && string.IsNullOrEmpty (fallbackValue))
				Log.LogError (MSBStrings.E7085 /* The "{0}" task was not given a value for the required parameter "{1}", nor was there a "{2}" metadata on the resource {3}. */, GetType ().Name, fallbackName ?? metadataName, metadataName, item.ItemSpec);
			foundInMetadata = false;
			return fallbackValue;
		}

		protected IEnumerable<ITaskItem> CreateItemsForAllFilesRecursively (IEnumerable<string> directories)
		{
			if (directories is null)
				yield break;

			foreach (var dir in directories) {
				// Don't try to find files if we don't have a directory in the first place (or it doesn't exist).
				if (!Directory.Exists (dir))
					continue;

				foreach (var file in Directory.EnumerateFiles (dir, "*", SearchOption.AllDirectories))
					yield return new TaskItem (file);
			}
		}

		protected IEnumerable<ITaskItem> CreateItemsForAllFilesRecursively (IEnumerable<ITaskItem> directories)
		{
			return CreateItemsForAllFilesRecursively (directories?.Select (v => v.ItemSpec));
		}
	}
}
