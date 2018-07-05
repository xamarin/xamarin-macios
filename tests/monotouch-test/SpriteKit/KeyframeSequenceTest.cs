//
// Unit tests for SKKeyframeSequence
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using SpriteKit;
#else
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
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

namespace MonoTouchFixtures.SpriteKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class KeyframeSequenceTest {

		[Test]
		public void Ctor_Capacity ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var s = new SKKeyframeSequence (0)) {
				Assert.That (s.Count, Is.EqualTo ((nuint) 0), "Count-0");
				Assert.That (s.InterpolationMode, Is.EqualTo (SKInterpolationMode.Linear), "SKInterpolationMode-0");
				Assert.That (s.RepeatMode, Is.EqualTo (SKRepeatMode.Clamp), "RepeatMode-0");
			}

			using (var s = new SKKeyframeSequence (1)) {
				Assert.That (s.Count, Is.EqualTo ((nuint) 0), "Count-1");
			}
		}

		[Test]
		public void Ctor_Arrays_Null ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			using (var s = new SKKeyframeSequence (null, (NSNumber []) null)) {
				Assert.That (s.Count, Is.EqualTo ((nuint) 0), "Count-1");
				Assert.That (s.InterpolationMode, Is.EqualTo (SKInterpolationMode.Linear), "SKInterpolationMode-1");
				Assert.That (s.RepeatMode, Is.EqualTo (SKRepeatMode.Clamp), "RepeatMode-1");
			}

			using (var s = new SKKeyframeSequence (null, (float []) null)) {
				Assert.That (s.Count, Is.EqualTo ((nuint) 0), "Count-2");
				Assert.That (s.InterpolationMode, Is.EqualTo (SKInterpolationMode.Linear), "SKInterpolationMode-2");
				Assert.That (s.RepeatMode, Is.EqualTo (SKRepeatMode.Clamp), "RepeatMode-2");
			}

			using (var s = new SKKeyframeSequence (null, (double []) null)) {
				Assert.That (s.Count, Is.EqualTo ((nuint) 0), "Count-3");
				Assert.That (s.InterpolationMode, Is.EqualTo (SKInterpolationMode.Linear), "SKInterpolationMode-3");
				Assert.That (s.RepeatMode, Is.EqualTo (SKRepeatMode.Clamp), "RepeatMode-3");
			}
		}

		[Test]
		public void Ctor_Arrays ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			NSNumber[] keys = new NSNumber[] { 1.0f, 2.0f, 3.0f };
			NSNumber[] values = new NSNumber[] { 1.0f, 2.0f, 3.0f };
			using (var s = new SKKeyframeSequence (keys, values)) {
				Assert.That (s.Count, Is.EqualTo ((nuint) 3), "Count-1");
				Assert.That (s.InterpolationMode, Is.EqualTo (SKInterpolationMode.Linear), "SKInterpolationMode-1");
				Assert.That (s.RepeatMode, Is.EqualTo (SKRepeatMode.Clamp), "RepeatMode-1");
			}

			float[] values_f = new [] { 4.0f, 5.0f, 6.0f };
			using (var s = new SKKeyframeSequence (keys, values_f)) {
				Assert.That (s.Count, Is.EqualTo ((nuint) 3), "Count-2");
				Assert.That (s.InterpolationMode, Is.EqualTo (SKInterpolationMode.Linear), "SKInterpolationMode-2");
				Assert.That (s.RepeatMode, Is.EqualTo (SKRepeatMode.Clamp), "RepeatMode-2");
			}

			double[] values_d = new [] { 7.0d, 8.0d, 9.0d };
			using (var s = new SKKeyframeSequence (keys, values_d)) {
				Assert.That (s.Count, Is.EqualTo ((nuint) 3), "Count-3");
				Assert.That (s.InterpolationMode, Is.EqualTo (SKInterpolationMode.Linear), "SKInterpolationMode-3");
				Assert.That (s.RepeatMode, Is.EqualTo (SKRepeatMode.Clamp), "RepeatMode-3");
			}
		}
	}
}

#endif // !__WATCHOS__
