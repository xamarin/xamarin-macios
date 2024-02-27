#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSUserDefaultsControllerTests {
		NSUserDefaultsController controller;

		[Test]
		public void NSUserDefaultsControllerShouldGetSharedController ()
		{
			controller = NSUserDefaultsController.SharedUserDefaultsController;

			Assert.IsNotNull (controller, "NSUserDefaultsControllerShouldGetDefaultController - SharedUserDefaultsController returned null");
		}

		[Test]
		public void NSUserDefaultsControllerShouldCreateNewControllerWithDefaultConstructor ()
		{
			controller = new NSUserDefaultsController ();

			Assert.IsNotNull (controller, "NSUserDefaultsControllerShouldCreateNewControllerWithDefaultConstructor - Constructor returned null");
		}

		[Test]
		public void NSUserDefaultsControllerShouldCreateNewControllerWithNullParameters ()
		{
			controller = new NSUserDefaultsController (null, null);

			Assert.IsTrue (controller.Defaults == NSUserDefaults.StandardUserDefaults);
			Assert.IsTrue (controller.InitialValues is null);
			Assert.IsNotNull (controller, "NSUserDefaultsControllerShouldCreateNewControllerWithNullParameters - Constructor returned null");
		}

		[Test]
		public void NSUserDefaultsControllerShouldCreateNewControllerWithParameters ()
		{
			var initialValues = new NSDictionary ();
			controller = new NSUserDefaultsController (NSUserDefaults.StandardUserDefaults, initialValues);

			Assert.IsTrue (controller.Defaults == NSUserDefaults.StandardUserDefaults);
			Assert.IsTrue (controller.InitialValues == initialValues);
			Assert.IsNotNull (controller, "NSUserDefaultsControllerShouldCreateNewControllerWithParameters - Constructor returned null");
		}

		[Test]
		public void NSUserDefaultsControllerShouldChangeInitialValues ()
		{
			controller = new NSUserDefaultsController (NSUserDefaults.StandardUserDefaults, null);
			var initialValues = controller.InitialValues;
			controller.InitialValues = new NSDictionary ();

			Assert.IsFalse (controller.InitialValues == initialValues, "NSUserDefaultsControllerShouldChangeInitialValues - Failed to set the InitialValues property");
		}

		[Test]
		public void NSUserDefaultsControllerShouldChangeAppliesImmediately ()
		{
			controller = new NSUserDefaultsController (NSUserDefaults.StandardUserDefaults, null);
			var appliesImmediately = controller.AppliesImmediately;
			controller.AppliesImmediately = !appliesImmediately;

			Assert.IsFalse (controller.AppliesImmediately == appliesImmediately, "NSUserDefaultsControllerShouldChangeAppliesImmediately - Failed to set the AppliesImmediately property");
		}
	}
}
#endif // __MACOS__
