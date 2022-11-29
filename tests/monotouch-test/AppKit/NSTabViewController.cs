#if __MACOS__
using System;
using NUnit.Framework;
using System.Linq;

using AppKit;
using ObjCRuntime;
using Foundation;
using Xamarin.Utils;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSTabViewControllerTests {
		NSTabViewController controller;

		[SetUp]
		public void SetUp ()
		{
			Asserts.EnsureYosemite ();

			controller = new NSTabViewController ();
		}

		[Test]
		public void NSTabViewControllerShouldChangeTabStyle ()
		{
			var tabStyle = controller.TabStyle;
			controller.TabStyle = NSTabViewControllerTabStyle.Toolbar;

			Assert.IsFalse (controller.TabStyle == tabStyle, "NSTabViewControllerShouldChangeTabStyle - Failed to set the TabStyle property");
		}

		//		[Test]
		//		public void NSTabViewControllerShouldChangeTabView ()
		//		{
		//			var tabView = controller.TabView;
		//			controller.TabView = new NSTabView ();
		//
		//			Assert.IsFalse (controller.TabView == tabView, "NSTabViewControllerShouldChangeTabView - Failed to set the TabView property");
		//		}

#if !NET
		[Test]
		public void NSTabViewControllerShouldChangeSegmentedControl ()
		{
			// This API was removed in 10.11
			if (TestRuntime.CheckXcodeVersion (7, 0))
				return;

			var segmentedControl = controller.SegmentedControl;
			controller.SegmentedControl = new NSSegmentedControl ();

			Assert.IsFalse (controller.SegmentedControl == segmentedControl, "NSTabViewControllerShouldChangeSegmentedControl - Failed to set the SegmentedControl property");
		}
#endif

		[Test]
		public void NSTabViewControllerShouldChangeTransitionOptions ()
		{
			var options = controller.TransitionOptions;
			controller.TransitionOptions = NSViewControllerTransitionOptions.Crossfade | NSViewControllerTransitionOptions.SlideRight;

			Assert.IsFalse (controller.TransitionOptions == options, "NSTabViewControllerShouldChangeTransitionOptions - Failed to set the TransitionOptions property");
		}

		[Test]
		public void NSTabViewControllerShouldChangeCanPropagateSelectedChildViewControllerTitle ()
		{
			var canPropogate = controller.CanPropagateSelectedChildViewControllerTitle;
			controller.CanPropagateSelectedChildViewControllerTitle = !canPropogate;

			Assert.IsFalse (controller.CanPropagateSelectedChildViewControllerTitle == canPropogate, "NSTabViewControllerShouldChangeCanPropagateSelectedChildViewControllerTitle - Failed to set the CanPropagateSelectedChildViewControllerTitle property");
		}

		[Test]
		public void NSTabViewControllerShouldChangeTabViewItems ()
		{
			var items = controller.TabViewItems;
			controller.TabViewItems = new NSTabViewItem [] { new NSTabViewItem { ViewController = new NSViewController () } };

			Assert.IsFalse (controller.TabViewItems == items, "NSTabViewControllerShouldChangeTabViewItems - Failed to set the TabViewItems property");
		}

		[Test]
		public void NSTabViewControllerShouldChangeSelectedTabViewItemIndex ()
		{
			controller.TabViewItems = new NSTabViewItem [] {
				new NSTabViewItem { ViewController = new NSViewController () },
				new NSTabViewItem { ViewController = new NSViewController () },
				new NSTabViewItem { ViewController = new NSViewController () }
			};

			var index = controller.SelectedTabViewItemIndex;
			controller.SelectedTabViewItemIndex = (index + 1) % 3;

			Assert.IsFalse (controller.SelectedTabViewItemIndex == index, "NSTabViewControllerShouldChangeSelectedTabViewItemIndex - Failed to set the SelectedTabViewItemIndex property");
		}

		[Test]
		public void NSTabViewControllerShouldAddTabViewItem ()
		{
			var item = new NSTabViewItem { ViewController = new NSViewController () };
			controller.AddTabViewItem (item);

			Assert.IsTrue (controller.TabViewItems.Contains (item), "NSTabViewControllerShouldAddTabViewItem - Failed to add TabViewItem");
		}

		[Test]
		public void NSTabViewControllerShouldRemoveTabViewItem ()
		{
			var item = new NSTabViewItem { ViewController = new NSViewController () };
			controller.AddTabViewItem (item);

			Assert.IsTrue (controller.TabViewItems.Contains (item), "NSTabViewControllerShouldRemoveTabViewItem - Failed to add item");

			controller.RemoveTabViewItem (item);

			Assert.IsFalse (controller.TabViewItems.Contains (item), "NSTabViewControllerShouldRemoveTabViewItem - Failed to remove item");
		}

		[Test]
		public void NSTabViewControllerShouldInsertTabViewItem ()
		{
			controller.AddTabViewItem (new NSTabViewItem { ViewController = new NSViewController () });
			controller.AddTabViewItem (new NSTabViewItem { ViewController = new NSViewController () });
			controller.AddTabViewItem (new NSTabViewItem { ViewController = new NSViewController () });
			var item = new NSTabViewItem { ViewController = new NSViewController () };
			controller.InsertTabViewItem (item, 1);

			Assert.IsTrue (controller.TabViewItems [1] == item, "NSTabViewControllerShouldInsertTabViewItem - Failed to insert the item at the given position.");
			Assert.IsFalse (controller.TabViewItems [0] == item, "NSTabViewControllerShouldInsertTabViewItem - Inserted the item in the wrong position.");
		}

		[Test]
		public void NSTabViewControllerShouldGetTabViewItem ()
		{
			controller.AddTabViewItem (new NSTabViewItem { ViewController = new NSViewController () });
			controller.AddTabViewItem (new NSTabViewItem { ViewController = new NSViewController () });
			controller.AddTabViewItem (new NSTabViewItem { ViewController = new NSViewController () });
			var viewController = new NSViewController ();
			var item = new NSTabViewItem { ViewController = viewController };
			controller.InsertTabViewItem (item, 1);

			var retrievedItem = controller.GetTabViewItem (viewController);

			Assert.IsTrue (retrievedItem == item, "NSTabViewControllerShouldGetTabViewItem - Failed to get TabViewItem from ViewController");
		}
	}
}
#endif // __MACOS__
