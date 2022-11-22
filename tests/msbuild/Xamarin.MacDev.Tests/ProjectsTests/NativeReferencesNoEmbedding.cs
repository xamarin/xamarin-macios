using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class NativeReferencesNoEmbedding : ProjectTest {

		public NativeReferencesNoEmbedding (string platform) : base (platform)
		{
		}

		void BuildProjectNoEmbedding (ProjectPaths project, bool clean = true)
		{
			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);

			if (clean)
				RunTarget (project, "Clean");
			RunTarget (project, "Build");
		}

		string GetMessages () => string.Join ("\n", Engine.Logger.MessageEvents.Select (x => x.Message));

		void ClearMessages () => Engine.Logger.MessageEvents.Clear ();

		// [TestCase (true)] MISSING_TEST - Tests are framework only
		// [TestCase (false)] MISSING_TEST - Also, project reference only
		public void LibrariesEmbeddedProperly (bool useProjectReference)
		{
			Assert.Fail ();
		}

		// [TestCase (false)] MISSING_TEST - Project reference only 
		[TestCase (true)]
		public void FrameworksEmbeddedProperly (bool useProjectReference)
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			// TODO - Checked in projects are project reference only...
			Assert.True (useProjectReference);

			var bindingApp = SetupProjectPaths ("MyiOSAppWithBinding");

			BuildProjectNoEmbedding (bindingApp);

			string libPath = Path.Combine (bindingApp.ProjectBinPath, "MyiOSFrameworkBinding.dll");
			Assert.True (File.Exists (libPath), $"Did not find expected library: {libPath}");

			int returnValue = ExecutionHelper.Execute ("/Library/Frameworks/Mono.framework/Commands/monodis", new [] { "--presources", libPath }, out string monoDisResults);
			Assert.AreEqual (0, returnValue);
			Assert.IsFalse (monoDisResults.Contains ("XTest.framework"), $"Binding Library contained embedded resource: {monoDisResults}");

			string finalFrameworkPath = Path.Combine (bindingApp.AppBundlePath, "Frameworks/XTest.framework/XTest");
			Assert.True (File.Exists (finalFrameworkPath), $"{finalFrameworkPath} file was not part of bundle?");
		}

		[TestCase (true)]
		// [TestCase (false)] MISSING_TEST - Framework only tests
		public void ShouldNotUnnecessarilyRebuildBindingProject (bool framework)
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			Assert.True (framework);

			var bindingLib = SetupProjectPaths ("MyiOSFrameworkBinding");

			const string CreatePackageString = "Creating binding resource package";

			// First build should create a package
			BuildProjectNoEmbedding (bindingLib);
			Assert.True (GetMessages ().Contains (CreatePackageString), "First build did not create package?");
			ClearMessages ();

			// No change build should not
			BuildProjectNoEmbedding (bindingLib, clean: false);
			Assert.False (GetMessages ().Contains (CreatePackageString), "Rebuild build did create package?");
			ClearMessages ();

			// Touching the binding project should
			Touch (bindingLib.ProjectCSProjPath);
			BuildProjectNoEmbedding (bindingLib, clean: false);
			Assert.True (GetMessages ().Contains (CreatePackageString), "Binding project build did not create package?");
			ClearMessages ();

			// Touching the binding file should
			Touch (Path.Combine (Configuration.RootPath, "tests", "bindings-framework-test", "ApiDefinition.cs"));
			BuildProjectNoEmbedding (bindingLib, clean: false);
			Assert.True (GetMessages ().Contains (CreatePackageString), "Binding file build did not create package?");
			ClearMessages ();

			// No change build should not
			BuildProjectNoEmbedding (bindingLib, clean: false);
			Assert.False (GetMessages ().Contains (CreatePackageString), "Second rebuild build did create package?");
			ClearMessages ();

			// Touching native library should
			Touch (Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "ios-fat", "XTest.framework/XTest"));
			BuildProjectNoEmbedding (bindingLib, clean: false);
			Assert.True (GetMessages ().Contains (CreatePackageString), "Binding build did not create package?");
		}

		[TestCase (true)]
		// [TestCase (false)] MISSING_TEST - Project reference only 
		public void ShouldNotUnnecessarilyRebuildFinalProject (bool useProjectReference)
		{
			Configuration.IgnoreIfIgnoredPlatform (ApplePlatform.iOS);
			Configuration.AssertLegacyXamarinAvailable (); // Investigate whether this test should be ported to .NET

			Assert.True (useProjectReference);

			var appProject = SetupProjectPaths ("MyiOSAppWithBinding");

			// Look for partial match of the '/path/to/mtouch @responsefile' command
			string BuildString = "mtouch @";

			// First build should create run mtouch
			BuildProjectNoEmbedding (appProject);
			Assert.That (GetMessages (), Does.Contain (BuildString), "First build did not run mtouch?");
			ClearMessages ();

			// But not a rebuild
			BuildProjectNoEmbedding (appProject, clean: false);
			Assert.That (GetMessages (), Does.Not.Contain (BuildString), "Rebuild build did run mtouch?");
			ClearMessages ();

			if (!useProjectReference) {
				Assert.Fail (); // TODO - Checked in projects are project reference only...
			} else {
				var libProject = SetupProjectPaths ("MyiOSFrameworkBinding");

				Touch (libProject.ProjectCSProjPath);
				BuildProjectNoEmbedding (appProject, clean: false);
				Assert.True (GetMessages ().Contains (BuildString), "Binding binary build did not run mtouch?");
			}
		}

		// [TestCase (true)] - MISSING_TEST - Requires special "chain" project that is not checked in
		// [TestCase (false)] - MISSING_TEST - Requires special "chain" project that is not checked in
		public void MultipleDependencyChain (bool useProjectReference)
		{
			Assert.Fail ();
		}
	}
}
