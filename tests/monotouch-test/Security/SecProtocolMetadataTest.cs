#if !__WATCHOS__
using System;
using System.Runtime.InteropServices;
using System.Threading;

using CoreFoundation;
using Foundation;
using Network;
using ObjCRuntime;
using Security;

using NUnit.Framework;

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
				connection.SetStateChangeHandler ((state, error) => {
					Console.WriteLine (state);
					anyStateChange.Set ();
					switch (state) {
					case NWConnectionState.Cancelled:
					case NWConnectionState.Failed:
						// We can't dispose until the connection has been closed or it failed.
						connection.Dispose ();
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

				// Wait until the connection is ready.
				if (!ready.WaitOne (TimeSpan.FromSeconds (10))) {
					// If we're in CI, and didn't get _any_ callbacks, then ignore the failure, since it's likely a network hiccup.
					if (!anyStateChange.WaitOne (0))
						TestRuntime.IgnoreInCI ("Transient network failure - ignore in CI");
					Assert.Fail ("Connection is ready");
				}

#if NET
				using (var m = connection.GetProtocolMetadata<NWTlsMetadata> (NWProtocolDefinition.CreateTlsDefinition ())) {
					var s = m.SecProtocolMetadata;
#else
				using (var m = connection.GetProtocolMetadata (NWProtocolDefinition.TlsDefinition)) {
					var s = m.TlsSecProtocolMetadata;
#endif
					Assert.False (s.EarlyDataAccepted, "EarlyDataAccepted");
#if !NET
					Assert.That (s.NegotiatedCipherSuite, Is.Not.EqualTo (SslCipherSuite.SSL_NULL_WITH_NULL_NULL), "NegotiatedCipherSuite");
#endif
					Assert.Null (s.NegotiatedProtocol, "NegotiatedProtocol");
					Assert.That (s.NegotiatedProtocolVersion, Is.EqualTo (SslProtocol.Tls_1_2).Or.EqualTo (SslProtocol.Tls_1_3), "NegotiatedProtocolVersion");
					Assert.NotNull (s.PeerPublicKey, "PeerPublicKey");

					Assert.True (SecProtocolMetadata.ChallengeParametersAreEqual (s, s), "ChallengeParametersAreEqual");
					Assert.True (SecProtocolMetadata.PeersAreEqual (s, s), "PeersAreEqual");

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
						Assert.That (s.ServerName, Is.EqualTo ("www.microsoft.com"), "ServerName");
						// we don't have a TLS-PSK enabled server to test this
						Assert.False (s.AccessPreSharedKeys ((psk, pskId) => { }), "AccessPreSharedKeys");
					}
				}

				connection.Cancel ();
			}
		}
	}
}
#endif
