namespace Microsoft.Build.Tasks
{
	public abstract class RemoveDirBase : RemoveDir
	{
		public string SessionId { get; set; }
	}
}
