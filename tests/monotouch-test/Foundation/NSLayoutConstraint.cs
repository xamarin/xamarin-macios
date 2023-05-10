#if __MACOS__
using AppKit;
using Foundation;
using NUnit.Framework;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSLayoutConstraintTest {
		[Test]
		public void FromVisualFormat ()
		{
			using (var testView = new TestView ()) {
				var constraints = NSLayoutConstraint.FromVisualFormat ("[firstLabel]-[secondLabel]-|",
					NSLayoutFormatOptions.DirectionLeadingToTrailing,
					"firstLabel", testView.FirstLabel,
					"secondLabel", testView.SecondLabel
				);

				const int expectedNumberOfConstraints = 2;

				Assert.That (constraints is not null,
					"'NSLayoutConstraint.FromVisualFormat' method returned no constraints");

				Assert.That (constraints.Length == expectedNumberOfConstraints,
					string.Format ("Expected number of constraints is {0}. Actual number is {1}", expectedNumberOfConstraints, constraints.Length));

				Assert.That (constraints [0].FirstItem == testView.SecondLabel,
					"First item of constraints[0] is not testView.SecondLabel");
				Assert.That (constraints [0].SecondItem == testView.FirstLabel,
					"Second item of constraints[0] is not testView.FirstLabel");
				Assert.That (constraints [0].Relation == NSLayoutRelation.Equal,
					string.Format ("Relation between views of constraints[0] must be `NSLayoutRelation.Equal`. Actual realtion is {0}", constraints [0].Relation));

				Assert.That (constraints [1].FirstItem == testView,
					"First item of constraints[1] is not testView");
				Assert.That (constraints [1].SecondItem == testView.SecondLabel,
					"Second item of constraints[1] is not testView.SecondLabel");
				Assert.That (constraints [1].Relation == NSLayoutRelation.Equal,
					string.Format ("Relation between views of constraints[1] must be `NSLayoutRelation.Equal`. Actual realtion is {0}", constraints [1].Relation));
			}
		}
	}

	public class TestView : NSView {
		public NSTextView FirstLabel { get; private set; }

		public NSTextView SecondLabel { get; private set; }

		public TestView ()
		{
			FirstLabel = new NSTextView {
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			AddSubview (FirstLabel);

			SecondLabel = new NSTextView {
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			AddSubview (SecondLabel);
		}
	}
}

#endif // __MACOS__
