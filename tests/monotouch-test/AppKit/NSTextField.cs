#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSTextFieldTests {
		NSTextField textField;

		[SetUp]
		public void SetUp ()
		{
			textField = new NSTextField ();
		}

		[Test]
		public void NSTextFieldShouldChangePlaceholderString ()
		{
			Asserts.EnsureYosemite ();

			var placeholder = textField.PlaceholderString;
			textField.PlaceholderString = "Test";

			Assert.IsFalse (textField.PlaceholderString == placeholder, "NSTextFieldShouldChangePlaceholderString - Failed to set the PlaceholderString property");
		}

		[Test]
		public void NSTextFieldShouldChangePlaceholderAttributedString ()
		{
			Asserts.EnsureYosemite ();

			var placeholder = textField.PlaceholderAttributedString;
			textField.PlaceholderAttributedString = new NSAttributedString ("Test");

			Assert.IsFalse (textField.PlaceholderAttributedString == placeholder, "NSTextFieldShouldChangePlaceholderAttributedString - Failed to set the PlaceholderAttributedString property");
		}
	}
}
#endif // __MACOS__
