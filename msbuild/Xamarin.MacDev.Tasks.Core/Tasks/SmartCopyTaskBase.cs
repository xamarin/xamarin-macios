using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class SmartCopyTaskBase : Task
	{
		readonly List<ITaskItem> copied = new List<ITaskItem> ();

		#region Inputs

		public string SessionId { get; set; }

		public ITaskItem[] DestinationFiles { get; set; }

		public ITaskItem DestinationFolder { get; set; }

		[Required]
		public ITaskItem[] SourceFiles { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] CopiedFiles { get; set; }

		#endregion

		static bool FileChanged (string source, string target)
		{
			if (!File.Exists (target))
				return true;

			var sourceInfo = new FileInfo (source);
			var targetInfo = new FileInfo (target);

			return sourceInfo.Length != targetInfo.Length || File.GetLastWriteTime (source) > File.GetLastWriteTime (target);
		}

		void EnsureDirectoryExists (string path)
		{
			if (Directory.Exists (path))
				return;

			if (File.Exists (path))
				File.Delete (path);

			Log.LogMessage ("Creating directory '{0}'", path);

			Directory.CreateDirectory (path);
		}

		void CopyFile (string source, string target, string targetItemSpec)
		{
			var dirName = Path.GetDirectoryName (target);

			EnsureDirectoryExists (dirName);

			Log.LogMessage (MessageImportance.Normal, "Copying file from '{0}' to '{1}'", source, target);

			File.Copy (source, target, true);

			copied.Add (new TaskItem (targetItemSpec));
		}

		public override bool Execute ()
		{
			Log.LogTaskName ("SmartCopy");
			Log.LogTaskProperty ("DestinationFiles", DestinationFiles);
			Log.LogTaskProperty ("DestinationFolder", DestinationFolder);
			Log.LogTaskProperty ("SourceFiles", SourceFiles);

			if (DestinationFiles != null && DestinationFolder != null) {
				Log.LogError ("You must specify a DestinationFolder or the DestinationFiles, but not both.");
				return false;
			}

			try {
				if (DestinationFiles != null) {
					if (DestinationFiles.Length != SourceFiles.Length) {
						Log.LogError ("The number of DestinationFiles must match the number of SourceFiles.");
						return false;
					}

					for (int i = 0; i < SourceFiles.Length; i++) {
						var target = DestinationFiles[i].GetMetadata ("FullPath");
						var source = SourceFiles[i].GetMetadata ("FullPath");
						var targetDir = Path.GetDirectoryName (target);

						EnsureDirectoryExists (targetDir);

						if (FileChanged (source, target))
							CopyFile (source, target, DestinationFiles[i].ItemSpec);
					}
				} else if (DestinationFolder != null) {
					var destinationFolder = DestinationFolder.GetMetadata ("FullPath");

					EnsureDirectoryExists (destinationFolder);

					foreach (var item in SourceFiles) {
						var target = Path.Combine (destinationFolder, Path.GetFileName (item.ItemSpec));
						var source = item.GetMetadata ("FullPath");

						if (FileChanged (source, target))
							CopyFile (source, target, Path.Combine (DestinationFolder.ItemSpec, Path.GetFileName (item.ItemSpec)));
					}
				} else {
					Log.LogError ("You must specify a DestinationFolder or the DestinationFiles.");
					return false;
				}
			} catch (Exception ex) {
				Log.LogError (ex.ToString ());
			}

			CopiedFiles = copied.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
