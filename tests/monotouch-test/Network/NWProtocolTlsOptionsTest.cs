#if !__WATCHOS__
using System;
using System.Threading;
using Foundation;
using Network;
using ObjCRuntime;
using CoreFoundation;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWProtocolTlsOptionsTest {
		NWProtocolTlsOptions options;

		[TestFixtureSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			options = new NWProtocolTlsOptions ();
		}

		[TearDown]
		public void TearDown () => options.Dispose ();

		[Test]
		public void ProtocolOptionsTest () => Assert.NotNull (options.TlsProtocolOptions);
	}
}
#endif