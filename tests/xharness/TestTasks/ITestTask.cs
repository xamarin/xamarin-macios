using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.TestTasks {
	public interface ITestTask {
		bool HasCustomTestName { get; }
		bool BuildSucceeded { get; }
		bool Succeeded { get; }
		bool Failed { get; }
		bool Ignored { get; set; }
		bool TimedOut { get; }

		string KnownFailure { get; set; }
		string ProjectConfiguration { get; set; }
		string ProjectPlatform { get; set; }
		string ProjectFile { get; }

		public string FailureMessage { get; set; }


		TimeSpan Duration { get; }
		Task InitialTask { get; set; }
		TestExecutingResult ExecutionResult { get; set; }
		IEnumerable<ILog> AggregatedLogs { get; }
		ILogs Logs { get; }


		Task RunAsync ();
		void Reset ();
	}
}
