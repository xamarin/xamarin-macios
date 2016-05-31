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

class TestRuntime
{
	[DllImport ("/usr/lib/system/libdyld.dylib")]
	static extern int dyld_get_program_sdk_version ();

	public const string BuildVersion_iOS9_GM = "13A340";

	public static string GetiOSBuildVersion ()
	{
#if __WATCHOS__
		throw new Exception ("Can't get iOS Build version on watchOS.");
#else
		return NSString.FromHandle (Messaging.IntPtr_objc_msgSend (UIDevice.CurrentDevice.Handle, Selector.GetHandle ("buildVersion")));
#endif
	}

	public static Version GetSDKVersion ()
	{
		var v = dyld_get_program_sdk_version ();
		var major = v >> 16;
		var minor = (v >> 8) & 0xFF;
		var build = v & 0xFF;
		return new Version (major, minor, build);
	}

	public static void AssertXcodeVersion (int major, int minor)
	{
		if (CheckXcodeVersion (major, minor))
			return;

		NUnit.Framework.Assert.Ignore ("Requires the platform version shipped with Xcode {0}.{1}", major, minor);
	}

	public static bool CheckXcodeVersion (int major, int minor)
	{
		switch (major) {
		case 7:
			switch (minor) {
			case 0:
#if __WATCHOS__
				return CheckWatchOSSystemVersion (2, 0);
#elif __TVOS__
				return ChecktvOSSystemVersion (9, 0);
#elif __IOS__
				return CheckiOSSystemVersion (9, 0);
#else
				throw new NotImplementedException ();
#endif
			case 1:
#if __WATCHOS__
				return CheckWatchOSSystemVersion (2, 0);
#elif __TVOS__
				return ChecktvOSSystemVersion (9, 0);
#elif __IOS__
				return CheckiOSSystemVersion (9, 1);
#else
				throw new NotImplementedException ();
#endif
			case 2:
#if __WATCHOS__
				return CheckWatchOSSystemVersion (2, 1);
#elif __TVOS__
				return ChecktvOSSystemVersion (9, 1);
#elif __IOS__
				return CheckiOSSystemVersion (9, 2);
#else
				throw new NotImplementedException ();
#endif
			case 3:
#if __WATCHOS__
				return CheckWatchOSSystemVersion (2, 2);
#elif __TVOS__
				return ChecktvOSSystemVersion (9, 2);
#elif __IOS__
				return CheckiOSSystemVersion (9, 3);
#else
				throw new NotImplementedException ();
#endif
			default:
				throw new NotImplementedException ();
			}
		case 6:
#if __IOS__
			// nothing
#elif __TVOS__ || __WATCHOS__
			return true;
#else
			throw new NotImplementedException ();
#endif
			switch (minor) {
			case 0:
				return CheckiOSSystemVersion (8, 0);
			case 1:
				return CheckiOSSystemVersion (8, 1);
			case 2:
				return CheckiOSSystemVersion (8, 2);
			case 3:
				return CheckiOSSystemVersion (8, 3);
			default:
				throw new NotImplementedException ();
			}
		case 5:
#if __IOS__
			// nothing
#elif __TVOS__ || __WATCHOS__
			return true;
#else
			throw new NotImplementedException ();
#endif
			switch (minor) {
			case 0:
				return CheckiOSSystemVersion (7, 0);
			case 1:
				return CheckiOSSystemVersion (7, 1);
			default:
				throw new NotImplementedException ();
			}
		case 4:
#if __IOS__
			// nothing
#elif __TVOS__ || __WATCHOS__
			return true;
#else
			throw new NotImplementedException ();
#endif
			switch (minor) {
			case 5:
				return CheckiOSSystemVersion (6, 0);
			default:
				throw new NotImplementedException ();
			}
		default:
			throw new NotImplementedException ();
		}
	}

	// This method returns true if:
	// system version >= specified version
	// AND
	// sdk version >= specified version
	public static bool CheckiOSSystemVersion (int major, int minor, bool throwIfOtherPlatform = true)
	{
#if __IOS__
		return UIDevice.CurrentDevice.CheckSystemVersion (major, minor);
#else
		if (throwIfOtherPlatform)
			throw new Exception ("Can't get iOS System version on other platforms.");
		return true;
#endif
	}

	// This method returns true if:
	// system version >= specified version
	// AND
	// sdk version >= specified version
	public static bool ChecktvOSSystemVersion (int major, int minor, bool throwIfOtherPlatform = true)
	{
#if __TVOS__
		return UIDevice.CurrentDevice.CheckSystemVersion (major, minor);
#else
		if (throwIfOtherPlatform)
			throw new Exception ("Can't get tvOS System version on other platforms.");
		return true;
#endif
	}

	// This method returns true if:
	// system version >= specified version
	// AND
	// sdk version >= specified version
	public static bool CheckWatchOSSystemVersion (int major, int minor, bool throwIfOtherPlatform = true)
	{
#if __WATCHOS__
		return WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (major, minor);
#else
		if (throwIfOtherPlatform)
			throw new Exception ("Can't get watchOS System version on iOS/tvOS.");
		// This is both iOS and tvOS
		return true;
#endif
	}

	// This method returns true if:
	// system version >= specified version
	// AND
	// sdk version >= specified version
	public static bool CheckSystemAndSDKVersion (int major, int minor)
	{
#if __WATCHOS__
		throw new Exception ("Can't get iOS System/SDK version on WatchOS.");
#else
		if (!UIDevice.CurrentDevice.CheckSystemVersion (major, minor))
			return false;
#endif

		// Check if the SDK version we're built includes the version we're checking for
		// We don't want to execute iOS7 tests on an iOS7 device when built with the iOS6 SDK.
		return CheckSDKVersion (major, minor);
	}

	public static bool CheckSDKVersion (int major, int minor)
	{
#if __WATCHOS__
		throw new Exception ("Can't get iOS SDK version on WatchOS.");
#else
		if (Runtime.Arch == Arch.SIMULATOR || !UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {
			// dyld_get_program_sdk_version was introduced with iOS 6.0, so don't do the SDK check on older deviecs.
			return true; // dyld_get_program_sdk_version doesn't return what we're looking for on the mac.
		}
#endif

		var sdk = GetSDKVersion ();
		if (sdk.Major > major)
			return true;
		if (sdk.Major == major && sdk.Minor >= minor)
			return true;
		return false;
	}

	public static void IgnoreOnTVOS ()
	{
#if __TVOS__
		NUnit.Framework.Assert.Ignore ("This test is disabled on TVOS.");
#endif
	}

	public static bool IsTVOS {
		get {
#if __TVOS__
			return true;
#else
			return false;
#endif
		}
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
