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

using Metal;
using MetalPerformanceShaders;

using NUnit.Framework;

namespace MonoTouchFixtures.MetalPerformanceShaders {
	[TestFixture]
	public class MPSImageNormalizedHistogramTests {
		IMTLDevice device;

		[TestFixtureSetUp]
		public void Metal ()
		{
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (9, 0);

			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device == null || !MPSKernel.Supports (device))
				Assert.Inconclusive ("Metal is not supported.");
		}

		[Test]
		public void Constructors ()
		{
			var info = new MPSImageHistogramInfo { NumberOfHistogramEntries = 256 };
			using (var obj = new MPSImageNormalizedHistogram (MTLDevice.SystemDefault, ref info)) {
				var rv = obj.HistogramInfo;
				Asserts.AreEqual (info, rv, "HistogramInfo");

				Assert.IsTrue (obj.ZeroHistogram, "ZeroHistogram");
				Assert.AreEqual (3072, obj.GetHistogramSize (MTLPixelFormat.RGBA16Sint), "HistogramSizeForSourceFormat");

				var crs = obj.ClipRectSource;
				Assert.AreEqual (0, crs.Origin.X, "ClipRectSource.Origin.X");
				Assert.AreEqual (0, crs.Origin.Y, "ClipRectSource.Origin.Y");
				Assert.AreEqual (0, crs.Origin.Z, "ClipRectSource.Origin.Z");
				Assert.AreEqual (nuint.MaxValue, (nuint) crs.Size.Depth, "ClipRectSource.Size.Depth");
				Assert.AreEqual (nuint.MaxValue, (nuint) crs.Size.Height, "ClipRectSource.Size.Height");
				Assert.AreEqual (nuint.MaxValue, (nuint) crs.Size.Width, "ClipRectSource.Size.Width");
			}
		}
	}
}

#endif // !__WATCHOS__
