using System;
using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class CollectBundleResources : CollectBundleResourcesTaskBase, ICancelableTask {
		public override bool Execute ()
		{
			try {
				if (ShouldExecuteRemotely ()) {
					// Copy the bundle files to the build server
					new TaskRunner (SessionId, BuildEngine4).CopyInputsAsync (this).Wait ();
				}

				// But execute locally
				return base.Execute ();
			} catch (Exception ex) {
				Log.LogErrorFromException (ex);

				return false;
			}
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
