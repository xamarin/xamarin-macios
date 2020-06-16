using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Xharness.Jenkins;

namespace Xharness.Tests.Jenkins {
	
	[TestFixture]
	public class TestSelectorTests {
		Harness harness;
		Xharness.Jenkins.Jenkins jenkins;
		Mock<IResultParser> resultParser;
		Mock<ITunnelBore> tunnelBore;
		Mock<IProcessManager> processManager;
		Mock<IVersionControlSystem> versionControlSystem;
		TestSelector testSelector;
		
		[SetUp]
		public void SetUp ()
		{
			resultParser = new Mock<IResultParser> ();
			tunnelBore = new Mock<ITunnelBore> ();
			processManager = new Mock<IProcessManager> ();
			versionControlSystem = new Mock<IVersionControlSystem> ();
			
			harness = new Harness (resultParser.Object, HarnessAction.Jenkins, new HarnessConfiguration ());

			jenkins = new Xharness.Jenkins.Jenkins (harness, processManager.Object, resultParser.Object,
				tunnelBore.Object);
			testSelector = new TestSelector (jenkins, processManager.Object, versionControlSystem.Object);
		}

		[TearDown]
		public void TearDown ()
		{
			resultParser = null;
			tunnelBore = null;
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
					yield return new TestCaseData ("testBranch", new List<string> {"run-all-tests"}, TestSelection.All);
					
					// all tests selected via tag, but we added some skips, therefore all are selected
					yield return new TestCaseData ("testBranch", 
						new List<string> ("run-all-tests", "skip-mac-tests"),
						TestSelection.All);
					
					// select the defaults + some additional ones
					yield return new TestCaseData ("testBranch",
						new List<string> ("run-bcl-tests", "run-cecil-tests"),
						Xharness.Jenkins.Jenkins.DefaultTestSelection | TestSelection.Bcl | TestSelection.Cecil);
					
					// de-select one of the default tests
					yield return new TestCaseData ("testBranch",
						new List<string> {"skip-ios-msbuild", "run-bcl-tests"},
						(Xharness.Jenkins.Jenkins.DefaultTestSelection | TestSelection.Bcl) & ~TestSelection.iOSMSBuild);
					
					// select the special case of the docs, this happens when the branch is main
					yield return new TestCaseData ("main", new List<string>(),
						Xharness.Jenkins.Jenkins.DefaultTestSelection | TestSelection.Docs);
					
					// select the docs specifically
					yield return new TestCaseData ("testBranch",
						new List<string> {"run-docs-tests"},
						Xharness.Jenkins.Jenkins.DefaultTestSelection | TestSelection.Docs);
					
					// 
				}
			}
		}
		
		[Test, TestCaseSource (typeof (TestCasesData), "SelectByLabelTestData")]
		public void SelectTestByLabelTests (string branchName, List<string> labels, TestSelection expectedSelection)
		{
			// set the labels to be returned by the vcs, which is what will be used to determine the tests
			// to be used
			versionControlSystem.Setup (vcs => vcs.GetLabels (It.IsAny<int> ())).Returns (labels);
			Environment.SetEnvironmentVariable ("BRANCH_NAME", branchName);
			testSelector.SelectTestsByLabel (10);
			Assert.AreEqual (jenkins.TestSelection, expectedSelection, $"Test selection for {string.Join (",", labels)}");
		}
	}
}
