#if !__WATCHOS__
using System;
using System.IO;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {
	[Preserve (AllMembers = true)]
	public class MTLResidencySetTests {
		[Test]
		public void AddOrRemoveAllocations ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			var device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device is null)
				Assert.Inconclusive ("Metal is not supported");

			var supportsResidencySets = device.SupportsFamily (MTLGpuFamily.Apple6);
			if (!supportsResidencySets)
				Assert.Inconclusive ("Residency sets are not supported on this device.");

			using var heapDescriptor = new MTLHeapDescriptor () {
				Size = 1024,
			};
			using var heap = device.CreateHeap (heapDescriptor);
			using var residencySetDescriptor = new MTLResidencySetDescriptor () {
				Label = "Label",
				InitialCapacity = 3
			};
			using var residencySet = device.CreateResidencySet (residencySetDescriptor, out var error);
			Assert.IsNull (error, "Error #1");
			Assert.IsNotNull (residencySet, "ResidencySet #1");

			residencySet.AddAllocations (heap);
			Assert.AreEqual (1, (int) residencySet.AllocationCount, "AllocationCount #1");
			residencySet.RemoveAllocations (heap);
			Assert.AreEqual (0, (int) residencySet.AllocationCount, "AllocationCount #2");

			residencySet.AddAllocations (new IMTLAllocation [] { heap });
			Assert.AreEqual (1, (int) residencySet.AllocationCount, "AllocationCount #3");
			residencySet.RemoveAllocations (new IMTLAllocation [] { heap });
			Assert.AreEqual (0, (int) residencySet.AllocationCount, "AllocationCount #4");
		}
	}
}
#endif // !__WATCHOS__
