using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class MergeAppBundles : MergeAppBundlesTaskBase {
		public override bool Execute ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public void Cancel ()
		{
			if (!string.IsNullOrEmpty (SessionId))
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
