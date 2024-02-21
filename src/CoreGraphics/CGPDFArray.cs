// 
// CGPDFArray.cs: Implements the managed CGPDFArray binding
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
	// CGPDFArray.h
	public class CGPDFArray : CGPDFObject {
		// The lifetime management of CGPDFObject (and CGPDFArray, CGPDFDictionary and CGPDFStream) are tied to
		// the containing CGPDFDocument, and not possible to handle independently, which is why this class
		// does not subclass NativeObject (there's no way to retain/release CGPDFObject instances). It's
		// also why this constructor doesn't have a 'bool owns' parameter: it's always owned by the containing CGPDFDocument.
#if NET
		internal CGPDFArray (NativeHandle handle)
#else
		public CGPDFArray (IntPtr handle)
#endif
			: base (handle)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGPDFArrayGetCount (/* CGPDFArrayRef */ IntPtr array);

		public nint Count {
			get {
				return CGPDFArrayGetCount (Handle);
			}
		}

		// CGPDFBoolean -> unsigned char -> CGPDFObject.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFArrayGetBoolean (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFBoolean* */ [MarshalAs (UnmanagedType.I1)] out bool value);

		public bool GetBoolean (nint idx, out bool result)
		{
			return CGPDFArrayGetBoolean (Handle, idx, out result);
		}

#if !NET
		public bool GetBoolean (int idx, out bool result)
		{
			return CGPDFArrayGetBoolean (Handle, idx, out result);
		}
#endif

		// CGPDFInteger -> long int 32/64 bits -> CGPDFObject.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFArrayGetInteger (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFInteger* */ out nint value);

		public bool GetInt (nint idx, out nint result)
		{
			return CGPDFArrayGetInteger (Handle, idx, out result);
		}

#if !NET
		public bool GetInt (int idx, out nint result)
		{
			return CGPDFArrayGetInteger (Handle, idx, out result);
		}
#endif

		// CGPDFReal -> CGFloat -> CGPDFObject.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFArrayGetNumber (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFReal* */ out nfloat value);

		public bool GetFloat (nint idx, out nfloat result)
		{
			return CGPDFArrayGetNumber (Handle, idx, out result);
		}

#if !NET
		public bool GetFloat (int idx, out nfloat result)
		{
			return CGPDFArrayGetNumber (Handle, idx, out result);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFArrayGetName (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* const char** */ out IntPtr value);

		public bool GetName (nint idx, out string? result)
		{
			IntPtr res;
			var r = CGPDFArrayGetName (Handle, idx, out res);
			result = r ? Marshal.PtrToStringAnsi (res) : null;
			return r;
		}

#if !NET
		public bool GetName (int idx, out string? result)
		{
			return GetName ((nint) idx, out result);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFArrayGetDictionary (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFDictionaryRef* */ out IntPtr value);

		public bool GetDictionary (nint idx, out CGPDFDictionary? result)
		{
			var r = CGPDFArrayGetDictionary (Handle, idx, out var res);
			result = r ? new CGPDFDictionary (res) : null;
			return r;
		}

#if !NET
		public bool GetDictionary (int idx, out CGPDFDictionary? result)
		{
			return GetDictionary ((nint) idx, out result);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFArrayGetStream (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFStreamRef* */ out IntPtr value);

		public bool GetStream (nint idx, out CGPDFStream? result)
		{
			var r = CGPDFArrayGetStream (Handle, idx, out var ptr);
			result = r ? new CGPDFStream (ptr) : null;
			return r;
		}

#if !NET
		public bool GetStream (int idx, out CGPDFStream? result)
		{
			return GetStream ((nint) idx, out result);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFArrayGetArray (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFArrayRef* */ out IntPtr value);

		public bool GetArray (nint idx, out CGPDFArray? array)
		{
			var r = CGPDFArrayGetArray (Handle, idx, out var ptr);
			array = r ? new CGPDFArray (ptr) : null;
			return r;
		}

#if !NET
		public bool GetArray (int idx, out CGPDFArray? array)
		{
			return GetArray ((nint) idx, out array);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFArrayGetString (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFStringRef* */ out IntPtr value);

		public bool GetString (nint idx, out string? result)
		{
			var r = CGPDFArrayGetString (Handle, idx, out var res);
			result = r ? CGPDFString.ToString (res) : null;
			return r;
		}

#if !NET
		public bool GetString (int idx, out string? result)
		{
			return GetString ((nint) idx, out result);
		}
#endif

#if !NET
		delegate byte ApplyBlockHandlerDelegate (IntPtr block, nint index, IntPtr value, IntPtr info);
		static readonly ApplyBlockHandlerDelegate applyblock_handler = ApplyBlockHandler;

#if !MONOMAC
		[MonoPInvokeCallback (typeof (ApplyBlockHandlerDelegate))]
#endif
#else
		[UnmanagedCallersOnly]
#endif
		static byte ApplyBlockHandler (IntPtr block, nint index, IntPtr value, IntPtr info)
		{
			var del = BlockLiteral.GetTarget<ApplyCallback> (block);
			if (del is not null) {
				var context = info == IntPtr.Zero ? null : GCHandle.FromIntPtr (info).Target;
				return del (index, CGPDFObject.FromHandle (value), context) ? (byte) 1 : (byte) 0;
			}

			return 0;
		}

		public delegate bool ApplyCallback (nint index, object? value, object? info);

#if NET
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (12, 0)]
		[TV (12, 0)]
		[Watch (5, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		unsafe extern static bool CGPDFArrayApplyBlock (/* CGPDFArrayRef */ IntPtr array, /* CGPDFArrayApplierBlock */ BlockLiteral* block, /* void* */ IntPtr info);

#if NET
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (12, 0)]
		[TV (12, 0)]
		[Watch (5, 0)]
#endif
		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool Apply (ApplyCallback callback, object? info = null)
		{
			if (callback is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (callback));

			unsafe {
#if NET
				delegate* unmanaged<IntPtr, nint, IntPtr, IntPtr, byte> trampoline = &ApplyBlockHandler;
				using var block = new BlockLiteral (trampoline, callback, typeof (CGPDFArray), nameof (ApplyBlockHandler));
#else
				using var block = new BlockLiteral ();
				block.SetupBlockUnsafe (applyblock_handler, callback);
#endif
				var gc_handle = info is null ? default (GCHandle) : GCHandle.Alloc (info);
				try {
					return CGPDFArrayApplyBlock (Handle, &block, info is null ? IntPtr.Zero : GCHandle.ToIntPtr (gc_handle));
				} finally {
					if (info is not null)
						gc_handle.Free ();
				}
			}
		}
	}
}
