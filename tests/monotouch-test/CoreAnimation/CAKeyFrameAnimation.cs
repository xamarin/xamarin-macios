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
	public class CAKeyFrameAnimationTests {
		[Test]
		public void CAKeyFrameAnimation_ValuesTests ()
		{
			CAKeyFrameAnimation keyFrameAni = new CAKeyFrameAnimation ();
			keyFrameAni.Values = new NSObject [] { new NSNumber (5) };
			Assert.AreEqual (1, keyFrameAni.Values.Length);
			NSNumber arrayNumber = (NSNumber) keyFrameAni.Values [0];
			Assert.AreEqual (5, arrayNumber.Int32Value);

			CGRect frame = new CGRect (10, 10, 10, 10);

			using var provider = new CGDataProvider (new byte [(int) frame.Width * (int) frame.Height * 4]);
			using var colorSpace = CGColorSpace.CreateDeviceRGB ();
			using var image = new CGImage ((int) frame.Width, (int) frame.Height, 8, 32, 4 * (int) frame.Width, colorSpace,
				CGBitmapFlags.ByteOrderDefault | CGBitmapFlags.Last, provider, null, false, CGColorRenderingIntent.Default);

			keyFrameAni.SetValues (new CGImage [] { image, image });
			Assert.AreEqual (2, keyFrameAni.Values.Length);
			CGImage arrayImage = (CGImage) keyFrameAni.GetValuesAs<CGImage> () [1];
			Assert.AreEqual (image.Handle, arrayImage.Handle);
		}
	}
}

#endif // __MACOS__
