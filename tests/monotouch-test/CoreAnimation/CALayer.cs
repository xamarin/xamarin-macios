#if __MACOS__
using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CALayerTests {
		[Test]
		public void CALayer_ValuesTests ()
		{
			CALayer layer = new CALayer ();
			CGRect frame = new CGRect (10, 10, 10, 10);
			using var provider = new CGDataProvider (new byte [(int) frame.Width * (int) frame.Height * 4]);
			using var colorSpace = CGColorSpace.CreateDeviceRGB ();
			using var image = new CGImage ((int) frame.Width, (int) frame.Height, 8, 32, 4 * (int) frame.Width, colorSpace,
				CGBitmapFlags.ByteOrderDefault | CGBitmapFlags.Last, provider, null, false, CGColorRenderingIntent.Default);

			NSImage NSImage = new NSImage ();

			layer.Contents = image;
			CGImage arrayImage = layer.Contents;
			Assert.AreEqual (image.Handle, arrayImage.Handle);

			layer.SetContents (NSImage);
			NSImage arrayNSImage = layer.GetContentsAs<NSImage> ();
			Assert.AreEqual (NSImage.Handle, arrayNSImage.Handle);

			layer.SetContents (null); // Should not throw
			layer.Contents = null; // Should not throw
		}
	}
}

#endif // __MACOS__
