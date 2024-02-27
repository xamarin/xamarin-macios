using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Xharness.Jenkins.TestTasks;

namespace Xharness.Jenkins {
	class MacTestTasksEnumerable : IEnumerable<RunTestTask> {

		readonly Jenkins jenkins;
		readonly IMlaunchProcessManager processManager;
		readonly ICrashSnapshotReporterFactory crashReportSnapshotFactory;
		readonly ITestVariationsFactory testVariationsFactory;

		public MacTestTasksEnumerable (Jenkins jenkins,
									IMlaunchProcessManager processManager,
									ICrashSnapshotReporterFactory crashReportSnapshotFactory,
									ITestVariationsFactory testVariationsFactory)
		{
			this.jenkins = jenkins ?? throw new ArgumentNullException (nameof (jenkins));
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
			this.crashReportSnapshotFactory = crashReportSnapshotFactory ?? throw new ArgumentNullException (nameof (crashReportSnapshotFactory));
			this.testVariationsFactory = testVariationsFactory ?? throw new ArgumentNullException (nameof (testVariationsFactory));
		}

		NUnitExecuteTask CreateNUnitTask (MacTestProject project, MSBuildTask build, bool ignored)
		{
			if (project is null)
				throw new ArgumentNullException (nameof (project));
			if (build is null)
				throw new ArgumentNullException (nameof (build));

			var dll = Path.Combine (Path.GetDirectoryName (build.TestProject.Path), project.Xml.GetOutputAssemblyPath (build.ProjectPlatform, build.ProjectConfiguration).Replace ('\\', '/'));
			return new NUnitExecuteTask (jenkins, build, processManager) {
				Ignored = ignored,
				TestLibrary = dll,
				TestProject = project,
				Platform = build.Platform,
				TestName = project.Name,
				Timeout = TimeSpan.FromMinutes (120),
				Mode = "macOS",
			};
		}

		IEnumerable<MacExecuteTask> CreateMacExecuteTask (MacTestProject project, MSBuildTask build, bool ignored)
		{
			if (project is null)
				throw new ArgumentNullException (nameof (project));
			if (build is null)
				throw new ArgumentNullException (nameof (build));

			var exec = new MacExecuteTask (jenkins, build, processManager, crashReportSnapshotFactory) {
				Ignored = ignored,
				BCLTest = project.Label == TestLabel.Bcl,
				TestName = project.Name,
				IsUnitTest = true,
			};
			return testVariationsFactory.CreateTestVariations (new [] { exec }, (buildTask, test, candidates) =>
				new MacExecuteTask (jenkins, buildTask, processManager, crashReportSnapshotFactory) { IsUnitTest = true });
		}

		public IEnumerator<RunTestTask> GetEnumerator ()
		{

			foreach (var project in jenkins.Harness.MacTestProjects) {
				bool ignored = false;

				if (project.TestPlatform == TestPlatform.MacCatalyst) {
					ignored |= !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst);
				} else {
					ignored |= !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac);
				}

				if (project.Ignore == true)
					ignored = true;

				if (!jenkins.TestSelection.IsEnabled (TestLabel.Mmp) && project.Path.Contains ("mmptest"))
					ignored = true;

				if (!jenkins.IsIncluded (project))
					ignored = true;

				var configurations = project.Configurations;
				if (configurations is null)
					configurations = new string [] { "Debug" };

				TestPlatform platform = project.TargetFrameworkFlavors.ToTestPlatform ();
				foreach (var config in configurations) {
					MSBuildTask build = new MSBuildTask (jenkins: jenkins, testProject: project, processManager: processManager);
					build.Platform = platform;
					build.CloneTestProject (jenkins.MainLog, processManager, project, HarnessConfiguration.RootDirectory);
					build.ProjectConfiguration = config;
					build.ProjectPlatform = project.Platform;
					build.SpecifyPlatform = false;
					build.SpecifyConfiguration = build.ProjectConfiguration != "Debug";
					build.Dependency = project.Dependency;

					var ignored_main = ignored;
					var execs = project.IsNUnitProject
						? (IEnumerable<RunTestTask>) new [] { CreateNUnitTask (project, build, ignored_main) }
						: CreateMacExecuteTask (project, build, ignored_main);

					foreach (var e in execs) {
						e.Variation = string.IsNullOrEmpty (e.Variation) ? config : e.Variation;
						yield return e;
					}

				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

	}
}
