using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class GetFileSystemEntries : XamarinTask, ICancelableTask, ITaskCallback {
		#region Inputs

		[Required]
		public ITaskItem [] DirectoryPath { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public string Pattern { get; set; } = string.Empty;

		[Required]
		public bool Recursive { get; set; }

		[Required]
		public bool IncludeDirectories { get; set; }

		// If the input directory should be copied from Windows to the Mac in
		// a remote build.
		public bool CopyFromWindows { get; set; }
		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] Entries { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		bool cancelled;

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var searchOption = Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var entries = new List<ITaskItem> ();
			foreach (var item in DirectoryPath) {
				var path = item.ItemSpec.TrimEnd ('\\', '/');
				var entriesFullPath = IncludeDirectories ?
					Directory.EnumerateFileSystemEntries (path, Pattern, searchOption) :
					Directory.EnumerateFiles (path, Pattern, searchOption);

				Log.LogMessage (MessageImportance.Low, $"Searching for {(IncludeDirectories ? "files and directories" : "files")} in {path} with pattern '{Pattern}' and search option {searchOption}. Current directory: {Environment.CurrentDirectory}");

				foreach (var entry in entriesFullPath) {
					if (cancelled) {
						Log.LogError (MSBStrings.E7119 /* Task execution was cancelled. */);
						return false;
					}
					var recursiveDir = entry.Substring (path.Length + 1);
					var newItem = new TaskItem (entry);
					item.CopyMetadataTo (newItem);
					newItem.SetMetadata ("RecursiveDir", recursiveDir);
					newItem.SetMetadata ("OriginalItemSpec", item.ItemSpec);
					entries.Add (newItem);
				}
			}

			Entries = entries.ToArray ();

			return !Log.HasLoggedErrors;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
			cancelled = true;
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			if (!CopyFromWindows)
				return Enumerable.Empty<ITaskItem> ();

			// TaskRunner doesn't know how to copy directories to Mac, so list each file.
			var rv = new List<string> ();
			foreach (var path in DirectoryPath) {
				var spec = path.ItemSpec;
				if (!Directory.Exists (spec))
					continue;

				var files = Directory.GetFiles (spec, "*", SearchOption.AllDirectories);
				foreach (var file in files) {
					// Only copy non-empty files, so that we don't end up
					// copying an empty file that happens to be an output file
					// from a previous target (and thus overwriting that file
					// on Windows).
					var finfo = new FileInfo (file);
					if (!finfo.Exists || finfo.Length == 0)
						continue;
					rv.Add (file);
				}
			}

			return rv.Select (f => new TaskItem (f));
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;
	}
}
