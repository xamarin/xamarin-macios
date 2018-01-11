using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;

namespace Xamarin.MacDev.Tasks
{
	public abstract class MetalTaskBase : ToolTask
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string ResourcePrefix { get; set; }

		[Required]
		public string SdkDevPath { get; set; }

		[Required]
		public ITaskItem SourceFile { get; set; }

		#endregion

		[Output]
		public ITaskItem OutputFile { get; set; }

		protected abstract string OperatingSystem {
			get;
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

		protected override string GenerateCommandLineCommands ()
		{
			var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);
			var intermediate = Path.Combine (IntermediateOutputPath, ToolName);
			var logicalName = BundleResource.GetLogicalName (ProjectDir, prefixes, SourceFile, !string.IsNullOrEmpty(SessionId));
			var path = Path.Combine (intermediate, logicalName);
			var args = new CommandLineArgumentBuilder ();
			var dir = Path.GetDirectoryName (path);

			if (!Directory.Exists (dir))
				Directory.CreateDirectory (dir);

			OutputFile = new TaskItem (Path.ChangeExtension (path, ".air"));
			OutputFile.SetMetadata ("LogicalName", Path.ChangeExtension (logicalName, ".air"));

			args.Add ("-arch", "air64");
			args.Add ("-emit-llvm");
			args.Add ("-c");
			args.Add ("-gline-tables-only");
			args.Add ("-ffast-math");
			args.Add (string.Format ("-std={0}-metal1.0", OperatingSystem));

			args.Add ("-serialize-diagnostics");
			args.AddQuoted (Path.ChangeExtension (path, ".dia"));

			args.Add ("-o");
			args.AddQuoted (Path.ChangeExtension (path, ".air"));

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
