using System;
using System.Runtime.InteropServices;

#nullable enable

namespace ObjCRuntime {
	// a short-lived holder for a C-like string for native interop
	// typical usage:
	// using var cstring = new NativeString (str);
	// SomePInvoke (cstring);
	//
	// If SomePInvoke doesn't make a copy, this is not the right tool
	// for you.
	internal struct TransientString : IDisposable {
		IntPtr ptr;
		public TransientString (string? str)
		{
			// the docs say when str is null the IntPtr will be 0
			ptr = Marshal.StringToHGlobalAuto (str);
		}

		public void Dispose ()
		{
			if (ptr != IntPtr.Zero) {
				Marshal.FreeHGlobal (ptr);
				ptr = IntPtr.Zero;
			}
		}

		public static implicit operator IntPtr (TransientString str) => str.ptr;
	}
}
