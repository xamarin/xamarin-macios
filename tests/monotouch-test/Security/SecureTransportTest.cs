//
// SecureTransport Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc.
//

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
#if XAMCORE_2_0
using Foundation;
using Security;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.Foundation;
using MonoTouch.Security;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.Security {

	[TestFixture]
	// we want the test to be available if we use the linker
	[Preserve (AllMembers = true)]
	public class SecureTransportTest {

		const int errSecParam = -50;
		const int errSecAllocate = -108;

		[Test]
		public void StreamDefaults ()
		{
			using (var ssl = new SslContext (SslProtocolSide.Client, SslConnectionType.Stream)) {
				Assert.That (ssl.BufferedReadSize, Is.EqualTo ((nint) 0), "BufferedReadSize");
				Assert.That (ssl.ClientCertificateState, Is.EqualTo (SslClientCertificateState.None), "ClientCertificateState");
				Assert.Null (ssl.Connection, "Connection");
				Assert.That (ssl.DatagramWriteSize, Is.EqualTo ((nint) 0), "DatagramWriteSize");
				Assert.That (ssl.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (ssl.MaxDatagramRecordSize, Is.EqualTo ((nint) 0), "MaxDatagramRecordSize");
				Assert.That (ssl.MaxProtocol, Is.EqualTo (SslProtocol.Tls_1_2), "MaxProtocol");
				if (TestRuntime.CheckXcodeVersion (8, 0))
					Assert.That (ssl.MinProtocol, Is.EqualTo (SslProtocol.Tls_1_0), "MinProtocol");
				else
					Assert.That (ssl.MinProtocol, Is.EqualTo (SslProtocol.Ssl_3_0), "MinProtocol");
				Assert.That (ssl.NegotiatedCipher, Is.EqualTo (SslCipherSuite.SSL_NULL_WITH_NULL_NULL), "NegotiatedCipher");
				Assert.That (ssl.NegotiatedProtocol, Is.EqualTo (SslProtocol.Unknown), "NegotiatedProtocol");

				Assert.That (ssl.PeerDomainName, Is.Empty, "PeerDomainName");
				ssl.PeerDomainName = "google.ca";
				Assert.That (ssl.PeerDomainName, Is.EqualTo ("google.ca"), "PeerDomainName-2");
				ssl.PeerDomainName = null;
				Assert.That (ssl.PeerDomainName, Is.Empty, "PeerDomainName");

				Assert.Null (ssl.PeerId, "PeerId");
				ssl.PeerId = new byte [] { 0xff };
				Assert.That (ssl.PeerId.Length, Is.EqualTo (1), "1a");

				// note: SSLSetPeerID (see Apple open source code) does not accept a null/zero-length value
				ssl.PeerId = new byte [0];
				Assert.That ((int) ssl.GetLastStatus (), Is.EqualTo (errSecParam), "set_PeerId/empty");
				Assert.That (ssl.PeerId.Length, Is.EqualTo (1), "1b");

				ssl.PeerId = new byte [] { 0x01, 0x02 };
				Assert.That (ssl.PeerId.Length, Is.EqualTo (2), "2");

				Assert.Null (ssl.PeerTrust, "PeerTrust");
				Assert.That (ssl.SessionState, Is.EqualTo (SslSessionState.Idle), "SessionState");

				Assert.That ((int)ssl.SetDatagramHelloCookie (new byte [32]), Is.EqualTo (-50), "no cookie in stream");

				// Assert.Null (ssl.GetDistinguishedNames<string> (), "GetDistinguishedNames");

				if (TestRuntime.CheckXcodeVersion (9,0)) {
					Assert.That (ssl.SetSessionTickets (false), Is.EqualTo (0), "SetSessionTickets");
					Assert.That (ssl.SetError (SecStatusCode.Success), Is.EqualTo (0), "SetError");

					Assert.Throws<ArgumentNullException> (() => ssl.SetOcspResponse (null), "SetOcspResponse/null");
					using (var data = new NSData ())
						Assert.That (ssl.SetOcspResponse (data), Is.EqualTo (0), "SetOcspResponse/empty");

#if MONOMAC
					if (TestRuntime.CheckXcodeVersion (9,3)) {
#endif
					int error;
					var alpn = ssl.GetAlpnProtocols (out error);
					Assert.That (alpn, Is.Empty, "alpn");
					Assert.That (error, Is.EqualTo ((int) SecStatusCode.Param), "GetAlpnProtocols");
					var protocols = new [] { "HTTP/1.1", "SPDY/1" };
					Assert.That (ssl.SetAlpnProtocols (protocols), Is.EqualTo (0), "SetAlpnProtocols");
#if MONOMAC
					}
#endif
				}
			}
		}

		[Test]
		public void DatagramDefaults ()
		{
			nint dsize = TestRuntime.CheckXcodeVersion (6, 0) ? 1327 : 1387;
			using (var ssl = new SslContext (SslProtocolSide.Client, SslConnectionType.Datagram)) {
				Assert.That (ssl.BufferedReadSize, Is.EqualTo ((nint) 0), "BufferedReadSize");
				Assert.Null (ssl.Connection, "Connection");
				Assert.That (ssl.DatagramWriteSize, Is.EqualTo (dsize), "DatagramWriteSize");
				Assert.That (ssl.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (ssl.MaxDatagramRecordSize, Is.EqualTo ((nint) 1400), "MaxDatagramRecordSize");
				Assert.That (ssl.MaxProtocol, Is.EqualTo (SslProtocol.Dtls_1_0), "MaxProtocol");
				Assert.That (ssl.MinProtocol, Is.EqualTo (SslProtocol.Dtls_1_0), "MinProtocol");
				Assert.That (ssl.NegotiatedCipher, Is.EqualTo (SslCipherSuite.SSL_NULL_WITH_NULL_NULL), "NegotiatedCipher");
				Assert.That (ssl.NegotiatedProtocol, Is.EqualTo (SslProtocol.Unknown), "NegotiatedProtocol");
				Assert.Null (ssl.PeerId, "PeerId");
				Assert.That (ssl.SessionState, Is.EqualTo (SslSessionState.Idle), "SessionState");

				ssl.PeerId = new byte [] { 0xff };
				Assert.That (ssl.PeerId.Length, Is.EqualTo (1), "1");
				// note: SSLSetPeerID (see Apple open source code) does not accept a null/zero-length value
				ssl.PeerId = null;
				Assert.That ((int) ssl.GetLastStatus (), Is.EqualTo (errSecParam), "set_PeerId/null");

				Assert.That ((int)ssl.SetDatagramHelloCookie (new byte [33]), Is.EqualTo (-50), "cookie to long");
				Assert.That (ssl.SetDatagramHelloCookie (new byte [32]), Is.EqualTo (SslStatus.Success), "tasty cookie");;
				Assert.That (ssl.SetDatagramHelloCookie (new byte [1]), Is.EqualTo (SslStatus.Success), "fat free cookie");
				Assert.That (ssl.SetDatagramHelloCookie (null), Is.EqualTo (SslStatus.Success), "no more cookies");
			}
		}

		[Test]
		public void SslSupportedCiphers ()
		{
			int ssl_client_ciphers = -1;
			using (var client = new SslContext (SslProtocolSide.Client, SslConnectionType.Stream)) {
				// maximum downgrade
				client.MaxProtocol = client.MinProtocol;
				var ciphers = client.GetSupportedCiphers ();
				ssl_client_ciphers = ciphers.Count;
				Assert.That (ssl_client_ciphers, Is.AtLeast (1), "GetSupportedCiphers");
				// we can't really scan for SSL_* since (some of) the values are identical to TLS_
				// useful the other way around
			}
			int ssl_server_ciphers = -1;
			using (var server = new SslContext (SslProtocolSide.Server, SslConnectionType.Stream)) {
				// no downgrade, shows that the ciphers are not really restriced
				var ciphers = server.GetSupportedCiphers ();
				ssl_server_ciphers = ciphers.Count;
				Assert.That (ssl_server_ciphers, Is.AtLeast (1), "GetSupportedCiphers");
				// we can't really scan for SSL_* since (some of) the values are identical to TLS_
				// useful the other way around

				// make sure we have names for all ciphers - except old export ones (that we do not want to promote)
				// e.g. iOS 5.1 still supports them
				foreach (var cipher in ciphers) {
					string s = cipher.ToString ();
					if (s.Length < 8)
						Console.WriteLine (s);
					Assert.True (s.StartsWith ("SSL_", StringComparison.Ordinal) || s.StartsWith ("TLS_", StringComparison.Ordinal), s);
				}
			}
			Assert.That (ssl_client_ciphers, Is.EqualTo (ssl_server_ciphers), "same");
		}

#if !__WATCHOS__
		// This test uses sockets (TcpClient), which doesn't work on watchOS.
		[Test]
		public void Tls12 ()
		{
			var client = new TcpClient ("google.ca", 443);
			using (NetworkStream ns = client.GetStream ())
			using (var ssl = new SslContext (SslProtocolSide.Client, SslConnectionType.Stream)) {

				ssl.MinProtocol = SslProtocol.Tls_1_2;
				Assert.That (ssl.MinProtocol, Is.EqualTo (SslProtocol.Tls_1_2), "MinProtocol");

				ssl.Connection = new SslStreamConnection (ns);

				var result = ssl.Handshake ();
				while (result == SslStatus.WouldBlock || result == (SslStatus) (-108)) {
					// we need to ask again - but if we're too fast we'll get -108 (errSecAllocate)
					Thread.Sleep (100);
					// during the above call SessionState is Handshake
					Assert.That (ssl.SessionState, Is.EqualTo (SslSessionState.Handshake), "Handshake/in progress");
					result = ssl.Handshake ();
				}
				Assert.That (result, Is.EqualTo (SslStatus.Success), "Handshake/done");

				// FIXME: iOS 8 beta 1 bug ?!? the state is not updated (maybe delayed?) but the code still works
				//Assert.That (ssl.SessionState, Is.EqualTo (SslSessionState.Connected), "Connected");
				Assert.That (ssl.NegotiatedCipher, Is.Not.EqualTo (SslCipherSuite.SSL_NULL_WITH_NULL_NULL), "NegotiatedCipher");
				Assert.That (ssl.NegotiatedProtocol, Is.EqualTo (SslProtocol.Tls_1_2), "NegotiatedProtocol");

				nint processed;
				var data = Encoding.UTF8.GetBytes ("GET / HTTP/1.0" + Environment.NewLine + Environment.NewLine);
				result = ssl.Write (data, out processed);
				Assert.That (processed, Is.EqualTo ((nint) data.Length), "small buffer");
				Assert.That (result, Is.EqualTo (SslStatus.Success), "Write");

				data = new byte [1024];
				result = ssl.Read (data, out processed);
				while (result == SslStatus.WouldBlock)
					result = ssl.Read (data, out processed);
				Assert.That (result, Is.EqualTo (SslStatus.Success), "Read");

				string s = Encoding.UTF8.GetString (data, 0, (int) processed);
				// The result apparently depends on where you are: I get a 302, the bots get a 200.
				Assert.That (s, Is.StringStarting ("HTTP/1.0 302 Found").Or.StringStarting ("HTTP/1.0 200 OK"), "response");
			}
		}
#endif // !__WATCHOS__
	}
}