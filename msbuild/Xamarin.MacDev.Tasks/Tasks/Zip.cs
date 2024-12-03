using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class Zip : XamarinTask, ICancelableTask {
		CancellationTokenSource? cancellationTokenSource;

		#region Inputs

		[Output]
		[Required]
		public ITaskItem? OutputFile { get; set; }

		public bool Recursive { get; set; }

		[Required]
		public ITaskItem [] Sources { get; set; } = Array.Empty<ITaskItem> ();

		public bool Symlinks { get; set; }

		[Required]
		public ITaskItem? WorkingDirectory { get; set; }

		public string ZipPath { get; set; } = string.Empty;

		#endregion

		static string GetExecutable (List<string> arguments, string toolName, string toolPathOverride)
		{
			if (string.IsNullOrEmpty (toolPathOverride)) {
				arguments.Insert (0, toolName);
				return "xcrun";
			}
			return toolPathOverride;
		}

		string GetWorkingDirectory ()
		{
			return WorkingDirectory!.GetMetadata ("FullPath");
		}

		List<string> GenerateCommandLineCommands ()
		{
			var args = new List<string> ();

			if (Recursive)
				args.Add ("-r");

			if (Symlinks)
				args.Add ("-y");

			args.Add (OutputFile!.GetMetadata ("FullPath"));

			var root = GetWorkingDirectory ();
			for (int i = 0; i < Sources.Length; i++) {
				var relative = PathUtils.AbsoluteToRelative (root, Sources [i].GetMetadata ("FullPath"));
				args.Add (relative);
			}

			return args;
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ()) {
				var taskRunner = new TaskRunner (SessionId, BuildEngine4);
				var rv = taskRunner.RunAsync (this).Result;

				// Copy the zipped file back to Windows.
				if (rv)
					taskRunner.GetFileAsync (this, OutputFile!.ItemSpec).Wait ();

				return rv;
			}

			var args = GenerateCommandLineCommands ();
			var executable = GetExecutable (args, "zip", ZipPath);
			cancellationTokenSource = new CancellationTokenSource ();
			ExecuteAsync (Log, executable, args, workingDirectory: GetWorkingDirectory (), cancellationToken: cancellationTokenSource.Token).Wait ();
			return !Log.HasLoggedErrors;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ()) {
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
			} else {
				cancellationTokenSource?.Cancel ();
			}
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
