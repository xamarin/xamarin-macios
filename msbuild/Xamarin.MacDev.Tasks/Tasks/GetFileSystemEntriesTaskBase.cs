using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public abstract class GetFileSystemEntriesTaskBase : XamarinTask {
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

		public override bool Execute ()
		{
			var searchOption = Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var entries = new List<ITaskItem> ();
			foreach (var item in DirectoryPath) {
				var path = item.ItemSpec.TrimEnd ('\\', '/');
				var entriesFullPath = IncludeDirectories ?
					Directory.GetFileSystemEntries (path, Pattern, searchOption) :
					Directory.GetFiles (path, Pattern, searchOption);

				foreach (var entry in entriesFullPath) {
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
	}
}
