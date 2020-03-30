namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {
	public static class WrenchLog {

		public static void WriteLine (string message, params object [] args)
		{
			WriteLine (string.Format (message, args));
		}

		public static void WriteLine (string message)
		{
			// Disable this for now, since we're not uploading directly to wrench anymore, but instead using the Html Report.
			//if (!InWrench)
			//	return;

			//Console.WriteLine ("@MonkeyWrench: " + message, args);
		}
	}
}
