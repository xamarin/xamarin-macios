using System.Threading;

using Network;

using MonoTests.System.Net.Http;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWConnectionTest {
		ConnectionManager manager;
		NWConnection connection;

		[SetUp]
		public void SetUp ()
		{
			manager = new ConnectionManager ();
			connection = manager.CreateConnection ();
		}

		[TearDown]
		public void TearDown () => manager?.Dispose ();

		[Test]
		public void TestEndpointProperty () => Assert.That (connection.Endpoint, Is.Not.Null);

		[Test]
		public void TestParametersProperty () => Assert.That (connection.Parameters, Is.Not.Null);

		[Test]
		public void TestSetQPropertyNull () => Assert.Throws<ArgumentNullException> (() => connection.SetQueue (null));

		[Test]
		public void TestBatch ()
		{
			var count = 0;
			connection.Batch (() => count++);
			Assert.That (count, Is.EqualTo (1), "Batch callback must run before returning");
		}

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
			Assert.That (cancelled.WaitOne (3000), Is.True, "Cancelled");
			connection.Cancel ();
			// lib should ignore the second call
			Assert.That (cancelled.WaitOne (3000), Is.False);
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
			Assert.That (cancelled.WaitOne (3000), Is.True, "Cancelled");
			connection.ForceCancel ();
			// lib should ignore the second call
			Assert.That (cancelled.WaitOne (3000), Is.False);
		}
	}

	class ConnectionManager : IDisposable {
		string host = NetworkResources.MicrosoftUri.Host;
		AutoResetEvent connectedEvent = new AutoResetEvent (false);  // used to let us know when the connection was established so that we can access the Report
		NWConnection? connection;
		NWParameters? parameters;
		bool tcp;

		public ConnectionManager (bool tcp = false, string? host = null)
		{
			this.tcp = tcp;
			if (host is not null)
				this.host = host;
		}

		public void Dispose ()
		{
			connection?.Dispose ();
			connection = null;
			parameters?.Dispose ();
			parameters = null;
		}

		void ConnectionStateHandler (NWConnectionState state, NWError error)
		{
			switch (state) {
			case NWConnectionState.Ready:
				connectedEvent.Set ();
				break;
			case NWConnectionState.Cancelled:
				break;
			case NWConnectionState.Invalid:
			case NWConnectionState.Failed:
				Assert.Inconclusive ($"Network connection could not be performed: {error}");
				break;
			}
		}

		public NWConnection CreateConnection ()
		{
			return CreateConnection (out var _);
		}

		public NWConnection CreateConnection (out NWParameters parameters)
		{
			// connect and once the connection is done, deal with the diff tests
			// we create a connection which we are going to use to get the availabe
			// interfaces, that way we can later test protperties of the NWParameters class.

			Exception? e = null;

			parameters = (tcp ? NWParameters.CreateTcp () : NWParameters.CreateUdp ());

			using (var endpoint = NWEndpoint.Create (host, "80")) {
				using (var protocolStack = parameters.ProtocolStack) {
					var ipOptions = protocolStack.InternetProtocol;
					ipOptions.SetVersion (NWIPVersion.Version4);
				}
				var connection = new NWConnection (endpoint, parameters);
				connection.SetQueue (DispatchQueue.DefaultGlobalQueue); // important, else we will get blocked
				connection.SetStateChangeHandler ((state, error) => {
					try {
						ConnectionStateHandler (state, error);
					} catch (Exception ex) {
						e = ex;
					}
				});
				connection.Start ();
				Assert.That (connectedEvent.WaitOne (20000), Is.True, "Connection timed out.");
				Assert.That (e, Is.Null, "No exception");
				return connection;
			}
		}
	}
}
