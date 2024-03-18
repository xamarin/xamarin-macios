using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
		public TestSelection TestSelection { get; } = new ();
		readonly TestSelector testSelector;
		readonly TestVariationsFactory testVariationsFactory;
		public JenkinsDeviceLoader DeviceLoader { get; private set; }
		readonly ResourceManager resourceManager;
		readonly DateTime startTimeUtc = DateTime.UtcNow;

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
			if (!project.IsExecutableProject) {
				MainLog.WriteLine ($"Ignoring {project.Name} with label {project.Label} because is not a executable project.");
				return false;
			}

			if (!TestSelection.IsEnabled (TestLabel.SystemPermission) && project.Label == TestLabel.Introspection) {
				MainLog.WriteLine ($"Ignoring {project.Name} with label {project.Label} because we cannot include the system permission tests");
				return false;
			}

			if (project.IsDotNetProject && !TestSelection.IsEnabled (PlatformLabel.Dotnet)) {
				MainLog.WriteLine ($"Ignoring {project.Name} with label {project.Label} because it's a .NET project and .NET is not included.");
				return false;
			}

			if (!project.IsDotNetProject && !TestSelection.IsEnabled (PlatformLabel.LegacyXamarin)) {
				MainLog.WriteLine ($"Ignoring {project.Name} with label {project.Label} because it's a legacy Xamarin project and legacy Xamarin projects are not included.");
				return false;
			}

			var rv = TestSelection.IsEnabled (project.Label);
			MainLog.WriteLine ($"Including {project.Name} with label {project.Label.ToString ()}: {rv}");
			return rv;
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
				Ignored = !TestSelection.IsEnabled (TestLabel.Xtro) || !TestSelection.IsEnabled (PlatformLabel.LegacyXamarin),
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
				Ignored = !(TestSelection.IsEnabled (TestLabel.Xtro) && TestSelection.IsEnabled (PlatformLabel.Dotnet)),
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

			var buildDotNetGeneratorProject = new TestProject (TestLabel.Generator, Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory, "bgen", "bgen-tests.csproj"))) {
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
				Ignored = !TestSelection.IsEnabled (TestLabel.Generator) || !TestSelection.IsEnabled (PlatformLabel.Dotnet),
			};
			Tasks.Add (runDotNetGenerator);

			var buildDotNetTestsProject = new TestProject (TestLabel.DotnetTest, Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory, "dotnet", "UnitTests", "DotNetUnitTests.csproj"))) {
				IsDotNetProject = true,
			};
			var buildDotNetTests = new MSBuildTask (this, testProject: buildDotNetTestsProject, processManager: processManager) {
				SpecifyPlatform = false,
				Platform = TestPlatform.All,
				ProjectConfiguration = "Debug",
				Ignored = !TestSelection.IsEnabled (TestLabel.DotnetTest),
			};
			var runDotNetTests = new DotNetTestTask (this, buildDotNetTests, processManager) {
				TestProject = buildDotNetTestsProject,
				Platform = TestPlatform.All,
				TestName = "DotNet tests",
				Filter = "Category!=Windows",
				Timeout = TimeSpan.FromMinutes (360),
				Ignored = !TestSelection.IsEnabled (TestLabel.DotnetTest) || !TestSelection.IsEnabled (PlatformLabel.Dotnet),
			};
			Tasks.Add (runDotNetTests);

			var deviceTestFactory = new RunDeviceTasksFactory ();
			var loaddev = deviceTestFactory.CreateAsync (this, processManager, testVariationsFactory).ContinueWith ((v) => {
				Console.WriteLine ("Got device tasks completed");
				Tasks.AddRange (v.Result);
			});

			return Task.WhenAll (loadsim, loaddev);
		}

		[DllImport ("libc")]
		static extern int getloadavg (double [] avg, int nelem);

		async Task<(bool Success, string Output)> GetProcessListAsync (ILog log)
		{
			try {
				using var ps = new Process ();
				ps.StartInfo.FileName = "ps";
				ps.StartInfo.Arguments = "auxww";
				var output = new MemoryLog () {
					Timestamp = false,
				};
				var rv = await processManager.RunAsync (ps, log, stdoutLog: output, stderrLog: output, timeout: TimeSpan.FromMinutes (1));
				if (!rv.Succeeded) {
					log.WriteLine ($"Failed to list processes, 'ps auxww' exited with exit code {rv.ExitCode}");
					return new (false, $"'ps auxww' exited with exit code {rv.ExitCode}");
				}

				var allLines = output.ToString ().Split ('\n');
				// First line contains headers
				var headers = allLines [0].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				var processes = new List<string []> (allLines.Length);
				processes.Add (headers);
				for (var p = 1; p < allLines.Length; p++) {
					var line = allLines [p].Trim ();
					if (string.IsNullOrEmpty (line))
						continue;

					// Split on space to get the fields for the process
					var fields = line.Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (fields.Length < headers.Length) {
						// shouldn't happen - not enough fields?
						processes.Add (fields);
						continue;
					}
					// Reconstruct the 'COMMAND' field, it's all the remaining entries in the array
					for (var f = headers.Length; f < fields.Length; f++) {
						fields [headers.Length - 1] += " " + fields [f];
					}
					Array.Resize (ref fields, headers.Length);
					processes.Add (fields);
				}

				var fieldWidth = new int [headers.Length];
				for (var p = 0; p < processes.Count; p++) {
					var fields = processes [p];
					for (var i = 0; i < fields.Length; i++) {
						fieldWidth [i] = Math.Max (fieldWidth [i], fields [i]?.Length ?? 0);
					}
				}

				var sb = new StringBuilder ();
				for (var p = 0; p < processes.Count; p++) {
					var fields = processes [p];

					for (var f = 0; f < fields.Length - 1; f++) {
						sb.Append (' ', fieldWidth [f] - (fields [f]?.Length ?? 0));
						sb.Append (fields [f]);
						sb.Append (' ');
					}

					// last is COMMAND, no padding
					sb.Append (fields [fields.Length - 1]);
					sb.AppendLine ();
				}

				return new (true, sb.ToString ());
			} catch (Exception e) {
				log.WriteLine ($"Failed to list processes: {e.Message}");
				return new (false, e.Message);
			}
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
							var averages = new double [3];
							getloadavg (averages, averages.Length);
							Console.WriteLine ($"{DateTime.UtcNow.ToString ("O")} Still running tests. Please be patient. Load averages: {averages [0],6:0.00} {averages [1],6:0.00} {averages [2],6:0.00}");
							if (averages [0] > 10) {
								var rv = await GetProcessListAsync (MainLog);
								if (rv.Success) {
									Console.WriteLine ($"{DateTime.UtcNow.ToString ("O")} Current process list from 'ps auxww' (due to high load average):\n\t\t{string.Join ("\n\t\t", rv.Output.Split ('\n'))}");
								} else {
									Console.WriteLine ($"{DateTime.UtcNow.ToString ("O")} Failed to list processes (due to high load average): {rv.Output}");
								}
							}
						}
					});

					Task.Factory.StartNew (async () => {
						while (true) {
							var rv = await GetProcessListAsync (MainLog);
							if (rv.Success) {
								Console.WriteLine ($"{DateTime.UtcNow.ToString ("O")} Current process list from 'ps auxww':\n\t\t{string.Join ("\n\t\t", rv.Output.Split ('\n'))}");
							} else {
								Console.WriteLine ($"{DateTime.UtcNow.ToString ("O")} Failed to list processes: {rv.Output}");
							}
							await Task.Delay (TimeSpan.FromHours (1));
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

				Console.WriteLine ("Summary:");
				Console.WriteLine ($"    Executed {Tasks.Count} tasks");
				Console.WriteLine ($"    Succeeded: {Tasks.Count (v => v.Succeeded)}");
				Console.WriteLine ($"    Failed: {Tasks.Count (v => v.Failed)}");
				Console.WriteLine ($"    Crashed: {Tasks.Count (v => v.Crashed)}");
				Console.WriteLine ($"    TimedOut: {Tasks.Count (v => v.TimedOut)}");
				Console.WriteLine ($"    DeviceNotFound: {Tasks.Count (v => v.DeviceNotFound)}");
				Console.WriteLine ($"    NotStarted: {Tasks.Count (v => v.NotStarted)}");

				return Tasks.Any ((v) => v.Failed || v.DeviceNotFound) ? 1 : 0;
			} catch (Exception ex) {
				MainLog.WriteLine ("Unexpected exception: {0}", ex);
				Console.WriteLine ("Unexpected exception: {0}", ex);
				return 2;
			} finally {
				CollectCrashReports ();
			}
		}

		// Collect any crash reports that were created during the test run
		void CollectCrashReports ()
		{
			try {
				var dir = Path.Combine (Environment.GetEnvironmentVariable ("HOME"), "Library", "Logs", "DiagnosticReports");
				var reports = Directory.GetFiles (dir).Select (v => {
					(string Path, DateTime LastWriteTimeUtc) rv = (v, File.GetLastWriteTimeUtc (v));
					return rv;
				}).ToArray ();
				MainLog.WriteLine ($"Found {reports.Length} crash reports in {dir} (the ones marked with 'x' occurred during this test run):");
				foreach (var report in reports.OrderBy (v => v)) {
					MainLog.WriteLine ($"  {(report.LastWriteTimeUtc > startTimeUtc ? "x" : " ")}  {report.LastWriteTimeUtc.ToString ("u")} {report.Path}");
				}
				var targetDir = Path.Combine (LogDirectory, "DiagnosticReports");
				Directory.CreateDirectory (targetDir);
				foreach (var report in reports.Where (v => v.LastWriteTimeUtc >= startTimeUtc)) {
					File.Copy (report.Path, Path.Combine (targetDir, Path.GetFileName (report.Path)));
				}
			} catch (Exception e) {
				MainLog.WriteLine ($"Failed to collect crash reports: {e}");
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
						if (aggregated is not null) {
							allSimulatorTasks.AddRange (aggregated.Tasks);
							continue;
						}

						var execute = task as MacExecuteTask;
						if (execute is not null) {
							allExecuteTasks.Add (execute);
							continue;
						}

						var nunit = task as NUnitExecuteTask;
						if (nunit is not null) {
							allNUnitTasks.Add (nunit);
							continue;
						}

						var make = task as MakeTask;
						if (make is not null) {
							allMakeTasks.Add (make);
							continue;
						}

						var run_device = task as RunDeviceTask;
						if (run_device is not null) {
							allDeviceTasks.Add (run_device);
							continue;
						}

						var simulator = task as RunSimulatorTask;
						if (simulator is not null) {
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
					if (vsdropsHtmlReportWriter is not null) {
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

					if (vsdropsHtmlReportWriter is not null) {
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
