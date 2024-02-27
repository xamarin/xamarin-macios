// https://bugzilla.xamarin.com/show_bug.cgi?id=15307#c3

using System;
using ObjCRuntime;
using Foundation;
using UIKit;

namespace BindingTests {

	// error BI1018: btouch: No [Export] attribute on property MonoTouch.ObjCRuntime.INativeObject.Handle
	[BaseType (typeof (UIPageViewController))]
	interface MyPageViewController : IUIPageViewControllerDelegate, IUIPageViewControllerDataSource {
	}
}
