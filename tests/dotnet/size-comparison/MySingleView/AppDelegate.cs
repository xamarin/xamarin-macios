using System;

using Foundation;
using UIKit;

namespace MyComparableApp {
	public partial class AppDelegate : UIApplicationDelegate {
		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			var dvc = new UIViewController ();
			var button = new UIButton (window.Bounds);
			button.SetTitle ("Hello World!", UIControlState.Normal);
			dvc.Add (button);

			window.RootViewController = dvc;
			window.MakeKeyAndVisible ();

			return true;
		}

		static void Main (string [] args)
		{
			UIApplication.Main (args, null, typeof (AppDelegate));
		}
	}
}
