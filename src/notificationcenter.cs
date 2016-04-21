using System;
using System.ComponentModel;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

#if !MONOMAC
using XamCore.UIKit;
#endif

namespace XamCore.NotificationCenter {

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // not meant to be user created
	interface NCWidgetController {

		[Static]
		[Export ("widgetController")]
		NCWidgetController GetWidgetController();

		[Export ("setHasContent:forWidgetWithBundleIdentifier:")]
		void SetHasContent (bool flag, string bundleID);
	}

	[iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NCWidgetProviding {

		[Export ("widgetPerformUpdateWithCompletionHandler:")]
		void WidgetPerformUpdate(Action<NCUpdateResult> completionHandler);

		[Export ("widgetMarginInsetsForProposedMarginInsets:"), DelegateName ("NCWidgetProvidingMarginInsets"), DefaultValueFromArgument ("defaultMarginInsets")]
		UIEdgeInsets GetWidgetMarginInsets (UIEdgeInsets defaultMarginInsets);
	}


	[iOS (8,0)]
	[BaseType (typeof (UIVibrancyEffect))]
	[Category]
	interface UIVibrancyEffect_NotificationCenter {
#if XAMCORE_2_0
		[Internal]
#else
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
#endif
		[Static, Export ("notificationCenterVibrancyEffect")]
		UIVibrancyEffect NotificationCenterVibrancyEffect ();
	}


}
