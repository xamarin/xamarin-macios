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
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using AVFoundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	[TestFixture]
	public class UtilitiesTest {

		[Test]
		public void AspectRatio ()
		{
			var r = CGRect.Empty.WithAspectRatio (CGSize.Empty);
			Assert.True (nfloat.IsNaN (r.Top), "Top");
			Assert.That (nfloat.IsNaN (r.Left), "Left");
			Assert.That (nfloat.IsNaN (r.Width), "Width");
			Assert.That (nfloat.IsNaN (r.Height), "Height");
		}
	}

#if __MACOS__ || !NET
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
#endif // __MACOS__ || !NET
}

#endif // !__WATCHOS__
