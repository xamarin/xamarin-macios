extern alias Microsoft_Build_Tasks_Core;

using Xamarin.Messaging.Build.Client;

namespace Microsoft.Build.Tasks {
	public class WriteLinesToFile : Microsoft_Build_Tasks_Core::Microsoft.Build.Tasks.WriteLinesToFile {
		public string SessionId { get; set; } = string.Empty;
		public override bool Execute ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}
	}
}
