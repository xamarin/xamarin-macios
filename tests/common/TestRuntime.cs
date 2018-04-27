#if __MACOS__
#define MONOMAC
#endif
#if __UNIFIED__
#define XAMCORE_2_0
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

#if XAMCORE_2_0
using AVFoundation;
using Foundation;
#if !__TVOS__
using Contacts;
#endif
#if MONOMAC
using AppKit;
#else
#if !__TVOS__ && !__WATCHOS__
using AddressBook;
#endif
#if !__WATCHOS__
using MediaPlayer;
#endif
using UIKit;
#endif
using ObjCRuntime;
#else
#if MONOMAC
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.AVFoundation;
#else
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
#endif

partial class TestRuntime
{
	[DllImport ("/usr/lib/system/libdyld.dylib")]
	static extern int dyld_get_program_sdk_version ();

	[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
	static extern IntPtr IntPtr_objc_msgSend (IntPtr receiver, IntPtr selector);

	public const string BuildVersion_iOS9_GM = "13A340";

	public static string GetiOSBuildVersion ()
	{
#if __WATCHOS__
		throw new Exception ("Can't get iOS Build version on watchOS.");
#elif MONOMAC
		throw new Exception ("Can't get iOS Build version on OSX.");
#else
		return NSString.FromHandle (IntPtr_objc_msgSend (UIDevice.CurrentDevice.Handle, Selector.GetHandle ("buildVersion")));
#endif
	}

#if MONOMAC
	const int sys1 = 1937339185;
	const int sys2 = 1937339186;
	const int sys3 = 1937339187;

	// Deprecated in OSX 10.8 - but no good alternative is (yet) available
	[System.Runtime.InteropServices.DllImport ("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	static extern int Gestalt (int selector, out int result);

	static Version version;

	public static Version OSXVersion {
		get {
			if (version == null) {
				int major, minor, build;
				Gestalt (sys1, out major);
				Gestalt (sys2, out minor);
				Gestalt (sys3, out build);
				version = new Version (major, minor, build);
			}
			return version;
		}
	}
#endif

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

	public static void AssertDevice ()
	{
#if !MONOMAC
		if (ObjCRuntime.Runtime.Arch == Arch.SIMULATOR)
			NUnit.Framework.Assert.Ignore ("This test only runs on device.");
#endif
	}

	// This function checks if the current Xcode version is exactly (neither higher nor lower) the requested one.
	public static bool CheckExactXcodeVersion (int major, int minor, int beta = 0)
	{
		// Add the Build number minus the one last character, sometimes Apple releases
		// different builds from the same Beta, for example in Xcode 9 Beta 3 we have
		// 15A5318g on device and 15A5318e on the simulator
		var nineb1 = new {
			Xcode = new { Major = 9, Minor = 0, Beta = 1 },
			iOS = new { Major = 11, Minor = 0, Build = "15A5278" },
			tvOS = new { Major = 11, Minor = 0, Build = "?" },
			macOS = new { Major = 10, Minor = 13, Build = "?" },
			watchOS = new { Major = 4, Minor = 0, Build = "?" },
		};
		var nineb2 = new {
			Xcode = new { Major = 9, Minor = 0, Beta = 2 },
			iOS = new { Major = 11, Minor = 0, Build = "15A5304" },
			tvOS = new { Major = 11, Minor = 0, Build = "?" },
			macOS = new { Major = 10, Minor = 13, Build = "?" },
			watchOS = new { Major = 4, Minor = 0, Build = "?" },
		};
		var nineb3 = new {
			Xcode = new { Major = 9, Minor = 0, Beta = 3 },
			iOS = new { Major = 11, Minor = 0, Build = "15A5318" },
			tvOS = new { Major = 11, Minor = 0, Build = "?" },
			macOS = new { Major = 10, Minor = 13, Build = "?" },
			watchOS = new { Major = 4, Minor = 0, Build = "?" },
		};

		var versions = new [] {
			nineb1,
			nineb2,
			nineb3,
		};

		foreach (var v in versions) {
			if (v.Xcode.Major != major)
				continue;
			if (v.Xcode.Minor != minor)
				continue;
			if (v.Xcode.Beta != beta)
				continue;

#if __IOS__
			if (!CheckExactiOSSystemVersion (v.iOS.Major, v.iOS.Minor))
				return false;
			if (v.iOS.Build == "?")
				throw new NotImplementedException ($"Build number for iOS {v.iOS.Major}.{v.iOS.Minor} beta {beta} (candidate: {GetiOSBuildVersion ()})");
			var actual = GetiOSBuildVersion ();
			Console.WriteLine (actual);
			return actual.StartsWith (v.iOS.Build, StringComparison.Ordinal);
#else
			throw new NotImplementedException ();
#endif
		}

		throw new NotImplementedException ($"Build information for Xcode version {major}.{minor} beta {beta} not found");
	}

	public static bool CheckXcodeVersion (int major, int minor, int build = 0)
	{
		switch (major) {
		case 9:
			switch (minor) {
			case 0:
#if __WATCHOS__
				return CheckWatchOSSystemVersion (4, 0);
#elif __TVOS__
				return ChecktvOSSystemVersion (11, 0);
#elif __IOS__
				return CheckiOSSystemVersion (11, 0);
#elif MONOMAC
				return CheckMacSystemVersion (10, 13, 0);
#else
				throw new NotImplementedException ();
				#endif
			case 3:
#if __WATCHOS__
				return CheckWatchOSSystemVersion (4, 3);
#elif __TVOS__
				return ChecktvOSSystemVersion (11, 3);
#elif __IOS__
				return CheckiOSSystemVersion (11, 3);
#elif MONOMAC
				return CheckMacSystemVersion (10, 13, 4);
#else
				throw new NotImplementedException ();
#endif
			default:
				throw new NotImplementedException ();
			}
		case 8:
			switch (minor) {
			case 0:
#if __WATCHOS__
				return CheckWatchOSSystemVersion (3, 0);
#elif __TVOS__
				return ChecktvOSSystemVersion (10, 0);
#elif __IOS__
				return CheckiOSSystemVersion (10, 0);
#elif MONOMAC
				return CheckMacSystemVersion (10, 12, 0);
#else
				throw new NotImplementedException ();
#endif
			case 1:
#if __WATCHOS__
				return CheckWatchOSSystemVersion (3, 1);
#elif __TVOS__
				return ChecktvOSSystemVersion (10, 0);
#elif __IOS__
				return CheckiOSSystemVersion (10, 1);
#elif MONOMAC
				return CheckMacSystemVersion (10, 12, 1);
#else
				throw new NotImplementedException ();
#endif
			case 2:
#if __WATCHOS__
				return CheckWatchOSSystemVersion (3, 1);
#elif __TVOS__
				return ChecktvOSSystemVersion (10, 1);
#elif __IOS__
				return CheckiOSSystemVersion (10, 2);
#elif MONOMAC
				return CheckMacSystemVersion (10, 12, 2);
#else
				throw new NotImplementedException ();
#endif
			case 3:
#if __WATCHOS__
				return CheckWatchOSSystemVersion (3, 2);
#elif __TVOS__
				return ChecktvOSSystemVersion (10, 2);
#elif __IOS__
				return CheckiOSSystemVersion (10, 3);
#elif MONOMAC
				return CheckMacSystemVersion (10, 12, 4);
#else
				throw new NotImplementedException ();
#endif
			default:
				throw new NotImplementedException ();
			}
		case 7:
			switch (minor) {
			case 0:
#if __WATCHOS__
				return CheckWatchOSSystemVersion (2, 0);
#elif __TVOS__
				return ChecktvOSSystemVersion (9, 0);
#elif __IOS__
				return CheckiOSSystemVersion (9, 0);
#elif MONOMAC
				return CheckMacSystemVersion (10, 11, 0);
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
#elif MONOMAC
				return CheckMacSystemVersion (10, 11, 0 /* yep */);
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
#elif MONOMAC
				return CheckMacSystemVersion (10, 11, 2);
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
#elif MONOMAC
				return CheckMacSystemVersion (10, 11, 4);
#else
				throw new NotImplementedException ();
#endif
			default:
				throw new NotImplementedException ();
			}
		case 6:
#if __IOS__
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
#elif __TVOS__ || __WATCHOS__
			return true;
#elif MONOMAC
			switch (minor) {
			case 0:
				return CheckMacSystemVersion (10, 9, 0);
			case 1:
				return CheckMacSystemVersion (10, 10, 0);
			case 2:
				return CheckMacSystemVersion (10, 10, 0);
			case 3:
				return CheckMacSystemVersion (10, 10, 0);
			default:
				throw new NotImplementedException ();
			}
#else
			throw new NotImplementedException ();
#endif
		case 5:
#if __IOS__
			switch (minor) {
			case 0:
				return CheckiOSSystemVersion (7, 0);
			case 1:
				return CheckiOSSystemVersion (7, 1);
			default:
				throw new NotImplementedException ();
			}
#elif __TVOS__ || __WATCHOS__
			return true;
#elif MONOMAC
			switch (minor) {
			case 0:
				// Xcode 5.0.1 ships OSX 10.9 SDK
				return CheckMacSystemVersion (10, build > 0 ? 9 : 8, 0);
			case 1:
				return CheckMacSystemVersion (10, 9, 0);
			default:
				throw new NotImplementedException ();
			}
#else
			throw new NotImplementedException ();
#endif
		case 4:
#if __IOS__
			switch (minor) {
			case 5:
				return CheckiOSSystemVersion (6, 0);
			case 6:
				return CheckiOSSystemVersion (6, 1);
			default:
				throw new NotImplementedException ();
			}
#elif __TVOS__ || __WATCHOS__
			return true;
#elif MONOMAC
			switch (minor) {
			case 5:
			case 6:
				return CheckMacSystemVersion (10, 8, 0);
			default:
				throw new NotImplementedException ();
			}
#else
			throw new NotImplementedException ();
#endif
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

	public static bool CheckExactiOSSystemVersion (int major, int minor)
	{
#if __IOS__
		var version = Version.Parse (UIDevice.CurrentDevice.SystemVersion);
		return version.Major == major && version.Minor == minor;
#else
		throw new Exception ("Can't get iOS System version on other platforms.");
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

	public static bool CheckMacSystemVersion (int major, int minor, int build = 0, bool throwIfOtherPlatform = true)
	{
#if MONOMAC
		return OSXVersion >= new Version (major, minor, build);
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
	public static bool CheckSystemAndSDKVersion (int major, int minor)
	{
#if __WATCHOS__
		throw new Exception ("Can't get iOS System/SDK version on WatchOS.");
#elif MONOMAC
		if (OSXVersion < new Version (major, minor))
			return false;
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
#elif !MONOMAC
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

	public static bool IgnoreTestThatRequiresSystemPermissions ()
	{
		return !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("DISABLE_SYSTEM_PERMISSION_TESTS"));
	}

#if !MONOMAC && !__TVOS__ && !__WATCHOS__
	public static void RequestCameraPermission (NSString mediaTypeToken, bool assert_granted = false)
	{
		if (AVCaptureDevice.GetAuthorizationStatus (mediaTypeToken) == AVAuthorizationStatus.NotDetermined) {
			if (IgnoreTestThatRequiresSystemPermissions ())
				NUnit.Framework.Assert.Ignore ("This test would show a dialog to ask for permission to access the camera.");

			AVCaptureDevice.RequestAccessForMediaType (mediaTypeToken, (accessGranted) =>
			{
				Console.WriteLine ("Camera permission {0}", accessGranted ? "granted" : "denied");
			});
		}

		switch (AVCaptureDevice.GetAuthorizationStatus (AVMediaType.Video)) {
		case AVAuthorizationStatus.Restricted:
		case AVAuthorizationStatus.Denied:
			if (assert_granted)
				NUnit.Framework.Assert.Fail ("This test requires permission to access the camera.");
			break;
		}
	}
#endif // !!MONOMAC && !__TVOS__ && !__WATCHOS__

#if XAMCORE_2_0 && !__TVOS__
	public static void CheckContactsPermission (bool assert_granted = false)
	{
		switch (CNContactStore.GetAuthorizationStatus (CNEntityType.Contacts)) {
		case CNAuthorizationStatus.NotDetermined:
			if (IgnoreTestThatRequiresSystemPermissions ())
				NUnit.Framework.Assert.Ignore ("This test would show a dialog to ask for permission to access the contacts.");
			// We don't request access here, because there's no global method to request access (an contact store instance is required).
			// Interestingly there is a global method to determine if access has been granted...
			break;
		case CNAuthorizationStatus.Restricted:
		case CNAuthorizationStatus.Denied:
			if (assert_granted)
				NUnit.Framework.Assert.Fail ("This test requires permission to access the contacts.");
			break;
		}
	}
#endif // XAMCORE_2_0

#if !MONOMAC && !__TVOS__ && !__WATCHOS__
	public static void CheckAddressBookPermission (bool assert_granted = false)
	{
		switch (ABAddressBook.GetAuthorizationStatus ()) {
		case ABAuthorizationStatus.NotDetermined:
			if (IgnoreTestThatRequiresSystemPermissions ())
				NUnit.Framework.Assert.Ignore ("This test would show a dialog to ask for permission to access the address book.");
			// We don't request access here, because there's no global method to request access (an addressbook instance is required).
			// Interestingly there is a global method to determine if access has been granted...
			break;
		case ABAuthorizationStatus.Restricted:
		case ABAuthorizationStatus.Denied:
			if (assert_granted)
				NUnit.Framework.Assert.Fail ("This test requires permission to access the address book.");
			break;
		}
	}
#endif // !MONOMAC && !__TVOS__ && !__WATCHOS__

#if !__WATCHOS__
	public static void RequestMicrophonePermission (bool assert_granted = false)
	{
#if MONOMAC
		// It looks like macOS does not restrict access to the microphone.
#elif __TVOS__
		// tvOS doesn't have a (developer-accessible) microphone, but it seems to have API that requires developers 
		// to request microphone access on other platforms (which means that it makes sense to both run those tests
		// on tvOS (because the API's there) and to request microphone access (because that's required on other platforms).
#else
		if (!CheckXcodeVersion (6, 0))
			return; // The API to check/request permission isn't available in earlier versions, the dialog will just pop up.

		if (AVAudioSession.SharedInstance ().RecordPermission == AVAudioSessionRecordPermission.Undetermined) {
			if (IgnoreTestThatRequiresSystemPermissions ())
				NUnit.Framework.Assert.Ignore ("This test would show a dialog to ask for permission to access the microphone.");

			AVAudioSession.SharedInstance ().RequestRecordPermission ((bool granted) =>
			{
				Console.WriteLine ("Microphone permission {0}", granted ? "granted" : "denied");
			});
		}

		switch (AVAudioSession.SharedInstance ().RecordPermission) { // iOS 8+
		case AVAudioSessionRecordPermission.Denied:
			if (assert_granted)
				NUnit.Framework.Assert.Fail ("This test requires permission to access the microphone.");
			break;
		}
#endif // !MONOMAC && !__TVOS__
	}
#endif // !__WATCHOS__

#if !MONOMAC && !__TVOS__ && !__WATCHOS__
	public static void RequestMediaLibraryPermission (bool assert_granted = false)
	{
		if (!CheckXcodeVersion (7, 3)) {
			if (IgnoreTestThatRequiresSystemPermissions ())
				NUnit.Framework.Assert.Ignore ("This test might show a dialog to ask for permission to access the media library, but the API to check if a dialog is required (or to request permission) is not available in this OS version.");
			return;
		}

		if (MPMediaLibrary.AuthorizationStatus == MPMediaLibraryAuthorizationStatus.NotDetermined) {
			if (IgnoreTestThatRequiresSystemPermissions ())
				NUnit.Framework.Assert.Ignore ("This test would show a dialog to ask for permission to access the media library.");

			MPMediaLibrary.RequestAuthorization ((access) =>
			{
				Console.WriteLine ("Media library permission: {0}", access);
			});
		}

		switch (MPMediaLibrary.AuthorizationStatus) {
		case MPMediaLibraryAuthorizationStatus.Denied:
		case MPMediaLibraryAuthorizationStatus.Restricted:
			if (assert_granted)
				NUnit.Framework.Assert.Fail ("This test requires permission to access the media library.");
			break;
		}
	}
#endif // !MONOMAC && !__TVOS__

	// Determine if linkall was enabled by checking if an unused class in this assembly is still here.
	static bool? link_all;
	public static bool IsLinkAll {
		get {
			if (!link_all.HasValue)
				link_all = typeof (TestRuntime).Assembly.GetType (typeof (TestRuntime).FullName + "+LinkerSentinel") == null;
			return link_all.Value;
		}
	}
	class LinkerSentinel { }
}
