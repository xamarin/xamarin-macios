using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Xharness.Jenkins;

namespace Xharness.Tests.Jenkins {
	public class TestSelectorTests {
		TestSelector selector;
		Mock<IVersionControlSystem> versionControlSystem;

		[SetUp]
		public void SetUp ()
		{
			versionControlSystem = new Mock<IVersionControlSystem> ();
			selector = new TestSelector (null, versionControlSystem.Object);
		}

		[TearDown]
		public void TearDown ()
		{
			selector = null;
		}

		[TestCase ("run-all-tests", TestLabel.All, PlatformLabel.All)]
		[TestCase ("skip-all-tests", TestLabel.None, PlatformLabel.None)]
		[TestCase ("skip-all-tests, run-monotouch-tests", TestLabel.Monotouch, PlatformLabel.None)]
		[TestCase ("skip-all-tests, run-monotouch-tests, run-ios-tests", TestLabel.Monotouch, PlatformLabel.iOS)]
		[TestCase ("skip-all-tests, run-monotouch-tests, run-bcl-tests", TestLabel.Bcl | TestLabel.Monotouch, PlatformLabel.None)]
		[TestCase ("skip-all-tests, run-monotouch-tests, run-bcl-tests, run-tvos-tests, run-ios-tests", TestLabel.Bcl | TestLabel.Monotouch, PlatformLabel.tvOS | PlatformLabel.iOS)]
		[TestCase ("skip-all-tests, run-monotouch-tests, run-bgen-tests, run-bcl-tests", TestLabel.Bcl | TestLabel.Bgen | TestLabel.Monotouch, PlatformLabel.None)]
		[TestCase ("run-monotouch-tests, run-bgen-tests, run-bcl-tests, skip-all-tests", TestLabel.Bcl | TestLabel.Bgen | TestLabel.Monotouch, PlatformLabel.None)]
		[TestCase ("run-monotouch-tests, run-bgen-tests, skip-all-tests, run-bcl-tests", TestLabel.Bcl | TestLabel.Bgen | TestLabel.Monotouch, PlatformLabel.None)]
		[TestCase ("run-monotouch-tests, run-bgen-tests, run-all-tests, run-bcl-tests", TestLabel.All, PlatformLabel.All)]
		public void SelectTestByLabelsTest (string cmdLabels, TestLabel expectedResult, PlatformLabel expectedPlatform)
		{
			var labels = new HashSet<string> ();
			labels.UnionWith (cmdLabels.Split (','));
			var selection = new TestSelection ();
			selector.SelectTestsByLabel (labels, selection);
			Assert.AreEqual (expectedResult, selection.SelectedTests, "Test selection failed.");
			Assert.AreEqual (expectedPlatform, selection.SelectedPlatforms, "Platform selection failed.");
		}
	}
}
