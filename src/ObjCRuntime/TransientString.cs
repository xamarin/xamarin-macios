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
	//
	// It can also allocate a chunk of memory to be used as a parameter
	// for a method that will write a string to a pointer:
	// using var outstr = new NativeString (255); // 255 bytes of memory
	// SomePInvoke (outstr);
	// var str = (string) outstr; // convert the returned native string
	// to a managed string.
	internal struct TransientString : IDisposable {
		IntPtr ptr;
		public enum Encoding {
			Auto = 0,
			BStr,
			Ansi, // aka LPStr
			Unicode,
		};

		public TransientString (nint size)
		{
			ptr = Marshal.AllocHGlobal ((IntPtr) size);
		}

		public TransientString (string? str, Encoding encoding = Encoding.Auto)
		{
			switch (encoding) {
			case Encoding.Auto:
				ptr = Marshal.StringToHGlobalAuto (str);
				break;
			case Encoding.BStr:
				ptr = Marshal.StringToBSTR (str);
				break;
			case Encoding.Ansi:
				ptr = Marshal.StringToHGlobalAnsi (str);
				break;
			case Encoding.Unicode:
				ptr = Marshal.StringToHGlobalUni (str);
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (encoding));
			}
		}

		public void Dispose ()
		{
			if (ptr != IntPtr.Zero) {
				Marshal.FreeHGlobal (ptr);
				ptr = IntPtr.Zero;
			}
		}

		public static implicit operator IntPtr (TransientString str) => str.ptr;
		public static explicit operator string? (TransientString str) => Marshal.PtrToStringAuto (str.ptr);

		public static string? ToStringAndFree (IntPtr ptr, Encoding encoding = Encoding.Auto)
		{
			string? result = null;
			switch (encoding) {
			case Encoding.Auto:
				result = Marshal.PtrToStringAuto (ptr);
				break;
			case Encoding.BStr:
				result = Marshal.PtrToStringBSTR (ptr);
				break;
			case Encoding.Ansi:
				result = Marshal.PtrToStringAnsi (ptr);
				break;
			case Encoding.Unicode:
				result = Marshal.PtrToStringUni (ptr);
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (encoding));
			}
			Marshal.FreeHGlobal (ptr);
			return result;
		}

		public static IntPtr AllocStringArray (string? []? arr, Encoding encoding = Encoding.Auto)
		{
			if (arr is null)
				return IntPtr.Zero;
			var ptrArr = Marshal.AllocHGlobal (arr.Length * IntPtr.Size);
			var offset = 0;
			var step = IntPtr.Size;
			for (int i = 0; i < arr.Length; i++, offset += step) {
				var str = arr [i] is null ? IntPtr.Zero : new TransientString (arr [i], encoding);
				Marshal.WriteIntPtr (ptrArr + offset, str);
			}
			return ptrArr;
		}

		public static void FreeStringArray (IntPtr ptrArr, int count)
		{
			if (ptrArr == IntPtr.Zero)
				return;
			var offset = 0;
			var step = IntPtr.Size;
			for (int i = 0; i < count; i++, offset += step) {
				var str = Marshal.ReadIntPtr (ptrArr + offset);
				if (str != IntPtr.Zero)
					Marshal.FreeHGlobal (Marshal.ReadIntPtr (ptrArr + offset));
			}
			Marshal.FreeHGlobal (ptrArr);
		}
	}
}
