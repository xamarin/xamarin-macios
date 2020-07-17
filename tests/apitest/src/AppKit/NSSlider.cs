using NUnit.Framework;
using System;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSSliderTests
	{
		[Test]
		public void NSSlider_VertialTests()
		{
			if (PlatformHelper.ToMacVersion (PlatformHelper.GetHostApiPlatform ()) < Platform.Mac_10_12)
				return;

			NSSlider slider = new NSSlider ();
			var isVert = slider.IsVertical;
#if XAMCORE_4_0
			slider.IsVertical = true;
#else
			slider.IsVertical = 1;
#endif
			Assert.AreEqual ((nint) 1, slider.IsVertical);
		}
	}
}


