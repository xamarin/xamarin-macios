#if __MACOS__
using System;
using NUnit.Framework;
using System.Linq;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSSplitViewControllerTests {
		NSSplitViewController controller;

		[SetUp]
		public void SetUp ()
		{
			Asserts.EnsureYosemite ();

			controller = new NSSplitViewController ();
		}

		[Test]
		public void NSSplitViewControllerShouldChangeSplitView ()
		{
			var splitView = controller.SplitView;
			controller.SplitView = new NSSplitView ();

			Assert.IsFalse (controller.SplitView == splitView, "NSSplitViewControllerShouldChangeSplitView - Failed to set the SplitView property");
		}

		[Test]
		public void NSSplitViewControllerShouldChangeSplitViewItems ()
		{
			var items = controller.SplitViewItems;
			controller.SplitViewItems = new NSSplitViewItem [] { new NSSplitViewItem { ViewController = new NSViewController () } };

			Assert.IsFalse (controller.SplitViewItems == items, "NSSplitViewControllerShouldChangeSplitViewItems - Failed to set the SplitViewItems property");
		}

		[Test]
		public void NSSplitViewControllerShouldAddSplitViewItem ()
		{
			var item = new NSSplitViewItem { ViewController = new NSViewController () };
			controller.AddSplitViewItem (item);

			Assert.IsTrue (controller.SplitViewItems.Contains (item), "NSSplitViewControllerShouldAddSplitViewItem - Failed to add item");
		}

		[Test]
		public void NSSplitViewControllerShouldRemoveSplitViewItem ()
		{
			var item = new NSSplitViewItem { ViewController = new NSViewController () };
			controller.AddSplitViewItem (item);

			Assert.IsTrue (controller.SplitViewItems.Contains (item), "NSSplitViewControllerShouldRemoveSplitViewItem - Failed to add item");

			controller.RemoveSplitViewItem (item);

			Assert.IsFalse (controller.SplitViewItems.Contains (item), "NSSplitViewControllerShouldRemoveSplitViewItem - Failed to remove item");
		}

		[Test]
		public void NSSplitViewControllerShouldInsertSplitViewItem ()
		{
			controller.AddSplitViewItem (new NSSplitViewItem { ViewController = new NSViewController () });
			controller.AddSplitViewItem (new NSSplitViewItem { ViewController = new NSViewController () });
			controller.AddSplitViewItem (new NSSplitViewItem { ViewController = new NSViewController () });
			var item = new NSSplitViewItem { ViewController = new NSViewController () };
			controller.InsertSplitViewItem (item, 1);

			Assert.IsTrue (controller.SplitViewItems [1] == item, "NSSplitViewControllerShouldInsertSplitViewItem - Failed to insert the item at the given position.");
			Assert.IsFalse (controller.SplitViewItems [0] == item, "NSSplitViewControllerShouldInsertSplitViewItem - Inserted the item in the wrong position.");
		}

		[Test]
		public void NSSplitViewControllerShouldGetSplitViewItem ()
		{
			controller.AddSplitViewItem (new NSSplitViewItem { ViewController = new NSViewController () });
			controller.AddSplitViewItem (new NSSplitViewItem { ViewController = new NSViewController () });
			controller.AddSplitViewItem (new NSSplitViewItem { ViewController = new NSViewController () });
			var viewController = new NSViewController ();
			var item = new NSSplitViewItem { ViewController = viewController };
			controller.InsertSplitViewItem (item, 1);

			var retrievedItem = controller.GetSplitViewItem (viewController);

			Assert.IsTrue (retrievedItem == item, "NSSplitViewControllerShouldGetSplitViewItem - Failed to get SplitViewItem from ViewController");
		}
	}
}
#endif // __MACOS__
