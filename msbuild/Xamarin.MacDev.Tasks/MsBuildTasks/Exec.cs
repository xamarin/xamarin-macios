using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;
using System.Security;
using System.Reactive.Linq;

namespace Microsoft.Build.Tasks {
	public class Exec : ExecBase, ITaskCallback {
		public string ServerPassword { get; set; }

		public override bool Execute ()
		{
			if (!this.ShouldExecuteRemotely (SessionId))
				return base.Execute ();

			if (string.IsNullOrEmpty (ServerPassword))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return RunSudoCommandAsync ().Result;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		async Task<bool> RunSudoCommandAsync ()
		{
			try {
				var client = BuildConnection
					.GetAsync (BuildEngine4)
					.Result
					.GetClient (SessionId);
				var sshCommands = client
					.MessagingService
					.Ssh
					.Commands;
				var commandOutput = await sshCommands
					.RunSudoCommandAsync (Command, ServerPassword.ToSecureString ())
					.ConfigureAwait (continueOnCapturedContext: false);

				await commandOutput.Do (output => {
					if (ConsoleToMSBuild && !string.IsNullOrEmpty (output))
						Log.LogMessage (output);
				});

				return true;
			} catch (Exception ex) {
				Log.LogErrorFromException (ex, showStackTrace: true, showDetail: true, file: null);

				return false;
			}
		}

		public override void Cancel ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				BuildConnection.CancelAsync (BuildEngine4).Wait ();

			base.Cancel ();
		}
	}
}
