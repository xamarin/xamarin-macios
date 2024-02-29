//
// corevideo.cs: Definitions for CoreVideo
//
// Copyright 2014 Xamarin Inc. All rights reserved.
// Copyright 2020 Microsoft Corporation
//

using System;
using Foundation;
using ObjCRuntime;
#if !WATCH
using Metal;
#endif

namespace CoreVideo {

	[Partial]
	interface CVPixelBufferPoolAllocationSettings {

		[Internal]
		[Field ("kCVPixelBufferPoolAllocationThresholdKey")]
		NSString ThresholdKey { get; }
	}

	[Partial]
	interface CVBuffer {

		[Field ("kCVBufferMovieTimeKey")]
		NSString MovieTimeKey { get; }

		[Field ("kCVBufferTimeValueKey")]
		NSString TimeValueKey { get; }

		[Field ("kCVBufferTimeScaleKey")]
		NSString TimeScaleKey { get; }

		[Field ("kCVBufferPropagatedAttachmentsKey")]
		NSString PropagatedAttachmentsKey { get; }

		[Field ("kCVBufferNonPropagatedAttachmentsKey")]
		NSString NonPropagatedAttachmentsKey { get; }
	}

	[Partial]
	interface CVImageBuffer : CVBuffer {

		[Field ("kCVImageBufferCGColorSpaceKey")]
		NSString CGColorSpaceKey { get; }

		[Field ("kCVImageBufferGammaLevelKey")]
		NSString GammaLevelKey { get; }

		[Field ("kCVImageBufferCleanApertureKey")]
		NSString CleanApertureKey { get; }

		[Field ("kCVImageBufferPreferredCleanApertureKey")]
		NSString PreferredCleanApertureKey { get; }

		[Field ("kCVImageBufferCleanApertureWidthKey")]
		NSString CleanApertureWidthKey { get; }

		[Field ("kCVImageBufferCleanApertureHeightKey")]
		NSString CleanApertureHeightKey { get; }

		[Field ("kCVImageBufferCleanApertureHorizontalOffsetKey")]
		NSString CleanApertureHorizontalOffsetKey { get; }

		[Field ("kCVImageBufferCleanApertureVerticalOffsetKey")]
		NSString CleanApertureVerticalOffsetKey { get; }

		[Field ("kCVImageBufferFieldCountKey")]
		NSString FieldCountKey { get; }

		[Field ("kCVImageBufferFieldDetailKey")]
		NSString FieldDetailKey { get; }

		[Field ("kCVImageBufferFieldDetailTemporalTopFirst")]
		NSString FieldDetailTemporalTopFirst { get; }

		[Field ("kCVImageBufferFieldDetailTemporalBottomFirst")]
		NSString FieldDetailTemporalBottomFirst { get; }

		[Field ("kCVImageBufferFieldDetailSpatialFirstLineEarly")]
		NSString FieldDetailSpatialFirstLineEarly { get; }

		[Field ("kCVImageBufferFieldDetailSpatialFirstLineLate")]
		NSString FieldDetailSpatialFirstLineLate { get; }

		[Field ("kCVImageBufferPixelAspectRatioKey")]
		NSString PixelAspectRatioKey { get; }

		[Field ("kCVImageBufferPixelAspectRatioHorizontalSpacingKey")]
		NSString PixelAspectRatioHorizontalSpacingKey { get; }

		[Field ("kCVImageBufferPixelAspectRatioVerticalSpacingKey")]
		NSString PixelAspectRatioVerticalSpacingKey { get; }

		[Field ("kCVImageBufferDisplayDimensionsKey")]
		NSString DisplayDimensionsKey { get; }

		[Field ("kCVImageBufferDisplayWidthKey")]
		NSString DisplayWidthKey { get; }

		[Field ("kCVImageBufferDisplayHeightKey")]
		NSString DisplayHeightKey { get; }

		[Field ("kCVImageBufferYCbCrMatrixKey")]
		NSString YCbCrMatrixKey { get; }

		[Static]
		[Wrap ("CVImageBufferYCbCrMatrix.ItuR709_2.GetConstant ()")]
		NSString YCbCrMatrix_ITU_R_709_2 { get; }

		[Static]
		[Wrap ("CVImageBufferYCbCrMatrix.ItuR601_4.GetConstant ()")]
		NSString YCbCrMatrix_ITU_R_601_4 { get; }

		[Static]
		[Wrap ("CVImageBufferYCbCrMatrix.Smpte240M1995.GetConstant ()")]
		NSString YCbCrMatrix_SMPTE_240M_1995 { get; }

		[Static]
		[Wrap ("CVImageBufferYCbCrMatrix.DciP3.GetConstant ()")]
		[MacCatalyst (13, 1)]
		NSString YCbCrMatrix_DCI_P3 { get; }

		[Static]
		[Wrap ("CVImageBufferYCbCrMatrix.P3D65.GetConstant ()")]
		[MacCatalyst (13, 1)]
		NSString YCbCrMatrix_P3_D65 { get; }

		[Static]
		[Wrap ("CVImageBufferYCbCrMatrix.ItuR2020.GetConstant ()")]
		[MacCatalyst (13, 1)]
		NSString YCbCrMatrix_ITU_R_2020 { get; }

		[Static]
		[Wrap ("CVImageBufferColorPrimaries.DciP3.GetConstant ()")]
		[MacCatalyst (13, 1)]
		NSString ColorPrimaries_DCI_P3 { get; }

		[Static]
		[Wrap ("CVImageBufferColorPrimaries.ItuR2020.GetConstant ()")]
		[MacCatalyst (13, 1)]
		NSString ColorPrimaries_ITU_R_2020 { get; }

		[Static]
		[Wrap ("CVImageBufferColorPrimaries.P3D65.GetConstant ()")]
		[MacCatalyst (13, 1)]
		NSString ColorPrimaries_P3_D65 { get; }

		[Field ("kCVImageBufferChromaSubsamplingKey")]
		NSString ChromaSubsamplingKey { get; }

		[Field ("kCVImageBufferChromaSubsampling_420")]
		NSString ChromaSubsampling_420 { get; }

		[Field ("kCVImageBufferChromaSubsampling_422")]
		NSString ChromaSubsampling_422 { get; }

		[Field ("kCVImageBufferChromaSubsampling_411")]
		NSString ChromaSubsampling_411 { get; }

		[Field ("kCVImageBufferTransferFunctionKey")]
		NSString TransferFunctionKey { get; }

		[Static]
		[Wrap ("CVImageBufferTransferFunction.ItuR709_2.GetConstant ()")]
		NSString TransferFunction_ITU_R_709_2 { get; }

		[Static]
		[Wrap ("CVImageBufferTransferFunction.Smpte240M1995.GetConstant ()")]
		NSString TransferFunction_SMPTE_240M_1995 { get; }

		[Static]
		[Wrap ("CVImageBufferTransferFunction.UseGamma.GetConstant ()")]
		NSString TransferFunction_UseGamma { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("CVImageBufferTransferFunction.ItuR2020.GetConstant ()")]
		NSString TransferFunction_ITU_R_2020 { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("CVImageBufferTransferFunction.SmpteST428_1.GetConstant ()")]
		NSString TransferFunction_SMPTE_ST_428_1 { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("CVImageBufferTransferFunction.SRgb.GetConstant ()")]
		NSString TransferFunction_sRGB { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("CVImageBufferTransferFunction.SmpteST2084PQ.GetConstant ()")]
		NSString TransferFunction_SMPTE_ST_2084_PQ { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("CVImageBufferTransferFunction.ItuR2100Hlg.GetConstant ()")]
		NSString TransferFunction_ITU_R_2100_HLG { get; }

		[Field ("kCVImageBufferChromaLocationTopFieldKey")]
		NSString ChromaLocationTopFieldKey { get; }

		[Field ("kCVImageBufferChromaLocationBottomFieldKey")]
		NSString ChromaLocationBottomFieldKey { get; }

		[Field ("kCVImageBufferChromaLocation_Left")]
		NSString ChromaLocation_Left { get; }

		[Field ("kCVImageBufferChromaLocation_Center")]
		NSString ChromaLocation_Center { get; }

		[Field ("kCVImageBufferChromaLocation_TopLeft")]
		NSString ChromaLocation_TopLeft { get; }

		[Field ("kCVImageBufferChromaLocation_Top")]
		NSString ChromaLocation_Top { get; }

		[Field ("kCVImageBufferChromaLocation_BottomLeft")]
		NSString ChromaLocation_BottomLeft { get; }

		[Field ("kCVImageBufferChromaLocation_Bottom")]
		NSString ChromaLocation_Bottom { get; }

		[Field ("kCVImageBufferChromaLocation_DV420")]
		NSString ChromaLocation_DV420 { get; }

		[Field ("kCVImageBufferColorPrimariesKey")]
		NSString ColorPrimariesKey { get; }

		[Static]
		[Wrap ("CVImageBufferColorPrimaries.ItuR709_2.GetConstant ()")]
		NSString ColorPrimaries_ITU_R_709_2 { get; }

		[Static]
		[Wrap ("CVImageBufferColorPrimaries.Ebu3213.GetConstant ()")]
		NSString ColorPrimaries_EBU_3213 { get; }

		[Static]
		[Wrap ("CVImageBufferColorPrimaries.SmpteC.GetConstant ()")]
		NSString ColorPrimaries_SMPTE_C { get; }

		[Static]
		[Wrap ("CVImageBufferColorPrimaries.P22.GetConstant ()")]
		NSString ColorPrimaries_P22 { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCVImageBufferAlphaChannelIsOpaque")]
		NSString AlphaChannelIsOpaque { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCVImageBufferMasteringDisplayColorVolumeKey")]
		NSString MasteringDisplayColorVolumeKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCVImageBufferContentLightLevelInfoKey")]
		NSString ContentLightLevelInfoKey { get; }

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCVImageBufferAlphaChannelModeKey")]
		NSString AlphaChannelModeKey { get; }

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kCVImageBufferRegionOfInterestKey")]
		NSString RegionOfInterestKey { get; }

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kCVImageBufferAmbientViewingEnvironmentKey")]
		NSString AmbientViewingEnvironmentKey { get; }

		[Watch (10, 2), TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
		[Field ("kCVImageBufferLogTransferFunctionKey")]
		NSString LogTransferFunctionKey { get; }

		[Watch (10, 2), TV (17, 2), Mac (14, 2), iOS (17, 2), MacCatalyst (17, 2)]
		[Field ("kCVImageBufferLogTransferFunction_AppleLog")]
		NSString LogTransferFunctionAppleLogKey { get; }
	}

	[MacCatalyst (13, 1)]
	enum CVImageBufferTransferFunction {

		[Field (null)]
		Unknown = 2, // 2 (the code point for "unknown")

		[Field ("kCVImageBufferTransferFunction_ITU_R_709_2")]
		ItuR709_2,

		[Field ("kCVImageBufferTransferFunction_SMPTE_240M_1995")]
		Smpte240M1995,

		[Field ("kCVImageBufferTransferFunction_UseGamma")]
		UseGamma,

		[MacCatalyst (13, 1)]
		[Field ("kCVImageBufferTransferFunction_ITU_R_2020")]
		ItuR2020,

		[MacCatalyst (13, 1)]
		[Field ("kCVImageBufferTransferFunction_SMPTE_ST_428_1")]
		SmpteST428_1,

		[MacCatalyst (13, 1)]
		[Field ("kCVImageBufferTransferFunction_sRGB")]
		SRgb,

		[MacCatalyst (13, 1)]
		[Field ("kCVImageBufferTransferFunction_SMPTE_ST_2084_PQ")]
		SmpteST2084PQ,

		[MacCatalyst (13, 1)]
		[Field ("kCVImageBufferTransferFunction_ITU_R_2100_HLG")]
		ItuR2100Hlg,

		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCVImageBufferTransferFunction_Linear")]
		Linear,
	}

	[MacCatalyst (13, 1)]
	enum CVImageBufferColorPrimaries {

		[Field (null)]
		Unknown = 2, // 2 (the code point for "unknown")

		[Field ("kCVImageBufferColorPrimaries_DCI_P3")]
		[MacCatalyst (13, 1)]
		DciP3,

		[Field ("kCVImageBufferColorPrimaries_ITU_R_2020")]
		[MacCatalyst (13, 1)]
		ItuR2020,

		[Field ("kCVImageBufferColorPrimaries_P3_D65")]
		[MacCatalyst (13, 1)]
		P3D65,

		[Field ("kCVImageBufferColorPrimaries_ITU_R_709_2")]
		ItuR709_2,

		[Field ("kCVImageBufferColorPrimaries_EBU_3213")]
		Ebu3213,

		[Field ("kCVImageBufferColorPrimaries_SMPTE_C")]
		SmpteC,

		[Field ("kCVImageBufferColorPrimaries_P22")]
		P22,
	}

	[MacCatalyst (13, 1)]
	enum CVImageBufferYCbCrMatrix {

		[Field (null)]
		Unknown = 2, // 2 (the code point for "unknown")

		[Field ("kCVImageBufferYCbCrMatrix_ITU_R_709_2")]
		ItuR709_2,

		[Field ("kCVImageBufferYCbCrMatrix_ITU_R_601_4")]
		ItuR601_4,

		[Field ("kCVImageBufferYCbCrMatrix_SMPTE_240M_1995")]
		Smpte240M1995,

		[Deprecated (PlatformName.iOS, 14, 0, message: "This API is no longer supported.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "This API is no longer supported.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "This API is no longer supported.")]
		[Field ("kCVImageBufferYCbCrMatrix_DCI_P3")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "This API is no longer supported.")]
		DciP3,

		[Deprecated (PlatformName.iOS, 14, 0, message: "This API is no longer supported.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "This API is no longer supported.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "This API is no longer supported.")]
		[Field ("kCVImageBufferYCbCrMatrix_P3_D65")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "This API is no longer supported.")]
		P3D65,

		[Field ("kCVImageBufferYCbCrMatrix_ITU_R_2020")]
		[MacCatalyst (13, 1)]
		ItuR2020,
	}

	[Partial]
	interface CVPixelBuffer {

		[Field ("kCVPixelBufferPixelFormatTypeKey")]
		NSString PixelFormatTypeKey { get; }

		[Field ("kCVPixelBufferMemoryAllocatorKey")]
		NSString MemoryAllocatorKey { get; }

		[Field ("kCVPixelBufferWidthKey")]
		NSString WidthKey { get; }

		[Field ("kCVPixelBufferHeightKey")]
		NSString HeightKey { get; }

		[Field ("kCVPixelBufferExtendedPixelsLeftKey")]
		NSString ExtendedPixelsLeftKey { get; }

		[Field ("kCVPixelBufferExtendedPixelsTopKey")]
		NSString ExtendedPixelsTopKey { get; }

		[Field ("kCVPixelBufferExtendedPixelsRightKey")]
		NSString ExtendedPixelsRightKey { get; }

		[Field ("kCVPixelBufferExtendedPixelsBottomKey")]
		NSString ExtendedPixelsBottomKey { get; }

		[Field ("kCVPixelBufferBytesPerRowAlignmentKey")]
		NSString BytesPerRowAlignmentKey { get; }

		[Field ("kCVPixelBufferCGBitmapContextCompatibilityKey")]
		NSString CGBitmapContextCompatibilityKey { get; }

		[Field ("kCVPixelBufferCGImageCompatibilityKey")]
		NSString CGImageCompatibilityKey { get; }

		[Field ("kCVPixelBufferOpenGLCompatibilityKey")]
		NSString OpenGLCompatibilityKey { get; }

		[Field ("kCVPixelBufferIOSurfacePropertiesKey")]
		NSString IOSurfacePropertiesKey { get; }

		[Field ("kCVPixelBufferPlaneAlignmentKey")]
		NSString PlaneAlignmentKey { get; }

		[NoMac]
		[NoWatch]
		[NoMacCatalyst]
		[Field ("kCVPixelBufferOpenGLESCompatibilityKey")]
		NSString OpenGLESCompatibilityKey { get; }

		[NoMac]
		[NoWatch]
		[NoMacCatalyst]
		[Field ("kCVPixelBufferOpenGLESTextureCacheCompatibilityKey")]
		NSString OpenGLESTextureCacheCompatibilityKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCVPixelBufferMetalCompatibilityKey")]
		NSString MetalCompatibilityKey { get; }

		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		[Field ("kCVPixelBufferOpenGLTextureCacheCompatibilityKey")]
		NSString OpenGLTextureCacheCompatibilityKey { get; }

		[NoWatch, NoTV, iOS (14, 0), MacCatalyst (15, 0)]
		[Field ("kCVPixelBufferProResRAWKey_BlackLevel")]
		NSString ProResRawKey_BlackLevel { get; }

		[NoWatch, NoTV, iOS (14, 0), MacCatalyst (15, 0)]
		[Field ("kCVPixelBufferProResRAWKey_ColorMatrix")]
		NSString ProResRawKey_ColorMatrix { get; }

		[NoWatch, NoTV, iOS (14, 0), MacCatalyst (15, 0)]
		[Field ("kCVPixelBufferProResRAWKey_GainFactor")]
		NSString ProResRawKey_GainFactor { get; }

		[NoWatch, NoTV, iOS (14, 0), MacCatalyst (15, 0)]
		[Field ("kCVPixelBufferProResRAWKey_RecommendedCrop")]
		NSString ProResRawKey_RecommendedCrop { get; }

		[NoWatch, NoTV, iOS (14, 0), MacCatalyst (15, 0)]
		[Field ("kCVPixelBufferProResRAWKey_SenselSitingOffsets")]
		NSString ProResRawKey_SenselSitingOffsets { get; }

		[NoWatch, NoTV, iOS (14, 0), MacCatalyst (15, 0)]
		[Field ("kCVPixelBufferProResRAWKey_WhiteBalanceBlueFactor")]
		NSString ProResRawKey_WhiteBalanceBlueFactor { get; }

		[NoWatch, NoTV, iOS (14, 0), MacCatalyst (15, 0)]
		[Field ("kCVPixelBufferProResRAWKey_WhiteBalanceCCT")]
		NSString ProResRawKey_WhiteBalanceCct { get; }

		[NoWatch, NoTV, iOS (14, 0), MacCatalyst (15, 0)]
		[Field ("kCVPixelBufferProResRAWKey_WhiteBalanceRedFactor")]
		NSString ProResRawKey_WhiteBalanceRedFactor { get; }

		[NoWatch, NoTV, iOS (14, 0), MacCatalyst (15, 0)]
		[Field ("kCVPixelBufferProResRAWKey_WhiteLevel")]
		NSString ProResRawKey_WhiteLevel { get; }

		[NoWatch, NoTV, iOS (14, 0), MacCatalyst (15, 0)]
		[Field ("kCVPixelBufferVersatileBayerKey_BayerPattern")]
		NSString VersatileBayerKey_BayerPattern { get; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kCVPixelBufferProResRAWKey_MetadataExtension")]
		NSString MetadataExtension { get; }
	}

	[Partial]
#if XAMCORE_3_0
	interface CVPixelBufferPool {
#else
	interface CVPixelBufferPool : CVImageBuffer {
#endif
		[Field ("kCVPixelBufferPoolMinimumBufferCountKey")]
		NSString MinimumBufferCountKey { get; }

		[Field ("kCVPixelBufferPoolMaximumBufferAgeKey")]
		NSString MaximumBufferAgeKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Partial]
	interface CVMetalTextureCache {
		[MacCatalyst (13, 1)]
		[Internal]
		[Field ("kCVMetalTextureCacheMaximumTextureAgeKey")]
		IntPtr MaxTextureAge { get; }

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCVMetalTextureStorageMode")]
		NSString StorageMode { get; }
	}

	// CVOpenGLESTextureCache is bound (manually) in OpenTK[-1.0].dll.
	// [Partial]
	// interface CVOpenGLESTextureCache {
	// 	[Internal]
	// 	[Field ("kCVOpenGLESTextureCacheMaximumTextureAgeKey")]
	// 	IntPtr MaxTextureAge { get; }
	// }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static, Internal]
	interface CVMetalTextureAttributesKeys {

		[Field ("kCVMetalTextureUsage")]
		NSString UsageKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("CVMetalTextureAttributesKeys")]
	interface CVMetalTextureAttributes {
		// Create stub DictionaryContainer class
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum CVVersatileBayerPattern : uint {
		Rggb = 0,
		Grbg = 1,
		Gbrg = 2,
		Bggr = 3,
	}
}
