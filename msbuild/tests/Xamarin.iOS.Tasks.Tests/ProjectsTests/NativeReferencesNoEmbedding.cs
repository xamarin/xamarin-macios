using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class NativeReferencesNoEmbedding : ProjectTest {

		public NativeReferencesNoEmbedding (string platform) : base (platform)
		{
		}

		void SetNoBindingEmbedding ()
		{
			// TODO - This is a hack. We should be setting it only on the binding project, but
			// the iOS msbuild test system doesn't have an easy way to access since it's just a project reference
			// and not loaded up here...
			Engine.ProjectCollection.SetGlobalProperty ("NoBindingEmbedding", "true");
		}

		void BuildProjectNoEmbedding (ProjectPaths project, bool clean = true)
		{
			Engine.ProjectCollection.SetGlobalProperty ("Platform", Platform);
			var proj = SetupProject (Engine, project.ProjectCSProjPath);

			SetNoBindingEmbedding ();

			if (clean)
				RunTarget (proj, "Clean", 0);
			RunTarget (proj, "Build", 0);
		}

		static void AssertMtouchArgs (string appBuildLog, int expectedNaitveRefLines, int expectedFrameworkLines)
		{
			string rspFileLine = appBuildLog.SplitLines ().First (x => x.Contains ("mtouch") && x.Contains ("response-file.rsp"));
			string rpsFile = rspFileLine.Split (new char[] { ' ' })[1].Replace ("@", "");

			List<string> responseLines = File.ReadAllLines (rpsFile).ToList ();

			var nativeReferenceLines = responseLines.Where (x => x.Contains ("--native-reference")).ToList ();
			Assert.AreEqual (expectedNaitveRefLines, nativeReferenceLines.Count, $"native-reference Count: {responseLines}");

			var frameworkLines = responseLines.Where (x => x.Contains ("--framework")).ToList ();
			Assert.AreEqual (expectedFrameworkLines, frameworkLines.Count, $"framework Count: {responseLines}");

			var noEmbedLines = responseLines.Where (x => x.Contains ("binding-project-no-embedding")).ToList ();
			// TODO - This should be 1 not 3 but we're harmlessly duplicating binding-project-no-embedding params
			Assert.AreEqual (3, noEmbedLines.Count, $"binding-project-no-embedding Count: {responseLines}");
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
			// TODO - Checked in projects are project reference only...
			Assert.True (useProjectReference);

			var bindingApp = SetupProjectPaths ("MyiOSAppWithBinding", "../", true, Platform);

			BuildProjectNoEmbedding (bindingApp);

			AssertMtouchArgs (GetMessages (), 0, 3);

			string libPath = Path.Combine (Directory.GetCurrentDirectory (), bindingApp.ProjectBinPath, "MyiOSBinding.dll");
			Assert.True (File.Exists (libPath), $"Did not find expected library: {libPath}");

			int returnValue = ExecutionHelper.Execute ("/Library/Frameworks/Mono.framework/Commands/monodis", "--presources " + libPath, out string monoDisResults);
			Assert.AreEqual (0, returnValue);
			Assert.IsFalse (monoDisResults.Contains ("XTest.framework"), $"Binding Library contianed embedded resource: {monoDisResults}");

			string finalFrameworkPath = Path.Combine (bindingApp.AppBundlePath, "Frameworks/XTest.framework/XTest");
			Assert.True (File.Exists (finalFrameworkPath), $"{finalFrameworkPath} file was not part of bundle?");
		}

		// [Test] MISSING_TEST - No LinkWith only projects
		public void DoesNotSupportLinkWith ()
		{
			Assert.Fail ();
		}

		[TestCase (true)]
		// [TestCase (false)] MISSING_TEST - Framework only tests
		public void ShouldNotUnnecessarilyRebuildBindingProject (bool framework)
		{
			Assert.True (framework);

			var bindingLib = SetupProjectPaths ("MyiOSFrameworkBinding", "../", true, "");

			const string CreatePackageString = "Creating binding resource package";

			// First build should create a package
			BuildProjectNoEmbedding (bindingLib);
			Assert.True (GetMessages ().Contains (CreatePackageString), "First build did not create package?");
			ClearMessages ();

			// No change build should not
			BuildProjectNoEmbedding (bindingLib, clean : false);
			Assert.False (GetMessages ().Contains (CreatePackageString), "Rebuild build did create package?");
			ClearMessages ();

			// Touching the binding project should
			Touch (bindingLib.ProjectCSProjPath);
			BuildProjectNoEmbedding (bindingLib, clean: false);
			Assert.True (GetMessages ().Contains (CreatePackageString), "Bindng project build did not create package?");
			ClearMessages ();

			// Touching the binding file should
			Touch (Path.Combine (Path.GetDirectoryName (bindingLib.ProjectCSProjPath), @"../../../tests/bindings-framework-test/ApiDefinition.cs"));
			BuildProjectNoEmbedding (bindingLib, clean: false);
			Assert.True (GetMessages ().Contains (CreatePackageString), "Bindng file build did not create package?");
			ClearMessages ();

			// No change build should not
			BuildProjectNoEmbedding (bindingLib, clean: false);
			Assert.False (GetMessages ().Contains (CreatePackageString), "Second rebuild build did create package?");
			ClearMessages ();

			// Touching native library should
			Touch (Path.Combine (Path.GetDirectoryName (bindingLib.ProjectCSProjPath), @"../../../tests/test-libraries/.libs/ios/XTest.framework/XTest"));
			BuildProjectNoEmbedding (bindingLib, clean: false);
			Assert.True (GetMessages ().Contains (CreatePackageString), "Bindng build did not create package?");
		}

		[TestCase (true)]
		// [TestCase (false)] MISSING_TEST - Project reference only 
		public void ShouldNotUnnecessarilyRebuildFinalProject (bool useProjectReference)
		{
			Assert.True (useProjectReference);

			var appProject = SetupProjectPaths ("MyiOSAppWithBinding", "../", true, "");

			const string BuildString = "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mtouch";

			// First build should create run mtouch
			BuildProjectNoEmbedding (appProject);
			Assert.True (GetMessages ().Contains (BuildString), "First build did not run mtouch?");
			ClearMessages ();

			// But not a rebuild
			BuildProjectNoEmbedding (appProject, clean: false);
			Assert.False (GetMessages ().Contains (BuildString), "Rebuild build did run mtouch?");
			ClearMessages ();

			if (!useProjectReference) {
				Assert.Fail (); // TODO - Checked in projects are project reference only...
			} else {
				Touch (Path.Combine (Path.GetDirectoryName (appProject.ProjectCSProjPath), @"../../../tests/test-libraries/.libs/ios/XTest.framework/XTest"));
				BuildProjectNoEmbedding (appProject, clean: false);
				Assert.True (GetMessages ().Contains (BuildString), "Binding binary build did not run mtouch?");
			}
		}

		// [Test] MISSING_TEST - Project framework only, this test only makes sense with the non-project reference cases
		public void MVIDMismatchDetected ()
		{
			Assert.Fail ();
		}

		// [TestCase (true)] - MISSING_TEST - Requires special "chain" project that is not checked in
		// [TestCase (false)] - TODO App -> Lib -> Binding is a known failing case for now
		public void MultipleDependencyChain (bool useProjectReference)
		{
			Assert.Fail ();
		}
	}
}
