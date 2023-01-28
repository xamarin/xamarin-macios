// 
// CGPDFDictionary.cs: Implements the managed CGPDFDictionary binding
//
// Authors:
//	Miguel de Icaza <miguel@xamarin.com>
//	Sebastien Pouliot <sebastien@xamarin.com>
// 
// Copyright 2010 Novell, Inc
// Copyright 2011-2014 Xamarin Inc. All rights reserved
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
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using CoreFoundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	// CGPDFDictionary.h
	public class CGPDFDictionary : CGPDFObject {
		// The lifetime management of CGPDFObject (and CGPDFArray, CGPDFDictionary and CGPDFStream) are tied to
		// the containing CGPDFDocument, and not possible to handle independently, which is why this class
		// does not subclass NativeObject (there's no way to retain/release CGPDFObject instances). It's
		// also why this constructor doesn't have a 'bool owns' parameter: it's always owned by the containing CGPDFDocument.
#if NET
		internal CGPDFDictionary (NativeHandle handle)
#else
		public CGPDFDictionary (IntPtr handle)
#endif
			: base (handle)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGPDFDictionaryGetCount (/* CGPDFDictionaryRef */ IntPtr dict);

		public int Count {
			get {
				return (int) CGPDFDictionaryGetCount (Handle);
			}
		}

		// CGPDFBoolean -> unsigned char -> CGPDFObject.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFDictionaryGetBoolean (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ IntPtr key, /* CGPDFBoolean* */ [MarshalAs (UnmanagedType.I1)] out bool value);

		public bool GetBoolean (string key, out bool result)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			using var keyPtr = new TransientString (key);
			return CGPDFDictionaryGetBoolean (Handle, keyPtr, out result);
		}

		// CGPDFInteger -> long int so 32/64 bits -> CGPDFObject.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFDictionaryGetInteger (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ IntPtr key, /* CGPDFInteger* */ out nint value);

		public bool GetInt (string key, out nint result)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			using var keyPtr = new TransientString (key);
			return CGPDFDictionaryGetInteger (Handle, keyPtr, out result);
		}

		// CGPDFReal -> CGFloat -> CGPDFObject.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFDictionaryGetNumber (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ IntPtr key, /* CGPDFReal* */ out nfloat value);

		public bool GetFloat (string key, out nfloat result)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

			using var keyPtr = new TransientString (key);
			return CGPDFDictionaryGetNumber (Handle, keyPtr, out result);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFDictionaryGetName (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ IntPtr key, /* const char ** */ out IntPtr value);

		public bool GetName (string key, out string? result)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			using var keyPtr = new TransientString (key);
			var r = CGPDFDictionaryGetName (Handle, keyPtr, out var res);
			result = r ? Marshal.PtrToStringAnsi (res) : null;
			return r;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFDictionaryGetDictionary (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ IntPtr key, /* CGPDFDictionaryRef* */ out IntPtr result);

		public bool GetDictionary (string key, out CGPDFDictionary? result)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

			using var keyPtr = new TransientString (key);
			var r = CGPDFDictionaryGetDictionary (Handle, keyPtr, out var res);
			result = r ? new CGPDFDictionary (res) : null;
			return r;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFDictionaryGetStream (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ IntPtr key, /* CGPDFStreamRef* */ out IntPtr value);

		public bool GetStream (string key, out CGPDFStream? result)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

			using var keyPtr = new TransientString (key);
			var r = CGPDFDictionaryGetStream (Handle, keyPtr, out var ptr);
			result = r ? new CGPDFStream (ptr) : null;
			return r;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFDictionaryGetArray (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ IntPtr key, /* CGPDFArrayRef* */ out IntPtr value);

		public bool GetArray (string key, out CGPDFArray? array)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

			using var keyPtr = new TransientString (key);
			var r = CGPDFDictionaryGetArray (Handle, keyPtr, out var ptr);
			array = r ? new CGPDFArray (ptr) : null;
			return r;
		}

#if NET
		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPDFDictionaryApplyFunction (/* CGPDFDictionaryRef */ IntPtr dic, delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> function, /* void* */ IntPtr info);
#else
		// CGPDFDictionaryApplierFunction
		delegate void ApplierFunction (/* const char* */ IntPtr key, /* CGPDFObjectRef */ IntPtr value, /* void* */ IntPtr info);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFDictionaryApplyFunction (/* CGPDFDictionaryRef */ IntPtr dic, ApplierFunction function, /* void* */ IntPtr info);

		static readonly ApplierFunction applyblock_handler = ApplyBridge;
#endif // NET

		public delegate void ApplyCallback (string? key, object? value, object? info);

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (ApplierFunction))]
#endif
#endif // NET
		static void ApplyBridge (IntPtr key, IntPtr pdfObject, IntPtr info)
		{
			var data = GCHandle.FromIntPtr (info).Target as Tuple<ApplyCallback, object?>;
			if (data is null)
				return;

			var callback = data.Item1;
			if (callback is not null)
				callback (Marshal.PtrToStringUTF8 (key), CGPDFObject.FromHandle (pdfObject), data.Item2);
		}

		public void Apply (ApplyCallback callback, object? info = null)
		{
			var data = new Tuple<ApplyCallback, object?> (callback, info);
			var gch = GCHandle.Alloc (data);
			try {
#if NET
				unsafe {
					CGPDFDictionaryApplyFunction (Handle, &ApplyBridge, GCHandle.ToIntPtr (gch));
				}
#else
				CGPDFDictionaryApplyFunction (Handle, applyblock_handler, GCHandle.ToIntPtr (gch));
#endif
			} finally {
				gch.Free ();
			}
		}

#if NET
		[UnmanagedCallersOnly]
#else
#if !MONOMAC
		[MonoPInvokeCallback (typeof (ApplierFunction))]
#endif
#endif // NET
		static void ApplyBridge2 (IntPtr key, IntPtr pdfObject, IntPtr info)
		{
			var callback = GCHandle.FromIntPtr (info).Target as Action<string?, CGPDFObject>;
			if (callback is not null)
				callback (Marshal.PtrToStringUTF8 (key), new CGPDFObject (pdfObject));
		}

		public void Apply (Action<string?, CGPDFObject> callback)
		{
			GCHandle gch = GCHandle.Alloc (callback);
#if NET
			unsafe {
				CGPDFDictionaryApplyFunction (Handle, &ApplyBridge2, GCHandle.ToIntPtr (gch));
			}
#else
			CGPDFDictionaryApplyFunction (Handle, ApplyBridge2, GCHandle.ToIntPtr (gch));
#endif
			gch.Free ();
		}

		// CGPDFDictionary.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFDictionaryGetString (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ IntPtr key, /* CGPDFStringRef* */ out IntPtr value);

		public bool GetString (string key, out string? result)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

			using var keyPtr = new TransientString (key);
			var r = CGPDFDictionaryGetString (Handle, keyPtr, out var res);
			result = r ? CGPDFString.ToString (res) : null;
			return r;
		}
	}
}
