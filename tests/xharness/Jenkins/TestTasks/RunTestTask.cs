using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Jenkins.TestTasks {
	internal abstract class RunTestTask : TestTask
	{
		protected IProcessManager ProcessManager { get; }
		IResultParser ResultParser { get; } = new XmlResultParser ();

		public readonly BuildToolTask BuildTask;
		public TimeSpan Timeout = TimeSpan.FromMinutes (10);
		public double TimeoutMultiplier { get; set; } = 1;
		public string WorkingDirectory;

		public RunTestTask (BuildToolTask build_task, IProcessManager processManager)
		{
			this.BuildTask = build_task;
			this.ProcessManager = processManager ?? throw new ArgumentNullException (nameof (processManager));

			Jenkins = build_task.Jenkins;
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
				if (!string.IsNullOrEmpty (BuildTask.KnownFailure))
					KnownFailure = BuildTask.KnownFailure;
				if (Harness.InCI && BuildTask is MSBuildTask projectTask)
					ResultParser.GenerateFailure (Logs, "build", projectTask.TestName, projectTask.Variation, $"App Build {projectTask.TestName} {projectTask.Variation}", $"App could not be built {FailureMessage}.", projectTask.BuildLog.FullPath, Harness.XmlJargon);
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

		protected Task ExecuteProcessAsync (string filename, List<string> arguments)
		{ 
			return ExecuteProcessAsync (null, filename, arguments);
		}

		protected async Task ExecuteProcessAsync (ILog log, string filename, List<string> arguments)
		{
			if (log == null)
				log = Logs.Create ($"execute-{Timestamp}.txt", LogType.ExecutionLog.ToString ());

			using var proc = new Process ();
			proc.StartInfo.FileName = filename;
			proc.StartInfo.Arguments = StringUtils.FormatArguments (arguments);
			if (!string.IsNullOrEmpty (WorkingDirectory))
				proc.StartInfo.WorkingDirectory = WorkingDirectory;
			SetEnvironmentVariables (proc);
			foreach (DictionaryEntry de in proc.StartInfo.EnvironmentVariables)
				log.WriteLine ($"export {de.Key}={de.Value}");
			Jenkins.MainLog.WriteLine ("Executing {0} ({1})", TestName, Mode);
			if (!Harness.DryRun) {
				ExecutionResult = TestExecutingResult.Running;
				var result = await ProcessManager.RunAsync (proc, log, Timeout);
				if (result.TimedOut) {
					FailureMessage = $"Execution timed out after {Timeout.TotalMinutes} minutes.";
					log.WriteLine (FailureMessage);
					ExecutionResult = TestExecutingResult.TimedOut;
				} else if (result.Succeeded) {
					ExecutionResult = TestExecutingResult.Succeeded;
				} else {
					ExecutionResult = TestExecutingResult.Failed;
					FailureMessage = $"Execution failed with exit code {result.ExitCode}";
				}
			}
			Jenkins.MainLog.WriteLine ("Executed {0} ({1})", TestName, Mode);
		}
	}
}
