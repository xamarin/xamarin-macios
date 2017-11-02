//
// UIAppearance Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;

#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AppearanceTest {

		[Test]
		public void Equality ()
		{
			using (var a = UITableView.Appearance)
			using (var b = UITableView.Appearance)
			using (var c = UILabel.Appearance) {
				Assert.True (a == b, "1");
				Assert.False (a == c, "2");
			}
		}

		[Test]
		public void Inequality ()
		{
			using (var a = UITableView.Appearance)
			using (var b = UITableView.Appearance)
			using (var c = UILabel.Appearance) {
				Assert.False (a != b, "1");
				Assert.True (a != c, "2");
			}
		}

		[Test]
		public void Appearance ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8,0))
				Assert.Inconclusive ("UITraitCollection requires iOS 8.0");

			using (var traits = new UITraitCollection ()) {
				nfloat r, g, b, a;

				// Appearance
				// it can be set
				Assert.IsNull (UILabel.Appearance.TextColor, "null 1");
				UILabel.Appearance.TextColor = UIColor.Red;
				UILabel.Appearance.TextColor.GetRGBA (out r, out g, out b, out a);
				Assert.AreEqual (1, a, "a1");
				Assert.AreEqual (1, r, "r1");
				Assert.AreEqual (0, g, "g1");
				Assert.AreEqual (0, b, "b1");

				// check that other appearance instances didn't change
				Assert.IsNull (UILabel.GetAppearance (traits).TextColor, "other null 2");
				Assert.IsNull (UILabel.GetAppearance (traits, typeof (UITextField)).TextColor, "other null 3");
				Assert.IsNull (UILabel.AppearanceWhenContainedIn (typeof (UITextField)).TextColor, "other null 4");

				// it can be cleared
				UILabel.Appearance.TextColor = null;
				Assert.IsNull (UILabel.Appearance.TextColor, "null 2");
			}
		}

		[Test]
		public void AppearanceWhenContainedIn ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8,0))
				Assert.Inconclusive ("UITraitCollection requires iOS 8.0");

			using (var traits = new UITraitCollection ()) {
				nfloat r, g, b, a;

				// it can be set
				Assert.IsNull (UILabel.AppearanceWhenContainedIn (typeof (UITextField)).TextColor, "null 1");
				UILabel.AppearanceWhenContainedIn (typeof (UITextField)).TextColor = UIColor.Blue;
				UILabel.AppearanceWhenContainedIn (typeof (UITextField)).TextColor.GetRGBA (out r, out g, out b, out a);
				Assert.AreEqual (1, a, "a1");
				Assert.AreEqual (0, r, "r1");
				Assert.AreEqual (0, g, "g1");
				Assert.AreEqual (1, b, "b1");

				// check that other appearance instances didn't change (bug 26353)
				Assert.IsNull (UILabel.Appearance.TextColor, "other null 1");
				Assert.IsNull (UILabel.GetAppearance (traits).TextColor, "other null 2");
				Assert.IsNull (UILabel.GetAppearance (traits, typeof (UITextField)).TextColor, "other null 3");

				// it can be cleared
				UILabel.AppearanceWhenContainedIn (typeof (UITextField)).TextColor = null;
				Assert.IsNull (UILabel.AppearanceWhenContainedIn (typeof (UITextField)).TextColor, "null 2");
			}
		}


		[Test]
		public void AppearanceWhenContainedIn_UITraitCollection ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8,0))
				Assert.Inconclusive ("UITraitCollection requires iOS 8.0");

			using (var traits = new UITraitCollection ()) {
				nfloat r, g, b, a;

				// it can be set
				Assert.IsNull (UILabel.GetAppearance (traits, typeof (UITextField)).TextColor, "null 1");
				UILabel.GetAppearance (traits, typeof (UITextField)).TextColor = UIColor.Blue;
				UILabel.GetAppearance (traits, typeof (UITextField)).TextColor.GetRGBA (out r, out g, out b, out a);
				Assert.AreEqual (1, a, "a1");
				Assert.AreEqual (0, r, "r1");
				Assert.AreEqual (0, g, "g1");
				Assert.AreEqual (1, b, "b1");

				// check that other appearance instances didn't change
				Assert.IsNull (UILabel.Appearance.TextColor, "other null 1");
				Assert.IsNull (UILabel.GetAppearance (traits).TextColor, "other null 2");
				Assert.IsNull (UILabel.AppearanceWhenContainedIn (typeof (UITextField)).TextColor, "other null 4");

				// it can be cleared
				UILabel.GetAppearance (traits, typeof (UITextField)).TextColor = null;
				Assert.IsNull (UILabel.GetAppearance (traits, typeof (UITextField)).TextColor, "null 2");
			}

			using (var traits = new UITraitCollection ()) {
				nfloat r, g, b, a;

				// it can be set
				Assert.IsNull (UILabel.GetAppearance<CustomLabel> (traits, typeof (UITextField)).TextColor, "g null 1");
				UILabel.GetAppearance<CustomLabel> (traits, typeof (UITextField)).TextColor = UIColor.Blue;
				UILabel.GetAppearance<CustomLabel> (traits, typeof (UITextField)).TextColor.GetRGBA (out r, out g, out b, out a);
				Assert.AreEqual (1, a, "g a1");
				Assert.AreEqual (0, r, "g r1");
				Assert.AreEqual (0, g, "g g1");
				Assert.AreEqual (1, b, "g b1");

				// check that other appearance instances didn't change
				Assert.IsNull (UILabel.Appearance.TextColor, "g other null 1");
				Assert.IsNull (UILabel.GetAppearance<CustomLabel> (traits).TextColor, "g other null 2");
				Assert.IsNull (UILabel.AppearanceWhenContainedIn (typeof (UITextField)).TextColor, "g other null 4");

				// it can be cleared
				UILabel.GetAppearance<CustomLabel> (traits, typeof (UITextField)).TextColor = null;
				Assert.IsNull (UILabel.GetAppearance<CustomLabel> (traits, typeof (UITextField)).TextColor, "g null 2");
			}
		}

		[Test]
		public void Appearance_UITraitCollection ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (8,0))
				Assert.Inconclusive ("UITraitCollection requires iOS 8.0");

			using (var traits = new UITraitCollection ()) {
				nfloat r, g, b, a;

				// it can be set
				Assert.IsNull (UILabel.GetAppearance (traits).TextColor, "null 1");
				UILabel.GetAppearance (traits).TextColor = UIColor.Blue;
				UILabel.GetAppearance (traits).TextColor.GetRGBA (out r, out g, out b, out a);
				Assert.AreEqual (1, a, "a1");
				Assert.AreEqual (0, r, "r1");
				Assert.AreEqual (0, g, "g1");
				Assert.AreEqual (1, b, "b1");

				// check that other appearance instances didn't change
				Assert.IsNull (UILabel.Appearance.TextColor, "other null 1");
				Assert.IsNull (UILabel.GetAppearance (traits, typeof (UITextField)).TextColor, "other null 3");
				Assert.IsNull (UILabel.AppearanceWhenContainedIn (typeof (UITextField)).TextColor, "other null 4");

				// it can be cleared
				UILabel.GetAppearance (traits).TextColor = null;
				Assert.IsNull (UILabel.GetAppearance (traits).TextColor, "null 2");
			}

			// generic version
			using (var traits = new UITraitCollection ()) {
				nfloat r, g, b, a;

				// it can be set
				Assert.IsNull (UILabel.GetAppearance<CustomLabel> (traits).TextColor, "g null 1");
				UILabel.GetAppearance<CustomLabel> (traits).TextColor = UIColor.Blue;
				UILabel.GetAppearance<CustomLabel> (traits).TextColor.GetRGBA (out r, out g, out b, out a);
				Assert.AreEqual (1, a, "g a1");
				Assert.AreEqual (0, r, "g r1");
				Assert.AreEqual (0, g, "g g1");
				Assert.AreEqual (1, b, "g b1");

				// check that other appearance instances didn't change
				Assert.IsNull (UILabel.Appearance.TextColor, "g other null 1");
				Assert.IsNull (UILabel.GetAppearance<CustomLabel> (traits, typeof (UITextField)).TextColor, "g other null 3");
				Assert.IsNull (UILabel.AppearanceWhenContainedIn (typeof (UITextField)).TextColor, "g other null 4");

				// it can be cleared
				UILabel.GetAppearance<CustomLabel> (traits).TextColor = null;
				Assert.IsNull (UILabel.GetAppearance<CustomLabel> (traits).TextColor, "g null 2");
			}
		}

		class CustomLabel : UILabel {}
	}
}

#endif // !__WATCHOS__
