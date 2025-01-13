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

	/// <summary>A class that encapsulates keys necessary for compression sessions. Used by <see cref="T:VideoToolbox.VTCompressionProperties" /></summary>
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

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
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

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("kVTCompressionPropertyKey_MaximumRealTimeFrameRate")]
		NSString MaximumRealTimeFrameRate { get; }

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

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		[Field ("kVTCompressionPropertyKey_ReferenceBufferCount")]
		NSString ReferenceBufferCount { get; }

		[Mac (14, 4), iOS (17, 4), TV (17, 4), MacCatalyst (17, 4)]
		[Field ("kVTCompressionPropertyKey_CalculateMeanSquaredError")]
		NSString CalculateMeanSquaredError { get; }

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

		[NoiOS, NoTV, NoMacCatalyst, Mac (14, 0)]
		[Field ("kVTCompressionPropertyKey_RecommendedParallelizationLimit")]
		NSString RecommendedParallelizationLimit { get; }

		[NoiOS, NoTV, NoMacCatalyst, Mac (14, 0)]
		[Field ("kVTCompressionPropertyKey_RecommendedParallelizedSubdivisionMinimumFrameCount")]
		NSString RecommendedParallelizedSubdivisionMinimumFrameCount { get; }

		[NoiOS, NoTV, NoMacCatalyst, Mac (14, 0)]
		[Field ("kVTCompressionPropertyKey_RecommendedParallelizedSubdivisionMinimumDuration")]
		NSString RecommendedParallelizedSubdivisionMinimumDuration { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("kVTCompressionPropertyKey_UsingGPURegistryID")]
		NSString UsingGpuRegistryId { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("kVTCompressionPropertyKey_HDRMetadataInsertionMode")]
		NSString HdrMetadataInsertionMode { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("kVTCompressionPropertyKey_PrioritizeEncodingSpeedOverQuality")]
		NSString PrioritizeEncodingSpeedOverQuality { get; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		[Field ("kVTCompressionPropertyKey_ConstantBitRate")]
		NSString ConstantBitRate { get; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
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

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		[Field ("kVTCompressionPropertyKey_MinAllowedFrameQP")]
		NSString MinAllowedFrameQP { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTCompressionPropertyKey_SupportsBaseFrameQP")]
		NSString SupportsBaseFrameQP { get; }

		[TV (15, 4), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Field ("kVTCompressionPropertyKey_OutputBitDepth")]
		NSString OutputBitDepth { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("kVTCompressionPropertyKey_ProjectionKind")]
		NSString ProjectionKind { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("kVTCompressionPropertyKey_ViewPackingKind")]
		NSString ViewPackingKind { get; }

		[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
		[Field ("kVTCompressionPropertyKey_SuggestedLookAheadFrameCount")]
		NSString SuggestedLookAheadFrameCount { get; }

		[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
		[Field ("kVTCompressionPropertyKey_SpatialAdaptiveQPLevel")]
		NSString SpatialAdaptiveQPLevel { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kVTCompressionPropertyKey_MVHEVCVideoLayerIDs")]
		NSString MvHevcVideoLayerIds { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kVTCompressionPropertyKey_MVHEVCViewIDs")]
		NSString MvHevcViewIds { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kVTCompressionPropertyKey_MVHEVCLeftAndRightViewIDs")]
		NSString MvHevcLeftAndRightViewIds { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kVTCompressionPropertyKey_HeroEye")]
		NSString HeroEye { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kVTCompressionPropertyKey_StereoCameraBaseline")]
		NSString StereoCameraBaseline { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kVTCompressionPropertyKey_HorizontalDisparityAdjustment")]
		NSString HorizontalDisparityAdjustment { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kVTCompressionPropertyKey_HasLeftStereoEyeView")]
		NSString HasLeftStereoEyeView { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kVTCompressionPropertyKey_HasRightStereoEyeView")]
		NSString HasRightStereoEyeView { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kVTCompressionPropertyKey_HorizontalFieldOfView")]
		NSString HorizontalFieldOfView { get; }

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

	/// <summary>Strongly typed set of options for compression sessions</summary>
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

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
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

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("MaximumRealTimeFrameRate")]
		double MaximumRealTimeFrameRate { get; }

		[Export ("ExpectedDuration")]
		double ExpectedDuration { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("BaseLayerFrameRate")]
		double BaseLayerFrameRate { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		[Export ("ReferenceBufferCount")]
		long ReferenceBufferCount { get; }

		[Mac (14, 4), iOS (17, 4), TV (17, 4), MacCatalyst (17, 4)]
		[Export ("CalculateMeanSquaredError")]
		bool CalculateMeanSquaredError { get; }

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

		[NoiOS, NoTV, NoMacCatalyst, Mac (14, 0)]
		[Export ("RecommendedParallelizationLimit")]
		int RecommendedParallelizationLimit { get; }

		[NoiOS, NoTV, NoMacCatalyst, Mac (14, 0)]
		[Export ("RecommendedParallelizedSubdivisionMinimumFrameCount")]
		ulong RecommendedParallelizedSubdivisionMinimumFrameCount { get; }

		[NoiOS, NoTV, NoMacCatalyst, Mac (14, 0)]
		[Export ("RecommendedParallelizedSubdivisionMinimumDuration")]
		NSDictionary RecommendedParallelizedSubdivisionMinimumDuration { get; }

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

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		[Export ("ConstantBitRate")]
		long ConstantBitRate { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		[Export ("EstimatedAverageBytesPerFrame")]
		long EstimatedAverageBytesPerFrame { get; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[MacCatalyst (14, 1)]
		[Export ("PreserveDynamicHdrMetadata")]
		bool PreserveDynamicHdrMetadata { get; set; }

		[TV (14, 5)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("EnableLowLatencyRateControl")]
		bool EnableLowLatencyRateControl { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("BaseLayerBitRateFraction")]
		float BaseLayerBitRateFraction { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("EnableLtr")]
		bool EnableLtr { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("MaxAllowedFrameQP")]
		uint MaxAllowedFrameQP { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		[Export ("MinAllowedFrameQP")]
		uint MinAllowedFrameQP { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("SupportsBaseFrameQP")]
		bool SupportsBaseFrameQP { get; }

		[TV (15, 4), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("OutputBitDepth")]
		bool OutputBitDepth { get; set; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("ProjectionKind")]
		CMFormatDescriptionProjectionKind /* NSString */ ProjectionKind { get; }

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("ViewPackingKind")]
		CMFormatDescriptionViewPackingKind /* NSString */ ViewPackingKind { get; }

		[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
		[Export ("SuggestedLookAheadFrameCount")]
		nint /* NSNumber */ SuggestedLookAheadFrameCount { get; }

		[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
		[Export ("SpatialAdaptiveQPLevel")]
		VTQPModulationLevel /* NSNumber */ SpatialAdaptiveQPLevel { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("MvHevcVideoLayerIds")]
		NSNumber [] MvHevcVideoLayerIds { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("MvHevcViewIds")]
		NSNumber [] MvHevcViewIds { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("MvHevcLeftAndRightViewIds")]
		NSNumber [] MvHevcLeftAndRightViewIds { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("HeroEye")]
		string HeroEye { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("StereoCameraBaseline")]
		uint StereoCameraBaseline { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("HorizontalDisparityAdjustment")]
		int HorizontalDisparityAdjustment { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("HasLeftStereoEyeView")]
		bool HasLeftStereoEyeView { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("HasRightStereoEyeView")]
		bool HasRightStereoEyeView { get; }

		[iOS (17, 0), NoTV, MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("HorizontalFieldOfView")]
		uint HorizontalFieldOfView { get; }
	}

	[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
	public enum VTQPModulationLevel {
		Default = -1,
		Disable = 0,
	}

	/// <summary>A class that encapsulates keys necessary by <see cref="T:VideoToolbox.VTProfileLevel" />.</summary>
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

		[Field ("kVTProfileLevel_HEVC_Monochrome_AutoLevel")]
		NSString Hevc_Monochrome_AutoLevel { get; }

		[TV (13, 0), iOS (13, 0), MacCatalyst (13, 0)]
		[Field ("kVTProfileLevel_HEVC_Monochrome10_AutoLevel")]
		NSString Hevc_Monochrome10_AutoLevel { get; }

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

	/// <summary>A class that encapsulates keys necessary by <see cref="T:VideoToolbox.VTH264EntropyMode" />.</summary>
	[Static]
	[MacCatalyst (13, 1)]
	interface VTH264EntropyModeKeys {
		[Field ("kVTH264EntropyMode_CAVLC")]
		NSString CAVLC { get; }

		[Field ("kVTH264EntropyMode_CABAC")]
		NSString CABAC { get; }
	}

	/// <summary>Strongly typed representation of a video encoder.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTVideoEncoderSpecificationKeys")]
	interface VTVideoEncoderSpecification {

		[iOS (17, 4), TV (17, 4), MacCatalyst (17, 4)]
		[Export ("EnableHardwareAcceleratedVideoEncoder")]
		bool EnableHardwareAcceleratedVideoEncoder { get; set; }

		[iOS (17, 4), TV (17, 4), MacCatalyst (17, 4)]
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

	/// <summary>A class that encapsulates keys necessary by <see cref="T:VideoToolbox.VTVideoEncoderSpecification" />.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface VTVideoEncoderSpecificationKeys {

		[Field ("kVTVideoEncoderSpecification_EnableHardwareAcceleratedVideoEncoder")]
		[iOS (17, 4), TV (17, 4), MacCatalyst (17, 4)]
		NSString EnableHardwareAcceleratedVideoEncoder { get; }

		[Field ("kVTVideoEncoderSpecification_RequireHardwareAcceleratedVideoEncoder")]
		[iOS (17, 4), TV (17, 4), MacCatalyst (17, 4)]
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

	/// <summary>Strongly typed set of options.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTEncodeFrameOptionKey")]
	interface VTEncodeFrameOptions {

		[Export ("ForceKeyFrame")]
		bool ForceKeyFrame { get; set; }
	}

	/// <summary>A class that encapsulates keys necessary by <see cref="T:VideoToolbox.VTEncodeFrameOptions" /></summary>
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

	[Static]
	interface VTSampleAttachmentKey {
		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("kVTSampleAttachmentKey_RequireLTRAcknowledgementToken")]
		NSString RequireLtrAcknowledgementToken { get; }

		[iOS (17, 4), TV (17, 4), Mac (14, 4), MacCatalyst (17, 4)]
		[Field ("kVTSampleAttachmentKey_QualityMetrics")]
		NSString QualityMetrics { get; }
	}

	[Static]
	[iOS (17, 4), TV (17, 4), Mac (14, 4), MacCatalyst (17, 4)]
	interface VTSampleAttachmentQualityMetricsKey {
		[Field ("kVTSampleAttachmentQualityMetricsKey_LumaMeanSquaredError")]
		NSString LumaMeanSquaredError { get; }

		[Field ("kVTSampleAttachmentQualityMetricsKey_ChromaBlueMeanSquaredError")]
		NSString ChromaBlueMeanSquaredError { get; }

		[Field ("kVTSampleAttachmentQualityMetricsKey_ChromaRedMeanSquaredError")]
		NSString ChromaRedMeanSquaredError { get; }
	}

	/// <summary>A class that encapsulates keys necessary for decompression sessions. Used by <see cref="T:VideoToolbox.VTDecompressionProperties" /></summary>
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

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
		[Field ("kVTDecompressionPropertyKey_GeneratePerFrameHDRDisplayMetadata")]
		NSString GeneratePerFrameHdrDisplayMetadata { get; }

		[iOS (18, 0), TV (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		[Field ("kVTDecompressionPropertyKey_AllowBitstreamToChangeFrameDimensions")]
		NSString AllowBitstreamToChangeFrameDimensions { get; }

		[NoiOS, NoTV, NoMacCatalyst, Mac (15, 0)]
		[Field ("kVTDecompressionPropertyKey_DecoderProducesRAWOutput")]
		NSString DecoderProducesRawOutput { get; }

		[NoiOS, NoTV, NoMacCatalyst, Mac (15, 0)]
		[Field ("kVTDecompressionPropertyKey_RequestRAWOutput")]
		NSString RequestRawWOutput { get; }

		[iOS (17, 0), NoTV, Mac (14, 0), MacCatalyst (17, 0)]
		[Field ("kVTDecompressionPropertyKey_RequestedMVHEVCVideoLayerIDs")]
		NSString RequestedMvHevcVideoLayerIds { get; }
	}

	/// <summary>Strongly typed set of options for decompression sessions.</summary>
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

		[Export ("GeneratePerFrameHdrDisplayMetadata")]
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
		bool GeneratePerFrameHdrDisplayMetadata { get; }

		[Export ("AllowBitstreamToChangeFrameDimensions")]
		[iOS (18, 0), TV (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
		bool AllowBitstreamToChangeFrameDimensions { get; }

		[Export ("DecoderProducesRawOutput")]
		[NoiOS, NoTV, NoMacCatalyst, Mac (15, 0)]
		bool DecoderProducesRawOutput { get; }

		[Export ("RequestRawWOutput")]
		[NoiOS, NoTV, NoMacCatalyst, Mac (15, 0)]
		bool RequestRawWOutput { get; }

		[iOS (17, 0), NoTV, Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("RequestedMvHevcVideoLayerIds")]
		NSNumber [] RequestedMvHevcVideoLayerIds { get; }
	}

	/// <summary>Strongly typed set of options.</summary>
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

	/// <summary>A class that encapsulates keys necessary by <see cref="T:VideoToolbox.VTVideoDecoderSpecification" />.</summary>
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

	/// <summary>Strongly typed set of options used by <see cref="P:VideoToolbox.VTDecompressionProperties.ReducedResolutionDecode" />.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTDecompressionResolutionKeys")]
	interface VTDecompressionResolutionOptions {
		[Export ("Width")]
		float Width { get; set; }

		[Export ("Height")]
		float Height { get; set; }
	}

	/// <summary>A class that encapsulates keys necessary by <see cref="T:VideoToolbox.VTEncodeFrameOptions" />.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface VTDecompressionResolutionKeys {
		[Field ("kVTDecompressionResolutionKey_Width")]
		NSString Width { get; }

		[Field ("kVTDecompressionResolutionKey_Height")]
		NSString Height { get; }
	}

	// VTSession.h
	/// <summary>Strongly typed set of options.</summary>
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

	/// <summary>A class that encapsulates keys necessary by <see cref="T:VideoToolbox.VTPropertyOptions" />.</summary>
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

	/// <summary>A class that encapsulates keys necessary by <see cref="T:VideoToolbox.VTPropertyType" />.</summary>
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

	/// <summary>A class that encapsulates keys necessary by <see cref="P:VideoToolbox.VTPropertyOptions.ReadWriteStatus" />.</summary>
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
	/// <summary>A class that encapsulates keys necessary by <see cref="T:VideoToolbox.VTMultiPassStorageCreationOptions" /></summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface VTMultiPassStorageCreationOptionKeys {
		[Field ("kVTMultiPassStorageCreationOption_DoNotDelete")]
		NSString DoNotDelete { get; }
	}

	/// <summary>Strongly typed set of options.</summary>
	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTMultiPassStorageCreationOptionKeys")]
	interface VTMultiPassStorageCreationOptions {
		[Export ("DoNotDelete")]
		bool DoNotDelete { get; set; }
	}

	// VTPixelTransferProperties are available in iOS 9 radar://22614931 https://trello.com/c/bTl6hRu9
	/// <summary>Strongly typed set of options used by <see cref="P:VideoToolbox.VTDecompressionProperties.PixelTransferSettings" />.</summary>
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
	/// <summary>A class that encapsulates keys needed by <see cref="T:VideoToolbox.VTPixelTransferProperties" />.</summary>
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

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
	[StrongDictionary ("VTPixelRotationPropertyKeys")]
	interface VTPixelRotationProperties {
		[Export ("FlipHorizontalOrientation")]
		bool FlipHorizontalOrientation { get; set; }

		[Export ("FlipVerticalOrientation")]
		bool FlipVerticalOrientation { get; set; }
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
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

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	public enum VTHdrPerFrameMetadataGenerationHdrFormatType {
		[Field ("kVTHDRPerFrameMetadataGenerationHDRFormatType_DolbyVision")]
		DolbyVision,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Static]
	interface VTHdrPerFrameMetadataGenerationOptionsKey {
		[Field ("kVTHDRPerFrameMetadataGenerationOptionsKey_HDRFormats")]
		NSString HdrFormats { get; }

	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[StrongDictionary ("VTHdrPerFrameMetadataGenerationOptionsKey")]
	interface VTHdrPerFrameMetadataGenerationOptions {
		[Export ("HdrFormats")]
		VTHdrPerFrameMetadataGenerationHdrFormatType HdrFormats { get; set; }
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
	[Static]
	interface VTExtensionPropertiesKey {
		[Field ("kVTExtensionProperties_ExtensionIdentifierKey")]
		NSString ExtensionIdentifier { get; }

		[Field ("kVTExtensionProperties_ExtensionNameKey")]
		NSString ExtensionName { get; }

		[Field ("kVTExtensionProperties_ContainingBundleNameKey")]
		NSString ContainingBundleName { get; }

		[Field ("kVTExtensionProperties_ExtensionURLKey")]
		NSString ExtensionUrl { get; }

		[Field ("kVTExtensionProperties_ContainingBundleURLKey")]
		NSString ContainingBundleUrl { get; }

		[Field ("kVTExtensionProperties_CodecNameKey")]
		NSString CodecName { get; }
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
	[StrongDictionary ("VTExtensionPropertiesKey")]
	interface VTDecoderExtensionProperties {
		[Export ("ExtensionIdentifier")]
		string ExtensionIdentifier { get; set; }

		[Export ("ExtensionName")]
		string ExtensionName { get; set; }

		[Export ("ContainingBundleName")]
		string ContainingBundleName { get; set; }

		[Export ("ExtensionUrl")]
		NSUrl ExtensionUrl { get; set; }

		[Export ("ContainingBundleUrl")]
		NSUrl ContainingBundleUrl { get; set; }

		[Export ("CodecName")]
		string CodecName { get; set; }
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
	[Static]
	interface VTRawProcessingParameterKey {
		[Field ("kVTRAWProcessingParameter_Key")]
		NSString Key { get; }

		[Field ("kVTRAWProcessingParameter_Name")]
		NSString Name { get; }

		[Field ("kVTRAWProcessingParameter_Description")]
		NSString Description { get; }

		[Field ("kVTRAWProcessingParameter_Enabled")]
		NSString Enabled { get; }

		[Field ("kVTRAWProcessingParameter_ValueType")]
		NSString ValueType { get; }

		[Field ("kVTRAWProcessingParameter_ListArray")]
		NSString ListArray { get; }

		[Field ("kVTRAWProcessingParameter_SubGroup")]
		NSString SubGroup { get; }

		[Field ("kVTRAWProcessingParameter_MaximumValue")]
		NSString MaximumValue { get; }

		[Field ("kVTRAWProcessingParameter_MinimumValue")]
		NSString MinimumValue { get; }

		[Field ("kVTRAWProcessingParameter_InitialValue")]
		NSString InitialValue { get; }

		[Field ("kVTRAWProcessingParameter_NeutralValue")]
		NSString NeutralValue { get; }

		[Field ("kVTRAWProcessingParameter_CameraValue")]
		NSString CameraValue { get; }

		[Field ("kVTRAWProcessingParameter_CurrentValue")]
		NSString CurrentValue { get; }
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
	[Static]
	interface VTRawProcessingParameterListElementKey {
		[Field ("kVTRAWProcessingParameterListElement_Label")]
		NSString Label { get; }

		[Field ("kVTRAWProcessingParameterListElement_Description")]
		NSString Description { get; }

		[Field ("kVTRAWProcessingParameterListElement_ListElementID")]
		NSString ListElementId { get; }
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
	[StrongDictionary ("VTRawProcessingParameterListElementKey")]
	interface VTRawProcessingParametersListElement {
		[Export ("Label")]
		string Label { get; set; }

		[Export ("Description")]
		string Description { get; set; }

		[Export ("ListElementId")]
		nint ListElementId { get; set; }
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
	[Static]
	interface VTRawProcessingParameterValueTypeKey {
		[Field ("kVTRAWProcessingParameterValueType_Boolean")]
		NSString Boolean { get; }

		[Field ("kVTRAWProcessingParameterValueType_Integer")]
		NSString Integer { get; }

		[Field ("kVTRAWProcessingParameterValueType_Float")]
		NSString Float { get; }

		[Field ("kVTRAWProcessingParameterValueType_List")]
		NSString List { get; }

		[Field ("kVTRAWProcessingParameterValueType_SubGroup")]
		NSString SubGroup { get; }
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
	[StrongDictionary ("VTRawProcessingParameterValueTypeKey")]
	interface VTRawProcessingParameterValueType {
		[Export ("Boolean")]
		bool Boolean { get; set; }

		[Export ("Integer")]
		int Integer { get; set; }

		[Export ("Float")]
		float Float { get; set; }

		[Export ("List")]
		NSObject [] List { get; set; }

		[Export ("SubGroup")]
		NSDictionary SubGroup { get; set; }
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
	[StrongDictionary ("VTRawProcessingParameterKey")]
	interface VTRawProcessingParameters {
		[Export ("Key")]
		string Key { get; set; }

		[Export ("Name")]
		string Name { get; set; }

		[Export ("Description")]
		string Description { get; set; }

		[Export ("Enabled")]
		bool Enabled { get; set; }

		[Export ("ValueType")]
		VTRawProcessingParameterValueType ValueType { get; set; }

		// FIXME: Generated code doesn't compile
		// [Export ("ListArray")]
		// VTRawProcessingParametersListElement [] ListArray { get; set; }

		// FIXME: Generated code doesn't compile
		// [Export ("SubGroup")]
		// VTRawProcessingParameters[] SubGroup { get; set; }

		[Export ("MaximumValue")]
		NSObject MaximumValue { get; set; }

		[Export ("MinimumValue")]
		NSObject MinimumValue { get; set; }

		[Export ("InitialValue")]
		NSObject InitialValue { get; set; }

		[Export ("NeutralValue")]
		NSObject NeutralValue { get; set; }

		[Export ("CameraValue")]
		NSObject CameraValue { get; set; }

		[Export ("CurrentValue")]
		NSObject CurrentValue { get; set; }
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
	[Static]
	interface VTRawProcessingPropertyKey {
		[Field ("kVTRAWProcessingPropertyKey_MetalDeviceRegistryID")]
		NSString MetalDeviceRegistryId { get; }

		[Field ("kVTRAWProcessingPropertyKey_OutputColorAttachments")]
		NSString OutputColorAttachments { get; }
	}

	[NoTV, NoiOS, NoMacCatalyst, Mac (15, 0)]
	[StrongDictionary ("VTRawProcessingPropertyKey", Suffix = "")]
	interface VTRawProcessingProperty {
		ulong MetalDeviceRegistryId { get; set; }

		NSDictionary OutputColorAttachments { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("VTSampleAttachmentKey", Suffix = "")]
	interface VTSampleAttachments {
		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		NSNumber RequireLtrAcknowledgementToken { get; set; }

		[iOS (17, 4), TV (17, 4), Mac (14, 4), MacCatalyst (17, 4)]
		VTSampleAttachmentQualityMetrics QualityMetrics { get; }
	}

	[StrongDictionary ("VTSampleAttachmentQualityMetricsKey", Suffix = "")]
	[iOS (17, 4), TV (17, 4), Mac (14, 4), MacCatalyst (17, 4)]
	interface VTSampleAttachmentQualityMetrics {
		// This can be either CFNumber or CFArray, so we have to bind it as NSObject
		NSObject LumaMeanSquaredError { get; }

		// This can be either CFNumber or CFArray, so we have to bind it as NSObject
		NSObject ChromaBlueMeanSquaredError { get; }

		// This can be either CFNumber or CFArray, so we have to bind it as NSObject
		NSString ChromaRedMeanSquaredError { get; }
	}
}
