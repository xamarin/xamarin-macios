// 
// CGGradient.cs: Implements the managed CGGradient
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2012-2014 Xamarin Inc.
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

using ObjCRuntime;
using CoreFoundation;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {

	// uint32_t -> CGGradient.h
	[Flags]
	public enum CGGradientDrawingOptions : uint {
		None = 0,
		DrawsBeforeStartLocation = (1 << 0),
		DrawsAfterEndLocation = (1 << 1)
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGGradient : NativeObject {
#if !COREBUILD
		[Preserve (Conditional = true)]
		internal CGGradient (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGGradientRef */ IntPtr CGGradientRetain (/* CGGradientRef */ IntPtr gradient);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGGradientRelease (/* CGGradientRef */ IntPtr gradient);

		protected internal override void Retain ()
		{
			CGGradientRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGGradientRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe /* CGGradientRef __nullable */ IntPtr CGGradientCreateWithColorComponents (
			/* CGColorSpaceRef __nullable */ IntPtr colorspace, /* const CGFloat* __nullable */ nfloat* components,
			/* const CGFloat* __nullable */ nfloat* locations, /* size_t */ nint count);

		static IntPtr Create (CGColorSpace colorspace, nfloat [] components, nfloat []? locations)
		{
			// those parameters are __nullable but would return a `nil` instance back,
			// which is not something we can handle nicely from a .NET constructor
			if (colorspace is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (colorspace));
			if (components is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (components));

			unsafe {
				fixed (nfloat* componentsPtr = components, locationsPtr = locations) {
					return CGGradientCreateWithColorComponents (colorspace.GetCheckedHandle (), componentsPtr, locationsPtr, components.Length / (colorspace.Components + 1));
				}
			}
		}

		public CGGradient (CGColorSpace colorspace, nfloat [] components, nfloat []? locations)
			: base (Create (colorspace, components, locations), true)
		{
		}

		static IntPtr Create (CGColorSpace colorspace, nfloat [] components)
		{
			// those parameters are __nullable but would return a `nil` instance back,
			// which is not something we can handle nicely from a .NET constructor
			if (colorspace is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (colorspace));
			if (components is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (components));

			unsafe {
				fixed (nfloat* componentsPtr = components) {
					return CGGradientCreateWithColorComponents (colorspace.GetCheckedHandle (), componentsPtr, null, components.Length / (colorspace.Components + 1));
				}
			}
		}

		public CGGradient (CGColorSpace colorspace, nfloat [] components)
			: base (Create (colorspace, components), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe /* CGGradientRef __nullable */ IntPtr CGGradientCreateWithColors (
			/* CGColorSpaceRef __nullable */ IntPtr space, /* CFArrayRef __nullable */ IntPtr colors,
			/* const CGFloat* __nullable */ nfloat* locations);

		static IntPtr Create (CGColorSpace? colorspace, CGColor [] colors, nfloat []? locations)
		{
			// colors is __nullable but would return a `nil` instance back,
			// which is not something we can handle nicely from a .NET constructor
			if (colors is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (colors));

			using (var array = CFArray.FromNativeObjects (colors)) {
				unsafe {
					fixed (nfloat* locationsPtr = locations) {
						return CGGradientCreateWithColors (colorspace.GetHandle (), array.Handle, locationsPtr);
					}
				}
			}
		}

		public CGGradient (CGColorSpace colorspace, CGColor [] colors, nfloat []? locations)
			: base (Create (colorspace, colors, locations), true)
		{
		}

		static IntPtr Create (CGColorSpace? colorspace, CGColor [] colors)
		{
			if (colors is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (colors));

			using (var array = CFArray.FromNativeObjects (colors)) {
				unsafe {
					return CGGradientCreateWithColors (colorspace.GetHandle (), array.Handle, null);
				}
			}
		}

		public CGGradient (CGColorSpace? colorspace, CGColor [] colors)
			: base (Create (colorspace, colors), true)
		{
		}
#endif // !COREBUILD
	}
}
