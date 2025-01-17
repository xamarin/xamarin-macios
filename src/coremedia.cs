//
// coremedia.cs: Definitions for CoreMedia
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace CoreMedia {

	/// <summary>Class that manages the repetitive allocation and deallocation of large blocks of memory.</summary>
	///     
	///     <!-- TODO: No Apple documentation on this as of 2013-05-01 -->
	[MacCatalyst (13, 1)]
	[Partial]
	interface CMMemoryPool {

		[Internal]
		[Field ("kCMMemoryPoolOption_AgeOutPeriod")]
		IntPtr AgeOutPeriodSelector { get; }
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	public enum CMFormatDescriptionProjectionKind {
		[Field ("kCMFormatDescriptionProjectionKind_Rectilinear")]
		Rectilinear,
	}

	[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	public enum CMFormatDescriptionViewPackingKind {
		[Field ("kCMFormatDescriptionViewPackingKind_SideBySide")]
		SideBySide,

		[Field ("kCMFormatDescriptionViewPackingKind_OverUnder")]
		OverUnder,
	}

	[Static]
	[Internal]
	[MacCatalyst (13, 1)]
	interface CMTextMarkupAttributesKeys {
		[Internal]
		[Field ("kCMTextMarkupAttribute_ForegroundColorARGB")]
		NSString ForegroundColorARGB { get; }

		[Internal]
		[Field ("kCMTextMarkupAttribute_BackgroundColorARGB")]
		NSString BackgroundColorARGB { get; }

		[Internal]
		[Field ("kCMTextMarkupAttribute_BoldStyle")]
		NSString BoldStyle { get; }

		[Internal]
		[Field ("kCMTextMarkupAttribute_ItalicStyle")]
		NSString ItalicStyle { get; }

		[Internal]
		[Field ("kCMTextMarkupAttribute_UnderlineStyle")]
		NSString UnderlineStyle { get; }

		[Internal]
		[Field ("kCMTextMarkupAttribute_FontFamilyName")]
		NSString FontFamilyName { get; }

		[Internal]
		[Field ("kCMTextMarkupAttribute_RelativeFontSize")]
		NSString RelativeFontSize { get; }

		[Internal]
		[Field ("kCMTextMarkupAttribute_BaseFontSizePercentageRelativeToVideoHeight")]
		NSString BaseFontSizePercentageRelativeToVideoHeight { get; }
	}

	[Static]
	[Internal]
	[MacCatalyst (13, 1)]
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
		NSString PostNotificationWhenConsumedKey { get; }

		[Field ("kCMSampleBufferAttachmentKey_ResumeOutput")]
		NSString ResumeOutputKey { get; }

		[Field ("kCMSampleBufferAttachmentKey_TransitionID")]
		NSString TransitionIdKey { get; }

		[Field ("kCMSampleBufferAttachmentKey_TrimDurationAtStart")]
		NSString TrimDurationAtStartKey { get; }

		[Field ("kCMSampleBufferAttachmentKey_TrimDurationAtEnd")]
		NSString TrimDurationAtEndKey { get; }

		[Field ("kCMSampleBufferAttachmentKey_SpeedMultiplier")]
		NSString SpeedMultiplierKey { get; }

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
		NSString SampleReferenceUrlKey { get; }

		[Field ("kCMSampleBufferAttachmentKey_SampleReferenceByteOffset")]
		NSString SampleReferenceByteOffsetKey { get; }

		[Field ("kCMSampleBufferAttachmentKey_GradualDecoderRefresh")]
		NSString GradualDecoderRefreshKey { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferAttachmentKey_DroppedFrameReason")]
		NSString DroppedFrameReason { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferAttachmentKey_StillImageLensStabilizationInfo")]
		NSString StillImageLensStabilizationInfo { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferLensStabilizationInfo_Active")]
		NSString BufferLensStabilizationInfo_Active { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferLensStabilizationInfo_OutOfRange")]
		NSString BufferLensStabilizationInfo_OutOfRange { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferLensStabilizationInfo_Unavailable")]
		NSString BufferLensStabilizationInfo_Unavailable { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferLensStabilizationInfo_Off")]
		NSString BufferLensStabilizationInfo_Off { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCMSampleAttachmentKey_HEVCTemporalLevelInfo")]
		NSString HevcTemporalLevelInfoKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCMSampleAttachmentKey_HEVCTemporalSubLayerAccess")]
		NSString HevcTemporalSubLayerAccessKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCMSampleAttachmentKey_HEVCStepwiseTemporalSubLayerAccess")]
		NSString HevcStepwiseTemporalSubLayerAccessKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCMSampleAttachmentKey_HEVCSyncSampleNALUnitType")]
		NSString HevcSyncSampleNalUnitTypeKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferAttachmentKey_CameraIntrinsicMatrix")]
		NSString CameraIntrinsicMatrixKey { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleAttachmentKey_AudioIndependentSampleDecoderRefreshCount")]
		NSString AudioIndependentSampleDecoderRefreshCountKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferAttachmentKey_ForceKeyFrame")]
		NSString ForceKeyFrameKey { get; }

		[TV (16, 0), Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Field ("kCMSampleAttachmentKey_HDR10PlusPerFrameData")]
		NSString Hdr10PlusPerFrameDataKey { get; }

		[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
		[Field ("kCMSampleAttachmentKey_PostDecodeProcessingMetadata")]
		NSString PostDecodeProcessingMetadataKey { get; }
	}

	/// <summary>The keys for <see cref="T:CoreMedia.CMSampleBuffer" /> attachments.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("CMSampleAttachmentKey")]
	interface CMSampleBufferAttachmentSettings {

		NSDictionary PostNotificationWhenConsumed { get; set; }
		bool ResumeOutput { get; set; }
		int TransitionId { get; set; }
		NSDictionary TrimDurationAtStart { get; set; }
		NSDictionary TrimDurationAtEnd { get; set; }
		float SpeedMultiplier { get; set; }
		NSUrl SampleReferenceUrl { get; set; }
		int SampleReferenceByteOffset { get; set; }
		NSNumber GradualDecoderRefresh { get; set; }

		[MacCatalyst (13, 1)]
		[StrongDictionary]
		CMHevcTemporalLevelInfoSettings HevcTemporalLevelInfo { get; set; }

		[MacCatalyst (13, 1)]
		bool HevcTemporalSubLayerAccess { get; set; }

		[MacCatalyst (13, 1)]
		bool HevcStepwiseTemporalSubLayerAccess { get; set; }

		[MacCatalyst (13, 1)]
		int HevcSyncSampleNalUnitType { get; set; }

		[MacCatalyst (13, 1)]
		NSData CameraIntrinsicMatrix { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		nint AudioIndependentSampleDecoderRefreshCount { get; set; }

		[MacCatalyst (13, 1)]
		bool ForceKeyFrame { get; set; }

		[TV (16, 0), Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		NSData Hdr10PlusPerFrameData { get; set; } // it is a CFData, but that is a toll-free bridged

		[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
		NSDictionary PostDecodeProcessingMetadata { get; set; } // it is a CFDictionary, but that is a toll-free bridged
	}

	[Internal]
	[MacCatalyst (13, 1)]
	[Static]
	interface CMHevcTemporalLevelInfoKeys {

		[Field ("kCMHEVCTemporalLevelInfoKey_TemporalLevel")]
		NSString TemporalLevelKey { get; }

		[Field ("kCMHEVCTemporalLevelInfoKey_ProfileSpace")]
		NSString ProfileSpaceKey { get; }

		[Field ("kCMHEVCTemporalLevelInfoKey_TierFlag")]
		NSString TierFlagKey { get; }

		[Field ("kCMHEVCTemporalLevelInfoKey_ProfileIndex")]
		NSString ProfileIndexKey { get; }

		[Field ("kCMHEVCTemporalLevelInfoKey_ProfileCompatibilityFlags")]
		NSString ProfileCompatibilityFlagsKey { get; }

		[Field ("kCMHEVCTemporalLevelInfoKey_ConstraintIndicatorFlags")]
		NSString ConstraintIndicatorFlagsKey { get; }

		[Field ("kCMHEVCTemporalLevelInfoKey_LevelIndex")]
		NSString LevelIndexKey { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("CMHevcTemporalLevelInfoKeys")]
	interface CMHevcTemporalLevelInfoSettings {

		int TemporalLevel { get; set; }
		int ProfileSpace { get; set; }
		int TierFlag { get; set; }
		int ProfileIndex { get; set; }
		NSData ProfileCompatibilityFlags { get; set; }
		NSData ConstraintIndicatorFlags { get; set; }
		int LevelIndex { get; set; }
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

	[Flags]
	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public enum CMStereoViewComponents : ulong {
		None = 0x0,
		LeftEye = 1uL << 0,
		RightEye = 1uL << 1,
	}

	[Flags]
	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public enum CMStereoViewInterpretationOptions : ulong {
		Default = 0,
		StereoOrderReversed = 1uL << 0,
		AdditionalViews = 1uL << 1,
	}

	[MacCatalyst (17, 0), TV (17, 0), Mac (14, 0), iOS (17, 0)]
	public enum CMTagCollectionError {
		Success = 0,
		ParamErr = -15740,
		AllocationFailed = -15741,
		InternalError = -15742,
		InvalidTag = -15743,
		InvalidTagCollectionDictionary = -15744,
		InvalidTagCollectionData = -15745,
		TagNotFound = -15746,
		InvalidTagCollectionDataVersion = -15747,
		ExhaustedBufferSize = -15748,
		NotYetImplemented = -15749,
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	public enum CMTaggedBufferGroupError {
		Success = 0,
		ParamErr = -15780,
		AllocationFailed = -15781,
		InternalError = -15782,
	}

	[MacCatalyst (17, 0), TV (17, 0), Mac (14, 0), iOS (17, 0)]
	public enum CMTagError {
		Success = 0,
		ParamErr = -15730,
		AllocationFailed = -15731,
	}

	[MacCatalyst (17, 0), TV (17, 0), Mac (14, 0), iOS (17, 0)]
	public enum CMTagCategory : uint {
		Undefined = 0,
		MediaType = ('m' << 24) + ('d' << 16) + ('i' << 8) + 'a', // 'mdia'
		MediaSubType = ('m' << 24) + ('s' << 16) + ('u' << 8) + 'b', // 'msub'
		TrackId = ('t' << 24) + ('r' << 16) + ('a' << 8) + 'k', // 'trak'
		ChannelId = ('v' << 24) + ('c' << 16) + ('h' << 8) + 'n', // 'vchn'
		VideoLayerId = ('v' << 24) + ('l' << 16) + ('a' << 8) + 'y', // 'vlay'
		PixelFormat = ('p' << 24) + ('i' << 16) + ('x' << 8) + 'f', // 'pixf'
		PackingType = ('p' << 24) + ('a' << 16) + ('c' << 8) + 'k', // 'pack'
		ProjectionType = ('p' << 24) + ('r' << 16) + ('o' << 8) + 'j', // 'proj'
		StereoView = ('e' << 24) + ('y' << 16) + ('e' << 8) + 's', // 'eyes'
		StereoViewInterpretation = ('e' << 24) + ('y' << 16) + ('i' << 8) + 'p', // 'eyip'
	}

	[MacCatalyst (17, 0), TV (17, 0), Mac (14, 0), iOS (17, 0)]
	public enum CMTagDataType : uint {
		Invalid = 0,
		SInt64 = 2,
		Float64 = 3,
		OSType = 5,
		Flags = 7,
	}

	[Internal]
	[Partial]
	interface CMTagCollectionConstants {
		[MacCatalyst (17, 0), TV (17, 0), Mac (14, 0), iOS (17, 0)]
		[Field ("kCMTagCollectionTagsArrayKey")]
		NSString ArrayKey { get; }
	}

	[Internal]
	[Partial]
	[MacCatalyst (17, 0), TV (17, 0), Mac (14, 0), iOS (17, 0)]
	interface CMTagConstants {
		[Field ("kCMTagInvalid")]
		CMTag Invalid { get; }

		[Field ("kCMTagMediaTypeVideo")]
		CMTag MediaTypeVideo { get; }

		[Field ("kCMTagMediaSubTypeMebx")]
		CMTag MediaSubTypeMebx { get; }

		[Field ("kCMTagMediaTypeAudio")]
		CMTag MediaTypeAudio { get; }

		[Field ("kCMTagMediaTypeMetadata")]
		CMTag MediaTypeMetadata { get; }

		[Field ("kCMTagStereoLeftEye")]
		CMTag StereoLeftEye { get; }

		[Field ("kCMTagStereoRightEye")]
		CMTag StereoRightEye { get; }

		[Field ("kCMTagStereoLeftAndRightEye")]
		CMTag StereoLeftAndRightEye { get; }

		[Field ("kCMTagStereoNone")]
		CMTag StereoNone { get; }

		[Field ("kCMTagStereoInterpretationOrderReversed")]
		CMTag StereoInterpretationOrderReversed { get; }

		[Field ("kCMTagProjectionTypeRectangular")]
		CMTag ProjectionTypeRectangular { get; }

		[Field ("kCMTagProjectionTypeEquirectangular")]
		CMTag ProjectionTypeEquirectangular { get; }

		[iOS (18, 0), TV (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("kCMTagProjectionTypeHalfEquirectangular")]
		CMTag ProjectionTypeHalfEquirectangular { get; }

		[Field ("kCMTagProjectionTypeFisheye")]
		CMTag ProjectionTypeFisheye { get; }

		[Field ("kCMTagPackingTypeNone")]
		CMTag PackingTypeNone { get; }

		[Field ("kCMTagPackingTypeSideBySide")]
		CMTag PackingTypeSideBySide { get; }

		[Field ("kCMTagPackingTypeOverUnder")]
		CMTag PackingTypeOverUnder { get; }

		[Field ("kCMTagValueKey")]
		NSString ValueKey { get; }

		[Field ("kCMTagCategoryKey")]
		NSString CategoryKey { get; }

		[Field ("kCMTagDataTypeKey")]
		NSString DataTypeKey { get; }
	}

	[MacCatalyst (17, 0), TV (17, 0), Mac (14, 0), iOS (17, 0)]
	public enum CMProjectionType : ulong {
		Rectangular = ('r' << 24) + ('e' << 16) + ('c' << 8) + 't', // 'rect',
		Equirectangular = ('e' << 24) + ('q' << 16) + ('u' << 8) + 'i', // 'equi',
		HalfEquirectangular = ('h' << 24) + ('e' << 16) + ('q' << 8) + 'u', // 'hequ',
		Fisheye = ('f' << 24) + ('i' << 16) + ('s' << 8) + 'h', // 'fish',
	}

	[MacCatalyst (17, 0), TV (17, 0), Mac (14, 0), iOS (17, 0)]
	public enum CMPackingType : ulong {
		None = ('n' << 24) + ('o' << 16) + ('n' << 8) + 'e', // 'none',
		SideBySide = ('s' << 24) + ('i' << 16) + ('d' << 8) + 'e', // 'side',
		OverUnder = ('o' << 24) + ('v' << 16) + ('e' << 8) + 'r', // 'over',
	}

	[MacCatalyst (17, 0), TV (17, 0), Mac (14, 0), iOS (17, 0)]
	public enum CMTaggedBufferGroupFormatType {
		TaggedBufferGroup = ('t' << 24) + ('b' << 16) + ('g' << 8) + 'r', // 'tbgr',
	}
}
