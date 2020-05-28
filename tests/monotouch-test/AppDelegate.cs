#if !__WATCHOS__ && !MONOMAC
using System;
using System.Collections.Generic;
using System.Reflection;
using Foundation;
using UIKit;
using MonoTouch.NUnit.UI;
using NUnit.Framework.Internal;
using MonoTouchFixtures.BackgroundTasks;

namespace MonoTouchFixtures {
	
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {
		// class-level declarations
		static UIWindow window;
		TouchRunner runner;

#if !__TVOS__
		public override bool AccessibilityPerformMagicTap ()
		{
			try {
				runner.OpenWriter ("Magic Tap");
				runner.Run (runner.LoadedTest as TestSuite);
			}
			finally {
				runner.CloseWriter ();
			}
			return true;
		}
#endif
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			runner = new TouchRunner (window);

			// tests can be inside the main assembly
			runner.Add (Assembly.GetExecutingAssembly ());
			runner.Add (typeof(EmbeddedResources.ResourcesTest).Assembly);
			runner.Add (typeof(Xamarin.BindingTests.ProtocolTest).Assembly);
			
			window.RootViewController = new UINavigationController (runner.GetViewController ());
			// make the window visible
			window.MakeKeyAndVisible ();


			// required for the background tasks tests, we can only register the tasks in this method
			BGTaskSchedulerTest.RegisterTestTasks ();
			return true;
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

		public static bool RunAsync (DateTime timeout, Action action, Func<bool> check_completed, UIImage imageToShow = null)
		{
			var vc = new AsyncController (action, imageToShow);
			var bckp = window.RootViewController;
			window.RootViewController = vc;
			try {
				do {
					if (timeout < DateTime.Now)
						return false;
					NSRunLoop.Main.RunUntil (NSDate.Now.AddSeconds (0.1));
				} while (!check_completed ());
			} finally {
				window.RootViewController = bckp;
			}

			return true;
		}
	}

	class AsyncController : UIViewController {
		Action action;
		UIImage imageToShow;
		static int counter;

		public AsyncController (Action action, UIImage imageToShow = null)
		{
			this.action = action;
			this.imageToShow = imageToShow;
			counter++;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			switch (counter % 2) {
			case 0:
				View.BackgroundColor = UIColor.Yellow;
				break;
			default:
				View.BackgroundColor = UIColor.LightGray;
				break;
			}
			if (imageToShow != null) {
				var imgView = new UIImageView (View.Bounds);
				imgView.Image = imageToShow;
				imgView.ContentMode = UIViewContentMode.Center;
				View.AddSubview (imgView);
			}
			NSTimer.CreateScheduledTimer (0.01, (v) => action ());
		}
	}
}

#endif // !__WATCHOS__
