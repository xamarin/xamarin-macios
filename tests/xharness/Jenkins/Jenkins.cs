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

namespace Xharness.Jenkins {
	public class Jenkins : IResourceManager {
		public readonly ISimulatorLoader Simulators;
		readonly IHardwareDeviceLoader devices;
		readonly IProcessManager processManager;
		readonly IResultParser resultParser;
		readonly ITunnelBore tunnelBore;
		readonly TestSelector testSelector;
		readonly TestVariationsFactory testVariationsFactory;
		
		bool populating = true;

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
		public ILog SimulatorLoadLog;
		public ILog DeviceLoadLog;

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

		List<AppleTestTask> Tasks = new List<AppleTestTask> ();
		Dictionary<string, MakeTask> DependencyTasks = new Dictionary<string, MakeTask> ();

		public Resource DesktopResource { get; } = new Resource ("Desktop", Environment.ProcessorCount);
		public Resource NugetResource { get;  } = new Resource ("Nuget", 1); // nuget is not parallel-safe :(
		
		public IErrorKnowledgeBase ErrorKnowledgeBase => new ErrorKnowledgeBase ();

		Dictionary<string, Resource> device_resources = new Dictionary<string, Resource> ();
		public Resources GetDeviceResources (IEnumerable<IHardwareDevice> devices)
		{
			List<Resource> resources = new List<Resource> ();
			lock (device_resources) {
				foreach (var device in devices) {
					Resource res;
					if (!device_resources.TryGetValue (device.UDID, out res))
						device_resources.Add (device.UDID, res = new Resource (device.UDID, 1, device.Name));
					resources.Add (res);
				}
			}
			return new Resources (resources);
		}

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
		}

		Task LoadAsync (ref ILog log, IDeviceLoader deviceManager, string name)
		{
			if (log == null)
				log = Logs.Create ($"{name}-list-{Helpers.Timestamp}.log", $"{name} Listing");

			log.Description = $"{name} Listing (in progress)";

			var capturedLog = log;
			return deviceManager.LoadDevices (capturedLog, includeLocked: false, forceRefresh: true).ContinueWith ((v) => {
				if (v.IsFaulted) {
					capturedLog.WriteLine ("Failed to load:");
					capturedLog.WriteLine (v.Exception.ToString ());
					capturedLog.Description = $"{name} Listing {v.Exception.Message})";
				} else if (v.IsCompleted) {
					if (deviceManager is HardwareDeviceLoader devices) {
						var devicesTypes = new StringBuilder ();
						if (devices.Connected32BitIOS.Any ()) {
							devicesTypes.Append ("iOS 32 bit");
						}
						if (devices.Connected64BitIOS.Any ()) {
							devicesTypes.Append (devicesTypes.Length == 0 ? "iOS 64 bit" : ", iOS 64 bit");
						}
						if (devices.ConnectedTV.Any ()) {
							devicesTypes.Append (devicesTypes.Length == 0 ? "tvOS" : ", tvOS");
						}
						if (devices.ConnectedWatch.Any ()) {
							devicesTypes.Append (devicesTypes.Length == 0 ? "watchOS" : ", watchOS");
						}
						capturedLog.Description = (devicesTypes.Length == 0) ? $"{name} Listing (ok - no devices found)." : $"{name} Listing (ok). Devices types are: {devicesTypes.ToString ()}";
					}
					if (deviceManager is SimulatorLoader simulators) {
						var simCount = simulators.AvailableDevices.Count ();
						capturedLog.Description = ( simCount == 0) ? $"{name} Listing (ok - no simulators found)." : $"{name} Listing (ok - Found {simCount} simulators).";
					}
				}
			});
		}

		// Loads both simulators and devices in parallel
		Task LoadSimulatorsAndDevicesAsync ()
		{
			var devs = LoadAsync (ref DeviceLoadLog, devices, "Device");
			var sims = LoadAsync (ref SimulatorLoadLog, Simulators, "Simulator");

			return Task.WhenAll (devs, sims);
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
					useTcpTunnel: Harness.UseTcpTunnel,
					candidates: candidates?.Cast<IHardwareDevice> () ?? test.Candidates)));
		}

		public bool IsBetaXcode => Harness.XcodeRoot.IndexOf ("beta", StringComparison.OrdinalIgnoreCase) >= 0;
		
		Task PopulateTasksAsync ()
		{
			// Missing:
			// api-diff

			testSelector.SelectTests ();

			LoadSimulatorsAndDevicesAsync ().DoNotAwait ();

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

		async Task ExecutePeriodicCommandAsync (ILog periodic_loc)
		{
			periodic_loc.WriteLine ($"Starting periodic task with interval {Harness.PeriodicCommandInterval.TotalMinutes} minutes.");
			while (true) {
				var watch = Stopwatch.StartNew ();
				using (var process = new Process ()) {
					process.StartInfo.FileName = Harness.PeriodicCommand;
					process.StartInfo.Arguments = Harness.PeriodicCommandArguments;
					var rv = await processManager.RunAsync (process, periodic_loc, timeout: Harness.PeriodicCommandInterval);
					if (!rv.Succeeded)
						periodic_loc.WriteLine ($"Periodic command failed with exit code {rv.ExitCode} (Timed out: {rv.TimedOut})");
				}
				var ticksLeft = watch.ElapsedTicks - Harness.PeriodicCommandInterval.Ticks;
				if (ticksLeft < 0)
					ticksLeft = Harness.PeriodicCommandInterval.Ticks;
				var wait = TimeSpan.FromTicks (ticksLeft);
				await Task.Delay (wait);
			}
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
					var periodic_log = Logs.Create ("PeriodicCommand.log", "Periodic command log");
					Task.Run (async () => await ExecutePeriodicCommandAsync (periodic_log));
				}

				// We can populate and build test-libraries in parallel.
				var populate = Task.Run (async () => {
					var simulator = new SimulatorDevice (processManager, new TCCDatabase (processManager));
					await simulator.KillEverything (MainLog);
					await PopulateTasksAsync ();
					populating = false;
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
							var rv = new List<AppleTestTask> ();
							var runsim = v as AggregatedRunSimulatorTask;
							if (runsim != null)
								rv.AddRange (runsim.Tasks);
							rv.Add (v);
							return rv;
						});

						IEnumerable<AppleTestTask> find_tasks (StreamWriter writer, string ids)
						{
							IEnumerable<AppleTestTask> tasks;
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
								var rv = new List<AppleTestTask> (id_inputs.Length);
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
							GenerateReportImpl (response.OutputStream);
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
							LoadAsync (ref DeviceLoadLog, devices, "Device").DoNotAwait ();
							break;
						case "/reload-simulators":
							LoadAsync (ref SimulatorLoadLog, Simulators, "Simulator").DoNotAwait ();
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
					using (var stream = new FileStream (tmpreport, FileMode.Create, FileAccess.ReadWrite)) {
						using (var markdown_writer = (string.IsNullOrEmpty (tmpmarkdown) ? null : new StreamWriter (tmpmarkdown))) {
							GenerateReportImpl (stream, markdown_writer);
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

		string previous_test_runs;
		void GenerateReportImpl (Stream stream, StreamWriter markdown_summary = null)
		{
			var id_counter = 0;

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
			if (!populating) {
				allTasks.AddRange (allExecuteTasks);
				allTasks.AddRange (allSimulatorTasks);
				allTasks.AddRange (allNUnitTasks);
				allTasks.AddRange (allMakeTasks);
				allTasks.AddRange (allDeviceTasks);
				allTasks.AddRange (allDotNetTestTasks);
			}

			var failedTests = allTasks.Where ((v) => v.Failed);
			var deviceNotFound = allTasks.Where ((v) => v.DeviceNotFound);
			var unfinishedTests = allTasks.Where ((v) => !v.Finished);
			var passedTests = allTasks.Where ((v) => v.Succeeded);
			var runningTests = allTasks.Where ((v) => v.Running && !v.Waiting);
			var buildingTests = allTasks.Where ((v) => v.Building && !v.Waiting);
			var runningQueuedTests = allTasks.Where ((v) => v.Running && v.Waiting);
			var buildingQueuedTests = allTasks.Where ((v) => v.Building && v.Waiting);

			if (markdown_summary != null) {
				var reportWriter = new MarkdownReportWriter ();
				reportWriter.Write (allTasks, markdown_summary);
			}

			using (var writer = new StreamWriter (stream)) {
				writer.WriteLine ("<!DOCTYPE html>");
				writer.WriteLine ("<html onkeypress='keyhandler(event)' lang='en'>");
				writer.WriteLine ("<meta charset=\"utf-8\"/>");
				if (IsServerMode && populating)
					writer.WriteLine ("<meta http-equiv=\"refresh\" content=\"1\">");
				writer.WriteLine ("<head>");
				writer.WriteLine ("<link rel='stylesheet' href='xharness.css'>");
				writer.WriteLine ("<title>Test results</title>");
				writer.WriteLine (@"<script type='text/javascript' src='xharness.js'></script>");
				if (IsServerMode) {
					writer.WriteLine ("<script type='text/javascript'>");
					writer.WriteLine ("setTimeout (autorefresh, 1000);");
					writer.WriteLine ("</script>");
				}
				writer.WriteLine ("</head>");
				writer.WriteLine ("<body onload='oninitialload ();'>");

				if (IsServerMode) {
					writer.WriteLine ("<div id='quit' style='position:absolute; top: 20px; right: 20px;'><a href='javascript:quit()'>Quit</a><br/><a id='ajax-log-button' href='javascript:toggleAjaxLogVisibility ();'>Show log</a></div>");
					writer.WriteLine ("<div id='ajax-log' style='position:absolute; top: 200px; right: 20px; max-width: 100px; display: none;'></div>");
				}

				writer.WriteLine ("<h1>Test results</h1>");

				writer.WriteLine ($"<span id='x{id_counter++}' class='autorefreshable'>");
				foreach (var log in Logs)
					writer.WriteLine ("<a href='{0}' type='text/plain;charset=UTF-8'>{1}</a><br />", log.FullPath.Substring (LogDirectory.Length + 1), log.Description);
				writer.WriteLine ("</span>");

				var headerColor = "black";
				if (unfinishedTests.Any ()) {
					; // default
				} else if (failedTests.Any ()) {
					headerColor = "red";
				} else if (deviceNotFound.Any ()) {
					headerColor = "orange";
				} else if (passedTests.Any ()) {
					headerColor = "green";
				} else {
					headerColor = "gray";
				}

				writer.Write ($"<h2 style='color: {headerColor}'>");
				writer.Write ($"<span id='x{id_counter++}' class='autorefreshable'>");
				if (allTasks.Count == 0) {
					writer.Write ($"Loading tests...");
				} else if (unfinishedTests.Any ()) {
					writer.Write ($"Test run in progress (");
					var list = new List<string> ();
					var grouped = allTasks.GroupBy ((v) => v.ExecutionResult).OrderBy ((v) => (int) v.Key);
					foreach (var @group in grouped)
						list.Add ($"<span style='color: {@group.GetTestColor ()}'>{@group.Key.ToString ()}: {@group.Count ()}</span>");
					writer.Write (string.Join (", ", list));
					writer.Write (")");
				} else if (failedTests.Any ()) {
					writer.Write ($"{failedTests.Count ()} tests failed, ");
					if (deviceNotFound.Any ())
						writer.Write ($"{deviceNotFound.Count ()} tests' device not found, ");
					writer.Write ($"{passedTests.Count ()} tests passed");
				} else if (deviceNotFound.Any ()) {
					writer.Write ($"{deviceNotFound.Count ()} tests' device not found, {passedTests.Count ()} tests passed");
				} else if (passedTests.Any ()) {
					writer.Write ($"All {passedTests.Count ()} tests passed");
				} else {
					writer.Write ($"No tests selected.");
				}
				writer.Write ("</span>");
				writer.WriteLine ("</h2>");
				if (allTasks.Count > 0) {
					writer.WriteLine ($"<ul id='nav'>");
					if (IsServerMode) {
						writer.WriteLine (@"
	<li>Select
		<ul>
			<li class=""adminitem""><a href='javascript:sendrequest (""/select?all"");'>All tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/select?all-device"");'>All device tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/select?all-simulator"");'>All simulator tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/select?all-ios"");'>All iOS tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/select?all-tvos"");'>All tvOS tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/select?all-watchos"");'>All watchOS tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/select?all-mac"");'>All Mac tests</a></li>
		</ul>
	</li>
	<li>Deselect
		<ul>
			<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all"");'>All tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all-device"");'>All device tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all-simulator"");'>All simulator tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all-ios"");'>All iOS tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all-tvos"");'>All tvOS tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all-watchos"");'>All watchOS tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/deselect?all-mac"");'>All Mac tests</a></li>
		</ul>
	</li>
	<li>Execute
		<ul>
			<li class=""adminitem""><a href='javascript:sendrequest (""/run?alltests"");'>Run all tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/run?selected"");'>Run all selected tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/run?failed"");'>Run all failed tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/build?all"");'>Build all tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/build?selected"");'>Build all selected tests</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/build?failed"");'>Build all failed tests</a></li>
		</ul>
	</li>");
					}
					writer.WriteLine (@"
	<li>Toggle visibility
		<ul>
			<li class=""adminitem""><a href='javascript:toggleAll (true);'>Expand all</a></li>
			<li class=""adminitem""><a href='javascript:toggleAll (false);'>Collapse all</a></li>
			<li class=""adminitem""><a href='javascript:toggleVisibility (""toggleable-ignored"");'>Hide/Show ignored tests</a></li>
		</ul>
	</li>");
					if (IsServerMode) {
						var include_system_permission_option = string.Empty;
						var include_system_permission_icon = string.Empty;
						if (Harness.IncludeSystemPermissionTests == null) {
							include_system_permission_option = "include-permission-tests";
							include_system_permission_icon = "2753";
						} else if (Harness.IncludeSystemPermissionTests.Value) {
							include_system_permission_option = "skip-permission-tests";
							include_system_permission_icon = "2705";
						} else {
							include_system_permission_option = "clear-permission-tests";
							include_system_permission_icon = "274C";
						}
						writer.WriteLine ($@"
	<li>Reload
		<ul>
			<li class=""adminitem""><a href='javascript:sendrequest (""/reload-devices"");'>Devices</a></li>
			<li class=""adminitem""><a href='javascript:sendrequest (""/reload-simulators"");'>Simulators</a></li>
		</ul>
	</li>

	<li>Options
			<ul>
				<li class=""adminitem""><span id='{id_counter++}' class='autorefreshable'><a href='javascript:sendrequest (""/set-option?{(CleanSuccessfulTestRuns ? "do-not-clean" : "clean")}"");'>&#x{(CleanSuccessfulTestRuns ? "2705" : "274C")} Clean successful test runs</a></span></li>
				<li class=""adminitem""><span id='{id_counter++}' class='autorefreshable'><a href='javascript:sendrequest (""/set-option?{(UninstallTestApp ? "do-not-uninstall-test-app" : "uninstall-test-app")}"");'>&#x{(UninstallTestApp ? "2705" : "274C")} Uninstall the app from device before and after the test run</a></span></li>
				<li class=""adminitem""><span id='{id_counter++}' class='autorefreshable'><a href='javascript:sendrequest (""/set-option?{include_system_permission_option}"");'>&#x{include_system_permission_icon} Run tests that require system permissions (might put up permission dialogs)</a></span></li>
			</ul>
	</li>
	");
						if (previous_test_runs == null) {
							var sb = new StringBuilder ();
							var previous = Directory.GetDirectories (Path.GetDirectoryName (LogDirectory)).
									Select ((v) => Path.Combine (v, "index.html")).
									    Where (File.Exists);
							if (previous.Any ()) {
								sb.AppendLine ("\t<li>Previous test runs");
								sb.AppendLine ("\t\t<ul>");
								foreach (var prev in previous.OrderBy ((v) => v).Reverse ()) {
									var dir = Path.GetFileName (Path.GetDirectoryName (prev));
									var ts = dir;
									var description = File.ReadAllLines (prev).Where ((v) => v.StartsWith ("<h2", StringComparison.Ordinal)).FirstOrDefault ();
									if (description != null) {
										description = description.Substring (description.IndexOf ('>') + 1); // <h2 ...>
										description = description.Substring (description.IndexOf ('>') + 1); // <span id= ...>

										var h2end = description.LastIndexOf ("</h2>", StringComparison.Ordinal);
										if (h2end > -1)
											description = description.Substring (0, h2end);
										description = description.Substring (0, description.LastIndexOf ('<'));
									} else {
										description = "<unknown state>";
									}
									sb.AppendLine ($"\t\t\t<li class=\"adminitem\"><a href='/{dir}/index.html'>{ts}: {description}</a></li>");
								}
								sb.AppendLine ("\t\t</ul>");
								sb.AppendLine ("\t</li>");
							}
							previous_test_runs = sb.ToString ();
						}
						if (!string.IsNullOrEmpty (previous_test_runs))
							writer.Write (previous_test_runs);
					}
					writer.WriteLine ("</ul>");
				}

				writer.WriteLine ("<div id='test-table' style='width: 100%; display: flex;'>");
				writer.WriteLine ("<div id='test-list'>");
				var orderedTasks = allTasks.GroupBy (v => v.TestName);

				if (IsServerMode) {
					// In server mode don't take into account anything that can change during a test run
					// when ordering, since it's confusing to have the tests reorder by themselves while
					// you're looking at the web page.
					orderedTasks = orderedTasks.OrderBy ((v) => v.Key, StringComparer.OrdinalIgnoreCase);
				} else {
					// Put failed tests at the top and ignored tests at the end.
					// Then order alphabetically.
					orderedTasks = orderedTasks.OrderBy ((v) =>
					 {
						 if (v.Any ((t) => t.Failed))
							 return -1;
						 if (v.All ((t) => t.Ignored))
							 return 1;
						 return 0;
					 }).
					ThenBy ((v) => v.Key, StringComparer.OrdinalIgnoreCase);
				}
				foreach (var group in orderedTasks) {
					var singleTask = group.Count () == 1;
					var groupId = group.Key.Replace (' ', '-');

					// Test header for multiple tests
					if (!singleTask) {
						var autoExpand = !IsServerMode && group.Any ((v) => v.Failed);
						var ignoredClass = group.All ((v) => v.Ignored) ? "toggleable-ignored" : string.Empty;
						var defaultExpander = autoExpand ? "-" : "+";
						var defaultDisplay = autoExpand ? "block" : "none";
						writer.Write ($"<div class='pdiv {ignoredClass}'>");
						writer.Write ($"<span id='button_container2_{groupId}' class='expander' onclick='javascript: toggleContainerVisibility2 (\"{groupId}\");'>{defaultExpander}</span>");
						writer.Write ($"<span id='x{id_counter++}' class='p1 autorefreshable' onclick='javascript: toggleContainerVisibility2 (\"{groupId}\");'>{group.Key}{RenderTextStates (group)}</span>");
						if (IsServerMode) {
							var groupIds = string.Join (",", group.Where ((v) => !v.KnownFailure.HasValue).Select ((v) => v.ID.ToString ()));
							writer.Write ($" <span class='runall'><a href='javascript: runtest (\"{groupIds}\");'>Run all</a> <a href='javascript: buildtest (\"{groupIds}\");'>Build all</a></span>");
						}
						writer.WriteLine ("</div>");
						writer.WriteLine ($"<div id='test_container2_{groupId}' class='togglable' style='display: {defaultDisplay}; margin-left: 20px;'>");
					}

					// Test data
					var groupedByMode = group.GroupBy ((v) => v.Mode);
					foreach (var modeGroup in groupedByMode) {
						var multipleModes = modeGroup.Count () > 1;
						if (multipleModes) {
							var modeGroupId = id_counter++.ToString ();
							var autoExpand = !IsServerMode && modeGroup.Any ((v) => v.Failed);
							var ignoredClass = modeGroup.All ((v) => v.Ignored) ? "toggleable-ignored" : string.Empty;
							var defaultExpander = autoExpand ? "-" : "+";
							var defaultDisplay = autoExpand ? "block" : "none";
							writer.Write ($"<div class='pdiv {ignoredClass}'>");
							writer.Write ($"<span id='button_container2_{modeGroupId}' class='expander' onclick='javascript: toggleContainerVisibility2 (\"{modeGroupId}\");'>{defaultExpander}</span>");
							writer.Write ($"<span id='x{id_counter++}' class='p2 autorefreshable' onclick='javascript: toggleContainerVisibility2 (\"{modeGroupId}\");'>{modeGroup.Key}{RenderTextStates (modeGroup)}</span>");
							if (IsServerMode) {
								var modeGroupIds = string.Join (",", modeGroup.Where ((v) => !v.KnownFailure.HasValue).Select ((v) => v.ID.ToString ()));
								writer.Write ($" <span class='runall'><a href='javascript: runtest (\"{modeGroupIds}\");'>Run all</a> <a href='javascript: buildtest (\"{modeGroupIds}\");'>Build all</a></span>");
							}
							writer.WriteLine ("</div>");

							writer.WriteLine ($"<div id='test_container2_{modeGroupId}' class='togglable' style='display: {defaultDisplay}; margin-left: 20px;'>");
						}
						foreach (var test in modeGroup.OrderBy ((v) => v.Variation, StringComparer.OrdinalIgnoreCase)) {
							var runTest = test as RunTestTask;
							string state;
							state = test.ExecutionResult.ToString ();
							var log_id = id_counter++;
							var logs = test.AggregatedLogs.ToList ();
							string title;
							if (multipleModes) {
								title = test.Variation ?? "Default";
							} else if (singleTask) {
								title = test.TestName;
							} else {
								title = test.Mode;
							}

							var autoExpand = !IsServerMode && test.Failed;
							var ignoredClass = test.Ignored ? "toggleable-ignored" : string.Empty;
							var defaultExpander = autoExpand ? "&nbsp;" : "+";
							var defaultDisplay = autoExpand ? "block" : "none";
							var buildOnly = test.BuildOnly ? ", BuildOnly" : string.Empty;

							writer.Write ($"<div class='pdiv {ignoredClass}'>");
							writer.Write ($"<span id='button_{log_id}' class='expander' onclick='javascript: toggleLogVisibility (\"{log_id}\");'>{defaultExpander}</span>");
							// we have a very common error we want to make this easier for the person that is dealing with the results
							var knownFailure = string.Empty;
							if (test.KnownFailure.HasValue)
								knownFailure = $" <a href='{test.KnownFailure.Value.IssueLink}'>{test.KnownFailure.Value.HumanMessage}</a>";
							writer.Write ($"<span id='x{id_counter++}' class='p3 autorefreshable' onclick='javascript: toggleLogVisibility (\"{log_id}\");'>{title} (<span style='color: {test.GetTestColor ()}'>{state}{knownFailure}</span>{buildOnly}) </span>");
							if (IsServerMode) {
								writer.Write ($" <span id='x{id_counter++}' class='autorefreshable'>");
								if (test.Waiting) {
									writer.Write ($" <a class='runall' href='javascript:stoptest ({test.ID})'>Stop</a> ");
								} else if (test.InProgress && !test.Built) {
									// Stopping is not implemented for tasks that are already executing
								} else {
									writer.Write ($" <a class='runall' href='javascript:runtest ({test.ID})'>Run</a> ");
									writer.Write ($" <a class='runall' href='javascript:buildtest ({test.ID})'>Build</a> ");
								}
								writer.Write ("</span> ");
							}
							writer.WriteLine ("</div>");
							writer.WriteLine ($"<div id='logs_{log_id}' class='autorefreshable logs togglable' data-onautorefresh='{log_id}' style='display: {defaultDisplay};'>");

							var testAssemblies = test.ReferencedNunitAndXunitTestAssemblies;
							if (testAssemblies.Any ())
								writer.WriteLine ($"Test assemblies:<br/>- {String.Join ("<br/>- ", testAssemblies)}<br />");

							if (test.KnownFailure.HasValue)
								writer.WriteLine ($"Known failure: <a href='{test.KnownFailure.Value.IssueLink}'>{test.KnownFailure.Value.HumanMessage}</a> <br />");

							if (!string.IsNullOrEmpty (test.FailureMessage)) {
								var msg = test.FailureMessage.AsHtml ();
								var prefix = test.Ignored ? "Ignored" : "Failure";
								if (test.FailureMessage.Contains ('\n')) {
									writer.WriteLine ($"{prefix}:<br /> <div style='margin-left: 20px;'>{msg}</div>");
								} else {
									writer.WriteLine ($"{prefix}: {msg} <br />");
								}
							}
							var progressMessage = test.ProgressMessage;
							if (!string.IsNullOrEmpty (progressMessage))
								writer.WriteLine (progressMessage.AsHtml () + " <br />");

							if (runTest != null) {
								if (runTest.BuildTask.Duration.Ticks > 0) {
									writer.WriteLine ($"Project file: {runTest.BuildTask.ProjectFile} <br />");
									writer.WriteLine ($"Platform: {runTest.BuildTask.ProjectPlatform} Configuration: {runTest.BuildTask.ProjectConfiguration} <br />");
									IEnumerable<IDevice> candidates = (runTest as RunDeviceTask)?.Candidates;
									if (candidates == null)
										candidates = (runTest as RunSimulatorTask)?.Candidates;
									if (candidates != null) {
										writer.WriteLine ($"Candidate devices:<br />");
										foreach (var candidate in candidates)
											writer.WriteLine ($"&nbsp;&nbsp;&nbsp;&nbsp;{candidate.Name} (Version: {candidate.OSVersion})<br />");
									}
									writer.WriteLine ($"Build duration: {runTest.BuildTask.Duration} <br />");
								}
								if (test.Duration.Ticks > 0)
									writer.WriteLine ($"Time Elapsed:  {test.TestName} - (waiting time : {test.WaitingDuration} , running time : {test.Duration}) <br />");
								var runDeviceTest = runTest as RunDeviceTask;
								if (runDeviceTest?.Device != null) {
									if (runDeviceTest.CompanionDevice != null) {
										writer.WriteLine ($"Device: {runDeviceTest.Device.Name} ({runDeviceTest.CompanionDevice.Name}) <br />");
									} else {
										writer.WriteLine ($"Device: {runDeviceTest.Device.Name} <br />");
									}
								}
							} else {
								if (test.Duration.Ticks > 0)
									writer.WriteLine ($"Duration: {test.Duration} <br />");
							}

							if (logs.Count () > 0) {
								foreach (var log in logs) {
									log.Flush ();
									var exists = File.Exists (log.FullPath);
									string log_type = System.Web.MimeMapping.GetMimeMapping (log.FullPath);
									string log_target;
									switch (log_type) {
									case "text/xml":
										log_target = "_top";
										break;
									case "text/plain":
										log_type += ";charset=UTF-8";
										goto default;
									default:
										log_target = "_self";
										break;
									}
									if (!exists) {
										writer.WriteLine ("<a href='{0}' type='{2}' target='{3}'>{1}</a> (does not exist)<br />", LinkEncode (log.FullPath.Substring (LogDirectory.Length + 1)), log.Description, log_type, log_target);
									} else if (log.Description == LogType.BuildLog.ToString ()) {
										var binlog = log.FullPath.Replace (".txt", ".binlog");
										if (File.Exists (binlog)) {
											var textLink = string.Format ("<a href='{0}' type='{2}' target='{3}'>{1}</a>", LinkEncode (log.FullPath.Substring (LogDirectory.Length + 1)), log.Description, log_type, log_target);
											var binLink = string.Format ("<a href='{0}' type='{2}' target='{3}' style='display:{4}'>{1}</a><br />", LinkEncode (binlog.Substring (LogDirectory.Length + 1)), "Binlog download", log_type, log_target, test.Building ? "none" : "inline");
											writer.Write ("{0} {1}", textLink, binLink);
										} else {
											writer.WriteLine ("<a href='{0}' type='{2}' target='{3}'>{1}</a><br />", LinkEncode (log.FullPath.Substring (LogDirectory.Length + 1)), log.Description, log_type, log_target);
										}
									} else {
										writer.WriteLine ("<a href='{0}' type='{2}' target='{3}'>{1}</a><br />", LinkEncode (log.FullPath.Substring (LogDirectory.Length + 1)), log.Description, log_type, log_target);
									}
									if (!exists) {
										// Don't try to parse files that don't exist
									} else if (log.Description == LogType.TestLog.ToString () || log.Description ==  LogType.ExecutionLog.ToString () || log.Description == LogType.ExecutionLog.ToString ()) {
										string summary;
										List<string> fails;
										try {
											using (var reader = log.GetReader ()) {
												Tuple<long, object> data;
												if (!log_data.TryGetValue (log, out data) || data.Item1 != reader.BaseStream.Length) {
													summary = string.Empty;
													fails = new List<string> ();
													while (!reader.EndOfStream) {
														string line = reader.ReadLine ()?.Trim ();
														if (line == null)
															continue;
														if (line.StartsWith ("Tests run:", StringComparison.Ordinal)) {
															summary = line;
														} else if (line.StartsWith ("[FAIL]", StringComparison.Ordinal)) {
															if (fails.Count < 100) {
																fails.Add (line);
															} else if (fails.Count == 100) {
																fails.Add ("...");
															}
														}
													}
												} else {
													var data_tuple = (Tuple<string, List<string>>) data.Item2;
													summary = data_tuple.Item1;
													fails = data_tuple.Item2;
												}
											}
											if (fails.Count > 0) {
												writer.WriteLine ("<div style='padding-left: 15px;'>");
												foreach (var fail in fails)
													writer.WriteLine ("{0} <br />", fail.AsHtml ());
												writer.WriteLine ("</div>");
											}
											if (!string.IsNullOrEmpty (summary))
												writer.WriteLine ("<span style='padding-left: 15px;'>{0}</span><br />", summary);
										} catch (Exception ex) {
											writer.WriteLine ("<span style='padding-left: 15px;'>Could not parse log file: {0}</span><br />", ex.Message.AsHtml ());
										}
									} else if (log.Description == LogType.BuildLog.ToString ()) {
										HashSet<string> errors;
										try {
											using (var reader = log.GetReader ()) {
												Tuple<long, object> data;
												if (!log_data.TryGetValue (log, out data) || data.Item1 != reader.BaseStream.Length) {
													errors = new HashSet<string> ();
													while (!reader.EndOfStream) {
														string line = reader.ReadLine ()?.Trim ();
														if (line == null)
															continue;
														// Sometimes we put error messages in pull request descriptions
														// Then Jenkins create environment variables containing the pull request descriptions (and other pull request data)
														// So exclude any lines matching 'ghprbPull', to avoid reporting those environment variables as build errors.
														if (line.Contains (": error") && !line.Contains ("ghprbPull")) {
															errors.Add (line);
															if (errors.Count > 20) {
																errors.Add ("...");
																break;
															}
														}
													}
													log_data [log] = new Tuple<long, object> (reader.BaseStream.Length, errors);
												} else {
													errors = (HashSet<string>) data.Item2;
												}
											}
											if (errors.Count > 0) {
												writer.WriteLine ("<div style='padding-left: 15px;'>");
												foreach (var error in errors)
													writer.WriteLine ("{0} <br />",  error.AsHtml ());
												writer.WriteLine ("</div>");
											}
										} catch (Exception ex) {
											writer.WriteLine ("<span style='padding-left: 15px;'>Could not parse log file: {0}</span><br />", ex.Message.AsHtml ());
										}
									} else if (log.Description == LogType.NUnitResult.ToString () || log.Description == LogType.XmlLog.ToString () ) {
										try {
											if (File.Exists (log.FullPath) && new FileInfo (log.FullPath).Length > 0) {
												if (resultParser.IsValidXml (log.FullPath, out var jargon)) {
													resultParser.GenerateTestReport (writer, log.FullPath, jargon);
												}
											}
										} catch (Exception ex) {
											writer.WriteLine ($"<span style='padding-left: 15px;'>Could not parse {log.Description}: {ex.Message.AsHtml ()}</span><br />");
										}
									}
								}
							}
							writer.WriteLine ("</div>");
						}
						if (multipleModes)
							writer.WriteLine ("</div>");
					}
					if (!singleTask)
						writer.WriteLine ("</div>");
				}
				writer.WriteLine ("</div>");
				if (IsServerMode) {
					writer.WriteLine ("<div id='test-status' style='margin-left: 100px;' class='autorefreshable'>");
					if (failedTests.Count () == 0) {
						foreach (var group in failedTests.GroupBy ((v) => v.TestName)) {
							var enumerableGroup = group as IEnumerable<AppleTestTask>;
							if (enumerableGroup != null) {
								writer.WriteLine ("<a href='#test_{2}'>{0}</a> ({1})<br />", group.Key, string.Join (", ", enumerableGroup.Select ((v) => string.Format ("<span style='color: {0}'>{1}</span>", v.GetTestColor (), string.IsNullOrEmpty (v.Mode) ? v.ExecutionResult.ToString () : v.Mode)).ToArray ()), group.Key.Replace (' ', '-'));
								continue;
							}

							throw new NotImplementedException ();
						}
					}

					if (buildingTests.Any ()) {
						writer.WriteLine ($"<h3>{buildingTests.Count ()} building tests:</h3>");
						foreach (var test in buildingTests) {
							var runTask = test as RunTestTask;
							var buildDuration = string.Empty;
							if (runTask != null)
								buildDuration = runTask.BuildTask.Duration.ToString ();
							writer.WriteLine ($"<a href='#test_{test.TestName}'>{test.TestName} ({test.Mode})</a> {buildDuration}<br />");
						}
					}

					if (runningTests.Any ()) {
						writer.WriteLine ($"<h3>{runningTests.Count ()} running tests:</h3>");
						foreach (var test in runningTests) {
							writer.WriteLine ($"<a href='#test_{test.TestName}'>{test.TestName} ({test.Mode})</a> {test.Duration.ToString ()} {("\n\t" + test.ProgressMessage).AsHtml ()}<br />");
						}
					}

					if (buildingQueuedTests.Any ()) {
						writer.WriteLine ($"<h3>{buildingQueuedTests.Count ()} tests in build queue:</h3>");
						foreach (var test in buildingQueuedTests) {
							writer.WriteLine ($"<a href='#test_{test.TestName}'>{test.TestName} ({test.Mode})</a><br />");
						}
					}

					if (runningQueuedTests.Any ()) {
						writer.WriteLine ($"<h3>{runningQueuedTests.Count ()} tests in run queue:</h3>");
						foreach (var test in runningQueuedTests) {
							writer.WriteLine ($"<a href='#test_{test.TestName}'>{test.TestName} ({test.Mode})</a><br />");
						}
					}

					var resources = device_resources.Values.Concat (new Resource [] { DesktopResource, NugetResource });
					if (resources.Any ()) {
						writer.WriteLine ($"<h3>Devices/Resources:</h3>");
						foreach (var dr in resources.OrderBy ((v) => v.Description, StringComparer.OrdinalIgnoreCase)) {
							writer.WriteLine ($"{dr.Description} - {dr.Users}/{dr.MaxConcurrentUsers} users - {dr.QueuedUsers} in queue<br />");
						}
					}
				}
				writer.WriteLine ("</div>");
				writer.WriteLine ("</div>");
				writer.WriteLine ("</body>");
				writer.WriteLine ("</html>");
			}
		}
		Dictionary<ILog, Tuple<long, object>> log_data = new Dictionary<ILog, Tuple<long, object>> ();

		static string LinkEncode (string path)
		{
			return System.Web.HttpUtility.UrlEncode (path).Replace ("%2f", "/").Replace ("+", "%20");
		}

		string RenderTextStates (IEnumerable<ITestTask> tests)
		{
			// Create a collection of all non-ignored tests in the group (unless all tests were ignored).
			var allIgnored = tests.All ((v) => v.ExecutionResult == TestExecutingResult.Ignored);
			IEnumerable<ITestTask> relevantGroup;
			if (allIgnored) {
				relevantGroup = tests;
			} else {
				relevantGroup = tests.Where ((v) => v.ExecutionResult != TestExecutingResult.NotStarted);
			}
			if (!relevantGroup.Any ())
				return string.Empty;
			
			var results = relevantGroup
				.GroupBy ((v) => v.ExecutionResult)
				.Select ((v) => v.First ()) // GroupBy + Select = Distinct (lambda)
				.OrderBy ((v) => v.ID)
				.Select ((v) => $"<span style='color: {v.GetTestColor ()}'>{v.ExecutionResult.ToString ()}</span>")
				.ToArray ();
			return " (" + string.Join ("; ", results) + ")";
		}
	}
}
