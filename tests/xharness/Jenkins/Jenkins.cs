using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	class Jenkins {
		public readonly ISimulatorLoader Simulators;
		public readonly IHardwareDeviceLoader Devices;
		readonly IProcessManager processManager;
		public ITunnelBore TunnelBore { get; private set; }
		readonly TestSelector testSelector;
		readonly TestVariationsFactory testVariationsFactory;
		public JenkinsDeviceLoader DeviceLoader { get; private set; }
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
		public bool IncludeiOS32 = true;
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
		public bool IncludeDotNet;

		public bool CleanSuccessfulTestRuns = true;
		public bool UninstallTestApp = true;

		public ILog MainLog;
		public ILog SimulatorLoadLog => DeviceLoader.SimulatorLoadLog;
		public ILog DeviceLoadLog => DeviceLoader.DeviceLoadLog;

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

		public List<ITestTask> Tasks { get; private set; } = new List<ITestTask> ();
		Dictionary<string, MakeTask> DependencyTasks = new Dictionary<string, MakeTask> ();

		public IErrorKnowledgeBase ErrorKnowledgeBase => new ErrorKnowledgeBase ();
		public IResourceManager ResourceManager => resourceManager;

		public Jenkins (Harness harness, IProcessManager processManager, IResultParser resultParser, ITunnelBore tunnelBore)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.TunnelBore = tunnelBore ?? throw new ArgumentNullException (nameof (tunnelBore));
			Harness = harness ?? throw new ArgumentNullException (nameof (harness));
			Simulators = new SimulatorLoader (processManager);
			Devices = new HardwareDeviceLoader (processManager);
			testSelector = new TestSelector (this, processManager, new GitHub (harness, processManager));
			testVariationsFactory = new TestVariationsFactory (this, processManager);
			DeviceLoader = new JenkinsDeviceLoader (Simulators, Devices, Logs);
			resourceManager = new ResourceManager ();
			htmlReportWriter = new HtmlReportWriter (jenkins: this, resourceManager: resourceManager, resultParser: resultParser);
			markdownReportWriter = new MarkdownReportWriter ();
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

		public bool IsBetaXcode => Harness.XcodeRoot.IndexOf ("beta", StringComparison.OrdinalIgnoreCase) >= 0;
		
		Task PopulateTasksAsync ()
		{
			// Missing:
			// api-diff

			testSelector.SelectTests ();

			DeviceLoader.LoadAllAsync ().DoNotAwait ();

			var simTasksFactory = new RunSimulatorTasksFactory ();
			var loadsim = simTasksFactory.CreateAsync (this, processManager, testVariationsFactory)
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

			var buildDotNetTestsProject = new TestProject (Path.GetFullPath (Path.Combine (Harness.RootDirectory, "dotnet", "UnitTests", "DotNetUnitTests.csproj")));
			var buildDotNetTests = new DotNetBuildTask (this, testProject: buildDotNetTestsProject, processManager: processManager) {
				SpecifyPlatform = false,
				Platform = TestPlatform.All,
				ProjectConfiguration = "Debug",
				Ignored = !IncludeDotNet,
			};
			var runDotNetTests = new DotNetTestTask (this, buildDotNetTests, processManager) {
				TestProject = buildDotNetTestsProject,
				Platform = TestPlatform.All,
				TestName = "DotNet tests",
				Timeout = TimeSpan.FromMinutes (5),
				Ignored = !IncludeDotNet,
			};
			Tasks.Add (runDotNetTests);

			var deviceTestFactory = new RunDeviceTasksFactory ();
			var loaddev = deviceTestFactory.CreateAsync (this, processManager, testVariationsFactory).ContinueWith ((v) => {
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
				if (IsServerMode) {
					var testServer = new TestServer ();
					tasks.Add (testServer.RunAsync (this, htmlReportWriter));
				}

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
