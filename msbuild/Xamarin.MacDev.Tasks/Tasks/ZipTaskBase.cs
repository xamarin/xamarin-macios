using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public abstract class ZipTaskBase : XamarinToolTask {
		#region Inputs

		[Output]
		[Required]
		public ITaskItem OutputFile { get; set; }

		public bool Recursive { get; set; }

		[Required]
		public ITaskItem [] Sources { get; set; }

		public bool Symlinks { get; set; }

		[Required]
		public ITaskItem WorkingDirectory { get; set; }

		#endregion

		protected override string ToolName {
			get { return "zip"; }
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine ("/usr/bin", ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		protected override string GetWorkingDirectory ()
		{
			return WorkingDirectory.GetMetadata ("FullPath");
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineArgumentBuilder ();

			if (Recursive)
				args.Add ("-r");

			if (Symlinks)
				args.Add ("-y");

			args.AddQuoted (OutputFile.GetMetadata ("FullPath"));

			var root = WorkingDirectory.GetMetadata ("FullPath");
			for (int i = 0; i < Sources.Length; i++) {
				var relative = PathUtils.AbsoluteToRelative (root, Sources [i].GetMetadata ("FullPath"));
				args.AddQuoted (relative);
			}

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}
	}
}
