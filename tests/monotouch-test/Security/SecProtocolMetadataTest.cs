using System.Threading;

using Network;
using Security;

namespace MonoTouchFixtures.Security {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SecProtocolMetadataTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
		}

		[Test]
		public void TlsDefaults ()
		{
			using (var ep = NWEndpoint.Create ("www.microsoft.com", "https"))
			using (var parameters = NWParameters.CreateSecureTcp ())
			using (var queue = new DispatchQueue (GetType ().FullName)) {
				var connection = new NWConnection (ep, parameters);

				var ready = new ManualResetEvent (false);
				var anyStateChange = new ManualResetEvent (false);
				var done = new ManualResetEvent (false);
				connection.SetStateChangeHandler ((state, error) => {
					Console.WriteLine (state);
					anyStateChange.Set ();
					switch (state) {
					case NWConnectionState.Cancelled:
					case NWConnectionState.Failed:
						// We can't dispose until the connection has been closed or it failed.
						connection.Dispose ();
						done.Set ();
						break;
					case NWConnectionState.Invalid:
					case NWConnectionState.Preparing:
					case NWConnectionState.Waiting:
						break;
					case NWConnectionState.Ready:
						ready.Set ();
						break;
					default:
						break;
					}
				});

				connection.SetQueue (queue);
				connection.Start ();

				try {
					// Wait until the connection is ready.
					if (!ready.WaitOne (TimeSpan.FromSeconds (10))) {
						// If we're in CI, and didn't get _any_ callbacks, then ignore the failure, since it's likely a network hiccup.
						if (!anyStateChange.WaitOne (0))
							TestRuntime.IgnoreInCI ("Transient network failure - ignore in CI");
						Assert.Fail ("Connection is ready");
					}

					using (var m = connection.GetProtocolMetadata<NWTlsMetadata> (NWProtocolDefinition.CreateTlsDefinition ())) {
						var s = m.SecProtocolMetadata;
						Assert.That (s.EarlyDataAccepted, Is.False, "EarlyDataAccepted");
						Assert.That (s.NegotiatedProtocol, Is.Null, "NegotiatedProtocol");
						Assert.That (s.NegotiatedProtocolVersion, Is.EqualTo (SslProtocol.Tls_1_2).Or.EqualTo (SslProtocol.Tls_1_3), "NegotiatedProtocolVersion");
						Assert.That (s.PeerPublicKey, Is.Null.Or.Not.Null, "PeerPublicKey");

						Assert.That (SecProtocolMetadata.ChallengeParametersAreEqual (s, s), Is.True, "ChallengeParametersAreEqual");
						Assert.That (SecProtocolMetadata.PeersAreEqual (s, s), Is.True, "PeersAreEqual");

						if (TestRuntime.CheckXcodeVersion (11, 0)) {
							using (var d = s.CreateSecret ("Xamarin", 128)) {
								Assert.That (d.Size, Is.EqualTo ((nuint) 128), "CreateSecret-1");
							}
							using (var d = s.CreateSecret ("Microsoft", new byte [1], 256)) {
								Assert.That (d.Size, Is.EqualTo ((nuint) 256), "CreateSecret-2");
							}

							Assert.That (s.NegotiatedTlsProtocolVersion, Is.EqualTo (TlsProtocolVersion.Tls12).Or.EqualTo (TlsProtocolVersion.Tls13), "NegotiatedTlsProtocolVersion");
							// we want to test the binding/API - not the exact value which can vary depending on the negotiation between the client (OS) and server...
							Assert.That (s.NegotiatedTlsCipherSuite, Is.Not.EqualTo (0), "NegotiatedTlsCipherSuite");
							var serverName = s.ServerName;
							if (serverName is null)
								TestRuntime.IgnoreInCI ("ServerName is null - likely network proxy interference");
							Assert.That (serverName, Is.EqualTo ("www.microsoft.com"), "ServerName");
							// we don't have a TLS-PSK enabled server to test this
							Assert.That (s.AccessPreSharedKeys ((psk, pskId) => { }), Is.False, "AccessPreSharedKeys");
						}
					}
				} finally {
					// Cancel the connection and wait for the asynchronous cancellation to complete before
					// leaving this scope. The state-change handler (which disposes the connection) runs on
					// 'queue', so if we let the 'using (queue)' block dispose the queue while a Cancelled/
					// Failed callback is still pending, that callback is left to run on a disposed queue and
					// can fire long after the test finished - wedging the process. Doing this in a 'finally'
					// also guarantees deterministic teardown on every path (the Assert.Fail above, an
					// assertion failure inside the inner 'using', or the IgnoreInCI case).
					try {
						connection.Cancel ();
					} catch (ObjectDisposedException) {
						// The connection already reached a terminal state (Failed/Cancelled) and was disposed
						// by the state-change handler; 'done' is (or is about to be) set by that handler, so the
						// wait below still completes. Swallowing this keeps the original failure - if any - from
						// being masked by an ObjectDisposedException.
					}
					if (!done.WaitOne (TimeSpan.FromSeconds (10)))
						connection.Dispose ();
				}
			}
		}
	}
}
