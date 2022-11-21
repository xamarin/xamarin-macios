//
// UIPageViewController.cs: Helper methods for the UIPageViewController
//
// Copyright 2011-2012, Xamarin, Inc.
//
// Author:
//  Miguel de Icaza
//

#if !WATCH

using Foundation;

namespace UIKit {
	public partial class UIPageViewController {
		public UIPageViewController (UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation navigationOrientation, UIPageViewControllerSpineLocation spineLocation) : this (style, navigationOrientation, NSDictionary.FromObjectsAndKeys (new object [] { spineLocation }, new object [] { OptionSpineLocationKey }))
		{
		}

		public UIPageViewController (UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation navigationOrientation, UIPageViewControllerSpineLocation spineLocation, float interPageSpacing) : this (style, navigationOrientation, NSDictionary.FromObjectsAndKeys (new object [] { spineLocation, interPageSpacing }, new object [] { OptionSpineLocationKey, OptionInterPageSpacingKey }))
		{
		}

		public UIPageViewController (UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation navigationOrientation) : this (style, navigationOrientation, NSDictionary.FromObjectsAndKeys (new object [] { UIPageViewControllerSpineLocation.Mid }, new object [] { OptionSpineLocationKey }))
		{
		}

	}
}

#endif // !WATCH
