//
// NSLayoutManager Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class LayoutManagerTest {

		[Test]
		public void Defaults ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			using (var lm = new NSLayoutManager ()) {
				Assert.False (lm.AllowsNonContiguousLayout, "AllowsNonContiguousLayout");
				Assert.True (lm.ExtraLineFragmentRect.IsEmpty, "ExtraLineFragmentRect");
				Assert.Null (lm.ExtraLineFragmentTextContainer, "ExtraLineFragmentTextContainer");
				Assert.True (lm.ExtraLineFragmentUsedRect.IsEmpty, "ExtraLineFragmentUsedRect");
				Assert.That (lm.FirstUnlaidCharacterIndex, Is.EqualTo ((nuint) 0), "FirstUnlaidCharacterIndex");
				Assert.That (lm.FirstUnlaidGlyphIndex, Is.EqualTo ((nuint) 0), "FirstUnlaidGlyphIndex");
				Assert.False (lm.HasNonContiguousLayout, "HasNonContiguousLayout");
#if !__MACCATALYST__
				Assert.That (lm.HyphenationFactor, Is.EqualTo ((nfloat) 0), "HyphenationFactor");
#endif
				Assert.That (lm.NumberOfGlyphs, Is.EqualTo ((nuint) 0), "NumberOfGlyphs");
				Assert.False (lm.ShowsControlCharacters, "ShowsControlCharacters");
				Assert.False (lm.ShowsInvisibleCharacters, "ShowsInvisibleCharacters");
				Assert.Null (lm.TextStorage, "TextStorage");
				Assert.True (lm.UsesFontLeading, "UsesFontLeading");
				if (TestRuntime.CheckXcodeVersion (10, 0))
					Assert.False (lm.LimitsLayoutForSuspiciousContents, "LimitsLayoutForSuspiciousContents");
			}
		}

		[Test]
		public void GetGlyphsTest ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.iOS, 7, 0, throwIfOtherPlatform: false);

			using (var txt = new NSTextStorage ()) {
				var str = "hello world\n\t";
				txt.SetString (new NSAttributedString (str));
				using (var lm = new NSLayoutManager ()) {
					lm.TextStorage = txt;
					var glyphs = new short [str.Length];
					var props = new NSGlyphProperty [glyphs.Length];
					var charIndexBuffer = new nuint [glyphs.Length];
					var bidiLevelBuffer = new byte [glyphs.Length];
					lm.GetGlyphs (new NSRange (0, str.Length), glyphs, props, charIndexBuffer, bidiLevelBuffer);
					Assert.That (glyphs, Is.EqualTo (new short [] { 75, 72, 79, 79, 82, 3, 90, 82, 85, 79, 71, -1, -1 }), "glyphs");
					Assert.That (props, Is.EqualTo (new NSGlyphProperty [] {
						0,
						0,
						0,
						0,
						0,
						NSGlyphProperty.Elastic,
						0,
						0,
						0,
						0,
						0,
						NSGlyphProperty.ControlCharacter,
						NSGlyphProperty.ControlCharacter
					}), "props");
					Assert.That (charIndexBuffer, Is.EqualTo (new nuint [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }), "charIndexBuffer");
					Assert.That (bidiLevelBuffer, Is.EqualTo (new byte [str.Length]), "bidiLevelBuffer");
				}
			}

			using (var txt = new NSTextStorage ()) {
				var str = "hello world\n\t";
				txt.SetString (new NSAttributedString (str));
				using (var lm = new NSLayoutManager ()) {
					lm.TextStorage = txt;
					var glyphs = new short [str.Length];
					var charIndexBuffer = new nuint [glyphs.Length];
					var bidiLevelBuffer = new byte [glyphs.Length];
					lm.GetGlyphs (new NSRange (0, str.Length), glyphs, null, charIndexBuffer, bidiLevelBuffer);
					Assert.That (glyphs, Is.EqualTo (new short [] { 75, 72, 79, 79, 82, 3, 90, 82, 85, 79, 71, -1, -1 }), "glyphs");

					Assert.That (charIndexBuffer, Is.EqualTo (new nuint [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }), "charIndexBuffer");
					Assert.That (bidiLevelBuffer, Is.EqualTo (new byte [str.Length]), "bidiLevelBuffer");
				}
			}
		}
	}
}

#endif // !__WATCHOS__
