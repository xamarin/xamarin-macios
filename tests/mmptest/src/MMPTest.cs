using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public partial class MMPTests 
	{
		void RunMMPTest (Action <string> test)
		{
			string tmpDir = Path.Combine (Path.GetTempPath (), "mmp-test-dir");
			try {
				Directory.CreateDirectory (tmpDir);
				test (tmpDir);
			}
			finally {
				Directory.Delete (tmpDir, true);
			}
		}

		[Test]
		public void CollisionsBetweenLibraryNameAndEXE_ShouldFailBuild ()
		{
			RunMMPTest (tmpDir => {
				Action <string, string, bool> testCore = (projectName, assemblyName, XM45) => {
					// Build a library with the conflicting name
					TI.UnifiedTestConfig libConfig = new TI.UnifiedTestConfig (tmpDir) { XM45 = XM45, ProjectName = projectName, AssemblyName = assemblyName };
					string csprojTarget = TI.GenerateUnifiedLibraryProject (libConfig);
					TI.BuildProject (csprojTarget, isUnified : true);

					// Build an exe using that library, it should fail the build
					string referenceCode = string.Format (@"<Reference Include=""TestApp""><HintPath>{0}</HintPath></Reference>", Path.Combine (tmpDir, "bin/Debug", assemblyName + ".dll"));

					TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { XM45 = XM45, References = referenceCode };
					TI.TestUnifiedExecutable (test, shouldFail : true);
				};
				// These library assembly names conflict with the exe name
				testCore ("UnifiedLibrary", "UnifiedExample", false);
				testCore ("XM45Library", "XM45Example", true);
			});
		}

		[Test]
		public void CollisionBetweenEXEAndSDKAssembly_ShouldFailBuild ()
		{
			RunMMPTest (tmpDir => {
				Action <string, bool> testCore = (assemblyName, XM45) => {
					// Build a library with the conflicting name
					TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { XM45 = XM45, AssemblyName = assemblyName };
					TI.TestUnifiedExecutable (test, shouldFail : true);
				};
				// These library assembly names conflict with the exe name
				testCore ("Xamarin.Mac", false);
				testCore ("Xamarin.Mac", true);

				testCore ("System.Xml", false);
				testCore ("System.Xml", true);
			});
		}

		[Test]
		public void Unified_Static_RegistrarTest ()
		{
			if (!PlatformHelpers.CheckSystemVersion (10, 11))
				return;

			RunMMPTest (tmpDir => {
				// First in 64-bit
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = "<MonoBundlingExtraArgs>--registrar=static</MonoBundlingExtraArgs><XamMacArch>x86_64</XamMacArch>" };
				// Mobile
				TI.TestUnifiedExecutable (test);
				// XM45
				test.XM45 = true;
				TI.TestUnifiedExecutable (test);

				// Now 32-bit
				test.CSProjConfig = "<MonoBundlingExtraArgs>--registrar=static</MonoBundlingExtraArgs><XamMacArch>i386</XamMacArch>";
				// Mobile
				TI.TestUnifiedExecutable (test);
				// XM45
				test.XM45 = true;
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void XM_45_NotAddingIncorrectDependencies_LicenseTest ()
		{
			RunMMPTest (tmpDir => {
				TI.TestUnifiedExecutable (new TI.UnifiedTestConfig (tmpDir) { XM45 = true });

				// XM 4.5 projects were accidently pulling in every assembly in the 4.5 folder. Assert that isn't happening again.
				string monoBundlePath = Path.Combine (tmpDir, "bin/Debug/XM45Example.app/Contents/MonoBundle/");
				Assert.IsFalse (Directory.GetFiles (monoBundlePath).Any (x => x.Contains ("FSharp.Core.dll")), "F# was pulled in?");
			});
		}

		[Test]
		public void Unified_SmokeTest ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir);
				// Mobile
				TI.TestUnifiedExecutable (test);
				// XM45
				test.XM45 = true;
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void FSharp_SmokeTest ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { FSharp = true };
				// Mobile
				TI.TestUnifiedExecutable (test);
				// XM45
				test.XM45 = true;
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void Mobile_SmokeTest_LinkSDK ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = "<LinkMode>SdkOnly</LinkMode>" };
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void Mobile_SmokeTest_LinkAll ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = "<LinkMode>Full</LinkMode>" };
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void Mobile_NewRefCount_Warns ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = "<MonoBundlingExtraArgs>--new-refcount=false</MonoBundlingExtraArgs>" };
				string buildOutput = TI.TestUnifiedExecutable (test).BuildOutput;
				Assert.IsTrue (buildOutput.Contains ("Disabling the new refcount logic is deprecated"), "Mobile_NewRefCount_Warns did not warn as expected:\n\n", buildOutput);
			});
		}

		[Test]
		public void Mobile_Verbose_Test ()
		{
			RunMMPTest (tmpDir => {
				// Just make sure we accept the option and it doesn't crash
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = "<MonoBundlingExtraArgs>-v -v</MonoBundlingExtraArgs>" };
				TI.TestUnifiedExecutable (test);
			});
		}

		// The check for name collisions between the root executable and other assemblies was mistakenly comparing against the entire path, not just the assembly name.
		// This meant that an assembly located in a folder/subfolder with a name matching the root executable name would incorrectly report as an error.
		[Test]
		public void FilePathCollisionShouldNotFailBuild ()
		{
			RunMMPTest (tmpDir => {
				var libraryDirectory = tmpDir + "/UnifiedExample/";
				var libraryName = "UnifiedLibrary";
				Directory.CreateDirectory (libraryDirectory);

				TI.UnifiedTestConfig libConfig = new TI.UnifiedTestConfig (libraryDirectory) { ProjectName = libraryName };
				string csprojTarget = TI.GenerateUnifiedLibraryProject (libConfig);

				TI.BuildProject (csprojTarget, isUnified : true);

				string referenceCode = string.Format (@"<Reference Include=""UnifiedLibrary""><HintPath>{0}</HintPath></Reference>", Path.Combine (libraryDirectory, "bin/Debug/", $"{libraryName}.dll"));

				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { References = referenceCode };
				TI.TestUnifiedExecutable  (test);
			});
		}

		[Test]
		public void Unified_HelloWorld_ShouldHaveNoWarnings ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir);
				// Mobile
				string buildResults = TI.TestUnifiedExecutable (test).BuildOutput;
				Assert.IsTrue (!buildResults.Contains ("warning"), "Unified_HelloWorld_ShouldHaveNoWarnings - Mobile had warning: \n" + buildResults);

				// XM45
				test.XM45 = true;
				buildResults = TI.TestUnifiedExecutable (test).BuildOutput;
				Assert.IsTrue (!buildResults.Contains ("warning"), "Unified_HelloWorld_ShouldHaveNoWarnings - XM45 had warning: \n" + buildResults);
			});
		}

		[Test]
		public void Unified_HelloWorld_ShouldHaveNoRegistrarWarnings ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir);
				// Mobile
				string output = TI.TestUnifiedExecutable (test).RunOutput;
				Assert.IsTrue (!output.Contains ("Could not register the assembly"), "Unified_HelloWorld_ShouldHaveNoRegistrarWarnings - Mobile had registrar issues: \n" + output);

				// XM45
				test.XM45 = true;
				output = TI.TestUnifiedExecutable (test).RunOutput;
				Assert.IsTrue (!output.Contains ("Could not register the assembly"), "Unified_HelloWorld_ShouldHaveNoRegistrarWarnings - XM45 had registrar issues: \n" + output);
			});
		}

		[Test]
		public void SystemMono_SmokeTest ()
		{
			if (TI.FindMonoVersion () < new Version ("4.3"))
				return;

			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir);
				TI.TestSystemMonoExecutable (test);

				test.SystemMonoVersion = "4.5";
				TI.TestSystemMonoExecutable (test);

				test.SystemMonoVersion = "4.5.1";
				TI.TestSystemMonoExecutable (test);

				test.SystemMonoVersion = "4.6";
				TI.TestSystemMonoExecutable (test);

				test.SystemMonoVersion = "4.6.1";
				TI.TestSystemMonoExecutable (test);
			});
		}

		[Test]
		public void BuildUnifiedMobile_Program_WithNonASCIIName ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { AssemblyName = "piñata" };
				TI.TestUnifiedExecutable (test);

				test = new TI.UnifiedTestConfig (tmpDir) { AssemblyName = "你好世界" };
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void BuildUnifiedMobile_Program_WithSpace ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { AssemblyName = "Test With Space" };
				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void BuildUnified45_ShouldNotAllowReferenceToSystemDrawing ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { References = " <Reference Include=\"System.Drawing\" />", TestCode = "System.Drawing.RectangleF f = new System.Drawing.RectangleF ();", XM45 = true};
				TI.TestUnifiedExecutable (test, shouldFail : true);
			});
		}

		[Test]
		public void Dontlink_AllowsUnresolvableReferences ()
		{
			var sb = new StringBuilder ();
			RunMMPTest (tmpDir =>
			{
				// build b.dll
				sb.Clear ();
				sb.AppendFormat ("-target:library -out:{0}/b.dll {0}/b.cs", tmpDir);
				File.WriteAllText (Path.Combine (tmpDir, "b.cs"), "public class B { }");
				TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/mcs", sb, "b");

				// build a.dll
				sb.Clear ();
				sb.AppendFormat ("-target:library -out:{0}/a.dll {0}/a.cs -r:{0}/b.dll", tmpDir);
				File.WriteAllText (Path.Combine (tmpDir, "a.cs"), "public class A { public A () { System.Console.WriteLine (typeof (B)); }}");
				TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/mcs", sb, "a");

				File.Delete (Path.Combine (tmpDir, "b.dll"));

				// build project referencing a.dll
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir)
				{
					References = string.Format (" <Reference Include=\"a\" > <HintPath>{0}/a.dll</HintPath> </Reference> ", tmpDir),
					TestCode = "System.Console.WriteLine (typeof (A));",
				};
				TI.BuildUnifiedExecutable (test, shouldFail: false);
			});
		}

		[Test]
		public void Dontlink_Allow_ReadonlyAssembly ()
		{
			var sb = new StringBuilder ();
			RunMMPTest (tmpDir =>
			{
				// build b.dll
				sb.Clear ();
				string assemblyPath = string.Format ("{0}/b.dll", tmpDir);
				sb.AppendFormat ("-target:library -debug -out:{0} {1}/b.cs", assemblyPath, tmpDir);
				File.WriteAllText (Path.Combine (tmpDir, "b.cs"), "public class B { }");
				TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/mcs", sb, "b");

				File.SetAttributes (assemblyPath, FileAttributes.ReadOnly);
				File.SetAttributes (assemblyPath + ".mdb", FileAttributes.ReadOnly);

				// build project referencing a.dll
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir)
				{
					References = string.Format (" <Reference Include=\"b\" > <HintPath>{0}</HintPath> </Reference> ", assemblyPath),
					TestCode = "System.Console.WriteLine (typeof (B));",
				};
				TI.BuildUnifiedExecutable (test, shouldFail: false);

				test.CSProjConfig = "<LinkMode>SdkOnly</LinkMode>";
				TI.BuildUnifiedExecutable (test, shouldFail: false);
			});
		}

		[Test]
		public void UnsafeGACResolutionOptions_AllowsWindowsBaseResolution ()
		{
			RunMMPTest (tmpDir =>
			{
				UnsafeGACTestCore (tmpDir, true);
				UnsafeGACTestCore (tmpDir, false);
			});
		}

		static void UnsafeGACTestCore (string tmpDir, bool useFullProfile)
		{
			TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir)
			{
				XM45 = useFullProfile,
				TestCode = "System.Console.WriteLine (typeof (System.Windows.DependencyObject));",
				References = "<Reference Include=\"WindowsBase\" /><Reference Include=\"System.Xaml\" />"
			};

			TI.TestUnifiedExecutable (test, shouldFail: true);

			// Mobile will fail terribly due to mismatch BCL, no need to see if this works. Just testing that Mobile fails
			if (useFullProfile)
			{
				test.CSProjConfig = "<MonoBundlingExtraArgs>--allow-unsafe-gac-resolution</MonoBundlingExtraArgs>";
				TI.TestUnifiedExecutable (test, shouldFail: false);
			}
		}

		[Test]
		public void DefaultProject_ShouldPullInMonoPosix_AndNaitveLib ()
		{
			RunMMPTest (tmpDir =>
			{
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { XM45 = true };
				MonoPosixTestCore (tmpDir, test);
				test.XM45 = false;
				MonoPosixTestCore (tmpDir, test);
			});
		}

		static void MonoPosixTestCore (string tmpDir, TI.UnifiedTestConfig test)
		{
			TI.TestUnifiedExecutable (test, shouldFail: false);

			Assert.IsTrue (File.Exists (Path.Combine (tmpDir, "bin/Debug/XM45Example.app/Contents/MonoBundle/Mono.Posix.dll")));
			Assert.IsTrue (File.Exists (Path.Combine (tmpDir, "bin/Debug/XM45Example.app/Contents/MonoBundle/libMonoPosixHelper.dylib")));
		}
}
}
