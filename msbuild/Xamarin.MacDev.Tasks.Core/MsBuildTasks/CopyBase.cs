extern alias SystemMSBuild;

namespace Microsoft.Build.Tasks
{
	public abstract class CopyBase : SystemMSBuild.Microsoft.Build.Tasks.Copy
	{
		public string SessionId { get; set; }
	}
}

