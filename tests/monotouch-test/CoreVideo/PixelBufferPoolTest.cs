//
// Unit tests for CVPixelBufferPool
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Drawing;
using Foundation;
using ObjCRuntime;
using CoreVideo;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreVideo {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PixelBufferPoolTest {
		[Test]
		public void AllocationSettings_Threshold ()
		{
			var pbp = new CVPixelBufferPool (
				new CVPixelBufferPoolSettings (),
				new CVPixelBufferAttributes (CVPixelFormatType.CV24RGB, 100, 50)
			);

			var a = new CVPixelBufferPoolAllocationSettings () {
				Threshold = 2
			};

			CVReturn error;
			Assert.IsNotNull (pbp.CreatePixelBuffer (a, out error), "#1");
			Assert.IsNotNull (pbp.CreatePixelBuffer (a, out error), "#2");
			Assert.IsNull (pbp.CreatePixelBuffer (a, out error), "#3");
			Assert.AreEqual (CVReturn.WouldExceedAllocationThreshold, error, "#3a");
		}
	}
}

#endif // !__WATCHOS__
