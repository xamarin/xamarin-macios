using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks {
	public class Delete : DeleteBase, ITaskCallback {
		public override bool Execute ()
		{
			var result = base.Execute ();

			if (!this.ShouldExecuteRemotely (SessionId)) {
				return result;
			}

			var client = BuildConnection
				.GetAsync (BuildEngine4)
				.Result
				.Client;

			if (!client.IsConnected) {
				return result;
			}

			foreach (var file in Files) {
				client.DeleteFileAsync (file.ItemSpec).Wait ();
			}

			return result;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;
	}
}
