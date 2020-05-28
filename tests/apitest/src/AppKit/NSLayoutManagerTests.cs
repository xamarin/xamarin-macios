using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;
using CoreGraphics;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSLayoutManagerTests
	{
		NSLayoutManager manager;

		[SetUp]
		public void CreateManager ()
		{
			// This sets up the global context so our drawing doesn't produce error messages
			NSBitmapImageRep bitmap = new NSBitmapImageRep (IntPtr.Zero, 1000, 1000, 16, 4, true, false, NSColorSpace.DeviceRGB, 0, 0);
			NSGraphicsContext.CurrentContext = NSGraphicsContext.FromBitmap (bitmap);

			NSTextStorage storage = new NSTextStorage ("Hello World");
			NSTextContainer container = new NSTextContainer ();
			manager = new NSLayoutManager ();

			manager.AddTextContainer (container);
			storage.AddLayoutManager (manager);
		}

		[Test]
		public void NSLayoutManager_DrawBackgroundForGlyphRange ()
		{
			manager.DrawBackgroundForGlyphRange (new NSRange (0, 4), new CGPoint (10, 10));
		}

		[Test]
		public void NSLayoutManager_DrawGlyphsForGlyphRange ()
		{
			manager.DrawGlyphsForGlyphRange (new NSRange (0, 4), new CGPoint (10, 10));
		}

		[Test]
		public void NSLayoutManager_CharacterRangeForGlyphRange ()
		{
			NSRange pnt;
			NSRange range = manager.CharacterRangeForGlyphRange (new NSRange (0, 4), out pnt);
			Assert.IsNotNull (range);
		}

		[Test]
		public void NSLayoutManager_GlyphRangeForCharacterRange ()
		{
			NSRange pnt;
			NSRange range = manager.GlyphRangeForCharacterRange (new NSRange (0, 4), out pnt);
			Assert.IsNotNull (range);
		}
	}
}
