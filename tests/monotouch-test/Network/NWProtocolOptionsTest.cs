#if !__WATCHOS__
using System;
#if XAMCORE_2_0
using Foundation;
using Network;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.Network;
#endif

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
	}
}
#endif