using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Xamarin.Utils;
using Xamarin.Tests;

using NUnit.Framework;

using MTouchLinker = Xamarin.Tests.LinkerOption;
using MTouchRegistrar = Xamarin.Tests.RegistrarOption;

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

	[TestFixture]
	public class MTouch
	{
		[Test]
		//[TestCase (Profile.iOS)] // tested as part of the watchOS case below, since that builds both for iOS and watchOS.
		[TestCase (Profile.tvOS)]
		[TestCase (Profile.watchOS)]
		public void Profiling (Profile profile)
		{
			using (var mtouch = new MTouchTool ()) {
				var tmpdir = mtouch.CreateTemporaryDirectory ();
				MTouchTool ext = null;
				if (profile == Profile.watchOS) {
					mtouch.Profile = Profile.iOS;

					ext = new MTouchTool ();
					ext.Profile = profile;
					ext.Profiling = true;
					ext.SymbolList = Path.Combine (tmpdir, "extsymbollist.txt");
					ext.CreateTemporaryWatchKitExtension ();
					ext.CreateTemporaryDirectory ();
					mtouch.AppExtensions.Add (ext);
					ext.AssertExecute (MTouchAction.BuildDev, "ext build");
				} else {
					mtouch.Profile = profile;
				}
				mtouch.CreateTemporaryApp ();
				mtouch.CreateTemporaryCacheDirectory ();

				mtouch.DSym = false; // faster test
				mtouch.MSym = false; // faster test
				mtouch.NoStrip = true; // faster test

				mtouch.Profiling = true;
				mtouch.SymbolList = Path.Combine (tmpdir, "symbollist.txt");
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				var profiler_symbol = "_mono_profiler_init_log";

				var symbols = (IEnumerable<string>) File.ReadAllLines (mtouch.SymbolList);
				Assert.That (symbols, Contains.Item (profiler_symbol), profiler_symbol);

				symbols = GetNativeSymbols (mtouch.NativeExecutablePath);
				Assert.That (symbols, Contains.Item (profiler_symbol), $"{profiler_symbol} nm");

				if (ext != null) {
					symbols = File.ReadAllLines (ext.SymbolList);
					Assert.That (symbols, Contains.Item (profiler_symbol), $"{profiler_symbol} - extension");

					symbols = GetNativeSymbols (ext.NativeExecutablePath);
					Assert.That (symbols, Contains.Item (profiler_symbol), $"{profiler_symbol} extension nm");

				}
			}
		}

		[Test]
		public void ExceptionMarshaling ()
		{
			using (var mtouch = new MTouchTool ()) {
				var code = @"
class X : Foundation.NSObject {
	public X ()
	{
		ValueForKey (null); // calls xamarin_IntPtr_objc_msgSend_IntPtr, so that it's not linked away.
	}
}
";
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: code);
				mtouch.CustomArguments = new string [] { "--marshal-objectivec-exceptions=throwmanagedexception", "--dlsym:+Xamarin.iOS.dll" };
				mtouch.Debug = false; // make sure the output is stripped
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Contain ("_xamarin_pinvoke_wrapper_objc_msgSend"), "symbols");
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Contain ("_xamarin_IntPtr_objc_msgSend_IntPtr"), "symbols 2");

				// build again with llvm enabled
				mtouch.Abi = "arm64+llvm";
				mtouch.AssertExecute (MTouchAction.BuildDev, "build llvm");

				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Contain ("_xamarin_pinvoke_wrapper_objc_msgSend"), "symbols llvm");
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Contain ("_xamarin_IntPtr_objc_msgSend_IntPtr"), "symbols llvm 2");
			}
		}

		[Test]
		[TestCase (NormalizationForm.FormC)]
		[TestCase (NormalizationForm.FormD)]
		[TestCase (NormalizationForm.FormKC)]
		[TestCase (NormalizationForm.FormKD)]
		public void StringNormalization (NormalizationForm form)
		{
			var str = "Tūhono".Normalize (form);

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (appName: str);
				mtouch.Linker = MTouchLinker.LinkSdk;
				mtouch.Verbosity = 9;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
			}
		}

		[Test]
		public void SymbolCollectionWithDlsym ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=57826

			using (var mtouch = new MTouchTool ()) {
				var tmpdir = mtouch.CreateTemporaryDirectory ();
				mtouch.CreateTemporaryCacheDirectory ();

				var externMethod = @"
class X {
	[System.Runtime.InteropServices.DllImport (""__Internal"")]
	static extern void xamarin_start_wwan ();
}
";

				var codeDll = externMethod + @"
public class A {}
";
				var codeExe = externMethod + @"
public class B : A {}
";

				var dllPath = CompileTestAppLibrary (tmpdir, codeDll, profile: Profile.iOS, appName: "A");

				mtouch.References = new string [] { dllPath };
				mtouch.CreateTemporaryApp (extraCode: codeExe, extraArg: $"-r:{StringUtils.Quote (dllPath)}");
				mtouch.Linker = MTouchLinker.LinkSdk;
				mtouch.Debug = false;
				mtouch.CustomArguments = new string [] { "--dlsym:+A.dll", "--dlsym:-testApp.exe" };
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				var symbols = GetNativeSymbols (mtouch.NativeExecutablePath);
				Assert.That (symbols, Does.Contain ("_xamarin_start_wwan"), "symb");
			}
		}

		[Test]
		public void FatAppFiles ()
		{
			AssertDeviceAvailable ();

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.Abi = "armv7,arm64";
				mtouch.TargetVer = "10.3"; // otherwise 32-bit build isn't possible
				mtouch.DSym = false; // speeds up the test
				mtouch.MSym = false; // speeds up the test
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				var expectedFiles = new string []
				{
					"NOTICE",
					"testApp",
					"testApp.aotdata.armv7",
					"testApp.aotdata.arm64",
					"testApp.exe",
					"mscorlib.dll",
					"mscorlib.aotdata.armv7",
					"mscorlib.aotdata.arm64",
					"Xamarin.iOS.dll",
					"Xamarin.iOS.aotdata.armv7",
					"Xamarin.iOS.aotdata.arm64",

				};
				var notExpectedFiles = new string [] {
					/* mscorlib.dll and Xamarin.iOS.dll can differ between 32-bit and 64-bit, other assemblies shouldn't */
					/* these files should end up in the root app directory, not the size-specific subdirectory */
					".monotouch-32/testApp.exe",
					".monotouch-32/testApp.aotdata.armv7",
					".monotouch-64/testApp.exe",
					".monotouch-64/testApp.aotdata.arm64",
					".monotouch-64/System.dll",
					".monotouch-64/System.aotdata.arm64",
				};
				var allFiles = Directory.GetFiles (mtouch.AppPath, "*", SearchOption.AllDirectories);
				var expectedFailed = new List<string> ();
				foreach (var expected in expectedFiles) {
					if (allFiles.Any ((v) => v.EndsWith (expected, StringComparison.Ordinal)))
						continue;
					expectedFailed.Add (expected);
				}
				Assert.IsEmpty (expectedFailed, "expected files");

				var notExpectedFailed = new List<string> ();
				foreach (var notExpected in notExpectedFiles) {
					if (!allFiles.Any ((v) => v.EndsWith (notExpected, StringComparison.Ordinal)))
						continue;
					notExpectedFailed.Add (notExpected);
				}
				Assert.IsEmpty (notExpectedFailed, "not expected files");
			}
		}

		[Test]
		[TestCase ("code sharing 32-bit", "armv7+llvm", new string [] { "@sdk=framework=Xamarin.Sdk", "@all=staticobject" })]
		[TestCase ("code sharing 64-bit", "arm64+llvm", new string [] { "@sdk=framework=Xamarin.Sdk", "@all=staticobject" })]
		[TestCase ("32-bit", "armv7+llvm", new string [] { } )]
		[TestCase ("64-bit", "arm64+llvm", new string [] { })]
		public void CodeSharingLLVM (string name, string abi, string[] assembly_build_targets)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.Abi = abi;
				mtouch.AssemblyBuildTargets.AddRange (assembly_build_targets);
				mtouch.Debug = false;
				mtouch.NoStrip = true; // faster test
				mtouch.NoSymbolStrip = string.Empty; // faster test
				mtouch.Verbosity = 4; // This is needed to get mtouch to print the output we're verifying
				mtouch.TargetVer = "10.3"; // otherwise 32-bit builds aren't possible
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");
				// Check that --llvm is passed to the AOT compiler for every assembly we AOT.
				var assemblies_checked = 0;
				mtouch.ForAllOutputLines ((line) =>
				{
					if (!line.Contains ("arm-darwin-mono-sgen") && !line.Contains ("arm64-darwin-mono-sgen"))
						return;
					StringAssert.Contains (" --llvm ", line, "aot command must pass --llvm to the AOT compiler");
					assemblies_checked++;
				});
				Assert.That (assemblies_checked, Is.AtLeast (3), "We build at least 3 dlls, so we must have had at least 3 asserts above."); // mscorlib.dll, Xamarin.iOS.dll, System.dll, theApp.exe
			}
		}

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
				mtouch.NoStrip = true;
				DateTime dt = DateTime.MinValue;

				mtouch.DSym = false; // we don't need the dSYMs for this test, so disable them to speed up the test.
				mtouch.MSym = false; // we don't need the mSYMs for this test, so disable them to speed up the test.
				mtouch.AssertExecute (MTouchAction.BuildDev, "first build");
				Console.WriteLine ("first build done");

				dt = DateTime.Now;
				System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older.

				mtouch.AssertExecute (MTouchAction.BuildDev, "second build");
				Console.WriteLine ("second build done");
				mtouch.AssertNoneModified (dt, name + " - second build");

				// Test that a rebuild (where something changed, in this case the .exe)
				// actually work. We compile with custom code to make sure it's different
				// from the previous exe we built.
				var subDir = Cache.CreateTemporaryDirectory ();
				var exe2 = CompileTestAppExecutable (subDir,
					/* the code here only changes the class name (default: 'TestApp1' changed to 'TestApp2') to minimize the related
					 * changes (there should be no changes in Xamarin.iOS.dll nor mscorlib.dll, even after linking) */
					code: codeB, profile: mtouch.Profile);
				File.Copy (exe2, mtouch.RootAssembly, true);

				dt = DateTime.Now;
				System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older.

				mtouch.AssertExecute (MTouchAction.BuildDev, "third build");
				Console.WriteLine ("third build done");
				mtouch.AssertNoneModified (dt, name + " - third build", "testApp", "testApp.exe", "testApp.aotdata.armv7", "testApp.aotdata.arm64");

				// Test that a complete rebuild occurs when command-line options changes
				dt = DateTime.Now;
				System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older.

				mtouch.GccFlags = "-v";
				mtouch.AssertExecute (MTouchAction.BuildDev, "fourth build");
				Console.WriteLine ("fourth build done");
				mtouch.AssertAllModified (dt, name + " - fourth build", "NOTICE");
			}
		}

		[Test]
		public void RebuildTest_Intl ()
		{
			using (var tool = new MTouchTool ()) {
				tool.Profile = Profile.iOS;
				tool.I18N = I18N.West;
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
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.Verbosity = 4; // This is required to get the debug output we're testing for
				mtouch.AssertExecute (MTouchAction.BuildSim, "build 1");
				mtouch.AssertOutputPattern ("Linking .*/testApp.exe into .*/2-PreBuild using mode 'None'");
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
				extension.CreateTemporaryServiceExtension (extraCode: codeA);
				extension.CreateTemporaryCacheDirectory ();
				extension.Abi = abi;
				extension.TargetVer = "10.3"; // otherwise 32-bit builds aren't possible
				extension.Debug = debug;
				extension.AssemblyBuildTargets.AddRange (assembly_build_targets);
				extension.DSym = false; // faster test
				extension.MSym = false; // faster test
				extension.NoStrip = true; // faster test
				extension.AssertExecute (MTouchAction.BuildDev, "extension build");

				using (var mtouch = new MTouchTool ()) {
					mtouch.AppExtensions.Add (extension);
					mtouch.CreateTemporaryApp (extraCode: codeA);
					mtouch.CreateTemporaryCacheDirectory ();
					mtouch.Abi = abi;
					mtouch.TargetVer = "10.3"; // otherwise 32-bit builds aren't possible
					mtouch.Debug = debug;
					mtouch.AssemblyBuildTargets.AddRange (assembly_build_targets);
					mtouch.DSym = false; // faster test
					mtouch.MSym = false; // faster test
					mtouch.NoStrip = true; // faster test

					var timestamp = DateTime.MinValue;

					mtouch.AssertExecute (MTouchAction.BuildDev, "first build");
					Console.WriteLine ($"{DateTime.Now} **** FIRST BUILD DONE ****");

					timestamp = DateTime.Now;
					System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older. HFS+ has a 1s timestamp resolution :(

					mtouch.AssertExecute (MTouchAction.BuildDev, "second build");
					Console.WriteLine ($"{DateTime.Now} **** SECOND BUILD DONE ****");

					mtouch.AssertNoneModified (timestamp, name);
					extension.AssertNoneModified (timestamp, name);

					// Touch the extension's executable, nothing should change
					new FileInfo (extension.RootAssembly).LastWriteTimeUtc = DateTime.UtcNow;
					mtouch.AssertExecute (MTouchAction.BuildDev, "touch extension executable");
					Console.WriteLine ($"{DateTime.Now} **** TOUCH EXTENSION EXECUTABLE DONE ****");
					mtouch.AssertNoneModified (timestamp, name);
					extension.AssertNoneModified (timestamp, name);

					// Touch the main app's executable, nothing should change
					new FileInfo (mtouch.RootAssembly).LastWriteTimeUtc = DateTime.UtcNow;
					mtouch.AssertExecute (MTouchAction.BuildDev, "touch main app executable");
					Console.WriteLine ($"{DateTime.Now} **** TOUCH MAIN APP EXECUTABLE DONE ****");
					mtouch.AssertNoneModified (timestamp, name);
					extension.AssertNoneModified (timestamp, name);

					// Test that a rebuild (where something changed, in this case the .exe)
					// actually work. We compile with custom code to make sure it's different
					// from the previous exe we built.
					//
					// The code change is minimal: only changes the class name (default: 'TestApp1' changed to 'TestApp2') to minimize the related
					// changes (there should be no changes in Xamarin.iOS.dll nor mscorlib.dll, even after linking)

					timestamp = DateTime.Now;
					System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older. HFS+ has a 1s timestamp resolution :(

					// Rebuild the extension's .exe
					extension.CreateTemporaryServiceExtension (extraCode: codeB);
					mtouch.AssertExecute (MTouchAction.BuildDev, "change extension executable");
					Console.WriteLine ($"{DateTime.Now} **** CHANGE EXTENSION EXECUTABLE DONE ****");
					mtouch.AssertNoneModified (timestamp, name);
					extension.AssertNoneModified (timestamp, name, "testServiceExtension", "testServiceExtension.aotdata.armv7", "testServiceExtension.aotdata.arm64", "testServiceExtension.dll");

					timestamp = DateTime.Now;
					System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older. HFS+ has a 1s timestamp resolution :(

					// Rebuild the main app's .exe
					mtouch.CreateTemporaryApp (extraCode: codeB);
					mtouch.AssertExecute (MTouchAction.BuildDev, "change app executable");
					Console.WriteLine ($"{DateTime.Now} **** CHANGE APP EXECUTABLE DONE ****");
					mtouch.AssertNoneModified (timestamp, name, "testApp", "testApp.aotdata.armv7", "testApp.aotdata.arm64", "testApp.exe");
					extension.AssertNoneModified (timestamp, name);

					timestamp = DateTime.Now;
					System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older. HFS+ has a 1s timestamp resolution :(

					// Add a config file to the extension. This file should be added to the app, and the AOT-compiler re-executed for the root assembly.
					File.WriteAllText (extension.RootAssembly + ".config", "<configuration></configuration>");
					mtouch.AssertExecute (MTouchAction.BuildDev, "add config to extension dll");
					Console.WriteLine ($"{DateTime.Now} **** ADD CONFIG TO EXTENSION DONE ****");
					mtouch.AssertNoneModified (timestamp, name);
					extension.AssertNoneModified (timestamp, name, "testServiceExtension.dll.config", "testServiceExtension", "testServiceExtension.aotdata.armv7", "testServiceExtension.aotdata.arm64");
					CollectionAssert.Contains (Directory.EnumerateFiles (extension.AppPath, "*", SearchOption.AllDirectories).Select ((v) => Path.GetFileName (v)), "testServiceExtension.dll.config", "extension config added");

					timestamp = DateTime.Now;
					System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older. HFS+ has a 1s timestamp resolution :(

					// Add a config file to the container. This file should be added to the app, and the AOT-compiler re-executed for the root assembly.
					File.WriteAllText (mtouch.RootAssembly + ".config", "<configuration></configuration>");
					mtouch.AssertExecute (MTouchAction.BuildDev, "add config to container exe");
					Console.WriteLine ($"{DateTime.Now} **** ADD CONFIG TO CONTAINER DONE ****");
					mtouch.AssertNoneModified (timestamp, name, "testApp.exe.config", "testApp", "testApp.aotdata.armv7", "testApp.aotdata.arm64");
					extension.AssertNoneModified (timestamp, name);
					CollectionAssert.Contains (Directory.EnumerateFiles (mtouch.AppPath, "*", SearchOption.AllDirectories).Select ((v) => Path.GetFileName (v)), "testApp.exe.config", "container config added");

					timestamp = DateTime.Now;
					System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older. HFS+ has a 1s timestamp resolution :(

					{
						// Add a satellite to the extension.
						var satellite = extension.CreateTemporarySatelliteAssembly ();
						mtouch.AssertExecute (MTouchAction.BuildDev, "add satellite to extension");
						Console.WriteLine ($"{DateTime.Now} **** ADD SATELLITE TO EXTENSION DONE ****");
						mtouch.AssertNoneModified (timestamp, name, Path.GetFileName (satellite));
						extension.AssertNoneModified (timestamp, name, Path.GetFileName (satellite));
						extension.AssertModified (timestamp, name, Path.GetFileName (satellite));
						CollectionAssert.Contains (Directory.EnumerateFiles (extension.AppPath, "*", SearchOption.AllDirectories).Select ((v) => Path.GetFileName (v)), Path.GetFileName (satellite), "extension satellite added");
					}

					timestamp = DateTime.Now;
					System.Threading.Thread.Sleep (1000); // make sure all new timestamps are at least a second older. HFS+ has a 1s timestamp resolution :(

					{
						// Add a satellite to the container.
						var satellite = mtouch.CreateTemporarySatelliteAssembly ();
						mtouch.AssertExecute (MTouchAction.BuildDev, "add satellite to container");
						Console.WriteLine ($"{DateTime.Now} **** ADD SATELLITE TO CONTAINER DONE ****");
						mtouch.AssertNoneModified (timestamp, name, Path.GetFileName (satellite));
						extension.AssertNoneModified (timestamp, name, Path.GetFileName (satellite));
						mtouch.AssertModified (timestamp, name, Path.GetFileName (satellite));
						CollectionAssert.Contains (Directory.EnumerateFiles (mtouch.AppPath, "*", SearchOption.AllDirectories).Select ((v) => Path.GetFileName (v)), Path.GetFileName (satellite), "container satellite added");
					}
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
					Assert.AreEqual (has_mdb, File.Exists (Path.Combine (appDir, "mscorlib.pdb")), "#pdb");
				} else {
					Assert.AreEqual (has_mdb, File.Exists (Path.Combine (appDir, ".monotouch-32", "mscorlib.pdb")), "#pdb");
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
		public void MT0010 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.CustomArguments = new string [] { "--optimize:?" };
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (10, "Could not parse the command line argument '--optimize=?'");
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

				mtouch.CreateTemporaryApp (code: "public class C { static void Main () { System.Console.WriteLine (typeof (X)); System.Console.WriteLine (typeof (UIKit.UIWindow)); } }", extraArg: "-r:" + StringUtils.Quote (dllA));
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
				extension.CreateTemporaryServiceExtension (extraArg: $"-r:{StringUtils.Quote (dll)}", extraCode: "class Z { static void Y () { System.Console.WriteLine (typeof (X)); } }", appName: "testApp");
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
				mtouch.TargetVer = "10.3";
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
			// Any old Xcode will do.
			var old_xcode = Configuration.GetOldXcodeRoot ();
			if (!Directory.Exists (old_xcode))
				Assert.Ignore ($"This test needs an Xcode older than {Configuration.XcodeVersion}");

			// Get the SDK version for this Xcode version
			string sdk_platform;
			switch (profile) {
			case Profile.iOS:
				sdk_platform = "iPhoneSimulator";
				break;
			case Profile.tvOS:
				sdk_platform = "AppleTVSimulator";
				break;
			case Profile.watchOS:
				sdk_platform = "WatchSimulator";
				break;
			default:
				throw new NotImplementedException ();
			}
			var sdk_settings = Path.Combine (old_xcode, "Platforms", $"{sdk_platform}.platform", "Developer", "SDKs", $"{sdk_platform}.sdk", "SDKSettings.plist");
			var sdk_version = Configuration.GetPListStringValue (sdk_settings, "DefaultDeploymentTarget");

			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = profile;
				mtouch.CreateTemporaryApp ();
				mtouch.SdkRoot = old_xcode;
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Sdk = sdk_version;
				Assert.AreEqual (1, mtouch.Execute (MTouchAction.BuildSim));
				var xcodeVersionString = Configuration.XcodeVersion;
				if (xcodeVersionString.EndsWith (".0", StringComparison.Ordinal))
					xcodeVersionString = xcodeVersionString.Substring (0, xcodeVersionString.Length - 2);
				mtouch.AssertError (91, String.Format ("This version of Xamarin.iOS requires the {0} {1} SDK (shipped with Xcode {2}). Either upgrade Xcode to get the required header files or set the managed linker behaviour to Link Framework SDKs Only (to try to avoid the new APIs).", name, GetSdkVersion (profile), xcodeVersionString));
			}
		}

		[Test]
		public void MT0095_SharedCode ()
		{
			using (var exttool = new MTouchTool ()) {
				exttool.Profile = Profile.iOS;
				exttool.CreateTemporaryCacheDirectory ();
				exttool.Linker = MTouchLinker.LinkAll;

				exttool.CreateTemporaryServiceExtension ();
				exttool.MSym = true;
				exttool.Debug = false;
				exttool.AssertExecute (MTouchAction.BuildDev, "build extension");

				using (var apptool = new MTouchTool ()) {
					apptool.Profile = Profile.iOS;
					apptool.MSym = true;
					apptool.Debug = false;
					apptool.CreateTemporaryCacheDirectory ();
					apptool.CreateTemporaryApp ();
					apptool.AppExtensions.Add (exttool);
					apptool.Linker = MTouchLinker.LinkAll;
					apptool.AssertExecute (MTouchAction.BuildDev, "build app");
					
					Assert.IsTrue(Directory.Exists(Path.Combine(apptool.Cache, "Build", "Msym")), "App Msym dir");
					Assert.IsFalse(Directory.Exists(Path.Combine(exttool.Cache, "Build", "Msym")), "Extenson Msym dir");
					exttool.AssertNoWarnings();
					apptool.AssertNoWarnings();
				}
			}
		}
		
		[Test]
		public void MT0095_NotSharedCode ()
		{
			using (var exttool = new MTouchTool ()) {
				exttool.Profile = Profile.iOS;
				exttool.CreateTemporaryCacheDirectory ();
				exttool.Linker = MTouchLinker.LinkAll;
				exttool.CustomArguments = new string [] { "--nodevcodeshare" };
				exttool.CreateTemporaryServiceExtension ();
				exttool.MSym = true;
				exttool.Debug = false;
				exttool.AssertExecute (MTouchAction.BuildDev, "build extension");

				using (var apptool = new MTouchTool ()) {
					apptool.Profile = Profile.iOS;
					apptool.MSym = true;
					apptool.Debug = false;
					apptool.CreateTemporaryCacheDirectory ();
					apptool.CreateTemporaryApp ();
					apptool.AppExtensions.Add (exttool);
					apptool.Linker = MTouchLinker.LinkAll;
					apptool.CustomArguments = new string [] { "--nodevcodeshare" };
					apptool.AssertExecute (MTouchAction.BuildDev, "build app");
					
					Assert.IsTrue(Directory.Exists(Path.Combine(apptool.Cache, "Build", "Msym")), "App Msym dir");
					Assert.IsTrue(Directory.Exists(Path.Combine(exttool.Cache, "Build", "Msym")), "Extenson Msym dir");
					exttool.AssertNoWarnings();
					apptool.AssertNoWarnings();
				}
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
		public void MT0112_deploymenttarget ()
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.TargetVer = "6.0";
					app.WarnAsError = new int [] { 112 };
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (112, "Native code sharing has been disabled because the container app's deployment target is earlier than iOS 8.0 (it's 6.0).");
				}
			}
		}

		[Test]
		public void MT0112_i18n ()
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.I18N = I18N.CJK;
					app.WarnAsError = new int [] { 112 };
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (112, "Native code sharing has been disabled because the container app includes I18N assemblies (CJK).");
				}
			}
		}

		[Test]
		public void MT0113_bitcode ()
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.Abi = "arm64+llvm";
					app.Bitcode = MTouchBitcode.Full;
					app.WarnAsError = new int [] { 113 };
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (113, "Native code sharing has been disabled for the extension 'testServiceExtension' because the bitcode options differ between the container app (None) and the extension (LLVMOnly).");
				}
			}
		}

		[Test]
		[TestCase ("framework app", new string [] { "@sdk=framework=Xamarin.Sdk" }, null)]
		[TestCase ("framework ext", null, new string [] { "@sdk=framework=Xamarin.Sdk" })]
		[TestCase ("fastdev app", new string [] { "@all=dynamiclibrary" }, null)]
		[TestCase ("fastdev ext", null, new string [] { "@all=dynamiclibrary" })]
		public void MT0113_assemblybuildtarget (string name, string[] extension_abt, string[] app_abt)
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				if (extension_abt != null)
					extension.AssemblyBuildTargets.AddRange (extension_abt);
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.WarnAsError = new int [] { 113 };
					if (app_abt != null)
						app.AssemblyBuildTargets.AddRange (app_abt);
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (113, $"Native code sharing has been disabled for the extension 'testServiceExtension' because the --assembly-build-target options are different between the container app ({(app_abt == null ? string.Empty : string.Join (", ", app_abt.Select ((v) => "--assembly-build-target:" + v)))}) and the extension ({(extension_abt == null ? string.Empty : string.Join (", ", extension_abt?.Select ((v) => "--assembly-build-target:" + v)))}).");
				}
			}
		}

		[Test]
		public void MT0113_i18n ()
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.I18N = I18N.CJK;
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.WarnAsError = new int [] { 113 };
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (113, "Native code sharing has been disabled for the extension 'testServiceExtension' because the I18N assemblies are different between the container app (None) and the extension (CJK).");
				}
			}
		}

		[Test]
		public void MT0113_aot ()
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.WarnAsError = new int [] { 113 };
					app.AotArguments = "dwarfdebug"; // doesn't matter exactly what, just that it's different from the extension.
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (113, "Native code sharing has been disabled for the extension 'testServiceExtension' because the arguments to the AOT compiler are different between the container app (dwarfdebug,static,asmonly,direct-icalls,) and the extension (static,asmonly,direct-icalls,).");
				}
			}
		}

		[Test]
		public void MT0113_aotother ()
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.WarnAsError = new int [] { 113 };
					app.AotOtherArguments = "--aot-options=-O=float32"; // doesn't matter exactly what, just that it's different from the extension.
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (113, "Native code sharing has been disabled for the extension 'testServiceExtension' because the other arguments to the AOT compiler are different between the container app (--aot-options=-O=float32 ) and the extension ().");
				}
			}
		}

		[Test]
		public void MT0113_llvm ()
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.Abi = "arm64";
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.WarnAsError = new int [] { 113 };
					app.Abi = "arm64+llvm";
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (113, "Native code sharing has been disabled for the extension 'testServiceExtension' because LLVM is not enabled or disabled in both the container app (True) and the extension (False).");
				}
			}
		}

		[Test]
		public void MT0113_linker ()
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.Abi = "arm64";
				extension.Linker = MTouchLinker.LinkAll;
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.Linker = MTouchLinker.DontLink;
					app.WarnAsError = new int [] { 113 };
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (113, "Native code sharing has been disabled for the extension 'testServiceExtension' because the managed linker settings are different between the container app (None) and the extension (All).");
				}
			}
		}

		[Test]
		public void MT0113_skipped ()
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.WarnAsError = new int [] { 113 };
					app.LinkSkip = new string [] { "mscorlib.dll" };
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (113, "Native code sharing has been disabled for the extension 'testServiceExtension' because the skipped assemblies for the managed linker are different between the container app (mscorlib.dll) and the extension ().");
				}
			}
		}

		[Test]
		public void MT0112_xml ()
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.WarnAsError = new int [] { 112 };
					app.XmlDefinitions = new string [] { "foo.xml" };
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (112, "Native code sharing has been disabled because the container app has custom xml definitions for the managed linker (foo.xml).");
				}
			}
		}

		[Test]
		public void MT0113_xml ()
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.XmlDefinitions = new string [] { "foo.xml" };
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.WarnAsError = new int [] { 113 };
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (113, "Native code sharing has been disabled for the extension 'testServiceExtension' because the extension has custom xml definitions for the managed linker (foo.xml).");
				}
			}
		}

		[Test]
		[TestCase ("arm64", "armv7", "ARMv7")]
		[TestCase ("armv7", "armv7,arm64", "ARM64")]
		public void MT0113_abi (string app_abi, string extension_abi, string error_abi)
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.Abi = extension_abi;
				extension.TargetVer = "10.3"; // otherwise 32-bit builds aren't possible
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.WarnAsError = new int [] { 113 };
					app.Abi = app_abi;
					app.TargetVer = "10.3"; // otherwise 32-bit builds aren't possible
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (113, $"Native code sharing has been disabled for the extension 'testServiceExtension' because the container app does not build for the ABI {error_abi} (while the extension is building for this ABI).");
				}
			}
		}

		[Test]
		[TestCase ("armv7+llvm+thumb2", "armv7+llvm", "ARMv7, Thumb, LLVM", "ARMv7, LLVM")]
		public void MT0113_incompatible_abi (string app_abi, string extension_abi, string container_error_abi, string extension_error_abi)
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.Abi = extension_abi;
				extension.TargetVer = "10.3"; // otherwise 32-bit builds aren't possible
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.WarnAsError = new int [] { 113 };
					app.Abi = app_abi;
					app.TargetVer = "10.3"; // otherwise 32-bit builds aren't possible
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (113, $"Native code sharing has been disabled for the extension 'testServiceExtension' because the container app is building for the ABI {container_error_abi}, which is not compatible with the extension's ABI ({extension_error_abi}).");
				}
			}
		}

		[Test]
		public void MT0113_refmismatch ()
		{
			using (var extension = new MTouchTool ()) {
				var ext_tmpdir = extension.CreateTemporaryDirectory ();
				var ext_dll = CompileTestAppLibrary (ext_tmpdir, @"public class X { }", appName: "testLibrary");
				extension.CreateTemporaryServiceExtension (extraCode: "class Y : X {}", extraArg: $"-r:{StringUtils.Quote (ext_dll)}");
				extension.CreateTemporaryCacheDirectory ();
				extension.References = new string [] { ext_dll };
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");

				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);

					var app_tmpdir = app.CreateTemporaryDirectory ();
					var app_dll = CompileTestAppLibrary (app_tmpdir, @"public abstract class X { }", appName: "testLibrary");
					app.CreateTemporaryApp (extraCode: "class Y : X {}", extraArg: $"-r:{StringUtils.Quote (app_dll)}");
					app.CreateTemporaryCacheDirectory ();
					app.References = new string [] { app_dll };
					app.WarnAsError = new int [] { 113 };
					app.AssertExecuteFailure (MTouchAction.BuildDev, "build app");
					app.AssertError (113, $"Native code sharing has been disabled for the extension 'testServiceExtension' because the container app is referencing the assembly 'testLibrary' from '{app_dll}', while the extension references a different version from '{ext_dll}'.");
				}
			}
		}

		[Test]
		public void CodeSharingExactContentsDifferentPaths ()
		{
			// Test that we allow code sharing when the exact same assembly (based on file content)
			// is referenced from different paths between extension and container project.
			using (var extension = new MTouchTool ()) {
				var ext_tmpdir = extension.CreateTemporaryDirectory ();
				var ext_dll = CompileTestAppLibrary (ext_tmpdir, @"public class X { }", appName: "testLibrary");
				extension.CreateTemporaryServiceExtension (extraCode: "class Y : X {}", extraArg: $"-r:{StringUtils.Quote (ext_dll)}");
				extension.CreateTemporaryCacheDirectory ();
				extension.References = new string [] { ext_dll };
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");

				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);

					var app_tmpdir = app.CreateTemporaryDirectory ();
					var app_dll = Path.Combine (app_tmpdir, Path.GetFileName (ext_dll));
					File.Copy (ext_dll, app_dll);
					app.CreateTemporaryApp (extraCode: "class Y : X {}", extraArg: $"-r:{StringUtils.Quote (app_dll)}");
					app.CreateTemporaryCacheDirectory ();
					app.References = new string [] { app_dll };
					app.WarnAsError = new int [] { 113 };
					app.AssertExecute (MTouchAction.BuildDev, "build app");
					// bug #56754 prevents this from working // app.AssertNoWarnings ();
				}
			}
		}

		[Test]
		[TestCase ("armv7", "ARMv7")]
		[TestCase ("armv7s", "ARMv7s")]
		[TestCase ("armv7,armv7s", "ARMv7")]
		[TestCase ("i386", "i386")]
		public void MT0116 (string abi, string messageAbi)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.TargetVer = "11.0";
				mtouch.Abi = abi;
				mtouch.AssertExecuteFailure (abi == "i386" ? MTouchAction.BuildSim : MTouchAction.BuildDev, "build");
				mtouch.AssertError (116, $"Invalid architecture: {messageAbi}. 32-bit architectures are not supported when deployment target is 11 or later.");
			}
		}

		[Test]
		public void MT0125 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.AssemblyBuildTargets.Add ("@all=framework");
				mtouch.Linker = MTouchLinker.DontLink; // faster test.
				mtouch.Debug = true; // faster test, because it enables fastsim
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
				mtouch.AssertWarning (125, "The --assembly-build-target command-line argument is ignored in the simulator.");
			}
		}

		[Test]
		public void MT0126 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.FastDev = true;
				mtouch.Linker = MTouchLinker.DontLink; // faster test.
				mtouch.Debug = true; // faster test, because it enables fastsim
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
				mtouch.AssertWarning (126, "Incremental builds have been disabled because incremental builds are not supported in the simulator.");
			}
		}

		[Test]
		public void MT0127 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryAppDirectory ();

				var tmpdir = mtouch.CreateTemporaryDirectory ();
				var nativeCodeA = @"
int getNumber () { return 123; }
";
				var nativeCodeB = @"
int getNumber ();
int getSameNumber () { return getNumber (); }
";

				var extraCodeA = @"
public class BindingAppA {
	[System.Runtime.InteropServices.DllImport (""__Internal"")]
	public static extern int getNumber ();
}
";
				var extraCodeB = @"
public class BindingAppB {
	[System.Runtime.InteropServices.DllImport (""__Internal"")]
	public static extern int getSameNumber ();
	public static int getNumber () { return BindingAppA.getNumber (); }
}
";

				var bindingLibA = CreateBindingLibrary (tmpdir, nativeCodeA, null, null, extraCodeA, name: "bindingA", arch: "arm64");
				var bindingLibB = CreateBindingLibrary (tmpdir, nativeCodeB, null, null, extraCodeB, name: "bindingB", references: new string [] { bindingLibA }, arch: "arm64");
				var exe = CompileTestAppExecutable (tmpdir, @"
public class TestApp { 
	static void Main () {
		System.Console.WriteLine (typeof (UIKit.UIWindow).ToString ());
		System.Console.WriteLine (BindingAppB.getSameNumber ());
		System.Console.WriteLine (BindingAppB.getNumber ());
	}
}
", $"-r:{StringUtils.Quote (bindingLibA)} -r:{StringUtils.Quote (bindingLibB)}");

				mtouch.RootAssembly = exe;
				mtouch.References = new [] { bindingLibA, bindingLibB };

				mtouch.FastDev = true;
				mtouch.AssertExecute (MTouchAction.BuildDev, "first build");
				mtouch.AssertWarning (127, "Incremental builds have been disabled because this version of Xamarin.iOS does not support incremental builds in projects that include more than one third-party binding libraries.");
			}
		}

		[Test]
		public void MT0132 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.Linker = MTouchLinker.LinkSdk;
				mtouch.Optimize = new string [] { "foo" };
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
				mtouch.AssertWarning (132, "Unknown optimization: 'foo'. Valid optimizations are: remove-uithread-checks, dead-code-elimination, inline-isdirectbinding, inline-intptr-size, inline-runtime-arch, blockliteral-setupblock, register-protocols, inline-dynamic-registration-supported, remove-dynamic-registrar.");
			}
		}

		[Test]
		[TestCase ("all")]
		[TestCase ("-all")]
		[TestCase ("remove-uithread-checks,dead-code-elimination,inline-isdirectbinding,inline-intptr-size,inline-runtime-arch,register-protocols")]
		public void Optimizations (string opt)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.Linker = MTouchLinker.LinkSdk;
				mtouch.Registrar = MTouchRegistrar.Static;
				mtouch.Optimize = new string [] { opt };
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
				mtouch.AssertNoWarnings ();
			}
		}

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

		[Test]
		[TestCase (Profile.tvOS, MTouchBitcode.Marker)]
		[TestCase (Profile.watchOS, MTouchBitcode.Marker)]
		public void StripBitcodeFromFrameworks (Profile profile, MTouchBitcode bitcode)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = profile;
				if (profile == Profile.watchOS) {
					mtouch.CreateTemporaryWatchKitExtension ();
				} else {
					mtouch.CreateTemporaryApp ();
				}
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.AssemblyBuildTargets.Add ("@all=framework=MyApp");
				mtouch.NoStrip = true; // faster test
				mtouch.DSym = false; // faster test
				mtouch.Bitcode = bitcode;
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				var frameworks = new string [] { "Mono", "Xamarin" };
				foreach (var framework in frameworks) {
					var relative_path = $"Frameworks/{framework}.framework/{framework}";
					var src = Path.Combine (GetXamarinSdkDirectory (profile, true), relative_path);
					var dst = Path.Combine (mtouch.AppPath, relative_path);
					var srcLength = new FileInfo (src).Length;
					var dstLength = new FileInfo (dst).Length;
					Assert.That (dstLength, Is.LessThan (srcLength), "Framework size");
				}
			}
		}

		static string GetXamarinSdkDirectory (Profile profile, bool device)
		{
			switch (profile) {
			case Profile.iOS:
				if (device) {
					return Path.Combine (Configuration.SdkRootXI, "SDKs", "MonoTouch.iphoneos.sdk");
				} else {
					return Path.Combine (Configuration.SdkRootXI, "SDKs", "MonoTouch.iphonesimulator.sdk");
				}
			case Profile.tvOS:
				if (device) {
					return Path.Combine (Configuration.SdkRootXI, "SDKs", "Xamarin.AppleTVOS.sdk");
				} else {
					return Path.Combine (Configuration.SdkRootXI, "SDKs", "Xamarin.AppleTVSimulator.sdk");
				}
			case Profile.watchOS:
				if (device) {
					return Path.Combine (Configuration.SdkRootXI, "SDKs", "Xamarin.WatchOS.sdk");
				} else {
					return Path.Combine (Configuration.SdkRootXI, "SDKs", "Xamarin.WatchSimulator.sdk");
				}
			default:
				throw new NotImplementedException ();
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
			return Configuration.GetBaseLibrary (profile);
		}

		public static string GetCompiler (Profile profile, StringBuilder args, bool use_csc = false)
		{
			args.Append (" -lib:").Append (Path.GetDirectoryName (GetBaseLibrary (profile))).Append (' ');
			if (use_csc) {
				return "/Library/Frameworks/Mono.framework/Commands/csc";
			} else {
				return "/Library/Frameworks/Mono.framework/Commands/mcs";
			}
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
			return Configuration.GetTargetFramework (profile);
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
				return "x86_64";
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
			return Configuration.GetSdkVersion (profile);
		}

		[Test]
		public void LinkAll_Frameworks ()
		{
			// Make sure that mtouch does not link with unused frameworks.

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = MTouchLinker.LinkAll;
				mtouch.AssertExecute (MTouchAction.BuildSim);

				var load_commands = ExecutionHelper.Execute ("otool", $"-l {StringUtils.Quote (mtouch.NativeExecutablePath)}", hide_output: true);
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

				var symbols = GetNativeSymbols (mtouch.NativeExecutablePath);
				Assert.That (symbols, Has.None.EqualTo ("_theUltimateAnswer"), "Binding symbol not in executable");

				symbols = GetNativeSymbols (Path.Combine (mtouch.AppPath, "libbindings-test.dll.dylib"));
				Assert.That (symbols, Has.Some.EqualTo ("_theUltimateAnswer"), "Binding symbol in binding library");
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
				TargetVer = "10.3", // otherwise 32-bit build isn't possible
				Abi = "armv7,arm64",
			}) {
				mtouch.CreateTemporaryApp ();

				mtouch.AssertExecute (MTouchAction.BuildDev);
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
				tool.Verbosity = 1; // This is required to get the debug output we're testing for.
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
		[TestCase (Target.Dev, "armv7", "10.3")]
		[TestCase (Target.Dev, "armv7s", "10.3")]
		[TestCase (Target.Dev, "armv7,armv7s", "10.3")]
		[TestCase (Target.Dev, "arm64", null)]
		[TestCase (Target.Dev, "arm64+llvm", null)]
		[TestCase (Target.Dev, "armv7,arm64", "10.3")]
		[TestCase (Target.Dev, "armv7s,arm64", "10.3")]
		[TestCase (Target.Dev, "armv7,armv7s,arm64", "10.3")]
		[TestCase (Target.Sim, "i386", "10.3")]
		[TestCase (Target.Sim, "x86_64", null)]
		public void Architectures_Unified (Target target, string abi, string deployment_target)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.iOS;
				mtouch.CreateTemporaryApp ();

				mtouch.Abi = abi;
				mtouch.TargetVer = deployment_target;

				var bin = Path.Combine (mtouch.AppPath, Path.GetFileNameWithoutExtension (mtouch.RootAssembly));

				mtouch.AssertExecute (target == Target.Dev ? MTouchAction.BuildDev : MTouchAction.BuildSim);

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
				mtouch.TargetVer = "10.3";

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

				mtouch.TargetVer = "10.3";
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
		public void MonoFrameworkArchitectures ()
		{
			using (var extension = new MTouchTool ()) {
				extension.CreateTemporaryServiceExtension ();
				extension.CreateTemporaryCacheDirectory ();
				extension.Abi = "armv7,arm64";
				extension.TargetVer = "10.3";
				extension.Linker = MTouchLinker.LinkAll; // faster test
				extension.NoStrip = true; // faster test
				extension.AssertExecute (MTouchAction.BuildDev, "build extension");
				using (var app = new MTouchTool ()) {
					app.AppExtensions.Add (extension);
					app.CreateTemporaryApp ();
					app.CreateTemporaryCacheDirectory ();
					app.Abi = "arm64";
					app.Linker = MTouchLinker.LinkAll; // faster test
					app.NoStrip = true; // faster test
					app.AssertExecute (MTouchAction.BuildDev, "build app");

					var mono_framework = Path.Combine (app.AppPath, "Frameworks", "Mono.framework", "Mono");
					Assert.That (mono_framework, Does.Exist, "mono framework existence");
					// Verify that mtouch removed armv7s from the framework.
					Assert.That (GetArchitectures (mono_framework), Is.EquivalentTo (new [] { "armv7", "arm64" }), "mono framework architectures");
				}
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
				subdir = "/linker/ios";
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
				mtouch.TargetVer = "10.3"; // otherwise 32-bit builds aren't possible
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
				var bindingLib = CreateBindingLibrary (tmpdir, nativeCode, null, null, extraCode, arch: "arm64");
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
				mtouch.Timeout = TimeSpan.FromMinutes (5);

				// each variation is tested twice so that we don't break when everything is found in the cache the second time around.

				mtouch.AssertExecute (MTouchAction.BuildDev, "first build");
				var symbols = GetNativeSymbols (mtouch.NativeExecutablePath);
				Assert.That (symbols, Has.Some.EqualTo ("_dummy_field"), "Field not found in initial build");
				Assert.That (symbols, Has.Some.EqualTo ("_DummyMethod"), "P/invoke not found in initial build");

				ExecutionHelper.Execute ("touch", bindingLib); // This will make it so that the second identical variation won't skip the final link step.
				mtouch.AssertExecute (MTouchAction.BuildDev, "second build");
				symbols = GetNativeSymbols (mtouch.NativeExecutablePath);
				Assert.That (symbols, Has.Some.EqualTo ("_dummy_field"), "Field not found in second build");
				Assert.That (symbols, Has.Some.EqualTo ("_DummyMethod"), "P/invoke not found in second build");
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

					var lines = GetNativeSymbols (mtouch.NativeExecutablePath);
					var found_field = lines.Contains ("_dummy_field");
					var found_pinvoke = lines.Contains ("_DummyMethod");

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
				mtouch.GccFlags = StringUtils.Quote (lib);
				mtouch.TargetVer = "10.3"; // otherwise 32-bit build isn't possible
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
				mtouch.Timeout = TimeSpan.FromMinutes (5);

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
				mtouch.TargetVer = "10.3"; // otherwise 32-bit builds aren't possible
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
				mtouch.Timeout = TimeSpan.FromMinutes (5);
				mtouch.TargetVer = "10.3"; // otherwise 32-bit builds aren't possible

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
				mtouch.TargetVer = "10.3"; // otherwise 32-bit builds aren't possible
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
				mtouch.TargetVer = "10.3"; // otherwise 32-bit builds aren't possible
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
				tool.CreateTemporaryWatchKitExtension ();

				tool.FastDev = true;
				Assert.AreEqual (0, tool.Execute (MTouchAction.BuildDev), "build");

				Assert.IsTrue (File.Exists (Path.Combine (tool.AppPath, "libpinvokes.dylib")), "libpinvokes.dylib existence");

				var otool_output = ExecutionHelper.Execute ("otool", $"-l {StringUtils.Quote (Path.Combine (tool.AppPath, "libpinvokes.dylib"))}", hide_output: true);
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
				exttool.CreateTemporaryWatchKitExtension ();
				exttool.Frameworks.Add (Path.Combine (Configuration.SourceRoot, "tests/test-libraries/.libs/watchos/XTest.framework"));
				exttool.AssertExecute (MTouchAction.BuildSim, "build extension");

				using (var apptool = new MTouchTool ()) {
					apptool.Profile = Profile.iOS;
					apptool.CreateTemporaryCacheDirectory ();
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

				exttool.CreateTemporaryServiceExtension ();
				exttool.Frameworks.Add (Path.Combine (Configuration.SourceRoot, "tests/test-libraries/.libs/ios/XTest.framework"));
				exttool.AssertExecute (MTouchAction.BuildSim, "build extension");

				using (var apptool = new MTouchTool ()) {
					apptool.Profile = Profile.iOS;
					apptool.CreateTemporaryCacheDirectory ();
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

				exttool.References = new string []
				{
					GetFrameworksBindingLibrary (exttool.Profile),
				};
				exttool.CreateTemporaryServiceExtension (code: @"using UserNotifications;
[Foundation.Register (""NotificationService"")]
public partial class NotificationService : UNNotificationServiceExtension
{
	protected NotificationService (System.IntPtr handle) : base (handle)
	{
		System.Console.WriteLine (Bindings.Test.CFunctions.theUltimateAnswer ());
	}
}", extraArg: StringUtils.Quote ("-r:" + exttool.References [0]));
				exttool.AssertExecute (MTouchAction.BuildSim, "build extension");

				using (var apptool = new MTouchTool ()) {
					apptool.Profile = Profile.iOS;
					apptool.CreateTemporaryCacheDirectory ();
					apptool.CreateTemporaryApp ();
					apptool.AppExtensions.Add (exttool);
					apptool.AssertExecute (MTouchAction.BuildSim, "build app");

					Assert.IsTrue (Directory.Exists (Path.Combine (apptool.AppPath, "Frameworks", "XTest.framework")), "framework exists");
					Assert.IsFalse (Directory.Exists (Path.Combine (exttool.AppPath, "Frameworks")), "extension framework inexistence");
				}
			}
		}

		[Test]
		public void AppAndExtensionWithBindingFramework ()
		{
			// There should be no problem to reference a binding library with a framework from both a container app and an extension.
			using (var exttool = new MTouchTool ()) {
				exttool.Profile = Profile.iOS;
				exttool.Linker = MTouchLinker.DontLink; // faster
				exttool.References = new string [] { GetFrameworksBindingLibrary (exttool.Profile) };
				exttool.CreateTemporaryCacheDirectory ();
				exttool.CreateTemporaryServiceExtension (extraCode: "\n\n[Foundation.Preserve] class X { public X () { System.Console.WriteLine (Bindings.Test.CFunctions.theUltimateAnswer ()); } }", extraArg: $"-r:{StringUtils.Quote (exttool.References [0])}");
				exttool.AssertExecute (MTouchAction.BuildSim, "build extension");

				using (var apptool = new MTouchTool ()) {
					apptool.Profile = Profile.iOS;
					apptool.CreateTemporaryCacheDirectory ();
					apptool.References = exttool.References;
					apptool.CreateTemporaryApp (extraCode: @"[Foundation.Preserve] class X { public X () { System.Console.WriteLine (Bindings.Test.CFunctions.theUltimateAnswer ()); } };", extraArg: $"-r:{StringUtils.Quote (apptool.References [0])}");
					apptool.AppExtensions.Add (exttool);
					apptool.AssertExecute (MTouchAction.BuildSim, "build app");

					Assert.IsTrue (Directory.Exists (Path.Combine (apptool.AppPath, "Frameworks", "XTest.framework")), "framework exists");
					Assert.IsFalse (Directory.Exists (Path.Combine (exttool.AppPath, "Frameworks")), "extension framework inexistence");
				}
			}
		}

		[Test]
		public void MT1035 ()
		{
			// Verify that an error is shown if two different frameworks with the same name are included.

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var framework_binding_library = GetFrameworksBindingLibrary (Profile.iOS);
			using (var exttool = new MTouchTool ()) {
				exttool.Profile = Profile.iOS;
				exttool.Linker = MTouchLinker.DontLink; // faster
				exttool.References = new string [] { framework_binding_library };
				exttool.CreateTemporaryCacheDirectory ();
				exttool.CreateTemporaryServiceExtension (extraCode: "\n\n[Foundation.Preserve] class X { public X () { System.Console.WriteLine (Bindings.Test.CFunctions.theUltimateAnswer ()); } }", extraArg: $"-r:{StringUtils.Quote (exttool.References [0])}");
				exttool.AssertExecute (MTouchAction.BuildSim, "build extension");

				using (var apptool = new MTouchTool ()) {
					// Here we do a little bit of surgery on the binding assembly to change the embedded framework (we just add a file into the zip).
					var modified_framework_binding_library = Path.Combine (tmpdir, Path.GetFileName (framework_binding_library));
					var framework_zip = Path.Combine (tmpdir, "XTest.framework");
					var extra_content = Path.Combine (tmpdir, "extra-content");
					Mono.Cecil.AssemblyDefinition ad = Mono.Cecil.AssemblyDefinition.ReadAssembly (framework_binding_library);
					var res = (Mono.Cecil.EmbeddedResource) ad.MainModule.Resources.Where ((v) => v.Name == "XTest.framework").First ();
					File.WriteAllBytes (framework_zip, res.GetResourceData ());
					File.WriteAllText (extra_content, "Hello world");
					ExecutionHelper.Execute ("zip", $"{StringUtils.Quote (framework_zip)} {StringUtils.Quote (extra_content)}");
					ad.MainModule.Resources.Remove (res);
					ad.MainModule.Resources.Add (new Mono.Cecil.EmbeddedResource (res.Name, res.Attributes, File.ReadAllBytes (framework_zip)));
					ad.Write (modified_framework_binding_library);

					apptool.Profile = Profile.iOS;
					apptool.Linker = MTouchLinker.DontLink; // faster
					apptool.References = new string [] { modified_framework_binding_library };
					apptool.CreateTemporaryCacheDirectory ();
					apptool.CreateTemporaryApp (extraCode: "\n\n[Foundation.Preserve] class X { public X () { System.Console.WriteLine (Bindings.Test.CFunctions.theUltimateAnswer ()); } }", extraArg: $"-r:{StringUtils.Quote (apptool.References [0])}");
					apptool.AppExtensions.Add (exttool);
					apptool.AssertExecuteFailure (MTouchAction.BuildSim, "build app");
					apptool.AssertError (1035, "Cannot include different versions of the framework 'XTest.framework'");
					apptool.AssertError (1036, $"Framework 'XTest.framework' included from: {exttool.Cache}/XTest.framework (Related to previous error)");
					apptool.AssertError (1036, $"Framework 'XTest.framework' included from: {apptool.Cache}/XTest.framework (Related to previous error)");
				}
			}
		}

		[Test]
		public void MultipleExtensionsWithBindingFramework ()
		{
			// if multiple extensions references a framework (but not the container app)
			// the framework should still be copied successfully to the main app's Framework directory.
			using (var service_ext = new MTouchTool ()) {
				service_ext.Profile = Profile.iOS;
				service_ext.Linker = MTouchLinker.DontLink; // faster
				service_ext.References = new string [] { GetFrameworksBindingLibrary (service_ext.Profile) };
				service_ext.CreateTemporaryCacheDirectory ();
				service_ext.CreateTemporaryServiceExtension (extraCode: "\n\n[Foundation.Preserve] class X { public X () { System.Console.WriteLine (Bindings.Test.CFunctions.theUltimateAnswer ()); } }", extraArg: $"-r:{StringUtils.Quote (service_ext.References [0])}");
				service_ext.AssertExecute (MTouchAction.BuildSim, "build service extension");

				using (var today_ext = new MTouchTool ()) {
					today_ext.Profile = Profile.iOS;
					today_ext.Linker = MTouchLinker.DontLink; // faster
					today_ext.References = service_ext.References;
					today_ext.CreateTemporaryCacheDirectory ();
					today_ext.CreateTemporaryTodayExtension (extraCode: "\n\n[Foundation.Preserve] class X { public X () { System.Console.WriteLine (Bindings.Test.CFunctions.theUltimateAnswer ()); } }", extraArg: $"-r:{StringUtils.Quote (today_ext.References [0])}");
					today_ext.AssertExecute (MTouchAction.BuildSim, "build today extension");

					using (var apptool = new MTouchTool ()) {
						apptool.Profile = Profile.iOS;
						apptool.Linker = MTouchLinker.DontLink; // faster
						apptool.CreateTemporaryCacheDirectory ();
						apptool.CreateTemporaryApp ();
						apptool.AppExtensions.Add (service_ext);
						apptool.AppExtensions.Add (today_ext);
						apptool.AssertExecute (MTouchAction.BuildSim, "build app");

						Assert.IsTrue (Directory.Exists (Path.Combine (apptool.AppPath, "Frameworks", "XTest.framework")), "framework exists");
						Assert.IsFalse (Directory.Exists (Path.Combine (service_ext.AppPath, "Frameworks")), "service extension framework inexistence");
						Assert.IsFalse (Directory.Exists (Path.Combine (today_ext.AppPath, "Frameworks")), "today framework inexistence");
					}
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

		[Test]
		public void MT2003 ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Debug = true; // makes simlauncher possible, which speeds up the build
				mtouch.Optimize = new string [] { "all"};
				mtouch.AssertExecute (MTouchAction.BuildSim);
				mtouch.AssertWarning (2003, "Option '--optimize=remove-uithread-checks' will be ignored since linking is disabled");
				mtouch.AssertWarning (2003, "Option '--optimize=dead-code-elimination' will be ignored since linking is disabled");
				mtouch.AssertWarning (2003, "Option '--optimize=inline-isdirectbinding' will be ignored since linking is disabled");
				mtouch.AssertWarning (2003, "Option '--optimize=inline-intptr-size' will be ignored since linking is disabled");
				mtouch.AssertWarning (2003, "Option '--optimize=inline-runtime-arch' will be ignored since linking is disabled");
				mtouch.AssertWarning (2003, "Option '--optimize=blockliteral-setupblock' will be ignored since linking is disabled");
				mtouch.AssertWarning (2003, "Option '--optimize=inline-dynamic-registration-supported' will be ignored since linking is disabled");
				mtouch.AssertWarning (2003, "Option '--optimize=register-protocols' will be ignored since the static registrar is not enabled");
				mtouch.AssertWarning (2003, "Option '--optimize=remove-dynamic-registrar' will be ignored since the static registrar is not enabled");
				mtouch.AssertWarningCount (9);
			}

			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.Debug = true; // makes simlauncher possible, which speeds up the build
				mtouch.Optimize = new string [] { "-inline-intptr-size" };
				mtouch.AssertExecute (MTouchAction.BuildSim);
				mtouch.AssertWarning (2003, "Option '--optimize=-inline-intptr-size' will be ignored since linking is disabled");
				mtouch.AssertWarningCount (1);
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

				mtouch.CreateTemporaryApp (code: "public class C { static void Main () { System.Console.WriteLine (typeof (X)); System.Console.WriteLine (typeof (UIKit.UIWindow)); } }", extraArg: "-r:" + StringUtils.Quote (dllA));
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

				mtouch.CreateTemporaryApp (code: "public class C { static void Main () { System.Console.WriteLine (typeof (X)); System.Console.WriteLine (typeof (UIKit.UIWindow)); } }", extraArg: "-r:" + StringUtils.Quote (dllA));
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

				mtouch.CreateTemporaryApp (code: "public class C { static void Main () { System.Console.WriteLine (typeof (X)); System.Console.WriteLine (typeof (UIKit.UIWindow)); } }", extraArg: "-r:" + StringUtils.Quote (dllA));
				mtouch.References = new string [] { dllB, dllA };

				// Without the linker we'll just copy the references, and not actually run into problems if we copy one that doesn't work
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");
				mtouch.AssertWarningPattern (109, "The assembly 'System.Net.Http.dll' was loaded from a different path than the provided path .provided path: .*CreateTemporaryDirectory.*/System.Net.Http.dll, actual path: .*/Library/Frameworks/Xamarin.iOS.framework/Versions/.*/lib/mono/Xamarin.iOS/System.Net.Http.dll..");

				// With the linker, we'll find out that the loaded reference doesn't work.
				mtouch.Linker = MTouchLinker.LinkSdk;
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (2101, "Can't resolve the reference 'X', referenced from the method 'System.Void C::Main()' in 'System.Net.Http, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'.");
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

		[Test]
		public void ManyBigPInvokes ()
		{
			var tmpdir = Cache.CreateTemporaryDirectory ();
			var m = Path.Combine (tmpdir, "file.m");
			var cs = Path.Combine (tmpdir, "file.cs");
			var functions = 2500;
			var m_writer = new StringBuilder ();
			var cs_writer = new StringBuilder ("\n");
			cs_writer.AppendLine ("namespace Tester {");
			cs_writer.AppendLine ("\tusing System.Runtime.InteropServices;");
			cs_writer.AppendLine ("\tclass PInvokes {");
			for (int i = 0; i < functions; i++) {
				var fname = $"this_is_a_big_function_with_very_very_very_very_very_very_very_very_very_very_very_very_very_very_very_long_name_number_{i}";
				m_writer.AppendLine ($"void {fname} () {{}}");
				cs_writer.AppendLine ($"\t\t[DllImport (\"__Internal\")]");
				cs_writer.AppendLine ($"\t\tstatic extern void {fname} ();");
			}
			cs_writer.AppendLine ("\t}");
			cs_writer.AppendLine ("}");
			var o = CompileNativeLibrary (Profile.iOS, tmpdir, m_writer.ToString (), device: false);
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.CreateTemporaryApp (extraCode: cs_writer.ToString ());
				mtouch.GccFlags = o;
				mtouch.Abi = "x86_64";
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "first build");
				mtouch.AssertWarningPattern (5217, "Native linking possibly failed because the linker command line was too long .[0-9]* characters..");

				mtouch.CustomArguments = new string [] { "--dynamic-symbol-mode=code" };
				mtouch.AssertExecute (MTouchAction.BuildSim, "second build");
			}
		}

		[Test]
		[TestCase ("sl_SI")] // Slovenian. Has a strange minus sign.
		[TestCase ("ur_IN")] // Urdu (India). Right-to-left.
		public void BuildWithCulture (string culture)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.Debug = false; // disables the simlauncher, and makes us produce a main.m
				mtouch.Verbosity = -200;
				mtouch.Linker = MTouchLinker.DontLink; // faster
				mtouch.EnvironmentVariables = new Dictionary<string, string> ();
				mtouch.EnvironmentVariables ["LANG"] = culture;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build sim");
				mtouch.AssertNoWarnings ();

				mtouch.Debug = true; // faster
				mtouch.Linker = MTouchLinker.LinkAll; // faster
				mtouch.AssertExecute (MTouchAction.BuildDev, "build dev");
				mtouch.AssertNoWarnings ();
			}
		}

		[Test]
		public void ResponseFile ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.ResponseFile = Path.Combine (mtouch.CreateTemporaryDirectory (), "rspfile");
				File.WriteAllLines (mtouch.ResponseFile, new string [] { "/version" });
				mtouch.AssertExecute (MTouchAction.None);
				mtouch.AssertNoWarnings ();
			}
		}

		[Test]
		[TestCase ("CFNetworkHandler", "CFNetworkHandler")]
		[TestCase ("NSUrlSessionHandler", "NSUrlSessionHandler")]
		[TestCase ("HttpClientHandler", "HttpClientHandler")]
		[TestCase (null, "HttpClientHandler")]
		[TestCase ("", "HttpClientHandler")]
		public void HttpClientHandler (string mtouchHandler, string expectedHandler)
		{
			var testCode = $@"
[TestFixture]
public class HandlerTest
{{
	[Test]
	public void Test ()
	{{
		var client = new System.Net.Http.HttpClient ();
		var field = client.GetType ().BaseType.GetField (""handler"", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		if (field == null)
			throw new System.Exception (""Could not find the field 'handler' in HttpClient's base type (which should be 'HttpMessageInvoker')."");
		var fieldValue = field.GetValue (client);
		if (fieldValue == null)
			throw new System.Exception (""Unexpected null value found in 'HttpMessageInvoker.handler' field."");
		Assert.AreEqual (""{expectedHandler}"", fieldValue.GetType ().Name, ""default http client handler"");
	}}
}}
";
			var csproj_configuration = mtouchHandler == null ? string.Empty : ("<MtouchHttpClientHandler>" + mtouchHandler + "</MtouchHttpClientHandler>");
			RunUnitTest (Profile.iOS, testCode, csproj_configuration, csproj_references: new string [] { "System.Net.Http" }, clean_simulator: false);
		}

		[Test]
		[TestCase (true)]
		[TestCase (false)]
		public void NoProductAssemblyReference (bool nofastsim)
		{
			using (var mtouch = new MTouchTool ()) {
				// The .exe contains no reference to Xamarin.iOS.dll, because no API is used from Xamarin.iOS.dll
				mtouch.CreateTemporaryApp (code: "public class TestApp { static void Main () { System.Console.WriteLine (\"Hello world\"); } }");
				mtouch.CreateTemporaryCacheDirectory ();
				if (nofastsim)
					mtouch.NoFastSim = nofastsim;
				mtouch.Debug = true; // makes simlauncher possible (when nofastsim is false)
				mtouch.Linker = MTouchLinker.DontLink; // faster
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build sim");
				mtouch.AssertErrorPattern (123, "The executable assembly .*/testApp.exe does not reference Xamarin.iOS.dll.");
				mtouch.AssertErrorCount (1);
				mtouch.AssertNoWarnings ();
			}
		}

		[Test]
		[TestCase (true, MTouchLinker.DontLink, false)]
		[TestCase (true, MTouchLinker.LinkAll, true)]
		[TestCase (true, MTouchLinker.LinkSdk, true)]
		[TestCase (false, MTouchLinker.DontLink, false)]
		[TestCase (false, MTouchLinker.LinkAll, true)]
		[TestCase (false, MTouchLinker.LinkSdk, true)]
		public void MixedModeAssembliesCanNotBeLinked (bool nofastsim, MTouchLinker linker, bool builds_successfully)
		{
			using (var mtouch = new MTouchTool ()) {
				var tmp = mtouch.CreateTemporaryDirectory ();
				string libraryPath = Path.Combine (Configuration.SourceRoot, "tests", "common", "MixedClassLibrary.dll");

				mtouch.CreateTemporaryApp (code: "public class TestApp { static void Main () { System.Console.WriteLine (typeof (MixedClassLibrary.Class1)); System.Console.WriteLine (typeof (ObjCRuntime.Runtime)); } }",
										   extraArg: $"-r:Xamarin.iOS.dll -r:{StringUtils.Quote (libraryPath)}");
				mtouch.CreateTemporaryCacheDirectory ();
				mtouch.References = new string [] { libraryPath };
				if (nofastsim)
					mtouch.NoFastSim = nofastsim;
				mtouch.Debug = true; // makes simlauncher possible (when nofastsim is false)

				mtouch.Linker = linker;

				if (builds_successfully) {
					mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build sim");
					mtouch.AssertErrorPattern (2014, "Unable to link assembly .* as it is mixed-mode.");
					mtouch.AssertErrorCount (1);
				}
				else {
					mtouch.AssertExecute (MTouchAction.BuildSim, "build sim");
					mtouch.AssertErrorCount (0);
				}

				mtouch.AssertNoWarnings ();
			}
		}

		[Test]
		[TestCase (true)]
		[TestCase (false)]
		public void SymbolsOutOfDate1 (bool use_csc)
		{
			using (var mtouch = new MTouchTool ()) {
				// Compile the managed executable twice, the second time without debugging symbols, which will cause the debugging symbols to become stale
				mtouch.CreateTemporaryApp (extraArg: "/debug:full", use_csc: use_csc);
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = MTouchLinker.DontLink; // makes the test faster
				mtouch.Debug = true; // makes the test faster because it makes simlauncher possible
				mtouch.AssertExecute (MTouchAction.BuildSim);
			}
		}

		[Test]
		[TestCase (true)]
		[TestCase (false)]
		public void SymbolsOutOfDate2 (bool use_csc)
		{
			using (var mtouch = new MTouchTool ()) {
				// Compile the managed executable twice, both times with debugging symbols, but restore the debugging symbols from the first build so that they're stale
				mtouch.CreateTemporaryApp (extraArg: "/debug:full", use_csc: use_csc);
				var symbol_file = use_csc ? Path.ChangeExtension (mtouch.RootAssembly, "pdb") : mtouch.RootAssembly + ".mdb";
				var symbols = File.ReadAllBytes (symbol_file);
				mtouch.CreateTemporaryApp (extraArg: "/debug:full", use_csc: use_csc);
				File.WriteAllBytes (symbol_file, symbols);
				mtouch.Linker = MTouchLinker.DontLink; // makes the test faster
				mtouch.Debug = true; // makes the test faster because it makes simlauncher possible
				mtouch.AssertExecute (MTouchAction.BuildSim);
			}
		}

		[Test]
		[TestCase (true, true)]
		[TestCase (false, true)]
		[TestCase (true, false)]
		[TestCase (false, false)]
		public void SymbolsBroken1 (bool use_csc, bool compile_with_debug_symbols)
		{
			using (var mtouch = new MTouchTool ()) {
				// (Over)write invalid data in the debug symbol file
				mtouch.CreateTemporaryApp (use_csc, extraArg: compile_with_debug_symbols ? "/debug:full" : string.Empty);
				var symbol_file = use_csc ? Path.ChangeExtension (mtouch.RootAssembly, "pdb") : mtouch.RootAssembly + ".mdb";
				File.WriteAllText (symbol_file, "invalid stuff");
				mtouch.Linker = MTouchLinker.DontLink; // makes the test faster
				mtouch.Debug = true; // makes the test faster because it makes simlauncher possible
				mtouch.AssertExecute (MTouchAction.BuildSim);
			}
		}

		[Test]
		[TestCase ("i386", "32-sgen")]
		[TestCase ("x86_64", "64-sgen")]
		public void SimlauncherSymbols (string arch, string simlauncher_suffix)
		{
			var libxamarin_path = Path.Combine (Configuration.SdkRootXI, "SDKs", "MonoTouch.iphonesimulator.sdk", "usr", "lib", "libxamarin.a");
			var simlauncher_path = Path.Combine (Configuration.BinDirXI, "simlauncher" + simlauncher_suffix);

			var libxamarin_symbols = new HashSet<string> (GetNativeSymbols (libxamarin_path, arch));
			var simlauncher_symbols = new HashSet<string> (GetNativeSymbols (simlauncher_path, arch));
			var only_libxamarin = libxamarin_symbols.Except (simlauncher_symbols);

			var missingSimlauncherSymbols = new List<string> ();
			foreach (var symbol in only_libxamarin) {
				switch (symbol) {
				case "_fix_ranlib_warning_about_no_symbols": // Dummy symbol to fix linker warning
				case "_fix_ranlib_warning_about_no_symbols_v2": // Dummy symbol to fix linker warning
				case "_monotouch_IntPtr_objc_msgSendSuper_IntPtr": // Classic only, this function can probably be removed when we switch to binary copy of a Classic version of libxamarin.a
				case "_monotouch_IntPtr_objc_msgSend_IntPtr": // Classic only, this function can probably be removed when we switch to binary copy of a Classic version of libxamarin.a
				case "_xamarin_float_objc_msgSend": // Classic only, this function can probably be removed when we switch to binary copy of a Classic version of libxamarin.a
				case "_xamarin_float_objc_msgSendSuper": // Classic only, this function can probably be removed when we switch to binary copy of a Classic version of libxamarin.a
				case "_xamarin_nfloat_objc_msgSend": // XM only
				case "_xamarin_nfloat_objc_msgSendSuper": // Xm only
					continue;
				default:
					missingSimlauncherSymbols.Add (symbol);
					break;
				}
			}
			Assert.That (missingSimlauncherSymbols, Is.Empty, "no missing simlauncher symbols");
		}

		public void XamarinSdkAdjustLibs ()
		{
			using (var exttool = new MTouchTool ()) {
				exttool.Profile = Profile.iOS;
				exttool.Abi = "arm64";
				exttool.CreateTemporaryCacheDirectory ();
				exttool.Debug = false;
				exttool.MSym = false;
				exttool.Linker = MTouchLinker.DontLink;
				exttool.TargetVer = "8.0";

				exttool.CreateTemporaryServiceExtension ();
				exttool.AssertExecute (MTouchAction.BuildDev, "build extension");

				using (var apptool = new MTouchTool ()) {
					apptool.Profile = exttool.Profile;
					apptool.Abi = exttool.Abi;
					apptool.Debug = exttool.Debug;
					apptool.MSym = exttool.MSym;
					apptool.TargetVer = exttool.TargetVer;
					apptool.CreateTemporaryCacheDirectory ();
					apptool.CreateTemporaryApp ();

					apptool.AppExtensions.Add (exttool);
					apptool.Linker = MTouchLinker.DontLink;
					apptool.AssertExecute (MTouchAction.BuildDev, "build app");

					var sdk = StringUtils.Quote (Path.Combine (apptool.Cache, "arm64", "Xamarin.Sdk"));
					var shared_libraries = ExecutionHelper.Execute ("otool", $"-L {sdk}", hide_output: true);
					Asserts.DoesNotContain ("Private", shared_libraries, "Private");

					exttool.AssertNoWarnings();
					apptool.AssertNoWarnings();
				}
			}
		}

#region Helper functions
		static void RunUnitTest (Profile profile, string code, string csproj_configuration = "", string [] csproj_references = null, string configuration = "Debug", string platform = "iPhoneSimulator", bool clean_simulator = true)
		{
			if (profile != Profile.iOS)
				throw new NotImplementedException ();
			var testfile = @"
using System;
using System.Collections.Generic;
using System.Reflection;
using Foundation;
using UIKit;
using MonoTouch.NUnit.UI;
using NUnit.Framework;
using NUnit.Framework.Internal;

[Register (""AppDelegate"")]
public partial class AppDelegate : UIApplicationDelegate {
	UIWindow window;
	TouchRunner runner;

	public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	{
		window = new UIWindow (UIScreen.MainScreen.Bounds);
		runner = new TouchRunner (window);
		runner.Add (Assembly.GetExecutingAssembly ());
		window.RootViewController = new UINavigationController (runner.GetViewController ());
		window.MakeKeyAndVisible ();

		return true;
	}

	static void Main (string[] args)
	{
		UIApplication.Main (args, null, typeof (AppDelegate));
	}
}

[TestFixture]
public class Dummy {
	[Test]
	public void DummyTest () {}
}
" + code;
			var csproj = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project DefaultTargets=""Build"" ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">iPhoneSimulator</Platform>
    <ProductVersion>8.0.30703</ProductVersion>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectGuid>{17EB364A-0D86-49AC-8B8C-C79C2C5AC9EF}</ProjectGuid>
    <ProjectTypeGuids>{FEACFBD2-3405-455C-9665-78FE426C6842};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
    <OutputType>Exe</OutputType>
    <RootNamespace>testapp</RootNamespace>
    <AssemblyName>testapp</AssemblyName>
    <TargetFrameworkIdentifier>Xamarin.iOS</TargetFrameworkIdentifier>
    <IntermediateOutputPath>obj\$(Platform)\$(Configuration)</IntermediateOutputPath>
    <OutputPath>bin\$(Platform)\$(Configuration)</OutputPath>
	" + csproj_configuration + @"
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|iPhoneSimulator' "">
    <DebugSymbols>True</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>False</Optimize>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>0</WarningLevel>
    <MtouchDebug>True</MtouchDebug>
    <MtouchExtraArgs>-v -v -v -v</MtouchExtraArgs>
    <AllowUnsafeBlocks>True</AllowUnsafeBlocks>
    <MtouchArch>x86_64</MtouchArch>
    <MtouchLink>None</MtouchLink>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|iPhone' "">
    <DebugSymbols>True</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>False</Optimize>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>0</WarningLevel>
    <MtouchDebug>True</MtouchDebug>
    <CodesignKey>iPhone Developer</CodesignKey>
    <MtouchExtraArgs>-v -v -v -v</MtouchExtraArgs>
    <MtouchArch>ARM64</MtouchArch>
    <MtouchLink>Full</MtouchLink>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""System"" />
    <Reference Include=""System.Xml"" />
    <Reference Include=""System.Core"" />
    <Reference Include=""Xamarin.iOS"" />
    <Reference Include=""MonoTouch.NUnitLite"" />
" + (csproj_references == null ? string.Empty : string.Join ("\n", csproj_references.Select ((v) => "    <Reference Include=\"" + v + "\" />\n"))) + @"
  </ItemGroup>
  <ItemGroup>
    <None Include=""Info.plist"">
      <LogicalName>Info.plist</LogicalName>
    </None>
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""testfile.cs"" />
  </ItemGroup>
  <Import Project=""$(MSBuildExtensionsPath)\Xamarin\iOS\Xamarin.iOS.CSharp.targets"" />
</Project>";

			var tmpdir = Cache.CreateTemporaryDirectory ();
			var csprojpath = Path.Combine (tmpdir, "testapp.csproj");
			var testfilepath = Path.Combine (tmpdir, "testfile.cs");
			var infoplistpath = Path.Combine (tmpdir, "Info.plist");
			File.WriteAllText (csprojpath, csproj);
			File.WriteAllText (testfilepath, testfile);
			File.WriteAllText (infoplistpath, MTouchTool.CreatePlist (profile, "testapp"));
			XBuild.Build (csprojpath, configuration, platform);
			var environment_variables = new Dictionary<string, string> ();
			if (!clean_simulator)
				environment_variables ["SKIP_SIMULATOR_SETUP"] = "1";
			ExecutionHelper.Execute ("mono", $"{StringUtils.Quote (Path.Combine (Configuration.RootPath, "tests", "xharness", "xharness.exe"))} --run {StringUtils.Quote (csprojpath)} --target ios-simulator-64 --sdkroot {Configuration.xcode_root} --logdirectory {StringUtils.Quote (Path.Combine (tmpdir, "log.txt"))} --configuration {configuration}", environmentVariables: environment_variables);
		}

		public static string CompileTestAppExecutable (string targetDirectory, string code = null, string extraArg = "", Profile profile = Profile.iOS, string appName = "testApp", string extraCode = null, string usings = null, bool use_csc = false)
		{
			return BundlerTool.CompileTestAppExecutable (targetDirectory, code, extraArg, profile, appName, extraCode, usings, use_csc);
		}

		public static string CompileTestAppLibrary (string targetDirectory, string code, string extraArg = null, Profile profile = Profile.iOS, string appName = "testApp")
		{
			return BundlerTool.CompileTestAppLibrary (targetDirectory, code, extraArg, profile, appName);
		}

		public static string CompileTestAppCode (string target, string targetDirectory, string code, string extraArg = "", Profile profile = Profile.iOS, string appName = "testApp", bool use_csc = false)
		{
			return BundlerTool.CompileTestAppCode (target, targetDirectory, code, extraArg, profile, appName, use_csc);
		}

		static string CreateBindingLibrary (string targetDirectory, string nativeCode, string bindingCode, string linkWith = null, string extraCode = "", string name = "binding", string[] references = null, string arch = "armv7")
		{
			var o = CompileNativeLibrary (targetDirectory, nativeCode, name: name, arch: arch);
			var cs = Path.Combine (targetDirectory, $"{name}Code.cs");
			var dll = Path.Combine (targetDirectory, $"{name}Library.dll");

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

			var x = Path.Combine (targetDirectory, $"extra{name}Code.cs");
			File.WriteAllText (x, extraCode);

			ExecutionHelper.Execute (Configuration.BtouchPath,
			                         string.Format ("{0} --out:{1} --link-with={2},{3} -x:{4} {5}", cs, dll, o, Path.GetFileName (o), x, references == null ? string.Empty : string.Join (", ", references.Select ((v) => "-r:" + v))));

			return dll;
		}

		static string CompileNativeLibrary (string targetDirectory, string code, string name = "testCode", string arch = "armv7", bool device = true)
		{
			return CompileNativeLibrary (Profile.iOS, targetDirectory, code, name, arch, device);
		}

		static string CompileNativeLibrary (Profile profile, string targetDirectory, string code, string name = "testCode", string arch = null, bool device = true)
		{
			var m = Path.Combine (targetDirectory, $"{name}.m");
			var o = Path.ChangeExtension (m, ".o");
			File.WriteAllText (m, code);

			string output;
			string fileName = Path.Combine (Configuration.xcode_root, "Toolchains/XcodeDefault.xctoolchain/usr/bin/clang");
			string min_os_version;
			string sdk;

			switch (profile) {
			case Profile.iOS:
				min_os_version = device ? "iphoneos-version-min=6.0" : "iphonesimulator-version-min=6.0";
				sdk = device ? "iPhoneOS" : "iPhoneSimulator";
				if (arch == null)
					arch = device ? "armv7" : "x86_64";
				break;
			default:
				throw new NotImplementedException ();
			}

			string args = $"-gdwarf-2 -arch {arch} -std=c99 -isysroot {Configuration.xcode_root}/Platforms/{sdk}.platform/Developer/SDKs/{sdk}{Configuration.sdk_version}.sdk -m{min_os_version} -c -DDEBUG  -o {o} -x objective-c {m}";

			Console.WriteLine ("{0} {1}", fileName, args);
			if (ExecutionHelper.Execute (fileName, args, out output) != 0) {
				Console.WriteLine (output);
				throw new Exception (output);
			}

			return o;
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
				args.Append (" -r:").Append (StringUtils.Quote (GetBaseLibrary (profile)));
				args.Append (" -out:").Append (StringUtils.Quote (outputPath));
				args.Append (" ").Append (StringUtils.Quote (tmpFile));
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
			var symbols = GetNativeSymbols (file);
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

		public static void AssertDeviceAvailable ()
		{
			if (!Configuration.include_device)
				Assert.Ignore ("This build does not include device support.");
		}

		public static IEnumerable<string> GetNativeSymbols (string file, string arch = null)
		{
			var arguments = $"-gUjA {StringUtils.Quote (file)}";
			if (!string.IsNullOrEmpty (arch))
				arguments += " -arch " + arch;
			var symbols = ExecutionHelper.Execute ("nm", arguments, hide_output: true).Split ('\n');
			return symbols.Select ((v) => {
				var idx = v.LastIndexOf (": ", StringComparison.Ordinal);
				if (idx <= 0)
					return v;
				return v.Substring (idx + 2);
			});
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
