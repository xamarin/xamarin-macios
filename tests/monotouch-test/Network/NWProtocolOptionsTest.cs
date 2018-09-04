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
	public class NWProtocolOptionsTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
		}

		[Test]
		public void CreateTlsTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 12, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 14, throwIfOtherPlatform: false);

			using (var options = NWProtocolOptions.CreateTls ()) {
				var sec = options.TlsProtocolOptions;
				// we cannot test much more :(
				Assert.AreNotEqual (IntPtr.Zero, options.Handle);
			}
		}

		[Test]
		public void CreateTcpTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 12, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 14, throwIfOtherPlatform: false);

			using (var options = NWProtocolOptions.CreateTcp ()) {
				// we cannot test much more :(
				Assert.AreNotEqual (IntPtr.Zero, options.Handle);
			}
		}

		[Test]
		public void CreateUdpTest ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 12, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 14, throwIfOtherPlatform: false);

			using (var options = NWProtocolOptions.CreateUdp ()) {
				// we cannot test much more :(
				Assert.AreNotEqual (IntPtr.Zero, options.Handle);
			}
		}
	}
}
