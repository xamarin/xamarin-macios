namespace Microsoft.Build.Tasks
{
	public abstract class CopyBase : Copy
	{
		public string SessionId { get; set; }
	}
}
