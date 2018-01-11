// 
// CVPixelFormatDescription.cs: Implements the managed CVPixelFormatDescription
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
// Copyright 2015 Xamarin Inc.
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
using CoreFoundation;
using ObjCRuntime;
using Foundation;

namespace CoreVideo {

	[Watch (4,0)]
	public static class CVPixelFormatDescription {
#if !COREBUILD
		public static readonly NSString NameKey;
		public static readonly NSString ConstantKey;
		public static readonly NSString CodecTypeKey;
		public static readonly NSString FourCCKey;
		public static readonly NSString PlanesKey;
		public static readonly NSString BlockWidthKey;
		public static readonly NSString BlockHeightKey;
		public static readonly NSString BitsPerBlockKey;
		public static readonly NSString BlockHorizontalAlignmentKey;
		public static readonly NSString BlockVerticalAlignmentKey;
		public static readonly NSString BlackBlockKey;
		public static readonly NSString HorizontalSubsamplingKey;
		public static readonly NSString VerticalSubsamplingKey;
   
		public static readonly NSString OpenGLFormatKey;
		public static readonly NSString OpenGLTypeKey;
		public static readonly NSString OpenGLInternalFormatKey;
   
		public static readonly NSString CGBitmapInfoKey;
   
		public static readonly NSString QDCompatibilityKey;
		public static readonly NSString CGBitmapContextCompatibilityKey;
		public static readonly NSString CGImageCompatibilityKey;
		public static readonly NSString OpenGLCompatibilityKey;
   
		public static readonly NSString FillExtendedPixelsCallbackKey;

		[iOS (8,0)][Mac (10,10)]
		public static readonly NSString ContainsRgb;
		[iOS (8,0)][Mac (10,10)]
		public static readonly NSString ContainsYCbCr;

		[iOS (9,0)][Mac (10,10)]
		public static readonly NSString ComponentRangeKey;
		[iOS (9,0)][Mac (10,10)]
		public static readonly NSString ComponentRangeFullRangeKey;
		[iOS (9,0)][Mac (10,10)]
		public static readonly NSString ComponentRangeVideoRangeKey;
		[iOS (9,0)][Mac (10,10)]
		public static readonly NSString ComponentRangeWideRangeKey;

		static CVPixelFormatDescription ()
		{
			var handle = Dlfcn.dlopen (Constants.CoreVideoLibrary, 0);
			if (handle == IntPtr.Zero)
				return;
			try {
				NameKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatName");
				ConstantKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatConstant");
				CodecTypeKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatCodecType");
				FourCCKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatFourCC");
				PlanesKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatPlanes");
				BlockWidthKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatBlockWidth");
				BlockHeightKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatBlockHeight");
				BitsPerBlockKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatBitsPerBlock");
				BlockHorizontalAlignmentKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatBlockHorizontalAlignment");
				BlockVerticalAlignmentKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatBlockVerticalAlignment");
				BlackBlockKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatBlackBlock");
				HorizontalSubsamplingKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatHorizontalSubsampling");
				VerticalSubsamplingKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatVerticalSubsampling");
				OpenGLFormatKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatOpenGLFormat");
				OpenGLTypeKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatOpenGLType");
				OpenGLInternalFormatKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatOpenGLInternalFormat");
				CGBitmapInfoKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatCGBitmapInfo");
				QDCompatibilityKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatQDCompatibility");
				CGBitmapContextCompatibilityKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatCGBitmapContextCompatibility");
				CGImageCompatibilityKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatCGImageCompatibility");
				OpenGLCompatibilityKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatOpenGLCompatibility");
				FillExtendedPixelsCallbackKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatFillExtendedPixelsCallback");

				//iOS8 only
				ContainsRgb = Dlfcn.GetStringConstant (handle, "kCVPixelFormatContainsRGB");
				ContainsYCbCr = Dlfcn.GetStringConstant (handle, "kCVPixelFormatContainsYCbCr");

				//iOS9 only
				ComponentRangeKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatComponentRange");
				ComponentRangeFullRangeKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatComponentRange_FullRange");
				ComponentRangeVideoRangeKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatComponentRange_VideoRange");
				ComponentRangeWideRangeKey = Dlfcn.GetStringConstant (handle, "kCVPixelFormatComponentRange_WideRange");
			}
			finally {
				Dlfcn.dlclose (handle);
			}
		}

		// note: bad documentation, ref: https://bugzilla.xamarin.com/show_bug.cgi?id=13917
		[DllImport (Constants.CoreVideoLibrary)]
		extern static/* CFArrayRef __nullable */ IntPtr CVPixelFormatDescriptionArrayCreateWithAllPixelFormatTypes (
			/* CFAllocatorRef __nullable */ IntPtr allocator);

		public static NSNumber [] AllTypes {
			get {
				return NSArray.ArrayFromHandle <NSNumber> (CVPixelFormatDescriptionArrayCreateWithAllPixelFormatTypes (IntPtr.Zero));
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CVPixelFormatDescriptionCreateWithPixelFormatType (
			/* CFAllocatorRef __nullable */ IntPtr allocator, int /* OSType = int32_t */ pixelFormat);

#if !XAMCORE_3_0
		public static NSDictionary Create (int pixelFormat)
		{
			return Runtime.GetNSObject<NSDictionary> (CVPixelFormatDescriptionCreateWithPixelFormatType (IntPtr.Zero, pixelFormat));
		}
#endif

		public static NSDictionary Create (CVPixelFormatType pixelFormat) 
		{
			return Runtime.GetNSObject<NSDictionary> (CVPixelFormatDescriptionCreateWithPixelFormatType (IntPtr.Zero, (int) pixelFormat));
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVPixelFormatDescriptionRegisterDescriptionWithPixelFormatType (
			/* CFDictionaryRef __nonnull */ IntPtr description, int /* OSType = int32_t */ pixelFormat);

#if !XAMCORE_3_0
		public static void Register (NSDictionary description, int pixelFormat)
		{
			if (description == null)
				throw new ArgumentNullException ("description");

			CVPixelFormatDescriptionRegisterDescriptionWithPixelFormatType (description.Handle, pixelFormat);
		}
#endif

		public static void Register (NSDictionary description, CVPixelFormatType pixelFormat)
		{
			if (description == null)
				throw new ArgumentNullException ("description");

			CVPixelFormatDescriptionRegisterDescriptionWithPixelFormatType (description.Handle, (int) pixelFormat);
		}
#endif // !COREBUILD
	}
}
