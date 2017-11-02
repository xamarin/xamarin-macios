//
// Unit tests for NSLayoutConstraint
//
// Authors:
//	Oleg Demchenko <oleg.demchenko@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//
#if !MONOMAC
#if XAMCORE_2_0
using UIKit;
#else
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	public class NSLayoutConstraintTest {

#if !__WATCHOS__ // FIXME: it looks like this test can be rewritten to not use UIViewController, so that it can run on WatchOS as well.
		[Test]
		public void FromVisualFormat ()
		{
			if (!TestRuntime.CheckSystemAndSDKVersion (7, 0))
				Assert.Ignore ("Ignoring FromVisualFormat tests: Requires iOS7+");

			using (var testViewController = new TestViewController ()) {
				var constraints = NSLayoutConstraint.FromVisualFormat ("V:|[topLayoutGuide]-[firstLabel]-[secondLabel]",
					NSLayoutFormatOptions.DirectionLeadingToTrailing,
					"topLayoutGuide", testViewController.TopLayoutGuide,
					"firstLabel", testViewController.FirstLabel,
					"secondLabel", testViewController.SecondLabel
				);

				const int expectedNumberOfConstraints = 3;

				Assert.That (constraints != null,
					"'NSLayoutConstraint.FromVisualFormat' method returned no constraints");

				Assert.That (constraints.Length, Is.EqualTo (expectedNumberOfConstraints),
					string.Format ("Expected number of constraints is {0}. Actual number is {1}", expectedNumberOfConstraints, constraints.Length));

				Assert.That (constraints [0].FirstItem.Handle == testViewController.TopLayoutGuide.Handle,
					"First item of constraints[0] is not testViewController.TopLayoutGuide");
				Assert.That (constraints [0].SecondItem == testViewController.View,
					"Second item of constraints[0] is not testViewController.View");
				Assert.That (constraints [0].Relation == NSLayoutRelation.Equal,
					string.Format ("Relation between views of constraints[0] must be `NSLayoutRelation.Equal`. Actual realtion is {0}", constraints [0].Relation));

				Assert.That (constraints [1].FirstItem == testViewController.FirstLabel,
					"First item of constraints[1] is not testViewController.FirstLabel");
				Assert.That (constraints [1].SecondItem.Handle == testViewController.TopLayoutGuide.Handle,
					"Second item of constraints[1] is not testViewController.TopLayoutGuide");
				Assert.That (constraints [1].Relation == NSLayoutRelation.Equal,
					string.Format ("Relation between views of constraints[1] must be `NSLayoutRelation.Equal`. Actual realtion is {0}", constraints [1].Relation));

				Assert.That (constraints [2].FirstItem == testViewController.SecondLabel,
					"First item of constraints[2] is not testViewController.SecondLabel");
				Assert.That (constraints [2].SecondItem == testViewController.FirstLabel,
					"Second item of constraints[2] is not testViewController.FirstLabel");
				Assert.That (constraints [2].Relation == NSLayoutRelation.Equal,
					string.Format ("Relation between views of constraints[2] must be `NSLayoutRelation.Equal`. Actual realtion is {0}", constraints [2].Relation));
			}
		}

		public class TestViewController : UIViewController {
			public UILabel FirstLabel { get; private set; }

			public UILabel SecondLabel { get; private set; }

			public TestViewController ()
			{
				FirstLabel = new UILabel {
					Font = UIFont.PreferredBody,
					TranslatesAutoresizingMaskIntoConstraints = false
				};

				View.Add (FirstLabel);

				SecondLabel = new UILabel {
					Font = UIFont.PreferredBody,
					TranslatesAutoresizingMaskIntoConstraints = false
				};

				View.Add (SecondLabel);
			}
		}
#endif // !__WATCHOS__
	}
}
#endif