using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.Jenkins.TestTasks {
	internal abstract class RunTestTask : AppleTestTask, IRunTestTask {
		protected RunTest runTest;
		public IMlaunchProcessManager ProcessManager => runTest.ProcessManager;
		public IBuildToolTask BuildTask => runTest.BuildTask;

		public double TimeoutMultiplier {
			get => runTest.TimeoutMultiplier;
			set => runTest.TimeoutMultiplier = value;
		}

		public string WorkingDirectory {
			get => runTest.WorkingDirectory;
			set => runTest.WorkingDirectory = value;
		}

		public TimeSpan Timeout {
			get => runTest.Timeout;
			set => runTest.Timeout = value;
		}

		public RunTestTask (Jenkins jenkins, IBuildToolTask build_task, IMlaunchProcessManager processManager) : base (jenkins)
		{
			runTest = new RunTest (
				testTask: this,
				buildTask: build_task,
				processManager: processManager,
				envManager: this,
				mainLog: Jenkins.MainLog,
				generateXmlFailures: Jenkins.Harness.InCI,
				xmlResultJargon: Jenkins.Harness.XmlJargon,
				dryRun: Jenkins.Harness.DryRun
			);

			TestProject = build_task.TestProject;
			Platform = build_task.Platform;
			ProjectPlatform = build_task.ProjectPlatform;
			ProjectConfiguration = build_task.ProjectConfiguration;
			if (build_task.HasCustomTestName)
				TestName = build_task.TestName;
		}

		public override IEnumerable<ILog> AggregatedLogs {
			get {
				var rv = base.AggregatedLogs;
				if (runTest.BuildAggregatedLogs is not null)
					rv = rv.Union (runTest.BuildAggregatedLogs);
				return rv;
			}
		}

		public override TestExecutingResult ExecutionResult {
			get {
				// If we're ignored, then build result doesn't matter.
				if (base.ExecutionResult == TestExecutingResult.Ignored)
					return TestExecutingResult.Ignored;

				// When building, the result is the build result.
				if ((runTest.BuildResult & (TestExecutingResult.InProgress | TestExecutingResult.Waiting)) != 0)
					return runTest.BuildResult & ~TestExecutingResult.InProgressMask | TestExecutingResult.Building;
				return base.ExecutionResult;
			}
			set {
				base.ExecutionResult = value;
			}
		}

		public Task<bool> BuildAsync () => runTest.BuildAsync ();

		protected override Task ExecuteAsync () => runTest.ExecuteAsync ();

		public abstract Task RunTestAsync ();

		// VerifyBuild is called in BuildAsync to verify that the task can be built.
		// Typically used to fail tasks if there's not enough disk space.
		public virtual Task VerifyBuildAsync ()
		{
			return VerifyDiskSpaceAsync ();
		}

		public override void Reset ()
		{
			base.Reset ();
			runTest.Reset ();
		}

		protected Task ExecuteProcessAsync (string filename, List<string> arguments)
		{
			return ExecuteProcessAsync (null, filename, arguments);
		}

		protected Task ExecuteProcessAsync (ILog log, string filename, List<string> arguments)
		{
			if (log is null)
				log = Logs.Create ($"execute-{Timestamp}.txt", LogType.ExecutionLog.ToString ());

			return runTest.ExecuteProcessAsync (log, filename, arguments);
		}
	}
}
