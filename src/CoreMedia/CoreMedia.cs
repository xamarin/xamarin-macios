// 
// CoreMedia.cs: Basic definitions for CoreMedia
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

namespace CoreMedia {

	// FourCharCode -> CMFormatDescription.h
	public enum CMMediaType : uint
	{
		Video         = 1986618469, // 'vide'
		Audio         = 1936684398, // 'soun'
		Muxed         = 1836415096, // 'muxx'
		Text          = 1952807028, // 'text'
		ClosedCaption = 1668047728, // 'clcp'
		Subtitle      = 1935832172, // 'sbtl'
		TimeCode      = 1953325924, // 'tmcd'
#if !XAMCORE_2_0
		// not part of the header file anymore
		[Obsolete ("Use 'Metadata' instead.")]
		TimedMetadata = 1953326452, // 'tmet'
#endif
		// note: the 4CC was obsoleted, i.e. Metadata is a new 4CC
		Metadata      = 0x6D657461, // 'meta'
	}

	// FourCharCode -> CMFormatDescription.h
	public enum CMClosedCaptionFormatType : uint
	{
		CEA608	= 0x63363038, // 'c608',
		CEA708	= 0x63373038, // 'c708',
		ATSC	= 0x61746363, // 'atcc'
	}

	// FourCharCode -> CMFormatDescription.h
	public enum CMMuxedStreamType : uint
	{
		MPEG1System		= 0x6D703173, // 'mp1s',
		MPEG2Transport	= 0x6D703274, // 'mp2t',
		MPEG2Program	= 0x6D703270, // 'mp2p',
		DV				= 0x64762020, // 'dv  '
	}

	// FourCharCode -> CMFormatDescription.h
	public enum CMSubtitleFormatType : uint
	{
		Text3G  = 0x74783367, // 'tx3g'
		WebVTT  = 0x77767474, // 'wvtt'
	}

	// FourCharCode -> CMFormatDescription.h
	public enum CMMetadataFormatType : uint
	{
		ICY   = 0x69637920, // 'icy '
		ID3   = 0x69643320, // 'id3 '
		Boxed = 0x6d656278, // 'mebx'
	}

	// FourCharCode -> CMFormatDescription.h
	public enum CMTimeCodeFormatType : uint
	{
		TimeCode32	= 0x746D6364, // 'tmcd',
		TimeCode64	= 0x74633634, // 'tc64',
		Counter32	= 0x636E3332, // 'cn32',
		Counter64	= 0x636E3634, // 'cn64'
	}

	// uint32_t -> CMTime.h
	public enum CMTimeRoundingMethod : uint
	{
		RoundHalfAwayFromZero = 1,
		RoundTowardZero = 2,
		RoundAwayFromZero = 3,
		QuickTime = 4,
		RoundTowardPositiveInfinity = 5,
		RoundTowardNegativeInfinity = 6,
		Default = RoundHalfAwayFromZero,
	}

	// CMSampleBuffer.h
	[StructLayout(LayoutKind.Sequential)]
	public struct CMSampleTimingInfo
	{
		public CMTime Duration;
		public CMTime PresentationTimeStamp;
		public CMTime DecodeTimeStamp;
	}

	// CMTimeRange.h
	[StructLayout(LayoutKind.Sequential)]
	public struct CMTimeRange {
		public CMTime Start;
		public CMTime Duration;
#if !COREBUILD
		public static readonly CMTimeRange Zero;

#if !XAMCORE_3_0
		[Obsolete ("Use 'InvalidRange'.")]
		public static readonly CMTimeRange Invalid;
#endif
		public static readonly CMTimeRange InvalidRange;

		[iOS (9,0)][Mac (10,11)]
		public static readonly CMTimeRange InvalidMapping;

		[iOS (9,0)][Mac (10,11)]
		public static NSString TimeMappingSourceKey { get; private set; }

		[iOS (9,0)][Mac (10,11)]
		public static NSString TimeMappingTargetKey { get; private set; }

		static CMTimeRange () {
			var lib = Libraries.CoreMedia.Handle;
			var retZero = Dlfcn.dlsym (lib, "kCMTimeRangeZero");
			Zero = (CMTimeRange)Marshal.PtrToStructure (retZero, typeof(CMTimeRange));

			var retInvalid = Dlfcn.dlsym (lib, "kCMTimeRangeInvalid");
#if !XAMCORE_3_0
			Invalid = (CMTimeRange)Marshal.PtrToStructure (retInvalid, typeof(CMTimeRange));
#endif
			InvalidRange = (CMTimeRange)Marshal.PtrToStructure (retInvalid, typeof(CMTimeRange));

			var retMappingInvalid = Dlfcn.dlsym (lib, "kCMTimeMappingInvalid");
			if (retMappingInvalid  != IntPtr.Zero)
				InvalidMapping = (CMTimeRange)Marshal.PtrToStructure (retMappingInvalid, typeof(CMTimeRange));

			TimeMappingSourceKey = Dlfcn.GetStringConstant (lib, "kCMTimeMappingSourceKey");
			TimeMappingTargetKey = Dlfcn.GetStringConstant (lib, "kCMTimeMappingTargetKey");
		}
#endif // !COREBUILD
	}

	// CMTimeRange.h
	[StructLayout(LayoutKind.Sequential)]
	public struct CMTimeMapping {
		public CMTimeRange Source;
		public CMTimeRange Target;

#if !COREBUILD
		[iOS (9,0)][Mac (10,11)]
		public static CMTimeMapping Create (CMTimeRange source, CMTimeRange target)
		{
			return CMTimeMappingMake (source, target);
		}

		[iOS (9,0)][Mac (10,11)]
		public static CMTimeMapping CreateEmpty (CMTimeRange target)
		{
			return CMTimeMappingMakeEmpty (target);
		}

		[iOS (9,0)][Mac (10,11)]
		public static CMTimeMapping CreateFromDictionary (NSDictionary dict)
		{
			return CMTimeMappingMakeFromDictionary (dict.Handle);
		}

		[iOS (9,0)][Mac (10,11)]
		public NSDictionary AsDictionary ()
		{
			return new NSDictionary (CMTimeMappingCopyAsDictionary (this, IntPtr.Zero), true);
		}

		[iOS (9,0)][Mac (10,11)]
		public string Description
		{
			get
			{
				return (string) new NSString (CMTimeMappingCopyDescription(IntPtr.Zero, this), true);
			}
		}

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTimeMapping CMTimeMappingMake (CMTimeRange source, CMTimeRange target);

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTimeMapping CMTimeMappingMakeEmpty (CMTimeRange target);

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern IntPtr /* CFDictionaryRef* */ CMTimeMappingCopyAsDictionary (CMTimeMapping mapping, IntPtr allocator);

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTimeMapping CMTimeMappingMakeFromDictionary (/* CFDictionaryRef* */ IntPtr dict);

		[iOS (9,0)][Mac (10,11)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern IntPtr /* CFStringRef* */ CMTimeMappingCopyDescription (IntPtr allocator, CMTimeMapping mapping);
#endif // !COREBUILD
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CMTimeScale
	{
		// CMTime.h
		public static readonly CMTimeScale MaxValue = new CMTimeScale (0x7fffffff);

		// int32_t -> CMTime.h
		public int Value;

		public CMTimeScale (int value)
		{
			if (value < 0 || value > 0x7fffffff)
				throw new ArgumentOutOfRangeException ("value");

			this.Value = value;
		}
	}

	// FourCharCode -> CMFormatDescription.h
	public enum CMVideoCodecType : uint
	{
		YUV422YpCbCr8    = 0x32767579,
		Animation        = 0x726c6520,
		Cinepak          = 0x63766964,
		JPEG             = 0x6a706567,
		JPEG_OpenDML     = 0x646d6231,
		SorensonVideo    = 0x53565131,
		SorensonVideo3   = 0x53565133,
		H263             = 0x68323633,
		H264             = 0x61766331,
		Mpeg4Video       = 0x6d703476,
		Mpeg2Video       = 0x6d703276,
		Mpeg1Video       = 0x6d703176,
		DvcNtsc          = 0x64766320,
		DvcPal           = 0x64766370,
		DvcProPal        = 0x64767070,
		DvcPro50NTSC     = 0x6476356e,
		DvcPro50PAL      = 0x64763570,
		DvcProHD720p60   = 0x64766870,
		DvcProHD720p50   = 0x64766871,
		DvcProHD1080i60  = 0x64766836,
		DvcProHD1080i50  = 0x64766835,
		DvcProHD1080p30  = 0x64766833,
		DvcProHD1080p25  = 0x64766832,
		AppleProRes4444  = 0x61703468,
		AppleProRes422HQ = 0x61706368,
		AppleProRes422   = 0x6170636e,
		AppleProRes422LT = 0x61706373,
		AppleProRes422Proxy = 0x6170636f,
		Hevc             = 0x68766331
	}

#if XAMCORE_2_0
	// CMVideoDimensions => int32_t width + int32_t height
	public struct CMVideoDimensions {
		public int Width;
		public int Height;

		public CMVideoDimensions (int width, int height)
		{
			Width = width;
			Height = height;
		}
	}
#endif

	// UInt32 enum => CMFormatDescription.h
	public enum CMPixelFormat : uint
	{
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
		IndexedGrayWhiteIsZero_8bits = 40
	}
}
