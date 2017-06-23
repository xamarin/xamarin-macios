using System;
using NUnit.Framework;
using CoreGraphics;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
#else
using AppKit;
using ObjCRuntime;
using Foundation;
#endif

namespace Xamarin.Mac.Tests {
	[TestFixture]
	public class NSFontTests {
		[Test]
		public void GetBoundingRect_Tests ()
		{
			var glyph = CGFont.CreateWithFontName ("Arial").GetGlyphWithGlyphName ("a");
		}
	}
}

