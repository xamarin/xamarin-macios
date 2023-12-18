using System;
using System.IO;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class OptimizePropertyListTaskBase : XamarinToolTask {
		#region Inputs

		[Required]
		public ITaskItem Input { get; set; }

		[Required]
		[Output]
		public ITaskItem Output { get; set; }

		#endregion

		protected override string ToolName {
			get { return "plutil"; }
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			const string path = "/usr/bin/plutil";

			return File.Exists (path) ? path : ToolExe;
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineBuilder ();

			args.AppendSwitch ("-convert");
			args.AppendSwitch ("binary1");
			args.AppendSwitch ("-o");
			args.AppendFileNameIfNotNull (Output.ItemSpec);
			args.AppendFileNameIfNotNull (Input.ItemSpec);

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}

		public override bool Execute ()
		{
			Directory.CreateDirectory (Path.GetDirectoryName (Output.ItemSpec));

			return base.Execute ();
		}
	}
}
