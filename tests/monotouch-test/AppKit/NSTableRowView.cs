#if __MACOS__
using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSTableRowViewTests {
		NSTableRowView view;

		[SetUp]
		public void SetUp ()
		{
			view = new NSTableRowView ();
		}

		[Test]
		public void NSTableRowViewShouldChangePreviousRowSelected ()
		{
			Asserts.EnsureYosemite ();

			var selected = view.PreviousRowSelected;
			view.PreviousRowSelected = !selected;

			Assert.IsFalse (view.PreviousRowSelected == selected, "NSTableRowViewShouldChangePreviousRowSelected - Failed to set the PreviousRowSelected property");
		}

		[Test]
		public void NSTableRowViewShouldChangeNextRowSelected ()
		{
			Asserts.EnsureYosemite ();

			var selected = view.NextRowSelected;
			view.NextRowSelected = !selected;

			Assert.IsFalse (view.NextRowSelected == selected, "NSTableRowViewShouldChangeNextRowSelected - Failed to set the NextRowSelected property");
		}
	}
}

#endif // __MACOS__
