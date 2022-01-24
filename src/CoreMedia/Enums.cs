using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace CoreMedia {
	// keys names got changed at some point, but they all refer to a CMSampleBuffer (there is not CMSample obj)
#if !NET
	[Watch (6,0)]
#endif
	enum CMSampleBufferAttachmentKey { 
		[Field ("kCMSampleAttachmentKey_NotSync")]
		NotSync,
		[Field ("kCMSampleAttachmentKey_PartialSync")]
		PartialSync,
		[Field ("kCMSampleAttachmentKey_HasRedundantCoding")]
		HasRedundantCoding,
		[Field ("kCMSampleAttachmentKey_IsDependedOnByOthers")]
		IsDependedOnByOthers,
		[Field ("kCMSampleAttachmentKey_DependsOnOthers")]
		DependsOnOthers,
		[Field ("kCMSampleAttachmentKey_EarlierDisplayTimesAllowed")]
		EarlierDisplayTimesAllowed,
		[Field ("kCMSampleAttachmentKey_DisplayImmediately")]
		DisplayImmediately,
		[Field ("kCMSampleAttachmentKey_DoNotDisplay")]
		DoNotDisplay,
#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("tvos11.0")]
#else
		[iOS (11,0)]
		[Mac (10,13)]
		[TV (11,0)]
#endif
		[Field ("kCMSampleAttachmentKey_HEVCTemporalLevelInfo")]
		HevcTemporalLevelInfo,
#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("tvos11.0")]
#else
		[iOS (11,0)]
		[Mac (10,13)]
		[TV (11,0)]
#endif
		[Field ("kCMSampleAttachmentKey_HEVCTemporalSubLayerAccess")]
		HevcTemporalSubLayerAccess,
#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("tvos11.0")]
#else
		[iOS (11,0)]
		[Mac (10,13)]
		[TV (11,0)]
#endif
		[Field ("kCMSampleAttachmentKey_HEVCStepwiseTemporalSubLayerAccess")]
		HevcStepwiseTemporalSubLayerAccess,
#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("tvos11.0")]
#else
		[iOS (11,0)]
		[Mac (10,13)]
		[TV (11,0)]
#endif
		[Field ("kCMSampleAttachmentKey_HEVCSyncSampleNALUnitType")]
		HevcSyncSampleNalUnitType,
		[Field ("kCMSampleBufferAttachmentKey_ResetDecoderBeforeDecoding")]
		ResetDecoderBeforeDecoding,
		[Field ("kCMSampleBufferAttachmentKey_DrainAfterDecoding")]
		DrainAfterDecoding,
		[Field ("kCMSampleBufferAttachmentKey_PostNotificationWhenConsumed")]
		PostNotificationWhenConsumed,
		[Field ("kCMSampleBufferAttachmentKey_ResumeOutput")]
		ResumeOutput,
		[Field ("kCMSampleBufferAttachmentKey_TransitionID")]
		TransitionId,
		[Field ("kCMSampleBufferAttachmentKey_TrimDurationAtStart")]
		TrimDurationAtStart,
		[Field ("kCMSampleBufferAttachmentKey_TrimDurationAtEnd")]
		TrimDurationAtEnd,
		[Field ("kCMSampleBufferAttachmentKey_SpeedMultiplier")]
		SpeedMultiplier,
		[Field ("kCMSampleBufferAttachmentKey_Reverse")]
		Reverse,
		[Field ("kCMSampleBufferAttachmentKey_FillDiscontinuitiesWithSilence")]
		FillDiscontinuitiesWithSilence,
		[Field ("kCMSampleBufferAttachmentKey_EmptyMedia")]
		EmptyMedia,
		[Field ("kCMSampleBufferAttachmentKey_PermanentEmptyMedia")]
		PermanentEmptyMedia,
		[Field ("kCMSampleBufferAttachmentKey_DisplayEmptyMediaImmediately")]
		DisplayEmptyMediaImmediately,
		[Field ("kCMSampleBufferAttachmentKey_EndsPreviousSampleDuration")]
		EndsPreviousSampleDuration,
		[Field ("kCMSampleBufferAttachmentKey_SampleReferenceURL")]
		SampleReferenceUrl,
		[Field ("kCMSampleBufferAttachmentKey_SampleReferenceByteOffset")]
		SampleReferenceByteOffset, 
		[Field ("kCMSampleBufferAttachmentKey_GradualDecoderRefresh")]
		GradualDecoderRefresh,
#if NET
		[SupportedOSPlatform ("macos10.14")]
#else
		[Mac (10,14)]
#endif
		[Field ("kCMSampleBufferAttachmentKey_DroppedFrameReason")]
		DroppedFrameReason,
#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.14")]
#else
		[iOS (9,0)]
		[Mac (10,14)]
#endif
		[Field ("kCMSampleBufferAttachmentKey_StillImageLensStabilizationInfo")]
		StillImageLensStabilizationInfo,
#if NET
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("macos10.13")]
		[SupportedOSPlatform ("tvos11.0")]
#else
		[iOS (11,0)]
		[Mac (10,13)]
		[TV (11,0)]
#endif
		[Field ("kCMSampleBufferAttachmentKey_CameraIntrinsicMatrix")]
		CameraIntrinsicMatrix,
#if NET
		[SupportedOSPlatform ("macos10.14")]
#else
		[Mac (10,14)]
#endif
		[Field ("kCMSampleBufferAttachmentKey_DroppedFrameReasonInfo")]
		DroppedFrameReasonInfo,
#if NET
		[SupportedOSPlatform ("macos10.10")]
#else
		[Mac (10,10)]
#endif
		[Field ("kCMSampleBufferAttachmentKey_ForceKeyFrame")]
		ForceKeyFrame,
	}
}
