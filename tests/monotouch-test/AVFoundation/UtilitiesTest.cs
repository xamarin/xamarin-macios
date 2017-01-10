//
// Unit tests for AVUtilities.h helpers
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Drawing;
using System.Runtime.InteropServices;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using AVFoundation;
#else
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	[TestFixture]
	public class UtilitiesTest {

		[Test]
		public void AspectRatio ()
		{
			var r = RectangleF.Empty.WithAspectRatio (SizeF.Empty);
			Assert.True (nfloat.IsNaN (r.Top), "Top");
			Assert.That (nfloat.IsNaN (r.Left), "Left");
			Assert.That (nfloat.IsNaN (r.Width), "Width");
			Assert.That (nfloat.IsNaN (r.Height), "Height");
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVStructTest {

		[Test]
		public void StructSizeTest ()
		{
			if (!TestRuntime.CheckXcodeVersion (6, 1))
				Assert.Ignore ("Ignoring Tests: Requires Xcode 6.1+ API");

			Assert.That (Marshal.SizeOf (typeof (AVSampleCursorSyncInfo)), Is.EqualTo (3), "AVSampleCursorSyncInfo Size");
			Assert.That (Marshal.SizeOf (typeof (AVSampleCursorDependencyInfo)), Is.EqualTo (6), "AVSampleCursorDependencyInfo Size");
			Assert.That (Marshal.SizeOf (typeof (AVSampleCursorStorageRange)), Is.EqualTo (16), "AVSampleCursorStorageRange Size");
			Assert.That (Marshal.SizeOf (typeof (AVSampleCursorChunkInfo)), Is.EqualTo (IntPtr.Size == 8 ? 16 : 12), "AVSampleCursorChunkInfo Size");
		}
	}
}
	
#endif // !__WATCHOS__
