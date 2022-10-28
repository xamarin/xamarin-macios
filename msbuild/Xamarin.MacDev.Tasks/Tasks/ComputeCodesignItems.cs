using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class ComputeCodesignItems : ComputeCodesignItemsTaskBase, ITaskCallback, ICancelableTask {
		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		// This task does not create or modify any files, and it should only
		// deal with files that are already on the mac, so no need to copy any
		// files either way.
		public bool ShouldCopyToBuildServer (ITaskItem item) => false;
		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
