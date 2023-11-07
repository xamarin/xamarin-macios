#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSTextViewTests {
		NSTextView view;

		[SetUp]
		public void SetUp ()
		{
			view = new NSTextView ();
		}

		[Test]
		public void NSTextViewShouldChangeUsesRolloverButtonForSelection ()
		{
			Asserts.EnsureYosemite ();

			var usesRollover = view.UsesRolloverButtonForSelection;
			view.UsesRolloverButtonForSelection = !usesRollover;

			Assert.IsFalse (view.UsesRolloverButtonForSelection == usesRollover, "NSTextViewShouldChangeUsesRolloverButtonForSelection - Failed to set the UsesRolloverButtonForSelection property");
		}
	}
}
#endif // __MACOS__
