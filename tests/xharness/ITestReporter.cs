using System;
using System.Threading;
using System.Threading.Tasks;
using Xharness.Execution;
using Xharness.Listeners;
using Xharness.Logging;

namespace Xharness {

	public interface ITestReporterFactory {
		ITestReporter Create (ILog mainLog,
			ILog runLog,
			ILogs logs,
			ISimpleListener simpleListener,
			IResultParser parser,
			AppBundleInformation appInformation,
			RunMode runMode,
			XmlResultJargon xmlJargon,
			string device,
			TimeSpan timeout,
			double launchTimeout,
			string additionalLogsDirectory = null,
			Action<int, string> exceptionLogger = null);
	}

	// interface that represents a class that know how to parse the results from an app run.
	public interface ITestReporter {

		ILog CallbackLog { get; }

		bool? Success { get; }
		CancellationToken CancellationToken { get; }

		void LaunchCallback (Task<bool> launchResult);

		Task CollectSimulatorResult (Task<ProcessExecutionResult> processExecution);
		Task CollectDeviceResult (Task<ProcessExecutionResult> processExecution);
		Task<(TestExecutingResult ExecutingResult, string FailureMessage)> ParseResult ();
	}
}
