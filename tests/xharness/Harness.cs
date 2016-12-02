using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace xharness
{
	public enum HarnessAction
	{
		None,
		Configure,
		Run,
		Install,
		Jenkins,
	}

	public class Harness
	{
		public HarnessAction Action { get; set; }
		public int Verbosity { get; set; }
		public Log HarnessLog { get; set; }

		// This is the maccore/tests directory.
		string root_directory;
		public string RootDirectory {
			get {
				if (root_directory == null)
					root_directory = Environment.CurrentDirectory;
				return root_directory;
			}
			set {
				root_directory = value;
			}
		}

		public List<TestProject> IOSTestProjects { get; set; } = new List<TestProject> ();
		public List<MacTestProject> MacTestProjects { get; set; } = new List<MacTestProject> ();
		public List<string> BclTests { get; set; } = new List<string> ();

		// Configure
		public bool AutoConf { get; set; }
		public bool Mac { get; set; }
		public string WatchOSContainerTemplate { get; set; }
		public string WatchOSAppTemplate { get; set; }
		public string WatchOSExtensionTemplate { get; set; }
		public string MONO_PATH { get; set; } // Use same name as in Makefiles, so that a grep finds it.
		public string WATCH_MONO_PATH { get; set; } // Use same name as in Makefiles, so that a grep finds it.
		public string TVOS_MONO_PATH { get; set; } // Use same name as in Makefiles, so that a grep finds it.
		public bool INCLUDE_WATCH { get; set; }
		public string JENKINS_RESULTS_DIRECTORY { get; set; } // Use same name as in Makefiles, so that a grep finds it.
		public string MAC_DESTDIR { get; set; }
		public string IOS_DESTDIR { get; set; }

		// Run
		public AppRunnerTarget Target { get; set; }
		public string SdkRoot { get; set; } = "/Applications/Xcode.app";
		public string Configuration { get; set; } = "Debug";
		public string LogFile { get; set; }
		public string LogDirectory { get; set; } = Environment.CurrentDirectory;
		public double Timeout { get; set; } = 10; // in minutes
		public double LaunchTimeout { get; set; } // in minutes
		public bool DryRun { get; set; } // Most things don't support this. If you need it somewhere, implement it!
		public string JenkinsConfiguration { get; set; }
		public Dictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string> ();

		public Harness ()
		{
			LaunchTimeout = InWrench ? 3 : 120;
		}

		public string XcodeRoot {
			get {
				var p = SdkRoot;
				do {
					if (p == "/") {
						throw new Exception (string.Format ("Could not find Xcode.app in {0}", SdkRoot));
					} else if (File.Exists (Path.Combine (p, "Contents", "MacOS", "Xcode"))) {
						return p;
					}
					p = Path.GetDirectoryName (p);
				} while (true);
			}
		}

		object mlaunch_lock = new object ();
		string DownloadMlaunch ()
		{
			// NOTE: the filename part in the url must be unique so that the caching logic works properly.
			var mlaunch_url = "http://bosstoragemirror.blob.core.windows.net/public-builder/mlaunch/mlaunch-63121a575eff6fa291c28d0f70bacff97b0ecc72.zip";
			var extraction_dir = Path.Combine (Path.GetTempPath (), Path.GetFileNameWithoutExtension (mlaunch_url));
			var mlaunch_path = Path.Combine (extraction_dir, "bin", "mlaunch");

			lock (mlaunch_lock) {
				if (File.Exists (mlaunch_path))
					return mlaunch_path;

				try {
					var local_zip = extraction_dir + ".zip";
					Log ("Downloading mlaunch to: {0}", local_zip);
					var wc = new System.Net.WebClient ();
					wc.DownloadFile (mlaunch_url, local_zip);
					Log ("Downloaded mlaunch.");

					var tmp_extraction_dir = extraction_dir + ".tmp";
					if (Directory.Exists (tmp_extraction_dir))
						Directory.Delete (tmp_extraction_dir, true);
					if (Directory.Exists (extraction_dir))
						Directory.Delete (extraction_dir, true);

					Log ("Extracting mlaunch...");
					using (var p = new Process ()) {
						p.StartInfo.FileName = "unzip";
						p.StartInfo.Arguments = $"-d {Quote (tmp_extraction_dir)} {Quote (local_zip)}";
						Log ("{0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments);
						p.Start ();
						p.WaitForExit ();
						if (p.ExitCode != 0) {
							Log ("Could not unzip mlaunch, exit code: {0}", p.ExitCode);
							return mlaunch_path;
						}
					}
					Directory.Move (tmp_extraction_dir, extraction_dir);

					Log ("Final mlaunch path: {0}", mlaunch_path);
				} catch (Exception e) {
					Log ("Could not download mlaunch: {0}", e);
				}
				return mlaunch_path;
			}
		}

		public string MtouchPath {
			get {
				return Path.Combine (IOS_DESTDIR, "Library", "Frameworks", "Xamarin.iOS.framework", "Versions", "Current", "bin", "mtouch");
			}
		}

		string mlaunch;
		public string MlaunchPath {
			get {
				if (mlaunch == null) {
					var dir = Path.GetFullPath (RootDirectory);
					while (dir.Length > 3) {
						var filename = Path.GetFullPath (Path.Combine (dir, "maccore", "tools", "mlaunch", "mlaunch"));
						if (File.Exists (filename))
							return mlaunch = filename;
						dir = Path.GetDirectoryName (dir);
					}

					string path = string.Empty;

					// check next to mtouch
					path = Path.Combine (Path.GetDirectoryName (MtouchPath), "mlaunch");
					if (File.Exists (path))
						return mlaunch = path;

					Log ("Could not find mlaunch locally, will try downloading it.");
					try {
						path = DownloadMlaunch ();
					} catch (Exception e) {
						Log ("Could not download mlaunch: {0}", e);
					}
					if (!File.Exists (path)) {
						Log ("Will try in Xamarin Studio.app.", path);
						path = "/Applications/Xamarin Studio.app/Contents/Resources/lib/monodevelop/AddIns/MonoDevelop.IPhone/mlaunch.app/Contents/MacOS/mlaunch";
					}

					if (!File.Exists (path))
						throw new FileNotFoundException (string.Format ("Could not find mlaunch: {0}", path));

					Log ("Found mlaunch: {0}", path);

					mlaunch = path;
				}

				return mlaunch;
			}
		}

		public static string Quote (string f)
		{
			if (f.IndexOf (' ') == -1 && f.IndexOf ('\'') == -1 && f.IndexOf (',') == -1)
				return f;

			var s = new StringBuilder ();

			s.Append ('"');
			foreach (var c in f) {
				if (c == '"' || c == '\\')
					s.Append ('\\');

				s.Append (c);
			}
			s.Append ('"');

			return s.ToString ();
		}

		void CreateBCLProjects ()
		{
			foreach (var bclTest in BclTests) {
				var target = new BCLTarget () {
					Harness = this,
					MonoPath = MONO_PATH,
					WatchMonoPath = WATCH_MONO_PATH,
					TestName = bclTest,
				};
				target.Convert ();
			}
		}

		void LoadConfig ()
		{
			ParseConfigFiles ();
			var src_root = Path.GetDirectoryName (RootDirectory);
			MONO_PATH = Path.GetFullPath (Path.Combine (src_root, "external", "mono"));
			WATCH_MONO_PATH = make_config ["WATCH_MONO_PATH"];
			TVOS_MONO_PATH = MONO_PATH;
			INCLUDE_WATCH = make_config.ContainsKey ("INCLUDE_WATCH") && !string.IsNullOrEmpty (make_config ["INCLUDE_WATCH"]);
			JENKINS_RESULTS_DIRECTORY = make_config ["JENKINS_RESULTS_DIRECTORY"];
			MAC_DESTDIR = make_config ["MAC_DESTDIR"];
			IOS_DESTDIR = make_config ["IOS_DESTDIR"];
		}
		 
		void AutoConfigureMac ()
		{
			var test_suites = new string[] { "apitest", "dontlink-mac" }; 
			var hard_coded_test_suites = new string[] { "mmptest", "msbuild-mac" };
			//var library_projects = new string[] { "BundledResources", "EmbeddedResources", "bindings-test", "bindings-framework-test" };
			//var fsharp_test_suites = new string[] { "fsharp" };
			//var fsharp_library_projects = new string[] { "fsharplibrary" };
			//var bcl_suites = new string[] { "mscorlib", "System", "System.Core", "System.Data", "System.Net.Http", "System.Numerics", "System.Runtime.Serialization", "System.Transactions", "System.Web.Services", "System.Xml", "System.Xml.Linq", "Mono.Security", "System.ComponentModel.DataAnnotations", "System.Json", "System.ServiceModel.Web", "Mono.Data.Sqlite" };
			foreach (var p in test_suites)
				MacTestProjects.Add (new MacTestProject (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj"))));
			MacTestProjects.Add (new MacTestProject (Path.GetFullPath (Path.Combine (RootDirectory, "introspection", "Mac", "introspection-mac.csproj")), skipXMVariations : true));
			foreach (var p in hard_coded_test_suites)
				MacTestProjects.Add (new MacTestProject (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj")), generateVariations: false));
			//foreach (var p in fsharp_test_suites)
			//	TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".fsproj")));
			//foreach (var p in library_projects)
			//TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj")));
			//foreach (var p in fsharp_library_projects)
			//TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".fsproj")));
			//foreach (var p in bcl_suites)
			//TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, "bcl-test/" + p + "/" + p + ".csproj")));

			// BclTests.AddRange (bcl_suites);
		}

		void AutoConfigureIOS ()
		{
			var test_suites = new string [] { "monotouch-test", "framework-test", "mini" };
			var library_projects = new string [] { "BundledResources", "EmbeddedResources", "bindings-test", "bindings-framework-test" };
			var fsharp_test_suites = new string [] { "fsharp" };
			var fsharp_library_projects = new string [] { "fsharplibrary" };
			var bcl_suites = new string [] { "mscorlib", "System", "System.Core", "System.Data", "System.Net.Http", "System.Numerics", "System.Runtime.Serialization", "System.Transactions", "System.Web.Services", "System.Xml", "System.Xml.Linq", "Mono.Security", "System.ComponentModel.DataAnnotations", "System.Json", "System.ServiceModel.Web", "Mono.Data.Sqlite" };
			IOSTestProjects.Add (new TestProject (Path.GetFullPath (Path.Combine (RootDirectory, "bcl-test/mscorlib/mscorlib-0.csproj")), false));
			IOSTestProjects.Add (new TestProject (Path.GetFullPath (Path.Combine (RootDirectory, "bcl-test/mscorlib/mscorlib-1.csproj")), false));
			foreach (var p in test_suites)
				IOSTestProjects.Add (new TestProject (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj"))));
			foreach (var p in fsharp_test_suites)
				IOSTestProjects.Add (new TestProject (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".fsproj"))));
			foreach (var p in library_projects)
				IOSTestProjects.Add (new TestProject (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj")), false));
			foreach (var p in fsharp_library_projects)
				IOSTestProjects.Add (new TestProject (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".fsproj")), false));
			foreach (var p in bcl_suites)
				IOSTestProjects.Add (new TestProject (Path.GetFullPath (Path.Combine (RootDirectory, "bcl-test/" + p + "/" + p + ".csproj"))));
			IOSTestProjects.Add (new TestProject (Path.GetFullPath (Path.Combine (RootDirectory, "introspection", "iOS", "introspection-ios.csproj"))));
			IOSTestProjects.Add (new TestProject (Path.GetFullPath (Path.Combine (RootDirectory, "linker-ios", "dont link", "dont link.csproj"))));
			IOSTestProjects.Add (new TestProject (Path.GetFullPath (Path.Combine (RootDirectory, "linker-ios", "link all", "link all.csproj"))));
			IOSTestProjects.Add (new TestProject (Path.GetFullPath (Path.Combine (RootDirectory, "linker-ios", "link sdk", "link sdk.csproj"))));

			BclTests.AddRange (bcl_suites);

			WatchOSContainerTemplate = Path.GetFullPath (Path.Combine (RootDirectory, "watchos/Container"));
			WatchOSAppTemplate = Path.GetFullPath (Path.Combine (RootDirectory, "watchos/App"));
			WatchOSExtensionTemplate = Path.GetFullPath (Path.Combine (RootDirectory, "watchos/Extension"));
		}

		static Dictionary<string, string> make_config = new Dictionary<string, string> ();
		static IEnumerable<string> FindConfigFiles (string name)
		{
			var dir = Environment.CurrentDirectory;
			while (dir != "/") {
				var file = Path.Combine (dir, name);
				if (File.Exists (file))
					yield return file;
				dir = Path.GetDirectoryName (dir);
			}
		}

		static void ParseConfigFiles ()
		{
			ParseConfigFiles (FindConfigFiles ("test.config"));
			ParseConfigFiles (FindConfigFiles ("Make.config.local"));
			ParseConfigFiles (FindConfigFiles ("Make.config"));
		}

		static void ParseConfigFiles (IEnumerable<string> files)
		{
			foreach (var file in files)
				ParseConfigFile (file);
		}

		static void ParseConfigFile (string file)
		{
			if (string.IsNullOrEmpty (file))
				return;

			foreach (var line in File.ReadAllLines (file)) {
				var eq = line.IndexOf ('=');
				if (eq == -1)
					continue;
				var key = line.Substring (0, eq);
				if (!make_config.ContainsKey (key))
					make_config [key] = line.Substring (eq + 1);
			}
		}

		public int Configure ()
		{
			if (Mac)
				ConfigureMac ();
			else
				ConfigureIOS ();
			return 0;
		}

		void ConfigureMac ()
		{
			var classic_targets = new List<MacClassicTarget> ();
			var unified_targets = new List<MacUnifiedTarget> ();
			var hardcoded_unified_targets = new List<MacUnifiedTarget> ();
 
 			RootDirectory = Path.GetFullPath (RootDirectory).TrimEnd ('/');
 
 			if (AutoConf)
				AutoConfigureMac ();
 
 			CreateBCLProjects ();
 
			foreach (var proj in MacTestProjects.Where ((v) => v.GenerateVariations)) {
				var file = proj.Path;
 				if (!File.Exists (file))
 					throw new FileNotFoundException (file);

				foreach (bool thirtyTwoBit in new bool[] { false, true })
				{
					var unifiedMobile = new MacUnifiedTarget (true, thirtyTwoBit)
					{
						TemplateProjectPath = file,
						Harness = this,
					};
					unifiedMobile.Execute ();
					unified_targets.Add (unifiedMobile);

					if (!proj.SkipXMVariations) {
						var unifiedXM45 = new MacUnifiedTarget (false, thirtyTwoBit)
						{
							TemplateProjectPath = file,
							Harness = this,
						};
						unifiedXM45.Execute ();
						unified_targets.Add (unifiedXM45);
					}
				}
 
				var classic = new MacClassicTarget () {
 					TemplateProjectPath = file,
 					Harness = this,
 				};
				classic.Execute ();
				classic_targets.Add (classic);
			}
 
			foreach (var proj in MacTestProjects.Where ((v) => !v.GenerateVariations)) {
				var file = proj.Path;
				var unifiedMobile = new MacUnifiedTarget (true, false, true)
				{
 					TemplateProjectPath = file,
 					Harness = this,
 				};
				unifiedMobile.Execute ();
				hardcoded_unified_targets.Add (unifiedMobile);
 			}
 
			MakefileGenerator.CreateMacMakefile (this, classic_targets.Union<MacTarget> (unified_targets).Union (hardcoded_unified_targets) );
		}

		void ConfigureIOS ()
		{
			var unified_targets = new List<UnifiedTarget> ();
			var tvos_targets = new List<TVOSTarget> ();
			var watchos_targets = new List<WatchOSTarget> ();

			RootDirectory = Path.GetFullPath (RootDirectory).TrimEnd ('/');

			if (AutoConf)
				AutoConfigureIOS ();

			CreateBCLProjects ();

			foreach (var proj in IOSTestProjects) {
				var file = proj.Path;
				if (!File.Exists (file))
					throw new FileNotFoundException (file);

				var watchos = new WatchOSTarget () {
					TemplateProjectPath = file,
					Harness = this,
				};
				watchos.Execute ();
				watchos_targets.Add (watchos);

				var tvos = new TVOSTarget () {
					TemplateProjectPath = file,
					Harness = this,
				};
				tvos.Execute ();
				tvos_targets.Add (tvos);

				var unified = new UnifiedTarget () {
					TemplateProjectPath = file,
					Harness = this,
				};
				unified.Execute ();
				unified_targets.Add (unified);
			}

			SolutionGenerator.CreateSolution (this, watchos_targets, "watchos");
			SolutionGenerator.CreateSolution (this, tvos_targets, "tvos");
			MakefileGenerator.CreateMakefile (this, unified_targets, tvos_targets, watchos_targets);
		}

		public int Install ()
		{
			if (HarnessLog == null)
				HarnessLog = new ConsoleLog ();
			
			foreach (var project in IOSTestProjects) {
				var runner = new AppRunner () {
					Harness = this,
					ProjectFile = project.Path,
					MainLog = HarnessLog,
				};
				var rv = runner.Install (HarnessLog);
				if (rv != 0)
					return rv;
			}
			return 0;
		}

		public int Run ()
		{
			if (HarnessLog == null)
				HarnessLog = new ConsoleLog ();
			
			foreach (var project in IOSTestProjects) {
				var runner = new AppRunner () {
					Harness = this,
					ProjectFile = project.Path,
					MainLog = HarnessLog,
				};
				var rv = runner.RunAsync ().Result;
				if (rv != 0)
					return rv;
			}
			return 0;
		}

		public void Log (int min_level, string message)
		{
			if (Verbosity < min_level)
				return;
			Console.WriteLine (message);
			HarnessLog?.WriteLine (message);
		}

		public void Log (int min_level, string message, params object[] args)
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

		public void Log (string message, params object[] args)
		{
			Log (0, message, args);
		}

		public void LogWrench (string message, params object[] args)
		{
			if (!InWrench)
				return;

			Console.WriteLine (message, args);
		}

		public void LogWrench (string message)
		{
			if (!InWrench)
				return;

			Console.WriteLine (message);
		}

		public bool InWrench {
			get {
				var buildRev = Environment.GetEnvironmentVariable ("BUILD_REVISION");
				return !string.IsNullOrEmpty (buildRev) && buildRev != "jenkins";
			}
		}
		
		public bool InJenkins {
			get {
				var buildRev = Environment.GetEnvironmentVariable ("BUILD_REVISION");
				return !string.IsNullOrEmpty (buildRev) && buildRev == "jenkins";
			}
		}

		public int Execute ()
		{
			LoadConfig ();
			switch (Action) {
			case HarnessAction.Configure:
				return Configure ();
			case HarnessAction.Run:
				return Run ();
			case HarnessAction.Install:
				return Install ();
			case HarnessAction.Jenkins:
				return Jenkins ();
			default:
				throw new NotImplementedException (Action.ToString ());
			}
		}

		public int Jenkins ()
		{
			if (AutoConf) {
				AutoConfigureIOS ();
				AutoConfigureMac ();
			}
			
			var jenkins = new Jenkins ()
			{
				Harness = this,
			};
			return jenkins.Run ();
		}

		public void Save (XmlDocument doc, string path)
		{
			if (!File.Exists (path)) {
				doc.Save (path);
				Log (1, "Created {0}", path);
			} else {
				var tmpPath = path + ".tmp";
				doc.Save (tmpPath);
				var existing = File.ReadAllText (path);
				var updated = File.ReadAllText (tmpPath);

				if (existing == updated) {
					File.Delete (tmpPath);
					Log (1, "Not saved {0}, no change", path);
				} else {
					File.Delete (path);
					File.Move (tmpPath, path);
					Log (1, "Updated {0}", path);
				}
			}
		}

		public void Save (StringWriter doc, string path)
		{
			if (!File.Exists (path)) {
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

		public void Save (string doc, string path)
		{
			if (!File.Exists (path)) {
				File.WriteAllText (path, doc);
				Log (1, "Created {0}", path);
			} else {
				var existing = File.ReadAllText (path);
				if (existing == doc) {
					Log (1, "Not saved {0}, no change", path);
				} else {
					File.WriteAllText (path, doc);
					Log (1, "Updated {0}", path);
				}
			}
		}

		// We want guids that nobody else has, but we also want to generate the same guid
		// on subsequent invocations (so that csprojs don't change unnecessarily, which is
		// annoying when XS reloads the projects, and also causes unnecessary rebuilds).
		// Nothing really breaks when the sequence isn't identical from run to run, so
		// this is just a best minimal effort.
		static Random guid_generator = new Random (unchecked ((int) 0xdeadf00d));
		public Guid NewStableGuid ()
		{
			var bytes = new byte [16];
			guid_generator.NextBytes (bytes);
			return new Guid (bytes);
		}

		bool? disable_watchos_on_wrench;
		public bool DisableWatchOSOnWrench {
			get {
				if (!disable_watchos_on_wrench.HasValue)
					disable_watchos_on_wrench = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("DISABLE_WATCH_ON_WRENCH"));
				return disable_watchos_on_wrench.Value;
			}
		}

		public Task<ProcessExecutionResult> ExecuteXcodeCommandAsync (string executable, string args, TextWriter output, TimeSpan timeout)
		{
			return ProcessHelper.ExecuteCommandAsync (Path.Combine (XcodeRoot, "Contents", "Developer", "usr", "bin", executable), args, output, timeout: timeout);
		}

		public Task<ProcessExecutionResult> ExecuteXcodeCommandAsync (string executable, string args, Log log, TimeSpan timeout)
		{
			return ProcessHelper.ExecuteCommandAsync (Path.Combine (XcodeRoot, "Contents", "Developer", "usr", "bin", executable), args, log.GetWriter () , timeout: timeout);
		}

		public async Task ShowSimulatorList (LogStream log)
		{
			await ExecuteXcodeCommandAsync ("simctl", "list", log.GetWriter (), TimeSpan.FromSeconds (10));
		}

		public async Task<LogFile> SymbolicateCrashReportAsync (Log log, LogFile report)
		{
			var symbolicatecrash = Path.Combine (XcodeRoot, "Contents/SharedFrameworks/DTDeviceKitBase.framework/Versions/A/Resources/symbolicatecrash");
			if (!File.Exists (symbolicatecrash))
				symbolicatecrash = Path.Combine (XcodeRoot, "Contents/SharedFrameworks/DVTFoundation.framework/Versions/A/Resources/symbolicatecrash");

			if (!File.Exists (symbolicatecrash)) {
				log.WriteLine ("Can't symbolicate {0} because the symbolicatecrash script {1} does not exist", report.Path, symbolicatecrash);
				return report;
			}

			var symbolicated = new LogFile ("Symbolicated crash report", report.Path + ".symbolicated");
			var environment = new Dictionary<string, string> { { "DEVELOPER_DIR", Path.Combine (XcodeRoot, "Contents", "Developer") } };
			var rv = await ProcessHelper.ExecuteCommandAsync (symbolicatecrash, Quote (report.Path), symbolicated, TimeSpan.FromMinutes (1), environment);
			if (rv.Succeeded) {;
				log.WriteLine ("Symbolicated {0} successfully.", report.Path);
				return symbolicated;
			} else {
				log.WriteLine ("Failed to symbolicate {0}.", report.Path);
				return report;
			}
		}

		public async Task<HashSet<string>> CreateCrashReportsSnapshotAsync (Log log, bool simulatorOrDesktop, string device)
		{
			var rv = new HashSet<string> ();

			if (simulatorOrDesktop) {
				var dir = Path.Combine (Environment.GetEnvironmentVariable ("HOME"), "Library", "Logs", "DiagnosticReports");
				if (Directory.Exists (dir))
					rv.UnionWith (Directory.EnumerateFiles (dir));
			} else {
				var tmp = Path.GetTempFileName ();
				try {
					var sb = new StringBuilder ();
					sb.Append (" --list-crash-reports=").Append (Quote (tmp));
					sb.Append (" --sdkroot ").Append (Quote (XcodeRoot));
					if (!string.IsNullOrEmpty (device))
						sb.Append (" --devname ").Append (Quote (device));
					var result = await ProcessHelper.ExecuteCommandAsync (MlaunchPath, sb.ToString (), log, TimeSpan.FromMinutes (1));
					if (result.Succeeded)
						rv.UnionWith (File.ReadAllLines (tmp));
				} finally {
					File.Delete (tmp);
				}
			}

			return rv;
		}
	}

	public class CrashReportSnapshot
	{
		public Harness Harness { get; set; }
		public Log Log { get; set; }
		public Logs Logs { get; set; }
		public string LogDirectory { get; set; }
		public bool Device { get; set; }
		public string DeviceName { get; set; }

		public HashSet<string> InitialSet { get; private set; }
		public IEnumerable<string> Reports { get; private set; }

		public async Task StartCaptureAsync ()
		{
			InitialSet = await Harness.CreateCrashReportsSnapshotAsync (Log, !Device, DeviceName);
		}

		public async Task EndCaptureAsync (TimeSpan timeout)
		{
			// Check for crash reports
			var crash_report_search_done = false;
			var crash_report_search_timeout = timeout.TotalSeconds;
			var watch = new Stopwatch ();
			watch.Start ();
			do {
				var end_crashes = await Harness.CreateCrashReportsSnapshotAsync (Log, !Device, DeviceName);
				end_crashes.ExceptWith (InitialSet);
				Reports = end_crashes;
				if (end_crashes.Count > 0) {
					Log.WriteLine ("Found {0} new crash report(s)", end_crashes.Count);
					List<LogFile> crash_reports;
					if (!Device) {
						crash_reports = new List<LogFile> (end_crashes.Count);
						foreach (var path in end_crashes) {
							var logPath = Path.Combine (LogDirectory, Path.GetFileName (path));
							File.Copy (path, logPath, true);
							crash_reports.Add (Logs.CreateFile ("Crash report: " + Path.GetFileName (path), logPath));
						}
					} else {
						// Download crash reports from the device. We put them in the project directory so that they're automatically deleted on wrench
						// (if we put them in /tmp, they'd never be deleted).
						var downloaded_crash_reports = new List<LogFile> ();
						foreach (var file in end_crashes) {
							var crash_report_target = Logs.CreateFile ("Crash report: " + Path.GetFileName (file), Path.Combine (LogDirectory, Path.GetFileName (file)));
							var sb = new StringBuilder ();
							sb.Append (" --download-crash-report=").Append (Harness.Quote (file));
							sb.Append (" --download-crash-report-to=").Append (Harness.Quote (crash_report_target.Path));
							sb.Append (" --sdkroot ").Append (Harness.Quote (Harness.XcodeRoot));
							if (!string.IsNullOrEmpty (DeviceName))
								sb.Append (" --devname ").Append (Harness.Quote (DeviceName));
							var result = await ProcessHelper.ExecuteCommandAsync (Harness.MlaunchPath, sb.ToString (), Log, TimeSpan.FromMinutes (1));
							if (result.Succeeded) {
								Log.WriteLine ("Downloaded crash report {0} to {1}", file, crash_report_target.Path);
								crash_report_target = await Harness.SymbolicateCrashReportAsync (Log, crash_report_target);
								Logs.Add (crash_report_target);
								downloaded_crash_reports.Add (crash_report_target);
							} else {
								Log.WriteLine ("Could not download crash report {0}", file);
							}
						}
						crash_reports = downloaded_crash_reports;
					}
					foreach (var cp in crash_reports) {
						Harness.LogWrench ("@MonkeyWrench: AddFile: {0}", cp.Path);
						Log.WriteLine ("    {0}", cp.Path);
					}
					crash_report_search_done = true;
				} else {
					if (watch.Elapsed.TotalSeconds > crash_report_search_timeout) {
						crash_report_search_done = true;
					} else {
						Log.WriteLine ("No crash reports, waiting a second to see if the crash report service just didn't complete in time ({0})", (int) (crash_report_search_timeout - watch.Elapsed.TotalSeconds));
						Thread.Sleep (TimeSpan.FromSeconds (1));
					}
				}
			} while (!crash_report_search_done);
		}
	}
}
