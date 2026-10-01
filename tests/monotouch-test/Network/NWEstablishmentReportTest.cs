using System.Threading;

using Network;

using MonoTests.System.Net.Http;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWEstablishmentReportTest {
		AutoResetEvent reportEvent;  // used to let us know when the connection was established and we got the report
		ConnectionManager manager;
		NWConnection connection;
		NWEstablishmentReport report;

		[OneTimeSetUp]
		public void Init ()
		{
			// connect so that we can later when the report and test with it
			reportEvent = new AutoResetEvent (false);


			manager = new ConnectionManager ();
			connection = manager.CreateConnection ();

			connection.GetEstablishmentReport (DispatchQueue.DefaultGlobalQueue, (r) => {
				report = r;
				reportEvent.Set ();
			});
			Assert.That (reportEvent.WaitOne (20000), Is.True, "Connection timed out.");
		}

		[OneTimeTearDown]
		public void Dispose ()
		{
			report?.Dispose ();
			manager?.Dispose ();
		}

		[Test]
		public void TestUsedProxy ()
		{
			TestRuntime.IgnoreInCI ("CI bots might have proxies setup and will mean that the test will fail.");
			Assert.That (report.UsedProxy, Is.False, "Used proxy");
		}

		[Test]
		public void TestProxyConfigured ()
		{
			TestRuntime.IgnoreInCI ("CI bots might have proxies setup and will mean that the test will fail.");
			Assert.That (report.ProxyConfigured, Is.False, "Proxy configured.");
		}

		[Test]
		public void TestPreviousAttemptCount () => Assert.That (report.PreviousAttemptCount, Is.Not.EqualTo (uint.MaxValue));

		[Test]
		public void TestDuration () => Assert.That (report.Duration > TimeSpan.MinValue, Is.True);

		[Test]
		public void TestConnectionSetupTime () => Assert.That (report.ConnectionSetupTime > TimeSpan.MinValue, Is.True);

		[Test]
		public void TestEnumerateResolutions ()
		{
			var e = new AutoResetEvent (false);
			report.EnumerateResolutions ((source, duration, count, endpoint, preferred) => {
				Assert.That (duration > TimeSpan.MinValue, Is.True, "Durantion");
				Assert.That (count, Is.Not.EqualTo (0), "Count");
				Assert.That (endpoint, Is.Not.Null, "endpoint");
				Assert.That (preferred, Is.Not.Null, "preferred");
				e.Set ();
			});
			e.WaitOne ();
		}

		[Test]
		public void TestProxyEnpoint ()
		{
			TestRuntime.IgnoreInCI ("CI bots might have proxies setup and will mean that the test will fail.");
			Assert.That (report.ProxyEndpoint, Is.Null);
		}

		[Test]
		public void EnumerateResolutionReportsTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
		}

	}
}
