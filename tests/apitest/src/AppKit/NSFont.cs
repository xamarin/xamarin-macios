using System;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.CoreText;
#else
using AppKit;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreText;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSFontTests
	{
		[Test]
		public void GetBoundingRect_SmokeTest ()
		{
			if (!PlatformHelper.CheckSystemVersion (10, 13))
				return;

			CGFont cgFont = CGFont.CreateWithFontName ("Arial");
			ushort [] glyphs = new ushort [5];
			for (int i = 0 ; i < 5 ; ++i)
				glyphs[i] = cgFont.GetGlyphWithGlyphName ("Hello"[i].ToString ());

			CTFont ctFont = new CTFont (cgFont, 14, new CTFontDescriptor ("Arial", 14));
			NSFont nsFont = NSFont.FromCTFont (ctFont);

			var bounding = nsFont.GetBoundingRects (glyphs);
			var advancement = nsFont.GetAdvancements (glyphs);
			Assert.AreEqual (5, bounding.Length);
			Assert.AreEqual (5, advancement.Length);
		}

		[Test]
		public void GetBoundingRect_WithEmptyGlyphs ()
		{
			if (!PlatformHelper.CheckSystemVersion (10, 13))
				return;

			CGFont cgFont = CGFont.CreateWithFontName ("Arial");
			ushort [] glyphs = new ushort [] {};

			CTFont ctFont = new CTFont (cgFont, 14, new CTFontDescriptor ("Arial", 14));
			NSFont nsFont = NSFont.FromCTFont (ctFont);

			Assert.Throws <ArgumentException> (() => nsFont.GetBoundingRects (glyphs));
			Assert.Throws <ArgumentException> (() => nsFont.GetAdvancements (glyphs));
		}
	}
}

