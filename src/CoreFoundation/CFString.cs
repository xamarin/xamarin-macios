//
// CFBase.cs: Contains base types
//
// Authors:
//    Miguel de Icaza (miguel@novell.com)
//    Rolf Bjarne Kvinge (rolf@xamarin.com)
//
// Copyright 2012 Xamarin Inc
//
// The class can be either constructed from a string (from user code)
// or from a handle (from iphone-sharp.dll internal calls).  This
// delays the creation of the actual managed string until actually
// required
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace CoreFoundation {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct CFRange {
		nint loc; // defined as 'long' in native code
		nint len; // defined as 'long' in native code

		public int Location {
			get { return (int) loc; }
		}

		public int Length {
			get { return (int) len; }
		}

		public long LongLocation {
			get { return (long) loc; }
		}

		public long LongLength {
			get { return (long) len; }
		}

		public CFRange (int loc, int len)
		{
			this.loc = loc;
			this.len = len;
		}

		public CFRange (long l, long len)
		{
			this.loc = (nint) l;
			this.len = (nint) len;
		}

		public CFRange (nint l, nint len)
		{
			this.loc = l;
			this.len = len;
		}

		public override string ToString ()
		{
			return string.Format ("CFRange [Location: {0} Length: {1}]", loc, len);
		}
	}

#if NET
	// nothing is exposed publicly
	internal static class CFObject {
#else
	public static class CFObject {
#endif

		[DllImport (Constants.CoreFoundationLibrary)]
		internal extern static void CFRelease (IntPtr obj);

		[DllImport (Constants.CoreFoundationLibrary)]
		internal extern static IntPtr CFRetain (IntPtr obj);
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CFString
#if !COREBUILD
		: NativeObject
#endif
	{
#if !COREBUILD
		internal string? str;

		protected CFString () { }

		[DllImport (Constants.CoreFoundationLibrary, CharSet = CharSet.Unicode)]
		extern static IntPtr CFStringCreateWithCharacters (IntPtr allocator, IntPtr str, nint count);

		[DllImport (Constants.CoreFoundationLibrary, CharSet = CharSet.Unicode)]
		extern static nint CFStringGetLength (IntPtr handle);

		[DllImport (Constants.CoreFoundationLibrary, CharSet = CharSet.Unicode)]
		extern static unsafe char* CFStringGetCharactersPtr (IntPtr handle);

		[DllImport (Constants.CoreFoundationLibrary, CharSet = CharSet.Unicode)]
		extern static unsafe IntPtr CFStringGetCharacters (IntPtr handle, CFRange range, char* buffer);

		public static NativeHandle CreateNative (string? value)
		{
			if (value is null)
				return NativeHandle.Zero;

			using var valuePtr = new TransientString (value, TransientString.Encoding.Unicode);
			return CFStringCreateWithCharacters (IntPtr.Zero, valuePtr, value.Length);
		}

		public static void ReleaseNative (NativeHandle handle)
		{
			if (handle != NativeHandle.Zero)
				CFObject.CFRelease (handle);
		}

		public CFString (string str)
		{
			if (str is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (str));

			using var strPtr = new TransientString (str, TransientString.Encoding.Unicode);
			Handle = CFStringCreateWithCharacters (IntPtr.Zero, strPtr, str.Length);
			this.str = str;
		}

		[DllImport (Constants.CoreFoundationLibrary, EntryPoint = "CFStringGetTypeID")]
		public extern static nint GetTypeID ();

#if !NET
		public CFString (NativeHandle handle)
			: this (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
#if NET
		internal CFString (NativeHandle handle, bool owns)
#else
		protected internal CFString (NativeHandle handle, bool owns)
#endif
			: base (handle, owns)
		{
		}

		// to be used when an API like CF*Get* returns a CFString
		public static string? FromHandle (NativeHandle handle)
		{
			if (handle == IntPtr.Zero)
				return null;

			int l = (int) CFStringGetLength (handle);
			if (l == 0)
				return String.Empty;

			string str;
			bool allocate_memory = false;
			CFRange r = new CFRange (0, l);
			unsafe {
				// this returns non-null only if the string can be represented as unicode
				char* u = CFStringGetCharactersPtr (handle);
				if (u is null) {
					// alloc short string on the stack, otherwise use the heap
					allocate_memory = l > 128;
					// var m = allocate_memory ? (char*) Marshal.AllocHGlobal (l * 2) : stackalloc char [l];
					// this ^ won't compile so...
					if (allocate_memory) {
						u = (char*) Marshal.AllocHGlobal (l * 2);
					} else {
						// `u = stackalloc char [l];` won't compile either, even with cast
						char* u2 = stackalloc char [l];
						u = u2;
					}
					CFStringGetCharacters (handle, r, u);
				}
				str = new string (u, 0, l);
				if (allocate_memory)
					Marshal.FreeHGlobal ((IntPtr) u);
			}
			return str;
		}

		// to be used when an API like CF*Copy* returns a CFString
		public static string? FromHandle (NativeHandle handle, bool releaseHandle)
		{
			var s = FromHandle (handle);
			if (releaseHandle && (handle != IntPtr.Zero))
				CFObject.CFRelease (handle);
			return s;
		}

		public static implicit operator string? (CFString? x)
		{
			if (x is null)
				return null;

			if (x.str is null)
				x.str = FromHandle (x.Handle);

			return x.str;
		}

		[return: NotNullIfNotNull ("s")]
		public static implicit operator CFString? (string? s)
		{
			if (s is null)
				return null;

			return new CFString (s);
		}

		public int Length {
			get {
				if (str is not null)
					return str.Length;
				else
					return (int) CFStringGetLength (Handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary, CharSet = CharSet.Unicode)]
		[return: MarshalAs (UnmanagedType.U2)]
		extern static char CFStringGetCharacterAtIndex (IntPtr handle, nint p);

		public char this [nint p] {
			get {
				if (str is not null)
					return str [(int) p];
				else
					return CFStringGetCharacterAtIndex (Handle, p);
			}
		}

		public override string ToString ()
		{
			if (str is null)
				str = FromHandle (Handle);
			return str ?? base.ToString ()!;
		}
#endif // !COREBUILD
	}
}
