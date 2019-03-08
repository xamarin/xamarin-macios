using System;
#if __WATCHOS__
using Foundation;
#endif
using UIKit;

namespace mononativetests
{
	public class Application
	{
#if !__WATCHOS__
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
#endif // !__WATCHOS__
	}
}

partial class TestRuntime
{
	public static bool RunAsync (DateTime timeout, Action action, Func<bool> check_completed)
	{
#if __WATCHOS__
		NSTimer.CreateScheduledTimer (0.01, (v) => action ());
		do {
			if (timeout < DateTime.Now)
				return false;
			NSRunLoop.Main.RunUntil (NSDate.Now.AddSeconds (0.1));
		} while (!check_completed ());

		return true;
#else
		return Mono.Native.Tests.AppDelegate.RunAsync (timeout, action, check_completed);
#endif
	}
}
