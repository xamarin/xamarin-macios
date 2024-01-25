using System;
using System.Runtime.InteropServices;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace CoreMedia {
	// keys names got changed at some point, but they all refer to a CMSampleBuffer (there is not CMSample obj)
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
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
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleAttachmentKey_HEVCTemporalLevelInfo")]
		HevcTemporalLevelInfo,
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleAttachmentKey_HEVCTemporalSubLayerAccess")]
		HevcTemporalSubLayerAccess,
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleAttachmentKey_HEVCStepwiseTemporalSubLayerAccess")]
		HevcStepwiseTemporalSubLayerAccess,
		[MacCatalyst (13, 1)]
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
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferAttachmentKey_DroppedFrameReason")]
		DroppedFrameReason,
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferAttachmentKey_StillImageLensStabilizationInfo")]
		StillImageLensStabilizationInfo,
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferAttachmentKey_CameraIntrinsicMatrix")]
		CameraIntrinsicMatrix,
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferAttachmentKey_DroppedFrameReasonInfo")]
		DroppedFrameReasonInfo,
		[MacCatalyst (13, 1)]
		[Field ("kCMSampleBufferAttachmentKey_ForceKeyFrame")]
		ForceKeyFrame,
		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Field ("kCMSampleAttachmentKey_HDR10PlusPerFrameData")]
		Hdr10PlusPerFrameData,
	}
}
