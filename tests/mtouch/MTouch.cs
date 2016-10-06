using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using MTouchTests;
using Xamarin.Tests;

using NUnit.Framework;

namespace Xamarin.Tests {
	static class TestTarget {
		public static string ToolPath { 
			get {
				return Path.Combine (Configuration.SdkBinDir, "mtouch");
			}
		}
	}
}

namespace MTouchTests
{
	[TestFixture]
	public class MTouch
	{
		[Test]
		[TestCase ("single", "-sdkroot {2} -v -v -v -v --dev {0} -sdk {3} --targetver 6.0 {1} -r:{4} --cache={5}/cache")]
		[TestCase ("dual",   "-sdkroot {2} -v -v -v -v --dev {0} -sdk {3} --targetver 6.0 {1} -r:{4} --cache={5}/cache --abi=armv7,arm64")]
		[TestCase ("llvm",   "-sdkroot {2} -v -v -v -v --dev {0} -sdk {3} --targetver 6.0 {1} -r:{4} --cache={5}/cache --abi=armv7+llvm")]
		[TestCase ("debug",  "-sdkroot {2} -v -v -v -v --dev {0} -sdk {3} --targetver 6.0 {1} -r:{4} --cache={5}/cache --debug")]
		public void RebuildTest (string name, string format)
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			DateTime dt = DateTime.MinValue;

			Action<string, IEnumerable<string>> checkNotModified = (filename, skip) => {
				var failed = new List<string> ();
				var files = Directory.EnumerateFiles (app, "*", SearchOption.AllDirectories);
				foreach (var file in files) {
					if (skip != null && skip.Contains (Path.GetFileName (file)))
						continue;
					var info = new FileInfo (file);
					if (info.LastWriteTime > dt) {
						failed.Add (string.Format ("{0} is modified, timestamp: {1}", file, info.LastWriteTime));
					} else {
						Console.WriteLine ("{0} not modified", file);
					}
				}
				Assert.IsTrue (failed.Count == 0, filename + "\n" + string.Join ("\n", failed.ToArray ()));
			};

			Directory.CreateDirectory (app);
			try {
				var exe = CompileUnifiedTestAppExecutable (testDir);
				var args = string.Format (format, app, exe, Configuration.xcode_root, Configuration.sdk_version, Configuration.XamarinIOSDll, testDir);
				ExecutionHelper.Execute (TestTarget.ToolPath, args);
				dt = DateTime.Now;
				System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older.
				ExecutionHelper.Execute (TestTarget.ToolPath, args);

				checkNotModified (name, null);

				// Test that a rebuild (where something changed, in this case the .exe)
				// actually work. We compile with custom code to make sure it's different
				// from the previous exe we built.
				var subDir = Path.Combine (testDir, "other");
				Directory.CreateDirectory (subDir);
				var exe2 = CompileUnifiedTestAppExecutable (subDir, 
					/* the code here only changes the class name (default: 'TestApp' changed to 'TestApp2') to minimize the related
					 * changes (there should be no changes in Xamarin.iOS.dll nor mscorlib.dll, even after linking) */
					code: "public class TestApp2 { static void Main () { System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ()); } }");
				File.Copy (exe2, exe, true);
				ExecutionHelper.Execute (TestTarget.ToolPath, args);

				var skip = new string [] { "testApp", "testApp.exe", "testApp.armv7.aotdata", "testApp.arm64.aotdata" };
				checkNotModified (name + "-rebuilt", skip);
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void RebuildTest_Intl ()
		{
			using (var tool = new MTouchTool ()) {
				tool.Profile = Profile.Unified;
				tool.I18N = I18N.West;
				tool.Verbosity = 5;
				tool.Cache = Path.Combine (tool.CreateTemporaryDirectory (), "mtouch-test-cache");
				tool.CreateTemporaryApp ();

				Assert.AreEqual (0, tool.Execute (MTouchAction.BuildSim));

				var pre_files = Directory.EnumerateFiles (tool.AppPath, "*", SearchOption.AllDirectories).ToArray ();

				Directory.Delete (tool.AppPath, true);
				Directory.CreateDirectory (tool.AppPath);

				Assert.AreEqual (0, tool.Execute (MTouchAction.BuildSim));

				var post_files = Directory.EnumerateFiles (tool.AppPath, "*", SearchOption.AllDirectories).ToArray ();

				Assert.That (post_files, Is.EquivalentTo (pre_files), "files");
			}
		}

		public enum Target { Sim, Dev }
		public enum Config { Debug, Release }
		public enum PackageMdb { Default, WithMdb, WoutMdb }
		public enum MSym { Default, WithMSym, WoutMSym }
		public enum Profile { Unified, TVOS, WatchOS }

		[Test]
		// Simulator
		[TestCase (Target.Sim, Config.Release, PackageMdb.Default, MSym.Default,  false, false, "")]
		[TestCase (Target.Sim, Config.Debug,   PackageMdb.Default, MSym.Default,  true,  false, "")]
		[TestCase (Target.Sim, Config.Debug,   PackageMdb.WoutMdb, MSym.Default,  false, false, "")]
		[TestCase (Target.Sim, Config.Release, PackageMdb.WithMdb, MSym.Default,  true,  false, "")]
		[TestCase (Target.Sim, Config.Debug,   PackageMdb.WoutMdb, MSym.Default,  false, false, "--nofastsim --nolink")]
		// Device
		[TestCase (Target.Dev, Config.Release, PackageMdb.WithMdb, MSym.Default,  true,  false, "")]
		[TestCase (Target.Dev, Config.Release, PackageMdb.WithMdb, MSym.WoutMSym, true,  false, "")]
		[TestCase (Target.Dev, Config.Release, PackageMdb.Default, MSym.Default,  false, false, "--abi:armv7,arm64")]
		[TestCase (Target.Dev, Config.Debug,   PackageMdb.WoutMdb, MSym.Default,  false, false, "")]
		[TestCase (Target.Dev, Config.Debug,   PackageMdb.WoutMdb, MSym.WithMSym, false, true,  "")]
		[TestCase (Target.Dev, Config.Release, PackageMdb.WithMdb, MSym.Default,  true,  false, "--abi:armv7+llvm")]
		public void SymbolicationData (Target target, Config configuration, PackageMdb package_mdb, MSym msym, bool has_mdb, bool has_msym, string extra_mtouch_args)
		{
			var testDir = GetTempDirectory ();
			var appDir = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (appDir);

			try {
				var is_sim = target == Target.Sim;
				var exe = CompileUnifiedTestAppExecutable (testDir);
				var msymDir = appDir + ".msym";
				var args = extra_mtouch_args;

				args += is_sim ? " --sim {0} " : " --dev {0} ";

				if (configuration == Config.Debug)
					args += "--debug ";

				if (package_mdb == PackageMdb.WithMdb)
					args += "--package-mdb:true ";
				else if (package_mdb == PackageMdb.WoutMdb)
					args += "--package-mdb:false ";

				if (msym == MSym.WithMSym)
					args += "--msym:true ";
				else if (msym == MSym.WoutMSym)
					args += "--msym:false ";
				
				args += " --sdkroot {2} -v -v -v -sdk {3} {1} -r:{4}";

				ExecutionHelper.Execute (TestTarget.ToolPath, string.Format (args, appDir, exe, Configuration.xcode_root, Configuration.sdk_version, Configuration.XamarinIOSDll));

				if (is_sim) {
					Assert.AreEqual (has_mdb, File.Exists (Path.Combine (appDir, "mscorlib.dll.mdb")), "#mdb");
				} else {
					Assert.AreEqual (has_mdb, File.Exists (Path.Combine (appDir, ".monotouch-32", "mscorlib.dll.mdb")), "#mdb");
				}

				if (has_msym) {
					// assert that we do have the msym in one of the subdirs. We do not know the AOTID so we
					// get all present files in the subdirs.
					var dirInfo = new DirectoryInfo (msymDir);
					var subDirs = dirInfo.GetDirectories ();
					var msymFiles = new List<string> ();
					foreach (var dir in subDirs) {
						foreach (var f in dir.GetFiles ()) {
							msymFiles.Add (f.Name);
						}
					}
					Assert.AreEqual (has_msym, msymFiles.Contains ("mscorlib.dll.msym"));
				} 
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void ExecutableName ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileTestAppExecutable (testDir);

				ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --executable=CustomExecutable -v -v -v --nofastsim --nolink --sim {0} -sdk " + Configuration.sdk_version + " {1} -debug --fastdev -r:{2}", app, exe, Configuration.XamarinIOSDll));
				Assert.That (File.Exists (Path.Combine (app, "CustomExecutable")), "1");
				Assert.That (!File.Exists (Path.Combine (app, "testApp")), "2");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void MT0015 ()
		{
			Asserts.Throws<TestExecutionException> (() =>
				ExecutionHelper.Execute (TestTarget.ToolPath, "--abi invalid-arm"),
				"error MT0015: Invalid ABI: invalid-arm. Supported ABIs are: i386, x86_64, armv7, armv7+llvm, armv7+llvm+thumb2, armv7s, armv7s+llvm, armv7s+llvm+thumb2, armv7k, armv7k+llvm, arm64 and arm64+llvm.\n");
		}

		[Test]
		public void MT0016 ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			var deprecated_list = new string [] {
				"-d={0}", "--dir={0}", "--cp", "--crossprefix", "--libdir", "-n", "--keeptemp", 
				"--main", "--nomanifest", "--mapinject", "--nosign", "--displayname", "--bundleid",
				"--mainnib", "--icon", "-c", "--certificate", "--enable-background-fetch",
				"--llvm", "--thumb", "--armv7", "--noregistrar", "--unsupported--enable-generics-in-registrar" 
			};


			try {
				var exe = CompileTestAppExecutable (testDir, profile: MTouch.Profile.Unified);
				var args = string.Format (string.Join (" ", deprecated_list), app);

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("--sdkroot {5} --dev {0} {1} -r:{2} {3} --sdk {4}", app, exe, Configuration.XamarinIOSDll, args, Configuration.sdk_version, Configuration.xcode_root), hide_output: false),
					"Xamarin.iOS .* using framework:.*\n" +
					"error MT0016: The option '--dir' has been deprecated.\n" +
					"error MT0016: The option '--dir' has been deprecated.\n" +
					"error MT0016: The option '--crossprefix' has been deprecated.\n" +
					"error MT0016: The option '--libdir' has been deprecated.\n" +
					"error MT0016: The option '-n' has been deprecated.\n" +
					"error MT0016: The option '--keeptemp' has been deprecated.\n" +
					"error MT0016: The option '--main' has been deprecated.\n" +
					"error MT0016: The option '--mapinject' has been deprecated.\n" +
					"error MT0016: The option '--nosign' has been deprecated.\n" +
					"error MT0016: The option '--displayname' has been deprecated.\n" +
					"error MT0016: The option '--mainnib' has been deprecated.\n" +
					"error MT0016: The option '--certificate' has been deprecated.\n" +
					"error MT0016: The option '--llvm' has been deprecated.\n" +
					"error MT0016: The option '--thumb' has been deprecated.\n" +
					"error MT0016: The option '--armv7' has been deprecated.\n" +
					"error MT0016: The option '--noregistrar' has been deprecated.\n" +
					"error MT0016: The option '--unsupported--enable-generics-in-registrar' has been deprecated.\n");
			} finally {
				Directory.Delete (testDir, true);
			}

		}

		[Test]
		[TestCase (Profile.Unified, Profile.TVOS)]
		[TestCase (Profile.Unified, Profile.WatchOS)]
		[TestCase (Profile.TVOS, Profile.Unified)]
		[TestCase (Profile.TVOS, Profile.WatchOS)]
		[TestCase (Profile.WatchOS, Profile.Unified)]
		[TestCase (Profile.WatchOS, Profile.TVOS)]
		public void MT0041 (Profile profile, Profile other)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = profile;
				mtouch.CreateTemporaryApp ();
				mtouch.References = new string [] {
					GetBaseLibrary (profile),
					GetBaseLibrary (other),
				};
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.BuildSim));
				mtouch.AssertError (41, string.Format ("Cannot reference '{0}' in a {1} app.", Path.GetFileName (GetBaseLibrary (other)), GetPlatformName (profile)));
			}
		}

		[Test]
		public void MT0073 ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileTestAppExecutable (testDir, profile: MTouch.Profile.Unified);

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --dev {0} -sdk " + Configuration.sdk_version + " --targetver 3.1 --abi=armv7s,arm64 {1} -debug -r:" + Configuration.XamarinIOSDll, app, exe)),
					"Xamarin.iOS .* using framework:.*\nerror MT0073: Xamarin.iOS .* does not support a deployment target of 3.1 for iOS .the minimum is 6.0.. Please select a newer deployment target in your project's Info.plist.\n");

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --dev {0} -sdk " + Configuration.sdk_version + " --targetver 3.1 --abi=armv7s {1} -debug -r:" + Configuration.XamarinIOSDll, app, exe)),
					"Xamarin.iOS .* using framework:.*\nerror MT0073: Xamarin.iOS .* does not support a deployment target of 3.1 for iOS .the minimum is 6.0.. Please select a newer deployment target in your project's Info.plist.\n");

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --dev {0} -sdk " + Configuration.sdk_version + " --targetver 5.1 --abi=arm64 {1} -debug -r:" + Configuration.XamarinIOSDll, app, exe)),
					"Xamarin.iOS .* using framework:.*\nerror MT0073: Xamarin.iOS .* does not support a deployment target of 5.1 for iOS .the minimum is 6.0.. Please select a newer deployment target in your project's Info.plist.\n");

				// No exception here.
				ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --dev {0} -sdk " + Configuration.sdk_version + " --targetver 6.0 --abi=arm64 {1} -debug -r:" + Configuration.XamarinIOSDll, app, exe));

			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void MT0074 ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileTestAppExecutable (testDir, profile: MTouch.Profile.Unified);

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --dev {0} -sdk " + Configuration.sdk_version + " --targetver 400.0.0 --abi=armv7s,arm64 {1} -debug -r:" + Configuration.XamarinIOSDll, app, exe)),
					string.Format ("Xamarin.iOS .* using framework:.*\nerror MT0074: Xamarin.iOS .* does not support a deployment target of 400.0.0 for iOS .the maximum is " + Configuration.sdk_version + ".. Please select an older deployment target in your project's Info.plist or upgrade to a newer version of Xamarin.iOS.\n", Configuration.sdk_version));
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void MT0048 ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileTestAppExecutable (testDir, profile: MTouch.Profile.Unified);

				Asserts.ThrowsPattern<TestExecutionException> (() => ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --dev {0} -sdk " + Configuration.sdk_version + " --targetver 6.0 --abi=armv7s {1} -debug -r:{2} --crashreporting-api-key APIKEY", app, exe, Configuration.XamarinIOSDll), hide_output: false),
					"error MT0016: The option '--crashreporting-api-key' has been deprecated.");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		[TestCase (Profile.Unified, Profile.TVOS)]
		[TestCase (Profile.Unified, Profile.WatchOS)]
		[TestCase (Profile.TVOS, Profile.Unified)]
		[TestCase (Profile.TVOS, Profile.WatchOS)]
		[TestCase (Profile.WatchOS, Profile.Unified)]
		[TestCase (Profile.WatchOS, Profile.TVOS)]
		public void MT0034 (Profile exe_profile, Profile dll_profile)
		{
			using (var mtouch = new MTouchTool ()) {
				var app = mtouch.CreateTemporaryAppDirectory ();
				var testDir = Path.GetDirectoryName (app);
			
				string exe = Path.Combine (testDir, "testApp.exe");
				string dll = Path.Combine (testDir, "testLib.dll");

				var dllCode = @"public class TestLib {
	public TestLib ()
	{
		System.Console.WriteLine (typeof (Foundation.NSObject).ToString ());
	}
}";

				var exeCode = @"public class TestApp { 
	static void Main () 
	{
		System.Console.WriteLine (typeof (Foundation.NSObject).ToString ());
		System.Console.WriteLine (new TestLib ());
	}
}";
				
				CompileCSharpCode (dll_profile, dllCode, dll);
				CompileCSharpCode (exe_profile, exeCode, exe, "-r:" + dll);

				mtouch.Profile = exe_profile;
				mtouch.Executable = exe;
				mtouch.References = new string [] { GetBaseLibrary (exe_profile) };
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.BuildSim), "build");
				var dllBase = Path.GetFileName (GetBaseLibrary (dll_profile));
				mtouch.AssertError (34, string.Format ("Cannot reference '{0}' in a {1} project - it is implicitly referenced by 'testLib, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'.", dllBase, GetPlatformName (exe_profile)));
			}
		}

		[Test]
		public void MT0020 ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileTestAppExecutable (testDir, profile: MTouch.Profile.Unified);

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --dev {0} -sdk " + Configuration.sdk_version + " --registrar:oldstatic --targetver 6.0 --abi=arm64 {1} -debug -r:{2} ", app, exe, Configuration.XamarinIOSDll)),
					"error MT0020: The valid options for '--registrar' are 'static, dynamic or default'.\n");

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --sim {0} -sdk " + Configuration.sdk_version + " --registrar:olddynamic --targetver 6.0 --abi=x86_64 {1} -debug -r:{2}", app, exe, Configuration.XamarinIOSDll)),
					"error MT0020: The valid options for '--registrar' are 'static, dynamic or default'.\n");
					
				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --sim {0} -sdk " + Configuration.sdk_version + " --registrar:legacy --targetver 6.0 --abi=x86_64 {1} -debug -r:{2}", app, exe, Configuration.XamarinIOSDll)),
					"error MT0020: The valid options for '--registrar' are 'static, dynamic or default'.\n");

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --sim {0} -sdk " + Configuration.sdk_version + " --registrar:legacystatic --targetver 6.0 --abi=x86_64 {1} -debug -r:{2}", app, exe, Configuration.XamarinIOSDll)),
					"error MT0020: The valid options for '--registrar' are 'static, dynamic or default'.\n");

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --sim {0} -sdk " + Configuration.sdk_version + " --registrar:legacydynamic --targetver 6.0 --abi=x86_64 {1} -debug -r:{2}", app, exe, Configuration.XamarinIOSDll)),
					"error MT0020: The valid options for '--registrar' are 'static, dynamic or default'.\n");
			} finally {
				Directory.Delete (testDir, true);
			}
		}
			
		[Test]
		public void MT0051 ()
		{
			if (Directory.Exists ("/Applications/Xcode44.app/Contents/Developer")) {
				Asserts.ThrowsPattern<TestExecutionException> (() => {
					ExecutionHelper.Execute (TestTarget.ToolPath, "-sdkroot /Applications/Xcode44.app/Contents/Developer -sim /tmp/foo");
				}, "error MT0051: Xamarin.iOS .* requires Xcode 6.0 or later. The current Xcode version [(]found in /Applications/Xcode44.app/Contents/Developer[)] is 4.*");
			}

			if (Directory.Exists (Configuration.xcode5_root)) {
				Asserts.ThrowsPattern<TestExecutionException> (() => {
					ExecutionHelper.Execute (TestTarget.ToolPath, "-sdkroot /Applications/Xcode511.app/Contents/Developer -sim /tmp/foo");
				}, "error MT0051: Xamarin.iOS .* requires Xcode 6.0 or later. The current Xcode version [(]found in " + Configuration.xcode5_root + "[)] is 6.0");
			}
		}

		[Test]
		public void MT0055 ()
		{
			Asserts.ThrowsPattern<TestExecutionException> (() => {
				ExecutionHelper.Execute (TestTarget.ToolPath, "--sdkroot /dir/that/does/not/exist");
			}, "error MT0055: The Xcode path '/dir/that/does/not/exist' does not exist.");
		}

		[Test]
		public void MT0060 ()
		{
			var msg = string.Empty;
			if (!Directory.Exists ("/Applications/Xcode.app")) {
				msg = "warning MT0060: Could not find the currently selected Xcode on the system. 'xcode-select --print-path' returned '/dir/that/does/not/exist', but that directory does not exist.\n" +
				"error MT0056: Cannot find Xcode in the default location ./Applications/Xcode.app.. Please install Xcode, or pass a custom path using --sdkroot <path>.\n";
			} else {
				msg = "warning MT0060: Could not find the currently selected Xcode on the system. 'xcode-select --print-path' returned '/dir/that/does/not/exist', but that directory does not exist.\n" +
				"warning MT0062: No Xcode.app specified .using --sdkroot or 'xcode-select --print-path'., using the default Xcode instead: /Applications/Xcode.app\n" +
				"Xamarin.iOS .* using framework: .*\n" +
				"error MT0052: No command specified.";
			}

			Asserts.ThrowsPattern<TestExecutionException> (() => {
				var envvars = new Dictionary<string, string> 
				{
					{ "DEVELOPER_DIR", "/dir/that/does/not/exist" }
				};
				ExecutionHelper.Execute (TestTarget.ToolPath, "", environmentVariables: envvars);
			},  msg);
		}

		[Test]
		public void MT0061 ()
		{
			// The MT0070 warning depends on system configuration, so it's optional in the regexp
			Asserts.ThrowsPattern<TestExecutionException> (() => {
				ExecutionHelper.Execute (TestTarget.ToolPath, "");
			}, "warning MT0061: No Xcode.app specified .using --sdkroot., using the system Xcode as reported by 'xcode-select --print-path': .*\n" +
				"(warning MT0078: The recommended Xcode version for Xamarin.iOS [0-9.]* is Xcode [0-9.]* or later. The current Xcode version .found in .* is .*)?\\s?" +
				"Xamarin.iOS .* using framework: .*\n" +
				"error MT0052: No command specified.");
		}

		public void MT0062 ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileUnifiedTestAppExecutable (testDir);

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot {3} --sim {0} -sdk {4} --targetver 7.1 --framework /foo/bar/zap.framework -r:{2} {1}", app, exe, Configuration.XamarinIOSDll, Configuration.xcode_root, Configuration.sdk_version), hide_output: false),
					"Xamarin.iOS .* using framework:.*\nerror MT0062: Xamarin.iOS only supports embedded frameworks when deployment target is at least 8.0 .current deployment target: '7.1'; embedded frameworks: '/foo/bar/zap.framework'.\n");

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot {3} --sim {0} -sdk {4} --targetver 7.1 --mono:framework -r:{2} {1}", app, exe, Configuration.XamarinIOSDll, Configuration.xcode_root, Configuration.sdk_version), hide_output: false),
					"Xamarin.iOS .* using framework:.*\nerror MT0062: Xamarin.iOS only supports embedded frameworks when deployment target is at least 8.0 .current deployment target: '7.1'; embedded frameworks: '.*/Mono.framework'.\n");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void MT0075 ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileTestAppExecutable (testDir);

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot {3} --dev {0} -sdk {4} -r:{2} {1} --abi armv7k", app, exe, Configuration.XamarinIOSDll, Configuration.xcode_root, Configuration.sdk_version), hide_output: false),
					"Xamarin.iOS .* using framework:.*\n" +
					"error MT0075: Invalid architecture 'ARMv7k' for iOS projects. Valid architectures are: ARMv7, ARMv7.Thumb, ARMv7.LLVM, ARMv7.LLVM.Thumb, ARMv7s, ARMv7s.Thumb, ARMv7s.LLVM, ARMv7s.LLVM.Thumb");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void MT0076 ()
		{
			if (!Configuration.include_watchos || !Configuration.include_tvos)
				Assert.Ignore ("This test requires WatchOS and TVOS to be enabled.");

			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileTestAppExecutable (testDir, profile: MTouch.Profile.WatchOS);

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot {3} --dev {0} -sdk {4} -r:{2} {1} --target-framework Xamarin.WatchOS,v1.0", app, exe, Configuration.XamarinWatchOSDll, Configuration.xcode_root, Configuration.watchos_sdk_version), hide_output: false),
					"error MT0076: No architecture specified .using the --abi argument.. An architecture is required for Xamarin.WatchOS projects.");
				
				exe = CompileTestAppExecutable (testDir, profile: MTouch.Profile.TVOS);

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot {3} --dev {0} -sdk {4} -r:{2} {1} --target-framework Xamarin.TVOS,v1.0", app, exe, Configuration.XamarinTVOSDll, Configuration.xcode_root, Configuration.tvos_sdk_version), hide_output: false),
					"error MT0076: No architecture specified .using the --abi argument.. An architecture is required for Xamarin.TVOS projects.");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void MT0077 ()
		{
			if (!Configuration.include_watchos)
				Assert.Ignore ("This test requires WatchOS and TVOS to be enabled.");
			
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileTestAppExecutable (testDir, profile: MTouch.Profile.WatchOS);

				Asserts.ThrowsPattern<TestExecutionException> (() =>
					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot {3} --dev {0} -sdk {4} -r:{2} {1} --target-framework Xamarin.WatchOS,v1.0 --abi armv7k", app, exe, Configuration.XamarinWatchOSDll, Configuration.xcode_root, Configuration.watchos_sdk_version), hide_output: false),
					"error MT0077: WatchOS projects must be extensions.");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		[TestCase (Profile.TVOS)]
		//[TestCase (Profile.WatchOS)] MT0077 interferring.
		[TestCase (Profile.Unified)]
		public void MT0085 (Profile profile)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = profile;
				mtouch.CreateTemporaryApp ();
				mtouch.TargetFramework = GetTargetFramework (profile);
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildSim));
				mtouch.AssertError (85, string.Format ("No reference to '{0}' was found. It will be added automatically.", Path.GetFileName (GetBaseLibrary (profile))));
			}
		}

		[Test]
		[TestCase (Profile.TVOS)]
		[TestCase (Profile.WatchOS)]
		public void MT0086 (Profile profile)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.References = new string [] { GetBaseLibrary (profile) };
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.BuildSim));
				mtouch.AssertError (86, "A target framework (--target-framework) must be specified when building for TVOS or WatchOS.");
			}
		}

		[Test]
		[TestCase (Profile.TVOS, "tvOS")]
		[TestCase (Profile.Unified, "iOS")]
		public void MT0091 (Profile profile, string name)
		{
			// Any old Xcode would do.
			if (!Directory.Exists (Configuration.xcode72_root))
				Assert.Ignore ("This test needs Xcode 7.0");
			
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = profile;
				mtouch.CreateTemporaryApp ();
				mtouch.SdkRoot = Configuration.xcode72_root;
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Sdk = "9.0";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.BuildSim));
				mtouch.AssertError (91, String.Format ("This version of Xamarin.iOS requires the {0} {1} SDK (shipped with Xcode {2}) when the managed linker is disabled. Either upgrade Xcode, or enable the managed linker.", name, GetSdkVersion (profile), Configuration.XcodeVersion));
			}
		}

		[Test]
		public void MT0096 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.NoPlatformAssemblyReference = true;
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.BuildSim));
				mtouch.AssertError (96, "No reference to Xamarin.iOS.dll was found.");
			}
		}

		[Test]
		public void ExtensionBuild ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileTestAppExecutable (testDir);
				File.WriteAllText (Path.Combine (app, "Info.plist"), @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
	<key>CFBundleDisplayName</key>
	<string>Extensiontest</string>
	<key>CFBundleIdentifier</key>
	<string>com.xamarin.extensiontest</string>
	<key>MinimumOSVersion</key>
	<string>6.0</string>
	<key>UIDeviceFamily</key>
	<array>
		<integer>1</integer>
		<integer>2</integer>
	</array>
	<key>UISupportedInterfaceOrientations</key>
	<array>
		<string>UIInterfaceOrientationPortrait</string>
		<string>UIInterfaceOrientationPortraitUpsideDown</string>
		<string>UIInterfaceOrientationLandscapeLeft</string>
		<string>UIInterfaceOrientationLandscapeRight</string>
	</array>
</dict>
</plist>
");
				ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --sim {0} -sdk {3} --targetver {3} --extension -r:{2} {1}", app, exe, Configuration.XamarinIOSDll, Configuration.sdk_version), hide_output: false);
				ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --dev {0} -sdk {3} --targetver {3} --extension -r:{2} {1}", app, exe, Configuration.XamarinIOSDll, Configuration.sdk_version), hide_output: false);
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		static string BindingsLibrary {
			get {
				return Path.Combine (Configuration.SourceRoot, "tests/bindings-test/bin/Debug/bindings-test.dll");
			}
		}

		static string GetBindingsLibrary (Profile profile)
		{
			var fn = Path.Combine (Configuration.SourceRoot, "tests", "bindings-test", "bin", "Any CPU", GetConfiguration (profile), "bindings-test.dll");

			if (!File.Exists (fn)) {
				var csproj = Path.Combine (Configuration.SourceRoot, "tests", "bindings-test", "bindings-test" + GetProjectSuffix (profile) + ".csproj");
				XBuild.Build (csproj, platform: "AnyCPU");
			}

			return fn;
		}

		public static string GetBaseLibrary (Profile profile)
		{
			switch (profile) {
			case Profile.Unified:
				return Configuration.XamarinIOSDll;
			case Profile.TVOS:
				return Configuration.XamarinTVOSDll;
			case Profile.WatchOS:
				return Configuration.XamarinWatchOSDll;
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetCompiler (Profile profile, StringBuilder args)
		{
			args.Append (" -lib:").Append (Path.GetDirectoryName (GetBaseLibrary (profile))).Append (' ');
			return "/Library/Frameworks/Mono.framework/Commands/mcs";
		}

		static string GetConfiguration (Profile profile)
		{
			switch (profile) {
			case Profile.Unified:
				return "Debug-unified";
			case Profile.TVOS:
				return "Debug-tvos";
			case Profile.WatchOS:
				return "Debug-watchos";
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetTargetFramework (Profile profile)
		{
			switch (profile) {
			case Profile.Unified:
				return "Xamarin.iOS,v1.0";
			case Profile.TVOS:
				return "Xamarin.TVOS,v1.0";
			case Profile.WatchOS:
				return "Xamarin.WatchOS,v1.0";
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetDeviceArchitecture (Profile profile)
		{
			switch (profile) {
			case Profile.Unified:
				return "armv7";
			case Profile.TVOS:
				return "arm64";
			case Profile.WatchOS:
				return "armv7k";
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetSimulatorArchitecture (Profile profile)
		{
			switch (profile) {
			case Profile.Unified:
			case Profile.WatchOS:
				return "i386";
			case Profile.TVOS:
				return "x86_64";
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetArchitecture (Profile profile, Target target)
		{
			return target == Target.Dev ? GetDeviceArchitecture (profile) : GetSimulatorArchitecture (profile);
		}

		static string GetPlatformName (Profile profile)
		{
			switch (profile) {
			case Profile.Unified:
				return "Xamarin.iOS";
			case Profile.TVOS:
				return "Xamarin.TVOS";
			case Profile.WatchOS:
				return "Xamarin.WatchOS";
			default:
				throw new NotImplementedException ();
			}
		}

		static string GetProjectSuffix (Profile profile)
		{
			switch (profile) {
			case Profile.Unified:
				return string.Empty;
			case Profile.TVOS:
				return "-tvos";
			case Profile.WatchOS:
				return "-watchos";
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetSdkVersion (Profile profile)
		{
			switch (profile) {
			case Profile.Unified:
				return Configuration.sdk_version;
			case Profile.TVOS:
				return Configuration.tvos_sdk_version;
			case Profile.WatchOS:
				return Configuration.watchos_sdk_version;
			default:
				throw new NotImplementedException ();
			}
		}

		[Test]
		public void LinkAll_Frameworks ()
		{
			// Make sure that mtouch does not link with unused frameworks.

			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileTestAppExecutable (testDir);
				var bin = Path.Combine (app, Path.GetFileNameWithoutExtension (exe));

				ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + "  -v -v -v --sim {0} -sdk " + Configuration.sdk_version + " {1} -debug -r:{2}", app, exe, Configuration.XamarinIOSDll));

				var load_commands = ExecutionHelper.Execute ("otool", "-l \"" + bin + "\"");
				Asserts.DoesNotContain ("SafariServices", load_commands, "SafariServices");
				Asserts.DoesNotContain ("GameController", load_commands, "GameController");
				Asserts.DoesNotContain ("QuickLook", load_commands, "QuickLook");
				Asserts.DoesNotContain ("NewsstandKit", load_commands, "NewsstandKit");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		[TestCase (Profile.Unified)]
		[TestCase (Profile.TVOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_LinkWithTest (Profile profile)
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				// --fastdev with static registrar and linkwith library - this will fail to build if the linkwith dylib isn't linked with the corresponding native library.
				var mtouch = new MTouchTool ()
				{
					Profile = profile,
					Debug = true,
					FastDev = true,
					References = new string [] { GetBindingsLibrary (profile) },
					Executable = CompileTestAppExecutableLinkWith (testDir, profile),
					AppPath = app,
					NoFastSim = true,
					Registrar = MTouchRegistrar.Static,
				};
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildDev), "build");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		[TestCase (Profile.Unified)]
		[TestCase (Profile.TVOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_NoFastSim_NoLink (Profile profile)
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				// --sim --nofastsim --nolink --fastdev
				var mtouch = new MTouchTool ()
				{
					Profile = profile,
					Debug = true,
					FastDev = true,
					References = new string [] { GetBindingsLibrary (profile) },
					Executable = CompileTestAppExecutableLinkWith (testDir, profile),
					AppPath = app,
					NoFastSim = true,
					Linker = MTouchLinker.DontLink,
				};
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildSim), "build");
			} finally {
				Directory.Delete (testDir, true);
			}
		}
		
		[Test]
		[TestCase (Profile.Unified)]
		[TestCase (Profile.TVOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_NoFastSim_LinkAll (Profile profile)
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				// --sim --nofastsim --fastdev
				var mtouch = new MTouchTool ()
				{
					Profile = profile,
					Debug = true,
					FastDev = true,
					References = new string [] { GetBindingsLibrary (profile) },
					Executable = CompileTestAppExecutableLinkWith (testDir, profile),
					AppPath = app,
					NoFastSim = true,
				};
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildSim), "build");
			} finally {
				Directory.Delete (testDir, true);
			}
		}
		
		[Test]
		[TestCase (Profile.Unified)]
		[TestCase (Profile.TVOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_NoFastSim_LinkSDK (Profile profile)
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				// --sim --nofastsim --linksdkonly --fastdev
				var mtouch = new MTouchTool ()
				{
					Profile = profile,
					Debug = true,
					FastDev = true,
					References = new string [] { GetBindingsLibrary (profile) },
					Linker = MTouchLinker.LinkSdk,
					Executable = CompileTestAppExecutableLinkWith (testDir, profile),
					AppPath = app,
					NoFastSim = true,
				};
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildSim), "build");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		[TestCase (Profile.Unified)]
		[TestCase (Profile.TVOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_Sim (Profile profile)
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				// --sim --fastdev
				var mtouch = new MTouchTool ()
				{
					Profile = profile,
					Debug = true,
					FastDev = true,
					References = new string [] { GetBindingsLibrary (profile) },
					Executable = CompileTestAppExecutableLinkWith (testDir, profile),
					AppPath = app,
				};
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildSim), "build");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		[TestCase (Profile.Unified)]
		[TestCase (Profile.TVOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_LinkAll (Profile profile)
		{
			using (var mtouch = new MTouchTool ()
			{
				Profile = profile,
				Debug = true,
				FastDev = true,
				References = new string [] { GetBindingsLibrary (profile) },
			}) {
				mtouch.CreateTemporaryApp_LinkWith ();
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildDev), "build");
			}
		}
		
		[Test]
		[TestCase (Profile.Unified)]
		[TestCase (Profile.TVOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_NoLink (Profile profile)
		{

			// --fastdev w/no link
			using (var mtouch = new MTouchTool ()
			{
				Profile = profile,
				Debug = true,
				FastDev = true,
				References = new string [] { GetBindingsLibrary (profile) },
				Linker = MTouchLinker.DontLink,
			}) {
				mtouch.CreateTemporaryApp_LinkWith ();
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildDev), "build 1");
			}
		}
		
		[Test]
		[TestCase (Profile.Unified)]
		[TestCase (Profile.TVOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_LinkAll_Then_NoLink (Profile profile)
		{
			using (var mtouch = new MTouchTool
			{
				Profile = profile,
				Debug = true,
				FastDev = true,
				References = new string [] { GetBindingsLibrary (profile) },
			}) {
				mtouch.CreateTemporaryApp_LinkWith ();

				// --fastdev w/all link
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildDev), "build 1");

				// --fastdev w/no link
				mtouch.Linker = MTouchLinker.DontLink;
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildDev), "build 2");
			}
		}

		[Test]
		[TestCase (Profile.Unified)]
		[TestCase (Profile.TVOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_LinkSDK (Profile profile)
		{
			using (var mtouch = new MTouchTool
			{
				Profile = profile,
				Debug = true,
				FastDev = true,
				References = new string [] { GetBindingsLibrary (profile) },
				Linker = MTouchLinker.LinkSdk,
			}) {
				mtouch.CreateTemporaryApp_LinkWith ();

				// --fastdev w/sdk link
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildDev), "build");
			}
		}

		[Test]
		public void FastDev_Dual ()
		{
			using (var mtouch = new MTouchTool ()
			{
				Profile = Profile.Unified,
				FastDev = true,
				Abi = "armv7,arm64",
			}) {
				mtouch.CreateTemporaryApp ();

				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildDev));
				var bin = mtouch.NativeExecutablePath;
				VerifyArchitectures (bin, "arm7s/64", "armv7", "arm64");
				foreach (var dylib in Directory.GetFileSystemEntries (mtouch.AppPath, "*.dylib")) {
					if (Path.GetFileName (dylib).StartsWith ("libmono"))
						continue;
					if (Path.GetFileName (dylib).StartsWith ("libxamarin"))
						continue;
					VerifyArchitectures (dylib, dylib + ": arm7s/64", "armv7", "arm64");
				}
			}
		}

		[Test]
		[TestCase (Target.Dev, "armv7")]
		[TestCase (Target.Dev, "armv7s")]
		[TestCase (Target.Dev, "armv7,armv7s")]
		[TestCase (Target.Dev, "arm64")]
		[TestCase (Target.Dev, "arm64+llvm")]
		[TestCase (Target.Dev, "armv7,arm64")]
		[TestCase (Target.Dev, "armv7s,arm64")]
		[TestCase (Target.Dev, "armv7,armv7s,arm64")]
		[TestCase (Target.Sim, "i386")]
		[TestCase (Target.Sim, "x86_64")]
		public void Architectures_Unified (Target target, string abi)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.Unified;
				mtouch.CreateTemporaryApp ();

				mtouch.Abi = abi;

				var bin = Path.Combine (mtouch.AppPath, Path.GetFileNameWithoutExtension (mtouch.Executable));

				Assert.AreEqual (0, mtouch.Execute (target == Target.Dev ? MTouchAction.BuildDev : MTouchAction.BuildSim));

				VerifyArchitectures (bin, abi, abi.Replace ("+llvm", string.Empty).Split (','));
			}
		}

		[Test]
		public void Architectures_Unified_FatSimulator ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.Unified;
				mtouch.CreateTemporaryApp ();

				mtouch.Abi = "i386,x86_64";

				var bin = Path.Combine (mtouch.AppPath, Path.GetFileNameWithoutExtension (mtouch.Executable));
				var bin32 = Path.Combine (mtouch.AppPath, ".monotouch-32", Path.GetFileNameWithoutExtension (mtouch.Executable));
				var bin64 = Path.Combine (mtouch.AppPath, ".monotouch-64", Path.GetFileNameWithoutExtension (mtouch.Executable));

				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildSim));

				Assert.IsFalse (File.Exists (bin), "none");
				VerifyArchitectures (bin64, "64/x86_64", "x86_64");
				VerifyArchitectures (bin32, "32/i386", "i386");
			}
		}

		[Test]
		public void Architectures_Unified_Invalid ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.Unified;
				mtouch.CreateTemporaryApp ();

				mtouch.Abi = "armv6";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.BuildDev));
				mtouch.AssertError ("MT", 15, "Invalid ABI: armv6. Supported ABIs are: i386, x86_64, armv7, armv7+llvm, armv7+llvm+thumb2, armv7s, armv7s+llvm, armv7s+llvm+thumb2, armv7k, armv7k+llvm, arm64 and arm64+llvm.");

				mtouch.Abi = "armv7";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.BuildSim));
				mtouch.AssertError ("MT", 75, "Invalid architecture 'ARMv7' for iOS projects. Valid architectures are: i386, x86_64");
			}
		}

		[Test]
		[TestCase (Target.Dev, null)]
		[TestCase (Target.Dev, "arm64+llvm")]
		[TestCase (Target.Sim, null)]
		public void Architectures_TVOS (Target target, string abi)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = MTouch.Profile.TVOS;
				mtouch.Abi = abi;
				mtouch.CreateTemporaryApp ();
				      
				var bin = Path.Combine (mtouch.AppPath, Path.GetFileNameWithoutExtension (mtouch.Executable));

				Assert.AreEqual (0, mtouch.Execute (target == Target.Dev ? MTouchAction.BuildDev : MTouchAction.BuildSim), "build");
				VerifyArchitectures (bin,  "arch",  target == Target.Dev ? "arm64" : "x86_64");
			}
		}

		[Test]
		public void Architectures_TVOS_Invalid ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.TVOS;
				mtouch.CreateTemporaryApp ();

				mtouch.Abi = "armv7";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.BuildDev), "device - armv7");
				mtouch.AssertError ("MT", 75, "Invalid architecture 'ARMv7' for TVOS projects. Valid architectures are: ARM64, ARM64+LLVM");
			}
		}

		[Test]
		public void GarbageCollectors ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");

			Directory.CreateDirectory (app);
			try {
				var code = "public class TestApp { static void Main () { System.Console.WriteLine (typeof (UIKit.UIWindow).ToString ()); } }";
				var exe = CompileTestAppExecutable (testDir, code: code, profile: MTouch.Profile.Unified);
				var bin = Path.Combine (app, Path.GetFileNameWithoutExtension (exe));
				var common_args = string.Format ("-sdkroot " + Configuration.xcode_root + " --sim {0} -sdk " + Configuration.sdk_version + " --targetver 6.0 --abi=i386 {1} -debug -gcc_flags -Wl,-w ", app, exe);
				var newstyle_args = common_args + "-r:" + Configuration.XamarinIOSDll;
				ExecutionHelper.Execute (TestTarget.ToolPath, newstyle_args);
				VerifyGC (bin, false, "dual/default");

				ExecutionHelper.Execute (TestTarget.ToolPath, newstyle_args + " --sgen");
				VerifyGC (bin, false, "dual/sgen");

				var output = ExecutionHelper.Execute (TestTarget.ToolPath, newstyle_args + " --boehm");
				VerifyGC (bin, false, "dual/boehm");
				VerifyOutput ("Test", output, 
					"Xamarin.iOS .* using framework:.*",
					"warning MT0043: The Boehm garbage collector is not supported. The SGen garbage collector has been selected instead.",
					".*testApp.app built successfully.");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		void ExecuteWithStats (string binary, string arguments)
		{
			ExecutionHelper.Execute (TestTarget.ToolPath, arguments);
			var fi = new FileInfo (binary);
			Console.WriteLine ("Binary Size: {0} bytes = {1} kb", fi.Length, fi.Length / 1024);
		}

		string ReplaceExtraArgs (string contents, string replace)
		{
			return ReplaceCsprojData (contents, "MtouchExtraArgs", replace);
		}

		string ReplaceCompilerDefines (string contents, string replace)
		{
			return ReplaceCsprojData (contents, "DefineConstants", replace);
		}

		string ReplaceCsprojData (string contents, string key, string replace)
		{
			int idx = 0;
			while (true) {
				var start = contents.IndexOf ("<" + key + ">", idx);
				if (start == -1)
					return contents;
				var end = contents.IndexOf("</" + key + ">", start);
				if (end == -1)
					return contents;
				contents = contents.Substring (0, start + ("<" + key + ">").Length) + replace + contents.Substring (end);
				idx = end;
			}
		}

		static string MDToolPath {
			get {
				return "/Applications/Xamarin Studio.app/Contents/MacOS/mdtool";
			}
		}

		[Test]
		[TestCase (Target.Dev, Profile.Unified, "dont link", "Release")]
		[TestCase (Target.Dev, Profile.Unified, "link all", "Release")]
		[TestCase (Target.Dev, Profile.Unified, "link sdk", "Release")]
		[TestCase (Target.Dev, Profile.Unified, "monotouch-test", "Release")]
		[TestCase (Target.Dev, Profile.Unified, "mscorlib", "Release")]
		[TestCase (Target.Dev, Profile.Unified, "System.Core", "Release")]
		public void BuildTestProject (Target target, Profile profile, string testname, string configuration)
		{
			var subdir = string.Empty;
			switch (testname) {
			case "dont link":
			case "link sdk":
			case "link all":
				subdir = "/linker-ios";
				break;
			case "monotouch-test":
				break;
			default:
				subdir = "/bcl-test";
				break;
			}
			var platform = target == Target.Dev ? "iPhone" : "iPhoneSimulator";
			var csproj = Path.Combine (Configuration.SourceRoot, "tests" + subdir, testname, testname + GetProjectSuffix (profile) + ".csproj");
			XBuild.Build (csproj, configuration, platform);
		}

		[Test]
		public void ScriptedTests ()
		{
			ExecutionHelper.Execute ("make", string.Format ("-C \"{0}\"", Path.Combine (Configuration.SourceRoot, "tests", "scripted")));
		}

		[Test]
		public void Registrar ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);
			
			try {
				var exe = CompileTestAppExecutable (testDir);
				var bin = Path.Combine (app, Path.GetFileNameWithoutExtension (exe));
				var common_args = string.Format ("-sdkroot " + Configuration.xcode_root + " --dev {0} -sdk {2} {1} -debug -r:{3}", app, exe, Configuration.sdk_version, Configuration.XamarinIOSDll);

				// fully linked + llvm (+thumb) + default registrar (currently llvm fails to build with clang, so we should transparently switch to gcc in this case)
				ExecuteWithStats (bin, common_args + " --registrar:static --abi:armv7+llvm");
				ExecuteWithStats (bin, common_args + " --registrar:static --abi:armv7+llvm+thumb2");
				
				// non-linked device build
				ExecuteWithStats (bin, common_args + " --compiler:clang --nolink --registrar:static");
				ExecuteWithStats (bin, common_args + " --compiler:clang --nolink --registrar:dynamic");

				// sdk device build
				ExecuteWithStats (bin, common_args + " --compiler:clang --linksdkonly --registrar:static");
				ExecuteWithStats (bin, common_args + " --compiler:clang --linksdkonly --registrar:dynamic");

				// fully linked device build
				ExecuteWithStats (bin, common_args + " --compiler:clang --registrar:static");
				ExecuteWithStats (bin, common_args + " --compiler:clang --registrar:dynamic");

				// non-linked device build
				common_args = string.Format ("-sdkroot " + Configuration.xcode_root + " --sim {0} -sdk {2} {1} -debug -r:{3}", app, exe, Configuration.sdk_version, Configuration.XamarinIOSDll);
				ExecuteWithStats (bin, common_args + " --compiler:clang --nolink --registrar:static");
				ExecuteWithStats (bin, common_args + " --compiler:clang --nolink --registrar:dynamic");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		[TestCase ("")]
		[TestCase ("-nolink")]
		[TestCase ("-linksdkonly")]
		public void ExportedSymbols (string linker_flag)
		{
			//
			// Here we test that symbols P/Invokes and [Field] attributes references are not
			// stripped by the native linker. mtouch has to pass '-u _SYMBOL' to the native linker
			// for this to work.
			//

			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			var cache = Path.Combine (testDir, "cache");
			Directory.CreateDirectory (app);

			try {
				var nativeCode = @"
void DummyMethod () {}
int dummy_field = 0;
";
				// var nativeLib = CompileNativeLibrary (testDir, nativeCode);
				var extraCode = @"
public class BindingApp {
	[Foundation.Field (""dummy_field"", ""__Internal"")]
	public static string DummyField { get { return null; } }

	[System.Runtime.InteropServices.DllImport (""__Internal"")]
	public static extern void DummyMethod ();
}
";
				var bindingLib = CreateBindingLibrary (testDir, nativeCode, null, null, extraCode);
				var exe = CompileTestAppExecutable (testDir, @"
public class TestApp { 
	static void Main () {
		System.Console.WriteLine (typeof (UIKit.UIWindow).ToString ());
		System.Console.WriteLine (BindingApp.DummyField);
		BindingApp.DummyMethod ();
	}
}
",
					"-r:" + bindingLib);
				var bin = Path.Combine (app, Path.GetFileNameWithoutExtension (exe));
				var args = string.Format ("-sdkroot {4} -sdk {2} {1} -debug --cache {0} -v -v -v -v -r:{3} -dev {5} -r:{6} " + linker_flag, 
					cache, exe, Configuration.sdk_version, bindingLib, Configuration.xcode_root, app, Configuration.XamarinIOSDll);
				
				// each variation is tested twice so that we don't break when everything is found in the cache the second time around.

				ExecutionHelper.Execute (TestTarget.ToolPath, args);
				var symbols = ExecutionHelper.Execute ("nm", bin, hide_output: true).Split ('\n');
				Assert.That (symbols, Has.Some.EndsWith (" S _dummy_field"), "Field not found in initial build");
				Assert.That (symbols, Has.Some.EndsWith (" T _DummyMethod"), "P/invoke not found in initial build");

				ExecutionHelper.Execute ("touch", bindingLib); // This will make it so that the second identical variation won't skip the final link step.
				ExecutionHelper.Execute (TestTarget.ToolPath, args);
				symbols = ExecutionHelper.Execute ("nm", bin, hide_output: true).Split ('\n');
				Assert.That (symbols, Has.Some.EndsWith (" S _dummy_field"), "Field not found in second build");
				Assert.That (symbols, Has.Some.EndsWith (" T _DummyMethod"), "P/invoke not found in second build");
			} finally {
				Directory.Delete (testDir, true);
			}
		}


		[Test]
		public void ExportedSymbols_VerifyLinkedAwayField ()
		{
			//
			// Here we test that unused P/Invokes and [Field] members are properly linked away
			// (and we do not request the native linker to preserve those symbols).
			//

			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			var cache = Path.Combine (testDir, "cache");

			Directory.CreateDirectory (app);

			try {
				var nativeCode = @"
void DummyMethod () {}
int dummy_field = 0;
";
				// var nativeLib = CompileNativeLibrary (testDir, nativeCode);
				var extraCode = @"
public class BindingApp {
	[Foundation.Field (""dummy_field"", ""__Internal"")]
	public static string DummyField { get { return null; } }

	[System.Runtime.InteropServices.DllImport (""__Internal"")]
	public static extern void DummyMethod ();
}
";
				var bindingLib = CreateBindingLibrary (testDir, nativeCode, null, null, extraCode);
				var exe = CompileTestAppExecutable (testDir, @"
public class TestApp { 
	static void Main () {
		System.Console.WriteLine (typeof (UIKit.UIWindow).ToString ());
	}
}
",
					"-r:" + bindingLib);
				var bin = Path.Combine (app, Path.GetFileNameWithoutExtension (exe));
				var common_args = string.Format ("-sdkroot " + Configuration.xcode_root + " -sdk {2} {1} -debug --cache {0} -v -v -v -v --registrar:static -r:{3} -r:{4}", 
					cache, exe, Configuration.sdk_version, bindingLib, Configuration.XamarinIOSDll);

				var variations = new string [] {
					// each variation is tested twice so that we don't break when everything is found in the cache the second time around.
					" -dev {0}",
					" -dev {0}",
				};

				for (int v = 0; v < variations.Length; v++) {
					var variation = variations [v];

					ExecutionHelper.Execute ("touch", bindingLib); // This will make it so that the second identical variation won't skip the final link step.

					ExecutionHelper.Execute (TestTarget.ToolPath, string.Format (common_args + variation, app));
					var lines = ExecutionHelper.Execute ("nm", bin, hide_output: true).Split ('\n');
					var found_field = false;
					var found_pinvoke = false;
					foreach (var line in lines) {
						found_field |= line.EndsWith (" S _dummy_field");
						found_pinvoke |= line.EndsWith (" T _DummyMethod");
						if (found_field && found_pinvoke)
							break;
					}

					Assert.IsFalse (found_field, string.Format ("Field found for variation #{0}: {1}", v, variation));
					Assert.IsFalse (found_field, string.Format ("P/Invoke found for variation #{0}: {1}", v, variation));
				}
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void LinkerWarnings ()
		{
			string output;
			var testDir = GetTempDirectory ();

			try {
				var app = Path.Combine (testDir, "testApp.app");
				Directory.CreateDirectory (app);

				var exe = CompileTestAppExecutable (testDir);

				output = ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot {2} --dev {0} -sdk {3} --force --abi=armv7,armv7s {1} -debug -r:{4}", app, exe, Configuration.xcode_root, Configuration.sdk_version, Configuration.XamarinIOSDll));
				Asserts.DoesNotContain ("ld: warning:", output, "#a");

				output = ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot {2} --dev {0} -sdk {3} --force --abi=armv7        {1} -debug -r:{5} --gcc_flags={4}", app, exe, Configuration.xcode_root, Configuration.sdk_version, Quote (Path.Combine (Configuration.SourceRoot, "tests/test-libraries/.libs/ios/libtest.armv7s.a")), Configuration.XamarinIOSDll));
				Asserts.Contains ("libtest.armv7s.a, file was built for archive which is not the architecture being linked (armv7)", output, "#b");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void NativeLinker_AllLoad ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=17199

			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileTestAppExecutable (testDir);

				ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("-sdkroot " + Configuration.xcode_root + " --dev {0} -sdk " + Configuration.sdk_version + " --targetver 7.0 --abi=armv7s {1} -debug --gcc_flags -all_load -r:{2}", app, exe, Configuration.XamarinIOSDll));

			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void CachedManagedLinker ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=17506

			var testDir = GetTempDirectory ();

			foreach (var linker in new string [] { "", "--linksdkonly", "--nolink" }) {
				try {
					var app = Path.Combine (testDir, "testApp.app");
					Directory.CreateDirectory (app);

					var exe = CompileTestAppExecutable (testDir);
					var cache = Path.Combine (testDir, "mtouch-cache");

					var args = string.Format ("{3} -sdkroot " + Configuration.xcode_root + " --dev {0} -sdk " + Configuration.sdk_version + " --targetver 7.0 --abi=armv7 {1} --cache={2} -r:{4}", app, exe, cache, linker, Configuration.XamarinIOSDll);
					ExecutionHelper.Execute (TestTarget.ToolPath, args);
					File.Delete (Path.Combine (app, "testApp")); // This will force the final native link to succeed, while everything before has been cached.
					ExecutionHelper.Execute (TestTarget.ToolPath, args);
				} finally {
					Directory.Delete (testDir, true);
				}
			}
		}

		[Test]
		public void MT1015 ()
		{
			// BXC 18659

			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");

			try {
				Directory.CreateDirectory (Path.Combine (app, "testApp"));

				var exe = CompileTestAppExecutable (testDir);
				var cache = Path.Combine (testDir, "mtouch-cache");

				var args = string.Format ("-sdkroot " + Configuration.xcode_root + " --debug --nolink --sim {0} -sdk " + Configuration.sdk_version + " --abi=i386 {1} --cache={2} --r:{3}", app, exe, cache, Configuration.XamarinIOSDll);
				Asserts.ThrowsPattern<TestExecutionException> (() => ExecutionHelper.Execute (TestTarget.ToolPath, args, hide_output: false), 
					"Xamarin.iOS .* using framework:.*\nerror MT1015: Failed to create the executable '.*/testApp.app/testApp': .*/testApp.app/testApp is a directory\n");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void MT1016 ()
		{
			// #20607

			var testDir = GetTempDirectory ();

			try {
				var app = Path.Combine (testDir, "testApp.app");
				Directory.CreateDirectory (Path.Combine (app, "NOTICE"));

				var exe = CompileTestAppExecutable (testDir);
				var cache = Path.Combine (testDir, "mtouch-cache");

				var args = string.Format ("-sdkroot " + Configuration.xcode_root + " --nolink --dev {0} -sdk {4} -targetver {4} --abi=armv7 {1} --cache={2} --r:{3} ", app, exe, cache, Configuration.XamarinIOSDll, Configuration.sdk_version);
				Asserts.ThrowsPattern<TestExecutionException> (() => ExecutionHelper.Execute (TestTarget.ToolPath, args, hide_output: false), 
					"Xamarin.iOS .* using framework:.*\nerror MT1016: Failed to create the NOTICE file because a directory already exists with the same name.\n");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void MT1017 ()
		{
			// #20607

			var testDir = GetTempDirectory ();

			try {
				var app = Path.Combine (testDir, "testApp.app");
				Directory.CreateDirectory (app);
				File.WriteAllText (Path.Combine (app, "NOTICE"), "contents");
				var fi = new FileInfo (Path.Combine (app, "NOTICE"));
				fi.IsReadOnly = true;

				var exe = CompileTestAppExecutable (testDir);
				var cache = Path.Combine (testDir, "mtouch-cache");

				var args = string.Format ("-sdkroot " + Configuration.xcode_root + " --nolink --dev {0} -sdk {4} -targetver {4} --abi=armv7 {1} --cache={2} --r:{3}", app, exe, cache, Configuration.XamarinIOSDll, Configuration.sdk_version);
				Asserts.ThrowsPattern<TestExecutionException> (() => ExecutionHelper.Execute (TestTarget.ToolPath, args, hide_output: false), 
					"Xamarin.iOS .* using framework:.*\nerror MT1017: Failed to create the NOTICE file: Access to the path \".*/testApp.app/NOTICE\" is denied.\n");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void MT1202 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.AppPath = "/tmp";
				mtouch.Device = ":vX;";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.LaunchSim), "launch");
				mtouch.HasError ("MT", 1202, "Invalid simulator configuration: :vX;");
			}
		}

		[Test]
		public void MT1203 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.AppPath = "/tmp";
				mtouch.Device = ":v2;a";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.LaunchSim), "launch");
				mtouch.HasError ("MT", 1203, "Invalid simulator specification: a");
			}
		}

		[Test]
		public void MT1204 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.AppPath = "/tmp";
				mtouch.Device = ":v2;";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.LaunchSim), "launch");
				mtouch.HasError ("MT", 1204, "Invalid simulator specification '': runtime not specified.");
			}

			using (var mtouch = new MTouchTool ()) {
				mtouch.AppPath = "/tmp";
				mtouch.Device = ":v2;devicetype=1";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.LaunchSim), "launch");
				mtouch.HasError ("MT", 1204, "Invalid simulator specification 'devicetype=1': runtime not specified.");
			}
		}

		[Test]
		public void MT1205 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.AppPath = "/tmp";
				mtouch.Device = ":v2;runtime=1";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.LaunchSim), "launch");
				mtouch.HasError ("MT", 1205, "Invalid simulator specification 'runtime=1': device type not specified.");
			}
		}

		[Test]
		public void MT1206 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.AppPath = "/tmp";
				mtouch.Device = ":v2;runtime=1,devicetype=2";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.LaunchSim), "launch");
				mtouch.HasError ("MT", 1206, "Could not find the simulator runtime '1'.");
			}
		}

		[Test]
		public void MT1207 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.AppPath = "/tmp";
				mtouch.Device = ":v2;runtime=com.apple.CoreSimulator.SimRuntime.iOS-" + Configuration.sdk_version.Replace ('.', '-') + ",devicetype=2";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.LaunchSim), "launch");
				mtouch.HasError ("MT", 1207, "Could not find the simulator device type '2'.");
			}
		}

		// I don't know which --runtime values would cause MT1208, I always end up with MT1215 instead

		// I don't know which --device values would cause MT1209

		[Test]
		public void MT1210 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.AppPath = "/tmp";
				mtouch.Device = ":v2;a=1";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.LaunchSim), "launch");
				mtouch.HasError ("MT", 1210, "Invalid simulator specification: 'a=1', unknown key 'a'");
			}
		}

		[Test]
		public void MT1211 ()
		{
			Assert.Ignore ("There are no device types in the iOS 9 simulator that the 8.1 simulator (earliest simulator Xcode 7 can run) doesn't support, so there's no way to produce the MT1211 error");
			Asserts.Throws<TestExecutionException> (() => ExecutionHelper.Execute (TestTarget.ToolPath,"--sdkroot " + Configuration.xcode_root +  " --launchsim /path/to/somewhere --device=:v2;runtime=com.apple.CoreSimulator.SimRuntime.iOS-7-1,devicetype=com.apple.CoreSimulator.SimDeviceType.Apple-Watch-38mm"),
				"error MT1211: The simulator version '7.1' does not support the simulator type 'Resizable iPhone'\n");
		}

		// MT1213: unused
		// MT1214: unused
		// MT1215: unused

		[Test]
		public void MT1216 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.AppPath = "/tmp";
				mtouch.Device = ":v2;udid=unknown";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.LaunchSim), "launch");
				mtouch.HasError ("MT", 1216, "Could not find the simulator UDID 'unknown'.");
			}
		}

		[Test]
		public void MT5211 ()
		{
			var testDir = GetTempDirectory ();

			try {
				var app = Path.Combine (testDir, "testApp.app");
				Directory.CreateDirectory (app);

				var code = @"
using System;
using System.Runtime.InteropServices;
using UIKit;
using Foundation;

class Test {
	[Register (""Inexistent"", true)]
	public class Inexistent : NSObject {}

	public class Subexistent : Inexistent {	}

	static void Main ()
	{
		Console.WriteLine (typeof (Subexistent));
	}
}
";
				var exe = CompileTestAppExecutable (testDir, code, profile: MTouch.Profile.Unified);
				var cache = Path.Combine (testDir, "mtouch-cache");

				var mtouch = new MTouchTool ();

				Assert.AreEqual (1, mtouch.Execute ("-sdkroot {5} --dev {0} -sdk {4} -targetver {4} --abi=armv7,arm64 {1} --cache={2} --r:{3}", app, exe, cache, Configuration.XamarinIOSDll, Configuration.sdk_version, Configuration.xcode_root), "build failure expected");

				mtouch.AssertOutputPattern ("Undefined symbols for architecture arm64:");
				mtouch.AssertOutputPattern (".*_OBJC_METACLASS_._Inexistent., referenced from:.*");
				mtouch.AssertOutputPattern (".*_OBJC_METACLASS_._Test_Subexistent in registrar.arm64.o.*");
				mtouch.AssertOutputPattern (".*_OBJC_CLASS_._Inexistent., referenced from:.*");
				mtouch.AssertOutputPattern (".*_OBJC_CLASS_._Test_Subexistent in registrar.arm64.o.*");
				mtouch.AssertOutputPattern (".*objc-class-ref in registrar.arm64.o.*");
				mtouch.AssertOutputPattern (".*ld: symbol.s. not found for architecture arm64.*");
				mtouch.AssertOutputPattern (".*clang: error: linker command failed with exit code 1 .use -v to see invocation.*");

				mtouch.AssertErrorPattern ("MT", 5210, "Native linking failed, undefined symbol: _OBJC_METACLASS_._Inexistent. Please verify that all the necessary frameworks have been referenced and native libraries are properly linked in.");
				mtouch.AssertErrorPattern ("MT", 5211, "Native linking failed, undefined Objective-C class: Inexistent. The symbol ._OBJC_CLASS_._Inexistent. could not be found in any of the libraries or frameworks linked with your application.");
				mtouch.AssertErrorPattern ("MT", 5202, "Native linking failed. Please review the build log.");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void TestCaseMismatchedAssemblyName ()
		{
			// desk #90367 (and others in the past as well)

			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (testDir);

			try {
				string dllcs = Path.Combine (testDir, "testLibrary.cs");
				string exe = Path.Combine (testDir, "testApp.exe");
				string dll = Path.Combine (testDir, "testLibrary.dll");
				string DLL = Path.Combine (testDir, "TESTLIBRARY.dll");
				string args;
				string output;

				File.WriteAllText (dllcs, "public class TestLib { public TestLib () { System.Console.WriteLine (typeof (UIKit.UIWindow).ToString ()); } }");

				args = string.Format ("\"{0}\" /debug:full /noconfig /t:library /nologo /out:\"{1}\" /r:" + Configuration.XamarinIOSDll, dllcs, dll);
				File.WriteAllText (DLL + ".config", "");
				if (ExecutionHelper.Execute (Configuration.SmcsPath, args, out output) != 0)
					throw new Exception (output);

				var execs = @"public class TestApp { 
	static void Main () 
	{
		System.Console.WriteLine (typeof (UIKit.UIWindow).ToString ());
		System.Console.WriteLine (new TestLib ());
	}
}";

				var exeF = Path.Combine (testDir, "testExe.cs");

				File.WriteAllText (exeF, execs);

				var cmds = string.Format ("\"{0}\" /noconfig /t:exe /nologo /out:\"{1}\" \"/r:{2}\" -r:{3}", exeF, exe, dll, Configuration.XamarinIOSDll);
				if (ExecutionHelper.Execute (Configuration.SmcsPath, cmds, out output) != 0)
					throw new Exception (output);

				File.Move (dll, DLL);

				Action<string> check = (v) =>
				{
					var msg = new StringBuilder ();
					int counter = 0;
					foreach (var file in Directory.EnumerateFiles (app, "*", SearchOption.AllDirectories)) {
						if (file.Contains ("TESTLIBRARY")) {
							msg.AppendFormat ("File {0} has incorrect case.\n", file);
						}
						counter++;
					}
					Console.WriteLine ("Checked {0} files", counter);
					if (msg.Length > 0)
						Assert.Fail (v + "\n" + msg.ToString ());
				};

				string [][] tests = new string[][] {
					new string [] { "linkall",  "-sdkroot {0} --dev {1} -sdk {2} --targetver {2} --abi=armv7s {3}      -debug -r:{4} -r:{5}" },
					new string [] { "dontlink", "-sdkroot {0} --dev {1} -sdk {2} --targetver {2} --abi=armv7s {3}      -debug -r:{4} -r:{5} --nolink" },
					new string [] { "dual",     "-sdkroot {0} --dev {1} -sdk {2} --targetver {2} --abi=armv7,arm64 {3} -debug -r:{4} -r:{5}" }
				};

				foreach (var kvp in tests) {
					var name = kvp [0];
					var format = kvp [1];
					var mtouch_fmt = string.Format (format, Configuration.xcode_root, app, Configuration.sdk_version, exe, Configuration.XamarinIOSDll, DLL);
					Directory.CreateDirectory (app);
					ExecutionHelper.Execute (TestTarget.ToolPath, mtouch_fmt, hide_output: false);
					check (name);
					Directory.Delete (app, true);
				}

			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void TestDuplicatedFatApp ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileUnifiedTestAppExecutable (testDir);
				var cache = Path.Combine (testDir, "mtouch-cache");

				var args = string.Format ("-sdkroot {5} --dev {0} -sdk {4} -targetver {4} --abi=armv7,arm64 {1} --cache={2} --r:{3} ", app, exe, cache, Configuration.XamarinIOSDll, Configuration.sdk_version, Configuration.xcode_root);
				ExecutionHelper.Execute (TestTarget.ToolPath, args, hide_output: false);
				var ufe = Mono.Unix.UnixFileInfo.GetFileSystemEntry (Path.Combine (app, ".monotouch-32", "testApp.exe"));
				Assert.IsTrue (ufe.IsSymbolicLink, "testApp.exe IsSymbolicLink");
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void TestAllLoad ()
		{
			var testDir = GetTempDirectory ();
			var app = Path.Combine (testDir, "testApp.app");
			Directory.CreateDirectory (app);

			try {
				var exe = CompileUnifiedTestAppExecutable (testDir);
				var cache = Path.Combine (testDir, "mtouch-cache");

				var args = string.Format ("-sdkroot {5} --dev {0} -sdk {4} -targetver {4} --abi=armv7,arm64 {1} --cache={2} --r:{3} -gcc_flags -all_load", app, exe, cache, Configuration.XamarinIOSDll, Configuration.sdk_version, Configuration.xcode_root);
				ExecutionHelper.Execute (TestTarget.ToolPath, args, hide_output: false);
			} finally {
				Directory.Delete (testDir, true);
			}
		}

		[Test]
		public void ListDev ()
		{
			Assert.Ignore ("This functionality has been migrated to mlaunch, and the test needs to be updated accordingly.");

			ExecutionHelper.Execute (TestTarget.ToolPath, string.Format ("--listdev --sdkroot {0}", Configuration.xcode_root));
		}

		[Test]
		public void LaunchOnDevice ()
		{
			Assert.Ignore ("This functionality has been migrated to mlaunch, and the test needs to be updated accordingly.");

			var mtouch = new MTouchTool ();
			var devices = mtouch.FindAvailableDevices (new string [] { "iPad", "iPhone" }).ToArray ();
			if (devices.Length == 0)
				Assert.Ignore ("Could not find any connected devices.");

			var projectDir = Path.Combine (Configuration.SourceRoot, "tests", "link all");
			var project = Path.Combine (projectDir, "link all.csproj");
			XBuild.Build (project, platform: "iPhone");
			var appPath = Path.Combine (projectDir, "bin", "iPhone", "Debug", "link all.app");
			foreach (var device in devices) {
				if (mtouch.InstallOnDevice (device, appPath) != 0) {
					Console.WriteLine ("Could not install on the device '{0}'.", device);
					continue;
				}
				if (mtouch.LaunchOnDevice (device, appPath, false, false) != 0) {
					if (mtouch.HasErrorPattern ("MT", 1031, "Could not launch the app '.*' on the device '.*' because the device is locked. Please unlock the device and try again."))
						continue;
					Assert.Fail ("Failed to launch on device.");
				} else {
					return;
				}
			}

			Assert.Ignore ("Could not find any non-locked devices.");
		}

		[Test]
		public void LaunchOnWatchDevice ()
		{
			Assert.Ignore ("This functionality has been migrated to mlaunch, and the test needs to be updated accordingly.");

			var mtouch = new MTouchTool ();
			mtouch.Verbosity = 2;
			var devices = mtouch.FindAvailableDevices (new string [] { "Watch" }).ToArray ();
			if (devices.Length == 0)
				Assert.Ignore ("Could not find any connected watches.");

			var projectDir = Path.Combine (Configuration.SourceRoot, "msbuild", "tests", "MyWatch2Container");
			var project = Path.Combine (projectDir, "MyWatch2Container.csproj");
			var containerPath = Path.Combine (projectDir, "bin", "iPhone", "Debug", "MyWatch2Container.app");
			var appPath = Path.Combine (containerPath, "Watch", "MyWatchApp2.app");

			XBuild.Build (project, platform: "iPhone");
			if (!Directory.Exists (appPath))
				Assert.Fail ("Failed to build the watchOS app.");

			foreach (var device in devices) {
				if (device.Companion == null)
					continue;

				if (mtouch.InstallOnDevice (device.Companion, containerPath, "ios,watch") != 0) {
					Console.WriteLine ("Could not install on the phone '{0}'. Trying another one.", device.Name);
					continue;
				}

				if (mtouch.LaunchOnDevice (device, appPath, false, false) != 0) {
					if (mtouch.HasErrorPattern ("MT", 1031, "Could not launch the app '.*' on the device '.*' because the device is locked. Please unlock the device and try again."))
						continue;
					Assert.Fail ("Failed to launch on device.");
				} else {
					return;
				}
			}

			Assert.Ignore ("Could not find any suitable devices.");
		}

		[Test]
		[TestCase (Profile.Unified)]
		public void DlsymDisabled (Profile profile)
		{
			using (var tool = new MTouchTool ()) {
				tool.Profile = profile;
				tool.Verbosity = 5;
				tool.Cache = Path.Combine (tool.CreateTemporaryDirectory (), "mtouch-test-cache");
				tool.CreateTemporaryApp ("using UIKit; class C { static void Main (string[] args) { UIApplication.Main (args); } }");
				tool.FastDev = true;
				tool.Dlsym = false;

				Assert.AreEqual (0, tool.Execute (MTouchAction.BuildDev));
			}
		}

		[Test]
		public void PInvokeWrapperGenerationTest ()
		{
			using (var tool = new MTouchTool ()) {
				tool.Profile = Profile.WatchOS;
				tool.CreateTemporaryCacheDirectory ();
				tool.Verbosity = 5;
				tool.Extension = true;
				tool.CreateTemporaryWatchKitExtension ();

				tool.FastDev = true;
				Assert.AreEqual (0, tool.Execute (MTouchAction.BuildDev));

				Assert.IsTrue (File.Exists (Path.Combine (tool.AppPath, "libpinvokes.dylib")), "libpinvokes.dylib existence");

				var otool_output = ExecutionHelper.Execute ("otool", $"-l {Quote (Path.Combine (tool.AppPath, "libpinvokes.dylib"))}", hide_output: true);
				Assert.That (otool_output, Is.StringContaining ("LC_ID_DYLIB"), "output contains LC_ID_DYLIB");

				var lines = otool_output.Split (new char [] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < lines.Length; i++) {
					if (lines [i].Contains ("LC_ID_DYLIB")) {
						Assert.That (lines [i + 2], Is.StringContaining ("name @executable_path/libpinvokes.dylib "), "LC_ID_DYLIB");
						break;
					}
				}
			}
		}

#region Helper functions
		static string CompileUnifiedTestAppExecutable (string targetDirectory, string code = null, string extraArg = "")
		{
			return CompileTestAppExecutable (targetDirectory, code, extraArg, profile: MTouch.Profile.Unified);
		}

		public static string CompileTestAppExecutable (string targetDirectory, string code = null, string extraArg = "", Profile profile = Profile.Unified)
		{
			if (code == null)
				code = "public class TestApp { static void Main () { System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ()); } }";

			return CompileTestAppCode ("exe", targetDirectory, code, extraArg, profile);
		}

		public static string CompileTestAppLibrary (string targetDirectory, string code, string extraArg = null, Profile profile = Profile.Unified)
		{
			return CompileTestAppCode ("library", targetDirectory, code, extraArg, profile);
		}

		public static string CompileTestAppCode (string target, string targetDirectory, string code, string extraArg = "", Profile profile = Profile.Unified)
		{
			var ext = target == "exe" ? "exe" : "dll";
			var cs = Path.Combine (targetDirectory, "testApp.cs");
			var assembly = Path.Combine (targetDirectory, "testApp." + ext);
			var root_library = GetBaseLibrary (profile);

			File.WriteAllText (cs, code);

			string output;
			StringBuilder args = new StringBuilder ();
			string fileName = GetCompiler (profile, args);
			args.AppendFormat ($" /noconfig /t:{target} /nologo /out:{Quote (assembly)} /r:{Quote (root_library)} {cs} {extraArg}");
			if (ExecutionHelper.Execute (fileName, args.ToString (), out output) != 0) {
				Console.WriteLine ("{0} {1}", fileName, args);
				Console.WriteLine (output);
				throw new Exception (output);
			}

			return assembly;
		}

		static string CreateBindingLibrary (string targetDirectory, string nativeCode, string bindingCode, string linkWith = null, string extraCode = "")
		{
			var o = CompileNativeLibrary (targetDirectory, nativeCode);
			var cs = Path.Combine (targetDirectory, "bindingCode.cs");
			var dll = Path.Combine (targetDirectory, "bindingLibrary.dll");

			if (linkWith == null) {
				linkWith = @"
using System;
using ObjCRuntime;

[assembly: LinkWith (""{0}"", LinkTarget.ArmV7, ForceLoad = true, SmartLink = true)]
";
				linkWith = string.Format (linkWith, Path.GetFileName (o));
			}

			File.WriteAllText (cs, bindingCode);

			extraCode = linkWith + "\n" + extraCode;

			var x = Path.Combine (targetDirectory, "extraBindingCode.cs");
			File.WriteAllText (x, extraCode);

			ExecutionHelper.Execute (Configuration.BtouchPath, 
				string.Format ("{0} --out:{1} --link-with={2},{3} -x:{4}", cs, dll, o, Path.GetFileName (o), x));

			return dll;
		}

		static string CompileNativeLibrary (string targetDirectory, string code)
		{
			var m = Path.Combine (targetDirectory, "testCode.m");

			File.WriteAllText (m, code);

			string output;
			string fileName = Path.Combine (Configuration.xcode_root, "Toolchains/XcodeDefault.xctoolchain/usr/bin/clang");
			string args = string.Format ("-gdwarf-2 -arch armv7 -std=c99 -isysroot {0}/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS{2}.sdk -miphoneos-version-min=3.1 " +
				"-c -DDEBUG  -o {1}/testCode.o -x objective-c {1}/testCode.m",
				Configuration.xcode_root, targetDirectory, Configuration.sdk_version);

			if (ExecutionHelper.Execute (fileName, args, out output) != 0) {
				Console.WriteLine ("{0} {1}", fileName, args);
				Console.WriteLine (output);
				throw new Exception (output);
			}

			return Path.Combine (targetDirectory, "testCode.o");
		}

		void CompileCSharpCode (Profile profile, string code, string outputPath, params string[] additional_arguments)
		{
			var tmpFile = Path.GetTempFileName ();
			try {
				File.WriteAllText (tmpFile, code);

				string output;
				var args = new StringBuilder ();
				var compiler = GetCompiler (profile, args);

				args.Append (" -target:").Append (outputPath.EndsWith (".dll") ? "library" : "exe");
				args.Append (" -r:").Append (Quote (GetBaseLibrary (profile)));
				args.Append (" -out:").Append (Quote (outputPath));
				args.Append (" ").Append (Quote (tmpFile));
				foreach (var aa in additional_arguments)
					args.Append (" ").Append (aa);

				if (ExecutionHelper.Execute (compiler, args.ToString (), out output) != 0)
					throw new Exception (output);
			} finally {
				File.Delete (tmpFile);
			}
		}

		static Dictionary<Profile, string> compiled_linkwith_apps = new Dictionary<Profile, string> ();
		public static string CompileTestAppExecutableLinkWith (string targetDirectory, Profile profile = Profile.Unified)
		{
			string compiled_linkwith_app;
			if (compiled_linkwith_apps.TryGetValue (profile, out compiled_linkwith_app) && File.Exists (compiled_linkwith_app))
				return compiled_linkwith_app;

			string cs = Path.Combine (targetDirectory, "testApp.cs");
			string exe = Path.Combine (targetDirectory, "testApp" + GetProjectSuffix (profile) + ".exe");
			File.WriteAllText (cs, @"
using System;
public class TestApp {
	static void Main ()
	{
		Console.WriteLine (typeof (UIKit.UIWindow).ToString ());
		Console.WriteLine (Bindings.Test.CFunctions.theUltimateAnswer ());
		Console.WriteLine (typeof (Bindings.Test.UltimateMachine).ToString ());
	}
 }");

			string output;
			var args = new StringBuilder ();
			args.AppendFormat ("\"{0}\" /noconfig /t:exe /nologo /out:\"{1}\" \"/r:{2}\" /r:\"{3}\"", cs, exe, GetBaseLibrary (profile), GetBindingsLibrary (profile));
			var compiler = GetCompiler (profile, args);
			if (ExecutionHelper.Execute (compiler, args.ToString (), out output) != 0)
				throw new Exception (output);

			compiled_linkwith_apps [profile] = exe;
			return exe;
		}
	
		static void VerifyGC (string file, bool isBoehm, string message)
		{
			var symbols = ExecutionHelper.Execute ("nm", file, hide_output: true);
			var _sgen_gc_lock = symbols.Contains ("_sgen_gc_lock");
			if (isBoehm && _sgen_gc_lock) {
				Assert.Fail ("Expected '{0}' to use Boehm: {1}", file, message);
			} else if (!isBoehm && !_sgen_gc_lock) {
				Assert.Fail ("Expected '{0}' to use SGen: {1}", file, message);
			}
		}

		static void VerifyArchitectures (string file, string message, params string[] expected)
		{
			var actual = GetArchitectures (file).ToArray ();

			Array.Sort (expected);
			Array.Sort (actual);

			var e = string.Join (", ", expected);
			var a = string.Join (", ", actual);

			Assert.AreEqual (e, a, message);
		}

		static void VerifyOutput (string msg, string actual, params string[] expected)
		{
			var split = actual.Split (new char[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
			var actual_messages = new HashSet<string> (split, new Registrar.PatternEquality ());
			var exp_messages = new HashSet<string> (expected, new Registrar.PatternEquality ());

			actual_messages.ExceptWith (exp_messages);
			exp_messages.ExceptWith (split);

			var text = new StringBuilder ();
			foreach (var a in actual_messages)
				text.AppendFormat ("Unexpected error/warning ({0}):\n\t{1}\n", msg, a);
			foreach (var a in exp_messages)
				text.AppendFormat ("Expected error/warning not shown ({0}):\n\t{1}\n", msg, a);
			if (text.Length != 0)
				Assert.Fail (text.ToString ());
		}

		static List<string> GetArchitectures (string file)
		{
			var result = new List<string> ();

			using (var fs = File.OpenRead (file)) {
				using (var reader = new BinaryReader (fs)) {
					int magic = reader.ReadInt32 ();
					switch ((uint) magic) {
					case 0xCAFEBABE: // little-endian fat binary
						throw new NotImplementedException ("little endian fat binary");
					case 0xBEBAFECA:
						int architectures = System.Net.IPAddress.NetworkToHostOrder (reader.ReadInt32 ());
						for (int i = 0; i < architectures; i++) {
							result.Add (GetArch (System.Net.IPAddress.NetworkToHostOrder (reader.ReadInt32 ()), System.Net.IPAddress.NetworkToHostOrder (reader.ReadInt32 ())));
							// skip to next entry
							reader.ReadInt32 (); // offset
							reader.ReadInt32 (); // size
							reader.ReadInt32 (); // align
						}
						break;
					case 0xFEEDFACE: // little-endian mach-o header
					case 0xFEEDFACF: // little-endian 64-big mach-o header
						result.Add (GetArch (reader.ReadInt32 (), reader.ReadInt32 ()));
						break;
					case 0xCFFAEDFE:
					case 0xCEFAEDFE:
						result.Add (GetArch (System.Net.IPAddress.NetworkToHostOrder (reader.ReadInt32 ()), System.Net.IPAddress.NetworkToHostOrder (reader.ReadInt32 ())));
						break;
					default:
						throw new Exception (string.Format ("File '{0}' is neither a Universal binary nor a Mach-O binary (magic: 0x{1})", file, magic.ToString ("x")));
					}
				}
			}

			return result;
		}

		static string GetArch (int cputype, int cpusubtype)
		{
			const int ABI64 = 0x01000000;
			const int X86 = 7;
			const int ARM = 12;

			switch (cputype) {
			case ARM : // arm
				switch (cpusubtype) {
				case 6: return "armv6";
				case 9: return "armv7";
				case 11: return "armv7s";
				default:
					return "unknown arm variation: " + cpusubtype.ToString ();
				}
			case ARM  | ABI64:
				switch (cpusubtype) {
				case 0:
					return "arm64";
				default:
					return "unknown arm/64 variation: " + cpusubtype.ToString ();
				}
			case X86: // x86
				return "i386";
			case X86 | ABI64: // x64
				return "x86_64";
			}

			return string.Format ("unknown: {0}/{1}", cputype, cpusubtype);
		}

		public static string Quote (string f)
		{
			if (f.IndexOf (' ') == -1 && f.IndexOf ('\'') == -1 && f.IndexOf (',') == -1)
				return f;

			var s = new StringBuilder ();

			s.Append ('"');
			foreach (var c in f){
				if (c == '"' || c == '\\')
					s.Append ('\\');

				s.Append (c);
			}
			s.Append ('"');

			return s.ToString ();
		}

		public static string GetTempDirectory ()
		{
			var tmp = Path.GetTempFileName ();
			File.Delete (tmp);
			Directory.CreateDirectory (tmp);
			return tmp;
		}
#endregion
	}

	class McsException : Exception {
		public McsException (string output)
			: base (output)
		{
		}
	}	

	class ActivationException : Exception {
		public ActivationException (string output)
			: base (output)
		{
		}
	}
}
