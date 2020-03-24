using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xharness.Execution;
using Xharness.Listeners;
using Xharness.Logging;

namespace Xharness {

	public interface ITestReporterFactory {
		ITestReporter Create (IAppRunner appRunner, string device, ISimpleListener simpleListener, ILog log, ICrashSnapshotReporter crashReports);
	}

	// interface that represents a class that 
	public interface ITestReporter {

		TimeSpan Timeout { get; }
		IProcessManager ProcessManager { get; set; }
		IResultParser ResultParser { get; set; }
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
