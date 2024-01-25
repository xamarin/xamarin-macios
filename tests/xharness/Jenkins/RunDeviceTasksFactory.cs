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
				bool createTodayExtension;
				bool createtvOS;
				bool createwatchOS;

				if (project.GenerateVariations) {
					createiOS = !project.SkipiOSVariation;
					createTodayExtension = !project.SkipTodayExtensionVariation;
					createtvOS = !project.SkiptvOSVariation;
					createwatchOS = !project.SkipwatchOSVariation;
				} else {
					createiOS = project.TestPlatform == TestPlatform.iOS_Unified;
					createTodayExtension = project.TestPlatform == TestPlatform.iOS_TodayExtension64;
					createtvOS = project.TestPlatform == TestPlatform.tvOS;
					createwatchOS = project.TestPlatform == TestPlatform.watchOS;
				}

				if (createiOS) {
					var build64 = new MSBuildTask (jenkins: jenkins, testProject: project, processManager: processManager) {
						ProjectConfiguration = "Debug",
						ProjectPlatform = "iPhone",
						Platform = TestPlatform.iOS_Unified64,
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

					if (createTodayExtension) {
						var todayProject = project.GenerateVariations ? project.AsTodayExtensionProject () : project;
						var buildToday = new MSBuildTask (jenkins: jenkins, testProject: todayProject, processManager: processManager) {
							ProjectConfiguration = "Debug",
							ProjectPlatform = "iPhone",
							Platform = TestPlatform.iOS_TodayExtension64,
							TestName = project.Name,
						};
						buildToday.CloneTestProject (jenkins.MainLog, processManager, todayProject, HarnessConfiguration.RootDirectory);
						projectTasks.Add (new RunDeviceTask (
							jenkins: jenkins,
							devices: jenkins.Devices,
							buildTask: buildToday,
							processManager: processManager,
							tunnelBore: jenkins.TunnelBore,
							errorKnowledgeBase: jenkins.ErrorKnowledgeBase,
							useTcpTunnel: jenkins.Harness.UseTcpTunnel,
							candidates: jenkins.Devices.Connected64BitIOS.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !jenkins.TestSelection.IsEnabled (PlatformLabel.iOSExtension), BuildOnly = jenkins.ForceExtensionBuildOnly });
					}
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

				if (createwatchOS) {
					var watchOSProject = project.GenerateVariations ? project.AsWatchOSProject () : project;
					if (!project.SkipwatchOSARM64_32Variation) {
						var buildWatch64_32 = new MSBuildTask (jenkins: jenkins, testProject: watchOSProject, processManager: processManager) {
							ProjectConfiguration = "Release", // We don't support Debug for ARM64_32 yet.
							ProjectPlatform = "iPhone",
							Platform = TestPlatform.watchOS_64_32,
							TestName = project.Name,
						};
						buildWatch64_32.CloneTestProject (jenkins.MainLog, processManager, watchOSProject, HarnessConfiguration.RootDirectory);
						projectTasks.Add (new RunDeviceTask (
							jenkins: jenkins,
							devices: jenkins.Devices,
							buildTask: buildWatch64_32,
							processManager: processManager,
							tunnelBore: jenkins.TunnelBore,
							errorKnowledgeBase: jenkins.ErrorKnowledgeBase,
							useTcpTunnel: jenkins.Harness.UseTcpTunnel,
							candidates: jenkins.Devices.ConnectedWatch32_64.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !jenkins.TestSelection.IsEnabled (PlatformLabel.watchOS) });
					}
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
