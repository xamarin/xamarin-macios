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
			using (var options = NWProtocolOptions.CreateTls ()) {
				var sec = options.TlsProtocolOptions;
				// we cannot test much more :(
				Assert.AreNotEqual (IntPtr.Zero, options.Handle);
			}
		}

		[Test]
		public void CreateTcpTest ()
		{
			using (var options = NWProtocolOptions.CreateTcp ()) {
				// we cannot test much more :(
				Assert.AreNotEqual (IntPtr.Zero, options.Handle);
			}
		}

		[Test]
		public void CreateUdpTest ()
		{
			using (var options = NWProtocolOptions.CreateUdp ()) {
				// we cannot test much more :(
				Assert.AreNotEqual (IntPtr.Zero, options.Handle);
			}
		}

		[Test]
		public void SetIPLocalAddressPreference ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);

			foreach (var ipOption in new [] { NWIPLocalAddressPreference.Default, NWIPLocalAddressPreference.Stable, NWIPLocalAddressPreference.Temporary}) {
				using (var options = NWProtocolOptions.CreateTls ())
					Assert.DoesNotThrow (() => options.IPLocalAddressPreference = ipOption, "Tls");
				using (var options = NWProtocolOptions.CreateTcp ())
					Assert.DoesNotThrow (() => options.IPLocalAddressPreference = ipOption, "Tcp");
				using (var options = NWProtocolOptions.CreateUdp ())
					Assert.DoesNotThrow (() => options.IPLocalAddressPreference = ipOption, "Udp");
			}
		}
	}
}
#endif
