using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Xamarin.Utils;

namespace Xamarin.MMP.Tests
{
	public class BindingProjectNoEmbeddingTests
	{
		static void Touch (string projectPath) => File.SetLastWriteTimeUtc (projectPath, DateTime.UtcNow);

		static void AssertNoResourceWithName (string tmpDir, string projectName, string resourceName)
		{
			string bindingName = BindingProjectTests.RemoveCSProj (projectName);
			string bindingLibraryPath = Path.Combine (tmpDir, $"bin/Debug/{bindingName}.dll");
			string resourceOutput = TI.RunAndAssert ("/Library/Frameworks/Mono.framework/Commands/monodis", "--presources " + bindingLibraryPath, "monodis");
			Assert.False (resourceOutput.Contains (resourceName), "Binding project output contained embedded library");
		}

		static void AssertFileInBundle (string tmpDir, BindingProjectType type, string path)
		{
			Assert.True (File.Exists (Path.Combine (tmpDir, $"bin/Debug/{(type == BindingProjectType.Modern ? "UnifiedExample" : "XM45Example")}.app/Contents/{path}")), $"{path} not in bundle as expected.");
		}

		[TestCase (BindingProjectType.Modern, false)]
		[TestCase (BindingProjectType.Full, true)]
		public void LibrariesEmbeddedProperly (BindingProjectType type, bool useProjectReference)
		{
			MMPTests.RunMMPTest (tmpDir => {
				var projects = BindingProjectTests.GenerateTestProject (type, tmpDir);
				BindingProjectTests.SetNoEmbedding (projects.Item1);

				string appBuildLog = BindingProjectTests.SetupAndBuildLinkedTestProjects (projects.Item1, projects.Item2, tmpDir, useProjectReference, setupDefaultNativeReference: true).Item2;

				AssertNoResourceWithName (tmpDir, projects.Item1.ProjectName, "SimpleClassDylib.dylib");
				AssertFileInBundle (tmpDir, type, "MonoBundle/SimpleClassDylib.dylib");
			});
		}

		[TestCase (BindingProjectType.Modern, true)]
		[TestCase (BindingProjectType.Full, false)]
		public void FrameworksEmbeddedProperly (BindingProjectType type, bool useProjectReference)
		{
			MMPTests.RunMMPTest (tmpDir => {
				string frameworkPath = FrameworkBuilder.CreateThinFramework (tmpDir);

				var projects = BindingProjectTests.GenerateTestProject (type, tmpDir);
				BindingProjectTests.SetNoEmbedding (projects.Item1);
				projects.Item1.ItemGroup = NativeReferenceTests.CreateSingleNativeRef (frameworkPath, "Framework");

				string appBuildLog = BindingProjectTests.SetupAndBuildLinkedTestProjects (projects.Item1, projects.Item2, tmpDir, useProjectReference, false).Item2;
			
				AssertNoResourceWithName (tmpDir, projects.Item1.ProjectName, "Foo");
				AssertFileInBundle (tmpDir, type, "Frameworks/Foo.framework/Foo");
			});
		}

		[TestCase (BindingProjectType.Modern)]
		public void DoesNotSupportLinkWith (BindingProjectType type)
		{
			MMPTests.RunMMPTest (tmpDir => {
				var projects = BindingProjectTests.GenerateTestProject (type, tmpDir);
				BindingProjectTests.SetNoEmbedding (projects.Item1);

				projects.Item1.LinkWithName = "SimpleClassDylib.dylib";

				string libBuildLog = BindingProjectTests.SetupAndBuildBindingProject (projects.Item1, false);
				Assert.True (libBuildLog.Contains ("NoBindingEmbedding style binding projects must have native reference"), $"Did not fail as expected: {TI.PrintRedirectIfLong (libBuildLog)}");
			});
		}

		[TestCase (true)]
		[TestCase (false)]
		public void ShouldNotUnnecessarilyRebuildBindingProject (bool framework)
		{
			MMPTests.RunMMPTest (tmpDir => {
				var projects = BindingProjectTests.GenerateTestProject (BindingProjectType.Modern, tmpDir);
				BindingProjectTests.SetNoEmbedding (projects.Item1);

				string projectPath = Path.Combine (tmpDir, "MobileBinding.csproj");
				string bindingFilePath = Path.Combine (tmpDir, "ApiDefinition.cs");
				const string CreatePackageString = "Creating binding resource package";

				// First build should create a package
				string frameworkPath = null;

				if (framework) {
					frameworkPath = FrameworkBuilder.CreateThinFramework (tmpDir);
					projects.Item1.ItemGroup += NativeReferenceTests.CreateSingleNativeRef (Path.GetFullPath (frameworkPath), "Framework");
				}
				string libBuildLog = BindingProjectTests.SetupAndBuildBindingProject (projects.Item1, setupDefaultNativeReference: !framework);

				Assert.True (libBuildLog.Contains (CreatePackageString), $"First build did not create package? {TI.PrintRedirectIfLong (libBuildLog)}");

				// No change build should not
				libBuildLog = TI.BuildProject (projectPath, true);
				Assert.False (libBuildLog.Contains (CreatePackageString), $"Rebuild build did create package? {TI.PrintRedirectIfLong (libBuildLog)}");

				// Touching the binding project should
				Touch (projectPath);
				libBuildLog = TI.BuildProject (projectPath, true);
				Assert.True (libBuildLog.Contains (CreatePackageString), $"Binding Project build did not create package? {TI.PrintRedirectIfLong (libBuildLog)}");

				// Touching the binding file should
				Touch (bindingFilePath);
				libBuildLog = TI.BuildProject (projectPath, true);
				Assert.True (libBuildLog.Contains (CreatePackageString), $"Binding File build did not create package? {TI.PrintRedirectIfLong (libBuildLog)}");

				// No change build should not
				libBuildLog = TI.BuildProject (projectPath, true);
				Assert.False (libBuildLog.Contains (CreatePackageString), $"Second Rebuild build did create package? {TI.PrintRedirectIfLong (libBuildLog)}");

				// Touching native library should
				Touch (framework ? frameworkPath + "/Foo" : NativeReferenceTests.SimpleDylibPath);
				libBuildLog = TI.BuildProject (projectPath, true);
				Assert.True (libBuildLog.Contains (CreatePackageString), $"Native Library build did not create package? {TI.PrintRedirectIfLong (libBuildLog)}");
			});
		}

		[TestCase (true, true)]
		[TestCase (true, false)]
		[TestCase (false, true)]
		[TestCase (false, false)]
		public void ShouldNotUnnecessarilyRebuildFinalProject (bool useProjectReference, bool framework)
		{
			MMPTests.RunMMPTest (tmpDir => {
				var projects = BindingProjectTests.GenerateTestProject (BindingProjectType.Modern, tmpDir);
				BindingProjectTests.SetNoEmbedding (projects.Item1);

				const string BuildString = "xcrun -sdk macosx clang";

				string frameworkPath = null;
				if (framework) {
					frameworkPath = FrameworkBuilder.CreateThinFramework (tmpDir);
					projects.Item1.ItemGroup += NativeReferenceTests.CreateSingleNativeRef (Path.GetFullPath (frameworkPath), "Framework");
				}
				string appBuildLog = BindingProjectTests.SetupAndBuildLinkedTestProjects (projects.Item1, projects.Item2, tmpDir, useProjectReference, setupDefaultNativeReference: !framework).Item2;

				Assert.True (appBuildLog.Contains (BuildString), $"First build did not run mmp? {TI.PrintRedirectIfLong (appBuildLog)}");

				string projectPath = Path.Combine (tmpDir, "UnifiedExample.csproj");
				string mainPath = Path.Combine (tmpDir, "Main.cs");

				// No change build should not
				string buildLog = TI.BuildProject (projectPath, true);
				Assert.False (buildLog.Contains (BuildString), $"Rebuild ran mmp again? {TI.PrintRedirectIfLong (buildLog)}");

				if (useProjectReference) {
					// Touching the binding definition should
					Touch (Path.Combine (tmpDir, "ApiDefinition.cs"));
					buildLog = TI.BuildProject (projectPath, true);
					Assert.True (buildLog.Contains (BuildString), $"Binding definition build did not run mmp again? {TI.PrintRedirectIfLong (buildLog)}");
				}
				else {
					// Touching the binding assembly should
					Touch (Path.Combine (tmpDir, "bin/Debug/MobileBinding.dll"));
					buildLog = TI.BuildProject (projectPath, true);
					Assert.True (buildLog.Contains (BuildString), $"Binding build did not run mmp again? {TI.PrintRedirectIfLong (buildLog)}");
				}
			});
		}

		[TestCase (true)]
		[TestCase (false)]
		public void MultipleNativeReferences (bool useProjectReference)
		{
			MMPTests.RunMMPTest (tmpDir => {
				// This is a bit of a hack, you can't just rename dylibs like this
				string secondNativeLibPath = Path.Combine (tmpDir, "SimpleClassDylib2.dylib");
				File.Copy (NativeReferenceTests.SimpleDylibPath, secondNativeLibPath);

				var projects = BindingProjectTests.GenerateTestProject (BindingProjectType.Modern, tmpDir);
				BindingProjectTests.SetNoEmbedding (projects.Item1);
				projects.Item1.ItemGroup += NativeReferenceTests.CreateSingleNativeRef (secondNativeLibPath, "Dynamic");

				BindingProjectTests.SetupAndBuildLinkedTestProjects (projects.Item1, projects.Item2, tmpDir, useProjectReference, setupDefaultNativeReference: true);

				// manifest and 2 dylibs
				Assert.AreEqual (3, Directory.GetFiles (Path.Combine (tmpDir, "bin/Debug/MobileBinding.resources")).Length);

				// 2 dylibs + libMonoPosixHelper.dylib
				Assert.AreEqual (3, Directory.GetFiles (Path.Combine (tmpDir, "bin/Debug/UnifiedExample.app/Contents/MonoBundle")).Where (x => x.EndsWith (".dylib")).Count ());
			});
		}

		[TestCase (true)]
		// [TestCase (false)] - TODO App -> Lib -> Binding is a known failing case for now
		public void MultipleDependencyChain (bool useProjectReference)
		{
			// App can depend on Lib that depends on binding lib with native reference and everything gets packaged in correctly
			MMPTests.RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig binding = new TI.UnifiedTestConfig (tmpDir) { ProjectName = "MobileBinding.csproj" };
				binding.ItemGroup += NativeReferenceTests.CreateSingleNativeRef (NativeReferenceTests.SimpleDylibPath, "Dynamic");
				binding.APIDefinitionConfig += @"[BaseType (typeof (NSObject))]
  interface SimpleClass {
    [Export (""doIt"")]
    int DoIt ();
  }";
				BindingProjectTests.SetNoEmbedding (binding);

				TI.GenerateBindingLibraryProject (binding);
				TI.BuildProject (Path.Combine (tmpDir, "MobileBinding.csproj"), true);


				TI.UnifiedTestConfig library = new TI.UnifiedTestConfig (tmpDir) { ProjectName = "UnifiedLibrary" };
				library.TestCode = "public class MyClass { public static void Go () { var c = new ExampleBinding.SimpleClass (); c.DoIt (); } }";

				if (useProjectReference) 
					library.References = $@"<ProjectReference Include=""MobileBinding.csproj"" />";
				else 
					library.References = $@"<Reference Include=""MobileBinding""><HintPath>{Path.Combine (tmpDir, "bin/Debug", "MobileBinding.dll")}</HintPath></Reference>";

				TI.GenerateUnifiedLibraryProject (library);
				TI.BuildProject (Path.Combine (tmpDir, "UnifiedLibrary.csproj"), true);

				TI.UnifiedTestConfig project = new TI.UnifiedTestConfig (tmpDir) { ProjectName = "UnifiedExample.csproj" };
				project.TestCode = "MyClass.Go ();";

				if (useProjectReference) 
					project.References = $@"<ProjectReference Include=""UnifiedLibrary.csproj"" />";
				else 
					project.References = $@"<Reference Include=""UnifiedLibrary""><HintPath>{Path.Combine (tmpDir, "bin/Debug", "UnifiedLibrary.dll")}</HintPath></Reference>";

				TI.GenerateUnifiedExecutableProject (project);
				TI.BuildProject (Path.Combine (tmpDir, "UnifiedExample.csproj"), true);
			});
		}

		[Test]
		public void CleanShouldRemoveBundle ()
		{
			MMPTests.RunMMPTest (tmpDir => {
				var projects = BindingProjectTests.GenerateTestProject (BindingProjectType.Modern, tmpDir);
				BindingProjectTests.SetNoEmbedding (projects.Item1);
				
				string libBuildLog = BindingProjectTests.SetupAndBuildBindingProject (projects.Item1, false);

				TI.CleanUnifiedProject (Path.Combine (tmpDir, projects.Item1.ProjectName));
				Assert.False (Directory.Exists (Path.Combine (tmpDir, "bin/Debug/MobileBinding.resources")), "Resource bundle was not cleaned up");
			});
		}
	}
}
