#if !__WATCHOS__
using Foundation;

using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWEndpointTests {

		NWEndpoint endpoint;

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			endpoint = NWEndpoint.Create ("https://github.com");
		}

		[TearDown]
		public void TearDown ()
		{
			endpoint.Dispose ();
		}

		[Test]
		public void TypeTest () => Assert.AreEqual (NWEndpointType.Url, endpoint.Type);

		[Test]
		public void HostNameTest () => Assert.AreEqual ("github.com", endpoint.Hostname);

		[Test]
		public void PortTest () => Assert.AreEqual ("443", endpoint.Port);

		[Test]
		public void BonjourServiceNameTest () => Assert.Null (endpoint.BonjourServiceName);

		[Test]
		public void BonjourServiceTypeTest () => Assert.Null (endpoint.BonjourServiceType);

		[Test]
		public void BonjourServiceDomainTest () => Assert.Null (endpoint.BonjourServiceDomain);

		[Test]
		public void UrlTest () => Assert.AreEqual ("https://github.com", endpoint.Url);

		[Test]
		public void SignatureTest ()
		{
			TestRuntime.AssertXcodeVersion (14, 0);
			var signature = endpoint.Signature;
			Assert.AreEqual (0, signature.Length);
		}

		[Test]
		public void TxtRecordTest ()
		{
			TestRuntime.AssertXcodeVersion (14, 0);
			Assert.Null (endpoint.TxtRecord);
		}
	}
}
#endif
