extern alias Microsoft_Build_Tasks_Core;

namespace Microsoft.Build.Tasks {
	public abstract class WriteLinesToFileBase : Microsoft_Build_Tasks_Core::Microsoft.Build.Tasks.WriteLinesToFile {
		public string SessionId { get; set; }
	}
}
