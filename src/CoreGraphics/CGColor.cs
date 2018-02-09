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
				throw new ArgumentNullException ("components");
			if (colorspace == null)
				throw new ArgumentNullException ("colorspace");
			if (colorspace.handle == IntPtr.Zero)
				throw new ObjectDisposedException ("colorspace");
			
			handle = CGColorCreate (colorspace.handle, components);
		}

#if !XAMCORE_3_0 || MONOMAC
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
				handle = CGColorGetConstantColor (s.handle);
				if (handle == IntPtr.Zero)
					throw new ArgumentException ("name");
				CGColorRetain (handle);
			}
		}
#endif

		[DllImport(Constants.CoreGraphicsLibrary)]
		extern static /* CGColorRef */ IntPtr CGColorCreateWithPattern (/* CGColorSpaceRef */ IntPtr space, /* CGPatternRef */ IntPtr pattern, /* const CGFloat[] */ nfloat [] components);

		public CGColor (CGColorSpace colorspace, CGPattern pattern, nfloat [] components)
		{
			if (colorspace == null)
				throw new ArgumentNullException ("colorspace");
			if (colorspace.handle == IntPtr.Zero)
				throw new ObjectDisposedException ("colorspace");
			if (pattern == null)
				throw new ArgumentNullException ("pattern");
			if (components == null)
				throw new ArgumentNullException ("components");

			handle = CGColorCreateWithPattern (colorspace.handle, pattern.handle, components);
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

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGColorRef __nullable */ IntPtr CGColorCreateCopyByMatchingToColorSpace (
			/* __nullable CGColorSpaceRef* */ IntPtr space, CGColorRenderingIntent intent,
			/* CGColorRef __nullable */ IntPtr color, /* __nullable CFDictionaryRef */ IntPtr options);

		static CGColor CreateByMatchingToColorSpace (CGColorSpace space, CGColorRenderingIntent intent,
			CGColor color, NSDictionary options)
		{
			var h = CGColorCreateCopyByMatchingToColorSpace (space == null ? IntPtr.Zero : space.Handle, intent,
				color == null ? IntPtr.Zero : color.Handle, options == null ? IntPtr.Zero : options.Handle);
			return h == IntPtr.Zero ? null : new CGColor (h);
		}
#endif // !COREBUILD
	}
}
