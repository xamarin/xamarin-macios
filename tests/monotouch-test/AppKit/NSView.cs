#if __MACOS__
using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSViewTests {
		NSView view;

		[SetUp]
		public void SetUp ()
		{
			view = new NSView ();
		}

		[Test]
		public void NSViewShouldAddGestureRecognizer ()
		{
			Asserts.EnsureYosemite ();

			var length = 0;
			if (view.GestureRecognizers is not null)
				length = view.GestureRecognizers.Length;
			view.AddGestureRecognizer (new NSGestureRecognizer ());

			Assert.IsTrue (view.GestureRecognizers.Length == length + 1, "NSViewShouldAddGestureRecognizer - Failed to add recognizer, count didn't change.");
		}

		[Test]
		public void NSViewShouldRemoveGestureRecognizer ()
		{
			Asserts.EnsureYosemite ();

			var recognizer = new NSClickGestureRecognizer ();
			view.AddGestureRecognizer (recognizer);

			Assert.IsTrue (view.GestureRecognizers.Length != 0, "NSViewShouldRemoveGestureRecognizer - Failed to add gesture recognizer");

			view.RemoveGestureRecognizer (recognizer);

			Assert.IsTrue (view.GestureRecognizers.Length == 0, "NSViewShouldRemoveGestureRecognizer - Failed to remove gesture recognizer");
		}

		[Test]
		public void NSViewShouldChangeGestureRecognizers ()
		{
			Asserts.EnsureYosemite ();

			var recognizers = view.GestureRecognizers;
			view.GestureRecognizers = new NSGestureRecognizer [] { new NSClickGestureRecognizer (), new NSPanGestureRecognizer () };

			Assert.IsFalse (view.GestureRecognizers == recognizers);
		}

		[Test]
		public void AllItemsWithNSMenuShouldAllowNull ()
		{
			// Can't test NSResponder since it is abstract
			var types = new List<Func<NSObject>> {
				() => new NSCell (),
				() => new NSMenuItem (),
				() => new NSPathControl (),
				() => new NSPopUpButton (),
				() => new NSPopUpButtonCell (),
			};

			foreach (var ctor in types) {
				var o = ctor ();
				var prop = o.GetType ().GetProperty ("Menu", BindingFlags.Public | BindingFlags.Instance);
				if (prop is null && TestRuntime.IsLinkAll)
					continue; // the property was linked away.
				prop.SetValue (o, null, null);
			}

			// NSStateBarItem can't be created via default constructor
			NSStatusBar.SystemStatusBar.CreateStatusItem (10).Menu = null;
		}

		[Test]
		public void SubviewSort ()
		{
			using (var containerView = new NSView ())
			using (var a = new NSTextView () { Value = "a" })
			using (var b = new NSTextView () { Value = "b" })
			using (var c = new NSTextView () { Value = "c" }) {

				containerView.AddSubview (b);
				containerView.AddSubview (c);
				containerView.AddSubview (a);

				Assert.Throws<ArgumentNullException> (() => containerView.SortSubviews (null), "ANE");

				Assert.AreEqual (3, containerView.Subviews.Length, "Presort Length");
				Assert.AreEqual ("b", ((NSTextView) containerView.Subviews [0]).Value, "Presort Value 0");
				Assert.AreEqual ("c", ((NSTextView) containerView.Subviews [1]).Value, "Presort Value 1");
				Assert.AreEqual ("a", ((NSTextView) containerView.Subviews [2]).Value, "Presort Value 2");

				containerView.SortSubviews ((x, y) => {
					var viewX = (NSTextView) x;
					var viewY = (NSTextView) y;
					var rv = string.Compare (viewX.Value, viewY.Value, StringComparison.Ordinal);
					if (rv == 0)
						return NSComparisonResult.Same;
					else if (rv < 0)
						return NSComparisonResult.Ascending;
					else
						return NSComparisonResult.Descending;
				});

				Assert.AreEqual (3, containerView.Subviews.Length, "Postsort Length");
				Assert.AreEqual ("a", ((NSTextView) containerView.Subviews [0]).Value, "Postsort Value 0");
				Assert.AreEqual ("b", ((NSTextView) containerView.Subviews [1]).Value, "Postsort Value 1");
				Assert.AreEqual ("c", ((NSTextView) containerView.Subviews [2]).Value, "Postsort Value 2");

				try {
					containerView.SortSubviews ((x, y) => {
						throw new ApplicationException ("Something went wrong");
					});
					Assert.Fail ("No exception thrown");
				} catch (Exception e) {
					Assert.AreEqual ("An exception occurred during sorting.", e.Message, "Exception Message");
					Assert.AreEqual ("Something went wrong", e.InnerException.Message, "InnerException Message");
				}
			}
		}
	}
}
#endif // __MACOS__
