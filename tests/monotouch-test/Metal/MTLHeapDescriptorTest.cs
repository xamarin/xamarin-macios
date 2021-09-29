using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLHeapDescriptorTest {
		MTLHeapDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			descriptor = new MTLHeapDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor != null)
				descriptor.Dispose ();
			descriptor = null;
		}

		[Test]
		public void SizeTest ()
			=> Assert.DoesNotThrow (() => {
				var s = descriptor.Size;
			});

		[Test]
		public void StorageModeTest ()
			=> Assert.DoesNotThrow (() => {
				var m = descriptor.StorageMode;
			});

		[Test]
		public void CpuCacheModeTest ()
			=> Assert.DoesNotThrow (() => {
				var m = descriptor.CpuCacheMode;
			});


		[Test]
		public void HazardTrackingModeTest ()
			=> Assert.DoesNotThrow (() => {
				var mode = descriptor.HazardTrackingMode;
			});

		[Test]
		public void ResourceOptionsTest ()
			=> Assert.DoesNotThrow (() => {
				var optiosn = descriptor.ResourceOptions;
			});

		[Test]
		public void TypeTest ()
			=> Assert.DoesNotThrow (() => {
				var t = descriptor.Type;
			});
	}
}
