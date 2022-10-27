using System;

using CoreFoundation;
using Foundation;
using Network;
using ObjCRuntime;
using Security;

using NUnit.Framework;

namespace MonoTouchFixtures.Security {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ProtocolOptionsTest {

		[Test]
		public void Defaults ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			Assert.That (SecProtocolOptions.DefaultMaxDtlsProtocolVersion, Is.EqualTo (TlsProtocolVersion.Dtls12), "MaxDtls");
			Assert.That (SecProtocolOptions.DefaultMinDtlsProtocolVersion, Is.EqualTo (TlsProtocolVersion.Dtls10), "MinDtls");
			Assert.That (SecProtocolOptions.DefaultMaxTlsProtocolVersion, Is.EqualTo (TlsProtocolVersion.Tls13), "MaxTls");
			Assert.That (SecProtocolOptions.DefaultMinTlsProtocolVersion, Is.EqualTo (TlsProtocolVersion.Tls10), "MinTls");
		}

		[Test]
		public void Equals ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
#if NET
			using (var npo = new NWProtocolTlsOptions ()) {
				var options = npo.ProtocolOptions;
#else
			using (var npo = NWProtocolOptions.CreateTls ()) {
				var options = npo.TlsProtocolOptions;
#endif

				Assert.True (SecProtocolOptions.Equals (null, null), "1");
				Assert.True (SecProtocolOptions.Equals (options, options), "2");
				Assert.False (SecProtocolOptions.Equals (null, options), "3");
				Assert.False (SecProtocolOptions.Equals (options, null), "4");

				Assert.True (options.Equals (options), "5");
				Assert.False (options.Equals (null), "6");
			}
		}

		[Test]
		public void NewTlsOptions ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
#if NET
			using (var npo = new NWProtocolTlsOptions ()) {
				var options = npo.ProtocolOptions;
#else
			using (var npo = NWProtocolOptions.CreateTls ()) {
				var options = npo.TlsProtocolOptions;
#endif
				options.SetTlsMaxVersion (TlsProtocolVersion.Tls12);
				options.SetTlsMinVersion (TlsProtocolVersion.Tls10);
				options.AddTlsCipherSuite (TlsCipherSuite.Aes128GcmSha256);
				options.AddTlsCipherSuiteGroup (TlsCipherSuiteGroup.Legacy);
				using (var dd = DispatchData.FromByteBuffer (new byte [1])) {
					options.SetTlsPreSharedKeyIdentityHint (dd);
				}
			}
		}
	}
}
