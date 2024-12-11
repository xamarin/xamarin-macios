using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Xharness.Jenkins.TestTasks;

#nullable enable

namespace Xharness.Jenkins {
	abstract class TaskFactory {
		protected Jenkins Jenkins { get; private set; }
		protected IMlaunchProcessManager ProcessManager { get; private set;  }
		protected TestVariationsFactory TestVariationsFactory { get; private set; }

		public TaskFactory (Jenkins jenkins, IMlaunchProcessManager processManager, TestVariationsFactory testVariationsFactory)
		{
			Jenkins = jenkins;
			ProcessManager = processManager;
			TestVariationsFactory = testVariationsFactory;
		}

		public abstract Task<IEnumerable<AppleTestTask>> CreateTasksAsync ();
	}

	class NUnitTestTasksEnumerable : TaskFactory {
		public NUnitTestTasksEnumerable (Jenkins jenkins, IMlaunchProcessManager processManager, TestVariationsFactory testVariationsFactory)
		: base (jenkins, processManager, testVariationsFactory)
		{
		}

		public override Task<IEnumerable<AppleTestTask>> CreateTasksAsync ()
		{
			var jenkins = this.Jenkins;
			var processManager = this.ProcessManager;

			var rv = new List<RunTestTask> ();
			foreach (var project in jenkins.Harness.TestProjects) {
				if (project.IsExecutableProject)
					continue;

				var build = new MSBuildTask (jenkins: jenkins, testProject: project, processManager: processManager) {
					SpecifyPlatform = false,
					ProjectConfiguration = "Debug",
					Platform = project.TestPlatform,
					Ignored = !jenkins.TestSelection.IsEnabled (project.Label),
					SupportsParallelExecution = false,
					TestName = project.Name,
				};
				var run = new DotNetTestTask (jenkins, build, processManager) {
					TestProject = project,
					Platform = build.Platform,
					TestName = project.Name,
					Timeout = project.Timeout,
					Ignored = !jenkins.TestSelection.IsEnabled (project.Label),
					SupportsParallelExecution = false,
				};
				rv.Add (run);
			}
			return Task.FromResult<IEnumerable<AppleTestTask>> (rv);
		}
	}
}
