extern alias Microsoft_Build_Tasks_Core;

namespace Microsoft.Build.Tasks {
	public abstract class MoveTaskBase : Microsoft_Build_Tasks_Core::Microsoft.Build.Tasks.Move {
		public string SessionId { get; set; }
	}
}
