extern alias Microsoft_Build_Tasks_Core;

namespace Microsoft.Build.Tasks {
	public abstract class DeleteBase : Microsoft_Build_Tasks_Core::Microsoft.Build.Tasks.Delete {
		public string SessionId { get; set; }
	}
}
