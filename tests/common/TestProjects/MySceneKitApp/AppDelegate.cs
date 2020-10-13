using Foundation;
using UIKit;

namespace MySceneKitApp {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		UIViewController dvc;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			dvc = new UIViewController ();
			dvc.View.BackgroundColor = UIColor.Green;

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

