using System;
using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks
{
	public class RemoveDir : RemoveDirBase
	{
		public bool RemoveAppDir { get; set; }

		public override bool Execute ()
		{
			var result = base.Execute ();

			if (Environment.OSVersion.Platform != PlatformID.Win32NT || string.IsNullOrEmpty (SessionId)) {
				return result;
			}

			var client = BuildConnection
				.GetAsync (SessionId, BuildEngine4)
				.Result
				.Client;

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
