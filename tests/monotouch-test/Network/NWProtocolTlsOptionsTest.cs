#if !__WATCHOS__
using Foundation;
using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolTlsOptionsTest {
		NWProtocolTlsOptions options;

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			options = new NWProtocolTlsOptions ();
		}

		[TearDown]
		public void TearDown () => options.Dispose ();

		[Test]
		public void ProtocolOptionsTest ()
		{
#if NET
			Assert.NotNull (options.ProtocolOptions);
#else
			Assert.NotNull (options.TlsProtocolOptions);
#endif
		}
	}
}
#endif
