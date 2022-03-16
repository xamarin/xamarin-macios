using System.Threading;
using UIKit;

namespace MyMPSGraphApp {
	public class MnistViewController : UIViewController
	{
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.SystemYellow;
			ThreadPool.QueueUserWorkItem (_ => {
				new MnistTest ().Run ();
				BeginInvokeOnMainThread (() => {
					View.BackgroundColor = UIColor.SystemGreen;
				});
			});
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Dispose of any resources that can be recreated.
		}
	}
}
