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

// Adding a single test is a simple way to make it possible to launch
// the app and test that it launches successfully (by using our existing
// infrastructure for unit testing).
// Also we can only launch today extensions if the today view is not already
// open (the API we use to launch the today extension does nothing if the
// extension is already visible and running), and one way to close the today
// view is to launch an app - so before we launch the today extension, we launch
// the container app, which will immediately auto-exit (since it only has a single
// test).
[TestFixture]
class Dummy {
	[Test]
	public void Test ()
	{
	}
}
