using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks {
	public class WriteLinesToFile : WriteLinesToFileBase {
		public override bool Execute ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}
	}
}
