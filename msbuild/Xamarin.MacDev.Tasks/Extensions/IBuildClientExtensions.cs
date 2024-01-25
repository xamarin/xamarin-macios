using System.Threading.Tasks;

using Xamarin.Messaging.Ssh;

namespace Xamarin.Messaging.Build.Client {
	public static class IBuildClientExtensions {
		public static async Task DeleteAppDirectoryAsync (this IBuildClient buildClient)
		{
			if (buildClient is BuildClient client) {
				await client
					.RunSshAsync (ssh => ssh.DeleteDirectoryAsync (buildClient.AppName))
					.ConfigureAwait (continueOnCapturedContext: false);
			}
		}
	}
}
