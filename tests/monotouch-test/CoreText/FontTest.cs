//
// Unit tests for CTFont
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using CoreGraphics;
using CoreText;
using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreText {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FontTest {

		[Test]
		public void CTFontCreateWithNameAndOptions ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var font = new CTFont ("HoeflerText-Regular", 10, CTFontOptions.Default)) {
				Assert.That (font.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void CTFontCreateWithFontDescriptorAndOptions ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			CTFontDescriptorAttributes fda = new CTFontDescriptorAttributes () {
				FamilyName = "Courier",
				StyleName = "Bold",
				Size = 16.0f
			};
			using (var fd = new CTFontDescriptor (fda))
			using (var font = new CTFont (fd, 10, CTFontOptions.Default)) {
				Assert.That (font.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void GetCascadeList ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var font = new CTFont ("HoeflerText-Regular", 10, CTFontOptions.Default)) {
				Assert.NotNull (font.GetDefaultCascadeList (null), "null");
			}
		}

		[Test]
		public void GetLocalizedName ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var font = new CTFont ("HoeflerText-Regular", 10, CTFontOptions.Default)) {
				Assert.NotNull (font.GetLocalizedName (CTFontNameKey.Copyright), "1");

				// We need to check if we are using english as our main language since this is the known case
				// that the following code works. It fails with spanish for example but it is a false positive
				// because the localized name for this font Full option does not have a spanish representation
				var language = NSLocale.PreferredLanguages [0];
				if (language == "en") {
					string str;
					Assert.NotNull (font.GetLocalizedName (CTFontNameKey.Full, out str), "2");
					Assert.NotNull (str, "out str");
				}
			}
		}

		[Test]
		public void GetGlyphsForCharacters_35048 ()
		{
			using (var font = CGFont.CreateWithFontName ("AppleColorEmoji"))
			using (var ctfont = font.ToCTFont ((nfloat) 10.0)) {
				ushort [] gid = new ushort [2];
				Assert.True (ctfont.GetGlyphsForCharacters ("\ud83d\ude00".ToCharArray (), gid), "GetGlyphsForCharacters");
				Assert.That (gid [0], Is.Not.EqualTo (0), "0");
				Assert.That (gid [1], Is.EqualTo (0), "1");
			}
		}

		[Test]
		public void CTFontCreateForString ()
		{
			TestRuntime.AssertXcodeVersion (5, 0);

			using (var f1 = new CTFont ("HoeflerText-Regular", 10, CTFontOptions.Default))
			using (var f2 = f1.ForString ("xamarin", new NSRange (0, 3))) {
				Assert.That (f2.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
		}

		[Test]
		public void CTFontCreateForStringWithLanguage ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);

			using (var f1 = new CTFont ("HoeflerText-Regular", 10, CTFontOptions.Default)) {
				using (var f2 = f1.ForString ("xamarin", new NSRange (0, 3), null))
					Assert.That (f2.Handle, Is.Not.EqualTo (IntPtr.Zero), "f2");
				using (var f3 = f1.ForString ("xamarin", new NSRange (0, 3), "FR"))
					Assert.That (f3.Handle, Is.Not.EqualTo (IntPtr.Zero), "f3");
			}
		}

		[Test]
		public void CTFontCopyNameForGlyph ()
		{
			TestRuntime.AssertXcodeVersion (12, 0);

			using (var ctfont = new CTFont ("HoeflerText-Regular", 10, CTFontOptions.Default))
				Assert.That (ctfont.GetGlyphName ((ushort) 65), Is.EqualTo ("asciicircum"), "1");

			using (var font = CGFont.CreateWithFontName ("AppleColorEmoji"))
			using (var ctfont = font.ToCTFont ((nfloat) 10.0))
				Assert.Null (ctfont.GetGlyphName ('\ud83d'), "2");
		}
	}
}
