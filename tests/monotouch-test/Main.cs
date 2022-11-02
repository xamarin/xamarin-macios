#if !MONOMAC
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using ObjCRuntime;
using System.Runtime.InteropServices;

partial class TestRuntime
{
	public static bool RunAsync (TimeSpan timeout, Action action, Func<bool> check_completed, UIImage imageToShow = null)
	{
		return RunAsync (DateTime.Now.Add (timeout), action, check_completed, imageToShow);
	}

	public static bool RunAsync (DateTime timeout, Action action, Func<bool> check_completed, UIImage imageToShow = null)
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
		return MonoTouchFixtures.AppDelegate.RunAsync (timeout, action, check_completed, imageToShow);
#endif
	}
}

// This prevents the need for putting lots of #ifdefs inside the list of usings.
#if __WATCHOS__
namespace System.Drawing {}
namespace OpenTK {}
#endif
#endif
