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

	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Field ("kVTCompressionPropertyKey_AllowOpenGOP")]
		NSString AllowOpenGop { get; }

		// Rate control

		[Field ("kVTCompressionPropertyKey_AverageBitRate")]
		NSString AverageBitRate { get; }

		[Field ("kVTCompressionPropertyKey_DataRateLimits")]
		NSString DataRateLimits { get; } // NSArray of an even number of CFNumbers alternating [int, double](bytes, seconds] Read/write

		[Field ("kVTCompressionPropertyKey_Quality")]
		NSString Quality { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTCompressionPropertyKey_TargetQualityForAlpha")]
		NSString TargetQualityForAlpha { get; }

		[Field ("kVTCompressionPropertyKey_MoreFramesBeforeStart")]
		NSString MoreFramesBeforeStart { get; }

		[Field ("kVTCompressionPropertyKey_MoreFramesAfterEnd")]
		NSString MoreFramesAfterEnd { get; }

		// Bitstream configuration

		[Field ("kVTCompressionPropertyKey_ProfileLevel")]
		NSString ProfileLevel { get; }

		[Field ("kVTCompressionPropertyKey_H264EntropyMode")]
		[MacCatalyst (13, 1)]
		NSString H264EntropyMode { get; }

		[Field ("kVTCompressionPropertyKey_Depth")]
		NSString Depth { get; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), Watch (9, 0), TV (16, 0)]
		[Field ("kVTCompressionPropertyKey_PreserveAlphaChannel")]
		NSString PreserveAlphaChannel { get; }

		// Runtime restrictions

		[Field ("kVTCompressionPropertyKey_MaxFrameDelayCount")]
		NSString MaxFrameDelayCount { get; }

		[Field ("kVTCompressionPropertyKey_MaxH264SliceBytes")]
		NSString MaxH264SliceBytes { get; }

		[Field ("kVTCompressionPropertyKey_RealTime")]
		[MacCatalyst (13, 1)]
		NSString RealTime { get; }

		[Field ("kVTCompressionPropertyKey_MaximizePowerEfficiency")]
		[MacCatalyst (13, 1)]
		NSString MaximizePowerEfficiency { get; }

		// Hints

		[Field ("kVTCompressionPropertyKey_SourceFrameCount")]
		NSString SourceFrameCount { get; }

		[Field ("kVTCompressionPropertyKey_ExpectedFrameRate")]
		NSString ExpectedFrameRate { get; }

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("kVTCompressionPropertyKey_BaseLayerFrameRateFraction")]
		NSString BaseLayerFrameRateFraction { get; }

		[Field ("kVTCompressionPropertyKey_ExpectedDuration")]
		NSString ExpectedDuration { get; }

		[MacCatalyst (13, 1)]
		[Field ("kVTCompressionPropertyKey_BaseLayerFrameRate")]
		NSString BaseLayerFrameRate { get; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), Watch (9, 0), TV (16, 0)]
		[Field ("kVTCompressionPropertyKey_ReferenceBufferCount")]
		NSString ReferenceBufferCount { get; }

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

		// AlphaChannelMode

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTCompressionPropertyKey_AlphaChannelMode")]
		NSString AlphaChannelMode { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTCompressionPropertyKey_GammaLevel")]
		NSString GammaLevel { get; }

		[MacCatalyst (13, 1)]
		[Field ("kVTCompressionPropertyKey_MasteringDisplayColorVolume")]
		NSString MasteringDisplayColorVolume { get; }

		[MacCatalyst (13, 1)]
		[Field ("kVTCompressionPropertyKey_ContentLightLevelInfo")]
		NSString ContentLightLevelInfo { get; }

		// Pre-compression processing

		[Field ("kVTCompressionPropertyKey_PixelTransferProperties")]
		NSString PixelTransferProperties { get; }

		// Multi-pass

		[Field ("kVTCompressionPropertyKey_MultiPassStorage")]
		[MacCatalyst (13, 1)]
		NSString MultiPassStorage { get; }

		// Encoder information

		[Field ("kVTCompressionPropertyKey_EncoderID")]
		[MacCatalyst (13, 1)]
		NSString EncoderId { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTCompressionPropertyKey_UsingGPURegistryID")]
		NSString UsingGpuRegistryId { get; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("kVTCompressionPropertyKey_HDRMetadataInsertionMode")]
		NSString HdrMetadataInsertionMode { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("kVTCompressionPropertyKey_PrioritizeEncodingSpeedOverQuality")]
		NSString PrioritizeEncodingSpeedOverQuality { get; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
		[Field ("kVTCompressionPropertyKey_ConstantBitRate")]
		NSString ConstantBitRate { get; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), Watch (9, 0), TV (16, 0)]
		[Field ("kVTCompressionPropertyKey_EstimatedAverageBytesPerFrame")]
		NSString EstimatedAverageBytesPerFrame { get; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		[Field ("kVTCompressionPropertyKey_PreserveDynamicHDRMetadata")]
		NSString PreserveDynamicHdrMetadata { get; }

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("kVTVideoEncoderSpecification_EnableLowLatencyRateControl")]
		NSString EnableLowLatencyRateControl { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTCompressionPropertyKey_BaseLayerBitRateFraction")]
		NSString BaseLayerBitRateFraction { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTCompressionPropertyKey_EnableLTR")]
		NSString EnableLtr { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTCompressionPropertyKey_MaxAllowedFrameQP")]
		NSString MaxAllowedFrameQP { get; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
		[Field ("kVTCompressionPropertyKey_MinAllowedFrameQP")]
		NSString MinAllowedFrameQP { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTCompressionPropertyKey_SupportsBaseFrameQP")]
		NSString SupportsBaseFrameQP { get; }

		[Watch (8, 5), TV (15, 4), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Field ("kVTCompressionPropertyKey_OutputBitDepth")]
		NSString OutputBitDepth { get; }
	}

	[iOS (13, 0), TV (13, 0)]
	[MacCatalyst (13, 1)]
	enum VTAlphaChannelMode {
		[Field ("kVTAlphaChannelMode_StraightAlpha")]
		StraightAlpha,
		[DefaultEnumValue]
		[Field ("kVTAlphaChannelMode_PremultipliedAlpha")]
		PremultipliedAlpha,
	}

	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Export ("AllowOpenGop")]
		bool AllowOpenGop { get; set; }

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

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), Watch (9, 0), TV (16, 0)]
		[Export ("PreserveAlphaChannel")]
		bool PreserveAlphaChannel { get; set; }

		[Export ("MaxFrameDelayCount")]
		int MaxFrameDelayCount { get; set; }

		[Export ("MaxH264SliceBytes")]
		int MaxH264SliceBytes { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("RealTime")]
		bool RealTime { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("MaximizePowerEfficiency")]
		bool MaximizePowerEfficiency { get; set; }

		[Export ("SourceFrameCount")]
		uint SourceFrameCount { get; set; }

		[Export ("ExpectedFrameRate")]
		double ExpectedFrameRate { get; set; }

		[Export ("ExpectedDuration")]
		double ExpectedDuration { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("BaseLayerFrameRate")]
		double BaseLayerFrameRate { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), Watch (9, 0), TV (16, 0)]
		[Export ("ReferenceBufferCount")]
		long ReferenceBufferCount { get; }

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

		[MacCatalyst (13, 1)]
		[Export ("MasteringDisplayColorVolume")]
		NSData MasteringDisplayColorVolume { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("ContentLightLevelInfo")]
		NSData ContentLightLevelInfo { get; set; }

		[Export ("PixelTransferProperties")]
		NSDictionary PixelTransferProperties { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("EncoderId")]
		string EncoderId { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("TargetQualityForAlpha")]
		float TargetQualityForAlpha { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("GammaLevel")]
		double GammaLevel { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("UsingGpuRegistryId")]
		uint UsingGpuRegistryId { get; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
		[Export ("ConstantBitRate")]
		long ConstantBitRate { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), Watch (9, 0), TV (16, 0)]
		[Export ("EstimatedAverageBytesPerFrame")]
		long EstimatedAverageBytesPerFrame { get; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		[Export ("PreserveDynamicHdrMetadata")]
		bool PreserveDynamicHdrMetadata { get; set; }

		[TV (14, 5)]
		[iOS (14, 5)]
		[NoWatch]
		[MacCatalyst (14, 5)]
		[Export ("EnableLowLatencyRateControl")]
		bool EnableLowLatencyRateControl { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("BaseLayerBitRateFraction")]
		float BaseLayerBitRateFraction { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("EnableLtr")]
		bool EnableLtr { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("MaxAllowedFrameQP")]
		uint MaxAllowedFrameQP { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, TV (16, 0)]
		[Export ("MinAllowedFrameQP")]
		uint MinAllowedFrameQP { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0), NoWatch]
		[Export ("SupportsBaseFrameQP")]
		bool SupportsBaseFrameQP { get; }

		[Watch (8, 5), TV (15, 4), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("OutputBitDepth")]
		bool OutputBitDepth { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface VTProfileLevelKeys {
		// HEVC

		[MacCatalyst (13, 1)]
		[Field ("kVTProfileLevel_HEVC_Main_AutoLevel")]
		NSString Hevc_Main_AutoLevel { get; }

		[MacCatalyst (13, 1)]
		[Field ("kVTProfileLevel_HEVC_Main10_AutoLevel")]
		NSString Hevc_Main10_AutoLevel { get; }

		[TV (15, 4), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Field ("kVTProfileLevel_HEVC_Main42210_AutoLevel")]
		NSString Hevc_Main42210_AutoLevel { get; }

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
		[MacCatalyst (13, 1)]
		NSString H264_Baseline_4_0 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_4_1")]
		NSString H264_Baseline_4_1 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_4_2")]
		[MacCatalyst (13, 1)]
		NSString H264_Baseline_4_2 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_5_0")]
		[MacCatalyst (13, 1)]
		NSString H264_Baseline_5_0 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_5_1")]
		[MacCatalyst (13, 1)]
		NSString H264_Baseline_5_1 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_5_2")]
		[MacCatalyst (13, 1)]
		NSString H264_Baseline_5_2 { get; }

		[Field ("kVTProfileLevel_H264_Baseline_AutoLevel")]
		[MacCatalyst (13, 1)]
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
		[MacCatalyst (13, 1)]
		NSString H264_Main_4_2 { get; }

		[Field ("kVTProfileLevel_H264_Main_5_0")]
		NSString H264_Main_5_0 { get; }

		[Field ("kVTProfileLevel_H264_Main_5_1")]
		[MacCatalyst (13, 1)]
		NSString H264_Main_5_1 { get; }

		[Field ("kVTProfileLevel_H264_Main_5_2")]
		[MacCatalyst (13, 1)]
		NSString H264_Main_5_2 { get; }

		[Field ("kVTProfileLevel_H264_Main_AutoLevel")]
		[MacCatalyst (13, 1)]
		NSString H264_Main_AutoLevel { get; }

		[Field ("kVTProfileLevel_H264_Extended_5_0")]
		NSString H264_Extended_5_0 { get; }

		[Field ("kVTProfileLevel_H264_Extended_AutoLevel")]
		[MacCatalyst (13, 1)]
		NSString H264_Extended_AutoLevel { get; }

		[Field ("kVTProfileLevel_H264_High_3_0")]
		[MacCatalyst (13, 1)]
		NSString H264_High_3_0 { get; }

		[Field ("kVTProfileLevel_H264_High_3_1")]
		[MacCatalyst (13, 1)]
		NSString H264_High_3_1 { get; }

		[Field ("kVTProfileLevel_H264_High_3_2")]
		[MacCatalyst (13, 1)]
		NSString H264_High_3_2 { get; }

		[Field ("kVTProfileLevel_H264_High_4_0")]
		[MacCatalyst (13, 1)]
		NSString H264_High_4_0 { get; }

		[Field ("kVTProfileLevel_H264_High_4_1")]
		[MacCatalyst (13, 1)]
		NSString H264_High_4_1 { get; }

		[Field ("kVTProfileLevel_H264_High_4_2")]
		[MacCatalyst (13, 1)]
		NSString H264_High_4_2 { get; }

		[Field ("kVTProfileLevel_H264_High_5_0")]
		NSString H264_High_5_0 { get; }

		[Field ("kVTProfileLevel_H264_High_5_1")]
		[MacCatalyst (13, 1)]
		NSString H264_High_5_1 { get; }

		[Field ("kVTProfileLevel_H264_High_5_2")]
		[MacCatalyst (13, 1)]
		NSString H264_High_5_2 { get; }

		[Field ("kVTProfileLevel_H264_High_AutoLevel")]
		[MacCatalyst (13, 1)]
		NSString H264_High_AutoLevel { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTProfileLevel_H264_ConstrainedBaseline_AutoLevel")]
		NSString H264_ConstrainedBaseline_AutoLevel { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTProfileLevel_H264_ConstrainedHigh_AutoLevel")]
		NSString H264_ConstrainedHigh_AutoLevel { get; }

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
	[MacCatalyst (13, 1)]
	interface VTH264EntropyModeKeys {
		[Field ("kVTH264EntropyMode_CAVLC")]
		NSString CAVLC { get; }

		[Field ("kVTH264EntropyMode_CABAC")]
		NSString CABAC { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTVideoEncoderSpecificationKeys")]
	interface VTVideoEncoderSpecification {

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("EnableHardwareAcceleratedVideoEncoder")]
		bool EnableHardwareAcceleratedVideoEncoder { get; set; }

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("RequireHardwareAcceleratedVideoEncoder")]
		bool RequireHardwareAcceleratedVideoEncoder { get; set; }

		[Export ("EncoderID")]
		string EncoderID { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("RequiredEncoderGpuRegistryId")]
		uint RequiredEncoderGpuRegistryId { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("PreferredEncoderGpuRegistryId")]
		uint PreferredEncoderGpuRegistryId { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface VTVideoEncoderSpecificationKeys {

		[Field ("kVTVideoEncoderSpecification_EnableHardwareAcceleratedVideoEncoder")]
		[NoiOS, NoTV]
		[NoMacCatalyst]
		NSString EnableHardwareAcceleratedVideoEncoder { get; }

		[Field ("kVTVideoEncoderSpecification_RequireHardwareAcceleratedVideoEncoder")]
		[NoiOS, NoTV]
		[NoMacCatalyst]
		NSString RequireHardwareAcceleratedVideoEncoder { get; }

		[Field ("kVTVideoEncoderSpecification_EncoderID")]
		NSString EncoderID { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTVideoEncoderSpecification_RequiredEncoderGPURegistryID")]
		NSString RequiredEncoderGpuRegistryId { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTVideoEncoderSpecification_PreferredEncoderGPURegistryID")]
		NSString PreferredEncoderGpuRegistryId { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTEncodeFrameOptionKey")]
	interface VTEncodeFrameOptions {

		[Export ("ForceKeyFrame")]
		bool ForceKeyFrame { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface VTEncodeFrameOptionKey {
		// Per-frame configuration

		[Field ("kVTEncodeFrameOptionKey_ForceKeyFrame")]
		NSString ForceKeyFrame { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTEncodeFrameOptionKey_AcknowledgedLTRTokens")]
		NSString AcknowledgedLtrTokens { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTEncodeFrameOptionKey_BaseFrameQP")]
		NSString BaseFrameQP { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTEncodeFrameOptionKey_ForceLTRRefresh")]
		NSString ForceLtrRefresh { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTSampleAttachmentKey_RequireLTRAcknowledgementToken")]
		NSString RequireLtrAcknowledgementToken { get; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface VTDecompressionPropertyKey {
		// Pixel buffer pools

		[Field ("kVTDecompressionPropertyKey_PixelBufferPool")]
		NSString PixelBufferPool { get; }

		[Field ("kVTDecompressionPropertyKey_PixelBufferPoolIsShared")]
		NSString PixelBufferPoolIsShared { get; }

		[Field ("kVTDecompressionPropertyKey_OutputPoolRequestedMinimumBufferCount")]
		[MacCatalyst (13, 1)]
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
		[MacCatalyst (13, 1)]
		NSString UsingHardwareAcceleratedVideoDecoder { get; }

		// Decoder behavior

		[Field ("kVTDecompressionPropertyKey_RealTime")]
		[MacCatalyst (13, 1)]
		NSString RealTime { get; }

		[Field ("kVTDecompressionPropertyKey_MaximizePowerEfficiency")]
		[MacCatalyst (13, 1)]
		NSString MaximizePowerEfficiency { get; }

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

		[MacCatalyst (13, 1)]
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

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTDecompressionPropertyKey_UsingGPURegistryID")]
		NSString UsingGpuRegistryId { get; }

		//Post-decompression processing

		[Field ("kVTDecompressionPropertyKey_PixelTransferProperties")]
		NSString PixelTransferProperties { get; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		[Field ("kVTDecompressionPropertyKey_PropagatePerFrameHDRDisplayMetadata")]
		NSString PropagatePerFrameHdrDisplayMetadata { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTDecompressionPropertyKey")]
	interface VTDecompressionProperties {

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
		[MacCatalyst (13, 1)]
		bool UsingHardwareAcceleratedVideoDecoder { get; }

		[MacCatalyst (13, 1)]
		[Export ("RealTime")]
		bool RealTime { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("MaximizePowerEfficiency")]
		bool MaximizePowerEfficiency { get; set; }

		[Export ("ThreadCount")]
		uint ThreadCount { get; set; }

		[StrongDictionary]
		[Export ("ReducedResolutionDecode")]
		VTDecompressionResolutionOptions ReducedResolutionDecode { get; set; }

		[Export ("ReducedCoefficientDecode")]
		uint ReducedCoefficientDecode { get; set; }

		[Export ("ReducedFrameDelivery")]
		float ReducedFrameDelivery { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("TemporalLevelLimit")]
		int TemporalLevelLimit { get; set; }

		[Export ("SuggestedQualityOfServiceTiers")]
		NSDictionary [] SuggestedQualityOfServiceTiers { get; }

		[Export ("SupportedPixelFormatsOrderedByQuality")]
		CMPixelFormat [] SupportedPixelFormatsOrderedByQuality { get; }

		[Export ("SupportedPixelFormatsOrderedByPerformance")]
		CMPixelFormat [] SupportedPixelFormatsOrderedByPerformance { get; }

		[Export ("PixelFormatsWithReducedResolutionSupport")]
		CMPixelFormat [] PixelFormatsWithReducedResolutionSupport { get; }

		[Advice ("Use Strongly typed version PixelTransferSettings")]
		[Export ("PixelTransferProperties")]
		NSDictionary PixelTransferProperties { get; set; }

		// VTPixelTransferProperties are available in iOS 9 radar://22614931 https://trello.com/c/bTl6hRu9
		[StrongDictionary]
		[MacCatalyst (13, 1)]
		[Export ("PixelTransferProperties")]
		VTPixelTransferProperties PixelTransferSettings { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("UsingGpuRegistryId")]
		uint UsingGpuRegistryId { get; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		[Export ("PropagatePerFrameHdrDisplayMetadata")]
		bool PropagatePerFrameHhrDisplayMetadata { get; set; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTVideoDecoderSpecificationKeys")]
	interface VTVideoDecoderSpecification {
		[Export ("EnableHardwareAcceleratedVideoDecoder")]
		bool EnableHardwareAcceleratedVideoDecoder { get; set; }

		[Export ("RequireHardwareAcceleratedVideoDecoder")]
		bool RequireHardwareAcceleratedVideoDecoder { get; set; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("RequiredDecoderGpuRegistryId")]
		NSNumber RequiredDecoderGpuRegistryId { get; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("PreferredDecoderGpuRegistryId")]
		NSNumber PreferredDecoderGpuRegistryId { get; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface VTVideoDecoderSpecificationKeys {
		[Field ("kVTVideoDecoderSpecification_EnableHardwareAcceleratedVideoDecoder")]
		NSString EnableHardwareAcceleratedVideoDecoder { get; }

		[Field ("kVTVideoDecoderSpecification_RequireHardwareAcceleratedVideoDecoder")]
		NSString RequireHardwareAcceleratedVideoDecoder { get; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Field ("kVTVideoDecoderSpecification_RequiredDecoderGPURegistryID")]
		NSString RequiredDecoderGpuRegistryId { get; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Field ("kVTVideoDecoderSpecification_PreferredDecoderGPURegistryID")]
		NSString PreferredDecoderGpuRegistryId { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTDecompressionResolutionKeys")]
	interface VTDecompressionResolutionOptions {
		[Export ("Width")]
		float Width { get; set; }

		[Export ("Height")]
		float Height { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface VTDecompressionResolutionKeys {
		[Field ("kVTDecompressionResolutionKey_Width")]
		NSString Width { get; }

		[Field ("kVTDecompressionResolutionKey_Height")]
		NSString Height { get; }
	}

	// VTSession.h
	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTPropertyKeys")]
	interface VTPropertyOptions {
		[Export ("ShouldBeSerialized")]
		bool ShouldBeSerialized { get; set; }

		[Export ("SupportedValueMinimumKey")]
		NSNumber SupportedValueMinimum { get; set; }

		[Export ("SupportedValueMaximumKey")]
		NSNumber SupportedValueMaximum { get; set; }

		[Export ("SupportedValueListKey")]
		NSNumber [] SupportedValueList { get; set; }

		[Export ("DocumentationKey")]
		NSString Documentation { get; set; }
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[Static]
	interface VTPropertyTypeKeys {
		[Field ("kVTPropertyType_Boolean")]
		NSString Boolean { get; }

		[Field ("kVTPropertyType_Enumeration")]
		NSString Enumeration { get; }

		[Field ("kVTPropertyType_Number")]
		NSString Number { get; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface VTPropertyReadWriteStatusKeys {
		[Field ("kVTPropertyReadWriteStatus_ReadOnly")]
		NSString ReadOnly { get; }

		[Field ("kVTPropertyReadWriteStatus_ReadWrite")]
		NSString ReadWrite { get; }
	}

	// VTVideoEncoderList.h
	[MacCatalyst (13, 1)]
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

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTVideoEncoderList_GPURegistryID")]
		NSString GpuRegistryId { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTVideoEncoderList_SupportedSelectionProperties")]
		NSString SupportedSelectionProperties { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTVideoEncoderList_PerformanceRating")]
		NSString PerformanceRating { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTVideoEncoderList_QualityRating")]
		NSString QualityRating { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTVideoEncoderList_InstanceLimit")]
		NSString InstanceLimit { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTVideoEncoderList_IsHardwareAccelerated")]
		NSString IsHardwareAccelerated { get; }

		[iOS (14, 2)]
		[TV (14, 2)]
		[MacCatalyst (14, 2)]
		[Field ("kVTVideoEncoderList_SupportsFrameReordering")]
		NSString SupportsFrameReordering { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTVideoEncoderListOption_IncludeStandardDefinitionDVEncoders")]
		NSString IncludeStandardDefinitionDVEncoders { get; }

		// VTVideoEncoder.cs should be updated when new constants are added here
		// some are missing https://github.com/xamarin/xamarin-macios/issues/9904
	}

	// VTMultiPassStorage.h
	[MacCatalyst (13, 1)]
	[Static]
	interface VTMultiPassStorageCreationOptionKeys {
		[Field ("kVTMultiPassStorageCreationOption_DoNotDelete")]
		NSString DoNotDelete { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTMultiPassStorageCreationOptionKeys")]
	interface VTMultiPassStorageCreationOptions {
		[Export ("DoNotDelete")]
		bool DoNotDelete { get; set; }
	}

	// VTPixelTransferProperties are available in iOS 9 radar://22614931 https://trello.com/c/bTl6hRu9
	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTPixelTransferPropertyKeys")]
	interface VTPixelTransferProperties {
		[StrongDictionary]
		[Export ("DestinationCleanAperture")]
		AVVideoCleanApertureSettings DestinationCleanAperture { get; set; }

		[StrongDictionary]
		[Export ("DestinationPixelAspectRatio")]
		AVVideoPixelAspectRatioSettings DestinationPixelAspectRatio { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("DestinationICCProfile")]
		NSData DestinationICCProfile { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("RealTime")]
		bool RealTime { get; set; }
	}

	// VTPixelTransferProperties are available in iOS 9 radar://22614931 https://trello.com/c/bTl6hRu9
	[MacCatalyst (13, 1)]
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

		[MacCatalyst (13, 1)]
		[Field ("kVTPixelTransferPropertyKey_DestinationColorPrimaries")]
		NSString DestinationColorPrimaries { get; }

		// DestinationColorPrimaries

		[MacCatalyst (13, 1)]
		[Field ("kVTPixelTransferPropertyKey_DestinationTransferFunction")]
		NSString DestinationTransferFunction { get; }

		// DestinationICCProfile

		[MacCatalyst (13, 1)]
		[Field ("kVTPixelTransferPropertyKey_DestinationICCProfile")]
		NSString DestinationICCProfile { get; }

		// DestinationYCbCrMatrix

		[Field ("kVTPixelTransferPropertyKey_DestinationYCbCrMatrix")]
		NSString DestinationYCbCrMatrix { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTPixelTransferPropertyKey_RealTime")]
		NSString RealTime { get; }
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), Watch (9, 0), TV (16, 0)]
	[StrongDictionary ("VTPixelRotationPropertyKeys")]
	interface VTPixelRotationProperties {
		[Export ("FlipHorizontalOrientation")]
		bool FlipHorizontalOrientation { get; set; }

		[Export ("FlipVerticalOrientation")]
		bool FlipVerticalOrientation { get; set; }
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), Watch (9, 0), TV (16, 0)]
	[Static]
	[Advanced]
	interface VTPixelRotationPropertyKeys {

		// Rotation

		[Field ("kVTPixelRotationPropertyKey_Rotation")]
		NSString Rotation { get; }

		// FlipHorizontalOrientation

		[Field ("kVTPixelRotationPropertyKey_FlipHorizontalOrientation")]
		NSString FlipHorizontalOrientation { get; }

		// FlipVerticalOrientation

		[Field ("kVTPixelRotationPropertyKey_FlipVerticalOrientation")]
		NSString FlipVerticalOrientation { get; }
	}
}
