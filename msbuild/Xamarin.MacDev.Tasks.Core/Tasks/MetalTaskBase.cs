using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Utils;
using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks
{
	public abstract class MetalTaskBase : ToolTask
	{
		#region Inputs

		public string SessionId { get; set; }

		public ITaskItem AppManifest { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string ResourcePrefix { get; set; }

		[Required]
		public string SdkDevPath { get; set; }

		[Required]
		public string SdkVersion { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SdkRoot { get; set; }

		[Required]
		public ITaskItem SourceFile { get; set; }

		public TargetFramework TargetFramework { get { return TargetFramework.Parse (TargetFrameworkMoniker); } }

		[Required]
		public string TargetFrameworkMoniker { get; set; }

		#endregion

		[Output]
		public ITaskItem OutputFile { get; set; }

		protected abstract string MinimumDeploymentTargetKey {
			get;
		}

		protected virtual string OperatingSystem {
			get {
				switch (PlatformFrameworkHelper.GetFramework (TargetFrameworkMoniker)) {
				case ApplePlatform.WatchOS:
					return SdkIsSimulator ? "watchos-simulator" : "watchos";
				case ApplePlatform.TVOS:
					return SdkIsSimulator ? "tvos-simulator" : "tvos";
				case ApplePlatform.MacOSX:
					return "macosx";
				case ApplePlatform.iOS:
					return SdkIsSimulator ? "iphonesimulator" : "ios";
				default:
					Log.LogError (MSBStrings.E0169, TargetFrameworkMoniker);
					return string.Empty;
				}
			}
		}

		protected abstract string DevicePlatformBinDir {
			get;
		}

		protected override string ToolName {
			get { return "metal"; }
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine (DevicePlatformBinDir, ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		public override bool Execute ()
		{
			if (AppleSdkSettings.XcodeVersion.Major >= 11)
				EnvironmentVariables = EnvironmentVariables.CopyAndAdd ($"SDKROOT={SdkRoot}");
			return base.Execute ();
		}

		protected override string GenerateCommandLineCommands ()
		{
			var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);
			var intermediate = Path.Combine (IntermediateOutputPath, ToolName);
			var logicalName = BundleResource.GetLogicalName (ProjectDir, prefixes, SourceFile, !string.IsNullOrEmpty(SessionId));
			var path = Path.Combine (intermediate, logicalName);
			var args = new CommandLineArgumentBuilder ();
			var dir = Path.GetDirectoryName (path);
			string minimumDeploymentTarget;

			if (!Directory.Exists (dir))
				Directory.CreateDirectory (dir);

			if (AppManifest != null) {
				var plist = PDictionary.FromFile (AppManifest.ItemSpec);
				PString value;

				if (!plist.TryGetValue (MinimumDeploymentTargetKey, out value) || string.IsNullOrEmpty (value.Value))
					minimumDeploymentTarget = SdkVersion;
				else
					minimumDeploymentTarget = value.Value;
			} else {
				minimumDeploymentTarget = SdkVersion;
			}

			OutputFile = new TaskItem (Path.ChangeExtension (path, ".air"));
			OutputFile.SetMetadata ("LogicalName", Path.ChangeExtension (logicalName, ".air"));

			args.Add ("-arch", "air64");
			args.Add ("-emit-llvm");
			args.Add ("-c");
			args.Add ("-gline-tables-only");
			args.Add ("-ffast-math");

			args.Add ("-serialize-diagnostics");
			args.AddQuoted (Path.ChangeExtension (path, ".dia"));

			args.Add ("-o");
			args.AddQuoted (Path.ChangeExtension (path, ".air"));

			args.Add (string.Format ("-m{0}-version-min={1}", OperatingSystem, minimumDeploymentTarget));

			args.AddQuoted (SourceFile.ItemSpec);

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}
	}
}
