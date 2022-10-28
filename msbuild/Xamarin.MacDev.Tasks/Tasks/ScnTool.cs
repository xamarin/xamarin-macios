using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class ScnTool : ScnToolTaskBase {
		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public override void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();

			base.Execute ();
		}
	}
}
