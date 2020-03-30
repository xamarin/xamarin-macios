using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Xharness.Jenkins.TestTasks {
	public abstract class TestTask
	{
		static int counter;
		public readonly int ID = counter++;

		bool? supports_parallel_execution;

		public Jenkins Jenkins;
		public Harness Harness { get { return Jenkins.Harness; } }
		public TestProject TestProject;
		public string ProjectFile { get { return TestProject?.Path; } }
		public string ProjectConfiguration;
		public string ProjectPlatform;
		public Dictionary<string, string> Environment = new Dictionary<string, string> ();

		public Func<Task> Dependency; // a task that's feteched and awaited before this task's ExecuteAsync method
		public Task InitialTask; // a task that's executed before this task's ExecuteAsync method.
		public Task CompletedTask; // a task that's executed after this task's ExecuteAsync method.

		public bool BuildOnly;
		public string KnownFailure;

		// VerifyRun is called in RunInternalAsync/ExecuteAsync to verify that the task can be executed/run.
		// Typically used to fail tasks that don't have an available device, or if there's not enough disk space.
		public virtual Task VerifyRunAsync ()
		{
			return VerifyDiskSpaceAsync ();
		}

		static DriveInfo RootDrive;
		protected Task VerifyDiskSpaceAsync ()
		{
			if (Finished)
				return Task.CompletedTask;

			if (RootDrive == null)
				RootDrive = new DriveInfo ("/");
			var afs = RootDrive.AvailableFreeSpace;
			const long minSpaceRequirement = 1024 * 1024 * 1024; /* 1 GB */
			if (afs < minSpaceRequirement) {
				FailureMessage = $"Not enough space on the root drive '{RootDrive.Name}': {afs / (1024.0 * 1024):#.##} MB left of {minSpaceRequirement / (1024.0 * 1024):#.##} MB required";
				ExecutionResult = TestExecutingResult.Failed;
			}
			return Task.CompletedTask;
		}

		public void CloneTestProject (TestProject project)
		{
			// Don't build in the original project directory
			// We can build multiple projects in parallel, and if some of those
			// projects have the same project dependencies, then we may end up
			// building the same (dependent) project simultaneously (and they can
			// stomp on eachother).
			// So we clone the project file to a separate directory and build there instead.
			// This is done asynchronously to speed to the initial test load.
			TestProject = project.Clone ();
			InitialTask = TestProject.CreateCopyAsync ();
		}

		protected Stopwatch duration = new Stopwatch ();
		public TimeSpan Duration {
			get {
				return duration.Elapsed;
			}
		}

		protected Stopwatch waitingDuration = new Stopwatch ();
		public TimeSpan WaitingDuration => waitingDuration.Elapsed;

		TestExecutingResult execution_result;
		public virtual TestExecutingResult ExecutionResult {
			get {
				return execution_result;
			}
			set {
				execution_result = value;
			}
		}

		string failure_message;
		public string FailureMessage {
			get { return failure_message; }
			set {
				failure_message = value;
				MainLog.WriteLine (failure_message);
			}
		}

		public virtual string ProgressMessage { get; }

		public bool NotStarted { get { return (ExecutionResult & TestExecutingResult.StateMask) == TestExecutingResult.NotStarted; } }
		public bool InProgress { get { return (ExecutionResult & TestExecutingResult.InProgress) == TestExecutingResult.InProgress; } }
		public bool Waiting { get { return (ExecutionResult & TestExecutingResult.Waiting) == TestExecutingResult.Waiting; } }
		public bool Finished { get { return (ExecutionResult & TestExecutingResult.Finished) == TestExecutingResult.Finished; } }

		public bool Building { get { return (ExecutionResult & TestExecutingResult.Building) == TestExecutingResult.Building; } }
		public bool Built { get { return (ExecutionResult & TestExecutingResult.Built) == TestExecutingResult.Built; } }
		public bool Running { get { return (ExecutionResult & TestExecutingResult.Running) == TestExecutingResult.Running; } }

		public bool BuildSucceeded { get { return (ExecutionResult & TestExecutingResult.BuildSucceeded) == TestExecutingResult.BuildSucceeded; } }
		public bool Succeeded { get { return (ExecutionResult & TestExecutingResult.Succeeded) == TestExecutingResult.Succeeded; } }
		public bool Failed { get { return (ExecutionResult & TestExecutingResult.Failed) == TestExecutingResult.Failed; } }
		public bool Ignored {
			get { return ExecutionResult == TestExecutingResult.Ignored; }
			set {
				if (ExecutionResult != TestExecutingResult.NotStarted && ExecutionResult != TestExecutingResult.Ignored)
					throw new InvalidOperationException ();
				ExecutionResult = value ? TestExecutingResult.Ignored : TestExecutingResult.NotStarted;
			}
		}
		public bool DeviceNotFound { get { return ExecutionResult == TestExecutingResult.DeviceNotFound; } }

		public bool Crashed { get { return (ExecutionResult & TestExecutingResult.Crashed) == TestExecutingResult.Crashed; } }
		public bool TimedOut { get { return (ExecutionResult & TestExecutingResult.TimedOut) == TestExecutingResult.TimedOut; } }
		public bool BuildFailure { get { return (ExecutionResult & TestExecutingResult.BuildFailure) == TestExecutingResult.BuildFailure; } }
		public bool HarnessException { get { return (ExecutionResult & TestExecutingResult.HarnessException) == TestExecutingResult.HarnessException; } }

		public virtual string Mode { get; set; }
		public virtual string Variation { get; set; }

		protected static string Timestamp {
			get {
				return Helpers.Timestamp;
			}
		}

		public bool HasCustomTestName {
			get {
				return test_name != null;
			}
		}

		string test_name;
		public virtual string TestName {
			get {
				if (test_name != null)
					return test_name;

				var rv = Path.GetFileNameWithoutExtension (ProjectFile);
				if (rv == null)
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
					if (rv.EndsWith ("-watchos", StringComparison.Ordinal)) {
						return rv.Substring (0, rv.Length - 8);
					} else if (rv.EndsWith ("-tvos", StringComparison.Ordinal)) {
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
				test_name = value;
			}
		}

		public TestPlatform Platform { get; set; }

		public List<Resource> Resources = new List<Resource> ();

		ILog test_log;
		public ILog MainLog {
			get {
				if (test_log == null)
					test_log = Logs.Create ($"main-{Timestamp}.log", "Main log");
				return test_log;
			}
		}

		public virtual IEnumerable<ILog> AggregatedLogs {
			get {
				return Logs;
			}
		}

		public string LogDirectory {
			get {
				var rv = Path.Combine (Jenkins.LogDirectory, TestName, ID.ToString ());
				Directory.CreateDirectory (rv);
				return rv;
			}
		}

		ILogs logs;
		public ILogs Logs {
			get {
				return logs ?? (logs = new Logs (LogDirectory));
			}
		}

		IEnumerable<string> referencedNunitAndXunitTestAssemblies;
		public IEnumerable<string> ReferencedNunitAndXunitTestAssemblies {
			get {
				if (referencedNunitAndXunitTestAssemblies != null)
					return referencedNunitAndXunitTestAssemblies;

				if (TestName.Contains ("BCL tests group")) { // avoid loading unrelated projects
					if (!File.Exists (ProjectFile))
						return Enumerable.Empty<string> ();

					var csproj = new XmlDocument ();
					try {
						csproj.LoadWithoutNetworkAccess (ProjectFile.Replace ("\\", "/"));
						referencedNunitAndXunitTestAssemblies = csproj.GetNunitAndXunitTestReferences ();
					} catch (Exception e) {
						referencedNunitAndXunitTestAssemblies = new string [] { $"Exception: {e.Message}", $"Filename: {ProjectFile}" };
					}
				} else {
					referencedNunitAndXunitTestAssemblies = Enumerable.Empty<string> ();
				}
				return referencedNunitAndXunitTestAssemblies;
			}
		}

		Task execute_task;
		async Task RunInternalAsync ()
		{
			if (Finished)
				return;

			ExecutionResult = ExecutionResult & ~TestExecutingResult.StateMask | TestExecutingResult.InProgress;

			try {
				if (Dependency != null)
					await Dependency ();

				if (InitialTask != null)
					await InitialTask;

				await VerifyRunAsync ();
				if (Finished)
					return;

				duration.Start ();

				execute_task = ExecuteAsync ();
				await execute_task;

				if (CompletedTask != null) {
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
				duration.Stop ();
			}

			Jenkins.GenerateReport ();
		}

		protected virtual void PropagateResults ()
		{
		}

		public virtual void Reset ()
		{
			test_log = null;
			failure_message = null;
			logs = null;
			duration.Reset ();
			execution_result = TestExecutingResult.NotStarted;
			execute_task = null;
		}

		public Task RunAsync ()
		{
			if (execute_task == null)
				execute_task = RunInternalAsync ();
			return execute_task;
		}

		protected abstract Task ExecuteAsync ();

		public override string ToString ()
		{
			return ExecutionResult.ToString ();
		}

		protected void SetEnvironmentVariables (Process process)
		{
			var xcodeRoot = Harness.XcodeRoot;

			switch (Platform) {
			case TestPlatform.iOS:
			case TestPlatform.iOS_Unified:
			case TestPlatform.iOS_Unified32:
			case TestPlatform.iOS_Unified64:
			case TestPlatform.iOS_TodayExtension64:
			case TestPlatform.tvOS:
			case TestPlatform.watchOS:
			case TestPlatform.watchOS_32:
			case TestPlatform.watchOS_64_32:
				process.StartInfo.EnvironmentVariables ["MD_APPLE_SDK_ROOT"] = xcodeRoot;
				process.StartInfo.EnvironmentVariables ["MD_MTOUCH_SDK_ROOT"] = Path.Combine (Harness.IOS_DESTDIR, "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current");
				process.StartInfo.EnvironmentVariables ["TargetFrameworkFallbackSearchPaths"] = Path.Combine (Harness.IOS_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild-frameworks");
				process.StartInfo.EnvironmentVariables ["MSBuildExtensionsPathFallbackPathsOverride"] = Path.Combine (Harness.IOS_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild");
				break;
			case TestPlatform.Mac:
			case TestPlatform.Mac_Modern:
			case TestPlatform.Mac_Full:
			case TestPlatform.Mac_System:
				process.StartInfo.EnvironmentVariables ["MD_APPLE_SDK_ROOT"] = xcodeRoot;
				process.StartInfo.EnvironmentVariables ["TargetFrameworkFallbackSearchPaths"] = Path.Combine (Harness.MAC_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild-frameworks");
				process.StartInfo.EnvironmentVariables ["MSBuildExtensionsPathFallbackPathsOverride"] = Path.Combine (Harness.MAC_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild");
				process.StartInfo.EnvironmentVariables ["XamarinMacFrameworkRoot"] = Path.Combine (Harness.MAC_DESTDIR, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				process.StartInfo.EnvironmentVariables ["XAMMAC_FRAMEWORK_PATH"] = Path.Combine (Harness.MAC_DESTDIR, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				break;
			case TestPlatform.All:
				// Don't set:
				//     MSBuildExtensionsPath 
				//     TargetFrameworkFallbackSearchPaths
				// because these values used by both XM and XI and we can't set it to two different values at the same time.
				// Any test that depends on these values should not be using 'TestPlatform.All'
				process.StartInfo.EnvironmentVariables ["MD_APPLE_SDK_ROOT"] = xcodeRoot;
				process.StartInfo.EnvironmentVariables ["MD_MTOUCH_SDK_ROOT"] = Path.Combine (Harness.IOS_DESTDIR, "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current");
				process.StartInfo.EnvironmentVariables ["XamarinMacFrameworkRoot"] = Path.Combine (Harness.MAC_DESTDIR, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				process.StartInfo.EnvironmentVariables ["XAMMAC_FRAMEWORK_PATH"] = Path.Combine (Harness.MAC_DESTDIR, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				break;
			default:
				throw new NotImplementedException ();
			}

			foreach (var kvp in Environment)
				process.StartInfo.EnvironmentVariables [kvp.Key] = kvp.Value;
		}

		protected void AddWrenchLogFiles (StreamReader stream)
		{
			string line;
			while ((line = stream.ReadLine ()) != null) {
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
					Harness.HarnessLog.WriteLine ("Unknown @MonkeyWrench command in {0}: {1}", TestName, name);
					break;
				}
			}
		}

		protected void LogEvent (ILog log, string text, params object [] args)
		{
			Jenkins.MainLog.WriteLine (text, args);
			log.WriteLine (text, args);
		}

		public string GuessFailureReason (ILog log)
		{
			try {
				using (var reader = log.GetReader ()) {
					string line;
					var error_msg = new System.Text.RegularExpressions.Regex ("([A-Z][A-Z][0-9][0-9][0-9][0-9]:.*)");
					while ((line = reader.ReadLine ()) != null) {
						var match = error_msg.Match (line);
						if (match.Success)
							return match.Groups [1].Captures [0].Value;
					}
				}
			} catch (Exception e) {
				Harness.Log ("Failed to guess failure reason: {0}", e.Message);
			}

			return null;
		}

		// This method will set (and clear) the Waiting flag correctly while waiting on a resource
		// It will also pause the duration.
		public async Task<IAcquiredResource> NotifyBlockingWaitAsync (Task<IAcquiredResource> task)
		{
			var rv = new BlockingWait ();

			// Stop the timer while we're waiting for a resource
			duration.Stop ();
			waitingDuration.Start ();
			ExecutionResult = ExecutionResult | TestExecutingResult.Waiting;
			rv.Wrapped = await task;
			ExecutionResult = ExecutionResult & ~TestExecutingResult.Waiting;
			waitingDuration.Stop ();
			duration.Start ();
			rv.OnDispose = duration.Stop;
			return rv;
		}

		public virtual bool SupportsParallelExecution {
			get {
				return supports_parallel_execution ?? true;
			}
			set {
				supports_parallel_execution = value;
			}
		}

		protected Task<IAcquiredResource> NotifyAndAcquireDesktopResourceAsync ()
		{
			return NotifyBlockingWaitAsync (SupportsParallelExecution ? Jenkins.DesktopResource.AcquireConcurrentAsync () : Jenkins.DesktopResource.AcquireExclusiveAsync ());
		}

		class BlockingWait : IAcquiredResource, IDisposable
		{
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
