namespace Microsoft.Build.Tasks
{
	public abstract class TouchBase : Microsoft.Build.Tasks.Touch
	{
		public string SessionId { get; set; }
	}
}
