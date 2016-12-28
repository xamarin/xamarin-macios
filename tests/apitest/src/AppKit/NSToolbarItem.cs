using System;
using NUnit.Framework;

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
	public class NSToolbarItemTests
	{
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
