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
		public enum Encoding {
			Auto = 0,
			BStr,
			Ansi, // aka LPStr
			Unicode,
		};


		public TransientString (string? str, Encoding encoding = Encoding.Auto)
		{
			ptr = (Encode (encoding)) (str);
		}

		public void Dispose ()
		{
			if (ptr != IntPtr.Zero) {
				Marshal.FreeHGlobal (ptr);
				ptr = IntPtr.Zero;
			}
		}

		static Func<string?, IntPtr> Encode (Encoding encoding) => encoding switch {
			Encoding.Auto => Marshal.StringToHGlobalAuto,
			Encoding.BStr => Marshal.StringToBSTR,
			Encoding.Ansi => Marshal.StringToHGlobalAnsi,
			Encoding.Unicode => Marshal.StringToHGlobalUni,
			_ => throw new ArgumentOutOfRangeException (nameof (encoding))
		};


		public static implicit operator IntPtr (TransientString str) => str.ptr;
	}
}
