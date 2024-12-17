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

#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {

#if MONOMAC || __MACCATALYST__
	// uint32_t -> CGWindow.h (OSX SDK only)
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
	[Flags]
	public enum CGWindowImageOption : uint {
		Default             = 0,
		BoundsIgnoreFraming = (1 << 0),
		ShouldBeOpaque      = (1 << 1),
		OnlyShadows         = (1 << 2),
		BestResolution      = (1 << 3),
		NominalResolution   = (1 << 4),
	}

	// uint32_t -> CGWindow.h (OSX SDK only)
#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#else
	[MacCatalyst (13,1)]
#endif
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

	public enum CGImagePixelFormatInfo : uint {
		Packed = 0,
		Rgb555 = 1 << 16,
		Rgb565 = 2 << 16,
		Rgb101010 = 3 << 16,
		RgbCif10 = 4 << 16,
		Mask = 0xF0000,
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
		FloatInfoMask = 0xf00,
		FloatComponents = (1 << 8),

		ByteOrderMask = 0x7000,
		ByteOrderDefault = (0 << 12),
		ByteOrder16Little = (1 << 12),
		ByteOrder32Little = (2 << 12),
		ByteOrder16Big = (3 << 12),
		ByteOrder32Big = (4 << 12)
	}

	[Flags]
	public enum CGImageByteOrderInfo : uint {
		ByteOrderMask = 0x7000,
		ByteOrderDefault = (0 << 12),
		ByteOrder16Little = (1 << 12),
		ByteOrder32Little = (2 << 12),
		ByteOrder16Big = (3 << 12),
		ByteOrder32Big = (4 << 12),
	}


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	// CGImage.h
	public class CGImage : NativeObject {
#if !COREBUILD
#if !NET
		public CGImage (NativeHandle handle)
			: base (handle, false, verify: false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGImage (NativeHandle handle, bool owns)
#if NET
			: base (handle, owns)
#else
			: base (handle, owns, verify: false)
#endif
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGImageRelease (/* CGImageRef */ IntPtr image);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageRetain (/* CGImageRef */ IntPtr image);

		protected internal override void Retain ()
		{
			CGImageRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGImageRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe /* CGImageRef */ IntPtr CGImageCreate (/* size_t */ nint width, /* size_t */ nint height,
			/* size_t */ nint bitsPerComponent, /* size_t */ nint bitsPerPixel, /* size_t */ nint bytesPerRow,
			/* CGColorSpaceRef */ IntPtr space, CGBitmapFlags bitmapInfo, /* CGDataProviderRef */ IntPtr provider,
			/* CGFloat[] */ nfloat* decode, byte shouldInterpolate, CGColorRenderingIntent intent);

		static IntPtr Create (int width, int height, int bitsPerComponent, int bitsPerPixel, int bytesPerRow,
				CGColorSpace? colorSpace, CGBitmapFlags bitmapFlags, CGDataProvider? provider,
				nfloat []? decode, bool shouldInterpolate, CGColorRenderingIntent intent)
		{
			if (width < 0)
				throw new ArgumentException (nameof (width));
			if (height < 0)
				throw new ArgumentException (nameof (height));
			if (bitsPerPixel < 0)
				throw new ArgumentException (nameof (bitsPerPixel));
			if (bitsPerComponent < 0)
				throw new ArgumentException (nameof (bitsPerComponent));
			if (bytesPerRow < 0)
				throw new ArgumentException (nameof (bytesPerRow));

			unsafe {
				fixed (nfloat* decodePtr = decode) {
					return CGImageCreate (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow,
						colorSpace.GetHandle (), bitmapFlags, provider.GetHandle (),
						decodePtr, shouldInterpolate.AsByte (), intent);
				}
			}
		}

		public CGImage (int width, int height, int bitsPerComponent, int bitsPerPixel, int bytesPerRow,
				CGColorSpace? colorSpace, CGBitmapFlags bitmapFlags, CGDataProvider? provider,
				nfloat []? decode, bool shouldInterpolate, CGColorRenderingIntent intent)
			: base (Create (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, colorSpace, bitmapFlags, provider, decode, shouldInterpolate, intent), true)
		{
		}

		static IntPtr Create (int width, int height, int bitsPerComponent, int bitsPerPixel, int bytesPerRow,
				CGColorSpace? colorSpace, CGImageAlphaInfo alphaInfo, CGDataProvider? provider,
				nfloat []? decode, bool shouldInterpolate, CGColorRenderingIntent intent)
		{
			if (width < 0)
				throw new ArgumentException (nameof (width));
			if (height < 0)
				throw new ArgumentException (nameof (height));
			if (bitsPerPixel < 0)
				throw new ArgumentException (nameof (bitsPerPixel));
			if (bitsPerComponent < 0)
				throw new ArgumentException (nameof (bitsPerComponent));
			if (bytesPerRow < 0)
				throw new ArgumentException (nameof (bytesPerRow));

			unsafe {
				fixed (nfloat* decodePtr = decode) {
					return CGImageCreate (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow,
						colorSpace.GetHandle (), (CGBitmapFlags) alphaInfo, provider.GetHandle (),
						decodePtr, shouldInterpolate.AsByte (), intent);
				}
			}
		}

		public CGImage (int width, int height, int bitsPerComponent, int bitsPerPixel, int bytesPerRow,
				CGColorSpace? colorSpace, CGImageAlphaInfo alphaInfo, CGDataProvider? provider,
				nfloat []? decode, bool shouldInterpolate, CGColorRenderingIntent intent)
			: base (Create (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, colorSpace, alphaInfo, provider, decode, shouldInterpolate, intent), true)
		{
		}

		internal static CGImage? FromHandle (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				return null;
			return new CGImage (handle, owns);
		}

#if MONOMAC || __MACCATALYST__
#if NET
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("maccatalyst18.0", "Use ScreenCaptureKit instead.")]
		[UnsupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use ScreenCaptureKit instead.")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[MacCatalyst (13,1)]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use ScreenCaptureKit instead.")]
		[Deprecated (PlatformName.MacOSX, 14, 0, message: "Use ScreenCaptureKit instead.")]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGWindowListCreateImage(CGRect screenBounds, CGWindowListOption windowOption, uint windowID, CGWindowImageOption imageOption);
        
#if NET
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("maccatalyst18.0", "Use ScreenCaptureKit instead.")]
		[UnsupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use ScreenCaptureKit instead.")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[MacCatalyst (13,1)]
#endif
		public static CGImage? ScreenImage (int windownumber, CGRect bounds)
		{
			return ScreenImage (windownumber, bounds, CGWindowListOption.IncludingWindow, CGWindowImageOption.Default);
		}

#if NET
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("maccatalyst18.0", "Use ScreenCaptureKit instead.")]
		[UnsupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("macos15.0", "Use ScreenCaptureKit instead.")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[MacCatalyst (13,1)]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use ScreenCaptureKit instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use ScreenCaptureKit instead.")]
#endif
		public static CGImage? ScreenImage (int windownumber, CGRect bounds, CGWindowListOption windowOption,
			CGWindowImageOption imageOption)
		{
			IntPtr imageRef = CGWindowListCreateImage (bounds, windowOption, (uint) windownumber,
								  imageOption);
			return FromHandle (imageRef, true);
		}
#else
		public static CGImage? ScreenImage {
			get {
				return UIKit.UIScreen.MainScreen.Capture ().CGImage;
			}
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe /* CGImageRef */ IntPtr CGImageCreateWithJPEGDataProvider (/* CGDataProviderRef */ IntPtr source,
			/* CGFloat[] */ nfloat* decode, byte shouldInterpolate, CGColorRenderingIntent intent);

		public static CGImage? FromJPEG (CGDataProvider? provider, nfloat []? decode, bool shouldInterpolate, CGColorRenderingIntent intent)
		{
			unsafe {
				fixed (nfloat* decodePtr = decode) {
					var handle = CGImageCreateWithJPEGDataProvider (provider.GetHandle (), decodePtr, shouldInterpolate.AsByte (), intent);
					return FromHandle (handle, true);
				}
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe /* CGImageRef */ IntPtr CGImageCreateWithPNGDataProvider (/* CGDataProviderRef */ IntPtr source,
			/* CGFloat[] */ nfloat* decode, byte shouldInterpolate, CGColorRenderingIntent intent);

		public static CGImage? FromPNG (CGDataProvider provider, nfloat []? decode, bool shouldInterpolate, CGColorRenderingIntent intent)
		{
			unsafe {
				fixed (nfloat* decodePtr = decode) {
					var handle = CGImageCreateWithPNGDataProvider (provider.GetHandle (), decodePtr, shouldInterpolate.AsByte (), intent);
					return FromHandle (handle, true);
				}
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe /* CGImageRef */ IntPtr CGImageMaskCreate (/* size */ nint width, /* size */ nint height,
			/* size */ nint bitsPerComponent, /* size */ nint bitsPerPixel, /* size */ nint bytesPerRow,
			/* CGDataProviderRef */ IntPtr provider, /* CGFloat[] */ nfloat* decode, byte shouldInterpolate);

		public static CGImage? CreateMask (int width, int height, int bitsPerComponent, int bitsPerPixel, int bytesPerRow, CGDataProvider? provider, nfloat []? decode, bool shouldInterpolate)
		{
			if (width < 0)
				throw new ArgumentException (nameof (width));
			if (height < 0)
				throw new ArgumentException (nameof (height));
			if (bitsPerPixel < 0)
				throw new ArgumentException (nameof (bitsPerPixel));
			if (bytesPerRow < 0)
				throw new ArgumentException (nameof (bytesPerRow));
			unsafe {
				fixed (nfloat* decodePtr = decode) {

					var handle = CGImageMaskCreate (width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, provider.GetHandle (), decodePtr, shouldInterpolate.AsByte ());
					return FromHandle (handle, true);
				}
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static unsafe /* CGImageRef */ IntPtr CGImageCreateWithMaskingColors (/* CGImageRef */ IntPtr image, /* CGFloat[] */ nfloat* components);

		public CGImage? WithMaskingColors (nfloat []? components)
		{
			var N = 2 * ColorSpace!.Components;
			if (components is not null && components.Length != N)
				throw new ArgumentException ("The argument 'components' must have 2N values, where N is the number of components in the color space of the image.", nameof (components));
			unsafe {
				fixed (nfloat* componentsPtr = components) {
					var handle = CGImageCreateWithMaskingColors (Handle, componentsPtr);
					return FromHandle (handle, true);
				}
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageCreateCopy (/* CGImageRef */ IntPtr image);

		public CGImage? Clone ()
		{
			var h = CGImageCreateCopy (Handle);
			return FromHandle (h, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageCreateCopyWithColorSpace (/* CGImageRef */ IntPtr image, /* CGColorSpaceRef */ IntPtr space);

		public CGImage? WithColorSpace (CGColorSpace? cs)
		{
			var h = CGImageCreateCopyWithColorSpace (Handle, cs.GetHandle ());
			return FromHandle (h, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageCreateWithImageInRect (/* CGImageRef */ IntPtr image, CGRect rect);

		public CGImage? WithImageInRect (CGRect rect)
		{
			var h = CGImageCreateWithImageInRect (Handle, rect);
			return FromHandle (h, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageCreateWithMask (/* CGImageRef */ IntPtr image, /* CGImageRef */ IntPtr mask);

		public CGImage? WithMask (CGImage mask)
		{
			if (mask is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (mask));
			return FromHandle (CGImageCreateWithMask (Handle, mask.Handle), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static byte CGImageIsMask (/* CGImageRef */ IntPtr image);

		public bool IsMask {
			get {
				return CGImageIsMask (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGImageGetWidth (/* CGImageRef */ IntPtr image);

		public nint Width {
			get {
				return CGImageGetWidth (Handle);
			}
		}


		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGImageGetHeight (/* CGImageRef */ IntPtr image);

		public nint Height {
			get {
				return CGImageGetHeight (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGImageGetBitsPerComponent (/* CGImageRef */ IntPtr image);

		public nint BitsPerComponent {
			get {
				return CGImageGetBitsPerComponent (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGImageGetBitsPerPixel (/* CGImageRef */ IntPtr image);

		public nint BitsPerPixel {
			get {
				return CGImageGetBitsPerPixel (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGImageGetBytesPerRow (/* CGImageRef */ IntPtr image);

		public nint BytesPerRow {
			get {
				return CGImageGetBytesPerRow (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGImageGetColorSpace (/* CGImageRef */ IntPtr image);

		public CGColorSpace? ColorSpace {
			get {
				var h = CGImageGetColorSpace (Handle);
				return h == IntPtr.Zero ? null : new CGColorSpace (h, false);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGImageAlphaInfo CGImageGetAlphaInfo (/* CGImageRef */ IntPtr image);

		public CGImageAlphaInfo AlphaInfo {
			get {
				return CGImageGetAlphaInfo (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataProviderRef */ IntPtr CGImageGetDataProvider (/* CGImageRef */ IntPtr image);

		public CGDataProvider DataProvider {
			get {
				return new CGDataProvider (CGImageGetDataProvider (Handle), false);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static /* CGFloat* */ nfloat* CGImageGetDecode (/* CGImageRef */ IntPtr image);

		public unsafe nfloat* Decode {
			get {
				return CGImageGetDecode (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static byte CGImageGetShouldInterpolate (/* CGImageRef */ IntPtr image);

		public bool ShouldInterpolate {
			get {
				return CGImageGetShouldInterpolate (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGColorRenderingIntent CGImageGetRenderingIntent (/* CGImageRef */ IntPtr image);

		public CGColorRenderingIntent RenderingIntent {
			get {
				return CGImageGetRenderingIntent (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGBitmapFlags CGImageGetBitmapInfo (/* CGImageRef */ IntPtr image);

		public CGBitmapFlags BitmapInfo {
			get {
				return CGImageGetBitmapInfo (Handle);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr /* CFStringRef */ CGImageGetUTType (/* __nullable CGImageRef* */ IntPtr image);

		// we return an NSString, instead of a string, as all our UTType constants are NSString (see mobilecoreservices.cs)
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public NSString? UTType {
			get {
				var h = CGImageGetUTType (Handle);
				return Runtime.GetNSObject<NSString> (h);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern CGImagePixelFormatInfo CGImageGetPixelFormatInfo (/* __nullable CGImageRef */ IntPtr handle);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public CGImagePixelFormatInfo PixelFormatInfo => CGImageGetPixelFormatInfo (Handle);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern CGImageByteOrderInfo CGImageGetByteOrderInfo (/* __nullable CGImageRef */ IntPtr handle);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public CGImageByteOrderInfo ByteOrderInfo => CGImageGetByteOrderInfo (Handle);

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe static extern /* CGImageRef __nullable */ IntPtr CGImageCreateWithContentHeadroom (
			/* float */ float headroom,
			/* size_t */ nint width,
			/* size_t */ nint height,
			/* size_t */ nint bitsPerComponent,
			/* size_t */ nint bitsPerPixel,
			/* size_t */ nint bytesPerRow,
			/* CGColorSpaceRef cg_nullable */ IntPtr space,
			/* CGBitmapInfo */ CGBitmapFlags bitmapInfo,
			/* CGDataProviderRef cg_nullable */ IntPtr provider,
			/* const CGFloat * __nullable */ nfloat* decode,
			/* bool */ byte shouldInterpolate,
			/* CGColorRenderingIntent */ CGColorRenderingIntent intent);

		/// <summary>Create a new CGImage instance with the specified content headroom.</summary>
		/// <param name="headroom">The content headroom. Must be either 0.0f or greater than or equal to 1.0f.</param>
		/// <param name="width">The width of the new image.</param>
		/// <param name="height">The height of the new image.</param>
		/// <param name="bitsPerComponent">Bits per component for the new image.</param>
		/// <param name="bitsPerPixel">Bits per pixel for the new image.</param>
		/// <param name="bytesPerRow">Bytes per row for the new image.</param>
		/// <param name="colorSpace">The colorspace for the new image.</param>
		/// <param name="bitmapFlags">The bitmap flags for the new image.</param>
		/// <param name="provider">The data provider for the new image.</param>
		/// <param name="decode">The decode array for the new image.</param>
		/// <param name="shouldInterpolate">Whether image is interpolated or not.</param>
		/// <param name="intent">The rendering intent for the new image.</param>
		/// <returns>A new CGImage instance.</returns>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public CGImage (float headroom, int width, int height, int bitsPerComponent, int bitsPerPixel, int bytesPerRow,
				CGColorSpace? colorSpace, CGBitmapFlags bitmapFlags, CGDataProvider? provider,
				nfloat []? decode, bool shouldInterpolate, CGColorRenderingIntent intent)
			: base (Create (headroom, width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, colorSpace, bitmapFlags, provider, decode, shouldInterpolate, intent), true)
		{
		}

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		static IntPtr Create (float headroom, int width, int height, int bitsPerComponent, int bitsPerPixel, int bytesPerRow,
				CGColorSpace? colorSpace, CGBitmapFlags bitmapFlags, CGDataProvider? provider,
				nfloat []? decode, bool shouldInterpolate, CGColorRenderingIntent intent)
		{
			if (headroom != 0.0f && headroom < 1.0f)
				throw new ArgumentException (nameof (headroom));
			if (width < 0)
				throw new ArgumentException (nameof (width));
			if (height < 0)
				throw new ArgumentException (nameof (height));
			if (bitsPerPixel < 0)
				throw new ArgumentException (nameof (bitsPerPixel));
			if (bitsPerComponent < 0)
				throw new ArgumentException (nameof (bitsPerComponent));
			if (bytesPerRow < 0)
				throw new ArgumentException (nameof (bytesPerRow));

			unsafe {
				fixed (nfloat* decodePtr = decode) {
					return CGImageCreateWithContentHeadroom (headroom, width, height, bitsPerComponent, bitsPerPixel, bytesPerRow,
						colorSpace.GetHandle (), bitmapFlags, provider.GetHandle (),
						decodePtr, shouldInterpolate.AsByte (), intent);
				}
			}
		}

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe static extern /* CGImageRef __nullable */ IntPtr CGImageCreateCopyWithContentHeadroom (
			/* float */ float headroom,
			/* CGImageRef cg_nullable */ IntPtr image);

		/// <summary>Create a copy of the current image, adding or replacing the current image's headroom.</summary>
		/// <param name="headroom">Must be either equal to 0 or greater or equal to 1.0.</param>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public CGImage? Copy (float headroom)
		{
			if (headroom != 0.0f && headroom < 1.0f)
				throw new ArgumentException (nameof (headroom));

			var rv = CGImageCreateCopyWithContentHeadroom (headroom, GetCheckedHandle ());
			return Runtime.GetINativeObject<CGImage> (rv, owns: true);
		}

		/// <summary>Get the default content headroom for HDR images.</summary>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public static float DefaultHdrImageContentHeadroom {
			get => CoreGraphicsFields.DefaultHdrImageContentHeadroom;
		}

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern float CGImageGetContentHeadroom (/* __nullable CGImageRef */ IntPtr handle);

		/// <summary>Get the content headroom for this image.</summary>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public float ContentHeadroom => CGImageGetContentHeadroom (Handle);

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* bool */ byte CGImageShouldToneMap (/* __nullable CGImageRef */ IntPtr handle);

		/// <summary>Get whether this image should be tone mapped when rendered.</summary>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public bool ShouldToneMap => CGImageShouldToneMap (Handle) != 0;

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* bool */ byte CGImageContainsImageSpecificToneMappingMetadata (/* __nullable CGImageRef */ IntPtr handle);

		/// <summary>Get whether this image has image-specific tone mapping metadata.</summary>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		public bool ContainsImageSpecificToneMappingMetadata => CGImageContainsImageSpecificToneMappingMetadata (Handle) != 0;
#endif // !COREBUILD
	}
}
