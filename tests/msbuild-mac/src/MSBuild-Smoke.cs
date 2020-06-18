using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

using Xamarin;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public partial class MMPTests
	{
		void RunMSBuildTest (Action <string> test, string directory_name = null)
		{
			test (Cache.CreateTemporaryDirectory (directory_name ?? "msbuild-tests"));
		}

		[Test]
		public void BuildUnifiedMobile_Program_SmokeTest ()
		{
			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateEXEProject (new TI.UnifiedTestConfig (tmpDir) { ProjectName = "UnifiedExample.csproj" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildUnifiedXM45_Program_SmokeTest ()
		{
			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateEXEProject (new TI.UnifiedTestConfig (tmpDir) { ProjectName = "XM45Example.csproj" });
				TI.BuildProject (projectPath);
			});
		}

		void TestBCLCore (string tmpDir, string projectName)
		{
			File.Copy (Path.Combine (TI.AssemblyDirectory, TI.TestDirectory + "common/mac/System.Collections.Immutable.dll"), Path.Combine (tmpDir, "System.Collections.Immutable.dll"));
			string reference = "<Reference Include=\"System.Collections.Immutable\"><HintPath>System.Collections.Immutable.dll</HintPath></Reference>";
			string testCode = "var v = System.Collections.Immutable.ImmutableArray.CreateRange (new int [] { 42 });";
			string projectPath = TI.GenerateEXEProject (new TI.UnifiedTestConfig (tmpDir) { ProjectName = projectName, References = reference, TestCode = testCode });
			TI.BuildProject (projectPath);
		}

		[Test]
		public void BuildUnifiedMobile_Program_WithBCL ()
		{
			RunMSBuildTest (tmpDir => {
				TestBCLCore (tmpDir, "UnifiedExample.csproj");
			});
		}

		[Test]
		public void BuildUnifiedXM45_Program_WithBCL ()
		{
			RunMSBuildTest (tmpDir => {
				TestBCLCore (tmpDir, "XM45Example.csproj");
			});
		}

		[Test]
		public void BuildFSharpUnifiedMobile_Program_SmokeTest ()
		{
			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateEXEProject (new TI.UnifiedTestConfig (tmpDir) { FSharp = true, ProjectName = "FSharpUnifiedExample.fsproj" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildFSharpUnifiedXM45_Program_SmokeTest ()
		{
			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateEXEProject (new TI.UnifiedTestConfig (tmpDir) { FSharp = true, ProjectName = "FSharpXM45Example.fsproj" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildUnifiedMobile_Library_SmokeTest ()
		{
			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateUnifiedLibraryProject (new TI.UnifiedTestConfig (tmpDir) { ProjectName = "UnifiedLibrary" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildUnifiedXM45_Library_SmokeTest ()
		{
			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateUnifiedLibraryProject (new TI.UnifiedTestConfig (tmpDir) { ProjectName = "XM45Library" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildFSharpUnifiedMobile_Library_SmokeTest ()
		{
			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateUnifiedLibraryProject (new TI.UnifiedTestConfig (tmpDir) { FSharp = true, ProjectName = "FSharpUnifiedLibrary" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildFSharpUnifiedXM45_Library_SmokeTest ()
		{
			RunMSBuildTest (tmpDir => {
				string projectPath = TI.GenerateUnifiedLibraryProject (new TI.UnifiedTestConfig (tmpDir) { FSharp = true, ProjectName = "FSharpXM45Library" });
				TI.BuildProject (projectPath);
			});
		}

		[Test]
		public void BuildUnifiedProject_WithJustNativeRefNoLinkWith_Builds()
		{
			RunMSBuildTest (tmpDir => {
				string dylibPath = Path.Combine (tmpDir, "dll/");
				Directory.CreateDirectory (dylibPath);
				File.Copy (Path.Combine (TI.AssemblyDirectory, TI.TestDirectory + "mac-binding-project/bin/SimpleClassDylib.dylib"), Path.Combine (dylibPath, "SimpleClassDylib.dylib"));
				string itemGroup = "<ItemGroup><NativeReference Include=\".\\dll\\SimpleClassDylib.dylib\"> <IsCxx>False</IsCxx><Kind>Dynamic</Kind> </NativeReference> </ItemGroup>";
				string projectPath = TI.GenerateEXEProject (new TI.UnifiedTestConfig (tmpDir) { ProjectName = "UnifiedExample.csproj", ItemGroup = itemGroup });
				string buildResults = TI.BuildProject (projectPath);
				Assert.IsFalse (buildResults.Contains ("MM2006"), "BuildUnifiedProject_WittJustNativeRefNoLinkWith_Builds found 2006 warning: " + buildResults);
				Assert.IsTrue (File.Exists (Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MonoBundle/SimpleClassDylib.dylib")));

				StringBuilder output = new StringBuilder ();
				Xamarin.Bundler.Driver.RunCommand ("/usr/bin/otool", new [] { "-L", Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MacOS/UnifiedExample") }, output);
				Assert.IsTrue (output.ToString ().Contains ("SimpleClassDylib.dylib"));
			});
		}

		[Test]
		public void Build_BindingLibrary_SmokeTest ()
		{
			RunMSBuildTest (tmpDir => {
				foreach (string projectName in new []{"XM45Binding.csproj", "MobileBinding.csproj", "BindingProjectWithNoTag.csproj"}) {
					TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { ProjectName = projectName };
					string projectPath = TI.GenerateBindingLibraryProject (test);
					TI.BuildProject (projectPath);
				}
			});
		}

		[Test]
		public void BuildingSameBindingProject_TwoTimes_ShallNotInvokeMMPTwoTimes ()
		{
			const string nativeRefItemGroup = "<ItemGroup><NativeReference Include = \"\\usr\\lib\\libz.dylib\"><Kind>Dynamic</Kind><SmartLink>False</SmartLink></NativeReference></ItemGroup>";

			RunMSBuildTest (tmpDir =>
			{
				foreach (string project in new[] { "XM45Binding.csproj", "MobileBinding.csproj", "BindingProjectWithNoTag.csproj" })
				{
					var config = new TI.UnifiedTestConfig (tmpDir) { ProjectName = project, ItemGroup = nativeRefItemGroup };
					string projectPath = TI.GenerateBindingLibraryProject (config);
					string buildOutput = TI.BuildProject (projectPath);
					Assert.IsTrue (buildOutput.Contains (@"Building target ""CoreCompile"""));

					string secondBuildOutput = TI.BuildProject (projectPath);
					Assert.IsFalse (secondBuildOutput.Contains (@"Building target ""CoreCompile"""));
				}
			});
		}

		[Test]
		public void BuildingSameProject_TwoTimes_ShallNotInvokeMMPTwoTimes ()
		{
			RunMSBuildTest (tmpDir =>
			{
				foreach (var project in new string[] { "UnifiedExample.csproj", "XM45Example.csproj" })
				{
					var config = new TI.UnifiedTestConfig (tmpDir) { ProjectName = project };
					string projectPath = TI.GenerateEXEProject (config);
					string buildOutput = TI.BuildProject (projectPath);
					Assert.IsTrue (buildOutput.Contains (@"Building target ""_CompileToNative"""));

					string secondBuildOutput = TI.BuildProject (projectPath);
					Assert.IsFalse (secondBuildOutput.Contains (@"Building target ""_CompileToNative"""));
				}
			});
		}
	}
}
