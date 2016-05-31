//
// Unit tests for CAMediaTimingFunction
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using CoreAnimation;
using ObjCRuntime;
#else
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
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

namespace MonoTouchFixtures.CoreMotion {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MediaTimingFunctionTest {

#if !XAMCORE_2_0 // the default ctor has been removed.
		[Test]
		public void DefaultCtor ()
		{
			// invalid default .ctor exposed, now marked as [Obsolete]
			using (CAMediaTimingFunction mtf = new CAMediaTimingFunction ()) {
				// invalid instance, we only verify it does not crash when called
				Assert.True (mtf.GetControlPoint (0).IsEmpty, "0");
				Assert.True (mtf.GetControlPoint (1).IsEmpty, "1");
				Assert.True (mtf.GetControlPoint (2).IsEmpty, "2");
				Assert.True (mtf.GetControlPoint (3).IsEmpty, "3");
				Assert.That (mtf.ToString (), Is.EqualTo ("MonoTouch.CoreAnimation.CAMediaTimingFunction"), "ToString");
			}
		}
#endif

		[Test]
		public void GetControlPoint ()
		{
			using (CAMediaTimingFunction mtf = CAMediaTimingFunction.FromControlPoints (0.1f, 0.2f, 0.3f, 0.4f)) {
				Assert.Throws<ArgumentOutOfRangeException> (delegate { mtf.GetControlPoint (-1); });
				Assert.That (mtf.GetControlPoint (0), Is.EqualTo (new PointF (0.0f, 0.0f)), "0");
				Assert.That (mtf.GetControlPoint (1), Is.EqualTo (new PointF (0.1f, 0.2f)), "1");
				Assert.That (mtf.GetControlPoint (2), Is.EqualTo (new PointF (0.3f, 0.4f)), "2");
				Assert.That (mtf.GetControlPoint (3), Is.EqualTo (new PointF (1.0f, 1.0f)), "3");
				Assert.Throws<ArgumentOutOfRangeException> (delegate { mtf.GetControlPoint (4); });
			}
		}

		[Test]
		public void Default ()
		{
			using (CAMediaTimingFunction mtf1 = new CAMediaTimingFunction (0.25f, 0.1f, 0.25f, 1f))
#if !XAMCORE_2_0
			using (CAMediaTimingFunction mtf2 = CAMediaTimingFunction.FromName ((string) CAMediaTimingFunction.Default))
#endif
			using (CAMediaTimingFunction mtf3 = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Default)) {
#if !XAMCORE_2_0
				Assert.That (mtf2.GetControlPoint (0), Is.EqualTo (mtf1.GetControlPoint (0)), "0a");
				Assert.That (mtf2.GetControlPoint (1), Is.EqualTo (mtf1.GetControlPoint (1)), "1a");
				Assert.That (mtf2.GetControlPoint (2), Is.EqualTo (mtf1.GetControlPoint (2)), "2a");
				Assert.That (mtf2.GetControlPoint (3), Is.EqualTo (mtf1.GetControlPoint (3)), "3a");
#endif
				Assert.That (mtf3.GetControlPoint (0), Is.EqualTo (mtf1.GetControlPoint (0)), "0b");
				Assert.That (mtf3.GetControlPoint (1), Is.EqualTo (mtf1.GetControlPoint (1)), "1b");
				Assert.That (mtf3.GetControlPoint (2), Is.EqualTo (mtf1.GetControlPoint (2)), "2b");
				Assert.That (mtf3.GetControlPoint (3), Is.EqualTo (mtf1.GetControlPoint (3)), "3b");
			}
		}

		[Test]
		public void EaseIn ()
		{
			using (CAMediaTimingFunction mtf1 = new CAMediaTimingFunction (0.42f, 0f, 1f, 1f))
#if !XAMCORE_2_0
			using (CAMediaTimingFunction mtf2 = CAMediaTimingFunction.FromName ((string) CAMediaTimingFunction.EaseIn))
#endif
			using (CAMediaTimingFunction mtf3 = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseIn)) {
#if !XAMCORE_2_0
				Assert.That (mtf2.GetControlPoint (0), Is.EqualTo (mtf1.GetControlPoint (0)), "0a");
				Assert.That (mtf2.GetControlPoint (1), Is.EqualTo (mtf1.GetControlPoint (1)), "1a");
				Assert.That (mtf2.GetControlPoint (2), Is.EqualTo (mtf1.GetControlPoint (2)), "2a");
				Assert.That (mtf2.GetControlPoint (3), Is.EqualTo (mtf1.GetControlPoint (3)), "3a");
#endif
				Assert.That (mtf3.GetControlPoint (0), Is.EqualTo (mtf1.GetControlPoint (0)), "0b");
				Assert.That (mtf3.GetControlPoint (1), Is.EqualTo (mtf1.GetControlPoint (1)), "1b");
				Assert.That (mtf3.GetControlPoint (2), Is.EqualTo (mtf1.GetControlPoint (2)), "2b");
				Assert.That (mtf3.GetControlPoint (3), Is.EqualTo (mtf1.GetControlPoint (3)), "3b");
			}
		}

		[Test]
		public void EaseOut ()
		{
			using (CAMediaTimingFunction mtf1 = new CAMediaTimingFunction (0f, 0f, 0.58f, 1f))
#if !XAMCORE_2_0
			using (CAMediaTimingFunction mtf2 = CAMediaTimingFunction.FromName ((string) CAMediaTimingFunction.EaseOut))
#endif
			using (CAMediaTimingFunction mtf3 = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut)) {
#if !XAMCORE_2_0
				Assert.That (mtf2.GetControlPoint (0), Is.EqualTo (mtf1.GetControlPoint (0)), "0a");
				Assert.That (mtf2.GetControlPoint (1), Is.EqualTo (mtf1.GetControlPoint (1)), "1a");
				Assert.That (mtf2.GetControlPoint (2), Is.EqualTo (mtf1.GetControlPoint (2)), "2a");
				Assert.That (mtf2.GetControlPoint (3), Is.EqualTo (mtf1.GetControlPoint (3)), "3a");
#endif
				Assert.That (mtf3.GetControlPoint (0), Is.EqualTo (mtf1.GetControlPoint (0)), "0b");
				Assert.That (mtf3.GetControlPoint (1), Is.EqualTo (mtf1.GetControlPoint (1)), "1b");
				Assert.That (mtf3.GetControlPoint (2), Is.EqualTo (mtf1.GetControlPoint (2)), "2b");
				Assert.That (mtf3.GetControlPoint (3), Is.EqualTo (mtf1.GetControlPoint (3)), "3b");
			}
		}

		[Test]
		public void EaseInEaseOut ()
		{
			using (CAMediaTimingFunction mtf1 = new CAMediaTimingFunction (0.42f, 0f, 0.58f, 1f))
#if !XAMCORE_2_0
			using (CAMediaTimingFunction mtf2 = CAMediaTimingFunction.FromName ((string) CAMediaTimingFunction.EaseInEaseOut))
#endif
			using (CAMediaTimingFunction mtf3 = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut)) {
#if !XAMCORE_2_0
				Assert.That (mtf2.GetControlPoint (0), Is.EqualTo (mtf1.GetControlPoint (0)), "0a");
				Assert.That (mtf2.GetControlPoint (1), Is.EqualTo (mtf1.GetControlPoint (1)), "1a");
				Assert.That (mtf2.GetControlPoint (2), Is.EqualTo (mtf1.GetControlPoint (2)), "2a");
				Assert.That (mtf2.GetControlPoint (3), Is.EqualTo (mtf1.GetControlPoint (3)), "3a");
#endif
				Assert.That (mtf3.GetControlPoint (0), Is.EqualTo (mtf1.GetControlPoint (0)), "0b");
				Assert.That (mtf3.GetControlPoint (1), Is.EqualTo (mtf1.GetControlPoint (1)), "1b");
				Assert.That (mtf3.GetControlPoint (2), Is.EqualTo (mtf1.GetControlPoint (2)), "2b");
				Assert.That (mtf3.GetControlPoint (3), Is.EqualTo (mtf1.GetControlPoint (3)), "3b");
			}
		}

		[Test]
		public void Linear ()
		{
			using (CAMediaTimingFunction mtf1 = new CAMediaTimingFunction (0f, 0f, 1f, 1f))
#if !XAMCORE_2_0
			using (CAMediaTimingFunction mtf2 = CAMediaTimingFunction.FromName ((string) CAMediaTimingFunction.Linear))
#endif
			using (CAMediaTimingFunction mtf3 = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Linear)) {
#if !XAMCORE_2_0
				Assert.That (mtf2.GetControlPoint (0), Is.EqualTo (mtf1.GetControlPoint (0)), "0a");
				Assert.That (mtf2.GetControlPoint (1), Is.EqualTo (mtf1.GetControlPoint (1)), "1a");
				Assert.That (mtf2.GetControlPoint (2), Is.EqualTo (mtf1.GetControlPoint (2)), "2a");
				Assert.That (mtf2.GetControlPoint (3), Is.EqualTo (mtf1.GetControlPoint (3)), "3a");
#endif
				Assert.That (mtf3.GetControlPoint (0), Is.EqualTo (mtf1.GetControlPoint (0)), "0b");
				Assert.That (mtf3.GetControlPoint (1), Is.EqualTo (mtf1.GetControlPoint (1)), "1b");
				Assert.That (mtf3.GetControlPoint (2), Is.EqualTo (mtf1.GetControlPoint (2)), "2b");
				Assert.That (mtf3.GetControlPoint (3), Is.EqualTo (mtf1.GetControlPoint (3)), "3b");
			}
		}
	}
}

#endif // !__WATCHOS__

