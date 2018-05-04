//
// Unit tests for CTFrame
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
#if XAMCORE_2_0
using Foundation;
using CoreText;
using CoreGraphics;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.CoreText;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreText {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CTFrameTests {
		
		[Test] // #4677
		public void GetPathTest ()
		{
			using (var framesetter = new CTFramesetter (new NSAttributedString ("Testing, testing, 1, 2, 3..."))) {
				using (var frame = framesetter.GetFrame (new NSRange (0, 0), new CGPath (), null)) {
					using (var f = frame.GetPath ()) {
					}
					using (var f = frame.GetPath ()) {
						Console.WriteLine (f.BoundingBox);
					}
				}
			}
		}
	}
}
