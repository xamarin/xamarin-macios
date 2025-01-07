using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Collections;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;

namespace Xharness.Jenkins.TestTasks {
	abstract class RunXITask<TDevice> : RunTestTask where TDevice : class, IDevice {
		public TestTarget AppRunnerTarget { get; set; }

		public AppRunner Runner { get; set; }
		public AppRunner AdditionalRunner { get; set; }

		public IEnumerable<TDevice> Candidates { get; }

		public TDevice Device { get; set; }

		public TDevice CompanionDevice { get; set; }

		public string BundleIdentifier => Runner.AppInformation.BundleIdentifier;

		public RunXITask (Jenkins jenkins, BuildToolTask build_task, IMlaunchProcessManager processManager, IEnumerable<TDevice> candidates)
			: base (jenkins, build_task, processManager)
		{
			this.Candidates = candidates;
		}

		public override IEnumerable<ILog> AggregatedLogs {
			get {
				var rv = base.AggregatedLogs;
				if (Runner is not null)
					rv = rv.Union (Runner.Logs);
				if (AdditionalRunner is not null)
					rv = rv.Union (AdditionalRunner.Logs);
				return rv;
			}
		}

		public override string Mode {
			get {

				switch (Platform) {
				case TestPlatform.tvOS:
					return Platform.ToString () + " - " + XIMode;
				case TestPlatform.iOS:
					return "iOS - " + XIMode;
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
			if (asyncEnumerable is not null)
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
			Runner = null;
			AdditionalRunner = null;
		}
	}
}
