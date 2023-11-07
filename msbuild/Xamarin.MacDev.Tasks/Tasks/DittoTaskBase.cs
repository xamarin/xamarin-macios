#nullable enable

using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks {
	public abstract class DittoTaskBase : XamarinToolTask {
		#region Inputs

		public string? AdditionalArguments { get; set; }

		// If the input directory should be copied from Windows to the Mac in
		// a remote build. In some cases we only maintain empty files on
		// Windows to keep track of modified files, so that we don't have to
		// transfer the entire file back to Windows, and in those cases we
		// don't want to copy the empty content back to the Mac. In other
		// cases the input comes from Windows, and in that case we want to
		// copy the entire input to the Mac - so we need an option to select
		// the mode.
		public bool CopyFromWindows { get; set; }

		[Required]
		public ITaskItem? Source { get; set; }

		[Required]
		public ITaskItem? Destination { get; set; }

		public bool TouchDestinationFiles { get; set; }

		// This property is required for XVS to work properly, even though it's not used for anything in the targets.
		[Output]
		public ITaskItem [] CopiedFiles { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		protected override string ToolName {
			get { return "ditto"; }
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine ("/usr/bin", ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineArgumentBuilder ();

			args.AddQuoted (Path.GetFullPath (Source!.ItemSpec));
			args.AddQuoted (Path.GetFullPath (Destination!.ItemSpec));
			if (!string.IsNullOrEmpty (AdditionalArguments))
				args.Add (AdditionalArguments);

			return args.ToString ();
		}

		public override bool Execute ()
		{
			if (!base.Execute ())
				return false;

			// Create a list of all the files we've copied
			var copiedFiles = new List<ITaskItem> ();
			var destination = Destination!.ItemSpec;
			if (Directory.Exists (destination)) {
				foreach (var file in Directory.EnumerateFiles (destination, "*", SearchOption.AllDirectories)) {
					if (TouchDestinationFiles)
						File.SetLastWriteTimeUtc (file, DateTime.UtcNow);
					copiedFiles.Add (new TaskItem (file));
				}
			} else {
				copiedFiles.Add (Destination);
			}
			CopiedFiles = copiedFiles.ToArray ();

			return !Log.HasLoggedErrors;
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}
	}
}
