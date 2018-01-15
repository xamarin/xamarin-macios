using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;

namespace Xamarin.MacDev.Tasks
{
	public abstract class ArToolTaskBase : ToolTask
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public ITaskItem Archive { get; set; }

		[Required]
		public ITaskItem[] Items { get; set; }

		#endregion

		protected override string ToolName {
			get { return "ar"; }
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
			var args = new CommandLineArgumentBuilder ();

			args.Add ("-r");
			args.AddQuoted (Archive.ItemSpec);

			foreach (var item in Items)
				args.AddQuoted (item.ItemSpec);

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			// TODO: do proper parsing of error messages and such
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}

		public override bool Execute ()
		{
			var dir = Path.GetDirectoryName (Archive.ItemSpec);

			if (!Directory.Exists (dir))
				Directory.CreateDirectory (dir);

			return base.Execute ();
		}
	}
}
