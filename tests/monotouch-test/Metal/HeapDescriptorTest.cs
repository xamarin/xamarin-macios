#if !__WATCHOS__

using System;

#if XAMCORE_2_0
using ObjCRuntime;
using Metal;
#else
using MonoTouch.ObjCRuntime;
using MonoTouch.Metal;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	public class HeapDescriptorTest {

		[Test]
		public void Properties ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);

			if (Runtime.Arch == Arch.SIMULATOR)
				Assert.Ignore ("Type is missing on the simulator");

			using (var hd = new MTLHeapDescriptor ()) {
				Assert.That (hd.CpuCacheMode, Is.EqualTo (MTLCpuCacheMode.DefaultCache), "CpuCacheMode");
				hd.CpuCacheMode = MTLCpuCacheMode.WriteCombined;
				Assert.That (hd.StorageMode, Is.EqualTo (MTLStorageMode.Private), "StorageMode");
				hd.StorageMode = MTLStorageMode.Memoryless;
				Assert.That (hd.Size, Is.EqualTo (0), "Size");
				hd.Size = 16;

				using (var hd2 = (MTLHeapDescriptor) hd.Copy ()) {
					Assert.That (hd2.CpuCacheMode, Is.EqualTo (MTLCpuCacheMode.WriteCombined), "CpuCacheMode");
					Assert.That (hd2.StorageMode, Is.EqualTo (MTLStorageMode.Memoryless), "StorageMode");
					Assert.That (hd2.Size, Is.EqualTo (16), "Size");

					// NSCopying
					Assert.That (hd2.Handle, Is.Not.EqualTo (hd.Handle), "Handle");
				}
			}
		}
	}
}

#endif // !__WATCHOS__
