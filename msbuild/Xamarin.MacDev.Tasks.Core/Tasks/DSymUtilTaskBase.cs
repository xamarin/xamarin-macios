using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	[Flags]
	enum TargetArchitecture
	{
		Default      = 0,
		ARMv6        = 1,
		ARMv7        = 2,
		ARMv7s       = 4,

		// Note: needed for backwards compatability
		ARMv6_ARMv7  = 3,

		i386		 = 8,
		x86_64		 = 16,
		ARM64		 = 32,
	}

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

		public override bool Execute ()
		{
			Log.LogTaskName ("DSymUtil");
			Log.LogTaskProperty ("AppBundleDir", AppBundleDir);
			Log.LogTaskProperty ("Architectures", Architectures);
			Log.LogTaskProperty ("DSymDir", DSymDir);
			Log.LogTaskProperty ("Executable", Executable);

			return base.Execute ();
		}
	}
}
