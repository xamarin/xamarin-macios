//
// Unit tests for MPSImageNormalizedHistogram
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
//
// Copyright 2019 Microsoft Corporation.
//

#if !__WATCHOS__

using System;

using Foundation;
using Metal;
using MetalPerformanceShaders;
using ObjCRuntime;

using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.MetalPerformanceShaders {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MPSImageNormalizedHistogramTests {
		IMTLDevice device;

		[OneTimeSetUp]
		public void Metal ()
		{
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (10, 0);
			TestRuntime.AssertNotVirtualMachine ();

			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device is null || !MPSKernel.Supports (device))
				Assert.Inconclusive ("Metal is not supported.");
		}

		[Test]
		public void Constructors ()
		{
			var info = new MPSImageHistogramInfo { NumberOfHistogramEntries = 256 };
#if !__MACOS__
			var obj = new MPSImageNormalizedHistogram (MTLDevice.SystemDefault, ref info);
#else
			MPSImageNormalizedHistogram obj = null;
			try {
				obj = new MPSImageNormalizedHistogram (MTLDevice.SystemDefault, ref info);
			} catch (Exception ex) {
				// This test fails on 10.13 bots but not on a local computer with 10.13. Must work on 10.14+.
				// there is no a good way to tell if MPSImageNormalizedHistogram will work or not...
				if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 14))
					Assert.Fail (ex.Message);
				Assert.Inconclusive ("In 10.13 this can fail in some hardware.");
			}
#endif
			Assert.NotNull (obj, "MPSImageNormalizedHistogram obj");
			var rv = obj.HistogramInfo;
			Asserts.AreEqual (info, rv, "HistogramInfo");

			Assert.IsTrue (obj.ZeroHistogram, "ZeroHistogram");
			Assert.AreEqual ((nuint) 3072, obj.GetHistogramSize (MTLPixelFormat.RGBA16Sint), "HistogramSizeForSourceFormat");

			var crs = obj.ClipRectSource;
			Assert.AreEqual ((nint) 0, crs.Origin.X, "ClipRectSource.Origin.X");
			Assert.AreEqual ((nint) 0, crs.Origin.Y, "ClipRectSource.Origin.Y");
			Assert.AreEqual ((nint) 0, crs.Origin.Z, "ClipRectSource.Origin.Z");
			Assert.AreEqual (nuint.MaxValue, (nuint) crs.Size.Depth, "ClipRectSource.Size.Depth");
			Assert.AreEqual (nuint.MaxValue, (nuint) crs.Size.Height, "ClipRectSource.Size.Height");
			Assert.AreEqual (nuint.MaxValue, (nuint) crs.Size.Width, "ClipRectSource.Size.Width");
		}
	}
}

#endif // !__WATCHOS__
