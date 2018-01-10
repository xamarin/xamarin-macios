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
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace CoreFoundation {

	[StructLayout (LayoutKind.Sequential)]
	public struct CFRange {
#if MTOUCH
		IntPtr loc;
		IntPtr len;
#else
		nint loc; // defined as 'long' in native code
		nint len; // defined as 'long' in native code
#endif

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
			: this ((long) loc, (long) len)
		{
		}

		public CFRange (long l, long len)
		{
#if MTOUCH
			this.loc = (IntPtr) l;
			this.len = (IntPtr) len;
#else
			this.loc = (nint) l;
			this.len = (nint) len;
#endif
		}

#if XAMCORE_2_0
		public CFRange (nint l, nint len)
		{
			this.loc = l;
			this.len = len;
		}
#endif

		public override string ToString ()
		{
			return string.Format ("CFRange [Location: {0} Length: {1}]", loc, len);
		}
	}

	public static class CFObject {
		[DllImport (Constants.CoreFoundationLibrary)]
		internal extern static void CFRelease (IntPtr obj);

		[DllImport (Constants.CoreFoundationLibrary)]
		internal extern static IntPtr CFRetain (IntPtr obj);
	}
	
	public class CFString
#if !COREBUILD
		: INativeObject, IDisposable
#endif
	{
#if !COREBUILD
		internal IntPtr handle;
		internal string str;

		[DllImport (Constants.CoreFoundationLibrary, CharSet=CharSet.Unicode)]
		extern static IntPtr CFStringCreateWithCharacters (IntPtr allocator, string str, nint count);

		[DllImport (Constants.CoreFoundationLibrary, CharSet=CharSet.Unicode)]
		extern static nint CFStringGetLength (IntPtr handle);

		[DllImport (Constants.CoreFoundationLibrary, CharSet=CharSet.Unicode)]
		extern static IntPtr CFStringGetCharactersPtr (IntPtr handle);

		[DllImport (Constants.CoreFoundationLibrary, CharSet=CharSet.Unicode)]
		extern static IntPtr CFStringGetCharacters (IntPtr handle, CFRange range, IntPtr buffer);

		internal static IntPtr LowLevelCreate (string str)
		{
			if (str == null)
				return IntPtr.Zero;
			
			return CFStringCreateWithCharacters (IntPtr.Zero, str, str.Length);
		}
		
		public CFString (string str)
		{
			if (str == null)
				throw new ArgumentNullException ("str");
			
			handle = CFStringCreateWithCharacters (IntPtr.Zero, str, str.Length);
			this.str = str;
		}

		~CFString ()
		{
			Dispose (false);
		}

		public IntPtr Handle {
			get {
				return handle;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary, EntryPoint="CFStringGetTypeID")]
		public extern static nint GetTypeID ();
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

#if XAMCORE_2_0
		protected virtual void Dispose (bool disposing)
#else
		public virtual void Dispose (bool disposing)
#endif
		{
			str = null;
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}
		
		public CFString (IntPtr handle)
			: this (handle, false)
		{
		}
		
		[Preserve (Conditional = true)]
		internal CFString (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}

		// to be used when an API like CF*Get* returns a CFString
		internal static string FetchString (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return null;
			
			string str;
			
			int l = (int)CFStringGetLength (handle);
			IntPtr u = CFStringGetCharactersPtr (handle);
			IntPtr buffer = IntPtr.Zero;
			if (u == IntPtr.Zero){
				CFRange r = new CFRange (0, l);
				buffer = Marshal.AllocCoTaskMem (l * 2);
				CFStringGetCharacters (handle, r, buffer);
				u = buffer;
			}
			unsafe {
				str = new string ((char *) u, 0, l);
			}
			
			if (buffer != IntPtr.Zero)
				Marshal.FreeCoTaskMem (buffer);

			return str;
		}

		// to be used when an API like CF*Copy* returns a CFString
		internal static string FetchString (IntPtr handle, bool releaseHandle)
		{
			var s = FetchString (handle);
			if (releaseHandle && (handle != IntPtr.Zero))
				CFObject.CFRelease (handle);
			return s;
		}

		public static implicit operator string (CFString x)
		{
			if (x.str == null)
				x.str = FetchString (x.handle);
			
			return x.str;
		}

		public static implicit operator CFString (string s)
		{
			return new CFString (s);
		}

		public int Length {
			get {
				if (str != null)
					return str.Length;
				else
					return (int)CFStringGetLength (handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary, CharSet=CharSet.Unicode)]
		extern static char CFStringGetCharacterAtIndex (IntPtr handle, nint p);
		
		public char this [nint p] {
			get {
				if (str != null)
					return str [(int) p];
				else
					return CFStringGetCharacterAtIndex (handle, p);
			}
		}
		
		public override string ToString ()
		{
			if (str != null)
				return str;
			return FetchString (handle);
		}
#endif // !COREBUILD
	}
}
