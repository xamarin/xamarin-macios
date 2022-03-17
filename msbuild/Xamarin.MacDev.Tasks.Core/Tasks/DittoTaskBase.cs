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
