#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSTabViewItemTests {
		NSTabViewItem item;

		[SetUp]
		public void SetUp ()
		{
			item = new NSTabViewItem ();
		}

		[Test]
		public void NSTabViewItemShouldChangeImage ()
		{
			Asserts.EnsureYosemite ();

			var image = item.Image;
			item.Image = new NSImage ();

			Assert.IsFalse (item.Image == image, "NSTabViewItemShouldChangeImage - Failed to set the Image property");
		}

		[Test]
		public void NSTabViewItemShouldChangeViewController ()
		{
			Asserts.EnsureYosemite ();

			var vc = item.ViewController;
			item.ViewController = new NSViewController ();

			Assert.IsFalse (item.ViewController == vc, "NSTabViewItemShouldChangeViewController - Failed to set the ViewController property");
		}
	}
}

#endif // __MACOS__
