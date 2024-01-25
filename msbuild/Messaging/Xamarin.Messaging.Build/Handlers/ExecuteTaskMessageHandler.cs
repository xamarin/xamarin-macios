using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using Xamarin.Messaging.Build.Contracts;
using Xamarin.Messaging.Build.Serialization;
using Xamarin.Messaging.Client;

namespace Xamarin.Messaging.Build {
	public class ExecuteTaskMessageHandler : RequestHandler<ExecuteTaskMessage, ExecuteTaskResult> {
		static readonly ITracer tracer = Tracer.Get<ExecuteTaskMessageHandler> ();
		static readonly object lockObject = new object ();

		ITaskRunner runner;

		public ExecuteTaskMessageHandler ()
		{
			var runner = new TaskRunner (new TaskSerializer ());

			runner.LoadXamarinTasks ();

			this.runner = runner;
		}

		public ExecuteTaskMessageHandler (ITaskRunner runner) => this.runner = runner;

		protected override async Task<ExecuteTaskResult> ExecuteAsync (ExecuteTaskMessage message)
		{
			return await Task.Run (() => {
				// We need to lock in order to change the current directory
				lock (lockObject) {
					var currentDirectory = Directory.GetCurrentDirectory ();

					try {
						var buildDirectory = Path.Combine (MessagingContext.GetBuildPath (), message.AppName, message.SessionId);

						Directory.CreateDirectory (buildDirectory);

						Directory.SetCurrentDirectory (buildDirectory);

						return runner.Execute (message.TaskName, message.Inputs);
					} finally {
						Directory.SetCurrentDirectory (currentDirectory);
					}
				}
			}).ConfigureAwait (continueOnCapturedContext: false);
		}
	}
}
