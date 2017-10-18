// Copyright 2017 Microsoft Inc. All rights reserved.

#if !__WATCHOS__

using System;

#if XAMCORE_2_0
using Metal;
using MetalPerformanceShaders;
#else
using MonoTouch.Metal;
using MonoTouch.MetalPerformanceShaders;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.MetalPerformanceShaders
{

	[TestFixture]
	public class MPSImageHistogramTest
	{

		[Test]
		public void Constructors ()
		{
#if !MONOMAC
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (7, 0);
#else
			TestRuntime.AssertXcodeVersion (9, 0);
#endif

			MPSImageHistogramInfo info = new MPSImageHistogramInfo ();
			info.NumberOfHistogramEntries = 256;
			using (var obj = new MPSImageHistogram (MTLDevice.SystemDefault, ref info)) {
				var rv = obj.HistogramInfo;
				Assert.That(info, Is.EqualTo(rv), "HistogramForAlpha");

				Assert.IsTrue (obj.ZeroHistogram, "ZeroHistogram");
				Assert.AreEqual (3072, obj.HistogramSizeForSourceFormat (MTLPixelFormat.RGBA16Sint), "HistogramSizeForSourceFormat");
				var crs = obj.ClipRectSource;
				Assert.AreEqual (0, crs.Origin.X, "ClipRectSource.Origin.X");
				Assert.AreEqual (0, crs.Origin.Y, "ClipRectSource.Origin.Y");
				Assert.AreEqual (0, crs.Origin.Z, "ClipRectSource.Origin.Z");
				Assert.AreEqual (-1, crs.Size.Depth, "ClipRectSource.Size.Depth");
				Assert.AreEqual (-1, crs.Size.Height, "ClipRectSource.Size.Height");
				Assert.AreEqual (-1, crs.Size.Width, "ClipRectSource.Size.Width");
			}
		}
	}
}

#endif // !__WATCHOS__
