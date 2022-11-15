#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSViewControllerTests {
		NSViewController controller;

		[SetUp]
		public void SetUp ()
		{
			controller = new NSViewController ();
		}

		[Test]
		public void NSViewControllerShouldAddChildViewController ()
		{
			Asserts.EnsureYosemite ();

			var child = new NSViewController ();
			controller.AddChildViewController (child);

			Assert.IsTrue (controller.ChildViewControllers.Length == 1, "NSViewControllerShouldAddChildViewControllers - Failed to add child view controller");
		}

		[Test]
		public void NSViewControllerShouldRemoveChildViewController ()
		{
			Asserts.EnsureYosemite ();

			var child = new NSViewController ();
			controller.AddChildViewController (child);

			Assert.IsTrue (controller.ChildViewControllers.Length == 1, "NSViewControllerShouldRemoveChildViewControllers - Failed to add child view controller");

			controller.RemoveChildViewController (0);

			Assert.IsTrue (controller.ChildViewControllers.Length == 0, "NSViewControllerShouldRemoveChildViewController - Failed to remove child view controller");
		}

		[Test]
		public void NSViewControllerShouldInsertChildViewController ()
		{
			Asserts.EnsureYosemite ();

			controller.AddChildViewController (new NSViewController ());
			controller.AddChildViewController (new NSViewController ());

			Assert.IsTrue (controller.ChildViewControllers.Length == 2, "NSViewControllerShouldInsertChildViewController - Failed to add child view controller");

			var child = new NSViewController ();
			controller.InsertChildViewController (child, 1);

			Assert.IsTrue (controller.ChildViewControllers.Length == 3, "NSViewControllerShouldInsertChildViewController - Failed to insert child view controller");
			Assert.IsTrue (controller.ChildViewControllers [1] == child, "NSViewControllerShouldInsertChildViewController - Inserted child view controller at the wrong index.");
		}
	}
}
#endif // __MACOS__
