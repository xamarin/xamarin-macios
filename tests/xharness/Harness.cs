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

using Xamarin.Utils;

namespace Xharness {
	public enum HarnessAction {
		None,
		Run,
		Install,
		Uninstall,
		Jenkins,
	}

	public class HarnessConfiguration {
		public string BuildConfiguration { get; set; } = "Debug";
		public bool DryRun { get; set; }
		public Dictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string> ();
		public bool? IncludeSystemPermissionTests { get; set; }
		public List<TestProject> TestProjects { get; set; } = new List<TestProject> ();
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

		string sdkRoot;
		string SdkRoot {
			get => sdkRoot;
			set {
				sdkRoot = value;
				XcodeRoot = FindXcode (sdkRoot);
			}
		}

		bool TryGetMlaunchDotnetPath (out string value)
		{
			value = null;

			ApplePlatform platform;
			if (INCLUDE_IOS) {
				platform = ApplePlatform.iOS;
			} else if (INCLUDE_TVOS) {
				platform = ApplePlatform.TVOS;
			} else {
				return false;
			}

			var sdkPlatform = platform.AsString ().ToUpperInvariant ();
			var sdkName = GetVariable ($"{sdkPlatform}_NUGET_SDK_NAME");
			// there is a diff between getting the path for the current platform when running on CI or off CI. The config files in the CI do not 
			// contain the correct workload version, the reason for this is that the workload is built in a different machine which means that
			// the Make.config will use the wrong version. The CI set the version in the environment variable {platform}_WORKLOAD_VERSION via a script.
			var workloadVersion = GetVariable ($"{sdkPlatform}_WORKLOAD_VERSION");
			var sdkVersion = GetVariable ($"{sdkPlatform}_NUGET_VERSION_NO_METADATA");
			value = Path.Combine (DOTNET_DIR, "packs", sdkName, string.IsNullOrEmpty (workloadVersion) ? sdkVersion : workloadVersion, "tools", "bin", "mlaunch");
			return true;
		}

		string MlaunchPath {
			get {
				if (TryGetMlaunchDotnetPath (out var mlaunch))
					return mlaunch;

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

		public List<TestProject> TestProjects { get; } = new ();

		readonly bool useSystemXamarinIOSMac; // if the system XI/XM should be used, or the locally build XI/XM.

		public bool INCLUDE_IOS { get; }
		public bool INCLUDE_TVOS { get; }
		public bool INCLUDE_MAC { get; }
		public bool INCLUDE_MACCATALYST { get; }
		public string JENKINS_RESULTS_DIRECTORY { get; } // Use same name as in Makefiles, so that a grep finds it.
		public string DOTNET_DIR { get; set; }
		public string DOTNET_TFM { get; set; }

		public Version DotNetVersion {
			get => Version.Parse (DOTNET_TFM.Replace ("net", ""));
		}

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

			buildConfiguration = configuration.BuildConfiguration ?? throw new ArgumentNullException (nameof (configuration));
			DryRun = configuration.DryRun;
			IncludeSystemPermissionTests = configuration.IncludeSystemPermissionTests;
			TestProjects = configuration.TestProjects;
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
			XmlJargon = configuration.XmlJargon;

			if (configuration.Labels is not null)
				Labels = new HashSet<string> (configuration.Labels);

			if (configuration.EnvironmentVariables is not null)
				EnvironmentVariables = new Dictionary<string, string> (configuration.EnvironmentVariables);

			LaunchTimeout = InCI ? 3 : 120;

			config = ParseConfigFiles ();
			var src_root = Path.GetDirectoryName (Path.GetFullPath (RootDirectory));

			INCLUDE_IOS = IsVariableSet (nameof (INCLUDE_IOS));
			INCLUDE_TVOS = IsVariableSet (nameof (INCLUDE_TVOS));
			JENKINS_RESULTS_DIRECTORY = GetVariable (nameof (JENKINS_RESULTS_DIRECTORY));
			INCLUDE_MAC = IsVariableSet (nameof (INCLUDE_MAC));
			INCLUDE_MACCATALYST = IsVariableSet (nameof (INCLUDE_MACCATALYST));
			DOTNET_DIR = GetVariable (nameof (DOTNET_DIR));
			DOTNET_TFM = GetVariable (nameof (DOTNET_TFM));

			if (string.IsNullOrEmpty (SdkRoot))
				SdkRoot = GetVariable ("XCODE_DEVELOPER_ROOT", configuration.SdkRoot);

			processManager = new MlaunchProcessManager (XcodeRoot, MlaunchPath);
			AppBundleLocator = new AppBundleLocator (processManager, () => HarnessLog, "/usr/local/share/dotnet/dotnet", GetVariable ("DOTNET"));
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

		void PopulateTests ()
		{
			PopulatePlatformSpecificProjects ();
			PopulateUnitTestProjects ();
		}

		void PopulateUnitTestProjects ()
		{
			var projectInfos = new [] {
				new {
					Label = TestLabel.Msbuild,
					ProjectPath = Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory, "msbuild", "Xamarin.MacDev.Tasks.Tests", "Xamarin.MacDev.Tasks.Tests.csproj")),
					Name = "MSBuild tasks tests",
					Timeout = (TimeSpan?) TimeSpan.FromMinutes (60),
					Filter = "",
				},
				new {
					Label = TestLabel.Msbuild,
					ProjectPath = Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory, "msbuild", "Xamarin.MacDev.Tests", "Xamarin.MacDev.Tests.csproj")),
					Name = "MSBuild integration tests",
					Timeout = (TimeSpan?) TimeSpan.FromMinutes (120),
					Filter = "",
				},
				new {
					Label = TestLabel.Cecil,
					ProjectPath = Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory, "cecil-tests", "cecil-tests.csproj")),
					Name = "Cecil-based tests",
					Timeout = (TimeSpan?) TimeSpan.FromMinutes (10),
					Filter = "",
				},
				new {
					Label = TestLabel.Generator,
					ProjectPath = Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory, "bgen", "bgen-tests.csproj")),
					Name = "BGen tests",
					Timeout = (TimeSpan?) null,
					Filter = "",
				},
				new {
					Label = TestLabel.Generator,
					ProjectPath = Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory,  "rgen", "Microsoft.Macios.Generator.Tests", "Microsoft.Macios.Generator.Tests.csproj")),
					Name = "Roslyn Generator tests",
					Timeout = (TimeSpan?) null,
					Filter = "",
				},
				new {
					Label = TestLabel.Generator,
					ProjectPath = Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory,  "rgen", "Microsoft.Macios.Bindings.Analyzer.Tests", "Microsoft.Macios.Bindings.Analyzer.Tests.csproj")),
					Name = "Roslyn Analyzer tests",
					Timeout = (TimeSpan?) null,
					Filter = "",
				},
				new {
					Label = TestLabel.Generator,
					ProjectPath = Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory,  "rgen",  "Microsoft.Macios.Transformer.Tests", "Microsoft.Macios.Transformer.Tests.csproj")),
					Name = "Roslyn Transformer tests",
					Timeout = (TimeSpan?) null,
					Filter = "",
				},
				new {
					Label = TestLabel.Generator,
					ProjectPath = Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory,  "rgen",  "Microsoft.Macios.Bindings.CodeFixers.Tests", "Microsoft.Macios.Bindings.CodeFixers.Tests.csproj")),
					Name = "Roslyn Codefixers tests",
					Timeout = (TimeSpan?) null,
					Filter = "",
				},
				new {
					Label = TestLabel.DotnetTest,
					ProjectPath = Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory, "dotnet", "UnitTests", "DotNetUnitTests.csproj")),
					Name = "DotNet tests",
					Timeout = (TimeSpan?) TimeSpan.FromMinutes (165 /* 2h45m: to time out here before the CI job does at 3h */),
					Filter = "Category!=Windows",
				},
				new {
					Label = TestLabel.Xtro,
					ProjectPath = Path.GetFullPath (Path.Combine (HarnessConfiguration.RootDirectory, "xtro-sharpie", "UnitTests", "UnitTests.csproj")),
					Name = "Xtro",
					Timeout = (TimeSpan?) TimeSpan.FromMinutes (15),
					Filter = "",
				},
			};

			foreach (var projectInfo in projectInfos) {
				var project = new TestProject (projectInfo.Label, projectInfo.ProjectPath) {
					TestPlatform = TestPlatform.All,
					IsExecutableProject = false,
					Name = projectInfo.Name,
				};
				if (projectInfo.Timeout is not null)
					project.Timeout = projectInfo.Timeout.Value;
				TestProjects.Add (project);
			}
		}

		void PopulatePlatformSpecificProjects ()
		{
			string [] noConfigurations = null;
			var debugAndRelease = new string [] { "Debug", "Release" };

			var projectInfos = new [] {
				new {
					Label = TestLabel.Introspection,
					Platforms = TestPlatform.All,
					ProjectPath = "introspection",
					IsFSharp = false,
					Configurations = noConfigurations,
				},
				new {
					Label = TestLabel.Monotouch,
					Platforms = TestPlatform.All,
					ProjectPath = "monotouch-test",
					IsFSharp = false,
					Configurations = noConfigurations,
				},
				new {
					Label = TestLabel.Linker,
					Platforms = TestPlatform.All,
					ProjectPath = Path.Combine ("linker", "ios", "dont link"),
					IsFSharp = false,
					Configurations = debugAndRelease,
				},
				new {
					Label = TestLabel.Linker,
					Platforms = TestPlatform.All,
					ProjectPath = Path.Combine ("linker", "ios", "link sdk"),
					IsFSharp = false,
					Configurations = debugAndRelease,
				},
				new {
					Label = TestLabel.Linker,
					Platforms = TestPlatform.All,
					ProjectPath = Path.Combine ("linker", "ios", "link all"),
					IsFSharp = false,
					Configurations = debugAndRelease,
				},
				new {
					Label = TestLabel.Linker,
					Platforms = TestPlatform.All,
					ProjectPath = Path.Combine ("linker", "ios", "trimmode copy"),
					IsFSharp = false,
					Configurations = debugAndRelease,
				},
				new {
					Label = TestLabel.Linker,
					Platforms = TestPlatform.All,
					ProjectPath = Path.Combine ("linker", "ios", "trimmode link"),
					IsFSharp = false,
					Configurations = debugAndRelease,
				},
				new {
					Label = TestLabel.Fsharp,
					Platforms = TestPlatform.All,
					ProjectPath = "fsharp",
					IsFSharp = true,
					Configurations = noConfigurations,
				},
				new {
					Label = TestLabel.Framework,
					Platforms = TestPlatform.Mac | TestPlatform.MacCatalyst,
					ProjectPath = "framework-test",
					IsFSharp = false,
					Configurations = noConfigurations,
				},
				new {
					Label = TestLabel.InterdependentBindingProjects,
					Platforms = TestPlatform.All,
					ProjectPath = "interdependent-binding-projects",
					IsFSharp = false,
					Configurations = noConfigurations,
				},
				new {
					Label = TestLabel.Xcframework,
					Platforms = TestPlatform.All,
					ProjectPath = "xcframework-test",
					IsFSharp = false,
					Configurations = noConfigurations,
				},
			};

			foreach (var projectInfo in projectInfos) {
				var projectPath = projectInfo.ProjectPath;
				var projectName = Path.GetFileName (projectPath);
				var projExtension = projectInfo.IsFSharp ? ".fsproj" : ".csproj";
				var platforms = projectInfo.Platforms;

				if (platforms.HasFlag (TestPlatform.iOS)) {
					TestProjects.Add (new TestProject (projectInfo.Label, Path.GetFullPath (Path.Combine (RootDirectory, projectPath, "dotnet", "iOS", projectName + projExtension))) {
						Name = projectName,
						TestPlatform = TestPlatform.iOS,
						Configurations = projectInfo.Configurations,
					});
				}

				if (platforms.HasFlag (TestPlatform.tvOS)) {
					TestProjects.Add (new TestProject (projectInfo.Label, Path.GetFullPath (Path.Combine (RootDirectory, projectPath, "dotnet", "tvOS", projectName + projExtension))) {
						Name = projectName,
						TestPlatform = TestPlatform.tvOS,
						Configurations = projectInfo.Configurations,
					});
				}

				if (platforms.HasFlag (TestPlatform.Mac)) {
					TestProjects.Add (new TestProject (projectInfo.Label, Path.GetFullPath (Path.Combine (RootDirectory, projectPath, "dotnet", "macOS", projectName + projExtension))) {
						Name = projectName,
						TestPlatform = TestPlatform.Mac,
						Configurations = projectInfo.Configurations,
					});
				}

				if (platforms.HasFlag (TestPlatform.MacCatalyst)) {
					TestProjects.Add (new TestProject (projectInfo.Label, Path.GetFullPath (Path.Combine (RootDirectory, projectPath, "dotnet", "MacCatalyst", projectName + projExtension))) {
						Name = projectName,
						TestPlatform = TestPlatform.MacCatalyst,
						Configurations = projectInfo.Configurations,
					});
				}
			}
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

		int Install ()
		{
			HarnessLog ??= GetAdHocLog ();

			foreach (var project in TestProjects) {
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

			foreach (var project in TestProjects) {
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

			foreach (var project in TestProjects) {
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

		public string VSDropsUri {
			get {
				var uri = Environment.GetEnvironmentVariable ("VSDROPS_URI");
				return string.IsNullOrEmpty (uri) ? null : uri;
			}
		}

		public int Execute ()
		{
			switch (Action) {
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
			PopulateTests ();

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
