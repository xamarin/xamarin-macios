#if !__WATCHOS__ && !MONOMAC
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using CoreFoundation;
using Foundation;
using UIKit;
using MonoTouch.NUnit.UI;
using NUnit.Framework.Internal;
using MonoTouchFixtures.BackgroundTasks;

public partial class AppDelegate : UIApplicationDelegate {
	// class-level declarations
	static UIWindow window;
	TouchRunner runner => Runner;

#if __IOS__ && !__MACCATALYST__
	public override bool AccessibilityPerformMagicTap ()
	{
		try {
			runner.OpenWriter ("Magic Tap");
			runner.Run (runner.LoadedTest as TestSuite);
		} finally {
			runner.CloseWriter ();
		}
		return true;
	}
#endif

	partial void PostFinishedLaunching ()
	{
		// required for the background tasks tests, we can only register the tasks in this method
		BGTaskSchedulerTest.RegisterTestTasks ();
		window = Window;
	}

	public static void PresentModalViewController (UIViewController vc, double duration)
	{
		var bckp = window.RootViewController;
		window.RootViewController = vc;
		try {
			NSRunLoop.Main.RunUntil (NSDate.Now.AddSeconds (duration));
		} finally {
			window.RootViewController = bckp;
		}
	}
}


#endif // !__WATCHOS__
