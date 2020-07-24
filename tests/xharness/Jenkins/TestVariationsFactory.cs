using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
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

			switch (test.ProjectPlatform) {
			case "iPhone":
				// arm64_32 is only supported for Release builds for now.
				// arm32 bits too big for debug builds - https://github.com/xamarin/maccore/issues/2080
				var supports_debug = test.Platform != TestPlatform.watchOS_64_32 && !(test.TestName == "dont link" && test.Platform == TestPlatform.iOS_Unified32);

				/* we don't add --assembly-build-target=@all=staticobject because that's the default in all our test projects */
				if (supports_debug) {
					yield return new TestData { Variation = "AssemblyBuildTarget: dylib (debug)", MTouchExtraArgs = $"--assembly-build-target=@all=dynamiclibrary {test.TestProject.MTouchExtraArgs}", Debug = true, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Dynamic };
					yield return new TestData { Variation = "AssemblyBuildTarget: SDK framework (debug)", MTouchExtraArgs = $"--assembly-build-target=@sdk=framework=Xamarin.Sdk --assembly-build-target=@all=staticobject {test.TestProject.MTouchExtraArgs}", Debug = true, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Static };
					yield return new TestData { Variation = "AssemblyBuildTarget: dylib (debug, profiling)", MTouchExtraArgs = $"--assembly-build-target=@all=dynamiclibrary {test.TestProject.MTouchExtraArgs}", Debug = true, Profiling = true, MonoNativeLinkMode = MonoNativeLinkMode.Dynamic };
					yield return new TestData { Variation = "AssemblyBuildTarget: SDK framework (debug, profiling)", MTouchExtraArgs = $"--assembly-build-target=@sdk=framework=Xamarin.Sdk --assembly-build-target=@all=staticobject {test.TestProject.MTouchExtraArgs}", Debug = true, Profiling = true, MonoNativeLinkMode = MonoNativeLinkMode.Static };
				}

				if (test.ProjectConfiguration.Contains ("Debug"))
					yield return new TestData { Variation = "Release", MTouchExtraArgs = test.TestProject.MTouchExtraArgs, Debug = false, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Static };
				if (test.Platform == TestPlatform.iOS_Unified32)
					yield return new TestData { Variation = "Release: UseThumb", MTouchExtraArgs = test.TestProject.MTouchExtraArgs, Debug = false, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Static, UseThumb = true };
				yield return new TestData { Variation = "AssemblyBuildTarget: SDK framework (release)", MTouchExtraArgs = $"--assembly-build-target=@sdk=framework=Xamarin.Sdk --assembly-build-target=@all=staticobject {test.TestProject.MTouchExtraArgs}", Debug = false, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Static };

				switch (test.TestName) {
				case "monotouch-test":
					if (supports_dynamic_registrar_on_device)
						yield return new TestData { Variation = "Debug (dynamic registrar)", MTouchExtraArgs = "--registrar:dynamic", Debug = true, Profiling = false };
					yield return new TestData { Variation = "Release (all optimizations)", MTouchExtraArgs = "--registrar:static --optimize:all", Debug = false, Profiling = false, Defines = "OPTIMIZEALL" };
					if (supports_debug) {
						yield return new TestData { Variation = "Debug (all optimizations)", MTouchExtraArgs = "--registrar:static --optimize:all", Debug = true, Profiling = false, Defines = "OPTIMIZEALL" };
						yield return new TestData { Variation = "Debug: SGenConc", MTouchExtraArgs = "", Debug = true, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Static, EnableSGenConc = true};
					}
					if (supports_interpreter) {
						if (supports_debug) {
							yield return new TestData { Variation = "Debug (interpreter)", MTouchExtraArgs = "--interpreter", Debug = true, Profiling = false, Undefines = "FULL_AOT_RUNTIME" };
							yield return new TestData { Variation = "Debug (interpreter -mscorlib)", MTouchExtraArgs = "--interpreter=-mscorlib", Debug = true, Profiling = false, Undefines = "FULL_AOT_RUNTIME" };
						}
						yield return new TestData { Variation = "Release (interpreter -mscorlib)", MTouchExtraArgs = "--interpreter=-mscorlib", Debug = false, Profiling = false, Undefines = "FULL_AOT_RUNTIME" };
					}
					break;
				case  string name when name.StartsWith ("mscorlib", StringComparison.Ordinal):
					if (supports_debug)
						yield return new TestData { Variation = "Debug: SGenConc", MTouchExtraArgs = "", Debug = true, Profiling = false, MonoNativeLinkMode = MonoNativeLinkMode.Static, EnableSGenConc = true};
					if (supports_interpreter) {
						if (supports_debug) {
							yield return new TestData { Variation = "Debug (interpreter)", MTouchExtraArgs = "--interpreter", Debug = true, Profiling = false, Undefines = "FULL_AOT_RUNTIME", KnownFailure = (HumanMessage: "#1683", IssueLink: "https://github.com/xamarin/maccore/issues/1683") };
							yield return new TestData { Variation = "Debug (interpreter -mscorlib)", MTouchExtraArgs = "--interpreter=-mscorlib", Debug = true, Profiling = false, Undefines = "FULL_AOT_RUNTIME", KnownFailure = (HumanMessage:"#1682", IssueLink:"https://github.com/xamarin/maccore/issues/1682") };
						}
						yield return new TestData { Variation = "Release (interpreter -mscorlib)", MTouchExtraArgs = "--interpreter=-mscorlib", Debug = false, Profiling = false, Undefines = "FULL_AOT_RUNTIME", KnownFailure = (HumanMessage:"#1682", IssueLink:"https://github.com/xamarin/maccore/issues/1682") };
					}
					break;
				}
				break;
			case "iPhoneSimulator":
				switch (test.TestName) {
				case "monotouch-test":
					// The default is to run monotouch-test with the dynamic registrar (in the simulator), so that's already covered
					yield return new TestData { Variation = "Debug (LinkSdk)", Debug = true, Profiling = false, LinkMode = "LinkSdk" };
					yield return new TestData { Variation = "Debug (static registrar)", MTouchExtraArgs = "--registrar:static", Debug = true, Profiling = false, Undefines = "DYNAMIC_REGISTRAR" };
					yield return new TestData { Variation = "Release (all optimizations)", MTouchExtraArgs = "--registrar:static --optimize:all", Debug = false, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Undefines = "DYNAMIC_REGISTRAR" };
					yield return new TestData { Variation = "Debug (all optimizations)", MTouchExtraArgs = "--registrar:static --optimize:all,-remove-uithread-checks", Debug = true, Profiling = false, LinkMode = "Full", Defines = "OPTIMIZEALL", Undefines = "DYNAMIC_REGISTRAR", Ignored = !jenkins.IncludeAll };
					break;
				case "introspection":
					if (test.TestProject.IsDotNetProject)
						break; // Our .NET 5 code hasn't implemented building using static libraries yet, and the iOS 10.3 simulator requires dylibs to be signed, which we don't do yet, thus this doesn't quite work yet for the iOS 10.3 simulator.
					foreach (var target in test.Platform.GetAppRunnerTargets ())
						yield return new TestData {
							Variation = $"Debug ({test.Platform.GetSimulatorMinVersion ()})",
							Debug = true,
							Candidates = jenkins.Simulators.SelectDevices (target, jenkins.SimulatorLoadLog, true),
							Ignored = !jenkins.IncludeOldSimulatorTests, 
						};
					break;
				}
				break;
			case "AnyCPU":
			case "x86":
				switch (test.TestName) {
				case "xammac tests":
					switch (test.ProjectConfiguration) {
					case "Release":
						yield return new TestData { Variation = "Release (all optimizations)", MonoBundlingExtraArgs = "--registrar:static --optimize:all", Debug = false, LinkMode = "Full", Defines = "OPTIMIZEALL"};
						break;
					case "Debug":
						yield return new TestData { Variation = "Debug (all optimizations)", MonoBundlingExtraArgs = "--registrar:static --optimize:all,-remove-uithread-checks", Debug = true, LinkMode = "Full", Defines = "OPTIMIZEALL", Ignored = !jenkins.IncludeAll };
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
					var mtouch_extra_args = test_data.MTouchExtraArgs;
					var bundling_extra_args = test_data.MonoBundlingExtraArgs;
					var configuration = test_data.Debug ? task.ProjectConfiguration : task.ProjectConfiguration.Replace ("Debug", "Release");
					var debug = test_data.Debug;
					var profiling = test_data.Profiling;
					var link_mode = test_data.LinkMode;
					var defines = test_data.Defines;
					var undefines = test_data.Undefines;
					var ignored = test_data.Ignored;
					var known_failure = test_data.KnownFailure;
					var candidates = test_data.Candidates;

					if (task.TestProject.IsDotNetProject)
						variation += " [dotnet]";

					if (known_failure.HasValue)
						ignored = true;

					var clone = task.TestProject.Clone ();
					var clone_task = Task.Run (async () => {
						await task.BuildTask.InitialTask; // this is the project cloning above
						await clone.CreateCopyAsync (jenkins.MainLog, processManager, task);

						var isMac = task.Platform.IsMac ();
						var canSymlink = task.Platform.CanSymlink();

						if (!string.IsNullOrEmpty (mtouch_extra_args))
							clone.Xml.AddExtraMtouchArgs (mtouch_extra_args, task.ProjectPlatform, configuration);
						if (!string.IsNullOrEmpty (bundling_extra_args))
							clone.Xml.AddMonoBundlingExtraArgs (bundling_extra_args, task.ProjectPlatform, configuration);
						if (!string.IsNullOrEmpty (link_mode))
							clone.Xml.SetNode (isMac ? "LinkMode" : "MtouchLink", link_mode, task.ProjectPlatform, configuration);
						if (!string.IsNullOrEmpty (defines)) {
							clone.Xml.AddAdditionalDefines (defines, task.ProjectPlatform, configuration);
							if (clone.ProjectReferences != null) {
								foreach (var pr in clone.ProjectReferences) {
									pr.Xml.AddAdditionalDefines (defines, task.ProjectPlatform, configuration);
									pr.Xml.Save (pr.Path);
								}
							}
						}
						if (!string.IsNullOrEmpty (undefines)) {
							clone.Xml.RemoveDefines (undefines, task.ProjectPlatform, configuration);
							if (clone.ProjectReferences != null) {
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
							clone.Xml.SetNode ("MtouchEnableSGenConc", "true", task.ProjectPlatform, configuration);
						if (test_data.UseThumb) // no need to check the platform, already done at the data iterator
							clone.Xml.SetNode ("MtouchUseThumb", "true", task.ProjectPlatform, configuration);

						if (!debug && !isMac)
							clone.Xml.SetMtouchUseLlvm (true, task.ProjectPlatform, configuration);
						clone.Xml.Save (clone.Path);
					});

					MSBuildTask build;
					if (clone.IsDotNetProject) {
						build = new DotNetBuildTask (jenkins: jenkins, testProject: clone, processManager: processManager);
						ignored |= !jenkins.IncludeDotNet;
					} else {
						build = new MSBuildTask (jenkins: jenkins, testProject: clone, processManager: processManager);
					}
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
