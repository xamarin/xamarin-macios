// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Xharness.Jenkins.TestTasks;

namespace Xharness.Jenkins {
	class AppExtensionTestTasksFactory : TaskFactory {
		public AppExtensionTestTasksFactory (Jenkins jenkins, IMlaunchProcessManager processManager, TestVariationsFactory testVariationsFactory)
			: base (jenkins, processManager, testVariationsFactory)
		{
		}

		public override Task<IEnumerable<AppleTestTask>> CreateTasksAsync ()
		{
			var tasks = new List<AppleTestTask> ();
			var selected = Jenkins.TestSelection.IsEnabled (TestLabel.AppExtensions);

			if (Jenkins.Harness.INCLUDE_MAC && Jenkins.TestSelection.IsEnabled (PlatformLabel.Mac)) {
				tasks.Add (CreateTask (
					TestPlatform.Mac,
					"Debug (trimmable static registrar)",
					"trimmable-static-registrar",
					!selected));
			}

			if (Jenkins.Harness.INCLUDE_MACCATALYST && Jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst)) {
				tasks.Add (CreateTask (
					TestPlatform.MacCatalyst,
					"Debug (trimmable static registrar)",
					"trimmable-static-registrar",
					!selected));
			}

			if (Jenkins.Harness.INCLUDE_IOS && Jenkins.TestSelection.IsEnabled (PlatformLabel.iOS) && Jenkins.TestSelection.IsEnabled (PlatformLabel.iOSSimulator)) {
				tasks.Add (CreateTask (
					TestPlatform.iOS,
					"Debug (trimmable static registrar)",
					"trimmable-static-registrar",
					!selected));
			}

			if (Jenkins.Harness.INCLUDE_TVOS && Jenkins.TestSelection.IsEnabled (PlatformLabel.tvOS) && Jenkins.TestSelection.IsEnabled (PlatformLabel.iOSSimulator)) {
				tasks.Add (CreateTask (
					TestPlatform.tvOS,
					"Debug (trimmable static registrar)",
					"trimmable-static-registrar",
					!selected));
			}

			return Task.FromResult<IEnumerable<AppleTestTask>> (tasks);
		}

		AppExtensionTestTask CreateTask (TestPlatform platform, string variation, string testVariation, bool ignored)
		{
			IEnumerable<ISimulatorDevice>? candidates = null;
			var targets = platform.GetTestTargetsForSimulator ();
			if (targets.Length > 0)
				candidates = Jenkins.Simulators.SelectDevices (targets [0].GetTargetOs (false), Jenkins.SimulatorLoadLog, false);
			var extensionDirectory = platform == TestPlatform.tvOS ? "tv-top-shelf" : "audio-unit";

			return new AppExtensionTestTask (Jenkins, ProcessManager, platform, extensionDirectory, testVariation, candidates) {
				TestName = "monotouch-test app extensions",
				Mode = platform.ToPlatformName (),
				Variation = variation,
				Ignored = ignored,
			};
		}
	}
}
