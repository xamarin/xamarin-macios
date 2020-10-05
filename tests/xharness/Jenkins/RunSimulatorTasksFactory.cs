using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Tasks;
using Xharness.Jenkins.TestTasks;

namespace Xharness.Jenkins {
	// lets try and keep this class stateless, will make our lifes better
	class RunSimulatorTasksFactory {

		public async Task<IEnumerable<ITestTask>> CreateAsync (Jenkins jenkins, IProcessManager processManager, TestVariationsFactory testVariationsFactory)
		{
			var runSimulatorTasks = new List<RunSimulatorTask> ();

			foreach (var project in jenkins.Harness.IOSTestProjects) {
				if (!project.IsExecutableProject)
					continue;

				bool ignored = project.Ignore ?? !jenkins.IncludeSimulator;
				if (!jenkins.IsIncluded (project))
					ignored = true;

				var ps = new List<Tuple<TestProject, TestPlatform, bool>> ();
				if (!project.GenerateVariations) {
					ps.Add (new Tuple<TestProject, TestPlatform, bool> (project, project.TestPlatform, ignored));
				} else {
					if (!project.SkipiOSVariation)
						ps.Add (new Tuple<TestProject, TestPlatform, bool> (project, TestPlatform.iOS_Unified, ignored));
					if (project.MonoNativeInfo != null)
						ps.Add (new Tuple<TestProject, TestPlatform, bool> (project, TestPlatform.iOS_TodayExtension64, ignored));
					if (!project.SkiptvOSVariation)
						ps.Add (new Tuple<TestProject, TestPlatform, bool> (project.AsTvOSProject (), TestPlatform.tvOS, ignored));
					if (!project.SkipwatchOSVariation)
						ps.Add (new Tuple<TestProject, TestPlatform, bool> (project.AsWatchOSProject (), TestPlatform.watchOS, ignored));
				}

				var configurations = project.Configurations;
				if (configurations == null)
					configurations = new string [] { "Debug" };
				foreach (var config in configurations) {
					foreach (var pair in ps) {
						var configIgnored = pair.Item3;
						var testPlatform = pair.Item2;
						switch (testPlatform) {
						case TestPlatform.iOS_Unified:
						case TestPlatform.iOS_TodayExtension64:
							configIgnored |= !jenkins.IncludeiOS64;
							break;
						case TestPlatform.tvOS:
							configIgnored |= !jenkins.IncludetvOS;
							break;
						case TestPlatform.watchOS:
							configIgnored |= !jenkins.IncludewatchOS;
							break;
						default:
							Console.WriteLine ("Unknown test platform for ignore check: {0}", testPlatform);
							break;
						}

						MSBuildTask derived;
						if (project.IsDotNetProject) {
							derived = new DotNetBuildTask (jenkins: jenkins, testProject: project, processManager: processManager);
							configIgnored |= !jenkins.IncludeDotNet;
						} else {
							derived = new MSBuildTask (jenkins: jenkins, testProject: project, processManager: processManager);
						}
						derived.ProjectConfiguration = config;
						derived.ProjectPlatform = "iPhoneSimulator";
						derived.Platform = testPlatform;
						derived.Ignored = configIgnored;
						derived.TestName = project.Name;
						derived.Dependency = project.Dependency;
						derived.CloneTestProject (jenkins.MainLog, processManager, pair.Item1, HarnessConfiguration.RootDirectory);
						var simTasks = CreateAsync (jenkins, processManager, derived);
						runSimulatorTasks.AddRange (simTasks);
						foreach (var task in simTasks) {
							if (configurations.Length > 1)
								task.Variation = config;
							task.TimeoutMultiplier = project.TimeoutMultiplier;
						}
					}
				}
			}

			var testVariations = testVariationsFactory.CreateTestVariations (runSimulatorTasks, (buildTask, test, candidates) =>
				new RunSimulatorTask (
					jenkins: jenkins, 
					simulators: jenkins.Simulators,
					buildTask: buildTask,
					processManager: processManager,
					tunnelBore: jenkins.TunnelBore,
					candidates: candidates?.Cast<SimulatorDevice> () ?? test.Candidates)).ToList ();

			if (jenkins.IsServerMode)
				return testVariations;

			foreach (var tv in testVariations) {
				if (!tv.Ignored)
					await tv.FindSimulatorAsync ();
			}

			var rv = new List<AggregatedRunSimulatorTask> ();
			foreach (var taskGroup in testVariations.GroupBy ((RunSimulatorTask task) => task.Device?.UDID ?? task.Candidates.ToString ())) {
				rv.Add (new AggregatedRunSimulatorTask (jenkins: jenkins, tasks: taskGroup) {
					TestName = $"Tests for {taskGroup.Key}",
				});
			}
			return rv;
		}

		IEnumerable<RunSimulatorTask> CreateAsync (Jenkins jenkins, IProcessManager processManager, MSBuildTask buildTask)
		{
			var runtasks = new List<RunSimulatorTask> ();

			TestTarget [] targets = buildTask.Platform.GetAppRunnerTargets ();
			TestPlatform [] platforms;
			bool [] ignored;

			switch (buildTask.Platform) {
			case TestPlatform.tvOS:
				platforms = new TestPlatform [] { TestPlatform.tvOS };
				ignored = new [] { false };
				break;
			case TestPlatform.watchOS:
				platforms = new TestPlatform [] { TestPlatform.watchOS_32 };
				ignored = new [] { false };
				break;
			case TestPlatform.iOS_Unified:
				var iOSProject = (iOSTestProject) buildTask.TestProject;
				if (iOSProject.SkipiOS32Variation && iOSProject.SkipiOS64Variation) {
					return runtasks;
				} else if (iOSProject.SkipiOS32Variation) {
					targets = new TestTarget [] { TestTarget.Simulator_iOS64 };
					platforms = new TestPlatform [] { TestPlatform.iOS_Unified64 };
					ignored = new [] { false };
				} else if (iOSProject.SkipiOS64Variation) {
					targets = new TestTarget [] { TestTarget.Simulator_iOS32 };
					platforms = new TestPlatform [] { TestPlatform.iOS_Unified32 };
					ignored = new [] { !jenkins.IncludeiOS32 };
				} else {
					platforms = new TestPlatform [] { TestPlatform.iOS_Unified32, TestPlatform.iOS_Unified64 };
					ignored = new [] { !jenkins.IncludeiOS32, false };
				}
				break;
			case TestPlatform.iOS_TodayExtension64:
				targets = new TestTarget [] { TestTarget.Simulator_iOS64 };
				platforms = new TestPlatform [] { TestPlatform.iOS_TodayExtension64 };
				ignored = new [] { false };
				break;
			default:
				throw new NotImplementedException ();
			}

			for (int i = 0; i < targets.Length; i++) {
				var sims = jenkins.Simulators.SelectDevices (targets [i], jenkins.SimulatorLoadLog, false);
				runtasks.Add (new RunSimulatorTask (
					jenkins: jenkins,
					simulators: jenkins.Simulators,
					buildTask: buildTask,
					processManager: processManager,
					tunnelBore: jenkins.TunnelBore,
					candidates: sims) {
					Platform = platforms [i],
					Ignored = ignored [i] || buildTask.Ignored
				});
			}

			return runtasks;
		}
	}
}
