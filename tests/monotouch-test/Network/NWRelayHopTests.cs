#if !__WATCHOS__
using Foundation;
using Network;

using NUnit.Framework;

#nullable enable

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWRelayHopTests {
		NWRelayHop? hop;

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (15, 0);

		[SetUp]
		public void SetUp ()
		{
			using var endpoint = NWEndpoint.Create ("https://github.com");
			hop = NWRelayHop.Create (endpoint, null, null);
		}

		[TearDown]
		public void TearDown ()
		{
			hop?.Dispose ();
		}


		[Test]
		public void AddAdditionalHttpHeaderFieldTest ()
		{
			Assert.DoesNotThrow (
				() => hop.AddAdditionalHttpHeaderField (
					"Keep-Alive", "timeout=5, max=1000"));
		}
	}
}
#endif
