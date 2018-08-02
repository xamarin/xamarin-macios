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
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using CoreFoundation;

namespace CoreGraphics {

	// CGPDFArray.h
	public class CGPDFArray : INativeObject {
		internal IntPtr handle;

		public IntPtr Handle {
			get { return handle; }
		}

		/* invoked by marshallers */
		public CGPDFArray (IntPtr handle)
		{
			this.handle = handle;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGPDFArrayGetCount (/* CGPDFArrayRef */ IntPtr array);
		
		public nint Count {
			get {
				return CGPDFArrayGetCount (handle);
			}
		}

		// CGPDFBoolean -> unsigned char -> CGPDFObject.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFArrayGetBoolean (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFBoolean* */ out bool value);

		public bool GetBoolean (nint idx, out bool result)
		{
			return CGPDFArrayGetBoolean (handle, idx, out result);
		}

#if !XAMCORE_4_0
		public bool GetBoolean (int idx, out bool result)
		{
			return CGPDFArrayGetBoolean (handle, idx, out result);
		}
#endif

		// CGPDFInteger -> long int 32/64 bits -> CGPDFObject.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFArrayGetInteger (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFInteger* */ out nint value);

		public bool GetInt (nint idx, out nint result)
		{
			return CGPDFArrayGetInteger (handle, idx, out result);
		}

#if !XAMCORE_4_0
		public bool GetInt (int idx, out nint result)
		{
			return CGPDFArrayGetInteger (handle, idx, out result);
		}
#endif

		// CGPDFReal -> CGFloat -> CGPDFObject.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFArrayGetNumber (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFReal* */ out nfloat value);

		public bool GetFloat (nint idx, out nfloat result)
		{
			return CGPDFArrayGetNumber (handle, idx, out result);
		}

#if !XAMCORE_4_0
		public bool GetFloat (int idx, out nfloat result)
		{
			return CGPDFArrayGetNumber (handle, idx, out result);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFArrayGetName (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* const char** */ out IntPtr value);

		public bool GetName (nint idx, out string result)
		{
			IntPtr res;
			var r = CGPDFArrayGetName (handle, idx, out res);
			result = r ? Marshal.PtrToStringAnsi (res) : null;
			return r;
		}

#if !XAMCORE_4_0
		public bool GetName (int idx, out string result)
		{
			return GetName ((nint) idx, out result);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFArrayGetDictionary (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFDictionaryRef* */ out IntPtr value);

		public bool GetDictionary (nint idx, out CGPDFDictionary result)
		{
			IntPtr res;
			var r = CGPDFArrayGetDictionary (handle, idx, out res);
			result = r ? new CGPDFDictionary (res) : null;
			return r;
		}

#if !XAMCORE_4_0
		public bool GetDictionary (int idx, out CGPDFDictionary result)
		{
			return GetDictionary ((nint) idx, out result);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFArrayGetStream (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFStreamRef* */ out IntPtr value);

		public bool GetStream (nint idx, out CGPDFStream result)
		{
			IntPtr ptr;
			var r = CGPDFArrayGetStream (handle, idx, out ptr); 
			result = r ? new CGPDFStream (ptr) : null;
			return r;
		}

#if !XAMCORE_4_0
		public bool GetStream (int idx, out CGPDFStream result)
		{
			return GetStream ((nint) idx, out result);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFArrayGetArray (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFArrayRef* */ out IntPtr value);

		public bool GetArray (nint idx, out CGPDFArray array)
		{
			IntPtr ptr;
			var r = CGPDFArrayGetArray (handle, idx, out ptr);
			array = r ? new CGPDFArray (ptr) : null;
			return r;
		}

#if !XAMCORE_4_0
		public bool GetArray (int idx, out CGPDFArray array)
		{
			return GetArray ((nint) idx, out array);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFArrayGetString (/* CGPDFArrayRef */ IntPtr array, /* size_t */ nint index, /* CGPDFStringRef* */ out IntPtr value);

		public bool GetString (nint idx, out string result)
		{
			IntPtr res;
			var r = CGPDFArrayGetString (handle, idx, out res);
			result = r ? CGPDFString.ToString (res) : null;
			return r;
		}

#if !XAMCORE_4_0
		public bool GetString (int idx, out string result)
		{
			return GetString ((nint) idx, out result);
		}
#endif

		delegate bool ApplyBlockHandlerDelegate (IntPtr block, nint index, IntPtr value, IntPtr info);
		static readonly ApplyBlockHandlerDelegate applyblock_handler = ApplyBlockHandler;

#if !MONOMAC
		[MonoPInvokeCallback (typeof (ApplyBlockHandlerDelegate))]
#endif
		static unsafe bool ApplyBlockHandler (IntPtr block, nint index, IntPtr value, IntPtr info)
		{
			var del = (ApplyCallback) ((BlockLiteral *) block)->Target;
			if (del != null)
				return del (index, new CGPDFObject (value), info);

			return false;
		}

		public delegate bool ApplyCallback (nint index, CGPDFObject value, object info);

		[DllImport (Constants.CoreGraphicsLibrary)]
		[iOS (12, 0)][Mac (10, 14)][TV (12, 0)][Watch (5, 0)]
		extern static bool CGPDFArrayApplyBlock (/* CGPDFArrayRef */ IntPtr array, /* CGPDFArrayApplierBlock */ ref BlockLiteral block, /* void* */ IntPtr info);

		[iOS (12, 0)][Mac (10, 14)][TV (12, 0)][Watch (5, 0)]
		[BindingImpl (BindingImplOptions.Optimizable)]
		public bool Apply (ApplyCallback callback, object info = null)
		{
			if (callback == null)
				throw new ArgumentNullException (nameof (callback));

			BlockLiteral block_handler = new BlockLiteral ();
			block_handler.SetupBlockUnsafe (applyblock_handler, callback);

			try {
				return CGPDFArrayApplyBlock (handle, ref block_handler, info);
			} finally {
				block_handler.CleanupBlock ();
			}
		}
	}
}
