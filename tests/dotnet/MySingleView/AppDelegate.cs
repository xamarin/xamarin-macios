using System;

using Foundation;
using UIKit;

namespace MySingleView {
	public partial class AppDelegate : UIApplicationDelegate {
	}

	[Register ("SceneDelegate")]
	public class SceneDelegate : UIResponder, IUIWindowSceneDelegate {
		[Export ("window")]
		public UIWindow? Window { get; set; }

		[Export ("scene:willConnectToSession:options:")]
		public void WillConnect (UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
		{
			if (scene is not UIWindowScene windowScene)
				return;

			var window = new UIWindow (windowScene);
			Window = window;
			var dvc = new UIViewController ();
			var button = new UIButton (window.Bounds);
			button.SetTitle ("net6!", UIControlState.Normal);
			dvc.Add (button);

			window.RootViewController = dvc;
			window.MakeKeyAndVisible ();
		}
	}
}
