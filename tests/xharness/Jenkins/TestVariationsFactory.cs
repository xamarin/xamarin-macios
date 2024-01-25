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

			// 32-bit interpreter doesn't work yet: https://github.com/mono/mono/issues/9871
			var supports_interpreter = test.Platform != TestPlatform.iOS_Unified32;
			var supports_dynamic_registrar_on_device = test.Platform == TestPlatform.iOS_Unified64 || test.Platform == TestPlatform.tvOS;
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
			case TestPlatform.iOS_Unified:
			case TestPlatform.iOS_Unified64:
				arm64_sim_runtime_identifier = "iossimulator-arm64";
				x64_sim_runtime_identifier = "iossimulator-x64";
				break;
			case TestPlatform.tvOS:
				arm64_sim_runtime_identifier = "tvossimulator-arm64";
				x64_sim_runtime_identifier = "tvossimulator-x64";
				break;
			}

			switch (test.ProjectPlatform) {
			case "iPhone":
				// arm64_32 is only supported for Release builds for now.
				// arm32 bits too big for debug builds - https://github.com/xamarin/maccore/issues/2080
				var supports_debug = test.Platform != TestPlatform.watchOS_64_32 && !(test.TestName == "dont link" && test.Platform == TestPlatform.iOS_Unified32);

				/* we don't add --assembly-build-target=@all=staticobject because that's the default in all our test projects */
				if (supports_debug && !test.TestProject.IsDotNetProject) {
					yield return new TestData { Variation = "AssemblyBuildTarget: dylib (debug)", BundlerArguments = $"--assembly-build-target=@all=dynamiclibrary", Debug = true, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Dynamic, Ignored = ignore };
					yield return new TestData { Variation = "AssemblyBuildTarget: SDK framework (debug)", BundlerArguments = $"--assembly-build-target=@sdk=framework=Xamarin.Sdk --assembly-build-target=@all=staticobject", Debug = true, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Static, Ignored = ignore };
					yield return new TestData { Variation = "AssemblyBuildTarget: dylib (debug, profiling)", BundlerArguments = $"--assembly-build-target=@all=dynamiclibrary", Debug = true, Profiling = true, MonoNativeLinkMode = MonoNativeLinkMode.Dynamic, Ignored = ignore };
					yield return new TestData { Variation = "AssemblyBuildTarget: SDK framework (debug, profiling)", BundlerArguments = $"--assembly-build-target=@sdk=framework=Xamarin.Sdk --assembly-build-target=@all=staticobject", Debug = true, Profiling = true, MonoNativeLinkMode = MonoNativeLinkMode.Static, Ignored = ignore };
				}

				if (test.ProjectConfiguration.Contains ("Debug"))
					yield return new TestData { Variation = "Release", Debug = false, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Static };
				if (test.Platform == TestPlatform.iOS_Unified32 && !test.TestProject.IsDotNetProject)
					yield return new TestData { Variation = "Release: UseThumb", Debug = false, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Static, UseThumb = true };

				// Disable this by default for tvOS/Release because the app ends up being too big (https://github.com/xamarin/maccore/issues/2282)
				var sdk_release_skip = test.Platform == TestPlatform.tvOS && test.TestName == "dont link";
				sdk_release_skip = sdk_release_skip || test.TestProject.IsDotNetProject;
				if (!sdk_release_skip)
					yield return new TestData { Variation = "AssemblyBuildTarget: SDK framework (release)", BundlerArguments = $"--assembly-build-target=@sdk=framework=Xamarin.Sdk --assembly-build-target=@all=staticobject", Debug = false, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Static, };

				switch (test.TestName) {
				case "monotouch-test":
					if (test.TestProject.IsDotNetProject)
						ignore = true;
					if (supports_dynamic_registrar_on_device)
						yield return new TestData { Variation = "Debug (dynamic registrar)", Registrar = "dynamic", Debug = true, Profiling = false, Ignored = ignore };
					yield return new TestData { Variation = "Release (all optimizations)", BundlerArguments = "--optimize:all", Registrar = "static", Debug = false, Profiling = false, Defines = "OPTIMIZEALL", Ignored = ignore };
					if (supports_debug) {
						yield return new TestData { Variation = "Debug (all optimizations)", BundlerArguments = "--optimize:all", Registrar = "static", Debug = true, Profiling = false, Defines = "OPTIMIZEALL", Ignored = ignore };
						yield return new TestData { Variation = "Debug: SGenConc", BundlerArguments = "", Debug = true, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Static, EnableSGenConc = true, Ignored = ignore };
					}
					if (supports_interpreter) {
						if (supports_debug) {
							yield return new TestData { Variation = "Debug (interpreter)", BundlerArguments = "--interpreter", Debug = true, Profiling = false, Undefines = "FULL_AOT_RUNTIME", Ignored = ignore };
							yield return new TestData { Variation = "Debug (interpreter -mscorlib)", BundlerArguments = "--interpreter=-mscorlib", Debug = true, Profiling = false, Undefines = "FULL_AOT_RUNTIME", Ignored = ignore };
						}
						yield return new TestData { Variation = "Release (interpreter -mscorlib)", BundlerArguments = "--interpreter=-mscorlib", Debug = false, Profiling = false, Undefines = "FULL_AOT_RUNTIME", Ignored = ignore };
					}
					if (test.TestProject.IsDotNetProject) {
						yield return new TestData { Variation = "Release (LLVM)", Debug = false, UseLlvm = true, Ignored = ignore };
						yield return new TestData { Variation = "Debug (managed static registrar)", Registrar = "managed-static", Debug = true, Profiling = false, Ignored = ignore };
						yield return new TestData { Variation = "Release (managed static registrar, all optimizations)", BundlerArguments = "--optimize:all", Registrar = "managed-static", Debug = false, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = ignore };
					}
					if (test.TestProject.IsDotNetProject)
						yield return new TestData { Variation = "Release (NativeAOT)", Debug = false, PublishAot = true, Ignored = ignore, Defines = "NATIVEAOT", LinkMode = "Full" };
					break;
				case string name when name.StartsWith ("mscorlib", StringComparison.Ordinal):
					if (supports_debug)
						yield return new TestData { Variation = "Debug: SGenConc", BundlerArguments = "", Debug = true, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Static, EnableSGenConc = true };
					if (supports_interpreter) {
						if (supports_debug) {
							yield return new TestData { Variation = "Debug (interpreter)", BundlerArguments = "--interpreter", Debug = true, Profiling = false, Undefines = "FULL_AOT_RUNTIME", KnownFailure = new KnownIssue ("#1683", issueLink: "https://github.com/xamarin/maccore/issues/1683") };
							yield return new TestData { Variation = "Debug (interpreter -mscorlib)", BundlerArguments = "--interpreter=-mscorlib", Debug = true, Profiling = false, Undefines = "FULL_AOT_RUNTIME", KnownFailure = new KnownIssue ("#1682", issueLink: "https://github.com/xamarin/maccore/issues/1682") };
						}
						yield return new TestData { Variation = "Release (interpreter -mscorlib)", BundlerArguments = "--interpreter=-mscorlib", Debug = false, Profiling = false, Undefines = "FULL_AOT_RUNTIME", KnownFailure = new KnownIssue ("#1682", issueLink: "https://github.com/xamarin/maccore/issues/1682") };
					}
					break;
				}
				break;
			case "iPhoneSimulator":
				if (test.Platform == TestPlatform.iOS_Unified32)
					ignore = true;

				switch (test.TestName) {
				case "monotouch-test":
					// The default is to run monotouch-test with the dynamic registrar (in the simulator), so that's already covered
					yield return new TestData { Variation = "Debug (LinkSdk)", Debug = true, Profiling = false, LinkMode = test.TestProject.IsDotNetProject ? "SdkOnly" : "LinkSdk", Ignored = ignore };
					yield return new TestData { Variation = "Debug (static registrar)", Registrar = "static", Debug = true, Profiling = false, Ignored = ignore };
					yield return new TestData { Variation = "Release (all optimizations)", BundlerArguments = "--optimize:all", Registrar = "static", Debug = false, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = ignore };
					yield return new TestData { Variation = "Debug (all optimizations)", BundlerArguments = "--optimize:all,-remove-uithread-checks", Registrar = "static", Debug = true, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = ignore ?? !jenkins.TestSelection.IsEnabled (TestLabel.All) };

					if (test.TestProject.IsDotNetProject && mac_supports_arm64) {
						yield return new TestData { Variation = "Debug (ARM64)", Debug = true, Profiling = false, Ignored = !mac_supports_arm64 ? true : ignore, RuntimeIdentifier = arm64_sim_runtime_identifier, };
						yield return new TestData { Variation = "Release (NativeAOT, ARM64)", Debug = false, PublishAot = true, Ignored = ignore, Defines = "NATIVEAOT", RuntimeIdentifier = arm64_sim_runtime_identifier, LinkMode = "Full" };
					}
					if (test.TestProject.IsDotNetProject) {
						yield return new TestData { Variation = "Debug (managed static registrar)", Registrar = "managed-static", Debug = true, Profiling = false, Ignored = ignore };
						yield return new TestData { Variation = "Release (managed static registrar, all optimizations)", BundlerArguments = "--optimize:all", Registrar = "managed-static", Debug = false, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = ignore };
						yield return new TestData { Variation = "Release (NativeAOT, x64)", Debug = false, PublishAot = true, Ignored = ignore, Defines = "NATIVEAOT", RuntimeIdentifier = x64_sim_runtime_identifier, LinkMode = "Full" };
					}
					break;
				case "introspection":
					if (test.TestProject.IsDotNetProject && mac_supports_arm64)
						yield return new TestData { Variation = "Debug (ARM64)", Debug = true, Profiling = false, Ignored = !mac_supports_arm64 ? true : ignore, RuntimeIdentifier = arm64_sim_runtime_identifier, };

					foreach (var target in test.Platform.GetAppRunnerTargets ())
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
				switch (test.TestName) {
				case "monotouch-test":
					if (test.TestProject.IsDotNetProject) {
						yield return new TestData { Variation = "Debug (ARM64)", Debug = true, Profiling = false, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac) || !mac_supports_arm64, RuntimeIdentifier = arm64_runtime_identifier, };
						if (test.Platform != TestPlatform.MacCatalyst) {
							yield return new TestData { Variation = "Debug (static registrar)", Registrar = "static", Debug = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac), };
							yield return new TestData { Variation = "Debug (static registrar, ARM64)", Registrar = "static", Debug = true, Profiling = false, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac) || !mac_supports_arm64, RuntimeIdentifier = arm64_runtime_identifier, };
							yield return new TestData { Variation = "Release (NativeAOT, ARM64)", Debug = false, PublishAot = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac) || !mac_supports_arm64, Defines = "NATIVEAOT", RuntimeIdentifier = arm64_runtime_identifier, LinkMode = "Full" };
							yield return new TestData { Variation = "Release (NativeAOT, x64)", Debug = false, PublishAot = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac), Defines = "NATIVEAOT", RuntimeIdentifier = "osx-x64", LinkMode = "Full" };
						}
						if (test.Platform == TestPlatform.MacCatalyst) {
							yield return new TestData { Variation = "Release (ARM64, LLVM)", Debug = false, UseLlvm = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst) || !mac_supports_arm64, RuntimeIdentifier = arm64_runtime_identifier };
							yield return new TestData { Variation = "Release", Debug = false, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst) };
							yield return new TestData { Variation = "Release (NativeAOT)", Debug = false, PublishAot = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst), Defines = "NATIVEAOT", LinkMode = "Full" };
							yield return new TestData { Variation = "Release (NativeAOT, ARM64)", Debug = false, PublishAot = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst) || !mac_supports_arm64, Defines = "NATIVEAOT", RuntimeIdentifier = arm64_runtime_identifier, LinkMode = "Full" };
							yield return new TestData { Variation = "Release (NativeAOT, x64)", Debug = false, PublishAot = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.MacCatalyst), Defines = "NATIVEAOT", LinkMode = "Full", RuntimeIdentifier = x64_runtime_identifier };
						}
						if (test.Platform == TestPlatform.Mac) {
							yield return new TestData { Variation = "Release", Debug = false, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac) };
							yield return new TestData { Variation = "Release (NativeAOT)", Debug = false, PublishAot = true, Ignored = !jenkins.TestSelection.IsEnabled (TestLabel.Monotouch) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac), Defines = "NATIVEAOT", LinkMode = "Full" };
						}
						if (test.TestProject.IsDotNetProject) {
							yield return new TestData { Variation = "Release (all optimizations)", BundlerArguments = "--optimize:all", Registrar = "static", Debug = false, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = ignore };
							yield return new TestData { Variation = "Debug (managed static registrar)", Registrar = "managed-static", Debug = true, Profiling = false, Ignored = ignore };
							yield return new TestData { Variation = "Release (managed static registrar, all optimizations)", BundlerArguments = "--optimize:all", Registrar = "managed-static", Debug = false, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = ignore };
						}
					}
					break;
				case "xammac tests":
					switch (test.ProjectConfiguration) {
					case "Release":
						yield return new TestData { Variation = "Release (all optimizations)", BundlerArguments = "--optimize:all", Registrar = "static", Debug = false, LinkMode = "Full", Defines = "OPTIMIZEALL" };
						yield return new TestData { Variation = "Release (ARM64)", XamMacArch = "ARM64", Debug = false, Ignored = !mac_supports_arm64 || !jenkins.TestSelection.IsEnabled (TestLabel.Xammac) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac) };
						break;
					case "Debug":
						yield return new TestData { Variation = "Debug (all optimizations)", BundlerArguments = "--optimize:all,-remove-uithread-checks", Registrar = "static", Debug = true, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = !(jenkins.TestSelection.IsEnabled (TestLabel.All) && jenkins.TestSelection.IsEnabled (PlatformLabel.Mac)) };
						yield return new TestData { Variation = "Debug (ARM64)", XamMacArch = "ARM64", Debug = true, Ignored = !mac_supports_arm64 || !jenkins.TestSelection.IsEnabled (TestLabel.Xammac) || !jenkins.TestSelection.IsEnabled (PlatformLabel.Mac) };
						break;
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

				if (task.TestProject.IsDotNetProject)
					task.Variation += " [dotnet]";
			}

			var rv = new List<T> (tests);
			foreach (var task in tests.ToArray ()) {
				foreach (var test_data in GetTestData (task)) {
					var variation = test_data.Variation;
					var bundler_arguments = test_data.BundlerArguments;
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
					var xammac_arch = test_data.XamMacArch;
					var runtime_identifer = test_data.RuntimeIdentifier;
					var use_llvm = test_data.UseLlvm;
					var registrar = test_data.Registrar;
					var publishaot = test_data.PublishAot;

					if (task.TestProject.IsDotNetProject)
						variation += " [dotnet]";

					if (known_failure is not null)
						ignored = true;

					var clone = task.TestProject.Clone ();
					var clone_task = Task.Run (async () => {
						await task.BuildTask.InitialTask; // this is the project cloning above
						await clone.CreateCopyAsync (jenkins.MainLog, processManager, task, HarnessConfiguration.RootDirectory);

						var isMac = task.Platform.IsMac ();
						var canSymlink = task.Platform.CanSymlink ();

						if (!string.IsNullOrEmpty (bundler_arguments)) {
							clone.Xml.AppendExtraMtouchArgs (bundler_arguments);
							clone.Xml.AppendMonoBundlingExtraArgs (bundler_arguments);
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
						if (test_data.MonoNativeLinkMode != MonoNativeLinkMode.None) {
							var mono_native_link = test_data.MonoNativeLinkMode;
							if (!canSymlink && mono_native_link == MonoNativeLinkMode.Symlink)
								mono_native_link = MonoNativeLinkMode.Static;
							MonoNativeHelper.AddProjectDefines (clone.Xml, mono_native_link);
						}
						if (test_data.EnableSGenConc)
							clone.Xml.SetProperty ("EnableSGenConc", "true");
						if (test_data.UseThumb) // no need to check the platform, already done at the data iterator
							clone.Xml.SetProperty ("MtouchUseThumb", "true");
						if (use_llvm)
							clone.Xml.SetProperty ("MtouchUseLlvm", "true");

						if (!debug && !isMac)
							clone.Xml.SetMtouchUseLlvm (true, task.ProjectPlatform, configuration);
						if (use_mono_runtime.HasValue)
							clone.Xml.SetProperty ("UseMonoRuntime", use_mono_runtime.Value ? "true" : "false");
						if (!string.IsNullOrEmpty (xammac_arch))
							clone.Xml.SetProperty ("XamMacArch", xammac_arch);
						if (!string.IsNullOrEmpty (runtime_identifer))
							clone.Xml.SetProperty ("RuntimeIdentifier", runtime_identifer);
						if (!string.IsNullOrEmpty (registrar))
							clone.Xml.SetProperty ("Registrar", registrar);
						if (publishaot) {
							clone.Xml.SetProperty ("PublishAot", "true");
							clone.Xml.SetProperty ("_IsPublishing", "true"); // quack like "dotnet publish", otherwise PublishAot=true has no effect.
							clone.Xml.SetProperty ("IlcTreatWarningsAsErrors", "false"); // We're enabling warnaserror by default, but we're not warning-free for ILC (especially for NUnit), so disable warnaserror for ILC - https://github.com/xamarin/xamarin-macios/issues/19911
						}
						clone.Xml.Save (clone.Path);
					});

					ignored |= clone.IsDotNetProject && !jenkins.TestSelection.IsEnabled (PlatformLabel.Dotnet);

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
