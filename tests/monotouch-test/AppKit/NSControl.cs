#if __MACOS__
using NUnit.Framework;
using System;

using AppKit;
using Foundation;
using ObjCRuntime;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSControlTests {
		[Test]
		public void NSControlShouldChangeControlSize ()
		{
			Asserts.EnsureYosemite ();
			var control = new NSButton ();
			var size = control.ControlSize;
			control.ControlSize = NSControlSize.Mini;

			Assert.IsFalse (size == control.ControlSize);
			Assert.IsTrue (control.ControlSize == NSControlSize.Mini);
		}

		[Test]
		public void NSControlShouldChangeHighlighted ()
		{
			Asserts.EnsureYosemite ();

			var control = new NSButton ();
			var highlighted = control.Highlighted;
			control.Highlighted = !highlighted;

			Assert.IsFalse (highlighted == control.Highlighted);
		}

		[Test]
		public void NSControlShouldChangeLineBreakMode ()
		{
			Asserts.EnsureYosemite ();

			var control = new NSButton ();
			var lineBreak = control.LineBreakMode;
			control.LineBreakMode = NSLineBreakMode.Clipping;

			Assert.IsTrue (control.LineBreakMode == NSLineBreakMode.Clipping);
			Assert.IsFalse (lineBreak == control.LineBreakMode);
		}

		[Test]
		public void NSControlShouldAddMultipleActivatedEventHandlers ()
		{
			var control = new NSButton ();

			int firstHitCount = 0;
			int secondHitCount = 0;

			control.Activated += (sender, e) => firstHitCount++;
			control.Activated += (sender, e) => secondHitCount++;

			control.PerformClick (control);

			Assert.IsTrue (firstHitCount == 1, "NSControlShouldAddMultipleActivatedEventHandlers - Did not call first EventHandler");
			Assert.IsTrue (secondHitCount == 1, "NSControlShouldAddMultipleActivatedEventHandlers - Did not call second EventHandler");
		}

		[Test]
		public void NSControlShouldRemoveAndAddActivatedEventHandlers ()
		{
			var control = new NSButton ();

			int firstHitCount = 0;
			int secondHitCount = 0;

			EventHandler firstDelegate = (object sender, EventArgs e) => firstHitCount++;

			control.Activated += firstDelegate;
			control.Activated -= firstDelegate;
			control.Activated += (sender, e) => secondHitCount++;

			control.PerformClick (control);

			Assert.IsTrue (firstHitCount == 0, "NSControlShouldRemoveAndAddActivatedEventHandlers - Called first EventHandler after it was removed");
			Assert.IsTrue (secondHitCount == 1, "NSControlShouldRemoveAndAddActivatedEventHandlers - Did not call second EventHandler");
		}
	}
}
#endif // __MACOS__
