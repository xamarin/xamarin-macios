#if !__WATCHOS__

using System;

using Foundation;
using ObjCRuntime;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class HeapDescriptorTest {
		MTLHeapDescriptor hd = null;

		[SetUp]
		public void SetUp ()
		{
#if !MONOMAC && !__MACCATALYST__
			TestRuntime.AssertXcodeVersion (8, 0);

			if (Runtime.Arch == Arch.SIMULATOR)
				Assert.Ignore ("Type is missing on the simulator");
#else
			TestRuntime.AssertXcodeVersion (9, 0);
#endif
			hd = new MTLHeapDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (hd is not null)
				hd.Dispose ();
			hd = null;
		}

		[Test]
		public void Properties ()
		{
			Assert.That (hd.CpuCacheMode, Is.EqualTo (MTLCpuCacheMode.DefaultCache), "CpuCacheMode");
			hd.CpuCacheMode = MTLCpuCacheMode.WriteCombined;
			Assert.That (hd.StorageMode, Is.EqualTo (MTLStorageMode.Private), "StorageMode");
			hd.StorageMode = MTLStorageMode.Memoryless;
			Assert.That (hd.Size, Is.EqualTo ((nuint) 0), "Size");
			hd.Size = 16;

			using (var hd2 = (MTLHeapDescriptor) hd.Copy ()) {
				Assert.That (hd2.CpuCacheMode, Is.EqualTo (MTLCpuCacheMode.WriteCombined), "CpuCacheMode");
				Assert.That (hd2.StorageMode, Is.EqualTo (MTLStorageMode.Memoryless), "StorageMode");
				Assert.That (hd2.Size, Is.EqualTo ((nuint) 16), "Size");

				// NSCopying
				Assert.That (hd2.Handle, Is.Not.EqualTo (hd.Handle), "Handle");
			}
		}

		[Test]
		public void GetSetCpuCacheModeTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			hd.CpuCacheMode = MTLCpuCacheMode.WriteCombined;
			Assert.AreEqual (MTLCpuCacheMode.WriteCombined, hd.CpuCacheMode);
		}

		[Test]
		public void GetSetSizeTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			hd.Size = 2;
			Assert.AreEqual ((nuint) 2, hd.Size);
		}

		[Test]
		public void GetSetStorageModeTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			hd.StorageMode = MTLStorageMode.Private;
			Assert.AreEqual (MTLStorageMode.Private, hd.StorageMode);
		}

		[Test]
		public void SizeTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			Assert.DoesNotThrow (() => {
				var s = hd.Size;
			});
		}

		[Test]
		public void StorageModeTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			Assert.DoesNotThrow (() => {
				var m = hd.StorageMode;
			});
		}

		[Test]
		public void CpuCacheModeTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			Assert.DoesNotThrow (() => {
				var m = hd.CpuCacheMode;
			});
		}


		[Test]
		public void HazardTrackingModeTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			Assert.DoesNotThrow (() => {
				var mode = hd.HazardTrackingMode;
			});
		}

		[Test]
		public void ResourceOptionsTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			Assert.DoesNotThrow (() => {
				var optiosn = hd.ResourceOptions;
			});
		}

		[Test]
		public void TypeTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			Assert.DoesNotThrow (() => {
				var t = hd.Type;
			});
		}
	}
}

#endif // !__WATCHOS__
