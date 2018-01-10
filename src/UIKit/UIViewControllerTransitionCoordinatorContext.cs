//
// UIViewControllerTransitionCoordinatorContext.cs: Helper methods to make the class more usable
//
// Authors: miguel de icaza
//
// Copyright 2014 Xamarin
//

#if !WATCH

namespace UIKit {
	public static partial class UIViewControllerTransitionCoordinatorContext_Extensions {
		public static UIView GetTransitionViewController (this IUIViewControllerTransitionCoordinatorContext This, UITransitionViewControllerKind kind)
		{
			switch (kind){
			case UITransitionViewControllerKind.ToView:
				return This.GetTransitionViewControllerForKey (UITransitionContext.ToViewKey);
			case UITransitionViewControllerKind.FromView:
				return This.GetTransitionViewControllerForKey (UITransitionContext.FromViewKey);
			default:
				return null;
			}
		}
	}
}

#endif // !WATCH
