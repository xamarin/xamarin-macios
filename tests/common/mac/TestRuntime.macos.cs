using System;

using AppKit;
using Foundation;

partial class TestRuntime {
	public static bool RunAsync (TimeSpan timeout, Action action, Func<bool> check_completed, NSImage imageToShow = null)
	{
		return RunAsync (DateTime.Now.Add (timeout), action, check_completed, imageToShow);
	}

	public static bool RunAsync (DateTime timeout, Action action, Func<bool> check_completed, NSImage imageToShow = null)
	{
		NSTimer.CreateScheduledTimer (0.01, (v) => action ());
		do {
			if (timeout < DateTime.Now)
				return false;
			NSRunLoop.Main.RunUntil (NSDate.Now.AddSeconds (0.1));
		} while (!check_completed ());

		return true;
	}
}
