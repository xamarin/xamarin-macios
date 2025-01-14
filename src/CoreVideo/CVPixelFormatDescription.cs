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
using System.ComponentModel;
using System.Runtime.InteropServices;
using CoreFoundation;
using ObjCRuntime;
using Foundation;

#if XAMCORE_5_0
using CVFillExtendedPixelsCallBackDataStruct = CoreVideo.CVFillExtendedPixelsCallBackData;
#endif

#nullable enable

namespace CoreVideo {
	public partial class CVPixelFormatDescription {
#if !COREBUILD
#if !XAMCORE_5_0
		[Obsolete ("Use 'CVPixelFormatKeys.Name' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString NameKey;

		[Obsolete ("Use 'CVPixelFormatKeys.Constant' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString ConstantKey;

		[Obsolete ("Use 'CVPixelFormatKeys.CodecType' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString CodecTypeKey;

		[Obsolete ("Use 'CVPixelFormatKeys.FourCCKey' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString FourCCKey;

		[Obsolete ("Use 'CVPixelFormatKeys.Planes' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString PlanesKey;

		[Obsolete ("Use 'CVPixelFormatKeys.BlockWidth' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString BlockWidthKey;

		[Obsolete ("Use 'CVPixelFormatKeys.BlockHeight' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString BlockHeightKey;

		[Obsolete ("Use 'CVPixelFormatKeys.BitsPerBlock' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString BitsPerBlockKey;

		[Obsolete ("Use 'CVPixelFormatKeys.BlockHorizontalAlignment' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString BlockHorizontalAlignmentKey;

		[Obsolete ("Use 'CVPixelFormatKeys.BlockVerticalAlignment' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString BlockVerticalAlignmentKey;

		[Obsolete ("Use 'CVPixelFormatKeys.BlackBlock' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString BlackBlockKey;

		[Obsolete ("Use 'CVPixelFormatKeys.HorizontalSubsampling' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString HorizontalSubsamplingKey;

		[Obsolete ("Use 'CVPixelFormatKeys.VerticalSubsampling' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString VerticalSubsamplingKey;

		[Obsolete ("Use 'CVPixelFormatKeys.OpenGLFormat' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString OpenGLFormatKey;

		[Obsolete ("Use 'CVPixelFormatKeys.OpenGLType' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString OpenGLTypeKey;

		[Obsolete ("Use 'CVPixelFormatKeys.OpenGLInternalFormat' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString OpenGLInternalFormatKey;

		[Obsolete ("Use 'CVPixelFormatKeys.CGBitmapInfo' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString CGBitmapInfoKey;

		[Obsolete ("Use 'CVPixelFormatKeys.QDCompatibility' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString QDCompatibilityKey;

		[Obsolete ("Use 'CVPixelFormatKeys.CGBitmapContextCompatibility' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString CGBitmapContextCompatibilityKey;

		[Obsolete ("Use 'CVPixelFormatKeys.CGImageCompatibility' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString CGImageCompatibilityKey;

		[Obsolete ("Use 'CVPixelFormatKeys.OpenGLCompatibility' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString OpenGLCompatibilityKey;

		[Obsolete ("Use 'CVPixelFormatKeys.FillExtendedPixelsCallback' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString FillExtendedPixelsCallbackKey;

		[Obsolete ("Use 'CVPixelFormatKeys.ContainsRgb' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString ContainsRgb;

		[Obsolete ("Use 'CVPixelFormatKeys.ContainsYCbCr' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString ContainsYCbCr;

		[Obsolete ("Use 'CVPixelFormatKeys.ComponentRange' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString ComponentRangeKey;

		[Obsolete ("Use 'CVPixelFormatComponentRangeKeys.FullRange' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString ComponentRangeFullRangeKey;

		[Obsolete ("Use 'CVPixelFormatComponentRangeKeys.VideoRange' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString ComponentRangeVideoRangeKey;

		[Obsolete ("Use 'CVPixelFormatComponentRangeKeys.WideRange' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static readonly NSString ComponentRangeWideRangeKey;

		[Obsolete ("Use 'CVPixelFormatKeys.ContainsGrayscale' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public static readonly NSString ContainsGrayscaleKey;

		[Obsolete ("Use 'CVPixelFormatKeys.ContainsSenselArray' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#endif
		public static readonly NSString ContainsSenselArray;
#endif // !XAMCORE_5_0

#if !XAMCORE_5_0
		static CVPixelFormatDescription ()
		{
			NameKey = CVPixelFormatKeys.Name;
			ConstantKey = CVPixelFormatKeys.Constant;
			CodecTypeKey = CVPixelFormatKeys.CodecType;
			FourCCKey = CVPixelFormatKeys.FourCC;
			PlanesKey = CVPixelFormatKeys.Planes;
			BlockWidthKey = CVPixelFormatKeys.BlockWidth;
			BlockHeightKey = CVPixelFormatKeys.BlockHeight;
			BitsPerBlockKey = CVPixelFormatKeys.BitsPerBlock;
			BlockHorizontalAlignmentKey = CVPixelFormatKeys.BlockHorizontalAlignment;
			BlockVerticalAlignmentKey = CVPixelFormatKeys.BlockVerticalAlignment;
			BlackBlockKey = CVPixelFormatKeys.BlackBlock;
			HorizontalSubsamplingKey = CVPixelFormatKeys.HorizontalSubsampling;
			VerticalSubsamplingKey = CVPixelFormatKeys.VerticalSubsampling;
			OpenGLFormatKey = CVPixelFormatKeys.OpenGLFormat;
			OpenGLTypeKey = CVPixelFormatKeys.OpenGLType;
			OpenGLInternalFormatKey = CVPixelFormatKeys.OpenGLInternalFormat;
			CGBitmapInfoKey = CVPixelFormatKeys.CGBitmapInfo;
			QDCompatibilityKey = CVPixelFormatKeys.QDCompatibility;
			CGBitmapContextCompatibilityKey = CVPixelFormatKeys.CGBitmapContextCompatibility;
			CGImageCompatibilityKey = CVPixelFormatKeys.CGImageCompatibility;
			OpenGLCompatibilityKey = CVPixelFormatKeys.OpenGLCompatibility;
			FillExtendedPixelsCallbackKey = CVPixelFormatKeys.FillExtendedPixelsCallback;

			//iOS8 only
			ContainsRgb = CVPixelFormatKeys.ContainsRgb;
			ContainsYCbCr = CVPixelFormatKeys.ContainsYCbCr;

			//iOS9 only
			ComponentRangeKey = CVPixelFormatKeys.ComponentRange;
			ComponentRangeFullRangeKey = CVPixelFormatComponentRangeKeys.FullRange;
			ComponentRangeVideoRangeKey = CVPixelFormatComponentRangeKeys.VideoRange;
			ComponentRangeWideRangeKey = CVPixelFormatComponentRangeKeys.WideRange;

			// Xcode 10
			ContainsGrayscaleKey = CVPixelFormatKeys.ContainsGrayscale;

			// Xcode 14
			ContainsSenselArray = CVPixelFormatKeys.ContainsSenselArray;
		}
#endif

		// note: bad documentation, ref: https://bugzilla.xamarin.com/show_bug.cgi?id=13917
		[DllImport (Constants.CoreVideoLibrary)]
		extern static/* CFArrayRef __nullable */ IntPtr CVPixelFormatDescriptionArrayCreateWithAllPixelFormatTypes (
			/* CFAllocatorRef __nullable */ IntPtr allocator);

		/// <summary>Get all the known pixel format types.</summary>
		public static NSNumber [] AllTypes {
			get {
				return NSArray.ArrayFromHandle<NSNumber> (CVPixelFormatDescriptionArrayCreateWithAllPixelFormatTypes (IntPtr.Zero));
			}
		}

		/// <summary>Get all the known pixel format types.</summary>
		public static CVPixelFormatType [] AllPixelFormatTypes {
			get {
				var all = AllTypes;
				var rv = new CVPixelFormatType [all.Length];
				for (var i = 0; i < rv.Length; i++)
					rv [i] = (CVPixelFormatType) all [i].Int32Value;
				return rv;
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CVPixelFormatDescriptionCreateWithPixelFormatType (
			/* CFAllocatorRef __nullable */ IntPtr allocator, int /* OSType = int32_t */ pixelFormat);

		/// <summary>Create a description of the specified pixel format.</summary>
		/// <param name="pixelFormat">The pixel format to create a description of.</param>
		public static NSDictionary? Create (CVPixelFormatType pixelFormat)
		{
			return Runtime.GetNSObject<NSDictionary> (CVPixelFormatDescriptionCreateWithPixelFormatType (IntPtr.Zero, (int) pixelFormat));
		}

		/// <summary>Create a description of the specified pixel format.</summary>
		/// <param name="pixelFormat">The pixel format to create a description of.</param>
		public static CVPixelFormatDescription? CreatePixelFormat (CVPixelFormatType pixelFormat)
		{
			var dict = Create (pixelFormat);
			if (dict is null)
				return null;
			return new CVPixelFormatDescription (dict);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVPixelFormatDescriptionRegisterDescriptionWithPixelFormatType (
			/* CFDictionaryRef __nonnull */ IntPtr description, int /* OSType = int32_t */ pixelFormat);

		/// <summary>Register a new pixel format with CoreVideo.</summary>
		/// <param name="description">The pixel format description for the pixel format to register.</param>
		/// <param name="pixelFormat">The pixel format to register.</param>
		public static void Register (NSDictionary description, CVPixelFormatType pixelFormat)
		{
			if (description is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (description));

			CVPixelFormatDescriptionRegisterDescriptionWithPixelFormatType (description.Handle, (int) pixelFormat);
		}

		/// <summary>Register a new pixel format with CoreVideo.</summary>
		/// <param name="description">The pixel format description for the pixel format to register.</param>
		/// <param name="pixelFormat">The pixel format to register.</param>
		public static void Register (CVPixelFormatDescription description, CVPixelFormatType pixelFormat)
		{
			Register (description?.Dictionary!, pixelFormat);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0), Mac (12, 0)]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static byte CVIsCompressedPixelFormatAvailable (int /* OSType = int32_t */ pixelFormat);

		/// <summary>Check if the specified pixel format is supported on this platform.</summary>
		/// <param name="pixelFormat">The pixel format to check.</param>
		/// <returns>Whether the specified pixel format is supported or not.</returns>
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0), Mac (12, 0)]
#endif
		public static bool IsPixelFormatAvailable (CVPixelFormatType pixelFormat)
		{
			return CVIsCompressedPixelFormatAvailable ((int) pixelFormat) != 0;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public CVFillExtendedPixelsCallBackDataStruct? FillExtendedPixelsCallbackStruct {
			get {
				var data = FillExtendedPixelsCallback;
				if (data is null)
					return null;
				var bytes = data.ToArray ();
				unsafe {
					if (bytes.Length < sizeof (CVFillExtendedPixelsCallBackDataStruct))
						throw new InvalidOperationException ($"The size of the callback data structure is smaller than expected (got {bytes.Length} bytes, expected at least {sizeof (CVFillExtendedPixelsCallBackDataStruct)} bytes)");
					fixed (byte *ptr = bytes)
						return Marshal.PtrToStructure<CVFillExtendedPixelsCallBackDataStruct> ((IntPtr) ptr);
				}
			}
			set {
				if (value is null) {
					FillExtendedPixelsCallback = null;
					return;
				}
				NSData data;
				CVFillExtendedPixelsCallBackDataStruct v = value.Value;
				unsafe {
					data = NSData.FromBytes ((IntPtr) (&v), (nuint) sizeof (CVFillExtendedPixelsCallBackDataStruct));
				}
				FillExtendedPixelsCallback = data;
			}
		}
#endif

#endif // !COREBUILD
	}
}
