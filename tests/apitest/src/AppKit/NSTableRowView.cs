using System;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
#else
using AppKit;
using ObjCRuntime;
using Foundation;
#endif

namespace Xamarin.Mac.Tests
{
	public class NSTableRowViewTests
	{
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

