// 
// CFArray.cs: P/Invokes for CFArray
//
// Authors:
//    Mono Team
//    Rolf Bjarne Kvinge (rolf@xamarin.com)
//
//     
// Copyright 2010 Novell, Inc
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

#if NET
using CFIndex = System.IntPtr;
#else
using CFIndex = System.nint;
#endif
using CFArrayRef = System.IntPtr;
using CFAllocatorRef = System.IntPtr;

#nullable enable

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {

	// interesting bits: https://github.com/opensource-apple/CF/blob/master/CFArray.c
	public partial class CFArray : NativeObject {

		// this cache the handle instead of issuing a native call
		internal static NativeHandle CFNullHandle = _CFNullHandle;

#if !NET
		internal CFArray (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CFArray (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreFoundationLibrary, EntryPoint = "CFArrayGetTypeID")]
		internal extern static /* CFTypeID */ nint GetTypeID ();

		// pointer to a const struct (REALLY APPLE?)
		static IntPtr kCFTypeArrayCallbacks_ptr_value;
		static IntPtr kCFTypeArrayCallbacks_ptr {
			get {
				// FIXME: right now we can't use [Fields] for GetIndirect
				if (kCFTypeArrayCallbacks_ptr_value == IntPtr.Zero)
					kCFTypeArrayCallbacks_ptr_value = Dlfcn.GetIndirect (Libraries.CoreFoundation.Handle, "kCFTypeArrayCallBacks");
				return kCFTypeArrayCallbacks_ptr_value;
			}
		}

		internal static CFArray FromIntPtrs (params NativeHandle [] values)
		{
			return new CFArray (Create (values), true);
		}

		internal static CFArray FromNativeObjects (params INativeObject [] values)
		{
			return new CFArray (Create (values), true);
		}

		public nint Count {
			get { return GetCount (GetCheckedHandle ()); }
		}

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static /* CFArrayRef */ IntPtr CFArrayCreate (/* CFAllocatorRef */ IntPtr allocator, /* void** */ IntPtr values, nint numValues, /* CFArrayCallBacks* */ IntPtr callBacks);

		[DllImport (Constants.CoreFoundationLibrary)]
		internal extern static /* void* */ IntPtr CFArrayGetValueAtIndex (/* CFArrayRef */ IntPtr theArray, /* CFIndex */ nint idx);

		public NativeHandle GetValue (nint index)
		{
			return CFArrayGetValueAtIndex (GetCheckedHandle (), index);
		}

		internal static unsafe NativeHandle Create (params NativeHandle [] values)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			fixed (NativeHandle* pv = values) {
				return CFArrayCreate (IntPtr.Zero,
						(IntPtr) pv,
						values.Length,
						kCFTypeArrayCallbacks_ptr);
			}
		}

		public static unsafe NativeHandle Create (params INativeObject [] values)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			int c = values.Length;
			var _values = c <= 256 ? stackalloc IntPtr [c] : new IntPtr [c];
			for (int i = 0; i < c; ++i)
				_values [i] = values [i].Handle;
			fixed (IntPtr* pv = _values)
				return CFArrayCreate (IntPtr.Zero, (IntPtr) pv, c, kCFTypeArrayCallbacks_ptr);
		}

		public static unsafe NativeHandle Create (params string [] values)
		{
			if (values is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (values));
			var c = values.Length;
			var _values = c <= 256 ? stackalloc IntPtr [c] : new IntPtr [c];
			for (var i = 0; i < c; ++i)
				_values [i] = values [i] is null ? CFNullHandle : CFString.CreateNative (values [i]);
			fixed (IntPtr* pv = _values)
				return CFArrayCreate (IntPtr.Zero, (IntPtr) pv, c, kCFTypeArrayCallbacks_ptr);
		}

		static public CFArray FromStrings (params string [] items)
		{
			return new CFArray (Create (items), true);
		}

		[DllImport (Constants.CoreFoundationLibrary, EntryPoint = "CFArrayGetCount")]
		internal extern static /* CFIndex */ nint GetCount (/* CFArrayRef */ IntPtr theArray);

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static CFArrayRef CFArrayCreateCopy (CFAllocatorRef allocator, CFArrayRef theArray);

		internal CFArray Clone () => new CFArray (CFArrayCreateCopy (IntPtr.Zero, GetCheckedHandle ()), true);

		[DllImport (Constants.CoreFoundationLibrary)]
		internal extern static void CFArrayGetValues (/* CFArrayRef */ IntPtr theArray, CFRange range, /* const void ** */ IntPtr values);

		// identical signature to NSArray API
		static unsafe public string? []? StringArrayFromHandle (NativeHandle handle)
		{
			return ArrayFromHandleFunc (handle, CFString.FromHandle);
		}

		static unsafe public string? []? StringArrayFromHandle (NativeHandle handle, bool releaseHandle)
		{
			var rv = StringArrayFromHandle (handle);
			if (releaseHandle && handle != NativeHandle.Zero)
				CFObject.CFRelease (handle);
			return rv;
		}

		// identical signature to NSArray API
		static public T? []? ArrayFromHandle<T> (NativeHandle handle) where T : class, INativeObject
		{
			var rv = ArrayFromHandleFunc<T> (handle, DefaultConvert<T>);
			return rv;
		}

		static public T? []? ArrayFromHandle<T> (NativeHandle handle, bool releaseHandle) where T : class, INativeObject
		{
			var rv = ArrayFromHandle<T> (handle);
			if (releaseHandle && handle != NativeHandle.Zero)
				CFObject.CFRelease (handle);
			return rv;
		}

		static T DefaultConvert<T> (NativeHandle handle) where T : class, INativeObject
		{
			if (handle != CFNullHandle)
				return Runtime.GetINativeObject<T> (handle, forced_type: false, owns: false)!;
			return null!;
		}

		// identical signature to NSArray API
		static public T []? ArrayFromHandleFunc<T> (NativeHandle handle, Func<NativeHandle, T> createObject)
		{
			if (handle == NativeHandle.Zero)
				return null;

			var c = (int) GetCount (handle);
			if (c == 0)
				return Array.Empty<T> ();

			var buffer = c <= 256 ? stackalloc IntPtr [c] : new IntPtr [c];
			unsafe {
				fixed (void* ptr = buffer)
					CFArrayGetValues (handle, new CFRange (0, c), (IntPtr) ptr);
			}

			var ret = new T [c];
			for (var i = 0; i < c; i++) {
				ret [i] = createObject (buffer [i]);
			}
			return ret;
		}

		static public T []? ArrayFromHandleFunc<T> (NativeHandle handle, Func<NativeHandle, T> createObject, bool releaseHandle)
		{
			var rv = ArrayFromHandleFunc<T> (handle, createObject);
			if (releaseHandle && handle != IntPtr.Zero)
				CFObject.CFRelease (handle);
			return rv;
		}
	}
}
