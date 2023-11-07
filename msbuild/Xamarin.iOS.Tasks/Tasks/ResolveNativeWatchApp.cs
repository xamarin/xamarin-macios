using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.iOS.Tasks {
	public class ResolveNativeWatchApp : ResolveNativeWatchAppTaskBase, ICancelableTask {
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
	}
}
