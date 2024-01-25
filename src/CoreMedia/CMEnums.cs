// 
// CMEnums.cs: Enumerations for CoreMedia
//
// Authors: Mono Team
//          Marek Safar (marek.safar@gmail.com)
//
// Copyright 2010-2011 Novell Inc
// Copyright 2012-2014 Xamarin Inc
//
using System;
using System.Runtime.InteropServices;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace CoreMedia {

	// FourCharCode -> CMFormatDescription.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMMediaType : uint {
		Video = 1986618469, // 'vide'
		Audio = 1936684398, // 'soun'
		Muxed = 1836415096, // 'muxx'
		Text = 1952807028, // 'text'
		ClosedCaption = 1668047728, // 'clcp'
		Subtitle = 1935832172, // 'sbtl'
		TimeCode = 1953325924, // 'tmcd'
							   // note: the 4CC was obsoleted, i.e. Metadata is a new 4CC
		Metadata = 0x6D657461, // 'meta'
	}

	// FourCharCode -> CMFormatDescription.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMClosedCaptionFormatType : uint {
		CEA608 = 0x63363038, // 'c608',
		CEA708 = 0x63373038, // 'c708',
		ATSC = 0x61746363, // 'atcc'
	}

	// FourCharCode -> CMFormatDescription.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMMuxedStreamType : uint {
		MPEG1System = 0x6D703173, // 'mp1s',
		MPEG2Transport = 0x6D703274, // 'mp2t',
		MPEG2Program = 0x6D703270, // 'mp2p',
		DV = 0x64762020, // 'dv  '
	}

	// FourCharCode -> CMFormatDescription.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMSubtitleFormatType : uint {
		Text3G = 0x74783367, // 'tx3g'
		WebVTT = 0x77767474, // 'wvtt'
	}

	// FourCharCode -> CMFormatDescription.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMMetadataFormatType : uint {
		ICY = 0x69637920, // 'icy '
		ID3 = 0x69643320, // 'id3 '
		Boxed = 0x6d656278, // 'mebx'
		Emsg = 0x656d7367, // 'emsg'
	}

	// FourCharCode -> CMFormatDescription.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMTimeCodeFormatType : uint {
		TimeCode32 = 0x746D6364, // 'tmcd',
		TimeCode64 = 0x74633634, // 'tc64',
		Counter32 = 0x636E3332, // 'cn32',
		Counter64 = 0x636E3634, // 'cn64'
	}

	// uint32_t -> CMTime.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMTimeRoundingMethod : uint {
		RoundHalfAwayFromZero = 1,
		RoundTowardZero = 2,
		RoundAwayFromZero = 3,
		QuickTime = 4,
		RoundTowardPositiveInfinity = 5,
		RoundTowardNegativeInfinity = 6,
		Default = RoundHalfAwayFromZero,
	}

	// FourCharCode -> CMFormatDescription.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMVideoCodecType : uint {
		YUV422YpCbCr8 = 0x32767579,
		Animation = 0x726c6520,
		Cinepak = 0x63766964,
		JPEG = 0x6a706567,
		JPEG_OpenDML = 0x646d6231,
		SorensonVideo = 0x53565131,
		SorensonVideo3 = 0x53565133,
		H263 = 0x68323633,
		H264 = 0x61766331,
		Mpeg4Video = 0x6d703476,
		Mpeg2Video = 0x6d703276,
		Mpeg1Video = 0x6d703176,
		[iOS (14, 0), TV (14, 0), Watch (7, 0), Mac (11, 0)]
		[MacCatalyst (14, 0)]
		VP9 = 0x76703039,
		DvcNtsc = 0x64766320,
		DvcPal = 0x64766370,
		DvcProPal = 0x64767070,
		DvcPro50NTSC = 0x6476356e,
		DvcPro50PAL = 0x64763570,
		DvcProHD720p60 = 0x64766870,
		DvcProHD720p50 = 0x64766871,
		DvcProHD1080i60 = 0x64766836,
		DvcProHD1080i50 = 0x64766835,
		DvcProHD1080p30 = 0x64766833,
		DvcProHD1080p25 = 0x64766832,
		AppleProRes4444 = 0x61703468,
		AppleProRes422HQ = 0x61706368,
		AppleProRes422 = 0x6170636e,
		AppleProRes422LT = 0x61706373,
		AppleProRes422Proxy = 0x6170636f,
		Hevc = 0x68766331,
		[iOS (14, 5)]
		[TV (14, 5)]
		[Watch (7, 4)]
		[Mac (11, 3)]
		[MacCatalyst (14, 5)]
		DolbyVisionHevc = 0x64766831,
		DisparityHevc = 0x64697368,
		DepthHevc = 0x64657068,
	}

	// UInt32 enum => CMFormatDescription.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMPixelFormat : uint {
		AlphaRedGreenBlue32bits = 32,
		BlueGreenRedAlpha32bits = 1111970369,
		RedGreenBlue24bits = 24,
		BigEndian555_16bits = 16,
		BigEndian565_16bits = 1110783541,
		LittleEndian555_16bits = 1278555445,
		LittleEndian565_16bits = 1278555701,
		LittleEndian5551_16bits = 892679473,
		YpCbCr422_8bits = 846624121,
		YpCbCr422yuvs_8bits = 2037741171,
		YpCbCr444_8bits = 1983066168,
		YpCbCrA4444_8bits = 1983131704,
		YpCbCr422_16bits = 1983000886,
		YpCbCr422_10bits = 1983000880,
		YpCbCr444_10bits = 1983131952,
		IndexedGrayWhiteIsZero_8bits = 40,
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMAttachmentMode : uint {
		ShouldNotPropagate = 0,
		ShouldPropagate = 1,
	};

	// untyped enum (used as OSStatus) -> CMBlockBuffer.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMBlockBufferError : int {
		None = 0,
		StructureAllocationFailed = -12700,
		BlockAllocationFailed = -12701,
		BadCustomBlockSource = -12702,
		BadOffsetParameter = -12703,
		BadLengthParameter = -12704,
		BadPointerParameter = -12705,
		EmptyBlockBuffer = -12706,
		UnallocatedBlock = -12707,
		InsufficientSpace = -12708,
	}

	// uint32_t -> CMBlockBuffer.h
	[Flags]
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMBlockBufferFlags : uint {
		AssureMemoryNow = (1 << 0),
		AlwaysCopyData = (1 << 1),
		DontOptimizeDepth = (1 << 2),
		PermitEmptyReference = (1 << 3),
	}

	// untyped enum (uses as OSStatus) -> CMFormatDescription.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMFormatDescriptionError : int {
		None = 0,
		InvalidParameter = -12710,
		AllocationFailed = -12711,
		ValueNotAvailable = -12718,
	}

	// untyped enum (used as an OSStatus) -> CMSampleBuffer.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMSampleBufferError : int {
		None = 0,
		AllocationFailed = -12730,
		RequiredParameterMissing = -12731,
		AlreadyHasDataBuffer = -12732,
		BufferNotReady = -12733,
		SampleIndexOutOfRange = -12734,
		BufferHasNoSampleSizes = -12735,
		BufferHasNoSampleTimingInfo = -12736,
		ArrayTooSmall = -12737,
		InvalidEntryCount = -12738,
		CannotSubdivide = -12739,
		SampleTimingInfoInvalid = -12740,
		InvalidMediaTypeForOperation = -12741,
		InvalidSampleData = -12742,
		InvalidMediaFormat = -12743,
		Invalidated = -12744,
	}

#if !WATCH
	public enum LensStabilizationStatus {
		Active,
		OutOfRange,
		Unavailable,
		Off,
		None,
	}
#endif

	// untyped enum (used as OSStatus) -> CMSync.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMClockError : int {
		None = 0,
		MissingRequiredParameter = -12745,
		InvalidParameter = -12746,
		AllocationFailed = -12747,
		UnsupportedOperation = -12756,
	}

	// untyped enum (used as OSStatus) -> CMSync.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMTimebaseError : int {
		None = 0,
		MissingRequiredParameter = -12748,
		InvalidParameter = -12749,
		AllocationFailed = -12750,
		TimerIntervalTooShort = -12751,
		ReadOnly = -12757,
	}

	// untyped enum (used as OSStatus) -> CMSync.h
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CMSyncError : int {
		None = 0,
		MissingRequiredParameter = -12752,
		InvalidParameter = -12753,
		AllocationFailed = -12754,
		RateMustBeNonZero = -12755,
	}
}
