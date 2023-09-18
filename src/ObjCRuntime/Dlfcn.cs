//
// Dlfcn.cs: Support for looking up symbols in shared libraries
//
// Authors:
//   Jonathan Pryor:
//   Miguel de Icaza.
//
// Copyright 2009-2010, Novell, Inc.
// Copyright 2011, 2012 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//

#nullable enable

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
#if !COREBUILD
using Foundation;
using CoreFoundation;
using CoreGraphics;
#if !WATCH
using CoreMedia;
#endif
#endif
#if !NO_SYSTEM_DRAWING
using System.Drawing;
#endif

namespace ObjCRuntime {

	// most are being generated from bindings
	static partial class Libraries {
#if !COREBUILD
		static public class System {
			static public readonly IntPtr Handle = Dlfcn._dlopen (Constants.libSystemLibrary, 0);
		}
		static public class LibC {
			static public readonly IntPtr Handle = Dlfcn._dlopen (Constants.libcLibrary, 0);
		}
#if HAS_OPENGLES
		static public class OpenGLES
		{
			static public readonly IntPtr Handle = Dlfcn._dlopen (Constants.OpenGLESLibrary, 0);
		}
#endif
#if !WATCH
		static public class AudioToolbox {
			static public readonly IntPtr Handle = Dlfcn._dlopen (Constants.AudioToolboxLibrary, 0);
		}
#endif
#endif
	}

	public static class Dlfcn {
#if !COREBUILD
		public enum RTLD {
			Next = -1,
			Default = -2,
			Self = -3,
			MainOnly = -5
		}

		[Flags]
		public enum Mode : int {
			None = 0x0,
			Lazy = 0x1,
			Now = 0x2,
			Local = 0x4,
			Global = 0x8,
			NoLoad = 0x10,
			NoDelete = 0x80,
			First = 0x100,
		}

#if MONOMAC && !NET
		[DllImport (Constants.libcLibrary)]
		internal static extern int dladdr (IntPtr addr, out Dl_info info);

		internal struct Dl_info
		{
			internal IntPtr dli_fname; /* Pathname of shared object */
			internal IntPtr dli_fbase; /* Base address of shared object */
			internal IntPtr dli_sname; /* Name of nearest symbol */
			internal IntPtr dli_saddr; /* Address of nearest symbol */
		}
#endif

		[DllImport (Constants.libSystemLibrary)]
		public static extern int dlclose (IntPtr handle);

		[DllImport (Constants.libSystemLibrary, EntryPoint = "dlopen")]
		static extern IntPtr _dlopen (IntPtr path, Mode mode /* this is int32, not nint */);

		internal static IntPtr _dlopen (string? path, Mode mode /* this is int32, not nint */)
		{
			using var pathPtr = new TransientString (path);
			return _dlopen (pathPtr, mode);
		}

		public static IntPtr dlopen (string? path, int mode)
		{
			return dlopen (path, mode, showWarning: true);
		}

		public static IntPtr dlopen (string? path, Mode mode)
		{
			return _dlopen (path, mode);
		}

		static bool warningShown;
		// the linker can eliminate the body of this method (and the above static variable) on release builds
		static void WarnOnce ()
		{
			if (!warningShown)
				Runtime.NSLog ("You are using dlopen without a full path, retrying by prepending /usr/lib");
			warningShown = true;
		}

		internal static IntPtr dlopen (string? path, int mode, bool showWarning)
		{
			var x = _dlopen (path, (Mode) mode);
			if (x != IntPtr.Zero)
				return x;

			// In iOS < 9, you could dlopen ("libc") and that would work.
			// In iOS >= 9, this fails with:
			// "no cache image with name (<top>)"
			if (path?.IndexOf ('/') == -1) {
				if (showWarning)
					WarnOnce ();
				return dlopen ("/usr/lib/" + path, mode, false);
			}
			return IntPtr.Zero;
		}

		[DllImport (Constants.libSystemLibrary)]
		static extern IntPtr dlsym (IntPtr handle, IntPtr symbol);

		public static IntPtr dlsym (IntPtr handle, string symbol)
		{
			using var symbolPtr = new TransientString (symbol);
			return dlsym (handle, symbolPtr);
		}

		public static IntPtr dlsym (RTLD lookupType, string symbol)
		{
			return dlsym ((IntPtr) lookupType, symbol);
		}

		[DllImport (Constants.libSystemLibrary, EntryPoint = "dlerror")]
		internal static extern IntPtr dlerror_ ();

		public static string? dlerror ()
		{
			// we can't free the string returned from dlerror
			return Marshal.PtrToStringAnsi (dlerror_ ());
		}

		public static NSString? GetStringConstant (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return null;
			var actual = Marshal.ReadIntPtr (indirect);
			if (actual == IntPtr.Zero)
				return null;
			return Runtime.GetNSObject<NSString> (actual);
		}

		public static IntPtr GetIndirect (IntPtr handle, string symbol)
		{
			return dlsym (handle, symbol);
		}

		public static NSNumber? GetNSNumber (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return null;
			var actual = Marshal.ReadIntPtr (indirect);
			if (actual == IntPtr.Zero)
				return null;
			return Runtime.GetNSObject<NSNumber> (actual);
		}

		public static int GetInt32 (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return 0;
			return Marshal.ReadInt32 (indirect);
		}

		public static void SetInt32 (IntPtr handle, string symbol, int value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			Marshal.WriteInt32 (indirect, value);
		}

		public static uint GetUInt32 (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return 0;
			return (uint) Marshal.ReadInt32 (indirect);
		}

		public static void SetUInt32 (IntPtr handle, string symbol, uint value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			Marshal.WriteInt32 (indirect, (int) value);
		}

		public static long GetInt64 (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return 0;
			return Marshal.ReadInt64 (indirect);
		}

		public static void SetInt64 (IntPtr handle, string symbol, long value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			Marshal.WriteInt64 (indirect, value);
		}

		public static ulong GetUInt64 (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return 0;

			return (ulong) Marshal.ReadInt64 (indirect);
		}

#if !NET
		[Obsolete ("Use 'SetInt64' for long values instead.")]
		public static void SetUInt64 (IntPtr handle, string symbol, long value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;

			Marshal.WriteInt64 (indirect, (long) value);
		}
#endif

		public static void SetUInt64 (IntPtr handle, string symbol, ulong value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;

			Marshal.WriteInt64 (indirect, (long) value);
		}

		public static void SetString (IntPtr handle, string symbol, string? value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			Marshal.WriteIntPtr (indirect, CFString.CreateNative (value));
		}

		public static void SetString (IntPtr handle, string symbol, NSString? value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			var strHandle = value.GetHandle ();
			if (strHandle != IntPtr.Zero)
				CFObject.CFRetain (strHandle);
			Marshal.WriteIntPtr (indirect, strHandle);
		}

		public static void SetArray (IntPtr handle, string symbol, NSArray? array)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			var arrayHandle = array.GetHandle ();
			if (arrayHandle != IntPtr.Zero)
				CFObject.CFRetain (arrayHandle);
			Marshal.WriteIntPtr (indirect, arrayHandle);
		}

		public static nint GetNInt (IntPtr handle, string symbol)
		{
			return (nint) GetIntPtr (handle, symbol);
		}

		public static void SetNInt (IntPtr handle, string symbol, nint value)
		{
			SetIntPtr (handle, symbol, (IntPtr) value);
		}

		public static nuint GetNUInt (IntPtr handle, string symbol)
		{
			return (nuint) (ulong) GetUIntPtr (handle, symbol);
		}

		public static void SetNUInt (IntPtr handle, string symbol, nuint value)
		{
			SetUIntPtr (handle, symbol, (UIntPtr) (ulong) value);
		}

		public static nfloat GetNFloat (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return 0;

			unsafe {
				if (sizeof (IntPtr) == 4)
					return (nfloat) (*(float*) indirect);
				else
					return (nfloat) (*(double*) indirect);
			}
		}

		public static void SetNFloat (IntPtr handle, string symbol, nfloat value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;

			unsafe {
				nfloat* ptr = (nfloat*) indirect;
				*ptr = value;
			}
		}

		public static IntPtr GetIntPtr (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return IntPtr.Zero;
			return Marshal.ReadIntPtr (indirect);
		}

		public static UIntPtr GetUIntPtr (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return UIntPtr.Zero;
			return (UIntPtr) (long) Marshal.ReadIntPtr (indirect);
		}

		public static void SetUIntPtr (IntPtr handle, string symbol, UIntPtr value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			Marshal.WriteIntPtr (indirect, (IntPtr) (ulong) value);
		}

		public static void SetIntPtr (IntPtr handle, string symbol, IntPtr value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			Marshal.WriteIntPtr (indirect, value);
		}

		public static CGRect GetCGRect (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return CGRect.Empty;
			unsafe {
				nfloat* ptr = (nfloat*) indirect;
				return new CGRect (ptr [0], ptr [1], ptr [2], ptr [3]);
			}
		}

		public static CGSize GetCGSize (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return CGSize.Empty;
			unsafe {
				nfloat* ptr = (nfloat*) indirect;
				return new CGSize (ptr [0], ptr [1]);
			}
		}

		public static void SetCGSize (IntPtr handle, string symbol, CGSize value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			unsafe {
				nfloat* ptr = (nfloat*) indirect;
				ptr [0] = value.Width;
				ptr [1] = value.Height;
			}
		}

		public static double GetDouble (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return 0;
			unsafe {
				double* d = (double*) indirect;

				return *d;
			}
		}

		public static void SetDouble (IntPtr handle, string symbol, double value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			unsafe {
				*(double*) indirect = value;
			}
		}

		public static float GetFloat (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return 0;
			unsafe {
				float* d = (float*) indirect;

				return *d;
			}
		}

		public static void SetFloat (IntPtr handle, string symbol, float value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			unsafe {
				*(float*) indirect = value;
			}
		}

		internal static int SlowGetInt32 (string? lib, string symbol)
		{
			var handle = dlopen (lib, 0);
			if (handle == IntPtr.Zero)
				return 0;
			try {
				return GetInt32 (handle, symbol);
			} finally {
				dlclose (handle);
			}
		}

		internal static long SlowGetInt64 (string? lib, string symbol)
		{
			var handle = dlopen (lib, 0);
			if (handle == IntPtr.Zero)
				return 0;
			try {
				return GetInt64 (handle, symbol);
			} finally {
				dlclose (handle);
			}
		}

		internal static IntPtr SlowGetIntPtr (string? lib, string symbol)
		{
			var handle = dlopen (lib, 0);
			if (handle == IntPtr.Zero)
				return IntPtr.Zero;
			try {
				return GetIntPtr (handle, symbol);
			} finally {
				dlclose (handle);
			}
		}

		internal static double SlowGetDouble (string? lib, string symbol)
		{
			var handle = dlopen (lib, 0);
			if (handle == IntPtr.Zero)
				return 0;
			try {
				return GetDouble (handle, symbol);
			} finally {
				dlclose (handle);
			}
		}

		internal static NSString? SlowGetStringConstant (string? lib, string symbol)
		{
			var handle = dlopen (lib, 0);
			if (handle == IntPtr.Zero)
				return null;
			try {
				return GetStringConstant (handle, symbol);
			} finally {
				dlclose (handle);
			}
		}
#endif // !COREBUILD
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public static unsafe IntPtr CachePointer (IntPtr handle, string constant, IntPtr* storage)
		{
			if (*storage == IntPtr.Zero)
				*storage = Dlfcn.GetIntPtr (handle, constant);
			return *storage;
		}
	}
}
