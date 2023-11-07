#if __MACOS__
using System;
using NUnit.Framework;
using System.Linq;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSSplitViewItemTests {
		NSSplitViewItem item;

		[SetUp]
		public void SetUp ()
		{
			Asserts.EnsureYosemite ();

			item = new NSSplitViewItem ();
		}

		[Test]
		public void NSSplitViewItemShouldCreateFromViewController ()
		{
			var viewController = new NSViewController ();
			var splitViewItem = NSSplitViewItem.FromViewController (viewController);

			Assert.IsFalse (splitViewItem is null, "NSSplitViewItemShouldCreateFromViewController - Returned null");
			Assert.IsTrue (splitViewItem.ViewController == viewController, "NSSplitViewItemShouldCreateFromViewController - ViewController property not set correctly");
		}

		[Test]
		public void NSSplitViewItemShouldChangeViewController ()
		{
			var viewController = item.ViewController;
			item.ViewController = new NSViewController ();

			Assert.IsFalse (item.ViewController == viewController, "NSSplitViewItemShouldChangeViewController - Failed to set the ViewController property");
		}

		[Test]
		public void NSSplitViewItemShouldChangeCollapsed ()
		{
			var collapsed = item.Collapsed;
			item.Collapsed = !collapsed;

			Assert.IsFalse (item.Collapsed == collapsed, "NSSplitViewItemShouldChangeCollapsed - Failed to set the Collapsed property");
		}

		[Test]
		public void NSSplitViewItemShouldChangeCanCollapse ()
		{
			var canCollapse = item.CanCollapse;
			item.CanCollapse = !canCollapse;

			Assert.IsFalse (item.CanCollapse == canCollapse, "NSSplitViewItemShouldChangeCanCollapse - Failed to set the CanCollapse property");
		}

		[Test]
		public void NSSplitViewItemShouldChangeHoldingPriority ()
		{
			var holdingPriority = item.HoldingPriority;
			item.HoldingPriority = 0.35f;

			Assert.IsFalse (item.HoldingPriority == holdingPriority, "NSSplitViewItemShouldChangeHoldingPriority - Failed to set the HoldingPriority property");
		}
	}
}
#endif // __MACOS__
