#if !__WATCHOS__
using System;
using System.Reflection;
using Foundation;
using UIKit;
using MonoTouch.NUnit.UI;

namespace Introspection {

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		static public TouchRunner Runner { get; private set; }

		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// create a new window instance based on the screen size
			Window = new UIWindow (UIScreen.MainScreen.Bounds);
			Runner = new TouchRunner (Window);

			// tests can be inside the main assembly
			Runner.Add (Assembly.GetExecutingAssembly ());

			Window.RootViewController = new UINavigationController (Runner.GetViewController ());
			
			// make the window visible
			Window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}
#endif // !__WATCHOS__
