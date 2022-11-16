extern alias Microsoft_Build_Tasks_Core;

namespace Microsoft.Build.Tasks {
	public abstract class MakeDirBase : Microsoft_Build_Tasks_Core::Microsoft.Build.Tasks.MakeDir {
		public string SessionId { get; set; }
	}
}
