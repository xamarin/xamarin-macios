using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class ArTool : XamarinToolTask2, ITaskCallback {
		#region Inputs

		[Required]
		public ITaskItem? Archive { get; set; }

		[Required]
		public ITaskItem [] Items { get; set; } = Array.Empty<ITaskItem> ();

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

		protected override IList<string> GenerateCommandLineCommands ()
		{
			var args = new List<string> ();

			args.Add ("-r");
			args.Add (Archive!.ItemSpec);

			foreach (var item in Items)
				args.Add (item.ItemSpec);

			return args;
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var dir = Path.GetDirectoryName (Archive!.ItemSpec)!;

			if (!Directory.Exists (dir))
				Directory.CreateDirectory (dir);

			return base.Execute ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		public override void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();

			base.Cancel ();
		}
	}
}
