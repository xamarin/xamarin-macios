using System;
using System.Drawing;

using NotificationCenter;
using Foundation;
using Social;
using UIKit;

namespace MyTodayExtension {
	public partial class TodayViewController : UIViewController, INCWidgetProviding {
		public TodayViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

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
