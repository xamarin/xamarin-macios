using System;
using System.IO;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using Xamarin.Utils;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class ScnToolTaskBase : XamarinToolTask {
		string sdkDevPath;

		#region Inputs

		[Required]
		public string IntermediateOutputPath { get; set; }

		[Required]
		public string InputScene { get; set; }

		public bool IsWatchApp { get; set; }

		[Required]
		public string OutputScene { get; set; }

		[Required]
		public string SdkPlatform { get; set; }

		[Required]
		public string SdkRoot { get; set; }

		[Required]
		public string SdkVersion { get; set; }

		[Required]
		public string SdkDevPath {
			get { return sdkDevPath; }
			set {
				sdkDevPath = value;

				SetEnvironmentVariable ("DEVELOPER_DIR", sdkDevPath);
			}
		}

		#endregion

		string DevicePlatformBinDir {
			get { return Path.Combine (SdkDevPath, "usr", "bin"); }
		}

		protected virtual string OperatingSystem {
			get {
				return PlatformFrameworkHelper.GetOperatingSystem (TargetFrameworkMoniker);
			}
		}

		protected override string ToolName {
			get { return "scntool"; }
		}

		void SetEnvironmentVariable (string variableName, string value)
		{
			var envVariables = EnvironmentVariables;
			var index = -1;

			if (envVariables is null) {
				envVariables = new string [1];
				index = 0;
			} else {
				for (int i = 0; i < envVariables.Length; i++) {
					if (envVariables [i].StartsWith (variableName + "=", StringComparison.Ordinal)) {
						index = i;
						break;
					}
				}

				if (index < 0) {
					Array.Resize<string> (ref envVariables, envVariables.Length + 1);
					index = envVariables.Length - 1;
				}
			}

			envVariables [index] = string.Format ("{0}={1}", variableName, value);

			EnvironmentVariables = envVariables;
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine (DevicePlatformBinDir, ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineArgumentBuilder ();

			args.Add ("--compress");
			args.AddQuoted (InputScene);
			args.Add ("-o");
			args.AddQuoted (OutputScene);
			args.AddQuotedFormat ("--sdk-root={0}", SdkRoot);
			args.AddQuotedFormat ("--target-build-dir={0}", IntermediateOutputPath);
			if (AppleSdkSettings.XcodeVersion.Major >= 13) {
				// I'm not sure which Xcode version these options are available in, but it's at least Xcode 13+
				args.AddQuotedFormat ("--target-version={0}", SdkVersion);
				args.AddQuotedFormat ("--target-platform={0}", PlatformUtils.GetTargetPlatform (SdkPlatform, IsWatchApp));
			} else {
				args.AddQuotedFormat ("--target-version-{0}={1}", OperatingSystem, SdkVersion);
			}

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}

		public override bool Execute ()
		{
			Directory.CreateDirectory (Path.GetDirectoryName (OutputScene));

			return base.Execute ();
		}
	}
}
