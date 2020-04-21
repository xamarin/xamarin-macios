extern alias SystemMSBuild;

namespace Microsoft.Build.Tasks
{
	public abstract class TouchBase : SystemMSBuild.Microsoft.Build.Tasks.Touch
	{
		public string SessionId { get; set; }
	}
}

