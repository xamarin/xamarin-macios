using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class DSymUtilTaskBase : ToolTask
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string AppBundleDir { get; set; }

		public string Architectures { get; set; }

		[Required]
		public string DSymDir { get; set; }

		[Required]
		public string Executable { get; set; }

		#endregion

		protected override string ToolName {
			get { return "dsymutil"; }
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

			args.AppendSwitch ("-t");
			args.AppendSwitch ("4");
			args.AppendSwitch ("-z");
			args.AppendSwitch ("-o");
			args.AppendFileNameIfNotNull (DSymDir);
			args.AppendFileNameIfNotNull (Executable);

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}
	}
}
