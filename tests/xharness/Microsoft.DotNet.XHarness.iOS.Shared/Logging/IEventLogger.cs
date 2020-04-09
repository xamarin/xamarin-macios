namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {
	public interface IEventLogger {
		public void LogEvent (ILog log, string text, params object [] args);
	}
}
