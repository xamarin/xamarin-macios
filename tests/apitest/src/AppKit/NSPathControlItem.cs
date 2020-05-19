using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests
{
	public class NSPathControlItemTests
	{
		[SetUp]
		public void Setup ()
		{
			Asserts.EnsureYosemite ();
		}

		[Test]
		public void NSPathControlItemShouldSetTitle ()
		{
			var item = new NSPathControlItem ();
			var title = item.Title;
			item.Title = "Test";

			Assert.IsTrue (item.Title != title, "NSPathControlShouldSetTitle - Title value did not change.");
		}

		[Test]
		public void NSPathControlItemShouldSetAttributedTitle ()
		{
			var item = new NSPathControlItem ();
			var attributedTitle = item.AttributedTitle;
			item.AttributedTitle = new NSAttributedString ("Test");

			Assert.IsTrue (item.AttributedTitle != attributedTitle, "NSPathControlShouldSetAttributedTitle - AttributedTitle value did not change.");

		}

		[Test]
		public void NSPathControlItemShouldSetImage ()
		{
			var item = new NSPathControlItem ();
			Assert.IsTrue (item.Image == null, "NSPathControlItemShouldSetImage - Image did not start as null");

			item.Image = new NSImage ();
			Assert.IsTrue (item.Image != null, "NSPathControlItemShouldSetImage - Failed to set Image property");
		}
	}
}

