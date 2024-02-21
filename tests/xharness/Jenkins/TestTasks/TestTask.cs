using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Jenkins.TestTasks {
	public abstract class TestTasks : IEnvManager, IEventLogger, ITestTask {
		static int counter;
		static DriveInfo rootDrive;

		protected readonly Stopwatch waitingDuration = new ();

		#region Private vars

		ILog testLog;
		bool? supportsParallelExecution;
		string testName;
		Task executeTask;

		#endregion

		#region Public vars

		public Dictionary<string, string> Environment = new ();
		public Func<Task> Dependency; // a task that's feteched and awaited before this task's ExecuteAsync method
		public Task InitialTask { get; set; } // a task that's executed before this task's ExecuteAsync method.
		public Task CompletedTask; // a task that's executed after this task's ExecuteAsync method.
		public List<Resource> Resources = new ();

		#endregion

		#region Properties

		public int ID { get; private set; }
		public bool BuildOnly { get; set; }
		public KnownIssue KnownFailure { get; set; }
		public string ProjectConfiguration { get; set; }
		public string ProjectPlatform { get; set; }

		protected static string Timestamp => Harness.Helpers.Timestamp;
		public string ProjectFile => TestProject?.Path;
		public bool HasCustomTestName => testName is not null;

		public bool NotStarted => (ExecutionResult & TestExecutingResult.StateMask) == TestExecutingResult.NotStarted;
		public bool InProgress => (ExecutionResult & TestExecutingResult.InProgress) == TestExecutingResult.InProgress;
		public bool Waiting => (ExecutionResult & TestExecutingResult.Waiting) == TestExecutingResult.Waiting;
		public bool Finished => (ExecutionResult & TestExecutingResult.Finished) == TestExecutingResult.Finished;

		public bool Building => (ExecutionResult & TestExecutingResult.Building) == TestExecutingResult.Building;
		public bool Built => (ExecutionResult & TestExecutingResult.Built) == TestExecutingResult.Built;
		public bool Running => (ExecutionResult & TestExecutingResult.Running) == TestExecutingResult.Running;

		public bool BuildSucceeded => (ExecutionResult & TestExecutingResult.BuildSucceeded) == TestExecutingResult.BuildSucceeded;
		public bool Succeeded => (ExecutionResult & TestExecutingResult.Succeeded) == TestExecutingResult.Succeeded;
		public bool Failed => (ExecutionResult & TestExecutingResult.Failed) == TestExecutingResult.Failed;
		public bool Ignored {
			get => ExecutionResult == TestExecutingResult.Ignored;
			set {
				if (ExecutionResult != TestExecutingResult.NotStarted && ExecutionResult != TestExecutingResult.Ignored)
					throw new InvalidOperationException ();
				ExecutionResult = value ? TestExecutingResult.Ignored : TestExecutingResult.NotStarted;
			}
		}
		public bool DeviceNotFound => ExecutionResult == TestExecutingResult.DeviceNotFound;

		public bool Crashed => (ExecutionResult & TestExecutingResult.Crashed) == TestExecutingResult.Crashed;
		public bool TimedOut => (ExecutionResult & TestExecutingResult.TimedOut) == TestExecutingResult.TimedOut;
		public bool BuildFailure => (ExecutionResult & TestExecutingResult.BuildFailure) == TestExecutingResult.BuildFailure;
		public bool HarnessException => (ExecutionResult & TestExecutingResult.HarnessException) == TestExecutingResult.HarnessException;

		public Stopwatch DurationStopWatch { get; } = new ();
		public TimeSpan Duration => DurationStopWatch.Elapsed;

		string failureMessage;
		public string FailureMessage {
			get { return failureMessage; }
			set {
				failureMessage = value;
				MainLog.WriteLine (failureMessage);
			}
		}

		public ILog MainLog
			=> testLog ??= Logs.Create ($"main-{Timestamp}.log", "Main log");

		ILogs logs;
		public ILogs Logs => logs ??= new Logs (LogDirectory);

		IEnumerable<string> referencedNunitAndXunitTestAssemblies;
		public IEnumerable<string> ReferencedNunitAndXunitTestAssemblies {
			get {
				if (referencedNunitAndXunitTestAssemblies is not null)
					return referencedNunitAndXunitTestAssemblies;

				if (TestName.Contains ("BCL tests group")) { // avoid loading unrelated projects
					if (!File.Exists (ProjectFile))
						return Enumerable.Empty<string> ();

					var csproj = new XmlDocument ();
					try {
						csproj.LoadWithoutNetworkAccess (ProjectFile.Replace ("\\", "/"));
						referencedNunitAndXunitTestAssemblies = csproj.GetNunitAndXunitTestReferences ();
					} catch (Exception e) {
						referencedNunitAndXunitTestAssemblies = new [] { $"Exception: {e.Message}", $"Filename: {ProjectFile}" };
					}
				} else {
					referencedNunitAndXunitTestAssemblies = Enumerable.Empty<string> ();
				}
				return referencedNunitAndXunitTestAssemblies;
			}
		}

		#endregion

		#region Abstract

		public abstract string RootDirectory { get; }
		public abstract IResourceManager ResourceManager { get; }
		public abstract void GenerateReport ();
		public abstract string LogDirectory { get; }
		public abstract void SetEnvironmentVariables (Process process);
		protected abstract Task ExecuteAsync ();
		protected abstract Task<IAcquiredResource> NotifyAndAcquireDesktopResourceAsync ();
		protected abstract void WriteLineToRunnerLog (string message);

		#endregion

		#region Virtual

		public virtual TestProject TestProject { get; set; }
		public virtual TestPlatform Platform { get; set; }
		public virtual string ProgressMessage { get; }
		public virtual string Mode { get; set; }
		public virtual string Variation { get; set; }

		public virtual bool SupportsParallelExecution {
			get => supportsParallelExecution ?? true;
			set => supportsParallelExecution = value;
		}

		public virtual IEnumerable<ILog> AggregatedLogs => Logs;

		TestExecutingResult executionResult;
		public virtual TestExecutingResult ExecutionResult {
			get => executionResult;
			set => executionResult = value;
		}

		public virtual string TestName {
			get {
				if (testName is not null)
					return testName;

				var rv = Path.GetFileNameWithoutExtension (ProjectFile);
				if (rv is null)
					return $"unknown test name ({GetType ().Name}";
				switch (Platform) {
				case TestPlatform.Mac:
					return rv;
				case TestPlatform.Mac_Modern:
					return rv;//.Substring (0, rv.Length - "-unified".Length);
				case TestPlatform.Mac_Full:
					return rv.Substring (0, rv.Length - "-full".Length);
				case TestPlatform.Mac_System:
					return rv.Substring (0, rv.Length - "-system".Length);
				default:
					if (rv.EndsWith ("-watchos", StringComparison.Ordinal)) return rv.Substring (0, rv.Length - 8);
					else if (rv.EndsWith ("-tvos", StringComparison.Ordinal)) {
						return rv.Substring (0, rv.Length - 5);
					} else if (rv.EndsWith ("-unified", StringComparison.Ordinal)) {
						return rv.Substring (0, rv.Length - 8);
					} else if (rv.EndsWith ("-today", StringComparison.Ordinal)) {
						return rv.Substring (0, rv.Length - 6);
					} else {
						return rv;
					}
				}
			}
			set {
				testName = value;
			}
		}

		protected virtual void PropagateResults () { }

		public virtual void LogEvent (ILog log, string text, params object [] args) => log.WriteLine (text, args);

		public virtual void Reset ()
		{
			testLog = null;
			failureMessage = null;
			logs = null;
			DurationStopWatch.Reset ();
			executionResult = TestExecutingResult.NotStarted;
			executeTask = null;
		}

		#endregion

		public TestTasks ()
		{
			ID = Interlocked.Increment (ref counter);
		}

		// VerifyRun is called in RunInternalAsync/ExecuteAsync to verify that the task can be executed/run.
		// Typically used to fail tasks that don't have an available device, or if there's not enough disk space.
		public virtual Task VerifyRunAsync () => VerifyDiskSpaceAsync ();

		protected Task VerifyDiskSpaceAsync ()
		{
			if (Finished)
				return Task.CompletedTask;

			rootDrive ??= new ("/");
			var afs = rootDrive.AvailableFreeSpace;
			const long minSpaceRequirement = 1024 * 1024 * 1024; /* 1 GB */
			if (afs < minSpaceRequirement) {
				FailureMessage = $"Not enough space on the root drive '{rootDrive.Name}': {afs / (1024.0 * 1024):#.##} MB left of {minSpaceRequirement / (1024.0 * 1024):#.##} MB required";
				ExecutionResult = TestExecutingResult.Failed;
			}
			return Task.CompletedTask;
		}

		public void CloneTestProject (ILog log, IProcessManager processManager, TestProject project, string rootDirectory)
		{
			// Don't build in the original project directory
			// We can build multiple projects in parallel, and if some of those
			// projects have the same project dependencies, then we may end up
			// building the same (dependent) project simultaneously (and they can
			// stomp on eachother).
			// So we clone the project file to a separate directory and build there instead.
			// This is done asynchronously to speed to the initial test load.
			TestProject = project.Clone ();
			InitialTask = TestProject.CreateCopyAsync (log, processManager, this, rootDirectory);
		}

		public TimeSpan WaitingDuration => waitingDuration.Elapsed;

		async Task RunInternalAsync ()
		{
			if (Finished)
				return;

			ExecutionResult = ExecutionResult & ~TestExecutingResult.StateMask | TestExecutingResult.InProgress;

			try {
				if (Dependency is not null)
					await Dependency ();

				if (InitialTask is not null)
					await InitialTask;

				await VerifyRunAsync ();
				if (Finished)
					return;

				DurationStopWatch.Start ();

				executeTask = ExecuteAsync ();
				await executeTask;

				if (CompletedTask is not null) {
					if (CompletedTask.Status == TaskStatus.Created)
						CompletedTask.Start ();
					await CompletedTask;
				}

				ExecutionResult = ExecutionResult & ~TestExecutingResult.StateMask | TestExecutingResult.Finished;
				if ((ExecutionResult & ~TestExecutingResult.StateMask) == 0)
					throw new Exception ("Result not set!");
			} catch (Exception e) {
				using (var log = Logs.Create ($"execution-failure-{Timestamp}.log", "Execution failure")) {
					ExecutionResult = TestExecutingResult.HarnessException;
					FailureMessage = $"Harness exception for '{TestName}': {e}";
					log.WriteLine (FailureMessage);
				}
				PropagateResults ();
			} finally {
				logs?.Dispose ();
				DurationStopWatch.Stop ();
			}

			GenerateReport ();
		}

		public Task RunAsync ()
			=> executeTask ??= RunInternalAsync ();

		public override string ToString ()
			=> ExecutionResult.ToString ();

		protected void AddCILogFiles (StreamReader stream)
		{
			string line;
			while ((line = stream.ReadLine ()) is not null) {
				if (!line.StartsWith ("@MonkeyWrench: ", StringComparison.Ordinal))
					continue;

				var cmd = line.Substring ("@MonkeyWrench:".Length).TrimStart ();
				var colon = cmd.IndexOf (':');
				if (colon <= 0)
					continue;
				var name = cmd.Substring (0, colon);
				switch (name) {
				case "AddFile":
					var src = cmd.Substring (name.Length + 1).Trim ();
					Logs.AddFile (src);
					break;
				default:
					WriteLineToRunnerLog ($"Unknown @MonkeyWrench command in {TestName}: {name}");
					break;
				}
			}
		}

		public string GuessFailureReason (IReadableLog log)
		{
			try {
				using (var reader = log.GetReader ()) {
					string line;
					var error_msg = new System.Text.RegularExpressions.Regex ("([A-Z][A-Z][0-9][0-9][0-9][0-9]:.*)");
					while ((line = reader.ReadLine ()) is not null) {
						var match = error_msg.Match (line);
						if (match.Success)
							return match.Groups [1].Captures [0].Value;
					}
				}
			} catch (Exception e) {
				WriteLineToRunnerLog ($"Failed to guess failure reason: {e.Message}");
			}

			return null;
		}

		// This method will set (and clear) the Waiting flag correctly while waiting on a resource
		// It will also pause the duration.
		public async Task<IAcquiredResource> NotifyBlockingWaitAsync (Task<IAcquiredResource> task)
		{
			var rv = new BlockingWait ();

			// Stop the timer while we're waiting for a resource
			DurationStopWatch.Stop ();
			waitingDuration.Start ();
			ExecutionResult = ExecutionResult | TestExecutingResult.Waiting;
			rv.Wrapped = await task;
			ExecutionResult = ExecutionResult & ~TestExecutingResult.Waiting;
			waitingDuration.Stop ();
			DurationStopWatch.Start ();
			rv.OnDispose = DurationStopWatch.Stop;
			return rv;
		}

		class BlockingWait : IAcquiredResource {
			public IAcquiredResource Wrapped;
			public Action OnDispose;

			public Resource Resource { get { return Wrapped.Resource; } }

			public void Dispose ()
			{
				OnDispose ();
				Wrapped.Dispose ();
			}
		}
	}
}
