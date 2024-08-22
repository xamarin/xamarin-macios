#if NET
#if __MACOS__ || __MACCATALYST__

using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace CoreGraphics {

	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
	public static class CGSession {
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CGSessionCopyCurrentDictionary ();

		/// <summary>Get the properties for the current window server session.</summary>
		/// <returns>The properties for the current window server session.</returns>
		/// <remarks>Returns null if the caller is not running in a Quartz GUI session or the window server is disabled.</remarks>
		public static CGSessionProperties? GetProperties ()
		{
			var handle = CGSessionCopyCurrentDictionary ();
			if (handle == IntPtr.Zero)
				return null;
			var dict = Runtime.GetNSObject<NSDictionary> (handle, owns: true);
			return new CGSessionProperties (dict);
		}
	}

	// This looks like it should be in an api definition bound using [Field] attributes,
	// but these aren't actual native fields, in the headers they're declared as constants:
	//     #define kCGSession*Key @"SomeStringValue"
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
	public static class CGSessionKeys {
		/// <summary>A key for the user ID for the session's current user.</summary>
		public static NSString UserIdKey { get => (NSString) "kCGSSessionUserIDKey"; }

		/// <summary>A key for the short user name for the session's current user.</summary>
		public static NSString UserNameKey { get => (NSString) "kCGSSessionUserNameKey"; }

		/// <summary>A key for the set of hardware composing a console for the session's current user.</summary>
		public static NSString ConsoleSetKey { get => (NSString) "kCGSSessionConsoleSetKey"; }

		/// <summary>A key for indicating whether the session's on a console.</summary>
		public static NSString OnConsoleKey { get => (NSString) "kCGSSessionOnConsoleKey"; }

		/// <summary>A key for indicating whether the session's login operation has been don</summary>
		public static NSString LoginDoneKey { get => (NSString) "kCGSessionLoginDoneKey"; }
	}
}

#endif // __MACOS__ || __MACCATALYST__
#endif // NET
