using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.IO;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Xharness.Jenkins.TestTasks;

#nullable enable

namespace Xharness.Jenkins {
	class MacTestTasksFactory : TaskFactory {
		readonly ICrashSnapshotReporterFactory crashReportSnapshotFactory;

		public MacTestTasksFactory (Jenkins jenkins, IMlaunchProcessManager processManager, TestVariationsFactory testVariationsFactory, ICrashSnapshotReporterFactory crashReportSnapshotFactory)
		: base (jenkins, processManager, testVariationsFactory)
		{
			this.crashReportSnapshotFactory = crashReportSnapshotFactory;
		}

		public override Task<IEnumerable<AppleTestTask>> CreateTasksAsync ()
		{
			var jenkins = this.Jenkins;
			var processManager = this.ProcessManager;
			var testVariationsFactory = this.TestVariationsFactory;

			var rv = new List<AppleTestTask> ();

			foreach (var project in jenkins.Harness.TestProjects) {
				if (!project.IsExecutableProject)
					continue;

				bool ignored = false;
				switch (project.TestPlatform) {
				case TestPlatform.Mac:
					ignored |= !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac);
					break;
				case TestPlatform.MacCatalyst:
					ignored |= !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst);
					break;
				default:
					continue;
				}

				if (project.Ignore == true)
					ignored = true;

				if (!jenkins.IsIncluded (project))
					ignored = true;

				var configurations = project.Configurations;
				if (configurations is null)
					configurations = new string [] { "Debug" };

				foreach (var config in configurations) {
					var build = new MSBuildTask (jenkins: jenkins, testProject: project, processManager: processManager) {
						SpecifyPlatform = false,
						ProjectConfiguration = config,
						Platform = project.TestPlatform,
						Ignored = !jenkins.TestSelection.IsEnabled (project.Label),
						SupportsParallelExecution = false,
					};
					build.CloneTestProject (jenkins.MainLog, processManager, project, HarnessConfiguration.RootDirectory);

					var exec = new MacExecuteTask (jenkins, build, processManager, crashReportSnapshotFactory) {
						Ignored = ignored,
						TestName = project.Name,
						IsUnitTest = true,
					};

					var variations = testVariationsFactory.CreateTestVariations (
						new [] { exec },
						(buildTask, test, candidates) =>
							new MacExecuteTask (jenkins, buildTask, processManager, crashReportSnapshotFactory) {
								IsUnitTest = true,
							}
					);

					foreach (var v in variations) {
						v.Variation = string.IsNullOrEmpty (v.Variation) ? config : v.Variation;
					}

					rv.AddRange (variations);
				}
			}

			return Task.FromResult<IEnumerable<AppleTestTask>> (rv);
		}
	}
}
