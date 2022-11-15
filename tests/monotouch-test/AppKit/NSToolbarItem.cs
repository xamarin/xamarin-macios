#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSToolbarItemTests {
		[Test]
		public void InitTests ()
		{
			const string TestLabel = "NSToolbarItemTests.Label";
			NSToolbarItem item = new NSToolbarItem ();
			Assert.IsNotNull (item.Handle, "NSToolbarItem has handle");
			item.Label = TestLabel;
			Assert.AreEqual (item.Label, TestLabel, "NSToolbarItem has non null Label");

			NSToolbarItemGroup group = new NSToolbarItemGroup ();
			Assert.IsNotNull (group.Handle, "NSToolbarItemGroup has handle");
			Assert.AreEqual (group.Subitems.Length, 0, "NSToolbarItemGroup has zero items");
			group.Label = TestLabel;
			Assert.AreEqual (group.Label, TestLabel, "NSToolbarItemGroup has non null Label");
		}
	}
}
#endif // __MACOS__
