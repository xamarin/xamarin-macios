using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class PrepareNativeReferences : PrepareNativeReferencesTaskBase, ITaskCallback, ICancelableTask {
		public override bool Execute ()
		{
			if (!ShouldExecuteRemotely ())
				return base.Execute ();

			var taskRunner = new TaskRunner (SessionId, BuildEngine4);

			try {
				var success = taskRunner.RunAsync (this).Result;

				if (success && LinkWithAttributes is not null)
					taskRunner.GetFileAsync (this, LinkWithAttributes.ItemSpec).Wait ();

				return success;
			} catch (Exception ex) {
				Log.LogErrorFromException (ex);

				return false;
			}
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			return CreateItemsForAllFilesRecursively (NativeReferences);
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
