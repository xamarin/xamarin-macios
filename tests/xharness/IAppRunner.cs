using System;
using Xharness.Execution;
using Xharness.Jenkins.TestTasks;
using Xharness.Logging;

namespace Xharness {
	// common interface that contains the basic info needed by the test result to be able to parse the results and 
	// log all the required data.
	public interface IAppRunner {
		AppBundleInformation AppInformation { get; }
		BuildToolTask BuildTask { get; }
		TimeSpan GetNewTimeout ();
		double LaunchTimeout { get; }
		IProcessManager ProcessManager { get; }
		ILogs Logs { get; }
		ILog MainLog { get; }
		RunMode RunMode { get; }
		XmlResultJargon XmlJargon { get; }
		void LogException (int minLevel, string message, params object [] args);
	}
}
