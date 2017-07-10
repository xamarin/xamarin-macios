using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class SymbolStripTaskBase : ToolTask
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string Executable { get; set; }

		public string SymbolFile { get; set; }

		[Required]
		public bool IsFramework { get; set; }

		#endregion

		protected override string ToolName {
			get { return "strip"; }
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine (AppleSdkSettings.DeveloperRoot, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin", ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineBuilder ();

			if (!string.IsNullOrEmpty (SymbolFile)) {
				args.AppendSwitch ("-i");
				args.AppendSwitch ("-s");
				args.AppendFileNameIfNotNull (SymbolFile);
			}

			if (IsFramework) {
				// Only remove debug symbols from frameworks.
				args.AppendSwitch ("-S");
				args.AppendSwitch ("-x");
			}

			args.AppendFileNameIfNotNull (Executable);

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}

		public override bool Execute ()
		{
			Log.LogTaskName ("SymbolStrip");
			Log.LogTaskProperty ("Executable", Executable);
			Log.LogTaskProperty ("IsFramework", IsFramework);
			Log.LogTaskProperty ("SymbolFile", SymbolFile);

			return base.Execute ();
		}
	}
}
