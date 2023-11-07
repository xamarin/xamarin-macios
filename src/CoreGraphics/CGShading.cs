// 
// CGShading.cs: Implements the managed CGShading
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2011-2014 Xamarin Inc
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

using CoreFoundation;
using ObjCRuntime;
using Foundation;

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
	// CGShading.h
	public class CGShading : NativeObject {
#if !COREBUILD
#if !NET
		public CGShading (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGShading (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		protected internal override void Retain ()
		{
			CGShadingRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGShadingRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGShadingRef */ IntPtr CGShadingCreateAxial (/* CGColorSpaceRef */ IntPtr space,
			CGPoint start, CGPoint end, /* CGFunctionRef */ IntPtr functionHandle, [MarshalAs (UnmanagedType.I1)] bool extendStart, [MarshalAs (UnmanagedType.I1)] bool extendEnd);

		public static CGShading CreateAxial (CGColorSpace colorspace, CGPoint start, CGPoint end, CGFunction function, bool extendStart, bool extendEnd)
		{
			if (colorspace is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (colorspace));
			if (function is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (function));

			return new CGShading (CGShadingCreateAxial (colorspace.GetCheckedHandle (), start, end, function.GetCheckedHandle (), extendStart, extendEnd), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGShadingRef */ IntPtr CGShadingCreateRadial (/* CGColorSpaceRef */ IntPtr space,
			CGPoint start, /* CGFloat */ nfloat startRadius, CGPoint end, /* CGFloat */ nfloat endRadius,
			/* CGFunctionRef */ IntPtr function, [MarshalAs (UnmanagedType.I1)] bool extendStart, [MarshalAs (UnmanagedType.I1)] bool extendEnd);

		public static CGShading CreateRadial (CGColorSpace colorspace, CGPoint start, nfloat startRadius, CGPoint end, nfloat endRadius,
							  CGFunction function, bool extendStart, bool extendEnd)
		{
			if (colorspace is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (colorspace));
			if (function is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (function));

			return new CGShading (CGShadingCreateRadial (colorspace.GetCheckedHandle (), start, startRadius, end, endRadius,
									 function.GetCheckedHandle (), extendStart, extendEnd), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGShadingRef */ IntPtr CGShadingRelease (/* CGShadingRef */ IntPtr shading);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGShadingRef */ IntPtr CGShadingRetain (/* CGShadingRef */ IntPtr shading);
#endif // !COREBUILD
	}
}
