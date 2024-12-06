//
// Shows how to configure a DialogViewController to support
// Pull-to-Refresh
//
using System;
using MonoTouch.Dialog;
using UIKit;
using Foundation;

namespace Sample {
	public partial class AppDelegate {
		public void DemoRefresh ()
		{
			int i = 0;
			var root = new RootElement ("Pull to Refresh"){
				new Section () {
					new MultilineElement ("Pull from the top to add\na new item at the bottom\nThen wait 1 second")
				}
			};

			var dvc = new DialogViewController (root, true);

			//
			// After the DialogViewController is created, but before it is displayed
			// Assign to the RefreshRequested event.   The event handler typically
			// will queue a network download, or compute something in some thread
			// when the update is complete, you must call "ReloadComplete" to put
			// the DialogViewController in the regular mode
			//
			dvc.RefreshRequested += delegate
			{
				// Wait 3 seconds, to simulate some network activity
				NSTimer.CreateScheduledTimer (1, delegate
				{
					root [0].Add (new StringElement ("Added " + (++i)));

					// Notify the dialog view controller that we are done
					// this will hide the progress info
					dvc.ReloadComplete ();
				});
			};
			dvc.Style = UITableViewStyle.Plain;

			navigation.PushViewController (dvc, true);
		}
	}
}

