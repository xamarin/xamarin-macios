// Copyright 2015 Xamarin Inc. All rights reserved.

#if !__WATCHOS__

using System;
using Foundation;

#if XAMCORE_2_0
using Metal;
using MetalPerformanceShaders;
#else
using MonoTouch.Metal;
using MonoTouch.MetalPerformanceShaders;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.MetalPerformanceShaders {

	[TestFixture]
	public class KernelTest {

		IMTLDevice device;

		[TestFixtureSetUp]
		public void Metal ()
		{
#if !MONOMAC
			TestRuntime.AssertXcodeVersion (7, 0);
#else
			TestRuntime.AssertXcodeVersion (9, 0);
#endif

			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device == null || !MPSKernel.Supports (device))
				Assert.Inconclusive ("Metal is not supported");
		}

		[Test]
		public void RectNoClip ()
		{
#if !MONOMAC
			TestRuntime.AssertXcodeVersion (7,0);
#else
			TestRuntime.AssertXcodeVersion (9, 0);
#endif

			var d = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (d == null)
				Assert.Inconclusive ("Metal is not supported");
			
			var r = MPSKernel.RectNoClip;
			var o = r.Origin;
			Assert.That (o.X, Is.EqualTo (0), "X");
			Assert.That (o.Y, Is.EqualTo (0), "Y");
			Assert.That (o.Z, Is.EqualTo (0), "Z");
			var s = r.Size;
			Assert.That (s.Depth, Is.EqualTo (-1), "Depth");
			Assert.That (s.Height, Is.EqualTo (-1), "Height");
			Assert.That (s.Width, Is.EqualTo (-1), "Width");
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
			Assert.That (kernel2.RetainCount, Is.EqualTo (1));
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
			Assert.That (layer2.RetainCount, Is.EqualTo (1));
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
			Assert.That (layer2.RetainCount, Is.EqualTo (1));
		}

		[Test]
		public void GetPreferredDeviceTest ()
		{
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (10, 2);

			var preferredDevice = MPSKernel.GetPreferredDevice (MPSDeviceOptions.Default);
			Assert.NotNull (preferredDevice);
			Assert.IsTrue (MPSKernel.Supports (preferredDevice));
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
