namespace Microsoft.Build.Tasks
{
	public abstract class CopyBase : Microsoft.Build.Tasks.Copy
	{
		public string SessionId { get; set; }
	}
}
