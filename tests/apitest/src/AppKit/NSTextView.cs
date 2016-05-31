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
	public class NSTextViewTests
	{
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