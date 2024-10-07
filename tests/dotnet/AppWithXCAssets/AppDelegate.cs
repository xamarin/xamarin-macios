using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using CoreGraphics;
using Foundation;

#if !__MACOS__
using UIKit;
#endif

#nullable enable

namespace AppWithXCAssets {
#if !(__MACCATALYST__ || __MACOS__)
	public class AppDelegate : UIApplicationDelegate {
		UIWindow? window;
		UIButton? button;
		UIColor blue = UIColor.FromRGB (31, 174, 206);
		UIColor green = UIColor.FromRGB (119, 187, 65);

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			var dvc = new UIViewController ();
			var bounds = window.Bounds;
			button = new UIButton (window.Bounds);
			button.SetTitleColor (blue, UIControlState.Normal);
			button.SetTitle ("Switch icon!", UIControlState.Normal);
			dvc.Add (button);

			window.RootViewController = dvc;
			window.MakeKeyAndVisible ();

			ScheduleIconSwitching ();

			return true;
		}

		void ScheduleIconSwitching ()
		{
			NSTimer.CreateScheduledTimer (TimeSpan.FromSeconds (1), async (v) => {
				Console.WriteLine ($"Starting icon switching!");
				await SwitchIcon ();
			});
		}

		async Task SwitchIcon ()
		{
			await Task.Delay (1000); // wait a bit, otherwise it doesn't work

			var supportsAlternateIcons =  UIApplication.SharedApplication.SupportsAlternateIcons;
			if (!supportsAlternateIcons)
				Console.WriteLine ("Alternate icons aren't currently supported, but trying anyway!");

			string? name;
			UIColor color;
			if (!string.IsNullOrEmpty (UIApplication.SharedApplication.AlternateIconName)) {
				Console.WriteLine ($"Switching back to blue icon...");
				name = null; // switch back
				color = blue;
			} else {
				Console.WriteLine ($"Switching to alternate green icon...");
				name = "AlternateAppIcons"; // switch
				color = green;
			}

			UIApplication.SharedApplication.SetAlternateIconName (name, (err) => {
				if (err is null) {
					Console.WriteLine($"Switched to {(name is null ? "original icon" : $"alternate icon {name}")}");
					button!.SetTitleColor (color, UIControlState.Normal);
					ScheduleIconSwitching ();
				} else {
					Console.WriteLine ($"Failed to switch icon : {err}");
				}
			});
		}
	}
#endif

	public class Program {
		static int Main (string [] args)
		{
#if __MACCATALYST__ || __MACOS__
			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return args.Length;
#else
			UIApplication.Main (args, null, typeof (AppDelegate));
			return 0;
#endif
		}
	}
}
