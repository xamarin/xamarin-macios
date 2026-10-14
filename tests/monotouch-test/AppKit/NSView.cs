#if __MACOS__
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using AppKit;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSViewTests {
		NSView view;

		[SetUp]
		public void SetUp ()
		{
			view = new NSView ();
		}

#if !XAMCORE_5_0
		class CompatibilityAccessibilityElement : NSAccessibilityElement, INSAccessibilityElementProtocol {
		}

		[Test]
		public void AccessibilityChildrenInNavigationOrder ()
		{
			using var parent = new NSView ();
			using var child = new CompatibilityAccessibilityElement ();
			using var childView = new NSView ();
			using var incompatibleChild = new NSAccessibilityElement ();

#pragma warning disable CS0618
			parent.AccessibilityChildrenInNavigationOrder = null;
			Assert.That (parent.AccessibilityChildrenInNavigationOrder, Is.Null.Or.Empty, "Null");
			Assert.That (parent.AccessibilityChildrenInNavigationOrder2, Is.Null.Or.Empty, "Null2");

			parent.AccessibilityChildrenInNavigationOrder = [];
			Assert.That (parent.AccessibilityChildrenInNavigationOrder, Is.Empty, "Empty");
			Assert.That (parent.AccessibilityChildrenInNavigationOrder2, Is.Empty, "Empty2");

			parent.AccessibilityChildrenInNavigationOrder = [child];
			Assert.That (parent.AccessibilityChildrenInNavigationOrder, Is.EqualTo (new [] { child }), "Children");
			Assert.That (parent.AccessibilityChildrenInNavigationOrder2, Is.EqualTo (new [] { child }), "Children2");

			parent.AccessibilityChildrenInNavigationOrder2 = [child];
			Assert.That (parent.AccessibilityChildrenInNavigationOrder, Is.EqualTo (new [] { child }), "FromChildren2");

			parent.AccessibilityChildrenInNavigationOrder2 = [childView];
			Assert.Throws<InvalidCastException> (() => _ = parent.AccessibilityChildrenInNavigationOrder, "InvalidElementType");
			Assert.Throws<InvalidCastException> (() => parent.AccessibilityChildrenInNavigationOrder = [incompatibleChild], "InvalidSetterElementType");
#pragma warning restore CS0618
		}
#endif

		[Test]
		public void NSViewShouldAddGestureRecognizer ()
		{
			var length = 0;
			if (view.GestureRecognizers is not null)
				length = view.GestureRecognizers.Length;
			view.AddGestureRecognizer (new NSGestureRecognizer ());

			Assert.That (view.GestureRecognizers.Length, Is.EqualTo (length + 1), "NSViewShouldAddGestureRecognizer - Failed to add recognizer, count didn't change.");
		}

		[Test]
		public void NSViewShouldRemoveGestureRecognizer ()
		{
			var recognizer = new NSClickGestureRecognizer ();
			view.AddGestureRecognizer (recognizer);

			Assert.That (view.GestureRecognizers.Length, Is.Not.EqualTo (0), "NSViewShouldRemoveGestureRecognizer - Failed to add gesture recognizer");

			view.RemoveGestureRecognizer (recognizer);

			Assert.That (view.GestureRecognizers.Length, Is.EqualTo (0), "NSViewShouldRemoveGestureRecognizer - Failed to remove gesture recognizer");
		}

		[Test]
		public void NSViewShouldChangeGestureRecognizers ()
		{
			var recognizers = view.GestureRecognizers;
			view.GestureRecognizers = new NSGestureRecognizer [] { new NSClickGestureRecognizer (), new NSPanGestureRecognizer () };

			Assert.That (view.GestureRecognizers, Is.Not.EqualTo (recognizers));
		}

		[Test]
		[UnconditionalSuppressMessage ("Trimming", "IL2075", Justification = "This test handles APIs that have been linked away, so it's trimmer-safe.")]
		[DynamicDependency ("Menu", typeof (AppKit.NSCell))]
		[DynamicDependency ("Menu", typeof (AppKit.NSMenuItem))]
		[DynamicDependency ("Menu", typeof (AppKit.NSPathControl))]
		[DynamicDependency ("Menu", typeof (AppKit.NSPopUpButton))]
		[DynamicDependency ("Menu", typeof (AppKit.NSPopUpButtonCell))]
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
				if (prop is null && TestRuntime.IsLinkAny)
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

				Assert.That (containerView.Subviews.Length, Is.EqualTo (3), "Presort Length");
				Assert.That (((NSTextView) containerView.Subviews [0]).Value, Is.EqualTo ("b"), "Presort Value 0");
				Assert.That (((NSTextView) containerView.Subviews [1]).Value, Is.EqualTo ("c"), "Presort Value 1");
				Assert.That (((NSTextView) containerView.Subviews [2]).Value, Is.EqualTo ("a"), "Presort Value 2");

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

				Assert.That (containerView.Subviews.Length, Is.EqualTo (3), "Postsort Length");
				Assert.That (((NSTextView) containerView.Subviews [0]).Value, Is.EqualTo ("a"), "Postsort Value 0");
				Assert.That (((NSTextView) containerView.Subviews [1]).Value, Is.EqualTo ("b"), "Postsort Value 1");
				Assert.That (((NSTextView) containerView.Subviews [2]).Value, Is.EqualTo ("c"), "Postsort Value 2");

				try {
					containerView.SortSubviews ((x, y) => {
						throw new ApplicationException ("Something went wrong");
					});
					Assert.Fail ("No exception thrown");
				} catch (Exception e) {
					Assert.That (e.Message, Is.EqualTo ("An exception occurred during sorting."), "Exception Message");
					Assert.That (e.InnerException.Message, Is.EqualTo ("Something went wrong"), "InnerException Message");
				}
			}
		}
	}
}
#endif // __MACOS__
