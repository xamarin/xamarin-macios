#if __MACOS__
using NUnit.Framework;
using System;

using AppKit;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSImageTests {
		[Test]
		public void ImageWithSize ()
		{
			Asserts.EnsureMountainLion ();
			var image = NSImage.ImageWithSize (new CGSize (50, 50), false, rect => {
				return true;
			});
			Assert.IsNotNull (image);
		}

		[Test]
		public void NSImageCapInsets ()
		{
			Asserts.EnsureYosemite ();

			var image = new NSImage ();
			image.CapInsets = new NSEdgeInsets (5f, 6f, 7f, 8f);

			Assert.IsNotNull (image.CapInsets);
			Assert.IsTrue (image.CapInsets.Top == 5f, "NSImageCapInsets - Top value was not 5");
			Assert.IsTrue (image.CapInsets.Left == 6f, "NSImageCapInsets - Left value was not 6");
			Assert.IsTrue (image.CapInsets.Bottom == 7f, "NSImageCapInsets - Bottom value was not 7");
			Assert.IsTrue (image.CapInsets.Right == 8f, "NSImageCapInsets - Right value was not 8");
		}

		[Test]
		public void NSImageResizingModeShouldChange ()
		{
			Asserts.EnsureYosemite ();

			var image = new NSImage ();
			image.ResizingMode = NSImageResizingMode.Stretch;
			Assert.AreEqual (image.ResizingMode, NSImageResizingMode.Stretch, "NSImageResizingMode - Was not equal to Stretch");
			Assert.AreNotEqual (image.ResizingMode, NSImageResizingMode.Tile, "NSImageResizingMode - Was incorrectly equal to Tile");
		}
	}
}
#endif // __MACOS__
