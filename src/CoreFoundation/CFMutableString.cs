// Copyright 2019 Microsoft Corporation

#if !COREBUILD

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CFMutableString : CFString {

#if !NET
		protected CFMutableString (NativeHandle handle)
			: this (handle, false)
		{
		}
#endif
		
		[Preserve (Conditional = true)]
#if NET
		internal CFMutableString (NativeHandle handle, bool owns)
#else
		protected CFMutableString (NativeHandle handle, bool owns)
#endif
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern /* CFMutableStringRef* */ IntPtr CFStringCreateMutable (/* CFAllocatorRef* */ IntPtr alloc, nint maxLength);

		public CFMutableString (string @string = "", nint maxLength = default (nint))
		{
			// not really needed - but it's consistant with other .ctor
			if (@string == null)
				throw new ArgumentNullException (nameof (@string));
			// NSMallocException Out of memory. We suggest restarting the application. If you have an unsaved document, create a backup copy in Finder, then try to save.
			if (maxLength < 0)
				throw new ArgumentException (nameof (maxLength));
			Handle = CFStringCreateMutable (IntPtr.Zero, maxLength);
			if (@string != null)
				CFStringAppendCharacters (Handle, @string, @string.Length);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern /* CFMutableStringRef* */ IntPtr CFStringCreateMutableCopy (/* CFAllocatorRef* */ IntPtr alloc, nint maxLength, /* CFStringRef* */ IntPtr theString);

		public CFMutableString (CFString theString, nint maxLength = default (nint))
		{
			if (theString == null)
				throw new ArgumentNullException (nameof (theString));
			// NSMallocException Out of memory. We suggest restarting the application. If you have an unsaved document, create a backup copy in Finder, then try to save.
			if (maxLength < 0)
				throw new ArgumentException (nameof (maxLength));
			Handle = CFStringCreateMutableCopy (IntPtr.Zero, maxLength, theString.GetHandle ());
		}

		[DllImport (Constants.CoreFoundationLibrary, CharSet=CharSet.Unicode)]
		static extern void CFStringAppendCharacters (/* CFMutableStringRef* */ IntPtr theString, string chars, nint numChars);

		public void Append (string @string)
		{
			if (@string == null)
				throw new ArgumentNullException (nameof (@string));
			str = null; // destroy any cached value
			CFStringAppendCharacters (Handle, @string, @string.Length);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern internal bool /* Boolean */ CFStringTransform (/* CFMutableStringRef* */ IntPtr @string, /* CFRange* */ ref CFRange range, /* CFStringRef* */ IntPtr transform, [MarshalAs (UnmanagedType.I1)] /* Boolean */ bool reverse);

		public bool Transform (ref CFRange range, CFStringTransform transform, bool reverse)
		{
			return Transform (ref range, transform.GetConstant ().GetHandle (), reverse);
		}

		// constant documentation mention it also accept any ICT transform
		public bool Transform (ref CFRange range, CFString transform, bool reverse)
		{
			return Transform (ref range, transform.GetHandle (), reverse);
		}

		public bool Transform (ref CFRange range, NSString transform, bool reverse)
		{
			return Transform (ref range, transform.GetHandle (), reverse);
		}

		public bool Transform (ref CFRange range, string transform, bool reverse)
		{
			var t = NSString.CreateNative (transform);
			try {
				return Transform (ref range, t, reverse);
			} finally {
				NSString.ReleaseNative (t);
			}
		}

		bool Transform (ref CFRange range, IntPtr transform, bool reverse)
		{
			if (transform == IntPtr.Zero)
				throw new ArgumentNullException (nameof (transform));
			str = null; // destroy any cached value
			return CFStringTransform (Handle, ref range, transform, reverse);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern internal bool /* Boolean */ CFStringTransform (/* CFMutableStringRef* */ IntPtr @string, /* CFRange* */ IntPtr range, /* CFStringRef* */ IntPtr transform, [MarshalAs (UnmanagedType.I1)] /* Boolean */ bool reverse);

		public bool Transform (CFStringTransform transform, bool reverse)
		{
			return Transform (transform.GetConstant ().GetHandle (), reverse);
		}

		// constant documentation mention it also accept any ICT transform
		public bool Transform (CFString transform, bool reverse)
		{
			return Transform (transform.GetHandle (), reverse);
		}

		public bool Transform (NSString transform, bool reverse)
		{
			return Transform (transform.GetHandle (), reverse);
		}

		public bool Transform (string transform, bool reverse)
		{
			var t = NSString.CreateNative (transform);
			try {
				return Transform (t, reverse);
			} finally {
				NSString.ReleaseNative (t);
			}
		}

		bool Transform (IntPtr transform, bool reverse)
		{
			if (transform == IntPtr.Zero)
				throw new ArgumentNullException (nameof (transform));
			str = null; // destroy any cached value
			return CFStringTransform (Handle, IntPtr.Zero, transform, reverse);
		}
	}
}

#endif // !COREBUILD
