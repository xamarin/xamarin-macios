using System;
using System.Runtime.InteropServices;
using System.Net.Http;

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
			WidgetKit.WidgetCenter.ReloadTimelines ("com.xamarin.apitestapp.widget");
			WidgetKit.WidgetCenter.ReloadAllTimelines ();
#endif

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));
#if HttpClientHandlerFeatureTrimmedAway
			var client = new System.Net.Http.HttpClient ();
			client.GetStringAsync ("https://microsoft.com");
#elif KEEP_LARIGHT
			if (OperatingSystem.IsIOSVersionAtLeast (17))
				GC.KeepAlive (typeof (LocalAuthentication.LARight));
#elif KEEP_LAPERSISTEDRIGHT
			if (OperatingSystem.IsIOSVersionAtLeast (17))
				GC.KeepAlive (typeof (LocalAuthentication.LAPersistedRight));
#elif KEEP_LACONTEXT
			using (var context = new LocalAuthentication.LAContext ())
				Console.WriteLine (context.BiometryType);
#endif

			return args.Length;
		}
	}
}
