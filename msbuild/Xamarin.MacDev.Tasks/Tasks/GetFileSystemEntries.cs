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

				rv.AddRange (Directory.GetFiles (spec, "*", SearchOption.AllDirectories));
			}

			return rv.Select (f => new TaskItem (f));
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;
	}
}

