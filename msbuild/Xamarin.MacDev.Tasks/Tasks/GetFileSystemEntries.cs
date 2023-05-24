using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class GetFileSystemEntries : GetFileSystemEntriesTaskBase, ICancelableTask, ITaskCallback {
		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
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

