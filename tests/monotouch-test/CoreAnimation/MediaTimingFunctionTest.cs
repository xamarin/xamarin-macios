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
using CoreGraphics;
using Foundation;
using CoreAnimation;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreMotion {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MediaTimingFunctionTest {

		[Test]
		public void GetControlPoint ()
		{
			using (CAMediaTimingFunction mtf = CAMediaTimingFunction.FromControlPoints (0.1f, 0.2f, 0.3f, 0.4f)) {
				Assert.Throws<ArgumentOutOfRangeException> (delegate { mtf.GetControlPoint (-1); });
				Assert.That (mtf.GetControlPoint (0), Is.EqualTo (new CGPoint (0.0f, 0.0f)), "0");
				Assert.That (mtf.GetControlPoint (1), Is.EqualTo (new CGPoint (0.1f, 0.2f)), "1");
				Assert.That (mtf.GetControlPoint (2), Is.EqualTo (new CGPoint (0.3f, 0.4f)), "2");
				Assert.That (mtf.GetControlPoint (3), Is.EqualTo (new CGPoint (1.0f, 1.0f)), "3");
				Assert.Throws<ArgumentOutOfRangeException> (delegate { mtf.GetControlPoint (4); });
			}
		}

		[Test]
		public void Default ()
		{
			using (CAMediaTimingFunction mtf1 = new CAMediaTimingFunction (0.25f, 0.1f, 0.25f, 1f))
			using (CAMediaTimingFunction mtf3 = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Default)) {
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
			using (CAMediaTimingFunction mtf3 = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseIn)) {
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
			using (CAMediaTimingFunction mtf3 = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut)) {
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
			using (CAMediaTimingFunction mtf3 = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut)) {
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
			using (CAMediaTimingFunction mtf3 = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Linear)) {
				Assert.That (mtf3.GetControlPoint (0), Is.EqualTo (mtf1.GetControlPoint (0)), "0b");
				Assert.That (mtf3.GetControlPoint (1), Is.EqualTo (mtf1.GetControlPoint (1)), "1b");
				Assert.That (mtf3.GetControlPoint (2), Is.EqualTo (mtf1.GetControlPoint (2)), "2b");
				Assert.That (mtf3.GetControlPoint (3), Is.EqualTo (mtf1.GetControlPoint (3)), "3b");
			}
		}
	}
}

#endif // !__WATCHOS__
