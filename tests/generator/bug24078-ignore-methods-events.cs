/*
 * Allow to ignore certain methods from the generation of events.
 * 
 */
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test {

	[BaseType (typeof (NSObject),
		Delegates = new string [] { "WeakDelegate" },
		Events = new Type [] { typeof (UIPopoverPresentationControllerDelegate) })]
	public partial interface TestController {
		[Export ("delegate", ArgumentSemantic.UnsafeUnretained)]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIPopoverPresentationControllerDelegate Delegate { get; set; }
	}

	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	public partial interface UIPopoverPresentationControllerDelegate {
		[IgnoredInDelegate]  // ignore this method in the c# events
		[Export ("adaptivePresentationStyleForPresentationController:")]
		UIModalPresentationStyle GetAdaptivePresentationStyle (UIPresentationController forPresentationController);

		[Export ("adaptivePresentationStyleForPresentationController:traitCollection:"),
			DelegateName ("AdaptivePresentationStyleWithTraitsRequested"), DefaultValue (UIModalPresentationStyle.None)]
		UIModalPresentationStyle GetAdaptivePresentationStyle (UIPresentationController controller, UITraitCollection traitCollection);

	}
}
