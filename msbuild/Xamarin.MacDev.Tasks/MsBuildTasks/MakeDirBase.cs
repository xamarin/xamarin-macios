extern alias Microsoft_Build_Tasks_Core;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Microsoft.Build.Tasks {
	public abstract class MakeDirBase : Microsoft_Build_Tasks_Core::Microsoft.Build.Tasks.MakeDir {
		public string SessionId { get; set; }
	}
}
