using System;
using System.Threading;

using Foundation;
using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWMulticastGroupTest {

		NWEndpoint endpoint;
		NWMulticastGroup descriptor;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			endpoint = NWEndpoint.Create ("224.0.0.251", "5353");
			descriptor = new NWMulticastGroup (endpoint);
		}

		[TearDown]
		public void TearDown ()
		{
			endpoint?.Dispose ();
			endpoint = null;
			descriptor?.Dispose ();
			descriptor = null;
		}

		[Test]
		public void DisabledUnicastTrafficTest ()
		{
			Assert.DoesNotThrow (() => {
				descriptor.DisabledUnicastTraffic = true;
			}, "Setter");
			Assert.DoesNotThrow (() => {
				Assert.IsTrue (descriptor.DisabledUnicastTraffic, "Value");
			}, "Getter");
		}

		[Test]
		public void AddEndpointTest ()
		{
			Assert.Throws<ArgumentNullException> (() => {
				descriptor.AddEndpoint (null);
			}, "Null argument.");

			// create new endpoint and later ensure that it is present in the enumeration
			var newEndpoint = NWEndpoint.Create ("224.0.0.252", "5454");
			descriptor.AddEndpoint (newEndpoint);

			var e = new AutoResetEvent (false);
			descriptor.EnumerateEndpoints ((endPoint) => {
				Assert.IsNotNull (endPoint);
				e.Set ();
				return true;
			});
			e.WaitOne (10000);
		}

		[Test]
		public void SetSpecificSourceTest ()
		{

			Assert.DoesNotThrow (() => {
				// create new endpoint and later ensure that it is present in the enumeration
				var newEndpoint = NWEndpoint.Create ("224.0.0.252", "5454");
				descriptor.SetSpecificSource (newEndpoint);
			});
		}
	}
}
