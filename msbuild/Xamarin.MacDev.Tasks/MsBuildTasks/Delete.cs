using System;
using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks
{
	public class Delete : DeleteBase
	{
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

			foreach (var file in Files) {
				client.DeleteFileAsync (file.ItemSpec).Wait ();
			}

			return result;
		}
	}
}
