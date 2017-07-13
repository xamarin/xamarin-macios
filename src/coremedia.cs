//
// coremedia.cs: Definitions for CoreMedia
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.CoreMedia {
	[Partial]
	interface CMMemoryPool {

		[Mac (10,8)]
		[Internal][Field ("kCMMemoryPoolOption_AgeOutPeriod")]
		IntPtr AgeOutPeriodSelector { get; }
	}

	[Static][Internal]
	[Mavericks, Since (6,0)]
	interface CMTextMarkupAttributesKeys {
		[Internal][Field ("kCMTextMarkupAttribute_ForegroundColorARGB")]
		NSString ForegroundColorARGB { get; }

		[Internal][Field ("kCMTextMarkupAttribute_BackgroundColorARGB")]
		NSString BackgroundColorARGB { get; }

		[Internal][Field ("kCMTextMarkupAttribute_BoldStyle")]
		NSString BoldStyle { get; }

		[Internal][Field ("kCMTextMarkupAttribute_ItalicStyle")]
		NSString ItalicStyle { get; }

		[Internal][Field ("kCMTextMarkupAttribute_UnderlineStyle")]
		NSString UnderlineStyle { get; }

		[Internal][Field ("kCMTextMarkupAttribute_FontFamilyName")]
		NSString FontFamilyName { get; }

		[Internal][Field ("kCMTextMarkupAttribute_RelativeFontSize")]
		NSString RelativeFontSize { get; }
	}

	[Static][Internal]
	interface CMSampleAttachmentKey {
		[Field ("kCMSampleAttachmentKey_NotSync")]
		NSString NotSync { get; }

		[Field ("kCMSampleAttachmentKey_PartialSync")]
		NSString PartialSync { get; }

		[Field ("kCMSampleAttachmentKey_HasRedundantCoding")]
		NSString HasRedundantCoding { get; }

		[Field ("kCMSampleAttachmentKey_IsDependedOnByOthers")]
		NSString IsDependedOnByOthers { get; }

		[Field ("kCMSampleAttachmentKey_DependsOnOthers")]
		NSString DependsOnOthers { get; }

		[Field ("kCMSampleAttachmentKey_EarlierDisplayTimesAllowed")]
		NSString EarlierDisplayTimesAllowed { get; }

		[Field ("kCMSampleAttachmentKey_DisplayImmediately")]
		NSString DisplayImmediately { get; }

		[Field ("kCMSampleAttachmentKey_DoNotDisplay")]
		NSString DoNotDisplay { get; }

		[Field ("kCMSampleBufferAttachmentKey_ResetDecoderBeforeDecoding")]
		NSString ResetDecoderBeforeDecoding { get; }

		[Field ("kCMSampleBufferAttachmentKey_DrainAfterDecoding")]
		NSString DrainAfterDecoding { get; }

		[Field ("kCMSampleBufferAttachmentKey_PostNotificationWhenConsumed")]
		NSString PostNotificationWhenConsumed { get; }

		[Field ("kCMSampleBufferAttachmentKey_ResumeOutput")]
		NSString ResumeOutput { get; }

		[Field ("kCMSampleBufferAttachmentKey_TransitionID")]
		NSString TransitionID { get; }

		[Field ("kCMSampleBufferAttachmentKey_TrimDurationAtStart")]
		NSString TrimDurationAtStart { get; }

		[Field ("kCMSampleBufferAttachmentKey_TrimDurationAtEnd")]
		NSString TrimDurationAtEnd { get; }

		[Field ("kCMSampleBufferAttachmentKey_SpeedMultiplier")]
		NSString SpeedMultiplier { get; }

		[Field ("kCMSampleBufferAttachmentKey_Reverse")]
		NSString Reverse { get; }

		[Field ("kCMSampleBufferAttachmentKey_FillDiscontinuitiesWithSilence")]
		NSString FillDiscontinuitiesWithSilence { get; }

		[Field ("kCMSampleBufferAttachmentKey_EmptyMedia")]
		NSString EmptyMedia { get; }

		[Field ("kCMSampleBufferAttachmentKey_PermanentEmptyMedia")]
		NSString PermanentEmptyMedia { get; }

		[Field ("kCMSampleBufferAttachmentKey_DisplayEmptyMediaImmediately")]
		NSString DisplayEmptyMediaImmediately { get; }

		[Field ("kCMSampleBufferAttachmentKey_EndsPreviousSampleDuration")]
		NSString EndsPreviousSampleDuration { get; }

		[Field ("kCMSampleBufferAttachmentKey_SampleReferenceURL")]
		NSString SampleReferenceURL { get; }

		[Field ("kCMSampleBufferAttachmentKey_SampleReferenceByteOffset")]
		NSString SampleReferenceByteOffset { get; }

		[Field ("kCMSampleBufferAttachmentKey_GradualDecoderRefresh")]
		NSString GradualDecoderRefresh { get; }

#if !MONOMAC
		[iOS (6,0)]
		[Field ("kCMSampleBufferAttachmentKey_DroppedFrameReason")]
		NSString DroppedFrameReason { get; }

		[iOS (9,0)]
		[Field ("kCMSampleBufferAttachmentKey_StillImageLensStabilizationInfo")]
		NSString StillImageLensStabilizationInfo { get; }

		[iOS (9,0)]
		[Field ("kCMSampleBufferLensStabilizationInfo_Active")]
		NSString BufferLensStabilizationInfo_Active { get; }

		[iOS (9,0)]
		[Field ("kCMSampleBufferLensStabilizationInfo_OutOfRange")]
		NSString BufferLensStabilizationInfo_OutOfRange { get; }

		[iOS (9,0)]
		[Field ("kCMSampleBufferLensStabilizationInfo_Unavailable")]
		NSString BufferLensStabilizationInfo_Unavailable { get; }

		[iOS (9,0)]
		[Field ("kCMSampleBufferLensStabilizationInfo_Off")]
		NSString BufferLensStabilizationInfo_Off { get; }
#endif
	}

#if false
	// right now the generator can't add fields in a partial struct
	[Partial]
	interface CMTime {
		[Field ("kCMTimeValueKey")]
		NSString ValueKey { get; }

		[Field ("kCMTimeScaleKey")]
		NSString ScaleKey { get; }

		[Field ("kCMTimeEpochKey")]
		NSString EpochKey { get; }

		[Field ("kCMTimeFlagsKey")]
		NSString FlagsKey { get; }
	}
#endif

	[Static]
	interface CMFormatDescriptionExtension {
		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionExtension_FormatName")]
		NSString FormatName { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionExtension_Depth")]
		NSString Depth { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionExtension_CleanAperture")]
		NSString CleanAperture { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionExtension_FieldCount")]
		NSString FieldCount { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionExtension_FieldDetail")]
		NSString FieldDetail { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionExtension_PixelAspectRatio")]
		NSString PixelAspectRatio { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionExtension_ColorPrimaries")]
		NSString ColorPrimaries { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionExtension_TransferFunction")]
		NSString TransferFunction { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionExtension_GammaLevel")]
		NSString GammaLevel { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionExtension_YCbCrMatrix")]
		NSString YCbCrMatrix { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionExtension_FullRangeVideo")]
		NSString FullRangeVideo { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionExtension_ICCProfile")]
		NSString IccProfile { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionExtension_BytesPerRow")]
		NSString BytesPerRow { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionExtension_ChromaLocationTopField")]
		NSString ChromaLocationTopField { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionExtension_ChromaLocationBottomField")]
		NSString ChromaLocationBottomField { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionExtension_TemporalQuality")]
		NSString TemporalQuality { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionExtension_SpatialQuality")]
		NSString SpatialQuality { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionExtension_VerbatimImageDescription")]
		NSString VerbatimImageDescription { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionExtension_Version")]
		NSString Version { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionExtension_RevisionLevel")]
		NSString RevisionLevel { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionExtension_Vendor")]
		NSString Vendor { get; }

		[Mac (10, 13)]
		[Field ("kCMFormatDescriptionExtension_MasteringDisplayColorVolume")]
		NSString MasteringDisplayColorVolume { get; }

		[Mac (10, 13)]
		[Field ("kCMFormatDescriptionExtension_ContentLightLevelInfo")]
		NSString ContentLightLevelInfo { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionExtension_DisplayFlags")]
		NSString DisplayFlags { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionExtension_BackgroundColor")]
		NSString BackgroundColor { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionExtension_DefaultTextBox")]
		NSString DefaultTextBox { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionExtension_DefaultStyle")]
		NSString DefaultStyle { get; }
	}

	[Static]
	interface CMFormatDescriptionKey {
		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionKey_CleanApertureWidth")]
		NSString CleanApertureWidth { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionKey_CleanApertureHeight")]
		NSString CleanApertureHeight { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionKey_CleanApertureHorizontalOffset")]
		NSString CleanApertureHorizontalOffset { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionKey_CleanApertureVerticalOffset")]
		NSString CleanApertureVerticalOffset { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionKey_CleanApertureWidthRational")]
		NSString CleanApertureWidthRational { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionKey_CleanApertureHeightRational")]
		NSString CleanApertureHeightRational { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionKey_CleanApertureHorizontalOffsetRational")]
		NSString CleanApertureHorizontalOffsetRational { get; }

		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionKey_CleanApertureVerticalOffsetRational")]
		NSString CleanApertureVerticalOffsetRational { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionKey_PixelAspectRatioHorizontalSpacing")]
		NSString PixelAspectRatioHorizontalSpacing { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionKey_PixelAspectRatioVerticalSpacing")]
		NSString PixelAspectRatioVerticalSpacing { get; }
	}

	[Static]
	interface CMFormatDescriptionFieldDetail {
		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionFieldDetail_TemporalTopFirst")]
		NSString TemporalTopFirst { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionFieldDetail_TemporalBottomFirst")]
		NSString TemporalBottomFirst { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionFieldDetail_SpatialFirstLineEarly")]
		NSString SpatialFirstLineEarly { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionFieldDetail_SpatialFirstLineLate")]
		NSString SpatialFirstLineLate { get; }
	}

	[Static]
	interface CMFormatDescriptionColorPrimaries {
		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionColorPrimaries_ITU_R_709_2")]
		NSString Itu_R_709_2 { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionColorPrimaries_EBU_3213")]
		NSString Ebu_3213 { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionColorPrimaries_SMPTE_C")]
		NSString Smpte_C { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionColorPrimaries_DCI_P3")]
		NSString Dci_P3 { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionColorPrimaries_P3_D65")]
		NSString P3_D65 { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionColorPrimaries_ITU_R_2020")]
		NSString Itu_R_2020 { get; }

		[Mac (10, 8)]
		[Field ("kCMFormatDescriptionColorPrimaries_P22")]
		NSString P22 { get; }
	}

	[Static]
	interface MFormatDescriptionTransferFunction {
		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionTransferFunction_ITU_R_709_2")]
		NSString Itu_R_709_2 { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionTransferFunction_SMPTE_240M_1995")]
		NSString Smpte_240M_1995 { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionTransferFunction_UseGamma")]
		NSString UseGamma { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionTransferFunction_ITU_R_2020")]
		NSString Itu_R_2020 { get; }

		[Mac (10, 12)]
		[Field ("kCMFormatDescriptionTransferFunction_SMPTE_ST_428_1")]
		NSString Smpte_ST_428_1 { get; }

		[Mac (10, 13)]
		[Field ("kCMFormatDescriptionTransferFunction_SMPTE_ST_2084_PQ")]
		NSString Smpte_ST_2084_PQ { get; }

		[Mac (10, 13)]
		[Field ("kCMFormatDescriptionTransferFunction_ITU_R_2100_HLG")]
		NSString Itu_R_2100_HLG { get; }
	}

	[Static]
	interface CMFormatDescriptionYCbCrMatrix {
		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionYCbCrMatrix_ITU_R_709_2")]
		NSString Itu_R_709_2 { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionYCbCrMatrix_ITU_R_601_4")]
		NSString Itu_R_601_4 { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionYCbCrMatrix_SMPTE_240M_1995")]
		NSString Smpte_240M_1995 { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionYCbCrMatrix_ITU_R_2020")]
		NSString Itu_R_2020 { get; }
	}

	[Static]
	interface CMFormatDescriptionChromaLocation {
		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionChromaLocation_Left")]
		NSString Left { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionChromaLocation_Center")]
		NSString Center { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionChromaLocation_TopLeft")]
		NSString TopLeft { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionChromaLocation_Top")]
		NSString Top { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionChromaLocation_BottomLeft")]
		NSString BottomLeft { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionChromaLocation_Bottom")]
		NSString Bottom { get; }

		[Mac (10, 11)]
		[Field ("kCMFormatDescriptionChromaLocation_DV420")]
		NSString DV420 { get; }
	}

	[Static]
	interface CMFormatDescriptionVendor {
		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionVendor_Apple")]
		NSString Apple { get; }
	}

	[Static]
	interface CMTextFormatDescriptionColor {
		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionColor_Red")]
		NSString Red { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionColor_Green")]
		NSString Green { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionColor_Blue")]
		NSString Blue { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionColor_Alpha")]
		NSString Alpha { get; }
	}

	[Static]
	interface CMTextFormatDescriptionRect {
		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionRect_Top")]
		NSString Top { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionRect_Left")]
		NSString Left { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionRect_Bottom")]
		NSString Bottom { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionRect_Right")]
		NSString Right { get; }
	}

	[Static]
	interface CMTextFormatDescriptionStyle {
		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionStyle_StartChar")]
		NSString StartChar { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionStyle_Font")]
		NSString Font { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionStyle_FontFace")]
		NSString FontFace { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionStyle_ForegroundColor")]
		NSString ForegroundColor { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionStyle_FontSize")]
		NSString FontSize { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionStyle_EndChar")]
		NSString EndChar { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionStyle_Height")]
		NSString Height { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionStyle_Ascent")]
		NSString Ascent { get; }
	}

	[Static]
	interface CMTextFormatDescriptionExtension {
		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionExtension_HorizontalJustification")]
		NSString HorizontalJustification { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionExtension_VerticalJustification")]
		NSString VerticalJustification { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionExtension_FontTable")]
		NSString FontTable { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionExtension_TextJustification")]
		NSString TextJustification { get; }

		[Mac (10, 7)]
		[Field ("kCMTextFormatDescriptionExtension_DefaultFontName")]
		NSString DefaultFontName { get; }
	}

	[Static]
	interface CMTimeCodeFormatDescriptionExtension {
		[Mac (10, 7)]
		[Field ("kCMTimeCodeFormatDescriptionExtension_SourceReferenceName")]
		NSString SourceReferenceName { get; }
	}

	[Static]
	interface CMFormatDescriptionExtensionKey {
		[Mac (10, 7)]
		[Field ("kCMFormatDescriptionExtensionKey_MetadataKeyTable")]
		NSString MetadataKeyTable { get; }
	}

	[Static]
	interface CMMetadataFormatDescriptionKey {
		[Mac (10, 7)]
		[Field ("kCMMetadataFormatDescriptionKey_Namespace")]
		NSString Namespace { get; }

		[Mac (10, 7)]
		[Field ("kCMMetadataFormatDescriptionKey_Value")]
		NSString Value { get; }

		[Mac (10, 7)]
		[Field ("kCMMetadataFormatDescriptionKey_LocalID")]
		NSString LocalId { get; }

		[Mac (10, 10)]
		[Field ("kCMMetadataFormatDescriptionKey_DataType")]
		NSString DataType { get; }

		[Mac (10, 10)]
		[Field ("kCMMetadataFormatDescriptionKey_DataTypeNamespace")]
		NSString DataTypeNamespace { get; }

		[Mac (10, 10)]
		[Field ("kCMMetadataFormatDescriptionKey_ConformingDataTypes")]
		NSString ConformingDataTypes { get; }

		[Mac (10, 10)]
		[Field ("kCMMetadataFormatDescriptionKey_LanguageTag")]
		NSString LanguageTag { get; }

		[Mac (10, 11)]
		[Field ("kCMMetadataFormatDescriptionKey_StructuralDependency")]
		NSString StructuralDependency { get; }

		[Mac (10, 11)]
		[Field ("kCMMetadataFormatDescriptionKey_SetupData")]
		NSString SetupData { get; }
	}

	[Static]
	interface CMMetadataFormatDescriptionMetadataSpecificationKey {
		[Mac (10, 10)]
		[Field ("kCMMetadataFormatDescriptionMetadataSpecificationKey_Identifier")]
		NSString Identifier { get; }

		[Mac (10, 10)]
		[Field ("kCMMetadataFormatDescriptionMetadataSpecificationKey_DataType")]
		NSString DataType { get; }

		[Mac (10, 10)]
		[Field ("kCMMetadataFormatDescriptionMetadataSpecificationKey_ExtendedLanguageTag")]
		NSString ExtendedLanguageTag { get; }

		[Mac (10, 11)]
		[Field ("kCMMetadataFormatDescriptionMetadataSpecificationKey_StructuralDependency")]
		NSString StructuralDependency { get; }

		[Mac (10, 11)]
		[Field ("kCMMetadataFormatDescriptionMetadataSpecificationKey_SetupData")]
		NSString SetupData { get ; }
	}

	[Static]
	interface CMMetadataFormatDescription_StructuralDependencyKey {
		[Mac (10, 11)]
		[Field ("kCMMetadataFormatDescription_StructuralDependencyKey_DependencyIsInvalidFlag")]
		NSString DependencyIsInvalidFlag { get; }
	}

	[Static]
	interface CMTimeCodeFormatDescriptionKey {
		[Mac (10, 7)]
		[Field ("kCMTimeCodeFormatDescriptionKey_Value")]
		NSString Value { get; }

		[Mac (10, 7)]
		[Field ("kCMTimeCodeFormatDescriptionKey_LangCode")]
		NSString LangCode { get; }
	}

	//partial interface CMFormatDescription {
	//	[Mac (10, 7)]
	//	[Field ("kCMFormatDescriptionConformsToMPEG2VideoProfile")]
	//	NSString ConformsToMPEG2VideoProfile { get; }
	//}
}
