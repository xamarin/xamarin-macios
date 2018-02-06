// 
// CFData.cs: Implements the managed CFData
//
// Authors:
//    Rolf Bjarne Kvinge (rolf@xamarin.com)
//     
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

	class CFData : INativeObject, IDisposable {
		internal IntPtr handle;

		public CFData (IntPtr handle)
			: this (handle, false)
		{
		}

		public CFData (IntPtr handle, bool owns)
		{
			if (!owns)
				CFObject.CFRetain (handle);
			this.handle = handle;
		}

		~CFData ()
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
		
		[DllImport (Constants.CoreFoundationLibrary, EntryPoint="CFDataGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

#if !COREBUILD
		public static CFData FromDataNoCopy (IntPtr buffer, nint length)
		{
			return new CFData (CFDataCreateWithBytesNoCopy (IntPtr.Zero, buffer, length, CFAllocator.null_ptr), true);
		}
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFDataRef */ IntPtr CFDataCreateWithBytesNoCopy (/* CFAllocatorRef */ IntPtr allocator, /* UInt8* */ IntPtr bytes, /* CFIndex */ nint length, /* CFAllocatorRef */ IntPtr bytesDeallocator);
#endif

		public nint Length {
			get { return CFDataGetLength (handle); }
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFIndex */ nint CFDataGetLength (/* CFDataRef */ IntPtr theData);

		public byte[] GetBuffer ()
		{
			var buffer = new byte [Length];
			var ptr = CFDataGetBytePtr (handle);
			Marshal.Copy (ptr, buffer, 0, buffer.Length);
			return buffer;
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* UInt8* */ IntPtr CFDataGetBytePtr (/* CFDataRef */ IntPtr theData);

		/*
		 * Exposes a read-only pointer to the underlying storage.
		 */
		public IntPtr Bytes {
			get { return CFDataGetBytePtr (handle); }
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFDataRef */ IntPtr CFDataCreate (/* CFAllocatorRef */ IntPtr allocator, /* UInt8* */ IntPtr bytes, /* CFIndex */ nint length);

		public static CFData FromData (IntPtr buffer, nint length)
		{
			return new CFData (CFDataCreate (IntPtr.Zero, buffer, length), true);
		}
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFDataRef */ IntPtr CFDataCreateCopy (/* CFAllocatorRef */ IntPtr allocator, /* CFDataRef */ IntPtr theData);

		public CFData Copy ()
		{
			return new CFData (CFDataCreateCopy (IntPtr.Zero, Handle), true);
		}
	}
}
