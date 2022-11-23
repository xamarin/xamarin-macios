#if !__WATCHOS__
using System;
using System.Collections.Generic;
using System.Reflection;

using Foundation;
#if !__MACOS__
using UIKit;
#endif

using MonoTouch.NUnit.UI;
using NUnit.Framework;
using NUnit.Framework.Internal;

[Register ("AppDelegate")]
public partial class AppDelegate : UIApplicationDelegate {
	public static TouchRunner Runner { get; set; }

#if !__MACOS__
	public override UIWindow Window { get; set; }
#endif

	public partial IEnumerable<Assembly> GetTestAssemblies ();

	partial void PostFinishedLaunching ();

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
#if __MACCATALYST__
		// Debug spew to track down https://github.com/xamarin/maccore/issues/2414
		Console.WriteLine ("AppDelegate.FinishedLaunching");
#endif
		var window = new UIWindow (UIScreen.MainScreen.Bounds);

		var runner = new TouchRunner (window);
		foreach (var assembly in GetTestAssemblies ())
			runner.Add (assembly);

		Window = window;
		Runner = runner;

		window.RootViewController = new UINavigationController (runner.GetViewController ());
		window.MakeKeyAndVisible ();

		PostFinishedLaunching ();

		return true;
	}
}

public static class MainClass {
	static void Main (string [] args)
	{
#if !__MACOS__
		UIApplication.Main (args, null, typeof (AppDelegate));
#endif
	}
}
#endif // !__WATCHOS__
