// 
// CFAllocator.cs
//
// Authors:
//    Rolf Bjarne Kvinge
//    Marek Safar (marek.safar@gmail.com)
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

	// CFBase.h
	public partial class CFAllocator : INativeObject, IDisposable 
	{
#if !COREBUILD && !MTOUCH
		static CFAllocator Default_cf;
		static CFAllocator SystemDefault_cf;
		static CFAllocator Malloc_cf;
		static CFAllocator MallocZone_cf;
		static CFAllocator Null_cf;
#endif
		IntPtr handle;

#if MTOUCH
		internal static IntPtr null_ptr;

		static CFAllocator ()
		{
			var handle = Dlfcn.dlopen (Constants.CoreFoundationLibrary, 0);
			null_ptr = Dlfcn.GetIntPtr (handle, "kCFAllocatorNull");
			Dlfcn.dlclose (handle);
		}
#endif

		public CFAllocator (IntPtr handle)
		{
			this.handle = handle;
		}

		public CFAllocator (IntPtr handle, bool owns)
		{
			if (!owns)
				CFObject.CFRetain (handle);
			this.handle = handle;
		}

		~CFAllocator ()
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
#if !COREBUILD && !MTOUCH
		public static CFAllocator Default {
			get {
				return Default_cf ?? (Default_cf = new CFAllocator (default_ptr)); 
			}
		}

		public static CFAllocator SystemDefault {
			get {
				return SystemDefault_cf ?? (SystemDefault_cf = new CFAllocator (system_default_ptr)); 
			}
		}
		
		public static CFAllocator Malloc {
			get {
				return Malloc_cf ?? (Malloc_cf = new CFAllocator (malloc_ptr)); 
			}
		}

		public static CFAllocator MallocZone {
			get {
				return MallocZone_cf ?? (MallocZone_cf = new CFAllocator (malloc_zone_ptr)); 
			}
		}

		public static CFAllocator Null {
			get {
				return Null_cf ?? (Null_cf = new CFAllocator (null_ptr)); 
			}
		}
#endif

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern /* void* */ IntPtr CFAllocatorAllocate (/* CFAllocatorRef*/ IntPtr allocator, /*CFIndex*/ nint size, /* CFOptionFlags */ nuint hint);

#if XAMCORE_2_0
		public IntPtr Allocate (long size)
		{
			return CFAllocatorAllocate (handle, (nint)size, 0);
		}
#else
		public IntPtr Allocate (long size, CFAllocatorFlags hint = 0)
		{
			return CFAllocatorAllocate (handle, (nint)size, (nuint) hint);
		}
#endif

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern void CFAllocatorDeallocate (/* CFAllocatorRef */ IntPtr allocator, /* void* */ IntPtr ptr);

		public void Deallocate (IntPtr ptr)
		{
			CFAllocatorDeallocate (handle, ptr);
		}

		[DllImport (Constants.CoreFoundationLibrary, EntryPoint="CFAllocatorGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();
	}

#if !XAMCORE_2_0
	// XAMCORE 2.0: removing this enum, I can't find any documentation anywhere about it,
	//              and in any case it seems to refer to the deprecated/deleted ObjC GC.
	// Seems to be some sort of secret values
	[Flags]
	[Native]
	public enum CFAllocatorFlags : ulong
	{
		GCScannedMemory	= 0x200,
		GCObjectMemory	= 0x400,
	}
#endif
}
