using System;

#if XAMCORE_2_0
using UIKit;
#else
using MonoTouch.UIKit;
#endif

namespace LinkAll
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
