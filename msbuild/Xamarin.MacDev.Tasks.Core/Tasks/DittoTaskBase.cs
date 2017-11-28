using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class DittoTaskBase : ToolTask
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public ITaskItem Source { get; set; }

		[Required]
		[Output]
		public ITaskItem Destination { get; set; }

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

			args.Add ("-rsrc");

			args.AddQuoted (Source.ItemSpec);
			args.AddQuoted (Destination.ItemSpec);

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}
	}
}
