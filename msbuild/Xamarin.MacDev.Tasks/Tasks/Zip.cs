using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class Zip : ZipTaskBase, ITaskCallback {
		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ()) {
				var taskRunner = new TaskRunner (SessionId, BuildEngine4);
				var rv = taskRunner.RunAsync (this).Result;

				// Copy the zipped file back to Windows.
				if (rv) {
					Log.LogWarning ($"Pre path: {OutputFile.ItemSpec} exists: {File.Exists (OutputFile.ItemSpec)}");
					OutputFile = new TaskItem (OutputFile.ItemSpec.Replace ('\\', '/'));
					Log.LogWarning ($"Post path: {OutputFile.ItemSpec} exists: {File.Exists (OutputFile.ItemSpec)}");
					taskRunner.GetFileAsync (this, OutputFile.ItemSpec).Wait ();
				}

				return rv;
			}

			return base.Execute ();
		}

		public override void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();

			base.Cancel ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}
