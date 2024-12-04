using System;
using System.Collections.Generic;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class CreateAssetPack : XamarinTask, ICancelableTask {
		CancellationTokenSource? cancellationTokenSource;

		#region Inputs

		[Required]
		public ITaskItem? OutputFile { get; set; }

		[Required]
		public ITaskItem? Source { get; set; }

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

		protected string GetWorkingDirectory ()
		{
			return Source!.GetMetadata ("FullPath");
		}

		List<string> GenerateCommandLineCommands ()
		{
			var args = new List<string> ();

			args.Add ("-r");
			args.Add ("-y");
			args.Add (OutputFile!.GetMetadata ("FullPath"));
			args.Add ("META-INF");

			long size = 0;
			int count = 0;

			foreach (var path in Directory.EnumerateFileSystemEntries (Source!.ItemSpec)) {
				if (Directory.Exists (path)) {
					foreach (var item in Directory.EnumerateFiles (path)) {
						var info = new FileInfo (item);

						size += info.Length;
						count++;
					}
				} else {
					var info = new FileInfo (path);

					size += info.Length;
				}

				args.Add (Path.GetFileName (path));
				count++;
			}

			SaveMetaFile (count, size);

			return args;
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			// We need to delete any existing ipa files, zip will just add to it otherwise
			if (File.Exists (OutputFile!.ItemSpec))
				File.Delete (OutputFile.ItemSpec);

			var args = GenerateCommandLineCommands ();
			var executable = GetExecutable (args, "zip", ZipPath);
			cancellationTokenSource = new CancellationTokenSource ();
			ExecuteAsync (Log, executable, args, workingDirectory: GetWorkingDirectory (), cancellationToken: cancellationTokenSource.Token).Wait ();
			return !Log.HasLoggedErrors;
		}

		void SaveMetaFile (int count, long size)
		{
			var meta = new PDictionary ();

			meta.Add ("RecordCount", new PNumber (count));
			meta.Add ("StandardDirectoryPerms", new PNumber (16877));
			meta.Add ("StandardFilePerms", new PNumber (-32348));
			meta.Add ("TotalUncompressedBytes", new PNumber ((int) size));
			meta.Add ("Version", new PNumber (2));

			Directory.CreateDirectory (Path.Combine (Source!.ItemSpec, "META-INF"));
			meta.Save (Path.Combine (Source.ItemSpec, "META-INF", "com.apple.ZipMetadata.plist"), true, true);
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ()) {
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
			} else {
				cancellationTokenSource?.Cancel ();
			}
		}
	}
}
