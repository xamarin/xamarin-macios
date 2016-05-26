//
// Unit tests for CTFont
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using CoreGraphics;
using CoreText;
using Foundation;
using UIKit;
#else
using MonoTouch.CoreGraphics;
using MonoTouch.CoreText;
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

namespace MonoTouchFixtures.CoreText {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class A_FontTest {

		[Test]
		public void CTFontCreateWithNameAndOptions ()
		{
			// Apple documents CTFontCreateWithNameAndOptions as availble since 3.2 but it does not work before 7.0 (e.g. 6.1)
			// it seems to be a known issue, http://stackoverflow.com/q/4419283 but I still left feedback to Apple to fix it
			try {
				using (var font = new CTFont ("HoeflerText-Regular", 10, CTFontOptions.Default)) {
					Assert.That (font.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				}
			}
			catch (EntryPointNotFoundException) {
				Assert.True (!UIDevice.CurrentDevice.CheckSystemVersion (7, 0), "< iOS 7.0");
			}
		}

		[Test]
		public void CTFontCreateWithFontDescriptorAndOptions ()
		{
			// Apple documents CTFontCreateWithFontDescriptorAndOptions as availble since 3.2 but it does not work before 7.0 (e.g. 6.1)
			// it seems to be a known issue, http://stackoverflow.com/q/4419283 but I still left feedback to Apple to fix it
			try {
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
			catch (EntryPointNotFoundException) {
				Assert.True (!UIDevice.CurrentDevice.CheckSystemVersion (7, 0), "< iOS 7.0");
			}
		}

		[Test]
		public void GetCascadeList ()
		{
			try {
				using (var font = new CTFont ("HoeflerText-Regular", 10, CTFontOptions.Default)) {
					Assert.NotNull (font.GetDefaultCascadeList (null), "null");
				}
			}
			catch (EntryPointNotFoundException) {
				Assert.True (!UIDevice.CurrentDevice.CheckSystemVersion (7, 0), "< iOS 7.0");
			}
		}

		[Test]
		public void GetLocalizedName ()
		{
			try {
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
			catch (EntryPointNotFoundException) {
				Assert.True (!UIDevice.CurrentDevice.CheckSystemVersion (7, 0), "< iOS 7.0");
			}
		}

		[Test]
		public void GetGlyphsForCharacters_35048 ()
		{
			using (var font = CGFont.CreateWithFontName ("AppleColorEmoji"))
			using (var ctfont = font.ToCTFont ((nfloat) 10.0)) {
				ushort[] gid = new ushort [2];
				Assert.True (ctfont.GetGlyphsForCharacters ("\ud83d\ude00".ToCharArray (), gid), "GetGlyphsForCharacters");
				Assert.That (gid [0], Is.Not.EqualTo (0), "0");
				Assert.That (gid [1], Is.EqualTo (0), "1");
			}
		}
	}
}

#endif // !__WATCHOS__
