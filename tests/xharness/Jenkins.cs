using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace xharness
{
	public class Jenkins
	{
		public Harness Harness;
		public bool IncludeClassiciOS;
		public bool IncludeClassicMac = true;
		public bool IncludeBcl;
		public bool IncludeMac = true;
		public bool IncludeiOS = true;
		public bool IncludetvOS = true;
		public bool IncludewatchOS = true;

		public Logs Logs = new Logs ();
		public Log MainLog;
		Log SimulatorLoadLog;

		public string LogDirectory {
			get {
				return Path.Combine (Harness.JENKINS_RESULTS_DIRECTORY, "tests");
			}
		}
		public Simulators Simulators = new Simulators ();

		List<TestTask> Tasks = new List<TestTask> ();

		internal static Resource DesktopResource = new Resource ("Desktop", Environment.ProcessorCount);

		async Task<IEnumerable<RunSimulatorTask>> CreateRunSimulatorTaskAsync (XBuildTask buildTask)
		{
			var runtasks = new List<RunSimulatorTask> ();

			Simulators.Harness = Harness;
			if (SimulatorLoadLog == null)
				SimulatorLoadLog = Logs.CreateStream (LogDirectory, "simulator-list.log", "Simulator Listing");
			try {
				await Simulators.LoadAsync (SimulatorLoadLog);
			} catch (Exception e) {
				SimulatorLoadLog.WriteLine ("Failed to load simulators:");
				SimulatorLoadLog.WriteLine (e.ToString ());
				return runtasks;
			}

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
				runtasks.Add (new RunSimulatorTask (buildTask, device) { Platform = TestPlatform.tvOS });
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
				var devices = 
					Simulators.AvailableDevices.
					          Where ((SimDevice d) => d.SimRuntime == latestwatchOSRuntime.Identifier && d.SimDeviceType == watchOSDeviceType.Identifier);
				var pair = Simulators.AvailableDevicePairs.
				              FirstOrDefault ((SimDevicePair v) => devices.Any ((SimDevice d) => d.UDID == v.Gizmo));
				var device =
					Simulators.AvailableDevices.
							  FirstOrDefault ((SimDevice v) => pair.Gizmo == v.UDID); // select the device in the device pair.
				var companion =
					Simulators.AvailableDevices.
							  FirstOrDefault ((SimDevice v) => pair.Companion == v.UDID);
				runtasks.Add (new RunSimulatorTask (buildTask, device, companion) { Platform = TestPlatform.watchOS, ExecutionResult = TestExecutingResult.Ignored });
			} else {
				var latestiOSRuntime =
					Simulators.SupportedRuntimes.
							  Where ((SimRuntime v) => v.Identifier.StartsWith ("com.apple.CoreSimulator.SimRuntime.iOS-", StringComparison.Ordinal)).
							  OrderBy ((SimRuntime v) => v.Version).
							  Last ();

				if (fn.EndsWith ("-unified", StringComparison.Ordinal)) {
					runtasks.Add (new RunSimulatorTask (buildTask, Simulators.AvailableDevices.Where ((SimDevice v) => v.SimRuntime == latestiOSRuntime.Identifier && v.SimDeviceType == "com.apple.CoreSimulator.SimDeviceType.iPhone-5").First ()) { Platform = TestPlatform.iOS_Unified32 });
					runtasks.Add (new RunSimulatorTask (buildTask, Simulators.AvailableDevices.Where ((SimDevice v) => v.SimRuntime == latestiOSRuntime.Identifier && v.SimDeviceType == "com.apple.CoreSimulator.SimDeviceType.iPhone-6s").First ()) { Platform = TestPlatform.iOS_Unified64 });
				} else {
					runtasks.Add (new RunSimulatorTask (buildTask, Simulators.AvailableDevices.Where ((SimDevice v) => v.SimRuntime == latestiOSRuntime.Identifier && v.SimDeviceType == "com.apple.CoreSimulator.SimDeviceType.iPhone-5").First ()) { Platform = TestPlatform.iOS_Classic });
				}
			}

			return runtasks;
		}

		static string AddSuffixToPath (string path, string suffix)
		{
			return Path.Combine (Path.GetDirectoryName (path), Path.GetFileNameWithoutExtension (path) + suffix + Path.GetExtension (path));
		}

		async Task PopulateTasksAsync ()
		{
			// Missing:
			// api-diff
			// msbuild tests

			if (IncludeiOS || IncludetvOS || IncludewatchOS) {
				var runSimulatorTasks = new List<RunSimulatorTask> ();

				foreach (var project in Harness.IOSTestProjects) {
					if (!project.IsExecutableProject)
						continue;

					if (!IncludeBcl && project.Path.Contains ("bcl-test"))
						continue;

					var build = new XBuildTask () {
						Jenkins = this,
						ProjectFile = project.Path,
						ProjectConfiguration = "Debug",
						ProjectPlatform = "iPhoneSimulator",
						Platform = TestPlatform.iOS_Classic,
					};
					if (IncludeClassiciOS && IncludeiOS)
						runSimulatorTasks.AddRange (await CreateRunSimulatorTaskAsync (build));

					var suffixes = new List<Tuple<string, TestPlatform>> ();
					if (IncludeiOS)
						suffixes.Add (new Tuple<string, TestPlatform> ("-unified", TestPlatform.iOS_Unified));
					if (IncludetvOS)
						suffixes.Add (new Tuple<string, TestPlatform> ("-tvos", TestPlatform.tvOS));
					if (IncludewatchOS)
						suffixes.Add (new Tuple<string, TestPlatform> ("-watchos", TestPlatform.watchOS));
					foreach (var pair in suffixes) {
						var derived = new XBuildTask () {
							Jenkins = this,
							ProjectFile = AddSuffixToPath (project.Path, pair.Item1),
							ProjectConfiguration = build.ProjectConfiguration,
							ProjectPlatform = build.ProjectPlatform,
							Platform = pair.Item2,
						};
						runSimulatorTasks.AddRange (await CreateRunSimulatorTaskAsync (derived));
					}
				}

				foreach (var taskGroup in runSimulatorTasks.GroupBy ((RunSimulatorTask task) => task.Device)) {
					Tasks.Add (new AggregatedRunSimulatorTask (taskGroup) {
						Jenkins = this,
						Devices = taskGroup.First ().Simulators,
					});
				}

				foreach (var task in runSimulatorTasks) {
					if (task.TestName == "framework-test")
						task.ExecutionResult = TestExecutingResult.Ignored;
				}
			}

			if (IncludeMac) {
				foreach (var project in Harness.MacTestProjects) {
					if (!project.IsExecutableProject)
						continue;

					BuildToolTask build;
					if (project.GenerateVariations) {
						build = new MdtoolTask ();
						build.Platform = TestPlatform.Mac_Classic;
					} else {
						build = new XBuildTask ();
						build.Platform = TestPlatform.Mac;
					}
					build.Jenkins = this;
					build.ProjectFile = project.Path;
					build.ProjectConfiguration = "Debug";
					build.ProjectPlatform = "x86";
					build.SpecifyPlatform = false;
					build.SpecifyConfiguration = false;
					var exec = new MacExecuteTask () {
						Platform = build.Platform,
						Jenkins = this,
						BuildTask = build,
						ProjectFile = build.ProjectFile,
						ProjectConfiguration = build.ProjectConfiguration,
						ProjectPlatform = build.ProjectPlatform,
					};
					if (IncludeClassicMac)
						Tasks.Add (exec);

					if (project.GenerateVariations) {
						Tasks.Add (CloneExecuteTask (exec, TestPlatform.Mac_Unified, "-unified"));
						Tasks.Add (CloneExecuteTask (exec, TestPlatform.Mac_UnifiedXM45, "-unifiedXM45"));
					}
				}
			}
		}

		static MacExecuteTask CloneExecuteTask (MacExecuteTask task, TestPlatform platform, string suffix)
		{
			var build = new XBuildTask ()
			{
				Platform = platform,
				Jenkins = task.Jenkins,
				ProjectFile = AddSuffixToPath (task.ProjectFile, suffix),
				ProjectConfiguration = task.ProjectConfiguration,
				ProjectPlatform = task.ProjectPlatform,
				SpecifyPlatform = task.BuildTask.SpecifyPlatform,
				SpecifyConfiguration = task.BuildTask.SpecifyConfiguration,
			};

			var execute = new MacExecuteTask ()
			{
				Platform = build.Platform,
				Jenkins = build.Jenkins,
				ProjectFile = build.ProjectFile,
				ProjectConfiguration = build.ProjectConfiguration,
				ProjectPlatform = build.ProjectPlatform,
				BuildTask = build,
			};

			return execute;
		}

		public int Run ()
		{
			try {
				Directory.CreateDirectory (LogDirectory);
				Harness.HarnessLog = MainLog = Logs.CreateStream (LogDirectory, "Harness.log", "Harness log");
				Harness.HarnessLog.Timestamp = true;

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
				MainLog.WriteLine ("Unexpected exception: {0}", ex);
				return 2;
			}
		}

		string GetTestColor (IEnumerable<TestTask> tests)
		{
			if (tests.All ((v) => v.Succeeded))
				return "green";
			else if (tests.Any ((v) => v.Crashed))
				return "maroon";
			else if (tests.Any ((v) => v.TimedOut))
				return "purple";
			else if (tests.Any ((v) => v.BuildFailure))
				return "darkred";
			else if (tests.Any ((v) => v.Failed))
				return "red";
			else if (tests.All ((v) => v.Building))
				return "darkblue";
			else if (tests.All ((v) => v.InProgress))
				return "blue";
			else if (tests.Any ((v) => v.NotStarted))
				return "black";
			else if (tests.Any ((v) => v.Ignored))
				return "gray";
			else
				return "black";
		}

		string GetTestColor (TestTask test)
		{
			if (test.NotStarted) {
				return "black";
			} else if (test.InProgress) {
				if (test.Building) {
					return "darkblue";
				} else if (test.Running) {
					return "lightblue";
				} else {
					return "blue";
				}
			} else {
				if (test.Crashed) {
					return "maroon";
				} else if (test.HarnessException) {
					return "yellow";
				} else if (test.TimedOut) {
					return "purple";
				} else if (test.BuildFailure) {
					return "darkred";
				} else if (test.Failed) {
					return "red";
				} else if (test.Succeeded) {
					return "green";
				} else if (test.Ignored) {
					return "gray";
				} else {
					return "pink";
				}
			}
		}

		object report_lock = new object ();
		public void GenerateReport ()
		{
			var id_counter = 0;
			using (var stream = new MemoryStream ()) {
				using (var writer = new StreamWriter (stream)) {
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
function toggleContainerVisibility (containerName)
{
	var button = document.getElementById ('button_container_' + containerName);
	var div = document.getElementById ('test_container_' + containerName);
	if (div.style.display == 'none') {
		div.style.display = 'block';
		button.innerText = 'Hide';
	} else {
		div.style.display = 'none';
		button.innerText = 'Show';
	}
}
</script>");
					writer.WriteLine ("<body>");
					writer.WriteLine ("<h1>Test results</h1>");

					foreach (var log in Logs)
						writer.WriteLine ("<a href='{0}' type='text/plain'>{1}</a><br />", log.FullPath.Substring (LogDirectory.Length + 1), log.Description);

					var allSimulatorTasks = new List<RunSimulatorTask> ();
					var allExecuteTasks = new List<MacExecuteTask> ();
					foreach (var task in Tasks) {
						var aggregated = task as AggregatedRunSimulatorTask;
						if (aggregated != null) {
							allSimulatorTasks.AddRange (aggregated.Tasks);
							continue;
						}

						var execute = task as MacExecuteTask;
						if (execute != null) {
							allExecuteTasks.Add (execute);
							continue;
						}

						throw new NotImplementedException ();
					}

					var allTasks = new List<TestTask> ();
					allTasks.AddRange (allExecuteTasks);
					allTasks.AddRange (allSimulatorTasks);

					var failedTests = allTasks.Where ((v) => v.Failed);
					var stillInProgress = allTasks.Any ((v) => v.InProgress);
					if (failedTests.Count () == 0) {
						if (stillInProgress) {
							writer.WriteLine ("<h2>All tests passed (but still tests in progress)</h2>");
						} else {
							writer.WriteLine ("<h2 style='color: green'>All tests passed</h2>");
						}
					} else {
						writer.WriteLine ("<h2 style='color: red'>{0} tests failed</h2>", failedTests.Count ());
						foreach (var group in failedTests.GroupBy ((v) => v.TestName)) {
							var enumerableGroup = group as IEnumerable<TestTask>;
							if (enumerableGroup != null) {
								writer.WriteLine ("<a href='#test_{2}'>{0}</a> ({1})<br />", group.Key, string.Join (", ", enumerableGroup.Select ((v) => string.Format ("<span style='color: {0}'>{1}</span>", GetTestColor (v), string.IsNullOrEmpty (v.Mode) ? v.ExecutionResult.ToString () : v.Mode)).ToArray ()), group.Key.Replace (' ', '-'));
								continue;
							}

							throw new NotImplementedException ();
						}
					}

					foreach (var group in allTasks.GroupBy ((TestTask v) => v.TestName)) {
						// Create a collection of all non-ignored tests in the group (unless all tests were ignored).
						var relevantGroup = group.Where ((v) => v.ExecutionResult != TestExecutingResult.Ignored);
						if (!relevantGroup.Any ())
							relevantGroup = group;
						var firstResult = relevantGroup.First ().ExecutionResult;
						var identicalResults = relevantGroup.All ((v) => v.ExecutionResult == firstResult);
						var defaultHide = !group.Any ((v) => v.Failed);
						writer.WriteLine ("<h2 id='test_{1}'>{0} (<span style='color: {2}'>{4}</span>) <small><a id='button_container_{1}' href=\"javascript: toggleContainerVisibility ('{1}');\">{3}</a></small> </h2>", 
						                  group.Key, group.Key.Replace (' ', '-'), GetTestColor (relevantGroup), defaultHide ? "Show" : "Hide", identicalResults ? firstResult.ToString () : "multiple results");
						writer.WriteLine ("<div id='test_container_{0}' style='display: {1}'>", group.Key.Replace (' ', '-'), defaultHide ? "none" : "block");
						foreach (var test in group) {
							string state;
							state = test.ExecutionResult.ToString ();
							var log_id = id_counter++;
							writer.WriteLine ("{0} (<span style='color: {3}'>{1}</span>) <a id='button_{2}' href=\"javascript: toggleLogVisibility ('{2}');\">Show details</a><br />", test.Mode, state, log_id, GetTestColor (test));
							writer.WriteLine ("<div id='logs_{0}' style='display: none; padding-bottom: 10px; padding-top: 10px; padding-left: 20px;'>", log_id);
							writer.WriteLine ("Duration: {0} <br />", test.Duration);
							var logs = test.AggregatedLogs;
							if (logs.Count () > 0) {
								foreach (var log in logs) {
									log.Flush ();
									writer.WriteLine ("<a href='{0}' type='text/plain'>{1}</a><br />", log.FullPath.Substring (LogDirectory.Length + 1), log.Description);
									if (log.Description == "Test log") {
										var summary = string.Empty;
										try {
											using (var reader = log.GetReader ()) {
												while (!reader.EndOfStream) {
													string line = reader.ReadLine ();
													if (line.StartsWith ("Tests run:", StringComparison.Ordinal)) {
														summary = line;
													} else if (line.Trim ().StartsWith ("[FAIL]", StringComparison.Ordinal)) {
														writer.WriteLine ("<span style='padding-left: 20px;'>{0}</span><br />", line.Trim ());
													}
												}
											}
											if (!string.IsNullOrEmpty (summary))
												writer.WriteLine ("<span style='padding-left: 15px;'>{0}</span><br />", summary);
										} catch (Exception ex) {
											writer.WriteLine ("<span style='padding-left: 15px;'>Could not parse log file: {0}</span><br />", System.Web.HttpUtility.HtmlEncode (ex.Message));
										}
									} else if (log.Description == "Build log") {
										var errors = new HashSet<string> ();
										try {
											using (var reader = log.GetReader ()) {
												while (!reader.EndOfStream) {
													string line = reader.ReadLine ().Trim ();
													if (line.Contains (": error"))
														errors.Add (line);
												}
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
						}
						writer.WriteLine ("</div>");
					}
					writer.WriteLine ("</body>");
					writer.WriteLine ("</html>");
				}
				lock (report_lock) {
					var report = Path.Combine (LogDirectory, "index.html");
					if (File.Exists (report))
						File.Delete (report);
					File.WriteAllBytes (report, stream.ToArray ());
				}
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

		TestExecutingResult execution_result;
		public TestExecutingResult ExecutionResult {
			get {
				return execution_result;
			}
			set {
				execution_result = value;
				Jenkins.GenerateReport ();
			}
		}

		public bool NotStarted { get { return (ExecutionResult & TestExecutingResult.StateMask) == TestExecutingResult.NotStarted; } }
		public bool InProgress { get { return (ExecutionResult & TestExecutingResult.StateMask) == TestExecutingResult.InProgress; } }
		public bool Finished { get { return (ExecutionResult & TestExecutingResult.StateMask) == TestExecutingResult.Finished; } }

		public bool Building { get { return (ExecutionResult & TestExecutingResult.InProgressMask) == TestExecutingResult.Building; } }
		public bool Built { get { return (ExecutionResult & TestExecutingResult.InProgressMask) == TestExecutingResult.Built; } }
		public bool Running { get { return (ExecutionResult & TestExecutingResult.InProgressMask) == TestExecutingResult.Running; } }

		public bool Succeeded { get { return (ExecutionResult & TestExecutingResult.Succeeded) == TestExecutingResult.Succeeded; } }
		public bool Failed { get { return (ExecutionResult & TestExecutingResult.Failed) == TestExecutingResult.Failed; } }
		public bool Ignored { get { return (ExecutionResult & TestExecutingResult.Ignored) == TestExecutingResult.Ignored; } }

		public bool Crashed { get { return (ExecutionResult & TestExecutingResult.Crashed) == TestExecutingResult.Crashed; } }
		public bool TimedOut { get { return (ExecutionResult & TestExecutingResult.TimedOut) == TestExecutingResult.TimedOut; } }
		public bool BuildFailure { get { return (ExecutionResult & TestExecutingResult.BuildFailure) == TestExecutingResult.BuildFailure; } }
		public bool HarnessException { get { return (ExecutionResult & TestExecutingResult.HarnessException) == TestExecutingResult.HarnessException; } }

		public virtual string Mode { get; set; }

		public virtual string TestName {
			get {
				var rv = Path.GetFileNameWithoutExtension (ProjectFile);
				switch (Platform) {
				case TestPlatform.Mac:
				case TestPlatform.Mac_Classic:
					return rv;
				case TestPlatform.Mac_Unified:
					return rv.Substring (0, rv.Length - "-unified".Length);
				case TestPlatform.Mac_UnifiedXM45:
					return rv.Substring (0, rv.Length - "-unifiedXM45".Length);
				default:
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
		}

		public TestPlatform Platform { get; set; }

		public Logs Logs = new Logs ();
		public List<Resource> Resources = new List<Resource> ();

		public virtual IEnumerable<Log> AggregatedLogs {
			get {
				return Logs;
			}
		}

		public string LogDirectory {
			get {
				return Path.Combine (Jenkins.LogDirectory, TestName);
			}
		}

		Task build_task;
		async Task RunInternalAsync ()
		{
			if (Finished)
				return;
			
			ExecutionResult = (ExecutionResult & ~TestExecutingResult.StateMask) | TestExecutingResult.InProgress;

			Jenkins.GenerateReport ();

			duration.Start ();

			build_task = ExecuteAsync ();
			await build_task;

			duration.Stop ();

			ExecutionResult = (ExecutionResult & ~TestExecutingResult.StateMask) | TestExecutingResult.Finished;
			if ((ExecutionResult & ~TestExecutingResult.StateMask) == 0)
				throw new Exception ("Result not set!");

			Jenkins.GenerateReport ();
		}

		public Task RunAsync ()
		{
			if (build_task == null)
				build_task = RunInternalAsync ();
			return build_task;
		}

		protected abstract Task ExecuteAsync ();

		public override string ToString ()
		{
			return ExecutionResult.ToString ();
		}
	}

	abstract class BuildToolTask : TestTask
	{
		public bool SpecifyPlatform = true;
		public bool SpecifyConfiguration = true;

		public override string Mode {
			get { return Platform.ToString (); }
			set { throw new NotSupportedException (); }
		}

		protected void SetEnvironmentVariables (Process process)
		{
			switch (Platform) {
			case TestPlatform.iOS_Classic:
			case TestPlatform.iOS_Unified:
			case TestPlatform.iOS_Unified32:
			case TestPlatform.iOS_Unified64:
			case TestPlatform.tvOS:
			case TestPlatform.watchOS:
				process.StartInfo.EnvironmentVariables ["MD_APPLE_SDK_ROOT"] = Harness.XcodeRoot;
				process.StartInfo.EnvironmentVariables ["MD_MTOUCH_SDK_ROOT"] = Path.Combine (Harness.IOS_DESTDIR, "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current");
				process.StartInfo.EnvironmentVariables ["XBUILD_FRAMEWORK_FOLDERS_PATH"] = Path.Combine (Harness.IOS_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild-frameworks");
				process.StartInfo.EnvironmentVariables ["MSBuildExtensionsPath"] = Path.Combine (Harness.IOS_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild");
				break;
			case TestPlatform.Mac:
			case TestPlatform.Mac_Classic:
			case TestPlatform.Mac_Unified:
			case TestPlatform.Mac_UnifiedXM45:
				process.StartInfo.EnvironmentVariables ["MD_APPLE_SDK_ROOT"] = Harness.XcodeRoot;
				process.StartInfo.EnvironmentVariables ["XBUILD_FRAMEWORK_FOLDERS_PATH"] = Path.Combine (Harness.MAC_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild-frameworks");
				process.StartInfo.EnvironmentVariables ["MSBuildExtensionsPath"] = Path.Combine (Harness.MAC_DESTDIR, "Library", "Frameworks", "Mono.framework", "External", "xbuild");
				process.StartInfo.EnvironmentVariables ["XamarinMacFrameworkRoot"] = Path.Combine (Harness.MAC_DESTDIR, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				process.StartInfo.EnvironmentVariables ["XAMMAC_FRAMEWORK_PATH"] = Path.Combine (Harness.MAC_DESTDIR, "Library", "Frameworks", "Xamarin.Mac.framework", "Versions", "Current");
				break;
			default:
				throw new NotImplementedException ();
			}
		}

	}

	class MdtoolTask : BuildToolTask
	{
		protected override async Task ExecuteAsync ()
		{
			using (var resource = await Jenkins.DesktopResource.AcquireConcurrentAsync ()) {
				using (var xbuild = new Process ()) {
					xbuild.StartInfo.FileName = "/Applications/Xamarin Studio.app/Contents/MacOS/mdtool";
					var args = new StringBuilder ();
					args.Append ("build ");
					var sln = Path.ChangeExtension (ProjectFile, "sln");
					args.Append (Harness.Quote (File.Exists (sln) ? sln : ProjectFile));
					xbuild.StartInfo.Arguments = args.ToString ();
					Jenkins.MainLog.WriteLine ("Building {0} ({1})", TestName, Mode);
					SetEnvironmentVariables (xbuild);
					var log = Logs.CreateStream (LogDirectory, "build-" + Platform + ".txt", "Build log");
					foreach (string key in xbuild.StartInfo.EnvironmentVariables.Keys)
						log.WriteLine ("{0}={1}", key, xbuild.StartInfo.EnvironmentVariables [key]);
					log.WriteLine ("{0} {1}", xbuild.StartInfo.FileName, xbuild.StartInfo.Arguments);
					if (!Harness.DryRun) {
						try {
							var timeout = TimeSpan.FromMinutes (5);
							var result = await xbuild.RunAsync (log, true, timeout);
							if (result.TimedOut) {
								ExecutionResult = TestExecutingResult.TimedOut;
								log.WriteLine ("Build timed out after {0} seconds.", timeout.TotalSeconds);
							} else if (result.Succeeded) {
								ExecutionResult = TestExecutingResult.Succeeded;
							} else {
								ExecutionResult = TestExecutingResult.Failed;
							}
						} catch (Exception e) {
							log.WriteLine ("Harness exception: {0}", e);
							ExecutionResult = TestExecutingResult.HarnessException;
						}
					}
					Jenkins.MainLog.WriteLine ("Built {0} ({1})", TestName, Mode);
				}
			}
		}
	}

	class XBuildTask : BuildToolTask
	{
		protected override async Task ExecuteAsync ()
		{
			using (var resource = await Jenkins.DesktopResource.AcquireConcurrentAsync ()) {
				using (var xbuild = new Process ()) {
					xbuild.StartInfo.FileName = "xbuild";
					var args = new StringBuilder ();
					args.Append ("/verbosity:diagnostic ");
					if (SpecifyPlatform)
						args.Append ($"/p:Platform={ProjectPlatform} ");
					if (SpecifyConfiguration)
						args.Append ($"/p:Configuration={ProjectConfiguration} ");
					args.Append (Harness.Quote (ProjectFile));
					xbuild.StartInfo.Arguments = args.ToString ();
					Jenkins.MainLog.WriteLine ("Building {0} ({1})", TestName, Mode);
					SetEnvironmentVariables (xbuild);
					var log = Logs.CreateStream (LogDirectory, "build-" + Platform + ".txt", "Build log");
					foreach (string key in xbuild.StartInfo.EnvironmentVariables.Keys)
						log.WriteLine ("{0}={1}", key, xbuild.StartInfo.EnvironmentVariables [key]);
					log.WriteLine ("{0} {1}", xbuild.StartInfo.FileName, xbuild.StartInfo.Arguments);
					if (!Harness.DryRun) {
						try {
							var timeout = TimeSpan.FromMinutes (5);
							var result = await xbuild.RunAsync (log, true, timeout);
							if (result.TimedOut) {
								ExecutionResult = TestExecutingResult.TimedOut;
								log.WriteLine ("Build timed out after {0} seconds.", timeout.TotalSeconds);
							} else if (result.Succeeded) {
								ExecutionResult = TestExecutingResult.Succeeded;
							} else {
								ExecutionResult = TestExecutingResult.Failed;
							}
						} catch (Exception e) {
							log.WriteLine ("Harness exception: {0}", e);
							ExecutionResult = TestExecutingResult.HarnessException;
						}
					}
					Jenkins.MainLog.WriteLine ("Built {0} ({1})", TestName, Mode);
				}
			}
		}
	}

	abstract class MacTask : TestTask
	{
		public override string Mode {
			get {
				switch (Platform) {
				case TestPlatform.Mac:
					return TestName;
				case TestPlatform.Mac_Classic:
					return "Classic";
				case TestPlatform.Mac_Unified:
					return "Unified";
				case TestPlatform.Mac_UnifiedXM45:
					return "Unified XM45";
				default:
					throw new NotImplementedException ();
				}
			}
			set {
				throw new NotSupportedException ();
			}
		}
	}

	class MacExecuteTask : MacTask
	{
		public string Path;
		public BuildToolTask BuildTask;

		public override IEnumerable<Log> AggregatedLogs {
			get {
				return base.AggregatedLogs.Union (BuildTask.Logs);
			}
		}

		protected override async Task ExecuteAsync ()
		{
			ExecutionResult = TestExecutingResult.Building;
			await BuildTask.RunAsync ();
			if (!BuildTask.Succeeded) {
				ExecutionResult = TestExecutingResult.BuildFailure;
				return;
			}

			ExecutionResult = TestExecutingResult.Built;

			var projectDir = System.IO.Path.GetDirectoryName (ProjectFile);
			var name = System.IO.Path.GetFileName (projectDir);
			if (string.Equals ("mac", name, StringComparison.OrdinalIgnoreCase))
				name = System.IO.Path.GetFileName (System.IO.Path.GetDirectoryName (projectDir));
			var suffix = string.Empty;
			switch (Platform) {
			case TestPlatform.Mac_Unified:
				suffix = "-unified";
				break;
			case TestPlatform.Mac_UnifiedXM45:
				suffix = "-unifiedXM45";
				break;
			}
			Path = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (ProjectFile), "bin", BuildTask.ProjectPlatform, BuildTask.ProjectConfiguration + suffix, name + ".app", "Contents", "MacOS", name);

			using (var resource = await Jenkins.DesktopResource.AcquireConcurrentAsync ()) {
				using (var proc = new Process ()) {
					proc.StartInfo.FileName = Path;
					Jenkins.MainLog.WriteLine ("Executing {0} ({1})", TestName, Mode);
					var log = Logs.CreateStream (LogDirectory, "execute-" + Platform + ".txt", "Execution log");
					log.WriteLine ("{0} {1}", proc.StartInfo.FileName, proc.StartInfo.Arguments);
					if (!Harness.DryRun) {
						ExecutionResult = TestExecutingResult.Running;
						try {
							var timeout = TimeSpan.FromMinutes (10);
							var result = await proc.RunAsync (log, true, timeout);
							if (result.TimedOut) {
								log.WriteLine ("Execution timed out after {0} seconds.", timeout.TotalSeconds);
								ExecutionResult = TestExecutingResult.TimedOut;
							} else if (result.Succeeded) {
								ExecutionResult = TestExecutingResult.Succeeded;
							} else {
								ExecutionResult = TestExecutingResult.Failed;
							}
						} catch (Exception e) {
							log.WriteLine (e.ToString ());
							ExecutionResult = TestExecutingResult.HarnessException;
						}
					}
					Jenkins.MainLog.WriteLine ("Executed {0} ({1})", TestName, Mode);
				}
			}
		}
	}

	class RunSimulatorTask : TestTask
	{
		public SimDevice Device;
		public SimDevice CompanionDevice;
		public XBuildTask BuildTask;
		public string AppRunnerTarget;

		AppRunner runner;

		public SimDevice [] Simulators {
			get {
				if (CompanionDevice == null) {
					return new SimDevice [] { Device };
				} else {
					return new SimDevice [] { Device, CompanionDevice };
				}
			}
		}

		public string BundleIdentifier {
			get {
				return runner.BundleIdentifier;
			}
		}

		public async Task BuildAsync ()
		{
			if (Finished)
				return;
			ExecutionResult |= TestExecutingResult.Building;
			await BuildTask.RunAsync ();
			if (BuildTask.Succeeded) {
				ExecutionResult = (ExecutionResult & ~TestExecutingResult.InProgressMask) | TestExecutingResult.Built;
			} else {
				ExecutionResult = (ExecutionResult & ~(TestExecutingResult.InProgressMask | TestExecutingResult.StateMask)) | TestExecutingResult.BuildFailure;
			}
		}

		public override IEnumerable<Log> AggregatedLogs {
			get {
				return base.AggregatedLogs.Union (BuildTask.Logs);
			}
		}

		public override string Mode {
			get {
				switch (Platform) {
				case TestPlatform.tvOS:
				case TestPlatform.watchOS:
					return Platform.ToString ();
				case TestPlatform.iOS_Classic:
					return "iOS Classic";
				case TestPlatform.iOS_Unified32:
					return "iOS Unified 32-bits";
				case TestPlatform.iOS_Unified64:
					return "iOS Unified 64-bits";
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

		public RunSimulatorTask (XBuildTask build_task, SimDevice device, SimDevice companion_device = null)
		{
			BuildTask = build_task;
			Device = device;
			CompanionDevice = companion_device;
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

		public Task PrepareSimulatorAsync ()
		{
			if (Finished)
				return Task.FromResult (true);
			
			if (!BuildTask.Succeeded) {
				ExecutionResult = TestExecutingResult.BuildFailure;
				return Task.FromResult (true);
			}

			var clean_state = false;//Platform == TestPlatform.tvOS;
			runner = new AppRunner ()
			{
				Harness = Harness,
				ProjectFile = ProjectFile,
				EnsureCleanSimulatorState = clean_state,
				Target = AppRunnerTarget,
				LogDirectory = LogDirectory,
				MainLog = Logs.CreateStream (LogDirectory, "run-" + Device.UDID + ".log", "Run log"),
			};
			runner.Simulators = Simulators;
			runner.Initialize ();

			return Task.FromResult (true);
		}

		protected override async Task ExecuteAsync ()
		{
			Jenkins.MainLog.WriteLine ("Running simulator '{0}' ({2}) for {1}", Device.Name, ProjectFile, Jenkins.Simulators.SupportedRuntimes.Where ((v) => v.Identifier == Device.SimRuntime).First ().Name);

			if (Finished)
				return;
			
			if (Harness.DryRun) {
				Jenkins.MainLog.WriteLine ("<running app in simulator>");
			} else {
				try {
					ExecutionResult = (ExecutionResult & ~TestExecutingResult.InProgressMask) | TestExecutingResult.Running;
					await runner.RunAsync ();
					ExecutionResult = runner.Result;
				} catch (Exception ex) {
					Jenkins.MainLog.WriteLine ("Test {0} failed: {1}", Path.GetFileName (ProjectFile), ex);
					ExecutionResult = TestExecutingResult.HarnessException;
				}
				Logs.AddRange (runner.Logs);
			}
		}
	}

	// This class groups simulator run tasks according to the
	// simulator they'll run from, so that we minimize switching
	// between different simulators (which is slow).
	class AggregatedRunSimulatorTask : TestTask
	{
		public SimDevice[] Devices;

		public IEnumerable<RunSimulatorTask> Tasks;

		// Due to parallelization this isn't the same as the sum of the duration for all the build tasks.
		Stopwatch build_timer = new Stopwatch ();
		public TimeSpan BuildDuration { get { return build_timer.Elapsed; } }

		Stopwatch run_timer = new Stopwatch ();
		public TimeSpan RunDuration { get { return run_timer.Elapsed; } }

		public AggregatedRunSimulatorTask (IEnumerable<RunSimulatorTask> tasks)
		{
			this.Tasks = tasks;
		}

		protected override async Task ExecuteAsync ()
		{
			// First build everything. This is required for the run simulator
			// task to properly configure the simulator.
			build_timer.Start ();
			await Task.WhenAll (Tasks.Select ((v) => v.BuildAsync ()).Distinct ());
			build_timer.Stop ();

			using (var desktop = await Jenkins.DesktopResource.AcquireExclusiveAsync ()) {
				run_timer.Start ();

				Jenkins.MainLog.WriteLine ("Preparing simulator: {0}", Devices [0].Name);
				// We need to set the dialog permissions for all the apps
				// before launching the simulator, because once launched
				// the simulator caches the values in-memory.
				foreach (var task in Tasks)
					await task.PrepareSimulatorAsync ();

				foreach (var dev in Devices)
					await dev.PrepareSimulatorAsync (Jenkins.MainLog, Tasks.Where ((v) => !v.Ignored).Select ((v) => v.BundleIdentifier).ToArray ());
				
				foreach (var task in Tasks)
					await task.RunAsync ();

				foreach (var dev in Devices)
					await dev.ShutdownAsync (Jenkins.MainLog);

				await SimDevice.KillEverythingAsync (Jenkins.MainLog);

				run_timer.Stop ();
			}

			ExecutionResult = Tasks.Any ((v) => !v.Succeeded) ? TestExecutingResult.Failed : TestExecutingResult.Succeeded;
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
		ConcurrentQueue<TaskCompletionSource<IDisposable>> exclusive_queue = new ConcurrentQueue<TaskCompletionSource<IDisposable>> ();
		int users;
		int max_concurrent_users = 1;
		bool exclusive;

		public Resource (string name, int max_concurrent_users = 1)
		{
			this.Name = name;
			this.max_concurrent_users = max_concurrent_users;
		}

		public Task<IDisposable> AcquireConcurrentAsync ()
		{
			lock (queue) {
				if (!exclusive && users < max_concurrent_users) {
					users++;
					return Task.FromResult<IDisposable> (new AcquiredResource (this));
				} else {
					var tcs = new TaskCompletionSource<IDisposable> (new AcquiredResource (this));
					queue.Enqueue (tcs);
					return tcs.Task;
				}
			}
		}

		public Task<IDisposable> AcquireExclusiveAsync ()
		{
			lock (queue) {
				if (users == 0) {
					users++;
					exclusive = true;
					return Task.FromResult<IDisposable> (new AcquiredResource (this));
				} else {
					var tcs = new TaskCompletionSource<IDisposable> (new AcquiredResource (this));
					exclusive_queue.Enqueue (tcs);
					return tcs.Task;
				}
			}
		}

		void Release ()
		{
			TaskCompletionSource<IDisposable> tcs;

			lock (queue) {
				users--;
				exclusive = false;
				if (queue.TryDequeue (out tcs)) {
					users++;
					tcs.SetResult ((IDisposable) tcs.Task.AsyncState);
				} else if (users == 0 && exclusive_queue.TryDequeue (out tcs)) {
					users++;
					exclusive = true;
					tcs.SetResult ((IDisposable) tcs.Task.AsyncState);
				}
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
		None,

		iOS_Classic,
		iOS_Unified,
		iOS_Unified32,
		iOS_Unified64,
		tvOS,
		watchOS,

		Mac,
		Mac_Classic,
		Mac_Unified,
		Mac_UnifiedXM45,
	}

	[Flags]
	public enum TestExecutingResult
	{
		NotStarted = 0,
		InProgress = 0x1,
		Finished   = 0x2,
		StateMask  = NotStarted + InProgress + Finished,

		// In progress state
		Building         =   0x10 + InProgress,
		Built            =   0x20 + InProgress,
		Running          =   0x40 + InProgress,
		InProgressMask   =   0x10 + 0x20 + 0x40,

		// Finished results
		Succeeded        =  0x100 + Finished,
		Failed           =  0x200 + Finished,
		Ignored          =  0x400 + Finished,

		// Finished & Failed results
		Crashed          = 0x1000 + Failed,
		TimedOut         = 0x2000 + Failed,
		HarnessException = 0x4000 + Failed,
		BuildFailure     = 0x8000 + Failed,
	}
}
