#if !__WATCHOS__
using System;
using Foundation;
using Network;
using NUnit.Framework;

#nullable enable

namespace MonoTouchFixtures.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWResolverConfigTest {

		NWEndpoint? endpoint;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			endpoint = NWEndpoint.Create ("https://microsoft.com");
		}

		[TearDown]
		public void TearDown ()
		{
			endpoint?.Dispose ();
		}

		[Test]
		public void HttpConstructorTest ()
		{
			using var resolver = new NWResolverConfig (endpoint, NWResolverConfigEndpointType.Https);
			Assert.NotNull (resolver, "Not null https");
			Assert.AreNotEqual (IntPtr.Zero, resolver.Handle, "Zero Handle htttps");
		}

		[Test]
		public void AddServerAddressTest ()
		{
			using var secondEndpoint = NWEndpoint.Create ("https://github.com");
			using var resolver = new NWResolverConfig (endpoint, NWResolverConfigEndpointType.Https);

			Assert.DoesNotThrow (() => resolver.AddServerAddress (secondEndpoint));
		}
	}
}
#endif
