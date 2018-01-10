// 
// CGImage.cs: Implements the managed CGImage
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
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;

namespace CoreGraphics {

#if MONOMAC
	// uint32_t -> CGWindow.h (OSX SDK only)
	[Flags]	
	public enum CGWindowImageOption : uint {
		Default             = 0,
		BoundsIgnoreFraming = (1 << 0),
		ShouldBeOpaque      = (1 << 1),
		OnlyShadows         = (1 << 2)
	}

	// uint32_t -> CGWindow.h (OSX SDK only)
	[Flags]
	public enum CGWindowListOption : uint {
		All                 = 0,
		OnScreenOnly        = (1 << 0),
		OnScreenAboveWindow = (1 << 1),
		OnScreenBelowWindow = (1 << 2),
		IncludingWindow     = (1 << 3),
		ExcludeDesktopElements    = (1 << 4)
	}
#endif

	// uint32_t -> CGImage.h
	public enum CGImageAlphaInfo : uint {
		None,               
		PremultipliedLast,  
		PremultipliedFirst, 
		Last,               
		First,              
		NoneSkipLast,       
		NoneSkipFirst,      
		Only                
	}

	// uint32_t -> CGImage.h
	[Flags]
	public enum CGBitmapFlags : uint {
		None,               
		PremultipliedLast,  
		PremultipliedFirst, 
		Last,               
		First,              
		NoneSkipLast,       
		NoneSkipFirst,      
		Only,
			
		AlphaInfoMask = 0x1F,
		FloatInfoMask  = 0xf00,
		FloatComponents = (1 << 8),
		
		ByteOrderMask     = 0x7000,
		ByteOrderDefault  = (0 << 12),
		ByteOrder16Little = (1 << 12),
		ByteOrder32Little = (2 << 12),
		ByteOrder16Big    = (3 << 12),
		ByteOrder32Big    = (4 << 12)
	}

	// CGImage.h
	public class CGImage : INativeObject
#if !COREBUILD
		, IDisposable
#endif
	{
#if !COREBUILD
		internal IntPtr handle;

		// invoked by marshallers
		public CGImage (IntPtr handle)
			: this (handle, false)
		{
			this.handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CGImage (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CGImageRetain (handle);
		}
		
		~CGImage ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}
	
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGImageRelease (/* CGImageRef */ IntPtr image);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageRetain (/* CGImageRef */ IntPtr image);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGImageRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageCreate (/* size_t */ nint width, /* size_t */ nint height, 
			/* size_t */ nint bitsPerComponent, /* size_t */ nint bitsPerPixel, /* size_t */ nint bytesPerRow,
			/* CGColorSpaceRef */ IntPtr space, CGBitmapFlags bitmapInfo, /* CGDataProviderRef */ IntPtr provider,
			/* CGFloat[] */ nfloat [] decode, bool shouldInterpolate, CGColorRenderingIntent intent);

		public CGImage (int width, int height, int bitsPerComponent, int bitsPerPixel, int bytesPerRow,
				CGColorSpace colorSpace, CGBitmapFlags bitmapFlags, CGDataProvider provider,
				nfloat [] decode, bool shouldInterpolate, CGColorRenderingIntent intent)
		{
			if (width < 0)
				throw new ArgumentException ("width");
			if (height < 0)
				throw new ArgumentException ("height");
			if (bitsPerPixel < 0)
				throw new ArgumentException ("bitsPerPixel");
			if (bitsPerComponent < 0)
				throw new ArgumentException ("bitsPerComponent");
			if (bytesPerRow < 0)
				throw new ArgumentException ("bytesPerRow");

			handle = CGImageCreate (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow,
						colorSpace == null ? IntPtr.Zero : colorSpace.Handle, bitmapFlags, provider == null ? IntPtr.Zero : provider.Handle,
						decode,
						shouldInterpolate, intent);
		}

		public CGImage (int width, int height, int bitsPerComponent, int bitsPerPixel, int bytesPerRow,
				CGColorSpace colorSpace, CGImageAlphaInfo alphaInfo, CGDataProvider provider,
				nfloat [] decode, bool shouldInterpolate, CGColorRenderingIntent intent)
		{
			if (width < 0)
				throw new ArgumentException ("width");
			if (height < 0)
				throw new ArgumentException ("height");
			if (bitsPerPixel < 0)
				throw new ArgumentException ("bitsPerPixel");
			if (bitsPerComponent < 0)
				throw new ArgumentException ("bitsPerComponent");
			if (bytesPerRow < 0)
				throw new ArgumentException ("bytesPerRow");

			handle = CGImageCreate (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow,
						colorSpace == null ? IntPtr.Zero : colorSpace.Handle, (CGBitmapFlags) alphaInfo, provider == null ? IntPtr.Zero : provider.Handle,
						decode,
						shouldInterpolate, intent);
		}

#if MONOMAC
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGWindowListCreateImage(CGRect screenBounds, CGWindowListOption windowOption, uint windowID, CGWindowImageOption imageOption);
        
		public static CGImage ScreenImage (int windownumber, CGRect bounds)
		{                    
			IntPtr imageRef = CGWindowListCreateImage(bounds, CGWindowListOption.IncludingWindow, (uint)windownumber,
								  CGWindowImageOption.Default);
			if (imageRef == IntPtr.Zero)
				return null;
			return new CGImage(imageRef, true);                              
		}
#elif !WATCH
		public static CGImage ScreenImage {
			get {
				return UIKit.UIScreen.MainScreen.Capture ().CGImage;
			}
		}
#endif

	
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageCreateWithJPEGDataProvider (/* CGDataProviderRef */ IntPtr source,
			/* CGFloat[] */ nfloat [] decode, bool shouldInterpolate, CGColorRenderingIntent intent);

		public static CGImage FromJPEG (CGDataProvider provider, nfloat [] decode, bool shouldInterpolate, CGColorRenderingIntent intent)
		{
			var handle = CGImageCreateWithJPEGDataProvider (provider == null ? IntPtr.Zero : provider.Handle, decode, shouldInterpolate, intent);
			if (handle == IntPtr.Zero)
				return null;

			return new CGImage (handle, true);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageCreateWithPNGDataProvider (/* CGDataProviderRef */ IntPtr source,
			/* CGFloat[] */ nfloat [] decode, bool shouldInterpolate, CGColorRenderingIntent intent);

		public static CGImage FromPNG (CGDataProvider provider, nfloat [] decode, bool shouldInterpolate, CGColorRenderingIntent intent)
		{
			var handle = CGImageCreateWithPNGDataProvider (provider == null ? IntPtr.Zero : provider.Handle, decode, shouldInterpolate, intent);
			if (handle == IntPtr.Zero)
				return null;

			return new CGImage (handle, true);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageMaskCreate (/* size */ nint width, /* size */ nint height, 
			/* size */ nint bitsPerComponent, /* size */ nint bitsPerPixel, /* size */ nint bytesPerRow, 
			/* CGDataProviderRef */ IntPtr provider, /* CGFloat[] */ nfloat [] decode, bool shouldInterpolate);

		public static CGImage CreateMask (int width, int height, int bitsPerComponent, int bitsPerPixel, int bytesPerRow, CGDataProvider provider, nfloat [] decode, bool shouldInterpolate)
		{
			if (width < 0)
				throw new ArgumentException ("width");
			if (height < 0)
				throw new ArgumentException ("height");
			if (bitsPerPixel < 0)
				throw new ArgumentException ("bitsPerPixel");
			if (bytesPerRow < 0)
				throw new ArgumentException ("bytesPerRow");

			var handle = CGImageMaskCreate (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, provider == null ? IntPtr.Zero : provider.Handle, decode, shouldInterpolate);
			if (handle == IntPtr.Zero)
				return null;

			return new CGImage (handle, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageCreateWithMaskingColors (/* CGImageRef */ IntPtr image, /* CGFloat[] */ nfloat [] components);

		public CGImage WithMaskingColors (nfloat[] components)
		{
			nint N = 2*ColorSpace.Components;
			if (components != null && components.Length != N)
				throw new ArgumentException ("The argument 'components' must have 2N values, where N is the number of components in the color space of the image.", "components");
			return new CGImage (CGImageCreateWithMaskingColors (Handle, components), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageCreateCopy (/* CGImageRef */ IntPtr image);

		public CGImage Clone ()
		{
			return new CGImage (CGImageCreateCopy (handle), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageCreateCopyWithColorSpace (/* CGImageRef */ IntPtr image, /* CGColorSpaceRef */ IntPtr space);

		public CGImage WithColorSpace (CGColorSpace cs)
		{
			var h = CGImageCreateCopyWithColorSpace (handle, cs == null ? IntPtr.Zero : cs.handle);
			return h == IntPtr.Zero ? null : new CGImage (h, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageCreateWithImageInRect (/* CGImageRef */ IntPtr image, CGRect rect);

		public CGImage WithImageInRect (CGRect rect)
		{
			var h = CGImageCreateWithImageInRect (handle, rect);
			return h == IntPtr.Zero ? null : new CGImage (h, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageCreateWithMask (/* CGImageRef */ IntPtr image, /* CGImageRef */ IntPtr mask);

		public CGImage WithMask (CGImage mask)
		{
			if (mask == null)
				throw new ArgumentNullException ("mask");
			return new CGImage (CGImageCreateWithMask (handle, mask.handle), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGImageIsMask (/* CGImageRef */ IntPtr image);

		public bool IsMask {
			get {
				return CGImageIsMask (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGImageGetWidth (/* CGImageRef */ IntPtr image);

		public nint Width {
			get {
				return CGImageGetWidth (handle);
			}
		}
		

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGImageGetHeight (/* CGImageRef */ IntPtr image);

		public nint Height {
			get {
				return CGImageGetHeight (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGImageGetBitsPerComponent (/* CGImageRef */ IntPtr image);

		public nint BitsPerComponent {
			get {
				return CGImageGetBitsPerComponent (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGImageGetBitsPerPixel (/* CGImageRef */ IntPtr image);

		public nint BitsPerPixel {
			get {
				return CGImageGetBitsPerPixel (handle);
			}
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGImageGetBytesPerRow (/* CGImageRef */ IntPtr image);

		public nint BytesPerRow {
			get {
				return CGImageGetBytesPerRow (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGImageGetColorSpace (/* CGImageRef */ IntPtr image);

		public CGColorSpace ColorSpace {
			get {
				var h = CGImageGetColorSpace (handle);
				return h == IntPtr.Zero ? null : new CGColorSpace (h);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGImageAlphaInfo CGImageGetAlphaInfo (/* CGImageRef */ IntPtr image);

		public CGImageAlphaInfo AlphaInfo {
			get {
				return CGImageGetAlphaInfo (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataProviderRef */ IntPtr CGImageGetDataProvider (/* CGImageRef */ IntPtr image);

		public CGDataProvider DataProvider {
			get {
				return new CGDataProvider (CGImageGetDataProvider (handle));
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static /* CGFloat* */ nfloat * CGImageGetDecode (/* CGImageRef */ IntPtr image);

		public unsafe nfloat *Decode {
			get {
				return CGImageGetDecode (handle);
			}
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGImageGetShouldInterpolate (/* CGImageRef */ IntPtr image);

		public bool ShouldInterpolate {
			get {
				return CGImageGetShouldInterpolate (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGColorRenderingIntent CGImageGetRenderingIntent (/* CGImageRef */ IntPtr image);

		public CGColorRenderingIntent RenderingIntent {
			get {
				return CGImageGetRenderingIntent (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGBitmapFlags CGImageGetBitmapInfo (/* CGImageRef */ IntPtr image);

		public CGBitmapFlags BitmapInfo {
			get {
				return CGImageGetBitmapInfo (handle);
			}
		}

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr /* CFStringRef */ CGImageGetUTType (/* __nullable CGImageRef* */ IntPtr image);

		// we return an NSString, instead of a string, as all our UTType constants are NSString (see mobilecoreservices.cs)
		[iOS (9,0)][Mac (10,11)]
		public NSString UTType {
			get {
				var h = CGImageGetUTType (handle);
				return h == IntPtr.Zero ? null : Runtime.GetNSObject<NSString> (h);
			}
		}
#endif // !COREBUILD
	}
}
