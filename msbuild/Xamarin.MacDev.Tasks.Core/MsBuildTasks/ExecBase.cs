extern alias SystemMSBuild;

namespace Microsoft.Build.Tasks
{
	public abstract class ExecBase : SystemMSBuild.Microsoft.Build.Tasks.Exec
	{
		public string SessionId { get; set; }
	}
}

