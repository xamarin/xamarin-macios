using System;
using System.Drawing;

using Foundation;

using NotificationCenter;

using ObjCRuntime;

using Social;

#if __MACOS__
using ViewController = AppKit.NSViewController;
#else
using ViewController = UIKit.NSViewController;
#endif

namespace MyTodayExtension {
	public partial class TodayViewController : ViewController, INCWidgetProviding {
		public TodayViewController (NativeHandle handle) : base (handle)
		{
		}

		[Export ("widgetPerformUpdateWithCompletionHandler:")]
		public void WidgetPerformUpdate (Action<NCUpdateResult> completionHandler)
		{
			// Perform any setup necessary in order to update the view.

			// If an error is encoutered, use NCUpdateResultFailed
			// If there's no update required, use NCUpdateResultNoData
			// If there's an update, use NCUpdateResultNewData

			completionHandler (NCUpdateResult.NewData);
		}
	}
}
