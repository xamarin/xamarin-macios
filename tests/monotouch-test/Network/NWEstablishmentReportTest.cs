#if !__WATCHOS__
using System;
using System.Threading;

using CoreFoundation;
using Foundation;
using Network;

using NUnit.Framework;
using MonoTests.System.Net.Http;


namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWEstablishmentReportTest {

		AutoResetEvent connectedEvent;  // used to let us know when the connection was established so that we can access the Report
		AutoResetEvent reportEvent;  // used to let us know when the connection was established and we got the report
		string host;
		NWConnection connection;
		NWEstablishmentReport report;

		void ConnectionStateHandler (NWConnectionState state, NWError error)
		{
			switch (state) {
			case NWConnectionState.Ready:
				connectedEvent.Set ();
				break;
			case NWConnectionState.Invalid:
			case NWConnectionState.Failed:
				Assert.Inconclusive ("Network connection could not be performed.");
				break;
			}
		}

		[OneTimeSetUp]
		public void Init ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			// connect so that we can later when the report and test with it
			connectedEvent = new AutoResetEvent (false);
			reportEvent = new AutoResetEvent (false);
			host = NetworkResources.MicrosoftUri.Host;
			// we create a connection which we are going to use to get the availabe
			// interfaces, that way we can later test protperties of the NWParameters class.
			using (var parameters = NWParameters.CreateUdp ())
			using (var endpoint = NWEndpoint.Create (host, "80")) {
				using (var protocolStack = parameters.ProtocolStack) {
					var ipOptions = protocolStack.InternetProtocol;
#if NET
					ipOptions.SetVersion (NWIPVersion.Version4);
#else
					ipOptions.IPSetVersion (NWIPVersion.Version4);
#endif
				}
				connection = new NWConnection (endpoint, parameters);
				connection.SetQueue (DispatchQueue.DefaultGlobalQueue); // important, else we will get blocked
				connection.SetStateChangeHandler (ConnectionStateHandler);
				connection.Start ();
				Assert.True (connectedEvent.WaitOne (20000), "Connection timed out.");
				connection.GetEstablishmentReport (DispatchQueue.DefaultGlobalQueue, (r) => {
					report = r;
					reportEvent.Set ();
				});
				Assert.True (reportEvent.WaitOne (20000), "Connection timed out.");
			}
		}

		[OneTimeTearDown]
		public void Dispose ()
		{
			report?.Dispose ();
			connection?.Dispose ();
		}

		[Test]
		public void TestUsedProxy ()
		{
			TestRuntime.IgnoreInCI ("CI bots might have proxies setup and will mean that the test will fail.");
			Assert.IsFalse (report.UsedProxy, "Used proxy");
		}

		[Test]
		public void TestProxyConfigured ()
		{
			TestRuntime.IgnoreInCI ("CI bots might have proxies setup and will mean that the test will fail.");
			Assert.IsFalse (report.ProxyConfigured, "Proxy configured.");
		}

		[Test]
		public void TestPreviousAttemptCount () => Assert.AreNotEqual (uint.MaxValue, report.PreviousAttemptCount);

		[Test]
		public void TestDuration () => Assert.IsTrue (report.Duration > TimeSpan.MinValue);

		[Test]
		public void TestConnectionSetupTime () => Assert.IsTrue (report.ConnectionSetupTime > TimeSpan.MinValue);

		[Test]
		public void TestEnumerateResolutions ()
		{
			var e = new AutoResetEvent (false);
			report.EnumerateResolutions ((source, duration, count, endpoint, preferred) => {
				Assert.IsTrue (duration > TimeSpan.MinValue, "Durantion");
				Assert.AreNotEqual (0, count, "Count");
				Assert.IsNotNull (endpoint, "endpoint");
				Assert.IsNotNull (preferred, "preferred");
				e.Set ();
			});
			e.WaitOne ();
		}

		[Test]
		public void TestProxyEnpoint ()
		{
			TestRuntime.IgnoreInCI ("CI bots might have proxies setup and will mean that the test will fail.");
			Assert.IsNull (report.ProxyEndpoint);
		}

		[Test]
		public void EnumerateResolutionReportsTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
		}

	}
}
#endif
