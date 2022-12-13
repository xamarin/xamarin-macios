using System;
using System.Runtime.InteropServices;

namespace ObjCRuntime {
	internal struct NativeString : IDisposable {
		IntPtr ptr;
		public NativeString (string str)
		{
			ptr = Marshal.StringToHGlobalAuto (str);
		}

		public void Dispose ()
		{
			if (ptr != IntPtr.Zero) {
				Marshal.FreeHGlobal (ptr);
				ptr = IntPtr.Zero;
			}
		}

		public static implicit operator IntPtr (NativeString str) => str.ptr;
	}
}
