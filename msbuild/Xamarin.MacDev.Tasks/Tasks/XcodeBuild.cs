using System;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public class XcodeBuild : XamarinToolTaskWithOutput
	{
		public override string TaskPrefix => "XCBD";

		protected override string ToolName => "xcodebuild";

		public string Command { get; set; } = string.Empty;

		public string Arguments { get; set; } = string.Empty;


		// archive arguments
		public string ProjectPath { get; set; } = string.Empty;

		public string SchemeName { get; set; } = string.Empty;

		public string Configuration { get; set; } = string.Empty;

		public string ArchivePlatform { get; set; } = string.Empty;

		public string ArchiveOutputPath { get; set; } = string.Empty;

		public string DerivedDataPath { get; set; } = string.Empty;

		public string PackageCachePath { get; set; } = string.Empty;

		// create-xcframework arguments
		public ITaskItem[] FrameworkArchives { get; set; } = Array.Empty<ITaskItem> ();

		public string FrameworkName { get; set; } = string.Empty;

		public string FrameworkOutputPath { get; set; } = string.Empty;


		protected override string GenerateFullPathToTool ()
		{
			return Path.Combine ("/usr", "bin", ToolExe);
		}

		protected override string GenerateCommandLineCommands ()
		{
			var cmd = new CommandLineBuilder ();

			if (!string.IsNullOrEmpty (Command))
				cmd.AppendSwitch (Command);

			if (!string.IsNullOrEmpty (ProjectPath))
				cmd.AppendSwitchIfNotNull ("-project ", ProjectPath);

			if (!string.IsNullOrEmpty (SchemeName))
				cmd.AppendSwitchIfNotNull ("-scheme ", SchemeName);

			if (!string.IsNullOrEmpty (Configuration))
				cmd.AppendSwitchIfNotNull ("-configuration ", Configuration);

			if (!string.IsNullOrEmpty (ArchivePlatform))
				cmd.AppendSwitchIfNotNull ("-destination ", ArchivePlatform);

			if (!string.IsNullOrEmpty (ArchiveOutputPath))
				cmd.AppendSwitchIfNotNull ("-archivePath ", ArchiveOutputPath);

			if (!string.IsNullOrEmpty (DerivedDataPath))
				cmd.AppendSwitchIfNotNull ("-derivedDataPath ", DerivedDataPath);

			if (!string.IsNullOrEmpty (PackageCachePath))
				cmd.AppendSwitchIfNotNull ("-packageCachePath ", PackageCachePath);

			foreach (var frameworkArchive in FrameworkArchives) {
				cmd.AppendSwitchIfNotNull ("-archive ", frameworkArchive.ItemSpec);
				cmd.AppendSwitchIfNotNull ("-framework ", FrameworkName);
			}

			if (!string.IsNullOrEmpty (FrameworkOutputPath))
				cmd.AppendSwitchIfNotNull ("-output ", FrameworkOutputPath);

			if (!string.IsNullOrEmpty (Arguments))
				cmd.AppendSwitch (Arguments);

			return cmd.ToString ();
		}

		public override bool RunTask ()
		{
			if (RuntimeInformation.IsOSPlatform (OSPlatform.OSX))
			{
				if (!File.Exists (GenerateFullPathToTool ())) {
					// TODO loc
					Log.LogError ($"{TaskPrefix}1000 {{0}}", $"Unable to to find executable \"{ToolName}\". Please make sure Xcode is properly installed.");
					return false;
				}

				return base.RunTask ();
			}
			else
			{
				Log.LogWarning ($"{TaskPrefix}5000 {{0}}", $"Skipping attempt to run \"{ToolName}\" with arguments \"{GenerateCommandLineCommands ()}\". The \"@(MaciOSXcodeProject)\" build action is only supported on macOS.");
				return true;
			}
		}

	}
}
