using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace xharness
{
	public class Jenkins
	{
		public Harness Harness;
		public bool IncludeClassic;

		public string LogDirectory {
			get {
				return Path.Combine (Harness.JENKINS_RESULTS_DIRECTORY, "tests");
			}
		}
		public Simulators Simulators = new Simulators ();

		List<TestTask> Tasks = new List<TestTask> ();

		internal static Resource DesktopResource = new Resource { Name = "Desktop" };

		async Task<IEnumerable<RunSimulatorTask>> CreateRunSimulatorTaskAsync (XBuildTask buildTask)
		{
			var runtasks = new List<RunSimulatorTask> ();

			Simulators.Harness = Harness;
			await Simulators.LoadAsync ();

			var fn = Path.GetFileNameWithoutExtension (buildTask.ProjectFile);
			if (fn.EndsWith ("-tvos", StringComparison.Ordinal)) {
				var latesttvOSRuntime =
					Simulators.SupportedRuntimes.
							  Where ((SimRuntime v) => v.Identifier.StartsWith ("com.apple.CoreSimulator.SimRuntime.tvOS-", StringComparison.Ordinal)).
							  OrderBy ((SimRuntime v) => v.Version).
							  Last ();
				var tvOSDeviceType =
					Simulators.SupportedDeviceTypes.
							  Where ((SimDeviceType v) => v.ProductFamilyId == "TV").
							  First ();
				var device =
					Simulators.AvailableDevices.
							  Where ((SimDevice v) => v.SimRuntime == latesttvOSRuntime.Identifier && v.SimDeviceType == tvOSDeviceType.Identifier).
							  First ();
				runtasks.Add (new RunSimulatorTask (buildTask, device));
			} else if (fn.EndsWith ("-watchos", StringComparison.Ordinal)) {
				var latestwatchOSRuntime =
					Simulators.SupportedRuntimes.
							  Where ((SimRuntime v) => v.Identifier.StartsWith ("com.apple.CoreSimulator.SimRuntime.watchOS-", StringComparison.Ordinal)).
							  OrderBy ((SimRuntime v) => v.Version).
							  Last ();
				var watchOSDeviceType =
					Simulators.SupportedDeviceTypes.
							  Where ((SimDeviceType v) => v.ProductFamilyId == "Watch").
							  First ();
				var device =
					Simulators.AvailableDevices.
							  Where ((SimDevice v) => v.SimRuntime == latestwatchOSRuntime.Identifier && v.SimDeviceType == watchOSDeviceType.Identifier).
							  Where ((SimDevice v) => Simulators.AvailableDevicePairs.Any ((pair) => pair.Gizmo == v.UDID)). // filter to watch devices that exists in a device pair
							  First ();
				runtasks.Add (new RunSimulatorTask (buildTask, device));
			} else {
				var latestiOSRuntime =
					Simulators.SupportedRuntimes.
							  Where ((SimRuntime v) => v.Identifier.StartsWith ("com.apple.CoreSimulator.SimRuntime.iOS-", StringComparison.Ordinal)).
							  OrderBy ((SimRuntime v) => v.Version).
							  Last ();

				if (fn.EndsWith ("-unified", StringComparison.Ordinal)) {
					runtasks.Add (new RunSimulatorTask (buildTask, Simulators.AvailableDevices.Where ((SimDevice v) => v.SimRuntime == latestiOSRuntime.Identifier && v.SimDeviceType == "com.apple.CoreSimulator.SimDeviceType.iPhone-5").First ()));
					runtasks.Add (new RunSimulatorTask (buildTask, Simulators.AvailableDevices.Where ((SimDevice v) => v.SimRuntime == latestiOSRuntime.Identifier && v.SimDeviceType == "com.apple.CoreSimulator.SimDeviceType.iPhone-6s").First ()));
				} else {
					runtasks.Add (new RunSimulatorTask (buildTask, Simulators.AvailableDevices.Where ((SimDevice v) => v.SimRuntime == latestiOSRuntime.Identifier && v.SimDeviceType == "com.apple.CoreSimulator.SimDeviceType.iPhone-4s").First ()));
				}
			}

			return runtasks;
		}

		async Task PopulateTasksAsync ()
		{
			var runTasks = new List<RunSimulatorTask> ();

			foreach (var project in Harness.TestProjects) {
				if (!project.IsExecutableProject)
					continue;
				
				var build = new XBuildTask ()
				{
					Jenkins = this,
					ProjectFile = project.Path,
					ProjectConfiguration = "Debug",
					ProjectPlatform = "iPhoneSimulator",
				};
				if (IncludeClassic)
					runTasks.AddRange (await CreateRunSimulatorTaskAsync (build));

				var suffixes = new string [] { "-unified", "-tvos", "-watchos" };
				foreach (var suffix in suffixes) {
					var derived = new XBuildTask ()
					{
						Jenkins = this,
						ProjectFile = Path.Combine (Path.GetDirectoryName (project.Path), Path.GetFileNameWithoutExtension (project.Path) + suffix + Path.GetExtension (project.Path)),
						ProjectConfiguration = build.ProjectConfiguration,
						ProjectPlatform = build.ProjectPlatform,
					};
					runTasks.AddRange (await CreateRunSimulatorTaskAsync (derived));
				}
			}

			foreach (var taskGroup in runTasks.GroupBy ((RunSimulatorTask task) => task.Device)) {
				Tasks.Add (new AggregatedRunSimulatorTask (taskGroup)
				{
					Jenkins = this,
					Device = taskGroup.Key,
				});
			}
		}

		public int Run ()
		{
			try {
				Directory.CreateDirectory (LogDirectory);
				Harness.HarnessLog = new LogFile ()
				{
					Description = "Harness log",
					Path = Path.Combine (LogDirectory, "Harness.log"),
				};
				Task.Run (async () =>
				{
					await PopulateTasksAsync ();
				}).Wait ();
				var tasks = new List<Task> ();
				foreach (var task in Tasks)
					tasks.Add (task.RunAsync ());
				Task.WaitAll (tasks.ToArray ());
				GenerateReport ();
				return Tasks.Any ((v) => v.ExecutionResult == TestExecutingResult.Failed || v.ExecutionResult == TestExecutingResult.Crashed) ? 1 : 0;
			} catch (Exception ex) {
				Harness.Log ("Unexpected exception: {0}", ex);
				return 2;
			}
		}

		string GetTestColor (IEnumerable<TestTask> tests)
		{
			if (tests.All ((v) => v.Succeeded))
				return "green";
			else if (tests.Any ((v) => v.Crashed))
				return "maroon";
			else if (tests.Any ((v) => v.Failed))
				return "red";
			else if (tests.Any ((v) => v.ExecutionResult == TestExecutingResult.TimedOut))
				return "purple";
			else if (tests.Any ((v) => v.ExecutionState == TestExecutionState.Running))
				return "blue";
			else if (tests.Any ((v) => v.ExecutionState == TestExecutionState.NotStarted))
				return "gray";
			else
				return "black";
		}

		string GetTestColor (TestTask test)
		{
			switch (test.ExecutionState) {
			case TestExecutionState.NotStarted:
				return "gray";
			case TestExecutionState.Running:
				return "blue";
			case TestExecutionState.Finished:
				switch (test.ExecutionResult) {
				case TestExecutingResult.Crashed:
					return "maroon";
				case TestExecutingResult.Failed:
					return "red";
				case TestExecutingResult.HarnessException:
					return "yellow";
				case TestExecutingResult.Succeeded:
					return "green";
				case TestExecutingResult.TimedOut:
					return "purple";
				case TestExecutingResult.None: // shouldn't happen
				default:
					return "pink";
				}
			default:
				return "pink";
			}
		}

		object report_lock = new object ();
		public void GenerateReport ()
		{
			var id_counter = 0;
			lock (report_lock) {
				var report = Path.Combine (LogDirectory, "index.html");
				if (File.Exists (report))
					File.Delete (report);
				using (var writer = new StreamWriter (report)) {
					writer.WriteLine ("<!DOCTYPE html>");
					writer.WriteLine ("<html>");
					writer.WriteLine ("<title>Test results</title>");
					writer.WriteLine (@"<script type='text/javascript'>
function toggleLogVisibility (logName)
{
	var button = document.getElementById ('button_' + logName);
	var logs = document.getElementById ('logs_' + logName);
	if (logs.style.display == 'none') {
		logs.style.display = 'block';
		button.innerText = 'Hide details';
	} else {
		logs.style.display = 'none';
		button.innerText = 'Show details';
	}
}
</script>");
					writer.WriteLine ("<body>");
					writer.WriteLine ("<h1>Test results</h1>");

					writer.WriteLine ("<a href='{0}'>Harness log</a> <br />", Harness.HarnessLog.Path.Substring (LogDirectory.Length + 1));

					var allSimulatorTasks = new List<RunSimulatorTask> ();
					foreach (var task in Tasks) {
						var aggregated = task as AggregatedRunSimulatorTask;
						if (aggregated != null) {
							allSimulatorTasks.AddRange (aggregated.Tasks);
						} else {
							task.WriteReport (writer);
						}
					}
					var failedTests = allSimulatorTasks.Where ((v) => v.ExecutionState == TestExecutionState.Finished && !v.Succeeded);
					var stillRunning = allSimulatorTasks.Any ((v) => v.ExecutionState != TestExecutionState.Finished);
					if (failedTests.Count () == 0) {
						if (stillRunning) {
							writer.WriteLine ("<h2>All tests passed (but still running tests)</h2>");
						} else {
							writer.WriteLine ("<h2 style='color: green'>All tests passed</h2>");
						}
					} else {
						writer.WriteLine ("<h2 style='color: red'>{0} tests failed</h2>", failedTests.Count ());
						foreach (var group in failedTests.GroupBy ((v) => v.TestName))
							writer.WriteLine ("<a href='#test_{2}'>{0}</a> ({1})<br />", group.Key, string.Join (", ", ((IEnumerable<RunSimulatorTask>) group).Select ((v) => string.Format ("<span style='color: {0}'>{1}</span>", GetTestColor (v), v.Mode)).ToArray ()), group.Key.Replace (' ', '-'));
					}

					foreach (var group in allSimulatorTasks.GroupBy ((RunSimulatorTask v) => v.TestName)) {
						writer.WriteLine ("<h2 id='test_{1}' style='color: {2}'>{0}</h2> <br />", group.Key, group.Key.Replace (' ', '-'), GetTestColor (group));
						foreach (var test in group) {
							string state;
							if (test.ExecutionState == TestExecutionState.Finished) {
								state = test.ExecutionResult.ToString ();
							} else {
								state = test.ExecutionState.ToString ();
							}
							if (test.ExecutionState != TestExecutionState.NotStarted) {
								var log_id = id_counter++;
								writer.WriteLine ("{0} (<span style='color: {3}'>{1}</span>) <a id='button_{2}' href=\"javascript: toggleLogVisibility ('{2}');\">Show details</a><br />", test.Mode, state, log_id, GetTestColor (test));
								writer.WriteLine ("<div id='logs_{0}' style='display: none; padding-bottom: 10px; padding-top: 10px; padding-left: 20px;'>", log_id);
								writer.WriteLine ("Duration: {0} <br />", test.Duration);
								if (test.Logs.Count > 0) {
									foreach (var log in test.Logs) {
										writer.WriteLine ("<a href='{0}' type='text/plain'>{1}</a><br />", log.Path.Substring (LogDirectory.Length + 1), log.Description);
										if (log.Description == "Test log") {
											var summary = string.Empty;
											try {
												foreach (var line in File.ReadAllLines (log.Path)) {
													if (line.StartsWith ("Tests run:", StringComparison.Ordinal)) {
														summary = line;
													} else if (line.Trim ().StartsWith ("[FAIL]", StringComparison.Ordinal)) {
														writer.WriteLine ("<span style='padding-left: 20px;'>{0}</span><br />", line.Trim ());
													}
												}
												if (summary != null)
													writer.WriteLine ("<span style='padding-left: 15px;'>{0}</span><br />", summary);
											} catch (Exception ex) {
												writer.WriteLine ("<span style='padding-left: 15px;'>Could not parse log file: {0}</span><br />", System.Web.HttpUtility.HtmlEncode (ex.Message));
											}
										} else if (log.Description == "Build log") {
											var errors = new HashSet<string> ();
											try {
												foreach (var line in File.ReadAllLines (log.Path)) {
													var ln = line.Trim ();
													if (ln.Contains (": error"))
														errors.Add (ln);
												}
												foreach (var error in errors)
													writer.WriteLine ("<span style='padding-left: 15\tpx;'>{0}</span> <br />", error);
											} catch (Exception ex) {
												writer.WriteLine ("<span style='padding-left: 15px;'>Could not parse log file: {0}</span><br />", System.Web.HttpUtility.HtmlEncode (ex.Message));
											}
										}
									}
								} else {
									writer.WriteLine ("No logs<br />");
								}
								writer.WriteLine ("</div>");
							} else {
								writer.WriteLine ("{0} ({1}) <br />", test.Mode, state);
							}
						}
					}
					writer.WriteLine ("</body>");
					writer.WriteLine ("</html>");
				}
				Harness.Log (2, "Generated report: {0}", report);
			}
		}
	}

	abstract class TestTask
	{
		public Jenkins Jenkins;
		public Harness Harness { get { return Jenkins.Harness; } }
		public string ProjectFile;
		public string ProjectConfiguration;
		public string ProjectPlatform;

		Stopwatch duration = new Stopwatch ();
		public TimeSpan Duration { 
			get {
				return duration.Elapsed;
			}
		}

		public TestExecutionState ExecutionState { get; protected set; }
		public TestExecutingResult ExecutionResult { get; protected set; }

		public bool Succeeded { get { return ExecutionResult == TestExecutingResult.Succeeded; } }
		public bool Failed { get { return ExecutionResult == TestExecutingResult.Failed; } }
		public bool Crashed { get { return ExecutionResult == TestExecutingResult.Crashed; } }

		public virtual string Mode { get; set; }

		public string TestName {
			get {
				var rv = Path.GetFileNameWithoutExtension (ProjectFile);
				if (rv.EndsWith ("-watchos", StringComparison.Ordinal)) {
					return rv.Substring (0, rv.Length - 8);
				} else if (rv.EndsWith ("-tvos", StringComparison.Ordinal)) {
					return rv.Substring (0, rv.Length - 5);
				} else if (rv.EndsWith ("-unified", StringComparison.Ordinal)) {
					return rv.Substring (0, rv.Length - 8);
				} else {
					return rv;
				}
			}
		}

		public TestPlatform Platform {
			get {
				var fn = Path.GetFileNameWithoutExtension (ProjectFile);
				if (fn.EndsWith ("-watchos", StringComparison.Ordinal)) {
					return TestPlatform.watchOS;
				} else if (fn.EndsWith ("-tvos", StringComparison.Ordinal)) {
					return TestPlatform.tvOS;
				} else if (fn.EndsWith ("-unified", StringComparison.Ordinal)) {
					return TestPlatform.iOS_Unified;
				} else {
					return TestPlatform.iOS_Classic;
				}
			}
		}

		public LogFiles Logs = new LogFiles ();
		public List<Resource> Resources = new List<Resource> ();

		public string LogDirectory {
			get {
				return Path.Combine (Jenkins.LogDirectory, TestName);
			}
		}

		public async Task RunAsync ()
		{
			if (ExecutionState == TestExecutionState.Finished)
				return;

			ExecutionState = TestExecutionState.Running;

			Jenkins.GenerateReport ();

			duration.Start ();

			await ExecuteAsync ();

			duration.Stop ();

			ExecutionState = TestExecutionState.Finished;
			if (ExecutionResult == TestExecutingResult.None)
				throw new Exception ("Result not set!");

			Jenkins.GenerateReport ();
		}

		protected abstract Task ExecuteAsync ();
		public abstract void WriteReport (StreamWriter writer);
	}

	class XBuildTask : TestTask
	{
		public override string Mode {
			get { return Platform.ToString (); }
			set { throw new NotSupportedException (); }
		}

		protected override async Task ExecuteAsync ()
		{
			using (var xbuild = new Process ()) {
				xbuild.StartInfo.FileName = "xbuild";
				xbuild.StartInfo.Arguments = $"/verbosity:diagnostic /p:Platform={ProjectPlatform} /p:Configuration={ProjectConfiguration} {Harness.Quote (ProjectFile)}";
				Harness.Log ("Building {0} ({1})", TestName, Mode);
				var log = Logs.Create (LogDirectory, "build-" + Platform + ".txt", "Build log");
				log.WriteLine ("{0} {1}", xbuild.StartInfo.FileName, xbuild.StartInfo.Arguments);
				if (Harness.DryRun) {
					Harness.Log ("{0} {1}", xbuild.StartInfo.FileName, xbuild.StartInfo.Arguments);
				} else {
					await xbuild.RunAsync (log.Path, true);
					ExecutionResult = xbuild.ExitCode == 0 ? TestExecutingResult.Succeeded : TestExecutingResult.Failed;
				}
				Harness.Log ("Built {0} ({1})", TestName, Mode);
			}
		}

		public override void WriteReport (StreamWriter writer)
		{
			throw new NotImplementedException ();
		}
	}

	class RunSimulatorTask : TestTask
	{
		public SimDevice Device;
		public XBuildTask BuildTask;
		public bool SkipSetupAndCleanup;
		public string AppRunnerTarget;

		public override string Mode {
			get {
				switch (Platform) {
				case TestPlatform.tvOS:
				case TestPlatform.watchOS:
					return Platform.ToString ();
				case TestPlatform.iOS_Classic:
					return "iOS Classic";
				case TestPlatform.iOS_Unified:
					if (Jenkins.Simulators.SupportedDeviceTypes.Find ((SimDeviceType v) => v.Identifier == Device.SimDeviceType).Supports64Bits) {
						return "iOS Unified 32-bits";
					} else {
						return "iOS Unified 64-bits";
					}
				default:
					throw new NotImplementedException ();
				}
			}
			set { throw new NotSupportedException (); }
		}

		public RunSimulatorTask (XBuildTask build_task, SimDevice device)
		{
			BuildTask = build_task;
			Device = device;
			Jenkins = build_task.Jenkins;
			ProjectFile = build_task.ProjectFile;

			var project = Path.GetFileNameWithoutExtension (ProjectFile);
			if (project.EndsWith ("-tvos", StringComparison.Ordinal)) {
				AppRunnerTarget = "tvos-simulator";
			} else if (project.EndsWith ("-watchos", StringComparison.Ordinal)) {
				AppRunnerTarget = "watchos-simulator";
			} else {
				AppRunnerTarget = "ios-simulator";
			}
		}

		protected override async Task ExecuteAsync ()
		{
			Harness.Log ("Running simulator '{0}' ({2}) for {1}", Device.Name, ProjectFile, Jenkins.Simulators.SupportedRuntimes.Where ((v) => v.Identifier == Device.SimRuntime).First ().Name);
			await BuildTask.RunAsync ();

			Logs.AddRange (BuildTask.Logs);

			if (!BuildTask.Succeeded) {
				ExecutionResult = TestExecutingResult.Failed;
				return;
			}

			if (Harness.DryRun) {
				Harness.Log ("<running app in simulator>");
			} else {
				var runner = new AppRunner ()
				{
					Harness = Harness,
					ProjectFile = ProjectFile,
					SkipSimulatorSetup = SkipSetupAndCleanup,
					SkipSimulatorCleanup = SkipSetupAndCleanup,
					Target = AppRunnerTarget,
					LogDirectory = LogDirectory,
				};
				runner.Simulators = new SimDevice [] { Device };
				try {
					runner.Run ();
					ExecutionResult = runner.Result;
				} catch (Exception ex) {
					Harness.Log ("Test {0} failed: {1}", Path.GetFileName (ProjectFile), ex);
					ExecutionResult = TestExecutingResult.HarnessException;
				}
				Logs.AddRange (runner.Logs);
			}

			foreach (var log in Logs)
				Console.WriteLine ("Log: {0}: {1}", log.Description, log.Path);
		}

		public override void WriteReport (StreamWriter writer)
		{
			writer.WriteLine ("<h2>{0}</h2>", Mode);
			writer.WriteLine ("Build: {0}", BuildTask.ExecutionResult);
			if (!BuildTask.Failed) {
				writer.WriteLine ("Run: {0}", ExecutionResult);
			}
		}
	}

	// This class groups simulator run tasks according to the
	// simulator they'll run from, so that we minimize switching
	// between different simulators (which is slow).
	class AggregatedRunSimulatorTask : TestTask
	{
		public SimDevice Device;

		public IEnumerable<RunSimulatorTask> Tasks;

		public AggregatedRunSimulatorTask (IEnumerable<RunSimulatorTask> tasks)
		{
			this.Tasks = tasks;
		}

		protected override async Task ExecuteAsync ()
		{
			using (var desktop = await Jenkins.DesktopResource.AcquireAsync ()) {
				Harness.Log ("Preparing simulator: {0}", Device.Name);
				bool first = true;
				foreach (var task in Tasks) {
					task.SkipSetupAndCleanup = !first;
					first = false;
					await task.RunAsync ();
				}
			}

			ExecutionResult = Tasks.Any ((v) => !v.Succeeded) ? TestExecutingResult.Failed : TestExecutingResult.Succeeded;
		}

		public override void WriteReport (StreamWriter writer)
		{
			foreach (var task in Tasks)
				task.WriteReport (writer);
		}
	}

	// This is a very simple class to manage the general concept of 'resource'.
	// Performance isn't important, so this is very simple.
	// Currently it's only used to make sure everything that happens on the desktop
	// is serialized (Jenkins.DesktopResource), but in the future the idea is to
	// make each connected device a separate resource, which will make it possible
	// to run tests in parallel across devices (and at the same time use the desktop
	// to build the next test project).
	class Resource
	{
		public string Name;
		ConcurrentQueue<TaskCompletionSource<IDisposable>> queue = new ConcurrentQueue<TaskCompletionSource<IDisposable>> ();
		int users;
		int max_users = 1;

		public Task<IDisposable> AcquireAsync ()
		{
			lock (queue) {
				if (users == max_users) {
					var tcs = new TaskCompletionSource<IDisposable> (new AcquiredResource (this));
					queue.Enqueue (tcs);
					return tcs.Task;
				} else {
					users++;
					return Task.FromResult<IDisposable> (new AcquiredResource (this));
				}
			}
		}

		void Release ()
		{
			TaskCompletionSource<IDisposable> tcs;

			lock (queue) {
				users--;
				if (queue.TryDequeue (out tcs))
					tcs.SetResult ((IDisposable) tcs.Task.AsyncState);
			}
		}

		class AcquiredResource : IDisposable
		{
			Resource resource;

			public AcquiredResource (Resource resource)
			{
				this.resource = resource;
			}

			void IDisposable.Dispose ()
			{
				resource.Release ();
			}
		}
	}

	public enum TestPlatform
	{
		iOS_Classic,
		iOS_Unified,
		tvOS,
		watchOS,
	}

	public enum TestExecutionState
	{
		NotStarted,
		Running,
		Finished,
	}

	public enum TestExecutingResult
	{
		None,
		Succeeded,
		Crashed,
		Failed,
		TimedOut,
		HarnessException,
	}
}

