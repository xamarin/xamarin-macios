#if __MACOS__
using System;
using System.Runtime.InteropServices;

using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreText;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSFontTests
	{
		[Test]
		public void GetBoundingRect_SmokeTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			CGFont cgFont = CGFont.CreateWithFontName ("Arial");
			ushort [] glyphs = new ushort [5];
			for (int i = 0 ; i < 5 ; ++i)
				glyphs[i] = cgFont.GetGlyphWithGlyphName ("Hello"[i].ToString ());

#if NO_NFLOAT_OPERATORS
			CTFont ctFont = new CTFont (cgFont, new NFloat (14), new CTFontDescriptor ("Arial", new NFloat (14)));
#else
			CTFont ctFont = new CTFont (cgFont, 14, new CTFontDescriptor ("Arial", 14));
#endif
			NSFont nsFont = NSFont.FromCTFont (ctFont);

			var bounding = nsFont.GetBoundingRects (glyphs);
			var advancement = nsFont.GetAdvancements (glyphs);
			Assert.AreEqual (5, bounding.Length);
			Assert.AreEqual (5, advancement.Length);
		}

		[Test]
		public void GetBoundingRect_WithEmptyGlyphs ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			CGFont cgFont = CGFont.CreateWithFontName ("Arial");
			ushort [] glyphs = new ushort [] {};

#if NO_NFLOAT_OPERATORS
			CTFont ctFont = new CTFont (cgFont, new NFloat (14), new CTFontDescriptor ("Arial", new NFloat (14)));
#else
			CTFont ctFont = new CTFont (cgFont, 14, new CTFontDescriptor ("Arial", 14));
#endif
			NSFont nsFont = NSFont.FromCTFont (ctFont);

			Assert.Throws <ArgumentException> (() => nsFont.GetBoundingRects (glyphs));
			Assert.Throws <ArgumentException> (() => nsFont.GetAdvancements (glyphs));
		}
	}
}

#endif // __MACOS__
