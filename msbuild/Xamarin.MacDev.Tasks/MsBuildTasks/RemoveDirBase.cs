extern alias Microsoft_Build_Tasks_Core;

namespace Microsoft.Build.Tasks {
	public abstract class RemoveDirBase : Microsoft_Build_Tasks_Core::Microsoft.Build.Tasks.RemoveDir {
		public string SessionId { get; set; }
	}
}
