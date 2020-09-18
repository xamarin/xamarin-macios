using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Xml;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Xharness.Targets;

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
		public bool Mac { get; set; }
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
				if (root_directory == null) {
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
					root_directory = path;
				}
				return root_directory;
			}
			set {
				root_directory = value;
				if (root_directory != null)
					root_directory = Path.GetFullPath (root_directory).TrimEnd ('/');
			}
		}
	}

	public class Harness : IHarness {
		readonly TestTarget target;
		readonly string buildConfiguration = "Debug";

		IProcessManager processManager;

		public HarnessAction Action { get; }
		public int Verbosity { get; }
		public ILog HarnessLog { get; set; }
		public HashSet<string> Labels { get; }
		public XmlResultJargon XmlJargon { get; }
		public IResultParser ResultParser { get; }
		public ITunnelBore TunnelBore { get; }
		
		public string XIBuildPath => Path.GetFullPath (Path.Combine (RootDirectory, "..", "tools", "xibuild", "xibuild"));

		string sdkRoot;
		string SdkRoot {
			get => sdkRoot;
			set {
				sdkRoot = value;
				XcodeRoot = FindXcode (sdkRoot);
			}
		}

		string MlaunchPath => Path.Combine (IOS_DESTDIR, "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current", "bin", "mlaunch");

		public List<iOSTestProject> IOSTestProjects { get; }
		public List<MacTestProject> MacTestProjects { get; } = new List<MacTestProject> ();

		// Configure
		readonly bool useSystemXamarinIOSMac; // if the system XI/XM should be used, or the locally build XI/XM.
		readonly bool autoConf;
		readonly bool mac;

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
		public string JENKINS_RESULTS_DIRECTORY { get; } // Use same name as in Makefiles, so that a grep finds it.
		public string MAC_DESTDIR { get; }
		public string IOS_DESTDIR { get; }
		public string MONO_IOS_SDK_DESTDIR { get; }
		public string MONO_MAC_SDK_DESTDIR { get; }
		public bool ENABLE_XAMARIN { get; }
		public string DOTNET { get; }
		public string DOTNET6 { get; }

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
			mac = configuration.Mac;
			MarkdownSummaryPath = configuration.MarkdownSummaryPath;
			PeriodicCommand = configuration.PeriodicCommand;
			PeriodicCommandArguments = configuration.PeriodicCommandArguments;
			PeriodicCommandInterval = configuration.PeriodicCommandInterval;
			target = configuration.Target;
			Timeout = configuration.TimeoutInMinutes;
			useSystemXamarinIOSMac = configuration.UseSystemXamarinIOSMac;
			Verbosity = configuration.Verbosity;
			WatchOSAppTemplate = configuration.WatchOSAppTemplate;
			WatchOSContainerTemplate = configuration.WatchOSContainerTemplate;
			XmlJargon = configuration.XmlJargon;

			if (configuration.Labels != null)
				Labels = new HashSet<string> (configuration.Labels);

			if (configuration.EnvironmentVariables != null)
				EnvironmentVariables = new Dictionary<string, string> (configuration.EnvironmentVariables);

			LaunchTimeout = InCI ? 3 : 120;

			var config = ParseConfigFiles ();
			var src_root = Path.GetDirectoryName (Path.GetFullPath (RootDirectory));

			MONO_PATH = Path.GetFullPath (Path.Combine (src_root, "external", "mono"));
			TVOS_MONO_PATH = MONO_PATH;
			INCLUDE_IOS = config.ContainsKey ("INCLUDE_IOS") && !string.IsNullOrEmpty (config ["INCLUDE_IOS"]);
			INCLUDE_TVOS = config.ContainsKey ("INCLUDE_TVOS") && !string.IsNullOrEmpty (config ["INCLUDE_TVOS"]);
			JENKINS_RESULTS_DIRECTORY = config ["JENKINS_RESULTS_DIRECTORY"];
			INCLUDE_WATCH = config.ContainsKey ("INCLUDE_WATCH") && !string.IsNullOrEmpty (config ["INCLUDE_WATCH"]);
			INCLUDE_MAC = config.ContainsKey ("INCLUDE_MAC") && !string.IsNullOrEmpty (config ["INCLUDE_MAC"]);
			MAC_DESTDIR = config ["MAC_DESTDIR"];

			IOS_DESTDIR = config ["IOS_DESTDIR"];
			MONO_IOS_SDK_DESTDIR = config ["MONO_IOS_SDK_DESTDIR"];
			MONO_MAC_SDK_DESTDIR = config ["MONO_MAC_SDK_DESTDIR"];
			ENABLE_XAMARIN = config.ContainsKey ("ENABLE_XAMARIN") && !string.IsNullOrEmpty (config ["ENABLE_XAMARIN"]);
			DOTNET = config ["DOTNET"];
			DOTNET6 = config ["DOTNET6"];

			if (string.IsNullOrEmpty (SdkRoot))
				SdkRoot = config ["XCODE_DEVELOPER_ROOT"] ?? configuration.SdkRoot;

			processManager = new ProcessManager (XcodeRoot, MlaunchPath, GetDotNetExecutable, XIBuildPath);
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

			do {
				if (path == "/") {
					throw new Exception (string.Format ("Could not find Xcode.app in {0}", path));
				} else if (File.Exists (Path.Combine (path, "Contents", "MacOS", "Xcode"))) {
					return path;
				}

				path = Path.GetDirectoryName (path);
			} while (true);
		}

		int AutoConfigureMac (bool generate_projects)
		{
			int rv = 0;

			var test_suites = new [] {
				new { Directory = "apitest", ProjectFile = "apitest", Name = "apitest", Flavors = MacFlavors.Full | MacFlavors.Modern },
				new { Directory = "linker/mac/dont link", ProjectFile = "dont link-mac", Name = "dont link", Flavors = MacFlavors.Modern | MacFlavors.Full | MacFlavors.System },
			};
			foreach (var p in test_suites) {
				MacTestProjects.Add (new MacTestProject (Path.GetFullPath (Path.Combine (RootDirectory, p.Directory, p.ProjectFile + ".csproj"))) {
					Name = p.Name,
					TargetFrameworkFlavors = p.Flavors,
				});
			}

			MacTestProjects.Add (new MacTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "introspection", "Mac", "introspection-mac.csproj")), targetFrameworkFlavor: MacFlavors.Modern) { Name = "introspection" });

			var hard_coded_test_suites = new [] {
				new { Directory = "mmptest", ProjectFile = "mmptest", Name = "mmptest", IsNUnit = true, Configurations = (string[]) null, Platform = "x86", Flavors = MacFlavors.Console, },
				new { Directory = "msbuild-mac", ProjectFile = "msbuild-mac", Name = "MSBuild tests", IsNUnit = true, Configurations = (string[]) null, Platform = "x86", Flavors = MacFlavors.Console, },
				new { Directory = "xammac_tests", ProjectFile = "xammac_tests", Name = "xammac tests", IsNUnit = false, Configurations = new string [] { "Debug", "Release" }, Platform = "AnyCPU", Flavors = MacFlavors.Modern, },
				new { Directory = "linker/mac/link all", ProjectFile = "link all-mac", Name = "link all", IsNUnit = false, Configurations = new string [] { "Debug", "Release" }, Platform = "x86", Flavors = MacFlavors.Modern, },
				new { Directory = "linker/mac/link sdk", ProjectFile = "link sdk-mac", Name = "link sdk", IsNUnit = false, Configurations = new string [] { "Debug", "Release" }, Platform = "x86", Flavors = MacFlavors.Modern, },
			};
			foreach (var p in hard_coded_test_suites) {
				MacTestProjects.Add (new MacTestProject (Path.GetFullPath (Path.Combine (RootDirectory, p.Directory, p.ProjectFile + ".csproj")), targetFrameworkFlavor: p.Flavors) {
					Name = p.Name,
					IsNUnitProject = p.IsNUnit,
					SolutionPath = Path.GetFullPath (Path.Combine (RootDirectory, "tests-mac.sln")),
					Configurations = p.Configurations,
					Platform = p.Platform,
				});
			}

			foreach (var flavor in new MonoNativeFlavor [] { MonoNativeFlavor.Compat, MonoNativeFlavor.Unified }) {
				var monoNativeInfo = new MonoNativeInfo (DevicePlatform.macOS, flavor, RootDirectory, Log);
				var macTestProject = new MacTestProject (monoNativeInfo.ProjectPath, targetFrameworkFlavor: MacFlavors.Modern | MacFlavors.Full) {
					MonoNativeInfo = monoNativeInfo,
					Name = monoNativeInfo.ProjectName,
					Platform = "AnyCPU",

				};

				MacTestProjects.Add (macTestProject);
			}

			var monoImportTestFactory = new BCLTestImportTargetFactory (this);
			MacTestProjects.AddRange (monoImportTestFactory.GetMacBclTargets ());

			// Generate test projects from templates (bcl/mono-native templates)
			if (generate_projects) {
				foreach (var mtp in MacTestProjects.Where (x => x.MonoNativeInfo != null).Select (x => x.MonoNativeInfo))
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

					var cloned_project = (MacTestProject)proj.Clone ();
					cloned_project.TargetFrameworkFlavors = MacFlavors.Full;
					cloned_project.Path = target.ProjectPath;
					MacTestProjects.Add (cloned_project);
				}

				if (proj.GenerateSystem) {
					var target = new MacTarget (MacFlavors.System);
					configureTarget (target, file, proj.IsNUnitProject, false);
					unified_targets.Add (target);

					var cloned_project = (MacTestProject)proj.Clone ();
					cloned_project.TargetFrameworkFlavors = MacFlavors.System;
					cloned_project.Path = target.ProjectPath;
					MacTestProjects.Add (cloned_project);
				}

				// We're done generating now
				// Re-use the existing TestProject instance instead of creating a new one.
				proj.TargetFrameworkFlavors = MacFlavors.Modern; // the default/template flavor is 'Modern'
			}

			if (generate_projects)
				MakefileGenerator.CreateMacMakefile (this, unified_targets);

			return rv;
		}

		void AutoConfigureIOS ()
		{
			var test_suites = new string [] { "monotouch-test", "framework-test" };
			var library_projects = new string [] { "BundledResources", "EmbeddedResources", "bindings-test2", "bindings-framework-test" };
			var fsharp_test_suites = new string [] { "fsharp" };
			var fsharp_library_projects = new string [] { "fsharplibrary" };

			foreach (var p in test_suites)
				IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj"))) { Name = p });
			foreach (var p in fsharp_test_suites)
				IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".fsproj"))) { Name = p });
			foreach (var p in library_projects)
				IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj")), false) { Name = p });
			foreach (var p in fsharp_library_projects)
				IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".fsproj")), false) { Name = p });

			IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "bindings-test", "iOS", "bindings-test.csproj")), false) { Name = "bindings-test" });

			IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "interdependent-binding-projects", "interdependent-binding-projects.csproj"))) { Name = "interdependent-binding-projects" });
			IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "interdependent-binding-projects", "dotnet", "iOS", "interdependent-binding-projects.csproj"))) { Name = "interdependent-binding-projects", IsDotNetProject = true, SkipiOSVariation = false, SkiptvOSVariation = true, SkipwatchOSVariation = true, SkipTodayExtensionVariation = true, SkipDeviceVariations = false, SkipiOS32Variation = true, });
			IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "introspection", "iOS", "introspection-ios.csproj"))) { Name = "introspection" });
			IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "introspection", "iOS", "introspection-ios-dotnet.csproj"))) { Name = "introspection", IsDotNetProject = true, SkipiOSVariation = false, SkiptvOSVariation = true, SkipwatchOSVariation = true, SkipTodayExtensionVariation = true, SkipDeviceVariations = false, SkipiOS32Variation = true, });
			IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "monotouch-test", "dotnet", "iOS", "monotouch-test.csproj"))) { Name = "monotouch-test", IsDotNetProject = true, SkipiOSVariation = false, SkiptvOSVariation = true, SkipwatchOSVariation = true, SkipTodayExtensionVariation = true, SkipDeviceVariations = false, SkipiOS32Variation = true, });
			IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "linker", "ios", "dont link", "dont link.csproj"))) { Configurations = new string [] { "Debug", "Release" } });
			IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "linker", "ios", "dont link", "dotnet", "iOS", "dont link.csproj"))) { Configurations = new string [] { "Debug", "Release" }, IsDotNetProject = true, SkipiOSVariation = false, SkiptvOSVariation = true, SkipwatchOSVariation = true, SkipTodayExtensionVariation = true, SkipDeviceVariations = false, SkipiOS32Variation = true });
			IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "linker", "ios", "link all", "link all.csproj"))) { Configurations = new string [] { "Debug", "Release" } });
			IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "linker", "ios", "link all", "dotnet", "iOS", "link all.csproj"))) { Configurations = new string [] { "Debug", "Release" }, IsDotNetProject = true, SkipiOSVariation = false, SkiptvOSVariation = true, SkipwatchOSVariation = true, SkipTodayExtensionVariation = true, SkipDeviceVariations = false, SkipiOS32Variation = true });
			IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "linker", "ios", "link sdk", "link sdk.csproj"))) { Configurations = new string [] { "Debug", "Release" } });
			IOSTestProjects.Add (new iOSTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "linker", "ios", "link sdk", "dotnet", "iOS", "link sdk.csproj"))) { Configurations = new string [] { "Debug", "Release" }, IsDotNetProject = true, SkipiOSVariation = false, SkiptvOSVariation = true, SkipwatchOSVariation = true, SkipTodayExtensionVariation = true, SkipDeviceVariations = false, SkipiOS32Variation = true });

			foreach (var flavor in new MonoNativeFlavor [] { MonoNativeFlavor.Compat, MonoNativeFlavor.Unified }) {
				var monoNativeInfo = new MonoNativeInfo (DevicePlatform.iOS, flavor, RootDirectory, Log);
				var iosTestProject = new iOSTestProject (monoNativeInfo.ProjectPath) {
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
				.Concat (FindConfigFiles ("Make.config"))
				.Concat (FindConfigFiles ("Make.config.local"));
		}

		void ParseConfigFile (string file, Dictionary<string, string> configuration)
		{
			if (string.IsNullOrEmpty (file))
				return;

			foreach (var line in File.ReadAllLines (file)) {
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
			return mac ? AutoConfigureMac (true) : ConfigureIOS ();
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

			foreach (var monoNativeInfo in IOSTestProjects.Where (x => x.MonoNativeInfo != null).Select (x => x.MonoNativeInfo))
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
			MakefileGenerator.CreateMakefile (this, unified_targets, tvos_targets, watchos_targets, today_targets);

			return rv;
		}

		int Install ()
		{
			if (HarnessLog == null)
				HarnessLog = new ConsoleLog ();

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
			if (HarnessLog == null)
				HarnessLog = new ConsoleLog ();

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
			if (HarnessLog == null)
				HarnessLog = new ConsoleLog ();

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

		private AppRunner CreateAppRunner (TestProject project)
		{
			var rv = new AppRunner (processManager,
				new AppBundleInformationParser (),
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

		// Gets either the DOTNET or DOTNET6 variable, depending on any global.json
		// config file found in the specified directory or any containing directories.
		Dictionary<string, string> dotnet_executables = new Dictionary<string, string> ();
		public string GetDotNetExecutable (string directory)
		{
			if (directory == null)
				throw new ArgumentNullException (nameof (directory));

			lock (dotnet_executables) {
				if (dotnet_executables.TryGetValue (directory, out var value))
					return value;
			}

			// Find the first global.json up the directory hierarchy (stopping at the root directory)
			string global_json = null;
			var dir = directory;
			while (dir.Length > 2) {
				global_json = Path.Combine (dir, "global.json");
				if (File.Exists (global_json))
					break;
				dir = Path.GetDirectoryName (dir);
			}
			if (!File.Exists (global_json))
				throw new Exception ($"Could not find any global.json file in {directory} or above");

			// Parse the global.json we found, and figure out if it tells us to use .NET 3.1.100 or not.
			var contents = File.ReadAllBytes (global_json);
			using (var reader =  JsonReaderWriterFactory.CreateJsonReader (contents, new XmlDictionaryReaderQuotas ())) {
				var doc = new XmlDocument ();
				doc.Load (reader);
				var version = doc.SelectSingleNode ("/root/sdk").InnerText;
				string executable;
				switch (version [0]) {
				case '3':
					executable = DOTNET;
					break;
				default:
					executable = DOTNET6;
					break;
				}
				Log ($"Mapped .NET SDK version {version} to {executable} for {directory}");
				lock (dotnet_executables) {
					dotnet_executables [directory] = executable;
				}
				return executable;
			}
		}
	}
}
