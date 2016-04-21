namespace Microsoft.Build.Tasks
{
	public abstract class MakeDirBase : Microsoft.Build.Tasks.MakeDir
	{
		public string SessionId { get; set; }
	}
}
