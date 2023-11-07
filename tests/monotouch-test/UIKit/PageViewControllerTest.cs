//
// Unit tests for UIPageViewController
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__ && !MONOMAC

using System;
using System.Reflection;
using Foundation;
using UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PageViewControllerTest {

		[Test]
		public void Defaults ()
		{
			UIPageViewController pvc = new UIPageViewController ();
			Assert.Null (pvc.DataSource, "DataSource");
			Assert.Null (pvc.Delegate, "Delegate");
			Assert.False (pvc.DoubleSided, "DoubleSided");
			Assert.That (pvc.GestureRecognizers.Length, Is.EqualTo (2), "GestureRecognizers");
			Assert.Null (pvc.GetNextViewController, "GetNextViewController");
			Assert.Null (pvc.GetPreviousViewController, "GetPreviousViewController");
#if !__TVOS__
			Assert.Null (pvc.GetSpineLocation, "GetSpineLocation");
#endif
			Assert.That (pvc.NavigationOrientation, Is.EqualTo (UIPageViewControllerNavigationOrientation.Horizontal), "NavigationOrientation");
			Assert.That (pvc.SpineLocation, Is.EqualTo (UIPageViewControllerSpineLocation.Min), "SpineLocation");
			Assert.That (pvc.TransitionStyle, Is.EqualTo (UIPageViewControllerTransitionStyle.PageCurl), "TransitionStyle");
			Assert.That (pvc.ViewControllers.Length, Is.EqualTo (0), "ViewControllers");
		}

		UIPageViewController pvc;

		[Test]
		public void SetViewControllers ()
		{
			pvc = new UIPageViewController ();
			// note: Complete is called synchronously
			pvc.SetViewControllers (pvc.ViewControllers, UIPageViewControllerNavigationDirection.Forward, false, Complete);
			Assert.Null (pvc, "pvc");
		}

		void Complete (bool finished)
		{
			Assert.True (finished, "finished");
			pvc = null;
		}
	}
}

#endif // !__WATCHOS__
