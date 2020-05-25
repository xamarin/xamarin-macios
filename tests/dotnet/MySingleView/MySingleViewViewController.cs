using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MySingleView
{
	public partial class MySingleViewViewController : UIViewController
	{
		public MySingleViewViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			MyButton.SetTitle ("net5!", UIControlState.Normal);
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

