////
// VideoToolbox core types and enumerations
//
// Author: Miguel de Icaza (miguel@xamarin.com)
//         Alex Soto (alex.soto@xamarin.com)
//
// Copyright 2014 Xamarin Inc
//
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreMedia;
using XamCore.AVFoundation;

namespace XamCore.VideoToolbox {

	[Static]
	interface VTCompressionPropertyKey {
		// Buffers

		[Field ("kVTCompressionPropertyKey_NumberOfPendingFrames")]
		[Mac (10,8), iOS (8,0)]
		NSString NumberOfPendingFrames { get; }

		[Field ("kVTCompressionPropertyKey_PixelBufferPoolIsShared")]
		[Mac (10,8), iOS (8,0)]
		NSString PixelBufferPoolIsShared { get; }

		[Field ("kVTCompressionPropertyKey_VideoEncoderPixelBufferAttributes")]
		[Mac (10,8), iOS (8,0)]
		NSString VideoEncoderPixelBufferAttributes { get; }

		// Frame dependency

		[Field ("kVTCompressionPropertyKey_MaxKeyFrameInterval")]
		[Mac (10,8), iOS (8,0)]
		NSString MaxKeyFrameInterval { get; }

		[Field ("kVTCompressionPropertyKey_MaxKeyFrameIntervalDuration")]
		[Mac (10,8), iOS (8,0)]
		NSString MaxKeyFrameIntervalDuration { get; }

		[Field ("kVTCompressionPropertyKey_AllowTemporalCompression")]
		[Mac (10,8), iOS (8,0)]
		NSString AllowTemporalCompression { get; }

		[Field ("kVTCompressionPropertyKey_AllowFrameReordering")]
		[Mac (10,8), iOS (8,0)]
		NSString AllowFrameReordering { get; }

		// Rate control

		[Field ("kVTCompressionPropertyKey_AverageBitRate")]
		[Mac (10,8), iOS (8,0)]
		NSString AverageBitRate { get; }

		[Field ("kVTCompressionPropertyKey_DataRateLimits")]
		[Mac (10,8), iOS (8,0)]
		NSString DataRateLimits { get; } // NSArray of an even number of CFNumbers alternating [int, double](bytes, seconds] Read/write

		[Field ("kVTCompressionPropertyKey_Quality")]
		[Mac (10,8), iOS (8,0)]
		NSString Quality { get; }

		[Field ("kVTCompressionPropertyKey_MoreFramesBeforeStart")]
		[Mac (10,8), iOS (8,0)]
		NSString MoreFramesBeforeStart { get; }

		[Field ("kVTCompressionPropertyKey_MoreFramesAfterEnd")]
		[Mac (10,8), iOS (8,0)]
		NSString MoreFramesAfterEnd { get; }

		// Bitstream configuration

		[Field ("kVTCompressionPropertyKey_ProfileLevel")]
		[Mac (10,8), iOS (8,0)]
		NSString ProfileLevel { get; }

		[Field ("kVTCompressionPropertyKey_H264EntropyMode")]
		[Mac (10,9), iOS (8,0)]	
		NSString H264EntropyMode { get; }

		[Field ("kVTCompressionPropertyKey_Depth")]
		[Mac (10,8), iOS (8,0)]
		NSString Depth { get; }

		// Runtime restrictions

		[Field ("kVTCompressionPropertyKey_MaxFrameDelayCount")]
		[Mac (10,8), iOS (8,0)]
		NSString MaxFrameDelayCount { get; }

		[Field ("kVTCompressionPropertyKey_MaxH264SliceBytes")]
		[Mac (10,8), iOS (8,0)]
		NSString MaxH264SliceBytes { get; }

		[Field ("kVTCompressionPropertyKey_RealTime")]
		[Mac (10,9), iOS (8,0)] 
		NSString RealTime { get; }

		// Hints

		[Field ("kVTCompressionPropertyKey_SourceFrameCount")]
		[Mac (10,8), iOS (8,0)]
		NSString SourceFrameCount { get; }

		[Field ("kVTCompressionPropertyKey_ExpectedFrameRate")]
		[Mac (10,8), iOS (8,0)]
		NSString ExpectedFrameRate { get; }

		[Field ("kVTCompressionPropertyKey_ExpectedDuration")]
		[Mac (10,8), iOS (8,0)]
		NSString ExpectedDuration { get; }

		// Hardware acceleration
		// Hardware acceleration is default behavior on iOS. No opt-in required.

		[Field ("kVTCompressionPropertyKey_UsingHardwareAcceleratedVideoEncoder")]
		[Mac (10,8), iOS (8,0)]
		NSString UsingHardwareAcceleratedVideoEncoder { get; } // CFBoolean Read

		// Clean aperture and pixel aspect ratio

		[Field ("kVTCompressionPropertyKey_CleanAperture")]
		[Mac (10,8), iOS (8,0)]
		NSString CleanAperture { get; }

		[Field ("kVTCompressionPropertyKey_PixelAspectRatio")]
		[Mac (10,8), iOS (8,0)]
		NSString PixelAspectRatio { get; }

		[Field ("kVTCompressionPropertyKey_FieldCount")]
		[Mac (10,8), iOS (8,0)]
		NSString FieldCount { get; } 

		[Field ("kVTCompressionPropertyKey_FieldDetail")]
		[Mac (10,8), iOS (8,0)]
		NSString FieldDetail { get; } 

		[Field ("kVTCompressionPropertyKey_AspectRatio16x9")]
		[Mac (10,8), iOS (8,0)]
		NSString AspectRatio16x9 { get; } 

		[Field ("kVTCompressionPropertyKey_ProgressiveScan")]
		[Mac (10,8), iOS (8,0)]
		NSString ProgressiveScan { get; } 

		// Color

		[Field ("kVTCompressionPropertyKey_ColorPrimaries")]
		[Mac (10,8), iOS (8,0)]
		NSString ColorPrimaries { get; } 

		[Field ("kVTCompressionPropertyKey_TransferFunction")]
		[Mac (10,8), iOS (8,0)]
		NSString TransferFunction { get; } 

		[Field ("kVTCompressionPropertyKey_YCbCrMatrix")]
		[Mac (10,8), iOS (8,0)]
		NSString YCbCrMatrix { get; } 

		[Field ("kVTCompressionPropertyKey_ICCProfile")]
		[Mac (10,8), iOS (8,0)]
		NSString ICCProfile { get; } 

		// Pre-compression processing

		[Field ("kVTCompressionPropertyKey_PixelTransferProperties")]
		[Mac (10,8), iOS (8,0)]
		NSString PixelTransferProperties { get; }

		// Multi-pass

		[Field ("kVTCompressionPropertyKey_MultiPassStorage")]
		[Mac (10,10), iOS (8,0)]
		NSString MultiPassStorage { get; }
	}

	[StrongDictionary ("VTCompressionPropertyKey")]
	interface VTCompressionProperties {

		[Mac (10,8), iOS (8,0)]
		[Export ("NumberOfPendingFrames")]
		int NumberOfPendingFrames { get; }

		[Mac (10,8), iOS (8,0)]
		[Export ("PixelBufferPoolIsShared")]
		bool PixelBufferPoolIsShared { get; }

		[Mac (10,8), iOS (8,0)]
		[Export ("VideoEncoderPixelBufferAttributes")]
		NSDictionary VideoEncoderPixelBufferAttributes { get; }

		[Mac (10,8), iOS (8,0)]
		[Export ("MaxKeyFrameInterval")]
		int MaxKeyFrameInterval { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("MaxKeyFrameIntervalDuration")]
		double MaxKeyFrameIntervalDuration { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("AllowTemporalCompression")]
		bool AllowTemporalCompression { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("AllowFrameReordering")]
		bool AllowFrameReordering { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("AverageBitRate")]
		int AverageBitRate { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("Quality")]
		float Quality { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("MoreFramesBeforeStart")]
		bool MoreFramesBeforeStart { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("MoreFramesAfterEnd")]
		bool MoreFramesAfterEnd { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("Depth")]
		CMPixelFormat Depth { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("MaxFrameDelayCount")]
		int MaxFrameDelayCount { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("MaxH264SliceBytes")]
		int MaxH264SliceBytes { get; set; }

		[Mac (10,9), iOS (8,0)] 
		[Export ("RealTime")]
		bool RealTime { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("SourceFrameCount")]
		uint SourceFrameCount { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("ExpectedFrameRate")]
		double ExpectedFrameRate { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("ExpectedDuration")]
		double ExpectedDuration { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("UsingHardwareAcceleratedVideoEncoder")]
		bool UsingHardwareAcceleratedVideoEncoder { get; }

		[Mac (10,8), iOS (8,0)]
		[Export ("CleanAperture")]
		NSDictionary CleanAperture { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("PixelAspectRatio")]
		NSDictionary PixelAspectRatio { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("FieldCount")]
		VTFieldCount FieldCount { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("AspectRatio16x9")]
		bool AspectRatio16x9 { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("ProgressiveScan")]
		bool ProgressiveScan { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("ICCProfile")]
		NSData ICCProfile { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("PixelTransferProperties")]
		NSDictionary PixelTransferProperties { get; set; }
	}

	[Static]
	interface VTProfileLevelKeys {
		// H264

		[Field ("kVTProfileLevel_H264_Baseline_1_3")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_Baseline_1_3 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_3_0")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_Baseline_3_0 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_3_1")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_Baseline_3_1 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_3_2")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_Baseline_3_2 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_4_0")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_Baseline_4_0 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_4_1")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_Baseline_4_1 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_4_2")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_Baseline_4_2 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_5_0")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_Baseline_5_0 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_5_1")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_Baseline_5_1 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_5_2")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_Baseline_5_2 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_AutoLevel")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_Baseline_AutoLevel { get; }

		[Field ("kVTProfileLevel_H264_Main_3_0")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_Main_3_0 { get; }

		[Field ("kVTProfileLevel_H264_Main_3_1")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_Main_3_1 { get; }

		[Field ("kVTProfileLevel_H264_Main_3_2")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_Main_3_2 { get; }

		[Field ("kVTProfileLevel_H264_Main_4_0")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_Main_4_0 { get; }

		[Field ("kVTProfileLevel_H264_Main_4_1")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_Main_4_1 { get; }

		[Field ("kVTProfileLevel_H264_Main_4_2")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_Main_4_2 { get; }

		[Field ("kVTProfileLevel_H264_Main_5_0")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_Main_5_0 { get; }

		[Field ("kVTProfileLevel_H264_Main_5_1")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_Main_5_1 { get; }

		[Field ("kVTProfileLevel_H264_Main_5_2")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_Main_5_2 { get; }

		[Field ("kVTProfileLevel_H264_Main_AutoLevel")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_Main_AutoLevel { get; }

		[Field ("kVTProfileLevel_H264_Extended_5_0")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_Extended_5_0 { get; }

		[Field ("kVTProfileLevel_H264_Extended_AutoLevel")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_Extended_AutoLevel { get; }

		[Field ("kVTProfileLevel_H264_High_3_0")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_High_3_0 { get; }

		[Field ("kVTProfileLevel_H264_High_3_1")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_High_3_1 { get; }

		[Field ("kVTProfileLevel_H264_High_3_2")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_High_3_2 { get; }

		[Field ("kVTProfileLevel_H264_High_4_0")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_High_4_0 { get; }

		[Field ("kVTProfileLevel_H264_High_4_1")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_High_4_1 { get; }

		[Field ("kVTProfileLevel_H264_High_4_2")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_High_4_2 { get; }

		[Field ("kVTProfileLevel_H264_High_5_0")]
		[Mac (10,8), iOS (8,0)]
		NSString H264_High_5_0 { get; }

		[Field ("kVTProfileLevel_H264_High_5_1")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_High_5_1 { get; }

		[Field ("kVTProfileLevel_H264_High_5_2")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_High_5_2 { get; }

		[Field ("kVTProfileLevel_H264_High_AutoLevel")]
		[Mac (10,9), iOS (8,0)]
		NSString H264_High_AutoLevel { get; }

		// MP4V

		[Field ("kVTProfileLevel_MP4V_Simple_L0")]
		[Mac (10,8), iOS (8,0)]
		NSString MP4V_Simple_L0 { get; }

		[Field ("kVTProfileLevel_MP4V_Simple_L1")]
		[Mac (10,8), iOS (8,0)]
		NSString MP4V_Simple_L1 { get; }

		[Field ("kVTProfileLevel_MP4V_Simple_L2")]
		[Mac (10,8), iOS (8,0)]
		NSString MP4V_Simple_L2 { get; }

		[Field ("kVTProfileLevel_MP4V_Simple_L3")]
		[Mac (10,8), iOS (8,0)]
		NSString MP4V_Simple_L3 { get; }

		[Field ("kVTProfileLevel_MP4V_Main_L2")]
		[Mac (10,8), iOS (8,0)]
		NSString MP4V_Main_L2 { get; }

		[Field ("kVTProfileLevel_MP4V_Main_L3")]
		[Mac (10,8), iOS (8,0)]
		NSString MP4V_Main_L3 { get; }

		[Field ("kVTProfileLevel_MP4V_Main_L4")]
		[Mac (10,8), iOS (8,0)]
		NSString MP4V_Main_L4 { get; }

		[Field ("kVTProfileLevel_MP4V_AdvancedSimple_L0")]
		[Mac (10,8), iOS (8,0)]
		NSString MP4V_AdvancedSimple_L0 { get; }

		[Field ("kVTProfileLevel_MP4V_AdvancedSimple_L1")]
		[Mac (10,8), iOS (8,0)]
		NSString MP4V_AdvancedSimple_L1 { get; }

		[Field ("kVTProfileLevel_MP4V_AdvancedSimple_L2")]
		[Mac (10,8), iOS (8,0)]
		NSString MP4V_AdvancedSimple_L2 { get; }

		[Field ("kVTProfileLevel_MP4V_AdvancedSimple_L3")]
		[Mac (10,8), iOS (8,0)]
		NSString MP4V_AdvancedSimple_L3 { get; }

		[Field ("kVTProfileLevel_MP4V_AdvancedSimple_L4")]
		[Mac (10,8), iOS (8,0)]
		NSString MP4V_AdvancedSimple_L4 { get; }

		// H263

		[Field ("kVTProfileLevel_H263_Profile0_Level10")]
		[Mac (10,8), iOS (8,0)]
		NSString H263_Profile0_Level10 { get; }

		[Field ("kVTProfileLevel_H263_Profile0_Level45")]
		[Mac (10,8), iOS (8,0)]
		NSString H263_Profile0_Level45 { get; }

		[Field ("kVTProfileLevel_H263_Profile3_Level45")]
		[Mac (10,8), iOS (8,0)]
		NSString H263_Profile3_Level45 { get; } 
	}

	[Static]
	interface VTH264EntropyModeKeys {
		[Field ("kVTH264EntropyMode_CAVLC")]
		[Mac (10,9), iOS (8,0)]
		NSString CAVLC { get; } 

		[Field ("kVTH264EntropyMode_CABAC")]
		[Mac (10,9), iOS (8,0)]
		NSString CABAC { get; } 
	}
		
	[StrongDictionary ("VTVideoEncoderSpecificationKeys")]
	interface VTVideoEncoderSpecification {
#if MONOMAC
		[Mac (10,9)] 
		[Export ("EnableHardwareAcceleratedVideoEncoder")]
		bool EnableHardwareAcceleratedVideoEncoder { get; set; }

		[Mac (10,9)] 
		[Export ("RequireHardwareAcceleratedVideoEncoder")]
		bool RequireHardwareAcceleratedVideoEncoder { get; set; }
#endif
		[Mac (10,8), iOS (8,0)]
		[Export ("EncoderID")]
		string EncoderID { get; set; }
	}

	[Static]
	interface VTVideoEncoderSpecificationKeys {
#if MONOMAC
		[Field ("kVTVideoEncoderSpecification_EnableHardwareAcceleratedVideoEncoder")]
		[Mac (10,9)] 
		NSString EnableHardwareAcceleratedVideoEncoder { get; }

		[Field ("kVTVideoEncoderSpecification_RequireHardwareAcceleratedVideoEncoder")]
		[Mac (10,9)] 
		NSString RequireHardwareAcceleratedVideoEncoder { get; }
#endif
		[Mac (10,8), iOS (8,0)]
		[Field ("kVTVideoEncoderSpecification_EncoderID")]
		NSString EncoderID { get; }
	}

	[StrongDictionary ("VTEncodeFrameOptionKey")]
	interface VTEncodeFrameOptions {
		[Export ("ForceKeyFrame")]
		bool ForceKeyFrame { get; set; }
	}

	[Static]
	interface VTEncodeFrameOptionKey {
		// Per-frame configuration

		[Field ("kVTEncodeFrameOptionKey_ForceKeyFrame")]
		[Mac (10,8), iOS (8,0)]
		NSString ForceKeyFrame { get; } 
	}

	[Static]
	interface VTDecompressionPropertyKey {
		// Pixel buffer pools

		[Field ("kVTDecompressionPropertyKey_PixelBufferPool")]
		[Mac (10,8), iOS (8,0)]
		NSString PixelBufferPool { get; }

		[Field ("kVTDecompressionPropertyKey_PixelBufferPoolIsShared")]
		[Mac (10,8), iOS (8,0)]
		NSString PixelBufferPoolIsShared { get; }

		[Field ("kVTDecompressionPropertyKey_OutputPoolRequestedMinimumBufferCount")]
		[Mac (10,9), iOS (8,0)] 
		NSString OutputPoolRequestedMinimumBufferCount { get; }

		// Asynchronous state

		[Field ("kVTDecompressionPropertyKey_NumberOfFramesBeingDecoded")]
		[Mac (10,8), iOS (8,0)]
		NSString NumberOfFramesBeingDecoded { get; }

		[Field ("kVTDecompressionPropertyKey_MinOutputPresentationTimeStampOfFramesBeingDecoded")]
		[Mac (10,8), iOS (8,0)]
		NSString MinOutputPresentationTimeStampOfFramesBeingDecoded { get; }

		[Field ("kVTDecompressionPropertyKey_MaxOutputPresentationTimeStampOfFramesBeingDecoded")]
		[Mac (10,8), iOS (8,0)]
		NSString MaxOutputPresentationTimeStampOfFramesBeingDecoded { get; } 

		// Content

		[Field ("kVTDecompressionPropertyKey_ContentHasInterframeDependencies")]
		[Mac (10,8), iOS (8,0)]
		NSString ContentHasInterframeDependencies { get; }

		// Hardware acceleration
		// hardware acceleration is default behavior on iOS.  no opt-in required.
		 
		[Field ("kVTDecompressionPropertyKey_UsingHardwareAcceleratedVideoDecoder")]
		[Mac (10,9), iOS (8,0)] 
		NSString UsingHardwareAcceleratedVideoDecoder { get; } 

		// Decoder behavior

		[Field ("kVTDecompressionPropertyKey_RealTime")]
		[Mac (10,10), iOS (8,0)]
		NSString RealTime { get; }

		[Field ("kVTDecompressionPropertyKey_ThreadCount")]
		[Mac (10,8), iOS (8,0)]
		NSString ThreadCount { get; }

		[Field ("kVTDecompressionPropertyKey_FieldMode")]
		[Mac (10,8), iOS (8,0)]
		NSString FieldMode { get; }

		[Field ("kVTDecompressionProperty_FieldMode_BothFields")]
		[Mac (10,8), iOS (8,0)]
		NSString FieldMode_BothFields { get; } 

		[Field ("kVTDecompressionProperty_FieldMode_TopFieldOnly")]
		[Mac (10,8), iOS (8,0)]
		NSString FieldMode_TopFieldOnly { get; }

		[Field ("kVTDecompressionProperty_FieldMode_BottomFieldOnly")]
		[Mac (10,8), iOS (8,0)]
		NSString FieldMode_BottomFieldOnly { get; }

		[Field ("kVTDecompressionProperty_FieldMode_SingleField")]
		[Mac (10,8), iOS (8,0)] 
		NSString FieldMode_SingleField { get; }

		[Field ("kVTDecompressionProperty_FieldMode_DeinterlaceFields")]
		[Mac (10,8), iOS (8,0)]
		NSString FieldMode_DeinterlaceFields { get; }

		[Field ("kVTDecompressionPropertyKey_DeinterlaceMode")]
		[Mac (10,8), iOS (8,0)]  
		NSString DeinterlaceMode { get; }

		[Field ("kVTDecompressionProperty_DeinterlaceMode_VerticalFilter")]
		[Mac (10,8), iOS (8,0)]  
		NSString DeinterlaceMode_VerticalFilter { get; } 

		[Field ("kVTDecompressionProperty_DeinterlaceMode_Temporal")]
		[Mac (10,8), iOS (8,0)]	
		NSString DeinterlaceMode_Temporal { get; } 

		[Field ("kVTDecompressionPropertyKey_ReducedResolutionDecode")]
		[Mac (10,8), iOS (8,0)]
		NSString ReducedResolutionDecode { get; }

		[Field ("kVTDecompressionPropertyKey_ReducedCoefficientDecode")]
		[Mac (10,8), iOS (8,0)]
		NSString ReducedCoefficientDecode { get; }

		[Field ("kVTDecompressionPropertyKey_ReducedFrameDelivery")]
		[Mac (10,8), iOS (8,0)]
		NSString ReducedFrameDelivery { get; }

		[Field ("kVTDecompressionPropertyKey_OnlyTheseFrames")]
		[Mac (10,8), iOS (8,0)]
		NSString OnlyTheseFrames { get; } 

		[Field ("kVTDecompressionProperty_OnlyTheseFrames_AllFrames")]
		[Mac (10,8), iOS (8,0)]
		NSString OnlyTheseFrames_AllFrames { get; } 

		[Field ("kVTDecompressionProperty_OnlyTheseFrames_NonDroppableFrames")]
		[Mac (10,8), iOS (8,0)]
		NSString OnlyTheseFrames_NonDroppableFrames { get; } 

		[Field ("kVTDecompressionProperty_OnlyTheseFrames_IFrames")]
		[Mac (10,8), iOS (8,0)]
		NSString OnlyTheseFrames_IFrames { get; } 

		[Field ("kVTDecompressionProperty_OnlyTheseFrames_KeyFrames")]
		[Mac (10,8), iOS (8,0)]
		NSString OnlyTheseFrames_KeyFrames { get; } 

		[Field ("kVTDecompressionPropertyKey_SuggestedQualityOfServiceTiers")]
		[Mac (10,8), iOS (8,0)]
		NSString SuggestedQualityOfServiceTiers { get; }

		[Field ("kVTDecompressionPropertyKey_SupportedPixelFormatsOrderedByQuality")]
		[Mac (10,8), iOS (8,0)]
		NSString SupportedPixelFormatsOrderedByQuality { get; }

		[Field ("kVTDecompressionPropertyKey_SupportedPixelFormatsOrderedByPerformance")]
		[Mac (10,8), iOS (8,0)]
		NSString SupportedPixelFormatsOrderedByPerformance { get; }

		[Field ("kVTDecompressionPropertyKey_PixelFormatsWithReducedResolutionSupport")]
		[Mac (10,8), iOS (8,0)]
		NSString PixelFormatsWithReducedResolutionSupport { get; }

		//Post-decompression processing

		[Field ("kVTDecompressionPropertyKey_PixelTransferProperties")]
		[Mac (10,8), iOS (8,0)]
		NSString PixelTransferProperties { get; }
	}

	[StrongDictionary ("VTDecompressionPropertyKey")]
	interface VTDecompressionProperties	{
		[Mac (10,8), iOS (8,0)]
		[Export ("PixelBufferPoolIsShared")]
		bool PixelBufferPoolIsShared { get; }

		[Mac (10,8), iOS (8,0)]
		[Export ("OutputPoolRequestedMinimumBufferCount")]
		uint OutputPoolRequestedMinimumBufferCount { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("NumberOfFramesBeingDecoded")]
		uint NumberOfFramesBeingDecoded { get; }

		[Mac (10,8), iOS (8,0)]
		[Export ("MinOutputPresentationTimeStampOfFramesBeingDecoded")]
		NSDictionary MinOutputPresentationTimeStampOfFramesBeingDecoded { get; }

		[Mac (10,8), iOS (8,0)]
		[Export ("MaxOutputPresentationTimeStampOfFramesBeingDecoded")]
		NSDictionary MaxOutputPresentationTimeStampOfFramesBeingDecoded { get; }

		[Mac (10,8), iOS (8,0)]
		[Export ("ContentHasInterframeDependencies")]
		bool ContentHasInterframeDependencies { get; }

		// Hardware acceleration
		// hardware acceleration is default behavior on iOS.  no opt-in required.

		[Export ("UsingHardwareAcceleratedVideoDecoder")]
		[Mac (10,9)] 
		bool UsingHardwareAcceleratedVideoDecoder { get; } 

		[Mac (10,10), iOS (8,0)]
		[Export ("RealTime")]
		bool RealTime { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("ThreadCount")]
		uint ThreadCount { get; set; }

		[StrongDictionary]
		[Mac (10,8), iOS (8,0)]
		[Export ("ReducedResolutionDecode")]
		VTDecompressionResolutionOptions ReducedResolutionDecode { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("ReducedCoefficientDecode")]
		uint ReducedCoefficientDecode { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("ReducedFrameDelivery")]
		float ReducedFrameDelivery { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("SuggestedQualityOfServiceTiers")]
		NSDictionary[] SuggestedQualityOfServiceTiers { get; }

		[Mac (10,8), iOS (8,0)]
		[Export ("SupportedPixelFormatsOrderedByQuality")]
		CMPixelFormat[] SupportedPixelFormatsOrderedByQuality { get; }

		[Mac (10,8), iOS (8,0)]
		[Export ("SupportedPixelFormatsOrderedByPerformance")]
		CMPixelFormat[] SupportedPixelFormatsOrderedByPerformance { get; }

		[Mac (10,8), iOS (8,0)]
		[Export ("PixelFormatsWithReducedResolutionSupport")]
		CMPixelFormat[] PixelFormatsWithReducedResolutionSupport { get; }

		[Mac (10,8), iOS (8,0)]
		[Advice ("Use Strongly typed version PixelTransferSettings")]
		[Export ("PixelTransferProperties")]
		NSDictionary PixelTransferProperties { get; set; }

		// VTPixelTransferProperties are available in iOS 9 radar://22614931 https://trello.com/c/bTl6hRu9
		[StrongDictionary]
		[Mac (10,8), iOS (9,0)]
		[Export ("PixelTransferProperties")]
		VTPixelTransferProperties PixelTransferSettings { get; set; }
	}

	[StrongDictionary ("VTVideoDecoderSpecificationKeys")]
	interface VTVideoDecoderSpecification {
		[Export ("EnableHardwareAcceleratedVideoDecoder")]
		[Mac (10,9)]
		bool EnableHardwareAcceleratedVideoDecoder { get; set; }

		[Export ("RequireHardwareAcceleratedVideoDecoder")]
		[Mac (10,9)]
		bool RequireHardwareAcceleratedVideoDecoder { get; set; }
	}

	[Static]
	interface VTVideoDecoderSpecificationKeys {
		[Field ("kVTVideoDecoderSpecification_EnableHardwareAcceleratedVideoDecoder")]
		[Mac (10,9), iOS (8,0)] 
		NSString EnableHardwareAcceleratedVideoDecoder { get; }

		[Field ("kVTVideoDecoderSpecification_RequireHardwareAcceleratedVideoDecoder")]
		[Mac (10,9), iOS (8,0)] 
		NSString RequireHardwareAcceleratedVideoDecoder { get; }
	}

	[StrongDictionary ("VTDecompressionResolutionKeys")]
	interface VTDecompressionResolutionOptions {
		[Export ("Width")]
		[Mac (10,8), iOS (8,0)]
		float Width { get; set; }

		[Export ("Height")]
		[Mac (10,8), iOS (8,0)]
		float Height { get; set; }
	}

	[Static]
	interface VTDecompressionResolutionKeys {
		[Field ("kVTDecompressionResolutionKey_Width")]
		[Mac (10,8), iOS (8,0)]
		NSString Width { get; }

		[Field ("kVTDecompressionResolutionKey_Height")]
		[Mac (10,8), iOS (8,0)]
		NSString Height { get; }
	}

	// VTSession.h
	[StrongDictionary ("VTPropertyKeys")]
	interface VTPropertyOptions {
		[Export ("ShouldBeSerialized")]
		[Mac (10,8), iOS (8,0)]
		bool ShouldBeSerialized { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("SupportedValueMinimumKey")]
		NSNumber SupportedValueMinimum { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("SupportedValueMaximumKey")]
		NSNumber SupportedValueMaximum { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("SupportedValueListKey")]
		NSNumber[] SupportedValueList { get; set; }

		[Mac (10,8), iOS (8,0)]
		[Export ("DocumentationKey")]
		NSString Documentation { get; set; }
	}

	[Static]
	interface VTPropertyKeys {
		[Mac (10,8), iOS (8,0)]
		[Field ("kVTPropertyTypeKey")]
		NSString Type { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTPropertyReadWriteStatusKey")]
		NSString ReadWriteStatus { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTPropertyShouldBeSerializedKey")]
		NSString ShouldBeSerialized { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTPropertySupportedValueMinimumKey")]
		NSString SupportedValueMinimumKey { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTPropertySupportedValueMaximumKey")]
		NSString SupportedValueMaximumKey { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTPropertySupportedValueListKey")]
		NSString SupportedValueListKey { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTPropertyDocumentationKey")]
		NSString DocumentationKey { get; }
	}

	[Static]
	interface VTPropertyTypeKeys {
		[Mac (10,8), iOS (8,0)]
		[Field ("kVTPropertyType_Boolean")]
		NSString Boolean { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTPropertyType_Enumeration")]
		NSString Enumeration { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTPropertyType_Number")]
		NSString Number { get; }
	}

	[Static]
	interface VTPropertyReadWriteStatusKeys {
		[Mac (10,8), iOS (8,0)]
		[Field ("kVTPropertyReadWriteStatus_ReadOnly")]
		NSString ReadOnly { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTPropertyReadWriteStatus_ReadWrite")]
		NSString ReadWrite { get; }
	}

	// VTVideoEncoderList.h
	[Static]
	[Internal]
	interface VTVideoEncoderList {
		[Mac (10,8), iOS (8,0)]
		[Field ("kVTVideoEncoderList_CodecName")]
		NSString CodecName { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTVideoEncoderList_CodecType")]
		NSString CodecType { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTVideoEncoderList_DisplayName")]
		NSString DisplayName { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTVideoEncoderList_EncoderID")]
		NSString EncoderID { get; }

		[Mac (10,8), iOS (8,0)]
		[Field ("kVTVideoEncoderList_EncoderName")]
		NSString EncoderName { get; }
	}

	// VTMultiPassStorage.h
	[Static]
	interface VTMultiPassStorageCreationOptionKeys {
		[Field ("kVTMultiPassStorageCreationOption_DoNotDelete")]
		[Mac (10,10), iOS (8,0)] // not decorated in the header files - but all other definitions are 10.10 & 8.0
		NSString DoNotDelete { get; }
	}

	[StrongDictionary ("VTMultiPassStorageCreationOptionKeys")]
	interface VTMultiPassStorageCreationOptions {
		[Export ("DoNotDelete")]
		[Mac (10,8), iOS (8,0)]
		bool DoNotDelete { get; set; }
	}

	// VTPixelTransferProperties are available in iOS 9 radar://22614931 https://trello.com/c/bTl6hRu9
	[StrongDictionary ("VTPixelTransferPropertyKeys")]
	interface VTPixelTransferProperties {
		[StrongDictionary]
		[Mac (10,8), iOS (9,0)]
		[Export ("DestinationCleanAperture")]
		AVVideoCleanApertureSettings DestinationCleanAperture { get; set; }

		[StrongDictionary]
		[Mac (10,8), iOS (9,0)]
		[Export ("DestinationPixelAspectRatio")]
		AVVideoPixelAspectRatioSettings DestinationPixelAspectRatio { get; set; }

		[iOS (10,0)]
		[Mac (10,8)]
		[Export ("DestinationICCProfile")]
		NSData DestinationICCProfile { get; set; }
	}

	// VTPixelTransferProperties are available in iOS 9 radar://22614931 https://trello.com/c/bTl6hRu9
	[Static]
	[AdvancedAttribute]
	interface VTPixelTransferPropertyKeys {

		// ScalingMode

		[Mac (10,8), iOS (9,0)]
		[Field ("kVTPixelTransferPropertyKey_ScalingMode")]
		NSString ScalingMode { get; }

		[Mac (10,8), iOS (9,0)]
		[Field ("kVTScalingMode_Normal")]
		NSString ScalingMode_Normal { get; }

		[Mac (10,8), iOS (9,0)]
		[Field ("kVTScalingMode_CropSourceToCleanAperture")]
		NSString ScalingMode_CropSourceToCleanAperture { get; }

		[Mac (10,8), iOS (9,0)]
		[Field ("kVTScalingMode_Letterbox")]
		NSString ScalingMode_Letterbox { get; }

		[Mac (10,8), iOS (9,0)]
		[Field ("kVTScalingMode_Trim")]
		NSString ScalingMode_Trim { get; }

		// DestinationCleanAperture

		[Mac (10,8), iOS (9,0)]
		[Field ("kVTPixelTransferPropertyKey_DestinationCleanAperture")]
		NSString DestinationCleanAperture { get; }

		// DestinationCleanAperture

		[Mac (10,8), iOS (9,0)]
		[Field ("kVTPixelTransferPropertyKey_DestinationPixelAspectRatio")]
		NSString DestinationPixelAspectRatio { get; }

		// DownsamplingMode

		[Mac (10,8), iOS (9,0)]
		[Field ("kVTPixelTransferPropertyKey_DownsamplingMode")]
		NSString DownsamplingMode { get; }

		[Mac (10,8), iOS (9,0)]
		[Field ("kVTDownsamplingMode_Decimate")]
		NSString DownsamplingMode_Decimate { get; }

		[Mac (10,8), iOS (9,0)]
		[Field ("kVTDownsamplingMode_Average")]
		NSString DownsamplingMode_Average { get; }

		// DestinationColorPrimaries

		[iOS (10,0)]
		[Mac (10,8)]
		[Field ("kVTPixelTransferPropertyKey_DestinationColorPrimaries")]
		NSString DestinationColorPrimaries { get; }

		// DestinationColorPrimaries

		[iOS (10,0)]
		[Mac (10,8)]
		[Field ("kVTPixelTransferPropertyKey_DestinationTransferFunction")]
		NSString DestinationTransferFunction { get; }

		// DestinationICCProfile

		[iOS (10,0)]
		[Mac (10,8)]
		[Field ("kVTPixelTransferPropertyKey_DestinationICCProfile")]
		NSString DestinationICCProfile { get; }

		// DestinationYCbCrMatrix

		[Mac (10,8), iOS (9,0)]
		[Field ("kVTPixelTransferPropertyKey_DestinationYCbCrMatrix")]
		NSString DestinationYCbCrMatrix { get; }
	}
}