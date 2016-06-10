using System;
using System.Threading.Tasks;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using CGRect = System.Drawing.RectangleF;
#else
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class CALayerTests
	{
		[Test]
		public void CALayer_ValuesTests ()
		{
			CALayer layer = new CALayer ();
			CGRect frame = new CGRect (10, 10, 0, 0);
			CGImage image = CGImage.ScreenImage (0, frame);
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

