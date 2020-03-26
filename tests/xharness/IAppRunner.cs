using System;
using Microsoft.DotNet.XHarness.iOS.Execution;
using Xharness.Jenkins.TestTasks;
using Microsoft.DotNet.XHarness.iOS.Logging;
using Microsoft.DotNet.XHarness.iOS;

namespace Xharness {
	// common interface that contains the basic info needed by the test result to be able to parse the results and 
	// log all the required data.
	public interface IAppRunner {
		IProcessManager ProcessManager { get; }
		AppBundleInformation AppInformation { get; }
		BuildToolTask BuildTask { get; }
		TimeSpan GetNewTimeout ();
		double LaunchTimeout { get; }
		ILogs Logs { get; }
		ILog MainLog { get; }
		RunMode RunMode { get; }
		XmlResultJargon XmlJargon { get; }
		void LogException (int minLevel, string message, params object [] args);
	}
}
