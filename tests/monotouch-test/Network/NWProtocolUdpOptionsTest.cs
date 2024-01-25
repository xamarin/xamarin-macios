#if !__WATCHOS__
using Foundation;

using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolUdpOptionsTest {
		NWProtocolUdpOptions options;

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			options = new NWProtocolUdpOptions ();
		}

		[TearDown]
		public void TearDown () => options.Dispose ();

		// properties do not have getters, but we know that if we call
		// the setter with the wrong pointer we do have a exception
		// thrown

		[Test]
		public void PreferNoChecksumTest () => Assert.DoesNotThrow (() => options.SetPreferNoChecksum (true));

	}
}
#endif
