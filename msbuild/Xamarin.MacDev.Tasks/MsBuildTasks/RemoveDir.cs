extern alias Microsoft_Build_Tasks_Core;
using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks {
	public class RemoveDir : Microsoft_Build_Tasks_Core::Microsoft.Build.Tasks.RemoveDir {
		public string SessionId { get; set; } = string.Empty;
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
