using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xharness.Logging;

namespace Xharness.Jenkins.TestTasks
{
	internal abstract class RunTestTask : TestTask
	{
		public readonly BuildToolTask BuildTask;
		public double TimeoutMultiplier { get; set; } = 1;

		public RunTestTask (BuildToolTask build_task)
		{
			this.BuildTask = build_task;

			Jenkins = build_task.Jenkins;
			TestProject = build_task.TestProject;
			Platform = build_task.Platform;
			ProjectPlatform = build_task.ProjectPlatform;
			ProjectConfiguration = build_task.ProjectConfiguration;
			if (build_task.HasCustomTestName)
				TestName = build_task.TestName;
		}

		public override IEnumerable<Log> AggregatedLogs {
			get {
				var rv = base.AggregatedLogs;
				if (BuildTask != null)
					rv = rv.Union (BuildTask.AggregatedLogs);
				return rv;
			}
		}

		public override TestExecutingResult ExecutionResult {
			get {
				// When building, the result is the build result.
				if ((BuildTask.ExecutionResult & (TestExecutingResult.InProgress | TestExecutingResult.Waiting)) != 0)
					return BuildTask.ExecutionResult & ~TestExecutingResult.InProgressMask | TestExecutingResult.Building;
				return base.ExecutionResult;
			}
			set {
				base.ExecutionResult = value;
			}
		}

		public async Task<bool> BuildAsync ()
		{
			if (Finished)
				return true;

			await VerifyBuildAsync ();
			if (Finished)
				return BuildTask.Succeeded;

			ExecutionResult = TestExecutingResult.Building;
			await BuildTask.RunAsync ();
			if (!BuildTask.Succeeded) {
				if (BuildTask.TimedOut) {
					ExecutionResult = TestExecutingResult.TimedOut;
				} else {
					ExecutionResult = TestExecutingResult.BuildFailure;
				}
				FailureMessage = BuildTask.FailureMessage;
				if (Harness.InCI && BuildTask is MSBuildTask projectTask)
					XmlResultParser.GenerateFailure (Logs, "build", projectTask.TestName, projectTask.Variation, "AppBuild", $"App could not be built {FailureMessage}.", projectTask.BuildLog.FullPath, Harness.XmlJargon);
			} else {
				ExecutionResult = TestExecutingResult.Built;
			}
			return BuildTask.Succeeded;
		}

		protected override async Task ExecuteAsync ()
		{
			if (Finished)
				return;

			await VerifyRunAsync ();
			if (Finished)
				return;

			if (!await BuildAsync ())
				return;

			if (BuildOnly) {
				ExecutionResult = TestExecutingResult.BuildSucceeded;
				return;
			}

			ExecutionResult = TestExecutingResult.Running;
			duration.Restart (); // don't count the build time.
			await RunTestAsync ();
		}

		protected abstract Task RunTestAsync ();
		// VerifyBuild is called in BuildAsync to verify that the task can be built.
		// Typically used to fail tasks if there's not enough disk space.
		public virtual Task VerifyBuildAsync ()
		{
			return VerifyDiskSpaceAsync ();
		}

		public override void Reset ()
		{
			base.Reset ();
			BuildTask.Reset ();
		}
	}
}
