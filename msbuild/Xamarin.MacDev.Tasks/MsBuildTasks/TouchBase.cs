extern alias Microsoft_Build_Tasks_Core;

namespace Microsoft.Build.Tasks {
	public abstract class TouchBase : Microsoft_Build_Tasks_Core::Microsoft.Build.Tasks.Touch {
		public string SessionId { get; set; }
	}
}
