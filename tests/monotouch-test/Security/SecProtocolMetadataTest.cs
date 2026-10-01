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

		static void CheckOptionalAccess<T> (Action<Action<T>> access, Action<T> visit, string unavailableMessage)
		{
			var count = 0;
			try {
				access (value => {
					count++;
					visit (value);
				});
			} catch (InvalidOperationException ex) {
				Assert.That (ex.Message, Is.EqualTo (unavailableMessage), "Unavailable data");
				Assert.That (count, Is.EqualTo (0), "An unavailable collection must not invoke the callback");
				Console.WriteLine (unavailableMessage);
			}
		}

		// The public AddPreSharedKey overload configures legacy DH parameters, not a PSK/identity pair.
		[DllImport (Constants.SecurityLibrary)]
		static extern void sec_protocol_options_add_pre_shared_key (IntPtr options, IntPtr psk, IntPtr identity);

		[Test]
		public void AccessPreSharedKeys ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);

			var keyBytes = new byte [] { 1, 2, 3, 4, 5, 6, 7, 8 };
			var identityBytes = new byte [] { 9, 10, 11, 12 };
			var keys = new List<(DispatchData Key, DispatchData Identity)> ();
			try {
				using (var key = DispatchData.FromByteBuffer (keyBytes))
				using (var identity = DispatchData.FromByteBuffer (identityBytes))
				using (var tls = new NWProtocolTlsOptions ())
				using (var options = tls.ProtocolOptions)
				using (var serverParameters = NWParameters.CreateTcp ())
				using (var clientParameters = NWParameters.CreateTcp ())
				using (var serverStack = serverParameters.ProtocolStack)
				using (var clientStack = clientParameters.ProtocolStack)
				using (var localEndpoint = NWEndpoint.Create ("127.0.0.1", "0"))
				using (var queue = new DispatchQueue ("SecProtocolMetadataTest.AccessPreSharedKeys"))
				using (var listenerReady = new ManualResetEvent (false))
				using (var clientReady = new ManualResetEvent (false)) {
					sec_protocol_options_add_pre_shared_key (options.Handle, key.Handle, identity.Handle);
					serverStack.PrependApplicationProtocol (tls);
					clientStack.PrependApplicationProtocol (tls);
					serverParameters.LocalEndpoint = localEndpoint;

					using var listener = NWListener.Create (serverParameters);
					Assert.That (listener, Is.Not.Null, "Listener");
					listener.ConnectionLimit = 1;
					var listenerState = NWListenerState.Invalid;
					var clientState = NWConnectionState.Invalid;
					NWConnection client = null;
					NWConnection server = null;
					listener.SetQueue (queue);
					listener.SetStateChangedHandler ((state, error) => {
						listenerState = state;
						if (state == NWListenerState.Ready || state == NWListenerState.Failed)
							listenerReady.Set ();
					});
					listener.SetNewConnectionHandler (connection => {
						server = connection;
						server.SetQueue (queue);
						server.Start ();
					});
					try {
						listener.Start ();
						Assert.That (listenerReady.WaitOne (TimeSpan.FromSeconds (10)), Is.True, "Listener state change");
						Assert.That (listenerState, Is.EqualTo (NWListenerState.Ready), "Listener ready");

						using var endpoint = NWEndpoint.Create ("127.0.0.1", listener.Port.ToString ());
						client = new NWConnection (endpoint, clientParameters);
						client.SetQueue (queue);
						client.SetStateChangeHandler ((state, error) => {
							clientState = state;
							if (state == NWConnectionState.Ready || state == NWConnectionState.Failed)
								clientReady.Set ();
						});
						client.Start ();
						Assert.That (clientReady.WaitOne (TimeSpan.FromSeconds (10)), Is.True, "Client state change");
						Assert.That (clientState, Is.EqualTo (NWConnectionState.Ready), "Client ready");

						using var definition = NWProtocolDefinition.CreateTlsDefinition ();
						using var metadata = client.GetProtocolMetadata<NWTlsMetadata> (definition);
						using var security = metadata.SecProtocolMetadata;
						Assert.Throws<ArgumentNullException> (() => security.AccessPreSharedKeys (null), "Null PSK handler");
						Assert.That (security.AccessPreSharedKeys ((psk, pskIdentity) => {
							keys.Add ((psk, pskIdentity));
						}), Is.True, "PSKs accessible");
					} finally {
						// Detach handlers on their serial queue before disposing captured state.
						queue.DispatchSync (() => {
							listener.SetStateChangedHandler (null);
							listener.SetNewConnectionHandler (null);
							listener.Cancel ();
							if (client is not null) {
								client.SetStateChangeHandler (null);
								client.Cancel ();
								client.Dispose ();
							}
							if (server is not null) {
								server.Cancel ();
								server.Dispose ();
							}
						});
					}
				}

				Assert.That (keys.Count, Is.EqualTo (1), "PSK callback count");
				Assert.That (keys [0].Key.ToArray (), Is.EqualTo (keyBytes), "Key after callback and connection disposal");
				Assert.That (keys [0].Identity.ToArray (), Is.EqualTo (identityBytes), "Identity after callback and connection disposal");
			} finally {
				foreach (var pair in keys) {
					pair.Key.Dispose ();
					pair.Identity.Dispose ();
				}
			}
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
						using var s = m.SecProtocolMetadata;
						Assert.That (s.EarlyDataAccepted, Is.False, "EarlyDataAccepted");
						Assert.That (s.NegotiatedProtocol, Is.Null, "NegotiatedProtocol");
						Assert.That (s.NegotiatedProtocolVersion, Is.EqualTo (SslProtocol.Tls_1_2).Or.EqualTo (SslProtocol.Tls_1_3), "NegotiatedProtocolVersion");
						Assert.That (s.PeerPublicKey, Is.Null.Or.Not.Null, "PeerPublicKey");

						Assert.That (SecProtocolMetadata.ChallengeParametersAreEqual (s, s), Is.True, "ChallengeParametersAreEqual");
						Assert.That (SecProtocolMetadata.PeersAreEqual (s, s), Is.True, "PeersAreEqual");

						Assert.Throws<ArgumentNullException> (() => s.SetDistinguishedNamesForPeerHandler (null), "Null distinguished names handler");
						Assert.Throws<ArgumentNullException> (() => s.SetOcspResponseForPeerHandler (null), "Null OCSP handler");
						Assert.Throws<ArgumentNullException> (() => s.SetCertificateChainForPeerHandler (null), "Null certificate handler");
						Assert.Throws<ArgumentNullException> (() => s.SetSignatureAlgorithmsForPeerHandler (null), "Null signature handler");

						var certificateSubjects = new List<string> ();
						s.SetCertificateChainForPeerHandler (certificate => {
							using (certificate)
								certificateSubjects.Add (certificate.SubjectSummary);
						});
						Assert.That (certificateSubjects, Is.Not.Empty, "Peer certificate chain");
						Assert.That (certificateSubjects, Has.None.Null, "Certificate subjects");

						// These collections depend on the TLS handshake and may be unavailable.
						CheckOptionalAccess<DispatchData> (s.SetDistinguishedNamesForPeerHandler, data => {
							data.Dispose ();
						}, "Distinguished names are not accessible.");
						CheckOptionalAccess<DispatchData> (s.SetOcspResponseForPeerHandler, data => {
							data.Dispose ();
						}, "The OCSP response is not accessible.");
						CheckOptionalAccess<ushort> (s.SetSignatureAlgorithmsForPeerHandler, algorithm => {
						}, "The supported signature list is not accessible.");

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
							// This public endpoint does not use TLS-PSK.
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
