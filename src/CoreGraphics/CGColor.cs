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
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;

#if !COREBUILD
using CoreFoundation;
using Foundation;
#endif

namespace CoreGraphics {

	// CGColor.h
	public class CGColor : INativeObject
#if !COREBUILD
			, IDisposable
#endif
	{
#if !COREBUILD
		internal IntPtr handle;
		
		~CGColor ()
		{
			Dispose (false);
		}

		//
		// Never call from this class, so we need to take a ref
		//
		public CGColor (IntPtr handle)
		{
			this.handle = handle;
			CGColorRetain (handle);
		}

		[Preserve (Conditional=true)]
		internal CGColor (IntPtr handle, bool owns)
		{
			if (!owns)
				CGColorRetain (handle);

			this.handle = handle;
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorCreate (/* CGColorSpaceRef */ IntPtr space, /* CGFloat */ nfloat [] components);

		public CGColor (CGColorSpace colorspace, nfloat [] components)
		{
			if (components == null)
				global::ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (components));
			var colorspace_handle = colorspace.GetNonNullHandle (nameof (colorspace));
			
			handle = CGColorCreate (colorspace_handle, components);
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorCreateGenericGray (/* CGFloat */ nfloat gray, /* CGFloat */ nfloat alpha);

		public CGColor (nfloat gray, nfloat alpha)
		{
			handle = CGColorCreateGenericGray (gray, alpha);
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorCreateGenericRGB (/* CGFloat */ nfloat red, /* CGFloat */ nfloat green, /* CGFloat */ nfloat blue, /* CGFloat */ nfloat alpha);

		public CGColor (nfloat red, nfloat green, nfloat blue, nfloat alpha)
		{
			handle = CGColorCreateGenericRGB (red, green, blue, alpha);
		}

		public CGColor (nfloat red, nfloat green, nfloat blue)
		{
			handle = CGColorCreateGenericRGB (red, green, blue, 1.0f);
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorGetConstantColor (/* CFStringRef */ IntPtr colorName);

		public CGColor (string name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			
			using (var s = new CFString (name)){
				handle = CGColorGetConstantColor (s.Handle);
				if (handle == IntPtr.Zero)
					throw new ArgumentException ("name");
				CGColorRetain (handle);
			}
		}

#if !NET
		[iOS (14,0)][TV (14,0)][Watch (7,0)]
		[MacCatalyst (14,0)]
#else
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#endif
		public CGColor (CGConstantColor color)
		{
			var constant = color.GetConstant ();
			if (constant == null)
				throw new ArgumentNullException (nameof (color));
			handle = CGColorGetConstantColor (constant.Handle);
			if (handle == IntPtr.Zero)
				throw new ArgumentException (nameof (color));
			CGColorRetain (handle);
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorCreateWithPattern (/* CGColorSpaceRef */ IntPtr space, /* CGPatternRef */ IntPtr pattern, /* const CGFloat[] */ nfloat [] components);

		public CGColor (CGColorSpace colorspace, CGPattern pattern, nfloat [] components)
		{
			if (components == null)
				global::ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (components));
			var colorspace_handle = colorspace.GetNonNullHandle (nameof (colorspace));
			var pattern_handle = pattern.GetNonNullHandle (nameof (pattern));

			handle = CGColorCreateWithPattern (colorspace_handle, pattern_handle, components);
			if (handle == IntPtr.Zero)
				throw new ArgumentException ();
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorCreateCopyWithAlpha (/* CGColorRef */ IntPtr color, nfloat alpha);

		public CGColor (CGColor source, nfloat alpha)
		{
			if (source == null)
				throw new ArgumentNullException ("source");
			if (source.handle == IntPtr.Zero)
				throw new ObjectDisposedException ("source");
			
			handle = CGColorCreateCopyWithAlpha (source.handle, alpha);
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGColorEqualToColor (/* CGColorRef */ IntPtr color1, /* CGColorRef */ IntPtr color2);

		public static bool operator == (CGColor color1, CGColor color2)
		{
			return Object.Equals (color1, color2);
		}

		public static bool operator != (CGColor color1, CGColor color2)
		{
			return !Object.Equals (color1, color2);
		}

		public override int GetHashCode ()
		{
			return handle.GetHashCode ();
		}

		public override bool Equals (object o)
		{
			CGColor other = o as CGColor;
			if (other == null)
				return false;

			return CGColorEqualToColor (this.handle, other.handle);
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGColorGetNumberOfComponents (/* CGColorRef */ IntPtr color);

		public nint NumberOfComponents {
			get {
				return CGColorGetNumberOfComponents (handle);
			}
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static unsafe /* CGFloat* */ nfloat *CGColorGetComponents (/* CGColorRef */ IntPtr color);

		public nfloat [] Components {
			get {
				int n = (int) NumberOfComponents;
				nfloat [] result = new nfloat [n];
				unsafe {
					nfloat *cptr = CGColorGetComponents (handle);

					for (int i = 0; i < n; i++){
						result [i] = cptr [i];
					}
				}
				return result;
			}
		}

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGFloat */ nfloat CGColorGetAlpha (/* CGColorRef */ IntPtr color);

		public nfloat Alpha {
			get {
				return CGColorGetAlpha (handle);
			}
		}
		
		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorGetColorSpace (/* CGColorRef */ IntPtr color);

		public CGColorSpace ColorSpace {
			get {
				var ptr = CGColorGetColorSpace (handle);
				return ptr == IntPtr.Zero ? null : new CGColorSpace (ptr, false);
			}
		}
		
		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGPatternRef */ IntPtr CGColorGetPattern (/* CGColorRef */ IntPtr color);
		public CGPattern Pattern {
			get {
				var h = CGColorGetPattern (handle);
				// return `null`, not an invalid instance, if there's no pattern
				return h == IntPtr.Zero ? null : new CGPattern (h);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorRetain (/* CGColorRef */ IntPtr color);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGColorRelease (/* CGColorRef */ IntPtr color);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGColorRelease (handle);
				handle = IntPtr.Zero;
			}
		}

#if !NET
		[iOS (9,0)][Mac (10,11)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGColorRef __nullable */ IntPtr CGColorCreateCopyByMatchingToColorSpace (
			/* __nullable CGColorSpaceRef* */ IntPtr space, CGColorRenderingIntent intent,
			/* CGColorRef __nullable */ IntPtr color, /* __nullable CFDictionaryRef */ IntPtr options);

#if !NET
		[iOS (9,0)][Mac (10,11)]
#endif
		static public CGColor CreateByMatchingToColorSpace (CGColorSpace space, CGColorRenderingIntent intent,
			CGColor color, NSDictionary options)
		{
			var h = CGColorCreateCopyByMatchingToColorSpace (space == null ? IntPtr.Zero : space.Handle, intent,
				color == null ? IntPtr.Zero : color.Handle, options == null ? IntPtr.Zero : options.Handle);
			return h == IntPtr.Zero ? null : new CGColor (h, owns: true);
		}

#if !NET
		[Mac (10,15)]
		[iOS (13,0)]
		[TV (13,0)]
		[Watch (6,0)]
#else
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGColorRef* */ IntPtr CGColorCreateSRGB (nfloat red, nfloat green, nfloat blue, nfloat alpha);

#if !NET
		[Mac (10,15)]
		[iOS (13,0)]
		[TV (13,0)]
		[Watch (6,0)]
#else
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
#endif
		static public CGColor CreateSrgb (nfloat red, nfloat green, nfloat blue, nfloat alpha)
		{
			var h = CGColorCreateSRGB (red, green, blue, alpha);
			return h == IntPtr.Zero ? null : new CGColor (h, owns: true);
		}

#if !NET
		[Mac (10,15)]
		[iOS (13,0)]
		[TV (13,0)]
		[Watch (6,0)]
#else
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGColorRef* */ IntPtr CGColorCreateGenericGrayGamma2_2 (nfloat gray, nfloat alpha);

#if !NET
		[Mac (10,15)]
		[iOS (13,0)]
		[TV (13,0)]
		[Watch (6,0)]
#else
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("macos10.15")]
#endif
		static public CGColor CreateGenericGrayGamma2_2 (nfloat gray, nfloat alpha)
		{
			var h = CGColorCreateGenericGrayGamma2_2 (gray, alpha);
			return h == IntPtr.Zero ? null : new CGColor (h, owns: true);
		}

#if !NET
		[iOS (14,0)][TV (14,0)][Watch (7,0)][Mac (11,0)]
		[MacCatalyst (14,0)]
#else
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#endif
		[DllImport(Constants.CoreGraphicsLibrary)]
		static extern /* CGColorRef */ IntPtr CGColorCreateGenericCMYK (nfloat cyan, nfloat magenta, nfloat yellow, nfloat black, nfloat alpha);

#if !NET
		[iOS (14,0)][TV (14,0)][Watch (7,0)][Mac (11,0)]
		[MacCatalyst (14,0)]
#else
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#endif
		static public CGColor CreateCmyk (nfloat cyan, nfloat magenta, nfloat yellow, nfloat black, nfloat alpha)
		{
			var h = CGColorCreateGenericCMYK (cyan, magenta, yellow, black, alpha);
			return h == IntPtr.Zero ? null : new CGColor (h, owns: true);
		}

#if !NET
		[iOS (14,0)][TV (14,0)][Watch (7,0)][Mac (11,0)]
		[MacCatalyst (14,0)]
#else
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#endif
		[DllImport (Constants.AccessibilityLibrary)]
		static extern /* NSString */ IntPtr AXNameFromColor (/* CGColorRef */ IntPtr color);

#if !NET
		[iOS (14,0)][TV (14,0)][Watch (7,0)][Mac (11,0)]
		[MacCatalyst (14,0)]
#else
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#endif
		public string AXName => CFString.FromHandle (AXNameFromColor (handle));


#endif // !COREBUILD
	}
}
