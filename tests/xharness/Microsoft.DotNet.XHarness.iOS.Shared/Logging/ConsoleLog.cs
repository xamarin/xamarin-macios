using System;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Logging {
	// A log that writes to standard output
	public class ConsoleLog : CallbackLog {

		public ConsoleLog ()
			: base (Console.Write, "Console log")
		{
		}
	}
}
