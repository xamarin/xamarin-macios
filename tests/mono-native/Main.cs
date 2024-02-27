using System;
#if __WATCHOS__
using Foundation;
#endif
using UIKit;

partial class TestRuntime {
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
