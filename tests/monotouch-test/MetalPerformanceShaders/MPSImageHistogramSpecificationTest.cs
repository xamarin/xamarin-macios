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
	public class MPSImageHistogramSpecificationTest
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
			using (var obj = new MPSImageHistogramSpecification (MTLDevice.SystemDefault, ref info)) {
				var rv = obj.HistogramInfo;
				Asserts.AreEqual (info, rv, "HistogramForAlpha");
			}
		}
	}
}

#endif // !__WATCHOS__
