#nullable enable

using System;
using System.IO;

using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks
{
	public abstract class DittoTaskBase : XamarinToolTask
	{
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
		[Output]
		public ITaskItem? Destination { get; set; }

		public bool TouchDestinationFiles { get; set; }
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

			if (TouchDestinationFiles) {
				foreach (var file in Directory.EnumerateFiles (Destination!.ItemSpec, "*", SearchOption.AllDirectories)) {
					File.SetLastWriteTimeUtc (file, DateTime.UtcNow);
				}
			}

			return !Log.HasLoggedErrors;
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}
	}
}
