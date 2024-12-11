using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Xharness.Jenkins.TestTasks;

namespace Xharness.Jenkins {
	class RunDeviceTasksFactory  : TaskFactory {
		public RunDeviceTasksFactory (Jenkins jenkins, IMlaunchProcessManager processManager, TestVariationsFactory testVariationsFactory)
		: base (jenkins, processManager, testVariationsFactory)
		{
		}

		public override Task<IEnumerable<AppleTestTask>> CreateTasksAsync ()
		{
			var jenkins = this.Jenkins;
			var processManager = this.ProcessManager;
			var testVariationsFactory = this.TestVariationsFactory;

			var rv = new List<RunDeviceTask> ();

			foreach (var project in jenkins.Harness.TestProjects) {
				if (!project.IsExecutableProject)
					continue;

				bool ignored = project.Ignore ?? !jenkins.TestSelection.IsEnabled (PlatformLabel.Device);
				if (!jenkins.IsIncluded (project))
					ignored = true;

				IEnumerable<IHardwareDevice> candidates;
				switch (project.TestPlatform) {
				case TestPlatform.iOS:
					ignored |= !jenkins.TestSelection.IsEnabled (PlatformLabel.iOS);
					candidates = jenkins.Devices.Connected64BitIOS;
					break;
				case TestPlatform.tvOS:
					ignored |= !jenkins.TestSelection.IsEnabled (PlatformLabel.tvOS);
					candidates = jenkins.Devices.ConnectedTV;
					break;
				default:
					continue;
				}

				var buildTask = new MSBuildTask (jenkins: jenkins, testProject: project, processManager: processManager) {
					ProjectConfiguration = "Debug",
					ProjectPlatform = "iPhone",
					Platform = project.TestPlatform,
					TestName = project.Name,
				};
				buildTask.CloneTestProject (jenkins.MainLog, processManager, project, HarnessConfiguration.RootDirectory);
				var runTask = new RunDeviceTask (
					jenkins: jenkins,
					devices: jenkins.Devices,
					buildTask: buildTask,
					processManager: processManager,
					tunnelBore: jenkins.TunnelBore,
					errorKnowledgeBase: jenkins.ErrorKnowledgeBase,
					useTcpTunnel: jenkins.Harness.UseTcpTunnel,
					candidates: candidates)
					{
						Ignored = ignored,
					};

				rv.Add (runTask);
			}

			return Task.FromResult<IEnumerable<AppleTestTask>> (testVariationsFactory.CreateTestVariations (rv, (buildTask, test, candidates)
				=> new RunDeviceTask (
					jenkins: jenkins,
					devices: jenkins.Devices,
					buildTask: buildTask,
					processManager: processManager,
					tunnelBore: jenkins.TunnelBore,
					errorKnowledgeBase: jenkins.ErrorKnowledgeBase,
					useTcpTunnel: jenkins.Harness.UseTcpTunnel,
					candidates: candidates?.Cast<IHardwareDevice> () ?? test.Candidates)));
		}
	}
}
