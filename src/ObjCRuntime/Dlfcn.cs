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
using CoreMedia;
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
		static public class OpenGLES {
			static public readonly IntPtr Handle = Dlfcn._dlopen (Constants.OpenGLESLibrary, 0);
		}
#endif
		static public class AudioToolbox {
			static public readonly IntPtr Handle = Dlfcn._dlopen (Constants.AudioToolboxLibrary, 0);
		}
#endif
	}

	public static class Dlfcn {
#if !COREBUILD
		public enum RTLD {
			/// <summary>The dynamic linker searches for the symbol in the dylibs the calling image linked against when built. It is usually used when you intentionally have multiply defined symbol across images and want to find the "next" definition. </summary>
			Next = -1,
			/// <summary>Searches all Mach-O images in the process (except those loaded with dlopen(xxx, RTLD_LOCAL)) in the order they were loaded.  This can be a costly search and should be avoided.</summary>
			Default = -2,
			/// <summary>Search for the symbol starts with the image that called dlsym.  If it is not found, the search continues as if Next was used.</summary>
			Self = -3,
			/// <summary>Only searches for symbol in the main executable.</summary>
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

		/// <summary>Gets the struct value exposed with the given symbol from the dynamic library.</summary>
		/// <param name="handle">Handle to the dynamic library previously opened with <see cref="dlopen(string,int)" />.</param>
		/// <param name="symbol">Name of the public symbol in the dynamic library to look up.</param>
		/// <returns>The struct from the library, or an empty struct (<c>default(T)</c>) if the symbol couldn't be found.</returns>
		public static T GetStruct<T> (IntPtr handle, string symbol) where T : unmanaged
		{
			var ptr = GetIndirect (handle, symbol);
			if (ptr == IntPtr.Zero)
				return default (T);
			unsafe {
				return *(T*) ptr;
			}
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

		/// <summary>Gets the signed byte value exposed with the given symbol from the dynamic library.</summary>
		/// <param name="handle">Handle to the dynamic library previously opened with <see cref="dlopen(System.String,System.Int32)" /> or <see cref="dlopen(System.String,Mode)" />.</param>
		/// <param name="symbol">Name of the public symbol in the dynamic library to look up.</param>
		/// <returns>The value from the library, or zero on failure.</returns>
		/// <remarks>If this routine fails, it will return zero.</remarks>
		public static sbyte GetSByte (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return 0;
			unchecked {
				return (sbyte) Marshal.ReadByte (indirect);
			}
		}

		/// <summary>Sets the specified symbol in the library handle to the specified signed byte value.</summary>
		/// <param name="handle">Handle to the dynamic library previously opened with <see cref="dlopen(System.String,System.Int32)" /> or <see cref="dlopen(System.String,Mode)" />.</param>
		/// <param name="symbol">Name of the public symbol in the dynamic library to look up.</param>
		/// <param name="value">The value to set.</param>
		public static void SetSByte (IntPtr handle, string symbol, sbyte value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			unsafe {
				Marshal.WriteByte (indirect, (byte) value);
			}
		}

		/// <summary>Gets the byte value exposed with the given symbol from the dynamic library.</summary>
		/// <param name="handle">Handle to the dynamic library previously opened with <see cref="dlopen(System.String,System.Int32)" /> or <see cref="dlopen(System.String,Mode)" />.</param>
		/// <param name="symbol">Name of the public symbol in the dynamic library to look up.</param>
		/// <returns>The value from the library, or zero on failure.</returns>
		/// <remarks>If this routine fails, it will return zero.</remarks>
		public static byte GetByte (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return 0;
			return Marshal.ReadByte (indirect);
		}

		/// <summary>Sets the specified symbol in the library handle to the specified byte value.</summary>
		/// <param name="handle">Handle to the dynamic library previously opened with <see cref="dlopen(System.String,System.Int32)" /> or <see cref="dlopen(System.String,Mode)" />.</param>
		/// <param name="symbol">Name of the public symbol in the dynamic library to look up.</param>
		/// <param name="value">The value to set.</param>
		public static void SetByte (IntPtr handle, string symbol, byte value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			Marshal.WriteByte (indirect, value);
		}

		/// <summary>Gets the short value exposed with the given symbol from the dynamic library.</summary>
		/// <param name="handle">Handle to the dynamic library previously opened with <see cref="dlopen(System.String,System.Int32)" /> or <see cref="dlopen(System.String,Mode)" />.</param>
		/// <param name="symbol">Name of the public symbol in the dynamic library to look up.</param>
		/// <returns>The value from the library, or zero on failure.</returns>
		/// <remarks>If this routine fails, it will return zero.</remarks>
		public static short GetInt16 (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return 0;
			return Marshal.ReadInt16 (indirect);
		}

		/// <summary>Sets the specified symbol in the library handle to the specified short value.</summary>
		/// <param name="handle">Handle to the dynamic library previously opened with <see cref="dlopen(System.String,System.Int32)" /> or <see cref="dlopen(System.String,Mode)" />.</param>
		/// <param name="symbol">Name of the public symbol in the dynamic library to look up.</param>
		/// <param name="value">The value to set.</param>
		public static void SetInt16 (IntPtr handle, string symbol, short value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			Marshal.WriteInt16 (indirect, value);
		}

		/// <summary>Gets the ushort value exposed with the given symbol from the dynamic library.</summary>
		/// <param name="handle">Handle to the dynamic library previously opened with <see cref="dlopen(System.String,System.Int32)" /> or <see cref="dlopen(System.String,Mode)" />.</param>
		/// <param name="symbol">Name of the public symbol in the dynamic library to look up.</param>
		/// <returns>The value from the library, or zero on failure.</returns>
		/// <remarks>If this routine fails, it will return zero.</remarks>
		public static ushort GetUInt16 (IntPtr handle, string symbol)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return 0;
			unchecked {
				return (ushort) Marshal.ReadInt16 (indirect);
			}
		}

		/// <summary>Sets the specified symbol in the library handle to the specified ushort value.</summary>
		/// <param name="handle">Handle to the dynamic library previously opened with <see cref="dlopen(System.String,System.Int32)" /> or <see cref="dlopen(System.String,Mode)" />.</param>
		/// <param name="symbol">Name of the public symbol in the dynamic library to look up.</param>
		/// <param name="value">The value to set.</param>
		public static void SetUInt16 (IntPtr handle, string symbol, ushort value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			unchecked {
				Marshal.WriteInt16 (indirect, (short) value);
			}
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
			SetObject (handle, symbol, value);
		}

		public static void SetArray (IntPtr handle, string symbol, NSArray? array)
		{
			SetObject (handle, symbol, array);
		}

		/// <summary>Sets the specified symbol in the library handle to the specified NSObject value.</summary>
		/// <param name="handle">Handle to the dynamic library previously opened with <see cref="dlopen(System.String,Mode)" />.</param>
		/// <param name="symbol">Name of the public symbol in the dynamic library to look up.</param>
		/// <param name="value">The object to set, can be null.</param>
		/// <remarks>The previous object value is not released, it is up to the developer to release the handle to that object if needed.</remarks>
		public static void SetObject (IntPtr handle, string symbol, NSObject? value)
		{
			var indirect = dlsym (handle, symbol);
			if (indirect == IntPtr.Zero)
				return;
			var objectHandle = value.GetHandle ();
			if (objectHandle != IntPtr.Zero)
				CFObject.CFRetain (objectHandle);
			Marshal.WriteIntPtr (indirect, objectHandle);
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
