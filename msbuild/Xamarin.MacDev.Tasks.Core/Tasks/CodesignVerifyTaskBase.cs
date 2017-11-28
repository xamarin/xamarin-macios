using System;
using System.IO;
using System.Collections.Specialized;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;

namespace Xamarin.MacDev.Tasks
{
	public abstract class CodesignVerifyTaskBase : ToolTask
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string CodesignAllocate { get; set; }

		[Required]
		public string Resource { get; set; }

		#endregion

		protected override string ToolName {
			get { return "codesign"; }
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine ("/usr/bin", ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		// Note: Xamarin.Mac and Xamarin.iOS should both override this method to do pass platform-specific verify rules
		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineArgumentBuilder ();

			args.Add ("--verify");
			args.Add ("-vvvv");

			args.AddQuoted (Resource);

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}

		public override bool Execute ()
		{
			EnvironmentVariables = new string[] {
				"CODESIGN_ALLOCATE=" + CodesignAllocate
			};

			return base.Execute ();
		}
	}
}
