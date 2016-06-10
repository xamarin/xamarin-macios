using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace xharness
{
	public enum HarnessAction
	{
		None,
		Configure,
		Run,
		Install,
	}

	public class Harness
	{
		public HarnessAction Action { get; set; }
		public int Verbosity { get; set; }

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

		public List<string> TestProjects { get; set; } = new List<string> ();
		public List<string> HardCodedTestProjects { get; set; } = new List<string> ();
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

		// Run
		public string Target { get; set; }
		public string SdkRoot { get; set; } = "/Applications/Xcode.app";
		public string Configuration { get; set; } = "Debug";
		public string LogFile { get; set; }
		public double Timeout { get; set; } = 10; // in minutes
		public double LaunchTimeout { get; set; } // in minutes

		public Harness ()
		{
			LaunchTimeout = InWrench ? 1 : 120;
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

		public string MlaunchPath {
			get {
				var path = Path.GetFullPath (Path.Combine (Path.GetDirectoryName (Path.GetDirectoryName (RootDirectory)), "maccore", "tools", "mlaunch", "mlaunch"));
				if (!File.Exists (path)) {
					Log ("Could not find mlaunch locally ({0}), will try in Xamarin Studio.app.", path);
					path = "/Applications/Xamarin Studio.app/Contents/Resources/lib/monodevelop/AddIns/MonoDevelop.IPhone/mlaunch.app/Contents/MacOS/mlaunch";
				}

				if (!File.Exists (path))
					throw new FileNotFoundException (string.Format ("Could not find mlaunch: {0}", path));
				
				return path;
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
					TVOSMonoPath = TVOS_MONO_PATH,
					TestName = bclTest,
				};
				target.Convert ();
			}
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
				TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj")));
			TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, "introspection", "Mac", "introspection-mac.csproj")));
			foreach (var p in hard_coded_test_suites)
				HardCodedTestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj")));
			//foreach (var p in fsharp_test_suites)
			//	TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".fsproj")));
			//foreach (var p in library_projects)
				//TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj")));
			//foreach (var p in fsharp_library_projects)
				//TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".fsproj")));
			//foreach (var p in bcl_suites)
				//TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, "bcl-test/" + p + "/" + p + ".csproj")));

			// BclTests.AddRange (bcl_suites);

			ParseConfigFiles ();
			var src_root = Path.Combine (Path.GetDirectoryName (Path.GetDirectoryName (RootDirectory)));
			MONO_PATH = Path.GetFullPath (Path.Combine (src_root, "mono"));
		}

		void AutoConfigure ()
		{
			var test_suites = new string [] { "monotouch-test", "framework-test", "mini" };
			var library_projects = new string [] { "BundledResources", "EmbeddedResources", "bindings-test", "bindings-framework-test" };
			var fsharp_test_suites = new string [] { "fsharp" };
			var fsharp_library_projects = new string [] { "fsharplibrary" };
			var bcl_suites = new string [] { "mscorlib", "System", "System.Core", "System.Data", "System.Net.Http", "System.Numerics", "System.Runtime.Serialization", "System.Transactions", "System.Web.Services", "System.Xml", "System.Xml.Linq", "Mono.Security", "System.ComponentModel.DataAnnotations", "System.Json", "System.ServiceModel.Web", "Mono.Data.Sqlite" };
			foreach (var p in test_suites)
				TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj")));
			foreach (var p in fsharp_test_suites)
				TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".fsproj")));
			foreach (var p in library_projects)
				TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".csproj")));
			foreach (var p in fsharp_library_projects)
				TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, p + "/" + p + ".fsproj")));
			foreach (var p in bcl_suites)
				TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, "bcl-test/" + p + "/" + p + ".csproj")));
			TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, "introspection", "iOS", "introspection-ios.csproj")));
			TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, "linker-ios", "dont link", "dont link.csproj")));
			TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, "linker-ios", "link all", "link all.csproj")));
			TestProjects.Add (Path.GetFullPath (Path.Combine (RootDirectory, "linker-ios", "link sdk", "link sdk.csproj")));
			BclTests.AddRange (bcl_suites);

			WatchOSContainerTemplate = Path.GetFullPath (Path.Combine (RootDirectory, "watchos/Container"));
			WatchOSAppTemplate = Path.GetFullPath (Path.Combine (RootDirectory, "watchos/App"));
			WatchOSExtensionTemplate = Path.GetFullPath (Path.Combine (RootDirectory, "watchos/Extension"));

			ParseConfigFiles ();
			var src_root = Path.GetDirectoryName (RootDirectory);
			MONO_PATH = Path.GetFullPath (Path.Combine (src_root, "external", "mono"));
			WATCH_MONO_PATH = make_config ["WATCH_MONO_PATH"];
			TVOS_MONO_PATH = MONO_PATH;
			INCLUDE_WATCH = make_config.ContainsKey ("INCLUDE_WATCH") && !string.IsNullOrEmpty (make_config ["INCLUDE_WATCH"]);
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
 
 			foreach (var file in TestProjects) {
 				if (!File.Exists (file))
 					throw new FileNotFoundException (file);
								
				var unifiedMobile = new MacUnifiedTarget (true) {
 					TemplateProjectPath = file,
 					Harness = this,
 				};
				unifiedMobile.Execute ();
				unified_targets.Add (unifiedMobile);
 
				var unifiedXM45 = new MacUnifiedTarget (false) {
 					TemplateProjectPath = file,
 					Harness = this,
 				};
				unifiedXM45.Execute ();
				unified_targets.Add (unifiedXM45);
 
				var classic = new MacClassicTarget () {
 					TemplateProjectPath = file,
 					Harness = this,
 				};
				classic.Execute ();
				classic_targets.Add (classic);
			}
 
			foreach (var file in HardCodedTestProjects) {
				var unifiedMobile = new MacUnifiedTarget (true, true)
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
			var classic_targets = new List<ClassicTarget> ();
			var unified_targets = new List<UnifiedTarget> ();
			var tvos_targets = new List<TVOSTarget> ();
			var watchos_targets = new List<WatchOSTarget> ();

			RootDirectory = Path.GetFullPath (RootDirectory).TrimEnd ('/');

			if (AutoConf)
				AutoConfigure ();

			CreateBCLProjects ();

			foreach (var file in TestProjects) {
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

				var classic = new ClassicTarget () {
					TemplateProjectPath = file,
					Harness = this,
				};
				classic.Execute ();
				classic_targets.Add (classic);
			}

			SolutionGenerator.CreateSolution (this, watchos_targets, "watchos");
			SolutionGenerator.CreateSolution (this, tvos_targets, "tvos");
			SolutionGenerator.CreateSolution (this, unified_targets, "unified");
			MakefileGenerator.CreateMakefile (this, classic_targets, unified_targets, tvos_targets, watchos_targets);
		}

		public int Install ()
		{
			foreach (var project in TestProjects) {
				var runner = new AppRunner () {
					Harness = this,
					ProjectFile = project,
				};
				var rv = runner.Install ();
				if (rv != 0)
					return rv;
			}
			return 0;
		}

		public int Run ()
		{
			foreach (var project in TestProjects) {
				var runner = new AppRunner () {
					Harness = this,
					ProjectFile = project,
				};
				var rv = runner.Run ();
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
		}

		public void Log (int min_level, string message, params object[] args)
		{
			if (Verbosity < min_level)
				return;
			Console.WriteLine (message, args);
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
				return !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("BUILD_REVISION"));
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
			default:
				throw new NotImplementedException (Action.ToString ());
			}
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
	}
}
