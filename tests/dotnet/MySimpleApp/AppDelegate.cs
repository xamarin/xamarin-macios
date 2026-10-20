using System;
using System.Runtime.InteropServices;

using Foundation;

namespace MySimpleApp {
	public class Program {
		static int Main (string [] args)
		{
			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

#if USE_SWIFT_REVIEW || USE_SWIFT_WIDGETKIT
#if __MACOS__
			GC.KeepAlive ((Action<AppKit.NSViewController>) StoreKit.AppStore.RequestReview);
#else
			GC.KeepAlive ((Action<UIKit.UIWindowScene>) StoreKit.AppStore.RequestReview);
#endif
#endif
#if USE_SWIFT_WIDGETKIT
			WidgetKit.WidgetCenter.ReloadTimelines ("com.xamarin.mysimpleapp.widget");
			WidgetKit.WidgetCenter.ReloadAllTimelines ();
#endif

			if (Environment.GetEnvironmentVariable ("CRASH_ON_LAUNCH") == "1")
				Environment.FailFast ("Crashing on launch as requested by the CRASH_ON_LAUNCH environment variable.");

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return args.Length;
		}
	}
}
