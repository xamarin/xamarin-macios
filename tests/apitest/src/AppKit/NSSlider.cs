using NUnit.Framework;
using System;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
#else
using AppKit;
using ObjCRuntime;
using Foundation;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSSliderTests
	{
		[Test]
		public void NSSlider_VertialTests()
		{
			NSSlider slider = new NSSlider ();
			var isVert = slider.IsVertical;
#if XAMCORE_4_0
			slider.IsVertical = true;
#else
			slider.IsVertical = 1;
#endif
			Assert.AreEqual (1, slider.IsVertical);
		}
	}
}


