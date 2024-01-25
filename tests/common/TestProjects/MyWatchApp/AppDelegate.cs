using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;

using UIKit;

namespace MyWatchApp {
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {
		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			window.MakeKeyAndVisible ();

			return true;
		}
	}
}
