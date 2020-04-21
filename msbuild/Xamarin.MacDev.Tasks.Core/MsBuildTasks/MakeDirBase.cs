extern alias SystemMSBuild;

namespace Microsoft.Build.Tasks
{
	public abstract class MakeDirBase : SystemMSBuild.Microsoft.Build.Tasks.MakeDir
	{
		public string SessionId { get; set; }
	}
}

