using System;
using System.Drawing;

using NotificationCenter;
using Foundation;
using Social;
using AppKit;

namespace TodayExtensionTest {
	public partial class TodayViewController : NSViewController, INCWidgetProviding {
		public TodayViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			REPLACE_CODE_REPLACE
			// Do any additional setup after loading the view.
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
