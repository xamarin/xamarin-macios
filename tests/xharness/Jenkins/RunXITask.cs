using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xharness.Logging;
using Xharness;

namespace xharness.Jenkins
{
    abstract class RunXITask<TDevice> : RunTestTask where TDevice : class, IDevice
	{
		IEnumerable<TDevice> candidates;
		TDevice device;
		TDevice companion_device;
		public AppRunnerTarget AppRunnerTarget;

		protected AppRunner runner;
		protected AppRunner additional_runner;

		public IEnumerable<TDevice> Candidates => candidates;

		public TDevice Device
		{
			get { return device; }
			protected set { device = value; }
		}

		public TDevice CompanionDevice
		{
			get { return companion_device; }
			protected set { companion_device = value; }
		}

		public string BundleIdentifier
		{
			get { return runner.BundleIdentifier; }
		}

		public RunXITask(BuildToolTask build_task, IEnumerable<TDevice> candidates)
			: base(build_task)
		{
			this.candidates = candidates;
		}

		public override IEnumerable<Log> AggregatedLogs
		{
			get
			{
				var rv = base.AggregatedLogs;
				if (runner != null)
					rv = rv.Union(runner.Logs);
				if (additional_runner != null)
					rv = rv.Union(additional_runner.Logs);
				return rv;
			}
		}

		public override string Mode
		{
			get
			{

				switch (Platform)
				{
					case TestPlatform.tvOS:
					case TestPlatform.watchOS:
						return Platform.ToString() + " - " + XIMode;
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
						throw new NotImplementedException();
				}
			}
			set { throw new NotImplementedException(); }
		}

		public override async Task VerifyRunAsync()
		{
			await base.VerifyRunAsync();
			if (Finished)
				return;

			var enumerable = candidates;
			var asyncEnumerable = enumerable as IAsyncEnumerable;
			if (asyncEnumerable != null)
				await asyncEnumerable.ReadyTask;
			if (!enumerable.Any())
			{
				ExecutionResult = TestExecutingResult.DeviceNotFound;
				FailureMessage = "No applicable devices found.";
			}
		}

		protected abstract string XIMode { get; }

		public override void Reset()
		{
			base.Reset();
			runner = null;
			additional_runner = null;
		}
	}
}
