#if __IOS__ || __MACOS__
using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLRasterizationRateMapDescriptorTest {
		MTLSize size;
		MTLRasterizationRateMapDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			size = new MTLSize (10, 10, 10);
			TestRuntime.AssertXcodeVersion (13, 0);
			descriptor = MTLRasterizationRateMapDescriptor.Create (size);
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor is not null)
				descriptor.Dispose ();
			descriptor = null;
		}

		[Test]
		public void GetLayerTest ()
			=> Assert.Null (descriptor.GetLayer (1));

		[Test]
		public void SetLayerTest ()
			=> Assert.DoesNotThrow (() => descriptor.SetLayer (null, 1));

		[Test]
		public void LayersTest ()
			=> Assert.DoesNotThrow (() => {
				var array = descriptor.Layers;
			});

		[Test]
		public void ScreenSizeTest ()
			=> Assert.DoesNotThrow (() => {
				var size = descriptor.ScreenSize;
			});

		[Test]
		public void LabelTest ()
		{
			string label = "my label";
			Assert.DoesNotThrow (() => descriptor.Label = label, "Set label");
			Assert.AreEqual (label, descriptor.Label, "Get label");
		}

		[Test]
		public void LayerCount ()
			=> Assert.DoesNotThrow (() => {
				var count = descriptor.LayerCount;
			});
	}
}
#endif
