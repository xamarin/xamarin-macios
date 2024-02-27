using System;
using System.Drawing;

using NotificationCenter;
using Foundation;
using ObjCRuntime;
using Social;
using UIKit;

namespace MyTodayExtension {
	public partial class TodayViewController : UIViewController, INCWidgetProviding {
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
