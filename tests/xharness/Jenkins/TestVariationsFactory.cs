using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Xharness.Jenkins.TestTasks;

namespace Xharness.Jenkins {

	interface ITestVariationsFactory {
		IEnumerable<T> CreateTestVariations<T> (IEnumerable<T> tests, Func<MSBuildTask, T, IEnumerable<IDevice>, T> creator) where T : RunTestTask;
	}

	class TestVariationsFactory : ITestVariationsFactory {

		readonly Jenkins jenkins;
		readonly IProcessManager processManager;

		public TestVariationsFactory (Jenkins jenkins, IProcessManager processManager)
		{
			this.jenkins = jenkins ?? throw new ArgumentNullException (nameof (jenkins));
			this.processManager = processManager ?? throw new ArgumentNullException (nameof (processManager));
		}

		IEnumerable<TestData> GetTestData (RunTestTask test)
		{
			// This function returns additional test configurations (in addition to the default one) for the specific test

			var supports_interpreter = test.Platform != TestPlatform.Mac;
			var ignore = test.TestProject.Ignore;
			var mac_supports_arm64 = Harness.CanRunArm64;
			var arm64_runtime_identifier = string.Empty;
			var x64_runtime_identifier = string.Empty;
			var arm64_sim_runtime_identifier = string.Empty;
			var x64_sim_runtime_identifier = string.Empty;

			switch (test.Platform) {
			case TestPlatform.Mac:
				arm64_runtime_identifier = "osx-arm64";
				x64_runtime_identifier = "osx-x64";
				break;
			case TestPlatform.MacCatalyst:
				arm64_runtime_identifier = "maccatalyst-arm64";
				x64_runtime_identifier = "maccatalyst-x64";
				break;
			case TestPlatform.iOS:
				arm64_sim_runtime_identifier = "iossimulator-arm64";
				x64_sim_runtime_identifier = "iossimulator-x64";
				break;
			case TestPlatform.tvOS:
				arm64_sim_runtime_identifier = "tvossimulator-arm64";
				x64_sim_runtime_identifier = "tvossimulator-x64";
				break;
			}

			switch (test.TestName) {
			case "link all":
				if (test.ProjectConfiguration == "Debug") {
					// in .NET 9 we add a variation to test original resource bundling (which is opt in in .NET 9),
					// and in .NET 10 we change the variation to test *not* bundling original resources (because it becomes opt out in .NET 10,
					// so we don't test it by default).
					if (jenkins.Harness.DotNetVersion.Major <= 9) {
						yield return new TestData { Variation = "Debug (bundle original resources)", TestVariation = "bundle-original-resources", Debug = true };
					} else {
						yield return new TestData { Variation = "Debug (don't bundle original resources)", TestVariation = "do-not-bundle-original-resources", Debug = true };
					}
				}
				break;
			}

			switch (test.ProjectPlatform) {
			case "iPhone":
				if (test.ProjectConfiguration.Contains ("Debug"))
					yield return new TestData { Variation = "Release", Debug = false, Profiling = false };

				switch (test.TestName) {
				case "monotouch-test":
					ignore = true;
					yield return new TestData { Variation = "Debug (dynamic registrar)", Registrar = "dynamic", Debug = true, Profiling = false, Ignored = ignore };
					yield return new TestData { Variation = "Release (all optimizations)", AppBundleExtraOptions = "--optimize:all", Registrar = "static", Debug = false, Profiling = false, Defines = "OPTIMIZEALL", Ignored = ignore };
					yield return new TestData { Variation = "Debug (all optimizations)", AppBundleExtraOptions = "--optimize:all", Registrar = "static", Debug = true, Profiling = false, Defines = "OPTIMIZEALL", Ignored = ignore };
					yield return new TestData { Variation = "Debug: SGenConc", AppBundleExtraOptions = "", Debug = true, Profiling = false, EnableSGenConc = true, Ignored = ignore };
					if (supports_interpreter) {
						yield return new TestData { Variation = "Debug (interpreter)", TestVariation = "interpreter", Debug = true, Profiling = false, Ignored = ignore, Defines = "" };
					}
					yield return new TestData { Variation = "Release (LLVM)", Debug = false, UseLlvm = true, Ignored = ignore };
					yield return new TestData { Variation = "Debug (managed static registrar)", Registrar = "managed-static", Debug = true, Profiling = false, Ignored = ignore };
					yield return new TestData { Variation = "Release (managed static registrar, all optimizations)", AppBundleExtraOptions = "--optimize:all", Registrar = "managed-static", Debug = false, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = ignore };
					yield return new TestData { Variation = "Release (NativeAOT)", Debug = false, PublishAot = true, Ignored = ignore, LinkMode = "Full" };
					break;
				}
				break;
			case "iPhoneSimulator":
				switch (test.TestName) {
				case "monotouch-test":
					// The default is to run monotouch-test with the dynamic registrar (in the simulator), so that's already covered
					yield return new TestData { Variation = "Debug (LinkSdk)", Debug = true, Profiling = false, LinkMode = "SdkOnly", Ignored = ignore };
					yield return new TestData { Variation = "Debug (static registrar)", Registrar = "static", Debug = true, Profiling = false, Ignored = ignore };
					yield return new TestData { Variation = "Release (all optimizations)", AppBundleExtraOptions = "--optimize:all", Registrar = "static", Debug = false, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = ignore };
					yield return new TestData { Variation = "Debug (all optimizations)", AppBundleExtraOptions = "--optimize:all,-remove-uithread-checks", Registrar = "static", Debug = true, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = ignore ?? !jenkins.TestSelection.IsEnabled (TestLabel.All) };

					if (mac_supports_arm64) {
						yield return new TestData { Variation = "Debug (ARM64)", Debug = true, Profiling = false, Ignored = !mac_supports_arm64 ? true : ignore, RuntimeIdentifier = arm64_sim_runtime_identifier, };
						yield return new TestData { Variation = "Release (NativeAOT, ARM64)", Debug = false, PublishAot = true, Ignored = ignore, RuntimeIdentifier = arm64_sim_runtime_identifier, LinkMode = "Full" };
					}
					yield return new TestData { Variation = "Debug (managed static registrar)", Registrar = "managed-static", Debug = true, Profiling = false, Ignored = ignore };
					yield return new TestData { Variation = "Release (managed static registrar, all optimizations)", AppBundleExtraOptions = "--optimize:all", Registrar = "managed-static", Debug = false, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = ignore };
					yield return new TestData { Variation = "Release (NativeAOT, x64)", Debug = false, PublishAot = true, Ignored = ignore, RuntimeIdentifier = x64_sim_runtime_identifier, LinkMode = "Full" };
					if (supports_interpreter) {
						yield return new TestData { Variation = "Debug (interpreter)", TestVariation = "interpreter", Debug = true, Profiling = false, Ignored = ignore };
						// interpreter+release fails due to https://github.com/dotnet/runtime/issues/110649.
						// yield return new TestData { Variation = "Release (interpreter)", TestVariation = "interpreter", Debug = false, Profiling = false, Ignored = ignore, UseLlvm = false };
					}
					break;
				case "introspection":
					if (mac_supports_arm64)
						yield return new TestData { Variation = "Debug (ARM64)", Debug = true, Profiling = false, Ignored = !mac_supports_arm64 ? true : ignore, RuntimeIdentifier = arm64_sim_runtime_identifier, };

					foreach (var target in test.Platform.GetTestTargetsForSimulator ())
						yield return new TestData {
							Variation = $"Debug ({test.Platform.GetSimulatorMinVersion ()})",
							Debug = true,
							Candidates = jenkins.Simulators.SelectDevices (target.GetTargetOs (true), jenkins.SimulatorLoadLog, true),
							Ignored = ignore ?? !jenkins.TestSelection.IsEnabled (PlatformLabel.OldiOSSimulator) || !jenkins.TestSelection.IsEnabled (TestLabel.Introspection),
						};
					break;
				}
				break;
			case "AnyCPU":
			case "x86":
			case "":
			case null:
				switch (test.TestName) {
				case "monotouch-test":
					yield return new TestData { Variation = "Debug (ARM64)", Debug = true, Profiling = false, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac) || !mac_supports_arm64, RuntimeIdentifier = arm64_runtime_identifier, };
					if (test.Platform != TestPlatform.MacCatalyst) {
						yield return new TestData { Variation = "Debug (static registrar)", Registrar = "static", Debug = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac), };
						yield return new TestData { Variation = "Debug (static registrar, ARM64)", Registrar = "static", Debug = true, Profiling = false, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac) || !mac_supports_arm64, RuntimeIdentifier = arm64_runtime_identifier, };
						yield return new TestData { Variation = "Release (NativeAOT, ARM64)", Debug = false, PublishAot = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac) || !mac_supports_arm64, RuntimeIdentifier = arm64_runtime_identifier, LinkMode = "Full" };
						yield return new TestData { Variation = "Release (NativeAOT, x64)", Debug = false, PublishAot = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac), RuntimeIdentifier = "osx-x64", LinkMode = "Full" };
					}
					if (test.Platform == TestPlatform.MacCatalyst) {
						yield return new TestData { Variation = "Release (ARM64, LLVM)", Debug = false, UseLlvm = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst) || !mac_supports_arm64, RuntimeIdentifier = arm64_runtime_identifier };
						yield return new TestData { Variation = "Release (static registrar)", Registrar = "static", Debug = false, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst) };
						yield return new TestData { Variation = "Release (managed static registrar)", Registrar = "managed-static", Debug = false, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst) };
						yield return new TestData { Variation = "Release (NativeAOT)", Debug = false, PublishAot = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst), LinkMode = "Full" };
						yield return new TestData { Variation = "Release (NativeAOT, ARM64)", Debug = false, PublishAot = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst) || !mac_supports_arm64, RuntimeIdentifier = arm64_runtime_identifier, LinkMode = "Full" };
						yield return new TestData { Variation = "Release (NativeAOT, x64)", Debug = false, PublishAot = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst), LinkMode = "Full", RuntimeIdentifier = x64_runtime_identifier };
					}
					if (test.Platform == TestPlatform.Mac) {
						yield return new TestData { Variation = "Release (static registrar)", Registrar = "static", Debug = false, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac) };
						yield return new TestData { Variation = "Release (managed static registrar)", Registrar = "managed-static", Debug = false, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac) };
						yield return new TestData { Variation = "Release (NativeAOT)", Debug = false, PublishAot = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac), LinkMode = "Full" };
					}
					yield return new TestData { Variation = "Release (static registrar, all optimizations)", AppBundleExtraOptions = "--optimize:all", Registrar = "static", Debug = false, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = ignore };
					yield return new TestData { Variation = "Debug (managed static registrar)", Registrar = "managed-static", Debug = true, Profiling = false, Ignored = ignore };
					yield return new TestData { Variation = "Release (managed static registrar, all optimizations)", AppBundleExtraOptions = "--optimize:all", Registrar = "managed-static", Debug = false, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = ignore };
					if (supports_interpreter) {
						yield return new TestData { Variation = "Debug (interpreter)", TestVariation = "interpreter", Debug = true, Profiling = false, Ignored = ignore };
						yield return new TestData { Variation = "Release (interpreter)", TestVariation = "interpreter", Debug = false, Profiling = false, Ignored = ignore, UseLlvm = false };
					}
					break;
				}
				break;
			default:
				throw new NotImplementedException (test.ProjectPlatform);
			}
		}

		public IEnumerable<T> CreateTestVariations<T> (IEnumerable<T> tests, Func<MSBuildTask, T, IEnumerable<IDevice>, T> creator) where T : RunTestTask
		{
			foreach (var task in tests) {
				if (string.IsNullOrEmpty (task.Variation))
					task.Variation = task.ProjectConfiguration.Contains ("Debug") ? "Debug" : "Release";
			}

			var rv = new List<T> (tests);
			foreach (var task in tests.ToArray ()) {
				foreach (var test_data in GetTestData (task)) {
					var variation = test_data.Variation;
					var bundler_arguments = test_data.AppBundleExtraOptions;
					var configuration = test_data.Debug ? task.ProjectConfiguration : task.ProjectConfiguration.Replace ("Debug", "Release");
					var debug = test_data.Debug;
					var profiling = test_data.Profiling;
					var link_mode = test_data.LinkMode;
					var defines = test_data.Defines;
					var undefines = test_data.Undefines;
					var ignored = test_data.Ignored;
					var known_failure = test_data.KnownFailure;
					var candidates = test_data.Candidates;
					var use_mono_runtime = test_data.UseMonoRuntime;
					var runtime_identifer = test_data.RuntimeIdentifier;
					var use_llvm = test_data.UseLlvm;
					var registrar = test_data.Registrar;
					var publishaot = test_data.PublishAot;
					var test_variation = test_data.TestVariation;

					if (known_failure is not null)
						ignored = true;

					var clone = task.TestProject.Clone ();
					var clone_task = Task.Run (async () => {
						await task.BuildTask.InitialTask; // this is the project cloning above
						await clone.CreateCopyAsync (jenkins.MainLog, processManager, task, HarnessConfiguration.RootDirectory);

						var isMac = task.Platform.IsMac ();
						var canSymlink = task.Platform.CanSymlink ();

						if (!string.IsNullOrEmpty (bundler_arguments)) {
							clone.Xml.AppendAppBundleExtraOptions (bundler_arguments);
						}
						if (!string.IsNullOrEmpty (link_mode)) {
							clone.Xml.SetProperty ("LinkMode", link_mode);
							clone.Xml.SetProperty ("MtouchLink", link_mode);
						}
						if (!string.IsNullOrEmpty (defines)) {
							clone.Xml.AddAdditionalDefines (defines, task.ProjectPlatform, configuration);
							if (clone.ProjectReferences is not null) {
								foreach (var pr in clone.ProjectReferences) {
									pr.Xml.AddAdditionalDefines (defines, task.ProjectPlatform, configuration);
									pr.Xml.Save (pr.Path);
								}
							}
						}
						if (!string.IsNullOrEmpty (undefines)) {
							clone.Xml.RemoveDefines (undefines, task.ProjectPlatform, configuration);
							if (clone.ProjectReferences is not null) {
								foreach (var pr in clone.ProjectReferences) {
									pr.Xml.RemoveDefines (undefines, task.ProjectPlatform, configuration);
									pr.Xml.Save (pr.Path);
								}
							}
						}
						clone.Xml.SetNode (isMac ? "Profiling" : "MTouchProfiling", profiling ? "True" : "False", task.ProjectPlatform, configuration);
						if (test_data.EnableSGenConc)
							clone.Xml.SetProperty ("EnableSGenConc", "true");
						if (use_llvm)
							clone.Xml.SetProperty ("MtouchUseLlvm", "true");

						if (!debug && !isMac)
							clone.Xml.SetMtouchUseLlvm (true, task.ProjectPlatform, configuration);
						if (use_mono_runtime.HasValue)
							clone.Xml.SetProperty ("UseMonoRuntime", use_mono_runtime.Value ? "true" : "false");
						if (!string.IsNullOrEmpty (runtime_identifer))
							clone.Xml.SetProperty ("RuntimeIdentifier", runtime_identifer);
						if (!string.IsNullOrEmpty (registrar))
							clone.Xml.SetProperty ("Registrar", registrar);
						if (publishaot) {
							clone.Xml.SetProperty ("PublishAot", "true", last: false);
							clone.Xml.SetProperty ("_IsPublishing", "true", last: false); // quack like "dotnet publish", otherwise PublishAot=true has no effect.
							clone.Xml.SetProperty ("IlcTreatWarningsAsErrors", "false", last: false); // We're enabling warnaserror by default, but we're not warning-free for ILC (especially for NUnit), so disable warnaserror for ILC - https://github.com/xamarin/xamarin-macios/issues/19911
						}
						if (!string.IsNullOrEmpty (test_variation)) {
							clone.Xml.SetProperty ("TestVariation", test_variation);
							foreach (var pr in clone.ProjectReferences) {
								pr.Xml.SetProperty ("TestVariation", test_variation);
								pr.Xml.Save (pr.Path);
							}
						}
						clone.Xml.Save (clone.Path);
					});

					var build = new MSBuildTask (jenkins: jenkins, testProject: clone, processManager: processManager);
					build.ProjectConfiguration = configuration;
					build.ProjectPlatform = task.ProjectPlatform;
					build.Platform = task.Platform;
					build.InitialTask = clone_task;
					build.TestName = clone.Name;

					T newVariation = creator (build, task, candidates);
					newVariation.Variation = variation;
					newVariation.Ignored = ignored ?? task.Ignored;
					newVariation.BuildOnly = task.BuildOnly;
					newVariation.TimeoutMultiplier = task.TimeoutMultiplier;
					newVariation.KnownFailure = known_failure;
					rv.Add (newVariation);
				}
			}

			return rv;
		}

	}
}
