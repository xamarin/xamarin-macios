#if !__WATCHOS__
using System;
using System.Runtime.InteropServices;
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
		public void IPDefaults ()
		{
			using (var m = NWProtocolMetadata.CreateIPMetadata ()) {
				var s = m.SecProtocolMetadata;
				Assert.False (s.EarlyDataAccepted, "EarlyDataAccepted");
				Assert.That (s.NegotiatedCipherSuite, Is.EqualTo (SslCipherSuite.SSL_NULL_WITH_NULL_NULL), "NegotiatedCipherSuite");
				Assert.Null (s.NegotiatedProtocol, "NegotiatedProtocol");
				Assert.That (s.NegotiatedProtocolVersion, Is.EqualTo (SslProtocol.Unknown), "NegotiatedProtocolVersion");
				Assert.Null (s.PeerPublicKey, "PeerPublicKey");
#if false
				Assert.True (SecProtocolMetadata.ChallengeParametersAreEqual (s, s), "ChallengeParametersAreEqual");
				Assert.True (SecProtocolMetadata.PeersAreEqual (s, s), "PeersAreEqual");
#endif
			}
		}

#if false
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static nint CFGetRetainCount (IntPtr handle);

		[Test]
		public void CreateSecret ()
		{
			using (var npm = NWProtocolMetadata.CreateIPMetadata ()) {
				// `npm` and `spm` have the same handle - same internal object satistfy both protocols
				Console.WriteLine ($"{CFGetRetainCount (npm.Handle)}");
				using (var spm = npm.SecProtocolMetadata) {
					Console.WriteLine ($"{CFGetRetainCount (npm.Handle)}");
					Console.WriteLine ($"{CFGetRetainCount (spm.Handle)}");
					var secret = spm.CreateSecret ("test", 16); // crash
				}
			}
		}
#endif
	}
}
#endif