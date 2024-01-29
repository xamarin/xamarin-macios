using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks {
	public class Copy : CopyBase {
		public override bool Execute ()
		{
			if (!this.ShouldExecuteRemotely (SessionId))
				return base.Execute ();

			var taskRunner = new TaskRunner (SessionId, BuildEngine4);

			taskRunner.FixReferencedItems (this, SourceFiles);

			return taskRunner.RunAsync (this).Result;
		}
	}
}
