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
}
