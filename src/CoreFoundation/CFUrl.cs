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

#nullable enable

using System;
using System.Runtime.InteropServices;

using Foundation;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {

	// CFURLPathStyle -> CFIndex -> CFURL.h
	[Native]
	public enum CFUrlPathStyle : long {
		POSIX = 0,
		HFS = 1,
		Windows = 2
	};


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	// CFURL.h
	public class CFUrl : NativeObject {
#if !COREBUILD
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFURLRef */ IntPtr CFURLCreateWithFileSystemPath (/* CFAllocatorRef */ IntPtr allocator,
			/* CFStringRef */ IntPtr filePath,
			/* CFURLPathStyle */ nint pathStyle,
			/* Boolean */ [MarshalAs (UnmanagedType.I1)] bool isDirectory);

		[Preserve (Conditional = true)]
		internal CFUrl (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		static public CFUrl? FromFile (string filename)
		{
			if (filename is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (filename));
			var strHandle = CFString.CreateNative (filename);
			try {
				var handle = CFURLCreateWithFileSystemPath (IntPtr.Zero, strHandle, (nint) (long) CFUrlPathStyle.POSIX, false);
				if (handle == IntPtr.Zero)
					return null;
				return new CFUrl (handle, true);
			} finally {
				CFString.ReleaseNative (strHandle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFURLRef */ IntPtr CFURLCreateWithString (/* CFAllocatorRef */ IntPtr allocator,
			/* CFStringRef */ IntPtr URLString,
			/* CFStringRef */ IntPtr baseURL);

		static public CFUrl? FromUrlString (string url, CFUrl? baseurl)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
			var strHandle = CFString.CreateNative (url);
			try {
				return FromStringHandle (strHandle, baseurl);
			} finally {
				CFString.ReleaseNative (strHandle);
			}
		}

		internal static CFUrl? FromStringHandle (IntPtr cfstringHandle, CFUrl? baseurl)
		{
			var handle = CFURLCreateWithString (IntPtr.Zero, cfstringHandle, baseurl.GetHandle ());
			if (handle == IntPtr.Zero)
				return null;
			return new CFUrl (handle, true);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFStringRef */ IntPtr CFURLGetString (/* CFURLRef */ IntPtr anURL);

		public override string? ToString ()
		{
			return CFString.FromHandle (CFURLGetString (Handle));
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFStringRef */ IntPtr CFURLCopyFileSystemPath (/* CFURLRef */ IntPtr anURL,
			/* CFURLPathStyle */ nint style);

		public string? FileSystemPath {
			get {
				return GetFileSystemPath (Handle);
			}
		}

		static internal string? GetFileSystemPath (IntPtr hcfurl)
		{
			return CFString.FromHandle (CFURLCopyFileSystemPath (hcfurl, 0), true);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.CoreFoundationLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* Boolean */ bool CFURLIsFileReferenceURL (/* CFURLRef */IntPtr url);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public bool IsFileReference {
			get {
				return CFURLIsFileReferenceURL (Handle);
			}
		}

		[DllImport (Constants.CoreFoundationLibrary, EntryPoint = "CFURLGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();
#endif // !COREBUILD
	}
}
