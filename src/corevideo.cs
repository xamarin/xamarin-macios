//
// corevideo.cs: Definitions for CoreVideo
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.CoreVideo {

	[Partial]
	interface CVPixelBufferPoolAllocationSettings {

		[Internal][Field ("kCVPixelBufferPoolAllocationThresholdKey")]
		NSString ThresholdKey { get; }
	}

#if XAMCORE_2_0
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
		NSString CGColorSpaceKey  { get; }

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

		[Field ("kCVImageBufferYCbCrMatrix_ITU_R_709_2")]
		NSString YCbCrMatrix_ITU_R_709_2 { get; }

		[Field ("kCVImageBufferYCbCrMatrix_ITU_R_601_4")]
		NSString YCbCrMatrix_ITU_R_601_4 { get; }

		[Field ("kCVImageBufferYCbCrMatrix_SMPTE_240M_1995")]
		NSString YCbCrMatrix_SMPTE_240M_1995 { get; }

#if !MONOMAC
		[Field ("kCVImageBufferYCbCrMatrix_DCI_P3")]
		[iOS (9,0)]
		NSString YCbCrMatrix_DCI_P3 { get; }

		[Field ("kCVImageBufferYCbCrMatrix_P3_D65")]
		[iOS (9,0)]
		NSString YCbCrMatrix_P3_D65 { get; }
#endif

		[Field ("kCVImageBufferYCbCrMatrix_ITU_R_2020")]
		[iOS (9,0), Mac (10,11)]
		NSString YCbCrMatrix_ITU_R_2020 { get; }

		[Field ("kCVImageBufferColorPrimaries_DCI_P3")]
		[iOS (9,0)][Mac (10,11)]
		NSString ColorPrimaries_DCI_P3 { get; }

		[Field ("kCVImageBufferColorPrimaries_ITU_R_2020")]
		[iOS (9,0)][Mac (10,11)]
		NSString ColorPrimaries_ITU_R_2020 { get; }

		[Field ("kCVImageBufferColorPrimaries_P3_D65")]
		[iOS (9,0)][Mac (10,11)]
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

		[Field ("kCVImageBufferTransferFunction_ITU_R_709_2")]
		NSString TransferFunction_ITU_R_709_2 { get; }

		[Field ("kCVImageBufferTransferFunction_SMPTE_240M_1995")]
		NSString TransferFunction_SMPTE_240M_1995 { get; }

		[Field ("kCVImageBufferTransferFunction_UseGamma")]
		NSString TransferFunction_UseGamma { get; }

		[iOS (9,0), Mac (10,11)]
		[TV (10,0)]
		[Field ("kCVImageBufferTransferFunction_ITU_R_2020")]
		NSString TransferFunction_ITU_R_2020 { get; }

		[iOS (10,0)][Mac (10,12)]
		[TV (10,0)]
		[Field ("kCVImageBufferTransferFunction_SMPTE_ST_428_1")]
		NSString TransferFunction_SMPTE_ST_428_1 { get; }

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

		[Field ("kCVImageBufferColorPrimaries_ITU_R_709_2")]
		NSString ColorPrimaries_ITU_R_709_2 { get; }

		[Field ("kCVImageBufferColorPrimaries_EBU_3213")]
		NSString ColorPrimaries_EBU_3213 { get; }

		[Field ("kCVImageBufferColorPrimaries_SMPTE_C")]
		NSString ColorPrimaries_SMPTE_C { get; }

		[iOS (6,0), Mac (10,8)]
		[Field ("kCVImageBufferColorPrimaries_P22")]
		NSString ColorPrimaries_P22 { get; }

		[iOS (8,0), Mac (10,10)]
		[Field ("kCVImageBufferAlphaChannelIsOpaque")]
		NSString AlphaChannelIsOpaque { get; }
		
	}

	[Partial]
	interface CVPixelBuffer {

		[Field ("kCVPixelBufferPixelFormatTypeKey")]
		NSString PixelFormatTypeKey{ get; }

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

#if !MONOMAC || !XAMCORE_2_0
		[iOS (6,0)]
		[Field ("kCVPixelBufferOpenGLESCompatibilityKey")]
		NSString OpenGLESCompatibilityKey { get; }

		[iOS (9,0)]
		[Field ("kCVPixelBufferOpenGLESTextureCacheCompatibilityKey")]
		NSString OpenGLESTextureCacheCompatibilityKey { get; }
#endif

		[iOS (8,0)][Mac (10,11)]
		[Field ("kCVPixelBufferMetalCompatibilityKey")]
		NSString MetalCompatibilityKey { get; }

#if MONOMAC
		[Mac (10,11)]
		[Field ("kCVPixelBufferOpenGLTextureCacheCompatibilityKey")]
		NSString OpenGLTextureCacheCompatibilityKey { get; }
#endif
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

#if !MONOMAC
	[Partial]
	interface CVMetalTextureCache {
		[Internal]
		[Field ("kCVMetalTextureCacheMaximumTextureAgeKey")]
		IntPtr MaxTextureAge { get; }
	}

	// CVOpenGLESTextureCache is bound (manually) in OpenTK[-1.0].dll.
	// [Partial]
	// interface CVOpenGLESTextureCache {
	// 	[Internal]
	// 	[Field ("kCVOpenGLESTextureCacheMaximumTextureAgeKey")]
	// 	IntPtr MaxTextureAge { get; }
	// }
#endif

#endif
}
