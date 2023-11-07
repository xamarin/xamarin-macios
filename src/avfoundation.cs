//
// AVFoundation.cs: This file describes the API that the generator will produce for AVFoundation
//
// Authors:
//   Miguel de Icaza
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2009, Novell, Inc.
// Copyright 2010, Novell, Inc.
// Copyright 2011-2015, Xamarin, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.ComponentModel;

#if NET
using Vector3 = global::System.Numerics.Vector3;
using NMatrix3 = global::CoreGraphics.NMatrix3;
using NMatrix4x3 = global::CoreGraphics.NMatrix4x3;
#else
using Vector3 = global::OpenTK.Vector3;
using NMatrix3 = global::OpenTK.NMatrix3;
using NMatrix4x3 = global::OpenTK.NMatrix4x3;
#endif

#if !WATCH
using AudioUnit;
using AVKit;
using CoreAnimation;
using CoreImage;
using MediaToolbox;
#else
// hack: ease compilation without extra defines
using CIBarcodeDescriptor = Foundation.NSObject;
#endif

// cinematic is not present in certain platforms
#if WATCH || __MACCATALYST__
using CNAssetInfo = Foundation.NSObject;
using CNCompositionInfo = Foundation.NSObject;
#else
using Cinematic;
#endif

using AudioToolbox;
using CoreMedia;
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;
using CoreVideo;
using UniformTypeIdentifiers;
using ImageIO;
using MediaPlayer;
using System;

#if MONOMAC
using AppKit;
using UIImage = AppKit.NSImage;
#else
using UIKit;
using AVSampleCursorChunkInfo = Foundation.NSObject;
using AVSampleCursorStorageRange = Foundation.NSObject;
using AVSampleCursorSyncInfo = Foundation.NSObject;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

#if WATCH
using AVCaptureWhiteBalanceGains = Foundation.NSString;
// stubs to ease compilation using [NoWatch]
namespace AudioUnit {
	interface AudioUnit {}
}
#endif

namespace AVFoundation {

#if WATCH
	// stubs to ease compilation using [NoWatch]
	interface AudioComponent {}
	interface AudioComponentDescription {}
	interface AudioComponentInstantiationOptions {}
	interface MusicSequence {}
	interface AVInterstitialTimeRange {}
	interface AVNavigationMarkersGroup {}
	interface AVVideoSettingsCompressed {}
	interface AVVideoSettingsUncompressed {}
	interface AUAudioUnit {}
	interface CALayer {}
	interface CIContext {}
	interface CIImage {}
	interface CVPixelBufferAttributes {}
	interface CVPixelBufferPool {}
	interface MTAudioProcessingTap {}
#endif

#if XAMCORE_5_0
	delegate void AVAssetImageGeneratorCompletionHandler (CMTime requestedTime, CGImage imageRef, CMTime actualTime, AVAssetImageGeneratorResult result, NSError error);
#else
	delegate void AVAssetImageGeneratorCompletionHandler (CMTime requestedTime, IntPtr imageRef, CMTime actualTime, AVAssetImageGeneratorResult result, NSError error);
	delegate void AVAssetImageGeneratorCompletionHandler2 (CMTime requestedTime, CGImage imageRef, CMTime actualTime, AVAssetImageGeneratorResult result, NSError error);
#endif
	delegate void AVAssetImageGenerateAsynchronouslyForTimeCompletionHandler (CGImage imageRef, CMTime actualTime, NSError error);
	delegate void AVCompletion (bool finished);
	delegate void AVRequestAccessStatus (bool accessGranted);
	delegate AVAudioBuffer AVAudioConverterInputHandler (uint inNumberOfPackets, out AVAudioConverterInputStatus outStatus);

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAsynchronousVideoCompositionRequest : NSCopying {
		[Export ("renderContext", ArgumentSemantic.Copy)]
		AVVideoCompositionRenderContext RenderContext { get; }

		[Export ("compositionTime", ArgumentSemantic.Copy)]
		CMTime CompositionTime { get; }

		[Export ("sourceTrackIDs")]
		NSNumber [] SourceTrackIDs { get; }

		[Export ("videoCompositionInstruction", ArgumentSemantic.Copy)]
		AVVideoCompositionInstruction VideoCompositionInstruction { get; }

		[return: NullAllowed]
		[Export ("sourceFrameByTrackID:")]
		CVPixelBuffer SourceFrameByTrackID (int /* CMPersistentTrackID = int32_t */ trackID);

		[Export ("finishWithComposedVideoFrame:")]
		void FinishWithComposedVideoFrame (CVPixelBuffer composedVideoFrame);

		[Export ("finishWithError:")]
		void FinishWithError (NSError error);

		[Export ("finishCancelledRequest")]
		void FinishCancelledRequest ();

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("sourceSampleBufferByTrackID:")]
		[return: NullAllowed]
		CMSampleBuffer GetSourceSampleBuffer (int trackId);

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("sourceTimedMetadataByTrackID:")]
		[return: NullAllowed]
		AVTimedMetadataGroup GetSourceTimedMetadata (int trackId);

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("sourceSampleDataTrackIDs")]
		[BindAs (typeof (int []))]
		NSNumber [] SourceSampleDataTrackIds { get; }
	}

	// values are manually given since not some are platform specific
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	enum AVMediaTypes {
		[Field ("AVMediaTypeVideo")]
		Video = 0,

		[Field ("AVMediaTypeAudio")]
		Audio = 1,

		[Field ("AVMediaTypeText")]
		Text = 2,

		[Field ("AVMediaTypeClosedCaption")]
		ClosedCaption = 3,

		[Field ("AVMediaTypeSubtitle")]
		Subtitle = 4,

		[Field ("AVMediaTypeTimecode")]
		Timecode = 5,

#if !NET
		[NoTV, NoWatch]
		[Obsoleted (PlatformName.iOS, 6, 0)]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Always 'null'.")]
		[Obsoleted (PlatformName.MacOSX, 10, 8)]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Always 'null'.")]
		[NoMacCatalyst]
		[Field ("AVMediaTypeTimedMetadata")] // last header where I can find this: iOS 5.1 SDK, 10.7 only on Mac
		TimedMetadata = 6,
#endif

		[Field ("AVMediaTypeMuxed")]
		Muxed = 7,

		[NoMac]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMediaTypeMetadataObject")]
		MetadataObject = 8,

		[Field ("AVMediaTypeMetadata")]
		Metadata = 9,

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMediaTypeDepthData")]
		DepthData = 10,
	}

#if !NET
	[Obsolete ("Use AVMediaTypes enum values")]
	[NoWatch]
	[BaseType (typeof (NSObject))]
	[Static]
	interface AVMediaType {
		[Field ("AVMediaTypeVideo")]
		NSString Video { get; }

		[Field ("AVMediaTypeAudio")]
		NSString Audio { get; }

		[Field ("AVMediaTypeText")]
		NSString Text { get; }

		[Field ("AVMediaTypeClosedCaption")]
		NSString ClosedCaption { get; }

		[Field ("AVMediaTypeSubtitle")]
		NSString Subtitle { get; }

		[Field ("AVMediaTypeTimecode")]
		NSString Timecode { get; }

		[NoTV]
		[NoWatch]
		[Field ("AVMediaTypeTimedMetadata")] // last header where I can find this: iOS 5.1 SDK, 10.7 only on Mac
		[Deprecated (PlatformName.iOS, 12, 0, message: "Always 'null'.")]
		[Obsoleted (PlatformName.iOS, 6, 0, message: "Always 'null'.")]
		[Obsoleted (PlatformName.MacOSX, 10, 8, message: "Always 'null'.")]
		[NoMacCatalyst]
		NSString TimedMetadata { get; }

		[Field ("AVMediaTypeMuxed")]
		NSString Muxed { get; }

		[NoMac]
		[Field ("AVMediaTypeMetadataObject")]
		NSString MetadataObject { get; }

		[Field ("AVMediaTypeMetadata")]
		NSString Metadata { get; }
	}
#endif // !NET

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVMetadataGroup))]
	interface AVDateRangeMetadataGroup : NSCopying, NSMutableCopying {
		[Export ("initWithItems:startDate:endDate:")]
		NativeHandle Constructor (AVMetadataItem [] items, NSDate startDate, [NullAllowed] NSDate endDate);

		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; [NotImplemented] set; }

		[NullAllowed, Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; [NotImplemented] set; }

		[Export ("items", ArgumentSemantic.Copy)]
		AVMetadataItem [] Items { get; [NotImplemented] set; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVDateRangeMetadataGroup))]
	interface AVMutableDateRangeMetadataGroup {
		[Export ("startDate", ArgumentSemantic.Copy)]
		[Override]
		NSDate StartDate { get; set; }

		[NullAllowed, Export ("endDate", ArgumentSemantic.Copy)]
		[Override]
		NSDate EndDate { get; set; }

		[Export ("items", ArgumentSemantic.Copy)]
		[Override]
		AVMetadataItem [] Items { get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVDepthData {
		[Static]
		[Export ("depthDataFromDictionaryRepresentation:error:")]
		[return: NullAllowed]
		AVDepthData Create (NSDictionary imageSourceAuxDataInfoDictionary, [NullAllowed] out NSError outError);

		[Export ("depthDataByConvertingToDepthDataType:")]
		AVDepthData ConvertToDepthDataType (CVPixelFormatType depthDataType);

		[Export ("depthDataByApplyingExifOrientation:")]
		AVDepthData ApplyExifOrientation (CGImagePropertyOrientation exifOrientation);

		[Export ("depthDataByReplacingDepthDataMapWithPixelBuffer:error:")]
		[return: NullAllowed]
		AVDepthData ReplaceDepthDataMap (CVPixelBuffer pixelBuffer, [NullAllowed] out NSError outError);

		[Protected]
		[Export ("availableDepthDataTypes")]
		NSNumber [] WeakAvailableDepthDataTypes { get; }

		[Export ("dictionaryRepresentationForAuxiliaryDataType:")]
		[return: NullAllowed]
		NSDictionary GetDictionaryRepresentation ([NullAllowed] out string outAuxDataType);

		[Export ("depthDataType")]
		CVPixelFormatType DepthDataType { get; }

		[Export ("depthDataMap")]
		CVPixelBuffer DepthDataMap { get; }

		[Export ("depthDataFiltered")]
		bool IsDepthDataFiltered { [Bind ("isDepthDataFiltered")] get; }

		[Export ("depthDataAccuracy")]
		AVDepthDataAccuracy DepthDataAccuracy { get; }

		[NullAllowed, Export ("cameraCalibrationData")]
		AVCameraCalibrationData CameraCalibrationData { get; }

		[Export ("depthDataQuality")]
		AVDepthDataQuality DepthDataQuality { get; }
	}

	// values are manually given since not some are platform specific
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	enum AVMediaCharacteristics {
		[Field ("AVMediaCharacteristicVisual")]
		Visual = 0,

		[Field ("AVMediaCharacteristicAudible")]
		Audible = 1,

		[Field ("AVMediaCharacteristicLegible")]
		Legible = 2,

		[Field ("AVMediaCharacteristicFrameBased")]
		FrameBased = 3,

		[MacCatalyst (13, 1)]
		[Field ("AVMediaCharacteristicUsesWideGamutColorSpace")]
		UsesWideGamutColorSpace = 4,

		[Field ("AVMediaCharacteristicIsMainProgramContent")]
		IsMainProgramContent = 5,

		[Field ("AVMediaCharacteristicIsAuxiliaryContent")]
		IsAuxiliaryContent = 6,

		[Field ("AVMediaCharacteristicContainsOnlyForcedSubtitles")]
		ContainsOnlyForcedSubtitles = 7,

		[Field ("AVMediaCharacteristicTranscribesSpokenDialogForAccessibility")]
		TranscribesSpokenDialogForAccessibility = 8,

		[Field ("AVMediaCharacteristicDescribesMusicAndSoundForAccessibility")]
		DescribesMusicAndSoundForAccessibility = 9,

		[Field ("AVMediaCharacteristicDescribesVideoForAccessibility")]
		DescribesVideoForAccessibility = 10,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVMediaCharacteristicEasyToRead")]
		EasyToRead = 11,

		[MacCatalyst (13, 1)]
		[Field ("AVMediaCharacteristicLanguageTranslation")]
		LanguageTranslation = 12,

		[MacCatalyst (13, 1)]
		[Field ("AVMediaCharacteristicDubbedTranslation")]
		DubbedTranslation = 13,

		[MacCatalyst (13, 1)]
		[Field ("AVMediaCharacteristicVoiceOverTranslation")]
		VoiceOverTranslation = 14,

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVMediaCharacteristicIsOriginalContent")]
		IsOriginalContent = 15,

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVMediaCharacteristicContainsHDRVideo")]
		ContainsHdrVideo = 16,

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVMediaCharacteristicContainsAlphaChannel")]
		ContainsAlphaChannel = 17,

	}

#if !NET
	[NoWatch]
	[Obsolete ("Use AVMediaCharacteristics enum values")]
	[BaseType (typeof (NSObject))]
	[Static]
	interface AVMediaCharacteristic {
		[Field ("AVMediaCharacteristicVisual")]
		NSString Visual { get; }

		[Field ("AVMediaCharacteristicAudible")]
		NSString Audible { get; }

		[Field ("AVMediaCharacteristicLegible")]
		NSString Legible { get; }

		[Field ("AVMediaCharacteristicFrameBased")]
		NSString FrameBased { get; }

		[Field ("AVMediaCharacteristicUsesWideGamutColorSpace")]
		NSString UsesWideGamutColorSpace { get; }

		[Field ("AVMediaCharacteristicIsMainProgramContent")]
		NSString IsMainProgramContent { get; }

		[Field ("AVMediaCharacteristicIsAuxiliaryContent")]
		NSString IsAuxiliaryContent { get; }

		[Field ("AVMediaCharacteristicContainsOnlyForcedSubtitles")]
		NSString ContainsOnlyForcedSubtitles { get; }

		[Field ("AVMediaCharacteristicTranscribesSpokenDialogForAccessibility")]
		NSString TranscribesSpokenDialogForAccessibility { get; }

		[Field ("AVMediaCharacteristicDescribesMusicAndSoundForAccessibility")]
		NSString DescribesMusicAndSoundForAccessibility { get; }

		[Field ("AVMediaCharacteristicDescribesVideoForAccessibility")]
		NSString DescribesVideoForAccessibility { get; }

		[NoMac]
		[Field ("AVMediaCharacteristicEasyToRead")]
		NSString EasyToRead { get; }

		[Field ("AVMediaCharacteristicLanguageTranslation")]
		NSString LanguageTranslation { get; }

		[Field ("AVMediaCharacteristicDubbedTranslation")]
		NSString DubbedTranslation { get; }

		[Field ("AVMediaCharacteristicVoiceOverTranslation")]
		NSString VoiceOverTranslation { get; }

		[TV (13, 0), iOS (13, 0)]
		[Field ("AVMediaCharacteristicIsOriginalContent")]
		NSString IsOriginalContent { get; }

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Field ("AVMediaCharacteristicContainsAlphaChannel")]
		NSString ContainsAlphaChannel { get; }

		// Do not add more fields here, add them to the AVMediaCharacteristics enum instead.
	}
#endif

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	enum AVMetadataFormat {
		[MacCatalyst (13, 1)]
		[Field ("AVMetadataFormatHLSMetadata")]
		FormatHlsMetadata = 0,

		[Field ("AVMetadataFormatiTunesMetadata")]
		FormatiTunesMetadata = 1,

		[Field ("AVMetadataFormatID3Metadata")]
		FormatID3Metadata = 2,

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataFormatISOUserData")]
		FormatISOUserData = 3,

		[Field ("AVMetadataFormatQuickTimeUserData")]
		FormatQuickTimeUserData = 4,

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataFormatUnknown")]
		Unknown = 5,
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	enum AVFileTypes {
		[Field ("AVFileTypeQuickTimeMovie")]
		QuickTimeMovie = 0,

		[Field ("AVFileTypeMPEG4")]
		Mpeg4 = 1,

		[Field ("AVFileTypeAppleM4V")]
		AppleM4V = 2,

		[MacCatalyst (13, 1)]
		[Field ("AVFileType3GPP")]
		ThreeGpp = 3,

		[Field ("AVFileTypeAppleM4A")]
		AppleM4a = 4,

		[Field ("AVFileTypeCoreAudioFormat")]
		CoreAudioFormat = 5,

		[Field ("AVFileTypeWAVE")]
		Wave = 6,

		[Field ("AVFileTypeAIFF")]
		Aiff = 7,

		[Field ("AVFileTypeAIFC")]
		Aifc = 8,

		[Field ("AVFileTypeAMR")]
		Amr = 9,

		[MacCatalyst (13, 1)]
		[Field ("AVFileType3GPP2")]
		ThreeGpp2 = 10,

		[MacCatalyst (13, 1)]
		[Field ("AVFileTypeMPEGLayer3")]
		MpegLayer3 = 11,

		[MacCatalyst (13, 1)]
		[Field ("AVFileTypeSunAU")]
		SunAU = 12,

		[MacCatalyst (13, 1)]
		[Field ("AVFileTypeAC3")]
		AC3 = 13,

		[MacCatalyst (13, 1)]
		[Field ("AVFileTypeEnhancedAC3")]
		EnhancedAC3 = 14,

		[MacCatalyst (13, 1)]
		[Field ("AVFileTypeJPEG")]
		Jpeg = 15,

		[MacCatalyst (13, 1)]
		[Field ("AVFileTypeDNG")]
		Dng = 16,

		[MacCatalyst (13, 1)]
		[Field ("AVFileTypeHEIC")]
		Heic = 17,

		[MacCatalyst (13, 1)]
		[Field ("AVFileTypeAVCI")]
		Avci = 18,

		[MacCatalyst (13, 1)]
		[Field ("AVFileTypeHEIF")]
		Heif = 19,

		[MacCatalyst (13, 1)]
		[Field ("AVFileTypeTIFF")]
		Tiff = 20,

		[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
		[Field ("AVFileTypeAppleiTT")]
		AppleiTT = 21,

		[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
		[Field ("AVFileTypeSCC")]
		Scc = 22,
	}

#if !NET
	[NoWatch]
	[BaseType (typeof (NSObject))]
	[Static]
	[Obsolete ("Use AVFileTypes enum values")]
	interface AVFileType {
		[Field ("AVFileTypeQuickTimeMovie")]
		NSString QuickTimeMovie { get; }

		[Field ("AVFileTypeMPEG4")]
		NSString Mpeg4 { get; }

		[Field ("AVFileTypeAppleM4V")]
		NSString AppleM4V { get; }
		[Field ("AVFileType3GPP")]
		NSString ThreeGpp { get; }

		[Field ("AVFileTypeAppleM4A")]
		NSString AppleM4A { get; }

		[Field ("AVFileTypeCoreAudioFormat")]
		NSString CoreAudioFormat { get; }

		[Field ("AVFileTypeWAVE")]
		NSString Wave { get; }

		[Field ("AVFileTypeAIFF")]
		NSString Aiff { get; }

		[Field ("AVFileTypeAIFC")]
		NSString Aifc { get; }

		[Field ("AVFileTypeAMR")]
		NSString Amr { get; }

		[Field ("AVFileType3GPP2")]
		NSString ThreeGpp2 { get; }

		[Field ("AVFileTypeMPEGLayer3")]
		NSString MpegLayer3 { get; }

		[Field ("AVFileTypeSunAU")]
		NSString SunAU { get; }

		[Field ("AVFileTypeAC3")]
		NSString AC3 { get; }

		[Field ("AVFileTypeEnhancedAC3")]
		NSString EnhancedAC3 { get; }
	}
#endif

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Static]
	interface AVStreamingKeyDelivery {

		[Field ("AVStreamingKeyDeliveryContentKeyType")]
		NSString ContentKeyType { get; }

		[Field ("AVStreamingKeyDeliveryPersistentContentKeyType")]
		NSString PersistentContentKeyType { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[DisableDefaultCtor] // crash -> immutable and you can get them but not set them (i.e. no point in creating them)
	[BaseType (typeof (NSObject))]
	interface AVFrameRateRange {

		[Export ("minFrameRate")]
		double MinFrameRate { get; }

		[Export ("maxFrameRate")]
		double MaxFrameRate { get; }

		[Export ("maxFrameDuration", ArgumentSemantic.Copy)]
		CMTime MaxFrameDuration { get; }

		[Export ("minFrameDuration", ArgumentSemantic.Copy)]
		CMTime MinFrameDuration { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Static]
	interface AVVideo {
		[Field ("AVVideoCodecKey")]
		NSString CodecKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoMaxKeyFrameIntervalDurationKey")]
		NSString MaxKeyFrameIntervalDurationKey { get; }

		[Watch (7, 2), TV (14, 3), Mac (11, 1), iOS (14, 3)]
		[MacCatalyst (14, 3)]
		[Field ("AVVideoAppleProRAWBitDepthKey")]
		NSString AppleProRawBitDepthKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoAllowFrameReorderingKey")]
		NSString AllowFrameReorderingKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoAverageNonDroppableFrameRateKey")]
		NSString AverageNonDroppableFrameRateKey { get; }

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Field ("AVVideoEncoderSpecificationKey")]
		NSString EncoderSpecificationKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoExpectedSourceFrameRateKey")]
		NSString ExpectedSourceFrameRateKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoH264EntropyModeCABAC")]
		NSString H264EntropyModeCABAC { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoH264EntropyModeCAVLC")]
		NSString H264EntropyModeCAVLC { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoH264EntropyModeKey")]
		NSString H264EntropyModeKey { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVVideoCodecType' enum instead.")]
		[Field ("AVVideoCodecH264")]
		NSString CodecH264 { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVVideoCodecType' enum instead.")]
		[Field ("AVVideoCodecJPEG")]
		NSString CodecJPEG { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'AVVideoCodecType' enum instead.")]
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVVideoCodecType' enum instead.")]
		[Field ("AVVideoCodecAppleProRes4444")]
		NSString AppleProRes4444 { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'AVVideoCodecType' enum instead.")]
		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Field ("AVVideoCodecAppleProRes422")]
		NSString AppleProRes422 { get; }

		[Field ("AVVideoWidthKey")]
		NSString WidthKey { get; }

		[Field ("AVVideoHeightKey")]
		NSString HeightKey { get; }

		[Field ("AVVideoScalingModeKey")]
		NSString ScalingModeKey { get; }

		[Field ("AVVideoCompressionPropertiesKey")]
		NSString CompressionPropertiesKey { get; }

		[Field ("AVVideoAverageBitRateKey")]
		NSString AverageBitRateKey { get; }

		[Field ("AVVideoMaxKeyFrameIntervalKey")]
		NSString MaxKeyFrameIntervalKey { get; }

		[Field ("AVVideoProfileLevelKey")]
		NSString ProfileLevelKey { get; }

		[Field ("AVVideoQualityKey")]
		NSString QualityKey { get; }

		[Field ("AVVideoProfileLevelH264Baseline30")]
		NSString ProfileLevelH264Baseline30 { get; }

		[Field ("AVVideoProfileLevelH264Baseline31")]
		NSString ProfileLevelH264Baseline31 { get; }

		[Field ("AVVideoProfileLevelH264Main30")]
		NSString ProfileLevelH264Main30 { get; }

		[Field ("AVVideoProfileLevelH264Main31")]
		NSString ProfileLevelH264Main31 { get; }

		[Field ("AVVideoProfileLevelH264Baseline41")]
		NSString ProfileLevelH264Baseline41 { get; }

		[Field ("AVVideoProfileLevelH264Main32")]
		NSString ProfileLevelH264Main32 { get; }

		[Field ("AVVideoProfileLevelH264Main41")]
		NSString ProfileLevelH264Main41 { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoProfileLevelH264High40")]
		NSString ProfileLevelH264High40 { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoProfileLevelH264High41")]
		NSString ProfileLevelH264High41 { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoProfileLevelH264BaselineAutoLevel")]
		NSString ProfileLevelH264BaselineAutoLevel { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoProfileLevelH264MainAutoLevel")]
		NSString ProfileLevelH264MainAutoLevel { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoProfileLevelH264HighAutoLevel")]
		NSString ProfileLevelH264HighAutoLevel { get; }

		[Field ("AVVideoPixelAspectRatioKey")]
		NSString PixelAspectRatioKey { get; }

		[Field ("AVVideoPixelAspectRatioHorizontalSpacingKey")]
		NSString PixelAspectRatioHorizontalSpacingKey { get; }

		[Field ("AVVideoPixelAspectRatioVerticalSpacingKey")]
		NSString PixelAspectRatioVerticalSpacingKey { get; }

		[Field ("AVVideoCleanApertureKey")]
		NSString CleanApertureKey { get; }

		[Field ("AVVideoCleanApertureWidthKey")]
		NSString CleanApertureWidthKey { get; }

		[Field ("AVVideoCleanApertureHeightKey")]
		NSString CleanApertureHeightKey { get; }

		[Field ("AVVideoCleanApertureHorizontalOffsetKey")]
		NSString CleanApertureHorizontalOffsetKey { get; }

		[Field ("AVVideoCleanApertureVerticalOffsetKey")]
		NSString CleanApertureVerticalOffsetKey { get; }

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	interface AVVideoScalingModeKey {
		[Field ("AVVideoScalingModeFit")]
		NSString Fit { get; }

		[Field ("AVVideoScalingModeResize")]
		NSString Resize { get; }

		[Field ("AVVideoScalingModeResizeAspect")]
		NSString ResizeAspect { get; }

		[Field ("AVVideoScalingModeResizeAspectFill")]
		NSString ResizeAspectFill { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // `init` crash in tests - it may be a bug or this is an abstract class (doc not helpful)
	interface AVAudioBuffer : NSCopying, NSMutableCopying {
		[Export ("format")]
		AVAudioFormat Format { get; }

		[Export ("audioBufferList"), Internal]
		IntPtr audioBufferList { get; }

		[Export ("mutableAudioBufferList"), Internal]
		IntPtr mutableAudioBufferList { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // fails (nil handle on iOS 10)
	interface AVAudioChannelLayout : NSSecureCoding {
		[Export ("initWithLayoutTag:")]
		NativeHandle Constructor (/* UInt32 */ uint layoutTag);

		[DesignatedInitializer]
		[Export ("initWithLayout:"), Internal]
#if NET
		NativeHandle Constructor (IntPtr layout);
#else
		NativeHandle Constructor (nint /* This is really an IntPtr, but it conflicts with the default (Handle) ctor. */ layout);
#endif

		[Export ("layoutTag")]
		uint /* AudioChannelLayoutTag = UInt32 */ LayoutTag { get; }

		[Export ("layout"), Internal]
		IntPtr _Layout { get; }

		[Export ("channelCount")]
		uint /* AVAudioChannelCount = uint32_t */ ChannelCount { get; }

		[Export ("isEqual:"), Internal]
		bool IsEqual (NSObject other);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioBuffer))]
	[DisableDefaultCtor] // just like base class (AVAudioBuffer) can't, avoid crash when ToString call `description`
	interface AVAudioCompressedBuffer {
		[Export ("initWithFormat:packetCapacity:maximumPacketSize:")]
		NativeHandle Constructor (AVAudioFormat format, uint packetCapacity, nint maximumPacketSize);

		[Export ("initWithFormat:packetCapacity:")]
		NativeHandle Constructor (AVAudioFormat format, uint packetCapacity);

		[Export ("packetCapacity")]
		uint PacketCapacity { get; }

		[Export ("packetCount")]
		uint PacketCount { get; set; }

		[Export ("maximumPacketSize")]
		nint MaximumPacketSize { get; }

		[Export ("data")]
		IntPtr Data { get; }

		[NullAllowed, Export ("packetDescriptions")]
		AudioStreamPacketDescription PacketDescriptions { get; }

		[MacCatalyst (13, 1)]
		[Export ("byteCapacity")]
		uint ByteCapacity { get; }

		[MacCatalyst (13, 1)]
		[Export ("byteLength")]
		uint ByteLength { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // fails (nil handle on iOS 10)
	interface AVAudioConnectionPoint {
		[Export ("initWithNode:bus:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVAudioNode node, nuint bus);

		[NullAllowed, Export ("node", ArgumentSemantic.Weak)]
		AVAudioNode Node { get; }

		[Export ("bus")]
		nuint Bus { get; }
	}

	[MacCatalyst (13, 1)]
	delegate AVAudioEngineManualRenderingStatus AVAudioEngineManualRenderingBlock (/* AVAudioFrameCount = uint */ uint numberOfFrames, AudioBuffers outBuffer, [NullAllowed] /* OSStatus */ ref int outError);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAudioEngine {

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("musicSequence"), NullAllowed]
		MusicSequence MusicSequence { get; set; }

		[Export ("outputNode")]
		AVAudioOutputNode OutputNode { get; }

		[MacCatalyst (13, 1)]
		[Export ("inputNode")]
		AVAudioInputNode InputNode { get; }

		[Export ("mainMixerNode")]
		AVAudioMixerNode MainMixerNode { get; }

		[Export ("running")]
		bool Running { [Bind ("isRunning")] get; }

		[Export ("attachNode:")]
		void AttachNode (AVAudioNode node);

		[Export ("detachNode:")]
		void DetachNode (AVAudioNode node);

		[Export ("connect:to:fromBus:toBus:format:")]
		void Connect (AVAudioNode sourceNode, AVAudioNode targetNode, nuint sourceBus, nuint targetBus, [NullAllowed] AVAudioFormat format);

		[Export ("connect:to:format:")]
		void Connect (AVAudioNode sourceNode, AVAudioNode targetNode, [NullAllowed] AVAudioFormat format);

		[MacCatalyst (13, 1)]
		[Export ("connect:toConnectionPoints:fromBus:format:")]
		void Connect (AVAudioNode sourceNode, AVAudioConnectionPoint [] destNodes, nuint sourceBus, [NullAllowed] AVAudioFormat format);

		[Export ("disconnectNodeInput:bus:")]
		void DisconnectNodeInput (AVAudioNode node, nuint bus);

		[Export ("disconnectNodeInput:")]
		void DisconnectNodeInput (AVAudioNode node);

		[Export ("disconnectNodeOutput:bus:")]
		void DisconnectNodeOutput (AVAudioNode node, nuint bus);

		[Export ("disconnectNodeOutput:")]
		void DisconnectNodeOutput (AVAudioNode node);

		[Export ("prepare")]
		void Prepare ();

		[Export ("startAndReturnError:")]
		bool StartAndReturnError (out NSError outError);

		[Export ("pause")]
		void Pause ();

		[Export ("reset")]
		void Reset ();

		[Export ("stop")]
		void Stop ();

		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Export ("inputConnectionPointForNode:inputBus:")]
		AVAudioConnectionPoint InputConnectionPoint (AVAudioNode node, nuint bus);

		[MacCatalyst (13, 1)]
		[Export ("outputConnectionPointsForNode:outputBus:")]
		AVAudioConnectionPoint [] OutputConnectionPoints (AVAudioNode node, nuint bus);

		[Notification]
		[Field ("AVAudioEngineConfigurationChangeNotification")]
		NSString ConfigurationChangeNotification { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("autoShutdownEnabled")]
		bool AutoShutdownEnabled { [Bind ("isAutoShutdownEnabled")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("enableManualRenderingMode:format:maximumFrameCount:error:")]
		bool EnableManualRenderingMode (AVAudioEngineManualRenderingMode mode, AVAudioFormat pcmFormat, uint maximumFrameCount, out NSError outError);

		[MacCatalyst (13, 1)]
		[Export ("renderOffline:toBuffer:error:")]
		AVAudioEngineManualRenderingStatus RenderOffline (uint numberOfFrames, AVAudioPcmBuffer buffer, [NullAllowed] out NSError outError);

		[MacCatalyst (13, 1)]
		[Export ("manualRenderingBlock")]
		AVAudioEngineManualRenderingBlock ManualRenderingBlock { get; }

		[MacCatalyst (13, 1)]
		[Export ("isInManualRenderingMode")]
		bool InManualRenderingMode { get; }

		[MacCatalyst (13, 1)]
		[Export ("manualRenderingMode")]
		AVAudioEngineManualRenderingMode ManualRenderingMode { get; }

		[MacCatalyst (13, 1)]
		[Export ("manualRenderingFormat")]
		AVAudioFormat ManualRenderingFormat { get; }

		[MacCatalyst (13, 1)]
		[Export ("manualRenderingMaximumFrameCount")]
		uint ManualRenderingMaximumFrameCount { get; }

		[MacCatalyst (13, 1)]
		[Export ("manualRenderingSampleTime")]
		long ManualRenderingSampleTime { get; }

		[MacCatalyst (13, 1)]
		[Export ("disableManualRenderingMode")]
		void DisableManualRenderingMode ();

#if !WATCH
		[TV (12, 0), iOS (12, 0), NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("connectMIDI:to:format:block:")]
		void ConnectMidi (AVAudioNode sourceNode, AVAudioNode destinationNode, [NullAllowed] AVAudioFormat format, [NullAllowed] AUMidiOutputEventBlock tapHandler);

		[TV (12, 0), iOS (12, 0), NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("connectMIDI:toNodes:format:block:")]
		void ConnectMidi (AVAudioNode sourceNode, AVAudioNode [] destinationNodes, [NullAllowed] AVAudioFormat format, [NullAllowed] AUMidiOutputEventBlock tapHandler);
#endif

		[TV (12, 0), iOS (12, 0), NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("disconnectMIDI:from:")]
		void DisconnectMidi (AVAudioNode sourceNode, AVAudioNode destinationNode);

		[TV (12, 0), iOS (12, 0), NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("disconnectMIDI:fromNodes:")]
		void DisconnectMidi (AVAudioNode sourceNode, AVAudioNode [] destinationNodes);

		[TV (12, 0), iOS (12, 0), NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("disconnectMIDIInput:")]
		void DisconnectMidiInput (AVAudioNode node);

		[TV (12, 0), iOS (12, 0), NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("disconnectMIDIOutput:")]
		void DisconnectMidiOutput (AVAudioNode node);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("attachedNodes", ArgumentSemantic.Copy)]
		NSSet<AVAudioNode> AttachedNodes { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioNode))]
	[DisableDefaultCtor] // designated
	interface AVAudioEnvironmentNode : AVAudioMixing {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("nextAvailableInputBus")]
		nuint NextAvailableInputBus { get; }

		[Export ("listenerPosition", ArgumentSemantic.Assign)]
		Vector3 ListenerPosition { get; set; }

		[Export ("listenerVectorOrientation", ArgumentSemantic.Assign)]
		AVAudio3DVectorOrientation ListenerVectorOrientation { get; set; }

		[Export ("listenerAngularOrientation", ArgumentSemantic.Assign)]
		AVAudio3DAngularOrientation ListenerAngularOrientation { get; set; }

		[Export ("distanceAttenuationParameters")]
		AVAudioEnvironmentDistanceAttenuationParameters DistanceAttenuationParameters { get; }

		[Export ("reverbParameters")]
		AVAudioEnvironmentReverbParameters ReverbParameters { get; }

		[Export ("applicableRenderingAlgorithms")]
#if NET
		NSNumber [] ApplicableRenderingAlgorithms { get; }
#else
		NSObject [] ApplicableRenderingAlgorithms ();
#endif

		[Export ("outputVolume")]
		float OutputVolume { get; set; } /* float, not CGFloat */

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("outputType", ArgumentSemantic.Assign)]
		AVAudioEnvironmentOutputType OutputType { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioEnvironmentDistanceAttenuationParameters {
		[Export ("distanceAttenuationModel", ArgumentSemantic.Assign)]
		AVAudioEnvironmentDistanceAttenuationModel DistanceAttenuationModel { get; set; }

		[Export ("referenceDistance")]
		float ReferenceDistance { get; set; } /* float, not CGFloat */

		[Export ("maximumDistance")]
		float MaximumDistance { get; set; } /* float, not CGFloat */

		[Export ("rolloffFactor")]
		float RolloffFactor { get; set; } /* float, not CGFloat */
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioEnvironmentReverbParameters {
		[Export ("enable")]
		bool Enable { get; set; }

		[Export ("level")]
		float Level { get; set; } /* float, not CGFloat */

		[Export ("filterParameters")]
		AVAudioUnitEQFilterParameters FilterParameters { get; }

		[Export ("loadFactoryReverbPreset:")]
		void LoadFactoryReverbPreset (AVAudioUnitReverbPreset preset);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAudioFile {
		[Export ("initForReading:error:")]
		NativeHandle Constructor (NSUrl fileUrl, out NSError outError);

		[Export ("initForReading:commonFormat:interleaved:error:")]
		NativeHandle Constructor (NSUrl fileUrl, AVAudioCommonFormat format, bool interleaved, out NSError outError);

		[Export ("initForWriting:settings:error:"), Internal]
		NativeHandle Constructor (NSUrl fileUrl, NSDictionary settings, out NSError outError);

		[Wrap ("this (fileUrl, settings.GetDictionary ()!, out outError)")]
		NativeHandle Constructor (NSUrl fileUrl, AudioSettings settings, out NSError outError);

		[Export ("initForWriting:settings:commonFormat:interleaved:error:"), Internal]
		NativeHandle Constructor (NSUrl fileUrl, NSDictionary settings, AVAudioCommonFormat format, bool interleaved, out NSError outError);

		[Wrap ("this (fileUrl, settings.GetDictionary ()!, format, interleaved, out outError)")]
		NativeHandle Constructor (NSUrl fileUrl, AudioSettings settings, AVAudioCommonFormat format, bool interleaved, out NSError outError);

		[Export ("url")]
		NSUrl Url { get; }

		[Export ("fileFormat")]
		AVAudioFormat FileFormat { get; }

		[Export ("processingFormat")]
		AVAudioFormat ProcessingFormat { get; }

		[Export ("length")]
		long Length { get; }

		[Export ("framePosition")]
		long FramePosition { get; set; }

		[Export ("readIntoBuffer:error:")]
		bool ReadIntoBuffer (AVAudioPcmBuffer buffer, out NSError outError);

		[Export ("readIntoBuffer:frameCount:error:")]
		bool ReadIntoBuffer (AVAudioPcmBuffer buffer, uint /* AVAudioFrameCount = uint32_t */ frames, out NSError outError);

		[Export ("writeFromBuffer:error:")]
		bool WriteFromBuffer (AVAudioPcmBuffer buffer, out NSError outError);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAudioFormat : NSSecureCoding {
		[Export ("initWithStreamDescription:")]
		NativeHandle Constructor (ref AudioStreamBasicDescription description);

		[Export ("initWithStreamDescription:channelLayout:")]
		NativeHandle Constructor (ref AudioStreamBasicDescription description, [NullAllowed] AVAudioChannelLayout layout);

		[Export ("initStandardFormatWithSampleRate:channels:")]
		NativeHandle Constructor (double sampleRate, uint /* AVAudioChannelCount = uint32_t */ channels);

		[Export ("initStandardFormatWithSampleRate:channelLayout:")]
		NativeHandle Constructor (double sampleRate, AVAudioChannelLayout layout);

		[Export ("initWithCommonFormat:sampleRate:channels:interleaved:")]
		NativeHandle Constructor (AVAudioCommonFormat format, double sampleRate, uint /* AVAudioChannelCount = uint32_t */ channels, bool interleaved);

		[Export ("initWithCommonFormat:sampleRate:interleaved:channelLayout:")]
		NativeHandle Constructor (AVAudioCommonFormat format, double sampleRate, bool interleaved, AVAudioChannelLayout layout);

		[Export ("initWithSettings:")]
		NativeHandle Constructor (NSDictionary settings);

		[Wrap ("this (settings.GetDictionary ()!)")]
		NativeHandle Constructor (AudioSettings settings);

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithCMAudioFormatDescription:")]
		NativeHandle Constructor (CMAudioFormatDescription formatDescription);

		[Export ("standard")]
		bool Standard { [Bind ("isStandard")] get; }

		[Export ("commonFormat")]
		AVAudioCommonFormat CommonFormat { get; }

		[Export ("channelCount")]
		uint ChannelCount { get; } /* AVAudioChannelCount = uint32_t */

		[Export ("sampleRate")]
		double SampleRate { get; }

		[Export ("interleaved")]
		bool Interleaved { [Bind ("isInterleaved")] get; }

		[Internal]
		[Export ("streamDescription")]
		IntPtr _StreamDescription { get; }

		[Export ("channelLayout"), NullAllowed]
		AVAudioChannelLayout ChannelLayout { get; }

		[Export ("settings")]
		NSDictionary WeakSettings { get; }

		[Wrap ("WeakSettings")]
		AudioSettings Settings { get; }

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("formatDescription")]
		CMAudioFormatDescription FormatDescription { get; }

		[Export ("isEqual:"), Internal]
		bool IsEqual (NSObject obj);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("magicCookie", ArgumentSemantic.Retain)]
		NSData MagicCookie { get; set; }
	}

	[NoWatch] // all members are unavailable
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVAudio3DMixing {
		[Abstract]
		[Export ("renderingAlgorithm")]
		AVAudio3DMixingRenderingAlgorithm RenderingAlgorithm { get; set; }

		[Abstract]
		[Export ("rate")]
		float Rate { get; set; } /* float, not CGFloat */

		[Abstract]
		[Export ("reverbBlend")]
		float ReverbBlend { get; set; } /* float, not CGFloat */

		[Abstract]
		[Export ("obstruction")]
		float Obstruction { get; set; } /* float, not CGFloat */

		[Abstract]
		[Export ("occlusion")]
		float Occlusion { get; set; } /* float, not CGFloat */

		[Abstract]
		[Export ("position")]
		Vector3 Position { get; set; }

#if NET
		[Abstract]
		[TV (13,0), NoWatch, iOS (13,0)]
		[MacCatalyst (13, 1)]
		[Export ("sourceMode", ArgumentSemantic.Assign)]
		AVAudio3DMixingSourceMode SourceMode { get; set; }

		[Abstract]
		[TV (13,0), NoWatch, iOS (13,0)]
		[MacCatalyst (13, 1)]
		[Export ("pointSourceInHeadMode", ArgumentSemantic.Assign)]
		AVAudio3DMixingPointSourceInHeadMode PointSourceInHeadMode { get; set; }
	
#else
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Export ("sourceMode", ArgumentSemantic.Assign)]
		AVAudio3DMixingSourceMode GetSourceMode ();

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Export ("setSourceMode:")]
		void SetSourceMode (AVAudio3DMixingSourceMode sourceMode);

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Export ("pointSourceInHeadMode", ArgumentSemantic.Assign)]
		AVAudio3DMixingPointSourceInHeadMode GetPointSourceInHeadMode ();

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Export ("setPointSourceInHeadMode:")]
		void SetPointSourceInHeadMode (AVAudio3DMixingPointSourceInHeadMode pointSourceInHeadMode);
#endif
	}

	[MacCatalyst (13, 1)]
	[Protocol]
	interface AVAudioMixing : AVAudioStereoMixing
#if !WATCH
		, AVAudio3DMixing
#endif
	{

		[MacCatalyst (13, 1)]
#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		[Export ("destinationForMixer:bus:")]
		[return: NullAllowed]
		AVAudioMixingDestination DestinationForMixer (AVAudioNode mixer, nuint bus);

		[Abstract]
		[Export ("volume")]
		float Volume { get; set; } /* float, not CGFloat */
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Default constructor not allowed : Objective-C exception thrown
	interface AVAudioMixingDestination : AVAudioMixing {

		[Export ("connectionPoint")]
		AVAudioConnectionPoint ConnectionPoint { get; }
	}

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVAudioStereoMixing {
		[Abstract]
		[Export ("pan")]
		float Pan { get; set; } /* float, not CGFloat */
	}

	delegate void AVAudioNodeTapBlock (AVAudioPcmBuffer buffer, AVAudioTime when);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // documented as an abstract class, returned Handle is nil
	interface AVAudioNode {
		[Export ("engine"), NullAllowed]
		AVAudioEngine Engine { get; }

		[Export ("numberOfInputs")]
		nuint NumberOfInputs { get; }

		[Export ("numberOfOutputs")]
		nuint NumberOfOutputs { get; }

		[Export ("lastRenderTime"), NullAllowed]
		AVAudioTime LastRenderTime { get; }

		[Export ("reset")]
		void Reset ();

		[Export ("inputFormatForBus:")]
		AVAudioFormat GetBusInputFormat (nuint bus);

		[Export ("outputFormatForBus:")]
		AVAudioFormat GetBusOutputFormat (nuint bus);

		[Export ("nameForInputBus:")]
		[return: NullAllowed]
		string GetNameForInputBus (nuint bus);

		[Export ("nameForOutputBus:")]
		[return: NullAllowed]
		string GetNameForOutputBus (nuint bus);

		[Export ("installTapOnBus:bufferSize:format:block:")]
		void InstallTapOnBus (nuint bus, uint /* AVAudioFrameCount = uint32_t */ bufferSize, [NullAllowed] AVAudioFormat format, AVAudioNodeTapBlock tapBlock);

		[Export ("removeTapOnBus:")]
		void RemoveTapOnBus (nuint bus);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("AUAudioUnit")]
		AUAudioUnit AUAudioUnit { get; }

		[MacCatalyst (13, 1)]
		[Export ("latency")]
		double Latency { get; }

		[MacCatalyst (13, 1)]
		[Export ("outputPresentationLatency")]
		double OutputPresentationLatency { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioNode))]
	[DisableDefaultCtor] // documented as a base class - returned Handle is nil
	interface AVAudioIONode {
		[Export ("presentationLatency")]
		double PresentationLatency { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("audioUnit"), NullAllowed]
		global::AudioUnit.AudioUnit AudioUnit { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("voiceProcessingEnabled")]
		bool VoiceProcessingEnabled { [Bind ("isVoiceProcessingEnabled")] get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setVoiceProcessingEnabled:error:")]
		bool SetVoiceProcessingEnabled (bool enabled, out NSError outError);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioNode))]
	[DisableDefaultCtor] // designated
	interface AVAudioMixerNode : AVAudioMixing {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("outputVolume")]
		float OutputVolume { get; set; } /* float, not CGFloat */

		[Export ("nextAvailableInputBus")]
		nuint NextAvailableInputBus { get; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // returned Handle is nil
						 // note: sample source (header) suggest it comes from AVAudioEngine properties
	[BaseType (typeof (AVAudioIONode))]
	interface AVAudioOutputNode {

	}

	[MacCatalyst (13, 1)]
	delegate AudioBuffers AVAudioIONodeInputBlock (uint frameCount);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioIONode))]
	[DisableDefaultCtor] // returned Handle is nil
						 // note: sample source (header) suggest it comes from AVAudioEngine properties
	interface AVAudioInputNode : AVAudioMixing {

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setManualRenderingInputPCMFormat:inputBlock:")]
		bool SetManualRenderingInputPcmFormat (AVAudioFormat format, AVAudioIONodeInputBlock block);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("voiceProcessingBypassed")]
		bool VoiceProcessingBypassed { [Bind ("isVoiceProcessingBypassed")] get; set; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("voiceProcessingAGCEnabled")]
		bool VoiceProcessingAgcEnabled { [Bind ("isVoiceProcessingAGCEnabled")] get; set; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("voiceProcessingInputMuted")]
		bool VoiceProcessingInputMuted { [Bind ("isVoiceProcessingInputMuted")] get; set; }

	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioBuffer), Name = "AVAudioPCMBuffer")]
	[DisableDefaultCtor] // crash in tests
	interface AVAudioPcmBuffer {

		[DesignatedInitializer]
		[Export ("initWithPCMFormat:frameCapacity:")]
		NativeHandle Constructor (AVAudioFormat format, uint /* AVAudioFrameCount = uint32_t */ frameCapacity);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithPCMFormat:bufferListNoCopy:deallocator:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVAudioFormat format, AudioBuffers bufferList, [NullAllowed] Action<AudioBuffers> deallocator);

		[Export ("frameCapacity")]
		uint FrameCapacity { get; } /* AVAudioFrameCount = uint32_t */

		[Export ("frameLength")]
		uint FrameLength { get; set; } /* AVAudioFrameCount = uint32_t */

		[Export ("stride")]
		nuint Stride { get; }

		[Export ("floatChannelData")]
		IntPtr FloatChannelData { get; }

		[Export ("int16ChannelData")]
		IntPtr Int16ChannelData { get; }

		[Export ("int32ChannelData")]
		IntPtr Int32ChannelData { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAudioPlayer {

		[Export ("prepareToPlay")]
		bool PrepareToPlay ();

		[Export ("play")]
		bool Play ();

		[Export ("pause")]
		void Pause ();

		[Export ("stop")]
		void Stop ();

		[Export ("playing")]
		bool Playing { [Bind ("isPlaying")] get; }

		[Export ("numberOfChannels")]
		nuint NumberOfChannels { get; }

		[Export ("duration")]
		double Duration { get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate"), NullAllowed]
		[Protocolize]
		AVAudioPlayerDelegate Delegate { get; set; }

		[Export ("url"), NullAllowed]
		NSUrl Url { get; }

		[Export ("data"), NullAllowed]
		NSData Data { get; }

		[Export ("volume")]
		float Volume { get; set; } // defined as 'float'

		[MacCatalyst (13, 1)]
		[Export ("setVolume:fadeDuration:")]
		void SetVolume (float volume, double duration);

		[Export ("currentTime")]
		double CurrentTime { get; set; }

		[Export ("numberOfLoops")]
		nint NumberOfLoops { get; set; }

		[Export ("meteringEnabled")]
		bool MeteringEnabled { [Bind ("isMeteringEnabled")] get; set; }

		[Export ("updateMeters")]
		void UpdateMeters ();

		[Export ("peakPowerForChannel:")]
		float PeakPower (nuint channelNumber); // defined as 'float'

		[Export ("averagePowerForChannel:")]
		float AveragePower (nuint channelNumber); // defined as 'float'

		[Export ("deviceCurrentTime")]
		double DeviceCurrentTime { get; }

		[Export ("pan")]
		float Pan { get; set; } // defined as 'float'

		[Export ("playAtTime:")]
		bool PlayAtTime (double time);

		[Export ("settings")]
		[Protected]
		NSDictionary WeakSettings { get; }

		[Wrap ("WeakSettings")]
		AudioSettings SoundSetting { get; }

		[Export ("enableRate")]
		bool EnableRate { get; set; }

		[Export ("rate")]
		float Rate { get; set; } // defined as 'float'		

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("channelAssignments", ArgumentSemantic.Copy), NullAllowed]
		AVAudioSessionChannelDescription [] ChannelAssignments { get; set; }

		[Export ("initWithData:fileTypeHint:error:")]
		[MacCatalyst (13, 1)]
		NativeHandle Constructor (NSData data, [NullAllowed] string fileTypeHint, out NSError outError);

		[Export ("initWithContentsOfURL:fileTypeHint:error:")]
		[MacCatalyst (13, 1)]
		NativeHandle Constructor (NSUrl url, [NullAllowed] string fileTypeHint, out NSError outError);

		[MacCatalyst (13, 1)]
		[Export ("format")]
		AVAudioFormat Format { get; }

		[NoiOS, NoTV, NoWatch, MacCatalyst (15, 0)]
		[NullAllowed, Export ("currentDevice")]
		string CurrentDevice { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVAudioPlayerDelegate {
		[Export ("audioPlayerDidFinishPlaying:successfully:"), CheckDisposed]
		void FinishedPlaying (AVAudioPlayer player, bool flag);

		[Export ("audioPlayerDecodeErrorDidOccur:error:")]
		void DecoderError (AVAudioPlayer player, [NullAllowed] NSError error);

		[NoMac]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("audioPlayerBeginInterruption:")]
		void BeginInterruption (AVAudioPlayer player);

		[NoMac]
		[Export ("audioPlayerEndInterruption:")]
		[Deprecated (PlatformName.iOS, 6, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void EndInterruption (AVAudioPlayer player);

		[NoMac]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("audioPlayerEndInterruption:withOptions:")]
#if NET
		void EndInterruption (AVAudioPlayer player, AVAudioSessionInterruptionOptions flags);
#else
		void EndInterruption (AVAudioPlayer player, AVAudioSessionInterruptionFlags flags);
#endif
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioNode))]
	[DisableDefaultCtor] // designated
	interface AVAudioPlayerNode : AVAudioMixing {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("playing")]
		bool Playing { [Bind ("isPlaying")] get; }

		[Async]
		[Export ("scheduleBuffer:completionHandler:")]
		void ScheduleBuffer (AVAudioPcmBuffer buffer, [NullAllowed] Action completionHandler);

		[Async]
		[Export ("scheduleBuffer:atTime:options:completionHandler:")]
		void ScheduleBuffer (AVAudioPcmBuffer buffer, [NullAllowed] AVAudioTime when, AVAudioPlayerNodeBufferOptions options, [NullAllowed] Action completionHandler);

		[Async]
		[MacCatalyst (13, 1)]
		[Export ("scheduleBuffer:completionCallbackType:completionHandler:")]
		void ScheduleBuffer (AVAudioPcmBuffer buffer, AVAudioPlayerNodeCompletionCallbackType callbackType, [NullAllowed] Action<AVAudioPlayerNodeCompletionCallbackType> completionHandler);

		[Async]
		[MacCatalyst (13, 1)]
		[Export ("scheduleBuffer:atTime:options:completionCallbackType:completionHandler:")]
		void ScheduleBuffer (AVAudioPcmBuffer buffer, [NullAllowed] AVAudioTime when, AVAudioPlayerNodeBufferOptions options, AVAudioPlayerNodeCompletionCallbackType callbackType, [NullAllowed] Action<AVAudioPlayerNodeCompletionCallbackType> completionHandler);

		[Async]
		[Export ("scheduleFile:atTime:completionHandler:")]
		void ScheduleFile (AVAudioFile file, [NullAllowed] AVAudioTime when, [NullAllowed] Action completionHandler);

		[Async]
		[MacCatalyst (13, 1)]
		[Export ("scheduleFile:atTime:completionCallbackType:completionHandler:")]
		void ScheduleFile (AVAudioFile file, [NullAllowed] AVAudioTime when, AVAudioPlayerNodeCompletionCallbackType callbackType, [NullAllowed] Action<AVAudioPlayerNodeCompletionCallbackType> completionHandler);

		[Async]
		[Export ("scheduleSegment:startingFrame:frameCount:atTime:completionHandler:")]
		void ScheduleSegment (AVAudioFile file, long startFrame, uint /* AVAudioFrameCount = uint32_t */ numberFrames, [NullAllowed] AVAudioTime when, [NullAllowed] Action completionHandler);

		[Async]
		[MacCatalyst (13, 1)]
		[Export ("scheduleSegment:startingFrame:frameCount:atTime:completionCallbackType:completionHandler:")]
		void ScheduleSegment (AVAudioFile file, long startFrame, uint numberFrames, [NullAllowed] AVAudioTime when, AVAudioPlayerNodeCompletionCallbackType callbackType, [NullAllowed] Action<AVAudioPlayerNodeCompletionCallbackType> completionHandler);

		[Export ("stop")]
		void Stop ();

		[Export ("prepareWithFrameCount:")]
		void PrepareWithFrameCount (uint /* AVAudioFrameCount = uint32_t */ frameCount);

		[Export ("play")]
		void Play ();

		[Export ("playAtTime:")]
		void PlayAtTime ([NullAllowed] AVAudioTime when);

		[Export ("pause")]
		void Pause ();

		[return: NullAllowed]
		[Export ("nodeTimeForPlayerTime:")]
		AVAudioTime GetNodeTimeFromPlayerTime (AVAudioTime playerTime);

		[return: NullAllowed]
		[Export ("playerTimeForNodeTime:")]
		AVAudioTime GetPlayerTimeFromNodeTime (AVAudioTime nodeTime);
	}

	[BaseType (typeof (NSObject))]
	[NoTV]
	[MacCatalyst (13, 1)]
	interface AVAudioRecorder {
		[Export ("initWithURL:settings:error:")]
		[Internal]
		IntPtr InitWithUrl (NSUrl url, NSDictionary settings, out NSError error);

		[Internal]
		[MacCatalyst (13, 1)]
		[Export ("initWithURL:format:error:")]
		IntPtr InitWithUrl (NSUrl url, AVAudioFormat format, out NSError outError);

		[Export ("prepareToRecord")]
		bool PrepareToRecord ();

		[Export ("record")]
		bool Record ();

		[Export ("recordForDuration:")]
		bool RecordFor (double duration);

		[Export ("pause")]
		void Pause ();

		[Export ("stop")]
		void Stop ();

		[Export ("deleteRecording")]
		bool DeleteRecording ();

		[Export ("recording")]
		bool Recording { [Bind ("isRecording")] get; }

		[Export ("url")]
		NSUrl Url { get; }

		[Export ("settings")]
		NSDictionary WeakSettings { get; }

		[Wrap ("WeakSettings")]
		AudioSettings Settings { get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate"), NullAllowed]
		[Protocolize]
		AVAudioRecorderDelegate Delegate { get; set; }

#if !XAMCORE_5_0
		[Obsolete ("Use the 'CurrentTime' property instead.")]
		[Wrap ("CurrentTime", IsVirtual = true)]
		double currentTime { get; }
#endif

		[Export ("currentTime")]
		double CurrentTime { get; }

		[Export ("meteringEnabled")]
		bool MeteringEnabled { [Bind ("isMeteringEnabled")] get; set; }

		[Export ("updateMeters")]
		void UpdateMeters ();

		[Export ("peakPowerForChannel:")]
		float PeakPower (nuint channelNumber); // defined as 'float'

		[Export ("averagePowerForChannel:")]
		float AveragePower (nuint channelNumber); // defined as 'float'

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("channelAssignments", ArgumentSemantic.Copy), NullAllowed]
		AVAudioSessionChannelDescription [] ChannelAssignments { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("recordAtTime:")]
		bool RecordAt (double time);

		[MacCatalyst (13, 1)]
		[Export ("recordAtTime:forDuration:")]
		bool RecordAt (double time, double duration);

		[MacCatalyst (13, 1)]
		[Export ("deviceCurrentTime")]
		double DeviceCurrentTime { get; }

		[MacCatalyst (13, 1)]
		[Export ("format")]
		AVAudioFormat Format { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[NoTV]
	[MacCatalyst (13, 1)]
	interface AVAudioRecorderDelegate {
		[Export ("audioRecorderDidFinishRecording:successfully:"), CheckDisposed]
		void FinishedRecording (AVAudioRecorder recorder, bool flag);

		[Export ("audioRecorderEncodeErrorDidOccur:error:")]
		void EncoderError (AVAudioRecorder recorder, [NullAllowed] NSError error);

		[NoMac]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("audioRecorderBeginInterruption:")]
		void BeginInterruption (AVAudioRecorder recorder);

		[NoMac]
		[Deprecated (PlatformName.iOS, 6, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("audioRecorderEndInterruption:")]
		void EndInterruption (AVAudioRecorder recorder);

		// Deprecated in iOS 6.0 but we have same C# signature as a method that was deprecated in iOS 8.0
		[Deprecated (PlatformName.iOS, 8, 0)]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
#if !NET
		[Export ("audioRecorderEndInterruption:withFlags:")]
		void EndInterruption (AVAudioRecorder recorder, AVAudioSessionInterruptionFlags flags);
#else
		[Export ("audioRecorderEndInterruption:withOptions:")]
		void EndInterruption (AVAudioRecorder recorder, AVAudioSessionInterruptionOptions flags);
#endif // !NET
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	interface AVAudioSessionSecondaryAudioHintEventArgs {
		[Export ("AVAudioSessionSilenceSecondaryAudioHintNotification")]
		AVAudioSessionSilenceSecondaryAudioHintType Hint { get; }

		[Export ("AVAudioSessionSilenceSecondaryAudioHintTypeKey")]
		AVAudioSessionRouteDescription HintType { get; }
	}

	delegate void AVPermissionGranted (bool granted);

	[iOS (14, 5), Watch (7, 4), NoTV, NoMac]
	[MacCatalyst (14, 5)]
	[Native]
	public enum AVAudioSessionInterruptionReason : ulong {
		Default = 0,
		AppWasSuspended = 1,
		BuiltInMicMuted = 2,
	}

	[Watch (8, 0), TV (15, 0), NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	interface SpatialPlaybackCapabilitiesChangedEventArgs {
		[Export ("AVAudioSessionSpatialAudioEnabledKey")]
		bool SpatialAudioEnabledKey { get; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
#if MONOMAC
	[Static]
#endif
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // for binary compatibility this is added in AVAudioSession.cs w/[Obsolete]
	interface AVAudioSession {

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("sharedInstance"), Static]
		AVAudioSession SharedInstance ();

		[NoWatch, NoMac]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'AVAudioSession.Notification.Observe*' methods instead.")]
		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVAudioSession.Notification.Observe*' methods instead.")]
		NSObject WeakDelegate { get; set; }

		[NoWatch, NoMac]
		[Wrap ("WeakDelegate")]
		[Protocolize]
		[NullAllowed]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'AVAudioSession.Notification.Observe*' methods instead.")]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVAudioSession.Notification.Observe*' methods instead.")]
		AVAudioSessionDelegate Delegate { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setActive:error:")]
		bool SetActive (bool beActive, out NSError outError);

		[return: NullAllowed]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("SetActive (beActive, out var outError) ? null : outError")]
		NSError SetActive (bool beActive);

#if !NET
		[NoTV, NoMac]
		[Export ("setActive:withFlags:error:")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'SetActive (bool, AVAudioSessionSetActiveOptions, out NSError)' instead.")]
		bool SetActive (bool beActive, AVAudioSessionFlags flags, out NSError outError);
#endif // !NET

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setCategory:error:")]
		bool SetCategory (NSString theCategory, out NSError outError);

		[return: NullAllowed]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("SetCategory (theCategory, out var outError) ? null : outError")]
		NSError SetCategory (NSString theCategory);

		[return: NullAllowed]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("SetCategory (category.GetConstant ()!, out var outError) ? null : outError")]
		NSError SetCategory (AVAudioSessionCategory category);

		[NoTV, NoMac]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'SetPreferredSampleRate' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetPreferredSampleRate' instead.")]
		[Export ("setPreferredHardwareSampleRate:error:")]
		bool SetPreferredHardwareSampleRate (double sampleRate, out NSError outError);

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setPreferredIOBufferDuration:error:")]
		bool SetPreferredIOBufferDuration (double duration, out NSError outError);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("category")]
		NSString Category { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("mode")]
		NSString Mode { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setMode:error:")]
		bool SetMode (NSString mode, out NSError error);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("SetMode (mode.GetConstant ()!, out error)")]
		bool SetMode (AVAudioSessionMode mode, out NSError error);

		[NoTV, NoMac]
		[Export ("preferredHardwareSampleRate")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'PreferredSampleRate' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PreferredSampleRate' instead.")]
		double PreferredHardwareSampleRate { get; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("preferredIOBufferDuration")]
		double PreferredIOBufferDuration { get; }

		[NoTV, NoMac]
		[Export ("inputIsAvailable")]
		[Deprecated (PlatformName.iOS, 6, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		bool InputIsAvailable { get; }

		[NoTV, NoMac]
		[Export ("currentHardwareSampleRate")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'SampleRate' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SampleRate' instead.")]
		double CurrentHardwareSampleRate { get; }

		[NoTV, NoMac]
		[Export ("currentHardwareInputNumberOfChannels")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'InputNumberOfChannels' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'InputNumberOfChannels' instead.")]
		nint CurrentHardwareInputNumberOfChannels { get; }

		[NoTV, NoMac]
		[Export ("currentHardwareOutputNumberOfChannels")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'OutputNumberOfChannels' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'OutputNumberOfChannels' instead.")]
		nint CurrentHardwareOutputNumberOfChannels { get; }

#if !XAMCORE_5_0
		[Obsolete ("Use 'AVAudioSessionCategory' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionCategoryAmbient")]
		NSString CategoryAmbient { get; }

		[Obsolete ("Use 'AVAudioSessionCategory' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionCategorySoloAmbient")]
		NSString CategorySoloAmbient { get; }

		[Obsolete ("Use 'AVAudioSessionCategory' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionCategoryPlayback")]
		NSString CategoryPlayback { get; }

		[Obsolete ("Use 'AVAudioSessionCategory' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionCategoryRecord")]
		NSString CategoryRecord { get; }

		[Obsolete ("Use 'AVAudioSessionCategory' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionCategoryPlayAndRecord")]
		NSString CategoryPlayAndRecord { get; }

		[Obsolete ("Use 'AVAudioSessionCategory' enum values instead.")]
		[NoTV]
		[NoWatch]
		[NoMac]
		[Deprecated (PlatformName.iOS, 10, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("AVAudioSessionCategoryAudioProcessing")]
		NSString CategoryAudioProcessing { get; }
#endif // !XAMCORE_5_0

		[Obsolete ("Use 'AVAudioSessionMode' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeDefault")]
		NSString ModeDefault { get; }

		[Obsolete ("Use 'AVAudioSessionMode' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeVoiceChat")]
		NSString ModeVoiceChat { get; }

		[Obsolete ("Use 'AVAudioSessionMode' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeVideoRecording")]
		NSString ModeVideoRecording { get; }

		[Obsolete ("Use 'AVAudioSessionMode' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeMeasurement")]
		NSString ModeMeasurement { get; }

		[Obsolete ("Use 'AVAudioSessionMode' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeGameChat")]
		NSString ModeGameChat { get; }

		[Obsolete ("Use 'AVAudioSessionMode' enum values instead.")]
		[Watch (5, 0), TV (12, 0), NoMac, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeVoicePrompt")]
		NSString VoicePrompt { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setActive:withOptions:error:")]
		bool SetActive (bool active, AVAudioSessionSetActiveOptions options, out NSError outError);

		[return: NullAllowed]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("SetActive (active, options, out var outError) ? null : outError")]
		NSError SetActive (bool active, AVAudioSessionSetActiveOptions options);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("availableCategories")]
		string [] AvailableCategories { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setCategory:withOptions:error:")]
		bool SetCategory (string category, AVAudioSessionCategoryOptions options, out NSError outError);

		[return: NullAllowed]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("SetCategory (category.GetConstant ()!, options, out var outError) ? null : outError")]
		NSError SetCategory (AVAudioSessionCategory category, AVAudioSessionCategoryOptions options);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("SetCategory (category.GetConstant ()!, options, out outError)")]
		bool SetCategory (AVAudioSessionCategory category, AVAudioSessionCategoryOptions options, out NSError outError);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setCategory:mode:options:error:")]
		bool SetCategory (string category, string mode, AVAudioSessionCategoryOptions options, out NSError outError);

		[NoMac]
		[iOS (10, 0), TV (10, 0)]
		[MacCatalyst (13, 1)]
		[Wrap ("SetCategory (category.GetConstant ()!, mode, options, out outError)")]
		bool SetCategory (AVAudioSessionCategory category, string mode, AVAudioSessionCategoryOptions options, out NSError outError);

		[return: NullAllowed]
		[NoMac]
		[iOS (10, 0), TV (10, 0)]
		[MacCatalyst (13, 1)]
		[Wrap ("SetCategory (category.GetConstant ()!, mode, options, out var outError) ? null : outError")]
		NSError SetCategory (AVAudioSessionCategory category, string mode, AVAudioSessionCategoryOptions options);

		[NoMac]
		[iOS (10, 0), TV (10, 0)]
		[MacCatalyst (13, 1)]
		[Wrap ("SetCategory (category.GetConstant ()!, mode.GetConstant ()!, options, out outError)")]
		bool SetCategory (AVAudioSessionCategory category, AVAudioSessionMode mode, AVAudioSessionCategoryOptions options, out NSError outError);

		[return: NullAllowed]
		[NoMac]
		[iOS (10, 0), TV (10, 0)]
		[MacCatalyst (13, 1)]
		[Wrap ("SetCategory (category.GetConstant ()!, mode.GetConstant ()!, options, out var outError) ? null : outError")]
		NSError SetCategory (AVAudioSessionCategory category, AVAudioSessionMode mode, AVAudioSessionCategoryOptions options);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("categoryOptions")]
		AVAudioSessionCategoryOptions CategoryOptions { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("availableModes")]
		string [] AvailableModes { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("overrideOutputAudioPort:error:")]
		bool OverrideOutputAudioPort (AVAudioSessionPortOverride portOverride, out NSError outError);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("otherAudioPlaying")]
		bool OtherAudioPlaying { [Bind ("isOtherAudioPlaying")] get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("currentRoute")]
		AVAudioSessionRouteDescription CurrentRoute { get; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setPreferredSampleRate:error:")]
		bool SetPreferredSampleRate (double sampleRate, out NSError error);

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("preferredSampleRate")]
		double PreferredSampleRate { get; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("inputGain")]
		float InputGain { get; } // defined as 'float'

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("inputGainSettable")]
		bool InputGainSettable { [Bind ("isInputGainSettable")] get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("inputAvailable")]
		bool InputAvailable { [Bind ("isInputAvailable")] get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("sampleRate")]
		double SampleRate { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("inputNumberOfChannels")]
		nint InputNumberOfChannels { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("outputNumberOfChannels")]
		nint OutputNumberOfChannels { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("outputVolume")]
		float OutputVolume { get; } // defined as 'float'

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("inputLatency")]
		double InputLatency { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("outputLatency")]
		double OutputLatency { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("IOBufferDuration")]
		double IOBufferDuration { get; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setInputGain:error:")]
		bool SetInputGain (float /* defined as 'float' */ gain, out NSError outError);

		[Field ("AVAudioSessionInterruptionNotification")]
		[Notification (typeof (AVAudioSessionInterruptionEventArgs))]
		NSString InterruptionNotification { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionRouteChangeNotification")]
		[Notification (typeof (AVAudioSessionRouteChangeEventArgs))]
		NSString RouteChangeNotification { get; }

		[Field ("AVAudioSessionMediaServicesWereResetNotification")]
		[Notification]
		NSString MediaServicesWereResetNotification { get; }

		[Notification, Field ("AVAudioSessionMediaServicesWereLostNotification")]
		NSString MediaServicesWereLostNotification { get; }

#if !XAMCORE_5_0
		[Obsolete ("Use 'AVAudioSessionCategory' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionCategoryMultiRoute")]
		NSString CategoryMultiRoute { get; }
#endif // !XAMCORE_5_0

		[Obsolete ("Use 'AVAudioSessionMode' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeMoviePlayback")]
		NSString ModeMoviePlayback { get; }

		[Obsolete ("Use 'AVAudioSessionMode' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeVideoChat")]
		NSString ModeVideoChat { get; }

		[Obsolete ("Use 'AVAudioSessionMode' enum values instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeSpokenAudio")]
		NSString ModeSpokenAudio { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortLineIn")]
		NSString PortLineIn { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortBuiltInMic")]
		NSString PortBuiltInMic { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortHeadsetMic")]
		NSString PortHeadsetMic { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortLineOut")]
		NSString PortLineOut { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortHeadphones")]
		NSString PortHeadphones { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortBluetoothA2DP")]
		NSString PortBluetoothA2DP { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortBuiltInReceiver")]
		NSString PortBuiltInReceiver { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortBuiltInSpeaker")]
		NSString PortBuiltInSpeaker { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortHDMI")]
		NSString PortHdmi { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortAirPlay")]
		NSString PortAirPlay { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortBluetoothHFP")]
		NSString PortBluetoothHfp { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortUSBAudio")]
		NSString PortUsbAudio { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortBluetoothLE")]
		NSString PortBluetoothLE { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionPortCarAudio")]
		NSString PortCarAudio { get; }

		[Watch (7, 0), TV (14, 0), NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVAudioSessionPortAVB")]
		NSString PortAvb { get; }

		[Watch (7, 0), TV (14, 0), NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVAudioSessionPortDisplayPort")]
		NSString PortDisplayPort { get; }

		[Watch (7, 0), TV (14, 0), NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVAudioSessionPortFireWire")]
		NSString PortFireWire { get; }

		[Watch (7, 0), TV (14, 0), NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVAudioSessionPortPCI")]
		NSString PortPci { get; }

		[Watch (7, 0), TV (14, 0), NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVAudioSessionPortThunderbolt")]
		NSString PortThunderbolt { get; }

		[Watch (7, 0), TV (14, 0), NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVAudioSessionPortVirtual")]
		NSString PortVirtual { get; }

		[UnifiedInternal, Field ("AVAudioSessionLocationUpper")]
		NSString LocationUpper { get; }

		[UnifiedInternal, Field ("AVAudioSessionLocationLower")]
		NSString LocationLower { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("inputDataSources"), NullAllowed]
		AVAudioSessionDataSourceDescription [] InputDataSources { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("inputDataSource"), NullAllowed]
		AVAudioSessionDataSourceDescription InputDataSource { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("outputDataSources"), NullAllowed]
		AVAudioSessionDataSourceDescription [] OutputDataSources { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("outputDataSource"), NullAllowed]
		AVAudioSessionDataSourceDescription OutputDataSource { get; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setInputDataSource:error:")]
		[PostGet ("InputDataSource")]
		bool SetInputDataSource ([NullAllowed] AVAudioSessionDataSourceDescription dataSource, out NSError outError);

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setOutputDataSource:error:")]
		[PostGet ("OutputDataSource")]
		bool SetOutputDataSource ([NullAllowed] AVAudioSessionDataSourceDescription dataSource, out NSError outError);

		[NoTV, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("requestRecordPermission:")]
		void RequestRecordPermission (AVPermissionGranted responseCallback);

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setPreferredInput:error:")]
		bool SetPreferredInput ([NullAllowed] AVAudioSessionPortDescription inPort, out NSError outError);

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("preferredInput", ArgumentSemantic.Copy), NullAllowed]
		AVAudioSessionPortDescription PreferredInput { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("availableInputs")]
		AVAudioSessionPortDescription [] AvailableInputs { get; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setPreferredInputNumberOfChannels:error:")]
		bool SetPreferredInputNumberOfChannels (nint count, out NSError outError);

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("preferredInputNumberOfChannels")]
		nint GetPreferredInputNumberOfChannels ();

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setPreferredOutputNumberOfChannels:error:")]
		bool SetPreferredOutputNumberOfChannels (nint count, out NSError outError);

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("preferredOutputNumberOfChannels")]
		nint GetPreferredOutputNumberOfChannels ();

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("maximumInputNumberOfChannels")]
		nint MaximumInputNumberOfChannels { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("maximumOutputNumberOfChannels")]
		nint MaximumOutputNumberOfChannels { get; }

		[UnifiedInternal, Field ("AVAudioSessionOrientationTop")]
		NSString OrientationTop { get; }

		[UnifiedInternal, Field ("AVAudioSessionOrientationBottom")]
		NSString OrientationBottom { get; }

		[UnifiedInternal, Field ("AVAudioSessionOrientationFront")]
		NSString OrientationFront { get; }

		[UnifiedInternal, Field ("AVAudioSessionOrientationBack")]
		NSString OrientationBack { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionOrientationLeft")]
		NSString OrientationLeft { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionOrientationRight")]
		NSString OrientationRight { get; }

		[UnifiedInternal, Field ("AVAudioSessionPolarPatternOmnidirectional")]
		NSString PolarPatternOmnidirectional { get; }

		[UnifiedInternal, Field ("AVAudioSessionPolarPatternCardioid")]
		NSString PolarPatternCardioid { get; }

		[UnifiedInternal, Field ("AVAudioSessionPolarPatternSubcardioid")]
		NSString PolarPatternSubcardioid { get; }

		[NoWatch, NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVAudioSessionPolarPatternStereo")]
		NSString PolarPatternStereo { get; }

		// 8.0
		[NoTV, NoMac]
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Export ("recordPermission")]
		AVAudioSessionRecordPermission RecordPermission { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("secondaryAudioShouldBeSilencedHint")]
		bool SecondaryAudioShouldBeSilencedHint { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionSilenceSecondaryAudioHintNotification")]
		[Notification (typeof (AVAudioSessionSecondaryAudioHintEventArgs))]
		NSString SilenceSecondaryAudioHintNotification { get; }

		[NoWatch, NoTV, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setAggregatedIOPreference:error:")]
		bool SetAggregatedIOPreference (AVAudioSessionIOType ioType, out NSError error);

		[Watch (5, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setCategory:mode:routeSharingPolicy:options:error:")]
		bool SetCategory (string category, string mode, AVAudioSessionRouteSharingPolicy policy, AVAudioSessionCategoryOptions options, [NullAllowed] out NSError outError);

		[Watch (5, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("SetCategory (category.GetConstant ()!, mode, policy, options, out outError)")]
		bool SetCategory (AVAudioSessionCategory category, string mode, AVAudioSessionRouteSharingPolicy policy, AVAudioSessionCategoryOptions options, [NullAllowed] out NSError outError);

		[TV (11, 0), Watch (5, 0), iOS (11, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("SetCategory (category.GetConstant ()!, mode.GetConstant ()!, policy, options, out outError)")]
		bool SetCategory (AVAudioSessionCategory category, AVAudioSessionMode mode, AVAudioSessionRouteSharingPolicy policy, AVAudioSessionCategoryOptions options, [NullAllowed] out NSError outError);

		[TV (11, 0), Watch (5, 0), iOS (11, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("routeSharingPolicy")]
		AVAudioSessionRouteSharingPolicy RouteSharingPolicy { get; }

		[Async]
		[Watch (5, 0), NoTV, NoMac, NoiOS, MacCatalyst (15, 0)]
		[Export ("activateWithOptions:completionHandler:")]
		void Activate (AVAudioSessionActivationOptions options, Action<bool, NSError> handler);

		[Watch (5, 2), TV (12, 2), NoMac, iOS (12, 2)]
		[MacCatalyst (13, 1)]
		[Export ("promptStyle")]
		AVAudioSessionPromptStyle PromptStyle { get; }

		[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setAllowHapticsAndSystemSoundsDuringRecording:error:")]
		bool SetAllowHapticsAndSystemSoundsDuringRecording (bool inValue, [NullAllowed] out NSError outError);

		[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("allowHapticsAndSystemSoundsDuringRecording")]
		bool AllowHapticsAndSystemSoundsDuringRecording { get; }

		[NoWatch, NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("preferredInputOrientation")]
		AVAudioStereoOrientation PreferredInputOrientation { get; }

		[NoWatch, NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("setPreferredInputOrientation:error:")]
		bool SetPreferredInputOrientation (AVAudioStereoOrientation orientation, [NullAllowed] out NSError outError);

		[NoWatch, NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("inputOrientation")]
		AVAudioStereoOrientation InputOrientation { get; }

		[Watch (7, 4), TV (14, 5), NoMac, iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("setPrefersNoInterruptionsFromSystemAlerts:error:")]
		bool SetPrefersNoInterruptionsFromSystemAlerts (bool inValue, [NullAllowed] out NSError outError);

		[Watch (7, 4), TV (14, 5), NoMac, iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("prefersNoInterruptionsFromSystemAlerts")]
		bool PrefersNoInterruptionsFromSystemAlerts { get; }

		[Watch (8, 0), TV (15, 0), NoMac, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("setSupportsMultichannelContent:error:")]
		bool SetSupportsMultichannelContent (bool inValue, [NullAllowed] out NSError outError);

		[Watch (8, 0), TV (15, 0), NoMac, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("supportsMultichannelContent")]
		bool SupportsMultichannelContent { get; }

		[Notification (typeof (SpatialPlaybackCapabilitiesChangedEventArgs))]
		[Watch (8, 0), TV (15, 0), NoMac, iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("AVAudioSessionSpatialPlaybackCapabilitiesChangedNotification")]
		NSString SpatialPlaybackCapabilitiesChangedNotification { get; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	enum AVAudioSessionCategory {
		[Field ("AVAudioSessionCategoryAmbient")]
		Ambient,

		[Field ("AVAudioSessionCategorySoloAmbient")]
		SoloAmbient,

		[Field ("AVAudioSessionCategoryPlayback")]
		Playback,

		[Field ("AVAudioSessionCategoryRecord")]
		Record,

		[Field ("AVAudioSessionCategoryPlayAndRecord")]
		PlayAndRecord,

		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("AVAudioSessionCategoryAudioProcessing")]
		AudioProcessing,

		[Field ("AVAudioSessionCategoryMultiRoute")]
		MultiRoute,
	}

	[NoMac] // Apple's documentation says the enum is available on macOS, but none of the individual values are, so just don't expose the enum on macOS.
	enum AVAudioSessionMode {
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeDefault")]
		Default,

		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeVoiceChat")]
		VoiceChat,

		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeVideoRecording")]
		VideoRecording,

		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeMeasurement")]
		Measurement,

		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeGameChat")]
		GameChat,

		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeMoviePlayback")]
		MoviePlayback,

		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeVideoChat")]
		VideoChat,

		[iOS (9, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeSpokenAudio")]
		SpokenAudio,

		[Watch (5, 0), TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioSessionModeVoicePrompt")]
		VoicePrompt,
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAudioSessionDataSourceDescription {
		[Export ("dataSourceID")]
		NSNumber DataSourceID { get; }

		[Export ("dataSourceName")]
		string DataSourceName { get; }

		[Export ("location", ArgumentSemantic.Copy), NullAllowed]
		[Internal]
		NSString Location_ { get; }

		[Export ("orientation", ArgumentSemantic.Copy), NullAllowed]
		[Internal]
		NSString Orientation_ { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[UnifiedInternal, Export ("supportedPolarPatterns"), NullAllowed]
		NSString [] SupportedPolarPatterns { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[UnifiedInternal, Export ("selectedPolarPattern", ArgumentSemantic.Copy), NullAllowed]
		NSString SelectedPolarPattern { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[UnifiedInternal, Export ("preferredPolarPattern", ArgumentSemantic.Copy), NullAllowed]
		NSString PreferredPolarPattern { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[UnifiedInternal, Export ("setPreferredPolarPattern:error:")]
		bool SetPreferredPolarPattern ([NullAllowed] NSString pattern, out NSError outError);

	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	interface AVAudioSessionInterruptionEventArgs {
		[Export ("AVAudioSessionInterruptionTypeKey")]
		AVAudioSessionInterruptionType InterruptionType { get; }

		[Export ("AVAudioSessionInterruptionOptionKey")]
		AVAudioSessionInterruptionOptions Option { get; }

		[iOS (14, 5), Watch (7, 4), NoTV, NoMac]
		[MacCatalyst (14, 5)]
		[Export ("AVAudioSessionInterruptionReasonKey")]
		AVAudioSessionInterruptionReason Reason { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("AVAudioSessionInterruptionWasSuspendedKey")]
		bool WasSuspended { get; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	interface AVAudioSessionRouteChangeEventArgs {
		[Export ("AVAudioSessionRouteChangeReasonKey")]
		AVAudioSessionRouteChangeReason Reason { get; }

		[Export ("AVAudioSessionRouteChangePreviousRouteKey")]
		AVAudioSessionRouteDescription PreviousRoute { get; }
	}

	[NoMac]
	[Deprecated (PlatformName.iOS, 6, 0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	interface AVAudioSessionDelegate {
		[Export ("beginInterruption")]
		void BeginInterruption ();

		[Export ("endInterruption")]
		void EndInterruption ();

		[Export ("inputIsAvailableChanged:")]
		void InputIsAvailableChanged (bool isInputAvailable);

		[Export ("endInterruptionWithFlags:")]
#if NET
		void EndInterruption (AVAudioSessionInterruptionOptions flags);
#else
		void EndInterruption (AVAudioSessionInterruptionFlags flags);
#endif
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAudioSessionChannelDescription {
		[Export ("channelName")]
		string ChannelName { get; }

		[Export ("owningPortUID")]
		string OwningPortUID { get; }

		[Export ("channelNumber")]
		nint ChannelNumber { get; }

		[Export ("channelLabel")]
		int /* AudioChannelLabel = UInt32 */ ChannelLabel { get; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAudioSessionPortDescription {
		[Export ("portType")]
		NSString PortType { get; }

		[Export ("portName")]
		string PortName { get; }

		[Export ("UID")]
		string UID { get; }

		[MacCatalyst (13, 1)]
		[Export ("hasHardwareVoiceCallProcessing")]
		bool HasHardwareVoiceCallProcessing { get; }

		[Export ("channels"), NullAllowed]
		AVAudioSessionChannelDescription [] Channels { get; }

		[Export ("dataSources"), NullAllowed]
#if NET
		AVAudioSessionDataSourceDescription [] DataSources { get; }
#else
		AVAudioSessionDataSourceDescription [] DataSourceDescriptions { get; }
#endif

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("selectedDataSource", ArgumentSemantic.Copy), NullAllowed]
		AVAudioSessionDataSourceDescription SelectedDataSource { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("preferredDataSource", ArgumentSemantic.Copy), NullAllowed]
		AVAudioSessionDataSourceDescription PreferredDataSource { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("setPreferredDataSource:error:")]
		bool SetPreferredDataSource ([NullAllowed] AVAudioSessionDataSourceDescription dataSource, out NSError outError);

		[Watch (8, 0), TV (15, 0), NoMac, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("spatialAudioEnabled")]
		bool SpatialAudioEnabled { [Bind ("isSpatialAudioEnabled")] get; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAudioSessionRouteDescription {
		[Export ("inputs")]
		AVAudioSessionPortDescription [] Inputs { get; }

		[Export ("outputs")]
		AVAudioSessionPortDescription [] Outputs { get; }

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioNode))]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioUnit {
		[Export ("audioComponentDescription"), Internal]
		AudioComponentDescription AudioComponentDescription { get; }

		[Export ("audioUnit")]
		global::AudioUnit.AudioUnit AudioUnit { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("manufacturerName")]
		string ManufacturerName { get; }

		[Export ("version")]
		nuint Version { get; }

		[Export ("loadAudioUnitPresetAtURL:error:")]
		bool LoadAudioUnitPreset (NSUrl url, out NSError error);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("instantiateWithComponentDescription:options:completionHandler:")]
		[Async]
		void FromComponentDescription (AudioComponentDescription audioComponentDescription, AudioComponentInstantiationOptions options, Action<AVAudioUnit, NSError> completionHandler);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("AUAudioUnit")]
		AUAudioUnit AUAudioUnit { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioUnitEffect))]
	interface AVAudioUnitDelay {
		[Export ("delayTime")]
		double DelayTime { get; set; }

		[Export ("feedback")]
		float Feedback { get; set; } /* float, not CGFloat */

		[Export ("lowPassCutoff")]
		float LowPassCutoff { get; set; } /* float, not CGFloat */

		[Export ("wetDryMix")]
		float WetDryMix { get; set; } /* float, not CGFloat */
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioUnitEffect))]
	interface AVAudioUnitDistortion {
		[Export ("preGain")]
		float PreGain { get; set; } /* float, not CGFloat */

		[Export ("wetDryMix")]
		float WetDryMix { get; set; } /* float, not CGFloat */

		[Export ("loadFactoryPreset:")]
		void LoadFactoryPreset (AVAudioUnitDistortionPreset preset);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioUnit))]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioUnitEffect {
		[Export ("initWithAudioComponentDescription:")]
		NativeHandle Constructor (AudioComponentDescription audioComponentDescription);

		[Export ("bypass")]
		bool Bypass { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioUnitEffect))]
	interface AVAudioUnitEQ {
		[Export ("initWithNumberOfBands:")]
		NativeHandle Constructor (nuint numberOfBands);

		[Export ("bands")]
		AVAudioUnitEQFilterParameters [] Bands { get; }

		[Export ("globalGain")]
		float GlobalGain { get; set; } /* float, not CGFloat */
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioUnitEQFilterParameters {
		[Export ("filterType", ArgumentSemantic.Assign)]
		AVAudioUnitEQFilterType FilterType { get; set; }

		[Export ("frequency")]
		float Frequency { get; set; } /* float, not CGFloat */

		[Export ("bandwidth")]
		float Bandwidth { get; set; } /* float, not CGFloat */

		[Export ("gain")]
		float Gain { get; set; } /* float, not CGFloat */

		[Export ("bypass")]
		bool Bypass { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioUnit))]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioUnitGenerator : AVAudioMixing {
		[Export ("initWithAudioComponentDescription:")]
		NativeHandle Constructor (AudioComponentDescription audioComponentDescription);

		[Export ("bypass")]
		bool Bypass { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioUnit), Name = "AVAudioUnitMIDIInstrument")]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioUnitMidiInstrument : AVAudioMixing {
		[Export ("initWithAudioComponentDescription:")]
		NativeHandle Constructor (AudioComponentDescription audioComponentDescription);

		[Export ("startNote:withVelocity:onChannel:")]
		void StartNote (byte note, byte velocity, byte channel);

		[Export ("stopNote:onChannel:")]
		void StopNote (byte note, byte channel);

		[Export ("sendController:withValue:onChannel:")]
		void SendController (byte controller, byte value, byte channel);

		[Export ("sendPitchBend:onChannel:")]
		void SendPitchBend (ushort pitchbend, byte channel);

		[Export ("sendPressure:onChannel:")]
		void SendPressure (byte pressure, byte channel);

		[Export ("sendPressureForKey:withValue:onChannel:")]
		void SendPressureForKey (byte key, byte value, byte channel);

		[Export ("sendProgramChange:onChannel:")]
		void SendProgramChange (byte program, byte channel);

		[Export ("sendProgramChange:bankMSB:bankLSB:onChannel:")]
		void SendProgramChange (byte program, byte bankMSB, byte bankLSB, byte channel);

		[Export ("sendMIDIEvent:data1:data2:")]
		void SendMidiEvent (byte midiStatus, byte data1, byte data2);

		[Export ("sendMIDIEvent:data1:")]
		void SendMidiEvent (byte midiStatus, byte data1);

		[Export ("sendMIDISysExEvent:")]
		void SendMidiSysExEvent (NSData midiData);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioUnitMidiInstrument))]
	interface AVAudioUnitSampler {
		[Export ("stereoPan")]
		float StereoPan { get; set; } /* float, not CGFloat */

		[Deprecated (PlatformName.MacOSX, 12, 0)]
		[Deprecated (PlatformName.iOS, 15, 0)]
		[Deprecated (PlatformName.TvOS, 15, 0)]
		[Deprecated (PlatformName.WatchOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0)]
		[Export ("masterGain")]
		float MasterGain { get; set; } /* float, not CGFloat */

		[Export ("globalTuning")]
		float GlobalTuning { get; set; } /* float, not CGFloat */

		[Export ("loadSoundBankInstrumentAtURL:program:bankMSB:bankLSB:error:")]
		bool LoadSoundBank (NSUrl bankUrl, byte program, byte bankMSB, byte bankLSB, out NSError outError);

		[Export ("loadInstrumentAtURL:error:")]
		bool LoadInstrument (NSUrl instrumentUrl, out NSError outError);

		[Export ("loadAudioFilesAtURLs:error:")]
		bool LoadAudioFiles (NSUrl [] audioFiles, out NSError outError);

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("overallGain")]
		float OverallGain { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioUnitEffect))]
	interface AVAudioUnitReverb {

		[Export ("wetDryMix")]
		float WetDryMix { get; set; } /* float, not CGFloat */

		[Export ("loadFactoryPreset:")]
		void LoadFactoryPreset (AVAudioUnitReverbPreset preset);
	}


	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioUnit))]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioUnitTimeEffect {
		[Export ("initWithAudioComponentDescription:")]
		NativeHandle Constructor (AudioComponentDescription audioComponentDescription);

		[Export ("bypass")]
		bool Bypass { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioUnitTimeEffect))]
	interface AVAudioUnitTimePitch {
		[Export ("initWithAudioComponentDescription:")]
		NativeHandle Constructor (AudioComponentDescription audioComponentDescription);


		[Export ("rate")]
		float Rate { get; set; } /* float, not CGFloat */

		[Export ("pitch")]
		float Pitch { get; set; } /* float, not CGFloat */

		[Export ("overlap")]
		float Overlap { get; set; } /* float, not CGFloat */
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioUnitTimeEffect))]
	interface AVAudioUnitVarispeed {
		[Export ("initWithAudioComponentDescription:")]
		NativeHandle Constructor (AudioComponentDescription audioComponentDescription);

		[Export ("rate")]
		float Rate { get; set; } /* float, not CGFloat */
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAudioTime {
		[Export ("initWithAudioTimeStamp:sampleRate:")]
		NativeHandle Constructor (ref AudioTimeStamp timestamp, double sampleRate);

		[Export ("initWithHostTime:")]
		NativeHandle Constructor (ulong hostTime);

		[Export ("initWithSampleTime:atRate:")]
		NativeHandle Constructor (long sampleTime, double sampleRate);

		[Export ("initWithHostTime:sampleTime:atRate:")]
		NativeHandle Constructor (ulong hostTime, long sampleTime, double sampleRate);

		[Export ("hostTimeValid")]
		bool HostTimeValid { [Bind ("isHostTimeValid")] get; }

		[Export ("hostTime")]
		ulong HostTime { get; }

		[Export ("sampleTimeValid")]
		bool SampleTimeValid { [Bind ("isSampleTimeValid")] get; }

		[Export ("sampleTime")]
		long SampleTime { get; }

		[Export ("sampleRate")]
		double SampleRate { get; }

		[Export ("audioTimeStamp")]
		AudioTimeStamp AudioTimeStamp { get; }

		[Static, Export ("timeWithAudioTimeStamp:sampleRate:")]
		AVAudioTime FromAudioTimeStamp (ref AudioTimeStamp timestamp, double sampleRate);

		[Static, Export ("timeWithHostTime:")]
		AVAudioTime FromHostTime (ulong hostTime);

		[Static, Export ("timeWithSampleTime:atRate:")]
		AVAudioTime FromSampleTime (long sampleTime, double sampleRate);

		[Static, Export ("timeWithHostTime:sampleTime:atRate:")]
		AVAudioTime FromHostTime (ulong hostTime, long sampleTime, double sampleRate);

		[Static, Export ("hostTimeForSeconds:")]
		ulong HostTimeForSeconds (double seconds);

		[Static, Export ("secondsForHostTime:")]
		double SecondsForHostTime (ulong hostTime);

		[Export ("extrapolateTimeFromAnchor:")]
		[return: NullAllowed]
		AVAudioTime ExtrapolateTimeFromAnchor (AVAudioTime anchorTime);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Docs/headers do not state that init is disallowed but if 
						 // you get an instance that way and try to use it, it will inmediatelly crash also tested in ObjC app same result
	interface AVAudioConverter {

		[Export ("initFromFormat:toFormat:")]
		NativeHandle Constructor (AVAudioFormat fromFormat, AVAudioFormat toFormat);

		[Export ("reset")]
		void Reset ();

		[Export ("inputFormat")]
		AVAudioFormat InputFormat { get; }

		[Export ("outputFormat")]
		AVAudioFormat OutputFormat { get; }

		[Export ("channelMap", ArgumentSemantic.Retain)]
		NSNumber [] ChannelMap { get; set; }

		[NullAllowed, Export ("magicCookie", ArgumentSemantic.Retain)]
		NSData MagicCookie { get; set; }

		[Export ("downmix")]
		bool Downmix { get; set; }

		[Export ("dither")]
		bool Dither { get; set; }

		[Export ("sampleRateConverterQuality", ArgumentSemantic.Assign)]
		nint SampleRateConverterQuality { get; set; }

		[NullAllowed, Export ("sampleRateConverterAlgorithm", ArgumentSemantic.Retain)]
		string SampleRateConverterAlgorithm { get; set; }

		[Export ("primeMethod", ArgumentSemantic.Assign)]
		AVAudioConverterPrimeMethod PrimeMethod { get; set; }

		[Export ("primeInfo", ArgumentSemantic.Assign)]
		AVAudioConverterPrimeInfo PrimeInfo { get; set; }

		[Export ("convertToBuffer:fromBuffer:error:")]
		bool ConvertToBuffer (AVAudioPcmBuffer outputBuffer, AVAudioPcmBuffer inputBuffer, [NullAllowed] out NSError outError);

		[Export ("convertToBuffer:error:withInputFromBlock:")]
		AVAudioConverterOutputStatus ConvertToBuffer (AVAudioBuffer outputBuffer, [NullAllowed] out NSError outError, AVAudioConverterInputHandler inputHandler);

		// AVAudioConverter (Encoding) Category
		// Inlined due to properties

		[Export ("bitRate", ArgumentSemantic.Assign)]
		nint BitRate { get; set; }

		[NullAllowed, Export ("bitRateStrategy", ArgumentSemantic.Retain)]
		string BitRateStrategy { get; set; }

		[Export ("maximumOutputPacketSize")]
		nint MaximumOutputPacketSize { get; }

		[NullAllowed, Export ("availableEncodeBitRates")]
		NSNumber [] AvailableEncodeBitRates { get; }

		[NullAllowed, Export ("applicableEncodeBitRates")]
		NSNumber [] ApplicableEncodeBitRates { get; }

		[NullAllowed, Export ("availableEncodeSampleRates")]
		NSNumber [] AvailableEncodeSampleRates { get; }

		[NullAllowed, Export ("applicableEncodeSampleRates")]
		NSNumber [] ApplicableEncodeSampleRates { get; }

		[NullAllowed, Export ("availableEncodeChannelLayoutTags")]
		NSNumber [] AvailableEncodeChannelLayoutTags { get; }
	}

	[TV (11, 2), NoWatch, NoMac, NoiOS]
	[NoMacCatalyst]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface AVDisplayCriteria : NSCopying {
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** initialization method -init cannot be sent to an abstract object of class AVAsset: Create a concrete instance!
	[DisableDefaultCtor]
	interface AVAsset : NSCopying {
		[Export ("duration")]
		CMTime Duration { get; }

		[Export ("preferredRate")]
		float PreferredRate { get; } // defined as 'float'

		[Export ("preferredVolume")]
		float PreferredVolume { get; } // defined as 'float'

		[Export ("preferredTransform")]
		CGAffineTransform PreferredTransform { get; }

		[Export ("naturalSize")]
		[Deprecated (PlatformName.iOS, 5, 0, message: "Use 'NaturalSize/PreferredTransform' as appropriate on the video track instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NaturalSize/PreferredTransform' as appropriate on the video track instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Use 'NaturalSize/PreferredTransform' as appropriate on the video track instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NaturalSize/PreferredTransform' as appropriate on the video track instead.")]
		CGSize NaturalSize { get; }

		[TV (11, 2), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("preferredDisplayCriteria")]
		AVDisplayCriteria PreferredDisplayCriteria { get; }

		[Export ("providesPreciseDurationAndTiming")]
		bool ProvidesPreciseDurationAndTiming { get; }

		[Export ("cancelLoading")]
		void CancelLoading ();

		[Export ("tracks")]
		AVAssetTrack [] Tracks { get; }

		[return: NullAllowed]
		[Export ("trackWithTrackID:")]
		AVAssetTrack TrackWithTrackID (int /* CMPersistentTrackID = int32_t */ trackID);

		[Export ("tracksWithMediaType:")]
		AVAssetTrack [] TracksWithMediaType (string mediaType);

		[Wrap ("TracksWithMediaType (mediaType.GetConstant ())")]
		AVAssetTrack [] GetTracks (AVMediaTypes mediaType);

		[Export ("tracksWithMediaCharacteristic:")]
		AVAssetTrack [] TracksWithMediaCharacteristic (string mediaCharacteristic);

		[Wrap ("TracksWithMediaType (mediaCharacteristic.GetConstant ())")]
		AVAssetTrack [] GetTracks (AVMediaCharacteristics mediaCharacteristic);

		[Export ("lyrics"), NullAllowed]
		string Lyrics { get; }

		[Export ("commonMetadata")]
		AVMetadataItem [] CommonMetadata { get; }

		[Export ("availableMetadataFormats")]
		string [] AvailableMetadataFormats { get; }

#if !NET
		[Obsolete ("Use 'GetMetadataForFormat' with enum values AVMetadataFormat.")]
		[Wrap ("GetMetadataForFormat (new NSString (format))", IsVirtual = true)]
		AVMetadataItem [] MetadataForFormat (string format);
#endif
		[Export ("metadataForFormat:")]
		AVMetadataItem [] GetMetadataForFormat (NSString format);

		[Wrap ("GetMetadataForFormat (format.GetConstant ()!)")]
		AVMetadataItem [] GetMetadataForFormat (AVMetadataFormat format);

		[Export ("hasProtectedContent")]
		bool ProtectedContent { get; }

		[Export ("availableChapterLocales")]
		NSLocale [] AvailableChapterLocales { get; }

		[Export ("chapterMetadataGroupsWithTitleLocale:containingItemsWithCommonKeys:")]
		AVTimedMetadataGroup [] GetChapterMetadataGroups (NSLocale forLocale, [NullAllowed] AVMetadataItem [] commonKeys);

		[Export ("isPlayable")]
		bool Playable { get; }

		[Export ("isExportable")]
		bool Exportable { get; }

		[Export ("isReadable")]
		bool Readable { get; }

		[Export ("isComposable")]
		bool Composable { get; }

		// 5.0 APIs:
		[Static, Export ("assetWithURL:")]
		AVAsset FromUrl (NSUrl url);

		[Export ("availableMediaCharacteristicsWithMediaSelectionOptions")]
		string [] AvailableMediaCharacteristicsWithMediaSelectionOptions { get; }

		[NoMac]
		[MacCatalyst (14, 0)] // the headers lie, not usable until at least Mac Catalyst 14.0
		[Export ("compatibleWithSavedPhotosAlbum")]
		bool CompatibleWithSavedPhotosAlbum { [Bind ("isCompatibleWithSavedPhotosAlbum")] get; }

		[Export ("creationDate"), NullAllowed]
		AVMetadataItem CreationDate { get; }

		[Export ("referenceRestrictions")]
		AVAssetReferenceRestrictions ReferenceRestrictions { get; }

		[return: NullAllowed]
		[Export ("mediaSelectionGroupForMediaCharacteristic:")]
		AVMediaSelectionGroup MediaSelectionGroupForMediaCharacteristic (string avMediaCharacteristic);

		[Wrap ("MediaSelectionGroupForMediaCharacteristic (avMediaCharacteristic.GetConstant ()!)")]
		[return: NullAllowed]
		AVMediaSelectionGroup GetMediaSelectionGroupForMediaCharacteristic (AVMediaCharacteristics avMediaCharacteristic);

		[Export ("statusOfValueForKey:error:")]
		AVKeyValueStatus StatusOfValue (string key, out NSError error);

		[Export ("loadValuesAsynchronouslyForKeys:completionHandler:")]
		[Async ("LoadValuesTaskAsync")]
		void LoadValuesAsynchronously (string [] keys, Action handler);

		[Export ("chapterMetadataGroupsBestMatchingPreferredLanguages:")]
		AVTimedMetadataGroup [] GetChapterMetadataGroupsBestMatchingPreferredLanguages (string [] languages);

		[MacCatalyst (13, 1)]
		[Export ("trackGroups", ArgumentSemantic.Copy)]
		AVAssetTrackGroup [] TrackGroups { get; }

		[MacCatalyst (13, 1)]
		[Export ("metadata")]
		AVMetadataItem [] Metadata { get; }

		[Export ("unusedTrackID")]
		int /* CMPersistentTrackID -> int32_t */ UnusedTrackId { get; }  // TODO: wrong name, should have benn UnusedTrackID

		[MacCatalyst (13, 1)]
		[Export ("preferredMediaSelection")]
		AVMediaSelection PreferredMediaSelection { get; }

		// AVAsset (AVAssetFragments) Category
		// This is being inlined because there are no property extensions

		[MacCatalyst (13, 1)]
		[Export ("canContainFragments")]
		bool CanContainFragments { get; }

		[MacCatalyst (13, 1)]
		[Export ("containsFragments")]
		bool ContainsFragments { get; }

		[MacCatalyst (13, 1)]
		[Export ("compatibleWithAirPlayVideo")]
		bool CompatibleWithAirPlayVideo { [Bind ("isCompatibleWithAirPlayVideo")] get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAssetDurationDidChangeNotification")]
		[Notification]
		NSString DurationDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAssetChapterMetadataGroupsDidChangeNotification")]
		[Notification]
		NSString ChapterMetadataGroupsDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Notification, Field ("AVAssetMediaSelectionGroupsDidChangeNotification")]
		NSString MediaSelectionGroupsDidChangeNotification { get; }

		[TV (12, 0), NoWatch, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVAssetContainsFragmentsDidChangeNotification")]
		[Notification]
		NSString ContainsFragmentsDidChangeNotification { get; }

		[TV (12, 0), NoWatch, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVAssetWasDefragmentedNotification")]
		[Notification]
		NSString WasDefragmentedNotification { get; }

		[MacCatalyst (13, 1)]
		[Export ("overallDurationHint")]
		CMTime OverallDurationHint { get; }

		[MacCatalyst (13, 1)]
		[Export ("allMediaSelections")]
		AVMediaSelection [] AllMediaSelections { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("minimumTimeOffsetFromLive")]
		CMTime MinimumTimeOffsetFromLive { get; }

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("findUnusedTrackIDWithCompletionHandler:")]
		void FindUnusedTrackId (Action<int, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadChapterMetadataGroupsBestMatchingPreferredLanguages:completionHandler:")]
		void LoadChapterMetadataGroups (string [] bestMatchingPreferredLanguages, Action<NSArray<AVTimedMetadataGroup>, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadChapterMetadataGroupsWithTitleLocale:containingItemsWithCommonKeys:completionHandler:")]
		void LoadChapterMetadataGroups (NSLocale titleLocale, string [] commonKeys, Action<NSArray<AVTimedMetadataGroup>, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadMediaSelectionGroupForMediaCharacteristic:completionHandler:")]
		void LoadMediaSelectionGroup (string mediaCharacteristic, Action<AVMediaSelectionGroup, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadMetadataForFormat:completionHandler:")]
		void LoadMetadata (string format, Action<NSArray<AVMetadataItem>, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTrackWithTrackID:completionHandler:")]
		void LoadTrack (int trackId, Action<AVCompositionTrack, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaCharacteristic:completionHandler:")]
		void LoadTrackWithMediaCharacteristics (string mediaCharacteristic, Action<NSArray<AVCompositionTrack>, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaType:completionHandler:")]
		void LoadTracksWithMediaType (string mediaType, Action<NSArray<AVMutableCompositionTrack>, NSError> completionHandler);
	}

	interface IAVFragmentMinding { }

	[Protocol]
	[iOS (12, 0), TV (12, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	interface AVFragmentMinding {

#if !MONOMAC || NET
		[Abstract] // not kept in Mac OS because is a breaking change, in other platforms we are ok
#endif
		[Export ("isAssociatedWithFragmentMinder")]
		bool IsAssociatedWithFragmentMinder ();
	}

	[iOS (12, 0), TV (12, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (AVUrlAsset))]
	interface AVFragmentedAsset : AVFragmentMinding {

		[Export ("initWithURL:options:")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] NSDictionary options);

		[Static]
		[Export ("fragmentedAssetWithURL:options:")]
		AVFragmentedAsset FromUrl (NSUrl url, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("tracks")]
		AVFragmentedAssetTrack [] Tracks { get; }
	}

	[iOS (12, 0), TV (12, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVFragmentedAsset))]
	interface AVFragmentedAsset_AVFragmentedAssetTrackInspection {

		[Export ("trackWithTrackID:")]
		[return: NullAllowed]
		AVFragmentedAssetTrack GetTrack (int trackID);

		[Export ("tracksWithMediaType:")]
		AVFragmentedAssetTrack [] GetTracks (string mediaType);

		[Wrap ("This.GetTracks (mediaType.GetConstant ())")]
		AVFragmentedAssetTrack [] GetTracks (AVMediaTypes mediaType);

		[Export ("tracksWithMediaCharacteristic:")]
		AVFragmentedAssetTrack [] GetTracksWithMediaCharacteristic (string mediaCharacteristic);

		[Wrap ("This.GetTracksWithMediaCharacteristic (mediaCharacteristic.GetConstant ())")]
		AVFragmentedAssetTrack [] GetTracks (AVMediaCharacteristics mediaCharacteristic);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTrackWithTrackID:completionHandler:")]
		void LoadTrack (int trackId, Action<AVFragmentedAssetTrack, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaType:completionHandler:")]
		void LoadTracksWithMediaType (string mediaType, Action<NSArray<AVFragmentedAssetTrack>, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaCharacteristic:completionHandler:")]
		void LoadTracksWithMediaCharacteristic (string mediaCharacteristic, Action<NSArray<AVFragmentedAssetTrack>, NSError> completionHandler);

	}

	[iOS (12, 0), TV (12, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVFragmentedAssetMinder {

		[Static]
		[Export ("fragmentedAssetMinderWithAsset:mindingInterval:")]
		AVFragmentedAssetMinder FromAsset (AVAsset asset, double mindingInterval);

		[MacCatalyst (13, 1)]
		[Export ("initWithAsset:mindingInterval:")]
		NativeHandle Constructor (IAVFragmentMinding asset, double mindingInterval);

		[Export ("mindingInterval")]
		double MindingInterval { get; set; }

		[Export ("assets")]
		AVAsset [] Assets { get; }

		[Export ("addFragmentedAsset:")]
		void AddFragmentedAsset (AVAsset asset);

		[Export ("removeFragmentedAsset:")]
		void RemoveFragmentedAsset (AVAsset asset);
	}

	[iOS (12, 0), TV (12, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (AVAssetTrack))]
	interface AVFragmentedAssetTrack {
	}

	interface IAVCaptureFileOutputDelegate { }

	[NoWatch]
	[NoiOS]
	[NoTV]
	[Unavailable (PlatformName.MacCatalyst)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVCaptureFileOutputDelegate {
		[Abstract]
		[Export ("captureOutputShouldProvideSampleAccurateRecordingStart:")]
		bool ShouldProvideSampleAccurateRecordingStart (AVCaptureOutput captureOutput);

		[Export ("captureOutput:didOutputSampleBuffer:fromConnection:")]
		void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection);
	}

#if NET
	// Making a class abstract has problems: https://github.com/xamarin/xamarin-macios/issues/4969, so we're not doing this yet
	// [Abstract] // Abstract superclass.
#endif
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureSynchronizedData {
		[Export ("timestamp")]
		CMTime Timestamp { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureSynchronizedDataCollection : INSFastEnumeration {
#if !NET
		[Obsolete ("Use 'GetSynchronizedData' instead.")]
		[Wrap ("GetSynchronizedData (captureOutput)", isVirtual: true)]
		[return: NullAllowed]
		AVCaptureSynchronizedData From (AVCaptureOutput captureOutput);

		// This is not reexposed because it is not needed you can use 'GetSynchronizedData' instead, also from docs:
		// https://developer.apple.com/documentation/avfoundation/avcapturesynchronizeddatacollection/2873892-objectforkeyedsubscript?language=objc
		// > This call is equivalent to the synchronizedDataForCaptureOutput: method, but allows subscript syntax.
		[Obsolete ("Use 'GetSynchronizedData' instead.")]
		[Export ("objectForKeyedSubscript:")]
		[return: NullAllowed]
		AVCaptureSynchronizedData ObjectForKeyedSubscript (AVCaptureOutput key);
#endif

		[Export ("synchronizedDataForCaptureOutput:")]
		[return: NullAllowed]
		AVCaptureSynchronizedData GetSynchronizedData (AVCaptureOutput captureOutput);

		[Export ("count")]
		nuint Count { get; }
	}

	interface IAVCaptureDataOutputSynchronizerDelegate { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV]
	[NoMac]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVCaptureDataOutputSynchronizerDelegate {
		[Abstract]
		[Export ("dataOutputSynchronizer:didOutputSynchronizedDataCollection:")]
		void DidOutputSynchronizedDataCollection (AVCaptureDataOutputSynchronizer synchronizer, AVCaptureSynchronizedDataCollection synchronizedDataCollection);
	}

	[NoWatch, NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureDataOutputSynchronizer {
		[Export ("initWithDataOutputs:")]
		NativeHandle Constructor (AVCaptureOutput [] dataOutputs);

		[Export ("dataOutputs", ArgumentSemantic.Retain)]
		AVCaptureOutput [] DataOutputs { get; }

		[Export ("setDelegate:queue:")]
		void SetDelegate ([NullAllowed] IAVCaptureDataOutputSynchronizerDelegate del, [NullAllowed] DispatchQueue delegateCallbackQueue);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IAVCaptureDataOutputSynchronizerDelegate Delegate { get; }

		[NullAllowed, Export ("delegate")]
		NSObject WeakDelegate { get; }

		[NullAllowed, Export ("delegateCallbackQueue")]
		DispatchQueue DelegateCallbackQueue { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoMac, NoWatch, NoTV]
	[BaseType (typeof (AVCaptureSynchronizedData))]
	interface AVCaptureSynchronizedSampleBufferData {
		[Export ("sampleBuffer")]
		CMSampleBuffer SampleBuffer { get; }

		[Export ("sampleBufferWasDropped")]
		bool SampleBufferWasDropped { get; }

		[Export ("droppedReason")]
		AVCaptureOutputDataDroppedReason DroppedReason { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoMac, NoWatch, NoTV]
	[BaseType (typeof (AVCaptureSynchronizedData))]
	interface AVCaptureSynchronizedMetadataObjectData {
		[Export ("metadataObjects")]
		AVMetadataObject [] MetadataObjects { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoMac, NoWatch, NoTV]
	[BaseType (typeof (AVCaptureSynchronizedData))]
	[DisableDefaultCtor]
	interface AVCaptureSynchronizedDepthData {
		[Export ("depthData")]
		AVDepthData DepthData { get; }

		[Export ("depthDataWasDropped")]
		bool DepthDataWasDropped { get; }

		[Export ("droppedReason")]
		AVCaptureOutputDataDroppedReason DroppedReason { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface AVQueuedSampleBufferRendering {
		[Abstract]
		[Export ("timebase", ArgumentSemantic.Retain)]
		CMTimebase Timebase { get; }

		[Abstract]
		[Export ("enqueueSampleBuffer:")]
		void Enqueue (CMSampleBuffer sampleBuffer);

		[Abstract]
		[Export ("flush")]
		void Flush ();

		[Abstract]
		[Export ("readyForMoreMediaData")]
		bool ReadyForMoreMediaData { [Bind ("isReadyForMoreMediaData")] get; }

		[Abstract]
		[Export ("requestMediaDataWhenReadyOnQueue:usingBlock:")]
		void RequestMediaData (DispatchQueue queue, Action handler);

		[Abstract]
		[Export ("stopRequestingMediaData")]
		void StopRequestingMediaData ();

#if NET
		[Abstract]
#endif
		[TV (14, 5), Watch (7, 4), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("hasSufficientMediaDataForReliablePlaybackStart")]
		bool HasSufficientMediaDataForReliablePlaybackStart { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVSampleBufferAudioRenderer : AVQueuedSampleBufferRendering {
		[Export ("status")]
		AVQueuedSampleBufferRenderingStatus Status { get; }

		[NullAllowed, Export ("error")]
		NSError Error { get; }

		[NullAllowed, Export ("audioOutputDeviceUniqueID"), NoWatch, NoTV, NoiOS, MacCatalyst (15, 0)]
		string AudioOutputDeviceUniqueId { get; set; }

		[Export ("audioTimePitchAlgorithm")]
		string PitchAlgorithm { get; set; }

		// AVSampleBufferAudioRenderer_AVSampleBufferAudioRendererVolumeControl
		[Export ("volume")]
		float Volume { get; set; }

		[Export ("muted")]
		bool Muted { [Bind ("isMuted")] get; set; }

		// AVSampleBufferAudioRenderer_AVSampleBufferAudioRendererQueueManagement

		[Async]
		[Export ("flushFromSourceTime:completionHandler:")]
		void Flush (CMTime time, Action<bool> completionHandler);

		[Notification (typeof (AudioRendererWasFlushedAutomaticallyEventArgs))]
		[Field ("AVSampleBufferAudioRendererWasFlushedAutomaticallyNotification")]
		NSString AudioRendererWasFlushedAutomaticallyNotification { get; }

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("allowedAudioSpatializationFormats", ArgumentSemantic.Assign)]
		AVAudioSpatializationFormats AllowedAudioSpatializationFormats { get; set; }

		[Notification]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("AVSampleBufferAudioRendererOutputConfigurationDidChangeNotification")]
		NSString ConfigurationDidChangeNotification { get; }

	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	interface AudioRendererWasFlushedAutomaticallyEventArgs {
		[Internal]
		[Export ("AVSampleBufferAudioRendererFlushTimeKey")]
		NSValue _AudioRendererFlushTime { get; set; }
	}

	interface IAVQueuedSampleBufferRendering { }

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVSampleBufferRenderSynchronizer {
		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVSampleBufferRenderSynchronizerRateDidChangeNotification")]
		[Notification]
		NSString RateDidChangeNotification { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("currentTime")]
		CMTime CurrentTime { get; }

		[Export ("timebase", ArgumentSemantic.Retain)]
		CMTimebase Timebase { get; }

		[Export ("rate")]
		float Rate { get; set; }

		[Export ("setRate:time:")]
		void SetRate (float rate, CMTime time);

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("setRate:time:atHostTime:")]
		void SetRate (float rate, CMTime time, CMTime hostTime);

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("delaysRateChangeUntilHasSufficientMediaData")]
		bool DelaysRateChangeUntilHasSufficientMediaData { get; set; }

		// AVSampleBufferRenderSynchronizer_AVSampleBufferRenderSynchronizerRendererManagement

		[Export ("renderers")]
		IAVQueuedSampleBufferRendering [] Renderers { get; }

		[Export ("addRenderer:")]
		void Add (IAVQueuedSampleBufferRendering renderer);

		[Async]
		[Export ("removeRenderer:atTime:completionHandler:")]
		void Remove (IAVQueuedSampleBufferRendering renderer, CMTime time, [NullAllowed] Action<bool> completionHandler);

		// AVSampleBufferRenderSynchronizer_AVSampleBufferRenderSynchronizerTimeObservation

		// as per the docs the returned observers are an opaque object that you pass as the argument to 
		// removeTimeObserver to cancel observation.

		// Regarding async usage:
		// The delegate can be called multiple times (once for each value in the times array according to the documentation),
		// which makes it a bad fit for [Async]

		// [Async] -> not added due to comment above
		[Export ("addPeriodicTimeObserverForInterval:queue:usingBlock:")]
		NSObject AddPeriodicTimeObserver (CMTime interval, [NullAllowed] DispatchQueue queue, Action<CMTime> handler);

		// [Async] -> not added due to comment above
		[Export ("addBoundaryTimeObserverForTimes:queue:usingBlock:")]
		NSObject AddBoundaryTimeObserver (NSValue [] times, [NullAllowed] DispatchQueue queue, Action handler);

		[Export ("removeTimeObserver:")]
		void RemoveTimeObserver (NSObject observer);
	}

	[NoTV, NoiOS, NoWatch, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVSampleBufferGenerator {

		[Export ("initWithAsset:timebase:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVAsset asset, [NullAllowed] CMTimebase timebase);

		[Export ("createSampleBufferForRequest:")]
		[return: Release]
		[return: NullAllowed]
		CMSampleBuffer CreateSampleBuffer (AVSampleBufferRequest request);

		[Static]
		[Async]
		[Export ("notifyOfDataReadyForSampleBuffer:completionHandler:")]
		void NotifyOfDataReady (CMSampleBuffer sbuf, Action<bool, NSError> completionHandler);
	}

	[NoTV, NoiOS, NoWatch, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVSampleBufferRequest {

		[Export ("initWithStartCursor:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVSampleCursor startCursor);

		[Export ("startCursor", ArgumentSemantic.Retain)]
		AVSampleCursor StartCursor { get; }

		[Export ("direction", ArgumentSemantic.Assign)]
		AVSampleBufferRequestDirection Direction { get; set; }

		[Export ("limitCursor", ArgumentSemantic.Retain), NullAllowed]
		AVSampleCursor LimitCursor { get; set; }

		[Export ("preferredMinSampleCount", ArgumentSemantic.Assign)]
		nint PreferredMinSampleCount { get; set; }

		[Export ("maxSampleCount", ArgumentSemantic.Assign)]
		nint MaxSampleCount { get; set; }

		[Export ("mode", ArgumentSemantic.Assign)]
		AVSampleBufferRequestMode Mode { get; set; }

		[Export ("overrideTime", ArgumentSemantic.Assign)]
		CMTime OverrideTime { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// <quote>You create an asset generator using initWithAsset: or assetImageGeneratorWithAsset:</quote> http://developer.apple.com/library/ios/#documentation/AVFoundation/Reference/AVAssetImageGenerator_Class/Reference/Reference.html
	// calling 'init' returns a NIL handle
	[DisableDefaultCtor]
	interface AVAssetImageGenerator {
		[Export ("maximumSize", ArgumentSemantic.Assign)]
		CGSize MaximumSize { get; set; }

		[Export ("apertureMode", ArgumentSemantic.Copy), NullAllowed]
		NSString ApertureMode { get; set; }

		[Export ("videoComposition", ArgumentSemantic.Copy), NullAllowed]
		AVVideoComposition VideoComposition { get; set; }

		[Export ("appliesPreferredTrackTransform")]
		bool AppliesPreferredTrackTransform { get; set; }

		[Static]
		[Export ("assetImageGeneratorWithAsset:")]
		AVAssetImageGenerator FromAsset (AVAsset asset);

		[DesignatedInitializer]
		[Export ("initWithAsset:")]
		NativeHandle Constructor (AVAsset asset);

		[Deprecated (PlatformName.MacOSX, 13, 0, message: "Use the 'GenerateCGImagesAsynchronously' method instead.")]
		[Deprecated (PlatformName.TvOS, 16, 0, message: "Use the 'GenerateCGImagesAsynchronously' method instead.")]
		[Deprecated (PlatformName.iOS, 16, 0, message: "Use the 'GenerateCGImagesAsynchronously' method instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use the 'GenerateCGImagesAsynchronously' method instead.")]
		[Export ("copyCGImageAtTime:actualTime:error:")]
		[return: NullAllowed]
		[return: Release ()]
		CGImage CopyCGImageAtTime (CMTime requestedTime, out CMTime actualTime, out NSError outError);

		[Export ("generateCGImagesAsynchronouslyForTimes:completionHandler:")]
		void GenerateCGImagesAsynchronously (NSValue [] cmTimesRequestedTimes, AVAssetImageGeneratorCompletionHandler handler);

#if !XAMCORE_5_0
		[Sealed]
		[Export ("generateCGImagesAsynchronouslyForTimes:completionHandler:")]
		void GenerateCGImagesAsynchronously (NSValue [] cmTimesRequestedTimes, AVAssetImageGeneratorCompletionHandler2 handler);
#endif

		[iOS (16, 0)]
		[Mac (13, 0)]
		[MacCatalyst (16, 0)]
		[TV (16, 0)]
		[Export ("generateCGImageAsynchronouslyForTime:completionHandler:")]
		void GenerateCGImageAsynchronously (CMTime requestedTime, AVAssetImageGenerateAsynchronouslyForTimeCompletionHandler handler);

		[Export ("cancelAllCGImageGeneration")]
		void CancelAllCGImageGeneration ();

		[Field ("AVAssetImageGeneratorApertureModeCleanAperture")]
		NSString ApertureModeCleanAperture { get; }

		[Field ("AVAssetImageGeneratorApertureModeProductionAperture")]
		NSString ApertureModeProductionAperture { get; }

		[Field ("AVAssetImageGeneratorApertureModeEncodedPixels")]
		NSString ApertureModeEncodedPixels { get; }

		// 5.0 APIs
		[Export ("requestedTimeToleranceBefore", ArgumentSemantic.Assign)]
		CMTime RequestedTimeToleranceBefore { get; set; }

		[Export ("requestedTimeToleranceAfter", ArgumentSemantic.Assign)]
		CMTime RequestedTimeToleranceAfter { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("asset")]
		AVAsset Asset { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("customVideoCompositor", ArgumentSemantic.Copy), NullAllowed]
		[Protocolize]
		AVVideoCompositing CustomVideoCompositor { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetReader initWithAsset:error:] invalid parameter not satisfying: asset != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetReader {
		[Export ("asset", ArgumentSemantic.Retain)]
		AVAsset Asset { get; }

		[Export ("status")]
		AVAssetReaderStatus Status { get; }

		[Export ("error"), NullAllowed]
		NSError Error { get; }

		[Export ("timeRange")]
		CMTimeRange TimeRange { get; set; }

		[Export ("outputs")]
		AVAssetReaderOutput [] Outputs { get; }

		[return: NullAllowed]
		[Static, Export ("assetReaderWithAsset:error:")]
		AVAssetReader FromAsset (AVAsset asset, out NSError error);

		[DesignatedInitializer]
		[Export ("initWithAsset:error:")]
		NativeHandle Constructor (AVAsset asset, out NSError error);

		[Export ("canAddOutput:")]
		bool CanAddOutput (AVAssetReaderOutput output);

		[Export ("addOutput:")]
		void AddOutput (AVAssetReaderOutput output);

		[Export ("startReading")]
		bool StartReading ();

		[Export ("cancelReading")]
		void CancelReading ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** initialization method -init cannot be sent to an abstract object of class AVAssetReaderOutput: Create a concrete instance!
	[DisableDefaultCtor]
	interface AVAssetReaderOutput {
		[Export ("mediaType")]
		string MediaType { get; }

		[return: NullAllowed, Release]
		[Export ("copyNextSampleBuffer")]
		CMSampleBuffer CopyNextSampleBuffer ();

		[Export ("alwaysCopiesSampleData")]
		bool AlwaysCopiesSampleData { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("supportsRandomAccess")]
		bool SupportsRandomAccess { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("resetForReadingTimeRanges:")]
		void ResetForReadingTimeRanges (NSValue [] timeRanges);

		[MacCatalyst (13, 1)]
		[Export ("markConfigurationAsFinal")]
		void MarkConfigurationAsFinal ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: *** -[AVAssetReaderOutputMetadataAdaptor initWithAssetReaderTrackOutput:] invalid parameter not satisfying: trackOutput != ((void*)0)
	interface AVAssetReaderOutputMetadataAdaptor {

		[DesignatedInitializer]
		[Export ("initWithAssetReaderTrackOutput:")]
		NativeHandle Constructor (AVAssetReaderTrackOutput trackOutput);

		[Export ("assetReaderTrackOutput")]
		AVAssetReaderTrackOutput AssetReaderTrackOutput { get; }

		[Static, Export ("assetReaderOutputMetadataAdaptorWithAssetReaderTrackOutput:")]
		AVAssetReaderOutputMetadataAdaptor Create (AVAssetReaderTrackOutput trackOutput);

		[return: NullAllowed]
		[Export ("nextTimedMetadataGroup")]
		AVTimedMetadataGroup NextTimedMetadataGroup ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAssetReaderOutput))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: *** -[AVAssetReaderSampleReferenceOutput initWithTrack:] invalid parameter not satisfying: track != ((void*)0)
	interface AVAssetReaderSampleReferenceOutput {

		[DesignatedInitializer]
		[Export ("initWithTrack:")]
		NativeHandle Constructor (AVAssetTrack track);

		[Export ("track")]
		AVAssetTrack Track { get; }

		[Static, Export ("assetReaderSampleReferenceOutputWithTrack:")]
		AVAssetReaderSampleReferenceOutput Create (AVAssetTrack track);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAssetReaderOutput))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetReaderTrackOutput initWithTrack:outputSettings:] invalid parameter not satisfying: track != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetReaderTrackOutput {
		[Export ("track")]
		AVAssetTrack Track { get; }

		[Internal]
		[Advice ("Use 'Create' method.")]
		[Static, Export ("assetReaderTrackOutputWithTrack:outputSettings:")]
		AVAssetReaderTrackOutput FromTrack (AVAssetTrack track, [NullAllowed] NSDictionary outputSettings);

		[Static, Wrap ("FromTrack (track, settings.GetDictionary ())")]
		AVAssetReaderTrackOutput Create (AVAssetTrack track, [NullAllowed] AudioSettings settings);

		[Static, Wrap ("FromTrack (track, settings.GetDictionary ())")]
		AVAssetReaderTrackOutput Create (AVAssetTrack track, [NullAllowed] AVVideoSettingsUncompressed settings);

		[DesignatedInitializer]
		[Export ("initWithTrack:outputSettings:")]
		NativeHandle Constructor (AVAssetTrack track, [NullAllowed] NSDictionary outputSettings);

		[Wrap ("this (track, settings.GetDictionary ())")]
		NativeHandle Constructor (AVAssetTrack track, [NullAllowed] AudioSettings settings);

		[Wrap ("this (track, settings.GetDictionary ())")]
		NativeHandle Constructor (AVAssetTrack track, [NullAllowed] AVVideoSettingsUncompressed settings);

		[Export ("outputSettings"), NullAllowed]
		NSDictionary OutputSettings { get; }

		[MacCatalyst (13, 1)]
		[Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		// DOC: this is a AVAudioTimePitch value
		NSString AudioTimePitchAlgorithm { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAssetReaderOutput))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetReaderAudioMixOutput initWithAudioTracks:audioSettings:] invalid parameter not satisfying: audioTracks != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetReaderAudioMixOutput {
		[Export ("audioTracks")]
		AVAssetTrack [] AudioTracks { get; }

		[Export ("audioMix", ArgumentSemantic.Copy), NullAllowed]
		AVAudioMix AudioMix { get; set; }

		[Internal]
		[Advice ("Use 'Create' method.")]
		[Static, Export ("assetReaderAudioMixOutputWithAudioTracks:audioSettings:")]
		AVAssetReaderAudioMixOutput FromTracks (AVAssetTrack [] audioTracks, [NullAllowed] NSDictionary audioSettings);

		[Wrap ("FromTracks (audioTracks, settings.GetDictionary ())")]
		AVAssetReaderAudioMixOutput Create (AVAssetTrack [] audioTracks, [NullAllowed] AudioSettings settings);

		[DesignatedInitializer]
		[Export ("initWithAudioTracks:audioSettings:")]
		NativeHandle Constructor (AVAssetTrack [] audioTracks, [NullAllowed] NSDictionary audioSettings);

		[Wrap ("this (audioTracks, settings.GetDictionary ())")]
		NativeHandle Constructor (AVAssetTrack [] audioTracks, [NullAllowed] AudioSettings settings);

		[Internal]
		[Advice ("Use 'Settings' property.")]
		[Export ("audioSettings"), NullAllowed]
		NSDictionary AudioSettings { get; }

		[Wrap ("AudioSettings"), NullAllowed]
		AudioSettings Settings { get; }

		[MacCatalyst (13, 1)]
		[Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		// This is an AVAudioTimePitch constant
		NSString AudioTimePitchAlgorithm { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAssetReaderOutput))]
	// crash application if 'init' is called
	[DisableDefaultCtor]
	interface AVAssetReaderVideoCompositionOutput {
		[Export ("videoTracks")]
		AVAssetTrack [] VideoTracks { get; }

		[Export ("videoComposition", ArgumentSemantic.Copy), NullAllowed]
		AVVideoComposition VideoComposition { get; set; }

		[Internal]
		[Advice ("Use 'Create' method.")]
		[Static]
		[Export ("assetReaderVideoCompositionOutputWithVideoTracks:videoSettings:")]
		AVAssetReaderVideoCompositionOutput WeakFromTracks (AVAssetTrack [] videoTracks, [NullAllowed] NSDictionary videoSettings);

		[Wrap ("WeakFromTracks (videoTracks, settings.GetDictionary ())")]
		[Static]
		AVAssetReaderVideoCompositionOutput Create (AVAssetTrack [] videoTracks, [NullAllowed] CVPixelBufferAttributes settings);

		[DesignatedInitializer]
		[Export ("initWithVideoTracks:videoSettings:")]
		NativeHandle Constructor (AVAssetTrack [] videoTracks, [NullAllowed] NSDictionary videoSettings);

		[Wrap ("this (videoTracks, settings.GetDictionary ())")]
		NativeHandle Constructor (AVAssetTrack [] videoTracks, [NullAllowed] CVPixelBufferAttributes settings);

		[Export ("videoSettings"), NullAllowed]
		NSDictionary WeakVideoSettings { get; }

		[Wrap ("WeakVideoSettings"), NullAllowed]
		CVPixelBufferAttributes UncompressedVideoSettings { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("customVideoCompositor", ArgumentSemantic.Copy), NullAllowed]
		[Protocolize]
		AVVideoCompositing CustomVideoCompositor { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no valid handle, docs now says "You do not create resource loader objects yourself."
	interface AVAssetResourceLoader {
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		[Protocolize]
		AVAssetResourceLoaderDelegate Delegate { get; }

		[Export ("delegateQueue"), NullAllowed]
		DispatchQueue DelegateQueue { get; }

		[Export ("setDelegate:queue:")]
		void SetDelegate ([Protocolize][NullAllowed] AVAssetResourceLoaderDelegate resourceLoaderDelegate, [NullAllowed] DispatchQueue delegateQueue);

		// AVAssetResourceLoader (AVAssetResourceLoaderContentKeySupport) Category
		[MacCatalyst (13, 1)]
		[Export ("preloadsEligibleContentKeys")]
		bool PreloadsEligibleContentKeys { get; set; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVAssetResourceLoaderDelegate {
#if !NET
		[Abstract]
#endif
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("resourceLoader:shouldWaitForLoadingOfRequestedResource:")]
		bool ShouldWaitForLoadingOfRequestedResource (AVAssetResourceLoader resourceLoader, AVAssetResourceLoadingRequest loadingRequest);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("resourceLoader:didCancelLoadingRequest:")]
		void DidCancelLoadingRequest (AVAssetResourceLoader resourceLoader, AVAssetResourceLoadingRequest loadingRequest);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("resourceLoader:shouldWaitForResponseToAuthenticationChallenge:")]
		bool ShouldWaitForResponseToAuthenticationChallenge (AVAssetResourceLoader resourceLoader, NSUrlAuthenticationChallenge authenticationChallenge);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("resourceLoader:didCancelAuthenticationChallenge:")]
		void DidCancelAuthenticationChallenge (AVAssetResourceLoader resourceLoader, NSUrlAuthenticationChallenge authenticationChallenge);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("resourceLoader:shouldWaitForRenewalOfRequestedResource:")]
		bool ShouldWaitForRenewalOfRequestedResource (AVAssetResourceLoader resourceLoader, AVAssetResourceRenewalRequest renewalRequest);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // crash at 'description' - not meant to be used callable (it's used from a property getter)
	interface AVAssetResourceLoadingDataRequest {
		[Export ("requestedOffset")]
		long RequestedOffset { get; }

		[Export ("requestedLength")]
		nint RequestedLength { get; }

		[Export ("currentOffset")]
		long CurrentOffset { get; }

		[Export ("respondWithData:")]
		void Respond (NSData responseData);

		[MacCatalyst (13, 1)]
		[Export ("requestsAllDataToEndOfResource")]
		bool RequestsAllDataToEndOfResource { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // not meant be be user created (resource loader job, see documentation)
	interface AVAssetResourceLoadingRequest {
		[Export ("request")]
		NSUrlRequest Request { get; }

		// note: we cannot use [Bind] here as it would break compatibility with iOS 6.x
		// `isFinished` was only added in iOS 7.0 SDK and cannot be called in earlier versions
		[Export ("isFinished")]
		bool Finished { get; }

		[Export ("finishLoadingWithResponse:data:redirect:")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the 'Response', 'Redirect' properties and the 'AVAssetResourceLoadingDataRequest.Responds' and 'AVAssetResourceLoadingRequest.FinishLoading' methods instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use the 'Response', 'Redirect' properties and the 'AVAssetResourceLoadingDataRequest.Responds' and 'AVAssetResourceLoadingRequest.FinishLoading' methods instead.")]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the 'Response', 'Redirect' properties and the 'AVAssetResourceLoadingDataRequest.Responds' and 'AVAssetResourceLoadingRequest.FinishLoading' methods instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'Response', 'Redirect' properties and the 'AVAssetResourceLoadingDataRequest.Responds' and 'AVAssetResourceLoadingRequest.FinishLoading' methods instead.")]
		void FinishLoading ([NullAllowed] NSUrlResponse usingResponse, [NullAllowed] NSData data, [NullAllowed] NSUrlRequest redirect);

		[Export ("finishLoadingWithError:")]
		void FinishLoadingWithError ([NullAllowed] NSError error); // TODO: Should have been FinishLoading (NSerror);

		[Deprecated (PlatformName.MacOSX, 12, 0)]
		[Deprecated (PlatformName.iOS, 15, 0)]
		[Deprecated (PlatformName.TvOS, 15, 0)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0)]
		[return: NullAllowed]
		[Export ("streamingContentKeyRequestDataForApp:contentIdentifier:options:error:")]
		NSData GetStreamingContentKey (NSData appIdentifier, NSData contentIdentifier, [NullAllowed] NSDictionary options, out NSError error);

		[Deprecated (PlatformName.MacOSX, 12, 0)]
		[Deprecated (PlatformName.iOS, 15, 0)]
		[Deprecated (PlatformName.TvOS, 15, 0)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0)]
		[MacCatalyst (13, 1)]
		[Export ("persistentContentKeyFromKeyVendorResponse:options:error:")]
		[return: NullAllowed]
		NSData GetPersistentContentKey (NSData keyVendorResponse, [NullAllowed] NSDictionary<NSString, NSObject> options, out NSError error);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVAssetResourceLoadingRequestStreamingContentKeyRequestRequiresPersistentKey")]
		NSString StreamingContentKeyRequestRequiresPersistentKey { get; }

		[Export ("isCancelled")]
		bool IsCancelled { get; }

		[Export ("contentInformationRequest"), NullAllowed]
		AVAssetResourceLoadingContentInformationRequest ContentInformationRequest { get; }

		[Export ("dataRequest"), NullAllowed]
		AVAssetResourceLoadingDataRequest DataRequest { get; }

		[Export ("response", ArgumentSemantic.Copy), NullAllowed]
		NSUrlResponse Response { get; set; }

		[Export ("redirect", ArgumentSemantic.Copy), NullAllowed]
		NSUrlRequest Redirect { get; set; }

		[Export ("finishLoading")]
		void FinishLoading ();

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("requestor")]
		AVAssetResourceLoadingRequestor Requestor { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor] // not meant be be user created (resource loader job, see documentation) fix crash
	[BaseType (typeof (AVAssetResourceLoadingRequest))]
	interface AVAssetResourceRenewalRequest {
	}


	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no valid handle, the instance is received (not created) -> see doc
	interface AVAssetResourceLoadingContentInformationRequest {
		[Export ("contentType"), NullAllowed]
		string ContentType { get; set; }

		[Export ("contentLength")]
		long ContentLength { get; set; }

		[Export ("byteRangeAccessSupported")]
		bool ByteRangeAccessSupported { [Bind ("isByteRangeAccessSupported")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("renewalDate", ArgumentSemantic.Copy), NullAllowed]
		NSDate RenewalDate { get; set; }

		[Watch (4, 2), TV (11, 2), iOS (11, 2)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("allowedContentTypes")]
		string [] AllowedContentTypes { get; }
	}

	interface IAVAssetWriterDelegate { }

	[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol]
	[Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface AVAssetWriterDelegate {
		[Export ("assetWriter:didOutputSegmentData:segmentType:segmentReport:")]
		void DidOutputSegmentData (AVAssetWriter writer, NSData segmentData, AVAssetSegmentType segmentType, [NullAllowed] AVAssetSegmentReport segmentReport);

		[Export ("assetWriter:didOutputSegmentData:segmentType:")]
		void DidOutputSegmentData (AVAssetWriter writer, NSData segmentData, AVAssetSegmentType segmentType);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetWriter initWithURL:fileType:error:] invalid parameter not satisfying: outputURL != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetWriter {
		[Export ("outputURL", ArgumentSemantic.Copy)]
		NSUrl OutputURL { get; }

		[Export ("outputFileType", ArgumentSemantic.Copy)]
		string OutputFileType { get; }

		[Export ("status")]
		AVAssetWriterStatus Status { get; }

		[Export ("error"), NullAllowed]
		NSError Error { get; }

		[Export ("movieFragmentInterval", ArgumentSemantic.Assign)]
		CMTime MovieFragmentInterval { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("overallDurationHint", ArgumentSemantic.Assign)]
		CMTime OverallDurationHint { get; set; }

		[Export ("shouldOptimizeForNetworkUse")]
		bool ShouldOptimizeForNetworkUse { get; set; }

#if !XAMCORE_5_0
		[Internal]
		[Export ("inputs")]
		NSArray InternalInputs { get; }

		[Obsolete ("Use the 'Inputs' property instead.")]
		[Wrap ("InternalInputs", IsVirtual = true)]
		AVAssetWriterInput [] inputs { get; }

		[Wrap ("InternalInputs", IsVirtual = true)]
		AVAssetWriterInput [] Inputs { get; }
#else
		[Export ("Inputs")]
		AVAssetWriterInput [] Inputs { get;  }
#endif

		[Export ("availableMediaTypes")]
		NSString [] AvailableMediaTypes { get; }

		[Export ("metadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] Metadata { get; set; }

		[return: NullAllowed]
		[Static, Export ("assetWriterWithURL:fileType:error:")]
		AVAssetWriter FromUrl (NSUrl outputUrl, string outputFileType, out NSError error);

		[DesignatedInitializer]
		[Export ("initWithURL:fileType:error:")]
		NativeHandle Constructor (NSUrl outputUrl, string outputFileType, out NSError error);

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithContentType:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UTType outputContentType);

		[Export ("canApplyOutputSettings:forMediaType:")]
		bool CanApplyOutputSettings ([NullAllowed] NSDictionary outputSettings, string mediaType);

		[Wrap ("CanApplyOutputSettings (outputSettings.GetDictionary (), mediaType)")]
		bool CanApplyOutputSettings (AudioSettings outputSettings, string mediaType);

		[Wrap ("CanApplyOutputSettings (outputSettings.GetDictionary (), mediaType)")]
		bool CanApplyOutputSettings (AVVideoSettingsCompressed outputSettings, string mediaType);

		[Export ("canAddInput:")]
		bool CanAddInput (AVAssetWriterInput input);

		[Export ("addInput:")]
		void AddInput (AVAssetWriterInput input);

		[Export ("startWriting")]
		bool StartWriting ();

		[Export ("startSessionAtSourceTime:")]
		void StartSessionAtSourceTime (CMTime startTime);

		[Export ("endSessionAtSourceTime:")]
		void EndSessionAtSourceTime (CMTime endTime);

		[Export ("cancelWriting")]
		void CancelWriting ();

		[Export ("finishWriting")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use the asynchronous 'FinishWriting (NSAction completionHandler)' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use the asynchronous 'FinishWriting (NSAction completionHandler)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the asynchronous 'FinishWriting (NSAction completionHandler)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the asynchronous 'FinishWriting (NSAction completionHandler)' instead.")]
		bool FinishWriting ();

		[MacCatalyst (13, 1)]
		[Export ("finishWritingWithCompletionHandler:")]
		[Async]
		void FinishWriting (Action completionHandler);

		[Export ("movieTimeScale")]
		int /* CMTimeScale = int32_t */ MovieTimeScale { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("canAddInputGroup:")]
		bool CanAddInputGroup (AVAssetWriterInputGroup inputGroup);

		[MacCatalyst (13, 1)]
		[Export ("addInputGroup:")]
		void AddInputGroup (AVAssetWriterInputGroup inputGroup);

		[MacCatalyst (13, 1)]
		[Export ("inputGroups")]
		AVAssetWriterInputGroup [] InputGroups { get; }

		[MacCatalyst (13, 1)]
		[Export ("directoryForTemporaryFiles", ArgumentSemantic.Copy), NullAllowed]
		NSUrl DirectoryForTemporaryFiles { get; set; }

		// from category AVAssetWriterSegmentation (AVAssetWriter)

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("preferredOutputSegmentInterval", ArgumentSemantic.Assign)]
		CMTime PreferredOutputSegmentInterval { get; set; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initialSegmentStartTime", ArgumentSemantic.Assign)]
		CMTime InitialSegmentStartTime { get; set; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("outputFileTypeProfile")]
		[NullAllowed]
		[BindAs (typeof (AVFileTypeProfile))]
		NSString OutputFileTypeProfile { get; set; }

		[Wrap ("WeakDelegate")]
		IAVAssetWriterDelegate Delegate { get; set; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("flushSegment")]
		void FlushSegment ();

		// from category AVAssetWriterFileTypeSpecificProperties (AVAssetWriter)

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initialMovieFragmentSequenceNumber")]
		nint InitialMovieFragmentSequenceNumber { get; set; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("producesCombinableFragments")]
		bool ProducesCombinableFragments { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetWriterInput initWithMediaType:outputSettings:] invalid parameter not satisfying: mediaType != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetWriterInput {
		[DesignatedInitializer]
		[Protected]
		[Export ("initWithMediaType:outputSettings:sourceFormatHint:")]
		NativeHandle Constructor (string mediaType, [NullAllowed] NSDictionary outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[Wrap ("this (mediaType, outputSettings.GetDictionary (), sourceFormatHint)")]
		NativeHandle Constructor (string mediaType, [NullAllowed] AudioSettings outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[Wrap ("this (mediaType, outputSettings.GetDictionary (), sourceFormatHint)")]
		NativeHandle Constructor (string mediaType, [NullAllowed] AVVideoSettingsCompressed outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[Static, Internal]
		[Export ("assetWriterInputWithMediaType:outputSettings:sourceFormatHint:")]
		AVAssetWriterInput Create (string mediaType, [NullAllowed] NSDictionary outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[Static]
		[Wrap ("Create(mediaType, outputSettings.GetDictionary (), sourceFormatHint)")]
		AVAssetWriterInput Create (string mediaType, [NullAllowed] AudioSettings outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[Static]
		[Wrap ("Create(mediaType, outputSettings.GetDictionary (), sourceFormatHint)")]
		AVAssetWriterInput Create (string mediaType, [NullAllowed] AVVideoSettingsCompressed outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[Export ("mediaType")]
		string MediaType { get; }

		[Export ("outputSettings"), NullAllowed]
		NSDictionary OutputSettings { get; }

		[Export ("transform", ArgumentSemantic.Assign)]
		CGAffineTransform Transform { get; set; }

		[Export ("metadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] Metadata { get; set; }

		[Export ("readyForMoreMediaData")]
		bool ReadyForMoreMediaData { [Bind ("isReadyForMoreMediaData")] get; }

		[Export ("expectsMediaDataInRealTime")]
		bool ExpectsMediaDataInRealTime { get; set; }

		[Internal]
		[Advice ("Use constructor or 'Create' method instead.")]
		[Static, Export ("assetWriterInputWithMediaType:outputSettings:")]
		AVAssetWriterInput FromType (string mediaType, [NullAllowed] NSDictionary outputSettings);

		[Static, Wrap ("FromType (mediaType, outputSettings.GetDictionary ())")]
		AVAssetWriterInput Create (string mediaType, [NullAllowed] AudioSettings outputSettings);

		[Static, Wrap ("FromType (mediaType, outputSettings.GetDictionary ())")]
		AVAssetWriterInput Create (string mediaType, [NullAllowed] AVVideoSettingsCompressed outputSettings);

		[Protected]
		[Export ("initWithMediaType:outputSettings:")]
		NativeHandle Constructor (string mediaType, [NullAllowed] NSDictionary outputSettings);

		[Wrap ("this (mediaType, outputSettings.GetDictionary ())")]
		NativeHandle Constructor (string mediaType, [NullAllowed] AudioSettings outputSettings);

		[Wrap ("this (mediaType, outputSettings.GetDictionary ())")]
		NativeHandle Constructor (string mediaType, [NullAllowed] AVVideoSettingsCompressed outputSettings);

		[Export ("requestMediaDataWhenReadyOnQueue:usingBlock:")]
		void RequestMediaData (DispatchQueue queue, Action action);

		[Export ("appendSampleBuffer:")]
		bool AppendSampleBuffer (CMSampleBuffer sampleBuffer);

		[Export ("markAsFinished")]
		void MarkAsFinished ();

		[Export ("mediaTimeScale")]
		int /* CMTimeScale = int32_t */ MediaTimeScale { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("languageCode", ArgumentSemantic.Copy), NullAllowed]
		string LanguageCode { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("extendedLanguageTag", ArgumentSemantic.Copy), NullAllowed]
		string ExtendedLanguageTag { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("naturalSize")]
		CGSize NaturalSize { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("preferredVolume")]
		float PreferredVolume { get; set; } // defined as 'float'

		[MacCatalyst (13, 1)]
		[Export ("marksOutputTrackAsEnabled")]
		bool MarksOutputTrackAsEnabled { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("canAddTrackAssociationWithTrackOfInput:type:")]
		bool CanAddTrackAssociationWithTrackOfInput (AVAssetWriterInput input, NSString trackAssociationType);

		[MacCatalyst (13, 1)]
		[Export ("addTrackAssociationWithTrackOfInput:type:")]
		void AddTrackAssociationWithTrackOfInput (AVAssetWriterInput input, NSString trackAssociationType);

		[Export ("sourceFormatHint"), NullAllowed]
		CMFormatDescription SourceFormatHint { get; }

		//
		// AVAssetWriterInputMultiPass Category
		//
		[MacCatalyst (13, 1)]
		[Export ("performsMultiPassEncodingIfSupported")]
		bool PerformsMultiPassEncodingIfSupported { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("canPerformMultiplePasses")]
		bool CanPerformMultiplePasses { get; }

		[MacCatalyst (13, 1)]
		[Export ("currentPassDescription"), NullAllowed]
		AVAssetWriterInputPassDescription CurrentPassDescription { get; }

		[MacCatalyst (13, 1)]
		[Export ("respondToEachPassDescriptionOnQueue:usingBlock:")]
		void SetPassHandler (DispatchQueue queue, Action passHandler);

		[MacCatalyst (13, 1)]
		[Export ("markCurrentPassAsFinished")]
		void MarkCurrentPassAsFinished ();

		[MacCatalyst (13, 1)]
		[Export ("preferredMediaChunkAlignment")]
		nint PreferredMediaChunkAlignment { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("preferredMediaChunkDuration")]
		CMTime PreferredMediaChunkDuration { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("sampleReferenceBaseURL", ArgumentSemantic.Copy), NullAllowed]
		NSUrl SampleReferenceBaseUrl { get; set; }

		// AVAssetWriterInput_AVAssetWriterInputFileTypeSpecificProperties

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("mediaDataLocation")]
		string MediaDataLocation { get; set; }

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAssetWriterInputPassDescription {

		[Export ("sourceTimeRanges")]
		NSValue [] SourceTimeRanges { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: *** -[AVAssetWriterInputMetadataAdaptor initWithAssetWriterInput:] invalid parameter not satisfying: input != ((void*)0)
	interface AVAssetWriterInputMetadataAdaptor {

		[DesignatedInitializer]
		[Export ("initWithAssetWriterInput:")]
		NativeHandle Constructor (AVAssetWriterInput assetWriterInput);

		[Export ("assetWriterInput")]
		AVAssetWriterInput AssetWriterInput { get; }

		[Static, Export ("assetWriterInputMetadataAdaptorWithAssetWriterInput:")]
		AVAssetWriterInputMetadataAdaptor Create (AVAssetWriterInput input);

		[Export ("appendTimedMetadataGroup:")]
		bool AppendTimedMetadataGroup (AVTimedMetadataGroup timedMetadataGroup);
	}

	[NoWatch]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetWriterInputGroup initWithInputs:defaultInput:] invalid parameter not satisfying: inputs != ((void*)0)
	[BaseType (typeof (AVMediaSelectionGroup))]
	[MacCatalyst (13, 1)]
	interface AVAssetWriterInputGroup {

		[Static, Export ("assetWriterInputGroupWithInputs:defaultInput:")]
		AVAssetWriterInputGroup Create (AVAssetWriterInput [] inputs, [NullAllowed] AVAssetWriterInput defaultInput);

		[DesignatedInitializer]
		[Export ("initWithInputs:defaultInput:")]
		NativeHandle Constructor (AVAssetWriterInput [] inputs, [NullAllowed] AVAssetWriterInput defaultInput);

		[Export ("inputs")]
		AVAssetWriterInput [] Inputs { get; }

		[Export ("defaultInput", ArgumentSemantic.Copy), NullAllowed]
		AVAssetWriterInput DefaultInput { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetWriterInputPixelBufferAdaptor initWithAssetWriterInput:sourcePixelBufferAttributes:] invalid parameter not satisfying: input != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetWriterInputPixelBufferAdaptor {
		[Export ("assetWriterInput")]
		AVAssetWriterInput AssetWriterInput { get; }

		[NullAllowed, Export ("sourcePixelBufferAttributes")]
		NSDictionary SourcePixelBufferAttributes { get; }

		[Wrap ("SourcePixelBufferAttributes")]
		CVPixelBufferAttributes Attributes { get; }

		[Export ("pixelBufferPool"), NullAllowed]
		CVPixelBufferPool PixelBufferPool { get; }

		[Advice ("Use 'Create' method.")]
		[Static, Export ("assetWriterInputPixelBufferAdaptorWithAssetWriterInput:sourcePixelBufferAttributes:")]
		AVAssetWriterInputPixelBufferAdaptor FromInput (AVAssetWriterInput input, [NullAllowed] NSDictionary sourcePixelBufferAttributes);

		[Static, Wrap ("FromInput (input, attributes.GetDictionary ())")]
		AVAssetWriterInputPixelBufferAdaptor Create (AVAssetWriterInput input, [NullAllowed] CVPixelBufferAttributes attributes);

		[DesignatedInitializer]
		[Export ("initWithAssetWriterInput:sourcePixelBufferAttributes:")]
		NativeHandle Constructor (AVAssetWriterInput input, [NullAllowed] NSDictionary sourcePixelBufferAttributes);

		[Wrap ("this (input, attributes.GetDictionary ())")]
		NativeHandle Constructor (AVAssetWriterInput input, [NullAllowed] CVPixelBufferAttributes attributes);

		[Export ("appendPixelBuffer:withPresentationTime:")]
		bool AppendPixelBufferWithPresentationTime (CVPixelBuffer pixelBuffer, CMTime presentationTime);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetCache {
		[Export ("playableOffline")]
		bool IsPlayableOffline { [Bind ("isPlayableOffline")] get; }

		[Export ("mediaSelectionOptionsInMediaSelectionGroup:")]
		AVMediaSelectionOption [] GetMediaSelectionOptions (AVMediaSelectionGroup mediaSelectionGroup);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAsset), Name = "AVURLAsset")]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVUrlAsset
#if !WATCH
		: AVContentKeyRecipient
#endif
{

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Internal]
		[Advice ("Use constructor or 'Create' method instead.")]
		[Static, Export ("URLAssetWithURL:options:")]
		AVUrlAsset FromUrl (NSUrl url, [NullAllowed] NSDictionary options);

		[Static]
		[Wrap ("FromUrl (url, options.GetDictionary ())")]
		AVUrlAsset Create (NSUrl url, [NullAllowed] AVUrlAssetOptions options);

		[Static]
		[Wrap ("FromUrl (url, (NSDictionary) null!)")]
		AVUrlAsset Create (NSUrl url);

		[DesignatedInitializer]
		[Export ("initWithURL:options:")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] NSDictionary options);

		[Wrap ("this (url, options.GetDictionary ())")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] AVUrlAssetOptions options);

		[Wrap ("this (url, (NSDictionary) null!)")]
		NativeHandle Constructor (NSUrl url);

		[return: NullAllowed]
		[Export ("compatibleTrackForCompositionTrack:")]
		AVAssetTrack CompatibleTrack (AVCompositionTrack forCompositionTrack);

		[Field ("AVURLAssetPreferPreciseDurationAndTimingKey")]
		NSString PreferPreciseDurationAndTimingKey { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVURLAssetReferenceRestrictionsKey")]
		NSString ReferenceRestrictionsKey { get; }

		[Static, Export ("audiovisualMIMETypes")]
		string [] AudiovisualMimeTypes { get; }

		[Static, Export ("audiovisualTypes")]
		string [] AudiovisualTypes { get; }

		[Static, Export ("isPlayableExtendedMIMEType:")]
		bool IsPlayable (string extendedMimeType);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("resourceLoader")]
		AVAssetResourceLoader ResourceLoader { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVURLAssetHTTPCookiesKey")]
		NSString HttpCookiesKey { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("assetCache")]
		AVAssetCache Cache { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVURLAssetAllowsCellularAccessKey")]
		NSString AllowsCellularAccessKey { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVURLAssetAllowsExpensiveNetworkAccessKey")]
		NSString AllowsExpensiveNetworkAccessKey { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVURLAssetAllowsConstrainedNetworkAccessKey")]
		NSString AllowsConstrainedNetworkAccessKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
		[Field ("AVURLAssetShouldSupportAliasDataReferencesKey")]
		NSString ShouldSupportAliasDataReferencesKey { get; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("variants")]
		AVAssetVariant [] Variants { get; }

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("findCompatibleTrackForCompositionTrack:completionHandler:")]
		void FindCompatibleTrack (AVCompositionTrack compositionTrack, Action<AVAssetTrack, NSError> completionHandler);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("AVURLAssetURLRequestAttributionKey")]
		NSString RequestAttributionKey { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVAssetTrack : NSCopying {
		[Export ("trackID")]
		int /* CMPersistentTrackID = int32_t */ TrackID { get; }

		[NullAllowed, Export ("asset", ArgumentSemantic.Weak)]
		AVAsset Asset { get; }

		[Export ("mediaType")]
		string MediaType { get; }

		[MacCatalyst (13, 1)]
		[Export ("decodable")]
		bool Decodable { [Bind ("isDecodable")] get; }

		// Weak version
		[Export ("formatDescriptions")]
		NSObject [] FormatDescriptionsAsObjects { get; }

		[Wrap ("Array.ConvertAll (FormatDescriptionsAsObjects, l => CMFormatDescription.Create (l.Handle, false))")]
		CMFormatDescription [] FormatDescriptions { get; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }

		[Export ("selfContained")]
		bool SelfContained { [Bind ("isSelfContained")] get; }

		[Export ("totalSampleDataLength")]
		long TotalSampleDataLength { get; }

		[Export ("hasMediaCharacteristic:")]
		bool HasMediaCharacteristic (string mediaCharacteristic);

		[Export ("timeRange")]
		CMTimeRange TimeRange { get; }

		[Export ("naturalTimeScale")]
		int NaturalTimeScale { get; } // defined as 'CMTimeScale' = int32_t

		[Export ("estimatedDataRate")]
		float EstimatedDataRate { get; } // defined as 'float'

		[NullAllowed, Export ("languageCode")]
		string LanguageCode { get; }

		[NullAllowed, Export ("extendedLanguageTag")]
		string ExtendedLanguageTag { get; }

		[Export ("naturalSize")]
		CGSize NaturalSize { get; }

		[Export ("preferredVolume")]
		float PreferredVolume { get; } // defined as 'float'

		[Export ("preferredTransform")]
		CGAffineTransform PreferredTransform { get; }

		[Export ("nominalFrameRate")]
		float NominalFrameRate { get; } // defined as 'float'

		[Export ("segments", ArgumentSemantic.Copy)]
		AVAssetTrackSegment [] Segments { get; }

		[return: NullAllowed]
		[Export ("segmentForTrackTime:")]
		AVAssetTrackSegment SegmentForTrackTime (CMTime trackTime);

		[Export ("samplePresentationTimeForTrackTime:")]
		CMTime SamplePresentationTimeForTrackTime (CMTime trackTime);

		[Export ("availableMetadataFormats")]
		string [] AvailableMetadataFormats { get; }

		[Export ("commonMetadata")]
		AVMetadataItem [] CommonMetadata { get; }

		[Export ("metadataForFormat:")]
		AVMetadataItem [] MetadataForFormat (string format);

		[Export ("isPlayable")]
		bool Playable { get; }

		[MacCatalyst (13, 1)]
		[Export ("availableTrackAssociationTypes")]
		NSString [] AvailableTrackAssociationTypes { get; }

		[MacCatalyst (13, 1)]
		[Export ("minFrameDuration")]
		CMTime MinFrameDuration { get; }

		[MacCatalyst (13, 1)]
		[Export ("associatedTracksOfType:")]
		AVAssetTrack [] GetAssociatedTracks (NSString avAssetTrackTrackAssociationType);

		[MacCatalyst (13, 1)]
		[Export ("metadata")]
		AVMetadataItem [] Metadata { get; }

		[MacCatalyst (13, 1)]
		[Export ("requiresFrameReordering")]
		bool RequiresFrameReordering { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAssetTrackTimeRangeDidChangeNotification")]
		[Notification]
		NSString TimeRangeDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAssetTrackSegmentsDidChangeNotification")]
		[Notification]
		NSString SegmentsDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAssetTrackTrackAssociationsDidChangeNotification")]
		[Notification]
		NSString TrackAssociationsDidChangeNotification { get; }

		[NoiOS, NoTV, NoWatch, MacCatalyst (15, 0)]
		[Export ("canProvideSampleCursors")]
		bool CanProvideSampleCursors { get; }

		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		[return: NullAllowed]
		[Export ("makeSampleCursorWithPresentationTimeStamp:")]
		AVSampleCursor MakeSampleCursor (CMTime presentationTimeStamp);

		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		[return: NullAllowed]
		[Export ("makeSampleCursorAtFirstSampleInDecodeOrder")]
		AVSampleCursor MakeSampleCursorAtFirstSampleInDecodeOrder ();

		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		[return: NullAllowed]
		[Export ("makeSampleCursorAtLastSampleInDecodeOrder")]
		AVSampleCursor MakeSampleCursorAtLastSampleInDecodeOrder ();

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("hasAudioSampleDependencies")]
		bool HasAudioSampleDependencies { get; }

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadAssociatedTracksOfType:completionHandler:")]
		void LoadAssociatedTracks (string trackAssociationType, Action<NSArray<AVAssetTrack>, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadMetadataForFormat:completionHandler:")]
		void LoadMetadata (string format, Action<NSArray<AVMetadataItem>, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadSamplePresentationTimeForTrackTime:completionHandler:")]
		void LoadSamplePresentationTime (CMTime trackTime, Action<CMTime, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadSegmentForTrackTime:completionHandler:")]
		void LoadSegment (CMTime trackTime, Action<AVAssetTrackSegment, NSError> completionHandler);
	}

	[NoiOS, NoTV, NoWatch]
	[NoMacCatalyst]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface AVSampleCursor : NSCopying {

		[Export ("stepInDecodeOrderByCount:")]
		long StepInDecodeOrder (long stepCount);

		[Export ("stepInPresentationOrderByCount:")]
		long StepInPresentationOrder (long stepCount);

		[Export ("stepByDecodeTime:wasPinned:")]
		CMTime StepByDecodeTime (CMTime deltaDecodeTime, out bool wasPinned);

		[Export ("stepByPresentationTime:wasPinned:")]
		CMTime StepByPresentationTime (CMTime deltaPresentationTime, out bool wasPinned);

		[Export ("presentationTimeStamp")]
		CMTime PresentationTimeStamp { get; }

		[Export ("decodeTimeStamp")]
		CMTime DecodeTimeStamp { get; }

		[Export ("comparePositionInDecodeOrderWithPositionOfCursor:")]
		NSComparisonResult ComparePositionInDecodeOrder (AVSampleCursor positionOfCursor);

		[Export ("samplesWithEarlierDecodeTimeStampsMayHaveLaterPresentationTimeStampsThanCursor:")]
		bool SamplesWithEarlierDecodeTimeStampsMayHaveLaterPresentationTimeStampsThan (AVSampleCursor positionOfCursor);

		[Export ("samplesWithLaterDecodeTimeStampsMayHaveEarlierPresentationTimeStampsThanCursor:")]
		bool SamplesWithLaterDecodeTimeStampsMayHaveEarlierPresentationTimeStampsThan (AVSampleCursor positionOfCursor);

		[Export ("currentSampleDuration")]
		CMTime CurrentSampleDuration { get; }

		[Export ("copyCurrentSampleFormatDescription")]
		[return: Release]
		CMFormatDescription CopyCurrentSampleFormatDescription ();

#pragma warning disable 0618 // warning CS0618: 'AVSampleCursorSyncInfo' is obsolete: 'This API is not available on this platform.'
		[Export ("currentSampleSyncInfo")]
		AVSampleCursorSyncInfo CurrentSampleSyncInfo { get; }
#pragma warning restore

#pragma warning disable 0618 // warning CS0618: 'AVSampleCursorSyncInfo' is obsolete: 'This API is not available on this platform.'
		[Export ("currentSampleDependencyInfo")]
		AVSampleCursorSyncInfo CurrentSampleDependencyInfo { get; }
#pragma warning restore

		[Export ("samplesRequiredForDecoderRefresh")]
		nint SamplesRequiredForDecoderRefresh { get; }

		[NullAllowed]
		[Export ("currentChunkStorageURL")]
		NSUrl CurrentChunkStorageUrl { get; }

#pragma warning disable 0618 // warning CS0618: 'AVSampleCursorStorageRange' is obsolete: 'This API is not available on this platform.'
		[Export ("currentChunkStorageRange")]
		AVSampleCursorStorageRange CurrentChunkStorageRange { get; }
#pragma warning restore

#pragma warning disable 0618 // warning CS0618: 'AVSampleCursorChunkInfo' is obsolete: 'This API is not available on this platform.'
		[Export ("currentChunkInfo")]
		AVSampleCursorChunkInfo CurrentChunkInfo { get; }
#pragma warning restore

		[Export ("currentSampleIndexInChunk")]
		long CurrentSampleIndexInChunk { get; }

#pragma warning disable 0618 // warning CS0618: 'AVSampleCursorStorageRange' is obsolete: 'This API is not available on this platform.'
		[Export ("currentSampleStorageRange")]
		AVSampleCursorStorageRange CurrentSampleStorageRange { get; }
#pragma warning restore

#if MONOMAC
		[NoiOS][NoWatch][NoTV]
		[Export ("currentSampleAudioDependencyInfo")]
		AVSampleCursorAudioDependencyInfo CurrentSampleAudioDependencyInfo { get; }
#endif

		[NoiOS]
		[NoWatch]
		[NoTV]
		[NullAllowed]
		[Mac (12, 0)]
		[Export ("currentSampleDependencyAttachments")]
		NSDictionary CurrentSampleDependencyAttachments { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Category, BaseType (typeof (AVAssetTrack))]
	interface AVAssetTrackTrackAssociation {
		[Field ("AVTrackAssociationTypeAudioFallback")]
		NSString AudioFallback { get; }

		[Field ("AVTrackAssociationTypeChapterList")]
		NSString ChapterList { get; }

		[Field ("AVTrackAssociationTypeForcedSubtitlesOnly")]
		NSString ForcedSubtitlesOnly { get; }

		[Field ("AVTrackAssociationTypeSelectionFollower")]
		NSString SelectionFollower { get; }

		[Field ("AVTrackAssociationTypeTimecode")]
		NSString Timecode { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVTrackAssociationTypeMetadataReferent")]
		NSString MetadataReferent { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAssetTrackGroup : NSCopying {
		[Export ("trackIDs", ArgumentSemantic.Copy)]
		NSNumber [] TrackIDs { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVMediaSelectionGroup : NSCopying {
		[Export ("options")]
		AVMediaSelectionOption [] Options { get; }

		[Export ("allowsEmptySelection")]
		bool AllowsEmptySelection { get; }

		[return: NullAllowed]
		[Export ("mediaSelectionOptionWithPropertyList:")]
		AVMediaSelectionOption GetMediaSelectionOptionForPropertyList (NSObject propertyList);

		[Static]
		[Export ("playableMediaSelectionOptionsFromArray:")]
		AVMediaSelectionOption [] PlayableMediaSelectionOptions (AVMediaSelectionOption [] source);

		[Static]
		[Export ("mediaSelectionOptionsFromArray:withLocale:")]
		AVMediaSelectionOption [] MediaSelectionOptions (AVMediaSelectionOption [] source, NSLocale locale);

		[Static]
		[Export ("mediaSelectionOptionsFromArray:withMediaCharacteristics:")]
		AVMediaSelectionOption [] MediaSelectionOptions (AVMediaSelectionOption [] source, NSString [] avmediaCharacteristics);

		[Static]
		[Export ("mediaSelectionOptionsFromArray:withoutMediaCharacteristics:")]
		AVMediaSelectionOption [] MediaSelectionOptionsExcludingCharacteristics (AVMediaSelectionOption [] source, NSString [] avmediaCharacteristics);

		[Static]
		[Export ("mediaSelectionOptionsFromArray:filteredAndSortedAccordingToPreferredLanguages:")]
		AVMediaSelectionOption [] MediaSelectionOptionsFilteredAndSorted (AVMediaSelectionOption [] mediaSelectionOptions, string [] preferredLanguages);

		[MacCatalyst (13, 1)]
		[Export ("defaultOption"), NullAllowed]
		AVMediaSelectionOption DefaultOption { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVMediaSelectionOption : NSCopying {
		[Export ("mediaType")]
		string MediaType { get; }

		[Export ("mediaSubTypes")]
		NSNumber [] MediaSubTypes { get; }

		[Export ("playable")]
		bool Playable { [Bind ("isPlayable")] get; }

		[Export ("locale"), NullAllowed]
		NSLocale Locale { get; }

		[Export ("commonMetadata")]
		AVMetadataItem [] CommonMetadata { get; }

		[Export ("availableMetadataFormats")]
		string [] AvailableMetadataFormats { get; }

		[Export ("hasMediaCharacteristic:")]
		bool HasMediaCharacteristic (string mediaCharacteristic);

		[Export ("metadataForFormat:")]
		AVMetadataItem [] GetMetadataForFormat (string format);

		[return: NullAllowed]
		[Export ("associatedMediaSelectionOptionInMediaSelectionGroup:")]
		AVMediaSelectionOption AssociatedMediaSelectionOptionInMediaSelectionGroup (AVMediaSelectionGroup mediaSelectionGroup);

		[Export ("propertyList")]
		NSObject PropertyList { get; }

		[MacCatalyst (13, 1)]
		[Export ("displayName")]
		string DisplayName { get; }

		[MacCatalyst (13, 1)]
		[Export ("displayNameWithLocale:")]
		string GetDisplayName (NSLocale locale);

		[MacCatalyst (13, 1)]
		[Export ("extendedLanguageTag"), NullAllowed]
		string ExtendedLanguageTag { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Static]
	interface AVMetadata {
		[Field ("AVMetadataKeySpaceCommon")]
		NSString KeySpaceCommon { get; }

		[Field ("AVMetadataCommonKeyTitle")]
		NSString CommonKeyTitle { get; }

		[Field ("AVMetadataCommonKeyCreator")]
		NSString CommonKeyCreator { get; }

		[Field ("AVMetadataCommonKeySubject")]
		NSString CommonKeySubject { get; }

		[Field ("AVMetadataCommonKeyDescription")]
		NSString CommonKeyDescription { get; }

		[Field ("AVMetadataCommonKeyPublisher")]
		NSString CommonKeyPublisher { get; }

		[Field ("AVMetadataCommonKeyContributor")]
		NSString CommonKeyContributor { get; }

		[Field ("AVMetadataCommonKeyCreationDate")]
		NSString CommonKeyCreationDate { get; }

		[Field ("AVMetadataCommonKeyLastModifiedDate")]
		NSString CommonKeyLastModifiedDate { get; }

		[Field ("AVMetadataCommonKeyType")]
		NSString CommonKeyType { get; }

		[Field ("AVMetadataCommonKeyFormat")]
		NSString CommonKeyFormat { get; }

		[Field ("AVMetadataCommonKeyIdentifier")]
		NSString CommonKeyIdentifier { get; }

		[Field ("AVMetadataCommonKeySource")]
		NSString CommonKeySource { get; }

		[Field ("AVMetadataCommonKeyLanguage")]
		NSString CommonKeyLanguage { get; }

		[Field ("AVMetadataCommonKeyRelation")]
		NSString CommonKeyRelation { get; }

		[Field ("AVMetadataCommonKeyLocation")]
		NSString CommonKeyLocation { get; }

		[Field ("AVMetadataCommonKeyCopyrights")]
		NSString CommonKeyCopyrights { get; }

		[Field ("AVMetadataCommonKeyAlbumName")]
		NSString CommonKeyAlbumName { get; }

		[Field ("AVMetadataCommonKeyAuthor")]
		NSString CommonKeyAuthor { get; }

		[Field ("AVMetadataCommonKeyArtist")]
		NSString CommonKeyArtist { get; }

		[Field ("AVMetadataCommonKeyArtwork")]
		NSString CommonKeyArtwork { get; }

		[Field ("AVMetadataCommonKeyMake")]
		NSString CommonKeyMake { get; }

		[Field ("AVMetadataCommonKeyModel")]
		NSString CommonKeyModel { get; }

		[Field ("AVMetadataCommonKeySoftware")]
		NSString CommonKeySoftware { get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVMetadataCommonKeyAccessibilityDescription")]
		NSString CommonKeyAccessibilityDescription { get; }

#if !NET
		[Field ("AVMetadataFormatQuickTimeUserData")]
		[Obsolete ("Use 'AVMetadataFormat' enum values.")]
		NSString FormatQuickTimeUserData { get; }
#endif

		[Field ("AVMetadataKeySpaceQuickTimeUserData")]
		NSString KeySpaceQuickTimeUserData { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyAlbum")]
		NSString QuickTimeUserDataKeyAlbum { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyArranger")]
		NSString QuickTimeUserDataKeyArranger { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyArtist")]
		NSString QuickTimeUserDataKeyArtist { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyAuthor")]
		NSString QuickTimeUserDataKeyAuthor { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyChapter")]
		NSString QuickTimeUserDataKeyChapter { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyComment")]
		NSString QuickTimeUserDataKeyComment { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyComposer")]
		NSString QuickTimeUserDataKeyComposer { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyCopyright")]
		NSString QuickTimeUserDataKeyCopyright { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyCreationDate")]
		NSString QuickTimeUserDataKeyCreationDate { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyDescription")]
		NSString QuickTimeUserDataKeyDescription { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyDirector")]
		NSString QuickTimeUserDataKeyDirector { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyDisclaimer")]
		NSString QuickTimeUserDataKeyDisclaimer { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyEncodedBy")]
		NSString QuickTimeUserDataKeyEncodedBy { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyFullName")]
		NSString QuickTimeUserDataKeyFullName { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyGenre")]
		NSString QuickTimeUserDataKeyGenre { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyHostComputer")]
		NSString QuickTimeUserDataKeyHostComputer { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyInformation")]
		NSString QuickTimeUserDataKeyInformation { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyKeywords")]
		NSString QuickTimeUserDataKeyKeywords { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyMake")]
		NSString QuickTimeUserDataKeyMake { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyModel")]
		NSString QuickTimeUserDataKeyModel { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyOriginalArtist")]
		NSString QuickTimeUserDataKeyOriginalArtist { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyOriginalFormat")]
		NSString QuickTimeUserDataKeyOriginalFormat { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyOriginalSource")]
		NSString QuickTimeUserDataKeyOriginalSource { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyPerformers")]
		NSString QuickTimeUserDataKeyPerformers { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyProducer")]
		NSString QuickTimeUserDataKeyProducer { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyPublisher")]
		NSString QuickTimeUserDataKeyPublisher { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyProduct")]
		NSString QuickTimeUserDataKeyProduct { get; }

		[Field ("AVMetadataQuickTimeUserDataKeySoftware")]
		NSString QuickTimeUserDataKeySoftware { get; }

		[Field ("AVMetadataQuickTimeUserDataKeySpecialPlaybackRequirements")]
		NSString QuickTimeUserDataKeySpecialPlaybackRequirements { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyTrack")]
		NSString QuickTimeUserDataKeyTrack { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyWarning")]
		NSString QuickTimeUserDataKeyWarning { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyWriter")]
		NSString QuickTimeUserDataKeyWriter { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyURLLink")]
		NSString QuickTimeUserDataKeyURLLink { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyLocationISO6709")]
		NSString QuickTimeUserDataKeyLocationISO6709 { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyTrackName")]
		NSString QuickTimeUserDataKeyTrackName { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyCredits")]
		NSString QuickTimeUserDataKeyCredits { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyPhonogramRights")]
		NSString QuickTimeUserDataKeyPhonogramRights { get; }

		[Field ("AVMetadataQuickTimeUserDataKeyTaggedCharacteristic")]
		NSString QuickTimeUserDataKeyTaggedCharacteristic { get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVMetadataQuickTimeUserDataKeyAccessibilityDescription")]
		NSString QuickTimeUserDataKeyAccessibilityDescription { get; }

		[Field ("AVMetadataISOUserDataKeyCopyright")]
		NSString ISOUserDataKeyCopyright { get; }

		[Field ("AVMetadata3GPUserDataKeyCopyright")]
		NSString K3GPUserDataKeyCopyright { get; }

		[Field ("AVMetadata3GPUserDataKeyAuthor")]
		NSString K3GPUserDataKeyAuthor { get; }

		[Field ("AVMetadata3GPUserDataKeyPerformer")]
		NSString K3GPUserDataKeyPerformer { get; }

		[Field ("AVMetadata3GPUserDataKeyGenre")]
		NSString K3GPUserDataKeyGenre { get; }

		[Field ("AVMetadata3GPUserDataKeyRecordingYear")]
		NSString K3GPUserDataKeyRecordingYear { get; }

		[Field ("AVMetadata3GPUserDataKeyLocation")]
		NSString K3GPUserDataKeyLocation { get; }

		[Field ("AVMetadata3GPUserDataKeyTitle")]
		NSString K3GPUserDataKeyTitle { get; }

		[Field ("AVMetadata3GPUserDataKeyDescription")]
		NSString K3GPUserDataKeyDescription { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadata3GPUserDataKeyCollection")]
		NSString K3GPUserDataKeyCollection { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadata3GPUserDataKeyUserRating")]
		NSString K3GPUserDataKeyUserRating { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadata3GPUserDataKeyThumbnail")]
		NSString K3GPUserDataKeyThumbnail { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadata3GPUserDataKeyAlbumAndTrack")]
		NSString K3GPUserDataKeyAlbumAndTrack { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadata3GPUserDataKeyKeywordList")]
		NSString K3GPUserDataKeyKeywordList { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadata3GPUserDataKeyMediaClassification")]
		NSString K3GPUserDataKeyMediaClassification { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadata3GPUserDataKeyMediaRating")]
		NSString K3GPUserDataKeyMediaRating { get; }

#if !NET
		[Field ("AVMetadataFormatISOUserData")]
		[Obsolete ("Use 'AVMetadataFormat' enum values.")]
		NSString KFormatISOUserData { get; }
#endif

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataKeySpaceISOUserData")]
		NSString KKeySpaceISOUserData { get; }

		[Field ("AVMetadataFormatQuickTimeMetadata")]
		NSString FormatQuickTimeMetadata { get; }

		[Field ("AVMetadataKeySpaceQuickTimeMetadata")]
		NSString KeySpaceQuickTimeMetadata { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyAuthor")]
		NSString QuickTimeMetadataKeyAuthor { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyComment")]
		NSString QuickTimeMetadataKeyComment { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyCopyright")]
		NSString QuickTimeMetadataKeyCopyright { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyCreationDate")]
		NSString QuickTimeMetadataKeyCreationDate { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyDirector")]
		NSString QuickTimeMetadataKeyDirector { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyDisplayName")]
		NSString QuickTimeMetadataKeyDisplayName { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyInformation")]
		NSString QuickTimeMetadataKeyInformation { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyKeywords")]
		NSString QuickTimeMetadataKeyKeywords { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyProducer")]
		NSString QuickTimeMetadataKeyProducer { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyPublisher")]
		NSString QuickTimeMetadataKeyPublisher { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyAlbum")]
		NSString QuickTimeMetadataKeyAlbum { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyArtist")]
		NSString QuickTimeMetadataKeyArtist { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyArtwork")]
		NSString QuickTimeMetadataKeyArtwork { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyDescription")]
		NSString QuickTimeMetadataKeyDescription { get; }

		[Field ("AVMetadataQuickTimeMetadataKeySoftware")]
		NSString QuickTimeMetadataKeySoftware { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyYear")]
		NSString QuickTimeMetadataKeyYear { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyGenre")]
		NSString QuickTimeMetadataKeyGenre { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyiXML")]
		NSString QuickTimeMetadataKeyiXML { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyLocationISO6709")]
		NSString QuickTimeMetadataKeyLocationISO6709 { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyMake")]
		NSString QuickTimeMetadataKeyMake { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyModel")]
		NSString QuickTimeMetadataKeyModel { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyArranger")]
		NSString QuickTimeMetadataKeyArranger { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyEncodedBy")]
		NSString QuickTimeMetadataKeyEncodedBy { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyOriginalArtist")]
		NSString QuickTimeMetadataKeyOriginalArtist { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyPerformer")]
		NSString QuickTimeMetadataKeyPerformer { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyComposer")]
		NSString QuickTimeMetadataKeyComposer { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyCredits")]
		NSString QuickTimeMetadataKeyCredits { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyPhonogramRights")]
		NSString QuickTimeMetadataKeyPhonogramRights { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyCameraIdentifier")]
		NSString QuickTimeMetadataKeyCameraIdentifier { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyCameraFrameReadoutTime")]
		NSString QuickTimeMetadataKeyCameraFrameReadoutTime { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyTitle")]
		NSString QuickTimeMetadataKeyTitle { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyCollectionUser")]
		NSString QuickTimeMetadataKeyCollectionUser { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyRatingUser")]
		NSString QuickTimeMetadataKeyRatingUser { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyLocationName")]
		NSString QuickTimeMetadataKeyLocationName { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyLocationBody")]
		NSString QuickTimeMetadataKeyLocationBody { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyLocationNote")]
		NSString QuickTimeMetadataKeyLocationNote { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyLocationRole")]
		NSString QuickTimeMetadataKeyLocationRole { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyLocationDate")]
		NSString QuickTimeMetadataKeyLocationDate { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyDirectionFacing")]
		NSString QuickTimeMetadataKeyDirectionFacing { get; }

		[Field ("AVMetadataQuickTimeMetadataKeyDirectionMotion")]
		NSString QuickTimeMetadataKeyDirectionMotion { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataQuickTimeMetadataKeyContentIdentifier")]
		NSString QuickTimeMetadataKeyContentIdentifier { get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVMetadataQuickTimeMetadataKeyAccessibilityDescription")]
		NSString QuickTimeMetadataKeyAccessibilityDescription { get; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("AVMetadataQuickTimeMetadataKeyIsMontage")]
		NSString QuickTimeMetadataKeyIsMontage { get; }

#if !NET
		[Field ("AVMetadataFormatiTunesMetadata")]
		[Obsolete ("Use 'AVMetadataFormat' enum values.")]
		NSString FormatiTunesMetadata { get; }
#endif

		[Field ("AVMetadataKeySpaceiTunes")]
		NSString KeySpaceiTunes { get; }


		[Field ("AVMetadataiTunesMetadataKeyAlbum")]
		NSString iTunesMetadataKeyAlbum { get; }

		[Field ("AVMetadataiTunesMetadataKeyArtist")]
		NSString iTunesMetadataKeyArtist { get; }

		[Field ("AVMetadataiTunesMetadataKeyUserComment")]
		NSString iTunesMetadataKeyUserComment { get; }

		[Field ("AVMetadataiTunesMetadataKeyCoverArt")]
		NSString iTunesMetadataKeyCoverArt { get; }

		[Field ("AVMetadataiTunesMetadataKeyCopyright")]
		NSString iTunesMetadataKeyCopyright { get; }

		[Field ("AVMetadataiTunesMetadataKeyReleaseDate")]
		NSString iTunesMetadataKeyReleaseDate { get; }

		[Field ("AVMetadataiTunesMetadataKeyEncodedBy")]
		NSString iTunesMetadataKeyEncodedBy { get; }

		[Field ("AVMetadataiTunesMetadataKeyPredefinedGenre")]
		NSString iTunesMetadataKeyPredefinedGenre { get; }

		[Field ("AVMetadataiTunesMetadataKeyUserGenre")]
		NSString iTunesMetadataKeyUserGenre { get; }

		[Field ("AVMetadataiTunesMetadataKeySongName")]
		NSString iTunesMetadataKeySongName { get; }

		[Field ("AVMetadataiTunesMetadataKeyTrackSubTitle")]
		NSString iTunesMetadataKeyTrackSubTitle { get; }

		[Field ("AVMetadataiTunesMetadataKeyEncodingTool")]
		NSString iTunesMetadataKeyEncodingTool { get; }

		[Field ("AVMetadataiTunesMetadataKeyComposer")]
		NSString iTunesMetadataKeyComposer { get; }

		[Field ("AVMetadataiTunesMetadataKeyAlbumArtist")]
		NSString iTunesMetadataKeyAlbumArtist { get; }

		[Field ("AVMetadataiTunesMetadataKeyAccountKind")]
		NSString iTunesMetadataKeyAccountKind { get; }

		[Field ("AVMetadataiTunesMetadataKeyAppleID")]
		NSString iTunesMetadataKeyAppleID { get; }

		[Field ("AVMetadataiTunesMetadataKeyArtistID")]
		NSString iTunesMetadataKeyArtistID { get; }

		[Field ("AVMetadataiTunesMetadataKeySongID")]
		NSString iTunesMetadataKeySongID { get; }

		[Field ("AVMetadataiTunesMetadataKeyDiscCompilation")]
		NSString iTunesMetadataKeyDiscCompilation { get; }

		[Field ("AVMetadataiTunesMetadataKeyDiscNumber")]
		NSString iTunesMetadataKeyDiscNumber { get; }

		[Field ("AVMetadataiTunesMetadataKeyGenreID")]
		NSString iTunesMetadataKeyGenreID { get; }

		[Field ("AVMetadataiTunesMetadataKeyGrouping")]
		NSString iTunesMetadataKeyGrouping { get; }

		[Field ("AVMetadataiTunesMetadataKeyPlaylistID")]
		NSString iTunesMetadataKeyPlaylistID { get; }

		[Field ("AVMetadataiTunesMetadataKeyContentRating")]
		NSString iTunesMetadataKeyContentRating { get; }

		[Field ("AVMetadataiTunesMetadataKeyBeatsPerMin")]
		NSString iTunesMetadataKeyBeatsPerMin { get; }

		[Field ("AVMetadataiTunesMetadataKeyTrackNumber")]
		NSString iTunesMetadataKeyTrackNumber { get; }

		[Field ("AVMetadataiTunesMetadataKeyArtDirector")]
		NSString iTunesMetadataKeyArtDirector { get; }

		[Field ("AVMetadataiTunesMetadataKeyArranger")]
		NSString iTunesMetadataKeyArranger { get; }

		[Field ("AVMetadataiTunesMetadataKeyAuthor")]
		NSString iTunesMetadataKeyAuthor { get; }

		[Field ("AVMetadataiTunesMetadataKeyLyrics")]
		NSString iTunesMetadataKeyLyrics { get; }

		[Field ("AVMetadataiTunesMetadataKeyAcknowledgement")]
		NSString iTunesMetadataKeyAcknowledgement { get; }

		[Field ("AVMetadataiTunesMetadataKeyConductor")]
		NSString iTunesMetadataKeyConductor { get; }

		[Field ("AVMetadataiTunesMetadataKeyDescription")]
		NSString iTunesMetadataKeyDescription { get; }

		[Field ("AVMetadataiTunesMetadataKeyDirector")]
		NSString iTunesMetadataKeyDirector { get; }

		[Field ("AVMetadataiTunesMetadataKeyEQ")]
		NSString iTunesMetadataKeyEQ { get; }

		[Field ("AVMetadataiTunesMetadataKeyLinerNotes")]
		NSString iTunesMetadataKeyLinerNotes { get; }

		[Field ("AVMetadataiTunesMetadataKeyRecordCompany")]
		NSString iTunesMetadataKeyRecordCompany { get; }

		[Field ("AVMetadataiTunesMetadataKeyOriginalArtist")]
		NSString iTunesMetadataKeyOriginalArtist { get; }

		[Field ("AVMetadataiTunesMetadataKeyPhonogramRights")]
		NSString iTunesMetadataKeyPhonogramRights { get; }

		[Field ("AVMetadataiTunesMetadataKeyProducer")]
		NSString iTunesMetadataKeyProducer { get; }

		[Field ("AVMetadataiTunesMetadataKeyPerformer")]
		NSString iTunesMetadataKeyPerformer { get; }

		[Field ("AVMetadataiTunesMetadataKeyPublisher")]
		NSString iTunesMetadataKeyPublisher { get; }

		[Field ("AVMetadataiTunesMetadataKeySoundEngineer")]
		NSString iTunesMetadataKeySoundEngineer { get; }

		[Field ("AVMetadataiTunesMetadataKeySoloist")]
		NSString iTunesMetadataKeySoloist { get; }

		[Field ("AVMetadataiTunesMetadataKeyCredits")]
		NSString iTunesMetadataKeyCredits { get; }

		[Field ("AVMetadataiTunesMetadataKeyThanks")]
		NSString iTunesMetadataKeyThanks { get; }

		[Field ("AVMetadataiTunesMetadataKeyOnlineExtras")]
		NSString iTunesMetadataKeyOnlineExtras { get; }

		[Field ("AVMetadataiTunesMetadataKeyExecProducer")]
		NSString iTunesMetadataKeyExecProducer { get; }

#if !NET
		[Field ("AVMetadataFormatID3Metadata")]
		[Obsolete ("Use 'AVMetadataFormat' enum values.")]
		NSString FormatID3Metadata { get; }
#endif

		[Field ("AVMetadataKeySpaceID3")]
		NSString KeySpaceID3 { get; }


		[Field ("AVMetadataID3MetadataKeyAudioEncryption")]
		NSString ID3MetadataKeyAudioEncryption { get; }

		[Field ("AVMetadataID3MetadataKeyAttachedPicture")]
		NSString ID3MetadataKeyAttachedPicture { get; }

		[Field ("AVMetadataID3MetadataKeyAudioSeekPointIndex")]
		NSString ID3MetadataKeyAudioSeekPointIndex { get; }

		[Field ("AVMetadataID3MetadataKeyComments")]
		NSString ID3MetadataKeyComments { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataID3MetadataKeyCommercial")]
		NSString ID3MetadataKeyCommercial { get; }

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("AVMetadataID3MetadataKeyCommerical")]
		NSString ID3MetadataKeyCommerical { get; }

		[Field ("AVMetadataID3MetadataKeyEncryption")]
		NSString ID3MetadataKeyEncryption { get; }

		[Field ("AVMetadataID3MetadataKeyEqualization")]
		NSString ID3MetadataKeyEqualization { get; }

		[Field ("AVMetadataID3MetadataKeyEqualization2")]
		NSString ID3MetadataKeyEqualization2 { get; }

		[Field ("AVMetadataID3MetadataKeyEventTimingCodes")]
		NSString ID3MetadataKeyEventTimingCodes { get; }

		[Field ("AVMetadataID3MetadataKeyGeneralEncapsulatedObject")]
		NSString ID3MetadataKeyGeneralEncapsulatedObject { get; }

		[Field ("AVMetadataID3MetadataKeyGroupIdentifier")]
		NSString ID3MetadataKeyGroupIdentifier { get; }

		[Field ("AVMetadataID3MetadataKeyInvolvedPeopleList_v23")]
		NSString ID3MetadataKeyInvolvedPeopleList { get; }

		[Field ("AVMetadataID3MetadataKeyLink")]
		NSString ID3MetadataKeyLink { get; }

		[Field ("AVMetadataID3MetadataKeyMusicCDIdentifier")]
		NSString ID3MetadataKeyMusicCDIdentifier { get; }

		[Field ("AVMetadataID3MetadataKeyMPEGLocationLookupTable")]
		NSString ID3MetadataKeyMPEGLocationLookupTable { get; }

		[Field ("AVMetadataID3MetadataKeyOwnership")]
		NSString ID3MetadataKeyOwnership { get; }

		[Field ("AVMetadataID3MetadataKeyPrivate")]
		NSString ID3MetadataKeyPrivate { get; }

		[Field ("AVMetadataID3MetadataKeyPlayCounter")]
		NSString ID3MetadataKeyPlayCounter { get; }

		[Field ("AVMetadataID3MetadataKeyPopularimeter")]
		NSString ID3MetadataKeyPopularimeter { get; }

		[Field ("AVMetadataID3MetadataKeyPositionSynchronization")]
		NSString ID3MetadataKeyPositionSynchronization { get; }

		[Field ("AVMetadataID3MetadataKeyRecommendedBufferSize")]
		NSString ID3MetadataKeyRecommendedBufferSize { get; }

		[Field ("AVMetadataID3MetadataKeyRelativeVolumeAdjustment")]
		NSString ID3MetadataKeyRelativeVolumeAdjustment { get; }

		[Field ("AVMetadataID3MetadataKeyRelativeVolumeAdjustment2")]
		NSString ID3MetadataKeyRelativeVolumeAdjustment2 { get; }

		[Field ("AVMetadataID3MetadataKeyReverb")]
		NSString ID3MetadataKeyReverb { get; }

		[Field ("AVMetadataID3MetadataKeySeek")]
		NSString ID3MetadataKeySeek { get; }

		[Field ("AVMetadataID3MetadataKeySignature")]
		NSString ID3MetadataKeySignature { get; }

		[Field ("AVMetadataID3MetadataKeySynchronizedLyric")]
		NSString ID3MetadataKeySynchronizedLyric { get; }

		[Field ("AVMetadataID3MetadataKeySynchronizedTempoCodes")]
		NSString ID3MetadataKeySynchronizedTempoCodes { get; }

		[Field ("AVMetadataID3MetadataKeyAlbumTitle")]
		NSString ID3MetadataKeyAlbumTitle { get; }

		[Field ("AVMetadataID3MetadataKeyBeatsPerMinute")]
		NSString ID3MetadataKeyBeatsPerMinute { get; }

		[Field ("AVMetadataID3MetadataKeyComposer")]
		NSString ID3MetadataKeyComposer { get; }

		[Field ("AVMetadataID3MetadataKeyContentType")]
		NSString ID3MetadataKeyContentType { get; }

		[Field ("AVMetadataID3MetadataKeyCopyright")]
		NSString ID3MetadataKeyCopyright { get; }

		[Field ("AVMetadataID3MetadataKeyDate")]
		NSString ID3MetadataKeyDate { get; }

		[Field ("AVMetadataID3MetadataKeyEncodingTime")]
		NSString ID3MetadataKeyEncodingTime { get; }

		[Field ("AVMetadataID3MetadataKeyPlaylistDelay")]
		NSString ID3MetadataKeyPlaylistDelay { get; }

		[Field ("AVMetadataID3MetadataKeyOriginalReleaseTime")]
		NSString ID3MetadataKeyOriginalReleaseTime { get; }

		[Field ("AVMetadataID3MetadataKeyRecordingTime")]
		NSString ID3MetadataKeyRecordingTime { get; }

		[Field ("AVMetadataID3MetadataKeyReleaseTime")]
		NSString ID3MetadataKeyReleaseTime { get; }

		[Field ("AVMetadataID3MetadataKeyTaggingTime")]
		NSString ID3MetadataKeyTaggingTime { get; }

		[Field ("AVMetadataID3MetadataKeyEncodedBy")]
		NSString ID3MetadataKeyEncodedBy { get; }

		[Field ("AVMetadataID3MetadataKeyLyricist")]
		NSString ID3MetadataKeyLyricist { get; }

		[Field ("AVMetadataID3MetadataKeyFileType")]
		NSString ID3MetadataKeyFileType { get; }

		[Field ("AVMetadataID3MetadataKeyTime")]
		NSString ID3MetadataKeyTime { get; }

		[Field ("AVMetadataID3MetadataKeyInvolvedPeopleList_v24")]
		NSString ID3MetadataKeyInvolvedPeopleList_v24 { get; }

		[Field ("AVMetadataID3MetadataKeyContentGroupDescription")]
		NSString ID3MetadataKeyContentGroupDescription { get; }

		[Field ("AVMetadataID3MetadataKeyTitleDescription")]
		NSString ID3MetadataKeyTitleDescription { get; }

		[Field ("AVMetadataID3MetadataKeySubTitle")]
		NSString ID3MetadataKeySubTitle { get; }

		[Field ("AVMetadataID3MetadataKeyInitialKey")]
		NSString ID3MetadataKeyInitialKey { get; }

		[Field ("AVMetadataID3MetadataKeyLanguage")]
		NSString ID3MetadataKeyLanguage { get; }

		[Field ("AVMetadataID3MetadataKeyLength")]
		NSString ID3MetadataKeyLength { get; }

		[Field ("AVMetadataID3MetadataKeyMusicianCreditsList")]
		NSString ID3MetadataKeyMusicianCreditsList { get; }

		[Field ("AVMetadataID3MetadataKeyMediaType")]
		NSString ID3MetadataKeyMediaType { get; }

		[Field ("AVMetadataID3MetadataKeyMood")]
		NSString ID3MetadataKeyMood { get; }

		[Field ("AVMetadataID3MetadataKeyOriginalAlbumTitle")]
		NSString ID3MetadataKeyOriginalAlbumTitle { get; }

		[Field ("AVMetadataID3MetadataKeyOriginalFilename")]
		NSString ID3MetadataKeyOriginalFilename { get; }

		[Field ("AVMetadataID3MetadataKeyOriginalLyricist")]
		NSString ID3MetadataKeyOriginalLyricist { get; }

		[Field ("AVMetadataID3MetadataKeyOriginalArtist")]
		NSString ID3MetadataKeyOriginalArtist { get; }

		[Field ("AVMetadataID3MetadataKeyOriginalReleaseYear")]
		NSString ID3MetadataKeyOriginalReleaseYear { get; }

		[Field ("AVMetadataID3MetadataKeyFileOwner")]
		NSString ID3MetadataKeyFileOwner { get; }

		[Field ("AVMetadataID3MetadataKeyLeadPerformer")]
		NSString ID3MetadataKeyLeadPerformer { get; }

		[Field ("AVMetadataID3MetadataKeyBand")]
		NSString ID3MetadataKeyBand { get; }

		[Field ("AVMetadataID3MetadataKeyConductor")]
		NSString ID3MetadataKeyConductor { get; }

		[Field ("AVMetadataID3MetadataKeyModifiedBy")]
		NSString ID3MetadataKeyModifiedBy { get; }

		[Field ("AVMetadataID3MetadataKeyPartOfASet")]
		NSString ID3MetadataKeyPartOfASet { get; }

		[Field ("AVMetadataID3MetadataKeyProducedNotice")]
		NSString ID3MetadataKeyProducedNotice { get; }

		[Field ("AVMetadataID3MetadataKeyPublisher")]
		NSString ID3MetadataKeyPublisher { get; }

		[Field ("AVMetadataID3MetadataKeyTrackNumber")]
		NSString ID3MetadataKeyTrackNumber { get; }

		[Field ("AVMetadataID3MetadataKeyRecordingDates")]
		NSString ID3MetadataKeyRecordingDates { get; }

		[Field ("AVMetadataID3MetadataKeyInternetRadioStationName")]
		NSString ID3MetadataKeyInternetRadioStationName { get; }

		[Field ("AVMetadataID3MetadataKeyInternetRadioStationOwner")]
		NSString ID3MetadataKeyInternetRadioStationOwner { get; }

		[Field ("AVMetadataID3MetadataKeySize")]
		NSString ID3MetadataKeySize { get; }

		[Field ("AVMetadataID3MetadataKeyAlbumSortOrder")]
		NSString ID3MetadataKeyAlbumSortOrder { get; }

		[Field ("AVMetadataID3MetadataKeyPerformerSortOrder")]
		NSString ID3MetadataKeyPerformerSortOrder { get; }

		[Field ("AVMetadataID3MetadataKeyTitleSortOrder")]
		NSString ID3MetadataKeyTitleSortOrder { get; }

		[Field ("AVMetadataID3MetadataKeyInternationalStandardRecordingCode")]
		NSString ID3MetadataKeyInternationalStandardRecordingCode { get; }

		[Field ("AVMetadataID3MetadataKeyEncodedWith")]
		NSString ID3MetadataKeyEncodedWith { get; }

		[Field ("AVMetadataID3MetadataKeySetSubtitle")]
		NSString ID3MetadataKeySetSubtitle { get; }

		[Field ("AVMetadataID3MetadataKeyYear")]
		NSString ID3MetadataKeyYear { get; }

		[Field ("AVMetadataID3MetadataKeyUserText")]
		NSString ID3MetadataKeyUserText { get; }

		[Field ("AVMetadataID3MetadataKeyUniqueFileIdentifier")]
		NSString ID3MetadataKeyUniqueFileIdentifier { get; }

		[Field ("AVMetadataID3MetadataKeyTermsOfUse")]
		NSString ID3MetadataKeyTermsOfUse { get; }

		[Field ("AVMetadataID3MetadataKeyUnsynchronizedLyric")]
		NSString ID3MetadataKeyUnsynchronizedLyric { get; }

		[Field ("AVMetadataID3MetadataKeyCommercialInformation")]
		NSString ID3MetadataKeyCommercialInformation { get; }

		[Field ("AVMetadataID3MetadataKeyCopyrightInformation")]
		NSString ID3MetadataKeyCopyrightInformation { get; }

		[Field ("AVMetadataID3MetadataKeyOfficialAudioFileWebpage")]
		NSString ID3MetadataKeyOfficialAudioFileWebpage { get; }

		[Field ("AVMetadataID3MetadataKeyOfficialArtistWebpage")]
		NSString ID3MetadataKeyOfficialArtistWebpage { get; }

		[Field ("AVMetadataID3MetadataKeyOfficialAudioSourceWebpage")]
		NSString ID3MetadataKeyOfficialAudioSourceWebpage { get; }

		[Field ("AVMetadataID3MetadataKeyOfficialInternetRadioStationHomepage")]
		NSString ID3MetadataKeyOfficialInternetRadioStationHomepage { get; }

		[Field ("AVMetadataID3MetadataKeyPayment")]
		NSString ID3MetadataKeyPayment { get; }

		[Field ("AVMetadataID3MetadataKeyOfficialPublisherWebpage")]
		NSString ID3MetadataKeyOfficialPublisherWebpage { get; }

		[Field ("AVMetadataID3MetadataKeyUserURL")]
		NSString ID3MetadataKeyUserURL { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataISOUserDataKeyTaggedCharacteristic")]
		NSString IsoUserDataKeyTaggedCharacteristic { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataISOUserDataKeyDate")]
		NSString IsoUserDataKeyDate { get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVMetadataISOUserDataKeyAccessibilityDescription")]
		NSString IsoUserDataKeyAccessibilityDescription { get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVMetadataIdentifierISOUserDataAccessibilityDescription")]
		NSString IsoUserDataAccessibilityDescription { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataKeySpaceIcy")]
		NSString KeySpaceIcy { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataIcyMetadataKeyStreamTitle")]
		NSString IcyMetadataKeyStreamTitle { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataIcyMetadataKeyStreamURL")]
		NSString IcyMetadataKeyStreamUrl { get; }

#if !NET
		[Field ("AVMetadataFormatHLSMetadata")]
		[Obsolete ("Use 'AVMetadataFormat' enum values.")]
		NSString FormatHlsMetadata { get; }
#endif

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataKeySpaceHLSDateRange")]
		NSString KeySpaceHlsDateRange { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataKeySpaceAudioFile")]
		NSString KeySpaceAudioFile { get; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("AVMetadataIdentifierQuickTimeMetadataIsMontage")]
		NSString QuickTimeMetadataIsMontage { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Static]
	interface AVMetadataExtraAttribute {

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataExtraAttributeValueURIKey")]
		NSString ValueUriKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataExtraAttributeBaseURIKey")]
		NSString BaseUriKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVMetadataExtraAttributeInfoKey")]
		NSString InfoKey { get; }
	}

	class AVMetadataIdentifiers {
		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		interface CommonIdentifier {
			[Field ("AVMetadataCommonIdentifierTitle")]
			NSString Title { get; }

			[Field ("AVMetadataCommonIdentifierCreator")]
			NSString Creator { get; }

			[Field ("AVMetadataCommonIdentifierSubject")]
			NSString Subject { get; }

			[Field ("AVMetadataCommonIdentifierDescription")]
			NSString Description { get; }

			[Field ("AVMetadataCommonIdentifierPublisher")]
			NSString Publisher { get; }

			[Field ("AVMetadataCommonIdentifierContributor")]
			NSString Contributor { get; }

			[Field ("AVMetadataCommonIdentifierCreationDate")]
			NSString CreationDate { get; }

			[Field ("AVMetadataCommonIdentifierLastModifiedDate")]
			NSString LastModifiedDate { get; }

			[Field ("AVMetadataCommonIdentifierType")]
			NSString Type { get; }

			[Field ("AVMetadataCommonIdentifierFormat")]
			NSString Format { get; }

			[Field ("AVMetadataCommonIdentifierAssetIdentifier")]
			NSString AssetIdentifier { get; }

			[Field ("AVMetadataCommonIdentifierSource")]
			NSString Source { get; }

			[Field ("AVMetadataCommonIdentifierLanguage")]
			NSString Language { get; }

			[Field ("AVMetadataCommonIdentifierRelation")]
			NSString Relation { get; }

			[Field ("AVMetadataCommonIdentifierLocation")]
			NSString Location { get; }

			[Field ("AVMetadataCommonIdentifierCopyrights")]
			NSString Copyrights { get; }

			[Field ("AVMetadataCommonIdentifierAlbumName")]
			NSString AlbumName { get; }

			[Field ("AVMetadataCommonIdentifierAuthor")]
			NSString Author { get; }

			[Field ("AVMetadataCommonIdentifierArtist")]
			NSString Artist { get; }

			[Field ("AVMetadataCommonIdentifierArtwork")]
			NSString Artwork { get; }

			[Field ("AVMetadataCommonIdentifierMake")]
			NSString Make { get; }

			[Field ("AVMetadataCommonIdentifierModel")]
			NSString Model { get; }

			[Field ("AVMetadataCommonIdentifierSoftware")]
			NSString Software { get; }

			[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
			[MacCatalyst (14, 0)]
			[Field ("AVMetadataCommonIdentifierAccessibilityDescription")]
			NSString AccessibilityDescription { get; }

		}

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		interface QuickTime {
			[Field ("AVMetadataIdentifierQuickTimeUserDataAlbum")]
			NSString UserDataAlbum { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataArranger")]
			NSString UserDataArranger { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataArtist")]
			NSString UserDataArtist { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataAuthor")]
			NSString UserDataAuthor { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataChapter")]
			NSString UserDataChapter { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataComment")]
			NSString UserDataComment { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataComposer")]
			NSString UserDataComposer { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataCopyright")]
			NSString UserDataCopyright { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataCreationDate")]
			NSString UserDataCreationDate { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataDescription")]
			NSString UserDataDescription { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataDirector")]
			NSString UserDataDirector { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataDisclaimer")]
			NSString UserDataDisclaimer { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataEncodedBy")]
			NSString UserDataEncodedBy { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataFullName")]
			NSString UserDataFullName { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataGenre")]
			NSString UserDataGenre { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataHostComputer")]
			NSString UserDataHostComputer { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataInformation")]
			NSString UserDataInformation { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataKeywords")]
			NSString UserDataKeywords { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataMake")]
			NSString UserDataMake { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataModel")]
			NSString UserDataModel { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataOriginalArtist")]
			NSString UserDataOriginalArtist { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataOriginalFormat")]
			NSString UserDataOriginalFormat { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataOriginalSource")]
			NSString UserDataOriginalSource { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataPerformers")]
			NSString UserDataPerformers { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataProducer")]
			NSString UserDataProducer { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataPublisher")]
			NSString UserDataPublisher { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataProduct")]
			NSString UserDataProduct { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataSoftware")]
			NSString UserDataSoftware { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataSpecialPlaybackRequirements")]
			NSString UserDataSpecialPlaybackRequirements { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataTrack")]
			NSString UserDataTrack { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataWarning")]
			NSString UserDataWarning { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataWriter")]
			NSString UserDataWriter { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataURLLink")]
			NSString UserDataUrlLink { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataLocationISO6709")]
			NSString UserDataLocationISO6709 { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataTrackName")]
			NSString UserDataTrackName { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataCredits")]
			NSString UserDataCredits { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataPhonogramRights")]
			NSString UserDataPhonogramRights { get; }

			[Field ("AVMetadataIdentifierQuickTimeUserDataTaggedCharacteristic")]
			NSString UserDataTaggedCharacteristic { get; }

			[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
			[MacCatalyst (14, 0)]
			[Field ("AVMetadataIdentifierQuickTimeUserDataAccessibilityDescription")]
			NSString UserDataAccessibilityDescription { get; }
		}

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		interface Iso {

			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierISOUserDataDate")]
			NSString UserDataDate { get; }

			[Field ("AVMetadataIdentifierISOUserDataCopyright")]
			NSString UserDataCopyright { get; }

			[Field ("AVMetadataIdentifierISOUserDataTaggedCharacteristic")]
			NSString UserDataTaggedCharacteristic { get; }
		}

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		interface ThreeGP {
			[Field ("AVMetadataIdentifier3GPUserDataCopyright")]
			NSString UserDataCopyright { get; }

			[Field ("AVMetadataIdentifier3GPUserDataAuthor")]
			NSString UserDataAuthor { get; }

			[Field ("AVMetadataIdentifier3GPUserDataPerformer")]
			NSString UserDataPerformer { get; }

			[Field ("AVMetadataIdentifier3GPUserDataGenre")]
			NSString UserDataGenre { get; }

			[Field ("AVMetadataIdentifier3GPUserDataRecordingYear")]
			NSString UserDataRecordingYear { get; }

			[Field ("AVMetadataIdentifier3GPUserDataLocation")]
			NSString UserDataLocation { get; }

			[Field ("AVMetadataIdentifier3GPUserDataTitle")]
			NSString UserDataTitle { get; }

			[Field ("AVMetadataIdentifier3GPUserDataDescription")]
			NSString UserDataDescription { get; }

			[Field ("AVMetadataIdentifier3GPUserDataCollection")]
			NSString UserDataCollection { get; }

			[Field ("AVMetadataIdentifier3GPUserDataUserRating")]
			NSString UserDataUserRating { get; }

			[Field ("AVMetadataIdentifier3GPUserDataThumbnail")]
			NSString UserDataThumbnail { get; }

			[Field ("AVMetadataIdentifier3GPUserDataAlbumAndTrack")]
			NSString UserDataAlbumAndTrack { get; }

			[Field ("AVMetadataIdentifier3GPUserDataKeywordList")]
			NSString UserDataKeywordList { get; }

			[Field ("AVMetadataIdentifier3GPUserDataMediaClassification")]
			NSString UserDataMediaClassification { get; }

			[Field ("AVMetadataIdentifier3GPUserDataMediaRating")]
			NSString UserDataMediaRating { get; }
		}

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		interface QuickTimeMetadata {
			[Field ("AVMetadataIdentifierQuickTimeMetadataAuthor")]
			NSString Author { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataComment")]
			NSString Comment { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataCopyright")]
			NSString Copyright { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataCreationDate")]
			NSString CreationDate { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataDirector")]
			NSString Director { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataDisplayName")]
			NSString DisplayName { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataInformation")]
			NSString Information { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataKeywords")]
			NSString Keywords { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataProducer")]
			NSString Producer { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataPublisher")]
			NSString Publisher { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataAlbum")]
			NSString Album { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataArtist")]
			NSString Artist { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataArtwork")]
			NSString Artwork { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataDescription")]
			NSString Description { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataSoftware")]
			NSString Software { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataYear")]
			NSString Year { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataGenre")]
			NSString Genre { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataiXML")]
			NSString iXML { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataLocationISO6709")]
			NSString LocationISO6709 { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataMake")]
			NSString Make { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataModel")]
			NSString Model { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataArranger")]
			NSString Arranger { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataEncodedBy")]
			NSString EncodedBy { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataOriginalArtist")]
			NSString OriginalArtist { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataPerformer")]
			NSString Performer { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataComposer")]
			NSString Composer { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataCredits")]
			NSString Credits { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataPhonogramRights")]
			NSString PhonogramRights { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataCameraIdentifier")]
			NSString CameraIdentifier { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataCameraFrameReadoutTime")]
			NSString CameraFrameReadoutTime { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataTitle")]
			NSString Title { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataCollectionUser")]
			NSString CollectionUser { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataRatingUser")]
			NSString RatingUser { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataLocationName")]
			NSString LocationName { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataLocationBody")]
			NSString LocationBody { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataLocationNote")]
			NSString LocationNote { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataLocationRole")]
			NSString LocationRole { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataLocationDate")]
			NSString LocationDate { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataDirectionFacing")]
			NSString DirectionFacing { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataDirectionMotion")]
			NSString DirectionMotion { get; }

			[Field ("AVMetadataIdentifierQuickTimeMetadataPreferredAffineTransform")]
			NSString PreferredAffineTransform { get; }

			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataDetectedFace")]
			NSString DetectedFace { get; }

			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataVideoOrientation")]
			NSString VideoOrientation { get; }

			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataContentIdentifier")]
			NSString ContentIdentifier { get; }

			[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0)]
			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataSpatialOverCaptureQualityScoringVersion")]
			NSString SpatialOverCaptureQualityScoringVersion { get; }

			[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0)]
			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataSpatialOverCaptureQualityScore")]
			NSString SpatialOverCaptureQualityScore { get; }

			[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0)]
			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataLivePhotoVitalityScoringVersion")]
			NSString LivePhotoVitalityScoringVersion { get; }

			[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0)]
			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataLivePhotoVitalityScore")]
			NSString LivePhotoVitalityScore { get; }

			[NoWatch, NoTV, NoMac, iOS (13, 0)]
			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataDetectedSalientObject")]
			NSString DetectedSalientObject { get; }

			[NoWatch, NoTV, NoMac, iOS (13, 0)]
			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataDetectedHumanBody")]
			NSString DetectedHumanBody { get; }

			[NoWatch, NoTV, NoMac, iOS (13, 0)]
			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataDetectedDogBody")]
			NSString DetectedDogBody { get; }

			[NoWatch, NoTV, NoMac, iOS (13, 0)]
			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataDetectedCatBody")]
			NSString DetectedCatBody { get; }

			[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0)]
			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataAutoLivePhoto")]
			NSString AutoLivePhoto { get; }

			[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
			[MacCatalyst (14, 0)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataAccessibilityDescription")]
			NSString AccessibilityDescription { get; }

			[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
			[MacCatalyst (14, 0)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataLocationHorizontalAccuracyInMeters")]
			NSString LocationHorizontalAccuracyInMeters { get; }
		}

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		interface iTunesMetadata {
			[Field ("AVMetadataIdentifieriTunesMetadataAlbum")]
			NSString Album { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataArtist")]
			NSString Artist { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataUserComment")]
			NSString UserComment { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataCoverArt")]
			NSString CoverArt { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataCopyright")]
			NSString Copyright { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataReleaseDate")]
			NSString ReleaseDate { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataEncodedBy")]
			NSString EncodedBy { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataPredefinedGenre")]
			NSString PredefinedGenre { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataUserGenre")]
			NSString UserGenre { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataSongName")]
			NSString SongName { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataTrackSubTitle")]
			NSString TrackSubTitle { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataEncodingTool")]
			NSString EncodingTool { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataComposer")]
			NSString Composer { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataAlbumArtist")]
			NSString AlbumArtist { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataAccountKind")]
			NSString AccountKind { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataAppleID")]
			NSString AppleID { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataArtistID")]
			NSString ArtistID { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataSongID")]
			NSString SongID { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataDiscCompilation")]
			NSString DiscCompilation { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataDiscNumber")]
			NSString DiscNumber { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataGenreID")]
			NSString GenreID { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataGrouping")]
			NSString Grouping { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataPlaylistID")]
			NSString PlaylistID { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataContentRating")]
			NSString ContentRating { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataBeatsPerMin")]
			NSString BeatsPerMin { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataTrackNumber")]
			NSString TrackNumber { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataArtDirector")]
			NSString ArtDirector { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataArranger")]
			NSString Arranger { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataAuthor")]
			NSString Author { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataLyrics")]
			NSString Lyrics { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataAcknowledgement")]
			NSString Acknowledgement { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataConductor")]
			NSString Conductor { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataDescription")]
			NSString Description { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataDirector")]
			NSString Director { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataEQ")]
			NSString EQ { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataLinerNotes")]
			NSString LinerNotes { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataRecordCompany")]
			NSString RecordCompany { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataOriginalArtist")]
			NSString OriginalArtist { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataPhonogramRights")]
			NSString PhonogramRights { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataProducer")]
			NSString Producer { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataPerformer")]
			NSString Performer { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataPublisher")]
			NSString Publisher { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataSoundEngineer")]
			NSString SoundEngineer { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataSoloist")]
			NSString Soloist { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataCredits")]
			NSString Credits { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataThanks")]
			NSString Thanks { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataOnlineExtras")]
			NSString OnlineExtras { get; }

			[Field ("AVMetadataIdentifieriTunesMetadataExecProducer")]
			NSString ExecProducer { get; }
		}

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		interface ID3Metadata {
			[Field ("AVMetadataIdentifierID3MetadataAudioEncryption")]
			NSString AudioEncryption { get; }

			[Field ("AVMetadataIdentifierID3MetadataAttachedPicture")]
			NSString AttachedPicture { get; }

			[Field ("AVMetadataIdentifierID3MetadataAudioSeekPointIndex")]
			NSString AudioSeekPointIndex { get; }

			[Field ("AVMetadataIdentifierID3MetadataComments")]
			NSString Comments { get; }

			[MacCatalyst (13, 1)]
			[Field ("AVMetadataIdentifierID3MetadataCommercial")]
			NSString Commercial { get; }

			[Deprecated (PlatformName.iOS, 9, 0)]
			[Deprecated (PlatformName.TvOS, 9, 0)]
			[Deprecated (PlatformName.MacOSX, 10, 11)]
			[MacCatalyst (13, 1)]
			[Deprecated (PlatformName.MacCatalyst, 13, 1)]
			[Field ("AVMetadataIdentifierID3MetadataCommerical")]
			NSString Commerical { get; }

			[Field ("AVMetadataIdentifierID3MetadataEncryption")]
			NSString Encryption { get; }

			[Field ("AVMetadataIdentifierID3MetadataEqualization")]
			NSString Equalization { get; }

			[Field ("AVMetadataIdentifierID3MetadataEqualization2")]
			NSString Equalization2 { get; }

			[Field ("AVMetadataIdentifierID3MetadataEventTimingCodes")]
			NSString EventTimingCodes { get; }

			[Field ("AVMetadataIdentifierID3MetadataGeneralEncapsulatedObject")]
			NSString GeneralEncapsulatedObject { get; }

			[Field ("AVMetadataIdentifierID3MetadataGroupIdentifier")]
			NSString GroupIdentifier { get; }

			[Field ("AVMetadataIdentifierID3MetadataInvolvedPeopleList_v23")]
			NSString InvolvedPeopleList_v23 { get; }

			[Field ("AVMetadataIdentifierID3MetadataLink")]
			NSString Link { get; }

			[Field ("AVMetadataIdentifierID3MetadataMusicCDIdentifier")]
			NSString MusicCDIdentifier { get; }

			[Field ("AVMetadataIdentifierID3MetadataMPEGLocationLookupTable")]
			NSString MpegLocationLookupTable { get; }

			[Field ("AVMetadataIdentifierID3MetadataOwnership")]
			NSString Ownership { get; }

			[Field ("AVMetadataIdentifierID3MetadataPrivate")]
			NSString Private { get; }

			[Field ("AVMetadataIdentifierID3MetadataPlayCounter")]
			NSString PlayCounter { get; }

			[Field ("AVMetadataIdentifierID3MetadataPopularimeter")]
			NSString Popularimeter { get; }

			[Field ("AVMetadataIdentifierID3MetadataPositionSynchronization")]
			NSString PositionSynchronization { get; }

			[Field ("AVMetadataIdentifierID3MetadataRecommendedBufferSize")]
			NSString RecommendedBufferSize { get; }

			[Field ("AVMetadataIdentifierID3MetadataRelativeVolumeAdjustment")]
			NSString RelativeVolumeAdjustment { get; }

			[Field ("AVMetadataIdentifierID3MetadataRelativeVolumeAdjustment2")]
			NSString RelativeVolumeAdjustment2 { get; }

			[Field ("AVMetadataIdentifierID3MetadataReverb")]
			NSString Reverb { get; }

			[Field ("AVMetadataIdentifierID3MetadataSeek")]
			NSString Seek { get; }

			[Field ("AVMetadataIdentifierID3MetadataSignature")]
			NSString Signature { get; }

			[Field ("AVMetadataIdentifierID3MetadataSynchronizedLyric")]
			NSString SynchronizedLyric { get; }

			[Field ("AVMetadataIdentifierID3MetadataSynchronizedTempoCodes")]
			NSString SynchronizedTempoCodes { get; }

			[Field ("AVMetadataIdentifierID3MetadataAlbumTitle")]
			NSString AlbumTitle { get; }

			[Field ("AVMetadataIdentifierID3MetadataBeatsPerMinute")]
			NSString BeatsPerMinute { get; }

			[Field ("AVMetadataIdentifierID3MetadataComposer")]
			NSString Composer { get; }

			[Field ("AVMetadataIdentifierID3MetadataContentType")]
			NSString ContentType { get; }

			[Field ("AVMetadataIdentifierID3MetadataCopyright")]
			NSString Copyright { get; }

			[Field ("AVMetadataIdentifierID3MetadataDate")]
			NSString Date { get; }

			[Field ("AVMetadataIdentifierID3MetadataEncodingTime")]
			NSString EncodingTime { get; }

			[Field ("AVMetadataIdentifierID3MetadataPlaylistDelay")]
			NSString PlaylistDelay { get; }

			[Field ("AVMetadataIdentifierID3MetadataOriginalReleaseTime")]
			NSString OriginalReleaseTime { get; }

			[Field ("AVMetadataIdentifierID3MetadataRecordingTime")]
			NSString RecordingTime { get; }

			[Field ("AVMetadataIdentifierID3MetadataReleaseTime")]
			NSString ReleaseTime { get; }

			[Field ("AVMetadataIdentifierID3MetadataTaggingTime")]
			NSString TaggingTime { get; }

			[Field ("AVMetadataIdentifierID3MetadataEncodedBy")]
			NSString EncodedBy { get; }

			[Field ("AVMetadataIdentifierID3MetadataLyricist")]
			NSString Lyricist { get; }

			[Field ("AVMetadataIdentifierID3MetadataFileType")]
			NSString FileType { get; }

			[Field ("AVMetadataIdentifierID3MetadataTime")]
			NSString Time { get; }

			[Field ("AVMetadataIdentifierID3MetadataInvolvedPeopleList_v24")]
			NSString InvolvedPeopleList_v24 { get; }

			[Field ("AVMetadataIdentifierID3MetadataContentGroupDescription")]
			NSString ContentGroupDescription { get; }

			[Field ("AVMetadataIdentifierID3MetadataTitleDescription")]
			NSString TitleDescription { get; }

			[Field ("AVMetadataIdentifierID3MetadataSubTitle")]
			NSString SubTitle { get; }

			[Field ("AVMetadataIdentifierID3MetadataInitialKey")]
			NSString InitialKey { get; }

			[Field ("AVMetadataIdentifierID3MetadataLanguage")]
			NSString Language { get; }

			[Field ("AVMetadataIdentifierID3MetadataLength")]
			NSString Length { get; }

			[Field ("AVMetadataIdentifierID3MetadataMusicianCreditsList")]
			NSString MusicianCreditsList { get; }

			[Field ("AVMetadataIdentifierID3MetadataMediaType")]
			NSString MediaType { get; }

			[Field ("AVMetadataIdentifierID3MetadataMood")]
			NSString Mood { get; }

			[Field ("AVMetadataIdentifierID3MetadataOriginalAlbumTitle")]
			NSString OriginalAlbumTitle { get; }

			[Field ("AVMetadataIdentifierID3MetadataOriginalFilename")]
			NSString OriginalFilename { get; }

			[Field ("AVMetadataIdentifierID3MetadataOriginalLyricist")]
			NSString OriginalLyricist { get; }

			[Field ("AVMetadataIdentifierID3MetadataOriginalArtist")]
			NSString OriginalArtist { get; }

			[Field ("AVMetadataIdentifierID3MetadataOriginalReleaseYear")]
			NSString OriginalReleaseYear { get; }

			[Field ("AVMetadataIdentifierID3MetadataFileOwner")]
			NSString FileOwner { get; }

			[Field ("AVMetadataIdentifierID3MetadataLeadPerformer")]
			NSString LeadPerformer { get; }

			[Field ("AVMetadataIdentifierID3MetadataBand")]
			NSString Band { get; }

			[Field ("AVMetadataIdentifierID3MetadataConductor")]
			NSString Conductor { get; }

			[Field ("AVMetadataIdentifierID3MetadataModifiedBy")]
			NSString ModifiedBy { get; }

			[Field ("AVMetadataIdentifierID3MetadataPartOfASet")]
			NSString PartOfASet { get; }

			[Field ("AVMetadataIdentifierID3MetadataProducedNotice")]
			NSString ProducedNotice { get; }

			[Field ("AVMetadataIdentifierID3MetadataPublisher")]
			NSString Publisher { get; }

			[Field ("AVMetadataIdentifierID3MetadataTrackNumber")]
			NSString TrackNumber { get; }

			[Field ("AVMetadataIdentifierID3MetadataRecordingDates")]
			NSString RecordingDates { get; }

			[Field ("AVMetadataIdentifierID3MetadataInternetRadioStationName")]
			NSString InternetRadioStationName { get; }

			[Field ("AVMetadataIdentifierID3MetadataInternetRadioStationOwner")]
			NSString InternetRadioStationOwner { get; }

			[Field ("AVMetadataIdentifierID3MetadataSize")]
			NSString Size { get; }

			[Field ("AVMetadataIdentifierID3MetadataAlbumSortOrder")]
			NSString AlbumSortOrder { get; }

			[Field ("AVMetadataIdentifierID3MetadataPerformerSortOrder")]
			NSString PerformerSortOrder { get; }

			[Field ("AVMetadataIdentifierID3MetadataTitleSortOrder")]
			NSString TitleSortOrder { get; }

			[Field ("AVMetadataIdentifierID3MetadataInternationalStandardRecordingCode")]
			NSString InternationalStandardRecordingCode { get; }

			[Field ("AVMetadataIdentifierID3MetadataEncodedWith")]
			NSString EncodedWith { get; }

			[Field ("AVMetadataIdentifierID3MetadataSetSubtitle")]
			NSString SetSubtitle { get; }

			[Field ("AVMetadataIdentifierID3MetadataYear")]
			NSString Year { get; }

			[Field ("AVMetadataIdentifierID3MetadataUserText")]
			NSString UserText { get; }

			[Field ("AVMetadataIdentifierID3MetadataUniqueFileIdentifier")]
			NSString UniqueFileIdentifier { get; }

			[Field ("AVMetadataIdentifierID3MetadataTermsOfUse")]
			NSString TermsOfUse { get; }

			[Field ("AVMetadataIdentifierID3MetadataUnsynchronizedLyric")]
			NSString UnsynchronizedLyric { get; }

			[Field ("AVMetadataIdentifierID3MetadataCommercialInformation")]
			NSString CommercialInformation { get; }

			[Field ("AVMetadataIdentifierID3MetadataCopyrightInformation")]
			NSString CopyrightInformation { get; }

			[Field ("AVMetadataIdentifierID3MetadataOfficialAudioFileWebpage")]
			NSString OfficialAudioFileWebpage { get; }

			[Field ("AVMetadataIdentifierID3MetadataOfficialArtistWebpage")]
			NSString OfficialArtistWebpage { get; }

			[Field ("AVMetadataIdentifierID3MetadataOfficialAudioSourceWebpage")]
			NSString OfficialAudioSourceWebpage { get; }

			[Field ("AVMetadataIdentifierID3MetadataOfficialInternetRadioStationHomepage")]
			NSString OfficialInternetRadioStationHomepage { get; }

			[Field ("AVMetadataIdentifierID3MetadataPayment")]
			NSString Payment { get; }

			[Field ("AVMetadataIdentifierID3MetadataOfficialPublisherWebpage")]
			NSString OfficialPublisherWebpage { get; }

			[Field ("AVMetadataIdentifierID3MetadataUserURL")]
			NSString UserUrl { get; }
		}

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		interface IcyMetadata {
			[Field ("AVMetadataIdentifierIcyMetadataStreamTitle")]
			NSString StreamTitle { get; }

			[Field ("AVMetadataIdentifierIcyMetadataStreamURL")]
			NSString StreamUrl { get; }
		}
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVMetadataItem : NSMutableCopying {
		[Export ("commonKey", ArgumentSemantic.Copy), NullAllowed]
		string CommonKey { get; }

		[Export ("keySpace", ArgumentSemantic.Copy), NullAllowed]
		string KeySpace { get; [NotImplemented] set; }

		[Export ("locale", ArgumentSemantic.Copy), NullAllowed]
		NSLocale Locale { get; [NotImplemented] set; }

		[Export ("time")]
		CMTime Time { get; [NotImplemented] set; }

		[Export ("value", ArgumentSemantic.Copy), NullAllowed]
		NSObject Value { get; [NotImplemented] set; }

		[Export ("extraAttributes", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary ExtraAttributes { get; [NotImplemented] set; }

		[Export ("key", ArgumentSemantic.Copy), NullAllowed]
		NSObject Key { get; }

		[Export ("stringValue"), NullAllowed]
		string StringValue { get; }

		[Export ("numberValue"), NullAllowed]
		NSNumber NumberValue { get; }

		[Export ("dateValue"), NullAllowed]
		NSDate DateValue { get; }

		[Export ("dataValue"), NullAllowed]
		NSData DataValue { get; }

		[Static]
		[Export ("metadataItemsFromArray:withLocale:")]
		AVMetadataItem [] FilterWithLocale (AVMetadataItem [] arrayToFilter, NSLocale locale);

		[Static]
		[Export ("metadataItemsFromArray:withKey:keySpace:")]
		AVMetadataItem [] FilterWithKey (AVMetadataItem [] metadataItems, [NullAllowed] NSObject key, [NullAllowed] string keySpace);

		[NoWatch] // headers say it is the watch, but the AVMetadataItemFilter is not
		[MacCatalyst (13, 1)]
		[Static, Export ("metadataItemsFromArray:filteredByMetadataItemFilter:")]
		AVMetadataItem [] FilterWithItemFilter (AVMetadataItem [] metadataItems, AVMetadataItemFilter metadataItemFilter);

		[Export ("duration")]
		CMTime Duration { get; [NotImplemented] set; }

		[Export ("statusOfValueForKey:error:")]
		AVKeyValueStatus StatusOfValueForKeyerror (string key, out NSError error);

		[Export ("loadValuesAsynchronouslyForKeys:completionHandler:")]
		[Async ("LoadValuesTaskAsync")]
		void LoadValuesAsynchronously (string [] keys, [NullAllowed] Action handler);

		[Static, Export ("metadataItemsFromArray:filteredAndSortedAccordingToPreferredLanguages:")]
		AVMetadataItem [] FilterFromPreferredLanguages (AVMetadataItem [] metadataItems, string [] preferredLanguages);

		[MacCatalyst (13, 1)]
		[Export ("identifier"), NullAllowed]
		NSString MetadataIdentifier { get; [NotImplemented] set; }

		[MacCatalyst (13, 1)]
		[Export ("extendedLanguageTag"), NullAllowed]
		string ExtendedLanguageTag { get; [NotImplemented] set; }

		[MacCatalyst (13, 1)]
		[Export ("dataType"), NullAllowed]
		NSString DataType { get; [NotImplemented] set; }

		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Static, Export ("identifierForKey:keySpace:")]
		NSString GetMetadataIdentifier (NSObject key, NSString keySpace);

		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Static, Export ("keySpaceForIdentifier:")]
		NSString GetKeySpaceForIdentifier (NSString identifier);

		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Static, Export ("keyForIdentifier:")]
		NSObject GetKeyForIdentifier (NSString identifier);

		[MacCatalyst (13, 1)]
		[Static, Export ("metadataItemsFromArray:filteredByIdentifier:")]
		AVMetadataItem [] FilterWithIdentifier (AVMetadataItem [] metadataItems, NSString metadataIdentifer);

		[MacCatalyst (13, 1)]
		[Export ("startDate"), NullAllowed]
		NSDate StartDate { get; [NotImplemented] set; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("metadataItemWithPropertiesOfMetadataItem:valueLoadingHandler:")]
		AVMetadataItem GetMetadataItem (AVMetadataItem metadataItem, Action<AVMetadataItemValueRequest> handler);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVMetadataItemValueRequest {

		[NullAllowed, Export ("metadataItem", ArgumentSemantic.Weak)]
		AVMetadataItem MetadataItem { get; }

		[Export ("respondWithValue:")]
		void Respond (NSObject value);

		[Export ("respondWithError:")]
		void Respond (NSError error);
	}

	[Watch (8, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // for binary compatibility this is added in AVMetadataItemFilter.cs w/[Obsolete]
	interface AVMetadataItemFilter {
		[Static, Export ("metadataItemFilterForSharing")]
		AVMetadataItemFilter ForSharing { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSGenericException Reason: Cannot instantiate AVMetadataObject because it is an abstract superclass.
	[DisableDefaultCtor]
	interface AVMetadataObject {
		[Export ("duration")]
		CMTime Duration { get; }

		[Export ("bounds")]
		CGRect Bounds { get; }

		[Export ("type")]
		NSString WeakType { get; }

		[Export ("time")]
		CMTime Time { get; }

#if !NET
		[Field ("AVMetadataObjectTypeFace")]
		NSString TypeFace { get; }

		[Field ("AVMetadataObjectTypeAztecCode")]
		NSString TypeAztecCode { get; }

		[Field ("AVMetadataObjectTypeCode128Code")]
		NSString TypeCode128Code { get; }

		[Field ("AVMetadataObjectTypeCode39Code")]
		NSString TypeCode39Code { get; }

		[Field ("AVMetadataObjectTypeCode39Mod43Code")]
		NSString TypeCode39Mod43Code { get; }

		[Field ("AVMetadataObjectTypeCode93Code")]
		NSString TypeCode93Code { get; }

		[Field ("AVMetadataObjectTypeEAN13Code")]
		NSString TypeEAN13Code { get; }

		[Field ("AVMetadataObjectTypeEAN8Code")]
		NSString TypeEAN8Code { get; }

		[Field ("AVMetadataObjectTypePDF417Code")]
		NSString TypePDF417Code { get; }

		[Field ("AVMetadataObjectTypeQRCode")]
		NSString TypeQRCode { get; }

		[Field ("AVMetadataObjectTypeUPCECode")]
		NSString TypeUPCECode { get; }

		[Field ("AVMetadataObjectTypeInterleaved2of5Code")]
		NSString TypeInterleaved2of5Code { get; }

		[Field ("AVMetadataObjectTypeITF14Code")]
		NSString TypeITF14Code { get; }

		[Field ("AVMetadataObjectTypeDataMatrixCode")]
		NSString TypeDataMatrixCode { get; }

		[NoWatch, NoTV, iOS (13, 0)]
		[Field ("AVMetadataObjectTypeCatBody")]
		NSString TypeCatBody { get; }

		[NoWatch, NoTV, iOS (13, 0)]
		[Field ("AVMetadataObjectTypeDogBody")]
		NSString TypeDogBody { get; }

		[NoWatch, NoTV, iOS (13, 0)]
		[Field ("AVMetadataObjectTypeHumanBody")]
		NSString TypeHumanBody { get; }

		[NoWatch, NoTV, iOS (13, 0)]
		[Field ("AVMetadataObjectTypeSalientObject")]
		NSString TypeSalientObject { get; }
#endif
	}

#if NET
	[NoWatch]
#endif
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Flags]
	enum AVMetadataObjectType : ulong {
		[Field (null)]
		None = 0,

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMetadataObjectTypeFace")]
		Face = 1 << 0,

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMetadataObjectTypeAztecCode")]
		AztecCode = 1 << 1,

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMetadataObjectTypeCode128Code")]
		Code128Code = 1 << 2,

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMetadataObjectTypeCode39Code")]
		Code39Code = 1 << 3,

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMetadataObjectTypeCode39Mod43Code")]
		Code39Mod43Code = 1 << 4,

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMetadataObjectTypeCode93Code")]
		Code93Code = 1 << 5,

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMetadataObjectTypeEAN13Code")]
		EAN13Code = 1 << 6,

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMetadataObjectTypeEAN8Code")]
		EAN8Code = 1 << 7,

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMetadataObjectTypePDF417Code")]
		PDF417Code = 1 << 8,

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMetadataObjectTypeQRCode")]
		QRCode = 1 << 9,

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVMetadataObjectTypeUPCECode")]
		UPCECode = 1 << 10,

		[NoWatch]
		[MacCatalyst (14, 0)]
		[Field ("AVMetadataObjectTypeInterleaved2of5Code")]
		Interleaved2of5Code = 1 << 11,

		[NoWatch]
		[MacCatalyst (14, 0)]
		[Field ("AVMetadataObjectTypeITF14Code")]
		ITF14Code = 1 << 12,

		[NoWatch]
		[MacCatalyst (14, 0)]
		[Field ("AVMetadataObjectTypeDataMatrixCode")]
		DataMatrixCode = 1 << 13,

		[iOS (13, 0)]
		[NoTV]
		[NoWatch]
		[MacCatalyst (14, 0)]
		[Field ("AVMetadataObjectTypeCatBody")]
		CatBody = 1 << 14,

		[iOS (13, 0)]
		[NoTV]
		[NoWatch]
		[MacCatalyst (14, 0)]
		[Field ("AVMetadataObjectTypeDogBody")]
		DogBody = 1 << 15,

		[iOS (13, 0)]
		[NoTV]
		[NoWatch]
		[MacCatalyst (14, 0)]
		[Field ("AVMetadataObjectTypeHumanBody")]
		HumanBody = 1 << 16,

		[iOS (13, 0)]
		[NoTV]
		[NoWatch]
		[MacCatalyst (14, 0)]
		[Field ("AVMetadataObjectTypeSalientObject")]
		SalientObject = 1 << 17,

		[TV (15, 4), NoWatch, MacCatalyst (15, 4), Mac (12, 3), iOS (15, 4)]
		[Field ("AVMetadataObjectTypeCodabarCode")]
		CodabarCode = 1 << 18,

		[TV (15, 4), NoWatch, MacCatalyst (15, 4), Mac (12, 3), iOS (15, 4)]
		[Field ("AVMetadataObjectTypeGS1DataBarCode")]
		GS1DataBarCode = 1 << 19,

		[TV (15, 4), NoWatch, MacCatalyst (15, 4), Mac (12, 3), iOS (15, 4)]
		[Field ("AVMetadataObjectTypeGS1DataBarExpandedCode")]
		GS1DataBarExpandedCode = 1 << 20,

		[TV (15, 4), NoWatch, MacCatalyst (15, 4), Mac (12, 3), iOS (15, 4)]
		[Field ("AVMetadataObjectTypeGS1DataBarLimitedCode")]
		GS1DataBarLimitedCode = 1 << 21,

		[TV (15, 4), NoWatch, MacCatalyst (15, 4), Mac (12, 3), iOS (15, 4)]
		[Field ("AVMetadataObjectTypeMicroQRCode")]
		MicroQRCode = 1 << 22,

		[TV (15, 4), NoWatch, MacCatalyst (15, 4), Mac (12, 3), iOS (15, 4)]
		[Field ("AVMetadataObjectTypeMicroPDF417Code")]
		MicroPdf417Code = 1 << 23,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[BaseType (typeof (AVMetadataObject))]
	interface AVMetadataFaceObject : NSCopying {
		[Export ("hasRollAngle")]
		bool HasRollAngle { get; }

		[Export ("rollAngle")]
		nfloat RollAngle { get; }

		[Export ("hasYawAngle")]
		bool HasYawAngle { get; }

		[Export ("yawAngle")]
		nfloat YawAngle { get; }

		[Export ("faceID")]
		nint FaceID { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[BaseType (typeof (AVMetadataObject))]
	interface AVMetadataMachineReadableCodeObject {
		[Export ("corners", ArgumentSemantic.Copy)]
		NSDictionary [] WeakCorners { get; }

		[NullAllowed, Export ("stringValue", ArgumentSemantic.Copy)]
		string StringValue { get; }

		// @interface AVMetadataMachineReadableCodeDescriptor (AVMetadataMachineReadableCodeObject)

		[MacCatalyst (14, 0)]
		[Export ("descriptor")]
		[NullAllowed]
		CIBarcodeDescriptor Descriptor { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "AVMIDIPlayer")]
	interface AVMidiPlayer {

		[Export ("initWithContentsOfURL:soundBankURL:error:")]
		NativeHandle Constructor (NSUrl contentsUrl, [NullAllowed] NSUrl soundBankUrl, out NSError outError);

		[Export ("initWithData:soundBankURL:error:")]
		NativeHandle Constructor (NSData data, [NullAllowed] NSUrl sounddBankUrl, out NSError outError);

		[Export ("duration")]
		double Duration { get; }

		[Export ("playing")]
		bool Playing { [Bind ("isPlaying")] get; }

		[Export ("rate")]
		float Rate { get; set; }  /* float, not CGFloat */

		[Export ("currentPosition")]
		double CurrentPosition { get; set; }

		[Export ("prepareToPlay")]
		void PrepareToPlay ();

		[Export ("play:")]
		[Async]
		void Play ([NullAllowed] Action completionHandler);

		[Export ("stop")]
		void Stop ();
	}

	[Watch (6, 0), NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (AVAsset))]
	interface AVMovie : NSCopying, NSMutableCopying {
		[Field ("AVMovieReferenceRestrictionsKey")]
		NSString ReferenceRestrictionsKey { get; }

		[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("AVMovieShouldSupportAliasDataReferencesKey")]
		NSString ShouldSupportAliasDataReferencesKey { get; }

		[Static]
		[Export ("movieTypes")]
		string [] MovieTypes { get; }

		[Static]
		[Export ("movieWithURL:options:")]
		AVMovie FromUrl (NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("initWithURL:options:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("movieWithData:options:")]
		AVMovie FromData (NSData data, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[MacCatalyst (13, 1)]
		[Export ("initWithData:options:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSData data, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[NullAllowed, Export ("URL")]
		NSUrl URL { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("data")]
		NSData Data { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("defaultMediaDataStorage")]
		AVMediaDataStorage DefaultMediaDataStorage { get; }

		[Export ("tracks")]
		AVMovieTrack [] Tracks { get; }

		[Export ("canContainMovieFragments")]
		bool CanContainMovieFragments { get; }

		[MacCatalyst (13, 1)]
		[Export ("containsMovieFragments")]
		bool ContainsMovieFragments { get; }
	}

	[Watch (6, 0), iOS (13, 0), NoTV]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVMovie))]
	interface AVMovie_AVMovieMovieHeaderSupport {
		[MacCatalyst (13, 1)]
		[Export ("movieHeaderWithFileType:error:")]
		[return: NullAllowed]
		NSData GetMovieHeader (string fileType, [NullAllowed] out NSError outError);

		[MacCatalyst (13, 1)]
		[Export ("writeMovieHeaderToURL:fileType:options:error:")]
		bool WriteMovieHeader (NSUrl URL, string fileType, AVMovieWritingOptions options, [NullAllowed] out NSError outError);

		[MacCatalyst (13, 1)]
		[Export ("isCompatibleWithFileType:")]
		bool IsCompatibleWithFileType (string fileType);
	}

	[Watch (6, 0), iOS (13, 0), NoTV]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVMovie))]
	interface AVMovie_AVMovieTrackInspection {
		[Export ("trackWithTrackID:")]
		[return: NullAllowed]
		AVMovieTrack GetTrack (int trackID);

		[Export ("tracksWithMediaType:")]
		AVMovieTrack [] GetTracks (string mediaType);

		[Wrap ("This.GetTracks (mediaType.GetConstant ())")]
		AVMovieTrack [] GetTracks (AVMediaTypes mediaType);

		[Export ("tracksWithMediaCharacteristic:")]
		AVMovieTrack [] GetTracksWithMediaCharacteristic (string mediaCharacteristic);

		[Wrap ("This.GetTracksWithMediaCharacteristic (mediaCharacteristic.GetConstant ())")]
		AVMovieTrack [] GetTracks (AVMediaCharacteristics mediaCharacteristic);

		[Async]
		[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTrackWithTrackID:completionHandler:")]
		void LoadTrack (int trackId, Action<AVMutableCompositionTrack, NSError> completionHandler);

		[Async]
		[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaType:completionHandler:")]
		void LoadTracksWithMediaType (string mediaType, Action<NSArray<AVMutableCompositionTrack>, NSError> completionHandler);

		[Async]
		[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaCharacteristic:completionHandler:")]
		void LoadTracksWithMediaCharacteristic (string mediaCharacteristic, Action<NSArray<AVMutableCompositionTrack>, NSError> completionHandler);
	}

	[Watch (6, 0), NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVMovie))]
	interface AVMutableMovie {
		[Static]
		[Export ("movieWithURL:options:error:")]
		[return: NullAllowed]
		AVMutableMovie FromUrl (NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError outError);

		[Export ("initWithURL:options:error:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError outError);

		[Static]
		[Export ("movieWithData:options:error:")]
		[return: NullAllowed]
		AVMutableMovie FromData (NSData data, [NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError outError);

		[Export ("initWithData:options:error:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSData data, [NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError outError);

		[Static]
		[Export ("movieWithSettingsFromMovie:options:error:")]
		[return: NullAllowed]
		AVMutableMovie FromMovie ([NullAllowed] AVMovie movie, [NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError outError);

		[Export ("initWithSettingsFromMovie:options:error:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] AVMovie movie, [NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError outError);

		[Export ("preferredRate")]
		float PreferredRate { get; set; }

		[Export ("preferredVolume")]
		float PreferredVolume { get; set; }

		[Export ("preferredTransform", ArgumentSemantic.Assign)]
		CGAffineTransform PreferredTransform { get; set; }

		[Export ("timescale")]
		int Timescale { get; set; }

		[Export ("tracks")]
		AVMutableMovieTrack [] Tracks { get; }

		// AVMutableMovie_AVMutableMovieMetadataEditing
		[Export ("metadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] Metadata { get; set; }

		// AVMutableMovie_AVMutableMovieMovieLevelEditing
		[Export ("modified")]
		bool Modified { [Bind ("isModified")] get; set; }

		[NullAllowed, Export ("defaultMediaDataStorage", ArgumentSemantic.Copy)]
		AVMediaDataStorage DefaultMediaDataStorage { get; set; }

		[Export ("interleavingPeriod", ArgumentSemantic.Assign)]
		CMTime InterleavingPeriod { get; set; }

		[Async]
		[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTrackWithTrackID:completionHandler:")]
		void LoadTrack (int trackId, Action<AVMovieTrack, NSError> completionHandler);

		[Async]
		[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaType:completionHandler:")]
		void LoadTracksWithMediaType (string mediaType, Action<NSArray<AVMovieTrack>, NSError> completionHandler);

		[Async]
		[Watch (8, 0), NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaCharacteristic:completionHandler:")]
		void LoadTracksWithMediaCharacteristic (string mediaCharacteristic, Action<NSArray<AVMovieTrack>, NSError> completionHandler);
	}

	[Watch (6, 0), NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVMutableMovie))]
	interface AVMutableMovie_AVMutableMovieMovieLevelEditing {
		[Export ("insertTimeRange:ofAsset:atTime:copySampleData:error:")]
		bool InsertTimeRange (CMTimeRange timeRange, AVAsset asset, CMTime startTime, bool copySampleData, [NullAllowed] out NSError outError);

		[Export ("insertEmptyTimeRange:")]
		void InsertEmptyTimeRange (CMTimeRange timeRange);

		[Export ("removeTimeRange:")]
		void RemoveTimeRange (CMTimeRange timeRange);

		[Export ("scaleTimeRange:toDuration:")]
		void ScaleTimeRange (CMTimeRange timeRange, CMTime duration);
	}

	[Watch (6, 0), NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVMutableMovie))]
	interface AVMutableMovie_AVMutableMovieTrackLevelEditing {
		[Export ("mutableTrackCompatibleWithTrack:")]
		[return: NullAllowed]
		AVMutableMovieTrack GetMutableTrack (AVAssetTrack track);

		[Export ("addMutableTrackWithMediaType:copySettingsFromTrack:options:")]
		[return: NullAllowed]
		AVMutableMovieTrack AddMutableTrack (string mediaType, [NullAllowed] AVAssetTrack track, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("addMutableTracksCopyingSettingsFromTracks:options:")]
		AVMutableMovieTrack [] AddMutableTracks (AVAssetTrack [] existingTracks, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("removeTrack:")]
		void RemoveTrack (AVMovieTrack track);
	}

	[Watch (6, 0), NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVMutableMovie))]
	interface AVMutableMovie_AVMutableMovieTrackInspection {
		[Export ("trackWithTrackID:")]
		[return: NullAllowed]
		AVMutableMovieTrack GetTrack (int trackID);

		[Export ("tracksWithMediaType:")]
		AVMutableMovieTrack [] GetTracks (string mediaType);

		[Wrap ("This.GetTracks (mediaType.GetConstant ())")]
		AVMutableMovieTrack [] GetTracks (AVMediaTypes mediaType);

		[Export ("tracksWithMediaCharacteristic:")]
		AVMutableMovieTrack [] GetTracksWithMediaCharacteristic (string mediaCharacteristic);

		[Wrap ("This.GetTracksWithMediaCharacteristic (mediaCharacteristic.GetConstant ())")]
		AVMutableMovieTrack [] GetTracks (AVMediaCharacteristics mediaCharacteristic);
	}

	[Watch (6, 0), iOS (13, 0), NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVMediaDataStorage {
		[Export ("initWithURL:options:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[NullAllowed, Export ("URL")]
		NSUrl URL { get; }
	}

	[Watch (6, 0), iOS (13, 0), NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (AVMovie))]
	interface AVFragmentedMovie : AVFragmentMinding {
		[Export ("initWithURL:options:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[MacCatalyst (13, 1)]
		[Export ("initWithData:options:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSData data, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("tracks")]
		AVFragmentedMovieTrack [] Tracks { get; }

		[Notification]
		[Field ("AVFragmentedMovieContainsMovieFragmentsDidChangeNotification")]
		NSString ContainsMovieFragmentsDidChangeNotification { get; }

		[Notification]
		[Field ("AVFragmentedMovieDurationDidChangeNotification")]
		NSString DurationDidChangeNotification { get; }

		[Notification]
		[Field ("AVFragmentedMovieWasDefragmentedNotification")]
		NSString WasDefragmentedNotification { get; }
	}

	[Watch (6, 0), NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVFragmentedMovie))]
	interface AVFragmentedMovie_AVFragmentedMovieTrackInspection {
		[Export ("trackWithTrackID:")]
		[return: NullAllowed]
		AVFragmentedMovieTrack GetTrack (int trackID);

		[Export ("tracksWithMediaType:")]
		AVFragmentedMovieTrack [] GetTracks (string mediaType);

		[Wrap ("This.GetTracks (mediaType.GetConstant ())")]
		AVFragmentedMovieTrack [] GetTracks (AVMediaTypes mediaType);

		[Export ("tracksWithMediaCharacteristic:")]
		AVFragmentedMovieTrack [] GetTracksWithMediaCharacteristic (string mediaCharacteristic);

		[Wrap ("This.GetTracksWithMediaCharacteristic (mediaCharacteristic.GetConstant ())")]
		AVFragmentedMovieTrack [] GetTracks (AVMediaCharacteristics mediaCharacteristic);

		[Async]
		[Watch (8, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTrackWithTrackID:completionHandler:")]
		void LoadTrack (int trackId, Action<AVMutableCompositionTrack, NSError> completionHandler);

		[Async]
		[Watch (8, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaType:completionHandler:")]
		void LoadTracksWithMediaType (string mediaType, Action<NSArray<AVMutableCompositionTrack>, NSError> completionHandler);

		[Async]
		[Watch (8, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaCharacteristic:completionHandler:")]
		void LoadTracksWithMediaCharacteristic (string mediaCharacteristic, Action<NSArray<AVMutableCompositionTrack>, NSError> completionHandler);
	}

	[Watch (6, 0), iOS (13, 0), NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVFragmentedAssetMinder))]
	interface AVFragmentedMovieMinder {
		[Static]
		[Export ("fragmentedMovieMinderWithMovie:mindingInterval:")]
		AVFragmentedMovieMinder FromMovie (AVFragmentedMovie movie, double mindingInterval);

		[Export ("initWithMovie:mindingInterval:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVFragmentedMovie movie, double mindingInterval);

		[Export ("mindingInterval")]
		double MindingInterval { get; set; }

		[Export ("movies")]
		AVFragmentedMovie [] Movies { get; }

		[Export ("addFragmentedMovie:")]
		void Add (AVFragmentedMovie movie);

		[Export ("removeFragmentedMovie:")]
		void Remove (AVFragmentedMovie movie);
	}

	[Watch (6, 0), iOS (13, 0), NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAssetTrack))]
	[DisableDefaultCtor]
	interface AVMovieTrack {
		[MacCatalyst (13, 1)]
		[Export ("mediaPresentationTimeRange")]
		CMTimeRange MediaPresentationTimeRange { get; }

		[MacCatalyst (13, 1)]
		[Export ("mediaDecodeTimeRange")]
		CMTimeRange MediaDecodeTimeRange { get; }

		[MacCatalyst (13, 1)]
		[Export ("alternateGroupID")]
		nint AlternateGroupID { get; }

		// AVMovieTrack_AVMovieTrackMediaDataStorage
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("mediaDataStorage", ArgumentSemantic.Copy)]
		AVMediaDataStorage MediaDataStorage { get; }
	}

	[Watch (6, 0), iOS (13, 0), NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVMovieTrack))]
	[DisableDefaultCtor]
	interface AVMutableMovieTrack {
		[NullAllowed, Export ("mediaDataStorage", ArgumentSemantic.Copy)]
		AVMediaDataStorage MediaDataStorage { get; set; }

		[NullAllowed, Export ("sampleReferenceBaseURL", ArgumentSemantic.Copy)]
		NSUrl SampleReferenceBaseURL { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("alternateGroupID")]
		nint AlternateGroupID { get; set; }

		[Export ("modified")]
		bool Modified { [Bind ("isModified")] get; set; }

		[Export ("hasProtectedContent")]
		bool HasProtectedContent { get; }

		[Export ("timescale")]
		int Timescale { get; set; }

		// AVMutableMovieTrack_AVMutableMovieTrack_LanguageProperties
		[NullAllowed, Export ("languageCode")]
		string LanguageCode { get; set; }

		[NullAllowed, Export ("extendedLanguageTag")]
		string ExtendedLanguageTag { get; set; }

		// AVMutableMovieTrack_AVMutableMovieTrack_PropertiesForVisualCharacteristic
		[Export ("naturalSize", ArgumentSemantic.Assign)]
		CGSize NaturalSize { get; set; }

		[Export ("preferredTransform", ArgumentSemantic.Assign)]
		CGAffineTransform PreferredTransform { get; set; }

		[Export ("layer")]
		nint Layer { get; set; }

		[Export ("cleanApertureDimensions", ArgumentSemantic.Assign)]
		CGSize CleanApertureDimensions { get; set; }

		[Export ("productionApertureDimensions", ArgumentSemantic.Assign)]
		CGSize ProductionApertureDimensions { get; set; }

		[Export ("encodedPixelsDimensions", ArgumentSemantic.Assign)]
		CGSize EncodedPixelsDimensions { get; set; }

		// AVMutableMovieTrack_AVMutableMovieTrack_PropertiesForAudibleCharacteristic
		[Export ("preferredVolume")]
		float PreferredVolume { get; set; }

		// AVMutableMovieTrack_AVMutableMovieTrack_ChunkProperties
		[Export ("preferredMediaChunkSize")]
		nint PreferredMediaChunkSize { get; set; }

		[Export ("preferredMediaChunkDuration", ArgumentSemantic.Assign)]
		CMTime PreferredMediaChunkDuration { get; set; }

		[Export ("preferredMediaChunkAlignment")]
		nint PreferredMediaChunkAlignment { get; set; }

		// AVMutableMovieTrack_AVMutableMovieTrackMetadataEditing
		[Export ("metadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] Metadata { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("appendSampleBuffer:decodeTime:presentationTime:error:")]
		bool AppendSampleBuffer (CMSampleBuffer sampleBuffer, out CMTime outDecodeTime, out CMTime presentationTime, out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("insertMediaTimeRange:intoTimeRange:")]
		bool InsertMediaTimeRange (CMTimeRange mediaTimeRange, CMTimeRange trackTimeRange);

		[MacCatalyst (13, 1)]
		[Export ("replaceFormatDescription:withFormatDescription:")]
		void ReplaceFormatDescription (CMFormatDescription formatDescription, CMFormatDescription newFormatDescription);
	}

	[Watch (6, 0), NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVMutableMovieTrack))]
	interface AVMutableMovieTrack_AVMutableMovieTrack_TrackLevelEditing {
		[Export ("insertTimeRange:ofTrack:atTime:copySampleData:error:")]
		bool InsertTimeRange (CMTimeRange timeRange, AVAssetTrack track, CMTime startTime, bool copySampleData, [NullAllowed] out NSError outError);

		[Export ("insertEmptyTimeRange:")]
		void InsertEmptyTimeRange (CMTimeRange timeRange);

		[Export ("removeTimeRange:")]
		void RemoveTimeRange (CMTimeRange timeRange);

		[Export ("scaleTimeRange:toDuration:")]
		void ScaleTimeRange (CMTimeRange timeRange, CMTime duration);
	}

	[Watch (6, 0), iOS (13, 0), NoTV]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVMutableMovieTrack))]
	interface AVMutableMovieTrack_AVMutableMovieTrackTrackAssociations {
		[Export ("addTrackAssociationToTrack:type:")]
		void AddTrackAssociation (AVMovieTrack movieTrack, string trackAssociationType);

		[Export ("removeTrackAssociationToTrack:type:")]
		void RemoveTrackAssociation (AVMovieTrack movieTrack, string trackAssociationType);
	}

	[NoTV, Watch (6, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVMovieTrack))]
	[DisableDefaultCtor]
	interface AVFragmentedMovieTrack {
#if !NET
		[NoiOS, NoWatch]
		[Field ("AVFragmentedMovieTrackTimeRangeDidChangeNotification")]
		NSString ATimeRangeDidChangeNotification { get; }
#endif

		[Field ("AVFragmentedMovieTrackTimeRangeDidChangeNotification")]
		[Notification]
		NSString TimeRangeDidChangeNotification { get; }

		[Notification]
		[Field ("AVFragmentedMovieTrackSegmentsDidChangeNotification")]
		NSString SegmentsDidChangeNotification { get; }

		[NoiOS, NoWatch]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use either 'AVFragmentedMovieTrackTimeRangeDidChangeNotification' or 'AVFragmentedMovieTrackSegmentsDidChangeNotification' instead. In either case, you can assume that the sender's 'TotalSampleDataLength' has changed.")]
		[NoMacCatalyst]
		[Field ("AVFragmentedMovieTrackTotalSampleDataLengthDidChangeNotification")]
		NSString TotalSampleDataLengthDidChangeNotification { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVMetadataItem))]
	interface AVMutableMetadataItem {
		[NullAllowed] // by default this property is null
		[Export ("keySpace", ArgumentSemantic.Copy)]
		[Override]
		string KeySpace { get; set; }

		[Export ("metadataItem"), Static]
		AVMutableMetadataItem Create ();

		[Export ("duration")]
		[Override]
		CMTime Duration { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("identifier", ArgumentSemantic.Copy)]
		[Override]
		NSString MetadataIdentifier { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("locale", ArgumentSemantic.Copy)]
		[Override]
		NSLocale Locale { get; set; }

		[Export ("time")]
		[Override]
		CMTime Time { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("value", ArgumentSemantic.Copy)]
		[Override]
		NSObject Value { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("extraAttributes", ArgumentSemantic.Copy)]
		[Override]
		NSDictionary ExtraAttributes { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("key", ArgumentSemantic.Copy)]
		NSObject Key { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("dataType", ArgumentSemantic.Copy)]
		[Override]
		NSString DataType { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("extendedLanguageTag")]
		[Override]
		string ExtendedLanguageTag { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("startDate"), NullAllowed]
		[Override]
		NSDate StartDate { get; set; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAssetTrack))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVCompositionTrack {
		[Export ("segments", ArgumentSemantic.Copy)]
		AVCompositionTrackSegment [] Segments { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("formatDescriptionReplacements")]
		AVCompositionTrackFormatDescriptionReplacement [] FormatDescriptionReplacements { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVCompositionTrack))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVMutableCompositionTrack {
		[Export ("segments", ArgumentSemantic.Copy), NullAllowed]
		[New]
		AVCompositionTrackSegment [] Segments { get; set; }

		[Export ("insertTimeRange:ofTrack:atTime:error:")]
		bool InsertTimeRange (CMTimeRange timeRange, AVAssetTrack ofTrack, CMTime atTime, out NSError error);

		[Export ("insertEmptyTimeRange:")]
		void InsertEmptyTimeRange (CMTimeRange timeRange);

		[Export ("removeTimeRange:")]
		void RemoveTimeRange (CMTimeRange timeRange);

		[Export ("scaleTimeRange:toDuration:")]
		void ScaleTimeRange (CMTimeRange timeRange, CMTime duration);

		[Export ("validateTrackSegments:error:")]
		bool ValidateTrackSegments (AVCompositionTrackSegment [] trackSegments, out NSError error);

		[Export ("extendedLanguageTag", ArgumentSemantic.Copy), NullAllowed]
		[New]
		string ExtendedLanguageTag { get; set; }

		[Export ("languageCode", ArgumentSemantic.Copy), NullAllowed]
		[New]
		string LanguageCode { get; set; }

		[Export ("naturalTimeScale")]
		[New]
		int /* CMTimeScale = int32_t */ NaturalTimeScale { get; set; }

		[Export ("preferredTransform")]
		[New]
		CGAffineTransform PreferredTransform { get; set; }

		[Export ("preferredVolume")]
		[New]
		float PreferredVolume { get; set; } // defined as 'float'

		// 5.0
		[Export ("insertTimeRanges:ofTracks:atTime:error:")]
		bool InsertTimeRanges (NSValue [] cmTimeRanges, AVAssetTrack [] tracks, CMTime startTime, out NSError error);

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("addTrackAssociationToTrack:type:")]
		void AddTrackAssociation (AVCompositionTrack compositionTrack, string trackAssociationType);

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("removeTrackAssociationToTrack:type:")]
		void RemoveTrackAssociation (AVCompositionTrack compositionTrack, string trackAssociationType);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("replaceFormatDescription:withFormatDescription:")]
		void ReplaceFormatDescription (CMFormatDescription originalFormatDescription, [NullAllowed] CMFormatDescription replacementFormatDescription);

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Static]
	interface AVErrorKeys {
		[Field ("AVFoundationErrorDomain")]
		NSString ErrorDomain { get; }

		[Field ("AVErrorDeviceKey")]
		NSString Device { get; }

		[Field ("AVErrorTimeKey")]
		NSString Time { get; }

		[Field ("AVErrorFileSizeKey")]
		NSString FileSize { get; }

		[Field ("AVErrorPIDKey")]
		NSString Pid { get; }

		[Field ("AVErrorRecordingSuccessfullyFinishedKey")]
		NSString RecordingSuccessfullyFinished { get; }

		[Field ("AVErrorMediaTypeKey")]
		NSString MediaType { get; }

		[Field ("AVErrorMediaSubTypeKey")]
		NSString MediaSubType { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVErrorPresentationTimeStampKey")]
		NSString PresentationTimeStamp { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVErrorPersistentTrackIDKey")]
		NSString PersistentTrackID { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVErrorFileTypeKey")]
		NSString FileType { get; }

		[NoiOS, NoWatch]
		[NoTV]
		[NoMacCatalyst]
		[Field ("AVErrorDiscontinuityFlagsKey")]
		NSString DiscontinuityFlags { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAssetTrackSegment {
		[Export ("empty")]
		bool Empty { [Bind ("isEmpty")] get; }

		[Export ("timeMapping")]
		CMTimeMapping TimeMapping { get; }

	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAsset))]
	interface AVComposition : NSMutableCopying {
		[Export ("tracks")]
		[New]
		AVCompositionTrack [] Tracks { get; }

		[Export ("naturalSize")]
		[New]
		CGSize NaturalSize { get; [NotImplemented] set; }

		[MacCatalyst (13, 1)]
		[Export ("URLAssetInitializationOptions", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> UrlAssetInitializationOptions { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVComposition))]
	interface AVComposition_AVCompositionTrackInspection {

		[Export ("trackWithTrackID:")]
		[return: NullAllowed]
		AVCompositionTrack GetTrack (int trackID);

		[Export ("tracksWithMediaType:")]
		AVCompositionTrack [] GetTracks (string mediaType);

		[Wrap ("This.GetTracks (mediaType.GetConstant ())")]
		AVCompositionTrack [] GetTracks (AVMediaTypes mediaType);

		[Export ("tracksWithMediaCharacteristic:")]
		AVCompositionTrack [] GetTracksWithMediaCharacteristic (string mediaCharacteristic);

		[Wrap ("This.GetTracksWithMediaCharacteristic (mediaCharacteristic.GetConstant ())")]
		AVCompositionTrack [] GetTracks (AVMediaCharacteristics mediaCharacteristic);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTrackWithTrackID:completionHandler:")]
		void LoadTrack (int trackId, Action<AVMutableCompositionTrack, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaType:completionHandler:")]
		void LoadTracksWithMediaType (string mediaType, Action<NSArray<AVMutableCompositionTrack>, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaCharacteristic:completionHandler:")]
		void LoadTracksWithMediaCharacteristic (string mediaCharacteristic, Action<NSArray<AVMutableCompositionTrack>, NSError> completionHandler);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVComposition))]
	interface AVMutableComposition {

		[Export ("composition"), Static]
		AVMutableComposition Create ();

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("compositionWithURLAssetInitializationOptions:")]
		AVMutableComposition FromOptions ([NullAllowed] NSDictionary<NSString, NSObject> urlAssetInitializationOptions);

		[Export ("insertTimeRange:ofAsset:atTime:error:")]
		bool Insert (CMTimeRange insertTimeRange, AVAsset sourceAsset, CMTime atTime, out NSError error);

		[Export ("insertEmptyTimeRange:")]
		void InserEmptyTimeRange (CMTimeRange timeRange);

		[Export ("removeTimeRange:")]
		void RemoveTimeRange (CMTimeRange timeRange);

		[Export ("scaleTimeRange:toDuration:")]
		void ScaleTimeRange (CMTimeRange timeRange, CMTime duration);

		[Export ("addMutableTrackWithMediaType:preferredTrackID:")]
		[return: NullAllowed]
		AVMutableCompositionTrack AddMutableTrack (string mediaType, int /* CMPersistentTrackID = int32_t */ preferredTrackId);

		[Export ("removeTrack:")]
		void RemoveTrack (AVCompositionTrack track);

		[Export ("mutableTrackCompatibleWithTrack:")]
		[return: NullAllowed]
		AVMutableCompositionTrack CreateMutableTrack (AVAssetTrack referenceTrack);

		[Export ("naturalSize")]
		[Override]
		CGSize NaturalSize { get; set; }

		// from @interface CNComposition (AVMutableComposition)
		[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
		[Export ("addTracksForCinematicAssetInfo:preferredStartingTrackID:")]
		CNCompositionInfo AddTracks (CNAssetInfo assetInfo, int preferredStartingTrackID);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVMutableComposition))]
	interface AVMutableComposition_AVMutableCompositionTrackInspection {

		[Export ("trackWithTrackID:")]
		[return: NullAllowed]
		AVMutableCompositionTrack GetTrack (int trackID);

		[Export ("tracksWithMediaType:")]
		AVMutableCompositionTrack [] GetTracks (string mediaType);

		[Wrap ("This.GetTracks (mediaType.GetConstant ())")]
		AVMutableCompositionTrack [] GetTracks (AVMediaTypes mediaType);

		[Export ("tracksWithMediaCharacteristic:")]
		AVMutableCompositionTrack [] GetTracksWithMediaCharacteristic (string mediaCharacteristic);

		[Wrap ("This.GetTracksWithMediaCharacteristic (mediaCharacteristic.GetConstant ())")]
		AVMutableCompositionTrack [] GetTracks (AVMediaCharacteristics mediaCharacteristic);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTrackWithTrackID:completionHandler:")]
		void LoadTrack (int trackId, Action<AVMutableCompositionTrack, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaType:completionHandler:")]
		void LoadTracksWithMediaType (string mediaType, Action<NSArray<AVMutableCompositionTrack>, NSError> completionHandler);

		[Async]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("loadTracksWithMediaCharacteristic:completionHandler:")]
		void LoadTracksWithMediaCharacteristic (string mediaCharacteristic, Action<NSArray<AVMutableCompositionTrack>, NSError> completionHandler);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAssetTrackSegment))]
	interface AVCompositionTrackSegment {
		[Export ("sourceURL"), NullAllowed]
		NSUrl SourceUrl { get; }

		[Export ("sourceTrackID")]
		int SourceTrackID { get; } /* CMPersistentTrackID = int32_t */

		[Static]
		[Export ("compositionTrackSegmentWithURL:trackID:sourceTimeRange:targetTimeRange:")]
		IntPtr FromUrl (NSUrl url, int /* CMPersistentTrackID = int32_t */ trackID, CMTimeRange sourceTimeRange, CMTimeRange targetTimeRange);

		[Static]
		[Export ("compositionTrackSegmentWithTimeRange:")]
		IntPtr FromTimeRange (CMTimeRange timeRange);

		[DesignatedInitializer]
		[Export ("initWithURL:trackID:sourceTimeRange:targetTimeRange:")]
		NativeHandle Constructor (NSUrl URL, int trackID /* CMPersistentTrackID = int32_t */, CMTimeRange sourceTimeRange, CMTimeRange targetTimeRange);

		[DesignatedInitializer]
		[Export ("initWithTimeRange:")]
		NativeHandle Constructor (CMTimeRange timeRange);

		[Export ("empty")]
		bool Empty { [Bind ("isEmpty")] get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVAssetExportSession {
		[Export ("presetName")]
		string PresetName { get; }

		[Export ("supportedFileTypes")]
#if NET
		string [] SupportedFileTypes { get;  }
#else
		NSObject [] SupportedFileTypes { get; }
#endif

		[NullAllowed]
		[Export ("outputFileType", ArgumentSemantic.Copy)]
		string OutputFileType { get; set; }

		[NullAllowed]
		[Export ("outputURL", ArgumentSemantic.Copy)]
		NSUrl OutputUrl { get; set; }

		[return: NullAllowed]
		[Static, Export ("exportSessionWithAsset:presetName:")]
		AVAssetExportSession FromAsset (AVAsset asset, string presetName);

		[Export ("status")]
		AVAssetExportSessionStatus Status { get; }

		[Export ("progress")]
		float Progress { get; } // defined as 'float'

		[NoWatch]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'EstimateMaximumDuration' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'EstimateMaximumDuration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'EstimateMaximumDuration' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'EstimateMaximumDuration' instead.")]
		[Export ("maxDuration")]
		CMTime MaxDuration { get; }

		[Export ("timeRange", ArgumentSemantic.Assign)]
		CMTimeRange TimeRange { get; set; }

		[Export ("metadata", ArgumentSemantic.Copy), NullAllowed]
		AVMetadataItem [] Metadata { get; set; }

		[Export ("fileLengthLimit")]
		long FileLengthLimit { get; set; }

		[Export ("audioMix", ArgumentSemantic.Copy), NullAllowed]
		AVAudioMix AudioMix { get; set; }

		[NullAllowed, Export ("videoComposition", ArgumentSemantic.Copy)]
		AVVideoComposition VideoComposition { get; set; }

		[Export ("shouldOptimizeForNetworkUse")]
		bool ShouldOptimizeForNetworkUse { get; set; }

		[Static, Export ("allExportPresets")]
		string [] AllExportPresets { get; }

		[Static]
		[Export ("exportPresetsCompatibleWithAsset:")]
		string [] ExportPresetsCompatibleWithAsset (AVAsset asset);

		[DesignatedInitializer]
		[Export ("initWithAsset:presetName:")]
		NativeHandle Constructor (AVAsset asset, string presetName);

		[Wrap ("this (asset, preset.GetConstant ())")]
		NativeHandle Constructor (AVAsset asset, AVAssetExportSessionPreset preset);

		[Export ("exportAsynchronouslyWithCompletionHandler:")]
		[Async ("ExportTaskAsync")]
		void ExportAsynchronously (Action handler);

		[Export ("cancelExport")]
		void CancelExport ();

		[Export ("error"), NullAllowed]
		NSError Error { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetLowQuality")]
		NSString PresetLowQuality { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetMediumQuality")]
		NSString PresetMediumQuality { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHighestQuality")]
		NSString PresetHighestQuality { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHEVCHighestQuality")]
		NSString PresetHevcHighestQuality { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHEVC3840x2160")]
		NSString PresetHevc3840x2160 { get; }

		[Field ("AVAssetExportPreset640x480")]
		NSString Preset640x480 { get; }

		[Field ("AVAssetExportPreset960x540")]
		NSString Preset960x540 { get; }

		[Field ("AVAssetExportPreset1280x720")]
		NSString Preset1280x720 { get; }

		[Field ("AVAssetExportPreset1920x1080")]
		NSString Preset1920x1080 { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPreset3840x2160")]
		NSString Preset3840x2160 { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHEVC1920x1080")]
		NSString PresetHevc1920x1080 { get; }

		[Field ("AVAssetExportPresetAppleM4A")]
		NSString PresetAppleM4A { get; }

		[Field ("AVAssetExportPresetPassthrough")]
		NSString PresetPassthrough { get; }

		[NoWatch, NoTV, MacCatalyst (15, 0), iOS (15, 0)]
		[Field ("AVAssetExportPresetAppleProRes4444LPCM")]
		NSString PresetAppleProRes4444Lpcm { get; }

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHEVC1920x1080WithAlpha")]
		NSString PresetHevc1920x1080WithAlpha { get; }

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHEVC3840x2160WithAlpha")]
		NSString PresetHevc3840x2160WithAlpha { get; }

		[NoWatch, NoTV, NoiOS, Mac (12, 1)]
		[NoMacCatalyst]
		[Field ("AVAssetExportPresetHEVC7680x4320")]
		NSString PresetHevc7680x4320 { get; }

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVAssetExportPresetHEVCHighestQualityWithAlpha")]
		NSString PresetHevcHighestQualityWithAlpha { get; }

		// 5.0 APIs
		[Export ("asset", ArgumentSemantic.Retain)]
		AVAsset Asset { get; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'EstimateOutputFileLength' for more precise results.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'EstimateOutputFileLength' for more precise results.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'EstimateOutputFileLength' for more precise results.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'EstimateOutputFileLength' for more precise results.")]
		[Export ("estimatedOutputFileLength")]
		long EstimatedOutputFileLength { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("determineCompatibilityOfExportPreset:withAsset:outputFileType:completionHandler:")]
		[Async]
		void DetermineCompatibilityOfExportPreset (string presetName, AVAsset asset, [NullAllowed] string outputFileType, Action<bool> isCompatibleResult);

		[Async]
		[Wrap ("DetermineCompatibilityOfExportPreset (presetName, asset, outputFileType.GetConstant (), isCompatibleResult)")]
		void DetermineCompatibilityOfExportPreset (string presetName, AVAsset asset, [NullAllowed] AVFileTypes outputFileType, Action<bool> isCompatibleResult);

		[MacCatalyst (13, 1)]
		[Export ("determineCompatibleFileTypesWithCompletionHandler:")]
		[Async]
		void DetermineCompatibleFileTypes (Action<string []> compatibleFileTypesHandler);

		[MacCatalyst (13, 1)]
		[Export ("metadataItemFilter", ArgumentSemantic.Retain), NullAllowed]
		AVMetadataItemFilter MetadataItemFilter { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("customVideoCompositor", ArgumentSemantic.Copy)]
		[Protocolize]
		AVVideoCompositing CustomVideoCompositor { get; }

		// DOC: Use the values from AVAudioTimePitchAlgorithm class.
		[MacCatalyst (13, 1)]
		[Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		NSString AudioTimePitchAlgorithm { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("canPerformMultiplePassesOverSourceMediaData")]
		[Advice ("This property cannot be set after the export has started.")]
		bool CanPerformMultiplePassesOverSourceMediaData { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("directoryForTemporaryFiles", ArgumentSemantic.Copy), NullAllowed]
		[Advice ("This property cannot be set after the export has started.")]
		NSUrl DirectoryForTemporaryFiles { get; set; }

		[Async]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("estimateMaximumDurationWithCompletionHandler:")]
		void EstimateMaximumDuration (Action<CMTime, NSError> handler);

		[Async]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("estimateOutputFileLengthWithCompletionHandler:")]
		void EstimateOutputFileLength (Action<long, NSError> handler);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Static]
	interface AVAudioTimePitchAlgorithm {
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioTimePitchAlgorithmLowQualityZeroLatency")]
		NSString LowQualityZeroLatency { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAudioTimePitchAlgorithmTimeDomain")]
		NSString TimeDomain { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAudioTimePitchAlgorithmSpectral")]
		NSString Spectral { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAudioTimePitchAlgorithmVarispeed")]
		NSString Varispeed { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAudioMix : NSMutableCopying {
		[Export ("inputParameters", ArgumentSemantic.Copy)]
		AVAudioMixInputParameters [] InputParameters { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioMix))]
	interface AVMutableAudioMix {
		[Export ("inputParameters", ArgumentSemantic.Copy)]
		AVAudioMixInputParameters [] InputParameters { get; set; }

		[Static, Export ("audioMix")]
		AVMutableAudioMix Create ();
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAudioMixInputParameters : NSMutableCopying {
		[Export ("trackID")]
		int TrackID { get; } // defined as 'CMPersistentTrackID' = int32_t

		[Export ("getVolumeRampForTime:startVolume:endVolume:timeRange:")]
		bool GetVolumeRamp (CMTime forTime, ref float /* defined as 'float*' */ startVolume, ref float /* defined as 'float*' */ endVolume, ref CMTimeRange timeRange);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("audioTapProcessor", ArgumentSemantic.Retain)]
		MTAudioProcessingTap AudioTapProcessor { get; [NotImplemented] set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		NSString AudioTimePitchAlgorithm { get; [NotImplemented] set; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioMixInputParameters))]
	interface AVMutableAudioMixInputParameters {
		[Export ("trackID")]
		int TrackID { get; set; } // defined as 'CMPersistentTrackID'

		[Static]
		[Export ("audioMixInputParametersWithTrack:")]
		AVMutableAudioMixInputParameters FromTrack ([NullAllowed] AVAssetTrack track);

		[Static]
		[Export ("audioMixInputParameters")]
		AVMutableAudioMixInputParameters Create ();

		[Export ("setVolumeRampFromStartVolume:toEndVolume:timeRange:")]
		void SetVolumeRamp (float /* defined as 'float' */ startVolume, float /* defined as 'float' */ endVolume, CMTimeRange timeRange);

		[Export ("setVolume:atTime:")]
		void SetVolume (float /* defined as 'float' */ volume, CMTime atTime);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("audioTapProcessor", ArgumentSemantic.Retain)]
		[Override]
		MTAudioProcessingTap AudioTapProcessor { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		[Override]
		NSString AudioTimePitchAlgorithm { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	interface AVVideoCompositing {
		[Abstract]
		[return: NullAllowed]
		[Export ("sourcePixelBufferAttributes")]
		NSDictionary SourcePixelBufferAttributes ();

		[Abstract]
		[Export ("requiredPixelBufferAttributesForRenderContext")]
		NSDictionary RequiredPixelBufferAttributesForRenderContext ();

		[Abstract]
		[Export ("renderContextChanged:")]
		void RenderContextChanged (AVVideoCompositionRenderContext newRenderContext);

		[Abstract]
		[Export ("startVideoCompositionRequest:")]
		void StartVideoCompositionRequest (AVAsynchronousVideoCompositionRequest asyncVideoCompositionRequest);

		[Export ("cancelAllPendingVideoCompositionRequests")]
		void CancelAllPendingVideoCompositionRequests ();

		[MacCatalyst (13, 1)]
		[Export ("supportsWideColorSourceFrames")]
		bool SupportsWideColorSourceFrames { get; }

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("anticipateRenderingUsingHint:")]
		void AnticipateRendering (AVVideoCompositionRenderHint renderHint);

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("prerollForRenderingUsingHint:")]
		void PrerollForRendering (AVVideoCompositionRenderHint renderHint);

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("supportsHDRSourceFrames")]
		bool SupportsHdrSourceFrames { get; }

		[TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("canConformColorOfSourceFrames")]
		bool CanConformColorOfSourceFrames { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVVideoComposition : NSMutableCopying {
		[Export ("frameDuration")]
		CMTime FrameDuration { get; }

		[Export ("renderSize")]
		CGSize RenderSize { get; }

		[Export ("instructions", ArgumentSemantic.Copy)]
		AVVideoCompositionInstruction [] Instructions { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("animationTool", ArgumentSemantic.Retain), NullAllowed]
		AVVideoCompositionCoreAnimationTool AnimationTool { get; }

		[MacCatalyst (13, 1)]
		[Export ("renderScale")]
		float RenderScale { get; [NotImplemented] set; } // defined as 'float'

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("isValidForAsset:timeRange:validationDelegate:")]
		bool IsValidForAsset ([NullAllowed] AVAsset asset, CMTimeRange timeRange, [Protocolize][NullAllowed] AVVideoCompositionValidationHandling validationDelegate);

		[MacCatalyst (13, 1)]
		[Static, Export ("videoCompositionWithPropertiesOfAsset:")]
		AVVideoComposition FromAssetProperties (AVAsset asset);

		[MacCatalyst (13, 1)]
		[Export ("customVideoCompositorClass", ArgumentSemantic.Copy), NullAllowed]
		Class CustomVideoCompositorClass { get; [NotImplemented] set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("videoCompositionWithAsset:applyingCIFiltersWithHandler:")]
		AVVideoComposition CreateVideoComposition (AVAsset asset, Action<AVAsynchronousCIImageFilteringRequest> applier);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("colorPrimaries")]
		string ColorPrimaries { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("colorYCbCrMatrix")]
		string ColorYCbCrMatrix { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("colorTransferFunction")]
		string ColorTransferFunction { get; }

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("sourceSampleDataTrackIDs")]
		[BindAs (typeof (int []))]
		NSNumber [] SourceSampleDataTrackIds { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVVideoCompositionRenderContext {
		[Export ("size", ArgumentSemantic.Copy)]
		CGSize Size { get; }

		[Export ("renderTransform", ArgumentSemantic.Copy)]
		CGAffineTransform RenderTransform { get; }

		[Export ("renderScale")]
		float RenderScale { get; } // defined as 'float'

		[Export ("pixelAspectRatio", ArgumentSemantic.Copy)]
		AVPixelAspectRatio PixelAspectRatio { get; }

		[Export ("edgeWidths", ArgumentSemantic.Copy)]
		AVEdgeWidths EdgeWidths { get; }

		[Export ("highQualityRendering")]
		bool HighQualityRendering { get; }

		[Export ("videoComposition", ArgumentSemantic.Copy)]
		AVVideoComposition VideoComposition { get; }

		[return: NullAllowed, Release]
		[Export ("newPixelBuffer")]
		CVPixelBuffer CreatePixelBuffer ();
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[DisableDefaultCtor]
	interface AVVideoCompositionValidationHandling {
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("videoComposition:shouldContinueValidatingAfterFindingInvalidValueForKey:")]
		bool ShouldContinueValidatingAfterFindingInvalidValueForKey (AVVideoComposition videoComposition, string key);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("videoComposition:shouldContinueValidatingAfterFindingEmptyTimeRange:")]
		bool ShouldContinueValidatingAfterFindingEmptyTimeRange (AVVideoComposition videoComposition, CMTimeRange timeRange);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("videoComposition:shouldContinueValidatingAfterFindingInvalidTimeRangeInInstruction:")]
		bool ShouldContinueValidatingAfterFindingInvalidTimeRangeInInstruction (AVVideoComposition videoComposition, AVVideoCompositionInstruction videoCompositionInstruction);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("videoComposition:shouldContinueValidatingAfterFindingInvalidTrackIDInInstruction:layerInstruction:asset:")]
		bool ShouldContinueValidatingAfterFindingInvalidTrackIDInInstruction (AVVideoComposition videoComposition, AVVideoCompositionInstruction videoCompositionInstruction, AVVideoCompositionLayerInstruction layerInstruction, AVAsset asset);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVVideoComposition))]
	interface AVMutableVideoComposition {
		[Export ("frameDuration", ArgumentSemantic.Assign)]
		CMTime FrameDuration { get; set; }

		[Export ("renderSize", ArgumentSemantic.Assign)]
		CGSize RenderSize { get; set; }

		[Export ("instructions", ArgumentSemantic.Copy)]
		AVVideoCompositionInstruction [] Instructions { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("animationTool", ArgumentSemantic.Retain)]
		AVVideoCompositionCoreAnimationTool AnimationTool { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("renderScale")]
		float RenderScale { get; set; } // defined as 'float'

		[Static, Export ("videoComposition")]
		AVMutableVideoComposition Create ();

		// in 7.0 they declared this was available in 6.0
		[MacCatalyst (13, 1)]
		[Static, Export ("videoCompositionWithPropertiesOfAsset:")]
		AVMutableVideoComposition Create (AVAsset asset);

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("videoCompositionWithPropertiesOfAsset:prototypeInstruction:")]
		AVMutableVideoComposition Create (AVAsset asset, AVVideoCompositionInstruction prototypeInstruction);

		[NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("customVideoCompositorClass", ArgumentSemantic.Retain)]
		[Override]
		Class CustomVideoCompositorClass { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("videoCompositionWithAsset:applyingCIFiltersWithHandler:")]
		AVMutableVideoComposition GetVideoComposition (AVAsset asset, Action<AVAsynchronousCIImageFilteringRequest> applier);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("colorPrimaries")]
		string ColorPrimaries { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("colorYCbCrMatrix")]
		string ColorYCbCrMatrix { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("colorTransferFunction")]
		string ColorTransferFunction { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("sourceTrackIDForFrameTiming")]
		int SourceTrackIdForFrameTiming { get; set; }

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("sourceSampleDataTrackIDs", ArgumentSemantic.Copy)]
		[BindAs (typeof (int []))]
		NSNumber [] SourceSampleDataTrackIds { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVVideoCompositionInstruction : NSSecureCoding, NSMutableCopying {
		[Export ("timeRange")]
		CMTimeRange TimeRange { get; [NotImplemented ("Not available on AVVideoCompositionInstruction, only available on AVMutableVideoCompositionInstruction")] set; }

		[NullAllowed]
		[Export ("backgroundColor", ArgumentSemantic.Retain)]
		CGColor BackgroundColor {
			get;
			[NotImplemented]
			set;
		}

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("layerInstructions", ArgumentSemantic.Copy)]
		AVVideoCompositionLayerInstruction [] LayerInstructions { get; [NotImplemented ("Not available on AVVideoCompositionInstruction, only available on AVMutableVideoCompositionInstruction")] set; }

		[Export ("enablePostProcessing")]
		bool EnablePostProcessing { get; [NotImplemented ("Not available on AVVideoCompositionInstruction, only available on AVMutableVideoCompositionInstruction")] set; }

		// These are there because it adopts the protocol *of the same name*

		[MacCatalyst (13, 1)]
		[Export ("containsTweening")]
		bool ContainsTweening { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("requiredSourceTrackIDs")]
		NSNumber [] RequiredSourceTrackIDs { get; }

		[MacCatalyst (13, 1)]
		[Export ("passthroughTrackID")]
		int PassthroughTrackID { get; } /* CMPersistentTrackID = int32_t */

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[BindAs (typeof (int []))]
		[Export ("requiredSourceSampleDataTrackIDs")] /* CMPersistentTrackID = int32_t */
		NSNumber [] RequiredSourceSampleDataTrackIds { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVVideoCompositionInstruction))]
	interface AVMutableVideoCompositionInstruction {
		[Export ("timeRange", ArgumentSemantic.Assign)]
		[Override]
		CMTimeRange TimeRange { get; set; }

		[NullAllowed]
		[Export ("backgroundColor", ArgumentSemantic.Retain)]
		[Override]
		CGColor BackgroundColor { get; set; }

		[Export ("enablePostProcessing", ArgumentSemantic.Assign)]
		[Override]
		bool EnablePostProcessing { get; set; }

		[Export ("layerInstructions", ArgumentSemantic.Copy)]
		[Override]
		AVVideoCompositionLayerInstruction [] LayerInstructions { get; set; }

		[Static, Export ("videoCompositionInstruction")]
		AVVideoCompositionInstruction Create ();

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[BindAs (typeof (int []))]
		[Export ("requiredSourceSampleDataTrackIDs", ArgumentSemantic.Copy)]
		NSNumber [] RequiredSourceSampleDataTrackIds { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVVideoCompositionLayerInstruction : NSSecureCoding, NSMutableCopying {
		[Export ("trackID", ArgumentSemantic.Assign)]
		int TrackID { get; } // defined as 'CMPersistentTrackID' = int32_t

		[Export ("getTransformRampForTime:startTransform:endTransform:timeRange:")]
		bool GetTransformRamp (CMTime time, ref CGAffineTransform startTransform, ref CGAffineTransform endTransform, ref CMTimeRange timeRange);

		[Export ("getOpacityRampForTime:startOpacity:endOpacity:timeRange:")]
		bool GetOpacityRamp (CMTime time, ref float /* defined as 'float*' */ startOpacity, ref float /* defined as 'float*' */ endOpacity, ref CMTimeRange timeRange);

		[Export ("getCropRectangleRampForTime:startCropRectangle:endCropRectangle:timeRange:")]
		[MacCatalyst (13, 1)]
		bool GetCrop (CMTime time, ref CGRect startCropRectangle, ref CGRect endCropRectangle, ref CMTimeRange timeRange);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVVideoCompositionLayerInstruction))]
	interface AVMutableVideoCompositionLayerInstruction {
		[Export ("trackID", ArgumentSemantic.Assign)]
		int TrackID { get; set; } // defined as 'CMPersistentTrackID' = int32w_t

		[Static]
		[Export ("videoCompositionLayerInstructionWithAssetTrack:")]
		AVMutableVideoCompositionLayerInstruction FromAssetTrack (AVAssetTrack track);

		[Static]
		[Export ("videoCompositionLayerInstruction")]
		AVMutableVideoCompositionLayerInstruction Create ();

		[Export ("setTransformRampFromStartTransform:toEndTransform:timeRange:")]
		void SetTransformRamp (CGAffineTransform startTransform, CGAffineTransform endTransform, CMTimeRange timeRange);

		[Export ("setTransform:atTime:")]
		void SetTransform (CGAffineTransform transform, CMTime atTime);

		[Export ("setOpacityRampFromStartOpacity:toEndOpacity:timeRange:")]
		void SetOpacityRamp (float /* defined as 'float' */ startOpacity, float /* defined as 'float' */ endOpacity, CMTimeRange timeRange);

		[Export ("setOpacity:atTime:")]
		void SetOpacity (float /* defined as 'float' */ opacity, CMTime time);

		[MacCatalyst (13, 1)]
		[Export ("setCropRectangleRampFromStartCropRectangle:toEndCropRectangle:timeRange:")]
		void SetCrop (CGRect startCropRectangle, CGRect endCropRectangle, CMTimeRange timeRange);

		[MacCatalyst (13, 1)]
		[Export ("setCropRectangle:atTime:")]
		void SetCrop (CGRect cropRectangle, CMTime time);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVVideoCompositionCoreAnimationTool {
		[Static]
		[Export ("videoCompositionCoreAnimationToolWithAdditionalLayer:asTrackID:")]
		AVVideoCompositionCoreAnimationTool FromLayer (CALayer layer, int /* CMPersistentTrackID = int32_t */ trackID);

		[Static]
		[Export ("videoCompositionCoreAnimationToolWithPostProcessingAsVideoLayer:inLayer:")]
		AVVideoCompositionCoreAnimationTool FromLayer (CALayer videoLayer, CALayer animationLayer);

		[MacCatalyst (13, 1)]
		[Static, Export ("videoCompositionCoreAnimationToolWithPostProcessingAsVideoLayers:inLayer:")]
		AVVideoCompositionCoreAnimationTool FromComposedVideoFrames (CALayer [] videoLayers, CALayer inAnimationlayer);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCameraCalibrationData {
		[Export ("intrinsicMatrix")]
		NMatrix3 IntrinsicMatrix { [MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")] get; }

		[Export ("intrinsicMatrixReferenceDimensions")]
		CGSize IntrinsicMatrixReferenceDimensions { get; }

		[Export ("extrinsicMatrix")]
		NMatrix4x3 ExtrinsicMatrix { [MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")] get; }

		[Export ("pixelSize")]
		float PixelSize { get; }

		[NullAllowed, Export ("lensDistortionLookupTable")]
		NSData LensDistortionLookupTable { get; }

		[NullAllowed, Export ("inverseLensDistortionLookupTable")]
		NSData InverseLensDistortionLookupTable { get; }

		[Export ("lensDistortionCenter")]
		CGPoint LensDistortionCenter { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	interface AVCaptureSessionRuntimeErrorEventArgs {
		[Export ("AVCaptureSessionErrorKey")]
		NSError Error { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface AVCaptureSession {

		[Export ("sessionPreset", ArgumentSemantic.Copy)]
		NSString SessionPreset { get; set; }

		[Export ("inputs")]
		AVCaptureInput [] Inputs { get; }

		[Export ("outputs")]
		AVCaptureOutput [] Outputs { get; }

		[Export ("running")]
		bool Running { [Bind ("isRunning")] get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("interrupted")]
		bool Interrupted { [Bind ("isInterrupted")] get; }

		[Export ("canSetSessionPreset:")]
		bool CanSetSessionPreset (NSString preset);

		[Export ("canAddInput:")]
		bool CanAddInput (AVCaptureInput input);

		[Export ("addInput:")]
		void AddInput (AVCaptureInput input);

		[Export ("removeInput:")]
		void RemoveInput (AVCaptureInput input);

		[Export ("canAddOutput:")]
		bool CanAddOutput (AVCaptureOutput output);

		[Export ("addOutput:")]
		void AddOutput (AVCaptureOutput output);

		[Export ("removeOutput:")]
		void RemoveOutput (AVCaptureOutput output);

		[Export ("beginConfiguration")]
		void BeginConfiguration ();

		[Export ("commitConfiguration")]
		void CommitConfiguration ();

		[Export ("startRunning")]
		void StartRunning ();

		[Export ("stopRunning")]
		void StopRunning ();

		[Field ("AVCaptureSessionPresetPhoto")]
		NSString PresetPhoto { get; }

		[Field ("AVCaptureSessionPresetHigh")]
		NSString PresetHigh { get; }

		[Field ("AVCaptureSessionPresetMedium")]
		NSString PresetMedium { get; }

		[Field ("AVCaptureSessionPresetLow")]
		NSString PresetLow { get; }

		[Field ("AVCaptureSessionPreset640x480")]
		NSString Preset640x480 { get; }

		[Field ("AVCaptureSessionPreset1280x720")]
		NSString Preset1280x720 { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVCaptureSessionPreset1920x1080")]
		NSString Preset1920x1080 { get; }

		[MacCatalyst (14, 0)]
		[Field ("AVCaptureSessionPreset3840x2160")]
		NSString Preset3840x2160 { get; }

		[MacCatalyst (14, 0)]
		[Field ("AVCaptureSessionPresetiFrame960x540")]
		NSString PresetiFrame960x540 { get; }

		[Field ("AVCaptureSessionPresetiFrame1280x720")]
		NSString PresetiFrame1280x720 { get; }

		[Field ("AVCaptureSessionPreset352x288")]
		NSString Preset352x288 { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVCaptureSessionPresetInputPriority")]
		NSString PresetInputPriority { get; }

		[NoiOS, NoMacCatalyst]
		[Field ("AVCaptureSessionPreset320x240")]
		NSString Preset320x240 { get; }

		[NoiOS, NoMacCatalyst]
		[Field ("AVCaptureSessionPreset960x540")]
		NSString Preset960x540 { get; }

		[Field ("AVCaptureSessionRuntimeErrorNotification")]
		[Notification (typeof (AVCaptureSessionRuntimeErrorEventArgs))]
		NSString RuntimeErrorNotification { get; }

		[Field ("AVCaptureSessionErrorKey")]
		NSString ErrorKey { get; }

		[Field ("AVCaptureSessionDidStartRunningNotification")]
		[Notification]
		NSString DidStartRunningNotification { get; }

		[Field ("AVCaptureSessionDidStopRunningNotification")]
		[Notification]
		NSString DidStopRunningNotification { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVCaptureSessionInterruptionEndedNotification")]
		[Notification]
		NSString InterruptionEndedNotification { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVCaptureSessionWasInterruptedNotification")]
		[Notification]
		NSString WasInterruptedNotification { get; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureSessionInterruptionReasonKey")]
		NSString InterruptionReasonKey { get; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("usesApplicationAudioSession")]
		bool UsesApplicationAudioSession { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("automaticallyConfiguresApplicationAudioSession")]
		bool AutomaticallyConfiguresApplicationAudioSession { get; set; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("automaticallyConfiguresCaptureDeviceForWideColor")]
		bool AutomaticallyConfiguresCaptureDeviceForWideColor { get; set; }

		[iOS (11, 1), NoMac]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureSessionInterruptionSystemPressureStateKey")]
		NSString InterruptionSystemPressureStateKey { get; }

		[NullAllowed]
		[MacCatalyst (15, 4), Mac (12, 3), iOS (15, 4)]
		[Export ("synchronizationClock")]
		CMClock SynchronizationClock { get; }

		[Deprecated (PlatformName.MacOSX, 12, 3, message: "Use 'SynchronizationClock' instead.")]
		[Deprecated (PlatformName.iOS, 15, 4, message: "Use 'SynchronizationClock' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 4, message: "Use 'SynchronizationClock' instead.")]
		[Export ("masterClock"), NullAllowed]
		CMClock MasterClock { get; }

		//
		// iOS 8
		//
		[MacCatalyst (14, 0)]
		[Export ("addInputWithNoConnections:")]
		void AddInputWithNoConnections (AVCaptureInput input);

		[MacCatalyst (14, 0)]
		[Export ("addOutputWithNoConnections:")]
		void AddOutputWithNoConnections (AVCaptureOutput output);

		[MacCatalyst (14, 0)]
		[Export ("canAddConnection:")]
		bool CanAddConnection (AVCaptureConnection connection);

		[MacCatalyst (14, 0)]
		[Export ("addConnection:")]
		void AddConnection (AVCaptureConnection connection);

		[MacCatalyst (14, 0)]
		[Export ("removeConnection:")]
		void RemoveConnection (AVCaptureConnection connection);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("connections")]
		AVCaptureConnection [] Connections { get; }

		[NoWatch, NoTV, NoMac, NoMacCatalyst]
		[iOS (16, 0)]
		[Export ("multitaskingCameraAccessEnabled")]
		bool MultitaskingCameraAccessEnabled { [Bind ("isMultitaskingCameraAccessEnabled")] get; set; }

		[NoWatch, NoTV, NoMac, NoMacCatalyst]
		[iOS (16, 0)]
		[Export ("multitaskingCameraAccessSupported")]
		bool MultitaskingCameraAccessSupported { [Bind ("isMultitaskingCameraAccessSupported")] get; }

		[NoWatch, NoTV, NoMac]
		[iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("hardwareCost")]
		float HardwareCost { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface AVCaptureConnection {

		[MacCatalyst (14, 0)]
		[Static]
		[Export ("connectionWithInputPorts:output:")]
		AVCaptureConnection FromInputPorts (AVCaptureInputPort [] ports, AVCaptureOutput output);

		[MacCatalyst (14, 0)]
		[Static]
		[Export ("connectionWithInputPort:videoPreviewLayer:")]
		AVCaptureConnection FromInputPort (AVCaptureInputPort port, AVCaptureVideoPreviewLayer layer);

		[MacCatalyst (14, 0)]
		[Export ("initWithInputPorts:output:")]
		NativeHandle Constructor (AVCaptureInputPort [] inputPorts, AVCaptureOutput output);

		[MacCatalyst (14, 0)]
		[Export ("initWithInputPort:videoPreviewLayer:")]
		NativeHandle Constructor (AVCaptureInputPort inputPort, AVCaptureVideoPreviewLayer layer);

		[NullAllowed]
		[Export ("output")]
		AVCaptureOutput Output { get; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("audioChannels")]
		AVCaptureAudioChannel [] AvailableAudioChannels { get; }

		[Export ("videoMirrored")]
		bool VideoMirrored { [Bind ("isVideoMirrored")] get; set; }

		[Export ("videoOrientation", ArgumentSemantic.Assign)]
		AVCaptureVideoOrientation VideoOrientation { get; set; }

		[Export ("inputPorts")]
		AVCaptureInputPort [] InputPorts { get; }

		[Export ("isActive")]
		bool Active { get; }

		[Export ("isVideoMirroringSupported")]
		bool SupportsVideoMirroring { get; }

		[Export ("isVideoOrientationSupported")]
		bool SupportsVideoOrientation { get; }

		[Deprecated (PlatformName.iOS, 7, 0 /* Only deprecated on iOS */)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("supportsVideoMinFrameDuration")]
		bool SupportsVideoMinFrameDuration { [Bind ("isVideoMinFrameDurationSupported")] get; }

		[Deprecated (PlatformName.iOS, 7, 0 /* Only deprecated on iOS */)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("videoMinFrameDuration")]
		CMTime VideoMinFrameDuration { get; set; }

		[Deprecated (PlatformName.iOS, 7, 0 /* Only deprecated on iOS */)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("supportsVideoMaxFrameDuration")]
		bool SupportsVideoMaxFrameDuration { [Bind ("isVideoMaxFrameDurationSupported")] get; }

		[Export ("videoMaxFrameDuration")]
		[Deprecated (PlatformName.iOS, 7, 0 /* Only deprecated on iOS */)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		CMTime VideoMaxFrameDuration { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("videoMaxScaleAndCropFactor")]
		nfloat VideoMaxScaleAndCropFactor { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("videoScaleAndCropFactor")]
		nfloat VideoScaleAndCropFactor { get; set; }

		[NullAllowed]
		[Export ("videoPreviewLayer")]
		AVCaptureVideoPreviewLayer VideoPreviewLayer { get; }

		[Export ("automaticallyAdjustsVideoMirroring")]
		bool AutomaticallyAdjustsVideoMirroring { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("supportsVideoStabilization")]
		bool SupportsVideoStabilization { [Bind ("isVideoStabilizationSupported")] get; }

		[NoMac]
		[Export ("videoStabilizationEnabled")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'ActiveVideoStabilizationMode' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ActiveVideoStabilizationMode' instead.")]
		bool VideoStabilizationEnabled { [Bind ("isVideoStabilizationEnabled")] get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'PreferredVideoStabilizationMode' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PreferredVideoStabilizationMode' instead.")]
		[Export ("enablesVideoStabilizationWhenAvailable")]
		bool EnablesVideoStabilizationWhenAvailable { get; set; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("preferredVideoStabilizationMode")]
		AVCaptureVideoStabilizationMode PreferredVideoStabilizationMode { get; set; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("activeVideoStabilizationMode")]
		AVCaptureVideoStabilizationMode ActiveVideoStabilizationMode { get; }

		[Unavailable (PlatformName.MacCatalyst)]
		[NoiOS]
		[Export ("supportsVideoFieldMode")]
		bool SupportsVideoFieldMode { [Bind ("isVideoFieldModeSupported")] get; }

		[NoiOS]
		[Unavailable (PlatformName.MacCatalyst)]
		[Export ("videoFieldMode")]
		AVVideoFieldMode VideoFieldMode { get; set; }

		[NoMac, NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("cameraIntrinsicMatrixDeliverySupported")]
		bool CameraIntrinsicMatrixDeliverySupported { [Bind ("isCameraIntrinsicMatrixDeliverySupported")] get; }

		[NoMac, NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("cameraIntrinsicMatrixDeliveryEnabled")]
		bool CameraIntrinsicMatrixDeliveryEnabled { [Bind ("isCameraIntrinsicMatrixDeliveryEnabled")] get; set; }

	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface AVCaptureAudioChannel {
		[Export ("peakHoldLevel")]
		float PeakHoldLevel { get; } // defined as 'float'

		[Export ("averagePowerLevel")]
		float AveragePowerLevel { get; } // defined as 'float'

		[NoiOS, MacCatalyst (15, 0)]
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NoiOS, MacCatalyst (15, 0)]
		[Export ("volume")]
		float Volume { get; set; } /* float intended here */
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSGenericException Reason: Cannot instantiate AVCaptureInput because it is an abstract superclass.
	[DisableDefaultCtor]
	interface AVCaptureInput {
		[Export ("ports")]
		AVCaptureInputPort [] Ports { get; }

		[Field ("AVCaptureInputPortFormatDescriptionDidChangeNotification")]
		[Notification]
		NSString PortFormatDescriptionDidChangeNotification { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureInputPort {
		[Export ("mediaType")]
		string MediaType { get; }

		[NullAllowed, Export ("formatDescription")]
		CMFormatDescription FormatDescription { get; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("input")]
		AVCaptureInput Input { get; }

		[Export ("clock", ArgumentSemantic.Copy), NullAllowed]
		[MacCatalyst (13, 1)]
		CMClock Clock { get; }

		[BindAs (typeof (AVCaptureDeviceType))]
		[NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("sourceDeviceType")]
		NSString SourceDeviceType { get; }

		[NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sourceDevicePosition")]
		AVCaptureDevicePosition SourceDevicePosition { get; }
	}

	interface IAVCaptureDepthDataOutputDelegate { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, NoMac]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVCaptureDepthDataOutputDelegate {
		[Export ("depthDataOutput:didOutputDepthData:timestamp:connection:")]
		void DidOutputDepthData (AVCaptureDepthDataOutput output, AVDepthData depthData, CMTime timestamp, AVCaptureConnection connection);

		[Export ("depthDataOutput:didDropDepthData:timestamp:connection:reason:")]
		void DidDropDepthData (AVCaptureDepthDataOutput output, AVDepthData depthData, CMTime timestamp, AVCaptureConnection connection, AVCaptureOutputDataDroppedReason reason);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, NoMac]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureDepthDataOutput {
		[Export ("setDelegate:callbackQueue:")]
		void SetDelegate ([NullAllowed] IAVCaptureDepthDataOutputDelegate del, [NullAllowed] DispatchQueue callbackQueue);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IAVCaptureDepthDataOutputDelegate Delegate { get; }

		[NullAllowed, Export ("delegate")]
		NSObject WeakDelegate { get; }

		[NullAllowed, Export ("delegateCallbackQueue")]
		DispatchQueue DelegateCallbackQueue { get; }

		[Export ("alwaysDiscardsLateDepthData")]
		bool AlwaysDiscardsLateDepthData { get; set; }

		[Export ("filteringEnabled")]
		bool FilteringEnabled { [Bind ("isFilteringEnabled")] get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (AVCaptureInput))]
	// crash application if 'init' is called
	[DisableDefaultCtor]
	interface AVCaptureDeviceInput {
		[Export ("device")]
		AVCaptureDevice Device { get; }

		[Static, Export ("deviceInputWithDevice:error:")]
		[return: NullAllowed]
		AVCaptureDeviceInput FromDevice (AVCaptureDevice device, out NSError error);

		[Export ("initWithDevice:error:")]
		NativeHandle Constructor (AVCaptureDevice device, out NSError error);

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("unifiedAutoExposureDefaultsEnabled")]
		bool UnifiedAutoExposureDefaultsEnabled { get; set; }

		[NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("portsWithMediaType:sourceDeviceType:sourceDevicePosition:")]
		AVCaptureInputPort [] GetPorts ([BindAs (typeof (AVMediaTypes))][NullAllowed] NSString mediaType, [BindAs (typeof (AVCaptureDeviceType))][NullAllowed] NSString sourceDeviceType, AVCaptureDevicePosition sourceDevicePosition);

		[NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("videoMinFrameDurationOverride", ArgumentSemantic.Assign)]
		CMTime VideoMinFrameDurationOverride { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface AVCaptureDeviceInputSource {
		[Export ("inputSourceID")]
		string InputSourceID { get; }

		[Export ("localizedName")]
		string LocalizedName { get; }
	}

	[NoiOS, NoWatch, NoTV, NoMacCatalyst]
	[BaseType (typeof (AVCaptureFileOutput))]
	interface AVCaptureAudioFileOutput {
		[Export ("metadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] Metadata { get; set; }

		[NullAllowed, Export ("audioSettings", ArgumentSemantic.Copy)]
		NSDictionary WeakAudioSettings { get; set; }

		[Wrap ("WeakAudioSettings")]
		[NullAllowed]
		AudioSettings AudioSettings { get; set; }

		[Static, Export ("availableOutputFileTypes")]
		NSString [] AvailableOutputFileTypes ();

		[Export ("startRecordingToOutputFileURL:outputFileType:recordingDelegate:")]
		void StartRecording (NSUrl outputFileUrl, string fileType, [Protocolize] AVCaptureFileOutputRecordingDelegate recordingDelegate);
	}

	[NoiOS, NoWatch, NoTV, NoMacCatalyst]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureAudioPreviewOutput {
		[Export ("outputDeviceUniqueID", ArgumentSemantic.Copy), NullAllowed]
		NSString OutputDeviceUniqueID { get; set; }

		[Export ("volume")]
		float Volume { get; set; } /* float, not CGFloat */
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Static]
	interface AVAssetExportPresetApple {

		[NoiOS]
		[NoMacCatalyst]
		[Field ("AVAssetExportPresetAppleM4VCellular")]
		NSString M4VCellular { get; }

		[NoiOS]
		[NoMacCatalyst]
		[Field ("AVAssetExportPresetAppleM4ViPod")]
		NSString M4ViPod { get; }

		[NoiOS]
		[NoMacCatalyst]
		[Field ("AVAssetExportPresetAppleM4V480pSD")]
		NSString M4V480pSD { get; }

		[NoiOS]
		[NoMacCatalyst]
		[Field ("AVAssetExportPresetAppleM4VAppleTV")]
		NSString M4VAppleTV { get; }

		[NoiOS]
		[NoMacCatalyst]
		[Field ("AVAssetExportPresetAppleM4VWiFi")]
		NSString M4VWiFi { get; }

		[NoiOS]
		[NoMacCatalyst]
		[Field ("AVAssetExportPresetAppleM4V720pHD")]
		NSString M4V720pHD { get; }

		[NoiOS]
		[NoMacCatalyst]
		[Field ("AVAssetExportPresetAppleM4V1080pHD")]
		NSString M4V1080pHD { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("AVAssetExportPresetAppleProRes422LPCM")]
		NSString ProRes422Lpcm { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
#if NET
	// Making a class abstract has problems (see https://github.com/xamarin/xamarin-macios/issues/4969), so not doing this (yet).
	// [Abstract] // as per docs
#endif
	// Objective-C exception thrown.  Name: NSGenericException Reason: Cannot instantiate AVCaptureOutput because it is an abstract superclass.
	[DisableDefaultCtor]
	interface AVCaptureOutput {
		[Export ("connections")]
		AVCaptureConnection [] Connections { get; }

		[Export ("connectionWithMediaType:")]
		[return: NullAllowed]
		AVCaptureConnection ConnectionFromMediaType (NSString avMediaType);

		[MacCatalyst (13, 1)]
		[Export ("metadataOutputRectOfInterestForRect:")]
		CGRect GetMetadataOutputRectOfInterestForRect (CGRect rectInOutputCoordinates);

		[MacCatalyst (13, 1)]
		[Export ("rectForMetadataOutputRectOfInterest:")]
		CGRect GetRectForMetadataOutputRectOfInterest (CGRect rectInMetadataOutputCoordinates);

		[MacCatalyst (13, 1)]
		[Export ("transformedMetadataObjectForMetadataObject:connection:")]
		[return: NullAllowed]
		AVMetadataObject GetTransformedMetadataObject (AVMetadataObject metadataObject, AVCaptureConnection connection);
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[BaseType (typeof (AVCaptureInput))]
	interface AVCaptureScreenInput {
		[Export ("initWithDisplayID:")]
		NativeHandle Constructor (uint /* CGDirectDisplayID = uint32_t */ displayID);

		[Export ("minFrameDuration")]
		CMTime MinFrameDuration { get; set; }

		[Export ("cropRect")]
		CGRect CropRect { get; set; }

		[Export ("scaleFactor")]
		nfloat ScaleFactor { get; set; }

		[Export ("capturesMouseClicks")]
		bool CapturesMouseClicks { get; set; }

		[Export ("capturesCursor")]
		bool CapturesCursor { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Ignored since 10.10, if you want to get this behavior, use AVCaptureVideoDataOutput and compare the frame contents on your own code.")]
		[Export ("removesDuplicateFrames")]
		bool RemovesDuplicateFrames { get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (CALayer))]
	interface AVCaptureVideoPreviewLayer {
		[NullAllowed] // by default this property is null
		[Export ("session", ArgumentSemantic.Retain)]
		AVCaptureSession Session { get; set; }

		[MacCatalyst (14, 0)]
		[Export ("setSessionWithNoConnection:")]
		void SetSessionWithNoConnection (AVCaptureSession session);

		[Export ("videoGravity", ArgumentSemantic.Copy)]
		[Protected]
		NSString WeakVideoGravity { get; set; }

		[NoMac]
		[Export ("orientation")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'AVCaptureConnection.VideoOrientation' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCaptureConnection.VideoOrientation' instead.")]
		AVCaptureVideoOrientation Orientation { get; set; }

		[NoMac]
		[Export ("automaticallyAdjustsMirroring")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'AVCaptureConnection.AutomaticallyAdjustsVideoMirroring' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCaptureConnection.AutomaticallyAdjustsVideoMirroring' instead.")]
		bool AutomaticallyAdjustsMirroring { get; set; }

		[NoMac]
		[Export ("mirrored")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'AVCaptureConnection.VideoMirrored' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCaptureConnection.VideoMirrored' instead.")]
		bool Mirrored { [Bind ("isMirrored")] get; set; }

		[NoMac]
		[Export ("isMirroringSupported")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'AVCaptureConnection.IsVideoMirroringSupported' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCaptureConnection.IsVideoMirroringSupported' instead.")]
		bool MirroringSupported { get; }

		[NoMac]
		[Export ("isOrientationSupported")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'AVCaptureConnection.IsVideoOrientationSupported' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCaptureConnection.IsVideoOrientationSupported' instead.")]
		bool OrientationSupported { get; }

		[Static, Export ("layerWithSession:")]
		AVCaptureVideoPreviewLayer FromSession (AVCaptureSession session);

		[Export ("initWithSession:")]
		[Internal]
		IntPtr InitWithConnection (AVCaptureSession session);

		[MacCatalyst (14, 0)]
		[Internal]
		[Export ("initWithSessionWithNoConnection:")]
		IntPtr InitWithNoConnection (AVCaptureSession session);

		[NullAllowed, Export ("connection")]
		AVCaptureConnection Connection { get; }

		[MacCatalyst (13, 1)]
		[Export ("captureDevicePointOfInterestForPoint:")]
		CGPoint CaptureDevicePointOfInterestForPoint (CGPoint pointInLayer);

		[MacCatalyst (13, 1)]
		[Export ("pointForCaptureDevicePointOfInterest:")]
		CGPoint PointForCaptureDevicePointOfInterest (CGPoint captureDevicePointOfInterest);

		[MacCatalyst (13, 1)]
		[Export ("transformedMetadataObjectForMetadataObject:")]
		[return: NullAllowed]
		AVMetadataObject GetTransformedMetadataObject (AVMetadataObject metadataObject);

		[MacCatalyst (13, 1)]
		[Export ("metadataOutputRectOfInterestForRect:")]
		CGRect MapToMetadataOutputCoordinates (CGRect rectInLayerCoordinates);

		[MacCatalyst (13, 1)]
		[Export ("rectForMetadataOutputRectOfInterest:")]
		CGRect MapToLayerCoordinates (CGRect rectInMetadataOutputCoordinates);

		[MacCatalyst (14, 0)]
		[Static]
		[Export ("layerWithSessionWithNoConnection:")]
		AVCaptureVideoPreviewLayer CreateWithNoConnection (AVCaptureSession session);

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("previewing")]
		bool Previewing { [Bind ("isPreviewing")] get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV]
	[NoWatch]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureVideoDataOutput {
		[NullAllowed, Export ("sampleBufferDelegate")]
		[Protocolize]
		AVCaptureVideoDataOutputSampleBufferDelegate SampleBufferDelegate { get; }

		[NullAllowed, Export ("sampleBufferCallbackQueue")]
		DispatchQueue SampleBufferCallbackQueue { get; }

		[Export ("videoSettings", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary WeakVideoSettings { get; set; }

		[Wrap ("WeakVideoSettings")]
		AVVideoSettingsUncompressed UncompressedVideoSetting { get; set; }

		[Wrap ("WeakVideoSettings")]
		AVVideoSettingsCompressed CompressedVideoSetting { get; set; }

		[Export ("minFrameDuration")]
		[Deprecated (PlatformName.iOS, 5, 0, message: "Use 'AVCaptureConnection.MinVideoFrameDuration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCaptureConnection.MinVideoFrameDuration' instead.")]
		CMTime MinFrameDuration { get; set; }

		[Export ("alwaysDiscardsLateVideoFrames")]
		bool AlwaysDiscardsLateVideoFrames { get; set; }

#if !NET
		[Obsolete ("Use overload accepting a 'IAVCaptureVideoDataOutputSampleBufferDelegate'.")]
		[Export ("setSampleBufferDelegate:queue:")]
		[PostGet ("SampleBufferDelegate")]
		[PostGet ("SampleBufferCallbackQueue")]
		void SetSampleBufferDelegate ([NullAllowed] AVCaptureVideoDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackQueue);
#endif

		[Export ("setSampleBufferDelegate:queue:")]
#if NET
		void SetSampleBufferDelegate ([NullAllowed] IAVCaptureVideoDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackQueue);
#else
		[Sealed]
		void SetSampleBufferDelegateQueue ([NullAllowed] IAVCaptureVideoDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackQueue);
#endif

		// 5.0 APIs
#if NET
		[BindAs (typeof (CoreVideo.CVPixelFormatType []))]
#endif
		[Export ("availableVideoCVPixelFormatTypes")]
		NSNumber [] AvailableVideoCVPixelFormatTypes { get; }

		// This is an NSString, because these are are codec types that can be used as keys in
		// the WeakVideoSettings properties.
		[Export ("availableVideoCodecTypes")]
		NSString [] AvailableVideoCodecTypes { get; }

		[MacCatalyst (13, 1)]
		[Export ("recommendedVideoSettingsForAssetWriterWithOutputFileType:")]
		[return: NullAllowed]
		NSDictionary GetRecommendedVideoSettingsForAssetWriter (string outputFileType);

		[MacCatalyst (14, 0)]
		[Export ("availableVideoCodecTypesForAssetWriterWithOutputFileType:")]
		string [] GetAvailableVideoCodecTypes (string outputFileType);

		[Internal]
		[MacCatalyst (14, 0)]
		[Export ("recommendedVideoSettingsForVideoCodecType:assetWriterOutputFileType:")]
		[return: NullAllowed]
		NSDictionary GetWeakRecommendedVideoSettings (string videoCodecType, string outputFileType);

		[MacCatalyst (14, 0)]
		[Wrap ("new AVPlayerItemVideoOutputSettings (GetWeakRecommendedVideoSettings (videoCodecType, outputFileType)!)")]
		[return: NullAllowed]
		AVPlayerItemVideoOutputSettings GetRecommendedVideoSettings (string videoCodecType, string outputFileType);

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("automaticallyConfiguresOutputBufferDimensions")]
		bool AutomaticallyConfiguresOutputBufferDimensions { get; set; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("deliversPreviewSizedOutputBuffers")]
		bool DeliversPreviewSizedOutputBuffers { get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVCaptureVideoDataOutputSampleBufferDelegate {
		[Export ("captureOutput:didOutputSampleBuffer:fromConnection:")]
		// CMSampleBufferRef		
		void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection);

		[Export ("captureOutput:didDropSampleBuffer:fromConnection:")]
		void DidDropSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection);
	}

	interface IAVCaptureVideoDataOutputSampleBufferDelegate { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureAudioDataOutput {
		[NullAllowed, Export ("sampleBufferDelegate")]
		[Protocolize]
		AVCaptureAudioDataOutputSampleBufferDelegate SampleBufferDelegate { get; }

		[NullAllowed, Export ("sampleBufferCallbackQueue")]
		DispatchQueue SampleBufferCallbackQueue { get; }

		[Export ("setSampleBufferDelegate:queue:")]
#if NET
		void SetSampleBufferDelegate ([NullAllowed] IAVCaptureAudioDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackDispatchQueue);
#else
		[Sealed]
		void SetSampleBufferDelegateQueue ([NullAllowed] IAVCaptureAudioDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackDispatchQueue);
#endif

#if !NET
		[Obsolete ("Use overload accepting a 'IAVCaptureVideoDataOutputSampleBufferDelegate'.")]
		[Export ("setSampleBufferDelegate:queue:")]
		void SetSampleBufferDelegateQueue ([NullAllowed] AVCaptureAudioDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackDispatchQueue);
#endif

		[MacCatalyst (13, 1)]
		[Export ("recommendedAudioSettingsForAssetWriterWithOutputFileType:")]
		[return: NullAllowed]
		NSDictionary GetRecommendedAudioSettingsForAssetWriter (string outputFileType);

		[NoiOS, MacCatalyst (15, 0)]
		[Export ("audioSettings", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDictionary WeakAudioSettings { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[Wrap ("WeakAudioSettings")]
		[NullAllowed]
		AudioSettings AudioSettings { get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVCaptureAudioDataOutputSampleBufferDelegate {
		[Export ("captureOutput:didOutputSampleBuffer:fromConnection:")]
		void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoMac]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	[Abstract]
	interface AVCaptureBracketedStillImageSettings {
		// Abstract class in obJC
	}

	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVCaptureBracketedStillImageSettings))]
	[DisableDefaultCtor]
	interface AVCaptureManualExposureBracketedStillImageSettings {
		[Export ("exposureDuration")]
		CMTime ExposureDuration { get; }

		[Export ("ISO")]
		float ISO { get; } /* float, not CGFloat */

		[Static, Export ("manualExposureSettingsWithExposureDuration:ISO:")]
		AVCaptureManualExposureBracketedStillImageSettings Create (CMTime duration, float /* float, not CGFloat */ ISO);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoMac]
	[NoTV]
	[BaseType (typeof (AVCaptureBracketedStillImageSettings))]
	[DisableDefaultCtor]
	interface AVCaptureAutoExposureBracketedStillImageSettings {
		[Export ("exposureTargetBias")]
		float ExposureTargetBias { get; } /* float, not CGFloat */

		[Static, Export ("autoExposureSettingsWithExposureTargetBias:")]
		AVCaptureAutoExposureBracketedStillImageSettings Create (float /* float, not CGFloat */ exposureTargetBias);
	}

	interface IAVCaptureAudioDataOutputSampleBufferDelegate { }

	interface IAVCaptureFileOutputRecordingDelegate { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[BaseType (typeof (AVCaptureOutput))]
	// Objective-C exception thrown.  Name: NSGenericException Reason: Cannot instantiate AVCaptureFileOutput because it is an abstract superclass.
	[DisableDefaultCtor]
	[NoTV]
	interface AVCaptureFileOutput {
		[Export ("recordedDuration")]
		CMTime RecordedDuration { get; }

		[Export ("recordedFileSize")]
		long RecordedFileSize { get; }

		[Export ("isRecording")]
		bool Recording { get; }

		[Export ("maxRecordedDuration")]
		CMTime MaxRecordedDuration { get; set; }

		[Export ("maxRecordedFileSize")]
		long MaxRecordedFileSize { get; set; }

		[Export ("minFreeDiskSpaceLimit")]
		long MinFreeDiskSpaceLimit { get; set; }

		[NullAllowed, Export ("outputFileURL")]
		NSUrl OutputFileURL { get; } // FIXME: should have been Url.

		[Export ("startRecordingToOutputFileURL:recordingDelegate:")]
		void StartRecordingToOutputFile (NSUrl outputFileUrl, [Protocolize] AVCaptureFileOutputRecordingDelegate recordingDelegate);

		[Export ("stopRecording")]
		void StopRecording ();

		[NoiOS, MacCatalyst (15, 0)]
		[Export ("pauseRecording")]
		void PauseRecording ();

		[NoiOS, MacCatalyst (15, 0)]
		[Export ("resumeRecording")]
		void ResumeRecording ();

		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		IAVCaptureFileOutputDelegate Delegate { get; set; }

		[NoiOS, MacCatalyst (15, 0)]
		[Export ("recordingPaused")]
		bool RecordingPaused { [Bind ("isRecordingPaused")] get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	interface AVCaptureFileOutputRecordingDelegate {
		[Export ("captureOutput:didStartRecordingToOutputFileAtURL:fromConnections:")]
		void DidStartRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject [] connections);

		[Abstract]
		[Export ("captureOutput:didFinishRecordingToOutputFileAtURL:fromConnections:error:"), CheckDisposed]
		void FinishedRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject [] connections, [NullAllowed] NSError error);

		[NoMacCatalyst, NoiOS]
		[Export ("captureOutput:didPauseRecordingToOutputFileAtURL:fromConnections:")]
		void DidPauseRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, AVCaptureConnection [] connections);

		[NoMacCatalyst, NoiOS]
		[Export ("captureOutput:didResumeRecordingToOutputFileAtURL:fromConnections:")]
		void DidResumeRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, AVCaptureConnection [] connections);

		[NoMacCatalyst, NoiOS]
		[Export ("captureOutput:willFinishRecordingToOutputFileAtURL:fromConnections:error:")]
		void WillFinishRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, AVCaptureConnection [] connections, [NullAllowed] NSError error);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[NoMac]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureMetadataOutput {
		[NullAllowed, Export ("metadataObjectsDelegate")]
		[Protocolize]
		AVCaptureMetadataOutputObjectsDelegate Delegate { get; }

		[NullAllowed, Export ("metadataObjectsCallbackQueue")]
		DispatchQueue CallbackQueue { get; }

		[Export ("availableMetadataObjectTypes")]
		NSString [] WeakAvailableMetadataObjectTypes { get; }

		[NullAllowed]
		[Export ("metadataObjectTypes", ArgumentSemantic.Copy)]
		NSString [] WeakMetadataObjectTypes { get; set; }

		[Export ("setMetadataObjectsDelegate:queue:")]
		void SetDelegate ([NullAllowed][Protocolize] AVCaptureMetadataOutputObjectsDelegate objectsDelegate, [NullAllowed] DispatchQueue objectsCallbackQueue);

		[Export ("rectOfInterest", ArgumentSemantic.Copy)]
		CGRect RectOfInterest { get; set; }

	}

	[NoWatch]
	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVCaptureMetadataOutputObjectsDelegate {
		[Export ("captureOutput:didOutputMetadataObjects:fromConnection:")]
		void DidOutputMetadataObjects (AVCaptureMetadataOutput captureOutput, AVMetadataObject [] metadataObjects, AVCaptureConnection connection);
	}

	[NoTV, NoWatch, iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Internal]
	[Static]
	interface AVCapturePhotoSettingsThumbnailFormatKeys {
		[Field ("AVVideoCodecKey")]
		NSString CodecKey { get; }

		[Field ("AVVideoWidthKey")]
		NSString WidthKey { get; }

		[Field ("AVVideoHeightKey")]
		NSString HeightKey { get; }
	}


	[NoTV, NoWatch, iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("AVCapturePhotoSettingsThumbnailFormatKeys")]
	interface AVCapturePhotoSettingsThumbnailFormat {
		NSString Codec { get; set; }
		NSNumber Width { get; set; }
		NSNumber Height { get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCapturePhotoSettings : NSCopying {
		[Static]
		[Export ("photoSettings")]
		AVCapturePhotoSettings Create ();

		[Static]
		[Export ("photoSettingsWithFormat:")]
		AVCapturePhotoSettings FromFormat ([NullAllowed] NSDictionary<NSString, NSObject> format);

		[Static]
		[Export ("photoSettingsWithRawPixelFormatType:")]
		AVCapturePhotoSettings FromRawPixelFormatType (uint rawPixelFormatType);

		[Static]
		[Export ("photoSettingsWithRawPixelFormatType:processedFormat:")]
		AVCapturePhotoSettings FromRawPixelFormatType (uint rawPixelFormatType, [NullAllowed] NSDictionary<NSString, NSObject> processedFormat);

		[Static]
		[Export ("photoSettingsFromPhotoSettings:")]
		AVCapturePhotoSettings FromPhotoSettings (AVCapturePhotoSettings photoSettings);

		[MacCatalyst (14, 0)]
		[Static]
		[Export ("photoSettingsWithRawPixelFormatType:rawFileType:processedFormat:processedFileType:")]
		AVCapturePhotoSettings FromRawPixelFormatType (uint rawPixelFormatType, [NullAllowed] string rawFileType, [NullAllowed] NSDictionary<NSString, NSObject> processedFormat, [NullAllowed] string processedFileType);

		[Export ("uniqueID")]
		long UniqueID { get; }

		[NullAllowed, Export ("format", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Format { get; }

		[Export ("rawPhotoPixelFormatType")]
		uint RawPhotoPixelFormatType { get; }

		[Export ("flashMode", ArgumentSemantic.Assign)]
		AVCaptureFlashMode FlashMode { get; set; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'PhotoQualityPrioritization' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PhotoQualityPrioritization' instead.")]
		[Export ("autoStillImageStabilizationEnabled")]
		bool IsAutoStillImageStabilizationEnabled { [Bind ("isAutoStillImageStabilizationEnabled")] get; set; }

		[Deprecated (PlatformName.iOS, 16, 0, message: "Use 'MaxPhotoDimensions' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use 'MaxPhotoDimensions' instead.")]
		[Deprecated (PlatformName.MacOSX, 13, 0, message: "Use 'MaxPhotoDimensions' instead.")]
		[Export ("highResolutionPhotoEnabled")]
		bool IsHighResolutionPhotoEnabled { [Bind ("isHighResolutionPhotoEnabled")] get; set; }

		[NullAllowed, Export ("livePhotoMovieFileURL", ArgumentSemantic.Copy)]
		NSUrl LivePhotoMovieFileUrl { get; set; }

		[NullAllowed, Export ("livePhotoMovieMetadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] LivePhotoMovieMetadata { get; set; }

		[Export ("availablePreviewPhotoPixelFormatTypes")]
		NSNumber [] AvailablePreviewPhotoPixelFormatTypes { get; }

		[NullAllowed, Export ("previewPhotoFormat", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> PreviewPhotoFormat { get; set; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'AutoVirtualDeviceFusionEnabled' instead.")]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AutoVirtualDeviceFusionEnabled' instead.")]
		[Export ("autoDualCameraFusionEnabled")]
		bool AutoDualCameraFusionEnabled { [Bind ("isAutoDualCameraFusionEnabled")] get; set; }

		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("processedFileType")]
		string ProcessedFileType { get; }

		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("rawFileType")]
		string RawFileType { get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'VirtualDeviceConstituentPhotoDeliveryEnabled' instead.")]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'VirtualDeviceConstituentPhotoDeliveryEnabled' instead.")]
		[Export ("dualCameraDualPhotoDeliveryEnabled")]
		bool DualCameraDualPhotoDeliveryEnabled { [Bind ("isDualCameraDualPhotoDeliveryEnabled")] get; set; }

		[MacCatalyst (14, 0)]
		[Export ("depthDataDeliveryEnabled")]
		bool DepthDataDeliveryEnabled { [Bind ("isDepthDataDeliveryEnabled")] get; set; }

		[MacCatalyst (14, 0)]
		[Export ("embedsDepthDataInPhoto")]
		bool EmbedsDepthDataInPhoto { get; set; }

		[MacCatalyst (14, 0)]
		[Export ("depthDataFiltered")]
		bool DepthDataFiltered { [Bind ("isDepthDataFiltered")] get; set; }

		[MacCatalyst (14, 0)]
		[Export ("cameraCalibrationDataDeliveryEnabled")]
		bool CameraCalibrationDataDeliveryEnabled { [Bind ("isCameraCalibrationDataDeliveryEnabled")] get; set; }

		[MacCatalyst (14, 0)]
		[Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary Metadata { get; set; }

		[MacCatalyst (14, 0)]
		[Export ("livePhotoVideoCodecType")]
		string LivePhotoVideoCodecType { get; set; }

		[Internal]
		[MacCatalyst (14, 0)]
		[Export ("availableEmbeddedThumbnailPhotoCodecTypes")]
		NSString [] _GetAvailableEmbeddedThumbnailPhotoCodecTypes { get; }

#if !NET
		[Obsolete ("Use 'AvailableEmbeddedThumbnailPhotoCodecTypes' instead.")]
		[Wrap ("Array.ConvertAll (_GetAvailableEmbeddedThumbnailPhotoCodecTypes, s => AVVideoCodecTypeExtensions.GetValue (s))", IsVirtual = false)]
		AVVideoCodecType [] GetAvailableEmbeddedThumbnailPhotoCodecTypes { get; }
#endif
		[MacCatalyst (14, 0)]
		[Wrap ("Array.ConvertAll (_GetAvailableEmbeddedThumbnailPhotoCodecTypes, s => AVVideoCodecTypeExtensions.GetValue (s))", IsVirtual = true)]
		AVVideoCodecType [] AvailableEmbeddedThumbnailPhotoCodecTypes { get; }

#if NET
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("embeddedThumbnailPhotoFormat", ArgumentSemantic.Copy)]
		NSDictionary WeakEmbeddedThumbnailPhotoFormat { get; set; }

		[MacCatalyst (14, 0)]
		[Wrap ("WeakEmbeddedThumbnailPhotoFormat")]
		AVCapturePhotoSettingsThumbnailFormat EmbeddedThumbnailPhotoFormat { get; set; }
#else
		[NullAllowed, Export ("embeddedThumbnailPhotoFormat", ArgumentSemantic.Copy)]
		NSDictionary EmbeddedThumbnailPhotoFormat { get; set; }
#endif

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("portraitEffectsMatteDeliveryEnabled")]
		bool PortraitEffectsMatteDeliveryEnabled { [Bind ("isPortraitEffectsMatteDeliveryEnabled")] get; set; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("embedsPortraitEffectsMatteInPhoto")]
		bool EmbedsPortraitEffectsMatteInPhoto { get; set; }

		[BindAs (typeof (AVVideoCodecType []))]
		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("availableRawEmbeddedThumbnailPhotoCodecTypes")]
		NSString [] AvailableRawEmbeddedThumbnailPhotoCodecTypes { get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("rawEmbeddedThumbnailPhotoFormat", ArgumentSemantic.Copy)]
		NSDictionary WeakRawEmbeddedThumbnailPhotoFormat { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("WeakRawEmbeddedThumbnailPhotoFormat")]
		AVCapturePhotoSettingsThumbnailFormat RawEmbeddedThumbnailPhotoFormat { get; set; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("autoRedEyeReductionEnabled")]
		bool AutoRedEyeReductionEnabled { [Bind ("isAutoRedEyeReductionEnabled")] get; set; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("photoQualityPrioritization", ArgumentSemantic.Assign)]
		AVCapturePhotoQualityPrioritization PhotoQualityPrioritization { get; set; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("embedsSemanticSegmentationMattesInPhoto")]
		bool EmbedsSemanticSegmentationMattesInPhoto { get; set; }

		[BindAs (typeof (AVSemanticSegmentationMatteType []))]
		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("enabledSemanticSegmentationMatteTypes", ArgumentSemantic.Assign)]
		NSString [] EnabledSemanticSegmentationMatteTypes { get; set; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("virtualDeviceConstituentPhotoDeliveryEnabledDevices", ArgumentSemantic.Copy)]
		AVCaptureDevice [] VirtualDeviceConstituentPhotoDeliveryEnabledDevices { get; set; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("autoVirtualDeviceFusionEnabled")]
		bool AutoVirtualDeviceFusionEnabled { [Bind ("isAutoVirtualDeviceFusionEnabled")] get; set; }

		[Introduced (PlatformName.MacCatalyst, 14, 1)]
		[iOS (14, 1)]
		[NoMac]
		[Export ("autoContentAwareDistortionCorrectionEnabled")]
		bool AutoContentAwareDistortionCorrectionEnabled { [Bind ("isAutoContentAwareDistortionCorrectionEnabled")] get; set; }

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Mac (13, 0)]
		[Export ("maxPhotoDimensions")]
		CMVideoDimensions MaxPhotoDimensions { get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV, NoMac]
	[BaseType (typeof (AVCapturePhotoSettings))]
	[DisableDefaultCtor]
	interface AVCapturePhotoBracketSettings {
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("photoBracketSettingsWithRawPixelFormatType:rawFileType:processedFormat:processedFileType:bracketedSettings:")]
		AVCapturePhotoBracketSettings FromPhotoBracketSettings (uint rawPixelFormatType, [NullAllowed] string rawFileType, [NullAllowed] NSDictionary<NSString, NSObject> processedFormat, [NullAllowed] string processedFileType, AVCaptureBracketedStillImageSettings [] bracketedSettings);

		[Static]
		[Export ("photoBracketSettingsWithRawPixelFormatType:processedFormat:bracketedSettings:")]
		AVCapturePhotoBracketSettings FromRawPixelFormatType (uint rawPixelFormatType, [NullAllowed] NSDictionary<NSString, NSObject> format, AVCaptureBracketedStillImageSettings [] bracketedSettings);

		[Export ("bracketedSettings")]
		AVCaptureBracketedStillImageSettings [] BracketedSettings { get; }

		[Export ("lensStabilizationEnabled")]
		bool IsLensStabilizationEnabled { [Bind ("isLensStabilizationEnabled")] get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureResolvedPhotoSettings {
		[Export ("uniqueID")]
		long UniqueID { get; }

		[Export ("photoDimensions")]
		CMVideoDimensions PhotoDimensions { get; }

		[Export ("rawPhotoDimensions")]
		CMVideoDimensions RawPhotoDimensions { get; }

		[Export ("previewDimensions")]
		CMVideoDimensions PreviewDimensions { get; }

		[Export ("livePhotoMovieDimensions")]
		CMVideoDimensions LivePhotoMovieDimensions { get; }

		[Export ("flashEnabled")]
		bool IsFlashEnabled { [Bind ("isFlashEnabled")] get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'AVCaptureResolvedPhotoSettings.PhotoProcessingTimeRange' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCaptureResolvedPhotoSettings.PhotoProcessingTimeRange' instead.")]
		[Export ("stillImageStabilizationEnabled")]
		bool IsStillImageStabilizationEnabled { [Bind ("isStillImageStabilizationEnabled")] get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'VirtualDeviceFusionEnabled' instead.")]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'VirtualDeviceFusionEnabled' instead.")]
		[Export ("dualCameraFusionEnabled")]
		bool DualCameraFusionEnabled { [Bind ("isDualCameraFusionEnabled")] get; }

		[MacCatalyst (14, 0)]
		[Export ("embeddedThumbnailDimensions")]
		CMVideoDimensions EmbeddedThumbnailDimensions { get; }

		[MacCatalyst (14, 0)]
		[Export ("expectedPhotoCount")]
		nuint ExpectedPhotoCount { get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("portraitEffectsMatteDimensions")]
		CMVideoDimensions PortraitEffectsMatteDimensions { get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("rawEmbeddedThumbnailDimensions")]
		CMVideoDimensions RawEmbeddedThumbnailDimensions { get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("redEyeReductionEnabled")]
		bool RedEyeReductionEnabled { [Bind ("isRedEyeReductionEnabled")] get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("dimensionsForSemanticSegmentationMatteOfType:")]
		CMVideoDimensions GetDimensions ([BindAs (typeof (AVSemanticSegmentationMatteType))] NSString semanticSegmentationMatteType);

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("photoProcessingTimeRange")]
		CMTimeRange PhotoProcessingTimeRange { get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("virtualDeviceFusionEnabled")]
		bool VirtualDeviceFusionEnabled { [Bind ("isVirtualDeviceFusionEnabled")] get; }

		[Introduced (PlatformName.MacCatalyst, 14, 1)]
		[iOS (14, 1)]
		[NoMac]
		[Export ("contentAwareDistortionCorrectionEnabled")]
		bool ContentAwareDistortionCorrectionEnabled { [Bind ("isContentAwareDistortionCorrectionEnabled")] get; }
	}


	interface IAVCapturePhotoCaptureDelegate { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVCapturePhotoCaptureDelegate {
		[Export ("captureOutput:willBeginCaptureForResolvedSettings:")]
		void WillBeginCapture (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings);

		[Export ("captureOutput:willCapturePhotoForResolvedSettings:")]
		void WillCapturePhoto (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings);

		[Export ("captureOutput:didCapturePhotoForResolvedSettings:")]
		void DidCapturePhoto (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings);

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use the 'DidFinishProcessingPhoto' overload accepting a 'AVCapturePhoto' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'DidFinishProcessingPhoto' overload accepting a 'AVCapturePhoto' instead.")]
		[Export ("captureOutput:didFinishProcessingPhotoSampleBuffer:previewPhotoSampleBuffer:resolvedSettings:bracketSettings:error:")]
		void DidFinishProcessingPhoto (AVCapturePhotoOutput captureOutput, [NullAllowed] CMSampleBuffer photoSampleBuffer, [NullAllowed] CMSampleBuffer previewPhotoSampleBuffer, AVCaptureResolvedPhotoSettings resolvedSettings, [NullAllowed] AVCaptureBracketedStillImageSettings bracketSettings, [NullAllowed] NSError error);

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use the 'DidFinishProcessingPhoto' overload accepting a 'AVCapturePhoto' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'DidFinishProcessingPhoto' overload accepting a 'AVCapturePhoto' instead.")]
		[Export ("captureOutput:didFinishProcessingRawPhotoSampleBuffer:previewPhotoSampleBuffer:resolvedSettings:bracketSettings:error:")]
		void DidFinishProcessingRawPhoto (AVCapturePhotoOutput captureOutput, [NullAllowed] CMSampleBuffer rawSampleBuffer, [NullAllowed] CMSampleBuffer previewPhotoSampleBuffer, AVCaptureResolvedPhotoSettings resolvedSettings, [NullAllowed] AVCaptureBracketedStillImageSettings bracketSettings, [NullAllowed] NSError error);

		[MacCatalyst (14, 0)]
		[Export ("captureOutput:didFinishProcessingPhoto:error:")]
		void DidFinishProcessingPhoto (AVCapturePhotoOutput output, AVCapturePhoto photo, [NullAllowed] NSError error);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("captureOutput:didFinishRecordingLivePhotoMovieForEventualFileAtURL:resolvedSettings:")]
		void DidFinishRecordingLivePhotoMovie (AVCapturePhotoOutput captureOutput, NSUrl outputFileUrl, AVCaptureResolvedPhotoSettings resolvedSettings);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("captureOutput:didFinishProcessingLivePhotoToMovieFileAtURL:duration:photoDisplayTime:resolvedSettings:error:")]
		void DidFinishProcessingLivePhotoMovie (AVCapturePhotoOutput captureOutput, NSUrl outputFileUrl, CMTime duration, CMTime photoDisplayTime, AVCaptureResolvedPhotoSettings resolvedSettings, [NullAllowed] NSError error);

		[Export ("captureOutput:didFinishCaptureForResolvedSettings:error:")]
		void DidFinishCapture (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings, [NullAllowed] NSError error);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCapturePhotoOutput {
		[Export ("capturePhotoWithSettings:delegate:")]
		void CapturePhoto (AVCapturePhotoSettings settings, IAVCapturePhotoCaptureDelegate cb);

		[Export ("availablePhotoPixelFormatTypes")]
		NSNumber [] AvailablePhotoPixelFormatTypes { get; }

		[Export ("availablePhotoCodecTypes")]
		string [] AvailablePhotoCodecTypes { get; }

		[Introduced (PlatformName.MacCatalyst, 14, 3)]
		[NoWatch, NoTV, NoMac, iOS (14, 3)]
		[Export ("appleProRAWSupported")]
		bool AppleProRawSupported { [Bind ("isAppleProRAWSupported")] get; }

		[Introduced (PlatformName.MacCatalyst, 14, 3)]
		[NoWatch, NoTV, NoMac, iOS (14, 3)]
		[Export ("appleProRAWEnabled")]
		bool AppleProRawEnabled { [Bind ("isAppleProRAWEnabled")] get; set; }

		[Introduced (PlatformName.MacCatalyst, 14, 3)]
		[NoWatch, NoTV, NoMac, iOS (14, 3)]
		[Static]
		[Export ("isBayerRAWPixelFormat:")]
		bool IsBayerRawPixelFormat (CVPixelFormatType pixelFormat);

		[Introduced (PlatformName.MacCatalyst, 14, 3)]
		[NoWatch, NoTV, NoMac, iOS (14, 3)]
		[Static]
		[Export ("isAppleProRAWPixelFormat:")]
		bool IsAppleProRawPixelFormat (CVPixelFormatType pixelFormat);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("availableRawPhotoPixelFormatTypes")]
		NSNumber [] AvailableRawPhotoPixelFormatTypes { get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'MaxPhotoQualityPrioritization' instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'MaxPhotoQualityPrioritization' instead.")]
		[Export ("stillImageStabilizationSupported")]
		bool IsStillImageStabilizationSupported { [Bind ("isStillImageStabilizationSupported")] get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'MaxPhotoQualityPrioritization' instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'MaxPhotoQualityPrioritization' instead.")]
		[Export ("isStillImageStabilizationScene")]
		bool IsStillImageStabilizationScene { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
#if NET
		[BindAs (typeof (AVCaptureFlashMode []))]
#endif
		[Export ("supportedFlashModes")]
		NSNumber [] SupportedFlashModes { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("isFlashScene")]
		bool IsFlashScene { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("photoSettingsForSceneMonitoring", ArgumentSemantic.Copy)]
		AVCapturePhotoSettings PhotoSettingsForSceneMonitoring { get; set; }

		[Deprecated (PlatformName.iOS, 16, 0, message: "Use 'MaxPhotoDimensions' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use 'MaxPhotoDimensions' instead.")]
		[Deprecated (PlatformName.MacOSX, 13, 0, message: "Use 'MaxPhotoDimensions' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("highResolutionCaptureEnabled")]
		bool IsHighResolutionCaptureEnabled { [Bind ("isHighResolutionCaptureEnabled")] get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("maxBracketedCapturePhotoCount")]
		nuint MaxBracketedCapturePhotoCount { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("lensStabilizationDuringBracketedCaptureSupported")]
		bool IsLensStabilizationDuringBracketedCaptureSupported { [Bind ("isLensStabilizationDuringBracketedCaptureSupported")] get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("livePhotoCaptureSupported")]
		bool IsLivePhotoCaptureSupported { [Bind ("isLivePhotoCaptureSupported")] get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("livePhotoCaptureEnabled")]
		bool IsLivePhotoCaptureEnabled { [Bind ("isLivePhotoCaptureEnabled")] get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("livePhotoCaptureSuspended")]
		bool IsLivePhotoCaptureSuspended { [Bind ("isLivePhotoCaptureSuspended")] get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("livePhotoAutoTrimmingEnabled")]
		bool IsLivePhotoAutoTrimmingEnabled { [Bind ("isLivePhotoAutoTrimmingEnabled")] get; set; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'AVCapturePhoto.FileDataRepresentation' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCapturePhoto.FileDataRepresentation' instead.")]
		[Static]
		[Export ("JPEGPhotoDataRepresentationForJPEGSampleBuffer:previewPhotoSampleBuffer:")]
		[return: NullAllowed]
		NSData GetJpegPhotoDataRepresentation (CMSampleBuffer JPEGSampleBuffer, [NullAllowed] CMSampleBuffer previewPhotoSampleBuffer);

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'AVCapturePhoto.FileDataRepresentation' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCapturePhoto.FileDataRepresentation' instead.")]
		[Static]
		[Export ("DNGPhotoDataRepresentationForRawSampleBuffer:previewPhotoSampleBuffer:")]
		[return: NullAllowed]
		NSData GetDngPhotoDataRepresentation (CMSampleBuffer rawSampleBuffer, [NullAllowed] CMSampleBuffer previewPhotoSampleBuffer);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("preparedPhotoSettingsArray")]
		AVCapturePhotoSettings [] PreparedPhotoSettings { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setPreparedPhotoSettingsArray:completionHandler:")]
		[Async]
		void SetPreparedPhotoSettings (AVCapturePhotoSettings [] preparedPhotoSettingsArray, [NullAllowed] Action<bool, NSError> completionHandler);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'VirtualDeviceFusionSupported' instead.")]
		[NoMac]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'VirtualDeviceFusionSupported' instead.")]
		[Export ("dualCameraFusionSupported")]
		bool DualCameraFusionSupported { [Bind ("isDualCameraFusionSupported")] get; }

		// From AVCapturePhotoOutput (AVCapturePhotoOutputDepthDataDeliverySupport) Category

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("depthDataDeliverySupported")]
		bool DepthDataDeliverySupported { [Bind ("isDepthDataDeliverySupported")] get; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("depthDataDeliveryEnabled")]
		bool DepthDataDeliveryEnabled { [Bind ("isDepthDataDeliveryEnabled")] get; set; }

		[Internal]
		[MacCatalyst (14, 0)]
		[Export ("availablePhotoFileTypes")]
		NSString [] _GetAvailablePhotoFileTypes { get; }

		[MacCatalyst (14, 0)]
		[Wrap ("Array.ConvertAll (_GetAvailablePhotoFileTypes, s => AVFileTypesExtensions.GetValue (s))")]
		AVFileTypes [] GetAvailablePhotoFileTypes { get; }

		[Internal]
		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("availableRawPhotoFileTypes")]
		NSString [] _GetAvailableRawPhotoFileTypes { get; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Wrap ("Array.ConvertAll (_GetAvailableRawPhotoFileTypes, s => AVFileTypesExtensions.GetValue (s))")]
		AVFileTypes [] GetAvailableRawPhotoFileTypes { get; }

		[MacCatalyst (14, 0)]
		[Export ("supportedPhotoPixelFormatTypesForFileType:")]
		NSNumber [] GetSupportedPhotoPixelFormatTypesForFileType (string fileType);

		[Internal]
		[MacCatalyst (14, 0)]
		[Export ("supportedPhotoCodecTypesForFileType:")]
		NSString [] _GetSupportedPhotoCodecTypesForFileType (string fileType);

		[NoMac]
		[MacCatalyst (14, 0)]
		[Wrap ("Array.ConvertAll (_GetSupportedPhotoCodecTypesForFileType (fileType), s => AVVideoCodecTypeExtensions.GetValue (s))")]
		AVVideoCodecType [] GetSupportedPhotoCodecTypesForFileType (string fileType);

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("supportedRawPhotoPixelFormatTypesForFileType:")]
		NSNumber [] GetSupportedRawPhotoPixelFormatTypesForFileType (string fileType);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'VirtualDeviceConstituentPhotoDeliverySupported' instead.")]
		[NoMac]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'VirtualDeviceConstituentPhotoDeliverySupported' instead.")]
		[Export ("dualCameraDualPhotoDeliverySupported")]
		bool DualCameraDualPhotoDeliverySupported { [Bind ("isDualCameraDualPhotoDeliverySupported")] get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'VirtualDeviceConstituentPhotoDeliveryEnabledDevices' instead.")]
		[NoMac]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'VirtualDeviceConstituentPhotoDeliveryEnabledDevices' instead.")]
		[Export ("dualCameraDualPhotoDeliveryEnabled")]
		bool DualCameraDualPhotoDeliveryEnabled { [Bind ("isDualCameraDualPhotoDeliveryEnabled")] get; set; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("availableLivePhotoVideoCodecTypes")]
		string [] AvailableLivePhotoVideoCodecTypes { [return: BindAs (typeof (AVVideoCodecType []))] get; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("cameraCalibrationDataDeliverySupported")]
		bool CameraCalibrationDataDeliverySupported { [Bind ("isCameraCalibrationDataDeliverySupported")] get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("portraitEffectsMatteDeliverySupported")]
		bool PortraitEffectsMatteDeliverySupported { [Bind ("isPortraitEffectsMatteDeliverySupported")] get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("portraitEffectsMatteDeliveryEnabled")]
		bool PortraitEffectsMatteDeliveryEnabled { [Bind ("isPortraitEffectsMatteDeliveryEnabled")] get; set; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("autoRedEyeReductionSupported")]
		bool AutoRedEyeReductionSupported { [Bind ("isAutoRedEyeReductionSupported")] get; }

		[BindAs (typeof (AVSemanticSegmentationMatteType []))]
		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("availableSemanticSegmentationMatteTypes")]
		NSString [] AvailableSemanticSegmentationMatteTypes { get; }

		[BindAs (typeof (AVSemanticSegmentationMatteType []))]
		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("enabledSemanticSegmentationMatteTypes", ArgumentSemantic.Assign)]
		NSString [] EnabledSemanticSegmentationMatteTypes { get; set; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("maxPhotoQualityPrioritization", ArgumentSemantic.Assign)]
		AVCapturePhotoQualityPrioritization MaxPhotoQualityPrioritization { get; set; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("virtualDeviceFusionSupported")]
		bool VirtualDeviceFusionSupported { [Bind ("isVirtualDeviceFusionSupported")] get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("virtualDeviceConstituentPhotoDeliverySupported")]
		bool VirtualDeviceConstituentPhotoDeliverySupported { [Bind ("isVirtualDeviceConstituentPhotoDeliverySupported")] get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("virtualDeviceConstituentPhotoDeliveryEnabled")]
		bool VirtualDeviceConstituentPhotoDeliveryEnabled { [Bind ("isVirtualDeviceConstituentPhotoDeliveryEnabled")] get; set; }

		[Introduced (PlatformName.MacCatalyst, 14, 1)]
		[iOS (14, 1)]
		[NoMac]
		[Export ("contentAwareDistortionCorrectionSupported")]
		bool ContentAwareDistortionCorrectionSupported { [Bind ("isContentAwareDistortionCorrectionSupported")] get; }

		[Introduced (PlatformName.MacCatalyst, 14, 1)]
		[iOS (14, 1)]
		[NoMac]
		[Export ("contentAwareDistortionCorrectionEnabled")]
		bool ContentAwareDistortionCorrectionEnabled { [Bind ("isContentAwareDistortionCorrectionEnabled")] get; set; }

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Mac (13, 0)]
		[Export ("maxPhotoDimensions")]
		CMVideoDimensions MaxPhotoDimensions { get; set; }

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Mac (13, 0)]
		[Export ("preservesLivePhotoCaptureSuspendedOnSessionStop")]
		bool PreservesLivePhotoCaptureSuspendedOnSessionStop { get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[BaseType (typeof (AVCaptureFileOutput))]
	[NoTV]
	[NoWatch]
	interface AVCaptureMovieFileOutput {
		[NullAllowed] // by default this property is null
		[Export ("metadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] Metadata { get; set; }

		[Export ("movieFragmentInterval")]
		CMTime MovieFragmentInterval { get; set; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("recordsVideoOrientationAndMirroringChangesAsMetadataTrackForConnection:")]
		bool RecordsVideoOrientationAndMirroringChangesAsMetadataTrack (AVCaptureConnection connection);

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("setRecordsVideoOrientationAndMirroringChanges:asMetadataTrackForConnection:")]
		void SetRecordsVideoOrientationAndMirroringChanges (bool doRecordChanges, AVCaptureConnection connection);

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("availableVideoCodecTypes")]
		NSString [] AvailableVideoCodecTypes { get; }

		[MacCatalyst (14, 0)]
		[Export ("outputSettingsForConnection:")]
		NSDictionary GetOutputSettings (AVCaptureConnection connection);

		[MacCatalyst (14, 0)]
		[Export ("setOutputSettings:forConnection:")]
		void SetOutputSettings ([NullAllowed] NSDictionary outputSettings, AVCaptureConnection connection);

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("supportedOutputSettingsKeysForConnection:")]
		string [] GetSupportedOutputSettingsKeys (AVCaptureConnection connection);

		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("primaryConstituentDeviceSwitchingBehaviorForRecordingEnabled")]
		bool PrimaryConstituentDeviceSwitchingBehaviorForRecordingEnabled { [Bind ("isPrimaryConstituentDeviceSwitchingBehaviorForRecordingEnabled")] get; set; }

		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("setPrimaryConstituentDeviceSwitchingBehaviorForRecording:restrictedSwitchingBehaviorConditions:")]
		void SetPrimaryConstituentDeviceSwitchingBehaviorForRecording (AVCapturePrimaryConstituentDeviceSwitchingBehavior switchingBehavior, AVCapturePrimaryConstituentDeviceRestrictedSwitchingBehaviorConditions restrictedSwitchingBehaviorConditions);

		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("primaryConstituentDeviceSwitchingBehaviorForRecording")]
		AVCapturePrimaryConstituentDeviceSwitchingBehavior PrimaryConstituentDeviceSwitchingBehaviorForRecording { get; }

		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("primaryConstituentDeviceRestrictedSwitchingBehaviorConditionsForRecording")]
		AVCapturePrimaryConstituentDeviceRestrictedSwitchingBehaviorConditions PrimaryConstituentDeviceRestrictedSwitchingBehaviorConditionsForRecording { get; }
	}

	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCapturePhotoOutput' instead.")]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'AVCapturePhotoOutput' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'AVCapturePhotoOutput' instead.")]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureStillImageOutput {
		[Export ("availableImageDataCVPixelFormatTypes")]
		NSNumber [] AvailableImageDataCVPixelFormatTypes { get; }

		[Export ("availableImageDataCodecTypes")]
		string [] AvailableImageDataCodecTypes { get; }

		[Export ("outputSettings", ArgumentSemantic.Copy)]
		NSDictionary OutputSettings { get; set; }

		[Wrap ("OutputSettings")]
		AVVideoSettingsUncompressed UncompressedVideoSetting { get; set; }

		[Wrap ("OutputSettings")]
		AVVideoSettingsCompressed CompressedVideoSetting { get; set; }

		[Export ("captureStillImageAsynchronouslyFromConnection:completionHandler:")]
		[Async ("CaptureStillImageTaskAsync")]
		void CaptureStillImageAsynchronously (AVCaptureConnection connection, AVCaptureCompletionHandler completionHandler);

		[Static, Export ("jpegStillImageNSDataRepresentation:")]
		[return: NullAllowed]
		NSData JpegStillToNSData (CMSampleBuffer buffer);

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		// 5.0
		[Export ("capturingStillImage")]
		bool CapturingStillImage { [Bind ("isCapturingStillImage")] get; }

		[NoMac]
		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("automaticallyEnablesStillImageStabilizationWhenAvailable")]
		bool AutomaticallyEnablesStillImageStabilizationWhenAvailable { get; set; }

		[NoMac]
		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("stillImageStabilizationActive")]
		bool IsStillImageStabilizationActive { [Bind ("isStillImageStabilizationActive")] get; }

		[NoMac]
		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("stillImageStabilizationSupported")]
		bool IsStillImageStabilizationSupported { [Bind ("isStillImageStabilizationSupported")] get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("captureStillImageBracketAsynchronouslyFromConnection:withSettingsArray:completionHandler:")]
		void CaptureStillImageBracket (AVCaptureConnection connection, AVCaptureBracketedStillImageSettings [] settings, Action<CMSampleBuffer, AVCaptureBracketedStillImageSettings, NSError> imageHandler);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("maxBracketedCaptureStillImageCount")]
		nuint MaxBracketedCaptureStillImageCount { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("prepareToCaptureStillImageBracketFromConnection:withSettingsArray:completionHandler:")]
		void PrepareToCaptureStillImageBracket (AVCaptureConnection connection, AVCaptureBracketedStillImageSettings [] settings, Action<bool, NSError> handler);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("lensStabilizationDuringBracketedCaptureSupported")]
		bool LensStabilizationDuringBracketedCaptureSupported { [Bind ("isLensStabilizationDuringBracketedCaptureSupported")] get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("lensStabilizationDuringBracketedCaptureEnabled")]
		bool LensStabilizationDuringBracketedCaptureEnabled { [Bind ("isLensStabilizationDuringBracketedCaptureEnabled")] get; set; }

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("highResolutionStillImageOutputEnabled")]
		bool HighResolutionStillImageOutputEnabled { [Bind ("isHighResolutionStillImageOutputEnabled")] get; set; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV, NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // init NS_UNAVAILABLE
	interface AVCaptureDeviceDiscoverySession {

#if !NET
		[Internal]
		[Static]
		[Export ("discoverySessionWithDeviceTypes:mediaType:position:")]
		AVCaptureDeviceDiscoverySession _Create (NSArray deviceTypes, [NullAllowed] string mediaType, AVCaptureDevicePosition position);
#else
		[Static]
		[Export ("discoverySessionWithDeviceTypes:mediaType:position:")]
		AVCaptureDeviceDiscoverySession Create ([BindAs (typeof (AVCaptureDeviceType[]))] NSString[] deviceTypes, [NullAllowed] [BindAs (typeof (AVMediaTypes))] NSString mediaType, AVCaptureDevicePosition position);
#endif

		[Export ("devices")]
		AVCaptureDevice [] Devices { get; }

		[NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("supportedMultiCamDeviceSets")]
		NSSet<AVCaptureDevice> [] SupportedMultiCamDeviceSets { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV, NoWatch]
	enum AVCaptureDeviceType {

		[Field ("AVCaptureDeviceTypeBuiltInMicrophone")]
		BuiltInMicrophone,

		[Field ("AVCaptureDeviceTypeBuiltInWideAngleCamera")]
		BuiltInWideAngleCamera,

		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVCaptureDeviceTypeBuiltInTelephotoCamera")]
		BuiltInTelephotoCamera,

		[NoMac]
		[Deprecated (PlatformName.iOS, 10, 2, message: "Use 'BuiltInDualCamera' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'BuiltInDualCamera' instead.")]
		[Field ("AVCaptureDeviceTypeBuiltInDuoCamera")]
		BuiltInDuoCamera,

		[NoMac]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureDeviceTypeBuiltInDualCamera")]
		BuiltInDualCamera,

		[iOS (11, 1), NoMac]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureDeviceTypeBuiltInTrueDepthCamera")]
		BuiltInTrueDepthCamera,

		[NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureDeviceTypeBuiltInUltraWideCamera")]
		BuiltInUltraWideCamera,

		[NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureDeviceTypeBuiltInTripleCamera")]
		BuiltInTripleCamera,

		[NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureDeviceTypeBuiltInDualWideCamera")]
		BuiltInDualWideCamera,

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("AVCaptureDeviceTypeExternalUnknown")]
		ExternalUnknown,

		[NoWatch, NoTV, NoMac, MacCatalyst (15, 4), iOS (15, 4)]
		[Field ("AVCaptureDeviceTypeBuiltInLiDARDepthCamera")]
		BuiltInLiDarDepthCamera,
	}

	[NoTV, NoWatch] // matches API that uses it.
	[MacCatalyst (13, 1)]
	enum AVAuthorizationMediaType {
		Video,
		Audio,
	}

#if WATCH
	[Static]
#endif
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV, Watch (6, 0)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: Cannot instantiate a AVCaptureDevice directly.
	[DisableDefaultCtor]
	interface AVCaptureDevice {
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("uniqueID")]
		string UniqueID { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("modelID")]
		string ModelID { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("localizedName")]
		string LocalizedName { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("connected")]
		bool Connected { [Bind ("isConnected")] get; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'AVCaptureDeviceDiscoverySession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'AVCaptureDeviceDiscoverySession' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCaptureDeviceDiscoverySession' instead.")]
		[Static, Export ("devices")]
		AVCaptureDevice [] Devices { get; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'AVCaptureDeviceDiscoverySession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'AVCaptureDeviceDiscoverySession' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCaptureDeviceDiscoverySession' instead.")]
		[Static]
		[Export ("devicesWithMediaType:")]
		AVCaptureDevice [] DevicesWithMediaType (string mediaType);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("defaultDeviceWithMediaType:")]
		[return: NullAllowed]
		AVCaptureDevice GetDefaultDevice (NSString mediaType);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetDefaultDevice (mediaType.GetConstant ()!)")]
		[return: NullAllowed]
		AVCaptureDevice GetDefaultDevice (AVMediaTypes mediaType);

#if !NET
		[NoWatch]
		[Obsolete ("Use 'GetDefaultDevice (AVMediaTypes)'.")]
		[Static]
		[Wrap ("GetDefaultDevice ((NSString) mediaType)")]
		[return: NullAllowed]
		AVCaptureDevice DefaultDeviceWithMediaType (string mediaType);
#endif

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("deviceWithUniqueID:")]
		[return: NullAllowed]
		AVCaptureDevice DeviceWithUniqueID (string deviceUniqueID);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("hasMediaType:")]
		bool HasMediaType (string mediaType);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Wrap ("HasMediaType ((string) mediaType.GetConstant ())")]
		bool HasMediaType (AVMediaTypes mediaType);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("lockForConfiguration:")]
		bool LockForConfiguration (out NSError error);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("unlockForConfiguration")]
		void UnlockForConfiguration ();

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("supportsAVCaptureSessionPreset:")]
		bool SupportsAVCaptureSessionPreset (string preset);

		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'AVCapturePhotoSettings.FlashMode' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCapturePhotoSettings.FlashMode' instead.")]
		[Export ("flashMode")]
		AVCaptureFlashMode FlashMode { get; set; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'AVCapturePhotoOutput.SupportedFlashModes' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCapturePhotoOutput.SupportedFlashModes' instead.")]
		[Export ("isFlashModeSupported:")]
		bool IsFlashModeSupported (AVCaptureFlashMode flashMode);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("torchMode", ArgumentSemantic.Assign)]
		AVCaptureTorchMode TorchMode { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("isTorchModeSupported:")]
		bool IsTorchModeSupported (AVCaptureTorchMode torchMode);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("isFocusModeSupported:")]
		bool IsFocusModeSupported (AVCaptureFocusMode focusMode);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("focusMode", ArgumentSemantic.Assign)]
		AVCaptureFocusMode FocusMode { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("focusPointOfInterestSupported")]
		bool FocusPointOfInterestSupported { [Bind ("isFocusPointOfInterestSupported")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("focusPointOfInterest", ArgumentSemantic.Assign)]
		CGPoint FocusPointOfInterest { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("adjustingFocus")]
		bool AdjustingFocus { [Bind ("isAdjustingFocus")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("exposureMode", ArgumentSemantic.Assign)]
		AVCaptureExposureMode ExposureMode { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("isExposureModeSupported:")]
		bool IsExposureModeSupported (AVCaptureExposureMode exposureMode);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("exposurePointOfInterestSupported")]
		bool ExposurePointOfInterestSupported { [Bind ("isExposurePointOfInterestSupported")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("exposurePointOfInterest")]
		CGPoint ExposurePointOfInterest { get; set; }

		[NoWatch]
		[NoTV, NoMac, MacCatalyst (15, 4), iOS (15, 4)]
		[Export ("automaticallyAdjustsFaceDrivenAutoExposureEnabled")]
		bool AutomaticallyAdjustsFaceDrivenAutoExposureEnabled { get; set; }

		[NoWatch]
		[NoTV, NoMac, MacCatalyst (15, 4), iOS (15, 4)]
		[Export ("faceDrivenAutoExposureEnabled")]
		bool FaceDrivenAutoExposureEnabled { [Bind ("isFaceDrivenAutoExposureEnabled")] get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("adjustingExposure")]
		bool AdjustingExposure { [Bind ("isAdjustingExposure")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("isWhiteBalanceModeSupported:")]
		bool IsWhiteBalanceModeSupported (AVCaptureWhiteBalanceMode whiteBalanceMode);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("whiteBalanceMode", ArgumentSemantic.Assign)]
		AVCaptureWhiteBalanceMode WhiteBalanceMode { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("adjustingWhiteBalance")]
		bool AdjustingWhiteBalance { [Bind ("isAdjustingWhiteBalance")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("position")]
		AVCaptureDevicePosition Position { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVCaptureDeviceWasConnectedNotification")]
		[Notification]
		NSString WasConnectedNotification { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVCaptureDeviceWasDisconnectedNotification")]
		[Notification]
		NSString WasDisconnectedNotification { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("AVCaptureMaxAvailableTorchLevel")]
		float MaxAvailableTorchLevel { get; } // defined as 'float'

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Field ("AVCaptureDeviceSubjectAreaDidChangeNotification")]
		[Notification]
		NSString SubjectAreaDidChangeNotification { get; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("subjectAreaChangeMonitoringEnabled")]
		bool SubjectAreaChangeMonitoringEnabled { [Bind ("isSubjectAreaChangeMonitoringEnabled")] get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("isFlashAvailable")]
		bool FlashAvailable { get; }

		[NoWatch, NoMac]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'AVCapturePhotoOutput.IsFlashScene' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVCapturePhotoOutput.IsFlashScene' instead.")]
		[Export ("isFlashActive")]
		bool FlashActive { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("isTorchAvailable")]
		bool TorchAvailable { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("torchLevel")]
		float TorchLevel { get; } // defined as 'float'

		// 6.0
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("torchActive")]
		bool TorchActive { [Bind ("isTorchActive")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("setTorchModeOnWithLevel:error:")]
		bool SetTorchModeLevel (float /* defined as 'float' */ torchLevel, out NSError outError);

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("lowLightBoostSupported")]
		bool LowLightBoostSupported { [Bind ("isLowLightBoostSupported")] get; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("lowLightBoostEnabled")]
		bool LowLightBoostEnabled { [Bind ("isLowLightBoostEnabled")] get; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("automaticallyEnablesLowLightBoostWhenAvailable")]
		bool AutomaticallyEnablesLowLightBoostWhenAvailable { get; set; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("videoZoomFactor")]
		nfloat VideoZoomFactor { get; set; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("rampToVideoZoomFactor:withRate:")]
		void RampToVideoZoom (nfloat factor, float /* float, not CGFloat */ rate);

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("rampingVideoZoom")]
		bool RampingVideoZoom { [Bind ("isRampingVideoZoom")] get; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("cancelVideoZoomRamp")]
		void CancelVideoZoomRamp ();

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("autoFocusRangeRestrictionSupported")]
		bool AutoFocusRangeRestrictionSupported { [Bind ("isAutoFocusRangeRestrictionSupported")] get; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("autoFocusRangeRestriction")]
		AVCaptureAutoFocusRangeRestriction AutoFocusRangeRestriction { get; set; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("smoothAutoFocusSupported")]
		bool SmoothAutoFocusSupported { [Bind ("isSmoothAutoFocusSupported")] get; }

		[NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("smoothAutoFocusEnabled")]
		bool SmoothAutoFocusEnabled { [Bind ("isSmoothAutoFocusEnabled")] get; set; }

		[NoWatch, NoTV, NoMac, MacCatalyst (15, 4), iOS (15, 4)]
		[Export ("automaticallyAdjustsFaceDrivenAutoFocusEnabled")]
		bool AutomaticallyAdjustsFaceDrivenAutoFocusEnabled { get; set; }

		[NoWatch, NoTV, NoMac, MacCatalyst (15, 4), iOS (15, 4)]
		[Export ("faceDrivenAutoFocusEnabled")]
		bool FaceDrivenAutoFocusEnabled { [Bind ("isFaceDrivenAutoFocusEnabled")] get; set; }

		// Either AVMediaTypeVideo or AVMediaTypeAudio.
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("RequestAccessForMediaType (mediaType == AVAuthorizationMediaType.Video ? AVMediaTypes.Video.GetConstant ()! : AVMediaTypes.Audio.GetConstant ()!, completion)")]
		[Async]
		void RequestAccessForMediaType (AVAuthorizationMediaType mediaType, AVRequestAccessStatus completion);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static, Export ("requestAccessForMediaType:completionHandler:")]
		[Async]
		void RequestAccessForMediaType (NSString avMediaTypeToken, AVRequestAccessStatus completion);

		// Calling this method with any media type other than AVMediaTypeVideo or AVMediaTypeAudio raises an exception.
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetAuthorizationStatus (mediaType == AVAuthorizationMediaType.Video ? AVMediaTypes.Video.GetConstant ()! : AVMediaTypes.Audio.GetConstant ()!)")]
		AVAuthorizationStatus GetAuthorizationStatus (AVAuthorizationMediaType mediaType);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static, Export ("authorizationStatusForMediaType:")]
		AVAuthorizationStatus GetAuthorizationStatus (NSString avMediaTypeToken);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("activeFormat", ArgumentSemantic.Retain)]
		AVCaptureDeviceFormat ActiveFormat { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("formats")]
		AVCaptureDeviceFormat [] Formats { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("hasFlash")]
		bool HasFlash { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("hasTorch")]
		bool HasTorch { get; }

		[NoiOS, NoWatch, MacCatalyst (15, 0)]
		[Export ("inUseByAnotherApplication")]
		bool InUseByAnotherApplication { [Bind ("isInUseByAnotherApplication")] get; }

		[iOS (14, 0), NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("suspended")]
		bool Suspended { [Bind ("isSuspended")] get; }

		[NoiOS, NoWatch, MacCatalyst (15, 0)]
		[Export ("linkedDevices")]
		AVCaptureDevice [] LinkedDevices { get; }

		[iOS (14, 0), NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("manufacturer")]
		string Manufacturer { get; }

		[NoiOS, NoWatch, NoMacCatalyst]
		[Export ("transportControlsSpeed")]
		float TransportControlsSpeed { get; } // float intended

		[NoiOS, NoWatch, NoMacCatalyst]
		[Export ("transportControlsSupported")]
		bool TransportControlsSupported { get; }

		[NoWatch]
		[NoiOS] // TODO: We can provide a better binding once IOKit is bound kIOAudioDeviceTransportType*
		[MacCatalyst (15, 0)]
		[Export ("transportType")]
		int WeakTransportType { get; } // int intended

		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[NullAllowed, Export ("activeInputSource", ArgumentSemantic.Retain)]
		AVCaptureDeviceInputSource ActiveInputSource { get; set; }

		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[Export ("inputSources")]
		AVCaptureDeviceInputSource [] InputSources { get; }

		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[Export ("setTransportControlsPlaybackMode:speed:")]
		void SetTransportControlsPlaybackMode (AVCaptureDeviceTransportControlsPlaybackMode mode, float speed); // Float intended

		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[Export ("transportControlsPlaybackMode")]
		AVCaptureDeviceTransportControlsPlaybackMode TransportControlsPlaybackMode { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("activeVideoMinFrameDuration", ArgumentSemantic.Copy)]
		CMTime ActiveVideoMinFrameDuration { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("activeVideoMaxFrameDuration", ArgumentSemantic.Copy)]
		CMTime ActiveVideoMaxFrameDuration { get; set; }

		[NoMac, NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("lockingFocusWithCustomLensPositionSupported")]
		bool LockingFocusWithCustomLensPositionSupported { [Bind ("isLockingFocusWithCustomLensPositionSupported")] get; }

		[NoMac, NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("lockingWhiteBalanceWithCustomDeviceGainsSupported")]
		bool LockingWhiteBalanceWithCustomDeviceGainsSupported { [Bind ("isLockingWhiteBalanceWithCustomDeviceGainsSupported")] get; }

		// From AVCaptureDevice (AVCaptureDeviceType) Category
		[Internal]
		[NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("deviceType")]
		NSString _DeviceType { get; }

		[NoWatch]
		[MacCatalyst (14, 0)]
		[Wrap ("AVCaptureDeviceTypeExtensions.GetValue (_DeviceType)")]
		AVCaptureDeviceType DeviceType { get; }

		[Internal]
		[NoWatch]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("defaultDeviceWithDeviceType:mediaType:position:")]
		AVCaptureDevice _DefaultDeviceWithDeviceType (NSString deviceType, string mediaType, AVCaptureDevicePosition position);

		[NoWatch]
		[MacCatalyst (14, 0)]
		[Static]
		[Wrap ("AVCaptureDevice._DefaultDeviceWithDeviceType (deviceType.GetConstant ()!, mediaType, position)")]
		AVCaptureDevice GetDefaultDevice (AVCaptureDeviceType deviceType, string mediaType, AVCaptureDevicePosition position);

		//
		// iOS 8
		//
		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureLensPositionCurrent")]
		float FocusModeLensPositionCurrent { get; } /* float, not CGFloat */

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("lensAperture")]
		float LensAperture { get; } /* float, not CGFloat */

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("exposureDuration")]
		CMTime ExposureDuration { get; }

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("ISO")]
		float ISO { get; } /* float, not CGFloat */

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("exposureTargetOffset")]
		float ExposureTargetOffset { get; } /* float, not CGFloat */

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("exposureTargetBias")]
		float ExposureTargetBias { get; } /* float, not CGFloat */

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("minExposureTargetBias")]
		float MinExposureTargetBias { get; } /* float, not CGFloat */

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("maxExposureTargetBias")]
		float MaxExposureTargetBias { get; } /* float, not CGFloat */

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("setExposureModeCustomWithDuration:ISO:completionHandler:")]
		[Async]
		void LockExposure (CMTime duration, float /* float, not CGFloat */ ISO, [NullAllowed] Action<CMTime> completionHandler);

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("setExposureTargetBias:completionHandler:")]
		[Async]
		void SetExposureTargetBias (float /* float, not CGFloat */ bias, [NullAllowed] Action<CMTime> completionHandler);

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("lensPosition")]
		float LensPosition { get; } /* float, not CGFloat */

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("setFocusModeLockedWithLensPosition:completionHandler:")]
		[Async]
		void SetFocusModeLocked (float /* float, not CGFloat */ lensPosition, [NullAllowed] Action<CMTime> completionHandler);

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("deviceWhiteBalanceGains")]
		AVCaptureWhiteBalanceGains DeviceWhiteBalanceGains { get; }

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("grayWorldDeviceWhiteBalanceGains")]
		AVCaptureWhiteBalanceGains GrayWorldDeviceWhiteBalanceGains { get; }

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("maxWhiteBalanceGain")]
		float MaxWhiteBalanceGain { get; } /* float, not CGFloat */

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("setWhiteBalanceModeLockedWithDeviceWhiteBalanceGains:completionHandler:")]
		[Async]
		void SetWhiteBalanceModeLockedWithDeviceWhiteBalanceGains (AVCaptureWhiteBalanceGains whiteBalanceGains, [NullAllowed] Action<CMTime> completionHandler);

		[NoMac, NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("chromaticityValuesForDeviceWhiteBalanceGains:")]
		AVCaptureWhiteBalanceChromaticityValues GetChromaticityValues (AVCaptureWhiteBalanceGains whiteBalanceGains);

		[NoMac, NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("deviceWhiteBalanceGainsForChromaticityValues:")]
		AVCaptureWhiteBalanceGains GetDeviceWhiteBalanceGains (AVCaptureWhiteBalanceChromaticityValues chromaticityValues);

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("temperatureAndTintValuesForDeviceWhiteBalanceGains:")]
		AVCaptureWhiteBalanceTemperatureAndTintValues GetTemperatureAndTintValues (AVCaptureWhiteBalanceGains whiteBalanceGains);

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("deviceWhiteBalanceGainsForTemperatureAndTintValues:")]
		AVCaptureWhiteBalanceGains GetDeviceWhiteBalanceGains (AVCaptureWhiteBalanceTemperatureAndTintValues tempAndTintValues);

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureExposureDurationCurrent")]
		CMTime ExposureDurationCurrent { get; }

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureExposureTargetBiasCurrent")]
		float ExposureTargetBiasCurrent { get; } /* float, not CGFloat */

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureISOCurrent")]
		float ISOCurrent { get; } /* float, not CGFloat */

		[NoMac]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureLensPositionCurrent")]
		float LensPositionCurrent { get; } /* float, not CGFloat */

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Field ("AVCaptureWhiteBalanceGainsCurrent")]
		AVCaptureWhiteBalanceGains WhiteBalanceGainsCurrent { get; }

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("automaticallyAdjustsVideoHDREnabled")]
		bool AutomaticallyAdjustsVideoHdrEnabled { get; set; }

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("videoHDREnabled")]
		bool VideoHdrEnabled { [Bind ("isVideoHDREnabled")] get; set; }

		[NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("activeColorSpace", ArgumentSemantic.Assign)]
		AVCaptureColorSpace ActiveColorSpace { get; set; }

		// From AVCaptureDevice (AVCaptureDeviceDepthSupport) Category

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("activeDepthDataFormat", ArgumentSemantic.Retain), NullAllowed]
		AVCaptureDeviceFormat ActiveDepthDataFormat { get; set; }

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("minAvailableVideoZoomFactor")]
		nfloat MinAvailableVideoZoomFactor { get; }

		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Export ("maxAvailableVideoZoomFactor")]
		nfloat MaxAvailableVideoZoomFactor { get; }

		// From  AVCaptureDevice (AVCaptureDeviceSystemPressure) Category
		[NoWatch, NoTV, NoMac, iOS (11, 1)]
		[MacCatalyst (14, 0)]
		[Export ("systemPressureState")]
		AVCaptureSystemPressureState SystemPressureState { get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'VirtualDeviceSwitchOverVideoZoomFactors' instead.")]
		[NoWatch, NoMac]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'VirtualDeviceSwitchOverVideoZoomFactors' instead.")]
		[Export ("dualCameraSwitchOverVideoZoomFactor")]
		nfloat DualCameraSwitchOverVideoZoomFactor { get; }

		// From @interface AVCaptureDeviceDepthSupport (AVCaptureDevice)

		[NoMac, iOS (12, 0), NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("activeDepthDataMinFrameDuration", ArgumentSemantic.Assign)]
		CMTime ActiveDepthDataMinFrameDuration { get; set; }

		// From @interface AVCaptureDeviceExposure (AVCaptureDevice)
		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("activeMaxExposureDuration", ArgumentSemantic.Assign)]
		CMTime ActiveMaxExposureDuration { get; set; }

		[NoTV, NoWatch]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("defaultDeviceWithDeviceType:mediaType:position:")]
		[return: NullAllowed]
		AVCaptureDevice GetDefaultDevice ([BindAs (typeof (AVCaptureDeviceType))] NSString deviceType, [NullAllowed][BindAs (typeof (AVMediaTypes))] NSString mediaType, AVCaptureDevicePosition position);

		// From AVCaptureDevice_AVCaptureDeviceVirtual
		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("virtualDevice")]
		bool VirtualDevice { [Bind ("isVirtualDevice")] get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("constituentDevices")]
		AVCaptureDevice [] ConstituentDevices { get; }

		// from AVCaptureDevice_AVCaptureDeviceCalibration
		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("extrinsicMatrixFromDevice:toDevice:")]
		[return: NullAllowed]
		NSData GetExtrinsicMatrix (AVCaptureDevice fromDevice, AVCaptureDevice toDevice);

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("globalToneMappingEnabled")]
		bool GlobalToneMappingEnabled { [Bind ("isGlobalToneMappingEnabled")] get; set; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("virtualDeviceSwitchOverVideoZoomFactors")]
		NSNumber [] VirtualDeviceSwitchOverVideoZoomFactors { get; }

		// From AVCaptureDevice_AVCaptureDeviceVirtual

		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("setPrimaryConstituentDeviceSwitchingBehavior:restrictedSwitchingBehaviorConditions:")]
		void SetPrimaryConstituentDeviceSwitchingBehavior (AVCapturePrimaryConstituentDeviceSwitchingBehavior switchingBehavior, AVCapturePrimaryConstituentDeviceRestrictedSwitchingBehaviorConditions restrictedSwitchingBehaviorConditions);

		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("primaryConstituentDeviceSwitchingBehavior")]
		AVCapturePrimaryConstituentDeviceSwitchingBehavior PrimaryConstituentDeviceSwitchingBehavior { get; }

		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("primaryConstituentDeviceRestrictedSwitchingBehaviorConditions")]
		AVCapturePrimaryConstituentDeviceRestrictedSwitchingBehaviorConditions PrimaryConstituentDeviceRestrictedSwitchingBehaviorConditions { get; }

		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("activePrimaryConstituentDeviceSwitchingBehavior")]
		AVCapturePrimaryConstituentDeviceSwitchingBehavior ActivePrimaryConstituentDeviceSwitchingBehavior { get; }

		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("activePrimaryConstituentDeviceRestrictedSwitchingBehaviorConditions")]
		AVCapturePrimaryConstituentDeviceRestrictedSwitchingBehaviorConditions ActivePrimaryConstituentDeviceRestrictedSwitchingBehaviorConditions { get; }

		[NullAllowed]
		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("activePrimaryConstituentDevice")]
		AVCaptureDevice ActivePrimaryConstituentDevice { get; }

		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("supportedFallbackPrimaryConstituentDevices")]
		AVCaptureDevice [] SupportedFallbackPrimaryConstituentDevices { get; }

		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("fallbackPrimaryConstituentDevices", ArgumentSemantic.Assign)]
		AVCaptureDevice [] FallbackPrimaryConstituentDevices { get; set; }

		// from AVCaptureDevice_AVCaptureDeviceGeometricDistortionCorrection

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("geometricDistortionCorrectionSupported")]
		bool GeometricDistortionCorrectionSupported { [Bind ("isGeometricDistortionCorrectionSupported")] get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("geometricDistortionCorrectionEnabled")]
		bool GeometricDistortionCorrectionEnabled { [Bind ("isGeometricDistortionCorrectionEnabled")] get; set; }

		// from AVCaptureDevice_AVCaptureDeviceCenterStage

		[NoWatch, NoTV]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Mac (12, 3)]
		[Static]
		[Export ("centerStageControlMode", ArgumentSemantic.Assign)]
		AVCaptureCenterStageControlMode CenterStageControlMode { get; set; }

		[NoWatch, NoTV]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Mac (12, 3)]
		[Static]
		[Export ("centerStageEnabled")]
		bool CenterStageEnabled { [Bind ("isCenterStageEnabled")] get; set; }

		[NoWatch, NoTV]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Mac (12, 3)]
		[Export ("centerStageActive")]
		bool CenterStageActive { [Bind ("isCenterStageActive")] get; }

		// AVCaptureDevice_AVCaptureMicrophoneMode
		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("preferredMicrophoneMode")]
		AVCaptureMicrophoneMode PreferredMicrophoneMode { get; }

		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("activeMicrophoneMode")]
		AVCaptureMicrophoneMode ActiveMicrophoneMode { get; }

		// AVCaptureDevice_AVCaptureDevicePortraitEffect
		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("portraitEffectEnabled")]
		bool PortraitEffectEnabled { [Bind ("isPortraitEffectEnabled")] get; }

		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("portraitEffectActive")]
		bool PortraitEffectActive { [Bind ("isPortraitEffectActive")] get; }

		// AVCaptureDevice_AVCaptureSystemUserInterface
		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("showSystemUserInterface:")]
		void ShowSystemUserInterface (AVCaptureSystemUserInterface systemUserInterface);

		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("minimumFocusDistance")]
		nint MinimumFocusDistance { get; }

	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV, iOS (11, 1), NoMac, NoWatch]
	enum AVCaptureSystemPressureLevel {
		[Field ("AVCaptureSystemPressureLevelNominal")]
		Nominal,

		[Field ("AVCaptureSystemPressureLevelFair")]
		Fair,

		[Field ("AVCaptureSystemPressureLevelSerious")]
		Serious,

		[Field ("AVCaptureSystemPressureLevelCritical")]
		Critical,

		[Field ("AVCaptureSystemPressureLevelShutdown")]
		Shutdown,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, NoMac, iOS (11, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureSystemPressureState {
		[Internal]
		[Export ("level")]
		NSString _Level { get; }

		[Wrap ("AVCaptureSystemPressureLevelExtensions.GetValue (_Level)")]
		AVCaptureSystemPressureLevel Level { get; }

		[Export ("factors")]
		AVCaptureSystemPressureFactors Factors { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[DisableDefaultCtor] // crash -> immutable, it can be set but it should be selected from tha available formats (not a custom one)
	[BaseType (typeof (NSObject))]
	interface AVCaptureDeviceFormat {
		[Export ("mediaType", ArgumentSemantic.Copy)]
		NSString MediaType { get; }

		[Export ("formatDescription", ArgumentSemantic.Copy)]
		CMFormatDescription FormatDescription { get; }

		[Export ("videoSupportedFrameRateRanges", ArgumentSemantic.Copy)]
		AVFrameRateRange [] VideoSupportedFrameRateRanges { get; }

		[MacCatalyst (14, 0)]
		[Export ("supportedColorSpaces")]
#if NET
		[BindAs (typeof (AVCaptureColorSpace []))]
#endif
		NSNumber [] SupportedColorSpaces { get; }

		[MacCatalyst (14, 0)]
		[Export ("autoFocusSystem")]
		AVCaptureAutoFocusSystem AutoFocusSystem { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("videoFieldOfView")]
		float VideoFieldOfView { get; } // defined as 'float'

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("videoBinned")]
		bool VideoBinned { [Bind ("isVideoBinned")] get; }

		[NoMac]
		[Export ("videoStabilizationSupported")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'IsVideoStabilizationModeSupported (AVCaptureVideoStabilizationMode)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'IsVideoStabilizationModeSupported (AVCaptureVideoStabilizationMode)' instead.")]
		bool VideoStabilizationSupported { [Bind ("isVideoStabilizationSupported")] get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("videoMaxZoomFactor")]
		nfloat VideoMaxZoomFactor { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("videoZoomFactorUpscaleThreshold")]
		nfloat VideoZoomFactorUpscaleThreshold { get; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("minExposureDuration")]
		CMTime MinExposureDuration { get; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("maxExposureDuration")]
		CMTime MaxExposureDuration { get; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("minISO")]
		float MinISO { get; } /* float, not CGFloat */

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("maxISO")]
		float MaxISO { get; } /* float, not CGFloat */

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("isVideoStabilizationModeSupported:")]
		bool IsVideoStabilizationModeSupported (AVCaptureVideoStabilizationMode mode);

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("videoHDRSupported")]
		bool IsVideoHdrSupported { [Bind ("isVideoHDRSupported")] get; }

#if !XAMCORE_5_0
		[NoMac]
		[MacCatalyst (14, 0)]
		[Obsolete ("Use the 'IsVideoHdrSupported' property instead.")]
		[Wrap ("IsVideoHdrSupported", IsVirtual = true)]
		bool videoHDRSupportedVideoHDREnabled { [Bind ("isVideoHDRSupported")] get; }
#endif

		[NoMac]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.iOS, 16, 0, message: "Use 'SupportedMaxPhotoDimension' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use 'SupportedMaxPhotoDimension' instead.")]
		[Export ("highResolutionStillImageDimensions")]
		CMVideoDimensions HighResolutionStillImageDimensions { get; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.iOS, 16, 0, message: "Use 'SupportedVideoZoomFactorsForDepthDataDelivery' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use 'SupportedVideoZoomFactorsForDepthDataDelivery' instead.")]
		[Export ("videoMinZoomFactorForDepthDataDelivery")]
		nfloat VideoMinZoomFactorForDepthDataDelivery { get; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.iOS, 16, 0, message: "Use 'SupportedVideoZoomFactorsForDepthDataDelivery' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use 'SupportedVideoZoomFactorsForDepthDataDelivery' instead.")]
		[Export ("videoMaxZoomFactorForDepthDataDelivery")]
		nfloat VideoMaxZoomFactorForDepthDataDelivery { get; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("supportedDepthDataFormats")]
		AVCaptureDeviceFormat [] SupportedDepthDataFormats { get; }

		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("unsupportedCaptureOutputClasses")]
		Class [] UnsupportedCaptureOutputClasses { get; }

		// from @interface AVCaptureDeviceFormatDepthDataAdditions (AVCaptureDeviceFormat)
		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("portraitEffectsMatteStillImageDeliverySupported")]
		bool PortraitEffectsMatteStillImageDeliverySupported { [Bind ("isPortraitEffectsMatteStillImageDeliverySupported")] get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("highestPhotoQualitySupported")]
		bool HighestPhotoQualitySupported { [Bind ("isHighestPhotoQualitySupported")] get; }

		// from AVCaptureDeviceFormat_AVCaptureDeviceFormatMultiCamAdditions 
		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("multiCamSupported")]
		bool MultiCamSupported { [Bind ("isMultiCamSupported")] get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("globalToneMappingSupported")]
		bool GlobalToneMappingSupported { [Bind ("isGlobalToneMappingSupported")] get; }

		// from AVCaptureDeviceFormat_AVCaptureDeviceFormatGeometricDistortionCorrection 
		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("geometricDistortionCorrectedVideoFieldOfView")]
		float GeometricDistortionCorrectedVideoFieldOfView { get; }

		// from AVCaptureDeviceFormat_AVCaptureDeviceFormatCenterStage

		[NoWatch, NoTV]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Mac (12, 3)]
		[Export ("centerStageSupported")]
		bool CenterStageSupported { [Bind ("isCenterStageSupported")] get; }

		[NoWatch, NoTV]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Mac (12, 3)]
		[Export ("videoMinZoomFactorForCenterStage")]
		nfloat VideoMinZoomFactorForCenterStage { get; }

		[NoWatch, NoTV]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Mac (12, 3)]
		[Export ("videoMaxZoomFactorForCenterStage")]
		nfloat VideoMaxZoomFactorForCenterStage { get; }

		[NoWatch, NoTV]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Mac (12, 3)]
		[Export ("videoFrameRateRangeForCenterStage")]
		[NullAllowed]
		AVFrameRateRange VideoFrameRateRangeForCenterStage { get; }

		[NoWatch, NoTV, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("highPhotoQualitySupported")]
		bool HighPhotoQualitySupported { [Bind ("isHighPhotoQualitySupported")] get; }

		// AVCaptureDeviceFormat_AVCaptureDeviceFormatPortraitEffect
		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("portraitEffectSupported")]
		bool PortraitEffectSupported { [Bind ("isPortraitEffectSupported")] get; }

		[NullAllowed]
		[NoWatch, NoTV, MacCatalyst (15, 0), Mac (12, 0), iOS (15, 0)]
		[Export ("videoFrameRateRangeForPortraitEffect")]
		AVFrameRateRange VideoFrameRateRangeForPortraitEffect { get; }

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Mac (13, 0)]
		[BindAs (typeof (CMVideoDimensions []))]
		[Export ("supportedMaxPhotoDimensions")]
		NSValue [] SupportedMaxPhotoDimensions { get; }

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Mac (13, 0)]
		[BindAs (typeof (nfloat []))]
		[Export ("secondaryNativeResolutionZoomFactors")]
		NSNumber [] SecondaryNativeResolutionZoomFactors { get; }

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Mac (13, 0)]
		[BindAs (typeof (nfloat []))]
		[Export ("supportedVideoZoomFactorsForDepthDataDelivery")]
		NSNumber [] SupportedVideoZoomFactorsForDepthDataDelivery { get; }
	}

	delegate void AVCaptureCompletionHandler (CMSampleBuffer imageDataSampleBuffer, NSError error);

	[NoWatch, TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	enum AVPlayerRateDidChangeReason {

		[Field ("AVPlayerRateDidChangeReasonSetRateCalled")]
		SetRateCalled,

		[Field ("AVPlayerRateDidChangeReasonSetRateFailed")]
		SetRateFailed,

		[Field ("AVPlayerRateDidChangeReasonAudioSessionInterrupted")]
		AudioSessionInterrupted,

		[Field ("AVPlayerRateDidChangeReasonAppBackgrounded")]
		AppBackgrounded,

	}

	[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), Mac (12, 0), Watch (8, 0)]
	enum AVVideoRange {

		[Field ("AVVideoRangeSDR")]
		Sdr,

		[Field ("AVVideoRangeHLG")]
		Hlg,

		[Field ("AVVideoRangePQ")]
		PQ,
	}


	[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), Mac (12, 0), NoWatch]
	interface AVPlayerRateDidChangeEventArgs {
		[Export ("AVPlayerRateDidChangeReasonKey")]
		NSString RateDidChangeStringReason { get; }

		[Export ("AVPlayerRateDidChangeOriginatingParticipantKey")]
		NSString RateDidChangeOriginatingParticipant { get; }
	}

	[MacCatalyst (14, 5)]
	enum AVPlayerWaitingReason {
		[MacCatalyst (15, 0)]
		[Field ("AVPlayerWaitingToMinimizeStallsReason")]
		WaitingToMinimizeStalls,

		[MacCatalyst (15, 0)]
		[Field ("AVPlayerWaitingWhileEvaluatingBufferingRateReason")]
		WaitingWhileEvaluatingBufferingRate,

		[MacCatalyst (15, 0)]
		[Field ("AVPlayerWaitingWithNoItemToPlayReason")]
		WaitingWithNoItemToPlay,

		[iOS (14, 5), TV (14, 5), Mac (11, 3), MacCatalyst (14, 5), Watch (8, 0)]
		[Field ("AVPlayerWaitingDuringInterstitialEventReason")]
		WaitingDuringInterstitialEvent,

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("AVPlayerWaitingForCoordinatedPlaybackReason")]
		WaitingForCoordinatedPlayback,
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVPlayer {

#if WATCH
		[Notification]
#else
		[Notification (typeof (AVPlayerRateDidChangeEventArgs))]
#endif
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("AVPlayerRateDidChangeNotification")]
		NSString RateDidChangeNotification { get; }

		[Export ("currentItem"), NullAllowed]
		AVPlayerItem CurrentItem { get; }

		[Export ("rate")]
		float Rate { get; set; } // defined as 'float'

		// note: not a property in ObjC
		[Export ("currentTime")]
		CMTime CurrentTime { get; }

		[Export ("actionAtItemEnd")]
		AVPlayerActionAtItemEnd ActionAtItemEnd { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[Deprecated (PlatformName.TvOS, 11, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("closedCaptionDisplayEnabled")]
		bool ClosedCaptionDisplayEnabled { [Bind ("isClosedCaptionDisplayEnabled")] get; set; }

		[Static, Export ("playerWithURL:")]
		AVPlayer FromUrl (NSUrl URL);

		[Static]
		[Export ("playerWithPlayerItem:")]
		AVPlayer FromPlayerItem ([NullAllowed] AVPlayerItem item);

		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl URL);

		[Export ("initWithPlayerItem:")]
		NativeHandle Constructor ([NullAllowed] AVPlayerItem item);

		[Export ("play")]
		void Play ();

		[Export ("pause")]
		void Pause ();

		[MacCatalyst (13, 1)]
		[Export ("timeControlStatus")]
		AVPlayerTimeControlStatus TimeControlStatus { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("reasonForWaitingToPlay")]
		string ReasonForWaitingToPlay { get; }

		[MacCatalyst (13, 1)]
		[Export ("playImmediatelyAtRate:")]
		void PlayImmediatelyAtRate (float rate);

		[Export ("replaceCurrentItemWithPlayerItem:")]
		void ReplaceCurrentItemWithPlayerItem ([NullAllowed] AVPlayerItem item);

		[Export ("addPeriodicTimeObserverForInterval:queue:usingBlock:")]
		NSObject AddPeriodicTimeObserver (CMTime interval, [NullAllowed] DispatchQueue queue, Action<CMTime> handler);

		[Export ("addBoundaryTimeObserverForTimes:queue:usingBlock:")]
		NSObject AddBoundaryTimeObserver (NSValue [] times, [NullAllowed] DispatchQueue queue, Action handler);

		[Export ("removeTimeObserver:")]
		void RemoveTimeObserver (NSObject observer);

		[Export ("seekToTime:")]
		void Seek (CMTime toTime);

		[Export ("seekToTime:toleranceBefore:toleranceAfter:")]
		void Seek (CMTime toTime, CMTime toleranceBefore, CMTime toleranceAfter);

		[Export ("error"), NullAllowed]
		NSError Error { get; }

		[Export ("status")]
		AVPlayerStatus Status { get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'AllowsExternalPlayback' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'AllowsExternalPlayback' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AllowsExternalPlayback' instead.")]
		[Export ("allowsAirPlayVideo")]
		bool AllowsAirPlayVideo { get; set; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'ExternalPlaybackActive' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'ExternalPlaybackActive' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ExternalPlaybackActive' instead.")]
		[Export ("airPlayVideoActive")]
		bool AirPlayVideoActive { [Bind ("isAirPlayVideoActive")] get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'UsesExternalPlaybackWhileExternalScreenIsActive' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'UsesExternalPlaybackWhileExternalScreenIsActive' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UsesExternalPlaybackWhileExternalScreenIsActive' instead.")]
		[Export ("usesAirPlayVideoWhileAirPlayScreenIsActive")]
		bool UsesAirPlayVideoWhileAirPlayScreenIsActive { get; set; }

		[Export ("seekToTime:completionHandler:")]
		[Async]
		void Seek (CMTime time, AVCompletion completion);

		[Export ("seekToTime:toleranceBefore:toleranceAfter:completionHandler:")]
		[Async]
		void Seek (CMTime time, CMTime toleranceBefore, CMTime toleranceAfter, AVCompletion completion);

		[MacCatalyst (13, 1)]
		[Export ("seekToDate:")]
		void Seek (NSDate date);

		[MacCatalyst (13, 1)]
		[Export ("seekToDate:completionHandler:")]
		[Async]
		void Seek (NSDate date, AVCompletion onComplete);

		[MacCatalyst (13, 1)]
		[Export ("automaticallyWaitsToMinimizeStalling")]
		bool AutomaticallyWaitsToMinimizeStalling { get; set; }

		[Export ("setRate:time:atHostTime:")]
		void SetRate (float /* defined as 'float' */ rate, CMTime itemTime, CMTime hostClockTime);

		[Export ("prerollAtRate:completionHandler:")]
		[Async]
		void Preroll (float /* defined as 'float' */ rate, [NullAllowed] AVCompletion onComplete);

		[Export ("cancelPendingPrerolls")]
		void CancelPendingPrerolls ();

		[MacCatalyst (13, 1)]
		[Export ("outputObscuredDueToInsufficientExternalProtection")]
		bool OutputObscuredDueToInsufficientExternalProtection { get; }

		[Deprecated (PlatformName.MacOSX, 15, 0)]
		[Deprecated (PlatformName.iOS, 15, 0)]
		[Deprecated (PlatformName.TvOS, 15, 0)]
		[Deprecated (PlatformName.WatchOS, 15, 0)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0)]
		[Export ("masterClock"), NullAllowed]
		CMClock MasterClock { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsExternalPlayback")]
		bool AllowsExternalPlayback { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("externalPlaybackActive")]
		bool ExternalPlaybackActive { [Bind ("isExternalPlaybackActive")] get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("usesExternalPlaybackWhileExternalScreenIsActive")]
		bool UsesExternalPlaybackWhileExternalScreenIsActive { get; set; }

		[MacCatalyst (13, 1)]
		[Protected]
		[Export ("externalPlaybackVideoGravity", ArgumentSemantic.Copy)]
		NSString WeakExternalPlaybackVideoGravity { get; set; }

		[Export ("volume")]
		float Volume { get; set; } // defined as 'float'

		[Export ("muted")]
		bool Muted { [Bind ("isMuted")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("appliesMediaSelectionCriteriaAutomatically")]
		bool AppliesMediaSelectionCriteriaAutomatically { get; set; }

		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Export ("mediaSelectionCriteriaForMediaCharacteristic:")]
		AVPlayerMediaSelectionCriteria MediaSelectionCriteriaForMediaCharacteristic (NSString avMediaCharacteristic);

		[MacCatalyst (13, 1)]
		[Export ("setMediaSelectionCriteria:forMediaCharacteristic:")]
		void SetMediaSelectionCriteria ([NullAllowed] AVPlayerMediaSelectionCriteria criteria, NSString avMediaCharacteristic);

		[NoiOS, NoTV, NoWatch, MacCatalyst (15, 0)]
		[Export ("audioOutputDeviceUniqueID"), NullAllowed]
		string AudioOutputDeviceUniqueID { get; set; }

#if !NET
		[Obsolete ("Use 'AVPlayerWaitingReason' enum instead.")]
		[Field ("AVPlayerWaitingToMinimizeStallsReason")]
		NSString WaitingToMinimizeStallsReason { get; }

		[Obsolete ("Use 'AVPlayerWaitingReason' enum instead.")]
		[Field ("AVPlayerWaitingWhileEvaluatingBufferingRateReason")]
		NSString WaitingWhileEvaluatingBufferingRateReason { get; }

		[Obsolete ("Use 'AVPlayerWaitingReason' enum instead.")]
		[Field ("AVPlayerWaitingWithNoItemToPlayReason")]
		NSString WaitingWithNoItemToPlayReason { get; }

		[Obsolete ("Use 'AVPlayerWaitingReason' enum instead.")]
		[iOS (14, 5), TV (14, 5), Mac (11, 3)]
		[MacCatalyst (14, 5)]
		[Watch (8, 0)]
		[Field ("AVPlayerWaitingDuringInterstitialEventReason")]
		NSString WaitingDuringInterstitialEventReason { get; }

		[Obsolete ("Use 'AVPlayerWaitingReason' enum instead.")]
		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("AVPlayerWaitingForCoordinatedPlaybackReason")]
		NSString AVPlayerWaitingForCoordinatedPlaybackReason { get; }
#endif // !NET

		// From AVPlayer (AVPlayerPlaybackCapabilities) Category

		[TV (11, 2), NoWatch, NoMac, iOS (11, 2)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("availableHDRModes")]
		AVPlayerHdrMode AvailableHdrModes { get; }

		[TV (11, 2), NoWatch, NoMac, iOS (11, 2)]
		[MacCatalyst (13, 1)]
		[Field ("AVPlayerAvailableHDRModesDidChangeNotification")]
		[Notification]
		NSString AvailableHdrModesDidChangeNotification { get; }

		// From AVPlayer (AVPlayerVideoDecoderGPUSupport) Category

		[NoWatch, NoTV, NoiOS, MacCatalyst (15, 0)]
		[Export ("preferredVideoDecoderGPURegistryID")]
		ulong PreferredVideoDecoderGpuRegistryId { get; set; }

		// From AVPlayerVideoDisplaySleepPrevention (AVPlayer) Category

		[TV (12, 0), NoWatch, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("preventsDisplaySleepDuringVideoPlayback")]
		bool PreventsDisplaySleepDuringVideoPlayback { get; set; }

		[TV (13, 4), NoWatch, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("eligibleForHDRPlayback")]
		bool EligibleForHdrPlayback { get; }

		[Notification]
		[TV (13, 4), NoWatch, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Field ("AVPlayerEligibleForHDRPlaybackDidChangeNotification")]
		NSString EligibleForHdrPlaybackDidChangeNotification { get; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("sourceClock", ArgumentSemantic.Retain)]
		CMClock SourceClock { get; set; }

		// AVPlayer_AVPlayerBackgroundSupport
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("audiovisualBackgroundPlaybackPolicy", ArgumentSemantic.Assign)]
		AVPlayerAudiovisualBackgroundPlaybackPolicy AudiovisualBackgroundPlaybackPolicy { get; set; }

		// AVPlayer_PlaybackCoordination
		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("playbackCoordinator", ArgumentSemantic.Strong)]
		AVPlayerPlaybackCoordinator PlaybackCoordinator { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerMediaSelectionCriteria {
		[Export ("preferredLanguages"), NullAllowed]
		string [] PreferredLanguages { get; }

		[Export ("preferredMediaCharacteristics"), NullAllowed]
		NSString [] PreferredMediaCharacteristics { get; }

		[Export ("initWithPreferredLanguages:preferredMediaCharacteristics:")]
		NativeHandle Constructor ([NullAllowed] string [] preferredLanguages, [NullAllowed] NSString [] preferredMediaCharacteristics);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithPrincipalMediaCharacteristics:preferredLanguages:preferredMediaCharacteristics:")]
		NativeHandle Constructor ([NullAllowed][BindAs (typeof (AVMediaCharacteristics []))] NSString [] principalMediaCharacteristics, [NullAllowed][BindAs (typeof (AVMediaCharacteristics []))] NSString [] preferredLanguages, [NullAllowed] string [] preferredMediaCharacteristics);

		[BindAs (typeof (AVMediaCharacteristics []))]
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("principalMediaCharacteristics")]
		NSString [] PrincipalMediaCharacteristics { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSGenericException *** -[AVTextStyleRule init] Not available.  Use initWithTextMarkupAttributes:textSelector: instead
	interface AVTextStyleRule : NSCopying {
		[Export ("textMarkupAttributes")]
		[Protected]
		NSDictionary WeakTextMarkupAttributes { get; }

		[Wrap ("WeakTextMarkupAttributes")]
		CMTextMarkupAttributes TextMarkupAttributes { get; }

		[Export ("textSelector"), NullAllowed]
		string TextSelector { get; }

		[Static]
		[Export ("propertyListForTextStyleRules:")]
		NSObject ToPropertyList (AVTextStyleRule [] textStyleRules);

		[return: NullAllowed]
		[Static]
		[Export ("textStyleRulesFromPropertyList:")]
		AVTextStyleRule [] FromPropertyList (NSObject plist);

		[return: NullAllowed]
		[Static]
		[Internal]
		[Export ("textStyleRuleWithTextMarkupAttributes:")]
		AVTextStyleRule FromTextMarkupAttributes (NSDictionary textMarkupAttributes);

		[return: NullAllowed]
		[Static]
		[Wrap ("FromTextMarkupAttributes (textMarkupAttributes.GetDictionary ()!)")]
		AVTextStyleRule FromTextMarkupAttributes (CMTextMarkupAttributes textMarkupAttributes);

		[return: NullAllowed]
		[Static]
		[Internal]
		[Export ("textStyleRuleWithTextMarkupAttributes:textSelector:")]
		AVTextStyleRule FromTextMarkupAttributes (NSDictionary textMarkupAttributes, [NullAllowed] string textSelector);

		[return: NullAllowed]
		[Static]
		[Wrap ("FromTextMarkupAttributes (textMarkupAttributes.GetDictionary ()!, textSelector)")]
		AVTextStyleRule FromTextMarkupAttributes (CMTextMarkupAttributes textMarkupAttributes, [NullAllowed] string textSelector);

		[Export ("initWithTextMarkupAttributes:")]
		[Protected]
		NativeHandle Constructor (NSDictionary textMarkupAttributes);

		[Wrap ("this (attributes.GetDictionary ()!)")]
		NativeHandle Constructor (CMTextMarkupAttributes attributes);

		[DesignatedInitializer]
		[Export ("initWithTextMarkupAttributes:textSelector:")]
		[Protected]
		NativeHandle Constructor (NSDictionary textMarkupAttributes, [NullAllowed] string textSelector);

		[Wrap ("this (attributes.GetDictionary ()!, textSelector)")]
		NativeHandle Constructor (CMTextMarkupAttributes attributes, string textSelector);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVMetadataGroup {

		[Export ("items", ArgumentSemantic.Copy)]
		AVMetadataItem [] Items { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("classifyingLabel")]
		string ClassifyingLabel { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("uniqueID")]
		string UniqueID { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVMetadataGroup))]
	interface AVTimedMetadataGroup : NSMutableCopying {
		[Export ("timeRange")]
		CMTimeRange TimeRange { get; [NotImplemented] set; }

		[Export ("items", ArgumentSemantic.Copy)]
		AVMetadataItem [] Items { get; [NotImplemented] set; }

		[Export ("initWithItems:timeRange:")]
		NativeHandle Constructor (AVMetadataItem [] items, CMTimeRange timeRange);

		[return: NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("copyFormatDescription")]
		CMFormatDescription CopyFormatDescription ();

		[MacCatalyst (13, 1)]
		[Export ("initWithSampleBuffer:")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVTimedMetadataGroup))]
	interface AVMutableTimedMetadataGroup {

		[Export ("items", ArgumentSemantic.Copy)]
		[Override]
		AVMetadataItem [] Items { get; set; }

		[Export ("timeRange")]
		[Override]
		CMTimeRange TimeRange { get; set; }
	}

	interface AVPlayerItemErrorEventArgs {
		[Export ("AVPlayerItemFailedToPlayToEndTimeErrorKey")]
		NSError Error { get; }
	}

	[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), Mac (12, 0), NoWatch]
	interface AVPlayerItemTimeJumpedEventArgs {
		[Export ("AVPlayerItemTimeJumpedOriginatingParticipantKey")]
		NSString OriginatingParticipant { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVPlayerItem : NSCopying {
		[Export ("status")]
		AVPlayerItemStatus Status { get; }

		[Export ("asset")]
		AVAsset Asset { get; }

		[Export ("tracks")]
		AVPlayerItemTrack [] Tracks { get; }

		[Export ("presentationSize")]
		CGSize PresentationSize { get; }

		[Export ("forwardPlaybackEndTime")]
		CMTime ForwardPlaybackEndTime { get; set; }

		[Export ("reversePlaybackEndTime")]
		CMTime ReversePlaybackEndTime { get; set; }

		[Export ("audioMix", ArgumentSemantic.Copy), NullAllowed]
		AVAudioMix AudioMix { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("videoComposition", ArgumentSemantic.Copy), NullAllowed]
		AVVideoComposition VideoComposition { get; set; }

		[Export ("currentTime")]
		CMTime CurrentTime { get; }

		[Export ("playbackLikelyToKeepUp")]
		bool PlaybackLikelyToKeepUp { [Bind ("isPlaybackLikelyToKeepUp")] get; }

		[Export ("playbackBufferFull")]
		bool PlaybackBufferFull { [Bind ("isPlaybackBufferFull")] get; }

		[Export ("playbackBufferEmpty")]
		bool PlaybackBufferEmpty { [Bind ("isPlaybackBufferEmpty")] get; }

		[MacCatalyst (13, 1)]
		[Export ("canUseNetworkResourcesForLiveStreamingWhilePaused", ArgumentSemantic.Assign)]
		bool CanUseNetworkResourcesForLiveStreamingWhilePaused { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("preferredForwardBufferDuration")]
		double PreferredForwardBufferDuration { get; set; }

		[Export ("seekableTimeRanges")]
		NSValue [] SeekableTimeRanges { get; }

		[Export ("loadedTimeRanges")]
		NSValue [] LoadedTimeRanges { get; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the class 'AVPlayerItemMetadataOutput' instead to get the time metadata info.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the class 'AVPlayerItemMetadataOutput' instead to get the time metadata info.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the class 'AVPlayerItemMetadataOutput' instead to get the time metadata info.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the class 'AVPlayerItemMetadataOutput' instead to get the time metadata info.")]
		[Export ("timedMetadata"), NullAllowed]
		NSObject [] TimedMetadata { get; }

		[Static, Export ("playerItemWithURL:")]
		AVPlayerItem FromUrl (NSUrl URL);

		[Static]
		[Export ("playerItemWithAsset:")]
		AVPlayerItem FromAsset ([NullAllowed] AVAsset asset);

		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl URL);

		[Export ("initWithAsset:")]
		NativeHandle Constructor (AVAsset asset);

		[Export ("stepByCount:")]
		void StepByCount (nint stepCount);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'Seek (NSDate, AVCompletion)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Seek (NSDate, AVCompletion)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'Seek (NSDate, AVCompletion)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Seek (NSDate, AVCompletion)' instead.")]
		[Export ("seekToDate:")]
		bool Seek (NSDate date);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'Seek (CMTime, AVCompletion)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Seek (CMTime, AVCompletion)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'Seek (CMTime, AVCompletion)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Seek (CMTime, AVCompletion)' instead.")]
		[Export ("seekToTime:")]
		void Seek (CMTime time);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'Seek (CMTime, CMTime, CMTime, AVCompletion)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Seek (CMTime, CMTime, CMTime, AVCompletion)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'Seek (CMTime, CMTime, CMTime, AVCompletion)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Seek (CMTime, CMTime, CMTime, AVCompletion)' instead.")]
		[Export ("seekToTime:toleranceBefore:toleranceAfter:")]
		void Seek (CMTime time, CMTime toleranceBefore, CMTime toleranceAfter);

		[Export ("error"), NullAllowed]
		NSError Error { get; }

		[Field ("AVPlayerItemDidPlayToEndTimeNotification")]
		[Notification]
		NSString DidPlayToEndTimeNotification { get; }

		[Field ("AVPlayerItemFailedToPlayToEndTimeNotification")]
		[Notification (typeof (AVPlayerItemErrorEventArgs))]
		NSString ItemFailedToPlayToEndTimeNotification { get; }

		[Field ("AVPlayerItemFailedToPlayToEndTimeErrorKey")]
		NSString ItemFailedToPlayToEndTimeErrorKey { get; }

		[Export ("accessLog"), NullAllowed]
		AVPlayerItemAccessLog AccessLog { get; }

		[Export ("errorLog"), NullAllowed]
		AVPlayerItemErrorLog ErrorLog { get; }

		[Export ("currentDate"), NullAllowed]
		NSDate CurrentDate { get; }

		[Export ("duration")]
		CMTime Duration { get; }

		[Export ("canPlayFastReverse")]
		bool CanPlayFastReverse { get; }

		[Export ("canPlayFastForward")]
		bool CanPlayFastForward { get; }

		[Field ("AVPlayerItemTimeJumpedNotification")]
#if !NET
		[Notification]
#else
		[Notification (typeof (AVPlayerItemTimeJumpedEventArgs))]
#endif
		NSString TimeJumpedNotification { get; }

		[Export ("seekToTime:completionHandler:")]
		[Async]
		void Seek (CMTime time, [NullAllowed] AVCompletion completion);

		[Export ("cancelPendingSeeks")]
		void CancelPendingSeeks ();

		[Export ("seekToTime:toleranceBefore:toleranceAfter:completionHandler:")]
		[Async]
		void Seek (CMTime time, CMTime toleranceBefore, CMTime toleranceAfter, [NullAllowed] AVCompletion completion);

		[Export ("selectMediaOption:inMediaSelectionGroup:")]
		void SelectMediaOption ([NullAllowed] AVMediaSelectionOption mediaSelectionOption, AVMediaSelectionGroup mediaSelectionGroup);

		[return: NullAllowed]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CurrentMediaSelection' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CurrentMediaSelection' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CurrentMediaSelection' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CurrentMediaSelection' instead.")]
		[Export ("selectedMediaOptionInMediaSelectionGroup:")]
		AVMediaSelectionOption SelectedMediaOption (AVMediaSelectionGroup inMediaSelectionGroup);

		[MacCatalyst (13, 1)]
		[Export ("currentMediaSelection")]
		AVMediaSelection CurrentMediaSelection { get; }

		[Export ("canPlaySlowForward")]
		bool CanPlaySlowForward { get; }

		[Export ("canPlayReverse")]
		bool CanPlayReverse { get; }

		[Export ("canPlaySlowReverse")]
		bool CanPlaySlowReverse { get; }

		[Export ("canStepForward")]
		bool CanStepForward { get; }

		[Export ("canStepBackward")]
		bool CanStepBackward { get; }

		[Export ("outputs")]
		AVPlayerItemOutput [] Outputs { get; }

		[Export ("addOutput:")]
		[PostGet ("Outputs")]
		void AddOutput (AVPlayerItemOutput output);

		[Export ("removeOutput:")]
		[PostGet ("Outputs")]
		void RemoveOutput (AVPlayerItemOutput output);

		[Export ("timebase"), NullAllowed]
		CMTimebase Timebase { get; }

		[MacCatalyst (13, 1)]
		[Export ("seekToDate:completionHandler:")]
		[Async]
		bool Seek (NSDate date, AVCompletion completion);

		[MacCatalyst (13, 1)]
		[Export ("seekingWaitsForVideoCompositionRendering")]
		bool SeekingWaitsForVideoCompositionRendering { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("textStyleRules", ArgumentSemantic.Copy), NullAllowed]
		AVTextStyleRule [] TextStyleRules { get; set; }

		[MacCatalyst (13, 1)]
		[Field ("AVPlayerItemPlaybackStalledNotification")]
		[Notification]
		NSString PlaybackStalledNotification { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVPlayerItemNewAccessLogEntryNotification")]
		[Notification]
		NSString NewAccessLogEntryNotification { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVPlayerItemNewErrorLogEntryNotification")]
		[Notification]
		NSString NewErrorLogEntryNotification { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("playerItemWithAsset:automaticallyLoadedAssetKeys:")]
		AVPlayerItem FromAsset ([NullAllowed] AVAsset asset, [NullAllowed] NSString [] automaticallyLoadedAssetKeys);

		[MacCatalyst (13, 1)]
		[DesignatedInitializer]
		[Export ("initWithAsset:automaticallyLoadedAssetKeys:")]
		NativeHandle Constructor (AVAsset asset, [NullAllowed] params NSString [] automaticallyLoadedAssetKeys);

		[MacCatalyst (13, 1)]
		[Export ("automaticallyLoadedAssetKeys", ArgumentSemantic.Copy)]
		NSString [] AutomaticallyLoadedAssetKeys { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("customVideoCompositor", ArgumentSemantic.Copy), NullAllowed]
		[Protocolize]
		AVVideoCompositing CustomVideoCompositor { get; }

		[MacCatalyst (13, 1)]
		[Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		// DOC: Mention this is an AVAudioTimePitch constant
		NSString AudioTimePitchAlgorithm { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("selectMediaOptionAutomaticallyInMediaSelectionGroup:")]
		void SelectMediaOptionAutomaticallyInMediaSelectionGroup (AVMediaSelectionGroup mediaSelectionGroup);

		[MacCatalyst (13, 1)]
		[Export ("preferredPeakBitRate")]
		double PreferredPeakBitRate { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("preferredMaximumResolution", ArgumentSemantic.Assign)]
		CGSize PreferredMaximumResolution { get; set; }

		#region AVPlayerViewControllerAdditions
		[NoiOS]
		[NoMac]
		[NoWatch]
		[NoMacCatalyst]
		[Export ("navigationMarkerGroups", ArgumentSemantic.Copy)]
		AVNavigationMarkersGroup [] NavigationMarkerGroups { get; set; }

		[NoMac]
		[NoWatch]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("externalMetadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] ExternalMetadata { get; set; }

		[iOS (16, 0)]
		[NoMacCatalyst]
		[NoMac]
		[NoWatch]
		[Export ("interstitialTimeRanges", ArgumentSemantic.Copy)]
		AVInterstitialTimeRange [] InterstitialTimeRanges { get; set; }
		#endregion

		[MacCatalyst (13, 1)]
		[Export ("addMediaDataCollector:")]
		void AddMediaDataCollector (AVPlayerItemMediaDataCollector collector);

		[MacCatalyst (13, 1)]
		[Export ("removeMediaDataCollector:")]
		void RemoveMediaDataCollector (AVPlayerItemMediaDataCollector collector);

		[MacCatalyst (13, 1)]
		[Export ("mediaDataCollectors")]
		AVPlayerItemMediaDataCollector [] MediaDataCollectors { get; }

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[NullAllowed, Export ("nextContentProposal", ArgumentSemantic.Assign)]
		AVContentProposal NextContentProposal { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("videoApertureMode")]
		NSString _VideoApertureMode { get; set; }

		[Notification]
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVPlayerItemRecommendedTimeOffsetFromLiveDidChangeNotification")]
		NSString RecommendedTimeOffsetFromLiveDidChangeNotification { get; }

		[Notification]
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVPlayerItemMediaSelectionDidChangeNotification")]
		NSString MediaSelectionDidChangeNotification { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("configuredTimeOffsetFromLive", ArgumentSemantic.Assign)]
		CMTime ConfiguredTimeOffsetFromLive { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("recommendedTimeOffsetFromLive")]
		CMTime RecommendedTimeOffsetFromLive { get; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("automaticallyPreservesTimeOffsetFromLive")]
		bool AutomaticallyPreservesTimeOffsetFromLive { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'AllowedAudioSpatializationFormats' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'AllowedAudioSpatializationFormats' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'AllowedAudioSpatializationFormats' instead.")]
		[Export ("audioSpatializationAllowed")]
		bool AudioSpatializationAllowed { [Bind ("isAudioSpatializationAllowed")] get; set; }

		[NoWatch, TV (15, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("allowedAudioSpatializationFormats", ArgumentSemantic.Assign)]
		AVAudioSpatializationFormats AllowedAudioSpatializationFormats { get; set; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("startsOnFirstEligibleVariant")]
		bool StartsOnFirstEligibleVariant { get; set; }

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("variantPreferences", ArgumentSemantic.Assign)]
		AVVariantPreferences VariantPreferences { get; set; }

		[iOS (14, 1)]
		[TV (14, 2)]
		[Mac (11, 0)]
		[NoWatch]
		[MacCatalyst (14, 1)]
		[Export ("appliesPerFrameHDRDisplayMetadata")]
		bool AppliesPerFrameHdrDisplayMetadata { get; set; }

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("preferredMaximumResolutionForExpensiveNetworks", ArgumentSemantic.Assign)]
		CGSize PreferredMaximumResolutionForExpensiveNetworks { get; set; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("preferredPeakBitRateForExpensiveNetworks")]
		double PreferredPeakBitRateForExpensiveNetworks { get; set; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("copy")]
		[return: Release]
		NSObject Copy ();

		[TV (15, 0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("translatesPlayerInterstitialEvents")]
		bool TranslatesPlayerInterstitialEvents { get; set; }

		[Watch (9, 0), TV (16, 0), NoMac, iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[NullAllowed, Export ("nowPlayingInfo", ArgumentSemantic.Copy)]
		NSDictionary WeakNowPlayingInfo { get; set; }
	}

	[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Flags]
	[Native]
	public enum AVVariantPreferences : ulong {
		None = 0,
		ScalabilityToLosslessAudio = 1 << 0,
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[Category]
	[BaseType (typeof (AVPlayerItem))]
	interface AVPlayerItem_AVPlayerItemProtectedContent {
		[Export ("isAuthorizationRequiredForPlayback")]
		bool IsAuthorizationRequiredForPlayback ();

		[Export ("isApplicationAuthorizedForPlayback")]
		bool IsApplicationAuthorizedForPlayback ();

		[Export ("isContentAuthorizedForPlayback")]
		bool IsContentAuthorizedForPlayback ();

		[Export ("requestContentAuthorizationAsynchronouslyWithTimeoutInterval:completionHandler:")]
		void RequestContentAuthorizationAsynchronously (/* NSTimeInterval */ double timeoutInterval, Action handler);

		[Export ("cancelContentAuthorizationRequest")]
		void CancelContentAuthorizationRequest ();

		[Export ("contentAuthorizationRequestStatus")]
		AVContentAuthorizationStatus GetContentAuthorizationRequestStatus ();
	}

	[NoWatch]
	[TV (14, 5), Mac (11, 3), iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Category]
	[BaseType (typeof (AVPlayerItem))]
	interface AVPlayerItem_AVPlayerInterstitialSupport {
		[Export ("automaticallyHandlesInterstitialEvents")]
		bool GetAutomaticallyHandlesInterstitialEvents ();

		[Export ("setAutomaticallyHandlesInterstitialEvents:")]
		void SetAutomaticallyHandlesInterstitialEvents (bool value);

		[Export ("templatePlayerItem")]
		[return: NullAllowed]
		AVPlayerItem GetTemplatePlayerItem ();
	}

	[NoWatch, NoMac, NoiOS]
	[TV (13, 0)]
	[NoMacCatalyst]
	[Category]
	[BaseType (typeof (AVPlayerItem))]
	interface AVPlayerItem_AVPlaybackRestrictions {
		[Async]
		[Export ("requestPlaybackRestrictionsAuthorization:")]
		void RequestPlaybackRestrictionsAuthorization (Action<bool, NSError> completion);

		[Export ("cancelPlaybackRestrictionsAuthorizationRequest")]
		void CancelPlaybackRestrictionsAuthorizationRequest ();
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** initialization method -init cannot be sent to an abstract object of class AVPlayerItemOutput: Create a concrete instance!
	[DisableDefaultCtor]
	interface AVPlayerItemOutput {
		[Export ("itemTimeForHostTime:")]
		CMTime GetItemTime (double hostTimeInSeconds);

		[Export ("itemTimeForMachAbsoluteTime:")]
		CMTime GetItemTime (long machAbsoluteTime);

		[Export ("suppressesPlayerRendering")]
		bool SuppressesPlayerRendering { get; set; }

		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[Export ("itemTimeForCVTimeStamp:")]
		CMTime GetItemTime (CVTimeStamp timestamp);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // initialization method -init cannot be sent to an abstract object of class AVPlayerItemMediaDataCollector: Create a concrete instance!
	[Abstract]
	interface AVPlayerItemMediaDataCollector {
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVPlayerItemOutput))]
	interface AVPlayerItemMetadataOutput {

		[DesignatedInitializer]
		[Export ("initWithIdentifiers:")]
		NativeHandle Constructor ([NullAllowed] NSString [] metadataIdentifiers);

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		[Protocolize]
		AVPlayerItemMetadataOutputPushDelegate Delegate { get; }

		[Export ("delegateQueue"), NullAllowed]
		DispatchQueue DelegateQueue { get; }

		[Export ("advanceIntervalForDelegateInvocation")]
		double AdvanceIntervalForDelegateInvocation { get; set; }

		[Export ("setDelegate:queue:")]
		void SetDelegate ([Protocolize][NullAllowed] AVPlayerItemMetadataOutputPushDelegate pushDelegate, [NullAllowed] DispatchQueue delegateQueue);
	}

	[BaseType (typeof (NSObject))]
	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	interface AVPlayerItemMetadataOutputPushDelegate : AVPlayerItemOutputPushDelegate {

		[MacCatalyst (13, 1)]
		[Export ("metadataOutput:didOutputTimedMetadataGroups:fromPlayerItemTrack:")]
		void DidOutputTimedMetadataGroups (AVPlayerItemMetadataOutput output, AVTimedMetadataGroup [] groups, [NullAllowed] AVPlayerItemTrack track);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	interface AVVideoColorPrimaries {
		[Field ("AVVideoColorPrimaries_ITU_R_709_2")]
		NSString Itu_R_709_2 { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("AVVideoColorPrimaries_EBU_3213")]
		NSString Ebu_3213 { get; }

		[Field ("AVVideoColorPrimaries_SMPTE_C")]
		NSString Smpte_C { get; }

		[Field ("AVVideoColorPrimaries_P3_D65")]
		NSString P3_D65 { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoColorPrimaries_ITU_R_2020")]
		NSString Itu_R_2020 { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	interface AVVideoTransferFunction {
		[MacCatalyst (13, 1)]
		[Field ("AVVideoTransferFunction_ITU_R_709_2")]
		NSString AVVideoTransferFunction_Itu_R_709_2 { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("AVVideoTransferFunction_SMPTE_240M_1995")]
		NSString AVVideoTransferFunction_Smpte_240M_1995 { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	interface AVVideoYCbCrMatrix {

		[MacCatalyst (13, 1)]
		[Field ("AVVideoYCbCrMatrix_ITU_R_709_2")]
		NSString Itu_R_709_2 { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoYCbCrMatrix_ITU_R_601_4")]
		NSString Itu_R_601_4 { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Field ("AVVideoYCbCrMatrix_SMPTE_240M_1995")]
		NSString Smpte_240M_1995 { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoYCbCrMatrix_ITU_R_2020")]
		NSString Itu_R_2020 { get; }

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("AVColorPropertiesKeys")]
	interface AVColorProperties {
		NSString AVVideoColorPrimaries { get; set; }
		NSString AVVideoTransferFunction { get; set; }
		NSString AVVideoYCbCrMatrix { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	interface AVColorPropertiesKeys {
		[MacCatalyst (13, 1)]
		[Field ("AVVideoColorPrimariesKey")]
		NSString AVVideoColorPrimariesKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoTransferFunctionKey")]
		NSString AVVideoTransferFunctionKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoYCbCrMatrixKey")]
		NSString AVVideoYCbCrMatrixKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("AVCleanAperturePropertiesKeys")]
	interface AVCleanApertureProperties {
		NSNumber Width { get; set; }
		NSNumber Height { get; set; }
		NSNumber HorizontalOffset { get; set; }
		NSNumber VerticalOffset { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	interface AVCleanAperturePropertiesKeys {
		[Field ("AVVideoCleanApertureWidthKey")]
		NSString WidthKey { get; }

		[Field ("AVVideoCleanApertureHeightKey")]
		NSString HeightKey { get; }

		[Field ("AVVideoCleanApertureHorizontalOffsetKey")]
		NSString HorizontalOffsetKey { get; }

		[Field ("AVVideoCleanApertureVerticalOffsetKey")]
		NSString VerticalOffsetKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("AVPixelAspectRatioPropertiesKeys")]
	interface AVPixelAspectRatioProperties {
		NSNumber PixelAspectRatioHorizontalSpacing { get; set; }
		NSNumber PixelAspectRatioVerticalSpacing { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Internal]
	[Static]
	interface AVPixelAspectRatioPropertiesKeys {
		[Field ("AVVideoPixelAspectRatioHorizontalSpacingKey")]
		NSString PixelAspectRatioHorizontalSpacingKey { get; }

		[Field ("AVVideoPixelAspectRatioVerticalSpacingKey")]
		NSString PixelAspectRatioVerticalSpacingKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("AVCompressionPropertiesKeys")]
	interface AVCompressionProperties {
		AVCleanApertureProperties CleanAperture { get; set; }
		AVPixelAspectRatioProperties PixelAspectRatio { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	interface AVCompressionPropertiesKeys {
		[Field ("AVVideoCleanApertureKey")]
		NSString CleanApertureKey { get; }

		[Field ("AVVideoPixelAspectRatioKey")]
		NSString PixelAspectRatioKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("AVPlayerItemVideoOutputSettingsKeys")]
	interface AVPlayerItemVideoOutputSettings {

		[MacCatalyst (13, 1)]
		AVColorProperties ColorProperties { get; set; }

		AVCompressionProperties CompressionProperties { get; set; }

		[MacCatalyst (13, 1)]
		bool AllowWideColor { get; set; }

		NSString Codec { get; set; }
		NSString ScalingMode { get; set; }
		NSNumber Width { get; set; }
		NSNumber Height { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	interface AVPlayerItemVideoOutputSettingsKeys {
		[MacCatalyst (13, 1)]
		[Field ("AVVideoColorPropertiesKey")]
		NSString ColorPropertiesKey { get; }

		[Field ("AVVideoCompressionPropertiesKey")]
		NSString CompressionPropertiesKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVVideoAllowWideColorKey")]
		NSString AllowWideColorKey { get; }

		[Field ("AVVideoCodecKey")]
		NSString CodecKey { get; }

		[Field ("AVVideoScalingModeKey")]
		NSString ScalingModeKey { get; }

		[Field ("AVVideoWidthKey")]
		NSString WidthKey { get; }

		[Field ("AVVideoHeightKey")]
		NSString HeightKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVPlayerItemOutput))]
	interface AVPlayerItemVideoOutput {
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		[Protocolize]
		AVPlayerItemOutputPullDelegate Delegate { get; }

		[Export ("delegateQueue"), NullAllowed]
		DispatchQueue DelegateQueue { get; }

		[Internal]
		[Export ("initWithPixelBufferAttributes:")]
		IntPtr _FromPixelBufferAttributes ([NullAllowed] NSDictionary pixelBufferAttributes);

		[Internal]
		[Export ("initWithOutputSettings:")]
		IntPtr _FromOutputSettings ([NullAllowed] NSDictionary outputSettings);

		[DesignatedInitializer]
		[Wrap ("this (attributes.GetDictionary (), AVPlayerItemVideoOutput.InitMode.PixelAttributes)")]
		NativeHandle Constructor (CVPixelBufferAttributes attributes);

		[DesignatedInitializer]
		[MacCatalyst (13, 1)]
		[Wrap ("this (settings.GetDictionary (), AVPlayerItemVideoOutput.InitMode.OutputSettings)")]
		NativeHandle Constructor (AVPlayerItemVideoOutputSettings settings);

		[Export ("hasNewPixelBufferForItemTime:")]
		bool HasNewPixelBufferForItemTime (CMTime itemTime);

		[Protected]
		[Export ("copyPixelBufferForItemTime:itemTimeForDisplay:")]
		IntPtr WeakCopyPixelBuffer (CMTime itemTime, ref CMTime outItemTimeForDisplay);

		[Export ("setDelegate:queue:")]
		void SetDelegate ([Protocolize][NullAllowed] AVPlayerItemOutputPullDelegate delegateClass, [NullAllowed] DispatchQueue delegateQueue);

		[Export ("requestNotificationOfMediaDataChangeWithAdvanceInterval:")]
		void RequestNotificationOfMediaDataChange (double advanceInterval);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVPlayerItemOutputPullDelegate {
		[Export ("outputMediaDataWillChange:")]
		void OutputMediaDataWillChange (AVPlayerItemOutput sender);

		[Export ("outputSequenceWasFlushed:")]
		void OutputSequenceWasFlushed (AVPlayerItemOutput output);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVPlayerItemOutputPushDelegate {
		[Export ("outputSequenceWasFlushed:")]
		void OutputSequenceWasFlushed (AVPlayerItemOutput output);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVPlayerItemOutputPushDelegate))]
	[Model]
	[Protocol]
	interface AVPlayerItemLegibleOutputPushDelegate {
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("legibleOutput:didOutputAttributedStrings:nativeSampleBuffers:forItemTime:")]
		void DidOutputAttributedStrings (AVPlayerItemLegibleOutput output, NSAttributedString [] strings, CMSampleBuffer [] nativeSamples, CMTime itemTime);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVPlayerItemOutput))]
	interface AVPlayerItemLegibleOutput {
		[Export ("initWithMediaSubtypesForNativeRepresentation:")]
		NativeHandle Constructor (NSNumber [] subtypesFourCCcodes);

		[Export ("setDelegate:queue:")]
		void SetDelegate ([Protocolize][NullAllowed] AVPlayerItemLegibleOutputPushDelegate delegateObject, [NullAllowed] DispatchQueue delegateQueue);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Copy)]
		[Protocolize]
		AVPlayerItemLegibleOutputPushDelegate Delegate { get; }

		[NullAllowed, Export ("delegateQueue", ArgumentSemantic.Copy)]
		DispatchQueue DelegateQueue { get; }

		[Export ("advanceIntervalForDelegateInvocation")]
		double AdvanceIntervalForDelegateInvocation { get; set; }

		// it defaults to null (7.1) but does not always want to be set back to null, e.g.
		// NSInvalidArgumentException *** -[AVPlayerItemLegibleOutput setTextStylingResolution:] Invalid text styling resolution (null)
		[Export ("textStylingResolution", ArgumentSemantic.Copy)]
		NSString TextStylingResolution { get; set; }

		[Field ("AVPlayerItemLegibleOutputTextStylingResolutionDefault")]
		NSString TextStylingResolutionDefault { get; }

		[Field ("AVPlayerItemLegibleOutputTextStylingResolutionSourceAndRulesOnly")]
		NSString TextStylingResolutionSourceAndRulesOnly { get; }

	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemAccessLog : NSCopying {

		[Export ("events")]
		AVPlayerItemAccessLogEvent [] Events { get; }

		[Export ("extendedLogDataStringEncoding")]
		NSStringEncoding ExtendedLogDataStringEncoding { get; }

		[Export ("extendedLogData"), NullAllowed]
		NSData ExtendedLogData { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemErrorLog : NSCopying {
		[Export ("events")]
		AVPlayerItemErrorLogEvent [] Events { get; }

		[Export ("extendedLogDataStringEncoding")]
		NSStringEncoding ExtendedLogDataStringEncoding { get; }

		[Export ("extendedLogData"), NullAllowed]
		NSData ExtendedLogData { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemAccessLogEvent : NSCopying {
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NumberOfMediaRequests' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NumberOfMediaRequests' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use 'NumberOfMediaRequests' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NumberOfMediaRequests' instead.")]
		[Export ("numberOfSegmentsDownloaded")]
		nint SegmentedDownloadedCount { get; }

		[Export ("playbackStartDate"), NullAllowed]
		NSData PlaybackStartDate { get; }

		[Export ("URI"), NullAllowed]
		string Uri { get; }

		[Export ("serverAddress"), NullAllowed]
		string ServerAddress { get; }

		[Export ("numberOfServerAddressChanges")]
		nint ServerAddressChangeCount { get; }

		[Export ("playbackSessionID"), NullAllowed]
		string PlaybackSessionID { get; }

		[Export ("playbackStartOffset")]
		double PlaybackStartOffset { get; }

		[Export ("segmentsDownloadedDuration")]
		double SegmentsDownloadedDuration { get; }

		[Export ("durationWatched")]
		double DurationWatched { get; }

		[Export ("numberOfStalls")]
		nint StallCount { get; }

		[Export ("numberOfBytesTransferred")]
		long BytesTransferred { get; }

		[Export ("observedBitrate")]
		double ObservedBitrate { get; }

		[MacCatalyst (13, 1)]
		[Export ("indicatedBitrate")]
		double IndicatedBitrate { get; }

		[MacCatalyst (13, 1)]
		[Export ("indicatedAverageBitrate")]
		double IndicatedAverageBitrate { get; }

		[MacCatalyst (13, 1)]
		[Export ("averageVideoBitrate")]
		double AverageVideoBitrate { get; }

		[MacCatalyst (13, 1)]
		[Export ("averageAudioBitrate")]
		double AverageAudioBitrate { get; }

		[Export ("numberOfDroppedVideoFrames")]
		nint DroppedVideoFrameCount { get; }

		[MacCatalyst (13, 1)]
		[Export ("numberOfMediaRequests")]
		nint NumberOfMediaRequests { get; }

		[MacCatalyst (13, 1)]
		[Export ("startupTime")]
		double StartupTime { get; }

		[MacCatalyst (13, 1)]
		[Export ("downloadOverdue")]
		nint DownloadOverdue { get; }

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'ObservedBitrateStandardDeviation' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'ObservedBitrateStandardDeviation' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'ObservedBitrateStandardDeviation' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'ObservedBitrateStandardDeviation' instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'ObservedBitrateStandardDeviation' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("observedMaxBitrate")]
		double ObservedMaxBitrate { get; }

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use 'ObservedBitrateStandardDeviation' instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'ObservedBitrateStandardDeviation' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'ObservedBitrateStandardDeviation' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'ObservedBitrateStandardDeviation' instead.")]
		[Deprecated (PlatformName.WatchOS, 8, 0, message: "Use 'ObservedBitrateStandardDeviation' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("observedMinBitrate")]
		double ObservedMinBitrate { get; }

		[MacCatalyst (13, 1)]
		[Export ("observedBitrateStandardDeviation")]
		double ObservedBitrateStandardDeviation { get; }

		[MacCatalyst (13, 1)]
		[Export ("playbackType", ArgumentSemantic.Copy), NullAllowed]
		string PlaybackType { get; }

		[MacCatalyst (13, 1)]
		[Export ("mediaRequestsWWAN")]
		nint MediaRequestsWWAN { get; }

		[MacCatalyst (13, 1)]
		[Export ("switchBitrate")]
		double SwitchBitrate { get; }

		[MacCatalyst (13, 1)]
		[Export ("transferDuration")]
		double TransferDuration { get; }

	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemErrorLogEvent : NSCopying {
		[Export ("date"), NullAllowed]
		NSDate Date { get; }

		[Export ("URI"), NullAllowed]
		string Uri { get; }

		[Export ("serverAddress"), NullAllowed]
		string ServerAddress { get; }

		[Export ("playbackSessionID"), NullAllowed]
		string PlaybackSessionID { get; }

		[Export ("errorStatusCode")]
		nint ErrorStatusCode { get; }

		[Export ("errorDomain")]
		string ErrorDomain { get; }

		[Export ("errorComment"), NullAllowed]
		string ErrorComment { get; }
	}

	interface IAVPlayerItemMetadataCollectorPushDelegate { }

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemMetadataCollectorPushDelegate {
		[Abstract]
		[Export ("metadataCollector:didCollectDateRangeMetadataGroups:indexesOfNewGroups:indexesOfModifiedGroups:")]
		void DidCollectDateRange (AVPlayerItemMetadataCollector metadataCollector, AVDateRangeMetadataGroup [] metadataGroups, NSIndexSet indexesOfNewGroups, NSIndexSet indexesOfModifiedGroups);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVPlayerItemMediaDataCollector))]
	interface AVPlayerItemMetadataCollector {
		[Export ("initWithIdentifiers:classifyingLabels:")]
		NativeHandle Constructor ([NullAllowed] string [] identifiers, [NullAllowed] string [] classifyingLabels);

		[Export ("setDelegate:queue:")]
		void SetDelegate ([NullAllowed] IAVPlayerItemMetadataCollectorPushDelegate pushDelegate, [NullAllowed] DispatchQueue delegateQueue);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IAVPlayerItemMetadataCollectorPushDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[NullAllowed, Export ("delegateQueue")]
		DispatchQueue DelegateQueue { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CALayer))]
	interface AVPlayerLayer {
		[NullAllowed] // by default this property is null
		[Export ("player", ArgumentSemantic.Retain)]
		AVPlayer Player { get; set; }

		[Static, Export ("playerLayerWithPlayer:")]
		AVPlayerLayer FromPlayer ([NullAllowed] AVPlayer player);

		[Export ("videoGravity", ArgumentSemantic.Copy)]
		[Protected]
		NSString WeakVideoGravity { get; set; }

		[Field ("AVLayerVideoGravityResizeAspect")]
		NSString GravityResizeAspect { get; }

		[Field ("AVLayerVideoGravityResizeAspectFill")]
		NSString GravityResizeAspectFill { get; }

		[Field ("AVLayerVideoGravityResize")]
		NSString GravityResize { get; }

		[Export ("isReadyForDisplay")]
		bool ReadyForDisplay { get; }

		[MacCatalyst (13, 1)]
		[Export ("videoRect")]
		CGRect VideoRect { get; }

		[MacCatalyst (13, 1)]
		[Export ("pixelBufferAttributes", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary WeakPixelBufferAttributes { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPlayerLooper {
		[Static]
		[Export ("playerLooperWithPlayer:templateItem:timeRange:")]
		AVPlayerLooper FromPlayer (AVQueuePlayer player, AVPlayerItem itemToLoop, CMTimeRange loopRange);

		[Static]
		[Export ("playerLooperWithPlayer:templateItem:")]
		AVPlayerLooper FromPlayer (AVQueuePlayer player, AVPlayerItem itemToLoop);

		[Export ("initWithPlayer:templateItem:timeRange:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVQueuePlayer player, AVPlayerItem itemToLoop, CMTimeRange loopRange);

		[Export ("disableLooping")]
		void DisableLooping ();

		[Export ("loopCount")]
		nint LoopCount { get; }

		[Export ("loopingPlayerItems")]
		AVPlayerItem [] LoopingPlayerItems { get; }

		[Export ("status")]
		AVPlayerLooperStatus Status { get; }

		[NullAllowed, Export ("error")]
		NSError Error { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemTrack {
		[Export ("enabled", ArgumentSemantic.Assign)]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NullAllowed, Export ("assetTrack")]
		AVAssetTrack AssetTrack { get; }

		[MacCatalyst (13, 1)]
		[Export ("currentVideoFrameRate")]
		float CurrentVideoFrameRate { get; } // defined as 'float'

		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[Field ("AVPlayerItemTrackVideoFieldModeDeinterlaceFields")]
		NSString VideoFieldModeDeinterlaceFields { get; }

		[NoiOS]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[Export ("videoFieldMode"), NullAllowed]
		string VideoFieldMode { get; set; }
	}

	[iOS (14, 5), Mac (11, 3), TV (14, 5)]
	[MacCatalyst (14, 5)]
	[Flags]
	[Native]
	enum AVPlayerInterstitialEventRestrictions : ulong {
		None = 0,
		ConstrainsSeekingForwardInPrimaryContent = (1 << 0),
		RequiresPlaybackAtPreferredRateForAdvancement = (1 << 2),
		DefaultPolicy = None,
	}

	[TV (14, 5), Mac (11, 3), iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Watch (7, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPlayerInterstitialEvent {
		// Apple changed the API signature ?!?
		// [Static]
		// [Export ("interstitialEventWithPrimaryItem:time:templateItems:restrictions:resumptionOffset:")]
		// AVPlayerInterstitialEvent GetInterstitialEvent (AVPlayerItem primaryItem, CMTime time, AVPlayerItem[] templateItems, AVPlayerInterstitialEventRestrictions restrictions, CMTime resumptionOffset);

		// Apple changed the API signature ?!?
		// [Static]
		// [Export ("playerInterstitialEventWithPrimaryItem:time:interstitialTemplateItems:restrictions:resumptionOffset:")]
		// AVPlayerInterstitialEvent GetPlayerInterstitialEvent (AVPlayerItem primaryItem, CMTime time, AVPlayerItem[] interstitialTemplateItems, AVPlayerInterstitialEventRestrictions restrictions, CMTime resumptionOffset);

		// Apple changed the API signature ?!?
		// [Static]
		// [Export ("interstitialEventWithPrimaryItem:date:templateItems:restrictions:resumptionOffset:")]
		// AVPlayerInterstitialEvent GetInterstitialEvent (AVPlayerItem primaryItem, NSDate date, AVPlayerItem[] templateItems, AVPlayerInterstitialEventRestrictions restrictions, CMTime resumptionOffset);

		// Apple changed the API signature ?!?
		// [Static]
		// [Export ("playerInterstitialEventWithPrimaryItem:date:interstitialTemplateItems:restrictions:resumptionOffset:")]
		// AVPlayerInterstitialEvent GetPlayerInterstitialEvent (AVPlayerItem primaryItem, NSDate date, AVPlayerItem[] interstitialTemplateItems, AVPlayerInterstitialEventRestrictions restrictions, CMTime resumptionOffset);

		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), Mac (12, 0), Watch (8, 0)]
		[Static]
		[Export ("interstitialEventWithPrimaryItem:identifier:date:templateItems:restrictions:resumptionOffset:playoutLimit:userDefinedAttributes:")]
		AVPlayerInterstitialEvent GetPlayerInterstitialEvent (AVPlayerItem primaryItem, [NullAllowed] string identifier, NSDate date, AVPlayerItem [] templateItems, AVPlayerInterstitialEventRestrictions restrictions, CMTime resumptionOffset, CMTime playoutLimit, [NullAllowed] NSDictionary userDefinedAttributes);

		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), Mac (12, 0), Watch (8, 0)]
		[Static]
		[Export ("interstitialEventWithPrimaryItem:identifier:time:templateItems:restrictions:resumptionOffset:playoutLimit:userDefinedAttributes:")]
		AVPlayerInterstitialEvent GetPlayerInterstitialEvent (AVPlayerItem primaryItem, [NullAllowed] string identifier, CMTime time, AVPlayerItem [] templateItems, AVPlayerInterstitialEventRestrictions restrictions, CMTime resumptionOffset, CMTime playoutLimit, [NullAllowed] NSDictionary userDefinedAttributes);

		[NullAllowed, Export ("primaryItem", ArgumentSemantic.Weak)]
		AVPlayerItem PrimaryItem { get; }

		[Export ("time")]
		CMTime Time { get; }

		[NullAllowed, Export ("date")]
		NSDate Date { get; }

		[Export ("templateItems")]
		AVPlayerItem [] TemplateItems { get; }

		[Export ("restrictions")]
		AVPlayerInterstitialEventRestrictions Restrictions { get; }

		[Export ("resumptionOffset")]
		CMTime ResumptionOffset { get; }

		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), Mac (12, 0), Watch (8, 0)]
		[Export ("identifier")]
		string Identifier { get; }

		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), Mac (12, 0), Watch (8, 0)]
		[Export ("playoutLimit")]
		CMTime PlayoutLimit { get; }

		// not a strong dictionary:
		// Storage for attributes defined by the client or the content vendor. Attribute names should begin with X- for uniformity with server insertion.
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0), Mac (12, 0), Watch (8, 0)]
		[Export ("userDefinedAttributes")]
		NSDictionary UserDefinedAttributes { get; }
	}

	[DisableDefaultCtor]
	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerInterstitialEventMonitor {

		[Notification]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("AVPlayerInterstitialEventMonitorEventsDidChangeNotification")]
		NSString EventsDidChangeNotification { get; }

		[Notification]
		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("AVPlayerInterstitialEventMonitorCurrentEventDidChangeNotification")]
		NSString CurrentEventDidChangeNotification { get; }

		[Static]
		[Export ("interstitialEventMonitorWithPrimaryPlayer:")]
		AVPlayerInterstitialEventMonitor InterstitialEventMonitorWithPrimaryPlayer (AVPlayer primaryPlayer);

		[Export ("initWithPrimaryPlayer:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVPlayer primaryPlayer);

		[NullAllowed, Export ("primaryPlayer", ArgumentSemantic.Weak)]
		AVPlayer PrimaryPlayer { get; }

		[Export ("interstitialPlayer")]
		AVQueuePlayer InterstitialPlayer { get; }

		[Export ("events")]
		AVPlayerInterstitialEvent [] Events { get; }

		[NullAllowed, Export ("currentEvent")]
		AVPlayerInterstitialEvent CurrentEvent { get; }
	}

	[DisableDefaultCtor]
	[TV (14, 5), Mac (11, 3), iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Watch (7, 4)]
	[BaseType (typeof (AVPlayerInterstitialEventMonitor))]
	interface AVPlayerInterstitialEventController {
		[Static]
		[Export ("interstitialEventControllerWithPrimaryPlayer:")]
		AVPlayerInterstitialEventController GetInterstitialEventController (AVPlayer primaryPlayer);

		[Export ("initWithPrimaryPlayer:")]
		NativeHandle Constructor (AVPlayer primaryPlayer);

		[NullAllowed, Export ("events", ArgumentSemantic.Copy)]
		AVPlayerInterstitialEvent [] Events { get; set; }

		[Export ("cancelCurrentEventWithResumptionOffset:")]
		void CancelCurrentEvent (CMTime resumptionOffset);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVAsynchronousKeyValueLoading {
		[Abstract]
		[Export ("statusOfValueForKey:error:")]
#if NET
		AVKeyValueStatus GetStatusOfValue (string forKey, out NSError error);
#else
		AVKeyValueStatus StatusOfValueForKeyerror (string key, [NullAllowed] IntPtr outError);
#endif
		[Abstract]
		[Export ("loadValuesAsynchronouslyForKeys:completionHandler:")]
		void LoadValuesAsynchronously (string [] keys, [NullAllowed] Action handler);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVPlayer))]
	interface AVQueuePlayer {

		[Static, Export ("queuePlayerWithItems:")]
		AVQueuePlayer FromItems (AVPlayerItem [] items);

		[Export ("initWithItems:")]
		NativeHandle Constructor (AVPlayerItem [] items);

		[Export ("items")]
		AVPlayerItem [] Items { get; }

		[Export ("advanceToNextItem")]
		void AdvanceToNextItem ();

		[Export ("canInsertItem:afterItem:")]
		bool CanInsert (AVPlayerItem item, [NullAllowed] AVPlayerItem afterItem);

		[Export ("insertItem:afterItem:")]
		void InsertItem (AVPlayerItem item, [NullAllowed] AVPlayerItem afterItem);

		[Export ("removeItem:")]
		void RemoveItem (AVPlayerItem item);

		[Export ("removeAllItems")]
		void RemoveAllItems ();
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface AVAudioSettings {
		[Field ("AVFormatIDKey")]
		NSString AVFormatIDKey { get; }

		[Field ("AVSampleRateKey")]
		NSString AVSampleRateKey { get; }

		[Field ("AVNumberOfChannelsKey")]
		NSString AVNumberOfChannelsKey { get; }

		[Field ("AVLinearPCMBitDepthKey")]
		NSString AVLinearPCMBitDepthKey { get; }

		[Field ("AVLinearPCMIsBigEndianKey")]
		NSString AVLinearPCMIsBigEndianKey { get; }

		[Field ("AVLinearPCMIsFloatKey")]
		NSString AVLinearPCMIsFloatKey { get; }

		[Field ("AVLinearPCMIsNonInterleaved")]
		NSString AVLinearPCMIsNonInterleaved { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAudioFileTypeKey")]
		NSString FileTypeKey { get; }

		[Field ("AVEncoderAudioQualityKey")]
		NSString AVEncoderAudioQualityKey { get; }

		[Field ("AVEncoderBitRateKey")]
		NSString AVEncoderBitRateKey { get; }

		[Field ("AVEncoderBitRatePerChannelKey")]
		NSString AVEncoderBitRatePerChannelKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVEncoderBitRateStrategyKey"), Internal]
		NSString AVEncoderBitRateStrategyKey { get; }

		[Field ("AVSampleRateConverterAlgorithmKey"), Internal]
		NSString AVSampleRateConverterAlgorithmKey { get; }

		[Field ("AVEncoderBitDepthHintKey")]
		NSString AVEncoderBitDepthHintKey { get; }

		[Field ("AVSampleRateConverterAudioQualityKey")]
		NSString AVSampleRateConverterAudioQualityKey { get; }

		[Field ("AVChannelLayoutKey")]
		NSString AVChannelLayoutKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAudioBitRateStrategy_Constant"), Internal]
		NSString _Constant { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAudioBitRateStrategy_LongTermAverage"), Internal]
		NSString _LongTermAverage { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAudioBitRateStrategy_VariableConstrained"), Internal]
		NSString _VariableConstrained { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVAudioBitRateStrategy_Variable"), Internal]
		NSString _Variable { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVSampleRateConverterAlgorithm_Normal"), Internal]
		NSString AVSampleRateConverterAlgorithm_Normal { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVSampleRateConverterAlgorithm_Mastering"), Internal]
		NSString AVSampleRateConverterAlgorithm_Mastering { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVSampleRateConverterAlgorithm_MinimumPhase")]
		NSString AVSampleRateConverterAlgorithm_MinimumPhase { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVEncoderAudioQualityForVBRKey"), Internal]
		NSString AVEncoderAudioQualityForVBRKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CALayer))]
	interface AVSampleBufferDisplayLayer {

		[NullAllowed]
		[Export ("controlTimebase", ArgumentSemantic.Retain)]
		CMTimebase ControlTimebase { get; set; }

		[Export ("videoGravity")]
		string VideoGravity { get; set; }

		[Export ("status")]
		AVQueuedSampleBufferRenderingStatus Status { get; }

		[Export ("error"), NullAllowed]
		NSError Error { get; }

		[Export ("readyForMoreMediaData")]
		bool ReadyForMoreMediaData { [Bind ("isReadyForMoreMediaData")] get; }

		[Export ("enqueueSampleBuffer:")]
		void Enqueue (CMSampleBuffer sampleBuffer);

#if !NET
		[Wrap ("Enqueue (sampleBuffer)", IsVirtual = true)]
		[Obsolete ("Use the 'Enqueue' method instead.")]
		void EnqueueSampleBuffer (CMSampleBuffer sampleBuffer);
#endif

		[Export ("flush")]
		void Flush ();

		[Export ("flushAndRemoveImage")]
		void FlushAndRemoveImage ();

		[Export ("requestMediaDataWhenReadyOnQueue:usingBlock:")]
		void RequestMediaData (DispatchQueue queue, Action handler);

#if !NET
		[Wrap ("RequestMediaData (queue, enqueuer)", IsVirtual = true)]
		[Obsolete ("Use the 'RequestMediaData' method instead.")]
		void RequestMediaDataWhenReadyOnQueue (DispatchQueue queue, Action enqueuer);
#endif

		[Export ("stopRequestingMediaData")]
		void StopRequestingMediaData ();

		// TODO: Remove (alongside others) when https://github.com/xamarin/xamarin-macios/issues/3213 is fixed and conformance to 'AVQueuedSampleBufferRendering' is restored.
		[MacCatalyst (13, 1)]
		[Export ("timebase", ArgumentSemantic.Retain)]
		CMTimebase Timebase { get; }

		[Field ("AVSampleBufferDisplayLayerFailedToDecodeNotification")]
		[Notification]
		NSString FailedToDecodeNotification { get; }

		[Field ("AVSampleBufferDisplayLayerFailedToDecodeNotificationErrorKey")]
		NSString FailedToDecodeNotificationErrorKey { get; }

		// AVSampleBufferDisplayLayerImageProtection

		[TV (12, 2), NoWatch, iOS (12, 2)]
		[MacCatalyst (13, 1)]
		[Export ("preventsCapture")]
		bool PreventsCapture { get; set; }

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("preventsDisplaySleepDuringVideoPlayback")]
		bool PreventsDisplaySleepDuringVideoPlayback { get; set; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("requiresFlushToResumeDecoding")]
		bool RequiresFlushToResumeDecoding { get; }

		[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVSampleBufferDisplayLayerRequiresFlushToResumeDecodingDidChangeNotification")]
		[Notification]
		NSString RequiresFlushToResumeDecodingDidChangeNotification { get; }

		[TV (14, 5), Watch (7, 4), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("AVSampleBufferDisplayLayerOutputObscuredDueToInsufficientExternalProtectionDidChangeNotification")]
		[Notification]
		NSString OutputObscuredDueToInsufficientExternalProtectionDidChangeNotification { get; }

	}

	[NoWatch]
	[TV (14, 5), Mac (11, 3), iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Category]
	[BaseType (typeof (AVSampleBufferDisplayLayer))]
	interface AVSampleBufferDisplayLayer_ProtectedContent {
		[Export ("outputObscuredDueToInsufficientExternalProtection")]
		bool GetOutputObscuredDueToInsufficientExternalProtection ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CALayer))]
	interface AVSynchronizedLayer {
		[Static, Export ("synchronizedLayerWithPlayerItem:")]
		AVSynchronizedLayer FromPlayerItem (AVPlayerItem playerItem);

		[NullAllowed] // by default this property is null
		[Export ("playerItem", ArgumentSemantic.Retain)]
		AVPlayerItem PlayerItem { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVSpeechSynthesisVoice : NSSecureCoding {

		[Static, Export ("speechVoices")]
		AVSpeechSynthesisVoice [] GetSpeechVoices ();

		[Static, Export ("currentLanguageCode")]
		string CurrentLanguageCode { get; }

		[return: NullAllowed]
		[Static, Export ("voiceWithLanguage:")]
		AVSpeechSynthesisVoice FromLanguage ([NullAllowed] string language);

		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Static, Export ("voiceWithIdentifier:")]
		AVSpeechSynthesisVoice FromIdentifier (string identifier);

		[Export ("language", ArgumentSemantic.Copy)]
		string Language { get; }

		[MacCatalyst (13, 1)]
		[Export ("identifier")]
		string Identifier { get; }

		[MacCatalyst (13, 1)]
		[Export ("name")]
		string Name { get; }

		[MacCatalyst (13, 1)]
		[Export ("quality")]
		AVSpeechSynthesisVoiceQuality Quality { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVSpeechSynthesisVoiceIdentifierAlex")]
		NSString IdentifierAlex { get; }

		[MacCatalyst (13, 1)]
		[Field ("AVSpeechSynthesisIPANotationAttribute")]
		NSString IpaNotationAttribute { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("gender")]
		AVSpeechSynthesisVoiceGender Gender { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("audioFileSettings")]
		NSDictionary<NSString, NSObject> AudioFileSettings { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVSpeechUtterance : NSCopying, NSSecureCoding {

		[Static, Export ("speechUtteranceWithString:")]
		AVSpeechUtterance FromString (string speechString);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("speechUtteranceWithAttributedString:")]
		AVSpeechUtterance FromString (NSAttributedString speechString);

		[Export ("initWithString:")]
		NativeHandle Constructor (string speechString);

		[MacCatalyst (13, 1)]
		[Export ("initWithAttributedString:")]
		NativeHandle Constructor (NSAttributedString speechString);

		[NullAllowed] // by default this property is null
		[Export ("voice", ArgumentSemantic.Retain)]
		AVSpeechSynthesisVoice Voice { get; set; }

		[Export ("speechString", ArgumentSemantic.Copy)]
		string SpeechString { get; }

		[MacCatalyst (13, 1)]
		[Export ("attributedSpeechString")]
		NSAttributedString AttributedSpeechString { get; }

		[Export ("rate")]
		float Rate { get; set; } // defined as 'float'

		[Export ("pitchMultiplier")]
		float PitchMultiplier { get; set; } // defined as 'float'

		[Export ("volume")]
		float Volume { get; set; } // defined as 'float'

		[Export ("preUtteranceDelay")]
		double PreUtteranceDelay { get; set; }

		[Export ("postUtteranceDelay")]
		double PostUtteranceDelay { get; set; }

		[Field ("AVSpeechUtteranceMinimumSpeechRate")]
		float MinimumSpeechRate { get; } // defined as 'float'

		[Field ("AVSpeechUtteranceMaximumSpeechRate")]
		float MaximumSpeechRate { get; } // defined as 'float'

		[Field ("AVSpeechUtteranceDefaultSpeechRate")]
		float DefaultSpeechRate { get; } // defined as 'float'

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("prefersAssistiveTechnologySettings")]
		bool PrefersAssistiveTechnologySettings { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (AVSpeechSynthesizerDelegate) })]
	interface AVSpeechSynthesizer {

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		[Protocolize]
		AVSpeechSynthesizerDelegate Delegate { get; set; }

		[Export ("speaking")]
		bool Speaking { [Bind ("isSpeaking")] get; }

		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; }

		[Watch (6, 0), TV (13, 0), Mac (12, 0), iOS (13, 0), MacCatalyst (15, 0)]
		[Export ("usesApplicationAudioSession")]
		bool UsesApplicationAudioSession { get; set; }

		[Watch (6, 0), NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("mixToTelephonyUplink")]
		bool MixToTelephonyUplink { get; set; }

		[Export ("speakUtterance:")]
		void SpeakUtterance (AVSpeechUtterance utterance);

		[Async]
		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("writeUtterance:toBufferCallback:")]
		void WriteUtterance (AVSpeechUtterance utterance, Action<AVAudioBuffer> bufferCallback);

		[Export ("stopSpeakingAtBoundary:")]
		bool StopSpeaking (AVSpeechBoundary boundary);

		[Export ("pauseSpeakingAtBoundary:")]
		bool PauseSpeaking (AVSpeechBoundary boundary);

		[Export ("continueSpeaking")]
		bool ContinueSpeaking ();

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("outputChannels", ArgumentSemantic.Retain)]
		AVAudioSessionChannelDescription [] OutputChannels { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
	interface AVSpeechSynthesizerDelegate {
		[Export ("speechSynthesizer:didStartSpeechUtterance:")]
		[EventArgs ("AVSpeechSynthesizerUterance")]
		void DidStartSpeechUtterance (AVSpeechSynthesizer synthesizer, AVSpeechUtterance utterance);

		[Export ("speechSynthesizer:didFinishSpeechUtterance:")]
		[EventArgs ("AVSpeechSynthesizerUterance")]
		void DidFinishSpeechUtterance (AVSpeechSynthesizer synthesizer, AVSpeechUtterance utterance);

		[Export ("speechSynthesizer:didPauseSpeechUtterance:")]
		[EventArgs ("AVSpeechSynthesizerUterance")]
		void DidPauseSpeechUtterance (AVSpeechSynthesizer synthesizer, AVSpeechUtterance utterance);

		[Export ("speechSynthesizer:didContinueSpeechUtterance:")]
		[EventArgs ("AVSpeechSynthesizerUterance")]
		void DidContinueSpeechUtterance (AVSpeechSynthesizer synthesizer, AVSpeechUtterance utterance);

		[Export ("speechSynthesizer:didCancelSpeechUtterance:")]
		[EventArgs ("AVSpeechSynthesizerUterance")]
		void DidCancelSpeechUtterance (AVSpeechSynthesizer synthesizer, AVSpeechUtterance utterance);

		[Export ("speechSynthesizer:willSpeakRangeOfSpeechString:utterance:")]
		[EventArgs ("AVSpeechSynthesizerWillSpeak")]
		void WillSpeakRangeOfSpeechString (AVSpeechSynthesizer synthesizer, NSRange characterRange, AVSpeechUtterance utterance);
	}

	[NoWatch, NoTV, Mac (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetDownloadStorageManager {
		[Static]
		[Export ("sharedDownloadStorageManager")]
		AVAssetDownloadStorageManager SharedDownloadStorageManager { get; }

		[Export ("setStorageManagementPolicy:forURL:")]
		void SetStorageManagementPolicy (AVAssetDownloadStorageManagementPolicy storageManagementPolicy, NSUrl downloadStorageUrl);

		[Export ("storageManagementPolicyForURL:")]
		[return: NullAllowed]
		AVAssetDownloadStorageManagementPolicy GetStorageManagementPolicy (NSUrl downloadStorageUrl);
	}

	[NoWatch, NoTV, Mac (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetDownloadStorageManagementPolicy : NSCopying, NSMutableCopying {
		[Internal]
		[Export ("priority")]
		NSString _Priority { get; [NotImplemented] set; }

		[Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; [NotImplemented] set; }
	}

	[NoWatch, NoTV, Mac (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAssetDownloadStorageManagementPolicy))]
	[DisableDefaultCtor]
	interface AVMutableAssetDownloadStorageManagementPolicy {
		[Internal]
		[Export ("priority")]
		NSString _Priority { get; set; }

		[Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; set; }
	}

	[NoWatch]
	[NoTV]
	[Mac (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSUrlSessionTask))]
	[DisableDefaultCtor] // not meant to be user createable
	interface AVAssetDownloadTask {

		[Export ("URLAsset")]
		AVUrlAsset UrlAsset { get; }

		[Deprecated (PlatformName.iOS, 10, 0)]
		[NoMacCatalyst, NoMac]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("destinationURL")]
		NSUrl DestinationUrl { get; }

		[NullAllowed, Export ("options")]
		NSDictionary<NSString, NSObject> Options { get; }

		[Export ("loadedTimeRanges")]
		NSValue [] LoadedTimeRanges { get; }

	}

	[Mac (12, 0), NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSUrlSessionTask))]
	[DisableDefaultCtor]
	interface AVAggregateAssetDownloadTask {
		[Export ("URLAsset")]
		AVUrlAsset UrlAsset { get; }
	}

	[NoWatch, Mac (11, 3)]
#if NET
	[NoTV]
	[MacCatalyst (13, 1)]
#else
	[Obsoleted (PlatformName.TvOS, 12, 0)]
#endif
	[Static, Internal]
	interface AVAssetDownloadTaskKeys {
		[Mac (12, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVAssetDownloadTaskMinimumRequiredMediaBitrateKey")]
		NSString MinimumRequiredMediaBitrateKey { get; }

		[Mac (12, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVAssetDownloadTaskMediaSelectionKey")]
		NSString MediaSelectionKey { get; }

		[NoWatch, NoTV, iOS (13, 0), Mac (12, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVAssetDownloadTaskMediaSelectionPrefersMultichannelKey")]
		NSString MediaSelectionPrefersMultichannelKey { get; }

		[NoWatch, NoTV, iOS (14, 0), Mac (12, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVAssetDownloadTaskPrefersHDRKey")]
		NSString PrefersHdrKey { get; }

		[NoWatch, NoTV, Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("AVAssetDownloadTaskPrefersLosslessAudioKey")]
		NSString PrefersLosslessAudioKey { get; }

		[NoWatch, NoTV, iOS (14, 0), Mac (12, 0)]
		[MacCatalyst (14, 0)]
		[Field ("AVAssetDownloadTaskMinimumRequiredPresentationSizeKey")]
		NSString MinimumRequiredPresentationSizeKey { get; }
	}

	[Mac (12, 0)]
	[NoWatch]
#if NET
	[NoTV]
	[MacCatalyst (13, 1)]
#else
	[Obsoleted (PlatformName.TvOS, 12, 0)]
#endif
	[StrongDictionary ("AVAssetDownloadTaskKeys")]
	interface AVAssetDownloadOptions {
		NSNumber MinimumRequiredMediaBitrate { get; set; }
		AVMediaSelection MediaSelection { get; set; }
		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		bool MediaSelectionPrefersMultichannel { get; set; }
		[NoWatch, NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		bool PrefersHdr { get; set; }
		[NoWatch, NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		CGSize MinimumRequiredPresentationSize { get; set; }
	}

	[NoTV]
	[NoWatch]
	[Mac (12, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSUrlSession), Name = "AVAssetDownloadURLSession")]
	interface AVAssetDownloadUrlSession {
		[Static]
		[return: ForcedType]
		[Export ("sessionWithConfiguration:assetDownloadDelegate:delegateQueue:")]
		AVAssetDownloadUrlSession CreateSession (NSUrlSessionConfiguration configuration, [NullAllowed] IAVAssetDownloadDelegate @delegate, [NullAllowed] NSOperationQueue delegateQueue);

		[Deprecated (PlatformName.iOS, 10, 0, message: "Please use 'GetAssetDownloadTask (AVUrlAsset, string, NSData, NSDictionary<NSString, NSObject>)'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use 'GetAssetDownloadTask (AVUrlAsset, string, NSData, NSDictionary<NSString, NSObject>)'.")]
		[Export ("assetDownloadTaskWithURLAsset:destinationURL:options:")]
		[return: NullAllowed]
		AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, NSUrl destinationUrl, [NullAllowed] NSDictionary options);

		[Wrap ("GetAssetDownloadTask (urlAsset, destinationUrl, options.GetDictionary ())")]
		[return: NullAllowed]
		AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, NSUrl destinationUrl, AVAssetDownloadOptions options);

		[MacCatalyst (13, 1)]
		[Export ("assetDownloadTaskWithURLAsset:assetTitle:assetArtworkData:options:")]
		[return: NullAllowed]
		AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, string title, [NullAllowed] NSData artworkData, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Wrap ("GetAssetDownloadTask (urlAsset, title, artworkData, options.GetDictionary ())")]
		[return: NullAllowed]
		AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, string title, [NullAllowed] NSData artworkData, AVAssetDownloadOptions options);

		[Mac (12, 0), NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("aggregateAssetDownloadTaskWithURLAsset:mediaSelections:assetTitle:assetArtworkData:options:")]
		[return: NullAllowed]
		AVAggregateAssetDownloadTask GetAssetDownloadTask (AVUrlAsset URLAsset, AVMediaSelection [] mediaSelections, string title, [NullAllowed] NSData artworkData, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Watch (8, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("assetDownloadTaskWithConfiguration:")]
		AVAssetDownloadTask GetAssetDownloadTask (AVAssetDownloadConfiguration downloadConfiguration);
	}

	interface IAVAssetDownloadDelegate { }

	[NoTV]
	[NoWatch]
	[Mac (12, 0), MacCatalyst (15, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVAssetDownloadDelegate : NSUrlSessionTaskDelegate {
		[Export ("URLSession:assetDownloadTask:didLoadTimeRange:totalTimeRangesLoaded:timeRangeExpectedToLoad:")]
		void DidLoadTimeRange (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, CMTimeRange timeRange, NSValue [] loadedTimeRanges, CMTimeRange timeRangeExpectedToLoad);

		[Export ("URLSession:assetDownloadTask:didResolveMediaSelection:")]
		void DidResolveMediaSelection (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, AVMediaSelection resolvedMediaSelection);

		[MacCatalyst (15, 0)]
		[Export ("URLSession:assetDownloadTask:didFinishDownloadingToURL:")]
		void DidFinishDownloadingToUrl (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, NSUrl location);

		[MacCatalyst (15, 0)]
		[Export ("URLSession:aggregateAssetDownloadTask:willDownloadToURL:")]
		void WillDownloadToUrl (NSUrlSession session, AVAggregateAssetDownloadTask aggregateAssetDownloadTask, NSUrl location);

		[MacCatalyst (15, 0)]
		[Export ("URLSession:aggregateAssetDownloadTask:didCompleteForMediaSelection:")]
		void DidCompleteForMediaSelection (NSUrlSession session, AVAggregateAssetDownloadTask aggregateAssetDownloadTask, AVMediaSelection mediaSelection);

		[MacCatalyst (15, 0)]
		[Export ("URLSession:aggregateAssetDownloadTask:didLoadTimeRange:totalTimeRangesLoaded:timeRangeExpectedToLoad:forMediaSelection:")]
		void DidLoadTimeRange (NSUrlSession session, AVAggregateAssetDownloadTask aggregateAssetDownloadTask, CMTimeRange timeRange, NSValue [] loadedTimeRanges, CMTimeRange timeRangeExpectedToLoad, AVMediaSelection mediaSelection);

		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Export ("URLSession:assetDownloadTask:willDownloadVariants:")]
		void WillDownloadVariants (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, AVAssetVariant [] variants);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVOutputSettingsAssistant {
		[MacCatalyst (13, 1)]
		[Static, Export ("availableOutputSettingsPresets")]
		string [] AvailableOutputSettingsPresets { get; }

		[return: NullAllowed]
		[Static, Export ("outputSettingsAssistantWithPreset:")]
		AVOutputSettingsAssistant FromPreset (string presetIdentifier);

		[Export ("audioSettings", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary WeakAudioSettings { get; }

		[Wrap ("WeakAudioSettings")]
		[NullAllowed]
		AudioSettings AudioSettings { get; }

		[Export ("videoSettings", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary WeakVideoSettings { get; }

		[Wrap ("WeakVideoSettings")]
		AVVideoSettingsCompressed CompressedVideoSettings { get; }

		[Wrap ("WeakVideoSettings")]
		AVVideoSettingsUncompressed UnCompressedVideoSettings { get; }

		[Export ("outputFileType", ArgumentSemantic.Copy)]
		string OutputFileType { get; }

		[Export ("sourceAudioFormat", ArgumentSemantic.Copy)]
		[NullAllowed]
		CMAudioFormatDescription SourceAudioFormat { get; set; }

		[Export ("sourceVideoFormat", ArgumentSemantic.Copy)]
		[NullAllowed]
		CMVideoFormatDescription SourceVideoFormat { get; set; }

		[Export ("sourceVideoAverageFrameDuration", ArgumentSemantic.Copy)]
		CMTime SourceVideoAverageFrameDuration { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("sourceVideoMinFrameDuration", ArgumentSemantic.Copy)]
		CMTime SourceVideoMinFrameDuration { get; set; }

		[Internal, Field ("AVOutputSettingsPreset640x480")]
		NSString _Preset640x480 { get; }

		[Internal, Field ("AVOutputSettingsPreset960x540")]
		NSString _Preset960x540 { get; }

		[Internal, Field ("AVOutputSettingsPreset1280x720")]
		NSString _Preset1280x720 { get; }

		[Internal, Field ("AVOutputSettingsPreset1920x1080")]
		NSString _Preset1920x1080 { get; }

		[MacCatalyst (13, 1)]
		[Internal, Field ("AVOutputSettingsPreset3840x2160")]
		NSString _Preset3840x2160 { get; }

		[MacCatalyst (13, 1)]
		[Internal, Field ("AVOutputSettingsPresetHEVC1920x1080")]
		NSString _PresetHevc1920x1080 { get; }

		[MacCatalyst (13, 1)]
		[Internal, Field ("AVOutputSettingsPresetHEVC3840x2160")]
		NSString _PresetHevc3840x2160 { get; }
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVMediaSelection : NSCopying, NSMutableCopying {

		[NullAllowed, Export ("asset", ArgumentSemantic.Weak)]
		AVAsset Asset { get; }

		[Export ("selectedMediaOptionInMediaSelectionGroup:")]
		[return: NullAllowed]
		AVMediaSelectionOption GetSelectedMediaOption (AVMediaSelectionGroup mediaSelectionGroup);

		[Export ("mediaSelectionCriteriaCanBeAppliedAutomaticallyToMediaSelectionGroup:")]
		bool CriteriaCanBeAppliedAutomaticallyToMediaSelectionGroup (AVMediaSelectionGroup mediaSelectionGroup);
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVMediaSelection))]
	interface AVMutableMediaSelection {

		[Export ("selectMediaOption:inMediaSelectionGroup:")]
		void SelectMediaOption ([NullAllowed] AVMediaSelectionOption mediaSelectionOption, AVMediaSelectionGroup mediaSelectionGroup);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAudioSequencer {

		[Export ("initWithAudioEngine:")]
		NativeHandle Constructor (AVAudioEngine engine);

		[Export ("loadFromURL:options:error:")]
		bool Load (NSUrl fileUrl, AVMusicSequenceLoadOptions options, out NSError outError);

		[Export ("loadFromData:options:error:")]
		bool Load (NSData data, AVMusicSequenceLoadOptions options, out NSError outError);

		[Export ("writeToURL:SMPTEResolution:replaceExisting:error:")]
		bool Write (NSUrl fileUrl, nint resolution, bool replace, out NSError outError);

		[Export ("dataWithSMPTEResolution:error:")]
		NSData GetData (nint smpteResolution, out NSError outError);

		[Export ("secondsForBeats:")]
		double GetSeconds (double beats);

		[Export ("beatsForSeconds:")]
		double GetBeats (double seconds);

		[Export ("tracks")]
		AVMusicTrack [] Tracks { get; }

		[Export ("tempoTrack")]
		AVMusicTrack TempoTrack { get; }

		[Export ("userInfo")]
		NSDictionary<NSString, NSObject> UserInfo { get; }

		// AVAudioSequencer_Player Category
		// Inlined due to properties

		[Export ("currentPositionInSeconds")]
		double CurrentPositionInSeconds { get; set; }

		[Export ("currentPositionInBeats")]
		double CurrentPositionInBeats { get; set; }

		[Export ("playing")]
		bool Playing { [Bind ("isPlaying")] get; }

		[Export ("rate")]
		float Rate { get; set; }

		[Export ("hostTimeForBeats:error:")]
		ulong GetHostTime (double inBeats, out NSError outError);

		[Export ("beatsForHostTime:error:")]
		double GetBeats (ulong inHostTime, out NSError outError);

		[Export ("prepareToPlay")]
		void PrepareToPlay ();

		[Export ("startAndReturnError:")]
		bool Start (out NSError outError);

		[Export ("stop")]
		void Stop ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Docs/headers do not state that init is disallowed but if 
						 // you get an instance that way and try to use it, it will inmediatelly crash also tested in ObjC app same result
	interface AVMusicTrack {

		[NullAllowed, Export ("destinationAudioUnit", ArgumentSemantic.Retain)]
		AVAudioUnit DestinationAudioUnit { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("destinationMIDIEndpoint")]
		uint DestinationMidiEndpoint { get; set; }

		[Export ("loopRange", ArgumentSemantic.Assign)]
		AVBeatRange LoopRange { get; set; }

		[Export ("loopingEnabled")]
		bool LoopingEnabled { [Bind ("isLoopingEnabled")] get; set; }

		[Export ("numberOfLoops", ArgumentSemantic.Assign)]
		nint NumberOfLoops { get; set; }

		[Export ("offsetTime")]
		double OffsetTime { get; set; }

		[Export ("muted")]
		bool Muted { [Bind ("isMuted")] get; set; }

		[Export ("soloed")]
		bool Soloed { [Bind ("isSoloed")] get; set; }

		[Export ("lengthInBeats")]
		double LengthInBeats { get; set; }

		[Export ("lengthInSeconds")]
		double LengthInSeconds { get; set; }

		[Export ("timeResolution")]
		nuint TimeResolution { get; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface AVAudioUnitType {

		[Field ("AVAudioUnitTypeOutput")]
		NSString Output { get; }

		[Field ("AVAudioUnitTypeMusicDevice")]
		NSString MusicDevice { get; }

		[Field ("AVAudioUnitTypeMusicEffect")]
		NSString MusicEffect { get; }

		[Field ("AVAudioUnitTypeFormatConverter")]
		NSString FormatConverter { get; }

		[Field ("AVAudioUnitTypeEffect")]
		NSString Effect { get; }

		[Field ("AVAudioUnitTypeMixer")]
		NSString Mixer { get; }

		[Field ("AVAudioUnitTypePanner")]
		NSString Panner { get; }

		[Field ("AVAudioUnitTypeGenerator")]
		NSString Generator { get; }

		[Field ("AVAudioUnitTypeOfflineEffect")]
		NSString OfflineEffect { get; }

		[Field ("AVAudioUnitTypeMIDIProcessor")]
		NSString MidiProcessor { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAudioUnitComponent {

		[Export ("name")]
		string Name { get; }

		[Export ("typeName")]
		string TypeName { get; }

		[Export ("localizedTypeName")]
		string LocalizedTypeName { get; }

		[Export ("manufacturerName")]
		string ManufacturerName { get; }

		[Export ("version")]
		nuint Version { get; }

		[Export ("versionString")]
		string VersionString { get; }

		[Export ("sandboxSafe")]
		bool SandboxSafe { [Bind ("isSandboxSafe")] get; }

		[Export ("hasMIDIInput")]
		bool HasMidiInput { get; }

		[Export ("hasMIDIOutput")]
		bool HasMidiOutput { get; }

		[Export ("audioComponent")]
		AudioComponent AudioComponent { get; }

		[MacCatalyst (15, 0), NoiOS, NoTV, NoWatch]
		[Export ("availableArchitectures")]
		NSNumber [] AvailableArchitectures { get; }

		[MacCatalyst (15, 0), NoiOS, NoTV, NoWatch]
		[Export ("userTagNames", ArgumentSemantic.Copy)]
		string [] UserTagNames { get; set; }

		[MacCatalyst (15, 0), NoiOS, NoTV, NoWatch]
		[NullAllowed, Export ("iconURL")]
		NSUrl IconUrl { get; }

		[MacCatalyst (15, 0), NoiOS, NoTV, NoWatch]
		[NullAllowed, Export ("icon")]
		UIImage Icon { get; }

		[MacCatalyst (15, 0), NoiOS, NoTV, NoWatch]
		[Export ("passesAUVal")]
		bool PassesAUVal { get; }

		[MacCatalyst (15, 0), NoiOS, NoTV, NoWatch]
		[Export ("hasCustomView")]
		bool HasCustomView { get; }

		[MacCatalyst (15, 0), NoiOS, NoTV, NoWatch]
		[Export ("configurationDictionary")]
		NSDictionary WeakConfigurationDictionary { get; }

		[MacCatalyst (15, 0), NoiOS, NoTV, NoWatch]
		[Export ("supportsNumberInputChannels:outputChannels:")]
		bool SupportsNumberInputChannels (nint numInputChannels, nint numOutputChannels);

		[Export ("allTagNames")]
		string [] AllTagNames { get; }

		[Export ("audioComponentDescription")]
		AudioComponentDescription AudioComponentDescription { get; }

		[Field ("AVAudioUnitComponentTagsDidChangeNotification")]
		[Notification]
		NSString TagsDidChangeNotification { get; }
	}

	delegate bool AVAudioUnitComponentFilter (AVAudioUnitComponent comp, ref bool stop);

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // for binary compatibility this is added in AVCompat.cs w/[Obsolete]
	interface AVAudioUnitComponentManager {

		[Export ("tagNames")]
		string [] TagNames { get; }

		[Export ("standardLocalizedTagNames")]
		string [] StandardLocalizedTagNames { get; }

		[Static]
		[Export ("sharedAudioUnitComponentManager")]
		AVAudioUnitComponentManager SharedInstance { get; }

		[Export ("componentsMatchingPredicate:")]
		AVAudioUnitComponent [] GetComponents (NSPredicate predicate);

		[Export ("componentsPassingTest:")]
		AVAudioUnitComponent [] GetComponents (AVAudioUnitComponentFilter testHandler);

		[Export ("componentsMatchingDescription:")]
		AVAudioUnitComponent [] GetComponents (AudioComponentDescription desc);

		[Notification]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVAudioUnitComponentManagerRegistrationsChangedNotification")]
		NSString RegistrationsChangedNotification { get; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface AVAudioUnitManufacturerName {
		[Field ("AVAudioUnitManufacturerNameApple")]
		[MacCatalyst (13, 1)]
		NSString Apple { get; }
	}

	// FIXME: Unsure about if CMMetadataFormatDescription will be an INativeObject and will need manual binding for Classic
	[NoMac]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch]
	[NoTV]
	[BaseType (typeof (AVCaptureInput))]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: Format description is required.
	interface AVCaptureMetadataInput {

		[Internal]
		[Static]
		[Export ("metadataInputWithFormatDescription:clock:")] // FIXME: Add CMMetadataFormatDescription
		AVCaptureMetadataInput MetadataInputWithFormatDescription (IntPtr /*CMMetadataFormatDescription*/ desc, CMClock clock);

		[Internal]
		[Export ("initWithFormatDescription:clock:")] // FIXME: Add CMMetadataFormatDescription
		NativeHandle Constructor (IntPtr /*CMMetadataFormatDescription*/ desc, CMClock clock);

		[Export ("appendTimedMetadataGroup:error:")]
		bool AppendTimedMetadataGroup (AVTimedMetadataGroup metadata, out NSError outError);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVAsynchronousCIImageFilteringRequest : NSCopying {

		[Export ("renderSize")]
		CGSize RenderSize { get; }

		[Export ("compositionTime")]
		CMTime CompositionTime { get; }

		[Export ("sourceImage")]
		CIImage SourceImage { get; }

		[Export ("finishWithImage:context:")]
		void Finish (CIImage filteredImage, [NullAllowed] CIContext context);

		[Export ("finishWithError:")]
		void Finish (NSError error);
	}

	[NoiOS, NoWatch, NoMac]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVContentProposal : NSCopying {
		[Export ("contentTimeForTransition")]
		CMTime ContentTimeForTransition { get; }

		[Export ("automaticAcceptanceInterval")]
		double AutomaticAcceptanceInterval { get; set; }

		[Export ("title")]
		string Title { get; }

		[NullAllowed, Export ("previewImage")]
		UIImage PreviewImage { get; }

		[NullAllowed, Export ("URL", ArgumentSemantic.Assign)]
		NSUrl Url { get; set; }

		[Export ("metadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] Metadata { get; set; }

		[Export ("initWithContentTimeForTransition:title:previewImage:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CMTime contentTimeForTransition, string title, [NullAllowed] UIImage previewImage);
	}

	partial interface IAVContentKeySessionDelegate { }

	[Watch (7, 0)]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVContentKeySessionDelegate {
		[Abstract]
		[Export ("contentKeySession:didProvideContentKeyRequest:")]
		void DidProvideContentKeyRequest (AVContentKeySession session, AVContentKeyRequest keyRequest);

		[Export ("contentKeySession:didProvideRenewingContentKeyRequest:")]
		void DidProvideRenewingContentKeyRequest (AVContentKeySession session, AVContentKeyRequest keyRequest);

		[Export ("contentKeySession:didProvidePersistableContentKeyRequest:")]
		void DidProvidePersistableContentKeyRequest (AVContentKeySession session, AVPersistableContentKeyRequest keyRequest);

		[Export ("contentKeySession:contentKeyRequest:didFailWithError:")]
		void DidFail (AVContentKeySession session, AVContentKeyRequest keyRequest, NSError err);

		[Export ("contentKeySession:shouldRetryContentKeyRequest:reason:")]
		bool ShouldRetryContentKeyRequest (AVContentKeySession session, AVContentKeyRequest keyRequest, string retryReason);

		[Export ("contentKeySessionContentProtectionSessionIdentifierDidChange:")]
		void DidChange (AVContentKeySession session);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("contentKeySession:didUpdatePersistableContentKey:forContentKeyIdentifier:")]
		void DidUpdate (AVContentKeySession session, NSData persistableContentKey, NSObject keyIdentifier);

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("contentKeySession:contentKeyRequestDidSucceed:")]
		void DidSucceed (AVContentKeySession session, AVContentKeyRequest keyRequest);

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("contentKeySessionDidGenerateExpiredSessionReport:")]
		void DidGenerateExpiredSessionReport (AVContentKeySession session);
	}

	partial interface IAVContentKeyRecipient { }

	[Watch (7, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface AVContentKeyRecipient {
		[TV (14, 5), Mac (11, 3), iOS (14, 5), Watch (7, 4)]
		[MacCatalyst (14, 5)]
		[Export ("contentKeySession:didProvideContentKey:")]
		void DidProvideContentKey (AVContentKeySession contentKeySession, AVContentKey contentKey);

		[Abstract]
		[Export ("mayRequireContentKeysForMediaDataProcessing")]
		bool MayRequireContentKeysForMediaDataProcessing { get; }
	}

	[Watch (7, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface AVContentKeySession {

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("contentKeySessionWithKeySystem:")]
		AVContentKeySession Create (string keySystem);

		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("contentKeySessionWithKeySystem:storageDirectoryAtURL:")]
		AVContentKeySession Create (NSString keySystem, NSUrl storageUrl);

		[Static]
		[Wrap ("Create (keySystem.GetConstant ()!, storageUrl)")]
		AVContentKeySession Create (AVContentKeySystem keySystem, NSUrl storageUrl);

		[Export ("setDelegate:queue:")]
		void SetDelegate ([NullAllowed] IAVContentKeySessionDelegate newDelegate, [NullAllowed] DispatchQueue delegateQueue);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IAVContentKeySessionDelegate Delegate { get; }

		[NullAllowed, Export ("delegateQueue")]
		DispatchQueue DelegateQueue { get; }

		[NullAllowed, Export ("storageURL")]
		NSUrl StorageUrl { get; }

		[Protected]
		[Export ("keySystem")]
		NSString KeySystemConstant { get; }

		[Wrap ("AVContentKeySystemExtensions.GetValue (this.KeySystemConstant)")]
		AVContentKeySystem KeySystem { get; }

		[Export ("expire")]
		void Expire ();

		[NullAllowed, Export ("contentProtectionSessionIdentifier")]
		NSData ContentProtectionSessionIdentifier { get; }

		[Export ("processContentKeyRequestWithIdentifier:initializationData:options:")]
		void ProcessContentKeyRequest ([NullAllowed] NSObject identifier, [NullAllowed] NSData initializationData, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("renewExpiringResponseDataForContentKeyRequest:")]
		void RenewExpiringResponseData (AVContentKeyRequest contentKeyRequest);

		[Async]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("makeSecureTokenForExpirationDateOfPersistableContentKey:completionHandler:")]
		void MakeSecureToken (NSData persistableContentKeyData, Action<NSData, NSError> handler);

		[Async]
		[NoTV, iOS (12, 2)]
		[MacCatalyst (13, 1)]
		[Export ("invalidatePersistableContentKey:options:completionHandler:")]
		void InvalidatePersistableContentKey (NSData persistableContentKeyData, [NullAllowed] NSDictionary options, Action<NSData, NSError> handler);

		[Async]
		[NoTV, NoMac, iOS (12, 2)]
		[MacCatalyst (13, 1)]
		[Wrap ("InvalidatePersistableContentKey (persistableContentKeyData, options.GetDictionary (), handler)")]
		void InvalidatePersistableContentKey (NSData persistableContentKeyData, [NullAllowed] AVContentKeySessionServerPlaybackContextOptions options, Action<NSData, NSError> handler);

		[Async]
		[NoTV, iOS (12, 2)]
		[MacCatalyst (13, 1)]
		[Export ("invalidateAllPersistableContentKeysForApp:options:completionHandler:")]
		void InvalidateAllPersistableContentKeys (NSData appIdentifier, [NullAllowed] NSDictionary options, Action<NSData, NSError> handler);

		[Async]
		[NoTV, NoMac, iOS (12, 2)]
		[MacCatalyst (13, 1)]
		[Wrap ("InvalidateAllPersistableContentKeys (appIdentifier, options.GetDictionary (), handler)")]
		void InvalidateAllPersistableContentKeys (NSData appIdentifier, [NullAllowed] AVContentKeySessionServerPlaybackContextOptions options, Action<NSData, NSError> handler);

		#region AVContentKeySession_AVContentKeySessionPendingExpiredSessionReports

		// binded because they are static and from a category.
		[Static]
		[Export ("pendingExpiredSessionReportsWithAppIdentifier:storageDirectoryAtURL:")]
		NSDictionary [] GetPendingExpiredSessionReports (NSData appIdentifier, NSUrl storageUrl);

		[Static]
		[Export ("removePendingExpiredSessionReports:withAppIdentifier:storageDirectoryAtURL:")]
		void RemovePendingExpiredSessionReports (NSDictionary [] expiredSessionReports, NSData appIdentifier, NSUrl storageUrl);

		#endregion
	}

	[Static]
	[Internal]
	[Watch (7, 0), NoTV, iOS (12, 2)]
	[MacCatalyst (13, 1)]
	interface AVContentKeySessionServerPlaybackContextOptionKeys {
		[Field ("AVContentKeySessionServerPlaybackContextOptionProtocolVersions")]
		NSString ProtocolVersionsKey { get; }

		[Field ("AVContentKeySessionServerPlaybackContextOptionServerChallenge")]
		NSString ServerChallengeKey { get; }
	}

	[StrongDictionary ("AVContentKeySessionServerPlaybackContextOptionKeys")]
	[Watch (7, 0), NoTV, NoMac, iOS (12, 2)]
	[MacCatalyst (13, 1)]
	interface AVContentKeySessionServerPlaybackContextOptions {
		NSNumber [] ProtocolVersions { get; }

		NSData ServerChallenge { get; }
	}

	[Watch (7, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVContentKeySession))]
	interface AVContentKeySession_AVContentKeyRecipients {
		[Export ("addContentKeyRecipient:")]
		void Add (IAVContentKeyRecipient recipient);

		[Export ("removeContentKeyRecipient:")]
		void Remove (IAVContentKeyRecipient recipient);

		[Export ("contentKeyRecipients")]
		IAVContentKeyRecipient [] GetContentKeyRecipients ();
	}

	[Watch (6, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface AVContentKeyRequest {
		[Watch (7, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVContentKeyRequestProtocolVersionsKey")]
		NSString ProtocolVersions { get; }

		[Watch (7, 0)]
		[MacCatalyst (13, 1)]
		[Export ("status")]
		AVContentKeyRequestStatus Status { get; }

		[Watch (7, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("error")]
		NSError Error { get; }

		[Watch (7, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("identifier")]
		NSObject Identifier { get; }

		[Watch (7, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("initializationData")]
		NSData InitializationData { get; }

		[Watch (7, 0)]
		[MacCatalyst (13, 1)]
		[Export ("canProvidePersistableContentKey")]
		bool CanProvidePersistableContentKey { get; }

		[TV (12, 2), iOS (12, 2), Watch (7, 0)]
		[MacCatalyst (13, 1)]
		[Export ("options", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Options { get; }

		[Watch (7, 0)]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("makeStreamingContentKeyRequestDataForApp:contentIdentifier:options:completionHandler:")]
		void MakeStreamingContentKeyRequestData (NSData appIdentifier, [NullAllowed] NSData contentIdentifier, [NullAllowed] NSDictionary<NSString, NSObject> options, Action<NSData, NSError> handler);

		[Watch (7, 0)]
		[MacCatalyst (13, 1)]
		[Export ("processContentKeyResponse:")]
		void Process (AVContentKeyResponse keyResponse);

		[Watch (7, 0)]
		[MacCatalyst (13, 1)]
		[Export ("processContentKeyResponseError:")]
		void Process (NSError error);

		[Deprecated (PlatformName.iOS, 11, 2, message: "Use the 'NSError' overload instead.")]
		[Export ("respondByRequestingPersistableContentKeyRequest"), NoWatch, NoTV, NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'NSError' overload instead.")]
		void RespondByRequestingPersistableContentKeyRequest ();

		[Watch (7, 0), NoTV, iOS (11, 2)]
		[MacCatalyst (13, 1)]
		[Export ("respondByRequestingPersistableContentKeyRequestAndReturnError:")]
		bool RespondByRequestingPersistableContentKeyRequest ([NullAllowed] out NSError error);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("AVContentKeyRequestRequiresValidationDataInSecureTokenKey")]
		NSString RequiresValidationDataInSecureTokenKey { get; }

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("contentKeySpecifier")]
		AVContentKeySpecifier ContentKeySpecifier { get; }

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[NullAllowed, Export ("contentKey")]
		AVContentKey ContentKey { get; }
	}

	[Category]
	[Watch (7, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVContentKeyRequest))]
	interface AVContentKeyRequest_AVContentKeyRequestRenewal {
		[Export ("renewsExpiringResponseData")]
		bool GetRenewsExpiringResponseData ();
	}

	[Watch (7, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (AVContentKeyRequest))]
	interface AVPersistableContentKeyRequest {
		[Export ("persistableContentKeyFromKeyVendorResponse:options:error:")]
		[return: NullAllowed]
		NSData GetPersistableContentKey (NSData keyVendorResponse, [NullAllowed] NSDictionary<NSString, NSObject> options, out NSError outError);

	}

	[Watch (7, 0)]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface AVContentKeyResponse {
		[Internal]
		[Static]
		[Export ("contentKeyResponseWithFairPlayStreamingKeyResponseData:")]
		AVContentKeyResponse _InitWithFairPlayStreamingKeyResponseData (NSData fairPlayStreamingKeyResponseData);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("contentKeyResponseWithClearKeyData:initializationVector:")]
		AVContentKeyResponse Create (NSData keyData, [NullAllowed] NSData initializationVector);

		[Internal]
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("contentKeyResponseWithAuthorizationTokenData:")]
		AVContentKeyResponse _InitWithAuthorizationToken (NSData authorizationTokenData);
	}

	[TV (14, 5), Mac (11, 3), iOS (14, 5), Watch (7, 4)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (NSObject))]
	interface AVContentKeySpecifier {
		// TODO https://github.com/xamarin/xamarin-macios/issues/10904
		[Static]
		[Export ("contentKeySpecifierForKeySystem:identifier:options:")]
		AVContentKeySpecifier GetContentKeySpecifier (AVContentKeySystem keySystem, NSObject contentKeyIdentifier, NSDictionary<NSString, NSObject> options);

		[Export ("initForKeySystem:identifier:options:")]
		NativeHandle Constructor (AVContentKeySystem keySystem, NSObject contentKeyIdentifier, NSDictionary<NSString, NSObject> options);

		[Export ("keySystem")]
		AVContentKeySystem KeySystem { get; }

		[Export ("identifier")]
		NSObject Identifier { get; }

		[Export ("options")]
		NSDictionary<NSString, NSObject> Options { get; }
	}

	[TV (14, 5), Mac (11, 3), iOS (14, 5), Watch (7, 4)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (NSObject))]
	interface AVContentKey {
		[Export ("contentKeySpecifier")]
		AVContentKeySpecifier ContentKeySpecifier { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface AVRouteDetector {
		[Notification]
		[Field ("AVRouteDetectorMultipleRoutesDetectedDidChangeNotification")]
		NSString MultipleRoutesDetectedDidChange { get; }

		[Export ("routeDetectionEnabled")]
		bool RouteDetectionEnabled { [Bind ("isRouteDetectionEnabled")] get; set; }

		[Export ("multipleRoutesDetected")]
		bool MultipleRoutesDetected { get; }
	}

	interface IAVCapturePhotoFileDataRepresentationCustomizer { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[Protocol]
	interface AVCapturePhotoFileDataRepresentationCustomizer {
		[Export ("replacementMetadataForPhoto:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSObject> GetReplacementMetadata (AVCapturePhoto photo);

		[Export ("replacementEmbeddedThumbnailPixelBufferWithPhotoFormat:forPhoto:")]
		[return: NullAllowed]
		CVPixelBuffer GetReplacementEmbeddedThumbnail ([NullAllowed] out NSDictionary<NSString, NSObject> replacementEmbeddedThumbnailPhotoFormatOut, AVCapturePhoto photo);

		[Export ("replacementDepthDataForPhoto:")]
		[return: NullAllowed]
		AVDepthData GetReplacementDepthData (AVCapturePhoto photo);

		[Export ("replacementPortraitEffectsMatteForPhoto:")]
		[return: NullAllowed]
		AVPortraitEffectsMatte GetReplacementPortraitEffectsMatte (AVCapturePhoto photo);

		[iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("replacementSemanticSegmentationMatteOfType:forPhoto:")]
		[return: NullAllowed]
		AVSemanticSegmentationMatte GetReplacementSemanticSegmentationMatte (NSString semanticSegmentationMatteType, AVCapturePhoto photo);

		[Introduced (PlatformName.MacCatalyst, 14, 3)]
		[NoWatch, NoTV, NoMac, iOS (14, 3)]
		[Export ("replacementAppleProRAWCompressionSettingsForPhoto:defaultSettings:maximumBitDepth:")]
		NSDictionary<NSString, NSObject> GetReplacementAppleProRawCompressionSettings (AVCapturePhoto photo, NSDictionary<NSString, NSObject> defaultSettings, nint maximumBitDepth);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoTV, NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCapturePhoto {
		[Export ("timestamp")]
		CMTime Timestamp { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("rawPhoto")]
		bool RawPhoto { [Bind ("isRawPhoto")] get; }

		[NullAllowed, Export ("pixelBuffer")]
		CVPixelBuffer PixelBuffer { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("previewPixelBuffer")]
		CVPixelBuffer PreviewPixelBuffer { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("embeddedThumbnailPhotoFormat")]
		NSDictionary WeakEmbeddedThumbnailPhotoFormat { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("WeakEmbeddedThumbnailPhotoFormat")]
		AVVideoSettingsCompressed EmbeddedThumbnailPhotoFormat { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("depthData")]
		AVDepthData DepthData { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("metadata")]
		NSDictionary WeakMetadata { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("WeakMetadata")]
		CoreGraphics.CGImageProperties Properties { get; }

		[NullAllowed, Export ("cameraCalibrationData")]
		AVCameraCalibrationData CameraCalibrationData { get; }

		[Export ("resolvedSettings")]
		AVCaptureResolvedPhotoSettings ResolvedSettings { get; }

		[Export ("photoCount")]
		nint PhotoCount { get; }

		[NullAllowed, Export ("sourceDeviceType")]
		NSString WeakSourceDeviceType { get; }

		[Wrap ("AVCaptureDeviceTypeExtensions.GetValue (WeakSourceDeviceType!)")]
		AVCaptureDeviceType SourceDeviceType { get; }

		// From @interface AVCapturePhotoBracketedCapture (AVCapturePhoto)

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("bracketSettings")]
		AVCaptureBracketedStillImageSettings BracketSettings { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("lensStabilizationStatus")]
		AVCaptureLensStabilizationStatus LensStabilizationStatus { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("sequenceCount")]
		nint SequenceCount { get; }

		// @interface AVCapturePhotoConversions (AVCapturePhoto)
		[NullAllowed, Export ("fileDataRepresentation")]
		NSData FileDataRepresentation { get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetFileDataRepresentation' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetFileDataRepresentation' instead.")]
		[Export ("fileDataRepresentationWithReplacementMetadata:replacementEmbeddedThumbnailPhotoFormat:replacementEmbeddedThumbnailPixelBuffer:replacementDepthData:")]
		[return: NullAllowed]
		NSData GetFileDataRepresentation ([NullAllowed] NSDictionary<NSString, NSObject> replacementMetadata, [NullAllowed] NSDictionary<NSString, NSObject> replacementEmbeddedThumbnailPhotoFormat, [NullAllowed] CVPixelBuffer replacementEmbeddedThumbnailPixelBuffer, [NullAllowed] AVDepthData replacementDepthData);

		[NullAllowed, Export ("CGImageRepresentation")]
		CGImage CGImageRepresentation { get; }

		[NullAllowed, Export ("previewCGImageRepresentation")]
		CGImage PreviewCGImageRepresentation { get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("portraitEffectsMatte")]
		AVPortraitEffectsMatte PortraitEffectsMatte { get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (14, 0)]
		[Export ("fileDataRepresentationWithCustomizer:")]
		[return: NullAllowed]
		NSData GetFileDataRepresentation (IAVCapturePhotoFileDataRepresentationCustomizer customizer);

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[MacCatalyst (14, 0)]
		[Export ("semanticSegmentationMatteForType:")]
		[return: NullAllowed]
		AVSemanticSegmentationMatte GetSemanticSegmentationMatte ([BindAs (typeof (AVSemanticSegmentationMatteType))] NSString semanticSegmentationMatteType);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Watch (6, 0), TV (12, 0), iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPortraitEffectsMatte {
		[Static]
		[Export ("portraitEffectsMatteFromDictionaryRepresentation:error:")]
		[return: NullAllowed]
		AVPortraitEffectsMatte Create (NSDictionary imageSourceAuxDataInfoDictionary, [NullAllowed] out NSError outError);

		[Export ("portraitEffectsMatteByApplyingExifOrientation:")]
		AVPortraitEffectsMatte Create (CGImagePropertyOrientation exifOrientation);

		[Export ("portraitEffectsMatteByReplacingPortraitEffectsMatteWithPixelBuffer:error:")]
		[return: NullAllowed]
		AVPortraitEffectsMatte Create (CVPixelBuffer pixelBuffer, [NullAllowed] out NSError outError);

		[Export ("dictionaryRepresentationForAuxiliaryDataType:")]
		[return: NullAllowed]
		NSDictionary GetDictionaryRepresentation ([NullAllowed] out string outAuxDataType);

		[Export ("pixelFormatType")]
		uint PixelFormatType { get; }

		[Export ("mattingImage")]
		CVPixelBuffer MattingImage { get; }
	}

	[NoWatch, TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetResourceLoadingRequestor {
		[Export ("providesExpiredSessionReports")]
		bool ProvidesExpiredSessionReports { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	enum AVSemanticSegmentationMatteType {
		[DefaultEnumValue]
		[Field (null)]
		None,
		[Field ("AVSemanticSegmentationMatteTypeSkin")]
		Skin,
		[Field ("AVSemanticSegmentationMatteTypeHair")]
		Hair,
		[Field ("AVSemanticSegmentationMatteTypeTeeth")]
		Teeth,
		[Introduced (PlatformName.MacCatalyst, 14, 1)]
		[iOS (14, 1)]
		[Mac (11, 0)]
		[Watch (8, 0)]
		[TV (15, 0)]
		[Field ("AVSemanticSegmentationMatteTypeGlasses")]
		Glasses,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVSemanticSegmentationMatte {

		[Static]
		[Export ("semanticSegmentationMatteFromImageSourceAuxiliaryDataType:dictionaryRepresentation:error:")]
		[return: NullAllowed]
		AVSemanticSegmentationMatte Create (NSString imageSourceAuxiliaryDataType, NSDictionary imageSourceAuxiliaryDataInfoDictionary, out NSError outError);

		[BindAs (typeof (AVSemanticSegmentationMatteType))]
		[Export ("matteType")]
		NSString MatteType { get; }

		[Export ("semanticSegmentationMatteByApplyingExifOrientation:")]
		AVSemanticSegmentationMatte ApplyExifOrientation (CGImagePropertyOrientation exifOrientation);

		[Export ("semanticSegmentationMatteByReplacingSemanticSegmentationMatteWithPixelBuffer:error:")]
		[return: NullAllowed]
		AVSemanticSegmentationMatte ReplaceSemanticSegmentationMatte (CVPixelBuffer pixelBuffer, out NSError outError);

		[Export ("dictionaryRepresentationForAuxiliaryDataType:")]
		[return: NullAllowed]
		NSDictionary GetDictionaryRepresentation ([NullAllowed] out string outAuxDataType);

		[Export ("pixelFormatType")]
		CVPixelFormatType PixelFormatType { get; }

		[Export ("mattingImage")]
		CVPixelBuffer MattingImage { get; }
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCompositionTrackFormatDescriptionReplacement : NSSecureCoding {
		[Export ("originalFormatDescription")]
		CMFormatDescription OriginalFormatDescription { get; }

		[Export ("replacementFormatDescription")]
		CMFormatDescription ReplacementFormatDescription { get; }
	}
#if NET
	delegate /* OSStatus */ int AVAudioSourceNodeRenderHandler (ref bool isSilence, ref AudioTimeStamp timestamp, uint frameCount, ref AudioBuffers outputData);
#else
	delegate /* OSStatus */ int AVAudioSourceNodeRenderHandler (bool isSilence, AudioToolbox.AudioTimeStamp timestamp, uint frameCount, ref AudioBuffers outputData);
	delegate /* OSStatus */ int AVAudioSourceNodeRenderHandler2 (ref bool isSilence, ref AudioTimeStamp timestamp, uint frameCount, ref AudioBuffers outputData);
#endif
	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioNode))]
	[DisableDefaultCtor]
	interface AVAudioSourceNode : AVAudioMixing {
		[Export ("initWithRenderBlock:")]
		[DesignatedInitializer]
#if NET
		NativeHandle Constructor (AVAudioSourceNodeRenderHandler renderHandler);
#else
		NativeHandle Constructor (AVAudioSourceNodeRenderHandler2 renderHandler);
#endif

		[Export ("initWithFormat:renderBlock:")]
		[DesignatedInitializer]
#if NET
		NativeHandle Constructor (AVAudioFormat format, AVAudioSourceNodeRenderHandler renderHandler);
#else
		NativeHandle Constructor (AVAudioFormat format, AVAudioSourceNodeRenderHandler2 renderHandler);
#endif
	}

	delegate int AVAudioSinkNodeReceiverHandlerRaw (IntPtr timestamp, uint frameCount, IntPtr inputData);

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AVAudioNode))]
	[DisableDefaultCtor]
	interface AVAudioSinkNode {
		[Export ("initWithReceiverBlock:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVAudioSinkNodeReceiverHandlerRaw receiverHandler);
	}

	[TV (13, 0), NoWatch, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AVVideoCompositionRenderHint {

		[Export ("startCompositionTime")]
		CMTime StartCompositionTime { get; }

		[Export ("endCompositionTime")]
		CMTime EndCompositionTime { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (AVCaptureSession))]
	interface AVCaptureMultiCamSession {
		[Static]
		[Export ("multiCamSupported")]
		bool MultiCamSupported { [Bind ("isMultiCamSupported")] get; }

		[Export ("hardwareCost")]
		float HardwareCost { get; }

		[Export ("systemPressureCost")]
		float SystemPressureCost { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, iOS (13, 0)]
	[BaseType (typeof (AVMetadataObject))]
	[DisableDefaultCtor]
	interface AVMetadataBodyObject : NSCopying {
		[Export ("bodyID")]
		nint BodyId { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, iOS (13, 0)]
	[BaseType (typeof (AVMetadataBodyObject))]
	[DisableDefaultCtor]
	interface AVMetadataCatBodyObject : NSCopying {
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, iOS (13, 0)]
	[BaseType (typeof (AVMetadataBodyObject))]
	[DisableDefaultCtor]
	interface AVMetadataDogBodyObject : NSCopying {
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, iOS (13, 0)]
	[BaseType (typeof (AVMetadataBodyObject))]
	[DisableDefaultCtor]
	interface AVMetadataHumanBodyObject : NSCopying {
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, iOS (13, 0)]
	[BaseType (typeof (AVMetadataObject))]
	[DisableDefaultCtor]
	interface AVMetadataSalientObject : NSCopying {
		[Export ("objectID")]
		nint ObjectId { get; }
	}

	[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetSegmentReport {
		[Export ("segmentType")]
		AVAssetSegmentType SegmentType { get; }

		[Export ("trackReports")]
		AVAssetSegmentTrackReport [] TrackReports { get; }
	}

	[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetSegmentReportSampleInformation {
		[Export ("presentationTimeStamp")]
		CMTime PresentationTimeStamp { get; }

		[Export ("offset")]
		nint Offset { get; }

		[Export ("length")]
		nint Length { get; }

		[Export ("isSyncSample")]
		bool IsSyncSample { get; }
	}

	[TV (14, 0), NoWatch, Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetSegmentTrackReport {
		[Export ("trackID")]
		int TrackId { get; }

		[BindAs (typeof (AVMediaTypes))]
		[Export ("mediaType")]
		NSString MediaType { get; }

		[Export ("earliestPresentationTimeStamp")]
		CMTime EarliestPresentationTimeStamp { get; }

		[Export ("duration")]
		CMTime Duration { get; }

		[NullAllowed, Export ("firstVideoSampleInformation")]
		AVAssetSegmentReportSampleInformation FirstVideoSampleInformation { get; }
	}

	[NoWatch, NoTV, NoiOS, Mac (11, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAudioRoutingArbiter {
		[Static]
		[Export ("sharedRoutingArbiter")]
		AVAudioRoutingArbiter SharedRoutingArbiter { get; }

		[Export ("beginArbitrationWithCategory:completionHandler:")]
		void BeginArbitration (AVAudioRoutingArbitrationCategory category, Action<bool, NSError> handler);

		[Export ("leaveArbitration")]
		void LeaveArbitration ();
	}

	[NoWatch, NoTV]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Mac (12, 3)]
	[Native]
	public enum AVCaptureCenterStageControlMode : long {
		User = 0,
		App = 1,
		Cooperative = 2,
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetDownloadConfiguration {
		[Static]
		[Export ("downloadConfigurationWithAsset:title:")]
		AVAssetDownloadConfiguration Create (AVUrlAsset asset, string title);

		[NullAllowed, Export ("artworkData", ArgumentSemantic.Copy)]
		NSData ArtworkData { get; set; }

		[Export ("primaryContentConfiguration")]
		AVAssetDownloadContentConfiguration PrimaryContentConfiguration { get; }

		[Export ("auxiliaryContentConfigurations", ArgumentSemantic.Copy)]
		AVAssetDownloadContentConfiguration [] AuxiliaryContentConfigurations { get; set; }

		[Export ("optimizesAuxiliaryContentConfigurations")]
		bool OptimizesAuxiliaryContentConfigurations { get; set; }
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface AVAssetDownloadContentConfiguration : NSCopying {
		[Export ("variantQualifiers", ArgumentSemantic.Copy)]
		AVAssetVariantQualifier [] VariantQualifiers { get; set; }

		[Export ("mediaSelections", ArgumentSemantic.Copy)]
		AVMediaSelection [] MediaSelections { get; set; }
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetVariant {
		[Export ("peakBitRate")]
		double PeakBitRate { get; }

		[Export ("averageBitRate")]
		double AverageBitRate { get; }

		[NullAllowed, Export ("videoAttributes")]
		AVAssetVariantVideoAttributes VideoAttributes { get; }

		[NullAllowed, Export ("audioAttributes")]
		AVAssetVariantAudioAttributes AudioAttributes { get; }
	}


	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetVariantAudioAttributes {
		[Export ("formatIDs")]
		NSNumber [] FormatIds { get; }

		[Export ("renditionSpecificAttributesForMediaOption:")]
		[return: NullAllowed]
		AVAssetVariantAudioRenditionSpecificAttributes GetRenditionSpecificAttributes (AVMediaSelectionOption mediaSelectionOption);
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface AVAssetVariantAudioRenditionSpecificAttributes {
		[Export ("channelCount")]
		nint ChannelCount { get; }
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetVariantQualifier : NSCopying {
		[Static]
		[Export ("assetVariantQualifierWithPredicate:")]
		AVAssetVariantQualifier Create (NSPredicate predicate);

		[Static]
		[Export ("assetVariantQualifierWithVariant:")]
		AVAssetVariantQualifier Create (AVAssetVariant variant);

		[Static]
		[Export ("predicateForChannelCount:mediaSelectionOption:operatorType:")]
		NSPredicate GetPredicate (nint channelCount, AVMediaSelectionOption mediaSelectionOption, NSPredicateOperatorType operatorType);

		[Internal]
		[Static]
		[Export ("predicateForPresentationWidth:operatorType:")]
		NSPredicate GetPredicateForPresentationWidth (nfloat width, NSPredicateOperatorType operatorType);

		[Internal]
		[Static]
		[Export ("predicateForPresentationHeight:operatorType:")]
		NSPredicate GetPredicateForPresentationHeight (nfloat height, NSPredicateOperatorType operatorType);
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetVariantVideoAttributes {
		[Export ("videoRange")]
		string VideoRange { get; }

		[Export ("codecTypes")]
		NSNumber [] CodecTypes { get; }

		[Export ("presentationSize")]
		CGSize PresentationSize { get; }

		[Export ("nominalFrameRate")]
		double NominalFrameRate { get; }
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface AVCoordinatedPlaybackParticipant {
		[Export ("suspensionReasons")]
		string [] SuspensionReasons { get; }

		[Export ("readyToPlay")]
		bool ReadyToPlay { [Bind ("isReadyToPlay")] get; }

		[Export ("identifier")]
		NSUuid Identifier { get; }
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCoordinatedPlaybackSuspension {
		[Export ("reason")]
		string Reason { get; }

		[Export ("beginDate")]
		NSDate BeginDate { get; }

		[Export ("end")]
		void End ();

		[Export ("endProposingNewTime:")]
		void EndProposingNewTime (CMTime time);
	}

	interface IAVPlaybackCoordinatorPlaybackControlDelegate { }

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface AVPlaybackCoordinatorPlaybackControlDelegate {
		[Abstract]
		[Export ("playbackCoordinator:didIssuePlayCommand:completionHandler:")]
		void DidIssuePlayCommand (AVDelegatingPlaybackCoordinator coordinator, AVDelegatingPlaybackCoordinatorPlayCommand playCommand, Action completionHandler);

		[Abstract]
		[Export ("playbackCoordinator:didIssuePauseCommand:completionHandler:")]
		void DidIssuePauseCommand (AVDelegatingPlaybackCoordinator coordinator, AVDelegatingPlaybackCoordinatorPauseCommand pauseCommand, Action completionHandler);

		[Abstract]
		[Export ("playbackCoordinator:didIssueSeekCommand:completionHandler:")]
		void DidIssueSeekCommand (AVDelegatingPlaybackCoordinator coordinator, AVDelegatingPlaybackCoordinatorSeekCommand seekCommand, Action completionHandler);

		[Abstract]
		[Export ("playbackCoordinator:didIssueBufferingCommand:completionHandler:")]
		void DidIssueBufferingCommand (AVDelegatingPlaybackCoordinator coordinator, AVDelegatingPlaybackCoordinatorBufferingCommand bufferingCommand, Action completionHandler);
	}


	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (AVPlaybackCoordinator))]
	[DisableDefaultCtor] // throws exception
	interface AVDelegatingPlaybackCoordinator {
		[Export ("initWithPlaybackControlDelegate:")]
		NativeHandle Constructor (IAVPlaybackCoordinatorPlaybackControlDelegate playbackControlDelegate);

		[Wrap ("WeakPlaybackControlDelegate")]
		[NullAllowed]
		IAVPlaybackCoordinatorPlaybackControlDelegate PlaybackControlDelegate { get; }

		[NullAllowed, Export ("playbackControlDelegate", ArgumentSemantic.Weak)]
		NSObject WeakPlaybackControlDelegate { get; }

		[Export ("coordinateRateChangeToRate:options:")]
		void CoordinateRateChangeToRate (float rate, AVDelegatingPlaybackCoordinatorRateChangeOptions options);

		[Export ("coordinateSeekToTime:options:")]
		void CoordinateSeekToTime (CMTime time, AVDelegatingPlaybackCoordinatorSeekOptions options);

		[Export ("transitionToItemWithIdentifier:proposingInitialTimingBasedOnTimebase:")]
		void TransitionToItem ([NullAllowed] string itemIdentifier, [NullAllowed] CMTimebase snapshotTimebase);

		[NullAllowed, Export ("currentItemIdentifier")]
		string CurrentItemIdentifier { get; }

		[Export ("reapplyCurrentItemStateToPlaybackControlDelegate")]
		void ReapplyCurrentItemStateToPlaybackControlDelegate ();
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (AVDelegatingPlaybackCoordinatorPlaybackControlCommand))]
	[DisableDefaultCtor]
	interface AVDelegatingPlaybackCoordinatorBufferingCommand {
		[Export ("anticipatedPlaybackRate")]
		float AnticipatedPlaybackRate { get; }

		[NullAllowed, Export ("completionDueDate")]
		NSDate CompletionDueDate { get; }
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (AVDelegatingPlaybackCoordinatorPlaybackControlCommand))]
	[DisableDefaultCtor]
	interface AVDelegatingPlaybackCoordinatorPauseCommand {
		[Export ("shouldBufferInAnticipationOfPlayback")]
		bool ShouldBufferInAnticipationOfPlayback { get; }

		[Export ("anticipatedPlaybackRate")]
		float AnticipatedPlaybackRate { get; }
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (AVDelegatingPlaybackCoordinatorPlaybackControlCommand))]
	[DisableDefaultCtor]
	interface AVDelegatingPlaybackCoordinatorPlayCommand {
		[Export ("rate")]
		float Rate { get; }

		[Export ("itemTime")]
		CMTime ItemTime { get; }

		[Export ("hostClockTime")]
		CMTime HostClockTime { get; }
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVDelegatingPlaybackCoordinatorPlaybackControlCommand {
		[NullAllowed, Export ("originator")]
		AVCoordinatedPlaybackParticipant Originator { get; }

		[Export ("expectedCurrentItemIdentifier")]
		string ExpectedCurrentItemIdentifier { get; }
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (AVDelegatingPlaybackCoordinatorPlaybackControlCommand))]
	[DisableDefaultCtor]
	interface AVDelegatingPlaybackCoordinatorSeekCommand {
		[Export ("itemTime")]
		CMTime ItemTime { get; }

		[Export ("shouldBufferInAnticipationOfPlayback")]
		bool ShouldBufferInAnticipationOfPlayback { get; }

		[Export ("anticipatedPlaybackRate")]
		float AnticipatedPlaybackRate { get; }

		[NullAllowed, Export ("completionDueDate")]
		NSDate CompletionDueDate { get; }
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPlaybackCoordinator {

		[Notification]
		[Field ("AVPlaybackCoordinatorOtherParticipantsDidChangeNotification")]
		NSString OtherParticipantsDidChangeNotification { get; }

		[Notification]
		[Field ("AVPlaybackCoordinatorSuspensionReasonsDidChangeNotification")]
		NSString SuspensionReasonsDidChangeNotification { get; }

		[Export ("otherParticipants")]
		AVCoordinatedPlaybackParticipant [] OtherParticipants { get; }

		[Export ("suspensionReasons")]
		string [] SuspensionReasons { get; }

		[Export ("beginSuspensionForReason:")]
		AVCoordinatedPlaybackSuspension BeginSuspension (string suspensionReason);

		[Export ("expectedItemTimeAtHostTime:")]
		CMTime GetExpectedItemTime (CMTime hostClockTime);

		// AVPlaybackCoordinator_AVCoordinatedPlaybackPolicies
		[Export ("setParticipantLimit:forWaitingOutSuspensionsWithReason:")]
		void SetParticipantLimit (nint participantLimit, string reason);

		[Export ("participantLimitForWaitingOutSuspensionsWithReason:")]
		nint GetParticipantLimit (string reason);

		[Export ("suspensionReasonsThatTriggerWaiting", ArgumentSemantic.Copy)]
		string [] SuspensionReasonsThatTriggerWaiting { get; set; }

		[Export ("pauseSnapsToMediaTimeOfOriginator")]
		bool PauseSnapsToMediaTimeOfOriginator { get; set; }
	}

	interface IAVPlayerPlaybackCoordinatorDelegate { }

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface AVPlayerPlaybackCoordinatorDelegate {
		[Export ("playbackCoordinator:identifierForPlayerItem:")]
		string GetIdentifier (AVPlayerPlaybackCoordinator coordinator, AVPlayerItem playerItem);

		[TV (15, 4), NoWatch, Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("playbackCoordinator:interstitialTimeRangesForPlayerItem:")]
		NSValue [] GetInterstitialTimeRanges (AVPlayerPlaybackCoordinator coordinator, AVPlayerItem playerItem);
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (AVPlaybackCoordinator))]
	[DisableDefaultCtor]
	interface AVPlayerPlaybackCoordinator {
		[NullAllowed, Export ("player", ArgumentSemantic.Weak)]
		AVPlayer Player { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IAVPlayerPlaybackCoordinatorDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

	interface IAVAssetReaderCaptionValidationHandling { }

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[Protocol]
	interface AVAssetReaderCaptionValidationHandling {
		[Export ("captionAdaptor:didVendCaption:skippingUnsupportedSourceSyntaxElements:")]
		void DidVendCaption (AVAssetReaderOutputCaptionAdaptor adaptor, AVCaption caption, string [] syntaxElements);
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetReaderOutputCaptionAdaptor {
		[Static]
		[Export ("assetReaderOutputCaptionAdaptorWithAssetReaderTrackOutput:")]
		AVAssetReaderOutputCaptionAdaptor Create (AVAssetReaderTrackOutput trackOutput);

		[Export ("initWithAssetReaderTrackOutput:")]
		NativeHandle Constructor (AVAssetReaderTrackOutput trackOutput);

		[Export ("assetReaderTrackOutput")]
		AVAssetReaderTrackOutput AssetReaderTrackOutput { get; }

		[NoMacCatalyst]
		[Export ("nextCaptionGroup")]
		[return: NullAllowed]
		AVCaptionGroup GetNextCaptionGroup ();

		[NoMacCatalyst]
		[Export ("captionsNotPresentInPreviousGroupsInCaptionGroup:")]
		AVCaption [] GetCaptionsNotPresentInPreviousGroups (AVCaptionGroup captionGroup);

		// interface AVAssetReaderOutputCaptionAdaptor_AVAssetReaderCaptionValidation
		[Wrap ("WeakValidationDelegate")]
		[NullAllowed]
		IAVAssetReaderCaptionValidationHandling ValidationDelegate { get; set; }

		[NullAllowed, Export ("validationDelegate", ArgumentSemantic.Weak)]
		NSObject WeakValidationDelegate { get; set; }
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAssetWriterInputCaptionAdaptor {
		[Static]
		[Export ("assetWriterInputCaptionAdaptorWithAssetWriterInput:")]
		AVAssetWriterInputCaptionAdaptor Create (AVAssetWriterInput input);

		[Export ("initWithAssetWriterInput:")]
		NativeHandle Constructor (AVAssetWriterInput input);

		[Export ("assetWriterInput")]
		AVAssetWriterInput AssetWriterInput { get; }

		[NoMacCatalyst]
		[Export ("appendCaption:")]
		bool AppendCaption (AVCaption caption);

		[NoMacCatalyst]
		[Export ("appendCaptionGroup:")]
		bool AppendCaptionGroup (AVCaptionGroup captionGroup);
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptionGroup {
		[Export ("initWithCaptions:timeRange:")]
		NativeHandle Constructor (AVCaption [] captions, CMTimeRange timeRange);

		[Export ("initWithTimeRange:")]
		NativeHandle Constructor (CMTimeRange timeRange);

		[Export ("timeRange")]
		CMTimeRange TimeRange { get; }

		[Export ("captions")]
		AVCaption [] Captions { get; }
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaption : NSCopying, NSMutableCopying, NSSecureCoding {
		[Export ("initWithText:timeRange:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string text, CMTimeRange timeRange);

		[Export ("text")]
		string Text { get; }

		[Export ("timeRange")]
		CMTimeRange TimeRange { get; }

		//	interface AVCaption_Region
		[NullAllowed, Export ("region")]
		AVCaptionRegion Region { get; }

		[Export ("textAlignment")]
		AVCaptionTextAlignment TextAlignment { get; }

		// interface AVCaption_Animation
		[Export ("animation")]
		AVCaptionAnimation Animation { get; }

		// interface AVCaption_Styling

		[Export ("textColorAtIndex:range:")]
		[return: NullAllowed]
		CGColor GetTextColor (nint index, [NullAllowed] out NSRange outRange);

		[Export ("backgroundColorAtIndex:range:")]
		[return: NullAllowed]
		CGColor GetBackgroundColor (nint index, [NullAllowed] out NSRange outRange);

		[Export ("fontWeightAtIndex:range:")]
		AVCaptionFontWeight GetFontWeight (nint index, [NullAllowed] out NSRange outRange);

		[Export ("fontStyleAtIndex:range:")]
		AVCaptionFontStyle GetFontStyle (nint index, [NullAllowed] out NSRange outRange);

		[Export ("decorationAtIndex:range:")]
		AVCaptionDecoration GetDecoration (nint index, [NullAllowed] out NSRange outRange);

		[Export ("textCombineAtIndex:range:")]
		AVCaptionTextCombine GetTextCombine (nint index, [NullAllowed] out NSRange outRange);

		[Export ("rubyAtIndex:range:")]
		[return: NullAllowed]
		AVCaptionRuby GetRuby (nint index, [NullAllowed] out NSRange outRange);
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0)]
	[NoMacCatalyst]
	[DisableDefaultCtor]
	[BaseType (typeof (AVCaption))]
	interface AVMutableCaption {
		[Export ("initWithText:timeRange:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string text, CMTimeRange timeRange);

		[Export ("text")]
		string Text { get; set; }

		[Export ("timeRange", ArgumentSemantic.Assign)]
		CMTimeRange TimeRange { get; set; }

		// interface AVMutableCaption_Styling

		[Export ("setTextColor:inRange:")]
		void SetTextColor (CGColor color, NSRange range);

		[Export ("setBackgroundColor:inRange:")]
		void SetBackgroundColor (CGColor color, NSRange range);

		[Export ("setFontWeight:inRange:")]
		void SetFontWeight (AVCaptionFontWeight fontWeight, NSRange range);

		[Export ("setFontStyle:inRange:")]
		void SetFontStyle (AVCaptionFontStyle fontStyle, NSRange range);

		[Export ("setDecoration:inRange:")]
		void SetDecoration (AVCaptionDecoration decoration, NSRange range);

		[Export ("setTextCombine:inRange:")]
		void SetTextCombine (AVCaptionTextCombine textCombine, NSRange range);

		[Export ("setRuby:inRange:")]
		void SetRuby (AVCaptionRuby ruby, NSRange range);

		[Export ("removeTextColorInRange:")]
		void RemoveTextColor (NSRange range);

		[Export ("removeBackgroundColorInRange:")]
		void RemoveBackgroundColor (NSRange range);

		[Export ("removeFontWeightInRange:")]
		void RemoveFontWeight (NSRange range);

		[Export ("removeFontStyleInRange:")]
		void RemoveFontStyle (NSRange range);

		[Export ("removeDecorationInRange:")]
		void RemoveDecoration (NSRange range);

		[Export ("removeTextCombineInRange:")]
		void RemoveTextCombine (NSRange range);

		[Export ("removeRubyInRange:")]
		void RemoveRuby (NSRange range);

		// interface AVMutableCaption_Region
		[Export ("region", ArgumentSemantic.Copy)]
		AVCaptionRegion Region { get; set; }

		[Export ("textAlignment", ArgumentSemantic.Assign)]
		AVCaptionTextAlignment TextAlignment { get; set; }

		// interface AVMutableCaption_Animation
		[Export ("animation", ArgumentSemantic.Assign)]
		AVCaptionAnimation Animation { get; set; }
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0)]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface AVCaptionRegion : NSCopying, NSMutableCopying, NSSecureCoding {
		[Static]
		[Export ("appleITTTopRegion")]
		AVCaptionRegion AppleIttTopRegion { get; }

		[Static]
		[Export ("appleITTBottomRegion")]
		AVCaptionRegion AppleIttBottomRegion { get; }

		[Static]
		[Export ("appleITTLeftRegion")]
		AVCaptionRegion AppleIttLeftRegion { get; }

		[Static]
		[Export ("appleITTRightRegion")]
		AVCaptionRegion AppleIttRightRegion { get; }

		[Static]
		[Export ("subRipTextBottomRegion")]
		AVCaptionRegion SubRipTextBottomRegion { get; }

		[NullAllowed, Export ("identifier")]
		string Identifier { get; }

#if MONOMAC // needed because the structs are inside a #if too
		[Export ("origin")]
		AVCaptionPoint Origin { get; }

		[Export ("size")]
		AVCaptionSize Size { get; }

		[Export ("scroll")]
		AVCaptionRegionScroll Scroll { get; }

		[Export ("displayAlignment")]
		AVCaptionRegionDisplayAlignment DisplayAlignment { get; }

		[Export ("writingMode")]
		AVCaptionRegionWritingMode WritingMode { get; }
#endif
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0)]
	[NoMacCatalyst]
	[BaseType (typeof (AVCaptionRegion))]
	interface AVMutableCaptionRegion {
		[Export ("initWithIdentifier:")]
		NativeHandle Constructor (string identifier);

#if MONOMAC // needed structs are inside a #if
		[Export ("origin", ArgumentSemantic.Assign)]
		AVCaptionPoint Origin { get; set; }

		[Export ("size", ArgumentSemantic.Assign)]
		AVCaptionSize Size { get; set; }

		[Export ("scroll", ArgumentSemantic.Assign)]
		AVCaptionRegionScroll Scroll { get; set; }

		[Export ("displayAlignment", ArgumentSemantic.Assign)]
		AVCaptionRegionDisplayAlignment DisplayAlignment { get; set; }

		[Export ("writingMode", ArgumentSemantic.Assign)]
		AVCaptionRegionWritingMode WritingMode { get; set; }
#endif
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptionRuby : NSCopying, NSSecureCoding {
		[Export ("initWithText:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string text);

		[Export ("initWithText:position:alignment:")]
		NativeHandle Constructor (string text, AVCaptionRubyPosition position, AVCaptionRubyAlignment alignment);

		[Export ("text")]
		string Text { get; }

		[Export ("position")]
		AVCaptionRubyPosition Position { get; }

		[Export ("alignment")]
		AVCaptionRubyAlignment Alignment { get; }
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptionRendererScene : NSCopying {
		[Export ("timeRange")]
		CMTimeRange TimeRange { get; }

		[Export ("hasActiveCaptions")]
		bool HasActiveCaptions { get; }

		[Export ("needsPeriodicRefresh")]
		bool NeedsPeriodicRefresh { get; }
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface AVCaptionRenderer {
		[Export ("captions", ArgumentSemantic.Copy)]
		AVCaption [] Captions { get; set; }

		[Export ("bounds", ArgumentSemantic.Assign)]
		CGRect Bounds { get; set; }

		[Export ("captionSceneChangesInRange:")]
		AVCaptionRendererScene [] GetCaptionSceneChanges (CMTimeRange consideredTimeRange);

		[Export ("renderInContext:forTime:")]
		void Render (CGContext ctx, CMTime time);
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface AVCaptionGrouper {
		[Export ("addCaption:")]
		void AddCaption (AVCaption input);

		[Export ("flushAddedCaptionsIntoGroupsUpToTime:")]
		AVCaptionGroup [] FlushAddedCaptionsIntoGroupsUpToTime (CMTime upToTime);
	}


	[NoiOS, NoTV, NoMacCatalyst, Mac (12, 0), NoWatch]
	[Static]
	[Internal]
	interface AVCaptionSettingsKeys {
		[Field ("AVCaptionMediaSubTypeKey")]
		NSString MediaSubTypeKey { get; }

		[Field ("AVCaptionMediaTypeKey")]
		NSString MediaTypeKey { get; }

		[Field ("AVCaptionTimeCodeFrameDurationKey")]
		NSString UseTimeCodeFrameDurationKey { get; }

		[Field ("AVCaptionUseDropFrameTimeCodeKey")]
		NSString UseDropFrameTimeCodeKey { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst, Mac (12, 0), NoWatch]
	[StrongDictionary ("AVCaptionSettingsKeys")]
	interface AVCaptionSettings {
		AVMediaTypes MediaSubType { get; set; }
		AVMediaTypes MediaType { get; set; }
		bool UseTimeCodeFrameDuration { get; set; }
		bool UseDropFrameTimeCode { get; set; }
	}


	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptionFormatConformer {
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static]
		[Export ("captionFormatConformerWithConversionSettings:")]
		AVCaptionFormatConformer CreateFromSettings (NSDictionary conversionSettings);

		[Wrap ("CreateFromSettings (conversionSettings.GetDictionary ()!)")]
		AVCaptionFormatConformer CreateFromSettings (AVCaptionSettings conversionSettings);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("initWithConversionSettings:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSDictionary conversionSettings);

		[Wrap ("this (conversionSettings.GetDictionary ()!)")]
		NativeHandle Constructor (AVCaptionSettings conversionSettings);

		[Export ("conformsCaptionsToTimeRange")]
		bool ConformsCaptionsToTimeRange { get; set; }

		[Export ("conformedCaptionForCaption:error:")]
		[return: NullAllowed]
		AVCaption GetConformedCaption (AVCaption caption, [NullAllowed] out NSError outError);
	}

	[NoWatch, NoTV, NoiOS, NoMacCatalyst, Mac (12, 0)]
	enum AVCaptionConversionWarningType {
		[Field ("AVCaptionConversionWarningTypeExcessMediaData")]
		ExcessMediaData,
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptionConversionWarning {
		[Export ("warningType")]
		string WarningType { get; }

		[Export ("rangeOfCaptions")]
		NSRange RangeOfCaptions { get; }

		[NullAllowed, Export ("adjustment")]
		AVCaptionConversionAdjustment Adjustment { get; }
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptionConversionValidator {
		[Static]
		[Export ("captionConversionValidatorWithCaptions:timeRange:conversionSettings:")]
		AVCaptionConversionValidator Create (AVCaption [] captions, CMTimeRange timeRange, NSDictionary<NSString, NSObject> conversionSettings);

		[Export ("initWithCaptions:timeRange:conversionSettings:")]
		NativeHandle Constructor (AVCaption [] captions, CMTimeRange timeRange, NSDictionary<NSString, NSObject> conversionSettings);

		[Export ("status")]
		AVCaptionConversionValidatorStatus Status { get; }

		[Export ("captions")]
		AVCaption [] Captions { get; }

		[Export ("timeRange")]
		CMTimeRange TimeRange { get; }

		[Export ("validateCaptionConversionWithWarningHandler:")]
		void ValidateCaptionConversion (Action<AVCaptionConversionWarning> handler);

		[Export ("stopValidating")]
		void StopValidating ();

		[Export ("warnings")]
		AVCaptionConversionWarning [] Warnings { get; }
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (AVCaptionConversionAdjustment))]
	[DisableDefaultCtor]
	interface AVCaptionConversionTimeRangeAdjustment {
		[Export ("startTimeOffset")]
		CMTime StartTimeOffset { get; }

		[Export ("durationOffset")]
		CMTime DurationOffset { get; }
	}

	[NoiOS, NoWatch, NoTV, NoMacCatalyst, Mac (12, 0)]
	enum AVCaptionConversionAdjustmentType {
		[Field ("AVCaptionConversionAdjustmentTypeTimeRange")]
		TimeRange,
	}

	[NoWatch, NoTV, NoiOS, Mac (12, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptionConversionAdjustment {
		[Export ("adjustmentType")]
		string AdjustmentType { get; }
	}

}
