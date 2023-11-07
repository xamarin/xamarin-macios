#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSToolbarTests {
		NSToolbar toolbar;

		[SetUp]
		public void SetUp ()
		{
			toolbar = new NSToolbar (NSToolbar.NSToolbarSeparatorItemIdentifier);
		}

		[Test]
		public void NSToolbarShouldChangeAllowsExtensionItems ()
		{
			Asserts.EnsureYosemite ();

			var allows = toolbar.AllowsExtensionItems;
			toolbar.AllowsExtensionItems = !allows;

			Assert.IsFalse (toolbar.AllowsExtensionItems == allows, "NSToolbarShouldChangeAllowsExtensionItems - Failed to set the AllowsExtensionItems property");
		}
	}
}
#endif // __MACOS__
