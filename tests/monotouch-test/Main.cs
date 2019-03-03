#if !MONOMAC
using System;
using System.Collections.Generic;
using System.Linq;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
#else
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
using System.Runtime.InteropServices;

namespace monotouchtest
{
	public class Application
	{
#if !__WATCHOS__
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// Make sure we have at least one reference to the bindings project so that mcs doesn't strip the reference to it.
			GC.KeepAlive (typeof(Bindings.Test.UltimateMachine));
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
#endif // !__WATCHOS__

	}
}

partial class TestRuntime
{
	public static bool RunAsync (TimeSpan timeout, Action action, Func<bool> check_completed)
	{
		return RunAsync (DateTime.Now.Add (timeout), action, check_completed);
	}

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
		return MonoTouchFixtures.AppDelegate.RunAsync (timeout, action, check_completed);
#endif
	}
}

// This prevents the need for putting lots of #ifdefs inside the list of usings.
#if __WATCHOS__
namespace System.Drawing {}
namespace OpenTK {}
#endif
#endif