using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.Common;
using Microsoft.DotNet.XHarness.Common.Execution;
using Microsoft.DotNet.XHarness.Common.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Xharness.Targets;

using Xamarin.Utils;

namespace Xharness {
	public enum HarnessAction {
		None,
		Configure,
		Run,
		Install,
		Uninstall,
		Jenkins,
	}

	public class HarnessConfiguration {
		public bool AutoConf { get; set; }
		public string BuildConfiguration { get; set; } = "Debug";
		public bool DryRun { get; set; }
		public Dictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string> ();
		public bool? IncludeSystemPermissionTests { get; set; }
		public List<iOSTestProject> IOSTestProjects { get; set; } = new List<iOSTestProject> ();
		public string JenkinsConfiguration { get; set; }
		public HashSet<string> Labels { get; set; } = new HashSet<string> ();
		public string LogDirectory { get; set; } = Environment.CurrentDirectory;
		public string MarkdownSummaryPath { get; set; }
		public string PeriodicCommand { get; set; }
		public string PeriodicCommandArguments { get; set; }
		public TimeSpan PeriodicCommandInterval { get; set; }
		public string SdkRoot { get; set; }
		public TestTarget Target { get; set; }
		public double TimeoutInMinutes { get; set; } = 15;
		public bool UseSystemXamarinIOSMac { get; set; }
		public int Verbosity { get; set; }
		public string WatchOSAppTemplate { get; set; }
		public string WatchOSContainerTemplate { get; set; }
		public XmlResultJargon XmlJargon { get; set; } = XmlResultJargon.NUnitV3;

		// This is the maccore/tests directory.
		static string root_directory;
		public static string RootDirectory {
			get {
				if (root_directory is null) {
					var testAssemblyDirectory = Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly ().Location);
					var dir = testAssemblyDirectory;
					var path = Path.Combine (testAssemblyDirectory, ".git");
					while (!Directory.Exists (path) && path.Length > 3) {
						dir = Path.GetDirectoryName (dir);
						path = Path.Combine (dir, ".git");
					}
					if (!Directory.Exists (path))
						throw new Exception ("Could not find the xamarin-macios repo.");
					path = Path.Combine (Path.GetDirectoryName (path), "tests");
					if (!Directory.Exists (path))
						throw new Exception ("Could not find the tests directory.");
					root_directory = Path.GetFullPath (path);
				}
				return root_directory;
			}
			set {
				root_directory = value;
				if (root_directory is not null)
					root_directory = Path.GetFullPath (root_directory).TrimEnd ('/');
			}
		}

		static SemaphoreSlim ls_files_semaphore = new SemaphoreSlim (1);
		static List<string> files_in_git = new List<string> ();
		public static async Task<IEnumerable<string>> ListFilesInGitAsync (ILog log, string test_dir, IProcessManager processManager)
		{
			var acquired = await ls_files_semaphore.WaitAsync (TimeSpan.FromMinutes (5));
			try {
				if (!acquired)
					log.WriteLine ($"Unable to acquire lock to run 'git ls-files {test_dir}' in 5 minutes; will try to run anyway.");
				if (files_in_git.Count == 0) {
					using var process = new Process ();
					process.StartInfo.FileName = "git";
					process.StartInfo.Arguments = "ls-files";
					process.StartInfo.WorkingDirectory = RootDirectory;
					var stdout = new MemoryLog () { Timestamp = false };
					var result = await processManager.RunAsync (process, log, stdout, stdout, timeout: TimeSpan.FromSeconds (60));
					if (!result.Succeeded)
						throw new Exception ($"Failed to list the files in the directory {test_dir} (TimedOut: {result.TimedOut} ExitCode: {result.ExitCode}):\n{stdout}");
					files_in_git.AddRange (stdout.ToString ().Split ('\n'));
				}
				var relative_dir = Path.GetFullPath (test_dir);
				if (relative_dir.StartsWith (RootDirectory)) {
					relative_dir = relative_dir.Substring (RootDirectory.Length);
					relative_dir = relative_dir.TrimStart ('/');
				}
				if (!relative_dir.EndsWith ("/", StringComparison.Ordinal))
					relative_dir += "/";
				return files_in_git
					.Where (v => v.StartsWith (relative_dir, StringComparison.OrdinalIgnoreCase))
					.Select (v => v.Substring (relative_dir.Length));
			} finally {
				if (acquired)
					ls_files_semaphore.Release ();
			}
		}

		public static string EvaluateRootTestsDirectory (string value)
		{
			return value.Replace ("$(RootTestsDirectory)", RootDirectory);
		}

		public static string InjectRootTestsDirectory (string value)
		{
			return value.Replace (RootDirectory, "$(RootTestsDirectory)");
		}
	}

	public class Harness : IHarness {
		readonly TestTarget target;
		readonly string buildConfiguration = "Debug";
		readonly IMlaunchProcessManager processManager;

		public static readonly IHelpers Helpers = new Helpers ();

		public HarnessAction Action { get; }
		public int Verbosity { get; }
		public IFileBackedLog HarnessLog { get; set; }
		public HashSet<string> Labels { get; }
		public XmlResultJargon XmlJargon { get; }
		public IResultParser ResultParser { get; }
		public ITunnelBore TunnelBore { get; }
		public AppBundleLocator AppBundleLocator { get; }

		public string XIBuildPath => Path.GetFullPath (Path.Combine (RootDirectory, "..", "tools", "xibuild", "xibuild"));

		string sdkRoot;
		string SdkRoot {
			get => sdkRoot;
			set {
				sdkRoot = value;
				XcodeRoot = FindXcode (sdkRoot);
			}
		}

		string MlaunchPath {
			get {
				if (ENABLE_DOTNET) {
					ApplePlatform platform;
					if (INCLUDE_IOS) {
						platform = ApplePlatform.iOS;
					} else if (INCLUDE_TVOS) {
						platform = ApplePlatform.TVOS;
					} else {
						return $"Not building any mobile platform, so can't provide a location to mlaunch.";
					}
					var sdkPlatform = platform.AsString ().ToUpperInvariant ();
					var sdkName = GetVariable ($"{sdkPlatform}_NUGET_SDK_NAME");
					// there is a diff between getting the path for the current platform when running on CI or off CI. The config files in the CI do not 
					// contain the correct workload version, the reason for this is that the workload is built in a different machine which means that
					// the Make.config will use the wrong version. The CI set the version in the environment variable {platform}_WORKLOAD_VERSION via a script.
					var workloadVersion = GetVariable ($"{sdkPlatform}_WORKLOAD_VERSION");
					var sdkVersion = GetVariable ($"{sdkPlatform}_NUGET_VERSION_NO_METADATA");
					return Path.Combine (DOTNET_DIR, "packs", sdkName, string.IsNullOrEmpty (workloadVersion) ? sdkVersion : workloadVersion, "tools", "bin", "mlaunch");
				} else if (INCLUDE_XAMARIN_LEGACY && INCLUDE_IOS) {
					return Path.Combine (IOS_DESTDIR, "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current", "bin", "mlaunch");
				}
				return $"Not building any mobile platform, so can't provide a location to mlaunch.";
			}
		}

		bool IsVariableSet (string variable)
		{
			return !string.IsNullOrEmpty (GetVariable (variable));
		}

		string GetVariable (string variable, string @default = null)
		{
			var result = Environment.GetEnvironmentVariable (variable);
			if (string.IsNullOrEmpty (result))
				config.TryGetValue (variable, out result);
			if (string.IsNullOrEmpty (result))
				result = @default;
			return result;
		}

		public List<iOSTestProject> IOSTestProjects { get; }
		public List<MacTestProject> MacTestProjects { get; } = new List<MacTestProject> ();

		// Configure
		readonly bool useSystemXamarinIOSMac; // if the system XI/XM should be used, or the locally build XI/XM.
		readonly bool autoConf;

		public string WatchOSContainerTemplate { get; private set; }
		public string WatchOSAppTemplate { get; private set; }
		public string WatchOSExtensionTemplate { get; private set; }
		public string TodayContainerTemplate { get; private set; }
		public string TodayExtensionTemplate { get; private set; }
		public string BCLTodayExtensionTemplate { get; private set; }
		public string MONO_PATH { get; } // Use same name as in Makefiles, so that a grep finds it.
		public string TVOS_MONO_PATH { get; } // Use same name as in Makefiles, so that a grep finds it.
		public bool INCLUDE_IOS { get; }
		public bool INCLUDE_TVOS { get; }
		public bool INCLUDE_WATCH { get; }
		public bool INCLUDE_MAC { get; }
		public bool INCLUDE_MACCATALYST { get; }
		public string JENKINS_RESULTS_DIRECTORY { get; } // Use same name as in Makefiles, so that a grep finds it.
		public string MAC_DESTDIR { get; }
		public string IOS_DESTDIR { get; }
		public string MONO_IOS_SDK_DESTDIR { get; }
		public string MONO_MAC_SDK_DESTDIR { get; }
		public bool ENABLE_DOTNET { get; }
		public bool INCLUDE_XAMARIN_LEGACY { get; }
		public string SYSTEM_MONO { get; set; }
		public string DOTNET_DIR { get; set; }
		public string DOTNET_TFM { get; set; }

		// Run

		public string XcodeRoot { get; private set; }
		public string LogDirectory { get; } = Environment.CurrentDirectory;
		public double Timeout { get; } = 15; // in minutes
		public double LaunchTimeout { get; } // in minutes
		public bool DryRun { get; } // Most things don't support this. If you need it somewhere, implement it!
		public string JenkinsConfiguration { get; }
		public Dictionary<string, string> EnvironmentVariables { get; } = new Dictionary<string, string> ();
		public string MarkdownSummaryPath { get; }
		public string PeriodicCommand { get; }
		public string PeriodicCommandArguments { get; }
		public TimeSpan PeriodicCommandInterval { get; }
		// whether tests that require access to system resources (system contacts, photo library, etc) should be executed or not
		public bool? IncludeSystemPermissionTests { get; set; }

		string RootDirectory => HarnessConfiguration.RootDirectory;
		Dictionary<string, string> config;

		public Harness (IResultParser resultParser, HarnessAction action, HarnessConfiguration configuration)
		{
			ResultParser = resultParser ?? throw new ArgumentNullException (nameof (resultParser));
			Action = action;

			if (configuration is null)
				throw new ArgumentNullException (nameof (configuration));

			autoConf = configuration.AutoConf;
			buildConfiguration = configuration.BuildConfiguration ?? throw new ArgumentNullException (nameof (configuration));
			DryRun = configuration.DryRun;
			IncludeSystemPermissionTests = configuration.IncludeSystemPermissionTests;
			IOSTestProjects = configuration.IOSTestProjects;
			JenkinsConfiguration = configuration.JenkinsConfiguration;
			LogDirectory = configuration.LogDirectory ?? throw new ArgumentNullException (nameof (configuration.LogDirectory));
			MarkdownSummaryPath = configuration.MarkdownSummaryPath;
			PeriodicCommand = configuration.PeriodicCommand;
			PeriodicCommandArguments = configuration.PeriodicCommandArguments;
			PeriodicCommandInterval = configuration.PeriodicCommandInterval;
			target = configuration.Target;
			Timeout = configuration.TimeoutInMinutes;
			useSystemXamarinIOSMac = configuration.UseSystemXamarinIOSMac;
			if (!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("TESTS_USE_SYSTEM")))
				useSystemXamarinIOSMac = true;
			Verbosity = configuration.Verbosity;
			WatchOSAppTemplate = configuration.WatchOSAppTemplate;
			WatchOSContainerTemplate = configuration.WatchOSContainerTemplate;
			XmlJargon = configuration.XmlJargon;

			if (configuration.Labels is not null)
				Labels = new HashSet<string> (configuration.Labels);

			if (configuration.EnvironmentVariables is not null)
				EnvironmentVariables = new Dictionary<string, string> (configuration.EnvironmentVariables);

			LaunchTimeout = InCI ? 3 : 120;

			config = ParseConfigFiles ();
			var src_root = Path.GetDirectoryName (Path.GetFullPath (RootDirectory));

			MONO_PATH = Path.GetFullPath (Path.Combine (src_root, "external", "mono"));
			TVOS_MONO_PATH = MONO_PATH;
			INCLUDE_IOS = IsVariableSet (nameof (INCLUDE_IOS));
			INCLUDE_TVOS = IsVariableSet (nameof (INCLUDE_TVOS));
			JENKINS_RESULTS_DIRECTORY = GetVariable (nameof (JENKINS_RESULTS_DIRECTORY));
			INCLUDE_WATCH = IsVariableSet (nameof (INCLUDE_WATCH));
			INCLUDE_MAC = IsVariableSet (nameof (INCLUDE_MAC));
			INCLUDE_MACCATALYST = IsVariableSet (nameof (INCLUDE_MACCATALYST));
			MAC_DESTDIR = GetVariable (nameof (MAC_DESTDIR));
			IOS_DESTDIR = GetVariable (nameof (IOS_DESTDIR));
			MONO_IOS_SDK_DESTDIR = GetVariable (nameof (MONO_IOS_SDK_DESTDIR));
			MONO_MAC_SDK_DESTDIR = GetVariable (nameof (MONO_MAC_SDK_DESTDIR));
			ENABLE_DOTNET = IsVariableSet (nameof (ENABLE_DOTNET));
			SYSTEM_MONO = GetVariable (nameof (SYSTEM_MONO));
			DOTNET_DIR = GetVariable (nameof (DOTNET_DIR));
			INCLUDE_XAMARIN_LEGACY = IsVariableSet (nameof (INCLUDE_XAMARIN_LEGACY));
			DOTNET_TFM = GetVariable (nameof (DOTNET_TFM));

			if (string.IsNullOrEmpty (SdkRoot))
				SdkRoot = GetVariable ("XCODE_DEVELOPER_ROOT", configuration.SdkRoot);

			processManager = new MlaunchProcessManager (XcodeRoot, MlaunchPath);
			AppBundleLocator = new AppBundleLocator (processManager, () => HarnessLog, XIBuildPath, "/usr/local/share/dotnet/dotnet", GetVariable ("DOTNET"));
			TunnelBore = new TunnelBore (processManager);
		}

		public bool GetIncludeSystemPermissionTests (TestPlatform platform, bool device)
		{
			// If we've been told something in particular, that takes precedence.
			if (IncludeSystemPermissionTests.HasValue)
				return IncludeSystemPermissionTests.Value;

			// If we haven't been told, try to be smart.
			switch (platform) {
			case TestPlatform.iOS:
			case TestPlatform.Mac:
			case TestPlatform.Mac_Full:
			case TestPlatform.Mac_Modern:
			case TestPlatform.Mac_System:
				// On macOS we can't edit the TCC database easily
				// (it requires adding the mac has to be using MDM: https://carlashley.com/2018/09/28/tcc-round-up/)
				// So by default ignore any tests that would pop up permission dialogs in CI.
				return !InCI;
			default:
				// On device we have the same issue as on the mac: we can't edit the TCC database.
				if (device)
					return !InCI;
				// But in the simulator we can just write to the simulator's TCC database (and we do)
				return true;
			}
		}

		static string FindXcode (string path)
		{
			if (string.IsNullOrEmpty (path))
				return path;

			string originalPath = path;

			do {
				if (path == "/") {
					throw new Exception (string.Format ("Could not find Xcode.app in {0}", originalPath));
				} else if (File.Exists (Path.Combine (path, "Contents", "MacOS", "Xcode"))) {
					return path;
				}

				path = Path.GetDirectoryName (path);
			} while (true);
		}

		void AutoConfigureDotNet ()
		{
			string [] noConfigurations = null;
			var debugAndRelease = new string [] { "Debug", "Release" };

			var projects = new [] {
				new { Label = TestLabel.Introspection ,ProjectPath = "introspection", IsFSharp = false, Configurations = noConfigurations, },
				new { Label = TestLabel.Monotouch, ProjectPath = "monotouch-test", IsFSharp = false, Configurations = noConfigurations, },
				new { Label = TestLabel.Linker,ProjectPath = Path.Combine ("linker", "ios", "dont link"), IsFSharp = false, Configurations = debugAndRelease, },
				new { Label = TestLabel.Linker,ProjectPath = Path.Combine ("linker", "ios", "link sdk"), IsFSharp = false, Configurations = debugAndRelease, },
				new { Label = TestLabel.Linker,ProjectPath = Path.Combine ("linker", "ios", "link all"), IsFSharp = false, Configurations = debugAndRelease, },
				new { Label = TestLabel.Linker,ProjectPath = Path.Combine ("linker", "ios", "trimmode copy"), IsFSharp = false, Configurations = debugAndRelease, },
				new { Label = TestLabel.Linker, ProjectPath = Path.Combine ("linker", "ios", "trimmode link"), IsFSharp = false, Configurations = debugAndRelease, },
				new { Label = TestLabel.Fsharp, ProjectPath = "fsharp", IsFSharp = true, Configurations = noConfigurations, },
				new { Label = TestLabel.Framework, ProjectPath = "framework-test", IsFSharp = false, Configurations = noConfigurations, },
				new { Label = TestLabel.InterdependentBindingProjects, ProjectPath = "interdependent-binding-projects", IsFSharp = false, Configurations = noConfigurations, },
				new { Label = TestLabel.Xcframework, ProjectPath = "xcframework-test", IsFSharp = false, Configurations = noConfigurations, },
			};

			// If .NET is not enabled, then ignore, otherwise leave undecided for other code to determine.
			bool? dotnetIgnored = ENABLE_DOTNET ? null : (bool?) true;
			foreach (var projectInfo in projects) {
				var projectPath = projectInfo.ProjectPath;
				var projectName = Path.GetFileName (projectPath);
				var projExtension = projectInfo.IsFSharp ? ".fsproj" : ".csproj";

				IOSTestProjects.Add (new iOSTestProject (projectInfo.Label, Path.GetFullPath (Path.Combine (RootDirectory, projectPath, "dotnet", "iOS", projectName + projExtension))) {
					Name = projectName,
					IsDotNetProject = true,
					SkipiOSVariation = false,
					SkiptvOSVariation = true,
					SkipwatchOSVariation = true,
					SkipTodayExtensionVariation = true,
					SkipDeviceVariations = false,
					TestPlatform = TestPlatform.iOS_Unified,
					Ignore = dotnetIgnored,
					Configurations = projectInfo.Configurations,
				});

				IOSTestProjects.Add (new iOSTestProject (projectInfo.Label, Path.GetFullPath (Path.Combine (RootDirectory, projectPath, "dotnet", "tvOS", projectName + projExtension))) {
					Name = projectName,
					IsDotNetProject = true,
					SkipiOSVariation = true,
					SkiptvOSVariation = true,
					SkipwatchOSVariation = true,
					SkipTodayExtensionVariation = true,
					SkipDeviceVariations = false,
					GenerateVariations = false,
					TestPlatform = TestPlatform.tvOS,
					Ignore = dotnetIgnored,
					Configurations = projectInfo.Configurations,
				});

				MacTestProjects.Add (new MacTestProject (projectInfo.Label, Path.GetFullPath (Path.Combine (RootDirectory, projectPath, "dotnet", "macOS", projectName + projExtension))) {
					Name = projectName,
					IsDotNetProject = true,
					TargetFrameworkFlavors = MacFlavors.DotNet,
					Platform = "AnyCPU",
					Ignore = dotnetIgnored,
					TestPlatform = TestPlatform.Mac,
					Configurations = projectInfo.Configurations,
				});

				MacTestProjects.Add (new MacTestProject (projectInfo.Label, Path.GetFullPath (Path.Combine (RootDirectory, projectPath, "dotnet", "MacCatalyst", projectName + projExtension))) {
					Name = projectName,
					IsDotNetProject = true,
					TargetFrameworkFlavors = MacFlavors.MacCatalyst,
					Platform = "AnyCPU",
					Ignore = dotnetIgnored,
					TestPlatform = TestPlatform.MacCatalyst,
					Configurations = projectInfo.Configurations,
				});
			}
		}

		int AutoConfigureMac (bool generate_projects)
		{
			int rv = 0;

			var test_suites = new [] {
				new { Label = TestLabel.Linker, Directory = "linker/mac/dont link", ProjectFile = "dont link-mac", Name = "dont link", Flavors = MacFlavors.Modern | MacFlavors.Full | MacFlavors.System },
			};
			foreach (var p in test_suites) {
				MacTestProjects.Add (new MacTestProject (p.Label, Path.GetFullPath (Path.Combine (RootDirectory, p.Directory, p.ProjectFile + ".csproj"))) {
					Name = p.Name,
					TargetFrameworkFlavors = p.Flavors,
				});
			}

			MacTestProjects.Add (new MacTestProject (TestLabel.Introspection, Path.GetFullPath (Path.Combine (RootDirectory, "introspection", "Mac", "introspection-mac.csproj")), targetFrameworkFlavor: MacFlavors.Modern) { Name = "introspection" });
			MacTestProjects.Add (new MacTestProject (TestLabel.Framework, Path.GetFullPath (Path.Combine (RootDirectory, "framework-test", "macOS", "framework-test-mac.csproj")), targetFrameworkFlavor: MacFlavors.Modern) { Name = "framework-test" });
			MacTestProjects.Add (new MacTestProject (TestLabel.Xcframework, Path.GetFullPath (Path.Combine (RootDirectory, "xcframework-test", "macOS", "xcframework-test-mac.csproj")), targetFrameworkFlavor: MacFlavors.Modern) { Name = "xcframework-test" });

			var hard_coded_test_suites = new [] {
				new { Label = TestLabel.Mmp, Directory = "mmptest", ProjectFile = "mmptest", Name = "mmptest", IsNUnit = true, Configurations = (string[]) null, Platform = "x86", Flavors = MacFlavors.Console, },
				new { Label = TestLabel.Xammac, Directory = "xammac_tests", ProjectFile = "xammac_tests", Name = "xammac tests", IsNUnit = false, Configurations = new string [] { "Debug", "Release" }, Platform = "AnyCPU", Flavors = MacFlavors.Modern, },
				new { Label = TestLabel.Linker, Directory = "linker/mac/link all", ProjectFile = "link all-mac", Name = "link all", IsNUnit = false, Configurations = new string [] { "Debug", "Release" }, Platform = "x86", Flavors = MacFlavors.Modern, },
				new { Label = TestLabel.Linker, Directory = "linker/mac/link sdk", ProjectFile = "link sdk-mac", Name = "link sdk", IsNUnit = false, Configurations = new string [] { "Debug", "Release" }, Platform = "x86", Flavors = MacFlavors.Modern, },
			};
			foreach (var p in hard_coded_test_suites) {
				MacTestProjects.Add (new MacTestProject (p.Label, Path.GetFullPath (Path.Combine (RootDirectory, p.Directory, p.ProjectFile + ".csproj")), targetFrameworkFlavor: p.Flavors) {
					Name = p.Name,
					IsNUnitProject = p.IsNUnit,
					SolutionPath = Path.GetFullPath (Path.Combine (RootDirectory, "tests-mac.sln")),
					Configurations = p.Configurations,
					Platform = p.Platform,
					Ignore = !INCLUDE_XAMARIN_LEGACY,
				});
			}

			foreach (var flavor in new MonoNativeFlavor [] { MonoNativeFlavor.Compat, MonoNativeFlavor.Unified }) {
				var monoNativeInfo = new MonoNativeInfo (DevicePlatform.macOS, flavor, RootDirectory, Log);
				var macTestProject = new MacTestProject (TestLabel.Mononative, monoNativeInfo.ProjectPath, targetFrameworkFlavor: MacFlavors.Modern | MacFlavors.Full) {
					MonoNativeInfo = monoNativeInfo,
					Name = monoNativeInfo.ProjectName,
					Platform = "AnyCPU",
					Ignore = !INCLUDE_XAMARIN_LEGACY,

				};

				MacTestProjects.Add (macTestProject);
			}

			var monoImportTestFactory = new BCLTestImportTargetFactory (this);
			MacTestProjects.AddRange (monoImportTestFactory.GetMacBclTargets ());

			// Generate test projects from templates (bcl/mono-native templates)
			if (generate_projects) {
				foreach (var mtp in MacTestProjects.Where (x => x.MonoNativeInfo is not null).Select (x => x.MonoNativeInfo))
					mtp.Convert ();
			}

			// All test projects should be either Modern projects or NUnit/console executables at this point.
			// If we need to generate Full/System variations, we do that here.
			var unified_targets = new List<MacTarget> ();

			Action<MacTarget, string, bool, bool> configureTarget = (MacTarget target, string file, bool isNUnitProject, bool skip_generation) => {
				target.TemplateProjectPath = file;
				target.Harness = this;
				target.IsNUnitProject = isNUnitProject;
				if (!generate_projects || skip_generation)
					target.ShouldSkipProjectGeneration = true;
				target.Execute ();
			};

			foreach (var proj in MacTestProjects) {
				var target = new MacTarget (MacFlavors.Modern);
				configureTarget (target, proj.Path, proj.IsNUnitProject, true);
				unified_targets.Add (target);
			}

			foreach (var proj in MacTestProjects.Where ((v) => v.GenerateVariations).ToArray ()) {
				var file = proj.Path;
				if (!File.Exists (file)) {
					Console.WriteLine ($"Can't find the project file {file}.");
					rv = 1;
					continue;
				}

				// Generate variations if requested
				if (proj.GenerateFull) {
					var target = new MacTarget (MacFlavors.Full);
					configureTarget (target, file, proj.IsNUnitProject, false);
					unified_targets.Add (target);

					var cloned_project = (MacTestProject) proj.Clone ();
					cloned_project.TargetFrameworkFlavors = MacFlavors.Full;
					cloned_project.Path = target.ProjectPath;
					MacTestProjects.Add (cloned_project);
				}

				if (proj.GenerateSystem) {
					var target = new MacTarget (MacFlavors.System);
					configureTarget (target, file, proj.IsNUnitProject, false);
					unified_targets.Add (target);

					var cloned_project = (MacTestProject) proj.Clone ();
					cloned_project.TargetFrameworkFlavors = MacFlavors.System;
					cloned_project.Path = target.ProjectPath;
					MacTestProjects.Add (cloned_project);
				}

				// We're done generating now
				// Re-use the existing TestProject instance instead of creating a new one.
				proj.TargetFrameworkFlavors = MacFlavors.Modern; // the default/template flavor is 'Modern'
			}

			return rv;
		}

		void AutoConfigureIOS ()
		{
			var library_projects = new string [] { "BundledResources", "EmbeddedResources", "bindings-test2" };
			var fsharp_test_suites = new string [] { "fsharp" };
			var fsharp_library_projects = new string [] { "fsharplibrary" };

			IOSTestProjects.Add (new iOSTestProject (TestLabel.Monotouch, Path.GetFullPath (Path.Combine (RootDirectory, "monotouch-test", "monotouch-test.csproj"))) {
				Name = "monotouch-test",
			});

			foreach (var p in fsharp_test_suites)
				IOSTestProjects.Add (new iOSTestProject (TestLabel.Fsharp, Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".fsproj"))) { Name = p });
			foreach (var p in library_projects)
				IOSTestProjects.Add (new iOSTestProject (TestLabel.LibraryProjects, Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj")), false) { Name = p });
			foreach (var p in fsharp_library_projects)
				IOSTestProjects.Add (new iOSTestProject (TestLabel.Fsharp, Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".fsproj")), false) { Name = p });

			IOSTestProjects.Add (new iOSTestProject (TestLabel.BindingFramework, Path.GetFullPath (Path.Combine (RootDirectory, "bindings-framework-test", "iOS", "bindings-framework-test.csproj")), false) {
				Name = "bindings-framework-test",
			});
			IOSTestProjects.Add (new iOSTestProject (TestLabel.BindingsXcframework, Path.GetFullPath (Path.Combine (RootDirectory, "bindings-xcframework-test", "iOS", "bindings-xcframework-test.csproj")), false) {
				Name = "bindings-xcframework-test",
			});
			IOSTestProjects.Add (new iOSTestProject (TestLabel.Framework, Path.GetFullPath (Path.Combine (RootDirectory, "framework-test", "iOS", "framework-test-ios.csproj"))) {
				Name = "framework-test",
			});
			IOSTestProjects.Add (new iOSTestProject (TestLabel.Xcframework, Path.GetFullPath (Path.Combine (RootDirectory, "xcframework-test", "iOS", "xcframework-test-ios.csproj"))) {
				Name = "xcframework-test",
			});

			IOSTestProjects.Add (new iOSTestProject (TestLabel.Binding, Path.GetFullPath (Path.Combine (RootDirectory, "bindings-test", "iOS", "bindings-test.csproj")), false) { Name = "bindings-test" });

			IOSTestProjects.Add (new iOSTestProject (TestLabel.InterdependentBindingProjects, Path.GetFullPath (Path.Combine (RootDirectory, "interdependent-binding-projects", "interdependent-binding-projects.csproj"))) { Name = "interdependent-binding-projects" });
			IOSTestProjects.Add (new iOSTestProject (TestLabel.Introspection, Path.GetFullPath (Path.Combine (RootDirectory, "introspection", "iOS", "introspection-ios.csproj"))) { Name = "introspection" });
			IOSTestProjects.Add (new iOSTestProject (TestLabel.Linker, Path.GetFullPath (Path.Combine (RootDirectory, "linker", "ios", "dont link", "dont link.csproj"))) {
				Configurations = new string [] { "Debug", "Release" },
			});
			IOSTestProjects.Add (new iOSTestProject (TestLabel.Linker, Path.GetFullPath (Path.Combine (RootDirectory, "linker", "ios", "link all", "link all.csproj"))) {
				Configurations = new string [] { "Debug", "Release" },
			});
			IOSTestProjects.Add (new iOSTestProject (TestLabel.Linker, Path.GetFullPath (Path.Combine (RootDirectory, "linker", "ios", "link sdk", "link sdk.csproj"))) {
				Configurations = new string [] { "Debug", "Release" },
			});

			foreach (var flavor in new MonoNativeFlavor [] { MonoNativeFlavor.Compat, MonoNativeFlavor.Unified }) {
				var monoNativeInfo = new MonoNativeInfo (DevicePlatform.iOS, flavor, RootDirectory, Log);
				var iosTestProject = new iOSTestProject (TestLabel.Mononative, monoNativeInfo.ProjectPath) {
					MonoNativeInfo = monoNativeInfo,
					Name = monoNativeInfo.ProjectName,
					SkipwatchOSARM64_32Variation = monoNativeInfo.ProjectName.Contains ("compat"),
				};

				IOSTestProjects.Add (iosTestProject);
			}

			// add all the tests that are using the precompiled mono assemblies
			var monoImportTestFactory = new BCLTestImportTargetFactory (this);
			IOSTestProjects.AddRange (monoImportTestFactory.GetiOSBclTargets ());

			WatchOSContainerTemplate = Path.GetFullPath (Path.Combine (RootDirectory, "templates/WatchContainer"));
			WatchOSAppTemplate = Path.GetFullPath (Path.Combine (RootDirectory, "templates/WatchApp"));
			WatchOSExtensionTemplate = Path.GetFullPath (Path.Combine (RootDirectory, "templates/WatchExtension"));

			TodayContainerTemplate = Path.GetFullPath (Path.Combine (RootDirectory, "templates", "TodayContainer"));
			TodayExtensionTemplate = Path.GetFullPath (Path.Combine (RootDirectory, "templates", "TodayExtension"));
			BCLTodayExtensionTemplate = Path.GetFullPath (Path.Combine (RootDirectory, "bcl-test", "templates", "today"));
		}

		// Dictionary<string, string> make_config = new Dictionary<string, string> ();
		IEnumerable<string> FindConfigFiles (string name)
		{
			var dir = Path.GetFullPath (RootDirectory);
			while (dir != "/") {
				var file = Path.Combine (dir, name);
				if (File.Exists (file))
					yield return file;
				dir = Path.GetDirectoryName (dir);
			}
		}

		Dictionary<string, string> ParseConfigFiles ()
		{
			var configuration = new Dictionary<string, string> ();
			foreach (var file in GetConfigFiles ()) {
				ParseConfigFile (file, configuration);
			}

			return configuration;
		}

		IEnumerable<string> GetConfigFiles ()
		{
			return FindConfigFiles (useSystemXamarinIOSMac ? "test-system.config" : "test.config")
				.Concat (FindConfigFiles ("configure.inc"))
				.Concat (FindConfigFiles ("Make.config"))
				.Concat (FindConfigFiles ("Make.config.local"));
		}

		void ParseConfigFile (string file, Dictionary<string, string> configuration)
		{
			if (string.IsNullOrEmpty (file))
				return;

			foreach (var line in File.ReadAllLines (file).Reverse ()) {
				var eq = line.IndexOf ('=');
				if (eq == -1)
					continue;

				var key = line.Substring (0, eq);
				if (!configuration.ContainsKey (key))
					configuration [key] = line.Substring (eq + 1);
			}
		}

		int Configure ()
		{
			var rv = AutoConfigureMac (true);
			if (rv != 0)
				return rv;
			return ConfigureIOS ();
		}

		// At startup we:
		// * Load a list of well-known test projects IOSTestProjects/MacTestProjects. This happens in AutoConfigureIOS/AutoConfigureMac.
		//   Example projects:
		//     * introspection
		//     * dont link, link all, link sdk
		// * Each of these test projects can used to generate other platform variations (tvOS, watchOS, macOS full, etc),
		//   if the the TestProject.GenerateVariations property is true.
		// * For the mono-native template project, we generate a compat+unified version of the mono-native template project (in MonoNativeInfo.Convert).
		//   GenerateVariations is true for mono-native projects, which means we'll generate platform variations.
		// * For the BCL tests, we use a BCL test project generator. The BCL test generator generates projects for
		//   all platforms we're interested in, so we set GenerateVariations to false to avoid generate the platform variations again.

		int ConfigureIOS ()
		{
			var rv = 0;
			var unified_targets = new List<UnifiedTarget> ();
			var tvos_targets = new List<TVOSTarget> ();
			var watchos_targets = new List<WatchOSTarget> ();
			var today_targets = new List<TodayExtensionTarget> ();

			if (autoConf)
				AutoConfigureIOS ();

			foreach (var monoNativeInfo in IOSTestProjects.Where (x => x.MonoNativeInfo is not null).Select (x => x.MonoNativeInfo))
				monoNativeInfo.Convert ();

			foreach (var proj in IOSTestProjects.Where ((v) => v.GenerateVariations)) {
				var file = proj.Path;

				if (!File.Exists (file)) {
					Console.WriteLine ($"Can't find the project file {file}.");
					rv = 1;
					continue;
				}

				if (!proj.SkipwatchOSVariation) {
					var watchos = new WatchOSTarget () {
						TemplateProjectPath = file,
						Harness = this,
						TestProject = proj,
					};
					watchos.Execute ();
					watchos_targets.Add (watchos);
				}

				if (!proj.SkiptvOSVariation) {
					var tvos = new TVOSTarget () {
						TemplateProjectPath = file,
						Harness = this,
						TestProject = proj,
					};
					tvos.Execute ();
					tvos_targets.Add (tvos);
				}

				if (!proj.SkipiOSVariation) {
					var unified = new UnifiedTarget () {
						TemplateProjectPath = file,
						Harness = this,
						TestProject = proj,
						ShouldSkipProjectGeneration = proj.IsDotNetProject,
					};
					unified.Execute ();
					unified_targets.Add (unified);

					if (!proj.SkipTodayExtensionVariation) {
						var today = new TodayExtensionTarget {
							TemplateProjectPath = file,
							Harness = this,
							TestProject = proj,
							ShouldSkipProjectGeneration = proj.IsDotNetProject,
						};
						today.Execute ();
						today_targets.Add (today);
					}
				}
			}

			SolutionGenerator.CreateSolution (this, watchos_targets, "watchos", DevicePlatform.watchOS);
			SolutionGenerator.CreateSolution (this, tvos_targets, "tvos", DevicePlatform.tvOS);
			SolutionGenerator.CreateSolution (this, today_targets, "today", DevicePlatform.iOS);

			return rv;
		}

		int Install ()
		{
			HarnessLog ??= GetAdHocLog ();

			foreach (var project in IOSTestProjects) {
				var runner = CreateAppRunner (project);
				using (var install_log = new AppInstallMonitorLog (runner.MainLog)) {
					var rv = runner.InstallAsync (install_log.CancellationToken).Result;
					if (!rv.Succeeded)
						return rv.ExitCode;
				}
			}
			return 0;
		}

		int Uninstall ()
		{
			HarnessLog ??= GetAdHocLog ();

			foreach (var project in IOSTestProjects) {
				var runner = CreateAppRunner (project);
				var rv = runner.UninstallAsync ().Result;
				if (!rv.Succeeded)
					return rv.ExitCode;
			}
			return 0;
		}

		int Run ()
		{
			HarnessLog ??= GetAdHocLog ();

			foreach (var project in IOSTestProjects) {
				var runner = CreateAppRunner (project);
				var rv = runner.RunAsync ().Result;
				if (rv != 0)
					return rv;
			}
			return 0;
		}

		void Log (int min_level, string message)
		{
			if (Verbosity < min_level)
				return;
			Console.WriteLine (message);
			HarnessLog?.WriteLine (message);
		}

		public void Log (int min_level, string message, params object [] args)
		{
			if (Verbosity < min_level)
				return;
			Console.WriteLine (message, args);
			HarnessLog?.WriteLine (message, args);
		}

		public void Log (string message)
		{
			Log (0, message);
		}

		public void Log (string message, params object [] args)
		{
			Log (0, message, args);
		}

		public bool InCI {
			get {
				// We use the 'BUILD_REVISION' variable to detect whether we're running CI or not.
				return !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("BUILD_REVISION"));
			}
		}

		public bool UseTcpTunnel {
			get {
				// We use the 'USE_TCP_TUNNEL' variable to detect whether we're running CI or not.
				return !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("USE_TCP_TUNNEL"));
			}
		}

		public bool UseGroupedApps {
			get {
				var groupApps = Environment.GetEnvironmentVariable ("BCL_GROUPED_APPS");
				return string.IsNullOrEmpty (groupApps) || groupApps == "grouped";
			}
		}

		public string VSDropsUri {
			get {
				var uri = Environment.GetEnvironmentVariable ("VSDROPS_URI");
				return string.IsNullOrEmpty (uri) ? null : uri;
			}
		}

		public int Execute ()
		{
			switch (Action) {
			case HarnessAction.Configure:
				return Configure ();
			case HarnessAction.Run:
				return Run ();
			case HarnessAction.Install:
				return Install ();
			case HarnessAction.Uninstall:
				return Uninstall ();
			case HarnessAction.Jenkins:
				return Jenkins ();
			default:
				throw new NotImplementedException (Action.ToString ());
			}
		}

		int Jenkins ()
		{
			if (autoConf) {
				AutoConfigureDotNet ();
				AutoConfigureIOS ();
				AutoConfigureMac (false);
			}

			var jenkins = new Jenkins.Jenkins (this, processManager, ResultParser, TunnelBore);
			return jenkins.Run ();
		}

		public void Save (StringWriter doc, string path)
		{
			if (!File.Exists (path)) {
				Directory.CreateDirectory (Path.GetDirectoryName (path));
				File.WriteAllText (path, doc.ToString ());
				Log (1, "Created {0}", path);
			} else {
				var existing = File.ReadAllText (path);
				var updated = doc.ToString ();

				if (existing == updated) {
					Log (1, "Not saved {0}, no change", path);
				} else {
					File.WriteAllText (path, updated);
					Log (1, "Updated {0}", path);
				}
			}
		}

		bool? disable_watchos_on_wrench;

		public bool DisableWatchOSOnWrench {
			get {
				if (!disable_watchos_on_wrench.HasValue)
					disable_watchos_on_wrench = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("DISABLE_WATCH_ON_WRENCH"));
				return disable_watchos_on_wrench.Value;
			}
		}

		AppRunner CreateAppRunner (TestProject project)
		{
			var rv = new AppRunner (processManager,
				new AppBundleInformationParser (processManager, AppBundleLocator),
				new SimulatorLoaderFactory (processManager),
				new SimpleListenerFactory (UseTcpTunnel ? TunnelBore : null),
				new DeviceLoaderFactory (processManager),
				new CrashSnapshotReporterFactory (processManager),
				new CaptureLogFactory (),
				new DeviceLogCapturerFactory (processManager),
				new TestReporterFactory (processManager),
				target,
				this,
				HarnessLog,
				new Logs (LogDirectory),
				project.Path,
				buildConfiguration);
			rv.InitializeAsync ().Wait ();
			return rv;
		}

		public string GetDotNetExecutable (string directory) => AppBundleLocator.GetDotNetExecutable (directory);

		private static IFileBackedLog GetAdHocLog () => Microsoft.DotNet.XHarness.Common.Logging.Log.CreateReadableAggregatedLog (
			new LogFile ("HarnessLog", Path.GetTempFileName ()), new ConsoleLog ());

		// Return true if the current machine can run ARM64 binaries.
		static bool? canRunArm64;
		public static bool CanRunArm64 {
			get {
				if (!canRunArm64.HasValue) {
					int rv = 0;
					IntPtr size = (IntPtr) sizeof (int);
					if (sysctlbyname ("hw.optional.arm64", ref rv, ref size, IntPtr.Zero, IntPtr.Zero) == 0) {
						canRunArm64 = rv == 1;
					} else {
						canRunArm64 = false;
					}
				}
				return canRunArm64.Value;
			}
		}

		[DllImport ("libc")]
		static extern int sysctlbyname (string name, ref int value, ref IntPtr size, IntPtr zero, IntPtr zeroAgain);
	}
}
