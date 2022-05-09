using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Xharness.Jenkins.Reports;
using Xharness.Jenkins.TestTasks;

namespace Xharness.Jenkins {
	class Jenkins {
		public readonly ISimulatorLoader Simulators;
		public readonly IHardwareDeviceLoader Devices;
		readonly IMlaunchProcessManager processManager;
		public ITunnelBore TunnelBore { get; private set; }
		public TestSelection TestSelection { get; } = new();
		readonly TestSelector testSelector;
		readonly TestVariationsFactory testVariationsFactory;
		public JenkinsDeviceLoader DeviceLoader { get; private set; }
		readonly ResourceManager resourceManager;

		// report writers, do need to be a class instance because the have state.
		readonly HtmlReportWriter xamarinStorageHtmlReportWriter;
		readonly HtmlReportWriter vsdropsHtmlReportWriter;
		readonly MarkdownReportWriter markdownReportWriter;
		
		public bool Populating { get; private set; } = true;

		public IHarness Harness { get; }
		public bool ForceExtensionBuildOnly;

		public bool CleanSuccessfulTestRuns = true;
		public bool UninstallTestApp = true;

		public IFileBackedLog MainLog;
		public ILog SimulatorLoadLog => DeviceLoader.SimulatorLoadLog;
		public ILog DeviceLoadLog => DeviceLoader.DeviceLoadLog;

		string log_directory;
		public string LogDirectory {
			get {
				if (string.IsNullOrEmpty (log_directory)) {
					log_directory = Path.Combine (Harness.JENKINS_RESULTS_DIRECTORY, "tests");
					if (IsServerMode)
						log_directory = Path.Combine (log_directory, Xharness.Harness.Helpers.Timestamp);
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

		public Jenkins (IHarness harness, IMlaunchProcessManager processManager, IResultParser resultParser, ITunnelBore tunnelBore)
		{
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.TunnelBore = tunnelBore ?? throw new ArgumentNullException (nameof (tunnelBore));
			Harness = harness ?? throw new ArgumentNullException (nameof (harness));
			Simulators = new SimulatorLoader (processManager, new SimulatorSelector ());
			Devices = new HardwareDeviceLoader (processManager);
			testSelector = new TestSelector (this, new GitHub (harness, processManager));
			testVariationsFactory = new TestVariationsFactory (this, processManager);
			DeviceLoader = new JenkinsDeviceLoader (Simulators, Devices, Logs);
			resourceManager = new ResourceManager ();
			xamarinStorageHtmlReportWriter = new HtmlReportWriter (jenkins: this, resourceManager: resourceManager, resultParser: resultParser);
			// we only care about the vsdrops writer if we are in the CI, locally makes no sense
			if (harness.InCI && !string.IsNullOrEmpty (Harness.VSDropsUri))
				vsdropsHtmlReportWriter = new HtmlReportWriter (this, resourceManager, resultParser, linksPrefix: Harness.VSDropsUri, embeddedResources: true);
			markdownReportWriter = new MarkdownReportWriter ();
		}

		public bool IsIncluded (TestProject project)
		{
			if (!project.IsExecutableProject)
				return false;
			
			if (project.IsBclTest ()) {
				if (!project.IsBclxUnit ())
					return TestSelection.IncludeBcl || TestSelection.IncludeBCLNUnit;
				if (project.IsMscorlib ()) 
					return TestSelection.IncludeMscorlib;
				return TestSelection.IncludeBcl || TestSelection.IncludeBCLxUnit;
			}

			if (!TestSelection.IncludeMonotouch && project.IsMonotouch ())
				return false;

			if (!TestSelection.IncludeNonMonotouch && !project.IsMonotouch ())
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

			testSelector.SelectTests (TestSelection);

			DeviceLoader.LoadAllAsync ().DoNotAwait ();

			var simTasksFactory = new RunSimulatorTasksFactory ();
			var loadsim = simTasksFactory.CreateAsync (this, processManager, testVariationsFactory)
				.ContinueWith ((v) => {
					if (v.Status == TaskStatus.RanToCompletion) {
						Console.WriteLine ("Simulator tasks created");
						Tasks.AddRange (v.Result);
					} else {
						Console.WriteLine ($"Failed to create simulator tasks: {v.Exception}");
					}
				});
			
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
				WorkingDirectory = Path.Combine (HarnessConfiguration.RootDirectory, "xtro-sharpie"),
				Ignored = !TestSelection.IncludeXtro,
				Timeout = TimeSpan.FromMinutes (15),
				SupportsParallelExecution = false,
			};

			var runXtroReporter = new RunXtroTask (this, buildXtroTests, processManager, crashReportSnapshotFactory) {
				Platform = TestPlatform.Mac,
				TestName = buildXtroTests.TestName,
				Mode = "Legacy Xamarin",
				Ignored = buildXtroTests.Ignored,
				WorkingDirectory = buildXtroTests.WorkingDirectory,
				AnnotationsDirectory = buildXtroTests.WorkingDirectory,
			};
			Tasks.Add (runXtroReporter);

			var buildDotNetXtroTests = new MakeTask (jenkins: this, processManager: processManager) {
				Platform = TestPlatform.All,
				TestName = "Xtro",
				Target = "dotnet-wrench",
				WorkingDirectory = Path.Combine (HarnessConfiguration.RootDirectory, "xtro-sharpie"),
				Ignored = !TestSelection.IncludeXtro && !TestSelection.IncludeDotNet,
				Timeout = TimeSpan.FromMinutes (15),
				SupportsParallelExecution = false,
			};

			var runDotNetXtroReporter = new RunXtroTask (this, buildDotNetXtroTests, processManager, crashReportSnapshotFactory) {
				Platform = TestPlatform.Mac,
				TestName = buildDotNetXtroTests.TestName,
				Mode = ".NET",
				Ignored = buildDotNetXtroTests.Ignored,
				WorkingDirectory = buildDotNetXtroTests.WorkingDirectory,
				AnnotationsDirectory = Path.Combine (buildDotNetXtroTests.WorkingDirectory, "api-annotations-dotnet"),
			};
			Tasks.Add (runDotNetXtroReporter);

			var buildDotNetGeneratorProject = new TestProject ("bgen", Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory, "bgen", "bgen-tests.csproj"))) {
				IsDotNetProject = true,
			};
			var buildDotNetGenerator = new MSBuildTask (jenkins: this, testProject: buildDotNetGeneratorProject, processManager: processManager) {
				TestProject = buildDotNetGeneratorProject,
				SpecifyPlatform = false,
				SpecifyConfiguration = false,
				Platform = TestPlatform.iOS,
			};
			var runDotNetGenerator = new DotNetTestTask (this, buildDotNetGenerator, processManager) {
				TestProject = buildDotNetGeneratorProject,
				Platform = TestPlatform.iOS,
				TestName = "Generator tests",
				Mode = ".NET",
				Ignored = !TestSelection.IncludeBtouch,
			};
			Tasks.Add (runDotNetGenerator);

			var buildDotNetTestsProject = new TestProject ("dotnet", Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory, "dotnet", "UnitTests", "DotNetUnitTests.csproj"))) {
				IsDotNetProject = true,
			};
			var buildDotNetTests = new MSBuildTask (this, testProject: buildDotNetTestsProject, processManager: processManager) {
				SpecifyPlatform = false,
				Platform = TestPlatform.All,
				ProjectConfiguration = "Debug",
				Ignored = !TestSelection.IncludeDotNet,
			};
			var runDotNetTests = new DotNetTestTask (this, buildDotNetTests, processManager) {
				TestProject = buildDotNetTestsProject,
				Platform = TestPlatform.All,
				TestName = "DotNet tests",
				Timeout = TimeSpan.FromMinutes (240),
				Ignored = !TestSelection.IncludeDotNet,
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
				IFileBackedLog log = Logs.Create ($"Harness-{Xharness.Harness.Helpers.Timestamp}.log", "Harness log");
				if (Harness.InCI)
					log = Log.CreateReadableAggregatedLog (log, new ConsoleLog ());
				Harness.HarnessLog = MainLog = log;

				var tasks = new List<Task> ();
				if (IsServerMode) {
					var testServer = new TestServer ();
					tasks.Add (testServer.RunAsync (this, xamarinStorageHtmlReportWriter));
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
			return processManager.ExecuteCommandAsync ("make", new [] { "all", $"-j{Environment.ProcessorCount}", "-C", Path.Combine (HarnessConfiguration.RootDirectory, "test-libraries") }, log, TimeSpan.FromMinutes (10)).ContinueWith ((v) => {
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
					var vsdropsReport = Path.Combine (LogDirectory, "vsdrops_index.html");
					var tmpreport = Path.Combine (LogDirectory, $"index-{Xharness.Harness.Helpers.Timestamp}.tmp.html");
					var tmpVsdropsReport = Path.Combine (LogDirectory, $"vsdrops_index-{Xharness.Harness.Helpers.Timestamp}.tmp.html");
					var tmpmarkdown = string.IsNullOrEmpty (Harness.MarkdownSummaryPath) ? string.Empty : (Harness.MarkdownSummaryPath + $".{Xharness.Harness.Helpers.Timestamp}.tmp");

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

						var simulator = task as RunSimulatorTask;
						if (simulator != null) {
							allSimulatorTasks.Add (simulator);
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
						xamarinStorageHtmlReportWriter.Write (allTasks, writer);
					}

					// write the vsdrops report only if needed
					if (vsdropsHtmlReportWriter != null) {
						using (var stream = new FileStream (tmpVsdropsReport, FileMode.Create, FileAccess.ReadWrite)) 
						using (var writer = new StreamWriter (stream)) { 
							vsdropsHtmlReportWriter.Write (allTasks, writer);
						}
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

					if (vsdropsHtmlReportWriter != null) {
						if (File.Exists (vsdropsReport))
							File.Delete (vsdropsReport);
						File.Move (tmpVsdropsReport, vsdropsReport);
					}
					
					if (!string.IsNullOrEmpty (tmpmarkdown)) {
						if (File.Exists (Harness.MarkdownSummaryPath))
							File.Delete (Harness.MarkdownSummaryPath);
						File.Move (tmpmarkdown, Harness.MarkdownSummaryPath);
					}
					
					var dependentFileLocation = Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly ().Location);
					foreach (var file in new string [] { "xharness.js", "xharness.css" }) {
						File.Copy (Path.Combine (dependentFileLocation, file), Path.Combine (LogDirectory, file), true);
					}
					File.Copy (Path.Combine (HarnessConfiguration.RootDirectory, "xharness", "favicon.ico"), Path.Combine (LogDirectory, "favicon.ico"), true);
				}
			} catch (Exception e) {
				this.MainLog.WriteLine ("Failed to write log: {0}", e);
			}
		}
	}
}
