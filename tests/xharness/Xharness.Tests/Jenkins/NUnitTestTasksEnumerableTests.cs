using System.Linq;
using Microsoft.DotNet.XHarness.iOS.Shared;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Listeners;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;
using Moq;
using NUnit.Framework;
using Xharness.Jenkins;

namespace Xharness.Tests.Jenkins {

	[TestFixture]
	public class NUnitTestTaskEnumerableTests {

		Mock<IProcessManager> processManager;
		Mock<IResultParser> resultParser;
		Mock<ITunnelBore> tunnel;
		Mock<ILog> log;

		Harness harness;
		HarnessConfiguration configuration;
		Xharness.Jenkins.Jenkins jenkins;
		NUnitTestTasksEnumerable factory;

		[SetUp]
		public void SetUp ()
		{
			processManager = new Mock<IProcessManager> ();
			resultParser = new Mock<IResultParser> ();
			tunnel = new Mock<ITunnelBore> ();
			log = new Mock<ILog> ();
			configuration = new HarnessConfiguration ();
			harness = new Harness (resultParser.Object, HarnessAction.Jenkins, configuration);
			jenkins = new Xharness.Jenkins.Jenkins (harness, processManager.Object, resultParser.Object, tunnel.Object);
			jenkins.MainLog = log.Object;

			factory = new NUnitTestTasksEnumerable (jenkins, processManager.Object);
		}

		[TestCase ("MSBuild tests")]
		[TestCase ("Install Sources tests")]
		[TestCase ("MTouch tests")]
		[TestCase ("Generator tests")]
		[TestCase ("Cecil-based tests")]
		[TestCase ("Sample tests")]
		public void ContainsTest (string testName)
			=> Assert.True (factory.Select (t => t.TestName == testName).Any (), testName);

		[Test]
		public void CountTest ()
			=> Assert.AreEqual (6, factory.Count (), "Added or removed tests?");
	}
}
