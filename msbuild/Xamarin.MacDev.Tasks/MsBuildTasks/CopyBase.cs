extern alias Microsoft_Build_Tasks_Core;

namespace Microsoft.Build.Tasks {
	public abstract class CopyBase : Microsoft_Build_Tasks_Core::Microsoft.Build.Tasks.Copy {
		public string SessionId { get; set; }
	}
}
