using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.Jenkins.TestTasks {
	public interface ITestTask {

		#region Status properties

		bool NotStarted { get; }
		bool Building { get; }
		bool BuildSucceeded { get; }
		bool BuildFailure { get; }
		bool Waiting { get; }
		bool InProgress { get; }
		bool Running { get; }
		bool Finished { get; }
		bool HarnessException { get; }
		bool Built { get; }

		bool Succeeded { get; }
		bool Failed { get; }
		bool TimedOut { get; }
		bool Crashed { get; }
		bool DeviceNotFound { get; }

		public TimeSpan WaitingDuration { get; }

		#endregion

		bool HasCustomTestName { get; }
		bool BuildOnly { get; set; }
		bool Ignored { get; set; }

		KnownIssue KnownFailure { get; set; }
		string ProjectConfiguration { get; set; }
		string ProjectPlatform { get; set; }
		string ProjectFile { get; }
		string Mode { get; set; }
		string Variation { get; set; }
		string TestName { get; }
		string FailureMessage { get; set; }
		string LogDirectory { get; }

		public int ID { get; }


		TimeSpan Duration { get; }
		TestPlatform Platform { get; set; }
		Task InitialTask { get; set; }
		TestExecutingResult ExecutionResult { get; set; }
		IEnumerable<ILog> AggregatedLogs { get; }
		ILogs Logs { get; }
		Stopwatch DurationStopWatch { get; }
		IEnumerable<string> ReferencedNunitAndXunitTestAssemblies { get; }
		string ProgressMessage { get; }
		string RootDirectory { get; }


		string GuessFailureReason (IReadableLog log);
		Task RunAsync ();
		Task VerifyRunAsync ();
		void Reset ();
		Task<IAcquiredResource> NotifyBlockingWaitAsync (Task<IAcquiredResource> task);
	}
}
