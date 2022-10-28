using System.Linq;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Moq;
using NUnit.Framework;
using Xharness.Jenkins;

namespace Xharness.Tests.Jenkins {

	[TestFixture]
	public class MakeTestTaskEnumerableTests {

		Mock<IMlaunchProcessManager> processManager;
		Mock<IResultParser> resultParser;
		Mock<ITunnelBore> tunnel;

		Harness harness;
		HarnessConfiguration configuration;
		Xharness.Jenkins.Jenkins jenkins;
		MakeTestTaskEnumerable factory;

		[SetUp]
		public void SetUp ()
		{
			processManager = new Mock<IMlaunchProcessManager> ();
			resultParser = new Mock<IResultParser> ();
			tunnel = new Mock<ITunnelBore> ();
			configuration = new HarnessConfiguration ();
			harness = new Harness (resultParser.Object, HarnessAction.Jenkins, configuration);
			jenkins = new Xharness.Jenkins.Jenkins (harness, processManager.Object, resultParser.Object, tunnel.Object);

			factory = new MakeTestTaskEnumerable (jenkins, processManager.Object);
		}

		[TearDown]
		public void TearDown ()
		{
			processManager = null;
			resultParser = null;
			configuration = null;
			harness = null;
			jenkins = null;
			factory = null;
		}

		[TestCase ("MMP Regression Tests")]
		[TestCase ("Mac Binding Projects")]
		[TestCase ("Documentation")]
		public void ContainsTest (string testName)
			=> Assert.True (factory.Select (t => t.TestName == testName).Any (), testName);

		[Test]
		public void CountTest ()
			=> Assert.AreEqual (3, factory.Count (), "Added or removed tests?");
	}
}
