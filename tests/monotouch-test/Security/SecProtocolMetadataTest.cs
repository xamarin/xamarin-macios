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
			TestRuntime.AssertXcodeVersion (10,0);
		}

		[Test]
		public void TlsDefaults ()
		{
			using (var ep = NWEndpoint.Create ("www.microsoft.com", "https"))
			using (var parameters = NWParameters.CreateSecureTcp ())
			using (var queue = new DispatchQueue (GetType ().FullName)) {
				var connection = new NWConnection (ep, parameters);

				var ready = new ManualResetEvent (false);
				connection.SetStateChangeHandler ((state, error) => {
					Console.WriteLine (state);
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
				Assert.True (ready.WaitOne (TimeSpan.FromSeconds (10)), "Connection is ready");

				using (var m = connection.GetProtocolMetadata (NWProtocolDefinition.TlsDefinition)) {
					var s = m.TlsSecProtocolMetadata;
					Assert.False (s.EarlyDataAccepted, "EarlyDataAccepted");
					Assert.That (s.NegotiatedCipherSuite, Is.Not.EqualTo (SslCipherSuite.SSL_NULL_WITH_NULL_NULL), "NegotiatedCipherSuite");
					Assert.Null (s.NegotiatedProtocol, "NegotiatedProtocol");
					Assert.That (s.NegotiatedProtocolVersion, Is.EqualTo (SslProtocol.Tls_1_2).Or.EqualTo (SslProtocol.Tls_1_3), "NegotiatedProtocolVersion");
					Assert.NotNull (s.PeerPublicKey, "PeerPublicKey");

					Assert.True (SecProtocolMetadata.ChallengeParametersAreEqual (s, s), "ChallengeParametersAreEqual");
					Assert.True (SecProtocolMetadata.PeersAreEqual (s, s), "PeersAreEqual");
				}

				connection.Cancel ();
			}
		}
	}
}
#endif
