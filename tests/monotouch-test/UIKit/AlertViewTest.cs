// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !__TVOS__ && !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
using System.Reflection;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
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

namespace MonoTouchFixtures.UIKit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AlertViewTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UIAlertView av = new UIAlertView (frame)) {
				Assert.That (av.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		class MyAlertViewDelegate : UIAlertViewDelegate {
		}

		[Test]
		public void CtorNull ()
		{
			// null title
			using (var a = new UIAlertView (null, "message", null, null, null)) {
				Assert.That (a.Handle, Is.Not.EqualTo (IntPtr.Zero), "1");
			}
			// null message
			using (var a = new UIAlertView ("title", null, null, null, null)) {
				Assert.That (a.Handle, Is.Not.EqualTo (IntPtr.Zero), "2");
			}
			// all null
			using (var a = new UIAlertView (null, null, null, null, null)) {
				Assert.That (a.Handle, Is.Not.EqualTo (IntPtr.Zero), "3");
			}
		}

		[Test]
		public void CtorDelegate ()
		{
			using (var del = new MyAlertViewDelegate ())
			using (var a = new UIAlertView ("title", "message", del, null, null)) {
				Assert.That (a.Title, Is.EqualTo ("title"), "Title");
				Assert.That (a.Message, Is.EqualTo ("message"), "Message");
				Assert.NotNull (typeof (UIAlertView).GetField ("__mt_WeakDelegate_var", BindingFlags.Instance | BindingFlags.NonPublic).GetValue (a), "backing field");
				// check properties after the field (so we're not setting it only when calling the properties)
				Assert.NotNull (a.Delegate, "Delegate");
				Assert.NotNull (a.WeakDelegate, "WeakDelegate");
			}
		}

		[Test]
		public void FirstOtherButtonIndex ()
		{
			using (var a = new UIAlertView ("title", "message", null, "cancel", "other")) {
				Assert.That (a.FirstOtherButtonIndex, Is.EqualTo ((nint) 1), "#other button index");
			}
		}
	}
}

#endif // !__TVOS__ && !__WATCHOS__
