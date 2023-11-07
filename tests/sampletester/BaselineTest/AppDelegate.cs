using Foundation;
using UIKit;

namespace BaselineTest {
	[Register ("AppDelegate")]
	public class AppDelegate : UIResponder, IUIApplicationDelegate {
		UIWindow window;
		UIViewController vc;

		[Export ("application:didFinishLaunchingWithOptions:")]
		public bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			vc = new UIViewController ();
			vc.View.BackgroundColor = UIColor.Green;
			window.RootViewController = vc;
			window.MakeKeyAndVisible ();

			return true;
		}

		static void Main (string [] args)
		{
			UIApplication.Main (args, null, typeof (AppDelegate));
		}
	}
}
