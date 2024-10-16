using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Xharness.Jenkins.TestTasks;

namespace Xharness.Jenkins {
	class RunDeviceTasksFactory {

		public Task<IEnumerable<ITestTask>> CreateAsync (Jenkins jenkins, IMlaunchProcessManager processManager, TestVariationsFactory testVariationsFactory)
		{
			var rv = new List<RunDeviceTask> ();
			var projectTasks = new List<RunDeviceTask> ();

			foreach (var project in jenkins.Harness.IOSTestProjects) {
				if (!project.IsExecutableProject)
					continue;

				if (project.SkipDeviceVariations)
					continue;

				bool ignored = project.Ignore ?? !jenkins.TestSelection.IsEnabled (PlatformLabel.Device);
				if (!jenkins.IsIncluded (project))
					ignored = true;
				if (project.IsDotNetProject)
					ignored = true;

				projectTasks.Clear ();

				bool createiOS;
				bool createtvOS;

				if (project.GenerateVariations) {
					createiOS = !project.SkipiOSVariation;
					createtvOS = !project.SkiptvOSVariation;
				} else {
					createiOS = project.TestPlatform == TestPlatform.iOS;
					createtvOS = project.TestPlatform == TestPlatform.tvOS;
				}

				if (createiOS) {
					var build64 = new MSBuildTask (jenkins: jenkins, testProject: project, processManager: processManager) {
						ProjectConfiguration = "Debug",
						ProjectPlatform = "iPhone",
						Platform = TestPlatform.iOS,
						TestName = project.Name,
					};
					build64.CloneTestProject (jenkins.MainLog, processManager, project, HarnessConfiguration.RootDirectory);
					projectTasks.Add (new RunDeviceTask (
						jenkins: jenkins,
						devices: jenkins.Devices,
						buildTask: build64,
						processManager: processManager,
						tunnelBore: jenkins.TunnelBore,
						errorKnowledgeBase: jenkins.ErrorKnowledgeBase,
						useTcpTunnel: jenkins.Harness.UseTcpTunnel,
						candidates: jenkins.Devices.Connected64BitIOS.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !jenkins.TestSelection.IsEnabled (PlatformLabel.iOS) });
				}

				if (createtvOS) {
					var tvOSProject = project.GenerateVariations ? project.AsTvOSProject () : project;
					var buildTV = new MSBuildTask (jenkins: jenkins, testProject: tvOSProject, processManager: processManager) {
						ProjectConfiguration = "Debug",
						ProjectPlatform = "iPhone",
						Platform = TestPlatform.tvOS,
						TestName = project.Name,
					};
					buildTV.CloneTestProject (jenkins.MainLog, processManager, tvOSProject, HarnessConfiguration.RootDirectory);
					projectTasks.Add (new RunDeviceTask (
						jenkins: jenkins,
						devices: jenkins.Devices,
						buildTask: buildTV,
						processManager: processManager,
						tunnelBore: jenkins.TunnelBore,
						errorKnowledgeBase: jenkins.ErrorKnowledgeBase,
						useTcpTunnel: jenkins.Harness.UseTcpTunnel,
						candidates: jenkins.Devices.ConnectedTV.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !jenkins.TestSelection.IsEnabled (PlatformLabel.tvOS) });
				}

				foreach (var task in projectTasks) {
					task.TimeoutMultiplier = project.TimeoutMultiplier;
					task.BuildOnly |= project.BuildOnly;
					task.Ignored |= ignored;
				}
				rv.AddRange (projectTasks);
			}

			return Task.FromResult<IEnumerable<ITestTask>> (testVariationsFactory.CreateTestVariations (rv, (buildTask, test, candidates)
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
