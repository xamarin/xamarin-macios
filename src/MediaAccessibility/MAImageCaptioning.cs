#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using CoreGraphics;
using CoreText;
using Foundation;

namespace MediaAccessibility {

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[TV (13, 0)]
	[iOS (13, 0)]
#endif
	public static partial class MAImageCaptioning {

		[DllImport (Constants.MediaAccessibilityLibrary)]
		// __attribute__((cf_returns_retained))
		unsafe static extern /* CFStringRef _Nullable */ IntPtr MAImageCaptioningCopyCaption (/* CFURLRef _Nonnull */ IntPtr url, /* CFErrorRef _Nullable * */ IntPtr* error);

		static public string? GetCaption (NSUrl url, out NSError? error)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			IntPtr result;
			IntPtr e;
			unsafe {
				result = MAImageCaptioningCopyCaption (url.Handle, &e);
			}
			error = e == IntPtr.Zero ? null : new NSError (e);
			return CFString.FromHandle (result, releaseHandle: true);
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		unsafe static extern byte MAImageCaptioningSetCaption (/* CFURLRef _Nonnull */ IntPtr url, /* CFStringRef _Nullable */ IntPtr @string, /* CFErrorRef _Nullable * */ IntPtr* error);

		static public bool SetCaption (NSUrl url, string @string, out NSError? error)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			var s = CFString.CreateNative (@string);
			try {
				bool result;
				IntPtr e;
				unsafe {
					result = MAImageCaptioningSetCaption (url.Handle, s, &e) != 0;
				}
				error = e == IntPtr.Zero ? null : new NSError (e);
				return result;
			} finally {
				NSString.ReleaseNative (s);
			}
		}

		[DllImport (Constants.MediaAccessibilityLibrary)]
		// __attribute__((cf_returns_retained))
		static extern /* CFStringRef _Nonnull */ IntPtr MAImageCaptioningCopyMetadataTagPath ();

		static public string? GetMetadataTagPath ()
		{
			return CFString.FromHandle (MAImageCaptioningCopyMetadataTagPath (), releaseHandle: true);
		}
	}
}
