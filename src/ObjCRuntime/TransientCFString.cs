using System;
using System.Runtime.InteropServices;

using CoreFoundation;

#nullable enable

namespace ObjCRuntime {
	// a short-lived holder for a CFString/NSString-like string for native interop
	// typical usage:
	// using var cstring = new NativeCFString (str);
	// SomePInvoke (cstring);
	//
	internal ref struct TransientCFString {
#if !COREBUILD
		IntPtr ptr;

		public TransientCFString (string? str)
		{
			ptr = CFString.CreateNative (str);
		}

		public void Dispose ()
		{
			if (ptr != IntPtr.Zero) {
				CFString.ReleaseNative (ptr);
				ptr = IntPtr.Zero;
			}
		}

		public static implicit operator IntPtr (TransientCFString str) => str.ptr;
		public static explicit operator string? (TransientCFString str) => CFString.FromHandle (str.ptr);
#endif
	}
}
