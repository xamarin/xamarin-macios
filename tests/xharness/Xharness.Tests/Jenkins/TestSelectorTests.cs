using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Xharness.Jenkins;

namespace Xharness.Tests.Jenkins {
	
	[TestFixture]
	public class TestSelectorTests {
		Xharness.Jenkins.Jenkins jenkins;
		Mock<IResultParser> resultParser;
		Mock<ITunnelBore> tunnelBore;	
		Mock<IHarness> harness;
		Mock<IProcessManager> processManager;
		Mock<IVersionControlSystem> versionControlSystem;
		Mock<ILog> log;
		TestSelector testSelector;
		string tmpPath;
		
		[SetUp]
		public void SetUp ()
		{
			tmpPath = Path.GetTempPath ();
			harness = new Mock<IHarness> (); 
			resultParser = new Mock<IResultParser> ();
			tunnelBore = new Mock<ITunnelBore> ();
			processManager = new Mock<IProcessManager> ();
			versionControlSystem = new Mock<IVersionControlSystem> ();
			log = new Mock<ILog> ();

			// defult expectations
			harness.Setup (h => h.ENABLE_XAMARIN).Returns (true);
			harness.Setup (h => h.JENKINS_RESULTS_DIRECTORY).Returns (tmpPath);
			harness.Setup (h => h.Labels).Returns (new HashSet<string> ());
			harness.Setup (h => h.XcodeRoot).Returns ("Xcode11.app");

			jenkins = new Xharness.Jenkins.Jenkins (harness.Object, processManager.Object, resultParser.Object,
				tunnelBore.Object);
			jenkins.MainLog = log.Object;
			testSelector = new TestSelector (jenkins, processManager.Object, versionControlSystem.Object);
		}

		[TearDown]
		public void TearDown ()
		{
			processManager = null;
			versionControlSystem = null;
			harness = null;
			jenkins = null;
			testSelector = null;
			Environment.SetEnvironmentVariable ("BRANCH_NAME", null);
		}


		public class TestCasesData {
			public static IEnumerable SelectByLabelTestData {
				get {
					// create the test data to ensure that the correct tests are selected according to the labels

					// all tests selected, not main branch
					TestSelection expectedSelection = TestSelection.All;
					yield return new TestCaseData ("testBranch", new List<string> {"run-all-tests"}, expectedSelection, true);

					// all tests selected via tag, but we added some skips, therefore we skip them
					expectedSelection = TestSelection.All;
					expectedSelection &= ~TestSelection.MacOS;
					yield return new TestCaseData ("testBranch", 
						new List<string> { "run-all-tests", "skip-mac-tests"},
						expectedSelection, true);
					
					// skip all ye select one of the tests
					expectedSelection = TestSelection.MacOS;
					yield return new TestCaseData ("testBranch",
						new List<string> {"skip-all-tests", "run-mac-tests"},
						expectedSelection, false);

					// select the defaults + some additional ones
					expectedSelection = Xharness.Jenkins.Jenkins.DefaultTestSelection | TestSelection.Bcl | TestSelection.Cecil;
					yield return new TestCaseData ("testBranch",
						new List<string> {"run-bcl-tests", "run-cecil-tests"},
						expectedSelection, false);

					// de-select one of the default tests
					expectedSelection = Xharness.Jenkins.Jenkins.DefaultTestSelection | TestSelection.Bcl;
					expectedSelection &= ~TestSelection.iOSMSBuild;
					yield return new TestCaseData ("testBranch",
						new List<string> {"skip-ios-msbuild-tests", "run-bcl-tests"},
						expectedSelection, false);

					// select the special case of the docs, this happens when the branch is main
					expectedSelection = Xharness.Jenkins.Jenkins.DefaultTestSelection | TestSelection.Docs;
					yield return new TestCaseData ("main", new List<string>(), expectedSelection, false);

					// select the docs specifically
					expectedSelection = Xharness.Jenkins.Jenkins.DefaultTestSelection | TestSelection.Docs;
					yield return new TestCaseData ("testBranch", new List<string> {"run-docs-tests"}, expectedSelection, false);

					// select the system permission tests, this is a special case since we do not update the selection
					// but we do check the value of the xharness propery
					expectedSelection = Xharness.Jenkins.Jenkins.DefaultTestSelection;
					yield return new TestCaseData ("testBranch", new List<string> { "run-system-permission-tests" }, expectedSelection, true);

					expectedSelection &= ~TestSelection.All; // skip all
					yield return new TestCaseData ("testBranch", new List<string> { "skip-all-tests" }, expectedSelection, false);

				}
			}
			
			public static IEnumerable SelectByModifiedFileTestData {
				get {
					var defaultSelection = Xharness.Jenkins.Jenkins.DefaultTestSelection;
					TestSelection expectedSelection = defaultSelection | TestSelection.Mtouch;
					yield return new TestCaseData (new List<string> {"tests/mtouch/test_file"}, expectedSelection); 
					
					expectedSelection = defaultSelection | TestSelection.Mmp;
					yield return new TestCaseData (new List<string> {"tests/mmptest/test_file"}, expectedSelection); 
					
					expectedSelection = defaultSelection | TestSelection.Bcl;
					yield return new TestCaseData (new List<string> {"tests/bcl-test/test_file"}, expectedSelection); 
					
					// special case, common updates a lot of tests
					expectedSelection = defaultSelection | TestSelection.Mtouch | TestSelection.Mmp | TestSelection.Bcl | TestSelection.Btouch;
					yield return new TestCaseData (new List<string> {"tests/common/test_file"}, expectedSelection);

					expectedSelection = defaultSelection | TestSelection.Xtro;
					yield return new TestCaseData (new List<string> {"tests/xtro-sharpie/test_file"}, expectedSelection); 
					
					expectedSelection = defaultSelection | TestSelection.Cecil;
					yield return new TestCaseData (new List<string> {"tests/cecil-tests/test_file"}, expectedSelection); 
					
					// src is a special case, updates a few
					expectedSelection = defaultSelection | TestSelection.Cecil | TestSelection.Xtro;
					yield return new TestCaseData (new List<string> {"src/test_file"}, expectedSelection); 
				}
			}
		}
		
		[Test, TestCaseSource (typeof (TestCasesData), "SelectByLabelTestData")]
		public void SelectTestByLabelTests (string branchName, List<string> labels, TestSelection expectedSelection, bool includeSystemPermission)
		{
			versionControlSystem.Setup (vcs => vcs.GetLabels (It.IsAny<int> ())).Returns (labels);
			Environment.SetEnvironmentVariable ("BRANCH_NAME", branchName);
			testSelector.SelectTestsByLabel (10);
			Assert.AreEqual (expectedSelection, jenkins.TestSelection, $"Test selection for {string.Join (",", labels)}");
			// verify that we did set the correct value in the xharness interface
			harness.VerifySet (x => x.IncludeSystemPermissionTests = includeSystemPermission, $"Setting system tests for {string.Join (",", labels)}");
		}

		[Test, TestCaseSource (typeof(TestCasesData), "SelectByModifiedFileTestData")]
		public void SelectTestsByModifiedFilesTests (List<string> modifiedFiles, TestSelection expectedSelection)
		{
			versionControlSystem.Setup (v => v.GetModifiedFiles (It.IsAny<int> ())).Returns (modifiedFiles);
			testSelector.SelectTestsByModifiedFiles (10);
			Assert.AreEqual (expectedSelection, jenkins.TestSelection, $"Test selection for modfied files {string.Join (",", modifiedFiles)}");
		}
	}
}
