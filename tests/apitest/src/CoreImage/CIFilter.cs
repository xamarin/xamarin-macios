using NUnit.Framework;
using System;

using AppKit;
using ObjCRuntime;
using CoreImage;
using CoreGraphics;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class CIFilterTests
	{
		[Test]
		public void CIFilterOutputImageTest ()
		{
			NSImage nsImg = new NSImage (new CGSize (400, 400));
			nsImg.LockFocus ();
			NSColor.Red.SetFill ();
			NSBezierPath.FillRect (new CGRect (0, 0, 400, 400));
			nsImg.UnlockFocus ();

			CIImage img = CIImage.FromCGImage (nsImg.CGImage);
			var gloom = new CIGloom () {
				Image = img,
				Intensity = 5,
				Radius = 1
			};
			var output = gloom.OutputImage;
			Assert.IsNotNull (output, "CIFilterOutputImageTest - output was null");
		}
	}
}