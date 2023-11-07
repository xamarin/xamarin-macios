using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;
using Xamarin.MacDev.Tasks;
using Xamarin.Messaging;
using Xamarin.Messaging.Build.Client;
using Xamarin.Messaging.Ssh;

namespace Xamarin.iOS.Tasks {
	public class CopyArchiveFiles : XamarinTask, ICancelableTask {
		[Required]
		public string ArchivePath { get; set; }

		[Required]
		public string AppName { get; set; }

		[Required]
		public string TargetPath { get; set; }

		public override bool Execute ()
		{
			if (Path.GetExtension (ArchivePath) != ".xcarchive") {
				Log.LogError (MSBStrings.E0186, ArchivePath);

				return false;
			}

			var client = GetBuildClientAsync ().Result;

			if (!client.IsConnected) {
				Log.LogError (MSBStrings.E0177, ArchivePath);

				return false;
			}

			var sshCommands = client.MessagingService.Ssh.Commands;

			if (sshCommands is null) {
				Log.LogError (MSBStrings.E0177, ArchivePath);

				return false;
			}

			try {
				CopyArchiveAsync (sshCommands).Wait ();

				return true;
			} catch (Exception ex) {
				Log.LogError (MSBStrings.E0178, ArchivePath, TargetPath, ex.Message);

				return false;
			}
		}

		public void Cancel () => BuildConnection.CancelAsync (BuildEngine4).Wait ();

		async System.Threading.Tasks.Task<IBuildClient> GetBuildClientAsync ()
		{
			var connection = await BuildConnection
				.GetAsync (BuildEngine4)
				.ConfigureAwait (continueOnCapturedContext: false);

			return connection.GetClient (SessionId);
		}

		async System.Threading.Tasks.Task CopyArchiveAsync (ISshCommands sshCommands)
		{
			var serverHomeDirectory = await sshCommands.GetHomeDirectoryAsync ().ConfigureAwait (continueOnCapturedContext: false);
			var buildPath = PlatformPath.GetServerBuildPath (serverHomeDirectory, AppName, SessionId, TargetPath);

			if (!Directory.Exists (buildPath)) {
				await sshCommands.CreateDirectoryAsync (buildPath).ConfigureAwait (continueOnCapturedContext: false);
			}

			await sshCommands.CopyDirectoryAsync (ArchivePath, buildPath).ConfigureAwait (continueOnCapturedContext: false);
		}
	}
}
