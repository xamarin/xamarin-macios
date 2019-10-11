#if !__WATCHOS__
using System;
using System.Threading;
#if XAMCORE_2_0
using Foundation;
using Network;
using ObjCRuntime;
using CoreFoundation;
#else
using MonoTouch.Foundation;
using MonoTouch.Network;
using MonoTouch.CoreFoundation;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolUdpOptionsTest {
		NWProtocolUdpOptions options;

		[TestFixtureSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			options = new NWProtocolUdpOptions ();
		}

		[TearDown]
		public void TearDown () => options.Dispose ();

		// proeprties do not have getters, but we know that if we call
		// the setter with the wrong pointer we do have a exception
		// thrown

		[Test]
		public void PreferNoChecksumTest () => Assert.DoesNotThrow (() => options.SetPreferNoChecksum (true));

	}
}
#endif