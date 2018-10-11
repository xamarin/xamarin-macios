using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
#endif

namespace BCL.Tests
{
#if !__WATCHOS__
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
#endif

	public class TestRuntime
	{
		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		static extern IntPtr IntPtr_objc_msgSend (IntPtr receiver, IntPtr selector);
			
		[DllImport ("/usr/lib/system/libdyld.dylib")]
		static extern int dyld_get_program_sdk_version ();

		public const string BuildVersion_iOS7_DP3 = "11D5134c";
		public const string BuildVersion_iOS8_Beta1 = "12A4265u";

		public static string GetiOSBuildVersion ()
		{
#if __WATCHOS__
			throw new Exception ("Can't get iOS build version on watchOS");
#else
			return NSString.FromHandle (IntPtr_objc_msgSend (UIDevice.CurrentDevice.Handle, Selector.GetHandle ("buildVersion")));
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

		public static bool CheckSystemVersion (int major, int minor)
		{
#if __WATCHOS__
			throw new Exception ("Can't get iOS System/SDK version on watchOS");
#else
			return UIDevice.CurrentDevice.CheckSystemVersion (major, minor);
#endif
		}

		public static bool CheckSDKVersion (int major, int minor)
		{
#if __WATCHOS__
			throw new Exception ("Can't get iOS SDK version on watchOS");
#else
			if (Runtime.Arch == Arch.SIMULATOR || !CheckSystemVersion (6, 0)) {
				// dyld_get_program_sdk_version was introduced with iOS 6.0, so don't do the SDK check on older deviecs.
				return true; // dyld_get_program_sdk_version doesn't return what we're looking for on the mac.
			}

			var sdk = GetSDKVersion ();
			if (sdk.Major > major)
				return true;
			if (sdk.Major == major && sdk.Minor >= minor)
				return true;
			return false;
#endif
		}
	}
}
