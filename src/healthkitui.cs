// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
using CoreGraphics;
using Foundation;
using HealthKit;
using ObjCRuntime;
using UIKit;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace HealthKitUI {

	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	[DisableDefaultCtor] // nil handle (introspection)
	interface HKActivityRingView {

		// inlined from UIView
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[NullAllowed, Export ("activitySummary", ArgumentSemantic.Strong)]
		HKActivitySummary ActivitySummary { get; set; }

		[Export ("setActivitySummary:animated:")]
		void SetActivitySummary ([NullAllowed] HKActivitySummary activitySummary, bool animated);
	}

	[iOS (17, 0), MacCatalyst (17, 0)]
	[Category]
	[BaseType (typeof (HKHealthStore))]
	interface HKHealthStore_UIViewController {

		[Export ("authorizationViewControllerPresenter")]
		[return: NullAllowed]
		UIViewController GetAuthorizationViewControllerPresenter ();

		[Export ("setAuthorizationViewControllerPresenter:")]
		void SetAuthorizationViewControllerPresenter ([NullAllowed] UIViewController authorizationViewControllerPresenter);
	}
}
