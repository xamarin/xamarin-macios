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

	// This method returns true if:
	// system version >= specified version
	// AND
	// sdk version >= specified version
	public static bool CheckiOSSystemVersion (int major, int minor)
	{
#if __WATCHOS__
		throw new Exception ("Can't get iOS System version on WatchOS.");
#else
		return UIDevice.CurrentDevice.CheckSystemVersion (major, minor);
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
		throw new NotImplementedException ("TestRuntime.RunAsync");
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
