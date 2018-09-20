using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace CoreMedia {
	// keys names got changed at some point, but they all refer to a CMSampleBuffer (there is not CMSample obj)
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
		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("kCMSampleAttachmentKey_HEVCTemporalLevelInfo")]
		HevcTemporalLevelInfo,
		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("kCMSampleAttachmentKey_HEVCTemporalSubLayerAccess")]
		HevcTemporalSubLayerAccess,
		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("kCMSampleAttachmentKey_HEVCStepwiseTemporalSubLayerAccess")]
		HevcStepwiseTemporalSubLayerAccess,
		[iOS (11,0), Mac (10,13), TV (11,0)]
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
		[iOS (6,0)][NoMac]
		[Field ("kCMSampleBufferAttachmentKey_DroppedFrameReason")]
		DroppedFrameReason,
		[iOS (9,0)][NoMac]
		[Field ("kCMSampleBufferAttachmentKey_StillImageLensStabilizationInfo")]
		StillImageLensStabilizationInfo,
		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("kCMSampleBufferAttachmentKey_CameraIntrinsicMatrix")]
		CameraIntrinsicMatrix,
	}
}
