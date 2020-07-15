#if !__WATCHOS__

using System;

using ObjCRuntime;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	public class HeapDescriptorTest {
		MTLHeapDescriptor hd = null;

		[SetUp]
		public void SetUp ()
		{
#if !MONOMAC
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
			if (hd != null)
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
	}
}

#endif // !__WATCHOS__
