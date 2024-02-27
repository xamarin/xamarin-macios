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

#nullable enable

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {

	// CFBase.h
	public partial class CFAllocator : NativeObject {
#if !COREBUILD
		static CFAllocator? Default_cf;
		static CFAllocator? SystemDefault_cf;
		static CFAllocator? Malloc_cf;
		static CFAllocator? MallocZone_cf;
		static CFAllocator? Null_cf;
#endif

#if !NET
		[Obsolete ("Use the overload that takes a 'bool owns' parameter instead.")]
		public CFAllocator (NativeHandle handle)
			: base (handle, true /* backwards compatibility means we have to pass true here as opposed to the general pattern */)
		{
		}
#endif

		[Preserve (Conditional = true)]
#if NET
		internal CFAllocator (NativeHandle handle, bool owns)
#else
		public CFAllocator (NativeHandle handle, bool owns)
#endif
			: base (handle, owns)
		{
		}

#if !COREBUILD
		public static CFAllocator Default {
			get {
				return Default_cf ?? (Default_cf = new CFAllocator (default_ptr, false));
			}
		}

		public static CFAllocator SystemDefault {
			get {
				return SystemDefault_cf ?? (SystemDefault_cf = new CFAllocator (system_default_ptr, false));
			}
		}

		public static CFAllocator Malloc {
			get {
				return Malloc_cf ?? (Malloc_cf = new CFAllocator (malloc_ptr, false));
			}
		}

		public static CFAllocator MallocZone {
			get {
				return MallocZone_cf ?? (MallocZone_cf = new CFAllocator (malloc_zone_ptr, false));
			}
		}

		public static CFAllocator Null {
			get {
				return Null_cf ?? (Null_cf = new CFAllocator (null_ptr, false));
			}
		}
#endif

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern /* void* */ IntPtr CFAllocatorAllocate (/* CFAllocatorRef*/ IntPtr allocator, /*CFIndex*/ nint size, /* CFOptionFlags */ nuint hint);

		public IntPtr Allocate (long size)
		{
			return CFAllocatorAllocate (Handle, (nint) size, 0);
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		static extern void CFAllocatorDeallocate (/* CFAllocatorRef */ IntPtr allocator, /* void* */ IntPtr ptr);

		public void Deallocate (IntPtr ptr)
		{
			CFAllocatorDeallocate (Handle, ptr);
		}

		[DllImport (Constants.CoreFoundationLibrary, EntryPoint = "CFAllocatorGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();
	}
}
