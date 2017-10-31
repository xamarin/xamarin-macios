// Copyright 2017 Microsoft Inc. All rights reserved.

#if !__WATCHOS__

using System;

#if XAMCORE_2_0
using Metal;
using MetalPerformanceShaders;
using UIKit;
#else
using MonoTouch.Metal;
using MonoTouch.MetalPerformanceShaders;
using MonoTouch.UIKit;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.MetalPerformanceShaders
{

	[TestFixture]
	public class MPSImageHistogramEqualizationTest
	{

		[Test]
		public void Constructors ()
		{
			TestRuntime.AssertDevice ();
			TestRuntime.AssertXcodeVersion (7, 0);

			MPSImageHistogramInfo info = new MPSImageHistogramInfo ();
			info.NumberOfHistogramEntries = 256;
			using (var obj = new MPSImageHistogramEqualization (MTLDevice.SystemDefault, ref info)) {
				var rv = obj.HistogramInfo;
				Asserts.AreEqual (info, rv, "HistogramForAlpha");
			}
		}
	}
}

#endif // !__WATCHOS__
