#if !__WATCHOS__
using System;
using Foundation;
using Network;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolOptionsTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
		}

		[Test]
		public void CreateTlsTest ()
		{
#if NET
			using (var options = new NWProtocolTlsOptions ()) {
				var sec = options.ProtocolOptions;
#else
			using (var options = NWProtocolOptions.CreateTls ()) {
				var sec = options.TlsProtocolOptions;
#endif
				// we cannot test much more :(
				Assert.AreNotEqual (IntPtr.Zero, options.Handle);
			}
		}

		[Test]
		public void CreateTcpTest ()
		{
#if NET
			using (var options = new NWProtocolTcpOptions ()) {
#else
			using (var options = NWProtocolOptions.CreateTcp ()) {
#endif
				// we cannot test much more :(
				Assert.AreNotEqual (IntPtr.Zero, options.Handle);
			}
		}

		[Test]
		public void CreateUdpTest ()
		{
#if NET
			using (var options = new NWProtocolUdpOptions ()) {
#else
			using (var options = NWProtocolOptions.CreateUdp ()) {
#endif
				// we cannot test much more :(
				Assert.AreNotEqual (IntPtr.Zero, options.Handle);
			}
		}

		[Test]
		public void SetIPLocalAddressPreference ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);

			foreach (var ipOption in new [] { NWIPLocalAddressPreference.Default, NWIPLocalAddressPreference.Stable, NWIPLocalAddressPreference.Temporary }) {
#if NET
				using (var options = new NWProtocolTlsOptions ())
#else
				using (var options = NWProtocolOptions.CreateTls ())
#endif
					Assert.DoesNotThrow (() => options.IPLocalAddressPreference = ipOption, "Tls");
#if NET
				using (var options = new NWProtocolTcpOptions ())
#else
				using (var options = NWProtocolOptions.CreateTcp ())
#endif
					Assert.DoesNotThrow (() => options.IPLocalAddressPreference = ipOption, "Tcp");
#if NET
				using (var options = new NWProtocolUdpOptions ())
#else
				using (var options = NWProtocolOptions.CreateUdp ())
#endif
					Assert.DoesNotThrow (() => options.IPLocalAddressPreference = ipOption, "Udp");
			}
		}
	}
}
#endif
