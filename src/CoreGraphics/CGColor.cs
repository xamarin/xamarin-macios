// 
// CGColor.cs: Implements the managed CGColor
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2014 Xamarin Inc.
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

using ObjCRuntime;
using CoreFoundation;
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
	// CGColor.h
	public class CGColor : NativeObject {
#if !COREBUILD
#if !NET
		public CGColor (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGColor (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		protected internal override void Retain ()
		{
			CGColorRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGColorRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern unsafe static /* CGColorRef */ IntPtr CGColorCreate (/* CGColorSpaceRef */ IntPtr space, /* CGFloat */ nfloat* components);

		static IntPtr Create (CGColorSpace colorspace, nfloat [] components)
		{
			if (components is null)
				global::ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (components));
			var colorspace_handle = colorspace.GetNonNullHandle (nameof (colorspace));

			unsafe {
				fixed (nfloat* componentsPtr = components) {
					return CGColorCreate (colorspace_handle, componentsPtr);
				}
			}
		}

		public CGColor (CGColorSpace colorspace, nfloat [] components)
			: base (Create (colorspace, components), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorCreateGenericGray (/* CGFloat */ nfloat gray, /* CGFloat */ nfloat alpha);

		public CGColor (nfloat gray, nfloat alpha)
			: base (CGColorCreateGenericGray (gray, alpha), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorCreateGenericRGB (/* CGFloat */ nfloat red, /* CGFloat */ nfloat green, /* CGFloat */ nfloat blue, /* CGFloat */ nfloat alpha);

		public CGColor (nfloat red, nfloat green, nfloat blue, nfloat alpha)
			: base (CGColorCreateGenericRGB (red, green, blue, alpha), true)
		{
		}

		public CGColor (nfloat red, nfloat green, nfloat blue)
			: base (CGColorCreateGenericRGB (red, green, blue, 1.0f), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorGetConstantColor (/* CFStringRef */ IntPtr colorName);

		static IntPtr Create (string name)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));

			var nameHandle = CFString.CreateNative (name);
			try {
				var handle = CGColorGetConstantColor (nameHandle);
				if (handle == IntPtr.Zero)
					throw new ArgumentException (nameof (name));
				CGColorRetain (handle);
				return handle;
			} finally {
				CFString.ReleaseNative (nameHandle);
			}

		}

		public CGColor (string name)
			: base (Create (name), true)
		{
		}

		static IntPtr Create (CGConstantColor color)
		{
			var constant = color.GetConstant ();
			if (constant is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (color));
			var handle = CGColorGetConstantColor (constant.Handle);
			if (handle == IntPtr.Zero)
				throw new ArgumentException (nameof (color));
			CGColorRetain (handle);
			return handle;
		}

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#else
		[iOS (14, 0)]
		[TV (14, 0)]
		[Watch (7, 0)]
		[MacCatalyst (14, 0)]
#endif
		public CGColor (CGConstantColor color)
			: base (Create (color), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern unsafe static /* CGColorRef */ IntPtr CGColorCreateWithPattern (/* CGColorSpaceRef */ IntPtr space, /* CGPatternRef */ IntPtr pattern, /* const CGFloat[] */ nfloat* components);

		static IntPtr Create (CGColorSpace colorspace, CGPattern pattern, nfloat [] components)
		{
			if (components is null)
				global::ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (components));
			var colorspace_handle = colorspace.GetNonNullHandle (nameof (colorspace));
			var pattern_handle = pattern.GetNonNullHandle (nameof (pattern));

			unsafe {
				fixed (nfloat* componentsPtr = components) {
					var handle = CGColorCreateWithPattern (colorspace_handle, pattern_handle, componentsPtr);
					if (handle == IntPtr.Zero)
						throw new ArgumentException ();
					return handle;
				}
			}
		}

		public CGColor (CGColorSpace colorspace, CGPattern pattern, nfloat [] components)
			: base (Create (colorspace, pattern, components), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorCreateCopyWithAlpha (/* CGColorRef */ IntPtr color, nfloat alpha);

		static IntPtr Create (CGColor source, nfloat alpha)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));
			return CGColorCreateCopyWithAlpha (source.GetCheckedHandle (), alpha);
		}

		public CGColor (CGColor source, nfloat alpha)
			: base (Create (source, alpha), true)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGColorEqualToColor (/* CGColorRef */ IntPtr color1, /* CGColorRef */ IntPtr color2);

		public static bool operator == (CGColor color1, CGColor color2)
		{
			if (color1 is null)
				return color2 is null;
			return color1.Equals (color2);
		}

		public static bool operator != (CGColor color1, CGColor color2)
		{
			if (color1 is null)
				return color2 is not null;
			return !color1.Equals (color2);
		}

		public override int GetHashCode ()
		{
			// looks weird but it's valid
			// using the Handle property would not be since there's a special function for equality
			// see Remarks in https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode?view=net-6.0
			return 0;
		}

		public override bool Equals (object? o)
		{
			var other = o as CGColor;
			if (other is null)
				return false;

			return CGColorEqualToColor (this.Handle, other.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGColorGetNumberOfComponents (/* CGColorRef */ IntPtr color);

		public nint NumberOfComponents {
			get {
				return CGColorGetNumberOfComponents (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe /* CGFloat* */ nfloat* CGColorGetComponents (/* CGColorRef */ IntPtr color);

		public nfloat [] Components {
			get {
				int n = (int) NumberOfComponents;
				nfloat [] result = new nfloat [n];
				unsafe {
					nfloat* cptr = CGColorGetComponents (Handle);

					for (int i = 0; i < n; i++) {
						result [i] = cptr [i];
					}
				}
				return result;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGFloat */ nfloat CGColorGetAlpha (/* CGColorRef */ IntPtr color);

		public nfloat Alpha {
			get {
				return CGColorGetAlpha (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorGetColorSpace (/* CGColorRef */ IntPtr color);

		public CGColorSpace? ColorSpace {
			get {
				var ptr = CGColorGetColorSpace (Handle);
				return ptr == IntPtr.Zero ? null : new CGColorSpace (ptr, false);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPatternRef */ IntPtr CGColorGetPattern (/* CGColorRef */ IntPtr color);
		public CGPattern? Pattern {
			get {
				var h = CGColorGetPattern (Handle);
				// return `null`, not an invalid instance, if there's no pattern
				return h == IntPtr.Zero ? null : new CGPattern (h, false);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorRetain (/* CGColorRef */ IntPtr color);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGColorRelease (/* CGColorRef */ IntPtr color);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGColorRef __nullable */ IntPtr CGColorCreateCopyByMatchingToColorSpace (
			/* __nullable CGColorSpaceRef* */ IntPtr space, CGColorRenderingIntent intent,
			/* CGColorRef __nullable */ IntPtr color, /* __nullable CFDictionaryRef */ IntPtr options);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		static public CGColor? CreateByMatchingToColorSpace (CGColorSpace space, CGColorRenderingIntent intent,
			CGColor color, NSDictionary options)
		{
			var h = CGColorCreateCopyByMatchingToColorSpace (space.GetHandle (), intent, color.GetHandle (), options.GetHandle ());
			return h == IntPtr.Zero ? null : new CGColor (h, owns: true);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGColorRef* */ IntPtr CGColorCreateSRGB (nfloat red, nfloat green, nfloat blue, nfloat alpha);

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
#endif
		static public CGColor? CreateSrgb (nfloat red, nfloat green, nfloat blue, nfloat alpha)
		{
			var h = CGColorCreateSRGB (red, green, blue, alpha);
			return h == IntPtr.Zero ? null : new CGColor (h, owns: true);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGColorRef* */ IntPtr CGColorCreateGenericGrayGamma2_2 (nfloat gray, nfloat alpha);

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
		[Watch (6, 0)]
#endif
		static public CGColor? CreateGenericGrayGamma2_2 (nfloat gray, nfloat alpha)
		{
			var h = CGColorCreateGenericGrayGamma2_2 (gray, alpha);
			return h == IntPtr.Zero ? null : new CGColor (h, owns: true);
		}

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (14, 0)]
		[TV (14, 0)]
		[Watch (7, 0)]
		[MacCatalyst (14, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGColorRef */ IntPtr CGColorCreateGenericCMYK (nfloat cyan, nfloat magenta, nfloat yellow, nfloat black, nfloat alpha);

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (14, 0)]
		[TV (14, 0)]
		[Watch (7, 0)]
		[MacCatalyst (14, 0)]
#endif
		static public CGColor? CreateCmyk (nfloat cyan, nfloat magenta, nfloat yellow, nfloat black, nfloat alpha)
		{
			var h = CGColorCreateGenericCMYK (cyan, magenta, yellow, black, alpha);
			return h == IntPtr.Zero ? null : new CGColor (h, owns: true);
		}

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (14, 0)]
		[TV (14, 0)]
		[Watch (7, 0)]
		[MacCatalyst (14, 0)]
#endif
		[DllImport (Constants.AccessibilityLibrary)]
		static extern /* NSString */ IntPtr AXNameFromColor (/* CGColorRef */ IntPtr color);

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (14, 0)]
		[TV (14, 0)]
		[Watch (7, 0)]
		[MacCatalyst (14, 0)]
#endif
		public string? AXName => CFString.FromHandle (AXNameFromColor (Handle));


#endif // !COREBUILD
	}
}
