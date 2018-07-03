// Copyright 2011-2013 Xamarin Inc. All rights reserved

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Drawing;
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
	public class TextViewTest {
		
		[Test]
		public void InitWithFrame ()
		{
			RectangleF frame = new RectangleF (10, 10, 100, 100);
			using (UITextView tv = new UITextView (frame)) {
				Assert.That (tv.Frame, Is.EqualTo (frame), "Frame");
			}
		}

		[Test]
		public void EmptySelection ()
		{
			using (UITextView tv = new UITextView ()) {
				Assert.That (tv.SelectedRange.Length, Is.EqualTo ((nint) 0), "SelectedRange");
				Assert.IsNull (tv.TypingAttributes, "default");
				// ^ without a [PreSnippet] attribute this would crash like:
				// 7   monotouchtest                 	0x00006340 mono_sigill_signal_handler + 64
				// 8   WebKit                        	0x06b6afa5 -[WebView(WebPrivate) styleAtSelectionStart] + 53
				// 9   UIKit                         	0x028daa8a -[UIWebDocumentView typingAttributes] + 50
				// 10  UIKit                         	0x0285de57 -[UITextView typingAttributes] + 42
				tv.TypingAttributes = new NSDictionary ();
				// Assert.IsNotNull (tv.TypingAttributes, "assigned");
				// ^ this would still crash
			}
		}

		[Test]
		public void NonEmptySelection ()
		{
			using (UITextView tv = new UITextView ()) {
				tv.Text = "Bla bla bla";
				tv.SelectAll (tv);
				Assert.That (tv.SelectedRange.Length, Is.Not.EqualTo (0), "SelectedRange");
				Assert.IsNotNull (tv.TypingAttributes, "TypingAttributes");
			}
		}

		[Test]
		// if this fails ping lobrien (or doc team) since it means Apple changed the defaults we documented
		public void LayoutManager ()
		{
			TestRuntime.AssertiOSSystemVersion (7, 0, throwIfOtherPlatform: false);

			using (UITextView tv = new UITextView ()) {
				var lm = tv.LayoutManager;
				Assert.True (lm.AllowsNonContiguousLayout, "AllowsNonContiguousLayout");
				Assert.False (lm.ExtraLineFragmentRect.IsEmpty, "ExtraLineFragmentRect");
				Assert.NotNull (lm.ExtraLineFragmentTextContainer, "ExtraLineFragmentTextContainer");
				Assert.False (lm.ExtraLineFragmentUsedRect.IsEmpty, "ExtraLineFragmentUsedRect");
				Assert.That (lm.FirstUnlaidCharacterIndex, Is.EqualTo ((nuint) 0), "FirstUnlaidCharacterIndex");
				Assert.That (lm.FirstUnlaidGlyphIndex, Is.EqualTo ((nuint) 0), "FirstUnlaidGlyphIndex");
				Assert.False (lm.HasNonContiguousLayout, "HasNonContiguousLayout");
				Assert.That (lm.HyphenationFactor, Is.EqualTo ((nfloat) 0), "HyphenationFactor");
				Assert.That (lm.NumberOfGlyphs, Is.EqualTo ((nuint) 0), "NumberOfGlyphs");
				Assert.False (lm.ShowsControlCharacters, "ShowsControlCharacters");
				Assert.False (lm.ShowsInvisibleCharacters, "ShowsInvisibleCharacters");
				Assert.NotNull (lm.TextStorage, "TextStorage");
				Assert.True (lm.UsesFontLeading, "UsesFontLeading");
			}
		}

		[Test]
		public void TextInputTraits ()
		{
			// UITextInputTraits members are not required but we marked them abstract
			// that's even more confusing since they all fails for respondToSelector tests but works in real life
			using (UITextView tv = new UITextView ()) {
				// this is just to show we can get and set those values (even if respondToSelector returns NO)
				tv.AutocapitalizationType = tv.AutocapitalizationType;
				tv.AutocorrectionType = tv.AutocorrectionType;
				tv.EnablesReturnKeyAutomatically = tv.EnablesReturnKeyAutomatically;
				tv.KeyboardAppearance = tv.KeyboardAppearance;
				tv.KeyboardType = tv.KeyboardType;
				tv.ReturnKeyType = tv.ReturnKeyType;
				tv.SecureTextEntry = tv.SecureTextEntry;
				tv.SpellCheckingType = tv.SpellCheckingType;
			}
		}
	}
}

#endif // !__WATCHOS__
