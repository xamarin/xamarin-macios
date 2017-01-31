using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Xamarin;
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

namespace Xamarin
{
	public enum Target { Sim, Dev }
	public enum Config { Debug, Release }
	public enum PackageMdb { Default, WithMdb, WoutMdb }
	public enum MSym { Default, WithMSym, WoutMSym }
	public enum Profile { iOS, tvOS, watchOS }

	[TestFixture]
	public class MTouch
	{
		[Test]
		[TestCase ("single", "",                   false)]
		[TestCase ("dual",   "armv7,arm64", false)]
		[TestCase ("llvm",   "armv7+llvm",  false)]
		[TestCase ("debug",  "",                   true)]
		public void RebuildTest (string name, string abi, bool debug)
		{
			AssertDeviceAvailable ();

			using (var mtouch = new MTouchTool ()) {
				var codeA = "public class TestApp1 { static void Main () { System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ()); } }";
				var codeB = "public class TestApp2 { static void Main () { System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ()); } }";
				mtouch.CreateTemporaryApp (code: codeA);
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.Abi = abi;
				mtouch.Debug = debug;
				mtouch.TargetVer = "6.0";
				DateTime dt = DateTime.MinValue;

				Action<string, IEnumerable<string>> checkNotModified = (filename, skip) =>
				{
					var failed = new List<string> ();
					var files = Directory.EnumerateFiles (mtouch.AppPath, "*", SearchOption.AllDirectories);
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
					Assert.IsEmpty (failed, filename);
				};

				mtouch.DSym = false; // we don't need the dSYMs for this test, so disable them to speed up the test.
				mtouch.MSym = false; // we don't need the mSYMs for this test, so disable them to speed up the test.
				mtouch.AssertExecute (MTouchAction.BuildDev, "first build");
				Console.WriteLine ("first build done");

				dt = DateTime.Now;
				System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older.

				mtouch.AssertExecute (MTouchAction.BuildDev, "second build");
				Console.WriteLine ("second build done");

				checkNotModified (name, null);

				// Test that a rebuild (where something changed, in this case the .exe)
				// actually work. We compile with custom code to make sure it's different
				// from the previous exe we built.
				var subDir = Cache.CreateTemporaryDirectory ();
				var exe2 = CompileUnifiedTestAppExecutable (subDir,
					/* the code here only changes the class name (default: 'TestApp1' changed to 'TestApp2') to minimize the related
					 * changes (there should be no changes in Xamarin.iOS.dll nor mscorlib.dll, even after linking) */
					code: codeB);
				File.Copy (exe2, mtouch.RootAssembly, true);

				mtouch.AssertExecute (MTouchAction.BuildDev, "third build");

				var skipFiles = new string [] { "testApp", "testApp.exe", "testApp.aotdata.armv7", "testApp.aotdata.arm64" };
				checkNotModified (name + "-rebuilt", skipFiles);
			}
		}

		[Test]
		public void RebuildTest_Intl ()
		{
			using (var tool = new MTouchTool ()) {
				tool.Profile = Profile.iOS;
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

		[Test]
		public void RebuildTest_DontLink ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.NoFastSim = true;
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.CreateTemporaryApp ();
				mtouch.Verbosity = 4;
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.AssertExecute (MTouchAction.BuildSim, "build 1");
				mtouch.AssertOutputPattern ("Linking .*/testApp.exe into .*/PreBuild using mode 'None'");
				mtouch.AssertExecute (MTouchAction.BuildSim, "build 2");
				mtouch.AssertOutputPattern ("Cached assemblies reloaded.");
			}
		}

		[Test]
		[TestCase ("single", "", false, new string [] { } )]
		[TestCase ("dual", "armv7,arm64", false, new string [] { })]
		[TestCase ("llvm", "armv7+llvm", false, new string [] { })]
		[TestCase ("debug", "", true, new string [] { })]
		[TestCase ("single-framework", "", false, new string [] { "@sdk=framework=Xamarin.Sdk", "@all=staticobject" })]
		public void RebuildTest_WithExtensions (string name, string abi, bool debug, string[] assembly_build_targets)
		{
			var codeA = "[Foundation.Preserve] public class TestApp1 { static void X () { System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ()); } }";
			var codeB = "[Foundation.Preserve] public class TestApp2 { static void X () { System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ()); } }";

			using (var extension = new MTouchTool ()) {
				extension.Extension = true;
				extension.CreateTemporararyServiceExtension (extraCode: codeA);
				extension.CreateTemporaryCacheDirectory ();
				extension.Abi = abi;
				extension.Debug = debug;
				extension.AssemblyBuildTargets.AddRange (assembly_build_targets);
				extension.DSym = false; // faster test
				extension.MSym = false; // faster test
				extension.AssertExecute (MTouchAction.BuildDev, "extension build");

				using (var mtouch = new MTouchTool ()) {
					mtouch.AppExtensions.Add (extension);
					mtouch.CreateTemporaryApp (extraCode: codeA);
					mtouch.CreateTemporaryCacheDirectory ();
					mtouch.Abi = abi;
					mtouch.Debug = debug;
					mtouch.AssemblyBuildTargets.AddRange (assembly_build_targets);
					mtouch.DSym = false; // faster test
					mtouch.MSym = false; // faster test

					var timestamp = DateTime.MinValue;

					Action<string, IEnumerable<string>> assertNotModified = (filename, skip) =>
					{
						var failed = new List<string> ();
						var files = Directory.EnumerateFiles (mtouch.AppPath, "*", SearchOption.AllDirectories);
						files = files.Concat (Directory.EnumerateFiles (extension.AppPath, "*", SearchOption.AllDirectories));
						foreach (var file in files) {
							if (skip != null && skip.Contains (Path.GetFileName (file)))
								continue;
							var info = new FileInfo (file);
							if (info.LastWriteTime > timestamp) {
								failed.Add (string.Format ("{0} is modified, timestamp: {1}", file, info.LastWriteTime));
								Console.WriteLine ("FAIL: {0} modified: {1}", file, info.LastWriteTime);
							} else {
								Console.WriteLine ("{0} not modified", file);
							}
						}
						Assert.IsEmpty (failed, filename);
					};

					mtouch.AssertExecute (MTouchAction.BuildDev, "first build");
					Console.WriteLine ($"{DateTime.Now} **** FIRST BUILD DONE ****");

					timestamp = DateTime.Now;
					System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older. HFS+ has a 1s timestamp resolution :(

					mtouch.AssertExecute (MTouchAction.BuildDev, "second build");
					Console.WriteLine ($"{DateTime.Now} **** SECOND BUILD DONE ****");

					assertNotModified (name, null);

					// Touch the extension's executable, nothing should change
					new FileInfo (extension.RootAssembly).LastWriteTimeUtc = DateTime.UtcNow;
					mtouch.AssertExecute (MTouchAction.BuildDev, "touch extension executable");
					Console.WriteLine ($"{DateTime.Now} **** TOUCH EXTENSION EXECUTABLE DONE ****");
					assertNotModified (name, null);

					// Touch the main app's executable, nothing should change
					new FileInfo (mtouch.RootAssembly).LastWriteTimeUtc = DateTime.UtcNow;
					mtouch.AssertExecute (MTouchAction.BuildDev, "touch main app executable");
					Console.WriteLine ($"{DateTime.Now} **** TOUCH MAIN APP EXECUTABLE DONE ****");
					assertNotModified (name, null);

					// Test that a rebuild (where something changed, in this case the .exe)
					// actually work. We compile with custom code to make sure it's different
					// from the previous exe we built.
					//
					// The code change is minimal: only changes the class name (default: 'TestApp1' changed to 'TestApp2') to minimize the related
					// changes (there should be no changes in Xamarin.iOS.dll nor mscorlib.dll, even after linking)

					// Rebuild the extension's .exe
					extension.CreateTemporararyServiceExtension (extraCode: codeB);
					mtouch.AssertExecute (MTouchAction.BuildDev, "change extension executable");
					Console.WriteLine ($"{DateTime.Now} **** CHANGE EXTENSION EXECUTABLE DONE ****");
					assertNotModified (name, new [] { "testServiceExtension", "testServiceExtension.aotdata.armv7", "testServiceExtension.aotdata.arm64", "testServiceExtension.dll" } );

					timestamp = DateTime.Now;
					System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older. HFS+ has a 1s timestamp resolution :(

					// Rebuild the main app's .exe
					mtouch.CreateTemporaryApp (extraCode: codeB);
					mtouch.AssertExecute (MTouchAction.BuildDev, "change app executable");
					Console.WriteLine ($"{DateTime.Now} **** CHANGE APP EXECUTABLE DONE ****");
					assertNotModified (name, new [] { "testApp", "testApp.aotdata.armv7", "testApp.aotdata.arm64", "testApp.exe" });
				}
			}
		}

		[Test]
		// Simulator
		[TestCase (Target.Sim, Config.Release, PackageMdb.Default, MSym.Default,  false, false, "")]
		[TestCase (Target.Sim, Config.Debug,   PackageMdb.Default, MSym.Default,  true,  false, "")]
		[TestCase (Target.Sim, Config.Debug,   PackageMdb.WoutMdb, MSym.Default,  false, false, "")]
		[TestCase (Target.Sim, Config.Release, PackageMdb.WithMdb, MSym.Default,  true,  false, "")]
		[TestCase (Target.Sim, Config.Debug,   PackageMdb.WoutMdb, MSym.Default,  false, false, "--nofastsim --nolink")]
		// Device
		[TestCase (Target.Dev, Config.Release, PackageMdb.WithMdb, MSym.Default,  true,  true,  "")]
		[TestCase (Target.Dev, Config.Release, PackageMdb.WithMdb, MSym.WoutMSym, true,  false, "")]
		[TestCase (Target.Dev, Config.Release, PackageMdb.Default, MSym.Default,  false, true,  "--abi:armv7,arm64")]
		[TestCase (Target.Dev, Config.Debug,   PackageMdb.WoutMdb, MSym.Default,  false, false, "")]
		[TestCase (Target.Dev, Config.Debug,   PackageMdb.WoutMdb, MSym.WithMSym, false, true,  "")]
		[TestCase (Target.Dev, Config.Release, PackageMdb.WithMdb, MSym.Default,  true,  true,  "--abi:armv7+llvm")]
		public void SymbolicationData (Target target, Config configuration, PackageMdb package_mdb, MSym msym, bool has_mdb, bool has_msym, string extra_mtouch_args)
		{
			if (target == Target.Dev)
				AssertDeviceAvailable ();

			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.iOS;
				mtouch.CreateTemporaryApp (hasPlist: true);
				switch (package_mdb) {
				case PackageMdb.WithMdb:
					mtouch.PackageMdb = true;
					break;
				case PackageMdb.WoutMdb:
					mtouch.PackageMdb = false;
					break;
				}
				switch (msym) {
				case MSym.WithMSym:
					mtouch.MSym = true;
					break;
				case MSym.WoutMSym:
					mtouch.MSym = false;
					break;
				}
				if (configuration == Config.Debug)
					mtouch.Debug = true;

				var is_sim = target == Target.Sim;
				mtouch.AssertExecute (is_sim ? MTouchAction.BuildSim : MTouchAction.BuildDev, "build");

				var appDir = mtouch.AppPath;
				var msymDir = appDir + ".mSYM";
				var is_dual_asm = !is_sim && extra_mtouch_args.Contains ("--abi") && extra_mtouch_args.Contains (",");
				if (!is_dual_asm) {
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
					var manifest = new XmlDocument ();
					manifest.Load (Path.Combine (msymDir, "manifest.xml"));
					Assert.AreEqual ("com.xamarin.testApp", manifest.SelectSingleNode ("/mono-debug/app-id").InnerText, "app-id");
				} else {
					DirectoryAssert.DoesNotExist (msymDir, "mSYM found when not expected");
				}
			}
		}

		[Test]
		public void ExecutableName ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Executable = "CustomExecutable";
				mtouch.NoFastSim = true;
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
				FileAssert.Exists (Path.Combine (mtouch.AppPath, "CustomExecutable"), "1");
				FileAssert.DoesNotExist (Path.Combine (mtouch.AppPath, Path.GetFileNameWithoutExtension (mtouch.RootAssembly)), "2");
			}
		}

		[Test]
		public void MT0003 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp (appName: "mscorlib");
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (3, "Application name 'mscorlib.exe' conflicts with an SDK or product assembly (.dll) name.");
			}
		}

		[Test]
		public void MT0008 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryAppDirectory ();
				mtouch.CustomArguments = new string [] { "foo.exe", "bar.exe" };
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (8, "You should provide one root assembly only, found 2 assemblies: 'foo.exe', 'bar.exe'");
			}
		}

		[Test]
		public void MT0015 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Abi = "invalid-arm";
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (15, "Invalid ABI: invalid-arm. Supported ABIs are: i386, x86_64, armv7, armv7+llvm, armv7+llvm+thumb2, armv7s, armv7s+llvm, armv7s+llvm+thumb2, armv7k, armv7k+llvm, arm64 and arm64+llvm.");
			}
		}

		[Test]
		public void MT0017 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryAppDirectory ();
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (17, "You should provide a root assembly.");
			}
		}

		[Test]
		public void MT0018 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CustomArguments = new string [] { "--unknown", "-unknown" };
				mtouch.CreateTemporaryAppDirectory ();
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (18, "Unknown command line argument: '-unknown'");
				mtouch.AssertError (18, "Unknown command line argument: '--unknown'");
			}
		}

		[Test]
		[TestCase (Profile.iOS, Profile.tvOS)]
		[TestCase (Profile.iOS, Profile.watchOS)]
		[TestCase (Profile.tvOS, Profile.iOS)]
		[TestCase (Profile.tvOS, Profile.watchOS)]
		[TestCase (Profile.watchOS, Profile.iOS)]
		[TestCase (Profile.watchOS, Profile.tvOS)]
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
			AssertDeviceAvailable ();

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.TargetVer = "3.1";

				mtouch.Abi = "armv7s,arm64";
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, $"build: {mtouch.Abi}");
				mtouch.AssertErrorPattern (73, "Xamarin.iOS .* does not support a deployment target of 3.1 for iOS .the minimum is 6.0.. Please select a newer deployment target in your project's Info.plist.");

				mtouch.Abi = "armv7s";
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, $"build: {mtouch.Abi}");
				mtouch.AssertErrorPattern (73, "Xamarin.iOS .* does not support a deployment target of 3.1 for iOS .the minimum is 6.0.. Please select a newer deployment target in your project's Info.plist.");

				mtouch.Abi = "arm64";
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, $"build: {mtouch.Abi}");
				mtouch.AssertErrorPattern (73, "Xamarin.iOS .* does not support a deployment target of 3.1 for iOS .the minimum is 6.0.. Please select a newer deployment target in your project's Info.plist.");

				mtouch.Abi = "armv7";
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, $"build: {mtouch.Abi}");
				mtouch.AssertErrorPattern (73, "Xamarin.iOS .* does not support a deployment target of 3.1 for iOS .the minimum is 6.0.. Please select a newer deployment target in your project's Info.plist.");
			}
		}

		[Test]
		public void MT0074 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.TargetVer = "400.0.0";
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");
				mtouch.AssertErrorPattern (74, $"Xamarin.iOS .* does not support a deployment target of 400.0.0 for iOS .the maximum is {Configuration.sdk_version}.. Please select an older deployment target in your project's Info.plist or upgrade to a newer version of Xamarin.iOS.");
			}
		}

		[Test]
		[TestCase (Profile.iOS, Profile.tvOS)]
		[TestCase (Profile.iOS, Profile.watchOS)]
		[TestCase (Profile.tvOS, Profile.iOS)]
		[TestCase (Profile.tvOS, Profile.watchOS)]
		[TestCase (Profile.watchOS, Profile.iOS)]
		[TestCase (Profile.watchOS, Profile.tvOS)]
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
				mtouch.RootAssembly = exe;
				mtouch.References = new string [] { GetBaseLibrary (exe_profile) };
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.BuildSim), "build");
				var dllBase = Path.GetFileName (GetBaseLibrary (dll_profile));
				mtouch.AssertError (34, string.Format ("Cannot reference '{0}' in a {1} project - it is implicitly referenced by 'testLib, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'.", dllBase, GetPlatformName (exe_profile)));
			}
		}

		[Test]
		public void MT0020 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();

				foreach (var registrar in new string [] { "oldstatic", "olddynamic", "legacy", "legacystatic", "legacydynamic" }) {
					mtouch.CustomArguments = new string [] { $"--registrar:{registrar}" };
					mtouch.AssertExecuteFailure (MTouchAction.BuildSim, $"build {registrar}");
					mtouch.AssertError (20, "The valid options for '--registrar' are 'static, dynamic or default'.");
				}
			}
		}

		[Test]
		public void MT0023 ()
		{
			using (var mtouch = new MTouchTool ()) {
				// Create a library with the same name as the exe
				var tmp = mtouch.CreateTemporaryDirectory ();
				var dllA = CompileTestAppCode ("library", tmp, "public class X {}");

				mtouch.CreateTemporaryApp (code: "public class C { static void Main () { System.Console.WriteLine (typeof (X)); System.Console.WriteLine (typeof (UIKit.UIWindow)); } }", extraArg: "-r:" + Quote (dllA));
				mtouch.References = new string [] { dllA };
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertErrorPattern (23, "The root assembly .*/testApp.exe conflicts with another assembly (.*/testApp.dll).");
			}
		}

		[Test]
		public void MT0023_Extension ()
		{
			using (var extension = new MTouchTool ()) {
				// Create a library with the same name as the root assembly
				var tmp = extension.CreateTemporaryDirectory ();
				var dll = CompileTestAppCode ("library", tmp, "public class X {}", appName: "testApp");

				extension.Linker = MTouchLinker.DontLink; // fastest.
				extension.Extension = true;
				extension.CreateTemporararyServiceExtension (extraArg: $"-r:{Quote (dll)}", extraCode: "class Z { static void Y () { System.Console.WriteLine (typeof (X)); } }", appName: "testApp");
				extension.CreateTemporaryCacheDirectory ();
				extension.References = new [] { dll };
				extension.AssertExecute (MTouchAction.BuildSim, "extension build");

				using (var app = new MTouchTool ()) {
					app.Linker = MTouchLinker.DontLink; // fastest.
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.AssertExecuteFailure (MTouchAction.BuildSim, "app build");
					app.AssertError (23, $"The root assembly {extension.RootAssembly} conflicts with another assembly ({dll}).");
				}
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.watchOS)]
		[TestCase (Profile.tvOS)]
		public void MT0025 (Profile profile)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = profile;
				mtouch.CreateTemporaryApp ();
				mtouch.Sdk = MTouchTool.None;

				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build dev");
				mtouch.AssertError (25, $"No SDK version was provided. Please add --sdk=X.Y to specify which {GetPlatformSimpleName (profile)} SDK should be used to build your application.");

				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build dev");
				mtouch.AssertError (25, $"No SDK version was provided. Please add --sdk=X.Y to specify which {GetPlatformSimpleName (profile)} SDK should be used to build your application.");
			}
		}

		[Test]
		public void MT0026 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.LLVMOptimizations = "-O2";
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");
				mtouch.AssertError (26, "Could not parse the command line argument '--llvm-opt=-O2': Both assembly and optimization must be specified (assembly=optimization)");
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
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.SdkRoot = "/dir/that/does/not/exist";
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (55, "The Xcode path '/dir/that/does/not/exist' does not exist.");
			}
		}

		[Test]
		public void MT0060 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.EnvironmentVariables = new Dictionary<string, string> { { "DEVELOPER_DIR", "/dir/that/does/not/exist" } };
				mtouch.SdkRoot = MTouchTool.None;
				mtouch.AssertExecuteFailure (MTouchAction.None, "build");
				mtouch.AssertWarning (60, "Could not find the currently selected Xcode on the system. 'xcode-select --print-path' returned '/dir/that/does/not/exist', but that directory does not exist.");
				if (!Directory.Exists ("/Applications/Xcode.app")) {
					mtouch.AssertError (56, "Cannot find Xcode in the default location (/Applications/Xcode.app). Please install Xcode, or pass a custom path using --sdkroot <path>.");
				} else {
					mtouch.AssertWarning (62, "No Xcode.app specified (using --sdkroot or 'xcode-select --print-path'), using the default Xcode instead: /Applications/Xcode.app");
					mtouch.AssertError (52, "No command specified.");
				}
			}
		}

		[Test]
		public void MT0061 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.SdkRoot = MTouchTool.None;
				mtouch.AssertExecuteFailure (MTouchAction.None, "build");
				mtouch.AssertWarningPattern (61, "No Xcode.app specified .using --sdkroot., using the system Xcode as reported by 'xcode-select --print-path': .*");
				mtouch.AssertError (52, "No command specified.");
			}
		}

		[Test]
		public void MT0065_Custom ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.TargetVer = "7.1";
				mtouch.Frameworks.Add ("/foo/bar/zap.framework");
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (65, "Xamarin.iOS only supports embedded frameworks when deployment target is at least 8.0 (current deployment target: '7.1'; embedded frameworks: '/foo/bar/zap.framework')");
			}
		}

		[Test]
		public void MT0065_Mono ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.TargetVer = "7.1";
				mtouch.Mono = "framework";
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertErrorPattern (65, "Xamarin.iOS only supports embedded frameworks when deployment target is at least 8.0 .current deployment target: '7.1'; embedded frameworks: '.*/Mono.framework'.");
			}
		}
		[Test]
		public void MT0075 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Abi = "armv7k";
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");
				mtouch.AssertError (75, "Invalid architecture 'ARMv7k' for iOS projects. Valid architectures are: ARMv7, ARMv7+Thumb, ARMv7+LLVM, ARMv7+LLVM+Thumb, ARMv7s, ARMv7s+Thumb, ARMv7s+LLVM, ARMv7s+LLVM+Thumb, ARM64, ARM64+LLVM");
			}
		}

		[Test]
		[TestCase (Profile.watchOS)]
		[TestCase (Profile.tvOS)]
		public void MT0076 (Profile profile)
		{
			if (!Configuration.include_watchos || !Configuration.include_tvos)
				Assert.Ignore ("This test requires WatchOS and TVOS to be enabled.");

			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = profile;
				mtouch.Abi = MTouchTool.None;
				mtouch.CreateTemporaryApp ();
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");
				mtouch.AssertError (76, $"No architecture specified (using the --abi argument). An architecture is required for {GetPlatformName (profile)} projects.");
			}
		}

		[Test]
		public void MT0077 ()
		{
			if (!Configuration.include_watchos)
				Assert.Ignore ("This test requires WatchOS and TVOS to be enabled.");

			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.watchOS;
				mtouch.CreateTemporaryApp ();
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (77, "WatchOS projects must be extensions.");
			}
		}

		[Test]
		[TestCase (Profile.tvOS)]
		//[TestCase (Profile.WatchOS)] MT0077 interferring.
		[TestCase (Profile.iOS)]
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
		[TestCase (Profile.tvOS)]
		[TestCase (Profile.watchOS)]
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
		[TestCase (Profile.tvOS, "tvOS")]
		[TestCase (Profile.iOS, "iOS")]
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
				mtouch.AssertError (91, String.Format ("This version of Xamarin.iOS requires the {0} {1} SDK (shipped with Xcode {2}) when the managed linker is disabled. Either upgrade Xcode, or enable the managed linker by changing the Linker behaviour to Link Framework SDKs Only.", name, GetSdkVersion (profile), Configuration.XcodeVersion));
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

		/* MT0100 is a consistency check, and should never be seen (and as such can never be tested either, since there's no known test cases that would produce it) */

		[Test]
		public void MT0101 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = MTouchLinker.DontLink; // the MT0101 check happens after linking, but before AOT-compiling, so not linking makes the test faster.
				mtouch.AssemblyBuildTargets.Add ("mscorlib=framework");
				mtouch.AssemblyBuildTargets.Add ("mscorlib=framework");
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");
				mtouch.AssertError (101, "The assembly 'mscorlib' is specified multiple times in --assembly-build-target arguments.");
			}
		}

		[Test]
		public void MT0102 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = MTouchLinker.DontLink; // the MT0102 check happens after linking, but before AOT-compiling, so not linking makes the test faster.
				mtouch.AssemblyBuildTargets.Add ("mscorlib=framework=MyBinary");
				mtouch.AssemblyBuildTargets.Add ("System=dynamiclibrary=MyBinary");
				mtouch.AssemblyBuildTargets.Add ("@all=dynamiclibrary");
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");
				mtouch.AssertError (102, "The assemblies 'mscorlib' and 'System' have the same target name ('MyBinary'), but different targets ('Framework' and 'DynamicLibrary').");
			}
		}

		[Test]
		public void MT0103 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = MTouchLinker.DontLink; // the MT0103 check happens after linking, but before AOT-compiling, so not linking makes the test faster.
				mtouch.AssemblyBuildTargets.Add ("mscorlib=staticobject=MyBinary");
				mtouch.AssemblyBuildTargets.Add ("System=staticobject=MyBinary");
				mtouch.AssemblyBuildTargets.Add ("@all=staticobject");
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");
				mtouch.AssertError (103, "The static object 'MyBinary' contains more than one assembly ('mscorlib', 'System'), but each static object must correspond with exactly one assembly.");
			}
		}

		[Test]
		public void MT0105 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = MTouchLinker.DontLink; // the MT0105 check happens after linking, but before AOT-compiling, so not linking makes the test faster.
				mtouch.AssemblyBuildTargets.Add ("mscorlib=framework");
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");
				mtouch.AssertError (105, "No assembly build target was specified for 'testApp'.");
				mtouch.AssertError (105, "No assembly build target was specified for 'System'.");
				mtouch.AssertError (105, "No assembly build target was specified for 'System.Xml'.");
				mtouch.AssertError (105, "No assembly build target was specified for 'System.Core'.");
				mtouch.AssertError (105, "No assembly build target was specified for 'Mono.Dynamic.Interpreter'.");
				mtouch.AssertError (105, "No assembly build target was specified for 'Xamarin.iOS'.");
			}
		}

		[Test]
		public void MT0106 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = MTouchLinker.DontLink; // the MT0106 check happens after linking, but before AOT-compiling, so not linking makes the test faster.

				mtouch.AssemblyBuildTargets.Add ("@all=staticobject=a/b");;
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");
				mtouch.AssertError (106, "The assembly build target name 'a/b' is invalid: the character '/' is not allowed.");

				mtouch.AssemblyBuildTargets.Clear ();
				mtouch.AssemblyBuildTargets.Add ("@all=staticobject=d\\e");
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");
				mtouch.AssertError (106, "The assembly build target name 'd\\e' is invalid: the character '\\' is not allowed.");
			}
		}

		[Test]
		public void MT0108 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = MTouchLinker.DontLink; // the MT0108 check happens after linking, but before AOT-compiling, so not linking makes the test faster.
				mtouch.AssemblyBuildTargets.Add ("@all=staticobject");
				mtouch.AssemblyBuildTargets.Add ("dummy=framework");
				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");
				mtouch.AssertError (108, "The assembly build target 'dummy' did not match any assemblies.");
			}
		}

		/* MT0109 is tested in other tests (MT2018) */

		[Test]
		public void ExtensionBuild ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp (hasPlist: true);
				mtouch.Extension = true;
				mtouch.TargetVer = Configuration.sdk_version;
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildSim));
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildDev));
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

		static string GetFrameworksBindingLibrary (Profile profile)
		{
			// Path.Combine (Configuration.SourceRoot, "tests/bindings-framework-test/bin/Any CPU/Debug-unified/bindings-framework-test.dll"),
			var fn = Path.Combine (Configuration.SourceRoot, "tests", "bindings-framework-test", "bin", "Any CPU", GetConfiguration (profile), "bindings-framework-test.dll");

			if (!File.Exists (fn)) {
				var csproj = Path.Combine (Configuration.SourceRoot, "tests", "bindings-framework-test", "bindings-framework-test" + GetProjectSuffix (profile) + ".csproj");
				XBuild.Build (csproj, platform: "AnyCPU");
			}

			return fn;
		}

		public static string GetBaseLibrary (Profile profile)
		{
			switch (profile) {
			case Profile.iOS:
				return Configuration.XamarinIOSDll;
			case Profile.tvOS:
				return Configuration.XamarinTVOSDll;
			case Profile.watchOS:
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
			case Profile.iOS:
				return "Debug-unified";
			case Profile.tvOS:
				return "Debug-tvos";
			case Profile.watchOS:
				return "Debug-watchos";
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetTargetFramework (Profile profile)
		{
			switch (profile) {
			case Profile.iOS:
				return "Xamarin.iOS,v1.0";
			case Profile.tvOS:
				return "Xamarin.TVOS,v1.0";
			case Profile.watchOS:
				return "Xamarin.WatchOS,v1.0";
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetDeviceArchitecture (Profile profile)
		{
			switch (profile) {
			case Profile.iOS:
				return "armv7";
			case Profile.tvOS:
				return "arm64";
			case Profile.watchOS:
				return "armv7k";
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetSimulatorArchitecture (Profile profile)
		{
			switch (profile) {
			case Profile.iOS:
			case Profile.watchOS:
				return "i386";
			case Profile.tvOS:
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
			case Profile.iOS:
				return "Xamarin.iOS";
			case Profile.tvOS:
				return "Xamarin.TVOS";
			case Profile.watchOS:
				return "Xamarin.WatchOS";
			default:
				throw new NotImplementedException ();
			}
		}

		static string GetPlatformSimpleName (Profile profile)
		{
			switch (profile) {
			case Profile.iOS:
				return "iOS";
			case Profile.tvOS:
				return "tvOS";
			case Profile.watchOS:
				return "watchOS";
			default:
				throw new NotImplementedException ();
			}
		}

		static string GetProjectSuffix (Profile profile)
		{
			switch (profile) {
			case Profile.iOS:
				return string.Empty;
			case Profile.tvOS:
				return "-tvos";
			case Profile.watchOS:
				return "-watchos";
			default:
				throw new NotImplementedException ();
			}
		}

		public static string GetSdkVersion (Profile profile)
		{
			switch (profile) {
			case Profile.iOS:
				return Configuration.sdk_version;
			case Profile.tvOS:
				return Configuration.tvos_sdk_version;
			case Profile.watchOS:
				return Configuration.watchos_sdk_version;
			default:
				throw new NotImplementedException ();
			}
		}

		[Test]
		public void LinkAll_Frameworks ()
		{
			// Make sure that mtouch does not link with unused frameworks.

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = MTouchLinker.LinkAll;
				mtouch.AssertExecute (MTouchAction.BuildSim);

				var load_commands = ExecutionHelper.Execute ("otool", $"-l {Quote (mtouch.NativeExecutablePath)}", hide_output: true);
				Asserts.DoesNotContain ("SafariServices", load_commands, "SafariServices");
				Asserts.DoesNotContain ("GameController", load_commands, "GameController");
				Asserts.DoesNotContain ("QuickLook", load_commands, "QuickLook");
				Asserts.DoesNotContain ("NewsstandKit", load_commands, "NewsstandKit");
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_LinkWithTest (Profile profile)
		{
			// --fastdev with static registrar and linkwith library - this will fail to build if the linkwith dylib isn't linked with the corresponding native library.
			using (var mtouch = new MTouchTool ()
			{
				Profile = profile,
				Debug = true,
				FastDev = true,
				References = new string [] { GetBindingsLibrary (profile) },
				NoFastSim = true,
				Registrar = MTouchRegistrar.Static,
			}) {
				mtouch.CreateTemporaryApp_LinkWith ();
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildDev), "build");

				bool workaround_for_bug51710 = profile != Profile.iOS; // see fe17d5db9f7c
				if (workaround_for_bug51710) {
					var symbols = ExecutionHelper.Execute ("nm", Quote (mtouch.NativeExecutablePath), hide_output: true).Split ('\n');
					Assert.That (symbols, Has.Some.EndsWith (" T _theUltimateAnswer"), "Binding symbol not in executable");
				} else {
					var symbols = ExecutionHelper.Execute ("nm", Quote (mtouch.NativeExecutablePath), hide_output: true).Split ('\n');
					Assert.That (symbols, Has.None.EndsWith (" T _theUltimateAnswer"), "Binding symbol not in executable");

					symbols = ExecutionHelper.Execute ("nm", Quote (Path.Combine (mtouch.AppPath, "libbindings-test.dll.dylib")), hide_output: true).Split ('\n');
					Assert.That (symbols, Has.Some.EndsWith (" T _theUltimateAnswer"), "Binding symbol in binding library");
				}
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_NoFastSim_NoLink (Profile profile)
		{
				// --sim --nofastsim --nolink --fastdev
			using (var mtouch = new MTouchTool ()
			{
				Profile = profile,
				Debug = true,
				FastDev = true,
				References = new string [] { GetBindingsLibrary (profile) },
				NoFastSim = true,
				Linker = MTouchLinker.DontLink,
			}) {
				mtouch.CreateTemporaryApp_LinkWith ();
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildSim), "build");
			}
		}
		
		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_NoFastSim_LinkAll (Profile profile)
		{
			// --sim --nofastsim --fastdev
			using (var mtouch = new MTouchTool ()
			{
				Profile = profile,
				Debug = true,
				FastDev = true,
				References = new string [] { GetBindingsLibrary (profile) },
				NoFastSim = true,
			}) {
				mtouch.CreateTemporaryApp_LinkWith ();
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildSim), "build");
			}
		}
		
		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_NoFastSim_LinkSDK (Profile profile)
		{
			// --sim --nofastsim --linksdkonly --fastdev
			using (var mtouch = new MTouchTool ()
			{
				Profile = profile,
				Debug = true,
				FastDev = true,
				References = new string [] { GetBindingsLibrary (profile) },
				Linker = MTouchLinker.LinkSdk,
				NoFastSim = true,
			}) {
				mtouch.CreateTemporaryApp_LinkWith ();
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildSim), "build");
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
		//[TestCase (Profile.WatchOS)] // needs testing improvement
		public void FastDev_Sim (Profile profile)
		{
			// --sim --fastdev
			using (var mtouch = new MTouchTool ()
			{
				Profile = profile,
				Debug = true,
				FastDev = true,
				References = new string [] { GetBindingsLibrary (profile) },
			}) {
				mtouch.CreateTemporaryApp_LinkWith ();
				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildSim), "build");
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
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
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
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
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
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
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
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
				Profile = Profile.iOS,
				FastDev = true,
				Abi = "armv7,arm64",
			}) {
				mtouch.CreateTemporaryApp ();

				Assert.AreEqual (0, mtouch.Execute (MTouchAction.BuildDev));
				var bin = mtouch.NativeExecutablePath;
				VerifyArchitectures (bin, "arm7s/64", "armv7", "arm64");
				foreach (var dylib in Directory.GetFileSystemEntries (mtouch.AppPath, "*.dylib")) {
					if (Path.GetFileName (dylib).StartsWith ("libmono", StringComparison.Ordinal))
						continue;
					if (Path.GetFileName (dylib).StartsWith ("libxamarin", StringComparison.Ordinal))
						continue;
					VerifyArchitectures (dylib, dylib + ": arm7s/64", "armv7", "arm64");
				}
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
		[TestCase (Profile.watchOS)]
		public void FastDev_WithSpace (Profile profile)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = profile;
				mtouch.AppPath = Path.Combine (mtouch.CreateTemporaryDirectory (), "with spaces");
				Directory.CreateDirectory (mtouch.AppPath);
				if (profile == Profile.watchOS) {
					mtouch.Extension = true;
					mtouch.CreateTemporaryWatchKitExtension ();
				} else {
					mtouch.CreateTemporaryApp ();
				}
				mtouch.FastDev = true;
				mtouch.Cache = Path.Combine (mtouch.CreateTemporaryDirectory (), "with spaces");
				mtouch.Linker = MTouchLinker.LinkAll; // faster build
				mtouch.Debug = true; // faster build
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void FastSim (Profile profile)
		{
			using (var tool = new MTouchTool ()) {
				tool.Verbosity = 1;
				tool.Profile = profile;
				tool.CreateTemporaryApp ();
				tool.Linker = MTouchLinker.DontLink;
				tool.Debug = true;
				System.Threading.Thread.Sleep (1000); // HFS does not have sub-second timestamp resolution, so make sure the timestamps actually are different...
				tool.AssertExecute (MTouchAction.BuildSim);
				tool.AssertOutputPattern ("was built using fast-path for simulator"); // This is just to ensure we're actually testing fastsim. If this fails, modify the mtouch options to make this test use fastsim again.
				Assert.That (File.GetLastWriteTimeUtc (tool.RootAssembly), Is.LessThan (File.GetLastWriteTimeUtc (tool.NativeExecutablePath)), "simlauncher timestamp");
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
				mtouch.Profile = Profile.iOS;
				mtouch.CreateTemporaryApp ();

				mtouch.Abi = abi;

				var bin = Path.Combine (mtouch.AppPath, Path.GetFileNameWithoutExtension (mtouch.RootAssembly));

				Assert.AreEqual (0, mtouch.Execute (target == Target.Dev ? MTouchAction.BuildDev : MTouchAction.BuildSim));

				VerifyArchitectures (bin, abi, abi.Replace ("+llvm", string.Empty).Split (','));
			}
		}

		[Test]
		public void Architectures_Unified_FatSimulator ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.iOS;
				mtouch.CreateTemporaryApp ();

				mtouch.Abi = "i386,x86_64";

				var bin = Path.Combine (mtouch.AppPath, Path.GetFileNameWithoutExtension (mtouch.RootAssembly));
				var bin32 = Path.Combine (mtouch.AppPath, ".monotouch-32", Path.GetFileNameWithoutExtension (mtouch.RootAssembly));
				var bin64 = Path.Combine (mtouch.AppPath, ".monotouch-64", Path.GetFileNameWithoutExtension (mtouch.RootAssembly));

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
				mtouch.Profile = Profile.iOS;
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
			AssertDeviceAvailable ();

			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.tvOS;
				mtouch.Abi = abi;
				mtouch.CreateTemporaryApp ();
				      
				var bin = Path.Combine (mtouch.AppPath, Path.GetFileNameWithoutExtension (mtouch.RootAssembly));

				Assert.AreEqual (0, mtouch.Execute (target == Target.Dev ? MTouchAction.BuildDev : MTouchAction.BuildSim), "build");
				VerifyArchitectures (bin,  "arch",  target == Target.Dev ? "arm64" : "x86_64");
			}
		}

		[Test]
		public void Architectures_TVOS_Invalid ()
		{
			AssertDeviceAvailable ();

			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.tvOS;
				mtouch.CreateTemporaryApp ();

				mtouch.Abi = "armv7";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.BuildDev), "device - armv7");
				mtouch.AssertError ("MT", 75, "Invalid architecture 'ARMv7' for TVOS projects. Valid architectures are: ARM64, ARM64+LLVM");
			}
		}

		[Test]
		public void GarbageCollectors ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.AssertExecute (MTouchAction.BuildSim, "build default");
				VerifyGC (mtouch.NativeExecutablePath, "default");
			}

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.CustomArguments = new string [] { "--sgen" };
				mtouch.AssertExecute (MTouchAction.BuildSim, "build sgen");
				VerifyGC (mtouch.NativeExecutablePath, "sgen");
			}

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.CustomArguments = new string [] { "--boehm" };
				mtouch.AssertExecute (MTouchAction.BuildSim, "build boehm");
				VerifyGC (mtouch.NativeExecutablePath, "boehm");
				mtouch.AssertWarning (43, "The Boehm garbage collector is not supported. The SGen garbage collector has been selected instead.");
			}
		}

		[Test]
		[TestCase (Target.Dev, Profile.iOS, "dont link", "Release64")]
		[TestCase (Target.Dev, Profile.iOS, "link all", "Release64")]
		[TestCase (Target.Dev, Profile.iOS, "link sdk", "Release64")]
		[TestCase (Target.Dev, Profile.iOS, "monotouch-test", "Release64")]
		[TestCase (Target.Dev, Profile.iOS, "mscorlib", "Release64")]
		[TestCase (Target.Dev, Profile.iOS, "System.Core", "Release64")]
		public void BuildTestProject (Target target, Profile profile, string testname, string configuration)
		{
			if (target == Target.Dev)
				AssertDeviceAvailable ();
			
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
			XBuild.Build (csproj, configuration, platform, timeout: TimeSpan.FromMinutes (15));
		}

		[Test]
		public void ScriptedTests ()
		{
			AssertDeviceAvailable ();

			ExecutionHelper.Execute ("make", string.Format ("-C \"{0}\"", Path.Combine (Configuration.SourceRoot, "tests", "scripted")), timeout: TimeSpan.FromMinutes (10));
		}

		[Test]
		// fully linked + llvm (+thumb) + default registrar
		[TestCase (Target.Dev, MTouchLinker.Unspecified, MTouchRegistrar.Static, "armv7+llvm")]
		[TestCase (Target.Dev, MTouchLinker.Unspecified, MTouchRegistrar.Static, "armv7+llvm+thumb2")]
		// non-linked device build
		[TestCase (Target.Dev, MTouchLinker.DontLink, MTouchRegistrar.Static, "")]
		[TestCase (Target.Dev, MTouchLinker.DontLink, MTouchRegistrar.Dynamic, "")]
		// sdk device build
		[TestCase (Target.Dev, MTouchLinker.LinkSdk, MTouchRegistrar.Static, "")]
		[TestCase (Target.Dev, MTouchLinker.LinkSdk, MTouchRegistrar.Dynamic, "")]
		// fully linked device build
		[TestCase (Target.Dev, MTouchLinker.Unspecified, MTouchRegistrar.Static, "")]
		[TestCase (Target.Dev, MTouchLinker.Unspecified, MTouchRegistrar.Dynamic, "")]
		// non-linked simulator build
		[TestCase (Target.Sim, MTouchLinker.DontLink, MTouchRegistrar.Static, "")]
		[TestCase (Target.Sim, MTouchLinker.DontLink, MTouchRegistrar.Dynamic, "")]
		public void Registrar (Target target, MTouchLinker linker, MTouchRegistrar registrar, string abi)
		{
			AssertDeviceAvailable ();

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = linker;
				mtouch.Registrar = registrar;
				mtouch.Abi = abi;
				mtouch.Timeout = TimeSpan.FromMinutes (5);
				mtouch.AssertExecute (target == Target.Dev ? MTouchAction.BuildDev : MTouchAction.BuildSim, "build");
				var fi = new FileInfo (mtouch.NativeExecutablePath);
				Console.WriteLine ("Binary Size: {0} bytes = {1} kb", fi.Length, fi.Length / 1024);
			}
		}

		[Test]
		[TestCase (MTouchLinker.Unspecified)]
		[TestCase (MTouchLinker.DontLink)]
		[TestCase (MTouchLinker.LinkSdk)]
		public void ExportedSymbols (MTouchLinker linker_flag)
		{
			AssertDeviceAvailable ();

			//
			// Here we test that symbols P/Invokes and [Field] attributes references are not
			// stripped by the native linker. mtouch has to pass '-u _SYMBOL' to the native linker
			// for this to work.
			//

			using (var mtouch = new MTouchTool ()) {
				mtouch.Linker = linker_flag;
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryAppDirectory ();

				var tmpdir = mtouch.CreateTemporaryDirectory ();
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
				var bindingLib = CreateBindingLibrary (tmpdir, nativeCode, null, null, extraCode);
				var exe = CompileTestAppExecutable (tmpdir, @"
public class TestApp { 
	static void Main () {
		System.Console.WriteLine (typeof (UIKit.UIWindow).ToString ());
		System.Console.WriteLine (BindingApp.DummyField);
		BindingApp.DummyMethod ();
	}
}
",
					"-r:" + bindingLib);

				mtouch.RootAssembly = exe;
				mtouch.References = new [] { bindingLib };

				// each variation is tested twice so that we don't break when everything is found in the cache the second time around.

				mtouch.AssertExecute (MTouchAction.BuildDev, "first build");
				var symbols = ExecutionHelper.Execute ("nm", mtouch.NativeExecutablePath, hide_output: true).Split ('\n');
				Assert.That (symbols, Has.Some.EndsWith (" S _dummy_field"), "Field not found in initial build");
				Assert.That (symbols, Has.Some.EndsWith (" T _DummyMethod"), "P/invoke not found in initial build");

				ExecutionHelper.Execute ("touch", bindingLib); // This will make it so that the second identical variation won't skip the final link step.
				mtouch.AssertExecute (MTouchAction.BuildDev, "second build");
				symbols = ExecutionHelper.Execute ("nm", mtouch.NativeExecutablePath, hide_output: true).Split ('\n');
				Assert.That (symbols, Has.Some.EndsWith (" S _dummy_field"), "Field not found in second build");
				Assert.That (symbols, Has.Some.EndsWith (" T _DummyMethod"), "P/invoke not found in second build");
			}
		}


		[Test]
		public void ExportedSymbols_VerifyLinkedAwayField ()
		{
			AssertDeviceAvailable ();

			//
			// Here we test that unused P/Invokes and [Field] members are properly linked away
			// (and we do not request the native linker to preserve those symbols).
			//

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();

				var tmpdir = mtouch.CreateTemporaryDirectory ();
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
				var bindingLib = CreateBindingLibrary (tmpdir, nativeCode, null, null, extraCode);
				var exe = CompileTestAppExecutable (tmpdir, @"
public class TestApp { 
	static void Main () {
		System.Console.WriteLine (typeof (UIKit.UIWindow).ToString ());
	}
}
",
					"-r:" + bindingLib);

				mtouch.RootAssembly = exe;
				mtouch.References = new [] { bindingLib };
				mtouch.CreateTemporaryAppDirectory ();

				// test twice so that we don't break when everything is found in the cache the second time around.
				for (int iteration = 0; iteration < 2; iteration++) {
					ExecutionHelper.Execute ("touch", bindingLib); // This will make it so that the second identical variation won't skip the final link step.

					mtouch.AssertExecute (MTouchAction.BuildDev, $"build #{iteration}");

					var lines = ExecutionHelper.Execute ("nm", mtouch.NativeExecutablePath, hide_output: true).Split ('\n');
					var found_field = false;
					var found_pinvoke = false;
					foreach (var line in lines) {
						found_field |= line.EndsWith (" S _dummy_field", StringComparison.Ordinal);
						found_pinvoke |= line.EndsWith (" T _DummyMethod", StringComparison.Ordinal);
						if (found_field && found_pinvoke)
							break;
					}

					Assert.IsFalse (found_field, string.Format ("Field found for variation #{0}", iteration));
					Assert.IsFalse (found_field, string.Format ("P/Invoke found for variation #{0}", iteration));
				}
			}
		}

		[Test]
		public void LinkerWarnings ()
		{
			AssertDeviceAvailable ();

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.NoFastSim = true;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build a");
				Assert.IsFalse (mtouch.HasOutput ("ld: warning:"), "#a");
				mtouch.AssertNoWarnings ();
			}

			using (var mtouch = new MTouchTool ()) {
				var lib = Path.Combine (Configuration.SourceRoot, "tests/test-libraries/.libs/ios/libtest.x86_64.a");
				mtouch.CreateTemporaryApp ();
				mtouch.NoFastSim = true;
				mtouch.Abi = "i386";
				mtouch.GccFlags = Quote (lib);
				mtouch.AssertExecute (MTouchAction.BuildSim, "build a");
				mtouch.AssertWarning (5203, $"Native linking warning: warning: ignoring file {lib}, file was built for archive which is not the architecture being linked (i386): {lib}");
			}
		}

		[Test]
		[TestCase (MTouchLinker.LinkSdk)]
		[TestCase (MTouchLinker.DontLink)]
		public void CachedManagedLinker (MTouchLinker linker)
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=17506

			using (var mtouch = new MTouchTool ()) {
				mtouch.Linker = linker;
				mtouch.CreateTemporaryApp ();
				mtouch.CreateTemporaryCacheDirectory ();

				mtouch.AssertExecute (MTouchAction.BuildDev, "first build");
				File.Delete (mtouch.NativeExecutablePath); // This will force the final native link to succeed, while everything before has been cached.
				mtouch.AssertExecute (MTouchAction.BuildDev, "second build");
			}
		}

		[Test]
		public void MT1015 ()
		{
			// BXC 18659

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				// make sure we hit the fastsim path
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Debug = true;
				Directory.CreateDirectory (Path.Combine (mtouch.AppPath, Path.GetFileNameWithoutExtension (mtouch.AppPath)));
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertErrorPattern (1015, "Failed to create the executable '.*/testApp.app/testApp': .*/testApp.app/testApp is a directory");
			}
		}

		[Test]
		public void MT1016 ()
		{
			AssertDeviceAvailable ();

			// #20607

			using (var tool = new MTouchTool ()) {
				tool.CreateTemporaryCacheDirectory ();
				tool.CreateTemporaryApp ();

				// Create a NOTICE directory
				var notice = Path.Combine (tool.AppPath, "NOTICE");
				Directory.CreateDirectory (notice);

				tool.AssertExecuteFailure (MTouchAction.BuildDev);
				tool.AssertError (1016, "Failed to create the NOTICE file because a directory already exists with the same name.");
			}
		}

		[Test]
		public void MT1017 ()
		{
			AssertDeviceAvailable ();

			// #20607

			using (var tool = new MTouchTool ()) {
				tool.CreateTemporaryCacheDirectory ();
				tool.CreateTemporaryApp ();

				// Create a readonly NOTICE file
				var notice = Path.Combine (tool.AppPath, "NOTICE");
				File.WriteAllText (notice, "contents");
				new FileInfo (notice).IsReadOnly = true;

				tool.AssertExecute (MTouchAction.BuildDev);
				Assert.AreNotEqual ("contents", File.ReadAllText (notice), "NOTICE file written successfully");
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
			using (var mtouch = new MTouchTool ()) {
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
				mtouch.Abi = "armv7,arm64";
				mtouch.CreateTemporaryApp (code: code);
				mtouch.CreateTemporaryCacheDirectory ();

				mtouch.AssertExecuteFailure (MTouchAction.BuildDev, "build");

				mtouch.AssertOutputPattern ("Undefined symbols for architecture arm64:");
				mtouch.AssertOutputPattern (".*_OBJC_METACLASS_._Inexistent., referenced from:.*");
				mtouch.AssertOutputPattern (".*_OBJC_METACLASS_._Test_Subexistent in registrar.o.*");
				mtouch.AssertOutputPattern (".*_OBJC_CLASS_._Inexistent., referenced from:.*");
				mtouch.AssertOutputPattern (".*_OBJC_CLASS_._Test_Subexistent in registrar.o.*");
				mtouch.AssertOutputPattern (".*objc-class-ref in registrar.o.*");
				mtouch.AssertOutputPattern (".*ld: symbol.s. not found for architecture arm64.*");
				mtouch.AssertOutputPattern (".*clang: error: linker command failed with exit code 1 .use -v to see invocation.*");

				mtouch.AssertErrorPattern ("MT", 5210, "Native linking failed, undefined symbol: _OBJC_METACLASS_._Inexistent. Please verify that all the necessary frameworks have been referenced and native libraries are properly linked in.");
				mtouch.AssertErrorPattern ("MT", 5211, "Native linking failed, undefined Objective-C class: Inexistent. The symbol ._OBJC_CLASS_._Inexistent. could not be found in any of the libraries or frameworks linked with your application.");
				mtouch.AssertErrorPattern ("MT", 5202, "Native linking failed. Please review the build log.");
			}
		}

		[Test]
		public void TestCaseMismatchedAssemblyName ()
		{
			// desk #90367 (and others in the past as well)
			using (var mtouch = new MTouchTool ()) {

				var testDir = mtouch.CreateTemporaryDirectory ();
				var app = Path.Combine (testDir, "testApp.app");
				Directory.CreateDirectory (testDir);

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

				var tests = new [] {
					new { Name = "linkall", Abi = "armv7s", Link = MTouchLinker.Unspecified },
					new { Name = "dontlink", Abi = "armv7s", Link = MTouchLinker.DontLink },
					new { Name = "dual", Abi = "armv7,arm64", Link = MTouchLinker.Unspecified },
				};

				mtouch.AppPath = app;
				mtouch.RootAssembly = exe;
				mtouch.References = new [] { DLL };

				foreach (var test in tests) {
					mtouch.Abi = test.Abi;
					mtouch.Linker = test.Link;
					Directory.CreateDirectory (app);
					mtouch.AssertExecute (MTouchAction.BuildDev, "build: " + test.Name);
					check (test.Name);
					Directory.Delete (app, true);
				}
			}
		}

		[Test]
		public void TestDuplicatedFatApp ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.Abi = "armv7,arm64";
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");
				FileAssert.Exists (Path.Combine (mtouch.AppPath, "testApp.exe"));
				// Don't check for mscorlib.dll, there might be two versions of it (since Xamarin.iOS.dll depends on it), or there might not.
				FileAssert.Exists (Path.Combine (mtouch.AppPath, ".monotouch-32", "Xamarin.iOS.dll"));
			}
		}

		[Test]
		public void TestAllLoad ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.GccFlags = "-all_load";
				mtouch.Abi = "armv7,arm64";
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");
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
		[TestCase (Profile.iOS)]
		public void DlsymDisabled (Profile profile)
		{
			using (var tool = new MTouchTool ()) {
				tool.Profile = profile;
				tool.Verbosity = 5;
				tool.Cache = Path.Combine (tool.CreateTemporaryDirectory (), "mtouch-test-cache");
				tool.CreateTemporaryApp (code: "using UIKit; class C { static void Main (string[] args) { UIApplication.Main (args); } }");
				tool.FastDev = true;
				tool.Dlsym = false;

				Assert.AreEqual (0, tool.Execute (MTouchAction.BuildDev));
			}
		}

		[Test]
		public void PInvokeWrapperGenerationTest ()
		{
			using (var tool = new MTouchTool ()) {
				tool.Profile = Profile.watchOS;
				tool.CreateTemporaryCacheDirectory ();
				tool.Verbosity = 5;
				tool.Extension = true;
				tool.CreateTemporaryWatchKitExtension ();

				tool.FastDev = true;
				Assert.AreEqual (0, tool.Execute (MTouchAction.BuildDev), "build");

				Assert.IsTrue (File.Exists (Path.Combine (tool.AppPath, "libpinvokes.dylib")), "libpinvokes.dylib existence");

				var otool_output = ExecutionHelper.Execute ("otool", $"-l {Quote (Path.Combine (tool.AppPath, "libpinvokes.dylib"))}", hide_output: true);
				Assert.That (otool_output, Does.Contain ("LC_ID_DYLIB"), "output contains LC_ID_DYLIB");

				var lines = otool_output.Split (new char [] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < lines.Length; i++) {
					if (lines [i].Contains ("LC_ID_DYLIB")) {
						Assert.That (lines [i + 2], Does.Contain ("name @rpath/libpinvokes.dylib "), "LC_ID_DYLIB");
						break;
					}
				}

				Assert.AreEqual (0, tool.Execute (MTouchAction.BuildDev), "cached build");
			}
		}

		[Test]
		public void LinkWithNoLibrary ()
		{
			using (var tool = new MTouchTool ()) {
				tool.Profile = Profile.iOS;
				tool.CreateTemporaryApp (code: @"
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
[assembly: LinkWith (Dlsym = DlsymOption.Required)]
class C {
	[DllImport (""libsqlite3"")]
	static extern void sqlite3_column_database_name16 ();
	static void Main ()
	{
	}
}
");
				tool.NoFastSim = true;
				tool.Dlsym = false;
				tool.Linker = MTouchLinker.LinkSdk;
				Assert.AreEqual (0, tool.Execute (MTouchAction.BuildDev), "build");
			}
		}

		[Test]
		public void WatchExtensionWithFramework ()
		{
			using (var exttool = new MTouchTool ()) {
				exttool.Profile = Profile.watchOS;
				exttool.CreateTemporaryCacheDirectory ();
				exttool.Verbosity = 5;

				exttool.Extension = true;
				exttool.CreateTemporaryWatchKitExtension ();
				exttool.Frameworks.Add (Path.Combine (Configuration.SourceRoot, "tests/test-libraries/.libs/watchos/XTest.framework"));
				exttool.AssertExecute (MTouchAction.BuildSim, "build extension");

				using (var apptool = new MTouchTool ()) {
					apptool.Profile = Profile.iOS;
					apptool.CreateTemporaryCacheDirectory ();
					apptool.Verbosity = exttool.Verbosity;
					apptool.Linker = MTouchLinker.DontLink; // faster
					apptool.CreateTemporaryApp ();
					apptool.AppExtensions.Add (exttool);
					apptool.AssertExecute (MTouchAction.BuildSim, "build app");

					Assert.IsFalse (Directory.Exists (Path.Combine (apptool.AppPath, "Frameworks", "XTest.framework")), "framework inexistence");
					Assert.IsTrue (Directory.Exists (Path.Combine (exttool.AppPath, "Frameworks", "XTest.framework")), "extension framework existence");
				}
			}
		}

		[Test]
		public void OnlyExtensionWithFramework ()
		{
			// if an extension references a framework, and the main app does not,
			// the framework should still be copied to the main app's Framework directory.
			using (var exttool = new MTouchTool ()) {
				exttool.Profile = Profile.iOS;
				exttool.CreateTemporaryCacheDirectory ();
				exttool.Linker = MTouchLinker.DontLink; // faster

				exttool.Extension = true;
				exttool.CreateTemporararyServiceExtension ();
				exttool.Frameworks.Add (Path.Combine (Configuration.SourceRoot, "tests/test-libraries/.libs/ios/XTest.framework"));
				exttool.AssertExecute (MTouchAction.BuildSim, "build extension");

				using (var apptool = new MTouchTool ()) {
					apptool.Profile = Profile.iOS;
					apptool.CreateTemporaryCacheDirectory ();
					apptool.Verbosity = exttool.Verbosity;
					apptool.CreateTemporaryApp ();
					apptool.AppExtensions.Add (exttool);
					apptool.Linker = MTouchLinker.DontLink; // faster
					apptool.AssertExecute (MTouchAction.BuildSim, "build app");

					Assert.IsTrue (Directory.Exists (Path.Combine (apptool.AppPath, "Frameworks", "XTest.framework")), "framework exists");
					Assert.IsFalse (Directory.Exists (Path.Combine (exttool.AppPath, "Frameworks")), "extension framework inexistence");
				}
			}
		}

		[Test]
		public void OnlyExtensionWithBindingFramework ()
		{
			// if an extension references a framework (from a binding library, and the main app does not,
			// the framework should still be copied to the main app's Framework directory.
			using (var exttool = new MTouchTool ()) {
				exttool.Profile = Profile.iOS;
				exttool.CreateTemporaryCacheDirectory ();
				exttool.Linker = MTouchLinker.DontLink; // faster

				exttool.Extension = true;
				exttool.References = new string []
				{
					GetFrameworksBindingLibrary (exttool.Profile),
				};
				exttool.CreateTemporararyServiceExtension (code: @"using UserNotifications;
[Foundation.Register (""NotificationService"")]
public partial class NotificationService : UNNotificationServiceExtension
{
	protected NotificationService (System.IntPtr handle) : base (handle)
	{
		System.Console.WriteLine (Bindings.Test.CFunctions.theUltimateAnswer ());
	}
}", extraArg: Quote ("-r:" + exttool.References [0]));
				exttool.AssertExecute (MTouchAction.BuildSim, "build extension");

				using (var apptool = new MTouchTool ()) {
					apptool.Profile = Profile.iOS;
					apptool.CreateTemporaryCacheDirectory ();
					apptool.Verbosity = exttool.Verbosity;
					apptool.CreateTemporaryApp ();
					apptool.AppExtensions.Add (exttool);
					apptool.AssertExecute (MTouchAction.BuildSim, "build app");

					Assert.IsTrue (Directory.Exists (Path.Combine (apptool.AppPath, "Frameworks", "XTest.framework")), "framework exists");
					Assert.IsFalse (Directory.Exists (Path.Combine (exttool.AppPath, "Frameworks")), "extension framework inexistence");
				}
			}
		}

		[Test]
		[TestCase (MTouchLinker.DontLink)]
		[TestCase (MTouchLinker.LinkAll)]
		// There shouldn't be a need to test LinkSdk as well.
		public void OnlyDebugFileChange (MTouchLinker linker_options)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.iOS;
				var tmp = mtouch.CreateTemporaryDirectory ();
				mtouch.CreateTemporaryCacheDirectory ();

				// Create a sample exe
				var code = "public class TestApp { static void Main () { System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ()); } }";
				var exe = MTouch.CompileTestAppExecutable (tmp, code, "/debug:full");

				mtouch.AppPath = mtouch.CreateTemporaryDirectory ();
				mtouch.RootAssembly = exe;
				mtouch.Debug = true;
				mtouch.Linker = linker_options;

				// Build app
				mtouch.AssertExecute (MTouchAction.BuildSim);

				var exePath = Path.Combine (mtouch.AppPath, Path.GetFileName (exe));
				var mdbPath = exePath + ".mdb";
				var exeStamp = File.GetLastWriteTimeUtc (exePath);
				var mdbStamp = File.GetLastWriteTimeUtc (mdbPath);

				System.Threading.Thread.Sleep (1000); // HFS does not have sub-second timestamp resolution, so make sure the timestamps actually change...
				// Recompile the exe, adding only whitespace. This will only change the debug files
				MTouch.CompileTestAppExecutable (tmp, "\n\n" + code + "\n\n", "/debug:full");

				// Rebuild the app
				mtouch.AssertExecute (MTouchAction.BuildSim);

				// The mdb files should be updated, but the exe should not.
				Assert.AreEqual (exeStamp, File.GetLastWriteTimeUtc (exePath), "exe no change");
				Assert.IsTrue (File.Exists (mdbPath), "mdb existence");
				Assert.AreNotEqual (mdbStamp, File.GetLastWriteTimeUtc (mdbPath), "mdb changed");
			}
		}

		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
		public void MT2010 (Profile profile)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = profile;
				mtouch.CreateTemporaryApp ();

				mtouch.HttpMessageHandler = "Dummy";
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.BuildSim));
				mtouch.AssertError (2010, "Unknown HttpMessageHandler `Dummy`. Valid values are HttpClientHandler (default), CFNetworkHandler or NSUrlSessionHandler");
			}
		}

		[Test]
		public void MT2015 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.watchOS;
				mtouch.CreateTemporaryWatchKitExtension ();
				mtouch.Extension = true;

				mtouch.HttpMessageHandler = "HttpClientHandler";
				mtouch.AssertExecute (MTouchAction.BuildSim);
				mtouch.AssertError (2015, "Invalid HttpMessageHandler `HttpClientHandler` for watchOS. The only valid value is NSUrlSessionHandler.");

				mtouch.HttpMessageHandler = "CFNetworkHandler";
				mtouch.AssertExecute (MTouchAction.BuildSim);
				mtouch.AssertError (2015, "Invalid HttpMessageHandler `CFNetworkHandler` for watchOS. The only valid value is NSUrlSessionHandler.");

				mtouch.HttpMessageHandler = "Dummy";
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim);
				mtouch.AssertError (2015, "Invalid HttpMessageHandler `Dummy` for watchOS. The only valid value is NSUrlSessionHandler.");
			}
		}

		[Test]
		public void MT2018_a ()
		{
			using (var mtouch = new MTouchTool ()) {
				// Create a library, copy it to a different directory, and then
				// pass both as -r:.. to mtouch. Due to assembly resolution being cached,
				// this will *not* show the MT2018 error (in fact I don't know if it's possible
				// to run into MT2018 at all).
				var tmpA = mtouch.CreateTemporaryDirectory ();
				var dllA = CompileTestAppCode ("library", tmpA, "public class X {}", appName: "testLib");

				var tmpB = mtouch.CreateTemporaryDirectory ();
				var dllB = Path.Combine (tmpB, Path.GetFileName (dllA));
				File.Copy (dllA, dllB);

				mtouch.CreateTemporaryApp (code: "public class C { static void Main () { System.Console.WriteLine (typeof (X)); System.Console.WriteLine (typeof (UIKit.UIWindow)); } }", extraArg: "-r:" + Quote (dllA));
				mtouch.References = new string [] { dllA, dllB };
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
				mtouch.AssertWarningPattern (109, "The assembly 'testLib.dll' was loaded from a different path than the provided path .provided path: .*/testLib.dll, actual path: .*/testLib.dll..");
			}
		}

		[Test]
		public void MT2018_b ()
		{
			using (var mtouch = new MTouchTool ()) {
				// Create a library named as an SDK assembly, and then
				// pass both as -r:.. to mtouch, this library being the first one.
				// Due to assembly resolution being cached,
				// this will *not* show the MT2018 error (in fact I don't know if it's possible
				// to run into MT2018 at all).
				var tmpA = mtouch.CreateTemporaryDirectory ();
				var dllA = CompileTestAppCode ("library", tmpA, "public class X {}", appName: "System.Net.Http");

				var dllB = Path.Combine (Configuration.SdkRootXI, "lib", "mono", "Xamarin.iOS", Path.GetFileName (dllA));

				mtouch.CreateTemporaryApp (code: "public class C { static void Main () { System.Console.WriteLine (typeof (X)); System.Console.WriteLine (typeof (UIKit.UIWindow)); } }", extraArg: "-r:" + Quote (dllA));
				mtouch.References = new string [] { dllA, dllB };

				// Without the linker we'll just copy the references, and not actually run into problems if we copy one that doesn't work
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
				mtouch.AssertWarningPattern (109, "The assembly 'System.Net.Http.dll' was loaded from a different path than the provided path .provided path: .*/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/System.Net.Http.dll, actual path: .*CreateTemporaryDirectory.*/System.Net.Http.dll..");

				// With the linker, we'll find out that we've loaded the right one.
				mtouch.Linker = MTouchLinker.LinkSdk;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
				mtouch.AssertWarningPattern (109, "The assembly 'System.Net.Http.dll' was loaded from a different path than the provided path .provided path: .*/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/System.Net.Http.dll, actual path: .*CreateTemporaryDirectory.*/System.Net.Http.dll..");
			}
		}

		[Test]
		public void MT2018_c ()
		{
			using (var mtouch = new MTouchTool ()) {
				// Create a library named as an SDK assembly, and then
				// pass both as -r:.. to mtouch, the SDK library being the first one.
				// Due to assembly resolution being cached,
				// this will *not* show the MT2018 error (in fact I don't know if it's possible
				// to run into MT2018 at all).
				var tmpA = mtouch.CreateTemporaryDirectory ();
				var dllA = CompileTestAppCode ("library", tmpA, "public class X {}", appName: "System.Net.Http");

				var dllB = Path.Combine (Configuration.SdkRootXI, "lib", "mono", "Xamarin.iOS", Path.GetFileName (dllA));

				mtouch.CreateTemporaryApp (code: "public class C { static void Main () { System.Console.WriteLine (typeof (X)); System.Console.WriteLine (typeof (UIKit.UIWindow)); } }", extraArg: "-r:" + Quote (dllA));
				mtouch.References = new string [] { dllB, dllA };

				// Without the linker we'll just copy the references, and not actually run into problems if we copy one that doesn't work
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
				mtouch.AssertWarningPattern (109, "The assembly 'System.Net.Http.dll' was loaded from a different path than the provided path .provided path: .*CreateTemporaryDirectory.*/System.Net.Http.dll, actual path: .*/Library/Frameworks/Xamarin.iOS.framework/Versions/.*/lib/mono/Xamarin.iOS/System.Net.Http.dll..");

				// With the linker, we'll find out that the loaded reference doesn't work.
				mtouch.Linker = MTouchLinker.LinkSdk;
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (2002, "Failed to resolve \"X\" reference from \"System.Net.Http, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\"");
			}
		}

		[Test]
		public void AutoLinkWithSqlite ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.iOS;
				mtouch.CreateTemporaryApp (code: @"
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

[assembly: LinkWith (ForceLoad = true)]

[Preserve (AllMembers = true)]
public class TestApp {
	[DllImport (""sqlite3"")]
	static extern void sqlite3_exec ();

	static void Main ()
	{
		System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ());
	}
}
");
				mtouch.Linker = MTouchLinker.DontLink; // just to make the test run faster.
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
			}
		}

#region Helper functions
		static string CompileUnifiedTestAppExecutable (string targetDirectory, string code = null, string extraArg = "")
		{
			return CompileTestAppExecutable (targetDirectory, code, extraArg, profile: Profile.iOS);
		}

		public static string CompileTestAppExecutable (string targetDirectory, string code = null, string extraArg = "", Profile profile = Profile.iOS, string appName = "testApp", string extraCode = null)
		{
			if (code == null)
				code = "public class TestApp { static void Main () { System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ()); } }";
			if (extraCode != null)
				code += extraCode;

			return CompileTestAppCode ("exe", targetDirectory, code, extraArg, profile, appName);
		}

		public static string CompileTestAppLibrary (string targetDirectory, string code, string extraArg = null, Profile profile = Profile.iOS, string appName = "testApp")
		{
			return CompileTestAppCode ("library", targetDirectory, code, extraArg, profile, appName);
		}

		public static string CompileTestAppCode (string target, string targetDirectory, string code, string extraArg = "", Profile profile = Profile.iOS, string appName = "testApp")
		{
			var ext = target == "exe" ? "exe" : "dll";
			var cs = Path.Combine (targetDirectory, "testApp.cs");
			var assembly = Path.Combine (targetDirectory, appName + "." + ext);
			var root_library = GetBaseLibrary (profile);

			File.WriteAllText (cs, code);

			string output;
			StringBuilder args = new StringBuilder ();
			string fileName = GetCompiler (profile, args);
			args.AppendFormat ($" /noconfig /t:{target} /nologo /out:{Quote (assembly)} /r:{Quote (root_library)} {Quote (cs)} {extraArg}");
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

				args.Append (" -target:").Append (outputPath.EndsWith (".dll", StringComparison.Ordinal) ? "library" : "exe");
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
		public static string CompileTestAppExecutableLinkWith (string targetDirectory, Profile profile = Profile.iOS)
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
	
		static void VerifyGC (string file, string message)
		{
			var symbols = ExecutionHelper.Execute ("nm", file, hide_output: true);
			var _sgen_gc_lock = symbols.Contains ("_sgen_gc_lock");
			if (!_sgen_gc_lock) {
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
			if (f.IndexOf (' ') == -1 && f.IndexOf ('\'') == -1 && f.IndexOf (',') == -1 && f.IndexOf ('\\') == -1)
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

		public static void AssertDeviceAvailable ()
		{
			if (!Configuration.include_device)
				Assert.Ignore ("This build does not include device support.");
		}
#endregion
	}

	class McsException : Exception {
		public McsException (string output)
			: base (output)
		{
		}
	}
}
