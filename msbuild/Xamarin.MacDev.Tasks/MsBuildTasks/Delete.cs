using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks
{
	public class Delete : DeleteBase
	{
		public override bool Execute ()
		{
			var result = base.Execute ();

			if (string.IsNullOrEmpty (SessionId)) {
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
