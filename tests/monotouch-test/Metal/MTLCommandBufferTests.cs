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
	public class MTLCommandBufferTests {
		[Test]
		public void UseResidencySets ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			var device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device is null)
				Assert.Inconclusive ("Metal is not supported");

			var supportsResidencySets = device.SupportsFamily (MTLGpuFamily.Apple6);
			if (!supportsResidencySets)
				Assert.Inconclusive ("Residency sets are not supported on this device.");

			using var commandQ = device.CreateCommandQueue ();
			if (commandQ is null)  // this happens on a simulator
				Assert.Inconclusive ("Could not get the functions library for the device.");

			using var commandBuffer = commandQ.CommandBuffer ();
			if (commandBuffer is null) // happens on sim
				Assert.Inconclusive ("Could not get the command buffer for the device.");

			using var residencySetDescriptor = new MTLResidencySetDescriptor () {
				Label = "Label",
				InitialCapacity = 3
			};
			using var residencySet = device.CreateResidencySet (residencySetDescriptor, out var error);
			Assert.IsNull (error, "Error #1");
			Assert.IsNotNull (residencySet, "ResidencySet #1");

			commandBuffer.UseResidencySets (residencySet);
			commandBuffer.UseResidencySets (new IMTLResidencySet [] { residencySet });
		}
	}
}
