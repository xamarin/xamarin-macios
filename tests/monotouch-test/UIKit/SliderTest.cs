// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !XAMCORE_3_0 && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
using CoreGraphics;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	class SliderPoker : UISlider {

		static FieldInfo bkCurrentThumbImage;
		static FieldInfo bkCurrentMinTrackImage;
		static FieldInfo bkCurrentMaxTrackImage;
		static FieldInfo bkMinValueImage;
		static FieldInfo bkMaxValueImage;

		static SliderPoker ()
		{
			var t = typeof (UISlider);
			bkCurrentThumbImage = t.GetField ("__mt_CurrentThumbImage_var", BindingFlags.Instance | BindingFlags.NonPublic);
			bkCurrentMinTrackImage = t.GetField ("__mt_CurrentMinTrackImage_var", BindingFlags.Instance | BindingFlags.NonPublic);
			bkCurrentMaxTrackImage = t.GetField ("__mt_CurrentMaxTrackImage_var", BindingFlags.Instance | BindingFlags.NonPublic);
			bkMinValueImage = t.GetField ("__mt_MinValueImage_var", BindingFlags.Instance | BindingFlags.NonPublic);
			bkMaxValueImage = t.GetField ("__mt_MaxValueImage_var", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public static bool NewRefcountEnabled ()
		{
			return NSObject.IsNewRefcountEnabled ();
		}

		public SliderPoker ()
		{
		}

		public UIImage CurrentThumbImageBackingField {
			get {
				return (UIImage) bkCurrentThumbImage.GetValue (this);
			}
		}

		public UIImage CurrentMinTrackImageBackingField {
			get {
				return (UIImage) bkCurrentMinTrackImage.GetValue (this);
			}
		}

		public UIImage CurrentMaxTrackImageBackingField {
			get {
				return (UIImage) bkCurrentMaxTrackImage.GetValue (this);
			}
		}

		public UIImage MinValueImageBackingField {
			get {
				return (UIImage) bkMinValueImage.GetValue (this);
			}
		}

		public UIImage MaxValueImageBackingField {
			get {
				return (UIImage) bkMaxValueImage.GetValue (this);
			}
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SliderTest {

		[Test]
		public void InitWithFrame ()
		{
			var frame = new CGRect (10, 10, 100, 100);
			using (UISlider s = new UISlider (frame)) {
				Assert.That (s.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		[Test]
		public void Ctor_Default_BackingFields ()
		{
			if (SliderPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");

			using (var s = new SliderPoker ()) {
				// default constructor does not set any UIViewController so the backing fields are null
				Assert.Null (s.CurrentThumbImageBackingField, "1a");
				Assert.Null (s.CurrentMinTrackImageBackingField, "2a");
				Assert.Null (s.CurrentMaxTrackImageBackingField, "3a");
				Assert.Null (s.MinValueImageBackingField, "4a");
				Assert.Null (s.MaxValueImageBackingField, "5a");

				Assert.Null (s.CurrentThumbImage, "1b");
				Assert.Null (s.CurrentMinTrackImage, "2b");
				Assert.Null (s.CurrentMaxTrackImage, "3b");
				Assert.Null (s.MinValueImage, "4b");
				Assert.Null (s.MaxValueImage, "5b");
			}
		}

		[Test]
		public void CurrentThumbImage_BackingFields ()
		{
			if (SliderPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");

			using (var i = new UIImage ())
			using (var s = new SliderPoker ()) {
				// default constructor does not set any UIViewController so the backing fields are null
				Assert.Null (s.CurrentThumbImageBackingField, "1a");
				Assert.Null (s.CurrentThumbImage, "1b");

				s.SetThumbImage (i, UIControlState.Normal);
				Assert.NotNull (s.CurrentThumbImageBackingField, "1c");
				Assert.NotNull (s.CurrentThumbImage, "1d");
			}
		}

		[Test]
		public void CurrentMinTrackImage_BackingFields ()
		{
			if (SliderPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");

			using (var i = new UIImage ())
			using (var s = new SliderPoker ()) {
				// default constructor does not set any UIViewController so the backing fields are null
				Assert.Null (s.CurrentMinTrackImageBackingField, "1a");
				Assert.Null (s.CurrentMinTrackImage, "1b");

				s.SetMinTrackImage (i, UIControlState.Normal);
				Assert.NotNull (s.CurrentMinTrackImageBackingField, "1c");
				Assert.NotNull (s.CurrentMinTrackImage, "1d");
			}
		}

		[Test]
		public void CurrentMaxTrackImage_BackingFields ()
		{
			if (SliderPoker.NewRefcountEnabled ())
				Assert.Inconclusive ("backing fields are removed when newrefcount is enabled");

			using (var i = new UIImage ())
			using (var s = new SliderPoker ()) {
				// default constructor does not set any UIViewController so the backing fields are null
				Assert.Null (s.CurrentMaxTrackImageBackingField, "1a");
				Assert.Null (s.CurrentMaxTrackImage, "1b");

				s.SetMaxTrackImage (i, UIControlState.Normal);
				Assert.NotNull (s.CurrentMaxTrackImageBackingField, "1c");
				Assert.NotNull (s.CurrentMaxTrackImage, "1d");
			}
		}
	}
}

#endif // !XAMCORE_3_0 && !MONOMAC
