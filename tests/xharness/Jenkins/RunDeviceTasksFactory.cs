using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Tasks;
using Xharness.Jenkins.TestTasks;

namespace Xharness.Jenkins {
	class RunDeviceTasksFactory {

		public Task<IEnumerable<ITestTask>> CreateAsync (Jenkins jenkins, IProcessManager processManager, TestVariationsFactory testVariationsFactory)
		{
			var rv = new List<RunDeviceTask> ();
			var projectTasks = new List<RunDeviceTask> ();

			foreach (var project in jenkins.Harness.IOSTestProjects) {
				if (!project.IsExecutableProject)
					continue;
				
				if (project.SkipDeviceVariations)
					continue;

				bool ignored = !jenkins.IncludeDevice;
				if (!jenkins.IsIncluded (project))
					ignored = true;

				projectTasks.Clear ();
				if (!project.SkipiOSVariation) {
					var build64 = new MSBuildTask (jenkins: jenkins, testProject: project, processManager: processManager) {
						ProjectConfiguration = "Debug64",
						ProjectPlatform = "iPhone",
						Platform = TestPlatform.iOS_Unified64,
						TestName = project.Name,
					};
					build64.CloneTestProject (jenkins.MainLog, processManager, project);
					projectTasks.Add (new RunDeviceTask (
						jenkins: jenkins,
						devices: jenkins.Devices,
						buildTask: build64,
						processManager: processManager,
						tunnelBore: jenkins.TunnelBore,
						errorKnowledgeBase: jenkins.ErrorKnowledgeBase,
						useTcpTunnel: jenkins.Harness.UseTcpTunnel,
						candidates: jenkins.Devices.Connected64BitIOS.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !jenkins.IncludeiOS64 });

					var build32 = new MSBuildTask (jenkins: jenkins, testProject: project, processManager: processManager) {
						ProjectConfiguration = project.Name != "dont link" ? "Debug32" : "Release32",
						ProjectPlatform = "iPhone",
						Platform = TestPlatform.iOS_Unified32,
						TestName = project.Name,
					};
					build32.CloneTestProject (jenkins.MainLog, processManager, project);
					projectTasks.Add (new RunDeviceTask (
						jenkins: jenkins,
						devices: jenkins.Devices,
						buildTask: build32,
						processManager: processManager,
						tunnelBore: jenkins.TunnelBore,
						errorKnowledgeBase: jenkins.ErrorKnowledgeBase,
						useTcpTunnel: jenkins.Harness.UseTcpTunnel,
						candidates: jenkins.Devices.Connected32BitIOS.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !jenkins.IncludeiOS32 });

					if (!project.SkipTodayExtensionVariation) {
						var todayProject = project.AsTodayExtensionProject ();
						var buildToday = new MSBuildTask (jenkins: jenkins, testProject: todayProject, processManager: processManager) {
							ProjectConfiguration = "Debug64",
							ProjectPlatform = "iPhone",
							Platform = TestPlatform.iOS_TodayExtension64,
							TestName = project.Name,
						};
						buildToday.CloneTestProject (jenkins.MainLog, processManager, todayProject);
						projectTasks.Add (new RunDeviceTask (
							jenkins: jenkins,
							devices: jenkins.Devices,
							buildTask: buildToday,
							processManager: processManager,
							tunnelBore: jenkins.TunnelBore,
							errorKnowledgeBase: jenkins.ErrorKnowledgeBase,
							useTcpTunnel: jenkins.Harness.UseTcpTunnel,
							candidates: jenkins.Devices.Connected64BitIOS.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !jenkins.IncludeiOSExtensions, BuildOnly = jenkins.ForceExtensionBuildOnly });
					}
				}

				if (!project.SkiptvOSVariation) {
					var tvOSProject = project.AsTvOSProject ();
					var buildTV = new MSBuildTask (jenkins: jenkins, testProject: tvOSProject, processManager: processManager) {
						ProjectConfiguration = "Debug",
						ProjectPlatform = "iPhone",
						Platform = TestPlatform.tvOS,
						TestName = project.Name,
					};
					buildTV.CloneTestProject (jenkins.MainLog, processManager, tvOSProject);
					projectTasks.Add (new RunDeviceTask (
						jenkins: jenkins,
						devices: jenkins.Devices,
						buildTask: buildTV,
						processManager: processManager,
						tunnelBore: jenkins.TunnelBore,
						errorKnowledgeBase: jenkins.ErrorKnowledgeBase,
						useTcpTunnel: jenkins.Harness.UseTcpTunnel,
						candidates: jenkins.Devices.ConnectedTV.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !jenkins.IncludetvOS });
				}

				if (!project.SkipwatchOSVariation) {
					var watchOSProject = project.AsWatchOSProject ();
					if (!project.SkipwatchOS32Variation) {
						var buildWatch32 = new MSBuildTask (jenkins: jenkins, testProject: watchOSProject, processManager: processManager) {
							ProjectConfiguration = "Debug32",
							ProjectPlatform = "iPhone",
							Platform = TestPlatform.watchOS_32,
							TestName = project.Name,
						};
						buildWatch32.CloneTestProject (jenkins.MainLog, processManager, watchOSProject);
						projectTasks.Add (new RunDeviceTask (
							jenkins: jenkins,
							devices: jenkins.Devices,
							buildTask: buildWatch32,
							processManager: processManager,
							tunnelBore: jenkins.TunnelBore,
							errorKnowledgeBase: jenkins.ErrorKnowledgeBase,
							useTcpTunnel: jenkins.Harness.UseTcpTunnel,
							candidates: jenkins.Devices.ConnectedWatch) { Ignored = !jenkins.IncludewatchOS });
					}

					if (!project.SkipwatchOSARM64_32Variation) {
						var buildWatch64_32 = new MSBuildTask (jenkins: jenkins, testProject: watchOSProject, processManager: processManager) {
							ProjectConfiguration = "Release64_32", // We don't support Debug for ARM64_32 yet.
							ProjectPlatform = "iPhone",
							Platform = TestPlatform.watchOS_64_32,
							TestName = project.Name,
						};
						buildWatch64_32.CloneTestProject (jenkins.MainLog, processManager, watchOSProject);
						projectTasks.Add (new RunDeviceTask (
							jenkins: jenkins,
							devices: jenkins.Devices,
							buildTask: buildWatch64_32,
							processManager: processManager,
							tunnelBore: jenkins.TunnelBore,
							errorKnowledgeBase: jenkins.ErrorKnowledgeBase,
							useTcpTunnel: jenkins.Harness.UseTcpTunnel,
							candidates: jenkins.Devices.ConnectedWatch32_64.Where (d => project.IsSupported (d.DevicePlatform, d.ProductVersion))) { Ignored = !jenkins.IncludewatchOS });
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
