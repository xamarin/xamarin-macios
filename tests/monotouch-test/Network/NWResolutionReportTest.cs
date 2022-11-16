#if !__WATCHOS__
using System.Threading;
using Foundation;
using Network;
using CoreFoundation;
using MonoTests.System.Net.Http;
using NUnit.Framework;

#nullable enable

namespace MonoTouchFixtures.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWResolutionReportTest {
		AutoResetEvent connectedEvent;  // used to let us know when the connection was established so that we can access the Report
		AutoResetEvent reportEvent;  // used to let us know when the connection was established and we got the report
		AutoResetEvent resolutionEvent;  // used to let us know when the connection was established and we got the report
		string host;
		NWConnection connection;
		NWEstablishmentReport report;
		NWResolutionReport resolutionReport;

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
			TestRuntime.AssertXcodeVersion (13, 0);
			TestRuntime.AssertDevice ();
			// connect so that we can later when the report and test with it
			connectedEvent = new AutoResetEvent (false);
			reportEvent = new AutoResetEvent (false);
			resolutionEvent = new AutoResetEvent (false);
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
				report.EnumerateResolutionReports ((r) => {
					resolutionReport = r;
					resolutionEvent.Set ();
				});
				Assert.True (resolutionEvent.WaitOne (20000), "Connection timed out.");
			}
		}

		[OneTimeTearDown]
		public void Dispose ()
		{
			report?.Dispose ();
			resolutionReport?.Dispose ();
			connection?.Dispose ();
		}

		[Test]
		public void SourceTest ()
			=> Assert.DoesNotThrow (() => {
				var src = resolutionReport.Source;
			});

		[Test]
		public void MillisecondsTest ()
			=> Assert.DoesNotThrow (() => {
				var rtime = resolutionReport.Milliseconds;
			});

		[Test]
		public void EndpointCountTest ()
			=> Assert.DoesNotThrow (() => {
				var c = resolutionReport.EndpointCount;
			});

		[Test]
		public void SuccessfulEndpointTest ()
			=> Assert.DoesNotThrow (() => {
				var s = resolutionReport.SuccessfulEndpoint;
			});

		[Test]
		public void PreferredEndpointTest ()
			=> Assert.DoesNotThrow (() => {
				var e = resolutionReport.PreferredEndpoint;
			});

		[Test]
		public void ProtocolTest ()
			=> Assert.DoesNotThrow (() => {
				var p = resolutionReport.Protocol;
			});
	}
}
#endif
