extern alias SystemMSBuild;

namespace Microsoft.Build.Tasks
{
	public abstract class RemoveDirBase : SystemMSBuild.Microsoft.Build.Tasks.RemoveDir
	{
		public string SessionId { get; set; }
	}
}
