// Copyright 2011-2013 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using CoreGraphics;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
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
	public class TextFieldTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UITextField tf = new UITextField (frame)) {
				Assert.That (tf.Frame, Is.EqualTo (frame), "Frame");
			}
		}
		
		[Test]
		public void InputAccessoryViewTest ()
		{
			using (var tf = new UITextField ()) {
				Assert.That (tf.InputAccessoryView, Is.Null, "1");
				tf.InputAccessoryView = new UIView ();
				Assert.That (tf.InputAccessoryView, Is.Not.Null, "2");
				tf.InputAccessoryView = null;
				Assert.That (tf.InputAccessoryView, Is.Null, "3");
			}
		}

		[Test]
		public void EmptySelection ()
		{
			using (var tf = new UITextField ()) {
				if (TestRuntime.CheckXcodeVersion (9, 0)) {
					Assert.IsNotNull (tf.SelectedTextRange, "SelectedTextRange 1");
				} else {
					Assert.IsNull (tf.SelectedTextRange, "SelectedTextRange");
				}
				Assert.IsNull (tf.TypingAttributes, "default");
				// ^ calling TypingAttributes does not crash like UITextView does, it simply returns null
				tf.TypingAttributes = new NSDictionary ();
				Assert.IsNull (tf.TypingAttributes, "empty");
				// and it stays null, even if assigned, since there's not selection
			}
		}

		[Test]
		public void LeftRight ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=15004
			using (UITextField tf = new UITextField ()) {
				Assert.IsNull (tf.LeftView, "LeftView");
				tf.LeftView = null;
				Assert.IsNull (tf.RightView, "RightView");
				tf.RightView = null;
			}
		}

		[Test]
		public void GetCaretRectForPositiont_Null ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=20572
			using (UITextField tf = new UITextField ()) {
				// most iOS versions returns `CGRect.Empty` but iOS [9-11.2[ did not
				// so we're not checking the return value - just that the call works (original bug)
				tf.GetCaretRectForPosition (null);
			}
		}

		[Test]
		public void TextInputTraits ()
		{
			// UITextInputTraits members are not required but we marked them abstract
			// that's even more confusing since they all fails for respondToSelector tests but works in real life
			using (UITextField tf = new UITextField ()) {
				// this is just to show we can get and set those values (even if respondToSelector returns NO)
				tf.AutocapitalizationType = tf.AutocapitalizationType;
				tf.AutocorrectionType = tf.AutocorrectionType;
				tf.EnablesReturnKeyAutomatically = tf.EnablesReturnKeyAutomatically;
				tf.KeyboardAppearance = tf.KeyboardAppearance;
				tf.KeyboardType = tf.KeyboardType;
				tf.ReturnKeyType = tf.ReturnKeyType;
				tf.SecureTextEntry = tf.SecureTextEntry;
				tf.SpellCheckingType = tf.SpellCheckingType;
			}
		}
	}
}

#endif // !__WATCHOS__
