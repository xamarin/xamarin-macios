//
// VideoToolbox core enumerations
//
// Authors: 
// 		Miguel de Icaza (miguel@xamarin.com)
//		Alex Soto (alex.soto@xamarin.com)
//
// Copyright 2014 Xamarin Inc
//

using System;
using Foundation;
using ObjCRuntime;

namespace VideoToolbox {

	// untyped enum -> VTErrors.h
	/// <summary>Possible VideoToolbox API errors.</summary>
	public enum VTStatus {
		Ok,
		PropertyNotSupported = -12900,
		PropertyReadOnly = -12901,
		Parameter = -12902,
		InvalidSession = -12903,
		AllocationFailed = -12904,
		PixelTransferNotSupported = -12905,
		CouldNotFindVideoDecoder = -12906,
		CouldNotCreateInstance = -12907,
		CouldNotFindVideoEncoder = -12908,
		VideoDecoderBadData = -12909,
		VideoDecoderUnsupportedDataFormat = -12910,
		VideoDecoderMalfunction = -12911,
		VideoEncoderMalfunction = -12912,
		VideoDecoderNotAvailableNow = -12913,
		[Obsolete ("Use PixelRotationNotSupported enum value instead.")]
		ImageRotationNotSupported = -12914,
		PixelRotationNotSupported = -12914,
		VideoEncoderNotAvailableNow = -12915,
		FormatDescriptionChangeNotSupported = -12916,
		InsufficientSourceColorData = -12917,
		CouldNotCreateColorCorrectionData = -12918,
		ColorSyncTransformConvertFailed = -12919,
		VideoDecoderAuthorization = -12210,
		VideoEncoderAuthorization = -12211,
		ColorCorrectionPixelTransferFailed = -12212,
		MultiPassStorageIdentifierMismatch = -12913,
		MultiPassStorageInvalid = -12214,
		FrameSiloInvalidTimeStamp = -12215,
		FrameSiloInvalidTimeRange = -12216,
		CouldNotFindTemporalFilter = -12217,
		PixelTransferNotPermitted = -12218,
		ColorCorrectionImageRotationFailed = -12219,
		VideoDecoderRemoved = -17690,
		SessionMalfunction = -17691,
		[MacCatalyst (13, 1)]
		DecoderNeedsRosetta = -17692,
		[MacCatalyst (13, 1)]
		EncoderNeedsRosetta = -17693,
		[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		VideoDecoderReferenceMissing = -17694,
		[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		VideoDecoderCallbackMessaging = -17695,
		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
		VideoDecoderUnknownErr = -17696,
		ExtensionDisabledErr = -17697,
		VideoEncoderMVHEVCVideoLayerIDsMismatchErr = -17698,
		CouldNotOutputTaggedBufferGroupErr = -17699,
		CouldNotFindExtensionErr = -19510,
		ExtensionConflictErr = -19511,
	}

	// uint32_t -> VTErrors.h
	/// <summary>Flags to control encoder in a decompression session</summary>
	[Flags]
	public enum VTDecodeFrameFlags : uint {
		/// <summary>To be added.</summary>
		EnableAsynchronousDecompression = 1 << 0,
		/// <summary>To be added.</summary>
		DoNotOutputFrame = 1 << 1,
		/// <summary>To be added.</summary>
		OneTimeRealTimePlayback = 1 << 2,
		/// <summary>To be added.</summary>
		EnableTemporalProcessing = 1 << 3,
	}

	// UInt32 -> VTErrors.h
	/// <summary>Flags for status reporting in decoding sessions.</summary>
	[Flags]
	public enum VTDecodeInfoFlags : uint {
		Asynchronous = 1 << 0,
		FrameDropped = 1 << 1,
		ImageBufferModifiable = 1 << 2,
		SkippedLeadingFrameDropped = 1 << 3,
	}

	// UInt32 -> VTErrors.h
	/// <summary>Encoding status flags.</summary>
	[Flags]
	public enum VTEncodeInfoFlags : uint {
		/// <summary>To be added.</summary>
		Asynchronous = 1 << 0,
		/// <summary>To be added.</summary>
		FrameDropped = 1 << 1
	}

	// uint32_t -> VTCompressionSession.h
	/// <summary>Flags to control encoder in multi pass compression sessions</summary>
	[Flags]
	public enum VTCompressionSessionOptionFlags : uint {
		BeginFinalPass = 1 << 0
	}

	// Strongly Typed VTProfileLevelKey
	/// <summary>Strongly typed set of options used by <see cref="P:VideoToolbox.VTCompressionProperties.ProfileLevel" />.</summary>
	public enum VTProfileLevel {
		/// <summary>The protocol, profile, and level have not been set.</summary>
		Unset,
		/// <summary>The H.264 compression format, baseline profile, level 1.3.</summary>
		H264Baseline13,
		/// <summary>The H.264 compression format, baseline profile, level 3.0.</summary>
		H264Baseline30,
		/// <summary>The H.264 compression format, baseline profile, level 3.1.</summary>
		H264Baseline31,
		/// <summary>The H.264 compression format, baseline profile, level 3.2.</summary>
		H264Baseline32,
		/// <summary>The H.264 compression format, baseline profile, level 4.0.</summary>
		H264Baseline40,
		/// <summary>The H.264 compression format, baseline profile, level 4.1.</summary>
		H264Baseline41,
		/// <summary>The H.264 compression format, baseline profile, level 4.2.</summary>
		H264Baseline42,
		/// <summary>The H.264 compression format, baseline profile, level 5.0.</summary>
		H264Baseline50,
		/// <summary>The H.264 compression format, baseline profile, level 5.1.</summary>
		H264Baseline51,
		/// <summary>The H.264 compression format, baseline profile, level 5.2.</summary>
		H264Baseline52,
		/// <summary>The H.264 compression format, baseline profile. Determine the level automatically.</summary>
		H264BaselineAutoLevel,
		/// <summary>The H.264 compression format, main profile, level 3.0.</summary>
		H264Main30,
		/// <summary>The H.264 compression format, main profile, level 3.1.</summary>
		H264Main31,
		H264Main32,
		H264Main40,
		/// <summary>The H.264 compression format, main profile, level 4.1.</summary>
		H264Main41,
		H264Main42,
		H264Main50,
		/// <summary>The H.264 compression format, main profile, level 5.1.</summary>
		H264Main51,
		/// <summary>The H.264 compression format, main profile, level 5.2.</summary>
		H264Main52,
		H264MainAutoLevel,
		H264Extended50,
		H264ExtendedAutoLevel,
		H264High30,
		H264High31,
		H264High32,
		H264High40,
		H264High41,
		H264High42,
		H264High50,
		H264High51,
		H264High52,
		H264HighAutoLevel,
		MP4VSimpleL0,
		MP4VSimpleL1,
		MP4VSimpleL2,
		MP4VSimpleL3,
		MP4VMainL2,
		MP4VMainL3,
		MP4VMainL4,
		MP4VAdvancedSimpleL0,
		MP4VAdvancedSimpleL1,
		MP4VAdvancedSimpleL2,
		MP4VAdvancedSimpleL3,
		MP4VAdvancedSimpleL4,
		H263Profile0Level10,
		H263Profile0Level45,
		H263Profile3Level45,
		[MacCatalyst (13, 1)]
		HevcMainAutoLevel,
		[MacCatalyst (13, 1)]
		HevcMain10AutoLevel,
	}

	// Strongly Typed VTH264EntropyModeKeys
	/// <summary>Strongly typed set of options used by <see cref="P:VideoToolbox.VTCompressionProperties.H264EntropyMode" />.</summary>
	public enum VTH264EntropyMode {
		/// <summary>To be added.</summary>
		Unset,
		/// <summary>To be added.</summary>
		Cavlc,
		/// <summary>To be added.</summary>
		Cabac
	}

	// Strongly Typed kVTCompressionPropertyKey_FieldCount
	/// <summary>Frame encoding options.</summary>
	public enum VTFieldCount {
		/// <summary>To be added.</summary>
		Progressive = 1,
		/// <summary>To be added.</summary>
		Interlaced = 2
	}

	// Strongly Typed kVTCompressionPropertyKey_FieldDetail
	/// <summary>Field ordering options.</summary>
	public enum VTFieldDetail {
		/// <summary>To be added.</summary>
		Unset,
		/// <summary>To be added.</summary>
		TemporalTopFirst,
		/// <summary>To be added.</summary>
		TemporalBottomFirst,
		/// <summary>To be added.</summary>
		SpatialFirstLineEarly,
		/// <summary>To be added.</summary>
		SpatialFirstLineLate
	}

	// Strongly Typed kVTCompressionPropertyKey_ColorPrimaries
	/// <summary>Represents color primaries in compressed sample buffers</summary>
	public enum VTColorPrimaries {
		/// <summary>To be added.</summary>
		Unset,
		/// <summary>To be added.</summary>
		ItuR7092,
		/// <summary>To be added.</summary>
		Ebu3213,
		/// <summary>To be added.</summary>
		SmpteC,
		/// <summary>To be added.</summary>
		P22
	}

	// Strongly Typed kVTCompressionPropertyKey_TransferFunction
	/// <summary>Strongly typed set of options used by <see cref="P:VideoToolbox.VTCompressionProperties.TransferFunction" />.</summary>
	public enum VTTransferFunction {
		Unset,
		ItuR7092,
		Smpte240M1955,
		UseGamma
	}

	// Strongly Typed kVTCompressionPropertyKey_YCbCrMatrix
	/// <summary>Colorimetry for compressed content.</summary>
	public enum VTYCbCrMatrix {
		Unset,
		ItuR7092,
		ItuR6014,
		Smpte240M1955
	}

	// Strongly Typed kVTDecompressionPropertyKey_FieldMode
	/// <summary>Decoder field mode options.</summary>
	public enum VTFieldMode {
		/// <summary>To be added.</summary>
		Unset,
		/// <summary>To be added.</summary>
		BothFields,
		/// <summary>To be added.</summary>
		TopFieldOnly,
		/// <summary>To be added.</summary>
		BottomFieldOnly,
		/// <summary>To be added.</summary>
		SingleField,
		/// <summary>To be added.</summary>
		DeinterlaceFields
	}

	// Strongly Typed kVTDecompressionPropertyKey_DeinterlaceMode
	/// <summary>Deinterlacing techniques.</summary>
	public enum VTDeinterlaceMode {
		Unset,
		VerticalFilter,
		Temporal
	}

	// Strongly Typed kVTDecompressionPropertyKey_OnlyTheseFrames
	/// <summary>Strongly typed set of options used by <see cref="P:VideoToolbox.VTDecompressionProperties.OnlyTheseFrames" />.</summary>
	public enum VTOnlyTheseFrames {
		/// <summary>To be added.</summary>
		Unset,
		/// <summary>To be added.</summary>
		AllFrames,
		/// <summary>To be added.</summary>
		NonDroppableFrames,
		/// <summary>To be added.</summary>
		IFrames,
		/// <summary>To be added.</summary>
		KeyFrames
	}

	// Strongly Typed kVTPropertyTypeKey
	/// <summary>Strongly typed set of options.</summary>
	public enum VTPropertyType {
		/// <summary>To be added.</summary>
		Unset,
		/// <summary>To be added.</summary>
		Enumeration,
		/// <summary>To be added.</summary>
		Boolean,
		/// <summary>To be added.</summary>
		Number
	}

	// Strongly Typed kVTPropertyReadWriteStatusKey
	/// <summary>Strongly typed set of options used by <see cref="P:VideoToolbox.VTPropertyOptions.ReadWriteStatus" />.</summary>
	public enum VTReadWriteStatus {
		Unset,
		ReadOnly,
		ReadWrite
	}

	public struct VTDataRateLimit {
		public uint NumberOfBytes { get; set; }
		public double Seconds { get; set; }

		public VTDataRateLimit (uint numberOfBytes, double seconds) : this ()
		{
			NumberOfBytes = numberOfBytes;
			Seconds = seconds;
		}
	}

	/// <summary>Image scaling options.</summary>
	[MacCatalyst (13, 1)]
	public enum VTScalingMode {
		Unset,
		Normal,
		CropSourceToCleanAperture,
		Letterbox,
		Trim
	}

	/// <summary>Chroma downsampling techniques.</summary>
	[MacCatalyst (13, 1)]
	public enum VTDownsamplingMode {
		/// <summary>To be added.</summary>
		Unset,
		/// <summary>To be added.</summary>
		Decimate,
		/// <summary>To be added.</summary>
		Average
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum HdrMetadataInsertionMode {
		[Field ("kVTHDRMetadataInsertionMode_None")]
		None,
		[Field ("kVTHDRMetadataInsertionMode_Auto")]
		Auto,
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), TV (16, 0)]
	public enum VTRotation {
		[DefaultEnumValue]
		[Field ("kVTRotation_0")]
		Zero,
		[Field ("kVTRotation_CW90")]
		ClockwiseNinety,
		[Field ("kVTRotation_180")]
		OneHundredAndEighty,
		[Field ("kVTRotation_CCW90")]
		CounterclockwiseNinety,
	}
}
