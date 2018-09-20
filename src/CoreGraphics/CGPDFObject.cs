// 
// CGPDFObject
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

	// untyped enum -> CGPDFObject.h
	public enum CGPDFObjectType {
		Null = 1,
		Boolean,
		Integer,
		Real,
		Name,
		String,
		Array,
		Dictionary,
		Stream
	};

	// CGPDFObject.h
	public class CGPDFObject : INativeObject {

		public IntPtr Handle { get; private set; }

		/* invoked by marshallers */
		public CGPDFObject (IntPtr handle)
		{
			Handle = handle;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPDFObjectType CGPDFObjectGetType (/* CGPDFObjectRef */ IntPtr pdfobj);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFObjectGetValue (/* CGPDFObjectRef */IntPtr pdfobj, CGPDFObjectType type, /* void* */ out byte value);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFObjectGetValue (/* CGPDFObjectRef */IntPtr pdfobj, CGPDFObjectType type, /* void* */ out nint value);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFObjectGetValue (/* CGPDFObjectRef */IntPtr pdfobj, CGPDFObjectType type, /* void* */ out nfloat value);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFObjectGetValue (/* CGPDFObjectRef */IntPtr pdfobj, CGPDFObjectType type, /* void* */ out IntPtr value);

		public CGPDFObjectType Type {
			get { return CGPDFObjectGetType (Handle); }
		}

		public bool IsNull {
			get { return Type == CGPDFObjectType.Null; }
		}

		public bool TryGetValue (out bool value)
		{
			byte b;
			if (CGPDFObjectGetValue (Handle, CGPDFObjectType.Boolean, out b)) {
				value = b != 0;
				return true;
			} else {
				value = false;
				return false;
			}
		}

		public bool TryGetValue (out nint value)
		{
			return CGPDFObjectGetValue (Handle, CGPDFObjectType.Integer, out value);
		}

		public bool TryGetValue (out nfloat value)
		{
			return CGPDFObjectGetValue (Handle, CGPDFObjectType.Real, out value);
		}

		public bool TryGetValue (out string value)
		{
			IntPtr ip;
			if (CGPDFObjectGetValue (Handle, CGPDFObjectType.String, out ip)) {
				value = CGPDFString.ToString (ip);
				return true;
			} else {
				value = null;
				return false;
			}
		}

		public bool TryGetValue (out CGPDFArray value)
		{
			IntPtr ip;
			if (CGPDFObjectGetValue (Handle, CGPDFObjectType.Array, out ip)) {
				value = new CGPDFArray (ip);
				return true;
			} else {
				value = null;
				return false;
			}
		}

		public bool TryGetValue (out CGPDFDictionary value)
		{
			IntPtr ip;
			if (CGPDFObjectGetValue (Handle, CGPDFObjectType.Dictionary, out ip)) {
				value = new CGPDFDictionary (ip);
				return true;
			} else {
				value = null;
				return false;
			}
		}

		public bool TryGetValue (out CGPDFStream value)
		{
			IntPtr ip;
			if (CGPDFObjectGetValue (Handle, CGPDFObjectType.Stream, out ip)) {
				value = new CGPDFStream (ip);
				return true;
			} else {
				value = null;
				return false;
			}
		}

		public bool TryGetName (out string name)
		{
			IntPtr ip;
			if (CGPDFObjectGetValue (Handle, CGPDFObjectType.Name, out ip)) {
				name = Marshal.PtrToStringAnsi (ip);
				return true;
			} else {
				name = null;
				return false;
			}
		}

		internal static object FromHandle (IntPtr handle)
		{
			IntPtr ip;

			var type = CGPDFObjectGetType (handle);
			switch (type) {
			case CGPDFObjectType.Null:
				return null;

			case CGPDFObjectType.Boolean:
				byte b;
				if (CGPDFObjectGetValue (handle, type, out b))
					return b != 0;
				return null;

			case CGPDFObjectType.Integer:
				nint i;
				if (CGPDFObjectGetValue (handle, type, out i))
					return i;
				return null;

			case CGPDFObjectType.Real:
				nfloat f;
				if (CGPDFObjectGetValue (handle, type, out f))
					return f;
				return null;

			case CGPDFObjectType.Name:
				if (CGPDFObjectGetValue (handle, type, out ip))
					return Marshal.PtrToStringAnsi (ip);
				return null;

			case CGPDFObjectType.String:
				if (CGPDFObjectGetValue (handle, type, out ip))
					return CGPDFString.ToString (ip);
				return null;

			case CGPDFObjectType.Array:
				if (CGPDFObjectGetValue (handle, type, out ip))
					return new CGPDFArray (ip);
				return null;

			case CGPDFObjectType.Dictionary:
				if (CGPDFObjectGetValue (handle, type, out ip))
					return new CGPDFDictionary (ip);
				return null;

			case CGPDFObjectType.Stream:
				if (CGPDFObjectGetValue (handle, type, out ip))
					return new CGPDFStream (ip);
				return null;
			}
			return null;
		}
	}
}