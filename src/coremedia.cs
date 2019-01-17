//
// coremedia.cs: Definitions for CoreMedia
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;

namespace CoreMedia {

	[Partial]
	interface CMMemoryPool {

		[Internal][Field ("kCMMemoryPoolOption_AgeOutPeriod")]
		IntPtr AgeOutPeriodSelector { get; }
	}

	[Static][Internal]
	[Mac (10, 9), iOS (6,0)]
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

        [Internal]
        [Field("kCMTextMarkupAttribute_BaseFontSizePercentageRelativeToVideoHeight")]
        NSString BaseFontSizePercentageRelativeToVideoHeight { get; }
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

		[iOS (6,0)][NoMac]
		[Field ("kCMSampleBufferAttachmentKey_DroppedFrameReason")]
		NSString DroppedFrameReason { get; }

		[iOS (9,0)][NoMac]
		[Field ("kCMSampleBufferAttachmentKey_StillImageLensStabilizationInfo")]
		NSString StillImageLensStabilizationInfo { get; }

		[iOS (9,0)][NoMac]
		[Field ("kCMSampleBufferLensStabilizationInfo_Active")]
		NSString BufferLensStabilizationInfo_Active { get; }

		[iOS (9,0)][NoMac]
		[Field ("kCMSampleBufferLensStabilizationInfo_OutOfRange")]
		NSString BufferLensStabilizationInfo_OutOfRange { get; }

		[iOS (9,0)][NoMac]
		[Field ("kCMSampleBufferLensStabilizationInfo_Unavailable")]
		NSString BufferLensStabilizationInfo_Unavailable { get; }

		[iOS (9,0)][NoMac]
		[Field ("kCMSampleBufferLensStabilizationInfo_Off")]
		NSString BufferLensStabilizationInfo_Off { get; }

		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("kCMSampleAttachmentKey_HEVCTemporalLevelInfo")]
		NSString HevcTemporalLevelInfoKey { get; }

		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("kCMSampleAttachmentKey_HEVCTemporalSubLayerAccess")]
		NSString HevcTemporalSubLayerAccessKey { get; }

		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("kCMSampleAttachmentKey_HEVCStepwiseTemporalSubLayerAccess")]
		NSString HevcStepwiseTemporalSubLayerAccessKey { get; }

		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("kCMSampleAttachmentKey_HEVCSyncSampleNALUnitType")]
		NSString HevcSyncSampleNalUnitTypeKey { get; }

		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("kCMSampleBufferAttachmentKey_CameraIntrinsicMatrix")]
		NSString CameraIntrinsicMatrixKey { get; }
	}

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

		[iOS (11,0), Mac (10,13), TV (11,0)]
		[StrongDictionary]
		CMHevcTemporalLevelInfoSettings HevcTemporalLevelInfo { get; set; }

		[iOS (11,0), Mac (10,13), TV (11,0)]
		bool HevcTemporalSubLayerAccess { get; set; }

		[iOS (11,0), Mac (10,13), TV (11,0)]
		bool HevcStepwiseTemporalSubLayerAccess { get; set; }

		[iOS (11,0), Mac (10,13), TV (11,0)]
		int HevcSyncSampleNalUnitType { get; set; }

		[iOS (11,0), Mac (10,13), TV (11,0)]
		NSData CameraIntrinsicMatrix { get; set; }
	}

	[Internal]
	[iOS (11,0), Mac (10,13), TV (11,0)]
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

	[iOS (11,0), Mac (10,13), TV (11,0)]
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
}
