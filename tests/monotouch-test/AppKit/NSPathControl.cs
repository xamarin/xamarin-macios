#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSPathControlTests {
		[Test]
		public void NSPathControlShouldSetEditable ()
		{
			Asserts.EnsureYosemite ();

			var control = new NSPathControl ();
			var editable = control.Editable;
			control.Editable = !editable;

			Assert.IsTrue (control.Editable != editable, "NSPathControlShouldSetEditable - Failed to change the Editable property");
		}

		[Test]
		public void NSPathControlShouldSetAllowedTypes ()
		{
			Asserts.EnsureYosemite ();

			var control = new NSPathControl ();
			var allowedTypes = control.AllowedTypes;
			control.AllowedTypes = new [] { (NSString) "exe", (NSString) "jpg" };

			Assert.IsTrue (control.AllowedTypes != allowedTypes, "NSPathControlShouldSetAllowedTypes - Failed to change AllowedTypes property");
		}

		[Test]
		public void NSPathControlShouldSetPlaceholderString ()
		{
			Asserts.EnsureYosemite ();

			var control = new NSPathControl ();
			var placeholderString = control.PlaceholderString;
			control.PlaceholderString = "Test Placeholder";

			Assert.IsTrue (control.PlaceholderString != placeholderString, "NSPathControlShouldSetPlaceholderString - Failed to change PlaceholderString property");
		}

		[Test]
		public void NSPathControlShouldSetPlaceholderAttributedString ()
		{
			Asserts.EnsureYosemite ();

			var control = new NSPathControl ();
			var placeholderAttributedString = control.PlaceholderAttributedString;
			control.PlaceholderAttributedString = new NSAttributedString ("Test Placeholder");

			Assert.IsTrue (control.PlaceholderAttributedString != placeholderAttributedString, "NSPathControlShouldSetPlaceholderAttributedString - Failed to change PlaceholderAttributedString property");
		}

		[Test]
		public void NSPathControlShouldSetPathItems ()
		{
			Asserts.EnsureYosemite ();

			var control = new NSPathControl ();
			var pathItems = control.PathItems;
			control.PathItems = new [] { new NSPathControlItem () };

			Assert.IsTrue (control.PathItems != pathItems, "NSPathControlShouldSetPathItems - Failed to set PathItems property");
		}
	}
}
#endif // __MACOS__
