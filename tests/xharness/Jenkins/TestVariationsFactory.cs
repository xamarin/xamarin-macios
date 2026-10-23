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

	class TestVariationsFactory {

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

			var ignore = test.TestProject!.Ignore;
			var arm64_runtime_identifier = string.Empty;
			var x64_runtime_identifier = string.Empty;
			var arm64_sim_runtime_identifier = string.Empty;
			var supports_coreclr = true;
			var supports_x64 = string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("ACES")); // x64 is not supported on ACES machines

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
				break;
			case TestPlatform.tvOS:
				arm64_sim_runtime_identifier = "tvossimulator-arm64";
				break;
			}

			switch (test.TestName) {
			case "dont link":
				if (supports_coreclr) {
					yield return new TestData { Variation = $"{test.ProjectConfiguration} (PrepareAssemblies, CoreCLR, Dynamic Registrar)", TestVariation = "coreclr|prepare-assemblies|dynamic-registrar", Ignored = ignore };
					yield return new TestData { Variation = $"{test.ProjectConfiguration} (PrepareAssemblies, CoreCLR, Managed Static Registrar)", TestVariation = "coreclr|prepare-assemblies|managed-static-registrar", Ignored = ignore };
					yield return new TestData { Variation = $"{test.ProjectConfiguration} (PrepareAssemblies, CoreCLR, Trimmable Static Registrar)", TestVariation = "coreclr|prepare-assemblies|trimmable-static-registrar", Ignored = ignore };
				}
				break;
			case "link sdk":
				if (supports_coreclr) {
					// if prepare-assemblies is enabled, then linking only works in any meaningful way when using the trimmable static registrar, which only works on CoreCLR in .NET 10
					yield return new TestData { Variation = $"{test.ProjectConfiguration} (PrepareAssemblies, CoreCLR, Trimmable Static Registrar)", TestVariation = "coreclr|prepare-assemblies|trimmable-static-registrar", Ignored = ignore };
				}
				break;
			case "link all":
				if (supports_coreclr) {
					// if prepare-assemblies is enabled, then linking only works in any meaningful way when using the trimmable static registrar, which only works on CoreCLR in .NET 10
					yield return new TestData { Variation = $"{test.ProjectConfiguration} (PrepareAssemblies, CoreCLR, Trimmable Static Registrar)", TestVariation = "coreclr|prepare-assemblies|trimmable-static-registrar", Ignored = ignore };
				}
				if (test.ProjectConfiguration == "Debug") {
					yield return new TestData { Variation = "Debug (don't bundle original resources)", TestVariation = "do-not-bundle-original-resources" };
				}
				break;
			case "monotouch-test":
				yield return new TestData { Variation = "Release (link sdk)", TestVariation = "release|linksdk", Ignored = ignore };
				yield return new TestData { Variation = "Release (link all)", TestVariation = "release|linkall", Ignored = ignore };
				yield return new TestData { Variation = $"{test.ProjectConfiguration} (PrepareAssemblies)", TestVariation = "prepare-assemblies", Ignored = ignore };
				// With PrepareAssemblies the registrar runs in a separate process after the trimmer, so it
				// can't get any information from the trimmer directly, and has to look up trimmed-away
				// metadata in the pre-trim assemblies instead. Enable the trimmer (linksdk), because
				// otherwise nothing is trimmed away and none of this is exercised.
				if (supports_coreclr)
					yield return new TestData { Variation = $"{test.ProjectConfiguration} (PrepareAssemblies, Trimmable Static Registrar, link sdk)", TestVariation = "linksdk|prepare-assemblies|trimmable-static-registrar", Ignored = ignore };
				// Explicitly disable the trimmer (dontlink) and enable InlineDlfcnMethods together with
				// PrepareAssemblies to exercise the inlined-dlfcn native symbol generation when the trimmer
				// is skipped. On .NET 11+ this is already covered by the plain 'prepare-assemblies' variation
				// above (iOS defaults to CoreCLR, which doesn't link, and InlineDlfcnMethods is enabled by
				// default there), so only run this explicit combination on .NET 10.
				yield return new TestData { Variation = $"{test.ProjectConfiguration} (PrepareAssemblies, inline dlfcn, dont link)", TestVariation = "dontlink|prepare-assemblies|inline-dlfcn-methods-compat", Ignored = jenkins.Harness.DotNetVersion.Major >= 11 ? true : ignore };
				break;
			}

			switch (test.ProjectPlatform) {
			case "iPhone":
				if (test.ProjectConfiguration?.Contains ("Debug") == true)
					yield return new TestData { Variation = "Release", TestVariation = "release" };

				switch (test.TestName) {
				case "monotouch-test":
					ignore = true;
					yield return new TestData { Variation = "Debug (dynamic registrar)", TestVariation = "dynamic-registrar", Ignored = ignore };
					yield return new TestData { Variation = "Release (all optimizations)", TestVariation = "release|static-registrar-all-optimizations", Ignored = ignore };
					yield return new TestData { Variation = "Debug (all optimizations)", TestVariation = "static-registrar-all-optimizations", Ignored = ignore };
					yield return new TestData { Variation = "Debug (managed static registrar)", TestVariation = "managed-static-registrar", Ignored = ignore };
					if (supports_coreclr)
						yield return new TestData { Variation = "Debug (trimmable static registrar)", TestVariation = "trimmable-static-registrar", Ignored = ignore };
					yield return new TestData { Variation = "Release (managed static registrar, all optimizations)", TestVariation = "release|managed-static-registrar-all-optimizations-linkall", Ignored = ignore };
					if (supports_coreclr) {
						yield return new TestData { Variation = "Debug (trimmable static registrar, all optimizations)", TestVariation = "trimmable-static-registrar-all-optimizations-linkall", Ignored = ignore };
						yield return new TestData { Variation = "Release (trimmable static registrar, all optimizations)", TestVariation = "release|trimmable-static-registrar-all-optimizations-linkall", Ignored = ignore };
					}
					yield return new TestData { Variation = "Release (NativeAOT)", TestVariation = "release|nativeaot", Ignored = ignore };
					yield return new TestData { Variation = "Release (trimmable static registrar, NativeAOT)", TestVariation = "trimmable-static-registrar|release|nativeaot", Ignored = ignore };
					break;
				}
				break;
			case "iPhoneSimulator":
				switch (test.TestName) {
				case "monotouch-test":
					// The default is to run monotouch-test with the dynamic registrar (in the simulator), so that's already covered
					yield return new TestData { Variation = "Debug (LinkSdk)", TestVariation = "linksdk", Ignored = ignore };
					yield return new TestData { Variation = "Debug (static registrar)", TestVariation = "static-registrar", Ignored = ignore };
					yield return new TestData { Variation = "Release (all optimizations)", TestVariation = "release|static-registrar-all-optimizations-linkall", Ignored = ignore };
					yield return new TestData { Variation = "Debug (all optimizations)", TestVariation = "static-registrar-optimizations-except-uithread-checks-linkall", Ignored = ignore ?? !jenkins.TestSelection.IsEnabled (TestLabel.All) };
					yield return new TestData { Variation = "Release (NativeAOT)", TestVariation = "release|nativeaot", Ignored = ignore, RuntimeIdentifier = arm64_sim_runtime_identifier }; // it's necessary to specify RID, because NativeAOT defaults to building for device
					yield return new TestData { Variation = "Release (trimmable static registrar, NativeAOT)", TestVariation = "trimmable-static-registrar|release|nativeaot", Ignored = ignore, RuntimeIdentifier = arm64_sim_runtime_identifier }; // it's necessary to specify RID, because NativeAOT defaults to building for device
					yield return new TestData { Variation = "Debug (managed static registrar)", TestVariation = "managed-static-registrar", Ignored = ignore };
					if (supports_coreclr)
						yield return new TestData { Variation = "Debug (trimmable static registrar)", TestVariation = "trimmable-static-registrar", Ignored = ignore };
					yield return new TestData { Variation = "Release (managed static registrar, all optimizations)", TestVariation = "release|managed-static-registrar-all-optimizations-linkall", Ignored = ignore };
					if (supports_coreclr) {
						yield return new TestData { Variation = "Debug (trimmable static registrar, all optimizations)", TestVariation = "trimmable-static-registrar-all-optimizations-linkall", Ignored = ignore };
						yield return new TestData { Variation = "Release (trimmable static registrar, all optimizations)", TestVariation = "release|trimmable-static-registrar-all-optimizations-linkall", Ignored = ignore };
					}
					yield return new TestData { Variation = $"Release (compat inline Class.GetHandle)", TestVariation = "inline-class-gethandle-compat|release", Ignored = ignore };
					yield return new TestData { Variation = $"Release (strict inline Class.GetHandle)", TestVariation = "inline-class-gethandle-strict|release", Ignored = ignore };
					yield return new TestData { Variation = $"Release (compat inline dlfcn)", TestVariation = "inline-dlfcn-methods-compat|release", Ignored = ignore };
					yield return new TestData { Variation = $"Release (strict inline dlfcn, link sdk)", TestVariation = "inline-dlfcn-methods-strict|linksdk|release", Ignored = ignore };
					yield return new TestData { Variation = $"Release (NativeAOT, .NET 11 defaults)", TestVariation = "nativeaot-net11-defaults|release", Ignored = ignore, RuntimeIdentifier = arm64_sim_runtime_identifier }; // it's necessary to specify RID, because NativeAOT defaults to building for device
					break;
				case "introspection":
					foreach (var target in test.Platform.GetTestTargetsForSimulator ())
						yield return new TestData {
							Variation = $"Debug ({test.Platform.GetSimulatorMinVersion ()})",
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
					yield return new TestData { Variation = "Debug (managed static registrar)", TestVariation = "managed-static-registrar", Ignored = ignore };
					if (supports_coreclr)
						yield return new TestData { Variation = "Debug (trimmable static registrar)", TestVariation = "trimmable-static-registrar", Ignored = ignore };
					yield return new TestData { Variation = "Debug (static registrar)", TestVariation = "static-registrar", Ignored = ignore, };
					yield return new TestData { Variation = "Release (managed static registrar)", TestVariation = "release|managed-static-registrar", Ignored = ignore };
					if (supports_coreclr)
						yield return new TestData { Variation = "Release (trimmable static registrar)", TestVariation = "trimmable-static-registrar", Ignored = ignore };
					yield return new TestData { Variation = "Release (managed static registrar, all optimizations)", TestVariation = "release|managed-static-registrar-all-optimizations-linkall", Ignored = ignore };
					if (supports_coreclr) {
						yield return new TestData { Variation = "Debug (trimmable static registrar, all optimizations)", TestVariation = "trimmable-static-registrar-all-optimizations-linkall", Ignored = ignore };
						yield return new TestData { Variation = "Release (trimmable static registrar, all optimizations)", TestVariation = "release|trimmable-static-registrar-all-optimizations-linkall", Ignored = ignore };
					}
					yield return new TestData { Variation = "Release (NativeAOT)", TestVariation = "release|nativeaot", Ignored = ignore };
					yield return new TestData { Variation = "Release (NativeAOT, x64)", TestVariation = "release|nativeaot", Ignored = !supports_x64 ? true : ignore, RuntimeIdentifier = x64_runtime_identifier };
					yield return new TestData { Variation = $"Release (NativeAOT, .NET 11 defaults)", TestVariation = "release|nativeaot-net11-defaults", Ignored = ignore };
					yield return new TestData { Variation = "Release (trimmable static registrar, NativeAOT)", TestVariation = "trimmable-static-registrar|nativeaot|release", Ignored = ignore };
					yield return new TestData { Variation = "Release (trimmable static registrar, NativeAOT, x64)", TestVariation = "trimmable-static-registrar|nativeaot|release", Ignored = !supports_x64 ? true : ignore, RuntimeIdentifier = x64_runtime_identifier };
					yield return new TestData { Variation = "Release (static registrar)", TestVariation = "release|static-registrar", Ignored = ignore };
					yield return new TestData { Variation = "Release (static registrar, all optimizations)", TestVariation = "release|static-registrar-all-optimizations-linkall", Ignored = ignore };
					yield return new TestData { Variation = $"Release (compat inline dlfcn)", TestVariation = "inline-dlfcn-methods-compat|release", Ignored = ignore };
					yield return new TestData { Variation = $"Release (strict inline dlfcn, link sdk)", TestVariation = "inline-dlfcn-methods-strict|linksdk|release", Ignored = ignore };
					break;
				}
				break;
			default:
				throw new NotImplementedException (test.ProjectPlatform);
			}
		}

		public IEnumerable<T> CreateTestVariations<T> (IEnumerable<T> tests, Func<MSBuildTask, T, IEnumerable<IDevice>?, T> creator) where T : RunTestTask
		{
			foreach (var task in tests) {
				if (string.IsNullOrEmpty (task.Variation))
					task.Variation = task.ProjectConfiguration?.Contains ("Debug") == true ? "Debug" : "Release";
			}

			var rv = new List<T> (tests);
			foreach (var task in tests.ToArray ()) {
				foreach (var test_data in GetTestData (task)) {
					var variation = test_data.Variation;
					var debug = !(test_data.TestVariation ?? "").Split ('|').Any (v => string.Equals (v, "release", StringComparison.OrdinalIgnoreCase));
					var configuration = debug ? task.ProjectConfiguration : task.ProjectConfiguration?.Replace ("Debug", "Release");
					var ignored = test_data.Ignored;
					var known_failure = test_data.KnownFailure;
					var candidates = test_data.Candidates;
					var runtime_identifer = test_data.RuntimeIdentifier;
					var test_variation = test_data.TestVariation;

					if (known_failure is not null)
						ignored = true;

					var clone = task.TestProject!.Clone ();
					var clone_task = Task.Run (async () => {
						await task.BuildTask.InitialTask!; // this is the project cloning above
						await clone.CreateCopyAsync (jenkins.MainLog, processManager, task, HarnessConfiguration.RootDirectory);

						if (!string.IsNullOrEmpty (runtime_identifer))
							clone.Xml.SetProperty ("RuntimeIdentifier", runtime_identifer);
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
					build.TestName = clone.Name ?? "";

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
