using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public abstract class MetalLibTaskBase : XamarinToolTask {
		#region Inputs

		[Required]
		public ITaskItem [] Items { get; set; }

		[Required]
		public string OutputLibrary { get; set; }

		[Required]
		public string SdkDevPath { get; set; }

		[Required]
		public string SdkRoot { get; set; }

		#endregion

		string DevicePlatformBinDir {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return AppleSdkSettings.XcodeVersion.Major >= 11
						? Path.Combine (SdkDevPath, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin")
						: Path.Combine (SdkDevPath, "Platforms", "iPhoneOS.platform", "usr", "bin");
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return AppleSdkSettings.XcodeVersion.Major >= 10
						? Path.Combine (SdkDevPath, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin")
						: Path.Combine (SdkDevPath, "Platforms", "MacOSX.platform", "usr", "bin");
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		protected override string ToolName {
			get { return "metallib"; }
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

			args.Add ("-o");
			args.AddQuoted (OutputLibrary);

			foreach (var item in Items)
				args.AddQuoted (item.ItemSpec);

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}

		public override bool Execute ()
		{
			var dir = Path.GetDirectoryName (OutputLibrary);

			if (!Directory.Exists (dir))
				Directory.CreateDirectory (dir);

			if (AppleSdkSettings.XcodeVersion.Major >= 11)
				EnvironmentVariables = EnvironmentVariables.CopyAndAdd ($"SDKROOT={SdkRoot}");

			return base.Execute ();
		}
	}
}
