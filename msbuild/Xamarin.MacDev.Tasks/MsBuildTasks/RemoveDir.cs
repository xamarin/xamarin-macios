using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks {
	public class RemoveDir : RemoveDirBase {
		public bool RemoveAppDir { get; set; }

		public override bool Execute ()
		{
			var result = base.Execute ();

			if (!this.ShouldExecuteRemotely (SessionId)) {
				return result;
			}

			var client = BuildConnection
				.GetAsync (BuildEngine4)
				.Result
				.GetClient (SessionId);

			if (!client.IsConnected) {
				return result;
			}

			foreach (var directory in Directories) {
				client.DeleteDirectoryAsync (directory.ItemSpec).Wait ();
			}

			if (RemoveAppDir) {
				client.DeleteAppDirectoryAsync ().Wait ();
			}

			return result;
		}
	}
}
