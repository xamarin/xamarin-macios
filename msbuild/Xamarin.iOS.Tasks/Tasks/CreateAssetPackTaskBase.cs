using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;
using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.iOS.Tasks {
	public class CreateAssetPack : XamarinToolTask {
		#region Inputs

		[Required]
		public ITaskItem? OutputFile { get; set; }

		[Required]
		public ITaskItem? Source { get; set; }

		#endregion

		protected override string ToolName {
			get { return "zip"; }
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine ("/usr/bin", ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		protected override string GetWorkingDirectory ()
		{
			return Source!.GetMetadata ("FullPath");
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineArgumentBuilder ();

			args.Add ("-r", "-y");
			args.AddQuoted (OutputFile!.GetMetadata ("FullPath"));
			args.AddQuoted ("META-INF");

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

				args.AddQuoted (Path.GetFileName (path));
				count++;
			}

			SaveMetaFile (count, size);

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			Log.LogMessage (messageImportance, "{0}", singleLine);
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			// We need to delete any existing ipa files, zip will just add to it otherwise
			if (File.Exists (OutputFile!.ItemSpec))
				File.Delete (OutputFile.ItemSpec);

			return base.Execute ();
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

		public override void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();

			base.Cancel ();
		}
	}
}
