extern alias SystemMSBuild;

namespace Microsoft.Build.Tasks
{
	public abstract class MoveTaskBase : SystemMSBuild.Microsoft.Build.Tasks.Move
	{
		public string SessionId { get; set; }
	}
}

