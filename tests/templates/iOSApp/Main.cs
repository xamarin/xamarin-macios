using System;

using Foundation;
using UIKit;

using MonoTouch.NUnit.UI;
using NUnit.Framework;
using NUnit.Framework.Internal;

[Register ("AppDelegate")]
public partial class AppDelegate : UIApplicationDelegate {
	UIWindow window;
	TouchRunner runner;

	public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	{
		window = new UIWindow (UIScreen.MainScreen.Bounds);

		runner = new TouchRunner (window);
		runner.Add (System.Reflection.Assembly.GetExecutingAssembly ());

		window.RootViewController = new UINavigationController (runner.GetViewController ());
		window.MakeKeyAndVisible ();

		return true;
	}

	static void Main (string [] args)
	{
		UIApplication.Main (args, null, typeof (AppDelegate));
	}
}
