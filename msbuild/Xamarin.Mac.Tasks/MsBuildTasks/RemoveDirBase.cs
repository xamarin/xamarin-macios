namespace Microsoft.Build.Tasks
{
	public abstract class RemoveDirBase : Microsoft.Build.Tasks.RemoveDir
	{
		public string SessionId { get; set; }
	}
}
