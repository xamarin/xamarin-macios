using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Collections;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Xharness.Jenkins.TestTasks {
	abstract class RunXITask<TDevice> : RunTestTask where TDevice : class, IDevice
	{
		public TestTarget AppRunnerTarget;

		protected AppRunner runner;
		protected AppRunner additional_runner;

		public IEnumerable<TDevice> Candidates { get; }

		public TDevice Device { get; protected set; }

		public TDevice CompanionDevice { get; protected set; }

		public string BundleIdentifier => runner.AppInformation.BundleIdentifier;

		public RunXITask (BuildToolTask build_task, IProcessManager processManager, IEnumerable<TDevice> candidates)
			: base (build_task, processManager)
		{
			this.Candidates = candidates;
		}

		public override IEnumerable<ILog> AggregatedLogs {
			get {
				var rv = base.AggregatedLogs;
				if (runner != null)
					rv = rv.Union (runner.Logs);
				if (additional_runner != null)
					rv = rv.Union (additional_runner.Logs);
				return rv;
			}
		}

		public override string Mode {
			get {

				switch (Platform) {
				case TestPlatform.tvOS:
				case TestPlatform.watchOS:
					return Platform.ToString () + " - " + XIMode;
				case TestPlatform.watchOS_32:
					return "watchOS 32-bits - " + XIMode;
				case TestPlatform.watchOS_64_32:
					return "watchOS 64-bits (ARM64_32) - " + XIMode;
				case TestPlatform.iOS_Unified32:
					return "iOS Unified 32-bits - " + XIMode;
				case TestPlatform.iOS_Unified64:
					return "iOS Unified 64-bits - " + XIMode;
				case TestPlatform.iOS_TodayExtension64:
					return "iOS Unified Today Extension 64-bits - " + XIMode;
				case TestPlatform.iOS_Unified:
					return "iOS Unified - " + XIMode;
				default:
					throw new NotImplementedException ();
				}
			}
			set { throw new NotImplementedException (); }
		}

		public override async Task VerifyRunAsync ()
		{
			await base.VerifyRunAsync ();
			if (Finished)
				return;

			var enumerable = Candidates;
			var asyncEnumerable = enumerable as IAsyncEnumerable;
			if (asyncEnumerable != null)
				await asyncEnumerable.ReadyTask;
			if (!enumerable.Any ()) {
				ExecutionResult = TestExecutingResult.DeviceNotFound;
				FailureMessage = "No applicable devices found.";
			}
		}

		protected abstract string XIMode { get; }

		public override void Reset ()
		{
			base.Reset ();
			runner = null;
			additional_runner = null;
		}
	}
}
