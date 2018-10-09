// 
// CFUrl.cs: Implements the managed CFUrl
//
// Authors:
//     Miguel de Icaza
//     Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2009 Novell, Inc
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
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
using Foundation;
using ObjCRuntime;

namespace CoreFoundation {

	// CFURLPathStyle -> CFIndex -> CFURL.h
	[Native]
	public enum CFUrlPathStyle : long {
		POSIX = 0,
		HFS = 1,
		Windows = 2
	};

	// CFURL.h
	public class CFUrl
#if !COREBUILD
		: INativeObject, IDisposable
#endif
	{
#if !COREBUILD
		internal IntPtr handle;

		~CFUrl ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFURLRef */ IntPtr CFURLCreateWithFileSystemPath (/* CFAllocatorRef */ IntPtr allocator, 
			/* CFStringRef */ IntPtr filePath, 
			/* CFURLPathStyle */ nint pathStyle, 
			/* Boolean */ [MarshalAs (UnmanagedType.I1)] bool isDirectory);
		
		internal CFUrl (IntPtr handle)
		{
			this.handle = handle;
		}
		
		internal CFUrl (IntPtr handle, bool owned)
		{
			if (!owned)
				CFObject.CFRetain (handle);
			this.handle = handle;
		}
		
		static public CFUrl FromFile (string filename)
		{
			using (var str = new CFString (filename)){
				IntPtr handle = CFURLCreateWithFileSystemPath (IntPtr.Zero, str.Handle, (nint)(long)CFUrlPathStyle.POSIX, false);
				if (handle == IntPtr.Zero)
					return null;
				return new CFUrl (handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFURLRef */ IntPtr CFURLCreateWithString (/* CFAllocatorRef */ IntPtr allocator, 
			/* CFStringRef */ IntPtr URLString, 
			/* CFStringRef */ IntPtr baseURL);

		static public CFUrl FromUrlString (string url, CFUrl baseurl)
		{
			// CFString ctor will throw an ANE if null
			using (var str = new CFString (url)){
				return FromStringHandle (str.Handle, baseurl);
			}
		}

		internal static CFUrl FromStringHandle (IntPtr cfstringHandle, CFUrl baseurl)
		{
			IntPtr handle = CFURLCreateWithString (IntPtr.Zero, cfstringHandle, baseurl != null ? baseurl.Handle : IntPtr.Zero);
			if (handle == IntPtr.Zero)
				return null;
			return new CFUrl (handle);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFStringRef */ IntPtr CFURLGetString (/* CFURLRef */ IntPtr anURL);
		
		public override string ToString ()
		{
			using (var str = new CFString (CFURLGetString (handle))) {
				return str.ToString ();
			}
		}
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFStringRef */ IntPtr CFURLCopyFileSystemPath (/* CFURLRef */ IntPtr anURL, 
			/* CFURLPathStyle */ nint style);
		
		public string FileSystemPath {
			get {
				return GetFileSystemPath (handle);
			}
		}

		static internal string GetFileSystemPath (IntPtr hcfurl)
		{
			using (var str = new CFString (CFURLCopyFileSystemPath (hcfurl, 0), true))
				return str.ToString ();
		}

		[iOS (7,0)][Mac (10,9)]
		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CFURLIsFileReferenceURL (/* CFURLRef */IntPtr url);

		[iOS (7,0)][Mac (10,9)]
		public bool IsFileReference {
			get {
				return CFURLIsFileReferenceURL (handle);
			}
		}
		
		[DllImport (Constants.CoreFoundationLibrary, EntryPoint="CFURLGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();
#endif // !COREBUILD
	}
}
