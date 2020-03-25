using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xharness.Execution;
using Xharness.Listeners;
using Xharness.Logging;

namespace Xharness {

	public interface ITestReporterFactory {
		ITestReporter Create (IAppRunner appRunner, 
							  string device,
							  ISimpleListener simpleListener,
							  ILog log,
							  ICrashSnapshotReporter crashReports,
							  IResultParser resultParser);
	}

	// interface that represents a class that know how to parse the results from an app run.
	public interface ITestReporter {

		TimeSpan Timeout { get; }
		ILog CallbackLog { get; }

		bool? Success { get; }
		Stopwatch TimeoutWatch { get; }
		CancellationToken CancellationToken { get; }

		void LaunchCallback (Task<bool> launchResult);

		Task CollectSimulatorResult (Task<ProcessExecutionResult> processExecution);
		Task CollectDeviceResult (Task<ProcessExecutionResult> processExecution);
		Task<(TestExecutingResult ExecutingResult, string FailureMessage)> ParseResult ();
	}
}
