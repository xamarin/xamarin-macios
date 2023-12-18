extern alias Microsoft_Build_Tasks_Core;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Microsoft.Build.Tasks {
	public abstract class DeleteBase : Microsoft_Build_Tasks_Core::Microsoft.Build.Tasks.Delete {
		public string SessionId { get; set; }
	}
}
