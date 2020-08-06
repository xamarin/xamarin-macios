using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.TestTasks {
	public class RunTest {
		public IProcessManager ProcessManager { get; private set; }
		public IBuildToolTask BuildTask { get; private set; }
		IResultParser ResultParser { get; } = new XmlResultParser ();

		readonly IRunTestTask testTask;
		readonly IEnvManager envManager;
		readonly ILog mainLog;
		readonly bool generateXmlFailures;
		readonly bool dryRun;
		readonly XmlResultJargon xmlResultJargon;

		public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes (10);
		public double TimeoutMultiplier { get; set; } = 1;
		public string WorkingDirectory;

		public RunTest (IRunTestTask testTask, 
						IBuildToolTask buildTask,
						IProcessManager processManager,
						IEnvManager envManager,
						ILog mainLog,
						bool generateXmlFailures,
						XmlResultJargon xmlResultJargon, bool dryRun)
		{
			this.testTask = testTask ?? throw new ArgumentNullException (nameof (testTask));
			this.BuildTask = buildTask ?? throw new ArgumentNullException (nameof (buildTask));
			this.ProcessManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.envManager = envManager ?? throw new ArgumentNullException (nameof (envManager));
			this.mainLog = mainLog ?? throw new ArgumentNullException (nameof (mainLog));
			this.generateXmlFailures = generateXmlFailures;
			this.dryRun = dryRun;
			this.xmlResultJargon = xmlResultJargon;
		}

		public IEnumerable<ILog> BuildAggregatedLogs => BuildTask.AggregatedLogs;
		public TestExecutingResult BuildResult => BuildTask.ExecutionResult;

		public async Task<bool> BuildAsync ()
		{
			if (testTask.Finished)
				return true;

			await testTask.VerifyBuildAsync ();
			if (testTask.Finished)
				return BuildTask.Succeeded;

			testTask.ExecutionResult = TestExecutingResult.Building;
			await BuildTask.RunAsync ();
			if (!BuildTask.Succeeded) {
				if (BuildTask.TimedOut) {
					testTask.ExecutionResult = TestExecutingResult.TimedOut;
				} else {
					testTask.ExecutionResult = TestExecutingResult.BuildFailure;
				}
				testTask.FailureMessage = BuildTask.FailureMessage;
				if (!string.IsNullOrEmpty (BuildTask.KnownFailure))
					testTask.KnownFailure = BuildTask.KnownFailure;
				if (generateXmlFailures)
					ResultParser.GenerateFailure (
						logs: testTask.Logs, 
						source: "build",
						appName: testTask.TestName,
						variation: testTask.Variation,
						title: $"App Build {testTask.TestName} {testTask.Variation}",
						message: $"App could not be built {testTask.FailureMessage}.",
						stderrPath: BuildTask.BuildLog.FullPath,
						jargon: xmlResultJargon);
			} else {
				testTask.ExecutionResult = TestExecutingResult.Built;
			}
			return BuildTask.Succeeded;
		}

		public async Task ExecuteAsync ()
		{
			if (testTask.Finished)
				return;

			await testTask.VerifyRunAsync ();
			if (testTask.Finished)
				return;

			if (!await BuildAsync ())
				return;

			if (testTask.BuildOnly) {
				testTask.ExecutionResult = TestExecutingResult.BuildSucceeded;
				return;
			}

			testTask.ExecutionResult = TestExecutingResult.Running;
			testTask.DurationStopWatch.Restart (); // don't count the build time.
			await testTask.RunTestAsync ();
		}

		public void Reset () => BuildTask.Reset ();

		public async Task ExecuteProcessAsync (ILog log, string filename, List<string> arguments)
		{
			if (log == null)
				throw new ArgumentNullException (nameof (log));

			using var proc = new Process ();
			proc.StartInfo.FileName = filename;
			proc.StartInfo.Arguments = StringUtils.FormatArguments (arguments);
			if (!string.IsNullOrEmpty (WorkingDirectory))
				proc.StartInfo.WorkingDirectory = WorkingDirectory;
			envManager.SetEnvironmentVariables (proc);
			foreach (DictionaryEntry de in proc.StartInfo.EnvironmentVariables)
				log.WriteLine ($"export {de.Key}={de.Value}");
			mainLog.WriteLine ("Executing {0} ({1})", testTask.TestName, testTask.Mode);
			if (!dryRun) {
				testTask.ExecutionResult = TestExecutingResult.Running;
				var result = await ProcessManager.RunAsync (proc, log, Timeout);
				if (result.TimedOut) {
					testTask.FailureMessage = $"Execution timed out after {Timeout.TotalMinutes} minutes.";
					log.WriteLine (testTask.FailureMessage);
					testTask.ExecutionResult = TestExecutingResult.TimedOut;
				} else if (result.Succeeded) {
					testTask.ExecutionResult = TestExecutingResult.Succeeded;
				} else {
					testTask.ExecutionResult = TestExecutingResult.Failed;
					testTask.FailureMessage = $"Execution failed with exit code {result.ExitCode}";
				}
			}
			mainLog.WriteLine ("Executed {0} ({1})", testTask.TestName, testTask.Mode);
		}
	}
}
