using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Xharness.Jenkins.TestTasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Xharness.Jenkins.Reports;

namespace Xharness.Jenkins {
	public class Jenkins {
		public readonly ISimulatorLoader Simulators;
		readonly IHardwareDeviceLoader devices;
		readonly IProcessManager processManager;
		readonly IResultParser resultParser;
		readonly ITunnelBore tunnelBore;
		readonly TestSelector testSelector;
		readonly TestVariationsFactory testVariationsFactory;
		readonly JenkinsDeviceLoader deviceLoader;
		readonly ResourceManager resourceManager;

		// report writers, do need to be a class instance because the have state.
		readonly HtmlReportWriter htmlReportWriter;
		readonly MarkdownReportWriter markdownReportWriter;
		
		public bool Populating { get; private set; } = true;

		public Harness Harness { get; }
		public bool IncludeAll;
		public bool IncludeBcl;
		public bool IncludeMac = true;
		public bool IncludeiOS = true;
		public bool IncludeiOS64 = true;
		public bool IncludeiOS32 = false; // broken in xcode 12 beta 3, not possible with DTK
		public bool IncludeiOSExtensions;
		public bool ForceExtensionBuildOnly;
		public bool IncludetvOS = true;
		public bool IncludewatchOS = true;
		public bool IncludeMmpTest;
		public bool IncludeiOSMSBuild = true;
		public bool IncludeMtouch;
		public bool IncludeBtouch;
		public bool IncludeMacBindingProject;
		public bool IncludeSimulator = true;
		public bool IncludeOldSimulatorTests;
		public bool IncludeDevice;
		public bool IncludeXtro;
		public bool IncludeCecil;
		public bool IncludeDocs;
		public bool IncludeBCLxUnit;
		public bool IncludeBCLNUnit;
		public bool IncludeMscorlib;
		public bool IncludeNonMonotouch = true;
		public bool IncludeMonotouch = true;

		public bool CleanSuccessfulTestRuns = true;
		public bool UninstallTestApp = true;

		public ILog MainLog;
		public ILog SimulatorLoadLog => deviceLoader.SimulatorLoadLog;
		public ILog DeviceLoadLog => deviceLoader.DeviceLoadLog;

		string log_directory;
		public string LogDirectory {
			get {
				if (string.IsNullOrEmpty (log_directory)) {
					log_directory = Path.Combine (Harness.JENKINS_RESULTS_DIRECTORY, "tests");
					if (IsServerMode)
						log_directory = Path.Combine (log_directory, Helpers.Timestamp);
				}
				return log_directory;
			}
		}

		ILogs logs;
		public ILogs Logs {
			get {
				return logs ?? (logs = new Logs (LogDirectory));
			}
		}

		List<ITestTask> Tasks = new List<ITestTask> ();
		Dictionary<string, MakeTask> DependencyTasks = new Dictionary<string, MakeTask> ();

		public IErrorKnowledgeBase ErrorKnowledgeBase => new ErrorKnowledgeBase ();
		public IResourceManager ResourceManager => resourceManager;

		public Jenkins (Harness harness, IProcessManager processManager, IResultParser resultParser, ITunnelBore tunnelBore)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.resultParser = resultParser ?? throw new ArgumentNullException (nameof (resultParser));
			this.tunnelBore = tunnelBore ?? throw new ArgumentNullException (nameof (tunnelBore));
			Harness = harness ?? throw new ArgumentNullException (nameof (harness));
			Simulators = new SimulatorLoader (processManager);
			devices = new HardwareDeviceLoader (processManager);
			testSelector = new TestSelector (this, processManager, new GitHub (harness, processManager));
			testVariationsFactory = new TestVariationsFactory (this, processManager);
			deviceLoader = new JenkinsDeviceLoader (Simulators, devices, Logs);
			resourceManager = new ResourceManager ();
			htmlReportWriter = new HtmlReportWriter (jenkins: this, resourceManager: resourceManager, resultParser: resultParser);
			markdownReportWriter = new MarkdownReportWriter ();
		}

		IEnumerable<RunSimulatorTask> CreateRunSimulatorTaskAsync (MSBuildTask buildTask)
		{
			var runtasks = new List<RunSimulatorTask> ();

			TestTarget [] targets = buildTask.Platform.GetAppRunnerTargets ();
			TestPlatform [] platforms;
			bool [] ignored;

			switch (buildTask.Platform) {
			case TestPlatform.tvOS:
				platforms = new TestPlatform [] { TestPlatform.tvOS };
				ignored = new [] { false };
				break;
			case TestPlatform.watchOS:
				platforms = new TestPlatform [] { TestPlatform.watchOS_32 };
				ignored = new [] { false };
				break;
			case TestPlatform.iOS_Unified:
				platforms = new TestPlatform [] { TestPlatform.iOS_Unified32, TestPlatform.iOS_Unified64 };
				ignored = new [] { !IncludeiOS32, false};
				break;
			case TestPlatform.iOS_TodayExtension64:
				targets = new TestTarget[] { TestTarget.Simulator_iOS64 };
				platforms = new TestPlatform[] { TestPlatform.iOS_TodayExtension64 };
				ignored = new [] { false };
				break;
			default:
				throw new NotImplementedException ();
			}

			for (int i = 0; i < targets.Length; i++) {
				var sims = Simulators.SelectDevices (targets [i], SimulatorLoadLog, false);
				runtasks.Add (new RunSimulatorTask (
					jenkins: this,
					simulators: Simulators,
					buildTask: buildTask,
					processManager: processManager,
					tunnelBore: tunnelBore,
					candidates: sims) {
					Platform = platforms [i],
					Ignored = ignored[i] || buildTask.Ignored
				});
			}

			return runtasks;
		}

		public bool IsIncluded (TestProject project)
		{
			if (!project.IsExecutableProject)
				return false;
			
			if (project.IsBclTest ()) {
				if (!project.IsBclxUnit ())
					return IncludeBcl || IncludeBCLNUnit;
				if (project.IsMscorlib ()) 
					return IncludeMscorlib;
				return IncludeBcl || IncludeBCLxUnit;
			}

			if (!IncludeMonotouch && project.IsMonotouch ())
				return false;

			if (!IncludeNonMonotouch && !project.IsMonotouch ())
				return false;

			if (Harness.IncludeSystemPermissionTests == false && project.Name == "introspection")
				return false;

			return true;
		}

		async Task<IEnumerable<AppleTestTask>> CreateRunSimulatorTasksAsync ()
		{
			var runSimulatorTasks = new List<RunSimulatorTask> ();

			foreach (var project in Harness.IOSTestProjects) {
				if (!project.IsExecutableProject)
					continue;

				bool ignored = !IncludeSimulator;
				if (!IsIncluded (project))
					ignored = true;

				var ps = new List<Tuple<TestProject, TestPlatform, bool>> ();
				if (!project.SkipiOSVariation)
					ps.Add (new Tuple<TestProject, TestPlatform, bool> (project, TestPlatform.iOS_Unified, ignored || !IncludeiOS64));
				if (project.MonoNativeInfo != null)
					ps.Add (new Tuple<TestProject, TestPlatform, bool> (project, TestPlatform.iOS_TodayExtension64, ignored || !IncludeiOS64));
				if (!project.SkiptvOSVariation)
					ps.Add (new Tuple<TestProject, TestPlatform, bool> (project.AsTvOSProject (), TestPlatform.tvOS, ignored || !IncludetvOS));
				if (!project.SkipwatchOSVariation)
					ps.Add (new Tuple<TestProject, TestPlatform, bool> (project.AsWatchOSProject (), TestPlatform.watchOS, ignored || !IncludewatchOS));
				
				var configurations = project.Configurations;
				if (configurations == null)
					configurations = new string [] { "Debug" };
				foreach (var config in configurations) {
					foreach (var pair in ps) {
						var derived = new MSBuildTask (jenkins: this, testProject: project, processManager: processManager) {
							ProjectConfiguration = config,
							ProjectPlatform = "iPhoneSimulator",
							Platform = pair.Item2,
							Ignored = pair.Item3,
							TestName = project.Name,
							Dependency = project.Dependency,
						};
						derived.CloneTestProject (MainLog, processManager, pair.Item1);
						var simTasks = CreateRunSimulatorTaskAsync (derived);
						runSimulatorTasks.AddRange (simTasks);
						foreach (var task in simTasks) {
							if (configurations.Length > 1)
								task.Variation = config;
							task.TimeoutMultiplier = project.TimeoutMultiplier;
						}
					}
				}
			}

			var testVariations = testVariationsFactory.CreateTestVariations (runSimulatorTasks, (buildTask, test, candidates) =>
				new RunSimulatorTask (
					jenkins: this,
					simulators: Simulators,
					buildTask: buildTask,
					processManager: processManager,
					tunnelBore: tunnelBore,
					candidates: candidates?.Cast<SimulatorDevice> () ?? test.Candidates)).ToList ();

			foreach (var tv in testVariations) {
				if (!tv.Ignored)
					await tv.FindSimulatorAsync ();
			}

			var rv = new List<AggregatedRunSimulatorTask> ();
			foreach (var taskGroup in testVariations.GroupBy ((RunSimulatorTask task) => task.Device?.UDID ?? task.Candidates.ToString ())) {
				rv.Add (new AggregatedRunSimulatorTask (jenkins: this, tasks: taskGroup) {
					TestName = $"Tests for {taskGroup.Key}",
				});
			}
			return rv;
		}

		Task<IEnumerable<AppleTestTask>> CreateRunDeviceTasksAsync ()
		{
			var rv = new List<RunDeviceTask> ();
			var projectTasks = new List<RunDeviceTask> ();

			foreach (var project in Harness.IOSTestProjects) {
				if (!project.IsExecutableProject)
					continue;
				
				bool ignored = !IncludeDevice;
				if (!IsIncluded (project))
					ignored = true;

				projectTasks.Clear ();
				if (!project.SkipiOSVariation) {
					var build64 = new MSBuildTask (jenkins: this, testProject: project, processManager: processManager) {
						ProjectConfiguration = "Debug64",
						ProjectPlatform = "iPhone",
						Platform = TestPlatform.iOS_Unified64,
						TestName = project.Name,
					};
					build64.CloneTestProject (MainLog, processManager, project);
					projectTasks.Add (new RunDeviceTask (
						jenkins: this,
						devices: devices,
						buildTask: build64,
						processManager: processManager,
						tunnelBore: tunnelBore,
						errorKnowledgeBase: ErrorKnowledgeBase,
						useTcpTunnel: Harness.UseTcpTunnel,
						candidates: devices.Connected64BitIOS.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !IncludeiOS64 });

					var build32 = new MSBuildTask (jenkins: this, testProject: project, processManager: processManager) {
						ProjectConfiguration = project.Name != "dont link" ? "Debug32" : "Release32",
						ProjectPlatform = "iPhone",
						Platform = TestPlatform.iOS_Unified32,
						TestName = project.Name,
					};
					build32.CloneTestProject (MainLog, processManager, project);
					projectTasks.Add (new RunDeviceTask (
						jenkins: this,
						devices: devices,
						buildTask: build32,
						processManager: processManager,
						tunnelBore: tunnelBore,
						errorKnowledgeBase: ErrorKnowledgeBase,
						useTcpTunnel: Harness.UseTcpTunnel,
						candidates: devices.Connected32BitIOS.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !IncludeiOS32 });

					var todayProject = project.AsTodayExtensionProject ();
					var buildToday = new MSBuildTask (jenkins: this, testProject: todayProject, processManager: processManager) {
						ProjectConfiguration = "Debug64",
						ProjectPlatform = "iPhone",
						Platform = TestPlatform.iOS_TodayExtension64,
						TestName = project.Name,
					};
					buildToday.CloneTestProject (MainLog, processManager, todayProject);
					projectTasks.Add (new RunDeviceTask (
						jenkins: this,
						devices: devices,
						buildTask: buildToday,
						processManager: processManager,
						tunnelBore: tunnelBore,
						errorKnowledgeBase: ErrorKnowledgeBase,
						useTcpTunnel: Harness.UseTcpTunnel,
						candidates: devices.Connected64BitIOS.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !IncludeiOSExtensions, BuildOnly = ForceExtensionBuildOnly });
				}

				if (!project.SkiptvOSVariation) {
					var tvOSProject = project.AsTvOSProject ();
					var buildTV = new MSBuildTask (jenkins: this, testProject: tvOSProject, processManager: processManager) {
						ProjectConfiguration = "Debug",
						ProjectPlatform = "iPhone",
						Platform = TestPlatform.tvOS,
						TestName = project.Name,
					};
					buildTV.CloneTestProject (MainLog, processManager, tvOSProject);
					projectTasks.Add (new RunDeviceTask (
						jenkins: this,
						devices: devices,
						buildTask: buildTV,
						processManager: processManager,
						tunnelBore: tunnelBore,
						errorKnowledgeBase: ErrorKnowledgeBase,
						useTcpTunnel: Harness.UseTcpTunnel,
						candidates: devices.ConnectedTV.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !IncludetvOS });
				}

				if (!project.SkipwatchOSVariation) {
					var watchOSProject = project.AsWatchOSProject ();
					if (!project.SkipwatchOS32Variation) {
						var buildWatch32 = new MSBuildTask (jenkins: this, testProject: watchOSProject, processManager: processManager) {
							ProjectConfiguration = "Debug32",
							ProjectPlatform = "iPhone",
							Platform = TestPlatform.watchOS_32,
							TestName = project.Name,
						};
						buildWatch32.CloneTestProject (MainLog, processManager, watchOSProject);
						projectTasks.Add (new RunDeviceTask (
							jenkins: this,
							devices: devices,
							buildTask: buildWatch32,
							processManager: processManager,
							tunnelBore: tunnelBore,
							errorKnowledgeBase: ErrorKnowledgeBase,
							useTcpTunnel: Harness.UseTcpTunnel,
							candidates: devices.ConnectedWatch) { Ignored = !IncludewatchOS });
					}

					if (!project.SkipwatchOSARM64_32Variation) {
						var buildWatch64_32 = new MSBuildTask (jenkins: this, testProject: watchOSProject, processManager: processManager) {
							ProjectConfiguration = "Release64_32", // We don't support Debug for ARM64_32 yet.
							ProjectPlatform = "iPhone",
							Platform = TestPlatform.watchOS_64_32,
							TestName = project.Name,
						};
						buildWatch64_32.CloneTestProject (MainLog, processManager, watchOSProject);
						projectTasks.Add (new RunDeviceTask (
							jenkins: this,
							devices: devices,
							buildTask: buildWatch64_32,
							processManager: processManager,
							tunnelBore: tunnelBore,
							errorKnowledgeBase: ErrorKnowledgeBase,
							useTcpTunnel: Harness.UseTcpTunnel,
							candidates: devices.ConnectedWatch32_64.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !IncludewatchOS });
					}
				}
				foreach (var task in projectTasks) {
					task.TimeoutMultiplier = project.TimeoutMultiplier;
					task.BuildOnly |= project.BuildOnly;
					task.Ignored |= ignored;
				}
				rv.AddRange (projectTasks);
			}

			return Task.FromResult<IEnumerable<AppleTestTask>> (testVariationsFactory.CreateTestVariations (rv, (buildTask, test, candidates)
				=> new RunDeviceTask (
					jenkins: this, 
					devices: devices,
					buildTask: buildTask,
					processManager: processManager,
					tunnelBore: tunnelBore,
					errorKnowledgeBase: ErrorKnowledgeBase,
					useTcpTunnel: Harness.UseTcpTunnel,
					candidates: candidates?.Cast<IHardwareDevice> () ?? test.Candidates)));
		}

		public bool IsBetaXcode => Harness.XcodeRoot.IndexOf ("beta", StringComparison.OrdinalIgnoreCase) >= 0;
		
		Task PopulateTasksAsync ()
		{
			// Missing:
			// api-diff

			testSelector.SelectTests ();

			deviceLoader.LoadAllAsync ().DoNotAwait ();

			var loadsim = CreateRunSimulatorTasksAsync ()
				.ContinueWith ((v) => { Console.WriteLine ("Simulator tasks created"); Tasks.AddRange (v.Result); });
			
			//Tasks.AddRange (await CreateRunSimulatorTasksAsync ());

			var crashReportSnapshotFactory = new CrashSnapshotReporterFactory (processManager);

			// all factories are enumerators \o/ 
			var testFactories = new IEnumerable<AppleTestTask> [] {
				new MacTestTasksEnumerable (this, processManager, crashReportSnapshotFactory, testVariationsFactory),
				new NUnitTestTasksEnumerable (this, processManager),
				new MakeTestTaskEnumerable (this, processManager)
			};

			// add all tests defined by the factory
			foreach (var f in testFactories) {
				Tasks.AddRange (f);
			}

			// individual special tasks
			var buildXtroTests = new MakeTask (jenkins: this, processManager: processManager) {
				Platform = TestPlatform.All,
				TestName = "Xtro",
				Target = "wrench",
				WorkingDirectory = Path.Combine (Harness.RootDirectory, "xtro-sharpie"),
				Ignored = !IncludeXtro,
				Timeout = TimeSpan.FromMinutes (15),
			};

			var runXtroReporter = new RunXtroTask (this, buildXtroTests, processManager, crashReportSnapshotFactory) {
				Platform = TestPlatform.Mac,
				TestName = buildXtroTests.TestName,
				Ignored = buildXtroTests.Ignored,
				WorkingDirectory = buildXtroTests.WorkingDirectory,
			};
			Tasks.Add (runXtroReporter);

			var buildDotNetGeneratorProject = new TestProject (Path.GetFullPath (Path.Combine (Harness.RootDirectory, "bgen", "bgen-tests.csproj")));
			var buildDotNetGenerator = new DotNetBuildTask (jenkins: this, testProject: buildDotNetGeneratorProject, processManager: processManager) {
				TestProject = new TestProject (Path.GetFullPath (Path.Combine (Harness.RootDirectory, "bgen", "bgen-tests.csproj"))),
				SpecifyPlatform = false,
				SpecifyConfiguration = false,
				Platform = TestPlatform.iOS,
			};
			var runDotNetGenerator = new DotNetTestTask (this, buildDotNetGenerator, processManager) {
				TestProject = buildDotNetGeneratorProject,
				Platform = TestPlatform.iOS,
				TestName = "Generator tests",
				Mode = ".NET",
				Ignored = !IncludeBtouch,
			};
			Tasks.Add (runDotNetGenerator);

			var loaddev = CreateRunDeviceTasksAsync ().ContinueWith ((v) => {
				Console.WriteLine ("Got device tasks completed");
				Tasks.AddRange (v.Result);
			});
			return Task.WhenAll (loadsim, loaddev);
		}

		public int Run ()
		{
			try {
				Directory.CreateDirectory (LogDirectory);
				ILog log = Logs.Create ($"Harness-{Helpers.Timestamp}.log", "Harness log");
				if (Harness.InCI)
					log = Log.CreateAggregatedLog (log, new ConsoleLog ());
				Harness.HarnessLog = MainLog = log;

				var tasks = new List<Task> ();
				if (IsServerMode)
					tasks.Add (RunTestServer ());

				if (Harness.InCI) {
					Task.Factory.StartNew (async () => {
						while (true) {
							await Task.Delay (TimeSpan.FromMinutes (10));
							Console.WriteLine ("Still running tests. Please be patient.");
						}
					});
				}
				if (!string.IsNullOrEmpty (Harness.PeriodicCommand)) {
					var periodicCommand = new PeriodicCommand (
						command: Harness.PeriodicCommand,
						processManager: processManager,
						interval: Harness.PeriodicCommandInterval,
						logs: logs,
						arguments: string.IsNullOrEmpty (Harness.PeriodicCommandArguments) ? null : Harness.PeriodicCommandArguments);
					periodicCommand.Execute ().DoNotAwait ();
				}

				// We can populate and build test-libraries in parallel.
				var populate = Task.Run (async () => {
					var simulator = new SimulatorDevice (processManager, new TCCDatabase (processManager));
					await simulator.KillEverything (MainLog);
					await PopulateTasksAsync ();
					Populating = false;
				});
				var preparations = new List<Task> ();
				preparations.Add (populate);
				preparations.Add (BuildTestLibrariesAsync ());
				Task.WaitAll (preparations.ToArray ());

				GenerateReport ();

				if (!IsServerMode) {
					foreach (var task in Tasks)
						tasks.Add (task.RunAsync ());
				}
				Task.WaitAll (tasks.ToArray ());
				GenerateReport ();
				return Tasks.Any ((v) => v.Failed || v.DeviceNotFound) ? 1 : 0;
			} catch (Exception ex) {
				MainLog.WriteLine ("Unexpected exception: {0}", ex);
				Console.WriteLine ("Unexpected exception: {0}", ex);
				return 2;
			}
		}

		public bool IsServerMode {
			get { return Harness.JenkinsConfiguration == "server"; }
		}

		Task BuildTestLibrariesAsync ()
		{
			var sb = new StringBuilder ();
			var callback_log = new CallbackLog ((v) => sb.Append (v));
			var log = Log.CreateAggregatedLog (callback_log, MainLog);
			return processManager.ExecuteCommandAsync ("make", new [] { "all", $"-j{Environment.ProcessorCount}", "-C", Path.Combine (Harness.RootDirectory, "test-libraries") }, log, TimeSpan.FromMinutes (10)).ContinueWith ((v) => {
				var per = v.Result;
				if (!per.Succeeded) {
					// Only show the log if something went wrong.
					using var fn = Logs.Create ("build-test-libraries.log", "⚠️ Build test/test-libraries failed ⚠️");
					File.WriteAllText (fn.FullPath, sb.ToString ());
				}
			});
		}

		Task RunTestServer ()
		{
			var server = new HttpListener ();

			// Try and find an unused port
			int attemptsLeft = 50;
			int port = 51234; // Try this port first, to try to not vary between runs just because.
			Random r = new Random ((int)DateTime.Now.Ticks);
			while (attemptsLeft-- > 0) {
				var newPort = port != 0 ? port : r.Next (49152, 65535); // The suggested range for dynamic ports is 49152-65535 (IANA)
				server.Prefixes.Clear ();
				server.Prefixes.Add ("http://*:" + newPort + "/");
				try {
					server.Start ();
					port = newPort;
					break;
				} catch (Exception ex) {
					MainLog.WriteLine ("Failed to listen on port {0}: {1}", newPort, ex.Message);
					port = 0;
				}
			}
			MainLog.WriteLine ($"Created server on localhost:{port}");

			var tcs = new TaskCompletionSource<bool> ();
			var thread = new System.Threading.Thread (() =>
			{
				while (server.IsListening) {
					var context = server.GetContext ();
					var request = context.Request;
					var response = context.Response;
					var arguments = System.Web.HttpUtility.ParseQueryString (request.Url.Query);
					try {
						var allTasks = Tasks.SelectMany ((v) =>
						{
							var rv = new List<ITestTask> ();
							var runsim = v as AggregatedRunSimulatorTask;
							if (runsim != null)
								rv.AddRange (runsim.Tasks);
							rv.Add (v);
							return rv;
						});

						IEnumerable<ITestTask> find_tasks (StreamWriter writer, string ids)
						{
							IEnumerable<ITestTask> tasks;
							switch (request.Url.Query) {
							case "?all":
								tasks = Tasks;
								break;
							case "?selected":
								tasks = allTasks.Where ((v) => !v.Ignored);
								break;
							case "?failed":
								tasks = allTasks.Where ((v) => v.Failed);
								break;
							case "?":
								writer.WriteLine ("No tasks specified");
								return Array.Empty<AppleTestTask> ();
							default:
								var id_inputs = ids.Substring (1).Split (',');
								var rv = new List<ITestTask> (id_inputs.Length);
								foreach (var id_input in id_inputs) {
									if (int.TryParse (id_input, out var id)) {
										var task = Tasks.FirstOrDefault ((t) => t.ID == id);
										if (task == null)
											task = Tasks.Where ((v) => v is AggregatedRunSimulatorTask).Cast<AggregatedRunSimulatorTask> ().SelectMany ((v) => v.Tasks).FirstOrDefault ((t) => t.ID == id);
										if (task == null) {
											writer.WriteLine ($"Could not find test {id}");
										} else {
											rv.Add (task);
										}
									} else {
										writer.WriteLine ($"Could not parse {arguments ["id"]}");
									}
								}
								tasks = rv;
								break;
							}
							return tasks;
						}

						string serveFile = null;
						switch (request.Url.LocalPath) {
						case "/":
							response.ContentType = System.Net.Mime.MediaTypeNames.Text.Html;
							using (var writer = new StreamWriter (response.OutputStream)) {
								htmlReportWriter.Write (Tasks, writer);
							}
							break;
						case "/set-option":
							response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
							switch (request.Url.Query) {
							case "?clean":
								CleanSuccessfulTestRuns = true;
								break;
							case "?do-not-clean":
								CleanSuccessfulTestRuns = false;
								break;
							case "?uninstall-test-app":
								UninstallTestApp = true;
								break;
							case "?do-not-uninstall-test-app":
								UninstallTestApp = false;
								break;
							case "?skip-permission-tests":
								Harness.IncludeSystemPermissionTests = false;
								break;
							case "?include-permission-tests":
								Harness.IncludeSystemPermissionTests = true;
								break;
							case "?clear-permission-tests":
								Harness.IncludeSystemPermissionTests = null;
								break;
							default:
								throw new NotImplementedException (request.Url.Query);
							}
							using (var writer = new StreamWriter (response.OutputStream)) {
								writer.WriteLine ("OK");
							}
							break;
						case "/select":
						case "/deselect":
							response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
							using (var writer = new StreamWriter (response.OutputStream)) {
								foreach (var task in allTasks) {
									bool? is_match = null;
									if (!(task.Ignored || task.NotStarted))
										continue;
									switch (request.Url.Query) {
									case "?all":
										is_match = true;
										break;
									case "?all-device":
										is_match = task is RunDeviceTask;
										break;
									case "?all-simulator":
										is_match = task is RunSimulatorTask;
										break;
									case "?all-ios":
										switch (task.Platform) {
										case TestPlatform.iOS:
										case TestPlatform.iOS_TodayExtension64:
										case TestPlatform.iOS_Unified:
										case TestPlatform.iOS_Unified32:
										case TestPlatform.iOS_Unified64:
											is_match = true;
											break;
										default:
											if (task.Platform.ToString ().StartsWith ("iOS", StringComparison.Ordinal))
												throw new NotImplementedException ();
											break;
										}
										break;
									case "?all-tvos":
										switch (task.Platform) {
										case TestPlatform.tvOS:
											is_match = true;
											break;
										default:
											if (task.Platform.ToString ().StartsWith ("tvOS", StringComparison.Ordinal))
												throw new NotImplementedException ();
											break;
										}
										break;
									case "?all-watchos":
										switch (task.Platform) {
										case TestPlatform.watchOS:
										case TestPlatform.watchOS_32:
										case TestPlatform.watchOS_64_32:
											is_match = true;
											break;
										default:
											if (task.Platform.ToString ().StartsWith ("watchOS", StringComparison.Ordinal))
												throw new NotImplementedException ();
											break;
										}
										break;
									case "?all-mac":
										switch (task.Platform) {
										case TestPlatform.Mac:
										case TestPlatform.Mac_Modern:
										case TestPlatform.Mac_Full:
										case TestPlatform.Mac_System:
											is_match = true;
											break;
										default:
											if (task.Platform.ToString ().StartsWith ("Mac", StringComparison.Ordinal))
												throw new NotImplementedException ();
											break;
										}
										break;
									default:
										writer.WriteLine ("unknown query: {0}", request.Url.Query);
										break;
									}
									if (request.Url.LocalPath == "/select") {
										if (is_match.HasValue && is_match.Value)
											task.Ignored = false;
									} else if (request.Url.LocalPath == "/deselect") {
										if (is_match.HasValue && is_match.Value)
											task.Ignored = true;
									}
								}

								writer.WriteLine ("OK");
							}
							break;
						case "/stop":
							response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
							using (var writer = new StreamWriter (response.OutputStream)) {
								foreach (var task in find_tasks (writer, request.Url.Query)) {
									if (!task.Waiting) {
										writer.WriteLine ($"Test '{task.TestName}' is not in a waiting state.");
									} else {
										task.Reset ();
									}
								}
								writer.WriteLine ("OK");
							}
							break;
						case "/run":
							response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
							using (var writer = new StreamWriter (response.OutputStream)) {
								// We want to randomize the order the tests are added, so that we don't build first the test for one device, 
								// then for another, since that would not take advantage of running tests on several devices in parallel.
								foreach (var task in find_tasks (writer, request.Url.Query).Shuffle ()) {
									if (task.InProgress || task.Waiting) {
										writer.WriteLine ($"Test '{task.TestName}' is already executing.");
									} else {
										task.Reset ();
										task.BuildOnly = false;
										task.RunAsync ();
									}
								}
								writer.WriteLine ("OK");
							}
							break;
						case "/build":
							response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
							using (var writer = new StreamWriter (response.OutputStream)) {
								foreach (var task in find_tasks (writer, request.Url.Query)) {
									if (task.InProgress || task.Waiting) {
										writer.WriteLine ($"Test '{task.TestName}' is already executing.");
									} else if (task is RunTestTask rtt) {
										rtt.Reset ();
										rtt.BuildAsync ().ContinueWith ((z) => {
											if (rtt.ExecutionResult == TestExecutingResult.Built)
												rtt.ExecutionResult = TestExecutingResult.BuildSucceeded;
										});
									} else {
										writer.WriteLine ($"Test '{task.TestName}' is not a test that can be only built.");
									}
								}

								writer.WriteLine ("OK");
							}
							break;
						case "/reload-devices":
							deviceLoader.LoadDevicesAsync ().DoNotAwait ();
							break;
						case "/reload-simulators":
							deviceLoader.LoadSimulatorsAsync ().DoNotAwait ();
							break;
						case "/quit":
							using (var writer = new StreamWriter (response.OutputStream)) {
								writer.WriteLine ("<!DOCTYPE html>");
								writer.WriteLine ("<html>");
								writer.WriteLine ("<body onload='close ();'>Closing web page...</body>");
								writer.WriteLine ("</html>");
							}
							server.Stop ();
							break;
						case "/favicon.ico":
							serveFile = Path.Combine (Harness.RootDirectory, "xharness", "favicon.ico");
							goto default;
						case "/index.html":
							var redirect_to = request.Url.AbsoluteUri.Replace ("/index.html", "/" + Path.GetFileName (LogDirectory) + "/index.html");
							response.Redirect (redirect_to);
							break;
						default:
							var filename = Path.GetFileName (request.Url.LocalPath);
							if (filename == "index.html" && Path.GetFileName (LogDirectory) == Path.GetFileName (Path.GetDirectoryName (request.Url.LocalPath))) {
									// We're asked for the report for the current test run, so re-generate it.
								GenerateReport ();
							}

							if (serveFile == null)
								serveFile = Path.Combine (Path.GetDirectoryName (LogDirectory), request.Url.LocalPath.Substring (1));
							var path = serveFile;
							if (File.Exists (path)) {
								var buffer = new byte [4096];
								using (var fs = new FileStream (path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
									int read;
									response.ContentLength64 = fs.Length;
									switch (Path.GetExtension (path).ToLowerInvariant ()) {
									case ".html":
										response.ContentType = System.Net.Mime.MediaTypeNames.Text.Html;
										break;
									case ".css":
										response.ContentType = "text/css";
										break;
									case ".js":
										response.ContentType = "text/javascript";
										break;
									case ".ico":
										response.ContentType = "image/png";
										break;
									default:
										response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain + ";charset=UTF-8";
										break;
									}
									while ((read = fs.Read (buffer, 0, buffer.Length)) > 0)
										response.OutputStream.Write (buffer, 0, read);
								}
							} else {
								Console.WriteLine ($"404: {request.Url.LocalPath}");
								response.StatusCode = 404;
								response.OutputStream.WriteByte ((byte) '?');
							}
							break;
						}
					} catch (IOException ioe) {
						Console.WriteLine (ioe.Message);
					} catch (Exception e) {
						Console.WriteLine (e);
					}
					response.Close ();
				}
				tcs.SetResult (true);
			})
			{
				IsBackground = true,
			};
			thread.Start ();

			var url = $"http://localhost:{port}/" + Path.GetFileName (LogDirectory) + "/index.html";
			Console.WriteLine ($"Launching {url} in the system's default browser.");
			Process.Start ("open", url);

			return tcs.Task;
		}

		object report_lock = new object ();
		public void GenerateReport ()
		{
			try {
				lock (report_lock) {
					var report = Path.Combine (LogDirectory, "index.html");
					var tmpreport = Path.Combine (LogDirectory, $"index-{Helpers.Timestamp}.tmp.html");
					var tmpmarkdown = string.IsNullOrEmpty (Harness.MarkdownSummaryPath) ? string.Empty : (Harness.MarkdownSummaryPath + $".{Helpers.Timestamp}.tmp");

					var allSimulatorTasks = new List<RunSimulatorTask> ();
					var allExecuteTasks = new List<MacExecuteTask> ();
					var allNUnitTasks = new List<NUnitExecuteTask> ();
					var allMakeTasks = new List<MakeTask> ();
					var allDeviceTasks = new List<RunDeviceTask> ();
					var allDotNetTestTasks = new List<DotNetTestTask> ();

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

						var nunit = task as NUnitExecuteTask;
						if (nunit != null) {
							allNUnitTasks.Add (nunit);
							continue;
						}

						var make = task as MakeTask;
						if (make != null) {
							allMakeTasks.Add (make);
							continue;
						}

						var run_device = task as RunDeviceTask;
						if (run_device != null) {
							allDeviceTasks.Add (run_device);
							continue;
						}

						if (task is DotNetTestTask dotnet) {
							allDotNetTestTasks.Add (dotnet);
							continue;
						}

						throw new NotImplementedException ();
					}

					var allTasks = new List<ITestTask> ();
					if (!Populating) {
						allTasks.AddRange (allExecuteTasks);
						allTasks.AddRange (allSimulatorTasks);
						allTasks.AddRange (allNUnitTasks);
						allTasks.AddRange (allMakeTasks);
						allTasks.AddRange (allDeviceTasks);
						allTasks.AddRange (allDotNetTestTasks);
					}

					// write the html
					using (var stream = new FileStream (tmpreport, FileMode.Create, FileAccess.ReadWrite)) 
					using (var writer = new StreamWriter (stream)) { 
						htmlReportWriter.Write (allTasks, writer);
					}

					// optionally, write the markdown
					if (!string.IsNullOrEmpty (tmpmarkdown)) {
						using (var writer = new StreamWriter (tmpmarkdown)) {
							markdownReportWriter.Write (allTasks, writer);
						}
					}

					if (File.Exists (report))
						File.Delete (report);
					File.Move (tmpreport, report);
					if (!string.IsNullOrEmpty (tmpmarkdown)) {
						if (File.Exists (Harness.MarkdownSummaryPath))
							File.Delete (Harness.MarkdownSummaryPath);
						File.Move (tmpmarkdown, Harness.MarkdownSummaryPath);
					}
					var dependentFileLocation = Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly ().Location);
					foreach (var file in new string [] { "xharness.js", "xharness.css" }) {
						File.Copy (Path.Combine (dependentFileLocation, file), Path.Combine (LogDirectory, file), true);
					}
					File.Copy (Path.Combine (Harness.RootDirectory, "xharness", "favicon.ico"), Path.Combine (LogDirectory, "favicon.ico"), true);
				}
			} catch (Exception e) {
				this.MainLog.WriteLine ("Failed to write log: {0}", e);
			}
		}
	}
}
