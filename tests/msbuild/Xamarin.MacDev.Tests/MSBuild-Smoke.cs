using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

using Xamarin;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MMP.Tests {
	[TestFixture]
	public partial class MMPTests {
		void RunMSBuildTest (Action<string> test, string directory_name = null)
		{
			test (Cache.CreateTemporaryDirectory (directory_name ?? "msbuild-tests"));
		}

		[Test]
		public void BuildUnifiedMobile_Program_SmokeTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateEXEProject (new TI.UnifiedTestConfig (tmpDir) { ProjectName = "UnifiedExample.csproj" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildUnifiedXM45_Program_SmokeTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateEXEProject (new TI.UnifiedTestConfig (tmpDir) { ProjectName = "XM45Example.csproj" });
				TI.BuildProject (projectPath);
			});
		}

		void TestBCLCore (string tmpDir, string projectName)
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			var dll = Path.GetFullPath (Path.Combine (TI.TestDirectory, "common", "mac", "System.Collections.Immutable.dll"));
			string reference = $"<Reference Include=\"System.Collections.Immutable\"><HintPath>{dll}</HintPath></Reference>";
			string testCode = "var v = System.Collections.Immutable.ImmutableArray.CreateRange (new int [] { 42 });";
			string projectPath = TI.GenerateEXEProject (new TI.UnifiedTestConfig (tmpDir) { ProjectName = projectName, References = reference, TestCode = testCode });
			TI.BuildProject (projectPath);
		}

		[Test]
		public void BuildUnifiedMobile_Program_WithBCL ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				TestBCLCore (tmpDir, "UnifiedExample.csproj");
			});
		}

		[Test]
		public void BuildUnifiedXM45_Program_WithBCL ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				TestBCLCore (tmpDir, "XM45Example.csproj");
			});
		}

		[Test]
		public void BuildFSharpUnifiedMobile_Program_SmokeTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateEXEProject (new TI.UnifiedTestConfig (tmpDir) { FSharp = true, ProjectName = "FSharpUnifiedExample.fsproj" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildFSharpUnifiedXM45_Program_SmokeTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateEXEProject (new TI.UnifiedTestConfig (tmpDir) { FSharp = true, ProjectName = "FSharpXM45Example.fsproj" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildUnifiedMobile_Library_SmokeTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateUnifiedLibraryProject (new TI.UnifiedTestConfig (tmpDir) { ProjectName = "UnifiedLibrary" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildUnifiedXM45_Library_SmokeTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateUnifiedLibraryProject (new TI.UnifiedTestConfig (tmpDir) { ProjectName = "XM45Library" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildFSharpUnifiedMobile_Library_SmokeTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateUnifiedLibraryProject (new TI.UnifiedTestConfig (tmpDir) { FSharp = true, ProjectName = "FSharpUnifiedLibrary" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildFSharpUnifiedXM45_Library_SmokeTest ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateUnifiedLibraryProject (new TI.UnifiedTestConfig (tmpDir) { FSharp = true, ProjectName = "FSharpXM45Library" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildUnifiedProject_WithJustNativeRefNoLinkWith_Builds ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				var dylib = Path.GetFullPath (Path.Combine (TI.TestDirectory, "test-libraries", ".libs", "macos", "libtest.dylib"));
				string itemGroup = $"<ItemGroup><NativeReference Include=\"{dylib}\"> <IsCxx>False</IsCxx><Kind>Dynamic</Kind> </NativeReference> </ItemGroup>";
				string projectPath = TI.GenerateEXEProject (new TI.UnifiedTestConfig (tmpDir) { ProjectName = "UnifiedExample.csproj", ItemGroup = itemGroup });
				var testResults = TI.BuildProject (projectPath);
				testResults.Messages.AssertNoMessage (2006);
				Assert.That (Path.Combine (tmpDir, $"bin", "Debug", "UnifiedExample.app", "Contents", "MonoBundle", Path.GetFileName (dylib)), Does.Exist, "dylib in app");

				Assert.AreEqual (0, ExecutionHelper.Execute ("/usr/bin/otool", new [] { "-L", Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MacOS/UnifiedExample") }, out var output));
				Assert.IsTrue (output.ToString ().Contains (Path.GetFileName (dylib)));
			});
		}

		[Test]
		[TestCase ("XM45Binding.csproj")]
		[TestCase ("MobileBinding.csproj")]
		[TestCase ("BindingProjectWithNoTag.csproj")]
		public void Build_BindingLibrary_SmokeTest (string projectName)
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { ProjectName = projectName };
				string projectPath = TI.GenerateBindingLibraryProject (test);
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		[TestCase ("XM45Binding.csproj")]
		[TestCase ("MobileBinding.csproj")]
		[TestCase ("BindingProjectWithNoTag.csproj")]
		public void BuildingSameBindingProject_TwoTimes_ShallNotInvokeMMPTwoTimes (string project)
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var dylib = Path.GetFullPath (Path.Combine (TI.TestDirectory, "test-libraries", ".libs", "macos", "libtest.dylib"));
			string nativeRefItemGroup = $"<ItemGroup><NativeReference Include = \"{dylib}\"><Kind>Dynamic</Kind><SmartLink>False</SmartLink></NativeReference></ItemGroup>";

			RunMSBuildTest (tmpDir => {
				var config = new TI.UnifiedTestConfig (tmpDir) { ProjectName = project, ItemGroup = nativeRefItemGroup };
				string projectPath = TI.GenerateBindingLibraryProject (config);
				string buildOutput = TI.BuildProject (projectPath).BuildOutput;
				Assert.IsTrue (buildOutput.Contains (@"Building target ""CoreCompile"""));

				string secondBuildOutput = TI.BuildProject (projectPath).BuildOutput;
				Assert.IsFalse (secondBuildOutput.Contains (@"Building target ""CoreCompile"""));
			});
		}

		[Test]
		[TestCase ("UnifiedExample.csproj")]
		[TestCase ("XM45Example.csproj")]
		public void BuildingSameProject_TwoTimes_ShallNotInvokeMMPTwoTimes (string project)
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			RunMSBuildTest (tmpDir => {
				var config = new TI.UnifiedTestConfig (tmpDir) { ProjectName = project };
				string projectPath = TI.GenerateEXEProject (config);
				string buildOutput = TI.BuildProject (projectPath).BuildOutput;
				Assert.IsTrue (buildOutput.Contains (@"Building target ""_CompileToNative"""));

				string secondBuildOutput = TI.BuildProject (projectPath).BuildOutput;
				Assert.IsFalse (secondBuildOutput.Contains (@"Building target ""_CompileToNative"""));
			});
		}

		[Test]
		public void MyCocoaSceneKitApp ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var projectPath = Path.Combine (Configuration.TestProjectsDirectory, "MyCocoaSceneKitApp", "MyCocoaSceneKitApp.csproj");
			// Clone the project directory to a temporary directory
			var testDirectory = Configuration.CloneTestDirectory (Path.GetDirectoryName (projectPath));
			// Update the project path to the clone project path in the temporary directory
			projectPath = Path.Combine (testDirectory, Path.GetFileName (projectPath));
			// build the project
			TI.BuildProject (projectPath);
			// verify that the scene kit assets are present in the app
			var resourceDir = Path.Combine (testDirectory, "bin", "Debug", "MyCocoaSceneKitApp.app", "Contents", "Resources");
			Assert.That (Path.Combine (resourceDir, "art.scnassets", "scene.scn"), Does.Exist, "scene.scn");
			Assert.That (Path.Combine (resourceDir, "art.scnassets", "texture.png"), Does.Exist, "texture.png");
		}

		[Test]
		public void MyCocoaCoreMLApp ()
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.MacOSX);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			var projectPath = Path.Combine (Configuration.TestProjectsDirectory, "MyCocoaCoreMLApp", "MyCocoaCoreMLApp.csproj");
			// Clone the project directory to a temporary directory
			var testDirectory = Configuration.CloneTestDirectory (Path.GetDirectoryName (projectPath));
			// Update the project path to the clone project path in the temporary directory
			projectPath = Path.Combine (testDirectory, Path.GetFileName (projectPath));
			// build the project
			TI.BuildProject (projectPath);
			// verify that the scene kit assets are present in the app
			var resourceDir = Path.Combine (testDirectory, "bin", "Debug", "MyCocoaCoreMLApp.app", "Contents", "Resources");
			AssertCompiledModelExists (resourceDir, "SqueezeNet");
		}

		void AssertCompiledModelExists (string appBundlePath, string modelName)
		{
			var expected = new string [] { "coremldata.bin", "model.espresso.net", "model.espresso.shape", "model.espresso.weights", "model/coremldata.bin", "neural_network_optionals/coremldata.bin" };
			var mlmodelc = Path.Combine (appBundlePath, modelName + ".mlmodelc");

			Assert.IsTrue (Directory.Exists (mlmodelc), "mlmodelc existence");

			var files = new HashSet<string> (Directory.EnumerateFiles (mlmodelc, "*.*", SearchOption.AllDirectories));

			foreach (var name in expected)
				Assert.IsTrue (files.Contains (Path.Combine (mlmodelc, name)), "{0} not found", name);

			var expected_length = expected.Length;
			if (Configuration.XcodeVersion.Major >= 12) {
				Assert.IsTrue (files.Contains (Path.Combine (mlmodelc, "metadata.json")), " metadata.json not found");
				expected_length++;
				Assert.IsTrue (files.Contains (Path.Combine (mlmodelc, "analytics", "coremldata.bin")), "analytics/coremldata.bin not found");
				expected_length++;
			}
			Assert.AreEqual (expected_length, files.Count, "File count");
		}

	}
}
