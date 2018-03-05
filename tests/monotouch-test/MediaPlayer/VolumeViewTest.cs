// Copyright 2011 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using MediaPlayer;
#else
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
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

namespace MonoTouchFixtures.MediaPlayer {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class VolumeViewTest {

		// TODO: Test temporarily ignored.
		// Reason: MPVolumeView started failing with iOS 11 beta 3.
		[Ignore]
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (MPVolumeView vv = new MPVolumeView (frame)) {
				Assert.That (vv.Frame, Is.EqualTo (frame), "Frame");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
