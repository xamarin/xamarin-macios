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
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using CoreFoundation;

namespace CoreGraphics {

	// CGPDFDictionary.h
	public class CGPDFDictionary : INativeObject {
		internal IntPtr handle;

		public IntPtr Handle {
			get { return handle; }
		}

		/* invoked by marshallers */
		public CGPDFDictionary (IntPtr handle)
		{
			this.handle = handle;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGPDFDictionaryGetCount (/* CGPDFDictionaryRef */ IntPtr dict);
		
		public int Count {
			get {
				return (int) CGPDFDictionaryGetCount (handle);
			}
		}

		// CGPDFBoolean -> unsigned char -> CGPDFObject.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDictionaryGetBoolean (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ string key, /* CGPDFBoolean* */ out bool value);

		public bool GetBoolean (string key, out bool result)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			return CGPDFDictionaryGetBoolean (handle, key, out result);
		}

		// CGPDFInteger -> long int so 32/64 bits -> CGPDFObject.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDictionaryGetInteger (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ string key, /* CGPDFInteger* */ out nint value);

		public bool GetInt (string key, out nint result)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			return CGPDFDictionaryGetInteger (handle, key, out result);
		}

		// CGPDFReal -> CGFloat -> CGPDFObject.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDictionaryGetNumber (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ string key, /* CGPDFReal* */ out nfloat value);

		public bool GetFloat (string key, out nfloat result)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			return CGPDFDictionaryGetNumber (handle, key, out result);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDictionaryGetName (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ string key, /* const char ** */ out IntPtr value);

		public bool GetName (string key, out string result)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			IntPtr res;
			var r = CGPDFDictionaryGetName (handle, key, out res);
			result = r ? Marshal.PtrToStringAnsi (res) : null;
			return r;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDictionaryGetDictionary (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ string key, /* CGPDFDictionaryRef* */ out IntPtr result);

		public bool GetDictionary (string key, out CGPDFDictionary result)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			IntPtr res;
			var r = CGPDFDictionaryGetDictionary (handle, key, out res);
			result = r ? new CGPDFDictionary (res) : null;
			return r;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDictionaryGetStream (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ string key, /* CGPDFStreamRef* */ out IntPtr value);

		public bool GetStream (string key, out CGPDFStream result)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			IntPtr ptr;
			var r = CGPDFDictionaryGetStream (handle, key, out ptr); 
			result = r ? new CGPDFStream (ptr) : null;
			return r;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDictionaryGetArray (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ string key, /* CGPDFArrayRef* */ out IntPtr value);

		public bool GetArray (string key, out CGPDFArray array)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			IntPtr ptr;
			var r = CGPDFDictionaryGetArray (handle, key, out ptr);
			array = r ? new CGPDFArray (ptr) : null;
			return r;
		}

		// CGPDFDictionaryApplierFunction
		delegate void ApplierFunction (/* const char* */ string key, /* CGPDFObjectRef */ IntPtr value, /* void* */ IntPtr info);
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFDictionaryApplyFunction (/* CGPDFDictionaryRef */ IntPtr dic, ApplierFunction function, /* void* */ IntPtr info);

		static readonly ApplierFunction applyblock_handler = ApplyBridge;

		public delegate void ApplyCallback (string key, object value, object info);
#if !MONOMAC
		[MonoPInvokeCallback (typeof (ApplierFunction))]
#endif
		static void ApplyBridge (string key, IntPtr pdfObject, IntPtr info)
		{
			var data = (Tuple<ApplyCallback, object>) GCHandle.FromIntPtr (info).Target;
			var callback = data.Item1;

			callback (key, CGPDFObject.FromHandle (pdfObject), data.Item2);
		}

		public void Apply (ApplyCallback callback, object info = null)
		{
			var data = new Tuple<ApplyCallback, object> (callback, info);
			var gch = GCHandle.Alloc (data);
			try {
				CGPDFDictionaryApplyFunction (Handle, applyblock_handler, GCHandle.ToIntPtr (gch));
			} finally {
				gch.Free ();
			}
		}

#if !MONOMAC
		[MonoPInvokeCallback (typeof (ApplierFunction))]
#endif
		static void ApplyBridge2 (string key, IntPtr pdfObject, IntPtr info)
		{
			var callback = (Action<string,CGPDFObject>) GCHandle.FromIntPtr (info).Target;

			callback (key, new CGPDFObject (pdfObject));
		}

		public void Apply (Action<string,CGPDFObject> callback)
		{
			GCHandle gch = GCHandle.Alloc (callback);
			CGPDFDictionaryApplyFunction (Handle, ApplyBridge2, GCHandle.ToIntPtr (gch));
			gch.Free ();
		}

		// CGPDFDictionary.h

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDictionaryGetString (/* CGPDFDictionaryRef */ IntPtr dict, /* const char* */ string key, /* CGPDFStringRef* */ out IntPtr value);

		public bool GetString (string key, out string result)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			IntPtr res;
			var r = CGPDFDictionaryGetString (handle, key, out res);
			result = r ? CGPDFString.ToString (res) : null;
			return r;
		}
	}
}
