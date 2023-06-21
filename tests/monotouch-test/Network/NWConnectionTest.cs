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
	public class NWConnectionTest {

		AutoResetEvent connectedEvent;  // used to let us know when the connection was established so that we can access the Report
		string host;
		NWConnection connection;

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
		public void Init () => TestRuntime.AssertXcodeVersion (10, 0);

		[SetUp]
		public void SetUp ()
		{
			// connect and once the connection is done, deal with the diff tests
			connectedEvent = new AutoResetEvent (false);
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
			}

		}

		[TearDown]
		public void TearDown () => connection?.Dispose ();

		[Test]
		public void TestEndpointProperty () => Assert.IsNotNull (connection.Endpoint);

		[Test]
		public void TestParametersProperty () => Assert.IsNotNull (connection.Parameters);

		[Test]
		public void TestSetQPropertyNull () => Assert.Throws<ArgumentNullException> (() => connection.SetQueue (null));

		[Test]
		public void TestCancel ()
		{
			// call cancel, several times, we should not crash
			AutoResetEvent cancelled = new AutoResetEvent (false);
			connection.SetStateChangeHandler ((s, e) => {
				switch (s) {
				case NWConnectionState.Cancelled:
					cancelled.Set ();
					break;
				}
			});
			connection.Cancel ();
			Assert.IsTrue (cancelled.WaitOne (3000), "Cancelled");
			connection.Cancel ();
			// lib should ignore the second call
			Assert.IsFalse (cancelled.WaitOne (3000));
		}

		[Test]
		public void TestForceCancel ()
		{
			// same as cancel, call it several times should be ok
			// call cancel, several times, we should not crash
			AutoResetEvent cancelled = new AutoResetEvent (false);
			connection.SetStateChangeHandler ((s, e) => {
				switch (s) {
				case NWConnectionState.Cancelled:
					cancelled.Set ();
					break;
				}
			});
			connection.ForceCancel ();
			Assert.IsTrue (cancelled.WaitOne (3000), "Cancelled");
			connection.ForceCancel ();
			// lib should ignore the second call
			Assert.IsFalse (cancelled.WaitOne (3000));
		}
	}
}
#endif
