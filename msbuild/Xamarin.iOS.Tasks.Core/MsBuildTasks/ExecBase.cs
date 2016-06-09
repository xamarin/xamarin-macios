namespace Microsoft.Build.Tasks
{
    public abstract class ExecBase : Microsoft.Build.Tasks.Exec
    {
        public string SessionId { get; set; }
    }
}
