////
// VideoToolbox core types and enumerations
//
// Author: Miguel de Icaza (miguel@xamarin.com)
//         Alex Soto (alex.soto@xamarin.com)
//
// Copyright 2014 Xamarin Inc
//
using System;
using Foundation;
using ObjCRuntime;
using CoreMedia;
using AVFoundation;

namespace VideoToolbox {

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[Static]
	interface VTCompressionPropertyKey {
		// Buffers

		[Field ("kVTCompressionPropertyKey_NumberOfPendingFrames")]
		NSString NumberOfPendingFrames { get; }

		[Field ("kVTCompressionPropertyKey_PixelBufferPoolIsShared")]
		NSString PixelBufferPoolIsShared { get; }

		[Field ("kVTCompressionPropertyKey_VideoEncoderPixelBufferAttributes")]
		NSString VideoEncoderPixelBufferAttributes { get; }

		// Frame dependency

		[Field ("kVTCompressionPropertyKey_MaxKeyFrameInterval")]
		NSString MaxKeyFrameInterval { get; }

		[Field ("kVTCompressionPropertyKey_MaxKeyFrameIntervalDuration")]
		NSString MaxKeyFrameIntervalDuration { get; }

		[Field ("kVTCompressionPropertyKey_AllowTemporalCompression")]
		NSString AllowTemporalCompression { get; }

		[Field ("kVTCompressionPropertyKey_AllowFrameReordering")]
		NSString AllowFrameReordering { get; }

		// Rate control

		[Field ("kVTCompressionPropertyKey_AverageBitRate")]
		NSString AverageBitRate { get; }

		[Field ("kVTCompressionPropertyKey_DataRateLimits")]
		NSString DataRateLimits { get; } // NSArray of an even number of CFNumbers alternating [int, double](bytes, seconds] Read/write

		[Field ("kVTCompressionPropertyKey_Quality")]
		NSString Quality { get; }

		[Field ("kVTCompressionPropertyKey_MoreFramesBeforeStart")]
		NSString MoreFramesBeforeStart { get; }

		[Field ("kVTCompressionPropertyKey_MoreFramesAfterEnd")]
		NSString MoreFramesAfterEnd { get; }

		// Bitstream configuration

		[Field ("kVTCompressionPropertyKey_ProfileLevel")]
		NSString ProfileLevel { get; }

		[Field ("kVTCompressionPropertyKey_H264EntropyMode")]
		[Mac (10,9)]
		NSString H264EntropyMode { get; }

		[Field ("kVTCompressionPropertyKey_Depth")]
		NSString Depth { get; }

		// Runtime restrictions

		[Field ("kVTCompressionPropertyKey_MaxFrameDelayCount")]
		NSString MaxFrameDelayCount { get; }

		[Field ("kVTCompressionPropertyKey_MaxH264SliceBytes")]
		NSString MaxH264SliceBytes { get; }

		[Field ("kVTCompressionPropertyKey_RealTime")]
		[Mac (10,9)]
		NSString RealTime { get; }

		// Hints

		[Field ("kVTCompressionPropertyKey_SourceFrameCount")]
		NSString SourceFrameCount { get; }

		[Field ("kVTCompressionPropertyKey_ExpectedFrameRate")]
		NSString ExpectedFrameRate { get; }

		[Field ("kVTCompressionPropertyKey_ExpectedDuration")]
		NSString ExpectedDuration { get; }

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[Field ("kVTCompressionPropertyKey_BaseLayerFrameRate")]
		NSString BaseLayerFrameRate { get; }

		// Hardware acceleration
		// Hardware acceleration is default behavior on iOS. No opt-in required.

		[Field ("kVTCompressionPropertyKey_UsingHardwareAcceleratedVideoEncoder")]
		NSString UsingHardwareAcceleratedVideoEncoder { get; } // CFBoolean Read

		// Clean aperture and pixel aspect ratio

		[Field ("kVTCompressionPropertyKey_CleanAperture")]
		NSString CleanAperture { get; }

		[Field ("kVTCompressionPropertyKey_PixelAspectRatio")]
		NSString PixelAspectRatio { get; }

		[Field ("kVTCompressionPropertyKey_FieldCount")]
		NSString FieldCount { get; } 

		[Field ("kVTCompressionPropertyKey_FieldDetail")]
		NSString FieldDetail { get; } 

		[Field ("kVTCompressionPropertyKey_AspectRatio16x9")]
		NSString AspectRatio16x9 { get; } 

		[Field ("kVTCompressionPropertyKey_ProgressiveScan")]
		NSString ProgressiveScan { get; } 

		// Color

		[Field ("kVTCompressionPropertyKey_ColorPrimaries")]
		NSString ColorPrimaries { get; } 

		[Field ("kVTCompressionPropertyKey_TransferFunction")]
		NSString TransferFunction { get; } 

		[Field ("kVTCompressionPropertyKey_YCbCrMatrix")]
		NSString YCbCrMatrix { get; } 

		[Field ("kVTCompressionPropertyKey_ICCProfile")]
		NSString ICCProfile { get; } 

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[Field ("kVTCompressionPropertyKey_MasteringDisplayColorVolume")]
		NSString MasteringDisplayColorVolume { get; }

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[Field ("kVTCompressionPropertyKey_ContentLightLevelInfo")]
		NSString ContentLightLevelInfo { get; }

		// Pre-compression processing

		[Field ("kVTCompressionPropertyKey_PixelTransferProperties")]
		NSString PixelTransferProperties { get; }

		// Multi-pass

		[Field ("kVTCompressionPropertyKey_MultiPassStorage")]
		[Mac (10,10)]
		NSString MultiPassStorage { get; }

		// Encoder information

		[Field ("kVTCompressionPropertyKey_EncoderID")]
		[Mac (10,13), iOS (11,0), TV (11,0)]
		NSString EncoderId { get; }
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[StrongDictionary ("VTCompressionPropertyKey")]
	interface VTCompressionProperties {

		[Export ("NumberOfPendingFrames")]
		int NumberOfPendingFrames { get; }

		[Export ("PixelBufferPoolIsShared")]
		bool PixelBufferPoolIsShared { get; }

		[Export ("VideoEncoderPixelBufferAttributes")]
		NSDictionary VideoEncoderPixelBufferAttributes { get; }

		[Export ("MaxKeyFrameInterval")]
		int MaxKeyFrameInterval { get; set; }

		[Export ("MaxKeyFrameIntervalDuration")]
		double MaxKeyFrameIntervalDuration { get; set; }

		[Export ("AllowTemporalCompression")]
		bool AllowTemporalCompression { get; set; }

		[Export ("AllowFrameReordering")]
		bool AllowFrameReordering { get; set; }

		[Export ("AverageBitRate")]
		int AverageBitRate { get; set; }

		[Export ("Quality")]
		float Quality { get; set; }

		[Export ("MoreFramesBeforeStart")]
		bool MoreFramesBeforeStart { get; set; }

		[Export ("MoreFramesAfterEnd")]
		bool MoreFramesAfterEnd { get; set; }

		[Export ("Depth")]
		CMPixelFormat Depth { get; set; }

		[Export ("MaxFrameDelayCount")]
		int MaxFrameDelayCount { get; set; }

		[Export ("MaxH264SliceBytes")]
		int MaxH264SliceBytes { get; set; }

		[Mac (10,9)]
		[Export ("RealTime")]
		bool RealTime { get; set; }

		[Export ("SourceFrameCount")]
		uint SourceFrameCount { get; set; }

		[Export ("ExpectedFrameRate")]
		double ExpectedFrameRate { get; set; }

		[Export ("ExpectedDuration")]
		double ExpectedDuration { get; set; }

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[Export ("BaseLayerFrameRate")]
		double BaseLayerFrameRate { get; set; }

		[Export ("UsingHardwareAcceleratedVideoEncoder")]
		bool UsingHardwareAcceleratedVideoEncoder { get; }

		[Export ("CleanAperture")]
		NSDictionary CleanAperture { get; set; }

		[Export ("PixelAspectRatio")]
		NSDictionary PixelAspectRatio { get; set; }

		[Export ("FieldCount")]
		VTFieldCount FieldCount { get; set; }

		[Export ("AspectRatio16x9")]
		bool AspectRatio16x9 { get; set; }

		[Export ("ProgressiveScan")]
		bool ProgressiveScan { get; set; }

		[Export ("ICCProfile")]
		NSData ICCProfile { get; set; }

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[Export ("MasteringDisplayColorVolume")]
		NSData MasteringDisplayColorVolume { get; set; }

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[Export ("ContentLightLevelInfo")]
		NSData ContentLightLevelInfo { get; set; }

		[Export ("PixelTransferProperties")]
		NSDictionary PixelTransferProperties { get; set; }

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[Export ("EncoderId")]
		string EncoderId { get; set; }
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[Static]
	interface VTProfileLevelKeys {
		// HEVC

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[Field ("kVTProfileLevel_HEVC_Main_AutoLevel")]
		NSString Hevc_Main_AutoLevel { get; }

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[Field ("kVTProfileLevel_HEVC_Main10_AutoLevel")]
		NSString Hevc_Main10_AutoLevel { get; }

		// H264

		[Field ("kVTProfileLevel_H264_Baseline_1_3")]
		NSString H264_Baseline_1_3 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_3_0")]
		NSString H264_Baseline_3_0 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_3_1")]
		NSString H264_Baseline_3_1 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_3_2")]
		NSString H264_Baseline_3_2 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_4_0")]
		[Mac (10,9)]
		NSString H264_Baseline_4_0 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_4_1")]
		NSString H264_Baseline_4_1 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_4_2")]
		[Mac (10,9)]
		NSString H264_Baseline_4_2 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_5_0")]
		[Mac (10,9)]
		NSString H264_Baseline_5_0 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_5_1")]
		[Mac (10,9)]
		NSString H264_Baseline_5_1 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_5_2")]
		[Mac (10,9)]
		NSString H264_Baseline_5_2 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_AutoLevel")]
		[Mac (10,9)]
		NSString H264_Baseline_AutoLevel { get; }

		[Field ("kVTProfileLevel_H264_Main_3_0")]
		NSString H264_Main_3_0 { get; }

		[Field ("kVTProfileLevel_H264_Main_3_1")]
		NSString H264_Main_3_1 { get; }

		[Field ("kVTProfileLevel_H264_Main_3_2")]
		NSString H264_Main_3_2 { get; }

		[Field ("kVTProfileLevel_H264_Main_4_0")]
		NSString H264_Main_4_0 { get; }

		[Field ("kVTProfileLevel_H264_Main_4_1")]
		NSString H264_Main_4_1 { get; }

		[Field ("kVTProfileLevel_H264_Main_4_2")]
		[Mac (10,9)]
		NSString H264_Main_4_2 { get; }

		[Field ("kVTProfileLevel_H264_Main_5_0")]
		NSString H264_Main_5_0 { get; }

		[Field ("kVTProfileLevel_H264_Main_5_1")]
		[Mac (10,9)]
		NSString H264_Main_5_1 { get; }

		[Field ("kVTProfileLevel_H264_Main_5_2")]
		[Mac (10,9)]
		NSString H264_Main_5_2 { get; }

		[Field ("kVTProfileLevel_H264_Main_AutoLevel")]
		[Mac (10,9)]
		NSString H264_Main_AutoLevel { get; }

		[Field ("kVTProfileLevel_H264_Extended_5_0")]
		NSString H264_Extended_5_0 { get; }

		[Field ("kVTProfileLevel_H264_Extended_AutoLevel")]
		[Mac (10,9)]
		NSString H264_Extended_AutoLevel { get; }

		[Field ("kVTProfileLevel_H264_High_3_0")]
		[Mac (10,9)]
		NSString H264_High_3_0 { get; }

		[Field ("kVTProfileLevel_H264_High_3_1")]
		[Mac (10,9)]
		NSString H264_High_3_1 { get; }

		[Field ("kVTProfileLevel_H264_High_3_2")]
		[Mac (10,9)]
		NSString H264_High_3_2 { get; }

		[Field ("kVTProfileLevel_H264_High_4_0")]
		[Mac (10,9)]
		NSString H264_High_4_0 { get; }

		[Field ("kVTProfileLevel_H264_High_4_1")]
		[Mac (10,9)]
		NSString H264_High_4_1 { get; }

		[Field ("kVTProfileLevel_H264_High_4_2")]
		[Mac (10,9)]
		NSString H264_High_4_2 { get; }

		[Field ("kVTProfileLevel_H264_High_5_0")]
		NSString H264_High_5_0 { get; }

		[Field ("kVTProfileLevel_H264_High_5_1")]
		[Mac (10,9)]
		NSString H264_High_5_1 { get; }

		[Field ("kVTProfileLevel_H264_High_5_2")]
		[Mac (10,9)]
		NSString H264_High_5_2 { get; }

		[Field ("kVTProfileLevel_H264_High_AutoLevel")]
		[Mac (10,9)]
		NSString H264_High_AutoLevel { get; }

		// MP4V

		[Field ("kVTProfileLevel_MP4V_Simple_L0")]
		NSString MP4V_Simple_L0 { get; }

		[Field ("kVTProfileLevel_MP4V_Simple_L1")]
		NSString MP4V_Simple_L1 { get; }

		[Field ("kVTProfileLevel_MP4V_Simple_L2")]
		NSString MP4V_Simple_L2 { get; }

		[Field ("kVTProfileLevel_MP4V_Simple_L3")]
		NSString MP4V_Simple_L3 { get; }

		[Field ("kVTProfileLevel_MP4V_Main_L2")]
		NSString MP4V_Main_L2 { get; }

		[Field ("kVTProfileLevel_MP4V_Main_L3")]
		NSString MP4V_Main_L3 { get; }

		[Field ("kVTProfileLevel_MP4V_Main_L4")]
		NSString MP4V_Main_L4 { get; }

		[Field ("kVTProfileLevel_MP4V_AdvancedSimple_L0")]
		NSString MP4V_AdvancedSimple_L0 { get; }

		[Field ("kVTProfileLevel_MP4V_AdvancedSimple_L1")]
		NSString MP4V_AdvancedSimple_L1 { get; }

		[Field ("kVTProfileLevel_MP4V_AdvancedSimple_L2")]
		NSString MP4V_AdvancedSimple_L2 { get; }

		[Field ("kVTProfileLevel_MP4V_AdvancedSimple_L3")]
		NSString MP4V_AdvancedSimple_L3 { get; }

		[Field ("kVTProfileLevel_MP4V_AdvancedSimple_L4")]
		NSString MP4V_AdvancedSimple_L4 { get; }

		// H263

		[Field ("kVTProfileLevel_H263_Profile0_Level10")]
		NSString H263_Profile0_Level10 { get; }

		[Field ("kVTProfileLevel_H263_Profile0_Level45")]
		NSString H263_Profile0_Level45 { get; }

		[Field ("kVTProfileLevel_H263_Profile3_Level45")]
		NSString H263_Profile3_Level45 { get; } 
	}

	[Static]
	[Mac (10,9), iOS (8,0), TV (10,2)]
	interface VTH264EntropyModeKeys {
		[Field ("kVTH264EntropyMode_CAVLC")]
		NSString CAVLC { get; } 

		[Field ("kVTH264EntropyMode_CABAC")]
		NSString CABAC { get; } 
	}
		
	[Mac (10,8), iOS (8,0), TV (10,2)]
	[StrongDictionary ("VTVideoEncoderSpecificationKeys")]
	interface VTVideoEncoderSpecification {

		[Mac (10,9), NoiOS, NoTV]
		[Export ("EnableHardwareAcceleratedVideoEncoder")]
		bool EnableHardwareAcceleratedVideoEncoder { get; set; }

		[Mac (10,9), NoiOS, NoTV]
		[Export ("RequireHardwareAcceleratedVideoEncoder")]
		bool RequireHardwareAcceleratedVideoEncoder { get; set; }

		[Export ("EncoderID")]
		string EncoderID { get; set; }
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[Static]
	interface VTVideoEncoderSpecificationKeys {

		[Field ("kVTVideoEncoderSpecification_EnableHardwareAcceleratedVideoEncoder")]
		[Mac (10,9), NoiOS, NoTV]
		NSString EnableHardwareAcceleratedVideoEncoder { get; }

		[Field ("kVTVideoEncoderSpecification_RequireHardwareAcceleratedVideoEncoder")]
		[Mac (10,9), NoiOS, NoTV]
		NSString RequireHardwareAcceleratedVideoEncoder { get; }

		[Field ("kVTVideoEncoderSpecification_EncoderID")]
		NSString EncoderID { get; }
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[StrongDictionary ("VTEncodeFrameOptionKey")]
	interface VTEncodeFrameOptions {

		[Export ("ForceKeyFrame")]
		bool ForceKeyFrame { get; set; }
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[Static]
	interface VTEncodeFrameOptionKey {
		// Per-frame configuration

		[Field ("kVTEncodeFrameOptionKey_ForceKeyFrame")]
		NSString ForceKeyFrame { get; } 
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[Static]
	interface VTDecompressionPropertyKey {
		// Pixel buffer pools

		[Field ("kVTDecompressionPropertyKey_PixelBufferPool")]
		NSString PixelBufferPool { get; }

		[Field ("kVTDecompressionPropertyKey_PixelBufferPoolIsShared")]
		NSString PixelBufferPoolIsShared { get; }

		[Field ("kVTDecompressionPropertyKey_OutputPoolRequestedMinimumBufferCount")]
		[Mac (10,9)]
		NSString OutputPoolRequestedMinimumBufferCount { get; }

		// Asynchronous state

		[Field ("kVTDecompressionPropertyKey_NumberOfFramesBeingDecoded")]
		NSString NumberOfFramesBeingDecoded { get; }

		[Field ("kVTDecompressionPropertyKey_MinOutputPresentationTimeStampOfFramesBeingDecoded")]
		NSString MinOutputPresentationTimeStampOfFramesBeingDecoded { get; }

		[Field ("kVTDecompressionPropertyKey_MaxOutputPresentationTimeStampOfFramesBeingDecoded")]
		NSString MaxOutputPresentationTimeStampOfFramesBeingDecoded { get; } 

		// Content

		[Field ("kVTDecompressionPropertyKey_ContentHasInterframeDependencies")]
		NSString ContentHasInterframeDependencies { get; }

		// Hardware acceleration
		// hardware acceleration is default behavior on iOS.  no opt-in required.
		 
		[Field ("kVTDecompressionPropertyKey_UsingHardwareAcceleratedVideoDecoder")]
		[Mac (10,9)]
		NSString UsingHardwareAcceleratedVideoDecoder { get; } 

		// Decoder behavior

		[Field ("kVTDecompressionPropertyKey_RealTime")]
		[Mac (10,10)]
		NSString RealTime { get; }

		[Field ("kVTDecompressionPropertyKey_ThreadCount")]
		NSString ThreadCount { get; }

		[Field ("kVTDecompressionPropertyKey_FieldMode")]
		NSString FieldMode { get; }

		[Field ("kVTDecompressionProperty_FieldMode_BothFields")]
		NSString FieldMode_BothFields { get; } 

		[Field ("kVTDecompressionProperty_FieldMode_TopFieldOnly")]
		NSString FieldMode_TopFieldOnly { get; }

		[Field ("kVTDecompressionProperty_FieldMode_BottomFieldOnly")]
		NSString FieldMode_BottomFieldOnly { get; }

		[Field ("kVTDecompressionProperty_FieldMode_SingleField")]
		NSString FieldMode_SingleField { get; }

		[Field ("kVTDecompressionProperty_FieldMode_DeinterlaceFields")]
		NSString FieldMode_DeinterlaceFields { get; }

		[Field ("kVTDecompressionPropertyKey_DeinterlaceMode")]
		NSString DeinterlaceMode { get; }

		[Field ("kVTDecompressionProperty_DeinterlaceMode_VerticalFilter")]
		NSString DeinterlaceMode_VerticalFilter { get; } 

		[Field ("kVTDecompressionProperty_DeinterlaceMode_Temporal")]
		NSString DeinterlaceMode_Temporal { get; } 

		[Field ("kVTDecompressionPropertyKey_ReducedResolutionDecode")]
		NSString ReducedResolutionDecode { get; }

		[Field ("kVTDecompressionPropertyKey_ReducedCoefficientDecode")]
		NSString ReducedCoefficientDecode { get; }

		[Field ("kVTDecompressionPropertyKey_ReducedFrameDelivery")]
		NSString ReducedFrameDelivery { get; }

		[Field ("kVTDecompressionPropertyKey_OnlyTheseFrames")]
		NSString OnlyTheseFrames { get; } 

		[Field ("kVTDecompressionProperty_OnlyTheseFrames_AllFrames")]
		NSString OnlyTheseFrames_AllFrames { get; } 

		[Field ("kVTDecompressionProperty_OnlyTheseFrames_NonDroppableFrames")]
		NSString OnlyTheseFrames_NonDroppableFrames { get; } 

		[Field ("kVTDecompressionProperty_OnlyTheseFrames_IFrames")]
		NSString OnlyTheseFrames_IFrames { get; } 

		[Field ("kVTDecompressionProperty_OnlyTheseFrames_KeyFrames")]
		NSString OnlyTheseFrames_KeyFrames { get; } 

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[Field ("kVTDecompressionProperty_TemporalLevelLimit")]
		NSString TemporalLevelLimit { get; }

		[Field ("kVTDecompressionPropertyKey_SuggestedQualityOfServiceTiers")]
		NSString SuggestedQualityOfServiceTiers { get; }

		[Field ("kVTDecompressionPropertyKey_SupportedPixelFormatsOrderedByQuality")]
		NSString SupportedPixelFormatsOrderedByQuality { get; }

		[Field ("kVTDecompressionPropertyKey_SupportedPixelFormatsOrderedByPerformance")]
		NSString SupportedPixelFormatsOrderedByPerformance { get; }

		[Field ("kVTDecompressionPropertyKey_PixelFormatsWithReducedResolutionSupport")]
		NSString PixelFormatsWithReducedResolutionSupport { get; }

		//Post-decompression processing

		[Field ("kVTDecompressionPropertyKey_PixelTransferProperties")]
		NSString PixelTransferProperties { get; }
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[StrongDictionary ("VTDecompressionPropertyKey")]
	interface VTDecompressionProperties	{
		
		[Export ("PixelBufferPoolIsShared")]
		bool PixelBufferPoolIsShared { get; }

		[Export ("OutputPoolRequestedMinimumBufferCount")]
		uint OutputPoolRequestedMinimumBufferCount { get; set; }

		[Export ("NumberOfFramesBeingDecoded")]
		uint NumberOfFramesBeingDecoded { get; }

		[Export ("MinOutputPresentationTimeStampOfFramesBeingDecoded")]
		NSDictionary MinOutputPresentationTimeStampOfFramesBeingDecoded { get; }

		[Export ("MaxOutputPresentationTimeStampOfFramesBeingDecoded")]
		NSDictionary MaxOutputPresentationTimeStampOfFramesBeingDecoded { get; }

		[Export ("ContentHasInterframeDependencies")]
		bool ContentHasInterframeDependencies { get; }

		// Hardware acceleration
		// hardware acceleration is default behavior on iOS.  no opt-in required.

		[Export ("UsingHardwareAcceleratedVideoDecoder")]
		[Mac (10,9)]
		bool UsingHardwareAcceleratedVideoDecoder { get; } 

		[Mac (10,10)]
		[Export ("RealTime")]
		bool RealTime { get; set; }

		[Export ("ThreadCount")]
		uint ThreadCount { get; set; }

		[StrongDictionary]
		[Export ("ReducedResolutionDecode")]
		VTDecompressionResolutionOptions ReducedResolutionDecode { get; set; }

		[Export ("ReducedCoefficientDecode")]
		uint ReducedCoefficientDecode { get; set; }

		[Export ("ReducedFrameDelivery")]
		float ReducedFrameDelivery { get; set; }

		[Mac (10,13), iOS (11,0), TV (11,0)]
		[Export ("TemporalLevelLimit")]
		int TemporalLevelLimit { get; set; }

		[Export ("SuggestedQualityOfServiceTiers")]
		NSDictionary[] SuggestedQualityOfServiceTiers { get; }

		[Export ("SupportedPixelFormatsOrderedByQuality")]
		CMPixelFormat[] SupportedPixelFormatsOrderedByQuality { get; }

		[Export ("SupportedPixelFormatsOrderedByPerformance")]
		CMPixelFormat[] SupportedPixelFormatsOrderedByPerformance { get; }

		[Export ("PixelFormatsWithReducedResolutionSupport")]
		CMPixelFormat[] PixelFormatsWithReducedResolutionSupport { get; }

		[Advice ("Use Strongly typed version PixelTransferSettings")]
		[Export ("PixelTransferProperties")]
		NSDictionary PixelTransferProperties { get; set; }

		// VTPixelTransferProperties are available in iOS 9 radar://22614931 https://trello.com/c/bTl6hRu9
		[StrongDictionary]
		[iOS (9,0)]
		[Export ("PixelTransferProperties")]
		VTPixelTransferProperties PixelTransferSettings { get; set; }
	}

	[Mac (10,9), iOS (8,0), TV (10,2)]
	[StrongDictionary ("VTVideoDecoderSpecificationKeys")]
	interface VTVideoDecoderSpecification {
		[Export ("EnableHardwareAcceleratedVideoDecoder")]
		bool EnableHardwareAcceleratedVideoDecoder { get; set; }

		[Export ("RequireHardwareAcceleratedVideoDecoder")]
		bool RequireHardwareAcceleratedVideoDecoder { get; set; }
	}

	[Mac (10,9), iOS (8,0), TV (10,2)]
	[Static]
	interface VTVideoDecoderSpecificationKeys {
		[Field ("kVTVideoDecoderSpecification_EnableHardwareAcceleratedVideoDecoder")]
		NSString EnableHardwareAcceleratedVideoDecoder { get; }

		[Field ("kVTVideoDecoderSpecification_RequireHardwareAcceleratedVideoDecoder")]
		NSString RequireHardwareAcceleratedVideoDecoder { get; }
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[StrongDictionary ("VTDecompressionResolutionKeys")]
	interface VTDecompressionResolutionOptions {
		[Export ("Width")]
		float Width { get; set; }

		[Export ("Height")]
		float Height { get; set; }
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[Static]
	interface VTDecompressionResolutionKeys {
		[Field ("kVTDecompressionResolutionKey_Width")]
		NSString Width { get; }

		[Field ("kVTDecompressionResolutionKey_Height")]
		NSString Height { get; }
	}

	// VTSession.h
	[Mac (10,8), iOS (8,0), TV (10,2)]
	[StrongDictionary ("VTPropertyKeys")]
	interface VTPropertyOptions {
		[Export ("ShouldBeSerialized")]
		bool ShouldBeSerialized { get; set; }

		[Export ("SupportedValueMinimumKey")]
		NSNumber SupportedValueMinimum { get; set; }

		[Export ("SupportedValueMaximumKey")]
		NSNumber SupportedValueMaximum { get; set; }

		[Export ("SupportedValueListKey")]
		NSNumber[] SupportedValueList { get; set; }

		[Export ("DocumentationKey")]
		NSString Documentation { get; set; }
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[Static]
	interface VTPropertyKeys {
		[Field ("kVTPropertyTypeKey")]
		NSString Type { get; }

		[Field ("kVTPropertyReadWriteStatusKey")]
		NSString ReadWriteStatus { get; }

		[Field ("kVTPropertyShouldBeSerializedKey")]
		NSString ShouldBeSerialized { get; }

		[Field ("kVTPropertySupportedValueMinimumKey")]
		NSString SupportedValueMinimumKey { get; }

		[Field ("kVTPropertySupportedValueMaximumKey")]
		NSString SupportedValueMaximumKey { get; }

		[Field ("kVTPropertySupportedValueListKey")]
		NSString SupportedValueListKey { get; }

		[Field ("kVTPropertyDocumentationKey")]
		NSString DocumentationKey { get; }
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[Static]
	interface VTPropertyTypeKeys {
		[Field ("kVTPropertyType_Boolean")]
		NSString Boolean { get; }

		[Field ("kVTPropertyType_Enumeration")]
		NSString Enumeration { get; }

		[Field ("kVTPropertyType_Number")]
		NSString Number { get; }
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[Static]
	interface VTPropertyReadWriteStatusKeys {
		[Field ("kVTPropertyReadWriteStatus_ReadOnly")]
		NSString ReadOnly { get; }

		[Field ("kVTPropertyReadWriteStatus_ReadWrite")]
		NSString ReadWrite { get; }
	}

	// VTVideoEncoderList.h
	[Mac (10,8), iOS (8,0), TV (10,2)]
	[Static]
	[Internal]
	interface VTVideoEncoderList {
		[Field ("kVTVideoEncoderList_CodecName")]
		NSString CodecName { get; }

		[Field ("kVTVideoEncoderList_CodecType")]
		NSString CodecType { get; }

		[Field ("kVTVideoEncoderList_DisplayName")]
		NSString DisplayName { get; }

		[Field ("kVTVideoEncoderList_EncoderID")]
		NSString EncoderID { get; }

		[Field ("kVTVideoEncoderList_EncoderName")]
		NSString EncoderName { get; }
	}

	// VTMultiPassStorage.h
	[Mac (10,10), iOS (8,0), TV (10,2)] // not decorated in the header files - but all other definitions are 10.10 & 8.0
	[Static]
	interface VTMultiPassStorageCreationOptionKeys {
		[Field ("kVTMultiPassStorageCreationOption_DoNotDelete")]
		NSString DoNotDelete { get; }
	}

	[Mac (10,8), iOS (8,0), TV (10,2)]
	[StrongDictionary ("VTMultiPassStorageCreationOptionKeys")]
	interface VTMultiPassStorageCreationOptions {
		[Export ("DoNotDelete")]
		bool DoNotDelete { get; set; }
	}

	// VTPixelTransferProperties are available in iOS 9 radar://22614931 https://trello.com/c/bTl6hRu9
	[Mac (10,8), iOS (9,0), TV (10,2)]
	[StrongDictionary ("VTPixelTransferPropertyKeys")]
	interface VTPixelTransferProperties {
		[StrongDictionary]
		[Export ("DestinationCleanAperture")]
		AVVideoCleanApertureSettings DestinationCleanAperture { get; set; }

		[StrongDictionary]
		[Export ("DestinationPixelAspectRatio")]
		AVVideoPixelAspectRatioSettings DestinationPixelAspectRatio { get; set; }

		[iOS (10,0)]
		[Export ("DestinationICCProfile")]
		NSData DestinationICCProfile { get; set; }
	}

	// VTPixelTransferProperties are available in iOS 9 radar://22614931 https://trello.com/c/bTl6hRu9
	[Mac (10,8), iOS (9,0), TV (10,2)]
	[Static]
	[AdvancedAttribute]
	interface VTPixelTransferPropertyKeys {

		// ScalingMode

		[Field ("kVTPixelTransferPropertyKey_ScalingMode")]
		NSString ScalingMode { get; }

		[Field ("kVTScalingMode_Normal")]
		NSString ScalingMode_Normal { get; }

		[Field ("kVTScalingMode_CropSourceToCleanAperture")]
		NSString ScalingMode_CropSourceToCleanAperture { get; }

		[Field ("kVTScalingMode_Letterbox")]
		NSString ScalingMode_Letterbox { get; }

		[Field ("kVTScalingMode_Trim")]
		NSString ScalingMode_Trim { get; }

		// DestinationCleanAperture

		[Field ("kVTPixelTransferPropertyKey_DestinationCleanAperture")]
		NSString DestinationCleanAperture { get; }

		// DestinationCleanAperture

		[Field ("kVTPixelTransferPropertyKey_DestinationPixelAspectRatio")]
		NSString DestinationPixelAspectRatio { get; }

		// DownsamplingMode

		[Field ("kVTPixelTransferPropertyKey_DownsamplingMode")]
		NSString DownsamplingMode { get; }

		[Field ("kVTDownsamplingMode_Decimate")]
		NSString DownsamplingMode_Decimate { get; }

		[Field ("kVTDownsamplingMode_Average")]
		NSString DownsamplingMode_Average { get; }

		// DestinationColorPrimaries

		[iOS (10,0)]
		[Field ("kVTPixelTransferPropertyKey_DestinationColorPrimaries")]
		NSString DestinationColorPrimaries { get; }

		// DestinationColorPrimaries

		[iOS (10,0)]
		[Field ("kVTPixelTransferPropertyKey_DestinationTransferFunction")]
		NSString DestinationTransferFunction { get; }

		// DestinationICCProfile

		[iOS (10,0)]
		[Field ("kVTPixelTransferPropertyKey_DestinationICCProfile")]
		NSString DestinationICCProfile { get; }

		// DestinationYCbCrMatrix

		[Field ("kVTPixelTransferPropertyKey_DestinationYCbCrMatrix")]
		NSString DestinationYCbCrMatrix { get; }
	}
}