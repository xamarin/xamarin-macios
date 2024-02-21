// Copyright 2015 Xamarin Inc. All rights reserved.

#if !__WATCHOS__

using System;
using Foundation;
using ObjCRuntime;

using Metal;
using MetalPerformanceShaders;

using NUnit.Framework;

namespace MonoTouchFixtures.MetalPerformanceShaders {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class KernelTest {

		IMTLDevice device;

		[OneTimeSetUp]
		public void Metal ()
		{
#if !MONOMAC && !__MACCATALYST__
			TestRuntime.AssertXcodeVersion (7, 0);

			if (Runtime.Arch == Arch.SIMULATOR)
				Assert.Inconclusive ("Metal Performance Shaders is not supported in the simulator");
#else
			TestRuntime.AssertXcodeVersion (9, 0);
#endif

			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device is null || !MPSKernel.Supports (device))
				Assert.Inconclusive ("Metal is not supported");
		}

		[Test]
		public void RectNoClip ()
		{
#if !MONOMAC
			TestRuntime.AssertXcodeVersion (7, 0);
#else
			TestRuntime.AssertXcodeVersion (9, 0);
#endif

			var d = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (d is null)
				Assert.Inconclusive ("Metal is not supported");

			var r = MPSKernel.RectNoClip;
			var o = r.Origin;
			Assert.That (o.X, Is.EqualTo ((nint) 0), "X");
			Assert.That (o.Y, Is.EqualTo ((nint) 0), "Y");
			Assert.That (o.Z, Is.EqualTo ((nint) 0), "Z");
			var s = r.Size;
			Assert.That (s.Depth, Is.EqualTo ((nint) (-1)), "Depth");
			Assert.That (s.Height, Is.EqualTo ((nint) (-1)), "Height");
			Assert.That (s.Width, Is.EqualTo ((nint) (-1)), "Width");
		}

		[Test]
		public void MPSKernelCopyTest ()
		{
#if !MONOMAC
			TestRuntime.AssertDevice ();
#endif

			TestRuntime.AssertXcodeVersion (9, 0);

			var kernel = new MPSKernel (MTLDevice.SystemDefault);
			var kernel2 = kernel.CopyWithZone (NSZone.Default, MTLDevice.SystemDefault);
			Assert.That (kernel2.RetainCount, Is.EqualTo ((nuint) 1));
		}

		[Test]
		public void MPSRnnImageInferenceLayerCopyTest ()
		{
#if !MONOMAC
			TestRuntime.AssertDevice ();
#endif

			TestRuntime.AssertXcodeVersion (9, 0);

			var layer = new MPSRnnImageInferenceLayer (MTLDevice.SystemDefault, MPSRnnSingleGateDescriptor.Create (1, 1));
			var layer2 = layer.Copy (NSZone.Default, MTLDevice.SystemDefault);
			Assert.That (layer2.RetainCount, Is.EqualTo ((nuint) 1));
		}

		[Test]
		public void MPSRnnMatrixInferenceLayerTest ()
		{
#if !MONOMAC
			TestRuntime.AssertDevice ();
#endif

			TestRuntime.AssertXcodeVersion (9, 0);

			var layer = new MPSRnnMatrixInferenceLayer (MTLDevice.SystemDefault, MPSRnnSingleGateDescriptor.Create (1, 1));
			var layer2 = layer.Copy (NSZone.Default, MTLDevice.SystemDefault);
			Assert.That (layer2.RetainCount, Is.EqualTo ((nuint) 1));
		}

		[Test]
		public void MPSImageLaplacianPyramidCtorArrTest ()
		{
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (10, 2);

			var validArr = new float [] {
				0, 1, 0,
				1, -4, 1,
				0, 1, 0,
			};

			var invalidArr = new float [] {
				0, 1, 0,
				1, -4, 1,
			};

			var bigvalidArr = new float [] {
				0, 1, 0,
				1, -4, 1,
				0, 1, 0,
				0, 1, 0,
				1, -4, 1,
				0, 1, 0,
			};

			var valid = new MPSImageLaplacianPyramid (device, 3, 3, validArr);
			Assert.NotNull (valid, "Valid Arr");

			Assert.Throws<ArgumentException> (() => new MPSImageLaplacianPyramid (device, 3, 3, invalidArr), "Invalid Arr");

			var bigvalid = new MPSImageLaplacianPyramid (device, 3, 3, bigvalidArr);
			Assert.NotNull (valid, "Big valid Arr");
		}
	}
}

#endif // !__WATCHOS__
