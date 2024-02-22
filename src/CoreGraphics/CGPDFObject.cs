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

#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using CoreFoundation;
using System.Runtime.Versioning;

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
	// CGPDFObject.h
	public class CGPDFObject : INativeObject {
		public NativeHandle Handle { get; private set; }

		// The lifetime management of CGPDFObject (and CGPDFArray, CGPDFDictionary and CGPDFStream) are tied to
		// the containing CGPDFDocument, and not possible to handle independently, which is why this class
		// does not subclass NativeObject (there's no way to retain/release CGPDFObject instances). It's
		// also why this constructor doesn't have a 'bool owns' parameter: it's always owned by the containing CGPDFDocument.
#if NET
		internal CGPDFObject (NativeHandle handle)
#else
		public CGPDFObject (NativeHandle handle)
#endif
		{
			Handle = handle;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPDFObjectType CGPDFObjectGetType (/* CGPDFObjectRef */ IntPtr pdfobj);

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPDFObjectGetValue (/* CGPDFObjectRef */IntPtr pdfobj, CGPDFObjectType type, /* void* */ byte* value);

		[DllImport (Constants.CoreGraphicsLibrary, EntryPoint = "CGPDFObjectGetValue")]
		unsafe extern static byte CGPDFObjectGetNIntValue (/* CGPDFObjectRef */IntPtr pdfobj, CGPDFObjectType type, /* void* */ nint* value);

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPDFObjectGetValue (/* CGPDFObjectRef */IntPtr pdfobj, CGPDFObjectType type, /* void* */ nfloat* value);

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPDFObjectGetValue (/* CGPDFObjectRef */IntPtr pdfobj, CGPDFObjectType type, /* void* */ IntPtr* value);

		public CGPDFObjectType Type {
			get { return CGPDFObjectGetType (Handle); }
		}

		public bool IsNull {
			get { return Type == CGPDFObjectType.Null; }
		}

		public bool TryGetValue (out bool value)
		{
			byte b;
			bool rv;
			unsafe {
				rv = CGPDFObjectGetValue (Handle, CGPDFObjectType.Boolean, &b) != 0;
			}
			value = rv && b != 0;
			return rv;
		}

		public bool TryGetValue (out nint value)
		{
			value = default;
			unsafe {
				return CGPDFObjectGetNIntValue (Handle, CGPDFObjectType.Integer, (nint *) Unsafe.AsPointer<nint> (ref value)) != 0;
			}
		}

		public bool TryGetValue (out nfloat value)
		{
			value = default;
			unsafe {
				return CGPDFObjectGetValue (Handle, CGPDFObjectType.Real, (nfloat *) Unsafe.AsPointer<nfloat> (ref value)) != 0;
			}
		}

		public bool TryGetValue (out string? value)
		{
			IntPtr ip;
			bool rv;
			unsafe {
				rv = CGPDFObjectGetValue (Handle, CGPDFObjectType.String, &ip) != 0;
			}
			value = rv ? CGPDFString.ToString (ip) : null;
			return rv;
		}

		public bool TryGetValue (out CGPDFArray? value)
		{
			IntPtr ip;
			bool rv;
			unsafe {
				rv = CGPDFObjectGetValue (Handle, CGPDFObjectType.Array, &ip) != 0;
			}
			value = rv ? new CGPDFArray (ip) : null;
			return rv;
		}

		public bool TryGetValue (out CGPDFDictionary? value)
		{
			IntPtr ip;
			bool rv;
			unsafe {
				rv = CGPDFObjectGetValue (Handle, CGPDFObjectType.Dictionary, &ip) != 0;
			}
			value = rv ? new CGPDFDictionary (ip) : null;
			return rv;
		}

		public bool TryGetValue (out CGPDFStream? value)
		{
			IntPtr ip;
			bool rv;
			unsafe {
				rv = CGPDFObjectGetValue (Handle, CGPDFObjectType.Stream, &ip) != 0;
			}
			value = rv ? new CGPDFStream (ip) : null;
			return rv;
		}

		public bool TryGetName (out string? name)
		{
			IntPtr ip;
			bool rv;
			unsafe {
				rv = CGPDFObjectGetValue (Handle, CGPDFObjectType.Name, &ip) != 0;
			}
			name = rv ? Marshal.PtrToStringAnsi (ip) : null;
			return rv;
		}

		internal static object? FromHandle (IntPtr handle)
		{
			IntPtr ip;

			var type = CGPDFObjectGetType (handle);
			switch (type) {
			case CGPDFObjectType.Null:
				return null;

			case CGPDFObjectType.Boolean:
				unsafe {
					byte b;
					if (CGPDFObjectGetValue (handle, type, &b) != 0)
						return b != 0;
				}
				return null;

			case CGPDFObjectType.Integer:
				unsafe {
					nint i;
					if (CGPDFObjectGetNIntValue (handle, type, &i) != 0)
						return i;
				}
				return null;

			case CGPDFObjectType.Real:
				unsafe {
					nfloat f;
					if (CGPDFObjectGetValue (handle, type, &f) != 0)
						return f;
				}
				return null;

			case CGPDFObjectType.Name:
				unsafe {
					if (CGPDFObjectGetValue (handle, type, &ip) != 0)
						return Marshal.PtrToStringAnsi (ip);
				}
				return null;

			case CGPDFObjectType.String:
				unsafe {
					if (CGPDFObjectGetValue (handle, type, &ip) != 0)
						return CGPDFString.ToString (ip);
				}
				return null;

			case CGPDFObjectType.Array:
				unsafe {
					if (CGPDFObjectGetValue (handle, type, &ip) != 0)
						return new CGPDFArray (ip);
				}
				return null;

			case CGPDFObjectType.Dictionary:
				unsafe {
					if (CGPDFObjectGetValue (handle, type, &ip) != 0)
						return new CGPDFDictionary (ip);
				}
				return null;

			case CGPDFObjectType.Stream:
				unsafe {
					if (CGPDFObjectGetValue (handle, type, &ip) != 0)
						return new CGPDFStream (ip);
				}
				return null;
			}
			return null;
		}
	}
}
