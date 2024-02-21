#if __IOS__ || __MACOS__
using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLRasterizationRateLayerDescriptorTest {

		MTLRasterizationRateLayerDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			descriptor = new MTLRasterizationRateLayerDescriptor (new MTLSize (10, 01, 0));
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor is not null)
				descriptor.Dispose ();
			descriptor = null;
		}

		[Test]
		public void Horizontal ()
			=> Assert.DoesNotThrow (() => {
				var h = descriptor.Horizontal;
			});

		[Test]
		public void Vertical ()
			=> Assert.DoesNotThrow (() => {
				var v = descriptor.Vertical;
			});

		[Test]
		public void MaxSampleCount ()
			=> Assert.DoesNotThrow (() => {
				var m = descriptor.MaxSampleCount;
			});

		[Test]
		public void SampleCount ()
			=> Assert.DoesNotThrow (() => {
				var c = descriptor.SampleCount;
			});
	}
}
#endif
