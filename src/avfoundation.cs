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

#if !WATCH
using AudioUnit;
using AVKit;
using CoreAnimation;
using CoreImage;
using CoreMedia;
using MediaToolbox;
#else
// hack: ease compilation without extra defines
using CIBarcodeDescriptor = Foundation.NSObject;
#endif
using AudioToolbox;
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;
using CoreVideo;
using ImageIO;
using System;

using OpenTK;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

#if !XAMCORE_2_0
using CMVideoDimensions = System.Drawing.Size;
#endif


#if WATCH
// hack for unexisting structs exposed as [Field]
using CMTime = Foundation.NSString;
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
	interface AVUrlAssetOptions {}
	interface AVVideoSettingsCompressed {}
	interface AVVideoSettingsUncompressed {}
	interface AUAudioUnit {}
	interface CALayer {}
	interface CIContext {}
	interface CIImage {}
	interface CMAudioFormatDescription {}
	interface CMClock {}
	interface CMFormatDescription {}
	interface CMSampleBuffer {}
	interface CMTextMarkupAttributes {}
	interface CMTimebase {}
	interface CMTimeMapping {}
	interface CMTimeRange {}
	interface CMVideoDimensions {}
	interface CMVideoFormatDescription {}
	interface CVPixelBuffer {}
	interface CVPixelBufferAttributes {}
	interface CVPixelBufferPool {}
	interface MTAudioProcessingTap {}
#endif

	delegate void AVAssetImageGeneratorCompletionHandler (CMTime requestedTime, IntPtr imageRef, CMTime actualTime, AVAssetImageGeneratorResult result, NSError error);
	delegate void AVCompletion (bool finished);
	delegate void AVRequestAccessStatus (bool accessGranted);
	delegate AVAudioBuffer AVAudioConverterInputHandler (uint inNumberOfPackets, out AVAudioConverterInputStatus outStatus);

	[NoWatch]
	[iOS (7,0)]
	[Mac (10, 9)]
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
	}

	// values are manually given since not some are platform specific
	[NoWatch]
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

		[NoTV]
		[Obsoleted (PlatformName.iOS, 6,0)]
		[Deprecated (PlatformName.iOS, 12,0, message: "Always 'null'.")]
		[Obsoleted (PlatformName.MacOSX, 10,8)]
		[Deprecated (PlatformName.MacOSX, 10,14, message: "Always 'null'.")]
		[Field ("AVMediaTypeTimedMetadata")] // last header where I can find this: iOS 5.1 SDK, 10.7 only on Mac
		TimedMetadata = 6,

		[Field ("AVMediaTypeMuxed")]
		Muxed = 7,

		[iOS (9,0)][NoMac]
		[Field ("AVMediaTypeMetadataObject")]
		MetadataObject = 8,

		[iOS (6,0)][Mac (10,8)]
		[Field ("AVMediaTypeMetadata")]
		Metadata = 9,

		[iOS (11, 0), Mac (10, 13)]
		[TV (11, 0)]
		[Field ("AVMediaTypeDepthData")]
		DepthData = 10,
	}

#if !XAMCORE_4_0
	[Obsolete ("Use AVMediaTypes enum values")]
	[NoWatch]
	[BaseType (typeof (NSObject))][Static]
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

		[NoTV][NoWatch]
		[Field ("AVMediaTypeTimedMetadata")] // last header where I can find this: iOS 5.1 SDK, 10.7 only on Mac
		[Availability (Obsoleted = Platform.iOS_6_0)]
		[Availability (Obsoleted = Platform.Mac_10_8)]
		NSString TimedMetadata { get; }

		[Field ("AVMediaTypeMuxed")]
		NSString Muxed { get; }

		[iOS (9,0)][NoMac]
		[Field ("AVMediaTypeMetadataObject")]
		NSString MetadataObject { get; }

		[iOS (6,0)][Mac (10,8)]
		[Field ("AVMediaTypeMetadata")]
		NSString Metadata { get; }
	}
#endif

	[NoWatch]
	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof(AVMetadataGroup))]
	interface AVDateRangeMetadataGroup : NSCopying, NSMutableCopying
	{
		[Export ("initWithItems:startDate:endDate:")]
		IntPtr Constructor (AVMetadataItem[] items, NSDate startDate, [NullAllowed] NSDate endDate);
	
		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; [NotImplemented] set; }
	
		[NullAllowed, Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; [NotImplemented] set; }
	
		[Export ("items", ArgumentSemantic.Copy)]
		AVMetadataItem[] Items { get; [NotImplemented] set; }
	}

	[NoWatch]
	[iOS (9,0), Mac (10,11)]
	[BaseType (typeof(AVDateRangeMetadataGroup))]
	interface AVMutableDateRangeMetadataGroup
	{
		[Export ("startDate", ArgumentSemantic.Copy)]
		[Override]
		NSDate StartDate { get; set; }
	
		[NullAllowed, Export ("endDate", ArgumentSemantic.Copy)]
		[Override]
		NSDate EndDate { get; set; }
	
		[Export ("items", ArgumentSemantic.Copy)]
		[Override]
		AVMetadataItem[] Items { get; set; }
	}
	
	[TV (11,0), NoWatch, iOS (11,0), Mac (10, 13)]
	[BaseType (typeof(NSObject))]
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
		NSNumber[] WeakAvailableDepthDataTypes { get; }

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
	[NoWatch]
	enum AVMediaCharacteristics {
		[Field ("AVMediaCharacteristicVisual")]
		Visual = 0,

		[Field ("AVMediaCharacteristicAudible")]
		Audible = 1,

		[Field ("AVMediaCharacteristicLegible")]
		Legible = 2,

		[Field ("AVMediaCharacteristicFrameBased")]
		FrameBased = 3,
		
		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Field ("AVMediaCharacteristicUsesWideGamutColorSpace")]
		UsesWideGamutColorSpace = 4,

		[Mac (10, 8)]
		[Field ("AVMediaCharacteristicIsMainProgramContent")]
		IsMainProgramContent = 5,

		[Mac (10, 8)]
		[Field ("AVMediaCharacteristicIsAuxiliaryContent")]
		IsAuxiliaryContent = 6,

		[Mac (10, 8)]
		[Field ("AVMediaCharacteristicContainsOnlyForcedSubtitles")]
		ContainsOnlyForcedSubtitles = 7,

		[Mac (10, 8)]
		[Field ("AVMediaCharacteristicTranscribesSpokenDialogForAccessibility")]
		TranscribesSpokenDialogForAccessibility = 8,

		[Mac (10, 8)]
		[Field ("AVMediaCharacteristicDescribesMusicAndSoundForAccessibility")]
		DescribesMusicAndSoundForAccessibility = 9,

		[Mac (10, 8)]
		[Field ("AVMediaCharacteristicDescribesVideoForAccessibility")]
		DescribesVideoForAccessibility = 10,

		[NoMac][iOS (6,0)]
		[Field ("AVMediaCharacteristicEasyToRead")]
		EasyToRead = 11,

		[iOS (9,0), Mac (10,11)]
		[Field ("AVMediaCharacteristicLanguageTranslation")]
		LanguageTranslation = 12,

		[iOS (9,0), Mac (10,11)]
		[Field ("AVMediaCharacteristicDubbedTranslation")]
		DubbedTranslation = 13,

		[iOS (9,0), Mac (10,11)]
		[Field ("AVMediaCharacteristicVoiceOverTranslation")]
		VoiceOverTranslation = 14,

	}

#if !XAMCORE_4_0
	[Obsolete ("Use AVMediaCharacteristics enum values")]
	[NoWatch]
	[BaseType (typeof (NSObject))][Static]
	interface AVMediaCharacteristic {
		[Field ("AVMediaCharacteristicVisual")]
		NSString Visual { get; }

		[Field ("AVMediaCharacteristicAudible")]
		NSString Audible { get; }

		[Field ("AVMediaCharacteristicLegible")]
		NSString Legible { get; }

		[Field ("AVMediaCharacteristicFrameBased")]
		NSString FrameBased { get; }
		
		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Field ("AVMediaCharacteristicUsesWideGamutColorSpace")]
		NSString UsesWideGamutColorSpace { get; }

		[Mac (10, 8)]
		[Field ("AVMediaCharacteristicIsMainProgramContent")]
		NSString IsMainProgramContent { get; }

		[Mac (10, 8)]
		[Field ("AVMediaCharacteristicIsAuxiliaryContent")]
		NSString IsAuxiliaryContent { get; }

		[Mac (10, 8)]
		[Field ("AVMediaCharacteristicContainsOnlyForcedSubtitles")]
		NSString ContainsOnlyForcedSubtitles { get; }

		[Mac (10, 8)]
		[Field ("AVMediaCharacteristicTranscribesSpokenDialogForAccessibility")]
		NSString TranscribesSpokenDialogForAccessibility { get; }

		[Mac (10, 8)]
		[Field ("AVMediaCharacteristicDescribesMusicAndSoundForAccessibility")]
		NSString DescribesMusicAndSoundForAccessibility { get; }

		[Mac (10, 8)]
		[Field ("AVMediaCharacteristicDescribesVideoForAccessibility")]
		NSString DescribesVideoForAccessibility { get;  }
#if !MONOMAC
		[iOS (6,0)]
		[Field ("AVMediaCharacteristicEasyToRead")]
		NSString EasyToRead { get; }
#endif

		[iOS (9,0), Mac (10,11)]
		[Field ("AVMediaCharacteristicLanguageTranslation")]
		NSString LanguageTranslation { get; }

		[iOS (9,0), Mac (10,11)]
		[Field ("AVMediaCharacteristicDubbedTranslation")]
		NSString DubbedTranslation { get; }

		[iOS (9,0), Mac (10,11)]
		[Field ("AVMediaCharacteristicVoiceOverTranslation")]
		NSString VoiceOverTranslation { get; }
	}
#endif

	[NoWatch]
	enum AVMetadataFormat {
		[iOS (8,0)][Mac (10,10)]
		[Field ("AVMetadataFormatHLSMetadata")]
		FormatHlsMetadata = 0,

		[Field ("AVMetadataFormatiTunesMetadata")]
		FormatiTunesMetadata = 1,

		[Field ("AVMetadataFormatID3Metadata")]
		FormatID3Metadata = 2,

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVMetadataFormatISOUserData")]
		FormatISOUserData = 3,

		[Field ("AVMetadataFormatQuickTimeUserData")]
		FormatQuickTimeUserData = 4,

		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("AVMetadataFormatUnknown")]
		Unknown = 5,
	}

	[NoWatch]
	enum AVFileTypes {
		[Field ("AVFileTypeQuickTimeMovie")]
		QuickTimeMovie = 0,
		
		[Field ("AVFileTypeMPEG4")]
		Mpeg4 = 1,
		
		[Field ("AVFileTypeAppleM4V")]
		AppleM4V = 2,

		[Mac (10,11)]
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

		[iOS (7,0), Mac (10,11)]
		[Field ("AVFileType3GPP2")]
		ThreeGpp2 = 10,

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVFileTypeMPEGLayer3")]
		MpegLayer3 = 11,

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVFileTypeSunAU")]
		SunAU = 12,

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVFileTypeAC3")]
		AC3 = 13,

		[iOS (9,0), Mac (10,11)]
		[Field ("AVFileTypeEnhancedAC3")]
		EnhancedAC3 = 14,

		[iOS (11, 0), Mac (10, 13)]
		[TV (11, 0)]
		[Field ("AVFileTypeJPEG")]
		Jpeg = 15,

		[iOS (11, 0), Mac (10, 13)]
		[TV (11, 0)]
		[Field ("AVFileTypeDNG")]
		Dng = 16,

		[iOS (11, 0), Mac (10, 13)]
		[TV (11, 0)]
		[Field ("AVFileTypeHEIC")]
		Heic = 17,

		[iOS (11, 0), Mac (10, 13)]
		[TV (11, 0)]
		[Field ("AVFileTypeAVCI")]
		Avci = 18,

		[iOS (11, 0), Mac (10, 13)]
		[TV (11, 0)]
		[Field ("AVFileTypeHEIF")]
		Heif = 19,

		[iOS (11, 0), Mac (10, 13)]
		[TV (11, 0)]
		[Field ("AVFileTypeTIFF")]
		Tiff = 20,
	}

#if !XAMCORE_4_0
	[NoWatch]
	[BaseType (typeof (NSObject))][Static]
	[Obsolete ("Use AVFileTypes enum values")]
	interface AVFileType {
		[Field ("AVFileTypeQuickTimeMovie")]
		NSString QuickTimeMovie { get; }
		
		[Field ("AVFileTypeMPEG4")]
		NSString Mpeg4 { get; }
		
		[Field ("AVFileTypeAppleM4V")]
		NSString AppleM4V { get; }
		[Mac (10,11)]
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

		[iOS (7,0), Mac (10,11)]
		[Field ("AVFileType3GPP2")]
		NSString ThreeGpp2 { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVFileTypeMPEGLayer3")]
		NSString MpegLayer3 { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVFileTypeSunAU")]
		NSString SunAU { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVFileTypeAC3")]
		NSString AC3 { get; }

		[iOS (9,0), Mac (10,11)]
		[Field ("AVFileTypeEnhancedAC3")]
		NSString EnhancedAC3 { get; }
	}
#endif

	[NoWatch]
	[iOS (9,0), Mac (10,11)]
	[Static]
	interface AVStreamingKeyDelivery {

		[Field ("AVStreamingKeyDeliveryContentKeyType")]
		NSString ContentKeyType { get; }

		[Field ("AVStreamingKeyDeliveryPersistentContentKeyType")]
		NSString PersistentContentKeyType { get; }
	}

	[NoWatch]
	[NoTV]
	[iOS (7,0)] // And OSX 10.7
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
	[BaseType (typeof (NSObject))][Static]
	interface AVVideo {
		[Field ("AVVideoCodecKey")]
		NSString CodecKey { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVVideoMaxKeyFrameIntervalDurationKey")]
		NSString MaxKeyFrameIntervalDurationKey { get; }

		[iOS (7,0)]
		[Mac (10,10)]
		[Field ("AVVideoAllowFrameReorderingKey")]
		NSString AllowFrameReorderingKey { get; }

		[iOS (7,0)]
		[Mac (10,10)]
		[Field ("AVVideoAverageNonDroppableFrameRateKey")]
		NSString AverageNonDroppableFrameRateKey { get; }

		[NoiOS, NoTV]
		[Mac (10,10)]
		[Field ("AVVideoEncoderSpecificationKey")]
		NSString EncoderSpecificationKey { get; }

		[iOS (7,0)]
		[Mac (10,10)]
		[Field ("AVVideoExpectedSourceFrameRateKey")]
		NSString ExpectedSourceFrameRateKey { get; }

		[iOS (7,0)]
		[Mac (10,10)]
		[Field ("AVVideoH264EntropyModeCABAC")]
		NSString H264EntropyModeCABAC { get; }

		[iOS (7, 0)]
		[Mac (10,10)]
		[Field ("AVVideoH264EntropyModeCAVLC")]
		NSString H264EntropyModeCAVLC { get; }

		[iOS (7,0)]
		[Mac (10,10)]
		[Field ("AVVideoH264EntropyModeKey")]
		NSString H264EntropyModeKey { get; }

		[Mac (10, 7)]
		[TV (9, 0)]
		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'AVVideoCodecType' enum instead.")]
		[Field ("AVVideoCodecH264")]
		NSString CodecH264 { get; }
		
		[Mac (10, 7)]
		[TV (9, 0)]
		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'AVVideoCodecType' enum instead.")]
		[Field ("AVVideoCodecJPEG")]
		NSString CodecJPEG { get; }
		
		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'AVVideoCodecType' enum instead.")]
		[NoiOS, NoTV, Mac (10,7)]
		[Field ("AVVideoCodecAppleProRes4444")]
		NSString AppleProRes4444 { get; }

		[Mac (10, 7)]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'AVVideoCodecType' enum instead.")]
		[NoiOS, NoTV, Mac (10,7)]
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

		[Mac (10, 8)]
		[Field ("AVVideoProfileLevelH264Main32")]
		NSString ProfileLevelH264Main32 { get; }

		[Field ("AVVideoProfileLevelH264Main41")]
		NSString ProfileLevelH264Main41 { get; }

		[iOS (6,0), Mac (10, 9)]
		[Field ("AVVideoProfileLevelH264High40")]
		NSString ProfileLevelH264High40 { get; }

		[iOS (6,0), Mac (10, 9)]
		[Field ("AVVideoProfileLevelH264High41")]
		NSString ProfileLevelH264High41 { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVVideoProfileLevelH264BaselineAutoLevel")]
		NSString ProfileLevelH264BaselineAutoLevel { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Field ("AVVideoProfileLevelH264MainAutoLevel")]
		NSString ProfileLevelH264MainAutoLevel { get; }
		
		[iOS (7,0), Mac (10, 9)]
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
	[Static]
	interface AVVideoScalingModeKey
	{
		[Field ("AVVideoScalingModeFit")]
		NSString Fit { get; }

		[Field ("AVVideoScalingModeResize")]
		NSString Resize { get; }

		[Field ("AVVideoScalingModeResizeAspect")]
		NSString ResizeAspect { get; }

		[Field ("AVVideoScalingModeResizeAspectFill")]
		NSString ResizeAspectFill { get; }
	}

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
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

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // fails (nil handle on iOS 10)
	interface AVAudioChannelLayout : NSSecureCoding {
		[Export ("initWithLayoutTag:")]
		IntPtr Constructor (/* UInt32 */ uint layoutTag);

		[DesignatedInitializer]
		[Export ("initWithLayout:"), Internal]
		IntPtr Constructor (nint /* This is really an IntPtr, but it conflicts with the default (Handle) ctor. */ layout);

		[Export ("layoutTag")]
		uint /* AudioChannelLayoutTag = UInt32 */ LayoutTag { get; }

		[Export ("layout"), Internal]
		IntPtr _Layout { get; }

		[Export ("channelCount")]
		uint /* AVAudioChannelCount = uint32_t */ ChannelCount { get; }

		[Export ("isEqual:"), Internal]
		bool IsEqual (NSObject other);
	}

	[Watch (3,0)]
	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof(AVAudioBuffer))]
	[DisableDefaultCtor] // just like base class (AVAudioBuffer) can't, avoid crash when ToString call `description`
	interface AVAudioCompressedBuffer
	{
		[Export ("initWithFormat:packetCapacity:maximumPacketSize:")]
		IntPtr Constructor (AVAudioFormat format, uint packetCapacity, nint maximumPacketSize);
	
		[Export ("initWithFormat:packetCapacity:")]
		IntPtr Constructor (AVAudioFormat format, uint packetCapacity);
	
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

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("byteCapacity")]
		uint ByteCapacity { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("byteLength")]
		uint ByteLength { get; set; }
	}

	[Watch (3,0)]
	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // fails (nil handle on iOS 10)
	interface AVAudioConnectionPoint
	{
		[Export ("initWithNode:bus:")]
		[DesignatedInitializer]
		IntPtr Constructor (AVAudioNode node, nuint bus);
	
		[NullAllowed, Export ("node", ArgumentSemantic.Weak)]
		AVAudioNode Node { get; }
	
		[Export ("bus")]
		nuint Bus { get; }
	}

#if XAMCORE_2_0
	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	delegate AVAudioEngineManualRenderingStatus AVAudioEngineManualRenderingBlock (/* AVAudioFrameCount = uint */ uint numberOfFrames, AudioBuffers outBuffer, [NullAllowed] /* OSStatus */ ref int outError);
#endif

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (NSObject))]
	interface AVAudioEngine {

		[NoWatch]
		[Export ("musicSequence"), NullAllowed]
		MusicSequence MusicSequence { get; set; }

		[Export ("outputNode")]
		AVAudioOutputNode OutputNode { get; }

		[TV (11,0)]
		[Watch (4,0)]
		[Export ("inputNode"), NullAllowed]
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

		[iOS (9,0), Mac (10,11)]
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

		[iOS (9,0), Mac (10,11)]
		[return: NullAllowed]
		[Export ("inputConnectionPointForNode:inputBus:")]
		AVAudioConnectionPoint InputConnectionPoint (AVAudioNode node, nuint bus);

		[iOS (9,0), Mac (10,11)]
		[return: NullAllowed]
		[Export ("outputConnectionPointsForNode:outputBus:")]
		AVAudioConnectionPoint [] OutputConnectionPoints (AVAudioNode node, nuint bus);

		[Notification]
		[Field ("AVAudioEngineConfigurationChangeNotification")]
		NSString ConfigurationChangeNotification { get; }

		[TV (11, 0), NoWatch, Mac (10, 13), iOS (11, 0)]
		[Export ("autoShutdownEnabled")]
		bool AutoShutdownEnabled { [Bind ("isAutoShutdownEnabled")] get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("enableManualRenderingMode:format:maximumFrameCount:error:")]
		bool EnableManualRenderingMode (AVAudioEngineManualRenderingMode mode, AVAudioFormat pcmFormat, uint maximumFrameCount, out NSError outError);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("renderOffline:toBuffer:error:")]
		AVAudioEngineManualRenderingStatus RenderOffline (uint numberOfFrames, AVAudioPcmBuffer buffer, [NullAllowed] out NSError outError);

#if XAMCORE_2_0
		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("manualRenderingBlock")]
		AVAudioEngineManualRenderingBlock ManualRenderingBlock { get; }
#endif

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("isInManualRenderingMode")]
		bool InManualRenderingMode { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("manualRenderingMode")]
		AVAudioEngineManualRenderingMode ManualRenderingMode { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("manualRenderingFormat")]
		AVAudioFormat ManualRenderingFormat { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("manualRenderingMaximumFrameCount")]
		uint ManualRenderingMaximumFrameCount { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("manualRenderingSampleTime")]
		long ManualRenderingSampleTime { get; }

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("disableManualRenderingMode")]
		void DisableManualRenderingMode ();

#if !WATCH
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0), NoWatch]
		[Export ("connectMIDI:to:format:block:")]
		void ConnectMidi (AVAudioNode sourceNode, AVAudioNode destinationNode, [NullAllowed] AVAudioFormat format, [NullAllowed] AUMidiOutputEventBlock tapHandler);

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0), NoWatch]
		[Export ("connectMIDI:toNodes:format:block:")]
		void ConnectMidi (AVAudioNode sourceNode, AVAudioNode[] destinationNodes, [NullAllowed] AVAudioFormat format, [NullAllowed] AUMidiOutputEventBlock tapHandler);
#endif

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0), NoWatch]
		[Export ("disconnectMIDI:from:")]
		void DisconnectMidi (AVAudioNode sourceNode, AVAudioNode destinationNode);

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0), NoWatch]
		[Export ("disconnectMIDI:fromNodes:")]
		void DisconnectMidi (AVAudioNode sourceNode, AVAudioNode[] destinationNodes);

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0), NoWatch]
		[Export ("disconnectMIDIInput:")]
		void DisconnectMidiInput (AVAudioNode node);

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0), NoWatch]
		[Export ("disconnectMIDIOutput:")]
		void DisconnectMidiOutput (AVAudioNode node);
	}

	[NoWatch]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (AVAudioNode))]
	[DisableDefaultCtor] // designated
	interface AVAudioEnvironmentNode : AVAudioMixing {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

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
#if XAMCORE_4_0
		NSNumber [] ApplicableRenderingAlgorithms { get; }
#else
		NSObject [] ApplicableRenderingAlgorithms ();
#endif

		[Export ("outputVolume")]
		float OutputVolume { get; set; } /* float, not CGFloat */
	}

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
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

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
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
	
	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (NSObject))]
	interface AVAudioFile {
		[Export ("initForReading:error:")]
		IntPtr Constructor (NSUrl fileUrl, out NSError outError);

		[Export ("initForReading:commonFormat:interleaved:error:")]
		IntPtr Constructor (NSUrl fileUrl, AVAudioCommonFormat format, bool interleaved, out NSError outError);

		[Export ("initForWriting:settings:error:"), Internal]
		IntPtr Constructor (NSUrl fileUrl, NSDictionary settings, out NSError outError);

		[Wrap ("this (fileUrl, settings == null ? null : settings.Dictionary, out outError)")]
		IntPtr Constructor (NSUrl fileUrl, AudioSettings settings, out NSError outError);

		[Export ("initForWriting:settings:commonFormat:interleaved:error:"), Internal]
		IntPtr Constructor (NSUrl fileUrl, NSDictionary settings, AVAudioCommonFormat format, bool interleaved, out NSError outError);
		
		[Wrap ("this (fileUrl, settings == null ? null : settings.Dictionary, format, interleaved, out outError)")]
		IntPtr Constructor (NSUrl fileUrl, AudioSettings settings, AVAudioCommonFormat format, bool interleaved, out NSError outError);

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

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (NSObject))]
	interface AVAudioFormat : NSSecureCoding {
		[Export ("initWithStreamDescription:")]
		IntPtr Constructor (ref AudioStreamBasicDescription description);

		[Export ("initWithStreamDescription:channelLayout:")]
		IntPtr Constructor (ref AudioStreamBasicDescription description, [NullAllowed] AVAudioChannelLayout layout);

		[Export ("initStandardFormatWithSampleRate:channels:")]
		IntPtr Constructor (double sampleRate, uint /* AVAudioChannelCount = uint32_t */ channels);

		[Export ("initStandardFormatWithSampleRate:channelLayout:")]
		IntPtr Constructor (double sampleRate, AVAudioChannelLayout layout);

		[Export ("initWithCommonFormat:sampleRate:channels:interleaved:")]
		IntPtr Constructor (AVAudioCommonFormat format, double sampleRate, uint /* AVAudioChannelCount = uint32_t */ channels, bool interleaved);

		[Export ("initWithCommonFormat:sampleRate:interleaved:channelLayout:")]
		IntPtr Constructor (AVAudioCommonFormat format, double sampleRate, bool interleaved, AVAudioChannelLayout layout);

		[Export ("initWithSettings:")]
		IntPtr Constructor (NSDictionary settings);

		[Wrap ("this (settings.Dictionary)")]
		IntPtr Constructor (AudioSettings settings);

		[NoWatch]
		[iOS (9,0)][Mac (10,11)]
		[Export ("initWithCMAudioFormatDescription:")]
		IntPtr Constructor (CMAudioFormatDescription formatDescription);

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

		[Export ("streamDescription")]
		AudioStreamBasicDescription StreamDescription { get; }

		[Export ("channelLayout"), NullAllowed]
		AVAudioChannelLayout ChannelLayout { get; }

		[Export ("settings")]
		NSDictionary WeakSettings { get; }

		[Wrap ("WeakSettings")]
		AudioSettings Settings { get; }

		[NoWatch]
		[iOS (9,0)][Mac (10,11)]
		[Export ("formatDescription")]
		CMAudioFormatDescription FormatDescription { get; }

		[Export ("isEqual:"), Internal]
		bool IsEqual (NSObject obj);
		
		[iOS (10,0), TV (10,0), Watch (3,0), Mac (10,12)]
		[NullAllowed, Export ("magicCookie", ArgumentSemantic.Retain)]
		NSData MagicCookie { get; set; }
	}

	[NoWatch] // all members are unavailable
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
	}

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
	[Protocol]
	interface AVAudioMixing : AVAudioStereoMixing
#if !WATCH
		, AVAudio3DMixing
#endif
	{

		[iOS (9,0), Mac (10,11)]
#if XAMCORE_4_0
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

	[Watch (3,0)]
	[iOS (9,0), Mac (10,11)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // Default constructor not allowed : Objective-C exception thrown
	interface AVAudioMixingDestination : AVAudioMixing {

		[Export ("connectionPoint")]
		AVAudioConnectionPoint ConnectionPoint { get; }
	}

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVAudioStereoMixing {
		[Abstract]
		[Export ("pan")]
		float Pan { get; set; } /* float, not CGFloat */
	}
	
	delegate void AVAudioNodeTapBlock (AVAudioPcmBuffer buffer, AVAudioTime when);

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
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
		string GetNameForInputBus (nuint bus);

		[Export ("nameForOutputBus:")]
		string GetNameForOutputBus (nuint bus);

		[Export ("installTapOnBus:bufferSize:format:block:")]
		void InstallTapOnBus (nuint bus, uint /* AVAudioFrameCount = uint32_t */ bufferSize, [NullAllowed] AVAudioFormat format, AVAudioNodeTapBlock tapBlock);

		[Export ("removeTapOnBus:")]
		void RemoveTapOnBus (nuint bus);

#if XAMCORE_2_0
		[NoWatch, TV (11, 0), Mac (10, 13, onlyOn64 : true), iOS (11, 0)]
		[Export ("AUAudioUnit")]
		AUAudioUnit AUAudioUnit { get; }
#endif

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("latency")]
		double Latency { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("outputPresentationLatency")]
		double OutputPresentationLatency { get; }
	}
	
	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (AVAudioNode))]
	[DisableDefaultCtor] // documented as a base class - returned Handle is nil
	interface AVAudioIONode {
		[Export ("presentationLatency")]
		double PresentationLatency { get; }

		[NoWatch]
		[Export ("audioUnit"), NullAllowed]
		global::AudioUnit.AudioUnit AudioUnit { get; }
	}

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (AVAudioNode))]
	[DisableDefaultCtor] // designated
	interface AVAudioMixerNode : AVAudioMixing {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("outputVolume")]
		float OutputVolume { get; set; } /* float, not CGFloat */

		[Export ("nextAvailableInputBus")]
		nuint NextAvailableInputBus { get; }
	}

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
	[DisableDefaultCtor] // returned Handle is nil
	// note: sample source (header) suggest it comes from AVAudioEngine properties
	[BaseType (typeof (AVAudioIONode))]
	interface AVAudioOutputNode {

	}	

#if XAMCORE_2_0
	[Watch (4,0), TV (11,0), Mac (10,10), iOS (8,0)]
	delegate AudioBuffers AVAudioIONodeInputBlock (uint frameCount);
#endif

 	[Watch (4,0)]
 	[iOS (8,0)][Mac (10,10)][TV (11,0)]
 	[BaseType (typeof (AVAudioIONode))]
	[DisableDefaultCtor] // returned Handle is nil
	// note: sample source (header) suggest it comes from AVAudioEngine properties
	interface AVAudioInputNode : AVAudioMixing {

#if XAMCORE_2_0
		[Mac (10,13), iOS (11,0)]
		[Export ("setManualRenderingInputPCMFormat:inputBlock:")]
		bool SetManualRenderingInputPcmFormat (AVAudioFormat format, AVAudioIONodeInputBlock block);
#endif
	}

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (AVAudioBuffer), Name="AVAudioPCMBuffer")]
	[DisableDefaultCtor] // crash in tests
	interface AVAudioPcmBuffer {

		[DesignatedInitializer]
		[Export ("initWithPCMFormat:frameCapacity:")]
		IntPtr Constructor (AVAudioFormat format, uint /* AVAudioFrameCount = uint32_t */ frameCapacity);

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

	[Watch (3,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVAudioPlayer {
		[Export ("initWithContentsOfURL:error:")][Internal]
		IntPtr Constructor (NSUrl url, IntPtr outError);
	
		[Export ("initWithData:error:")][Internal]
		IntPtr Constructor (NSData  data, IntPtr outError);

		[Export ("prepareToPlay")]
		bool PrepareToPlay ();
	
		[Export ("play")]
		bool Play ();
	
		[Export ("pause")]
		void Pause ();
	
		[Export ("stop")]
		void Stop ();
	
		[Export ("playing")]
		bool Playing { [Bind ("isPlaying")] get;  }
	
		[Export ("numberOfChannels")]
		nuint NumberOfChannels { get;  }
	
		[Export ("duration")]
		double Duration { get;  }
	
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set;  }

		[Wrap ("WeakDelegate"), NullAllowed]
		[Protocolize]
		AVAudioPlayerDelegate Delegate { get; set; }
	
		[Export ("url"), NullAllowed]
		NSUrl Url { get;  }
	
		[Export ("data"), NullAllowed]
		NSData Data { get;  }
	
		[Export ("volume")]
		float Volume { get; set;  } // defined as 'float'

		[iOS (10,0), TV (10,0), Mac (10,12)]
		[Export ("setVolume:fadeDuration:")]
		void SetVolume (float volume, double duration);
	
		[Export ("currentTime")]
		double CurrentTime { get; set;  }
	
		[Export ("numberOfLoops")]
		nint NumberOfLoops { get; set;  }
	
		[Export ("meteringEnabled")]
		bool MeteringEnabled { [Bind ("isMeteringEnabled")] get; set;  }
	
		[Export ("updateMeters")]
		void UpdateMeters ();
	
		[Export ("peakPowerForChannel:")]
		float PeakPower (nuint channelNumber); // defined as 'float'
	
		[Export ("averagePowerForChannel:")]
		float AveragePower (nuint channelNumber); // defined as 'float'

		[Export ("deviceCurrentTime")]
		double DeviceCurrentTime { get;  }

		[Export ("pan")]
		float Pan { get; set; } // defined as 'float'

		[Export ("playAtTime:")]
		bool PlayAtTime (double time);

		[Export ("settings")][Protected]
		NSDictionary WeakSettings { get;  }

		[Wrap ("WeakSettings")]
		AudioSettings SoundSetting { get; }

		[Export ("enableRate")]
		bool EnableRate { get; set; }

		[Export ("rate")]
		float Rate { get; set; } // defined as 'float'		
#if !MONOMAC
		[iOS (6,0)]
		[Export ("channelAssignments", ArgumentSemantic.Copy), NullAllowed]
		AVAudioSessionChannelDescription [] ChannelAssignments { get; set; }
#endif
		[iOS (7,0), Mac (10,9), Export ("initWithData:fileTypeHint:error:")]
		IntPtr Constructor (NSData data, [NullAllowed] string fileTypeHint, out NSError outError);

		[iOS (7,0), Mac (10,9), Export ("initWithContentsOfURL:fileTypeHint:error:")]
		IntPtr Constructor (NSUrl url, [NullAllowed] string fileTypeHint, out NSError outError);

		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Watch (4,0)]
		[Export ("format")]
		AVAudioFormat Format { get; }

		[NoiOS, Mac (10, 13), NoTV, NoWatch]
		[NullAllowed, Export ("currentDevice")]
		string CurrentDevice { get; set; }
	}
	
	[Watch (3,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVAudioPlayerDelegate {
		[Export ("audioPlayerDidFinishPlaying:successfully:"), CheckDisposed]
		void FinishedPlaying (AVAudioPlayer player, bool flag);
	
		[Export ("audioPlayerDecodeErrorDidOccur:error:")]
		void DecoderError (AVAudioPlayer player, [NullAllowed] NSError error);

#if !MONOMAC
		[Availability (Deprecated = Platform.iOS_8_0)]
		[Export ("audioPlayerBeginInterruption:")]
		void BeginInterruption (AVAudioPlayer  player);
	
		[Export ("audioPlayerEndInterruption:")]
		[Availability (Deprecated = Platform.iOS_6_0)]
		void EndInterruption (AVAudioPlayer player);

		[Availability (Deprecated = Platform.iOS_8_0)]
		[iOS (6,0)]
		[Export ("audioPlayerEndInterruption:withOptions:")]
		void EndInterruption (AVAudioPlayer player, AVAudioSessionInterruptionFlags flags);
#endif
	}

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (AVAudioNode))]
	[DisableDefaultCtor] // designated
	interface AVAudioPlayerNode : AVAudioMixing {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("playing")]
		bool Playing { [Bind ("isPlaying")] get; }

		[Async]
		[Export ("scheduleBuffer:completionHandler:")]
		void ScheduleBuffer (AVAudioPcmBuffer buffer, [NullAllowed] Action completionHandler);

		[Async]
		[Export ("scheduleBuffer:atTime:options:completionHandler:")]
		void ScheduleBuffer (AVAudioPcmBuffer buffer, [NullAllowed] AVAudioTime when, AVAudioPlayerNodeBufferOptions options, [NullAllowed] Action completionHandler);

		[Async]
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("scheduleBuffer:completionCallbackType:completionHandler:")]
		void ScheduleBuffer (AVAudioPcmBuffer buffer, AVAudioPlayerNodeCompletionCallbackType callbackType, [NullAllowed] Action<AVAudioPlayerNodeCompletionCallbackType> completionHandler);

		[Async]
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("scheduleBuffer:atTime:options:completionCallbackType:completionHandler:")]
		void ScheduleBuffer (AVAudioPcmBuffer buffer, [NullAllowed] AVAudioTime when, AVAudioPlayerNodeBufferOptions options, AVAudioPlayerNodeCompletionCallbackType callbackType, [NullAllowed] Action<AVAudioPlayerNodeCompletionCallbackType> completionHandler);

		[Async]
		[Export ("scheduleFile:atTime:completionHandler:")]
		void ScheduleFile (AVAudioFile file, [NullAllowed] AVAudioTime when, [NullAllowed] Action completionHandler);

		[Async]
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("scheduleFile:atTime:completionCallbackType:completionHandler:")]
		void ScheduleFile (AVAudioFile file, [NullAllowed] AVAudioTime when, AVAudioPlayerNodeCompletionCallbackType callbackType, [NullAllowed] Action<AVAudioPlayerNodeCompletionCallbackType> completionHandler);

		[Async]
		[Export ("scheduleSegment:startingFrame:frameCount:atTime:completionHandler:")]
		void ScheduleSegment (AVAudioFile file, long startFrame, uint /* AVAudioFrameCount = uint32_t */ numberFrames, [NullAllowed] AVAudioTime when, [NullAllowed] Action completionHandler);

		[Async]
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
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
	[Watch (4,0)]
	interface AVAudioRecorder {
		[Export ("initWithURL:settings:error:")][Internal]
		IntPtr InitWithUrl (NSUrl url, NSDictionary settings, out NSError error);
		
		[Internal]
		[iOS (10,0), Mac (10,12)]
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
		bool Recording { [Bind ("isRecording")] get;  }
	
		[Export ("url")]
		NSUrl Url { get;  }

#if XAMCORE_2_0
		[Export ("settings")]
		NSDictionary WeakSettings { get;  }

		[Wrap ("WeakSettings")]
		AudioSettings Settings { get; }
#else
		[Export ("settings")]
		[Advice ("Use 'AudioSettings' property instead.")]
		NSDictionary Settings { get;  }

		[Wrap ("Settings")]
		AudioSettings AudioSettings { get; }
#endif

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set;  }

		[Wrap ("WeakDelegate"), NullAllowed]
		[Protocolize]
		AVAudioRecorderDelegate Delegate { get; set;  }
	
		[Export ("currentTime")]
		double currentTime { get; }
	
		[Export ("meteringEnabled")]
		bool MeteringEnabled { [Bind ("isMeteringEnabled")] get; set;  }
	
		[Export ("updateMeters")]
		void UpdateMeters ();
	
		[Export ("peakPowerForChannel:")]
		float PeakPower (nuint channelNumber); // defined as 'float'
	
		[Export ("averagePowerForChannel:")]
		float AveragePower (nuint channelNumber); // defined as 'float'
#if !MONOMAC
		[iOS (6,0)]
		[Export ("channelAssignments", ArgumentSemantic.Copy), NullAllowed]
		AVAudioSessionChannelDescription [] ChannelAssignments { get; set; }
#endif
		[iOS (6,0)]
		[Mac (10,14, onlyOn64: true)]
		[Export ("recordAtTime:")]
		bool RecordAt (double time);

		[iOS (6,0)]
		[Mac (10,14, onlyOn64: true)]
		[Export ("recordAtTime:forDuration:")]
		bool RecordAt (double time, double duration);

		[iOS (6,0)]
		[Mac (10,14, onlyOn64: true)]
		[Export ("deviceCurrentTime")]
		double DeviceCurrentTime { get; }

		[iOS (10,0), Mac (10,12)]
		[Export ("format")]
		AVAudioFormat Format { get; }
	}
	
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[NoTV]
	[Watch (4,0)]
	interface AVAudioRecorderDelegate {
		[Export ("audioRecorderDidFinishRecording:successfully:"), CheckDisposed]
		void FinishedRecording (AVAudioRecorder recorder, bool flag);
	
		[Export ("audioRecorderEncodeErrorDidOccur:error:")]
		void EncoderError (AVAudioRecorder recorder, [NullAllowed] NSError error);

#if !MONOMAC
		[Availability (Deprecated = Platform.iOS_8_0)]
		[Export ("audioRecorderBeginInterruption:")]
		void BeginInterruption (AVAudioRecorder  recorder);

		[Availability (Deprecated = Platform.iOS_6_0)]
		[Export ("audioRecorderEndInterruption:")]
		void EndInterruption (AVAudioRecorder  recorder);

		// Deprecated in iOS 6.0 but we have same C# signature as a method that was deprecated in iOS 8.0
		[Availability (Deprecated = Platform.iOS_8_0)]
		[Export ("audioRecorderEndInterruption:withFlags:")]
		void EndInterruption (AVAudioRecorder recorder, AVAudioSessionInterruptionFlags flags);

		//[Availability (Deprecated = Platform.iOS_8_0)]
		//[iOS (6,0)]
		//[Export ("audioRecorderEndInterruption:withOptions:")]
		//void EndInterruption (AVAudioRecorder recorder, AVAudioSessionInterruptionFlags flags);
#endif
	}

#if !MONOMAC

	interface AVAudioSessionSecondaryAudioHintEventArgs {
		[Export ("AVAudioSessionSilenceSecondaryAudioHintNotification")]
		AVAudioSessionSilenceSecondaryAudioHintType Hint { get; }

		[Export ("AVAudioSessionSilenceSecondaryAudioHintTypeKey")]
		AVAudioSessionRouteDescription HintType { get; }
	}

	delegate void AVPermissionGranted (bool granted);

	[Watch (3,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // for binary compatibility this is added in AVAudioSession.cs w/[Obsolete]
	interface AVAudioSession {
		
		[Export ("sharedInstance"), Static]
		AVAudioSession SharedInstance ();
	
		[NoWatch]
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		[NoTV]
		NSObject WeakDelegate { get; set;  }

		[NoWatch]
		[Wrap ("WeakDelegate")]
		[Protocolize]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'AVAudioSession.Notification.Observe*' methods instead.")]
		[NoTV]
		AVAudioSessionDelegate Delegate { get; set;  }
	
		[Export ("setActive:error:")]
		bool SetActive (bool beActive, out NSError outError);

		[NoTV]
		[Export ("setActive:withFlags:error:")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'SetActive (bool, AVAudioSessionSetActiveOptions, out NSError)' instead.")]
		bool SetActive (bool beActive, AVAudioSessionFlags flags, out NSError outError);

		[Export ("setCategory:error:")]
		bool SetCategory (NSString theCategory, out NSError outError);
	
		[NoTV]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'SetPreferredSampleRate' instead.")]
		[Export ("setPreferredHardwareSampleRate:error:")]
		bool SetPreferredHardwareSampleRate (double sampleRate, out NSError outError);
	
		[NoWatch]
		[Export ("setPreferredIOBufferDuration:error:")]
		bool SetPreferredIOBufferDuration (double duration, out NSError outError);
	
		[Export ("category")]
		NSString Category { get;  }

		[Export ("mode")]
		NSString Mode { get; }

		[Export ("setMode:error:")]
		bool SetMode (NSString mode, out NSError error);
	
		[NoTV]
		[Export ("preferredHardwareSampleRate")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'PreferredSampleRate' instead.")]
		double PreferredHardwareSampleRate { get;  }
	
		[NoWatch]
		[Export ("preferredIOBufferDuration")]
		double PreferredIOBufferDuration { get;  }
	
		[NoTV]
		[Export ("inputIsAvailable")]
		[Availability (Deprecated = Platform.iOS_6_0)]
		bool InputIsAvailable { get;  }
	
		[NoTV]
		[Export ("currentHardwareSampleRate")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'SampleRate' instead.")]
		double CurrentHardwareSampleRate { get;  }

		[NoTV]
		[Export ("currentHardwareInputNumberOfChannels")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'InputNumberOfChannels' instead.")]
		nint CurrentHardwareInputNumberOfChannels { get;  }
	
		[NoTV]
		[Export ("currentHardwareOutputNumberOfChannels")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'OutputNumberOfChannels' instead.")]
		nint CurrentHardwareOutputNumberOfChannels { get;  }

		[Field ("AVAudioSessionCategoryAmbient")]
		NSString CategoryAmbient { get; }

		[Field ("AVAudioSessionCategorySoloAmbient")]
		NSString CategorySoloAmbient { get; }

		[Field ("AVAudioSessionCategoryPlayback")]
		NSString CategoryPlayback { get; }

		[Field ("AVAudioSessionCategoryRecord")]
		NSString CategoryRecord { get; }

		[Field ("AVAudioSessionCategoryPlayAndRecord")]
		NSString CategoryPlayAndRecord { get; }

		[NoTV][NoWatch]
		[Availability (Deprecated = Platform.iOS_10_0)] // FIXME: Find the new value to use
		[Field ("AVAudioSessionCategoryAudioProcessing")]
		NSString CategoryAudioProcessing { get; }

		[Field ("AVAudioSessionModeDefault")]
		NSString ModeDefault { get; }

		[Field ("AVAudioSessionModeVoiceChat")]
		NSString ModeVoiceChat { get; }

		[Field ("AVAudioSessionModeVideoRecording")]
		NSString ModeVideoRecording { get; }

		[Field ("AVAudioSessionModeMeasurement")]
		NSString ModeMeasurement { get; }

		[Field ("AVAudioSessionModeGameChat")]
		NSString ModeGameChat { get; }

		[Watch (5, 0), TV (12, 0), NoMac, iOS (12, 0)]
		[Field ("AVAudioSessionModeVoicePrompt")]
		NSString VoicePrompt { get; }

		[iOS (6,0)]
		[Export ("setActive:withOptions:error:")]
		bool SetActive (bool active, AVAudioSessionSetActiveOptions options, out NSError outError);

		[iOS (9,0)]
		[Export ("availableCategories")]
		string [] AvailableCategories { get; }

		[iOS (6,0)]
		[Export ("setCategory:withOptions:error:")]
		bool SetCategory (string category, AVAudioSessionCategoryOptions options, out NSError outError);
		
		[iOS (10,0), TV (10,0), Watch (3,0)]
		[Export ("setCategory:mode:options:error:")]
		bool SetCategory (string category, string mode, AVAudioSessionCategoryOptions options, out NSError outError);

		[iOS (6,0)]
		[Export ("categoryOptions")]
		AVAudioSessionCategoryOptions CategoryOptions { get;  }

		[iOS (9,0)]
		[Export ("availableModes")]
		string [] AvailableModes { get; }

		[iOS (6,0)]
		[Export ("overrideOutputAudioPort:error:")]
		bool OverrideOutputAudioPort (AVAudioSessionPortOverride portOverride, out NSError outError);

		[iOS (6,0)]
		[Export ("otherAudioPlaying")]
		bool OtherAudioPlaying { [Bind ("isOtherAudioPlaying")] get;  }

		[iOS (6,0)]
		[Export ("currentRoute")]
		AVAudioSessionRouteDescription CurrentRoute { get;  }

		[NoWatch, iOS (6,0)]
		[Export ("setPreferredSampleRate:error:")]
		bool SetPreferredSampleRate (double sampleRate, out NSError error);
		
		[NoWatch, iOS (6,0)]
		[Export ("preferredSampleRate")]
		double PreferredSampleRate { get;  }

		[NoWatch]
		[iOS (6,0)]
		[Export ("inputGain")]
		float InputGain { get;  } // defined as 'float'

		[NoWatch]
		[iOS (6,0)]
		[Export ("inputGainSettable")]
		bool InputGainSettable { [Bind ("isInputGainSettable")] get;  }

		[iOS (6,0)]
		[Export ("inputAvailable")]
		bool InputAvailable { [Bind ("isInputAvailable")] get;  }

		[iOS (6,0)]
		[Export ("sampleRate")]
		double SampleRate { get;  }

		[iOS (6,0)]
		[Export ("inputNumberOfChannels")]
		nint InputNumberOfChannels { get;  }

		[iOS (6,0)]
		[Export ("outputNumberOfChannels")]
		nint OutputNumberOfChannels { get;  }

		[iOS (6,0)]
		[Export ("outputVolume")]
		float OutputVolume { get;  } // defined as 'float'

		[iOS (6,0)]
		[Export ("inputLatency")]
		double InputLatency { get;  }

		[iOS (6,0)]
		[Export ("outputLatency")]
		double OutputLatency { get;  }

		[iOS (6,0)]
		[Export ("IOBufferDuration")]
		double IOBufferDuration { get;  }

		[NoWatch]
		[iOS (6,0)]
		[Export ("setInputGain:error:")]
		bool SetInputGain (float /* defined as 'float' */ gain, out NSError outError);

		[iOS (6,0)]
		[Field ("AVAudioSessionInterruptionNotification")]
		[Notification (typeof (AVAudioSessionInterruptionEventArgs))]
		NSString InterruptionNotification { get; }

		[iOS (6,0)]
		[Field ("AVAudioSessionRouteChangeNotification")]
		[Notification (typeof (AVAudioSessionRouteChangeEventArgs))]
		NSString RouteChangeNotification { get; }

		[iOS (6,0)]
		[Field ("AVAudioSessionMediaServicesWereResetNotification")]
		[Notification]
		NSString MediaServicesWereResetNotification { get; }

		[iOS (7,0), Notification, Field ("AVAudioSessionMediaServicesWereLostNotification")]
		NSString MediaServicesWereLostNotification { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionCategoryMultiRoute")]
		NSString CategoryMultiRoute { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionModeMoviePlayback")]
		NSString ModeMoviePlayback { get; }

		[iOS (7,0)]
		[Field ("AVAudioSessionModeVideoChat")]
		NSString ModeVideoChat { get; }

		[iOS (9,0)]
		[Field ("AVAudioSessionModeSpokenAudio")]
		NSString ModeSpokenAudio { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionPortLineIn")]
		NSString PortLineIn { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionPortBuiltInMic")]
		NSString PortBuiltInMic { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionPortHeadsetMic")]
		NSString PortHeadsetMic { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionPortLineOut")]
		NSString PortLineOut { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionPortHeadphones")]
		NSString PortHeadphones { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionPortBluetoothA2DP")]
		NSString PortBluetoothA2DP { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionPortBuiltInReceiver")]
		NSString PortBuiltInReceiver { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionPortBuiltInSpeaker")]
		NSString PortBuiltInSpeaker { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionPortHDMI")]
		NSString PortHdmi { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionPortAirPlay")]
		NSString PortAirPlay { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionPortBluetoothHFP")]
		NSString PortBluetoothHfp { get; }
		
		[iOS (6,0)]
		[Field ("AVAudioSessionPortUSBAudio")]
		NSString PortUsbAudio { get; }

		[iOS (7,0)]
		[Field ("AVAudioSessionPortBluetoothLE")]
		NSString PortBluetoothLE { get; }

		[iOS (7,1)]
		[Field ("AVAudioSessionPortCarAudio")]
		NSString PortCarAudio { get; }

		[iOS (7,0)]
		[UnifiedInternal, Field ("AVAudioSessionLocationUpper")]
		NSString LocationUpper { get; }

		[iOS (7,0)]
		[UnifiedInternal, Field ("AVAudioSessionLocationLower")]
		NSString LocationLower { get; }
		
		[iOS (6,0)]
		[Export ("inputDataSources"), NullAllowed]
		AVAudioSessionDataSourceDescription [] InputDataSources { get;  }

		[iOS (6,0)]
		[Export ("inputDataSource")]
		AVAudioSessionDataSourceDescription InputDataSource { get;  }

		[iOS (6,0)]
		[Export ("outputDataSources"), NullAllowed]
		AVAudioSessionDataSourceDescription [] OutputDataSources { get;  }

		[iOS (6,0)]
		[Export ("outputDataSource")]
		AVAudioSessionDataSourceDescription OutputDataSource { get;  }
		
		[NoWatch]
		[iOS (6,0)]
		[Export ("setInputDataSource:error:")]
		[PostGet ("InputDataSource")]
		bool SetInputDataSource ([NullAllowed] AVAudioSessionDataSourceDescription dataSource, out NSError outError);

		[NoWatch]
		[iOS (6,0)]
		[Export ("setOutputDataSource:error:")]
		[PostGet ("OutputDataSource")]
		bool SetOutputDataSource ([NullAllowed] AVAudioSessionDataSourceDescription dataSource, out NSError outError);

		[NoTV]
		[Watch (4,0)]
		[iOS (7,0)]
		[Export ("requestRecordPermission:")]
		void RequestRecordPermission (AVPermissionGranted responseCallback);

		[NoWatch, iOS (7,0)]
		[Export ("setPreferredInput:error:")]
		bool SetPreferredInput ([NullAllowed] AVAudioSessionPortDescription inPort, out NSError outError);

		[NoWatch, iOS (7,0)]
		[Export ("preferredInput", ArgumentSemantic.Copy), NullAllowed]
		AVAudioSessionPortDescription PreferredInput { get; }

		[iOS (7,0)]
		[Export ("availableInputs")]
		AVAudioSessionPortDescription [] AvailableInputs { get; }

		[NoWatch]
		[iOS (7,0)]
		[Export ("setPreferredInputNumberOfChannels:error:")]
		bool SetPreferredInputNumberOfChannels (nint count, out NSError outError);
	
		[NoWatch]
		[iOS (7,0)]
		[Export ("preferredInputNumberOfChannels")]
		nint GetPreferredInputNumberOfChannels ();
	
		[NoWatch]
		[iOS (7,0)]
		[Export ("setPreferredOutputNumberOfChannels:error:")]
		bool SetPreferredOutputNumberOfChannels (nint count, out NSError outError);
	
		[NoWatch]
		[iOS (7,0)]
		[Export ("preferredOutputNumberOfChannels")]
		nint GetPreferredOutputNumberOfChannels ();
	
		[iOS (7,0)]
		[Export ("maximumInputNumberOfChannels")]
		nint MaximumInputNumberOfChannels { get; }
	
		[iOS (7,0)]
		[Export ("maximumOutputNumberOfChannels")]
		nint MaximumOutputNumberOfChannels { get; }

		[iOS (7,0)]
		[UnifiedInternal, Field ("AVAudioSessionOrientationTop")]
		NSString OrientationTop { get; }
	
		[iOS (7,0)]
		[UnifiedInternal, Field ("AVAudioSessionOrientationBottom")]
		NSString OrientationBottom { get; }
	
		[iOS (7,0)]
		[UnifiedInternal, Field ("AVAudioSessionOrientationFront")]
		NSString OrientationFront { get; }
	
		[iOS (7,0)]
		[UnifiedInternal, Field ("AVAudioSessionOrientationBack")]
		NSString OrientationBack { get; }

		[iOS (8,0)]
		[Field ("AVAudioSessionOrientationLeft")]
		NSString OrientationLeft { get; }

		[iOS (8,0)]
		[Field ("AVAudioSessionOrientationRight")]
		NSString OrientationRight { get; }
	
		[iOS (7,0)]
		[UnifiedInternal, Field ("AVAudioSessionPolarPatternOmnidirectional")]
		NSString PolarPatternOmnidirectional { get; }
	
		[iOS (7,0)]
		[UnifiedInternal, Field ("AVAudioSessionPolarPatternCardioid")]
		NSString PolarPatternCardioid { get; }
	
		[iOS (7,0)]
		[UnifiedInternal, Field ("AVAudioSessionPolarPatternSubcardioid")]
		NSString PolarPatternSubcardioid { get; }

		// 8.0
		[NoTV]
		[iOS (8,0), Watch (5,0)]
		[Export ("recordPermission")]
		AVAudioSessionRecordPermission RecordPermission { get; }

		[iOS (8,0)]
		[Export ("secondaryAudioShouldBeSilencedHint")]
		bool SecondaryAudioShouldBeSilencedHint { get; }

		[iOS (8,0)]
		[Field ("AVAudioSessionSilenceSecondaryAudioHintNotification")]
		[Notification (typeof (AVAudioSessionSecondaryAudioHintEventArgs))]
		NSString SilenceSecondaryAudioHintNotification { get; }
		
		[NoWatch, NoTV, iOS(10,0)]
		[Export ("setAggregatedIOPreference:error:")]
		bool SetAggregatedIOPreference (AVAudioSessionIOType ioType, out NSError error);

		[TV (11,0), Watch (5,0), iOS (11,0), NoMac]
		[Export ("setCategory:mode:routeSharingPolicy:options:error:")]
		bool SetCategory (string category, string mode, AVAudioSessionRouteSharingPolicy policy, AVAudioSessionCategoryOptions options, [NullAllowed] out NSError outError);

		[TV (11, 0), Watch (5,0), iOS (11, 0), NoMac]
		[Export ("routeSharingPolicy")]
		AVAudioSessionRouteSharingPolicy RouteSharingPolicy { get; }

		[Async]
		[Watch (5,0), NoTV, NoMac, NoiOS]
		[Export ("activateWithOptions:completionHandler:")]
		void Activate (AVAudioSessionActivationOptions options, Action<bool, NSError> handler);
	}
	
	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVAudioSessionDataSourceDescription {
		[Export ("dataSourceID")]
		NSNumber DataSourceID { get;  }

		[Export ("dataSourceName")]
		string DataSourceName { get;  }

#if XAMCORE_2_0
		[iOS (7,0)]
		[Export ("location", ArgumentSemantic.Copy)]
		[Internal]
		NSString Location_ { get; }
	
		[iOS (7,0)]
		[Export ("orientation", ArgumentSemantic.Copy)]
		[Internal]
		NSString Orientation_ { get; }
#else
		[iOS (7,0)]
		[Export ("location", ArgumentSemantic.Copy), NullAllowed]
		string Location { get; }
	
		[iOS (7,0)]
		[Export ("orientation", ArgumentSemantic.Copy), NullAllowed]
		string Orientation { get; }
#endif

		[NoWatch]
		[iOS (7,0)]
		[UnifiedInternal, Export ("supportedPolarPatterns")]
		NSString [] SupportedPolarPatterns { get; }
	
		[NoWatch]
		[iOS (7,0)]
		[UnifiedInternal, Export ("selectedPolarPattern", ArgumentSemantic.Copy)]
		NSString SelectedPolarPattern { get; }
	
		[NoWatch]
		[iOS (7,0)]
		[UnifiedInternal, Export ("preferredPolarPattern", ArgumentSemantic.Copy)]
		NSString PreferredPolarPattern { get; }
	
		[NoWatch]
		[iOS (7,0)]
		[UnifiedInternal, Export ("setPreferredPolarPattern:error:")]
		bool SetPreferredPolarPattern ([NullAllowed] NSString pattern, out NSError outError);
		
	}

	interface AVAudioSessionInterruptionEventArgs {
		[Export ("AVAudioSessionInterruptionTypeKey")]
		AVAudioSessionInterruptionType InterruptionType { get; }

		[Export ("AVAudioSessionInterruptionOptionKey")]
		AVAudioSessionInterruptionOptions Option { get; }

		[iOS (10, 3), NoMac, TV (10, 2), Watch (3,2)]
		[NullAllowed]
		[Export ("AVAudioSessionInterruptionWasSuspendedKey")]
		bool WasSuspended { get; }
	}

	interface AVAudioSessionRouteChangeEventArgs {
		[Export ("AVAudioSessionRouteChangeReasonKey")]
		AVAudioSessionRouteChangeReason Reason { get; }
		
		[Export ("AVAudioSessionRouteChangePreviousRouteKey")]
		AVAudioSessionRouteDescription PreviousRoute { get; }
	}
	
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[NoTV][NoWatch]
	interface AVAudioSessionDelegate {
		[Export ("beginInterruption")]
		void BeginInterruption ();
	
		[Export ("endInterruption")]
		void EndInterruption ();

		[Export ("inputIsAvailableChanged:")]
		void InputIsAvailableChanged (bool isInputAvailable);
	
		[Export ("endInterruptionWithFlags:")]
		void EndInterruption (AVAudioSessionInterruptionFlags flags);
	}

	[Watch (3,0)]
	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVAudioSessionChannelDescription {
		[Export ("channelName")]
		string ChannelName { get;  }

		[Export ("owningPortUID")]
		string OwningPortUID { get;  }

		[Export ("channelNumber")]
		nint ChannelNumber { get;  }

		[iOS (7,0)]
		[Export ("channelLabel")]
		int /* AudioChannelLabel = UInt32 */ ChannelLabel { get; }
	}

	[Watch (3,0)]
	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVAudioSessionPortDescription {
		[Export ("portType")]
		NSString PortType { get;  }

		[Export ("portName")]
		string PortName { get;  }

		[Export ("UID")]
		string UID { get;  }

		[iOS (10, 0), TV (10,0), Watch (3,0)]
		[Export ("hasHardwareVoiceCallProcessing")]
		bool HasHardwareVoiceCallProcessing { get; }

		[Export ("channels"), NullAllowed]
		AVAudioSessionChannelDescription [] Channels { get;  }

		[iOS (7,0)]
		[Export ("dataSources"), NullAllowed]
#if XAMCORE_4_0
		AVAudioSessionDataSourceDescription [] DataSources { get; }
#else
		AVAudioSessionDataSourceDescription [] DataSourceDescriptions { get; }
#endif

		[NoWatch]
		[iOS (7,0)]
		[Export ("selectedDataSource", ArgumentSemantic.Copy), NullAllowed]
		AVAudioSessionDataSourceDescription SelectedDataSource { get; }

		[NoWatch]
		[iOS (7,0)]
		[Export ("preferredDataSource", ArgumentSemantic.Copy), NullAllowed]
		AVAudioSessionDataSourceDescription PreferredDataSource { get; }

		[NoWatch]
		[iOS (7,0)]
		[Export ("setPreferredDataSource:error:")]
		bool SetPreferredDataSource ([NullAllowed] AVAudioSessionDataSourceDescription dataSource, out NSError outError);
		
	}

	[Watch (3,0)]
	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVAudioSessionRouteDescription {
		[Export ("inputs")]
		AVAudioSessionPortDescription [] Inputs { get;  }

		[Export ("outputs")]
		AVAudioSessionPortDescription [] Outputs { get;  }

	}
#endif

	[NoWatch, iOS (8,0)]
	[BaseType (typeof (AVAudioNode))]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioUnit {
		[Export ("audioComponentDescription"), Internal]
#if XAMCORE_2_0
		AudioComponentDescription AudioComponentDescription { get; }
#else
		AudioComponentDescriptionNative _AudioComponentDescription { get; }
#endif

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

#if XAMCORE_2_0
		[iOS (9,0), Mac (10,11)]
		[Static]
		[Export ("instantiateWithComponentDescription:options:completionHandler:")]
		[Async]
		void FromComponentDescription (AudioComponentDescription audioComponentDescription, AudioComponentInstantiationOptions options, Action<AVAudioUnit, NSError> completionHandler);

		[NoWatch, iOS (9,0), Mac (10,11, onlyOn64 : true)]
		[Export ("AUAudioUnit")]
		AUAudioUnit AUAudioUnit { get; }
#endif
	}

	[NoWatch, iOS (8,0)]
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
	[iOS (8,0)]
	[BaseType (typeof (AVAudioUnitEffect))]
	interface AVAudioUnitDistortion {
		[Export ("preGain")]
		float PreGain { get; set; } /* float, not CGFloat */

		[Export ("wetDryMix")]
		float WetDryMix { get; set; } /* float, not CGFloat */

		[Export ("loadFactoryPreset:")]
		void LoadFactoryPreset (AVAudioUnitDistortionPreset preset);
	}

	[NoWatch, iOS (8,0)]
	[BaseType (typeof (AVAudioUnit))]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioUnitEffect {
		[Export ("initWithAudioComponentDescription:")]
#if XAMCORE_2_0
		IntPtr Constructor (AudioComponentDescription audioComponentDescription);
#else
		[Internal]
		IntPtr Constructor (AudioComponentDescriptionNative audioComponentDescription);
#endif

		[Export ("bypass")]
		bool Bypass { get; set; }
	}

	[NoWatch]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (AVAudioUnitEffect))]
	interface AVAudioUnitEQ {
		[Export ("initWithNumberOfBands:")]
		IntPtr Constructor (nuint numberOfBands);

		[Export ("bands")]
		AVAudioUnitEQFilterParameters [] Bands { get; }

		[Export ("globalGain")]
		float GlobalGain { get; set; } /* float, not CGFloat */
	}

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
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

	[NoWatch, iOS (8,0)]
	[BaseType (typeof (AVAudioUnit))]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioUnitGenerator : AVAudioMixing {
		[Export ("initWithAudioComponentDescription:")]
#if XAMCORE_2_0
		IntPtr Constructor (AudioComponentDescription audioComponentDescription);
#else
		[Internal]
		IntPtr Constructor (AudioComponentDescriptionNative audioComponentDescription);
#endif

		[Export ("bypass")]
		bool Bypass { get; set; }
	}

	[NoWatch, iOS (8,0)]
	[BaseType (typeof (AVAudioUnit), Name="AVAudioUnitMIDIInstrument")]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioUnitMidiInstrument : AVAudioMixing { 
		[Export ("initWithAudioComponentDescription:")]
#if XAMCORE_2_0
		IntPtr Constructor (AudioComponentDescription audioComponentDescription);
#else
		[Internal]
		IntPtr Constructor (AudioComponentDescriptionNative audioComponentDescription);
#endif

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

	[NoWatch, iOS (8,0)]
	[BaseType (typeof (AVAudioUnitMidiInstrument))]
	interface AVAudioUnitSampler {
		[Export ("stereoPan")]
		float StereoPan { get; set; } /* float, not CGFloat */

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
	}

	[NoWatch, iOS (8,0)]
	[BaseType (typeof (AVAudioUnitEffect))]
	interface AVAudioUnitReverb {

		[Export ("wetDryMix")]
		float WetDryMix { get; set; } /* float, not CGFloat */

		[Export ("loadFactoryPreset:")]
		void LoadFactoryPreset (AVAudioUnitReverbPreset preset);
	}
	

	[NoWatch, iOS (8,0)]
	[BaseType (typeof (AVAudioUnit))]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioUnitTimeEffect {
		[Export ("initWithAudioComponentDescription:")]
#if XAMCORE_2_0
		IntPtr Constructor (AudioComponentDescription audioComponentDescription);
#else
		[Internal]
		IntPtr Constructor (AudioComponentDescriptionNative audioComponentDescription);
#endif

		[Export ("bypass")]
		bool Bypass { get; set; }
	}
	
	[NoWatch, iOS (8,0)]
	[BaseType (typeof (AVAudioUnitTimeEffect))]
	interface AVAudioUnitTimePitch {
		[Export ("initWithAudioComponentDescription:")]
#if XAMCORE_2_0
		IntPtr Constructor (AudioComponentDescription audioComponentDescription);
#else
		[Internal]
		IntPtr Constructor (AudioComponentDescriptionNative audioComponentDescription);
#endif


		[Export ("rate")]
		float Rate { get; set; } /* float, not CGFloat */

		[Export ("pitch")]
		float Pitch { get; set; } /* float, not CGFloat */

		[Export ("overlap")]
		float Overlap { get; set; } /* float, not CGFloat */
	}

	[NoWatch, iOS (8,0)]
	[BaseType (typeof (AVAudioUnitTimeEffect))]
	interface AVAudioUnitVarispeed {
		[Export ("initWithAudioComponentDescription:")]
#if XAMCORE_2_0
		IntPtr Constructor (AudioComponentDescription audioComponentDescription);
#else
		[Internal]
		IntPtr Constructor (AudioComponentDescriptionNative audioComponentDescription);
#endif

		[Export ("rate")]
		float Rate { get; set; } /* float, not CGFloat */
	}

	[Watch (3,0)]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (NSObject))]
	interface AVAudioTime {
		[Export ("initWithAudioTimeStamp:sampleRate:")]
		IntPtr Constructor (ref AudioTimeStamp timestamp, double sampleRate);

		[Export ("initWithHostTime:")]
		IntPtr Constructor (ulong hostTime);

		[Export ("initWithSampleTime:atRate:")]
		IntPtr Constructor (long sampleTime, double sampleRate);

		[Export ("initWithHostTime:sampleTime:atRate:")]
		IntPtr Constructor (ulong hostTime, long sampleTime, double sampleRate);

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
		AVAudioTime ExtrapolateTimeFromAnchor (AVAudioTime anchorTime);
	}

	[Watch (3,0)]
	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Docs/headers do not state that init is disallowed but if 
	// you get an instance that way and try to use it, it will inmediatelly crash also tested in ObjC app same result
	interface AVAudioConverter {

		[Export ("initFromFormat:toFormat:")]
		IntPtr Constructor (AVAudioFormat fromFormat, AVAudioFormat toFormat);

		[Export ("reset")]
		void Reset ();

		[Export ("inputFormat")]
		AVAudioFormat InputFormat { get; }

		[Export ("outputFormat")]
		AVAudioFormat OutputFormat { get; }

		[Export ("channelMap", ArgumentSemantic.Retain)]
		NSNumber[] ChannelMap { get; set; }

		[NullAllowed, Export ("magicCookie", ArgumentSemantic.Retain)]
		NSData MagicCookie { get; set; }

		[Export ("downmix")]
		bool Downmix { get; set; }

		[Export ("dither")]
		bool Dither { get; set; }

		[Export ("sampleRateConverterQuality", ArgumentSemantic.Assign)]
		nint SampleRateConverterQuality { get; set; }

		[Export ("sampleRateConverterAlgorithm", ArgumentSemantic.Retain)]
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
		NSNumber[] AvailableEncodeBitRates { get; }

		[NullAllowed, Export ("applicableEncodeBitRates")]
		NSNumber[] ApplicableEncodeBitRates { get; }

		[NullAllowed, Export ("availableEncodeSampleRates")]
		NSNumber[] AvailableEncodeSampleRates { get; }

		[NullAllowed, Export ("applicableEncodeSampleRates")]
		NSNumber[] ApplicableEncodeSampleRates { get; }

		[NullAllowed, Export ("availableEncodeChannelLayoutTags")]
		NSNumber[] AvailableEncodeChannelLayoutTags { get; }
	}

	[TV (11,2), NoWatch, NoMac, NoiOS]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface AVDisplayCriteria : NSCopying {
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** initialization method -init cannot be sent to an abstract object of class AVAsset: Create a concrete instance!
	[DisableDefaultCtor]
	interface AVAsset : NSCopying {
		[Export ("duration")]
		CMTime Duration { get;  }

		[Export ("preferredRate")]
		float PreferredRate { get;  } // defined as 'float'

		[Export ("preferredVolume")]
		float PreferredVolume { get;  } // defined as 'float'

		[Export ("preferredTransform")]
		CGAffineTransform PreferredTransform { get;  }

		[Export ("naturalSize")]
		[Deprecated (PlatformName.iOS, 5, 0, message : "Use 'NaturalSize/PreferredTransform' as appropriate on the video track instead.")]
		[Mac (10, 7)]
		[Deprecated (PlatformName.MacOSX, 10, 8, message : "Use 'NaturalSize/PreferredTransform' as appropriate on the video track instead.")]
		CGSize NaturalSize { get;  }

		[TV (11,2), NoWatch, NoMac, NoiOS]
		[Export ("preferredDisplayCriteria")]
		AVDisplayCriteria PreferredDisplayCriteria { get; }

		[Export ("providesPreciseDurationAndTiming")]
		bool ProvidesPreciseDurationAndTiming { get;  }

		[Export ("cancelLoading")]
		void CancelLoading ();

		[Export ("tracks")]
		AVAssetTrack [] Tracks { get;  }

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
		string Lyrics { get;  }

		[Export ("commonMetadata")]
		AVMetadataItem [] CommonMetadata { get;  }

		[Export ("availableMetadataFormats")]
		string [] AvailableMetadataFormats { get;  }

#if !XAMCORE_4_0
		[Obsolete ("Use 'GetMetadataForFormat' with enum values AVMetadataFormat.")]
		[Wrap ("GetMetadataForFormat (new NSString (format))", IsVirtual = true)]
		AVMetadataItem [] MetadataForFormat (string format);
#endif
		[Export ("metadataForFormat:")]
		AVMetadataItem [] GetMetadataForFormat (NSString format);

		[Wrap ("GetMetadataForFormat (new NSString (format.GetConstant ()))")]
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

		[Mac (10,8)]
		[Export ("availableMediaCharacteristicsWithMediaSelectionOptions")]
		string [] AvailableMediaCharacteristicsWithMediaSelectionOptions { get; }

#if !MONOMAC
		[Export ("compatibleWithSavedPhotosAlbum")]
		bool CompatibleWithSavedPhotosAlbum  { [Bind ("isCompatibleWithSavedPhotosAlbum")] get; }
#endif

		[Mac (10,8)]
		[Export ("creationDate"), NullAllowed]
		AVMetadataItem CreationDate { get; }

		[Export ("referenceRestrictions")]
		AVAssetReferenceRestrictions ReferenceRestrictions { get; }

		[Mac (10,8)]
		[return: NullAllowed]
		[Export ("mediaSelectionGroupForMediaCharacteristic:")]
		AVMediaSelectionGroup MediaSelectionGroupForMediaCharacteristic (string avMediaCharacteristic);

		[Wrap ("MediaSelectionGroupForMediaCharacteristic (avMediaCharacteristic.GetConstant ())")]
		AVMediaSelectionGroup GetMediaSelectionGroupForMediaCharacteristic (AVMediaCharacteristics avMediaCharacteristic);

		[Export ("statusOfValueForKey:error:")]
		AVKeyValueStatus StatusOfValue (string key, out NSError error);

		[Export ("loadValuesAsynchronouslyForKeys:completionHandler:")]
		[Async ("LoadValuesTaskAsync")]
		void LoadValuesAsynchronously (string [] keys, Action handler);

		[iOS (6, 0)]
		[Mac (10,8)]
		[Export ("chapterMetadataGroupsBestMatchingPreferredLanguages:")]
		AVTimedMetadataGroup [] GetChapterMetadataGroupsBestMatchingPreferredLanguages (string [] languages);

		[iOS (7,0)]
		[Mac (10,9)]
		[Export ("trackGroups", ArgumentSemantic.Copy)]
		AVAssetTrackGroup [] TrackGroups { get; }

		[iOS (8,0), Mac (10,10)]
		[Export ("metadata")]
		AVMetadataItem [] Metadata { get; }

		[Export ("unusedTrackID")]
		int /* CMPersistentTrackID -> int32_t */ UnusedTrackId { get; }  // TODO: wrong name, should have benn UnusedTrackID

		[iOS (9,0), Mac(10,11)]
		[Export ("preferredMediaSelection")]
		AVMediaSelection PreferredMediaSelection { get; }

		// AVAsset (AVAssetFragments) Category
		// This is being inlined because there are no property extensions

		[iOS (9,0), Mac (10,11)]
		[Export ("canContainFragments")]
		bool CanContainFragments { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("containsFragments")]
		bool ContainsFragments { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("compatibleWithAirPlayVideo")]
		bool CompatibleWithAirPlayVideo { [Bind ("isCompatibleWithAirPlayVideo")] get; }

		[iOS (9,0), Mac (10,11)]
		[Field ("AVAssetDurationDidChangeNotification")]
		[Notification]
		NSString DurationDidChangeNotification { get; }

		[iOS (9,0), Mac (10,11)]
		[Field ("AVAssetChapterMetadataGroupsDidChangeNotification")]
		[Notification]
		NSString ChapterMetadataGroupsDidChangeNotification { get; }

		[iOS(9,0), Mac(10,11)]
		[Notification, Field ("AVAssetMediaSelectionGroupsDidChangeNotification")]
		NSString MediaSelectionGroupsDidChangeNotification { get; }

		[Mac (10,11)]
		[TV (12, 0), NoWatch, iOS (12, 0)]
		[Field ("AVAssetContainsFragmentsDidChangeNotification")]
		[Notification]
		NSString ContainsFragmentsDidChangeNotification { get; }

		[Mac (10,11)]
		[TV (12, 0), NoWatch, iOS (12, 0)]
		[Field ("AVAssetWasDefragmentedNotification")]
		[Notification]
		NSString WasDefragmentedNotification { get; }

		[iOS (10, 2), Mac (10,12,2), TV (10, 2)]
		[Export ("overallDurationHint")]
		CMTime OverallDurationHint { get; }

		[iOS (11, 0), TV (11, 0), Mac (10, 13), NoWatch]
		[Export ("allMediaSelections")]
		AVMediaSelection[] AllMediaSelections { get; }
	}

#if !WATCH

	interface IAVFragmentMinding {}

	[Protocol]
	[Mac (10,11)]
	[iOS (12, 0), TV (12,0)]
	interface AVFragmentMinding {

#if !MONOMAC || XAMCORE_4_0
		[Abstract] // not kept in Mac OS because is a breaking change, in other paltforms we are ok
#endif
		[Export ("isAssociatedWithFragmentMinder")]
		bool IsAssociatedWithFragmentMinder ();
	}

	[Mac (10,11)]
	[iOS (12, 0), TV (12,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (AVUrlAsset))]
	interface AVFragmentedAsset : AVFragmentMinding {

		[Export ("initWithURL:options:")]
		IntPtr Constructor (NSUrl url, [NullAllowed] NSDictionary options);

		[Static]
		[Export ("fragmentedAssetWithURL:options:")]
		AVFragmentedAsset FromUrl (NSUrl url, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("tracks")]
		AVFragmentedAssetTrack [] Tracks { get; }
	}

	[Mac (10,11)]
	[iOS (12, 0), TV (12,0)]
	[Category]
	[BaseType (typeof(AVFragmentedAsset))]
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

	}

	[Mac (10,11)]
	[iOS (12,0), TV (12,0)]
	[BaseType (typeof(NSObject))]
	interface AVFragmentedAssetMinder {

		[Static]
		[Export ("fragmentedAssetMinderWithAsset:mindingInterval:")]
		AVFragmentedAssetMinder FromAsset (AVAsset asset, double mindingInterval);

		[Mac (10,14, onlyOn64: true)]
		[Export ("initWithAsset:mindingInterval:")]
		IntPtr Constructor (IAVFragmentMinding asset, double mindingInterval);

		[Export ("mindingInterval")]
		double MindingInterval { get; set; }

		[Export ("assets")]
		AVAsset [] Assets { get; }

		[Export ("addFragmentedAsset:")]
		void AddFragmentedAsset (AVAsset asset);

		[Export ("removeFragmentedAsset:")]
		void RemoveFragmentedAsset (AVAsset asset);
	}

	[Mac (10,11)]
	[iOS (12,0), TV (12,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (AVAssetTrack))]
	interface AVFragmentedAssetTrack {
	}

#endif

#if MONOMAC

	interface IAVCaptureFileOutputDelegate {}

	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVCaptureFileOutputDelegate {
		[Abstract]
		[Export ("captureOutputShouldProvideSampleAccurateRecordingStart:")]
		bool ShouldProvideSampleAccurateRecordingStart (AVCaptureOutput captureOutput);

		[Export ("captureOutput:didOutputSampleBuffer:fromConnection:")]
		void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection);
	}

#endif // MONOMAC

#if XAMCORE_4_0
	[Abstract] // Abstract superclass.
#endif
	[NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureSynchronizedData
	{
		[Export ("timestamp")]
		CMTime Timestamp { get; }
	}

	[NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureSynchronizedDataCollection
	{
#if !XAMCORE_4_0
		[Obsolete ("Use 'GetSynchronizedData' instead.")]
		[Wrap ("GetSynchronizedData (captureOutput)", isVirtual: true)]
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

	interface IAVCaptureDataOutputSynchronizerDelegate {}
	
	[NoWatch, NoTV, iOS (11,0)]
	[NoMac]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface AVCaptureDataOutputSynchronizerDelegate
	{
		[Abstract]
		[Export ("dataOutputSynchronizer:didOutputSynchronizedDataCollection:")]
		void DidOutputSynchronizedDataCollection (AVCaptureDataOutputSynchronizer synchronizer, AVCaptureSynchronizedDataCollection synchronizedDataCollection);
	}

	[NoWatch, NoTV, iOS (11,0)]
	[NoMac]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureDataOutputSynchronizer
	{
		[Export ("initWithDataOutputs:")]
		IntPtr Constructor (AVCaptureOutput[] dataOutputs);

		[Export ("dataOutputs", ArgumentSemantic.Retain)]
		AVCaptureOutput[] DataOutputs { get; }

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

	[NoMac, NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(AVCaptureSynchronizedData))]
	interface AVCaptureSynchronizedSampleBufferData
	{
		[Export ("sampleBuffer")]
		CMSampleBuffer SampleBuffer { get; }

		[Export ("sampleBufferWasDropped")]
		bool SampleBufferWasDropped { get; }

		[Export ("droppedReason")]
		AVCaptureOutputDataDroppedReason DroppedReason { get; }
	}

	[NoMac, NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(AVCaptureSynchronizedData))]
	interface AVCaptureSynchronizedMetadataObjectData
	{
		[Export ("metadataObjects")]
		AVMetadataObject[] MetadataObjects { get; }
	}

	[NoMac, NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(AVCaptureSynchronizedData))]
	interface AVCaptureSynchronizedDepthData
	{
		[Export ("depthData")]
		AVDepthData DepthData { get; }

		[Export ("depthDataWasDropped")]
		bool DepthDataWasDropped { get; }

		[Export ("droppedReason")]
		AVCaptureOutputDataDroppedReason DroppedReason { get; }
	}

	[TV (11,0), NoWatch, Mac (10,13), iOS (11,0)]
	[Protocol]
	interface AVQueuedSampleBufferRendering
	{
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
	}

	[TV (11,0), NoWatch, Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface AVSampleBufferAudioRenderer : AVQueuedSampleBufferRendering
	{
		[Export ("status")]
		AVQueuedSampleBufferRenderingStatus Status { get; }

		[NullAllowed, Export ("error")]
		NSError Error { get; }

		[NullAllowed, Export ("audioOutputDeviceUniqueID"), NoWatch, NoTV, NoiOS]
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

		[TV (11, 0), NoWatch, Mac (10, 13), iOS (11, 0)]
		[Notification (typeof (AudioRendererWasFlushedAutomaticallyEventArgs))]
		[Field ("AVSampleBufferAudioRendererWasFlushedAutomaticallyNotification")]
		NSString AudioRendererWasFlushedAutomaticallyNotification { get; }

	}

	[TV (11, 0), NoWatch, Mac (10, 13), iOS (11, 0)]
	interface AudioRendererWasFlushedAutomaticallyEventArgs {
		[Internal]
		[Export ("AVSampleBufferAudioRendererFlushTimeKey")]
		NSValue _AudioRendererFlushTime { get; set; }
	}

	interface IAVQueuedSampleBufferRendering {}

	[TV (11,0), NoWatch, Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface AVSampleBufferRenderSynchronizer
	{
		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Field ("AVSampleBufferRenderSynchronizerRateDidChangeNotification")]
		[Notification]
		NSString RateDidChangeNotification { get; }

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Export ("currentTime")]
		CMTime CurrentTime { get; }

		[Export ("timebase", ArgumentSemantic.Retain)]
		CMTimebase Timebase { get; }

		[Export ("rate")]
		float Rate { get; set; }

		[Export ("setRate:time:")]
		void SetRate (float rate, CMTime time);

		// AVSampleBufferRenderSynchronizer_AVSampleBufferRenderSynchronizerRendererManagement

		[Export ("renderers")]
		IAVQueuedSampleBufferRendering[] Renderers { get; }

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
		NSObject AddBoundaryTimeObserver (NSValue[] times, [NullAllowed] DispatchQueue queue, Action handler);

		[Export ("removeTimeObserver:")]
		void RemoveTimeObserver (NSObject observer);
	}


#if MONOMAC
	[Mac (10,10)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVSampleBufferGenerator {

		[Export ("initWithAsset:timebase:")]
		[DesignatedInitializer]
		IntPtr Constructor (AVAsset asset, [NullAllowed] CMTimebase timebase);

		[Export ("createSampleBufferForRequest:")]
		[return: Release]
		CMSampleBuffer CreateSampleBuffer (AVSampleBufferRequest request);

		[Static]
		[Async]
		[Export ("notifyOfDataReadyForSampleBuffer:completionHandler:")]
		void NotifyOfDataReady (CMSampleBuffer sbuf, Action<bool, NSError> completionHandler);
	}

	[Mac (10,10)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVSampleBufferRequest {

		[Export ("initWithStartCursor:")]
		[DesignatedInitializer]
		IntPtr Constructor (AVSampleCursor startCursor);

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

#endif

	[NoWatch]
	[BaseType (typeof (NSObject))]
	// <quote>You create an asset generator using initWithAsset: or assetImageGeneratorWithAsset:</quote> http://developer.apple.com/library/ios/#documentation/AVFoundation/Reference/AVAssetImageGenerator_Class/Reference/Reference.html
	// calling 'init' returns a NIL handle
	[DisableDefaultCtor]
	interface AVAssetImageGenerator {
		[Export ("maximumSize", ArgumentSemantic.Assign)]
		CGSize MaximumSize { get; set;  }

		[Export ("apertureMode", ArgumentSemantic.Copy), NullAllowed]
		NSString ApertureMode { get; set;  }

		[Export ("videoComposition", ArgumentSemantic.Copy), NullAllowed]
		AVVideoComposition VideoComposition { get; set;  }

		[Export ("appliesPreferredTrackTransform")]
		bool AppliesPreferredTrackTransform { get; set; }

		[Static]
		[Export ("assetImageGeneratorWithAsset:")]
		AVAssetImageGenerator FromAsset (AVAsset asset);

		[DesignatedInitializer]
		[Export ("initWithAsset:")]
		IntPtr Constructor (AVAsset asset);

		[Export ("copyCGImageAtTime:actualTime:error:")]
		[return: Release ()]
		CGImage CopyCGImageAtTime (CMTime requestedTime, out CMTime actualTime, out NSError outError);

		[Export ("generateCGImagesAsynchronouslyForTimes:completionHandler:")]
		void GenerateCGImagesAsynchronously (NSValue[] cmTimesRequestedTimes, AVAssetImageGeneratorCompletionHandler handler);

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
		CMTime RequestedTimeToleranceBefore { get; set;  }

		[Export ("requestedTimeToleranceAfter", ArgumentSemantic.Assign)]
		CMTime RequestedTimeToleranceAfter { get; set;  }

		[iOS (6,0)]
		[Mac (10,9)]
		[Export ("asset")]
		AVAsset Asset { get; }

		[iOS (7,0)]
		[Mac (10,9)]
		[Export ("customVideoCompositor", ArgumentSemantic.Copy), NullAllowed]
		[Protocolize]
		AVVideoCompositing CustomVideoCompositor { get; }
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetReader initWithAsset:error:] invalid parameter not satisfying: asset != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetReader {
		[Export ("asset", ArgumentSemantic.Retain)]
		AVAsset Asset { get;  }

		[Export ("status")]
		AVAssetReaderStatus Status { get;  }

		[Export ("error"), NullAllowed]
		NSError Error { get;  }

		[Export ("timeRange")]
		CMTimeRange TimeRange { get; set;  }

		[Export ("outputs")]
		AVAssetReaderOutput [] Outputs { get;  }

		[return: NullAllowed]
		[Static, Export ("assetReaderWithAsset:error:")]
		AVAssetReader FromAsset (AVAsset asset, out NSError error);

		[DesignatedInitializer]
		[Export ("initWithAsset:error:")]
		IntPtr Constructor (AVAsset asset, out NSError error);

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
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** initialization method -init cannot be sent to an abstract object of class AVAssetReaderOutput: Create a concrete instance!
	[DisableDefaultCtor]
	interface AVAssetReaderOutput {
		[Export ("mediaType")]
		string MediaType { get; }

		[return: NullAllowed]
		[Export ("copyNextSampleBuffer")]
		CMSampleBuffer CopyNextSampleBuffer ();

		[Export ("alwaysCopiesSampleData")]
		bool AlwaysCopiesSampleData { get; set; }

		[iOS (8,0), Mac (10, 10)]
		[Export ("supportsRandomAccess")]
		bool SupportsRandomAccess { get; set; }

		[iOS (8,0), Mac (10, 10)]
		[Export ("resetForReadingTimeRanges:")]
		void ResetForReadingTimeRanges (NSValue [] timeRanges);

		[iOS (8,0), Mac (10, 10)]
		[Export ("markConfigurationAsFinal")]
		void MarkConfigurationAsFinal ();
	}

	[NoWatch]
	[iOS (8,0), Mac (10, 10)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: *** -[AVAssetReaderOutputMetadataAdaptor initWithAssetReaderTrackOutput:] invalid parameter not satisfying: trackOutput != ((void*)0)
	interface AVAssetReaderOutputMetadataAdaptor {

		[DesignatedInitializer]
		[Export ("initWithAssetReaderTrackOutput:")]
		IntPtr Constructor (AVAssetReaderTrackOutput trackOutput);

		[Export ("assetReaderTrackOutput")]
		AVAssetReaderTrackOutput AssetReaderTrackOutput { get; }

		[Static, Export ("assetReaderOutputMetadataAdaptorWithAssetReaderTrackOutput:")]
		AVAssetReaderOutputMetadataAdaptor Create (AVAssetReaderTrackOutput trackOutput);

		[return: NullAllowed]
		[Export ("nextTimedMetadataGroup")]
		AVTimedMetadataGroup NextTimedMetadataGroup ();
	}

	[NoWatch]
	[iOS (8,0), Mac (10, 10)]
	[BaseType (typeof (AVAssetReaderOutput))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: *** -[AVAssetReaderSampleReferenceOutput initWithTrack:] invalid parameter not satisfying: track != ((void*)0)
	interface AVAssetReaderSampleReferenceOutput {

		[DesignatedInitializer]
		[Export ("initWithTrack:")]
		IntPtr Constructor (AVAssetTrack track);

		[Export ("track")]
		AVAssetTrack Track { get; }

		[Static, Export ("assetReaderSampleReferenceOutputWithTrack:")]
		AVAssetReaderSampleReferenceOutput Create (AVAssetTrack track);
	}

	[NoWatch]
	[BaseType (typeof (AVAssetReaderOutput))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetReaderTrackOutput initWithTrack:outputSettings:] invalid parameter not satisfying: track != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetReaderTrackOutput {
		[Export ("track")]
		AVAssetTrack Track { get;  }

#if XAMCORE_2_0
		[Internal]
#endif
		[Advice ("Use 'Create' method.")]
		[Static, Export ("assetReaderTrackOutputWithTrack:outputSettings:")]
		AVAssetReaderTrackOutput FromTrack (AVAssetTrack track, [NullAllowed] NSDictionary outputSettings);

		[Static, Wrap ("FromTrack (track, settings == null ? null : settings.Dictionary)")]
		AVAssetReaderTrackOutput Create (AVAssetTrack track, [NullAllowed] AudioSettings settings);

		[Static, Wrap ("FromTrack (track, settings == null ? null : settings.Dictionary)")]
		AVAssetReaderTrackOutput Create (AVAssetTrack track, [NullAllowed] AVVideoSettingsUncompressed settings);		

		[DesignatedInitializer]
		[Export ("initWithTrack:outputSettings:")]
		IntPtr Constructor (AVAssetTrack track, [NullAllowed] NSDictionary outputSettings);

		[Wrap ("this (track, settings == null ? null : settings.Dictionary)")]		
		IntPtr Constructor (AVAssetTrack track, [NullAllowed] AudioSettings settings);

		[Wrap ("this (track, settings == null ? null : settings.Dictionary)")]		
		IntPtr Constructor (AVAssetTrack track, [NullAllowed] AVVideoSettingsUncompressed settings);

		[Export ("outputSettings"), NullAllowed]
		NSDictionary OutputSettings { get; }

		[iOS (7,0)]
		[Mac (10,9)]
		[Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		// DOC: this is a AVAudioTimePitch value
		NSString AudioTimePitchAlgorithm { get; set; }
	}

	[NoWatch]
	[BaseType (typeof (AVAssetReaderOutput))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetReaderAudioMixOutput initWithAudioTracks:audioSettings:] invalid parameter not satisfying: audioTracks != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetReaderAudioMixOutput {
		[Export ("audioTracks")]
		AVAssetTrack [] AudioTracks { get;  }

		[Export ("audioMix", ArgumentSemantic.Copy), NullAllowed]
		AVAudioMix AudioMix { get; set;  }

#if XAMCORE_2_0
		[Internal]
#endif
		[Advice ("Use 'Create' method.")]
		[Static, Export ("assetReaderAudioMixOutputWithAudioTracks:audioSettings:")]
		AVAssetReaderAudioMixOutput FromTracks (AVAssetTrack [] audioTracks, [NullAllowed] NSDictionary audioSettings);

		[Wrap ("FromTracks (audioTracks, settings == null ? null : settings.Dictionary)")]
		AVAssetReaderAudioMixOutput Create (AVAssetTrack [] audioTracks, [NullAllowed] AudioSettings settings);

		[DesignatedInitializer]
		[Export ("initWithAudioTracks:audioSettings:")]
		IntPtr Constructor (AVAssetTrack [] audioTracks, [NullAllowed] NSDictionary audioSettings);

		[Wrap ("this (audioTracks, settings == null ? null : settings.Dictionary)")]
		IntPtr Constructor (AVAssetTrack [] audioTracks, [NullAllowed] AudioSettings settings);

#if XAMCORE_2_0
		[Internal]
#endif
		[Advice ("Use 'Settings' property.")]
		[Export ("audioSettings"), NullAllowed]
		NSDictionary AudioSettings { get; }

		[Wrap ("AudioSettings"), NullAllowed]
		AudioSettings Settings { get; }

		[iOS (7,0)]
		[Mac (10, 9)]
		[Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		// This is an AVAudioTimePitch constant
		NSString AudioTimePitchAlgorithm { get; set; }
	}

	[NoWatch]
	[BaseType (typeof (AVAssetReaderOutput))]
	// crash application if 'init' is called
	[DisableDefaultCtor]
	interface AVAssetReaderVideoCompositionOutput {
		[Export ("videoTracks")]
		AVAssetTrack [] VideoTracks { get;  }

		[Export ("videoComposition", ArgumentSemantic.Copy), NullAllowed]
		AVVideoComposition VideoComposition { get; set;  }

#if XAMCORE_2_0
		[Internal]
#endif
		[Advice ("Use 'Create' method.")]
		[Static]
		[Export ("assetReaderVideoCompositionOutputWithVideoTracks:videoSettings:")]
		AVAssetReaderVideoCompositionOutput WeakFromTracks (AVAssetTrack [] videoTracks, [NullAllowed] NSDictionary videoSettings);

		[Wrap ("WeakFromTracks (videoTracks, settings == null ? null : settings.Dictionary)")]
		[Static]
		AVAssetReaderVideoCompositionOutput Create (AVAssetTrack [] videoTracks, [NullAllowed] CVPixelBufferAttributes settings);

		[DesignatedInitializer]
		[Export ("initWithVideoTracks:videoSettings:")]
		IntPtr Constructor (AVAssetTrack [] videoTracks, [NullAllowed] NSDictionary videoSettings);

		[Wrap ("this (videoTracks, settings == null ? null : settings.Dictionary)")]
		IntPtr Constructor (AVAssetTrack [] videoTracks, [NullAllowed] CVPixelBufferAttributes settings);		

		[Export ("videoSettings"), NullAllowed]
		NSDictionary WeakVideoSettings { get; }

		[Wrap ("WeakVideoSettings"), NullAllowed]
		CVPixelBufferAttributes UncompressedVideoSettings { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("customVideoCompositor", ArgumentSemantic.Copy), NullAllowed]
		[Protocolize]
		AVVideoCompositing CustomVideoCompositor { get; }
	}

	[NoWatch]
	[iOS (6,0), Mac (10, 9)]
	[BaseType (typeof (NSObject))]
#if XAMCORE_2_0
	[DisableDefaultCtor] // no valid handle, docs now says "You do not create resource loader objects yourself."
#endif
	interface AVAssetResourceLoader {
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		[Protocolize]
		AVAssetResourceLoaderDelegate Delegate { get;  }

		[Export ("delegateQueue"), NullAllowed]
		DispatchQueue DelegateQueue { get; }

		[Export ("setDelegate:queue:")]
		void SetDelegate ([Protocolize][NullAllowed] AVAssetResourceLoaderDelegate resourceLoaderDelegate, [NullAllowed] DispatchQueue delegateQueue);

		// AVAssetResourceLoader (AVAssetResourceLoaderContentKeySupport) Category
		[iOS (9,0), Mac (10, 11)]
		[Export ("preloadsEligibleContentKeys")]
		bool PreloadsEligibleContentKeys { get; set; }
	}

	[NoWatch]
	[iOS (6,0), Mac (10, 9)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVAssetResourceLoaderDelegate {
#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("resourceLoader:shouldWaitForLoadingOfRequestedResource:")]
		bool ShouldWaitForLoadingOfRequestedResource (AVAssetResourceLoader resourceLoader, AVAssetResourceLoadingRequest loadingRequest);

		[iOS (7,0), Mac (10, 9)]
		[Export ("resourceLoader:didCancelLoadingRequest:")]
		void DidCancelLoadingRequest (AVAssetResourceLoader resourceLoader, AVAssetResourceLoadingRequest loadingRequest);

		[iOS (8,0), Mac (10, 10)]
		[Export ("resourceLoader:shouldWaitForResponseToAuthenticationChallenge:")]
		bool ShouldWaitForResponseToAuthenticationChallenge (AVAssetResourceLoader resourceLoader, NSUrlAuthenticationChallenge authenticationChallenge);

		[iOS (8,0), Mac (10, 10)]
		[Export ("resourceLoader:didCancelAuthenticationChallenge:")]
		void DidCancelAuthenticationChallenge (AVAssetResourceLoader resourceLoader, NSUrlAuthenticationChallenge authenticationChallenge);

		[iOS (8,0)]
		[Export ("resourceLoader:shouldWaitForRenewalOfRequestedResource:")]
		bool ShouldWaitForRenewalOfRequestedResource (AVAssetResourceLoader resourceLoader, AVAssetResourceRenewalRequest renewalRequest);		
	}

	[NoWatch]
	[iOS (7,0), Mac (10, 9)]
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

		[iOS (9,0), Mac (10, 11)]
		[Export ("requestsAllDataToEndOfResource")]
		bool RequestsAllDataToEndOfResource { get; }
	}
	
	[NoWatch]
	[iOS (6,0), Mac (10, 9)]
	[BaseType (typeof (NSObject))]
#if XAMCORE_2_0
	[DisableDefaultCtor] // not meant be be user created (resource loader job, see documentation)
#endif
	interface AVAssetResourceLoadingRequest {
		[Export ("request")]
		NSUrlRequest Request { get;  }

		// note: we cannot use [Bind] here as it would break compatibility with iOS 6.x
		// `isFinished` was only added in iOS 7.0 SDK and cannot be called in earlier versions
		[Export ("finished")]
		bool Finished { /* [Bind ("isFinished")] */ get;  }

		[Export ("finishLoadingWithResponse:data:redirect:")]
		[Availability (Introduced = Platform.iOS_6_0, Deprecated = Platform.iOS_7_0, Message = "Use the 'Response', 'Redirect' properties and the 'AVAssetResourceLoadingDataRequest.Responds' and 'AVAssetResourceLoadingRequest.FinishLoading' methods instead.")]
		void FinishLoading ([NullAllowed] NSUrlResponse usingResponse, [NullAllowed] NSData data, [NullAllowed] NSUrlRequest redirect);

		[Export ("finishLoadingWithError:")]
		void FinishLoadingWithError ([NullAllowed]NSError error); // TODO: Should have been FinishLoading (NSerror);

		[return: NullAllowed]
		[Export ("streamingContentKeyRequestDataForApp:contentIdentifier:options:error:")]
		NSData GetStreamingContentKey (NSData appIdentifier, NSData contentIdentifier, [NullAllowed] NSDictionary options, out NSError error);

#if !MONOMAC
		[iOS (9,0)]
		[Export ("persistentContentKeyFromKeyVendorResponse:options:error:")]
		NSData GetPersistentContentKey (NSData keyVendorResponse, [NullAllowed] NSDictionary<NSString,NSObject> options, out NSError error);
#endif

		[iOS (9,0), Mac (10, 14, onlyOn64: true)]
		[Field ("AVAssetResourceLoadingRequestStreamingContentKeyRequestRequiresPersistentKey")]
		NSString StreamingContentKeyRequestRequiresPersistentKey { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("isCancelled")]
		bool IsCancelled { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("contentInformationRequest"), NullAllowed]
		AVAssetResourceLoadingContentInformationRequest ContentInformationRequest { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("dataRequest"), NullAllowed]
		AVAssetResourceLoadingDataRequest DataRequest { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("response", ArgumentSemantic.Copy), NullAllowed]
		NSUrlResponse Response { get; set; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("redirect", ArgumentSemantic.Copy), NullAllowed]
		NSUrlRequest Redirect { get; set; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("finishLoading")]
		void FinishLoading ();
		
		[TV (12, 0), Mac (10, 14, onlyOn64: true), iOS (12, 0)]
		[Export ("requestor")]
		AVAssetResourceLoadingRequestor Requestor { get; }
	}

	[NoWatch]
	[iOS (8,0)]
	[DisableDefaultCtor] // not meant be be user created (resource loader job, see documentation) fix crash
	[BaseType (typeof (AVAssetResourceLoadingRequest))]
	interface AVAssetResourceRenewalRequest {
	}
	

	[NoWatch]
	[iOS (7,0), Mac (10, 9)]
	[BaseType (typeof (NSObject))]
#if XAMCORE_2_0
	[DisableDefaultCtor] // no valid handle, the instance is received (not created) -> see doc
#endif
	interface AVAssetResourceLoadingContentInformationRequest {
		[Export ("contentType"), NullAllowed]
		string ContentType { get; set; }

		[Export ("contentLength")]
		long ContentLength { get; set; }

		[Export ("byteRangeAccessSupported")]
		bool ByteRangeAccessSupported { [Bind ("isByteRangeAccessSupported")] get; set; }

		[iOS (8,0), Mac (10, 10)]
		[Export ("renewalDate", ArgumentSemantic.Copy), NullAllowed]
		NSDate RenewalDate { get; set; }

		[Watch (4,2), TV (11,2), Mac (10,13,2), iOS (11,2)]
		[NullAllowed, Export ("allowedContentTypes")]
		string[] AllowedContentTypes { get; }
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetWriter initWithURL:fileType:error:] invalid parameter not satisfying: outputURL != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetWriter {
		[Export ("outputURL", ArgumentSemantic.Copy)]
		NSUrl OutputURL { get;  }

		[Export ("outputFileType", ArgumentSemantic.Copy)]
		string OutputFileType { get;  }

		[Export ("status")]
		AVAssetWriterStatus Status { get;  }

		[Export ("error"), NullAllowed]
		NSError Error { get;  }

		[Export ("movieFragmentInterval", ArgumentSemantic.Assign)] 
		CMTime MovieFragmentInterval { get; set;  }

		[iOS (9,0), Mac (10,11)] // There is no availability attribute on headers but was introduced on iOS9 and Mac 10.11
		[Export ("overallDurationHint", ArgumentSemantic.Assign)]
		CMTime OverallDurationHint { get; set; }

		[Export ("shouldOptimizeForNetworkUse")]
		bool ShouldOptimizeForNetworkUse { get; set;  }

		[Export ("inputs")]
		AVAssetWriterInput [] inputs { get;  }  // TODO: Should have been Inputs

		[Export ("availableMediaTypes")]
		NSString [] AvailableMediaTypes { get; }
		
		[Export ("metadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] Metadata { get; set;  }

		[return: NullAllowed]
		[Static, Export ("assetWriterWithURL:fileType:error:")]
		AVAssetWriter FromUrl (NSUrl outputUrl, string outputFileType, out NSError error);

		[DesignatedInitializer]
		[Export ("initWithURL:fileType:error:")]
		IntPtr Constructor (NSUrl outputUrl, string outputFileType, out NSError error);

		[Export ("canApplyOutputSettings:forMediaType:")]
		bool CanApplyOutputSettings ([NullAllowed] NSDictionary outputSettings, string mediaType);

		[Wrap ("CanApplyOutputSettings (outputSettings == null ? null : outputSettings.Dictionary, mediaType)")]
		bool CanApplyOutputSettings (AudioSettings outputSettings, string mediaType);

		[Wrap ("CanApplyOutputSettings (outputSettings == null ? null : outputSettings.Dictionary, mediaType)")]
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
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use the asynchronous 'FinishWriting (NSAction completionHandler)' instead.")]
		bool FinishWriting ();

		[Mac (10,9)]
		[iOS (6,0)]
		[Export ("finishWritingWithCompletionHandler:")]
		[Async]
		void FinishWriting (Action completionHandler);

		[Export ("movieTimeScale")]
		int /* CMTimeScale = int32_t */ MovieTimeScale { get; set; }

		[Mac (10,9)]
		[iOS (7,0)]
		[Export ("canAddInputGroup:")]
		bool CanAddInputGroup (AVAssetWriterInputGroup inputGroup);

		[iOS (7,0)]
		[Mac (10,9)]
		[Export ("addInputGroup:")]
		void AddInputGroup (AVAssetWriterInputGroup inputGroup);

		[iOS (7,0), Mac (10,9)]
		[Export ("inputGroups")]
		AVAssetWriterInputGroup [] InputGroups { get; }

		[iOS (8,0), Mac (10,10)]
		[Export ("directoryForTemporaryFiles", ArgumentSemantic.Copy), NullAllowed]
		NSUrl DirectoryForTemporaryFiles { get; set; }
	}

	[NoWatch]
	[Mac (10, 7)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetWriterInput initWithMediaType:outputSettings:] invalid parameter not satisfying: mediaType != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetWriterInput {
		[iOS (6,0), Mac (10, 8)]
		[DesignatedInitializer]
		[Protected]
		[Export ("initWithMediaType:outputSettings:sourceFormatHint:")]
		IntPtr Constructor (string mediaType, [NullAllowed] NSDictionary outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[iOS (6,0), Mac (10, 8)]
		[Wrap ("this (mediaType, outputSettings == null ? null : outputSettings.Dictionary, sourceFormatHint)")]
		IntPtr Constructor (string mediaType, [NullAllowed] AudioSettings outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[iOS (6,0), Mac (10, 8)]
		[Wrap ("this (mediaType, outputSettings == null ? null : outputSettings.Dictionary, sourceFormatHint)")]
		IntPtr Constructor (string mediaType, [NullAllowed] AVVideoSettingsCompressed outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[iOS (6,0), Mac (10, 8)]
		[Static, Internal]
		[Export ("assetWriterInputWithMediaType:outputSettings:sourceFormatHint:")]
		AVAssetWriterInput Create (string mediaType, [NullAllowed] NSDictionary outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[iOS (6,0), Mac (10, 8)]
		[Static]
		[Wrap ("Create(mediaType, outputSettings == null ? null : outputSettings.Dictionary, sourceFormatHint)")]
		AVAssetWriterInput Create (string mediaType, [NullAllowed] AudioSettings outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[iOS (6,0), Mac (10, 8)]
		[Static]
		[Wrap ("Create(mediaType, outputSettings == null ? null : outputSettings.Dictionary, sourceFormatHint)")]
		AVAssetWriterInput Create (string mediaType, [NullAllowed] AVVideoSettingsCompressed outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[Export ("mediaType")]
		string MediaType { get;  }

		[Export ("outputSettings"), NullAllowed]
		NSDictionary OutputSettings { get;  }

		[Export ("transform", ArgumentSemantic.Assign)]
		CGAffineTransform Transform { get; set;  }

		[Export ("metadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] Metadata { get; set;  }

		[Export ("readyForMoreMediaData")]
		bool ReadyForMoreMediaData { [Bind ("isReadyForMoreMediaData")] get;  }

		[Export ("expectsMediaDataInRealTime")]
		bool ExpectsMediaDataInRealTime { get; set;  }

#if XAMCORE_2_0
		[Internal]
#endif
		[Advice ("Use constructor or 'Create' method instead.")]
		[Static, Export ("assetWriterInputWithMediaType:outputSettings:")]
		AVAssetWriterInput FromType (string mediaType, [NullAllowed] NSDictionary outputSettings);

		[Static, Wrap ("FromType (mediaType, outputSettings == null ? null : outputSettings.Dictionary)")]
		AVAssetWriterInput Create (string mediaType, [NullAllowed] AudioSettings outputSettings);

		[Static, Wrap ("FromType (mediaType, outputSettings == null ? null : outputSettings.Dictionary)")]
		AVAssetWriterInput Create (string mediaType, [NullAllowed] AVVideoSettingsCompressed outputSettings);

#if XAMCORE_2_0
		[Protected]
#endif
		// Should be protected
		[Export ("initWithMediaType:outputSettings:")]
		IntPtr Constructor (string mediaType, [NullAllowed] NSDictionary outputSettings);

		[Wrap ("this (mediaType, outputSettings == null ? null : outputSettings.Dictionary)")]		
		IntPtr Constructor (string mediaType, [NullAllowed] AudioSettings outputSettings);

		[Wrap ("this (mediaType, outputSettings == null ? null : outputSettings.Dictionary)")]		
		IntPtr Constructor (string mediaType, [NullAllowed] AVVideoSettingsCompressed outputSettings);

		[Export ("requestMediaDataWhenReadyOnQueue:usingBlock:")]
		void RequestMediaData (DispatchQueue queue, Action action);

		[Export ("appendSampleBuffer:")]
		bool AppendSampleBuffer (CMSampleBuffer sampleBuffer);

		[Export ("markAsFinished")]
		void MarkAsFinished ();

		[Export ("mediaTimeScale")]
		int /* CMTimeScale = int32_t */ MediaTimeScale { get; set; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("languageCode", ArgumentSemantic.Copy), NullAllowed]
		string LanguageCode { get; set; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("extendedLanguageTag", ArgumentSemantic.Copy), NullAllowed]
		string ExtendedLanguageTag { get; set; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("naturalSize")]
		CGSize NaturalSize { get; set; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("preferredVolume")]
		float PreferredVolume { get; set; } // defined as 'float'

		[iOS (7,0), Mac (10, 9)]
		[Export ("marksOutputTrackAsEnabled")]
		bool MarksOutputTrackAsEnabled { get; set; }
		
		[iOS (7,0), Mac (10, 9)]
		[Export ("canAddTrackAssociationWithTrackOfInput:type:")]
		bool CanAddTrackAssociationWithTrackOfInput (AVAssetWriterInput input, NSString trackAssociationType);
		
		[iOS (7,0), Mac (10, 9)]
		[Export ("addTrackAssociationWithTrackOfInput:type:")]
		void AddTrackAssociationWithTrackOfInput (AVAssetWriterInput input, NSString trackAssociationType);
		
		[iOS (6,0), Mac (10, 8)]
		[Export ("sourceFormatHint"), NullAllowed]
		CMFormatDescription SourceFormatHint { get; }

		//
		// AVAssetWriterInputMultiPass Category
		//
		[iOS (8,0), Mac (10,10)]
		[Export ("performsMultiPassEncodingIfSupported")]
		bool PerformsMultiPassEncodingIfSupported { get; set; }

		[iOS (8,0), Mac (10,10)]
		[Export ("canPerformMultiplePasses")]
		bool CanPerformMultiplePasses { get; }

		[iOS (8,0), Mac (10,10)]
		[Export ("currentPassDescription"), NullAllowed]
		AVAssetWriterInputPassDescription CurrentPassDescription { get; }

		[iOS (8,0), Mac (10,10)]
		[Export ("respondToEachPassDescriptionOnQueue:usingBlock:")]
		void SetPassHandler (DispatchQueue queue, Action passHandler);

		[iOS (8,0), Mac (10,10)]
		[Export ("markCurrentPassAsFinished")]
		void MarkCurrentPassAsFinished ();

		[iOS (8,0), Mac (10, 10)]
		[Export ("preferredMediaChunkAlignment")]
		nint PreferredMediaChunkAlignment { get; set; }

		[iOS (8,0), Mac (10, 10)]
		[Export ("preferredMediaChunkDuration")]
		CMTime PreferredMediaChunkDuration { get; set; }

		[iOS (8,0), Mac (10, 10)]
		[Export ("sampleReferenceBaseURL", ArgumentSemantic.Copy), NullAllowed]
		NSUrl SampleReferenceBaseUrl { get; set; }

		// AVAssetWriterInput_AVAssetWriterInputFileTypeSpecificProperties

		[iOS (11, 0), Mac (10, 13), TV (11, 0), NoWatch]
		[Export ("mediaDataLocation")]
		string MediaDataLocation { get; set; }

	}

	[NoWatch]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (NSObject))]
	interface AVAssetWriterInputPassDescription {

		[Export ("sourceTimeRanges")]
		NSValue [] SourceTimeRanges { get; }
	}

	[NoWatch]
	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: *** -[AVAssetWriterInputMetadataAdaptor initWithAssetWriterInput:] invalid parameter not satisfying: input != ((void*)0)
	interface AVAssetWriterInputMetadataAdaptor {

		[DesignatedInitializer]
		[Export ("initWithAssetWriterInput:")]
		IntPtr Constructor (AVAssetWriterInput assetWriterInput);

		[Export ("assetWriterInput")]
		AVAssetWriterInput AssetWriterInput { get; }

		[Static, Export ("assetWriterInputMetadataAdaptorWithAssetWriterInput:")]
		AVAssetWriterInputMetadataAdaptor Create (AVAssetWriterInput input);

		[Export ("appendTimedMetadataGroup:")]
		bool AppendTimedMetadataGroup (AVTimedMetadataGroup timedMetadataGroup);
	}

	[NoWatch]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetWriterInputGroup initWithInputs:defaultInput:] invalid parameter not satisfying: inputs != ((void*)0)
	[iOS (7,0), Mac (10, 9), BaseType (typeof (AVMediaSelectionGroup))]
	interface AVAssetWriterInputGroup {
	
		[Static, Export ("assetWriterInputGroupWithInputs:defaultInput:")]
		AVAssetWriterInputGroup Create (AVAssetWriterInput [] inputs, [NullAllowed] AVAssetWriterInput defaultInput);
	
		[DesignatedInitializer]
		[Export ("initWithInputs:defaultInput:")]
		IntPtr Constructor (AVAssetWriterInput [] inputs, [NullAllowed] AVAssetWriterInput defaultInput);
	
		[Export ("inputs")]
		AVAssetWriterInput [] Inputs { get; }
	
		[Export ("defaultInput", ArgumentSemantic.Copy), NullAllowed]
		AVAssetWriterInput DefaultInput { get; }
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetWriterInputPixelBufferAdaptor initWithAssetWriterInput:sourcePixelBufferAttributes:] invalid parameter not satisfying: input != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetWriterInputPixelBufferAdaptor {
		[Export ("assetWriterInput")]
		AVAssetWriterInput AssetWriterInput { get;  }

		[NullAllowed, Export ("sourcePixelBufferAttributes")]
		NSDictionary SourcePixelBufferAttributes { get;  }

		[Wrap ("SourcePixelBufferAttributes")]
		CVPixelBufferAttributes Attributes { get; }

		[Export ("pixelBufferPool"), NullAllowed]
		CVPixelBufferPool PixelBufferPool { get;  }

#if XAMCORE_2_0
		[Advice ("Use 'Create' method.")]
#endif
		[Static, Export ("assetWriterInputPixelBufferAdaptorWithAssetWriterInput:sourcePixelBufferAttributes:")]
		AVAssetWriterInputPixelBufferAdaptor FromInput (AVAssetWriterInput input, [NullAllowed] NSDictionary sourcePixelBufferAttributes);

		[Static, Wrap ("FromInput (input, attributes == null ? null : attributes.Dictionary)")]
		AVAssetWriterInputPixelBufferAdaptor Create (AVAssetWriterInput input, [NullAllowed] CVPixelBufferAttributes attributes);

		[DesignatedInitializer]
		[Export ("initWithAssetWriterInput:sourcePixelBufferAttributes:")]
		IntPtr Constructor (AVAssetWriterInput input, [NullAllowed] NSDictionary sourcePixelBufferAttributes);

		[Wrap ("this (input, attributes == null ? null : attributes.Dictionary)")]
		IntPtr Constructor (AVAssetWriterInput input, [NullAllowed] CVPixelBufferAttributes attributes);		

		[Export ("appendPixelBuffer:withPresentationTime:")]
		bool AppendPixelBufferWithPresentationTime (CVPixelBuffer pixelBuffer, CMTime presentationTime);
	}

	[NoWatch, iOS (10,0), TV (10,0), Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVAssetCache
	{
		[Export ("playableOffline")]
		bool IsPlayableOffline { [Bind ("isPlayableOffline")] get; }

		[Export ("mediaSelectionOptionsInMediaSelectionGroup:")]
		AVMediaSelectionOption[] GetMediaSelectionOptions (AVMediaSelectionGroup mediaSelectionGroup);
	}

	[NoWatch]
	[BaseType (typeof (AVAsset), Name="AVURLAsset")]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVUrlAsset : AVContentKeyRecipient {

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get;  }

#if XAMCORE_2_0
		[Internal]
#endif
		[Advice ("Use constructor or 'Create' method instead.")]
		[Static, Export ("URLAssetWithURL:options:")]
		AVUrlAsset FromUrl (NSUrl url, [NullAllowed] NSDictionary options);

		[Static]
		[Wrap ("FromUrl (url, options == null ? null : options.Dictionary)")]
		AVUrlAsset Create (NSUrl url, [NullAllowed] AVUrlAssetOptions options);

		[Static]
		[Wrap ("FromUrl (url, (NSDictionary) null)")]
		AVUrlAsset Create (NSUrl url);

		[DesignatedInitializer]
		[Export ("initWithURL:options:")]
		IntPtr Constructor (NSUrl url, [NullAllowed] NSDictionary options);

		[Wrap ("this (url, options == null ? null : options.Dictionary)")]
		IntPtr Constructor (NSUrl url, [NullAllowed] AVUrlAssetOptions options);

		[Wrap ("this (url, (NSDictionary) null)")]
		IntPtr Constructor (NSUrl url);

		[return: NullAllowed]
		[Export ("compatibleTrackForCompositionTrack:")]
		AVAssetTrack CompatibleTrack (AVCompositionTrack forCompositionTrack);

		[Field ("AVURLAssetPreferPreciseDurationAndTimingKey")]
		NSString PreferPreciseDurationAndTimingKey { get; }

		[Field ("AVURLAssetReferenceRestrictionsKey")]
		NSString ReferenceRestrictionsKey { get; }

		[Static, Export ("audiovisualMIMETypes")]
		string [] AudiovisualMimeTypes { get; }

		[Static, Export ("audiovisualTypes")]
		string [] AudiovisualTypes { get; }

		[Static, Export ("isPlayableExtendedMIMEType:")]
		bool IsPlayable (string extendedMimeType);

		[iOS (6,0), Mac (10, 9)]
		[Export ("resourceLoader")]
		AVAssetResourceLoader ResourceLoader { get;  }

#if !MONOMAC
		[iOS (8,0)]
		[Field ("AVURLAssetHTTPCookiesKey")]
		NSString HttpCookiesKey { get; }
#endif

		[iOS (10,0), TV (10,0), Mac (10,12)]
		[NullAllowed, Export ("assetCache")]
		AVAssetCache Cache { get; }

		[iOS (10, 0), TV (10, 0), NoWatch, NoMac]
		[Field ("AVURLAssetAllowsCellularAccessKey")]
		NSString AllowsCellularAccessKey { get; }	
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVAssetTrack : NSCopying {
		[Export ("trackID")]
		int /* CMPersistentTrackID = int32_t */ TrackID { get;  }

		[Export ("asset", ArgumentSemantic.Weak)]
		AVAsset Asset { get; }

		[Export ("mediaType")]
		string MediaType { get;  }

		[iOS (11, 0), Mac (10, 13), TV (11, 0), NoWatch]
		[Export ("decodable")]
		bool Decodable { [Bind ("isDecodable")] get; }

		// Weak version
		[Export ("formatDescriptions")]
		NSObject [] FormatDescriptionsAsObjects { get;  }

		[Wrap ("Array.ConvertAll (FormatDescriptionsAsObjects, l => CMFormatDescription.Create (l.Handle, false))")]
		CMFormatDescription[] FormatDescriptions { get; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get;  }

		[Export ("selfContained")]
		bool SelfContained { [Bind ("isSelfContained")] get;  }

		[Export ("totalSampleDataLength")]
		long TotalSampleDataLength { get;  }

		[Export ("hasMediaCharacteristic:")]
		bool HasMediaCharacteristic (string mediaCharacteristic);

		[Export ("timeRange")]
		CMTimeRange TimeRange { get;  }

		[Export ("naturalTimeScale")]
		int NaturalTimeScale { get;  } // defined as 'CMTimeScale' = int32_t

		[Export ("estimatedDataRate")]
		float EstimatedDataRate { get;  } // defined as 'float'

		[Export ("languageCode")]
		string LanguageCode { get;  }

		[Export ("extendedLanguageTag")]
		string ExtendedLanguageTag { get;  }

		[Export ("naturalSize")]
		CGSize NaturalSize { get;  }

		[Export ("preferredVolume")]
		float PreferredVolume { get;  } // defined as 'float'

		[Export ("preferredTransform")]
		CGAffineTransform PreferredTransform { get; }

		[Export ("nominalFrameRate")]
		float NominalFrameRate { get;  } // defined as 'float'

		[Export ("segments", ArgumentSemantic.Copy)]
		AVAssetTrackSegment [] Segments { get;  }

		[return: NullAllowed]
		[Export ("segmentForTrackTime:")]
		AVAssetTrackSegment SegmentForTrackTime (CMTime trackTime);

		[Export ("samplePresentationTimeForTrackTime:")]
		CMTime SamplePresentationTimeForTrackTime (CMTime trackTime);

		[Export ("availableMetadataFormats")]
		string [] AvailableMetadataFormats { get;  }

		[Export ("commonMetadata")]
		AVMetadataItem [] CommonMetadata { get; }

		[Export ("metadataForFormat:")]
		AVMetadataItem [] MetadataForFormat (string format);

		[Export ("isPlayable")]
		bool Playable { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("availableTrackAssociationTypes")]
		NSString [] AvailableTrackAssociationTypes { get; }

		[iOS (7,0)]
		[Mac (10,10)]
		[Export ("minFrameDuration")]
		CMTime MinFrameDuration { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("associatedTracksOfType:")]
		AVAssetTrack [] GetAssociatedTracks (NSString avAssetTrackTrackAssociationType);

		[iOS (8,0), Mac (10, 10)]
		[Export ("metadata")]
		AVMetadataItem [] Metadata { get; }

		[iOS (8,0), Mac (10, 10)]
		[Export ("requiresFrameReordering")]
		bool RequiresFrameReordering { get; }

		[iOS (9,0), Mac (10,11)]
		[Field ("AVAssetTrackTimeRangeDidChangeNotification")]
		[Notification]
		NSString TimeRangeDidChangeNotification { get; }

		[iOS (9,0), Mac (10,11)]
		[Field ("AVAssetTrackSegmentsDidChangeNotification")]
		[Notification]
		NSString SegmentsDidChangeNotification { get; }

		[iOS (9,0), Mac (10,11)]
		[Field ("AVAssetTrackTrackAssociationsDidChangeNotification")]
		[Notification]
		NSString TrackAssociationsDidChangeNotification { get; }

		[Mac (10,10), NoiOS, NoTV, NoWatch]
		[Export ("canProvideSampleCursors")]
		bool CanProvideSampleCursors { get; }

		[Mac (10,10), NoiOS, NoTV, NoWatch]
		[return: NullAllowed]
		[Export ("makeSampleCursorWithPresentationTimeStamp:")]
		AVSampleCursor MakeSampleCursor (CMTime presentationTimeStamp);

		[Mac (10,10), NoiOS, NoTV, NoWatch]
		[return: NullAllowed]
		[Export ("makeSampleCursorAtFirstSampleInDecodeOrder")]
		AVSampleCursor MakeSampleCursorAtFirstSampleInDecodeOrder ();

		[Mac (10,10), NoiOS, NoTV, NoWatch]
		[return: NullAllowed]
		[Export ("makeSampleCursorAtLastSampleInDecodeOrder")]
		AVSampleCursor MakeSampleCursorAtLastSampleInDecodeOrder ();
	}

	[Mac (10,10), NoiOS, NoTV, NoWatch]
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

		[Export ("currentSampleSyncInfo")]
		AVSampleCursorSyncInfo CurrentSampleSyncInfo { get; }

		[Export ("currentSampleDependencyInfo")]
		AVSampleCursorSyncInfo CurrentSampleDependencyInfo { get; }

		[Mac (10,11)]
		[Export ("samplesRequiredForDecoderRefresh")]
		nint SamplesRequiredForDecoderRefresh { get; }

		[NullAllowed]
		[Export ("currentChunkStorageURL")]
		NSUrl CurrentChunkStorageUrl { get; }

		[Export ("currentChunkStorageRange")]
		AVSampleCursorStorageRange CurrentChunkStorageRange { get; }

		[Export ("currentChunkInfo")]
		AVSampleCursorChunkInfo CurrentChunkInfo { get; }

		[Export ("currentSampleIndexInChunk")]
		long CurrentSampleIndexInChunk { get; }

		[Export ("currentSampleStorageRange")]
		AVSampleCursorStorageRange CurrentSampleStorageRange { get; }
	}

	[NoWatch]
	[iOS (7,0), Mac (10, 9)]
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

		[iOS (8,0)][Mac (10,10)]
		[Field ("AVTrackAssociationTypeMetadataReferent")]
		NSString MetadataReferent { get; }		
	}

	[NoWatch]
	[iOS (7,0), Mac (10, 9)]
	[BaseType (typeof (NSObject))]
	interface AVAssetTrackGroup : NSCopying {
		[Export ("trackIDs", ArgumentSemantic.Copy)]
		NSNumber [] TrackIDs { get; }
	}

	[NoWatch]
	[Mac (10, 8)]
	[BaseType (typeof (NSObject))]
	interface AVMediaSelectionGroup : NSCopying {
		[Export ("options")]
		[Mac (10, 8)]
		AVMediaSelectionOption [] Options { get;  }
		
		[Export ("allowsEmptySelection")]
		[Mac (10, 8)]
		bool AllowsEmptySelection { get;  }

		[return: NullAllowed]
		[Export ("mediaSelectionOptionWithPropertyList:")]
		[Mac (10, 8)]
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

		[iOS (6,0), Mac (10,8)]
		[Static]
		[Export ("mediaSelectionOptionsFromArray:filteredAndSortedAccordingToPreferredLanguages:")]
		AVMediaSelectionOption[] MediaSelectionOptionsFilteredAndSorted (AVMediaSelectionOption[] mediaSelectionOptions, string[] preferredLanguages);

		[iOS (8,0)][Mac (10,10)]
		[Export ("defaultOption"), NullAllowed]
		AVMediaSelectionOption DefaultOption { get; }
	}

	[NoWatch]
	[Mac (10, 8)]
	[BaseType (typeof (NSObject))]
	interface AVMediaSelectionOption : NSCopying {
		[Export ("mediaType")]
		string MediaType { get;  }

		[Export ("mediaSubTypes")]
		NSNumber []  MediaSubTypes { get;  }

		[Export ("playable")]
		bool Playable { [Bind ("isPlayable")] get;  }

		[Export ("locale"), NullAllowed]
		NSLocale Locale { get;  }

		[Export ("commonMetadata")]
		AVMetadataItem [] CommonMetadata { get;  }

		[Export ("availableMetadataFormats")]
		string [] AvailableMetadataFormats { get;  }

		[Export ("hasMediaCharacteristic:")]
		bool HasMediaCharacteristic (string mediaCharacteristic);

		[Export ("metadataForFormat:")]
		AVMetadataItem [] GetMetadataForFormat (string format);

		[return: NullAllowed]
		[Export ("associatedMediaSelectionOptionInMediaSelectionGroup:")]
		AVMediaSelectionOption AssociatedMediaSelectionOptionInMediaSelectionGroup (AVMediaSelectionGroup mediaSelectionGroup);

		[Export ("propertyList")]
		NSObject PropertyList { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("displayName")]
		string DisplayName { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("displayNameWithLocale:")]
		string GetDisplayName (NSLocale locale);

		[iOS (7,0), Mac (10, 9)]
		[Export ("extendedLanguageTag"), NullAllowed]
		string ExtendedLanguageTag { get; }
	}

	[NoWatch]
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

#if !XAMCORE_4_0
		[Field ("AVMetadataFormatQuickTimeUserData")]
		[Obsolete ("Use 'AVMetadataFormat' enum values")]
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

		[Mac (10, 8)]
		[Field ("AVMetadataQuickTimeUserDataKeyTaggedCharacteristic")]
		NSString QuickTimeUserDataKeyTaggedCharacteristic { get; }
		
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
		
		[iOS (7,0), Mac (10, 9)]
		[Field ("AVMetadata3GPUserDataKeyCollection")]
		NSString K3GPUserDataKeyCollection { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Field ("AVMetadata3GPUserDataKeyUserRating")]
		NSString K3GPUserDataKeyUserRating { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Field ("AVMetadata3GPUserDataKeyThumbnail")]
		NSString K3GPUserDataKeyThumbnail { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Field ("AVMetadata3GPUserDataKeyAlbumAndTrack")]
		NSString K3GPUserDataKeyAlbumAndTrack { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Field ("AVMetadata3GPUserDataKeyKeywordList")]
		NSString K3GPUserDataKeyKeywordList { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Field ("AVMetadata3GPUserDataKeyMediaClassification")]
		NSString K3GPUserDataKeyMediaClassification { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Field ("AVMetadata3GPUserDataKeyMediaRating")]
		NSString K3GPUserDataKeyMediaRating { get; }

#if !XAMCORE_4_0
		[iOS (7,0), Mac (10, 9)]
		[Field ("AVMetadataFormatISOUserData")]
		[Obsolete ("Use 'AVMetadataFormat' enum values")]
		NSString KFormatISOUserData { get; }
#endif

		[iOS (7,0), Mac (10, 9)]
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

		[iOS (9,0), Mac (10,11)]
		[Field ("AVMetadataQuickTimeMetadataKeyContentIdentifier")]
		NSString QuickTimeMetadataKeyContentIdentifier { get; }
		
#if !XAMCORE_4_0
		[Field ("AVMetadataFormatiTunesMetadata")]
		[Obsolete ("Use 'AVMetadataFormat' enum values")]
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
		
#if !XAMCORE_4_0
		[Field ("AVMetadataFormatID3Metadata")]
		[Obsolete ("Use 'AVMetadataFormat' enum values")]
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

		[iOS (9,0), Mac (10,11)]
		[Field ("AVMetadataID3MetadataKeyCommercial")]
		NSString ID3MetadataKeyCommercial { get; }

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Mac (10, 7)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
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

		[iOS (8,0)][Mac (10,10)]
		[Field ("AVMetadataISOUserDataKeyTaggedCharacteristic")]
		NSString IsoUserDataKeyTaggedCharacteristic { get; }
		
		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Field ("AVMetadataISOUserDataKeyDate")]
		NSString IsoUserDataKeyDate { get; }

		[iOS (8,0)][Mac (10,10)]
		[Field ("AVMetadataKeySpaceIcy")]
		NSString KeySpaceIcy { get; }
		
		[iOS (8,0)][Mac (10,10)]
		[Field ("AVMetadataIcyMetadataKeyStreamTitle")]
		NSString IcyMetadataKeyStreamTitle { get; }
		
		[iOS (8,0)][Mac (10,10)]
		[Field ("AVMetadataIcyMetadataKeyStreamURL")]
		NSString IcyMetadataKeyStreamUrl { get; }
		
#if !XAMCORE_4_0
		[iOS (8,0)][Mac (10,10)]
		[Field ("AVMetadataFormatHLSMetadata")]
		[Obsolete ("Use 'AVMetadataFormat' enum values")]
		NSString FormatHlsMetadata { get; }
#endif

		[iOS (9,3)][Mac (10, 12)]
		[TV (9,2)]
		[Field ("AVMetadataKeySpaceHLSDateRange")]
		NSString KeySpaceHlsDateRange { get; }

		[iOS (11, 0), Mac (10, 13)]
		[TV (11, 0)]
		[Field ("AVMetadataKeySpaceAudioFile")]
		NSString KeySpaceAudioFile { get; }
	}

	[NoWatch]
	[Static]
	interface AVMetadataExtraAttribute {

		[iOS (8,0)][Mac (10,10)]
		[Field ("AVMetadataExtraAttributeValueURIKey")]
		NSString ValueUriKey { get; }

		[iOS (8,0)][Mac (10,10)]
		[Field ("AVMetadataExtraAttributeBaseURIKey")]
		NSString BaseUriKey { get; }

		[iOS (9,0)][Mac (10,11)]
		[Field ("AVMetadataExtraAttributeInfoKey")]
		NSString InfoKey { get; }
	}

	class AVMetadataIdentifiers {
		[NoWatch]
		[iOS (8,0)][Mac (10,10)]
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
		}

		[NoWatch]
		[iOS (8,0)][Mac (10,10)]
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
		}

		[NoWatch]
		[iOS (8,0)][Mac (10,10)]
		[Static]
		interface Iso {
			
			[iOS (10, 0), TV (10,0), Mac (10,12)]
			[Field ("AVMetadataIdentifierISOUserDataDate")]
			NSString UserDataDate { get; }

			[Field ("AVMetadataIdentifierISOUserDataCopyright")]
			NSString UserDataCopyright { get; }
			
			[Field ("AVMetadataIdentifierISOUserDataTaggedCharacteristic")]
			NSString UserDataTaggedCharacteristic { get; }
		}

		[NoWatch]
		[iOS (8,0)][Mac (10,10)]
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

		[NoWatch]
		[iOS (8,0)][Mac (10,10)]
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

			[iOS (9,0), Mac (10,11)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataDetectedFace")]
			NSString DetectedFace { get; }

			[iOS (9,0), Mac (10,11)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataVideoOrientation")]
			NSString VideoOrientation { get; }

			[iOS (9,0), Mac (10,11)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataContentIdentifier")]
			NSString ContentIdentifier { get; }
		}

		[NoWatch]
		[iOS (8,0)][Mac (10,10)]
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

		[NoWatch]
		[iOS (8,0)][Mac (10,10)]
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

			[iOS (9,0), Mac (10,11)]
			[Field ("AVMetadataIdentifierID3MetadataCommercial")]
			NSString Commercial { get; }

			[iOS (8, 0)]
			[Deprecated (PlatformName.iOS, 9, 0)]
			[Mac (10, 10)]
			[Deprecated (PlatformName.MacOSX, 10, 11)]
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

		[NoWatch]
		[iOS (8,0)][Mac (10,10)]
		[Static]
		interface IcyMetadata {
			[Field ("AVMetadataIdentifierIcyMetadataStreamTitle")]
			NSString StreamTitle { get; }
			
			[Field ("AVMetadataIdentifierIcyMetadataStreamURL")]
			NSString StreamUrl { get; }
		}
	}

	[NoWatch]
	[Mac (10,7)]
	[BaseType (typeof (NSObject))]
	interface AVMetadataItem : NSMutableCopying {
		[Export ("commonKey", ArgumentSemantic.Copy), NullAllowed]
		string CommonKey { get;  }

		[Export ("keySpace", ArgumentSemantic.Copy), NullAllowed]
		string KeySpace { get; [NotImplemented] set; }

		[Export ("locale", ArgumentSemantic.Copy), NullAllowed]
		NSLocale Locale { get;  [NotImplemented] set; }

		[Export ("time")]
		CMTime Time { get; [NotImplemented] set; }

		[Export ("value", ArgumentSemantic.Copy), NullAllowed]
		NSObject Value { get; [NotImplemented] set; }

		[Export ("extraAttributes", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary ExtraAttributes { get; [NotImplemented] set; }

		[Export ("key", ArgumentSemantic.Copy), NullAllowed]
		NSObject Key { get; }

		[Export ("stringValue"), NullAllowed]
		string StringValue { get;  }

		[Export ("numberValue"), NullAllowed]
		NSNumber NumberValue { get;  }

		[Export ("dateValue"), NullAllowed]
		NSDate DateValue { get;  }

		[Export ("dataValue"), NullAllowed]
		NSData DataValue { get;  }

		[Static]
		[Export ("metadataItemsFromArray:withLocale:")]
		AVMetadataItem [] FilterWithLocale (AVMetadataItem [] arrayToFilter, NSLocale locale);

		[Static]
		[Export ("metadataItemsFromArray:withKey:keySpace:")]
		AVMetadataItem [] FilterWithKey (AVMetadataItem [] metadataItems, NSObject key, string keySpace);

		[iOS (7,0), Mac (10,9)]
		[Static, Export ("metadataItemsFromArray:filteredByMetadataItemFilter:")]
		AVMetadataItem [] FilterWithItemFilter (AVMetadataItem [] metadataItems, AVMetadataItemFilter metadataItemFilter);

		[Export ("duration")]
		CMTime Duration { get; [NotImplemented] set; }

		[Export ("statusOfValueForKey:error:")]
		AVKeyValueStatus StatusOfValueForKeyerror (string key, out NSError error);

		[Export ("loadValuesAsynchronouslyForKeys:completionHandler:")]
		[Async ("LoadValuesTaskAsync")]
		void LoadValuesAsynchronously (string [] keys, [NullAllowed] Action handler);

		[iOS (6,0)]
		[Static, Export ("metadataItemsFromArray:filteredAndSortedAccordingToPreferredLanguages:")]
		AVMetadataItem [] FilterFromPreferredLanguages (AVMetadataItem [] metadataItems, string [] preferredLanguages);

		[iOS (8,0)][Mac (10,10)]
		[Export ("identifier"), NullAllowed]
		NSString MetadataIdentifier { get; [NotImplemented] set; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("extendedLanguageTag"), NullAllowed]
		string ExtendedLanguageTag { get; [NotImplemented] set; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("dataType"), NullAllowed]
		NSString DataType { get; [NotImplemented] set; }

		[iOS (8,0)][Mac (10,10)]
		[return: NullAllowed]
		[Static, Export ("identifierForKey:keySpace:")]
		NSString GetMetadataIdentifier (NSObject key, NSString keySpace);

		[iOS (8,0)][Mac (10,10)]
		[return: NullAllowed]
		[Static, Export ("keySpaceForIdentifier:")]
		NSString GetKeySpaceForIdentifier (NSString identifier);

		[iOS (8,0)][Mac (10,10)]
		[return: NullAllowed]
		[Static, Export ("keyForIdentifier:")]
		NSObject GetKeyForIdentifier (NSString identifier);

		[iOS(8,0)][Mac (10,10)]
		[Static, Export ("metadataItemsFromArray:filteredByIdentifier:")]
		AVMetadataItem [] FilterWithIdentifier (AVMetadataItem [] metadataItems, NSString metadataIdentifer);

		[iOS(9,0)][Mac (10,11)]
		[Export ("startDate"), NullAllowed]
		NSDate StartDate { get; [NotImplemented] set; }

		[iOS (9,0), Mac (10,11)]
		[Static]
		[Export ("metadataItemWithPropertiesOfMetadataItem:valueLoadingHandler:")]
		AVMetadataItem GetMetadataItem (AVMetadataItem metadataItem, Action<AVMetadataItemValueRequest> handler);
	}

	[NoWatch]
	[iOS (9,0), Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface AVMetadataItemValueRequest {
		
		[NullAllowed, Export ("metadataItem", ArgumentSemantic.Weak)]
		AVMetadataItem MetadataItem { get; }

		[Export ("respondWithValue:")]
		void Respond (NSObject value);

		[Export ("respondWithError:")]
		void Respond (NSError error);
	}

	[NoWatch]
	[iOS (7,0), Mac (10, 9)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // for binary compatibility this is added in AVMetadataItemFilter.cs w/[Obsolete]
	interface AVMetadataItemFilter {
		[Static, Export ("metadataItemFilterForSharing")]
		AVMetadataItemFilter ForSharing { get; }
	}

	[NoWatch]
	[NoTV]
	[iOS (6,0)][Mac (10,10, onlyOn64:true)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSGenericException Reason: Cannot instantiate AVMetadataObject because it is an abstract superclass.
	[DisableDefaultCtor]
	interface AVMetadataObject {
		[Export ("duration")]
		CMTime Duration { get;  }

		[Export ("bounds")]
		CGRect Bounds { get;  }

		[Export ("type")]
#if XAMCORE_2_0
		NSString WeakType { get;  }
#else
		NSString Type { get;  }
#endif

		[Export ("time")]
		CMTime Time{ get;}

		[Field ("AVMetadataObjectTypeFace"), Mac (10,10)]
		NSString TypeFace { get; }

		[NoTV, iOS (7,0), NoMac]
		[Field ("AVMetadataObjectTypeAztecCode")]
		NSString TypeAztecCode { get; }
		
		[NoTV, iOS (7,0), NoMac]
		[Field ("AVMetadataObjectTypeCode128Code")]
		NSString TypeCode128Code { get; }
		
		[NoTV, iOS (7,0), NoMac]
		[Field ("AVMetadataObjectTypeCode39Code")]
		NSString TypeCode39Code { get; }
		
		[NoTV, iOS (7,0), NoMac]
		[Field ("AVMetadataObjectTypeCode39Mod43Code")]
		NSString TypeCode39Mod43Code { get; }
		
		[NoTV, iOS (7,0), NoMac]
		[Field ("AVMetadataObjectTypeCode93Code")]
		NSString TypeCode93Code { get; }
		
		[NoTV, iOS (7,0), NoMac]
		[Field ("AVMetadataObjectTypeEAN13Code")]
		NSString TypeEAN13Code { get; }
		
		[NoTV, iOS (7,0), NoMac]
		[Field ("AVMetadataObjectTypeEAN8Code")]
		NSString TypeEAN8Code { get; }
		
		[Field ("AVMetadataObjectTypePDF417Code")]
		[NoTV, iOS (7,0), NoMac]
		NSString TypePDF417Code { get; }
		
		[NoTV, iOS (7,0), NoMac]
		[Field ("AVMetadataObjectTypeQRCode")]
		NSString TypeQRCode { get; }
		
		[NoTV, iOS (7,0), NoMac]
		[Field ("AVMetadataObjectTypeUPCECode")]
		NSString TypeUPCECode { get; }

		[NoTV, iOS (8,0), NoMac]
		[Field ("AVMetadataObjectTypeInterleaved2of5Code")]
		NSString TypeInterleaved2of5Code { get; }
		
		[NoTV, iOS (8,0), NoMac]
		[Field ("AVMetadataObjectTypeITF14Code")]
		NSString TypeITF14Code { get; }
		
		[NoTV, iOS (8,0), NoMac]
		[Field ("AVMetadataObjectTypeDataMatrixCode")]
		NSString TypeDataMatrixCode { get; }
	}

	[NoWatch]
	[NoTV]
	[iOS (6,0)][Mac (10,10, onlyOn64:true)]
	[BaseType (typeof (AVMetadataObject))]
	interface AVMetadataFaceObject : NSCopying {
		[Export ("hasRollAngle")]
		bool HasRollAngle { get;  }

		[Export ("rollAngle")]
		nfloat RollAngle { get;  }

		[Export ("hasYawAngle")]
		bool HasYawAngle { get;  }

		[Export ("yawAngle")]
		nfloat YawAngle { get;  }

		[Export ("faceID")]
		nint FaceID { get; }
	}

	[NoWatch]
	[NoTV]
	[NoMac]
	[iOS (7,0), BaseType (typeof (AVMetadataObject))]
	interface AVMetadataMachineReadableCodeObject {
		[NullAllowed]
		[Export ("corners", ArgumentSemantic.Copy)]
#if XAMCORE_2_0
		NSDictionary [] WeakCorners { get; }
#else
		NSDictionary [] Corners { get; }
#endif

		[Export ("stringValue", ArgumentSemantic.Copy)]
		string StringValue { get; }

		// @interface AVMetadataMachineReadableCodeDescriptor (AVMetadataMachineReadableCodeObject)

		[iOS (11, 0)]
		[Export ("descriptor")]
		[NullAllowed]
		CIBarcodeDescriptor Descriptor { get; }
	}

	[NoWatch]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (NSObject), Name="AVMIDIPlayer")]
	interface AVMidiPlayer {

		[Export ("initWithContentsOfURL:soundBankURL:error:")]
		IntPtr Constructor (NSUrl contentsUrl, [NullAllowed] NSUrl soundBankUrl, out NSError outError);

		[Export ("initWithData:soundBankURL:error:")]
		IntPtr Constructor (NSData data, [NullAllowed] NSUrl sounddBankUrl, out NSError outError);

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

#if MONOMAC
	[Mac (10,10)]
	[DisableDefaultCtor]
	[BaseType (typeof(AVAsset))]
	interface AVMovie : NSCopying, NSMutableCopying
	{
		[Field ("AVMovieReferenceRestrictionsKey")]
		NSString ReferenceRestrictionsKey { get; }
		
		[Static]
		[Export ("movieTypes")]
		string[] MovieTypes { get; }

		[Static]
		[Export ("movieWithURL:options:")]
		AVMovie FromUrl (NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("initWithURL:options:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Mac (10,11)]
		[Static]
		[Export ("movieWithData:options:")]
		AVMovie FromData (NSData data, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Mac (10,11)]
		[Export ("initWithData:options:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSData data, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[NullAllowed, Export ("URL")]
		NSUrl URL { get; }

		[Mac (10, 11)]
		[NullAllowed, Export ("data")]
		NSData Data { get; }

		[Mac (10, 11)]
		[NullAllowed, Export ("defaultMediaDataStorage")]
		AVMediaDataStorage DefaultMediaDataStorage { get; }

		[Export ("tracks")]
		AVMovieTrack[] Tracks { get; }

		[Export ("canContainMovieFragments")]
		bool CanContainMovieFragments { get; }

		[Mac (10, 11)]
		[Export ("containsMovieFragments")]
		bool ContainsMovieFragments { get; }
	}

	[Category]
	[BaseType (typeof(AVMovie))]
	interface AVMovie_AVMovieMovieHeaderSupport
	{
		[Mac (10,11)]
		[Export ("movieHeaderWithFileType:error:")]
		[return: NullAllowed]
		NSData GetMovieHeader (string fileType, [NullAllowed] out NSError outError);

		[Mac (10,11)]
		[Export ("writeMovieHeaderToURL:fileType:options:error:")]
		bool WriteMovieHeader (NSUrl URL, string fileType, AVMovieWritingOptions options, [NullAllowed] out NSError outError);

		[Mac (10,11)]
		[Export ("isCompatibleWithFileType:")]
		bool IsCompatibleWithFileType (string fileType);
	}

	[Category]
	[BaseType (typeof(AVMovie))]
	interface AVMovie_AVMovieTrackInspection
	{
		[Export ("trackWithTrackID:")]
		[return: NullAllowed]
		AVMovieTrack GetTrack (int trackID);

		[Export ("tracksWithMediaType:")]
		AVMovieTrack[] GetTracks (string mediaType);

		[Wrap ("This.GetTracks (mediaType.GetConstant ())")]
		AVMovieTrack[] GetTracks (AVMediaTypes mediaType);

		[Export ("tracksWithMediaCharacteristic:")]
		AVMovieTrack[] GetTracksWithMediaCharacteristic (string mediaCharacteristic);

		[Wrap ("This.GetTracksWithMediaCharacteristic (mediaCharacteristic.GetConstant ())")]
		AVMovieTrack[] GetTracks (AVMediaCharacteristics mediaCharacteristic);

	}

	[Mac (10,11)]
	[BaseType (typeof(AVMovie))]
	interface AVMutableMovie
	{
		[Static]
		[Export ("movieWithURL:options:error:")]
		[return: NullAllowed]
		AVMutableMovie FromUrl (NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError outError);

		[Export ("initWithURL:options:error:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError outError);

		[Static]
		[Export ("movieWithData:options:error:")]
		[return: NullAllowed]
		AVMutableMovie FromData (NSData data, [NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError outError);

		[Export ("initWithData:options:error:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSData data, [NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError outError);

		[Static]
		[Export ("movieWithSettingsFromMovie:options:error:")]
		[return: NullAllowed]
		AVMutableMovie FromMovie ([NullAllowed] AVMovie movie, [NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError outError);

		[Export ("initWithSettingsFromMovie:options:error:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] AVMovie movie, [NullAllowed] NSDictionary<NSString, NSObject> options, [NullAllowed] out NSError outError);

		[Export ("preferredRate")]
		float PreferredRate { get; set; }

		[Export ("preferredVolume")]
		float PreferredVolume { get; set; }

		[Export ("preferredTransform", ArgumentSemantic.Assign)]
		CGAffineTransform PreferredTransform { get; set; }

		[Export ("timescale")]
		int Timescale { get; set; }

		[Export ("tracks")]
		AVMutableMovieTrack[] Tracks { get; }

		// AVMutableMovie_AVMutableMovieMetadataEditing
		[Export ("metadata", ArgumentSemantic.Copy)]
		AVMetadataItem[] Metadata { get; set; }

		// AVMutableMovie_AVMutableMovieMovieLevelEditing
		[Export ("modified")]
		bool Modified { [Bind ("isModified")] get; set; }

		[Export ("defaultMediaDataStorage", ArgumentSemantic.Copy)]
		AVMediaDataStorage DefaultMediaDataStorage { get; set; }

		[Export ("interleavingPeriod", ArgumentSemantic.Assign)]
		CMTime InterleavingPeriod { get; set; }
	}

	[NoWatch]
	[Category]
	[BaseType (typeof(AVMutableMovie))]
	interface AVMutableMovie_AVMutableMovieMovieLevelEditing
	{
		[Export ("insertTimeRange:ofAsset:atTime:copySampleData:error:")]
		bool InsertTimeRange (CMTimeRange timeRange, AVAsset asset, CMTime startTime, bool copySampleData, [NullAllowed] out NSError outError);

		[Export ("insertEmptyTimeRange:")]
		void InsertEmptyTimeRange (CMTimeRange timeRange);

		[Export ("removeTimeRange:")]
		void RemoveTimeRange (CMTimeRange timeRange);

		[Export ("scaleTimeRange:toDuration:")]
		void ScaleTimeRange (CMTimeRange timeRange, CMTime duration);
	}

	[NoWatch]
	[Category]
	[BaseType (typeof(AVMutableMovie))]
	interface AVMutableMovie_AVMutableMovieTrackLevelEditing
	{
		[Export ("mutableTrackCompatibleWithTrack:")]
		[return: NullAllowed]
		AVMutableMovieTrack GetMutableTrack (AVAssetTrack track);

		[Export ("addMutableTrackWithMediaType:copySettingsFromTrack:options:")]
		AVMutableMovieTrack AddMutableTrack (string mediaType, [NullAllowed] AVAssetTrack track, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("addMutableTracksCopyingSettingsFromTracks:options:")]
		AVMutableMovieTrack[] AddMutableTracks (AVAssetTrack[] existingTracks, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("removeTrack:")]
		void RemoveTrack (AVMovieTrack track);
	}

	[Category]
	[BaseType (typeof(AVMutableMovie))]
	interface AVMutableMovie_AVMutableMovieTrackInspection
	{
		[Export ("trackWithTrackID:")]
		[return: NullAllowed]
		AVMutableMovieTrack GetTrack (int trackID);

		[Export ("tracksWithMediaType:")]
		AVMutableMovieTrack[] GetTracks (string mediaType);

		[Wrap ("This.GetTracks (mediaType.GetConstant ())")]
		AVMutableMovieTrack[] GetTracks (AVMediaTypes mediaType);

		[Export ("tracksWithMediaCharacteristic:")]
		AVMutableMovieTrack[] GetTracksWithMediaCharacteristic (string mediaCharacteristic);

		[Wrap ("This.GetTracksWithMediaCharacteristic (mediaCharacteristic.GetConstant ())")]
		AVMutableMovieTrack[] GetTracks (AVMediaCharacteristics mediaCharacteristic);
	}

	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVMediaDataStorage
	{
		[Export ("initWithURL:options:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[NullAllowed, Export ("URL")]
		NSUrl URL { get; }
	}

	[Mac (10,10)]
	[DisableDefaultCtor]
	[BaseType (typeof(AVMovie))]
	interface AVFragmentedMovie : AVFragmentMinding
	{
		[Export ("initWithURL:options:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl URL, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Mac (10,11)]
		[Export ("initWithData:options:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSData data, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("tracks")]
		AVFragmentedMovieTrack[] Tracks { get; }

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

	[Category]
	[BaseType (typeof(AVFragmentedMovie))]
	interface AVFragmentedMovie_AVFragmentedMovieTrackInspection
	{
		[Export ("trackWithTrackID:")]
		[return: NullAllowed]
		AVFragmentedMovieTrack GetTrack (int trackID);

		[Export ("tracksWithMediaType:")]
		AVFragmentedMovieTrack[] GetTracks (string mediaType);

		[Wrap ("This.GetTracks (mediaType.GetConstant ())")]
		AVFragmentedMovieTrack[] GetTracks (AVMediaTypes mediaType);

		[Export ("tracksWithMediaCharacteristic:")]
		AVFragmentedMovieTrack[] GetTracksWithMediaCharacteristic (string mediaCharacteristic);

		[Wrap ("This.GetTracksWithMediaCharacteristic (mediaCharacteristic.GetConstant ())")]
		AVFragmentedMovieTrack[] GetTracks (AVMediaCharacteristics mediaCharacteristic);
	}

	[Mac (10,10)]
	[BaseType (typeof(AVFragmentedAssetMinder))]
	interface AVFragmentedMovieMinder
	{
		[Static]
		[Export ("fragmentedMovieMinderWithMovie:mindingInterval:")]
		AVFragmentedMovieMinder FromMovie (AVFragmentedMovie movie, double mindingInterval);

		[Export ("initWithMovie:mindingInterval:")]
		[DesignatedInitializer]
		IntPtr Constructor (AVFragmentedMovie movie, double mindingInterval);

		[Export ("mindingInterval")]
		double MindingInterval { get; set; }

		[Export ("movies")]
		AVFragmentedMovie[] Movies { get; }

		[Export ("addFragmentedMovie:")]
		void Add (AVFragmentedMovie movie);

		[Export ("removeFragmentedMovie:")]
		void Remove (AVFragmentedMovie movie);
	}

	[NoWatch]
	[Mac (10,10)]
	[BaseType (typeof(AVAssetTrack))]
	[DisableDefaultCtor]
	interface AVMovieTrack
	{
		[Mac (10, 11)]
		[Export ("mediaPresentationTimeRange")]
		CMTimeRange MediaPresentationTimeRange { get; }

		[Mac (10, 11)]
		[Export ("mediaDecodeTimeRange")]
		CMTimeRange MediaDecodeTimeRange { get; }

		[Mac (10, 11)]
		[Export ("alternateGroupID")]
		nint AlternateGroupID { get; }

		// AVMovieTrack_AVMovieTrackMediaDataStorage
		[Mac (10, 11)]
		[NullAllowed, Export ("mediaDataStorage", ArgumentSemantic.Copy)]
		AVMediaDataStorage MediaDataStorage { get; }
	}

	[Mac (10,11)]
	[BaseType (typeof(AVMovieTrack))]
	[DisableDefaultCtor]
	interface AVMutableMovieTrack
	{
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
		AVMetadataItem[] Metadata { get; set; }

		[Mac (10,12)]
		[Export ("appendSampleBuffer:decodeTime:presentationTime:error:")]
		bool AppendSampleBuffer (CMSampleBuffer sampleBuffer, out CMTime outDecodeTime, out CMTime presentationTime, out NSError error);

		[Mac (10,12)]
		[Export ("insertMediaTimeRange:intoTimeRange:")]
		bool InsertMediaTimeRange (CMTimeRange mediaTimeRange, CMTimeRange trackTimeRange);

		[NoWatch, NoTV, NoiOS, Mac (10,13)]
		[Export ("replaceFormatDescription:withFormatDescription:")]
		void ReplaceFormatDescription (CMFormatDescription formatDescription, CMFormatDescription newFormatDescription);
	}

	[NoWatch]
	[Category]
	[BaseType (typeof(AVMutableMovieTrack))]
	interface AVMutableMovieTrack_AVMutableMovieTrack_TrackLevelEditing
	{
		[Export ("insertTimeRange:ofTrack:atTime:copySampleData:error:")]
		bool InsertTimeRange (CMTimeRange timeRange, AVAssetTrack track, CMTime startTime, bool copySampleData, [NullAllowed] out NSError outError);

		[Export ("insertEmptyTimeRange:")]
		void InsertEmptyTimeRange (CMTimeRange timeRange);

		[Export ("removeTimeRange:")]
		void RemoveTimeRange (CMTimeRange timeRange);

		[Export ("scaleTimeRange:toDuration:")]
		void ScaleTimeRange (CMTimeRange timeRange, CMTime duration);
	}

	[Category]
	[BaseType (typeof(AVMutableMovieTrack))]
	interface AVMutableMovieTrack_AVMutableMovieTrackTrackAssociations
	{
		[Export ("addTrackAssociationToTrack:type:")]
		void AddTrackAssociation (AVMovieTrack movieTrack, string trackAssociationType);

		[Export ("removeTrackAssociationToTrack:type:")]
		void RemoveTrackAssociation (AVMovieTrack movieTrack, string trackAssociationType);
	}

	[Mac (10,10)]
	[BaseType (typeof(AVMovieTrack))]
	[DisableDefaultCtor]
	interface AVFragmentedMovieTrack
	{
#if !XAMCORE_4_0
		[Mac (10, 10)]
		[Field ("AVFragmentedMovieTrackTimeRangeDidChangeNotification")]
		NSString ATimeRangeDidChangeNotification { get; }
#endif

		[Mac (10, 10)]
		[Field ("AVFragmentedMovieTrackTimeRangeDidChangeNotification")]
		[Notification]
		NSString TimeRangeDidChangeNotification { get; }

		[Mac (10, 10)]
		[Notification]
		[Field ("AVFragmentedMovieTrackSegmentsDidChangeNotification")]
		NSString SegmentsDidChangeNotification { get; }

		[Mac (10, 10)]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use either 'AVFragmentedMovieTrackTimeRangeDidChangeNotification' or 'AVFragmentedMovieTrackSegmentsDidChangeNotification' instead. In either case, you can assume that the sender's 'TotalSampleDataLength' has changed.")]
		[Field ("AVFragmentedMovieTrackTotalSampleDataLengthDidChangeNotification")]
		NSString TotalSampleDataLengthDidChangeNotification { get; }
	}
#endif

	[NoWatch]
	[BaseType (typeof (AVMetadataItem))]
	interface AVMutableMetadataItem {
		[NullAllowed] // by default this property is null
		[Export ("keySpace", ArgumentSemantic.Copy)]
		[Override]
		string KeySpace { get; set;  }

		[Export ("metadataItem"), Static]
		AVMutableMetadataItem Create ();

		[Export ("duration")]
		[Override]
		CMTime Duration { get; set; }

		[iOS (8,0)]
		[NullAllowed] // by default this property is null
		[Export ("identifier", ArgumentSemantic.Copy)]
		[Override]
		NSString MetadataIdentifier { get; set; }
		
		[NullAllowed] // by default this property is null
		[Export ("locale", ArgumentSemantic.Copy)]
		[Override]
		NSLocale Locale { get; set;  }

		[Export ("time")]
		[Override]
		CMTime Time { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("value", ArgumentSemantic.Copy)]
		[Override]
		NSObject Value { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("extraAttributes", ArgumentSemantic.Copy)]
		[Override]
		NSDictionary ExtraAttributes { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("key", ArgumentSemantic.Copy)]
		NSObject Key { get; set; }
		
		[iOS (8,0)]
		[NullAllowed] // by default this property is null
		[Export ("dataType", ArgumentSemantic.Copy)]
		[Override]
		NSString DataType { get; set; }

		[iOS (8,0)]
		[NullAllowed] // by default this property is null
		[Export ("extendedLanguageTag")]
		[Override]
		string ExtendedLanguageTag { get; set; }

		[iOS(9,0)][Mac (10,11)]
		[Export ("startDate"), NullAllowed]
		[Override]
		NSDate StartDate { get; set; }
	}

	[NoWatch]
	[BaseType (typeof (AVAssetTrack))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVCompositionTrack {
		[Export ("segments", ArgumentSemantic.Copy)]
		AVCompositionTrackSegment [] Segments { get; }
	}

	[NoWatch]
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
		bool InsertTimeRanges (NSValue[] cmTimeRanges, AVAssetTrack [] tracks, CMTime startTime, out NSError error);

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Export ("addTrackAssociationToTrack:type:")]
		void AddTrackAssociation (AVCompositionTrack compositionTrack, string trackAssociationType);

		[TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Export ("removeTrackAssociationToTrack:type:")]
		void RemoveTrackAssociation (AVCompositionTrack compositionTrack, string trackAssociationType);
	}

	[NoWatch]
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

		[iOS (8,0)]
		[Mac (10,10)]
		[Field ("AVErrorPresentationTimeStampKey")]
		NSString PresentationTimeStamp { get; }
		
		[iOS (8,0)]
		[Mac (10,10)]
		[Field ("AVErrorPersistentTrackIDKey")]
		NSString PersistentTrackID { get; }
		
		[iOS (8,0)]
		[Mac (10,10)]
		[Field ("AVErrorFileTypeKey")]
		NSString FileType { get; }

		[NoiOS, NoWatch]
		[NoTV]
		[Mac (10,7)]
		[Field ("AVErrorDiscontinuityFlagsKey")]
		NSString DiscontinuityFlags { get; }
	}
	
	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface AVAssetTrackSegment {
		[Export ("empty")]
		bool Empty { [Bind ("isEmpty")] get;  }

		[Export ("timeMapping")]
		CMTimeMapping TimeMapping { get; }

	}

	[NoWatch]
	[BaseType (typeof (AVAsset))]
	interface AVComposition : NSMutableCopying {
		[Export ("tracks")]
		[New]
		AVCompositionTrack [] Tracks { get; }

		[Export ("naturalSize")]
		[New]
		CGSize NaturalSize { get; [NotImplemented] set; }

		[iOS (9,0), Mac (10,11)]
		[Export ("URLAssetInitializationOptions", ArgumentSemantic.Copy)]
		NSDictionary<NSString,NSObject> UrlAssetInitializationOptions { get; }
	}

	[NoWatch]
	[iOS (9,0), Mac (10,11)]
	[Category]
	[BaseType (typeof (AVComposition))]
	interface AVComposition_AVCompositionTrackInspection {
		
		[Export ("trackWithTrackID:")]
		[return: NullAllowed]
		AVCompositionTrack GetTrack (int trackID);

		[Export ("tracksWithMediaType:")]
		AVCompositionTrack[] GetTracks (string mediaType);

		[Wrap ("This.GetTracks (mediaType.GetConstant ())")]
		AVCompositionTrack[] GetTracks (AVMediaTypes mediaType);

		[Export ("tracksWithMediaCharacteristic:")]
		AVCompositionTrack[] GetTracksWithMediaCharacteristic (string mediaCharacteristic);

		[Wrap ("This.GetTracksWithMediaCharacteristic (mediaCharacteristic.GetConstant ())")]
		AVCompositionTrack[] GetTracks (AVMediaCharacteristics mediaCharacteristic);
	}

	[NoWatch]
	[BaseType (typeof (AVComposition))]
	interface AVMutableComposition {
		
		[Export ("composition"), Static]
		AVMutableComposition Create ();

		[iOS (9,0), Mac (10,11)]
		[Static]
		[Export ("compositionWithURLAssetInitializationOptions:")]
		AVMutableComposition FromOptions ([NullAllowed] NSDictionary<NSString,NSObject> urlAssetInitializationOptions);

		[Export ("insertTimeRange:ofAsset:atTime:error:")]
		bool Insert (CMTimeRange insertTimeRange, AVAsset sourceAsset, CMTime atTime, out NSError error);

		[Export ("insertEmptyTimeRange:")]
		void InserEmptyTimeRange (CMTimeRange timeRange);

		[Export ("removeTimeRange:")]
		void RemoveTimeRange (CMTimeRange timeRange);

		[Export ("scaleTimeRange:toDuration:")]
		void ScaleTimeRange (CMTimeRange timeRange, CMTime duration);

		[Export ("addMutableTrackWithMediaType:preferredTrackID:")]
		AVMutableCompositionTrack AddMutableTrack (string mediaType, int /* CMPersistentTrackID = int32_t */ preferredTrackId);

		[Export ("removeTrack:")]
		void RemoveTrack (AVCompositionTrack track);

		[Export ("mutableTrackCompatibleWithTrack:")]
		AVMutableCompositionTrack CreateMutableTrack (AVAssetTrack referenceTrack);

		[Export ("naturalSize")]
		[Override]
		CGSize NaturalSize { get; set; }
	}

	[NoWatch]
	[iOS (9,0), Mac (10,11)]
	[Category]
	[BaseType (typeof (AVMutableComposition))]
	interface AVMutableComposition_AVMutableCompositionTrackInspection {
		
		[Export ("trackWithTrackID:")]
		[return: NullAllowed]
		AVMutableCompositionTrack GetTrack (int trackID);

		[Export ("tracksWithMediaType:")]
		AVMutableCompositionTrack[] GetTracks (string mediaType);

		[Wrap ("This.GetTracks (mediaType.GetConstant ())")]
		AVMutableCompositionTrack[] GetTracks (AVMediaTypes mediaType);

		[Export ("tracksWithMediaCharacteristic:")]
		AVMutableCompositionTrack[] GetTracksWithMediaCharacteristic (string mediaCharacteristic);

		[Wrap ("This.GetTracksWithMediaCharacteristic (mediaCharacteristic.GetConstant ())")]
		AVMutableCompositionTrack[] GetTracks (AVMediaCharacteristics mediaCharacteristic);
	}

	[NoWatch]
	[BaseType (typeof (AVAssetTrackSegment))]
	interface AVCompositionTrackSegment {
		[Export ("sourceURL"), NullAllowed]
		NSUrl SourceUrl { get;  }

		[Export ("sourceTrackID")]
		int SourceTrackID { get;  } /* CMPersistentTrackID = int32_t */

		[Static]
		[Export ("compositionTrackSegmentWithURL:trackID:sourceTimeRange:targetTimeRange:")]
		IntPtr FromUrl (NSUrl url, int /* CMPersistentTrackID = int32_t */ trackID, CMTimeRange sourceTimeRange, CMTimeRange targetTimeRange);

		[Static]
		[Export ("compositionTrackSegmentWithTimeRange:")]
		IntPtr FromTimeRange (CMTimeRange timeRange);

		[DesignatedInitializer]
		[Export ("initWithURL:trackID:sourceTimeRange:targetTimeRange:")]
		IntPtr Constructor (NSUrl URL, int trackID /* CMPersistentTrackID = int32_t */, CMTimeRange sourceTimeRange, CMTimeRange targetTimeRange);

		[DesignatedInitializer]
		[Export ("initWithTimeRange:")]
		IntPtr Constructor (CMTimeRange timeRange);

		[Export ("empty")]
		bool Empty { [Bind ("isEmpty")] get;  }
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVAssetExportSession {
		[Export ("presetName")]
		string PresetName { get;  }

		[Export ("supportedFileTypes")]
#if XAMCORE_4_0
		string [] SupportedFileTypes { get;  }
#else
		NSObject [] SupportedFileTypes { get;  }
#endif

		[NullAllowed]
		[Export ("outputFileType", ArgumentSemantic.Copy)]
		string OutputFileType { get; set;  }

		[NullAllowed]
		[Export ("outputURL", ArgumentSemantic.Copy)]
		NSUrl OutputUrl { get; set;  }

		[return: NullAllowed]
		[Static, Export ("exportSessionWithAsset:presetName:")]
		AVAssetExportSession FromAsset (AVAsset asset, string presetName);
		
		[Export ("status")]
		AVAssetExportSessionStatus Status { get;  }

		[Export ("progress")]
		float Progress { get;  } // defined as 'float'

		[Export ("maxDuration")]
		CMTime MaxDuration { get;  }

		[Export ("timeRange", ArgumentSemantic.Assign)]
		CMTimeRange TimeRange { get; set;  }

		[Export ("metadata", ArgumentSemantic.Copy), NullAllowed]
		AVMetadataItem [] Metadata { get; set;  }

		[Export ("fileLengthLimit")]
		long FileLengthLimit { get; set;  }

		[Export ("audioMix", ArgumentSemantic.Copy), NullAllowed]
		AVAudioMix AudioMix { get; set;  }

		[NullAllowed, Export ("videoComposition", ArgumentSemantic.Copy)]
		AVVideoComposition VideoComposition { get; set;  }

		[Export ("shouldOptimizeForNetworkUse")]
		bool ShouldOptimizeForNetworkUse { get; set;  }

		[Static, Export ("allExportPresets")]
		string [] AllExportPresets { get; }

		[Static]
		[Export ("exportPresetsCompatibleWithAsset:")]
		string [] ExportPresetsCompatibleWithAsset (AVAsset asset);

		[DesignatedInitializer]
		[Export ("initWithAsset:presetName:")]
		IntPtr Constructor (AVAsset asset, string presetName);

		[Wrap ("this (asset, preset.GetConstant ())")]
		IntPtr Constructor (AVAsset asset, AVAssetExportSessionPreset preset);

		[Export ("exportAsynchronouslyWithCompletionHandler:")]
		[Async ("ExportTaskAsync")]
#if XAMCORE_2_0
		void ExportAsynchronously (Action handler);
#else
		void ExportAsynchronously (AVCompletionHandler handler);
#endif

		[Export ("cancelExport")]
		void CancelExport ();

		[Export ("error"), NullAllowed]
		NSError Error { get; }

		[Mac(10,11)]
		[Field ("AVAssetExportPresetLowQuality")]
		NSString PresetLowQuality { get; }

		[Mac(10,11)]
		[Field ("AVAssetExportPresetMediumQuality")]
		NSString PresetMediumQuality { get; }

		[Mac(10,11)]
		[Field ("AVAssetExportPresetHighestQuality")]
		NSString PresetHighestQuality { get; }

		[iOS (11, 0), Mac (10,13)]
		[TV (11, 0)]
		[Field ("AVAssetExportPresetHEVCHighestQuality")]
		NSString PresetHevcHighestQuality { get; }

		[iOS (11, 0), Mac (10,13)]
		[TV (11, 0)]
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

		[iOS (9,0)]
		[Mac (10,10)]
		[Field ("AVAssetExportPreset3840x2160")]
		NSString Preset3840x2160 { get; }

		[iOS (11, 0), Mac (10,13)]
		[TV (11, 0)]
		[Field ("AVAssetExportPresetHEVC1920x1080")]
		NSString PresetHevc1920x1080 { get; }

		[Field ("AVAssetExportPresetAppleM4A")]
		NSString PresetAppleM4A { get; }

		[Field ("AVAssetExportPresetPassthrough")]
		NSString PresetPassthrough { get; }

		// 5.0 APIs
		[Export ("asset", ArgumentSemantic.Retain)]
		AVAsset Asset { get; }

		[Export ("estimatedOutputFileLength")]
		long EstimatedOutputFileLength { get; }

		[iOS (6,0)]
		[Mac (10, 9)]
		[Static, Export ("determineCompatibilityOfExportPreset:withAsset:outputFileType:completionHandler:")]
		[Async]
		void DetermineCompatibilityOfExportPreset (string presetName, AVAsset asset, [NullAllowed] string outputFileType, Action<bool> isCompatibleResult);

		[Async]
		[Wrap ("DetermineCompatibilityOfExportPreset (presetName, asset, outputFileType.GetConstant (), isCompatibleResult)")]
		void DetermineCompatibilityOfExportPreset (string presetName, AVAsset asset, [NullAllowed] AVFileTypes outputFileType, Action<bool> isCompatibleResult);

		[iOS (6,0)]
		[Mac (10, 9)]
		[Export ("determineCompatibleFileTypesWithCompletionHandler:")]
		[Async]
		void DetermineCompatibleFileTypes (Action<string []> compatibleFileTypesHandler);

		[iOS (7,0), Mac (10,9)]
		[Export ("metadataItemFilter", ArgumentSemantic.Retain), NullAllowed]
		AVMetadataItemFilter MetadataItemFilter { get; set; }

		[Mac (10,9)]
		[iOS (7,0)]
		[Export ("customVideoCompositor", ArgumentSemantic.Copy)]
		[Protocolize]
		AVVideoCompositing CustomVideoCompositor { get; }

		// DOC: Use the values from AVAudioTimePitchAlgorithm class.
		[Mac (10,9)]
		[iOS (7, 0), Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		NSString AudioTimePitchAlgorithm { get; set; }

		[iOS (8,0), Mac (10,10)]
		[Export ("canPerformMultiplePassesOverSourceMediaData")]
		[Advice ("This property cannot be set after the export has started.")]
		bool CanPerformMultiplePassesOverSourceMediaData { get; set; }

		[iOS (8,0), Mac (10,10)]
		[Export ("directoryForTemporaryFiles", ArgumentSemantic.Copy), NullAllowed]
		[Advice ("This property cannot be set after the export has started.")]
		NSUrl DirectoryForTemporaryFiles { get; set; }
	}

	[NoWatch]
	[iOS (7,0)]
	[Static]
	interface AVAudioTimePitchAlgorithm {
		[NoMac]
		[Field ("AVAudioTimePitchAlgorithmLowQualityZeroLatency")]
		NSString LowQualityZeroLatency { get; }
		
		[Mac (10,9)]
		[Field ("AVAudioTimePitchAlgorithmTimeDomain")]
		NSString TimeDomain { get; }
		
		[Mac (10,9)]
		[Field ("AVAudioTimePitchAlgorithmSpectral")]
		NSString Spectral { get; }
		
		[Mac (10,9)]
		[Field ("AVAudioTimePitchAlgorithmVarispeed")]
		NSString Varispeed { get; }
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface AVAudioMix : NSMutableCopying {
		[Export ("inputParameters", ArgumentSemantic.Copy)]
		AVAudioMixInputParameters [] InputParameters { get;  }
	}

	[NoWatch]
	[BaseType (typeof (AVAudioMix))]
	interface AVMutableAudioMix {
		[NullAllowed] // by default this property is null
		[Export ("inputParameters", ArgumentSemantic.Copy)]
		AVAudioMixInputParameters [] InputParameters { get; set;  }

		[Static, Export ("audioMix")]
		AVMutableAudioMix Create ();
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface AVAudioMixInputParameters : NSMutableCopying {
		[Export ("trackID")]
		int TrackID { get;  } // defined as 'CMPersistentTrackID' = int32_t

		[Export ("getVolumeRampForTime:startVolume:endVolume:timeRange:")]
		bool GetVolumeRamp (CMTime forTime, ref float /* defined as 'float*' */ startVolume, ref float /* defined as 'float*' */ endVolume, ref CMTimeRange timeRange);

		[iOS (6,0), Mac (10,9)]
		[Export ("audioTapProcessor", ArgumentSemantic.Retain)]
		MTAudioProcessingTap AudioTapProcessor { get; [NotImplemented] set;}

		[iOS (7,0), Mac (10,11)]
		[NullAllowed] // by default this property is null
		[Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		NSString AudioTimePitchAlgorithm { get; [NotImplemented] set; }
	}

	[NoWatch]
	[BaseType (typeof (AVAudioMixInputParameters))]
	interface AVMutableAudioMixInputParameters {
		[Export ("trackID")]
		int TrackID { get; set;  } // defined as 'CMPersistentTrackID'

		[Static]
		[Export ("audioMixInputParametersWithTrack:")]
		AVMutableAudioMixInputParameters FromTrack ([NullAllowed] AVAssetTrack track);

		[Static]
		[return: NullAllowed]
		[Export ("audioMixInputParameters")]
		AVMutableAudioMixInputParameters Create ();
		
		[Export ("setVolumeRampFromStartVolume:toEndVolume:timeRange:")]
		void SetVolumeRamp (float /* defined as 'float' */ startVolume, float /* defined as 'float' */ endVolume, CMTimeRange timeRange);

		[Export ("setVolume:atTime:")]
		void SetVolume (float /* defined as 'float' */ volume, CMTime atTime);

		[iOS (6,0), Mac (10,9)]
		[NullAllowed] // by default this property is null
		[Export ("audioTapProcessor", ArgumentSemantic.Retain)]
		[Override]
		MTAudioProcessingTap AudioTapProcessor { get; set; }

		[iOS (7,0), Mac (10,10)]
		[NullAllowed] // by default this property is null
		[Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		[Override]
		NSString AudioTimePitchAlgorithm { get; set; }
	}

	[NoWatch]
	[iOS (7,0)]
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
		
		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Export ("supportsWideColorSourceFrames")]
		bool SupportsWideColorSourceFrames { get; }
	}
	
	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface AVVideoComposition : NSMutableCopying {
		[Export ("frameDuration")]
		CMTime FrameDuration { get;  }

		[Export ("renderSize")]
		CGSize RenderSize { get;  }

		[Export ("instructions", ArgumentSemantic.Copy)]
		AVVideoCompositionInstruction [] Instructions { get;  }

		[Export ("animationTool", ArgumentSemantic.Retain), NullAllowed]
		AVVideoCompositionCoreAnimationTool AnimationTool { get;  }

		[Mac (10,14, onlyOn64: true)]
		[Export ("renderScale")]
		float RenderScale { get; [NotImplemented] set; } // defined as 'float'

		[Export ("isValidForAsset:timeRange:validationDelegate:")]
		bool IsValidForAsset ([NullAllowed] AVAsset asset, CMTimeRange timeRange, [Protocolize] [NullAllowed] AVVideoCompositionValidationHandling validationDelegate);

		[iOS (6,0), Mac (10, 9)]
		[Static, Export ("videoCompositionWithPropertiesOfAsset:")]
		AVVideoComposition FromAssetProperties (AVAsset asset);

		[iOS (7,0), Mac (10, 9)]
		[Export ("customVideoCompositorClass", ArgumentSemantic.Copy), NullAllowed]
		Class CustomVideoCompositorClass { get; [NotImplemented] set; }

		[iOS (9,0), Mac (10,11)]
		[Static]
		[Export ("videoCompositionWithAsset:applyingCIFiltersWithHandler:")]
		AVVideoComposition CreateVideoComposition (AVAsset asset, Action<AVAsynchronousCIImageFilteringRequest> applier);
		
		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[NullAllowed, Export ("colorPrimaries")]
		string ColorPrimaries { get; }

		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[NullAllowed, Export ("colorYCbCrMatrix")]
		string ColorYCbCrMatrix { get; }

		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[NullAllowed, Export ("colorTransferFunction")]
		string ColorTransferFunction { get; }
	}

	[NoWatch]
	[iOS (7,0), Mac (10, 9)]
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

		[return: NullAllowed]
		[Export ("newPixelBuffer")]
		CVPixelBuffer CreatePixelBuffer ();
	}
	
	[NoWatch]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[DisableDefaultCtor]
	interface AVVideoCompositionValidationHandling {
		[Export ("videoComposition:shouldContinueValidatingAfterFindingInvalidValueForKey:")]
		bool ShouldContinueValidatingAfterFindingInvalidValueForKey (AVVideoComposition videoComposition, string key);

		[Export ("videoComposition:shouldContinueValidatingAfterFindingEmptyTimeRange:")]
		bool ShouldContinueValidatingAfterFindingEmptyTimeRange (AVVideoComposition videoComposition, CMTimeRange timeRange);

		[Export ("videoComposition:shouldContinueValidatingAfterFindingInvalidTimeRangeInInstruction:")]
		bool ShouldContinueValidatingAfterFindingInvalidTimeRangeInInstruction (AVVideoComposition videoComposition, AVVideoCompositionInstruction videoCompositionInstruction);

		[Export ("videoComposition:shouldContinueValidatingAfterFindingInvalidTrackIDInInstruction:layerInstruction:asset:")]
		bool ShouldContinueValidatingAfterFindingInvalidTrackIDInInstruction (AVVideoComposition videoComposition, AVVideoCompositionInstruction videoCompositionInstruction, AVVideoCompositionLayerInstruction layerInstruction, AVAsset asset);
	}

	[NoWatch]
	[BaseType (typeof (AVVideoComposition))]
	interface AVMutableVideoComposition {
		[Export ("frameDuration", ArgumentSemantic.Assign)]
		CMTime FrameDuration { get; set;  }

		[Export ("renderSize", ArgumentSemantic.Assign)]
		CGSize RenderSize { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("instructions", ArgumentSemantic.Copy)]
		AVVideoCompositionInstruction [] Instructions { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("animationTool", ArgumentSemantic.Retain)]
		AVVideoCompositionCoreAnimationTool AnimationTool { get; set;  }

		[Mac (10,14, onlyOn64: true)]
		[Export ("renderScale")]
		float RenderScale { get; set; } // defined as 'float'

		[Static, Export ("videoComposition")]
		AVMutableVideoComposition Create ();

		// in 7.0 they declared this was available in 6.0
		[iOS (6,0), Mac (10, 9)]
		[Static, Export ("videoCompositionWithPropertiesOfAsset:")]
		AVMutableVideoComposition Create (AVAsset asset);

		[NullAllowed]
		[iOS (7,0), Mac (10, 9)]
		[Export ("customVideoCompositorClass", ArgumentSemantic.Retain)]
		[Override]
		Class CustomVideoCompositorClass { get; set; }

		[iOS (9,0), Mac (10,11)]
		[Static]
		[Export ("videoCompositionWithAsset:applyingCIFiltersWithHandler:")]
		AVMutableVideoComposition GetVideoComposition (AVAsset asset, Action<AVAsynchronousCIImageFilteringRequest> applier);
		
		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[NullAllowed, Export ("colorPrimaries")]
		string ColorPrimaries { get; set; }

		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[NullAllowed, Export ("colorYCbCrMatrix")]
		string ColorYCbCrMatrix { get; set; }

		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[NullAllowed, Export ("colorTransferFunction")]
		string ColorTransferFunction { get; set; }

		[Mac (10, 13), iOS (11, 0), TV (11, 0)]
		[Export ("sourceTrackIDForFrameTiming")]
		int SourceTrackIdForFrameTiming { get; set; }
	}

	[NoWatch]
	[Mac (10, 7)]
	[BaseType (typeof (NSObject))]
	interface AVVideoCompositionInstruction : NSSecureCoding, NSMutableCopying {
		[Export ("timeRange")]
		CMTimeRange TimeRange { get;  [NotImplemented ("Not available on AVVideoCompositionInstruction, only available on AVMutableVideoCompositionInstruction")]set; }

		[NullAllowed]
		[Export ("backgroundColor", ArgumentSemantic.Retain)]
		CGColor BackgroundColor { get;
			[NotImplemented] set;
		}

		[Export ("layerInstructions", ArgumentSemantic.Copy)]
		AVVideoCompositionLayerInstruction [] LayerInstructions { get; [NotImplemented ("Not available on AVVideoCompositionInstruction, only available on AVMutableVideoCompositionInstruction")]set; }

		[Export ("enablePostProcessing")]
		bool EnablePostProcessing { get; [NotImplemented ("Not available on AVVideoCompositionInstruction, only available on AVMutableVideoCompositionInstruction")]set; }

		// These are there because it adopts the protocol *of the same name*

		[iOS (7,0), Mac (10, 9)]
		[Export ("containsTweening")]
		bool ContainsTweening { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("requiredSourceTrackIDs")]
		NSNumber[] RequiredSourceTrackIDs { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("passthroughTrackID")]
		int PassthroughTrackID { get; } /* CMPersistentTrackID = int32_t */
	}

	[NoWatch]
	[BaseType (typeof (AVVideoCompositionInstruction))]
	interface AVMutableVideoCompositionInstruction {
		[Export ("timeRange", ArgumentSemantic.Assign)]
		[Override]
		CMTimeRange TimeRange { get; set;  }

		[NullAllowed]
		[Export ("backgroundColor", ArgumentSemantic.Retain)]
		[Override]
		CGColor BackgroundColor { get; set;  }

		[Export ("enablePostProcessing", ArgumentSemantic.Assign)]
		[Override]
		bool EnablePostProcessing { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("layerInstructions", ArgumentSemantic.Copy)]
		[Override]
		AVVideoCompositionLayerInstruction [] LayerInstructions { get; set;  }

		[Static, Export ("videoCompositionInstruction")]
		AVVideoCompositionInstruction Create (); 		
	}

	[NoWatch]
	[Mac (10, 7)]
	[BaseType (typeof (NSObject))]
	interface AVVideoCompositionLayerInstruction : NSSecureCoding, NSMutableCopying {
		[Export ("trackID", ArgumentSemantic.Assign)]
		int TrackID { get;  } // defined as 'CMPersistentTrackID' = int32_t

		[Export ("getTransformRampForTime:startTransform:endTransform:timeRange:")]
		bool GetTransformRamp (CMTime time, ref CGAffineTransform startTransform, ref CGAffineTransform endTransform, ref CMTimeRange timeRange);

		[Export ("getOpacityRampForTime:startOpacity:endOpacity:timeRange:")]
		bool GetOpacityRamp (CMTime time, ref float /* defined as 'float*' */ startOpacity, ref float /* defined as 'float*' */ endOpacity, ref CMTimeRange timeRange);

		[iOS (7,0), Mac (10, 9), Export ("getCropRectangleRampForTime:startCropRectangle:endCropRectangle:timeRange:")]
		bool GetCrop (CMTime time, ref CGRect startCropRectangle, ref CGRect endCropRectangle, ref CMTimeRange timeRange);
	}

	[NoWatch]
	[BaseType (typeof (AVVideoCompositionLayerInstruction))]
	interface AVMutableVideoCompositionLayerInstruction {
		[Export ("trackID", ArgumentSemantic.Assign)]
		int TrackID { get; set;  } // defined as 'CMPersistentTrackID' = int32w_t

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

		[iOS (7,0), Mac (10, 9)]
		[Export ("setCropRectangleRampFromStartCropRectangle:toEndCropRectangle:timeRange:")]
		void SetCrop (CGRect startCropRectangle, CGRect endCropRectangle, CMTimeRange timeRange);

		[iOS (7,0), Mac (10, 9)]
		[Export ("setCropRectangle:atTime:")]
		void SetCrop (CGRect cropRectangle, CMTime time);
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface AVVideoCompositionCoreAnimationTool {
		[Static]
		[Export ("videoCompositionCoreAnimationToolWithAdditionalLayer:asTrackID:")]
		AVVideoCompositionCoreAnimationTool FromLayer (CALayer layer, int /* CMPersistentTrackID = int32_t */ trackID);

		[Static]
		[Export ("videoCompositionCoreAnimationToolWithPostProcessingAsVideoLayer:inLayer:")]
		AVVideoCompositionCoreAnimationTool FromLayer (CALayer videoLayer, CALayer animationLayer);

		[iOS (7,0), Mac (10, 9)]
		[Static, Export ("videoCompositionCoreAnimationToolWithPostProcessingAsVideoLayers:inLayer:")]
		AVVideoCompositionCoreAnimationTool FromComposedVideoFrames (CALayer [] videoLayers, CALayer inAnimationlayer);
	}

	[TV (11,0), NoWatch, iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVCameraCalibrationData
	{
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
	interface AVCaptureSessionRuntimeErrorEventArgs {
		[Export ("AVCaptureSessionErrorKey")]
		NSError Error { get; }
	}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface AVCaptureSession {

		[Export ("sessionPreset", ArgumentSemantic.Copy)]
		NSString SessionPreset { get; set;  }

		[Export ("inputs")]
		AVCaptureInput [] Inputs { get;  }

		[Export ("outputs")]
		AVCaptureOutput [] Outputs { get;  }

		[Export ("running")]
		bool Running { [Bind ("isRunning")] get;  }
#if !MONOMAC
		[Export ("interrupted")]
		bool Interrupted { [Bind ("isInterrupted")] get;  }
#endif
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
#if !MONOMAC
		[Field ("AVCaptureSessionPreset1920x1080")]
		NSString Preset1920x1080 { get; }

		[iOS (9,0)]
		[Field ("AVCaptureSessionPreset3840x2160")]
		NSString Preset3840x2160 { get; }
#endif

		[Field ("AVCaptureSessionPresetiFrame960x540")]
		NSString PresetiFrame960x540 { get; }

		[Field ("AVCaptureSessionPresetiFrame1280x720")]
		NSString PresetiFrame1280x720 { get; }

		[Field ("AVCaptureSessionPreset352x288")]
		NSString Preset352x288 { get; }

#if !MONOMAC
		[iOS (7,0)]
		[Field ("AVCaptureSessionPresetInputPriority")]
		NSString PresetInputPriority { get; }
#endif
		[NoiOS]
		[Mac (10,7)]
		[Field ("AVCaptureSessionPreset320x240")]
		NSString Preset320x240 { get; }

		[NoiOS]
		[Mac (10, 7)]
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

		[Mac (10, 14, onlyOn64: true)]
		[Field ("AVCaptureSessionInterruptionEndedNotification")]
		[Notification]
		NSString InterruptionEndedNotification { get; }

		[Mac (10, 14, onlyOn64: true)]
		[Field ("AVCaptureSessionWasInterruptedNotification")]
		[Notification]
		NSString WasInterruptedNotification { get; }

#if !MONOMAC
		[iOS (9,0)]
		[Field ("AVCaptureSessionInterruptionReasonKey")]
		NSString InterruptionReasonKey { get; }

		[iOS (7,0)]
		[Export ("usesApplicationAudioSession")]
		bool UsesApplicationAudioSession { get; set; }

		[iOS (7,0)]
		[Export ("automaticallyConfiguresApplicationAudioSession")]
		bool AutomaticallyConfiguresApplicationAudioSession { get; set; }
		
		[iOS (10, 0)]
		[Export ("automaticallyConfiguresCaptureDeviceForWideColor")]
		bool AutomaticallyConfiguresCaptureDeviceForWideColor { get; set; }
#endif

		[iOS (11, 1), NoMac]
		[Field ("AVCaptureSessionInterruptionSystemPressureStateKey")]
		NSString InterruptionSystemPressureStateKey { get; }

		[iOS (7,0)]
		[Export ("masterClock")]
		CMClock MasterClock { get; }

		//
		// iOS 8
		//
		[iOS (8,0)]
		[Export ("addInputWithNoConnections:")]
		void AddInputWithNoConnections (AVCaptureInput input);

		[iOS (8,0)]
		[Export ("addOutputWithNoConnections:")]
		void AddOutputWithNoConnections (AVCaptureOutput output);

		[iOS (8,0)]
		[Export ("canAddConnection:")]
		bool CanAddConnection (AVCaptureConnection connection);

		[iOS (8,0)]
		[Export ("addConnection:")]
		void AddConnection (AVCaptureConnection connection);

		[iOS (8,0)]
		[Export ("removeConnection:")]
		void RemoveConnection (AVCaptureConnection connection);
	}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface AVCaptureConnection {
		
		[iOS (8,0), Mac (10,7)]
		[Static]
		[Export ("connectionWithInputPorts:output:")]
		AVCaptureConnection FromInputPorts (AVCaptureInputPort [] ports, AVCaptureOutput output);
		
		[iOS (8,0)]
		[Static]
		[Export ("connectionWithInputPort:videoPreviewLayer:")]
		AVCaptureConnection FromInputPort (AVCaptureInputPort port, AVCaptureVideoPreviewLayer layer);
		
		[iOS (8,0), Mac (10,7)]
		[Export ("initWithInputPorts:output:")]
		IntPtr Constructor (AVCaptureInputPort [] inputPorts, AVCaptureOutput output);

		[iOS (8,0), Mac (10,7)]
		[Export ("initWithInputPort:videoPreviewLayer:")]
		IntPtr Constructor (AVCaptureInputPort inputPort, AVCaptureVideoPreviewLayer layer);

		[Export ("output")]
		AVCaptureOutput Output { get;  }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set;  }

		[Export ("audioChannels")]
		AVCaptureAudioChannel [] AvailableAudioChannels { get;  }

		[Export ("videoMirrored")]
		bool VideoMirrored { [Bind ("isVideoMirrored")] get; set;  }

		[Export ("videoOrientation", ArgumentSemantic.Assign)]
		AVCaptureVideoOrientation VideoOrientation { get; set;  }

		[Export ("inputPorts")]
		AVCaptureInputPort [] InputPorts { get; }

		[Export ("isActive")]
		bool Active { get; }

		[Export ("isVideoMirroringSupported")]
		bool SupportsVideoMirroring { get; }

		[Export ("isVideoOrientationSupported")]
		bool SupportsVideoOrientation { get; }

		[Export ("supportsVideoMinFrameDuration"), Internal]
		bool _SupportsVideoMinFrameDuration { [Bind ("isVideoMinFrameDurationSupported")] get;  }

		[Deprecated (PlatformName.iOS, 7, 0 /* Only deprecated on iOS */)]
		[Mac (10, 7)]
		[Export ("videoMinFrameDuration")]
		CMTime VideoMinFrameDuration { get; set;  }
#if !MONOMAC
		[Export ("supportsVideoMaxFrameDuration"), Internal]
		bool _SupportsVideoMaxFrameDuration { [Bind ("isVideoMaxFrameDurationSupported")] get;  }

		[Export ("videoMaxFrameDuration")]
		[Deprecated (PlatformName.iOS, 7, 0 /* Only deprecated on iOS */)] 
		[Mac (10, 7)] 
		CMTime VideoMaxFrameDuration { get; set;  }

		[Export ("videoMaxScaleAndCropFactor")]
		nfloat VideoMaxScaleAndCropFactor { get;  }

		[Export ("videoScaleAndCropFactor")]
		nfloat VideoScaleAndCropFactor { get; set;  }
#endif
		[iOS (6,0)]
		[Export ("videoPreviewLayer")]
		AVCaptureVideoPreviewLayer VideoPreviewLayer { get;  }

		[iOS (6,0)]
		[Export ("automaticallyAdjustsVideoMirroring")]
		bool AutomaticallyAdjustsVideoMirroring { get; set;  }
#if !MONOMAC
		[iOS (6,0)]
		[Export ("supportsVideoStabilization")]
		bool SupportsVideoStabilization { [Bind ("isVideoStabilizationSupported")] get;  }

		[iOS (6,0)]
		[Export ("videoStabilizationEnabled")]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'ActiveVideoStabilizationMode' instead.")]
		bool VideoStabilizationEnabled { [Bind ("isVideoStabilizationEnabled")] get;  }

		[iOS (6,0)]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'PreferredVideoStabilizationMode' instead.")]
		[Export ("enablesVideoStabilizationWhenAvailable")]
		bool EnablesVideoStabilizationWhenAvailable { get; set;  }

		[iOS (8,0)]
		[Export ("preferredVideoStabilizationMode")]
		AVCaptureVideoStabilizationMode PreferredVideoStabilizationMode { get; set; }

		[iOS (8,0)]
		[Export ("activeVideoStabilizationMode")]
		AVCaptureVideoStabilizationMode ActiveVideoStabilizationMode { get; }
#endif
		[NoiOS]
		[Export ("supportsVideoFieldMode")]
		bool SupportsVideoFieldMode { [Bind ("isVideoFieldModeSupported")] get; }

#if MONOMAC
		[Export ("videoFieldMode")]
		AVVideoFieldMode VideoFieldMode { get; set; }
#endif

		[iOS (11, 0), NoMac, TV (11, 0), NoWatch]
		[Export ("cameraIntrinsicMatrixDeliverySupported")]
		bool CameraIntrinsicMatrixDeliverySupported { [Bind ("isCameraIntrinsicMatrixDeliverySupported")] get; }

		[iOS (11, 0), NoMac, TV (11, 0), NoWatch]
		[Export ("cameraIntrinsicMatrixDeliveryEnabled")]
		bool CameraIntrinsicMatrixDeliveryEnabled { [Bind ("isCameraIntrinsicMatrixDeliveryEnabled")] get; set; }

	}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface AVCaptureAudioChannel {
		[Export ("peakHoldLevel")]
		float PeakHoldLevel { get;  } // defined as 'float'

		[Export ("averagePowerLevel")]
		float AveragePowerLevel { get; } // defined as 'float'

		[NoiOS]
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NoiOS]
		[Export ("volume")]
		float Volume { get; set; } /* float intended here */
	}

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

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureInputPort {
		[Export ("mediaType")]
		string MediaType { get;  }

		[Export ("formatDescription")]
		CMFormatDescription FormatDescription { get;  }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set;  }

		[Export ("input")]
		AVCaptureInput Input  { get; }

		[iOS (7,0), Mac (10, 9), Export ("clock", ArgumentSemantic.Copy)]
		CMClock Clock { get; }
	}

	interface IAVCaptureDepthDataOutputDelegate {}
	
	[NoWatch, NoTV, iOS (11,0), NoMac]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface AVCaptureDepthDataOutputDelegate
	{
		[Export ("depthDataOutput:didOutputDepthData:timestamp:connection:")]
		void DidOutputDepthData (AVCaptureDepthDataOutput output, AVDepthData depthData, CMTime timestamp, AVCaptureConnection connection);

		[Export ("depthDataOutput:didDropDepthData:timestamp:connection:reason:")]
		void DidDropDepthData (AVCaptureDepthDataOutput output, AVDepthData depthData, CMTime timestamp, AVCaptureConnection connection, AVCaptureOutputDataDroppedReason reason);
	}

	[NoWatch, NoTV, iOS (11,0), NoMac]
	[BaseType (typeof(AVCaptureOutput))]
	interface AVCaptureDepthDataOutput
	{
		[Export ("setDelegate:callbackQueue:")]
		void SetDelegate (IAVCaptureDepthDataOutputDelegate del, DispatchQueue callbackQueue);

		[Wrap ("WeakDelegate")]
		IAVCaptureDepthDataOutputDelegate Delegate { get; }

		[NullAllowed, Export ("delegate")]
		NSObject WeakDelegate { get; }

		[Export ("delegateCallbackQueue")]
		DispatchQueue DelegateCallbackQueue { get; }

		[Export ("alwaysDiscardsLateDepthData")]
		bool AlwaysDiscardsLateDepthData { get; set; }

		[Export ("filteringEnabled")]
		bool FilteringEnabled { [Bind ("isFilteringEnabled")] get; set; }
	}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (AVCaptureInput))]
	// crash application if 'init' is called
	[DisableDefaultCtor]
	interface AVCaptureDeviceInput {
		[Export ("device")]
		AVCaptureDevice Device { get;  }

		[Static, Export ("deviceInputWithDevice:error:")]
		AVCaptureDeviceInput FromDevice (AVCaptureDevice device, out NSError error);

		[Export ("initWithDevice:error:")]
		IntPtr Constructor (AVCaptureDevice device, out NSError error);

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("unifiedAutoExposureDefaultsEnabled")]
		bool UnifiedAutoExposureDefaultsEnabled { get; set; }
	}

#if MONOMAC
	[NoWatch]
	[NoTV]
	[Mac (10,7)]
	[BaseType (typeof (NSObject))]
	interface AVCaptureDeviceInputSource {
		[Export ("inputSourceID")]
		string InputSourceID { get; }

		[Export ("localizedName")]
		string LocalizedName { get; }
	}

	[NoWatch]
	[Mac (10,7)]
	[BaseType (typeof (AVCaptureFileOutput))]
	[NoTV]
	interface AVCaptureAudioFileOutput {
		[Export ("metadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] Metadata { get; set; }

		[Export ("audioSettings", ArgumentSemantic.Copy)]
		NSDictionary WeakAudioSettings { get; set; }

		[Wrap ("WeakAudioSettings")]
		AudioSettings AudioSettings { get; set; }

		[Static, Export ("availableOutputFileTypes")]
		NSString [] AvailableOutputFileTypes ();

		[Export ("startRecordingToOutputFileURL:outputFileType:recordingDelegate:")]
		void StartRecording (NSUrl outputFileUrl, string fileType, [Protocolize] AVCaptureFileOutputRecordingDelegate recordingDelegate);
	}

	[NoWatch]
	[NoTV]
	[Mac (10,7)]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureAudioPreviewOutput {
		[Export ("outputDeviceUniqueID", ArgumentSemantic.Copy), NullAllowed]
		NSString OutputDeviceUniqueID { get; set; }

		[Export ("volume")]
		float Volume { get; set; } /* float, not CGFloat */
	}

	[Static]
	interface AVAssetExportPresetApple {

		[Field ("AVAssetExportPresetAppleM4VCellular")]
		NSString M4VCellular { get; }

		[Field ("AVAssetExportPresetAppleM4ViPod")]
		NSString M4ViPod { get; }

		[Field ("AVAssetExportPresetAppleM4V480pSD")]
		NSString M4V480pSD { get; }

		[Field ("AVAssetExportPresetAppleM4VAppleTV")]
		NSString M4VAppleTV { get; }

		[Field ("AVAssetExportPresetAppleM4VWiFi")]
		NSString M4VWiFi { get; }

		[Field ("AVAssetExportPresetAppleM4V720pHD")]
		NSString M4V720pHD { get; }

		[Field ("AVAssetExportPresetAppleM4V1080pHD")]
		NSString M4V1080pHD { get; }

		[Field ("AVAssetExportPresetAppleProRes422LPCM")]
		NSString ProRes422Lpcm { get; }
	}

#endif

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
#if XAMCORE_4_0
	[Abstract] // as per docs
#endif
	// Objective-C exception thrown.  Name: NSGenericException Reason: Cannot instantiate AVCaptureOutput because it is an abstract superclass.
	[DisableDefaultCtor]
	interface AVCaptureOutput {
		[Export ("connections")]
		AVCaptureConnection [] Connections { get; }

		[Export ("connectionWithMediaType:")]
		AVCaptureConnection ConnectionFromMediaType (NSString avMediaType);

#if !MONOMAC
		[iOS (7,0)]
		[Export ("metadataOutputRectOfInterestForRect:")]
		CGRect GetMetadataOutputRectOfInterestForRect (CGRect rectInOutputCoordinates);

		[iOS (7,0)]
		[Export ("rectForMetadataOutputRectOfInterest:")]
		CGRect GetRectForMetadataOutputRectOfInterest (CGRect rectInMetadataOutputCoordinates);

		[iOS (6,0)]
		[Export ("transformedMetadataObjectForMetadataObject:connection:")]
		AVMetadataObject GetTransformedMetadataObject (AVMetadataObject metadataObject, AVCaptureConnection connection);
#endif
	}

#if MONOMAC
	[NoWatch]
	[NoTV]
	[Mac (10,7)]
	[BaseType (typeof (AVCaptureInput))]
	interface AVCaptureScreenInput {
		[Export ("initWithDisplayID:")]
		IntPtr Constructor (uint /* CGDirectDisplayID = uint32_t */ displayID);

		[Export ("minFrameDuration")]
		CMTime MinFrameDuration { get; set; }

		[Export ("cropRect")]
		CGRect CropRect { get; set; }

		[Export ("scaleFactor")]
		nfloat ScaleFactor { get; set; }

		[Export ("capturesMouseClicks")]
		bool CapturesMouseClicks { get; set; }

		[Mac (10,8)]
		[Export ("capturesCursor")]
		bool CapturesCursor { get; set; }

		[Mac (10,8)]
		[Availability (Deprecated=Platform.Mac_10_10, Message="Ignored since 10.10, if you want to get this behavior, use AVCaptureVideoDataOutput and compare the frame contents on your own code.")]
		[Export ("removesDuplicateFrames")]
		bool RemovesDuplicateFrames { get; set; }
	}
#endif

	[NoWatch]
	[NoTV]
	[BaseType (typeof (CALayer))]
	interface AVCaptureVideoPreviewLayer {
		[NullAllowed] // by default this property is null
		[Export ("session", ArgumentSemantic.Retain)]
		AVCaptureSession Session { get; set;  }

		[Mac (10,7), iOS (8,0)]
		[Export ("setSessionWithNoConnection:")]
		void SetSessionWithNoConnection (AVCaptureSession session);

#if !XAMCORE_2_0
		[Advice ("Use 'LayerVideoGravity' property instead.")]
		[Export ("videoGravity", ArgumentSemantic.Copy)][Sealed]
		string VideoGravity { get; set; }
#endif

		[Export ("videoGravity", ArgumentSemantic.Copy)][Protected]
		NSString WeakVideoGravity { get; set; }

#if !MONOMAC
		[Export ("orientation")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'AVCaptureConnection.VideoOrientation' instead.")]
		AVCaptureVideoOrientation Orientation { get; set;  }

		[Export ("automaticallyAdjustsMirroring")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'AVCaptureConnection.AutomaticallyAdjustsVideoMirroring' instead.")]
		bool AutomaticallyAdjustsMirroring { get; set;  }

		[Export ("mirrored")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'AVCaptureConnection.VideoMirrored' instead.")]
		bool Mirrored { [Bind ("isMirrored")] get; set;  }

		[Export ("isMirroringSupported")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'AVCaptureConnection.IsVideoMirroringSupported' instead.")]
		bool MirroringSupported { get; }

		[Export ("isOrientationSupported")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'AVCaptureConnection.IsVideoOrientationSupported' instead.")]
		bool OrientationSupported { get; }

#endif

		[Static, Export ("layerWithSession:")]
		AVCaptureVideoPreviewLayer FromSession (AVCaptureSession session);

		[Export ("initWithSession:")]
		[Internal]
		IntPtr InitWithConnection (AVCaptureSession session);

		[iOS (8,0), Mac (10,7)]
		[Internal]
		[Export ("initWithSessionWithNoConnection:")]
		IntPtr InitWithNoConnection (AVCaptureSession session);

		[iOS (6,0)]
		[Export ("connection")]
		AVCaptureConnection Connection { get; }
#if !MONOMAC
		[iOS (6,0)]
		[Export ("captureDevicePointOfInterestForPoint:")]
		CGPoint CaptureDevicePointOfInterestForPoint (CGPoint pointInLayer);

		[iOS (6,0)]
		[Export ("pointForCaptureDevicePointOfInterest:")]
		CGPoint PointForCaptureDevicePointOfInterest (CGPoint captureDevicePointOfInterest);

		[iOS (6,0)]
		[Export ("transformedMetadataObjectForMetadataObject:")]
		AVMetadataObject GetTransformedMetadataObject (AVMetadataObject metadataObject);

		[iOS (7,0), Export ("metadataOutputRectOfInterestForRect:")]
		CGRect MapToMetadataOutputCoordinates (CGRect rectInLayerCoordinates);
		
		[iOS (7,0), Export ("rectForMetadataOutputRectOfInterest:")]
		CGRect MapToLayerCoordinates (CGRect rectInMetadataOutputCoordinates);
#endif

		[iOS (8,0)]
		[Static]
		[Export ("layerWithSessionWithNoConnection:")]
		AVCaptureVideoPreviewLayer CreateWithNoConnection (AVCaptureSession session);
	}

	[NoTV]
	[NoWatch]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureVideoDataOutput {
		[Export ("sampleBufferDelegate")]
		[Protocolize]
		AVCaptureVideoDataOutputSampleBufferDelegate SampleBufferDelegate { get; }

		[Export ("sampleBufferCallbackQueue")]
		DispatchQueue SampleBufferCallbackQueue { get;  }

		[Export ("videoSettings", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary WeakVideoSettings { get; set;  }

		[Wrap ("WeakVideoSettings")]
		AVVideoSettingsUncompressed UncompressedVideoSetting { get; set; }

		[Wrap ("WeakVideoSettings")]
		AVVideoSettingsCompressed CompressedVideoSetting { get; set; }

		[Export ("minFrameDuration")]
		[Availability (Deprecated = Platform.iOS_5_0, Message = "Use 'AVCaptureConnection.MinVideoFrameDuration' instead.")]
		CMTime MinFrameDuration { get; set;  }

		[Export ("alwaysDiscardsLateVideoFrames")]
		bool AlwaysDiscardsLateVideoFrames { get; set;  }

#if !XAMARIN_4_0
		[Obsolete ("Use overload accepting a 'IAVCaptureVideoDataOutputSampleBufferDelegate'.")]
		[Export ("setSampleBufferDelegate:queue:")]
		[PostGet ("SampleBufferDelegate")]
		[PostGet ("SampleBufferCallbackQueue")]
		void SetSampleBufferDelegate ([NullAllowed] AVCaptureVideoDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackQueue);

		[Sealed]
#endif
		[Export ("setSampleBufferDelegate:queue:")]
		void SetSampleBufferDelegateQueue ([NullAllowed] IAVCaptureVideoDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackQueue);

		// 5.0 APIs
#if XAMCORE_4_0
		[BindAs (typeof (CoreVideo.CVPixelFormatType []))]
#endif
		[Export ("availableVideoCVPixelFormatTypes")]
		NSNumber [] AvailableVideoCVPixelFormatTypes { get;  }

		// This is an NSString, because these are are codec types that can be used as keys in
		// the WeakVideoSettings properties.
		[Export ("availableVideoCodecTypes")]
		NSString [] AvailableVideoCodecTypes { get;  }

#if !MONOMAC
		[iOS (7,0)]
		[Export ("recommendedVideoSettingsForAssetWriterWithOutputFileType:")]
		NSDictionary GetRecommendedVideoSettingsForAssetWriter (string outputFileType);
#endif

		[iOS (11,0), NoMac]
		[Export ("availableVideoCodecTypesForAssetWriterWithOutputFileType:")]
		string[] GetAvailableVideoCodecTypes (string outputFileType);

		[Internal]
		[iOS (11,0), NoMac]
		[Export ("recommendedVideoSettingsForVideoCodecType:assetWriterOutputFileType:")]
		[return: NullAllowed]
		NSDictionary GetWeakRecommendedVideoSettings (string videoCodecType, string outputFileType);

		[iOS (11,0), NoMac]
		[Wrap ("new AVPlayerItemVideoOutputSettings (GetWeakRecommendedVideoSettings (videoCodecType, outputFileType))")]
		AVPlayerItemVideoOutputSettings GetRecommendedVideoSettings (string videoCodecType, string outputFileType);
	}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVCaptureVideoDataOutputSampleBufferDelegate {
		[Export ("captureOutput:didOutputSampleBuffer:fromConnection:")]
		// CMSampleBufferRef		
		void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection);

		[iOS (6,0)]
		[Export ("captureOutput:didDropSampleBuffer:fromConnection:")]
		void DidDropSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection);
	}

	interface IAVCaptureVideoDataOutputSampleBufferDelegate {}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureAudioDataOutput {
		[Export ("sampleBufferDelegate")]
		[Protocolize]
		AVCaptureAudioDataOutputSampleBufferDelegate SampleBufferDelegate { get;  }

		[Export ("sampleBufferCallbackQueue")]
		DispatchQueue SampleBufferCallbackQueue { get;  }

#if XAMCORE_4_0
		[Export ("setSampleBufferDelegate:queue:")]
		void SetSampleBufferDelegate ([NullAllowed] IAVCaptureAudioDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackDispatchQueue);
#else
		[Export ("setSampleBufferDelegate:queue:")]
		[Sealed]
		void SetSampleBufferDelegateQueue ([NullAllowed] IAVCaptureAudioDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackDispatchQueue);

		[Obsolete ("Use overload accepting a 'IAVCaptureVideoDataOutputSampleBufferDelegate'.")]
		[Export ("setSampleBufferDelegate:queue:")]
#if XAMCORE_2_0
		void SetSampleBufferDelegateQueue ([NullAllowed] AVCaptureAudioDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackDispatchQueue);
#else
		void SetSampleBufferDelegatequeue ([NullAllowed] AVCaptureAudioDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackDispatchQueue);
#endif
#endif

#if !MONOMAC
		[iOS (7,0)]
		[Export ("recommendedAudioSettingsForAssetWriterWithOutputFileType:")]
		NSDictionary GetRecommendedAudioSettingsForAssetWriter (string outputFileType);
#endif

		[NoiOS]
		[Export ("audioSettings", ArgumentSemantic.Copy)]
		NSDictionary WeakAudioSettings { get; set; }

		[NoiOS]
		[Wrap ("WeakAudioSettings")]
		AudioSettings AudioSettings { get; set; }
	}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVCaptureAudioDataOutputSampleBufferDelegate {
		[Export ("captureOutput:didOutputSampleBuffer:fromConnection:")]
		void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection);
	}

#if !MONOMAC
	[NoWatch]
	[NoTV]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
#if XAMCORE_2_0
	[Abstract]
#endif
	interface AVCaptureBracketedStillImageSettings {
		// Abstract class in obJC
	}

	[NoWatch]
	[NoTV]
	[iOS (8,0)]
	[BaseType (typeof (AVCaptureBracketedStillImageSettings))]
	interface AVCaptureManualExposureBracketedStillImageSettings {
		[Export ("exposureDuration")]
		CMTime ExposureDuration { get; }

		[Export ("ISO")]
		float ISO { get; } /* float, not CGFloat */

		[Static, Export ("manualExposureSettingsWithExposureDuration:ISO:")]
		AVCaptureManualExposureBracketedStillImageSettings Create (CMTime duration, float /* float, not CGFloat */ ISO);
	}

	[NoWatch]
	[NoTV]
	[iOS (8,0)]
	[BaseType (typeof (AVCaptureBracketedStillImageSettings))]
	interface AVCaptureAutoExposureBracketedStillImageSettings {
		[Export ("exposureTargetBias")]
		float ExposureTargetBias { get; } /* float, not CGFloat */

		[Static, Export ("autoExposureSettingsWithExposureTargetBias:")]
		AVCaptureAutoExposureBracketedStillImageSettings Create (float /* float, not CGFloat */ exposureTargetBias);
	}
#endif
	
	interface IAVCaptureAudioDataOutputSampleBufferDelegate {}

	interface IAVCaptureFileOutputRecordingDelegate {}

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

		[Export ("outputFileURL")]
		NSUrl OutputFileURL { get; } // FIXME: should have been Url.

		[Export ("startRecordingToOutputFileURL:recordingDelegate:")]
		void StartRecordingToOutputFile (NSUrl outputFileUrl, [Protocolize] AVCaptureFileOutputRecordingDelegate recordingDelegate);

		[Export ("stopRecording")]
		void StopRecording ();

		[NoiOS]
		[Export ("pauseRecording")]
		void PauseRecording ();

		[NoiOS]
		[Export ("resumeRecording")]
		void ResumeRecording ();

#if MONOMAC
		[Mac (10,7)]
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		IAVCaptureFileOutputDelegate Delegate { get; set; }

		[Mac (10,7)]
		[Export ("recordingPaused")]
		bool RecordingPaused { [Bind ("isRecordingPaused")] get; }
#endif
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[NoTV]
	[NoWatch]
	interface AVCaptureFileOutputRecordingDelegate {
		[Export ("captureOutput:didStartRecordingToOutputFileAtURL:fromConnections:")]
		void DidStartRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject [] connections);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("captureOutput:didFinishRecordingToOutputFileAtURL:fromConnections:error:"), CheckDisposed]
		void FinishedRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject [] connections, NSError error);

#if MONOMAC
		[Export ("captureOutput:didPauseRecordingToOutputFileAtURL:fromConnections:")]
		void DidPauseRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, AVCaptureConnection [] connections);

		[Export ("captureOutput:didResumeRecordingToOutputFileAtURL:fromConnections:")]
		void DidResumeRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, AVCaptureConnection [] connections);

		[Export ("captureOutput:willFinishRecordingToOutputFileAtURL:fromConnections:error:")]
		void WillFinishRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, AVCaptureConnection [] connections, NSError error);
#endif
	}

#if !MONOMAC
	[NoWatch]
	[NoTV]
	[iOS (6,0)]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureMetadataOutput {
		[Export ("metadataObjectsDelegate")]
		[Protocolize]
		AVCaptureMetadataOutputObjectsDelegate Delegate { get;  }

		[Export ("metadataObjectsCallbackQueue")]
		DispatchQueue CallbackQueue { get;  }

		[Export ("availableMetadataObjectTypes")]
#if XAMCORE_2_0
		NSString [] WeakAvailableMetadataObjectTypes { get;  }
#else
		NSString [] AvailableMetadataObjectTypes { get;  }
#endif

		[NullAllowed]
		[Export ("metadataObjectTypes", ArgumentSemantic.Copy)]
#if XAMCORE_2_0
		NSString [] WeakMetadataObjectTypes { get; set;  }
#else
		NSString [] MetadataObjectTypes { get; set;  }
#endif

		[Export ("setMetadataObjectsDelegate:queue:")]
		void SetDelegate ([NullAllowed][Protocolize] AVCaptureMetadataOutputObjectsDelegate objectsDelegate, [NullAllowed] DispatchQueue objectsCallbackQueue);

		[iOS (7,0)]
		[Export ("rectOfInterest", ArgumentSemantic.Copy)]
		CGRect RectOfInterest { get; set; }
		
	}

	[NoWatch]
	[NoTV]
	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVCaptureMetadataOutputObjectsDelegate {
		[Export ("captureOutput:didOutputMetadataObjects:fromConnection:")]
		void DidOutputMetadataObjects (AVCaptureMetadataOutput captureOutput, AVMetadataObject [] metadataObjects, AVCaptureConnection connection);
	}
#endif

	[NoTV, NoMac, NoWatch, iOS (12,0)]
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

	
	[NoTV, NoMac, NoWatch, iOS (12,0)]
	[StrongDictionary ("AVCapturePhotoSettingsThumbnailFormatKeys")]
	interface AVCapturePhotoSettingsThumbnailFormat {
		NSString Codec { get; set; }
		NSNumber Width { get; set; }
		NSNumber Height { get; set; }
	}

	[NoWatch]
	[NoTV, NoMac, iOS (10,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVCapturePhotoSettings : NSCopying
	{
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

		[iOS (11,0)]
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

		[Export ("autoStillImageStabilizationEnabled")]
		bool IsAutoStillImageStabilizationEnabled { [Bind ("isAutoStillImageStabilizationEnabled")] get; set; }

		[Export ("highResolutionPhotoEnabled")]
		bool IsHighResolutionPhotoEnabled { [Bind ("isHighResolutionPhotoEnabled")] get; set; }

		[NullAllowed, Export ("livePhotoMovieFileURL", ArgumentSemantic.Copy)]
		NSUrl LivePhotoMovieFileUrl { get; set; }

		[Export ("livePhotoMovieMetadata", ArgumentSemantic.Copy)]
		AVMetadataItem[] LivePhotoMovieMetadata { get; set; }

		[Export ("availablePreviewPhotoPixelFormatTypes")]
		NSNumber[] AvailablePreviewPhotoPixelFormatTypes { get; }

		[NullAllowed, Export ("previewPhotoFormat", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> PreviewPhotoFormat { get; set; }

		[iOS (10, 2)]
		[Export ("autoDualCameraFusionEnabled")]
		bool AutoDualCameraFusionEnabled { [Bind ("isAutoDualCameraFusionEnabled")] get; set; }

		[iOS (11, 0)]
		[NullAllowed, Export ("processedFileType")]
		string ProcessedFileType { get; }

		[iOS (11, 0)]
		[NullAllowed, Export ("rawFileType")]
		string RawFileType { get; }

		[iOS (11, 0)]
		[Export ("dualCameraDualPhotoDeliveryEnabled")]
		bool DualCameraDualPhotoDeliveryEnabled { [Bind ("isDualCameraDualPhotoDeliveryEnabled")] get; set; }

		[iOS (11, 0)]
		[Export ("depthDataDeliveryEnabled")]
		bool DepthDataDeliveryEnabled { [Bind ("isDepthDataDeliveryEnabled")] get; set; }

		[iOS (11, 0)]
		[Export ("embedsDepthDataInPhoto")]
		bool EmbedsDepthDataInPhoto { get; set; }

		[iOS (11, 0)]
		[Export ("depthDataFiltered")]
		bool DepthDataFiltered { [Bind ("isDepthDataFiltered")] get; set; }

		[iOS (11, 0)]
		[Export ("cameraCalibrationDataDeliveryEnabled")]
		bool CameraCalibrationDataDeliveryEnabled { [Bind ("isCameraCalibrationDataDeliveryEnabled")] get; set; }

		[iOS (11, 0)]
		[Export ("metadata", ArgumentSemantic.Copy)]
		NSDictionary Metadata { get; set; }

		[iOS (11, 0)]
		[Export ("livePhotoVideoCodecType")]
		string LivePhotoVideoCodecType { get; set; }

		[Internal]
		[iOS (11, 0)]
		[Export ("availableEmbeddedThumbnailPhotoCodecTypes")]
		NSString[] _GetAvailableEmbeddedThumbnailPhotoCodecTypes { get; }

#if !XAMCORE_4_0
		[Obsolete ("Use 'AvailableEmbeddedThumbnailPhotoCodecTypes' instead.")]
		[iOS (11, 0)]
		[Wrap ("Array.ConvertAll (_GetAvailableEmbeddedThumbnailPhotoCodecTypes, s => AVVideoCodecTypeExtensions.GetValue (s))", IsVirtual = false)]
		AVVideoCodecType[] GetAvailableEmbeddedThumbnailPhotoCodecTypes { get; }
#endif
		[iOS (11, 0)]
		[Wrap ("Array.ConvertAll (_GetAvailableEmbeddedThumbnailPhotoCodecTypes, s => AVVideoCodecTypeExtensions.GetValue (s))", IsVirtual = true)]
		AVVideoCodecType[] AvailableEmbeddedThumbnailPhotoCodecTypes { get; }

#if XAMCORE_4_0
		[iOS (11, 0)]
		[NullAllowed, Export ("embeddedThumbnailPhotoFormat", ArgumentSemantic.Copy)]
		NSDictionary WeakEmbeddedThumbnailPhotoFormat { get; set; }

		[Warp ("WeakEmbeddedThumbnailPhotoFormat")]
		AVCapturePhotoSettingsThumbnailFormat EmbeddedThumbnailPhotoFormat { get; set; }
#else
		[iOS (11, 0)]
		[NullAllowed, Export ("embeddedThumbnailPhotoFormat", ArgumentSemantic.Copy)]
		NSDictionary EmbeddedThumbnailPhotoFormat { get; set; }
#endif

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("portraitEffectsMatteDeliveryEnabled")]
		bool PortraitEffectsMatteDeliveryEnabled { [Bind ("isPortraitEffectsMatteDeliveryEnabled")] get; set; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("embedsPortraitEffectsMatteInPhoto")]
		bool EmbedsPortraitEffectsMatteInPhoto { get; set; }

		[BindAs (typeof (AVVideoCodecType []))]
		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("availableRawEmbeddedThumbnailPhotoCodecTypes")]
		NSString[] AvailableRawEmbeddedThumbnailPhotoCodecTypes { get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[NullAllowed, Export ("rawEmbeddedThumbnailPhotoFormat", ArgumentSemantic.Copy)]
		NSDictionary WeakRawEmbeddedThumbnailPhotoFormat { get; set; }

		[Wrap ("WeakRawEmbeddedThumbnailPhotoFormat")]
		AVCapturePhotoSettingsThumbnailFormat RawEmbeddedThumbnailPhotoFormat { get; set; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("autoRedEyeReductionEnabled")]
		bool AutoRedEyeReductionEnabled { [Bind ("isAutoRedEyeReductionEnabled")] get; set; }
	}
	
#if !MONOMAC
	[NoWatch]
	[NoTV, NoMac, iOS (10,0)]
	[BaseType (typeof(AVCapturePhotoSettings))]
	[DisableDefaultCtor]
	interface AVCapturePhotoBracketSettings
	{
		[iOS (11,0)]
		[Static]
		[Export ("photoBracketSettingsWithRawPixelFormatType:rawFileType:processedFormat:processedFileType:bracketedSettings:")]
		AVCapturePhotoBracketSettings FromPhotoBracketSettings (uint rawPixelFormatType, [NullAllowed] string rawFileType, [NullAllowed] NSDictionary<NSString, NSObject> processedFormat, [NullAllowed] string processedFileType, AVCaptureBracketedStillImageSettings[] bracketedSettings);

		[Static]
		[Export ("photoBracketSettingsWithRawPixelFormatType:processedFormat:bracketedSettings:")]
		AVCapturePhotoBracketSettings FromRawPixelFormatType (uint rawPixelFormatType, [NullAllowed] NSDictionary<NSString, NSObject> format, AVCaptureBracketedStillImageSettings [] bracketedSettings);

		[Export ("bracketedSettings")]
		AVCaptureBracketedStillImageSettings [] BracketedSettings { get; }
		
		[Export ("lensStabilizationEnabled")]
		bool IsLensStabilizationEnabled { [Bind ("isLensStabilizationEnabled")] get; set; }
	}
#endif

	[NoWatch]
	[NoTV, NoMac, iOS (10,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureResolvedPhotoSettings
	{
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

		[Export ("stillImageStabilizationEnabled")]
		bool IsStillImageStabilizationEnabled { [Bind ("isStillImageStabilizationEnabled")] get; }

		[iOS (10, 2)]
		[Export ("dualCameraFusionEnabled")]
		bool DualCameraFusionEnabled { [Bind ("isDualCameraFusionEnabled")] get; }

		[iOS (11, 0)]
		[Export ("embeddedThumbnailDimensions")]
		CMVideoDimensions EmbeddedThumbnailDimensions { get; }

		[iOS (11, 0)]
		[Export ("expectedPhotoCount")]
		nuint ExpectedPhotoCount { get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("portraitEffectsMatteDimensions")]
		CMVideoDimensions PortraitEffectsMatteDimensions { get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("rawEmbeddedThumbnailDimensions")]
		CMVideoDimensions RawEmbeddedThumbnailDimensions { get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("redEyeReductionEnabled")]
		bool RedEyeReductionEnabled { [Bind ("isRedEyeReductionEnabled")] get; }
	}

#if !MONOMAC

	interface IAVCapturePhotoCaptureDelegate {}

	[NoWatch]
	[NoTV, NoMac, iOS (10,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface AVCapturePhotoCaptureDelegate
	{
		[Export ("captureOutput:willBeginCaptureForResolvedSettings:")]
		void WillBeginCapture (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings);

		[Export ("captureOutput:willCapturePhotoForResolvedSettings:")]
		void WillCapturePhoto (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings);

		[Export ("captureOutput:didCapturePhotoForResolvedSettings:")]
		void DidCapturePhoto (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings);

		[Deprecated (PlatformName.iOS, 11,0, message: "Use the 'DidFinishProcessingPhoto' overload accepting a 'AVCapturePhoto' instead.")]
		[Export ("captureOutput:didFinishProcessingPhotoSampleBuffer:previewPhotoSampleBuffer:resolvedSettings:bracketSettings:error:")]
		void DidFinishProcessingPhoto (AVCapturePhotoOutput captureOutput, [NullAllowed] CMSampleBuffer photoSampleBuffer, [NullAllowed] CMSampleBuffer previewPhotoSampleBuffer, AVCaptureResolvedPhotoSettings resolvedSettings, [NullAllowed] AVCaptureBracketedStillImageSettings bracketSettings, [NullAllowed] NSError error);

		[Deprecated (PlatformName.iOS, 11,0, message: "Use the 'DidFinishProcessingPhoto' overload accepting a 'AVCapturePhoto' instead.")]
		[Export ("captureOutput:didFinishProcessingRawPhotoSampleBuffer:previewPhotoSampleBuffer:resolvedSettings:bracketSettings:error:")]
		void DidFinishProcessingRawPhoto (AVCapturePhotoOutput captureOutput, [NullAllowed] CMSampleBuffer rawSampleBuffer, [NullAllowed] CMSampleBuffer previewPhotoSampleBuffer, AVCaptureResolvedPhotoSettings resolvedSettings, [NullAllowed] AVCaptureBracketedStillImageSettings bracketSettings, [NullAllowed] NSError error);

		[iOS (11,0)]
		[Export ("captureOutput:didFinishProcessingPhoto:error:")]
		void DidFinishProcessingPhoto (AVCapturePhotoOutput output, AVCapturePhoto photo, [NullAllowed] NSError error);

		[Export ("captureOutput:didFinishRecordingLivePhotoMovieForEventualFileAtURL:resolvedSettings:")]
		void DidFinishRecordingLivePhotoMovie (AVCapturePhotoOutput captureOutput, NSUrl outputFileUrl, AVCaptureResolvedPhotoSettings resolvedSettings);

		[Export ("captureOutput:didFinishProcessingLivePhotoToMovieFileAtURL:duration:photoDisplayTime:resolvedSettings:error:")]
		void DidFinishProcessingLivePhotoMovie (AVCapturePhotoOutput captureOutput, NSUrl outputFileUrl, CMTime duration, CMTime photoDisplayTime, AVCaptureResolvedPhotoSettings resolvedSettings, [NullAllowed] NSError error);

		[Export ("captureOutput:didFinishCaptureForResolvedSettings:error:")]
		void DidFinishCapture (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings, [NullAllowed] NSError error);
	}

	[NoWatch]
	[NoTV, NoMac, iOS (10,0)]
	[BaseType (typeof(AVCaptureOutput))]
	interface AVCapturePhotoOutput
	{
		[Export ("capturePhotoWithSettings:delegate:")]
		void CapturePhoto (AVCapturePhotoSettings settings, IAVCapturePhotoCaptureDelegate cb);

		[Export ("availablePhotoPixelFormatTypes")]
		NSNumber [] AvailablePhotoPixelFormatTypes { get; }

		[Export ("availablePhotoCodecTypes")]
		string [] AvailablePhotoCodecTypes { get; }

		[Export ("availableRawPhotoPixelFormatTypes")]
		NSNumber [] AvailableRawPhotoPixelFormatTypes { get; }

		[Export ("stillImageStabilizationSupported")]
		bool IsStillImageStabilizationSupported { [Bind ("isStillImageStabilizationSupported")] get; }

		[Export ("isStillImageStabilizationScene")]
		bool IsStillImageStabilizationScene { get; }

#if XAMCORE_4_0
		[BindAs (typeof (AVCaptureFlashMode []))]
#endif		
		[Export ("supportedFlashModes")]
		NSNumber[] SupportedFlashModes { get; }

		[Export ("isFlashScene")]
		bool IsFlashScene { get; }

		[Export ("photoSettingsForSceneMonitoring", ArgumentSemantic.Copy)]
		AVCapturePhotoSettings PhotoSettingsForSceneMonitoring { get; set; }

		[Export ("highResolutionCaptureEnabled")]
		bool IsHighResolutionCaptureEnabled { [Bind ("isHighResolutionCaptureEnabled")] get; set; }

		[Export ("maxBracketedCapturePhotoCount")]
		nuint MaxBracketedCapturePhotoCount { get; }

		[Export ("lensStabilizationDuringBracketedCaptureSupported")]
		bool IsLensStabilizationDuringBracketedCaptureSupported { [Bind ("isLensStabilizationDuringBracketedCaptureSupported")] get; }

		[Export ("livePhotoCaptureSupported")]
		bool IsLivePhotoCaptureSupported { [Bind ("isLivePhotoCaptureSupported")] get; }

		[Export ("livePhotoCaptureEnabled")]
		bool IsLivePhotoCaptureEnabled { [Bind ("isLivePhotoCaptureEnabled")] get; set; }

		[Export ("livePhotoCaptureSuspended")]
		bool IsLivePhotoCaptureSuspended { [Bind ("isLivePhotoCaptureSuspended")] get; set; }

		[Export ("livePhotoAutoTrimmingEnabled")]
		bool IsLivePhotoAutoTrimmingEnabled { [Bind ("isLivePhotoAutoTrimmingEnabled")] get; set; }

		[Static]
		[Export ("JPEGPhotoDataRepresentationForJPEGSampleBuffer:previewPhotoSampleBuffer:")]
		[return: NullAllowed]
		NSData GetJpegPhotoDataRepresentation (CMSampleBuffer JPEGSampleBuffer, [NullAllowed] CMSampleBuffer previewPhotoSampleBuffer);

		[Static]
		[Export ("DNGPhotoDataRepresentationForRawSampleBuffer:previewPhotoSampleBuffer:")]
		[return: NullAllowed]
		NSData GetDngPhotoDataRepresentation (CMSampleBuffer rawSampleBuffer, [NullAllowed] CMSampleBuffer previewPhotoSampleBuffer);

		[Export ("preparedPhotoSettingsArray")]
		AVCapturePhotoSettings[] PreparedPhotoSettings { get; }

		[Export ("setPreparedPhotoSettingsArray:completionHandler:")]
		[Async]
		void SetPreparedPhotoSettings (AVCapturePhotoSettings[] preparedPhotoSettingsArray, [NullAllowed] Action<bool, NSError> completionHandler);

		[iOS (10, 2)]
		[Export ("dualCameraFusionSupported")]
		bool DualCameraFusionSupported { [Bind ("isDualCameraFusionSupported")] get; }

		// From AVCapturePhotoOutput (AVCapturePhotoOutputDepthDataDeliverySupport) Category

		[iOS (11,0)]
		[Export ("depthDataDeliverySupported")]
		bool DepthDataDeliverySupported { [Bind ("isDepthDataDeliverySupported")] get; }

		[iOS (11,0)]
		[Export ("depthDataDeliveryEnabled")]
		bool DepthDataDeliveryEnabled { [Bind ("isDepthDataDeliveryEnabled")] get; set; }

		[Internal]
		[iOS (11, 0)]
		[Export ("availablePhotoFileTypes")]
		NSString[] _GetAvailablePhotoFileTypes { get; }

		[iOS (11, 0)]
		[Wrap ("Array.ConvertAll (_GetAvailablePhotoFileTypes, s => AVFileTypesExtensions.GetValue (s))")]
		AVFileTypes[] GetAvailablePhotoFileTypes { get; }


		[Internal]
		[iOS (11, 0)]
		[Export ("availableRawPhotoFileTypes")]
		NSString[] _GetAvailableRawPhotoFileTypes { get; }

		[iOS (11, 0)]
		[Wrap ("Array.ConvertAll (_GetAvailableRawPhotoFileTypes, s => AVFileTypesExtensions.GetValue (s))")]
		AVFileTypes[] GetAvailableRawPhotoFileTypes { get; }

		[iOS (11,0)]
		[Export ("supportedPhotoPixelFormatTypesForFileType:")]
		NSNumber[] GetSupportedPhotoPixelFormatTypesForFileType (string fileType);

		[Internal]
		[iOS (11,0)]
		[Export ("supportedPhotoCodecTypesForFileType:")]
		NSString[] _GetSupportedPhotoCodecTypesForFileType (string fileType);

		[iOS (11,0)]
		[Wrap ("Array.ConvertAll (_GetSupportedPhotoCodecTypesForFileType (fileType), s => AVVideoCodecTypeExtensions.GetValue (s))")]
		AVVideoCodecType[] GetSupportedPhotoCodecTypesForFileType (string fileType);

		[iOS (11,0)]
		[Export ("supportedRawPhotoPixelFormatTypesForFileType:")]
		NSNumber[] GetSupportedRawPhotoPixelFormatTypesForFileType (string fileType);

		[iOS (11, 0)]
		[Export ("dualCameraDualPhotoDeliverySupported")]
		bool DualCameraDualPhotoDeliverySupported { [Bind ("isDualCameraDualPhotoDeliverySupported")] get; }

		[iOS (11, 0)]
		[Export ("dualCameraDualPhotoDeliveryEnabled")]
		bool DualCameraDualPhotoDeliveryEnabled { [Bind ("isDualCameraDualPhotoDeliveryEnabled")] get; set; }

		[iOS (11, 0)]
		[Export ("availableLivePhotoVideoCodecTypes")]
		string[] AvailableLivePhotoVideoCodecTypes { [return: BindAs (typeof (AVVideoCodecType []))] get; }

		[iOS (11, 0)]
		[Export ("cameraCalibrationDataDeliverySupported")]
		bool CameraCalibrationDataDeliverySupported { [Bind ("isCameraCalibrationDataDeliverySupported")] get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("portraitEffectsMatteDeliverySupported")]
		bool PortraitEffectsMatteDeliverySupported { [Bind ("isPortraitEffectsMatteDeliverySupported")] get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("portraitEffectsMatteDeliveryEnabled")]
		bool PortraitEffectsMatteDeliveryEnabled { [Bind ("isPortraitEffectsMatteDeliveryEnabled")] get; set; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("autoRedEyeReductionSupported")]
		bool AutoRedEyeReductionSupported { [Bind ("isAutoRedEyeReductionSupported")] get; }
	}
#endif
	
	[BaseType (typeof (AVCaptureFileOutput))]
	[NoTV]
	[NoWatch]
	interface AVCaptureMovieFileOutput {
		[NullAllowed] // by default this property is null
		[Export ("metadata", ArgumentSemantic.Copy)]
		AVMetadataItem [] Metadata { get; set;  }

		[Export ("movieFragmentInterval")]
		CMTime MovieFragmentInterval { get; set; }

#if !MONOMAC
		[iOS (9,0)]
		[Export ("recordsVideoOrientationAndMirroringChangesAsMetadataTrackForConnection:")]
		bool RecordsVideoOrientationAndMirroringChangesAsMetadataTrack (AVCaptureConnection connection);

		[iOS (9,0)]
		[Export ("setRecordsVideoOrientationAndMirroringChanges:asMetadataTrackForConnection:")]
		void SetRecordsVideoOrientationAndMirroringChanges (bool doRecordChanges, AVCaptureConnection connection);

		[iOS (10, 0)]
		[Export ("availableVideoCodecTypes")]
		NSString [] AvailableVideoCodecTypes { get; }
#endif

		[iOS (10,0), Mac (10,7)]
		[Export ("outputSettingsForConnection:")]
		NSDictionary GetOutputSettings (AVCaptureConnection connection);

		[iOS (10,0), Mac (10,7)]
		[Export ("setOutputSettings:forConnection:")]
		void SetOutputSettings (NSDictionary outputSettings, AVCaptureConnection connection);

		[NoWatch, NoTV, NoMac, iOS (12,0)]
		[Export ("supportedOutputSettingsKeysForConnection:")]
		string[] GetSupportedOutputSettingsKeys (AVCaptureConnection connection);
	}

	[NoTV]
	[NoWatch]
	[Availability (Deprecated = Platform.iOS_10_0, Message = "Use 'AVCapturePhotoOutput' instead.")]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureStillImageOutput {
		[Export ("availableImageDataCVPixelFormatTypes")]
		NSNumber [] AvailableImageDataCVPixelFormatTypes { get;  }

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
		NSData JpegStillToNSData (CMSampleBuffer buffer);

		// 5.0
		[Export ("capturingStillImage")]
		bool CapturingStillImage { [Bind ("isCapturingStillImage")] get;  }

#if !MONOMAC
		[iOS (7,0)]
		[Export ("automaticallyEnablesStillImageStabilizationWhenAvailable")]
		bool AutomaticallyEnablesStillImageStabilizationWhenAvailable { get; set; }

		[iOS (7,0)]
		[Export ("stillImageStabilizationActive")]
		bool IsStillImageStabilizationActive { [Bind ("isStillImageStabilizationActive")] get; }

		[iOS (7,0)]
		[Export ("stillImageStabilizationSupported")]
		bool IsStillImageStabilizationSupported { [Bind ("isStillImageStabilizationSupported")] get; }

		[iOS (8,0)]
		[Export ("captureStillImageBracketAsynchronouslyFromConnection:withSettingsArray:completionHandler:")]
		void CaptureStillImageBracket (AVCaptureConnection connection, AVCaptureBracketedStillImageSettings [] settings, Action<CMSampleBuffer, AVCaptureBracketedStillImageSettings, NSError> imageHandler);

		[iOS (8,0)]
		[Export ("maxBracketedCaptureStillImageCount")]
		nuint MaxBracketedCaptureStillImageCount { get; }

		[iOS (8,0)]
		[Export ("prepareToCaptureStillImageBracketFromConnection:withSettingsArray:completionHandler:")]
		void PrepareToCaptureStillImageBracket (AVCaptureConnection connection, AVCaptureBracketedStillImageSettings [] settings, Action<bool,NSError> handler);

		[iOS (8,0)]
		[Export ("highResolutionStillImageOutputEnabled")]
		bool HighResolutionStillImageOutputEnabled { [Bind ("isHighResolutionStillImageOutputEnabled")] get; set; }

		[iOS (9,0)]
		[Export ("lensStabilizationDuringBracketedCaptureSupported")]
		bool LensStabilizationDuringBracketedCaptureSupported { [Bind ("isLensStabilizationDuringBracketedCaptureSupported")] get; }

		[iOS (9,0)]
		[Export ("lensStabilizationDuringBracketedCaptureEnabled")]
		bool LensStabilizationDuringBracketedCaptureEnabled { [Bind ("isLensStabilizationDuringBracketedCaptureEnabled")] get; set; }
#endif
	}

	[NoTV, iOS (10,0), NoMac, NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // init NS_UNAVAILABLE
	interface AVCaptureDeviceDiscoverySession {

		[Internal]
		[Static]
		[Export ("discoverySessionWithDeviceTypes:mediaType:position:")]
		AVCaptureDeviceDiscoverySession _Create (NSArray deviceTypes, [NullAllowed] string mediaType, AVCaptureDevicePosition position);

		[Export ("devices")]
		AVCaptureDevice [] Devices { get; }
	}

	[NoTV, iOS (10,0), NoMac, NoWatch]
	enum AVCaptureDeviceType {

		[Field ("AVCaptureDeviceTypeBuiltInMicrophone")]
		BuiltInMicrophone,

		[Field ("AVCaptureDeviceTypeBuiltInWideAngleCamera")]
		BuiltInWideAngleCamera,

		[Field ("AVCaptureDeviceTypeBuiltInTelephotoCamera")]
		BuiltInTelephotoCamera,

		[iOS (10, 0)]
		[Deprecated (PlatformName.iOS, 10, 2, message: "Use 'BuiltInDualCamera' instead.")]
		[Field ("AVCaptureDeviceTypeBuiltInDuoCamera")]
		BuiltInDuoCamera,

		[iOS (10, 2)]
		[Field ("AVCaptureDeviceTypeBuiltInDualCamera")]
		BuiltInDualCamera,

		[iOS (11, 1)]
		[Field ("AVCaptureDeviceTypeBuiltInTrueDepthCamera")]
		BuiltInTrueDepthCamera,
	}

	[NoTV, iOS (7,0), Mac (10,14, onlyOn64: true), NoWatch] // matches API that uses it.
	enum AVAuthorizationMediaType {
		Video,
		Audio,
	}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: Cannot instantiate a AVCaptureDevice directly.
	[DisableDefaultCtor]
	interface AVCaptureDevice {
		[Export ("uniqueID")]
		string UniqueID { get;  }

		[Export ("modelID")]
		string ModelID { get;  }

		[Export ("localizedName")]
		string LocalizedName { get;  }

		[Export ("connected")]
		bool Connected { [Bind ("isConnected")] get;  }

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'AVCaptureDeviceDiscoverySession' instead.")]
		[Static, Export ("devices")]
		AVCaptureDevice [] Devices { get;  }

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'AVCaptureDeviceDiscoverySession' instead.")]
		[Static]
		[Export ("devicesWithMediaType:")]
		AVCaptureDevice [] DevicesWithMediaType (string mediaType);

		[Static]
		[Export ("defaultDeviceWithMediaType:")]
		AVCaptureDevice GetDefaultDevice (NSString mediaType);

		[Static]
		[Wrap ("GetDefaultDevice (mediaType.GetConstant ())")]
		AVCaptureDevice GetDefaultDevice (AVMediaTypes mediaType);

#if !XAMCORE_4_0
		[Obsolete ("Use 'GetDefaultDevice (AVMediaTypes)'.")]
		[Static]
		[Wrap ("GetDefaultDevice ((NSString) mediaType)")]
		AVCaptureDevice DefaultDeviceWithMediaType (string mediaType);
#endif

		[Static]
		[Export ("deviceWithUniqueID:")]
		AVCaptureDevice DeviceWithUniqueID (string deviceUniqueID);

		[Export ("hasMediaType:")]
		bool HasMediaType (string mediaType);

		[Wrap ("HasMediaType ((string) mediaType.GetConstant ())")]
		bool HasMediaType (AVMediaTypes mediaType);

		[Export ("lockForConfiguration:")]
		bool LockForConfiguration (out NSError error);

		[Export ("unlockForConfiguration")]
		void UnlockForConfiguration ();

		[Export ("supportsAVCaptureSessionPreset:")]
		bool SupportsAVCaptureSessionPreset (string preset);

		[Availability (Deprecated = Platform.iOS_10_0, Message="Use 'AVCapturePhotoSettings.FlashMode' instead.")]
		[Export ("flashMode")]
		AVCaptureFlashMode FlashMode { get; set;  }

		[Availability (Deprecated = Platform.iOS_10_0, Message="Use 'AVCapturePhotoOutput.SupportedFlashModes' instead.")]
		[Export ("isFlashModeSupported:")]
		bool IsFlashModeSupported (AVCaptureFlashMode flashMode);

		[Export ("torchMode", ArgumentSemantic.Assign)] 
		AVCaptureTorchMode TorchMode { get; set;  }

		[Export ("isTorchModeSupported:")]
		bool IsTorchModeSupported (AVCaptureTorchMode torchMode);

		[Export ("isFocusModeSupported:")]
		bool IsFocusModeSupported (AVCaptureFocusMode focusMode);

		[Export ("focusMode", ArgumentSemantic.Assign)]
		AVCaptureFocusMode FocusMode { get; set;  }

		[Export ("focusPointOfInterestSupported")]
		bool FocusPointOfInterestSupported { [Bind ("isFocusPointOfInterestSupported")] get;  }

		[Export ("focusPointOfInterest", ArgumentSemantic.Assign)]
		CGPoint FocusPointOfInterest { get; set;  }

		[Export ("adjustingFocus")]
		bool AdjustingFocus { [Bind ("isAdjustingFocus")] get;  }

		[Export ("exposureMode", ArgumentSemantic.Assign)]
		AVCaptureExposureMode ExposureMode { get; set;  }

		[Export ("isExposureModeSupported:")]
		bool IsExposureModeSupported (AVCaptureExposureMode exposureMode);

		[Export ("exposurePointOfInterestSupported")]
		bool ExposurePointOfInterestSupported { [Bind ("isExposurePointOfInterestSupported")] get;  }

		[Export ("exposurePointOfInterest")]
		CGPoint ExposurePointOfInterest { get; set;  }

		[Export ("adjustingExposure")]
		bool AdjustingExposure { [Bind ("isAdjustingExposure")] get;  }

		[Export ("isWhiteBalanceModeSupported:")]
		bool IsWhiteBalanceModeSupported (AVCaptureWhiteBalanceMode whiteBalanceMode);
		
		[Export ("whiteBalanceMode", ArgumentSemantic.Assign)]
		AVCaptureWhiteBalanceMode WhiteBalanceMode { get; set;  }

		[Export ("adjustingWhiteBalance")]
		bool AdjustingWhiteBalance { [Bind ("isAdjustingWhiteBalance")] get;  }

		[Export ("position")]
		AVCaptureDevicePosition Position { get; }

		[Field ("AVCaptureDeviceWasConnectedNotification")]
		[Notification]
		NSString WasConnectedNotification { get; }

		[Field ("AVCaptureDeviceWasDisconnectedNotification")]
		[Notification]
		NSString WasDisconnectedNotification { get; }

		[iOS (6, 0)]
		[NoMac]
		[Field ("AVCaptureMaxAvailableTorchLevel")]
		float MaxAvailableTorchLevel { get; } // defined as 'float'

#if !MONOMAC
		[Field ("AVCaptureDeviceSubjectAreaDidChangeNotification")]
		[Notification]
		NSString SubjectAreaDidChangeNotification { get; }

		[Export ("subjectAreaChangeMonitoringEnabled")]
		bool SubjectAreaChangeMonitoringEnabled { [Bind ("isSubjectAreaChangeMonitoringEnabled")] get; set; }
		
		[Export ("isFlashAvailable")]
		bool FlashAvailable { get;  }

		[Availability (Deprecated = Platform.iOS_10_0, Message="Use 'AVCapturePhotoOutput.IsFlashScene' instead.")]
		[Export ("isFlashActive")]
		bool FlashActive { get; }

		[Export ("isTorchAvailable")]
		bool TorchAvailable { get; }

		[Export ("torchLevel")]
		float TorchLevel { get; } // defined as 'float'

		// 6.0
		[iOS (6,0)]
		[Export ("torchActive")]
		bool TorchActive { [Bind ("isTorchActive")] get;  }

		[iOS (6,0)]
		[Export ("setTorchModeOnWithLevel:error:")]
		bool SetTorchModeLevel (float /* defined as 'float' */ torchLevel, out NSError outError);

		[iOS (6,0)]
		[Export ("lowLightBoostSupported")]
		bool LowLightBoostSupported { [Bind ("isLowLightBoostSupported")] get; }

		[iOS (6,0)]
		[Export ("lowLightBoostEnabled")]
		bool LowLightBoostEnabled { [Bind ("isLowLightBoostEnabled")] get; }

		[iOS (6,0)]
		[Export ("automaticallyEnablesLowLightBoostWhenAvailable")]
		bool AutomaticallyEnablesLowLightBoostWhenAvailable { get; set; }

		[iOS (7,0)]
		[Export ("videoZoomFactor")]
		nfloat VideoZoomFactor { get; set; }
	
		[iOS (7,0)]
		[Export ("rampToVideoZoomFactor:withRate:")]
		void RampToVideoZoom (nfloat factor, float /* float, not CGFloat */ rate);
	
		[iOS (7,0)]
		[Export ("rampingVideoZoom")]
		bool RampingVideoZoom { [Bind ("isRampingVideoZoom")] get; }
	
		[iOS (7,0)]
		[Export ("cancelVideoZoomRamp")]
		void CancelVideoZoomRamp ();
			
		[iOS (7,0)]
		[Export ("autoFocusRangeRestrictionSupported")]
		bool AutoFocusRangeRestrictionSupported { [Bind ("isAutoFocusRangeRestrictionSupported")] get; }
	
		[iOS (7,0)]
		[Export ("autoFocusRangeRestriction")]
		AVCaptureAutoFocusRangeRestriction AutoFocusRangeRestriction { get; set; }
	
		[iOS (7,0)]
		[Export ("smoothAutoFocusSupported")]
		bool SmoothAutoFocusSupported { [Bind ("isSmoothAutoFocusSupported")] get; }
	
		[iOS (7,0)]
		[Export ("smoothAutoFocusEnabled")]
		bool SmoothAutoFocusEnabled { [Bind ("isSmoothAutoFocusEnabled")] get; set; }

		// Either AVMediaTypeVideo or AVMediaTypeAudio.
		[iOS (7,0)]
		[Static]
		[Wrap ("RequestAccessForMediaType (mediaType == AVAuthorizationMediaType.Video ? AVMediaTypes.Video.GetConstant () : AVMediaTypes.Audio.GetConstant (), completion)")]
		[Async]
		void RequestAccessForMediaType (AVAuthorizationMediaType mediaType, AVRequestAccessStatus completion);
#endif

		[iOS (7,0)]
		[Mac (10,14, onlyOn64: true)]
		[Static, Export ("requestAccessForMediaType:completionHandler:")]
		[Async]
		void RequestAccessForMediaType (NSString avMediaTypeToken, AVRequestAccessStatus completion);

		// Calling this method with any media type other than AVMediaTypeVideo or AVMediaTypeAudio raises an exception.
		[iOS (7,0)]
		[Mac (10,14, onlyOn64: true)]
		[Static]
		[Wrap ("GetAuthorizationStatus (mediaType == AVAuthorizationMediaType.Video ? AVMediaTypes.Video.GetConstant () : AVMediaTypes.Audio.GetConstant ())")]
		AVAuthorizationStatus GetAuthorizationStatus (AVAuthorizationMediaType mediaType);

		[iOS (7,0)]
		[Mac (10,14, onlyOn64: true)]
		[Static, Export ("authorizationStatusForMediaType:")]
		AVAuthorizationStatus GetAuthorizationStatus (NSString avMediaTypeToken);

		[iOS (7,0)][Mac (10,7)]
		[Export ("activeFormat", ArgumentSemantic.Retain)]
		AVCaptureDeviceFormat ActiveFormat { get; set; }

		[iOS (7,0)][Mac (10,7)]
		[Export ("formats")]
		AVCaptureDeviceFormat [] Formats { get; }

		[Mac (10,7)] // Also iOS 4.0+ but we do not need this metadata
		[Export ("hasFlash")]
		bool HasFlash { get; }

		[Mac (10,7)] // Also iOS 4.0+ but we do not need this metadata
		[Export ("hasTorch")]
		bool HasTorch { get; }

		[Mac (10,7), NoiOS]
		[Export ("inUseByAnotherApplication")]
		bool InUseByAnotherApplication { [Bind ("isInUseByAnotherApplication")] get; }

		[Mac (10,7), NoiOS]
		[Export ("suspended")]
		bool Suspended { [Bind ("isSuspended")] get; }

		[Mac (10,7), NoiOS]
		[Export ("linkedDevices")]
		AVCaptureDevice [] LinkedDevices { get; }

		[Mac (10,9), NoiOS]
		[Export ("manufacturer")]
		string Manufacturer { get; }

		[Mac (10,7), NoiOS]
		[Export ("transportControlsSpeed")]
		float TransportControlsSpeed { get; } // float intended

		[Mac (10,7), NoiOS]
		[Export ("transportControlsSupported")]
		bool TransportControlsSupported { get; }

		[Mac (10,7), NoiOS] // TODO: We can provide a better binding once IOKit is bound kIOAudioDeviceTransportType*
		[Export ("transportType")]
		int WeakTransportType { get; } // int intended

#if MONOMAC // Can't use [NoiOS] since types are also inside a block
		[Mac (10,7)]
		[Export ("activeInputSource", ArgumentSemantic.Retain)]
		AVCaptureDeviceInputSource ActiveInputSource { get; set; }

		[Mac (10,7)]
		[Export ("inputSources")]
		AVCaptureDeviceInputSource [] InputSources { get; }

		[Mac (10,7)]
		[Export ("setTransportControlsPlaybackMode:speed:")]
		void SetTransportControlsPlaybackMode (AVCaptureDeviceTransportControlsPlaybackMode mode, float speed); // Float intended

		[Mac (10,7)]
		[Export ("transportControlsPlaybackMode")]
		AVCaptureDeviceTransportControlsPlaybackMode TransportControlsPlaybackMode { get; }
#endif

		[iOS (7,0)]
		[Mac (10,9)]
		[Export ("activeVideoMinFrameDuration", ArgumentSemantic.Copy)]
		CMTime ActiveVideoMinFrameDuration { get; set; }

		[iOS (7,0)]
		[Mac (10,9)]
		[Export ("activeVideoMaxFrameDuration", ArgumentSemantic.Copy)]
		CMTime ActiveVideoMaxFrameDuration { get; set; }

		[iOS (10,0), NoMac]
		[Export ("lockingFocusWithCustomLensPositionSupported")]
		bool LockingFocusWithCustomLensPositionSupported { [Bind ("isLockingFocusWithCustomLensPositionSupported")] get; }

		[iOS (10,0), NoMac]
		[Export ("lockingWhiteBalanceWithCustomDeviceGainsSupported")]
		bool LockingWhiteBalanceWithCustomDeviceGainsSupported { [Bind ("isLockingWhiteBalanceWithCustomDeviceGainsSupported")] get; }

		// From AVCaptureDevice (AVCaptureDeviceType) Category
		[Internal]
		[iOS (10,0), NoMac]
		[Export ("deviceType")]
		NSString _DeviceType { get; }

		[iOS (10, 0), NoMac]
		[Wrap ("AVCaptureDeviceTypeExtensions.GetValue (_DeviceType)")]
		AVCaptureDeviceType DeviceType { get; }

		[Internal]
		[iOS (10,0), NoMac]
		[Static]
		[Export ("defaultDeviceWithDeviceType:mediaType:position:")]
		AVCaptureDevice _DefaultDeviceWithDeviceType (NSString deviceType, string mediaType, AVCaptureDevicePosition position);

		[iOS (10,0), NoMac]
		[Static]
		[Wrap ("AVCaptureDevice._DefaultDeviceWithDeviceType (deviceType.GetConstant (), mediaType, position)")]
		AVCaptureDevice GetDefaultDevice (AVCaptureDeviceType deviceType, string mediaType, AVCaptureDevicePosition position);

#if !MONOMAC && !WATCH
		//
		// iOS 8
		//
		[iOS (8,0)]
		[Field ("AVCaptureLensPositionCurrent")]
		float FocusModeLensPositionCurrent { get; } /* float, not CGFloat */
		
		[iOS (8,0)]
		[Export ("lensAperture")]
		float LensAperture { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Export ("exposureDuration")]
		CMTime ExposureDuration { get; }

		[iOS (8,0)]
		[Export ("ISO")]
		float ISO { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Export ("exposureTargetOffset")]
		float ExposureTargetOffset { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Export ("exposureTargetBias")]
		float ExposureTargetBias { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Export ("minExposureTargetBias")]
		float MinExposureTargetBias { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Export ("maxExposureTargetBias")]
		float MaxExposureTargetBias { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Export ("setExposureModeCustomWithDuration:ISO:completionHandler:")]
		[Async]
		void LockExposure (CMTime duration, float /* float, not CGFloat */ ISO, [NullAllowed] Action<CMTime> completionHandler);

		[iOS (8,0)]
		[Export ("setExposureTargetBias:completionHandler:")]
		[Async]
		void SetExposureTargetBias (float /* float, not CGFloat */ bias, [NullAllowed] Action<CMTime> completionHandler);

		[iOS (8,0)]
		[Export ("lensPosition")]
		float LensPosition { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Export ("setFocusModeLockedWithLensPosition:completionHandler:")]
		[Async]
		void SetFocusModeLocked (float /* float, not CGFloat */ lensPosition, [NullAllowed] Action<CMTime> completionHandler);		

		[iOS (8,0)]
		[Export ("deviceWhiteBalanceGains")]
		AVCaptureWhiteBalanceGains DeviceWhiteBalanceGains { get; }

		[iOS (8,0)]
		[Export ("grayWorldDeviceWhiteBalanceGains")]
		AVCaptureWhiteBalanceGains GrayWorldDeviceWhiteBalanceGains { get; }

		[iOS (8,0)]
		[Export ("maxWhiteBalanceGain")]
		float MaxWhiteBalanceGain { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Export ("setWhiteBalanceModeLockedWithDeviceWhiteBalanceGains:completionHandler:")]
		[Async]
		void SetWhiteBalanceModeLockedWithDeviceWhiteBalanceGains (AVCaptureWhiteBalanceGains whiteBalanceGains, [NullAllowed] Action<CMTime> completionHandler);

		[iOS (8,0)]
		[Export ("chromaticityValuesForDeviceWhiteBalanceGains:")]
		AVCaptureWhiteBalanceChromaticityValues GetChromaticityValues (AVCaptureWhiteBalanceGains whiteBalanceGains);

		[iOS (8,0)]
		[Export ("deviceWhiteBalanceGainsForChromaticityValues:")]
		AVCaptureWhiteBalanceGains GetDeviceWhiteBalanceGains (AVCaptureWhiteBalanceChromaticityValues chromaticityValues);


		[iOS (8,0)]
		[Export ("temperatureAndTintValuesForDeviceWhiteBalanceGains:")]
		AVCaptureWhiteBalanceTemperatureAndTintValues GetTemperatureAndTintValues (AVCaptureWhiteBalanceGains whiteBalanceGains);

		[iOS (8,0)]
		[Export ("deviceWhiteBalanceGainsForTemperatureAndTintValues:")]
		AVCaptureWhiteBalanceGains GetDeviceWhiteBalanceGains (AVCaptureWhiteBalanceTemperatureAndTintValues tempAndTintValues);

		[iOS (8,0)]
		[Field ("AVCaptureExposureDurationCurrent")]
		CMTime ExposureDurationCurrent { get; }

		[iOS (8,0)]
		[Field ("AVCaptureExposureTargetBiasCurrent")]
		float ExposureTargetBiasCurrent { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Field ("AVCaptureISOCurrent")]
		float ISOCurrent { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Field ("AVCaptureLensPositionCurrent")]
		float LensPositionCurrent { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Field ("AVCaptureWhiteBalanceGainsCurrent")]
		AVCaptureWhiteBalanceGains WhiteBalanceGainsCurrent { get; }

		[iOS (8,0)]
		[Export ("automaticallyAdjustsVideoHDREnabled")]
		bool AutomaticallyAdjustsVideoHdrEnabled { get; set; }

		[iOS (8,0)]
		[Export ("videoHDREnabled")]
		bool VideoHdrEnabled { [Bind ("isVideoHDREnabled")] get; set; }

		[iOS (10, 0)]
		[Export ("activeColorSpace", ArgumentSemantic.Assign)]
		AVCaptureColorSpace ActiveColorSpace { get; set; }

		// From AVCaptureDevice (AVCaptureDeviceDepthSupport) Category

		[iOS (11,0)]
		[Export ("activeDepthDataFormat", ArgumentSemantic.Retain), NullAllowed]
		AVCaptureDeviceFormat ActiveDepthDataFormat { get; set; }

		[iOS (11,0)]
		[Export ("minAvailableVideoZoomFactor")]
		nfloat MinAvailableVideoZoomFactor { get; }

		[iOS (11,0)]
		[Export ("maxAvailableVideoZoomFactor")]
		nfloat MaxAvailableVideoZoomFactor { get; }

		// From  AVCaptureDevice (AVCaptureDeviceSystemPressure) Category
		[NoWatch, NoTV, NoMac, iOS (11, 1)]
		[Export ("systemPressureState")]
		AVCaptureSystemPressureState SystemPressureState { get; }

		[iOS (11, 0)]
		[Export ("dualCameraSwitchOverVideoZoomFactor")]
		nfloat DualCameraSwitchOverVideoZoomFactor { get; }

		// From @interface AVCaptureDeviceDepthSupport (AVCaptureDevice)

		[NoMac, iOS (12, 0)]
		[Export ("activeDepthDataMinFrameDuration", ArgumentSemantic.Assign)]
		CMTime ActiveDepthDataMinFrameDuration { get; set; }

		// From @interface AVCaptureDeviceExposure (AVCaptureDevice)
		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("activeMaxExposureDuration", ArgumentSemantic.Assign)]
		CMTime ActiveMaxExposureDuration { get; set; }

#endif
	}

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

	[NoWatch, NoTV, NoMac, iOS (11,1)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureSystemPressureState
	{
		[Internal]
		[Export ("level")]
		NSString _Level { get; }

		[Wrap ("AVCaptureSystemPressureLevelExtensions.GetValue (_Level)")]
		AVCaptureSystemPressureLevel Level { get; }

		[Export ("factors")]
		AVCaptureSystemPressureFactors Factors { get; }
	}

	[NoWatch]
	[NoTV]
	[iOS (7,0), Mac(10,7)]
	[DisableDefaultCtor] // crash -> immutable, it can be set but it should be selected from tha available formats (not a custom one)
	[BaseType (typeof (NSObject))]
	interface AVCaptureDeviceFormat {
		[Export ("mediaType", ArgumentSemantic.Copy)]
		NSString MediaType { get; }
	
		[Export ("formatDescription", ArgumentSemantic.Copy)]
		CMFormatDescription FormatDescription { get; }
	
		[Export ("videoSupportedFrameRateRanges", ArgumentSemantic.Copy)]
		AVFrameRateRange [] VideoSupportedFrameRateRanges { get; }

#if !MONOMAC
		[Export ("videoFieldOfView")]
		float VideoFieldOfView { get; } // defined as 'float'
	
		[Export ("videoBinned")]
		bool VideoBinned { [Bind ("isVideoBinned")] get; }
	
		[Export ("videoStabilizationSupported")]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'IsVideoStabilizationModeSupported (AVCaptureVideoStabilizationMode)' instead.")]
		bool VideoStabilizationSupported { [Bind ("isVideoStabilizationSupported")] get; }
	
		[Export ("videoMaxZoomFactor")]
		nfloat VideoMaxZoomFactor { get; }
	
		[Export ("videoZoomFactorUpscaleThreshold")]
		nfloat VideoZoomFactorUpscaleThreshold { get; }

		[iOS (8,0)]
		[Export ("minExposureDuration")]
		CMTime MinExposureDuration { get; }

		[iOS (8,0)]
		[Export ("maxExposureDuration")]
		CMTime MaxExposureDuration { get; }

		[iOS (8,0)]
		[Export ("minISO")]
		float MinISO { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Export ("maxISO")]
		float MaxISO { get; } /* float, not CGFloat */

		[iOS (8,0)]
		[Export ("isVideoStabilizationModeSupported:")]
		bool IsVideoStabilizationModeSupported (AVCaptureVideoStabilizationMode mode);

		[iOS (8,0)]
		[Export ("videoHDRSupported")]
		bool videoHDRSupportedVideoHDREnabled { [Bind ("isVideoHDRSupported")] get; }

		[iOS (8,0)]
		[Export ("highResolutionStillImageDimensions")]
		CMVideoDimensions HighResolutionStillImageDimensions { get; }

		[iOS (8,0)]
		[Export ("autoFocusSystem")]
		AVCaptureAutoFocusSystem AutoFocusSystem { get; }

		[iOS (10, 0)]
		[Export ("supportedColorSpaces")]
		NSNumber[] SupportedColorSpaces { get; }

		[iOS (11, 0)]
		[Export ("videoMinZoomFactorForDepthDataDelivery")]
		nfloat VideoMinZoomFactorForDepthDataDelivery { get; }

		[iOS (11, 0)]
		[Export ("videoMaxZoomFactorForDepthDataDelivery")]
		nfloat VideoMaxZoomFactorForDepthDataDelivery { get; }

		[iOS (11, 0)]
		[Export ("supportedDepthDataFormats")]
		AVCaptureDeviceFormat[] SupportedDepthDataFormats { get; }

		[iOS (11, 0)]
		[Export ("unsupportedCaptureOutputClasses")]
		Class[] UnsupportedCaptureOutputClasses { get; }

		// from @interface AVCaptureDeviceFormatDepthDataAdditions (AVCaptureDeviceFormat)
		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("portraitEffectsMatteStillImageDeliverySupported")]
		bool PortraitEffectsMatteStillImageDeliverySupported { [Bind ("isPortraitEffectsMatteStillImageDeliverySupported")] get; }
#endif
	}

#if !XAMCORE_2_0
	delegate void AVCompletionHandler ();
#endif
	delegate void AVCaptureCompletionHandler (CMSampleBuffer imageDataSampleBuffer, NSError error);

	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface AVPlayer {
		[Export ("currentItem"), NullAllowed]
		AVPlayerItem CurrentItem { get;  }

		[Export ("rate")]
		float Rate { get; set;  } // defined as 'float'

		// note: not a property in ObjC
		[Export ("currentTime")]
		CMTime CurrentTime { get; }

		[Export ("actionAtItemEnd")]
		AVPlayerActionAtItemEnd ActionAtItemEnd { get; set;  }

		[Export ("closedCaptionDisplayEnabled")]
		bool ClosedCaptionDisplayEnabled { [Bind ("isClosedCaptionDisplayEnabled")] get; set;  }

		[Static, Export ("playerWithURL:")]
		AVPlayer FromUrl (NSUrl URL);

		[Static]
		[Export ("playerWithPlayerItem:")]
		AVPlayer FromPlayerItem ([NullAllowed] AVPlayerItem item);

		[Export ("initWithURL:")]
		IntPtr Constructor (NSUrl URL);

		[Export ("initWithPlayerItem:")]
		IntPtr Constructor ([NullAllowed] AVPlayerItem item);

		[Export ("play")]
		void Play ();

		[Export ("pause")]
		void Pause ();
		
		[iOS (10,0), TV(10,0), Mac (10,12)]
		[Export ("timeControlStatus")]
		AVPlayerTimeControlStatus TimeControlStatus { get; }

		[iOS (10,0), TV(10,0), Mac (10,12)]
		[NullAllowed, Export ("reasonForWaitingToPlay")]
		string ReasonForWaitingToPlay { get; }

		[iOS (10,0), TV(10,0), Mac (10,12)]
		[Export ("playImmediatelyAtRate:")]
		void PlayImmediatelyAtRate (float rate);

		[Export ("replaceCurrentItemWithPlayerItem:")]
		void ReplaceCurrentItemWithPlayerItem ([NullAllowed] AVPlayerItem item);

		[Export ("addPeriodicTimeObserverForInterval:queue:usingBlock:")]
#if XAMCORE_2_0
		NSObject AddPeriodicTimeObserver (CMTime interval, [NullAllowed] DispatchQueue queue, Action<CMTime> handler);
#else
		NSObject AddPeriodicTimeObserver (CMTime interval, [NullAllowed] DispatchQueue queue, AVTimeHandler handler);
#endif

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

#if !MONOMAC
		// 5.0
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'AllowsExternalPlayback' instead.")]
		[Export ("allowsAirPlayVideo")]
		bool AllowsAirPlayVideo { get; set;  }

		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'ExternalPlaybackActive' instead.")]
		[Export ("airPlayVideoActive")]
		bool AirPlayVideoActive { [Bind ("isAirPlayVideoActive")] get;  }

		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'UsesExternalPlaybackWhileExternalScreenIsActive' instead.")]
		[Export ("usesAirPlayVideoWhileAirPlayScreenIsActive")]
		bool UsesAirPlayVideoWhileAirPlayScreenIsActive { get; set;  }
#endif

		[Export ("seekToTime:completionHandler:")]
		[Async]
		void Seek (CMTime time, AVCompletion completion);

		[Export ("seekToTime:toleranceBefore:toleranceAfter:completionHandler:")]
		[Async]
		void Seek (CMTime time, CMTime toleranceBefore, CMTime toleranceAfter, AVCompletion completion);

		[iOS (6,0), Mac (10,9)]
		[Export ("seekToDate:")]
		void Seek (NSDate date);

		[iOS (6,0)]
		[Mac (10,9)] // Header says 10.7, but it's a lie
		[Export ("seekToDate:completionHandler:")]
		[Async]
		void Seek (NSDate date, AVCompletion onComplete);

		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Export ("automaticallyWaitsToMinimizeStalling")]
		bool AutomaticallyWaitsToMinimizeStalling { get; set; }
		
		[iOS (6,0)]
		[Export ("setRate:time:atHostTime:")]
		void SetRate (float /* defined as 'float' */ rate, CMTime itemTime, CMTime hostClockTime);

		[iOS (6,0)]
		[Export ("prerollAtRate:completionHandler:")]
		[Async]
		void Preroll (float /* defined as 'float' */ rate, AVCompletion onComplete);

		[iOS (6,0)]
		[Export ("cancelPendingPrerolls")]
		void CancelPendingPrerolls ();

		[iOS (6,0), Mac (10,12)]
		[Export ("outputObscuredDueToInsufficientExternalProtection")]
		bool OutputObscuredDueToInsufficientExternalProtection { get; }

		[iOS (6,0)]
		[Export ("masterClock"), NullAllowed]
		CMClock MasterClock { get; set; }

		[iOS (6,0), Mac (10,11)]
		[Export ("allowsExternalPlayback")]
		bool AllowsExternalPlayback { get; set;  }

		[iOS (6,0), Mac (10,11)]
		[Export ("externalPlaybackActive")]
		bool ExternalPlaybackActive { [Bind ("isExternalPlaybackActive")] get; }

#if !MONOMAC
		[iOS (6,0)]
		[Export ("usesExternalPlaybackWhileExternalScreenIsActive")]
		bool UsesExternalPlaybackWhileExternalScreenIsActive { get; set;  }
#endif

		[Mac (10, 9)]
		[iOS (6,0)][Protected]
		[NullAllowed] // by default this property is null
		[Export ("externalPlaybackVideoGravity", ArgumentSemantic.Copy)]
		NSString WeakExternalPlaybackVideoGravity { get; set; }

		[iOS (7,0)][Mac (10, 7)]
		[Export ("volume")]
		float Volume { get; set; } // defined as 'float'

		[iOS (7,0)][Mac (10, 7)]
		[Export ("muted")]
		bool Muted { [Bind ("isMuted")] get; set; }

		[iOS (7,0)][Mac (10,9)]
		[Export ("appliesMediaSelectionCriteriaAutomatically")]
		bool AppliesMediaSelectionCriteriaAutomatically { get; set; }

		[iOS (7,0)][Mac (10,9)]
		[return: NullAllowed]
		[Export ("mediaSelectionCriteriaForMediaCharacteristic:")]
		AVPlayerMediaSelectionCriteria MediaSelectionCriteriaForMediaCharacteristic (NSString avMediaCharacteristic);

		[iOS (7, 0)][Mac (10,9)]
		[Export ("setMediaSelectionCriteria:forMediaCharacteristic:")]
		void SetMediaSelectionCriteria ([NullAllowed] AVPlayerMediaSelectionCriteria criteria, NSString avMediaCharacteristic);

#if MONOMAC
		[Mac (10,9)]
		[Export ("audioOutputDeviceUniqueID"), NullAllowed]
		string AudioOutputDeviceUniqueID { get; set; }
#endif

		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Field ("AVPlayerWaitingToMinimizeStallsReason")]
		NSString WaitingToMinimizeStallsReason { get; }

		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Field ("AVPlayerWaitingWhileEvaluatingBufferingRateReason")]
		NSString WaitingWhileEvaluatingBufferingRateReason { get; }

		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Field ("AVPlayerWaitingWithNoItemToPlayReason")]
		NSString WaitingWithNoItemToPlayReason { get; }

		// From AVPlayer (AVPlayerPlaybackCapabilities) Category

		[TV (11,2), NoWatch, NoMac, iOS (11,2)]
		[Static]
		[Export ("availableHDRModes")]
		AVPlayerHdrMode AvailableHdrModes { get; }

		[TV (11, 2), NoWatch, NoMac, iOS (11, 2)]
		[Field ("AVPlayerAvailableHDRModesDidChangeNotification")]
		[Notification]
		NSString AvailableHdrModesDidChangeNotification { get; }

		// From AVPlayer (AVPlayerVideoDecoderGPUSupport) Category

		[NoWatch, NoTV, NoiOS, Mac (10,13,4)]
		[Export ("preferredVideoDecoderGPURegistryID")]
		ulong PreferredVideoDecoderGpuRegistryId { get; set; }

		// From AVPlayerVideoDisplaySleepPrevention (AVPlayer) Category

		[TV (12, 0), NoWatch, Mac (10, 14, onlyOn64: true), iOS (12, 0)]
		[Export ("preventsDisplaySleepDuringVideoPlayback")]
		bool PreventsDisplaySleepDuringVideoPlayback { get; set; }
	}

	[NoWatch]
	[iOS (7,0), Mac (10, 9)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerMediaSelectionCriteria {
		[Export ("preferredLanguages"), NullAllowed]
		string [] PreferredLanguages { get; }

		[Export ("preferredMediaCharacteristics"), NullAllowed]
		NSString [] PreferredMediaCharacteristics { get; }

		[Export ("initWithPreferredLanguages:preferredMediaCharacteristics:")]
		IntPtr Constructor ([NullAllowed] string [] preferredLanguages, [NullAllowed] NSString [] preferredMediaCharacteristics);
	}

	[NoWatch]
	[iOS (6,0), Mac (10, 9)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSGenericException *** -[AVTextStyleRule init] Not available.  Use initWithTextMarkupAttributes:textSelector: instead
	interface AVTextStyleRule : NSCopying {
		[Export ("textMarkupAttributes")][Protected]
		NSDictionary WeakTextMarkupAttributes { get;  }

		[Wrap ("WeakTextMarkupAttributes")]
		CMTextMarkupAttributes TextMarkupAttributes { get;  }

		[Export ("textSelector"), NullAllowed]
		string TextSelector { get;  }

		[Static]
		[Export ("propertyListForTextStyleRules:")]
		NSObject ToPropertyList (AVTextStyleRule [] textStyleRules);

		[return: NullAllowed]
		[Static]
		[Export ("textStyleRulesFromPropertyList:")]
		AVTextStyleRule [] FromPropertyList (NSObject plist);

		[return: NullAllowed]
		[Static][Internal]
		[Export ("textStyleRuleWithTextMarkupAttributes:")]
		AVTextStyleRule FromTextMarkupAttributes (NSDictionary textMarkupAttributes);

		[return: NullAllowed]
		[Static]
		[Wrap ("FromTextMarkupAttributes (textMarkupAttributes == null ? null : textMarkupAttributes.Dictionary)")]
		AVTextStyleRule FromTextMarkupAttributes (CMTextMarkupAttributes textMarkupAttributes);

		[return: NullAllowed]
		[Static][Internal]
		[Export ("textStyleRuleWithTextMarkupAttributes:textSelector:")]
		AVTextStyleRule FromTextMarkupAttributes (NSDictionary textMarkupAttributes, [NullAllowed] string textSelector);

		[return: NullAllowed]
		[Static]
		[Wrap ("FromTextMarkupAttributes (textMarkupAttributes == null ? null : textMarkupAttributes.Dictionary, textSelector)")]
		AVTextStyleRule FromTextMarkupAttributes (CMTextMarkupAttributes textMarkupAttributes, [NullAllowed] string textSelector);

		[Export ("initWithTextMarkupAttributes:")]
		[Protected]
		IntPtr Constructor (NSDictionary textMarkupAttributes);

		[Wrap ("this (attributes == null ? null : attributes.Dictionary)")]
		IntPtr Constructor (CMTextMarkupAttributes attributes);

		[DesignatedInitializer]
		[Export ("initWithTextMarkupAttributes:textSelector:")]
		[Protected]
		IntPtr Constructor (NSDictionary textMarkupAttributes, [NullAllowed] string textSelector);
	
		[Wrap ("this (attributes == null ? null : attributes.Dictionary, textSelector)")]
		IntPtr Constructor (CMTextMarkupAttributes attributes, string textSelector);
	}

	[NoWatch]
	[iOS (9,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface AVMetadataGroup {
		
		[Export ("items", ArgumentSemantic.Copy)]
		AVMetadataItem[] Items { get; }

		[iOS (9,3)][Mac (10,12)]
		[TV (9,2)]
		[NullAllowed, Export ("classifyingLabel")]
		string ClassifyingLabel { get; }

		[iOS (9,3)]
		[TV (9,2)]
		[Mac (10,12)]
		[NullAllowed, Export ("uniqueID")]
		string UniqueID { get; }
	}

	[NoWatch]
	[BaseType (typeof (AVMetadataGroup))]
	interface AVTimedMetadataGroup : NSMutableCopying {
		[Export ("timeRange")]
		CMTimeRange TimeRange { get; [NotImplemented] set; }

		[Export ("items", ArgumentSemantic.Copy)]
		AVMetadataItem [] Items { get; [NotImplemented] set; }

		[Export ("initWithItems:timeRange:")]
		IntPtr Constructor (AVMetadataItem [] items, CMTimeRange timeRange);

		[return: NullAllowed]
		[iOS (8,0)][Mac (10,10)]
		[Export ("copyFormatDescription")]
		CMFormatDescription CopyFormatDescription ();

		[iOS (8,0)][Mac (10,10)]
		[Export ("initWithSampleBuffer:")]
		IntPtr Constructor (CMSampleBuffer sampleBuffer);
	}

	[NoWatch]
	[BaseType (typeof (AVTimedMetadataGroup))]
	interface AVMutableTimedMetadataGroup {
		[NullAllowed] // by default this property is null
		[Export ("items", ArgumentSemantic.Copy)]
		[Override]
		AVMetadataItem [] Items { get; set;  }

		[Export ("timeRange")]
		[Override]
		CMTimeRange TimeRange { get; set; }
	}

#if !XAMCORE_2_0
	delegate void AVTimeHandler (CMTime time);
#endif

	interface AVPlayerItemErrorEventArgs {
		[Export ("AVPlayerItemFailedToPlayToEndTimeErrorKey")]
		NSError Error { get; }
	}
		
	[NoWatch]
	[BaseType (typeof (NSObject))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVPlayerItem : NSCopying {
		[Export ("status")]
		AVPlayerItemStatus Status { get;  }

		[Export ("asset")]
		AVAsset Asset { get;  }

		[Export ("tracks")]
		AVPlayerItemTrack [] Tracks { get;  }

		[Export ("presentationSize")]
		CGSize PresentationSize { get;  }

		[Export ("forwardPlaybackEndTime")]
		CMTime ForwardPlaybackEndTime { get; set;  }

		[Export ("reversePlaybackEndTime")]
		CMTime ReversePlaybackEndTime { get; set;  }

		[Export ("audioMix", ArgumentSemantic.Copy), NullAllowed]
		AVAudioMix AudioMix { get; set;  }

		[Export ("videoComposition", ArgumentSemantic.Copy), NullAllowed]
		AVVideoComposition VideoComposition { get; set;  }

		[Export ("currentTime")]
		CMTime CurrentTime { get; }

		[Export ("playbackLikelyToKeepUp")]
		bool PlaybackLikelyToKeepUp { [Bind ("isPlaybackLikelyToKeepUp")] get;  }

		[Export ("playbackBufferFull")]
		bool PlaybackBufferFull { [Bind ("isPlaybackBufferFull")] get;  }

		[Export ("playbackBufferEmpty")]
		bool PlaybackBufferEmpty { [Bind ("isPlaybackBufferEmpty")] get;  }

		[iOS (9,0), Mac (10,11)]
		[Export ("canUseNetworkResourcesForLiveStreamingWhilePaused", ArgumentSemantic.Assign)]
		bool CanUseNetworkResourcesForLiveStreamingWhilePaused { get; set; }
		
		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Export ("preferredForwardBufferDuration")]
		double PreferredForwardBufferDuration { get; set; }

		[Export ("seekableTimeRanges"), NullAllowed]
		NSValue [] SeekableTimeRanges { get;  }

		[Export ("loadedTimeRanges")]
		NSValue [] LoadedTimeRanges { get;  }

		[Export ("timedMetadata"), NullAllowed]
		NSObject [] TimedMetadata { get;  }

		[Static, Export ("playerItemWithURL:")]
		AVPlayerItem FromUrl (NSUrl URL);

		[Static]
		[Export ("playerItemWithAsset:")]
		AVPlayerItem FromAsset ([NullAllowed] AVAsset asset);

		[Export ("initWithURL:")]
		IntPtr Constructor (NSUrl URL);

		[Export ("initWithAsset:")]
		IntPtr Constructor (AVAsset asset);

		[Export ("stepByCount:")]
		void StepByCount (nint stepCount);

		[Export ("seekToDate:")]
		bool Seek (NSDate date);

		[Export ("seekToTime:")]
		void Seek (CMTime time);
		
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
		bool CanPlayFastReverse { get;  }

		[Export ("canPlayFastForward")]
		bool CanPlayFastForward { get; }

		[Field ("AVPlayerItemTimeJumpedNotification")]
		[Notification]
		NSString TimeJumpedNotification { get; }

		[Export ("seekToTime:completionHandler:")]
		[Async]
		void Seek (CMTime time, AVCompletion completion);

		[Export ("cancelPendingSeeks")]
		void CancelPendingSeeks ();

		[Export ("seekToTime:toleranceBefore:toleranceAfter:completionHandler:")]
		[Async]
		void Seek (CMTime time, CMTime toleranceBefore, CMTime toleranceAfter, AVCompletion completion);

		[Mac (10, 8)]
		[Export ("selectMediaOption:inMediaSelectionGroup:")]
		void SelectMediaOption ([NullAllowed] AVMediaSelectionOption mediaSelectionOption, AVMediaSelectionGroup mediaSelectionGroup);

		[return: NullAllowed]
		[Mac (10, 8)]
		[Export ("selectedMediaOptionInMediaSelectionGroup:")]
		AVMediaSelectionOption SelectedMediaOption (AVMediaSelectionGroup inMediaSelectionGroup);

		[iOS (9,0), MacAttribute (10,11)]
		[Export ("currentMediaSelection")]
		AVMediaSelection CurrentMediaSelection { get; }

		[iOS (6,0)]
		[Export ("canPlaySlowForward")]
		bool CanPlaySlowForward { get;  }

		[iOS (6,0)]
		[Export ("canPlayReverse")]
		bool CanPlayReverse { get;  }

		[iOS (6,0)]
		[Export ("canPlaySlowReverse")]
		bool CanPlaySlowReverse { get;  }

		[iOS (6,0)]
		[Export ("canStepForward")]
		bool CanStepForward { get;  }

		[iOS (6,0)]
		[Export ("canStepBackward")]
		bool CanStepBackward { get;  }
		
		[iOS (6,0)]
		[Export ("outputs")]
		AVPlayerItemOutput [] Outputs { get;  }

		[iOS (6,0)]
		[Export ("addOutput:")]
		[PostGet ("Outputs")]
		void AddOutput (AVPlayerItemOutput output);

		[iOS (6,0)]
		[Export ("removeOutput:")]
		[PostGet ("Outputs")]
		void RemoveOutput (AVPlayerItemOutput output);

		[iOS (6,0)]
		[Export ("timebase"), NullAllowed]
		CMTimebase Timebase { get;  }

		[iOS (6,0), Mac (10, 9)]
		[Export ("seekToDate:completionHandler:")]
		[Async]
		bool Seek (NSDate date, AVCompletion completion);

		[iOS (6,0), Mac (10, 9)]
		[Export ("seekingWaitsForVideoCompositionRendering")]
		bool SeekingWaitsForVideoCompositionRendering { get; set;  }

		[iOS (6,0), Mac (10, 9)]
		[Export ("textStyleRules", ArgumentSemantic.Copy), NullAllowed]
		AVTextStyleRule [] TextStyleRules { get; set;  }

		[iOS (6,0), Mac (10, 9)]
		[Field ("AVPlayerItemPlaybackStalledNotification")]
		[Notification]
		NSString PlaybackStalledNotification { get; }
		
		[iOS (6,0), Mac (10, 9)]
		[Field ("AVPlayerItemNewAccessLogEntryNotification")]
		[Notification]
		NSString NewAccessLogEntryNotification { get; }
		
		[iOS (6,0), Mac (10, 9)]
		[Field ("AVPlayerItemNewErrorLogEntryNotification")]
		[Notification]
		NSString NewErrorLogEntryNotification { get; }

		[iOS (7,0), Mac (10, 9)]
		[Static, Export ("playerItemWithAsset:automaticallyLoadedAssetKeys:")]
		AVPlayerItem FromAsset ([NullAllowed] AVAsset asset, [NullAllowed] NSString [] automaticallyLoadedAssetKeys);

		[iOS (7,0), Mac (10, 9)]
		[DesignatedInitializer]
		[Export ("initWithAsset:automaticallyLoadedAssetKeys:")]
		IntPtr Constructor (AVAsset asset, [NullAllowed] params NSString [] automaticallyLoadedAssetKeys);

		[iOS (7,0), Mac (10, 9)]
		[Export ("automaticallyLoadedAssetKeys", ArgumentSemantic.Copy)]
		NSString [] AutomaticallyLoadedAssetKeys { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("customVideoCompositor", ArgumentSemantic.Copy), NullAllowed]
		[Protocolize]
		AVVideoCompositing CustomVideoCompositor { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		// DOC: Mention this is an AVAudioTimePitch constant
		NSString AudioTimePitchAlgorithm { get; set; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("selectMediaOptionAutomaticallyInMediaSelectionGroup:")]
		void SelectMediaOptionAutomaticallyInMediaSelectionGroup (AVMediaSelectionGroup mediaSelectionGroup);

		[iOS (8,0)][Mac (10,10)]
		[Export ("preferredPeakBitRate")]
		double PreferredPeakBitRate { get; set; }

		[iOS (11,0)][TV (11,0)][Mac (10,13)]
		[Export ("preferredMaximumResolution", ArgumentSemantic.Assign)]
		CGSize PreferredMaximumResolution { get; set; }

#region AVPlayerViewControllerAdditions
		[NoiOS][NoMac]
		[TV (9,0)]
		[Export ("navigationMarkerGroups", ArgumentSemantic.Copy)]
		AVNavigationMarkersGroup[] NavigationMarkerGroups { get; set; }

		[NoiOS][NoMac]
		[TV (9,0)]
		[Export ("externalMetadata", ArgumentSemantic.Copy)]
		AVMetadataItem[] ExternalMetadata { get; set; }

		[NoiOS][NoMac]
		[TV (9,0)]
		[Export ("interstitialTimeRanges", ArgumentSemantic.Copy)]
		AVInterstitialTimeRange[] InterstitialTimeRanges { get; set; }
#endregion

		[iOS (9,3)][Mac (10,12)]
		[TV (9,2)]
		[Export ("addMediaDataCollector:")]
		void AddMediaDataCollector (AVPlayerItemMediaDataCollector collector);
		
		[iOS (9,3)][Mac (10,12)]
		[TV (9,2)]
		[Export ("removeMediaDataCollector:")]
		void RemoveMediaDataCollector (AVPlayerItemMediaDataCollector collector);
		
		[iOS (9,3)][Mac (10,12)]
		[TV (9,2)]
		[Export ("mediaDataCollectors")]
		AVPlayerItemMediaDataCollector[] MediaDataCollectors { get; }

#if !MONOMAC
		[NoiOS, TV (10, 0), NoWatch, NoMac]
		[NullAllowed, Export ("nextContentProposal", ArgumentSemantic.Assign)]
		AVContentProposal NextContentProposal { get; set; }
#endif

		[TV (11, 0), NoWatch, Mac (10, 13), iOS (11, 0)]
		[Internal]
		[Export ("videoApertureMode")]
		NSString _VideoApertureMode { get; set; }
	}

	[NoiOS][NoTV][NoWatch]
	[Mac (10, 7)]
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
	[iOS (6,0)]
	[BaseType (typeof (NSObject)), Mac (10, 8)]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** initialization method -init cannot be sent to an abstract object of class AVPlayerItemOutput: Create a concrete instance!
	[DisableDefaultCtor]
	interface AVPlayerItemOutput {
		[Export ("itemTimeForHostTime:")]
		CMTime GetItemTime (double hostTimeInSeconds);

		[Export ("itemTimeForMachAbsoluteTime:")]
		CMTime GetItemTime (long machAbsoluteTime);

		[Export ("suppressesPlayerRendering")]
		bool SuppressesPlayerRendering { get; set; }

		[NoiOS][NoTV]
		[Mac (10,8)]
		[Export ("itemTimeForCVTimeStamp:")]
		CMTime GetItemTime (CVTimeStamp timestamp);
	}

	[NoWatch]
	[iOS (9,3)]
	[TV (9,2)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // initialization method -init cannot be sent to an abstract object of class AVPlayerItemMediaDataCollector: Create a concrete instance!
	[Abstract]
	interface AVPlayerItemMediaDataCollector
	{
	}

	[NoWatch]
	[iOS (8,0)][Mac (10,10)]
	[BaseType (typeof (AVPlayerItemOutput))]
	interface AVPlayerItemMetadataOutput {

		[DesignatedInitializer]
		[Export ("initWithIdentifiers:")]
		IntPtr Constructor ([NullAllowed] NSString [] metadataIdentifiers);

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		AVPlayerItemMetadataOutputPushDelegate Delegate { get; }

		[Export ("delegateQueue"), NullAllowed]
		DispatchQueue DelegateQueue { get; }

		[Export ("advanceIntervalForDelegateInvocation")]
		double AdvanceIntervalForDelegateInvocation { get; set; }

		[Export ("setDelegate:queue:")]
		void SetDelegate ([Protocolize] AVPlayerItemMetadataOutputPushDelegate pushDelegate, DispatchQueue delegateQueue);
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	[iOS (8,0)][Mac (10,10)]
	[Protocol, Model]
	interface AVPlayerItemMetadataOutputPushDelegate : AVPlayerItemOutputPushDelegate {

		[iOS (8,0)]
		[Export ("metadataOutput:didOutputTimedMetadataGroups:fromPlayerItemTrack:")]
		void DidOutputTimedMetadataGroups (AVPlayerItemMetadataOutput output, AVTimedMetadataGroup [] groups, AVPlayerItemTrack track);
	}

	[iOS (10,0)]
	[Mac (10,12)]
	[TV (10,0)]
	[NoWatch]
	[Static]
	interface AVVideoColorPrimaries {
		[Field ("AVVideoColorPrimaries_ITU_R_709_2")]
		NSString Itu_R_709_2 { get; }

		[NoiOS, NoTV]
		[Field ("AVVideoColorPrimaries_EBU_3213")]
		NSString Ebu_3213 { get; }

		[Field ("AVVideoColorPrimaries_SMPTE_C")]
		NSString Smpte_C { get; }

		[Field ("AVVideoColorPrimaries_P3_D65")]
		NSString P3_D65 { get; }

		[iOS (11,0), Mac (10,13), TV (11,0)]
		[Field ("AVVideoColorPrimaries_ITU_R_2020")]
		NSString Itu_R_2020 { get; }
	}

	[NoWatch]
	[Static]
	interface AVVideoTransferFunction {
		[iOS (10, 0)]
		[Field ("AVVideoTransferFunction_ITU_R_709_2")]
		NSString AVVideoTransferFunction_Itu_R_709_2 { get; }

		[NoiOS, NoTV, Mac (10,12)]
		[Field ("AVVideoTransferFunction_SMPTE_240M_1995")]
		NSString AVVideoTransferFunction_Smpte_240M_1995 { get; }
	}
	
	[NoWatch]
	[Static]
	interface AVVideoYCbCrMatrix {
		
		[iOS (10, 0)]
		[Field ("AVVideoYCbCrMatrix_ITU_R_709_2")]
		NSString Itu_R_709_2 { get; }

		[iOS (10, 0)]
		[Field ("AVVideoYCbCrMatrix_ITU_R_601_4")]
		NSString Itu_R_601_4 { get; }

		[NoiOS, NoTV, Mac (10,12)]
		[Field ("AVVideoYCbCrMatrix_SMPTE_240M_1995")]
		NSString Smpte_240M_1995 { get; }

		[iOS (11, 0), TV (11, 0), Mac (10,13)]
		[Field ("AVVideoYCbCrMatrix_ITU_R_2020")]
		NSString Itu_R_2020 { get; }

	}
	
	[NoWatch]
	[StrongDictionary ("AVColorPropertiesKeys")]
	interface AVColorProperties {
		NSString AVVideoColorPrimaries { get; set; }
		NSString AVVideoTransferFunction { get; set; } 
		NSString AVVideoYCbCrMatrix { get; }
	}

	[NoWatch]
	[Static]
	[Internal]
	interface AVColorPropertiesKeys {
		[iOS (10, 0)]
		[Field ("AVVideoColorPrimariesKey")]
		NSString AVVideoColorPrimariesKey { get; }

		[iOS (10, 0)]
		[Field ("AVVideoTransferFunctionKey")]
		NSString AVVideoTransferFunctionKey { get; }

		[iOS (10, 0)]
		[Field ("AVVideoYCbCrMatrixKey")]
		NSString AVVideoYCbCrMatrixKey { get; }
	}
	
	[NoWatch]
	[StrongDictionary ("AVCleanAperturePropertiesKeys")]
	interface AVCleanApertureProperties {
		NSNumber Width { get; set; }
		NSNumber Height { get; set; }
		NSNumber HorizontalOffset { get; set; }
		NSNumber VerticalOffset { get; set; }
	}
		
	[NoWatch]
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
	[StrongDictionary ("AVPixelAspectRatioPropertiesKeys")]
	interface AVPixelAspectRatioProperties {
		NSNumber PixelAspectRatioHorizontalSpacing { get; set; }
		NSNumber PixelAspectRatioVerticalSpacing { get; set; }
	}

	[NoWatch]
	[Internal]
	[Static]
	interface AVPixelAspectRatioPropertiesKeys {
		[Field ("AVVideoPixelAspectRatioHorizontalSpacingKey")]
		NSString PixelAspectRatioHorizontalSpacingKey { get; }
		
		[Field ("AVVideoPixelAspectRatioVerticalSpacingKey")]
		NSString PixelAspectRatioVerticalSpacingKey { get; }
	}

	[NoWatch]
	[StrongDictionary ("AVCompressionPropertiesKeys")]
	interface AVCompressionProperties {
		AVCleanApertureProperties CleanAperture { get; set; }
		AVPixelAspectRatioProperties PixelAspectRatio { get; set; }
	}

	[NoWatch]
	[Static]
	[Internal]
	interface AVCompressionPropertiesKeys {
		[Field ("AVVideoCleanApertureKey")]
		NSString CleanApertureKey { get; }
		
		[Field ("AVVideoPixelAspectRatioKey")]
		NSString PixelAspectRatioKey { get; }
	}

	[NoWatch]
	[StrongDictionary ("AVPlayerItemVideoOutputSettingsKeys")]
	interface AVPlayerItemVideoOutputSettings {

		[iOS (10,0)]
		[TV (10,0)]
		AVColorProperties ColorProperties { get; set; }

		AVCompressionProperties CompressionProperties { get; set; }

		[iOS (10,0)]
		[TV (10,0)][Mac (10,12)]
		bool AllowWideColor { get; set; }

		NSString Codec { get; set; }
		NSString ScalingMode { get; set; }
		NSNumber Width { get; set; }
		NSNumber Height { get; set; }
	}

	[NoWatch]
	[Static]
	[Internal]
	interface AVPlayerItemVideoOutputSettingsKeys {
		[iOS (10,0)]
		[TV (10,0)]
		[Field ("AVVideoColorPropertiesKey")]
		NSString ColorPropertiesKey { get; }
		
		[Field ("AVVideoCompressionPropertiesKey")]
		NSString CompressionPropertiesKey { get; }
		
		[iOS (10,0), Mac (10,12)]
		[TV (10,0)]
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
	[iOS (6,0)]
	[BaseType (typeof (AVPlayerItemOutput))]
	interface AVPlayerItemVideoOutput {
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get;  }
		
		[Wrap ("WeakDelegate")]
		[Protocolize]
		AVPlayerItemOutputPullDelegate Delegate  { get;  }

		[Export ("delegateQueue"), NullAllowed]
		DispatchQueue DelegateQueue { get;  }

		[Internal]
		[Export ("initWithPixelBufferAttributes:")]
		IntPtr _FromPixelBufferAttributes ([NullAllowed] NSDictionary pixelBufferAttributes);

		[Internal]
		[Export ("initWithOutputSettings:")]
		IntPtr _FromOutputSettings ([NullAllowed] NSDictionary outputSettings);

		[DesignatedInitializer]
		[Wrap ("this (attributes == null ? null : attributes.Dictionary, AVPlayerItemVideoOutput.InitMode.PixelAttributes)")]
		IntPtr Constructor (CVPixelBufferAttributes attributes);
		
		[DesignatedInitializer]
		[iOS (10,0), TV (10,0), Mac (10,12)]
		[Wrap ("this (settings == null ? null : settings.Dictionary, AVPlayerItemVideoOutput.InitMode.OutputSettings)")]
		IntPtr Constructor (AVPlayerItemVideoOutputSettings settings);

		[Export ("hasNewPixelBufferForItemTime:")]
		bool HasNewPixelBufferForItemTime (CMTime itemTime);

		[Protected]
		[Export ("copyPixelBufferForItemTime:itemTimeForDisplay:")]
		IntPtr WeakCopyPixelBuffer (CMTime itemTime, ref CMTime outItemTimeForDisplay);

		[Export ("setDelegate:queue:")]
		void SetDelegate ([Protocolize] [NullAllowed] AVPlayerItemOutputPullDelegate delegateClass, [NullAllowed] DispatchQueue delegateQueue);

		[Export ("requestNotificationOfMediaDataChangeWithAdvanceInterval:")]
		void RequestNotificationOfMediaDataChange (double advanceInterval);
	}

	[NoWatch]
	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVPlayerItemOutputPullDelegate {
		[Export ("outputMediaDataWillChange:")]
		void OutputMediaDataWillChange (AVPlayerItemOutput sender);

		[Export ("outputSequenceWasFlushed:")]
		void OutputSequenceWasFlushed (AVPlayerItemOutput output);
	}

	[NoWatch]
	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVPlayerItemOutputPushDelegate {
		[Export ("outputSequenceWasFlushed:")]
		void OutputSequenceWasFlushed (AVPlayerItemOutput output);
	}

	[NoWatch]
	[iOS (7,0)]
	[BaseType (typeof (AVPlayerItemOutputPushDelegate))]
	[Model]
	[Protocol]
	interface AVPlayerItemLegibleOutputPushDelegate  {
		[Export ("legibleOutput:didOutputAttributedStrings:nativeSampleBuffers:forItemTime:")]
		void DidOutputAttributedStrings (AVPlayerItemLegibleOutput output, NSAttributedString [] strings, CMSampleBuffer [] nativeSamples, CMTime itemTime);		
	}

	[NoWatch]
	[iOS (7,0), Mac (10, 9)]
	[BaseType (typeof (AVPlayerItemOutput))]
	interface AVPlayerItemLegibleOutput {
		[Export ("initWithMediaSubtypesForNativeRepresentation:")]
		IntPtr Constructor (NSNumber [] subtypesFourCCcodes);
		
		[Export ("setDelegate:queue:")]
		void SetDelegate ([Protocolize]AVPlayerItemLegibleOutputPushDelegate delegateObject, DispatchQueue delegateQueue);
	
		[Export ("delegate", ArgumentSemantic.Copy)]
		[Protocolize]
		AVPlayerItemLegibleOutputPushDelegate Delegate { get; }
	
		[Export ("delegateQueue", ArgumentSemantic.Copy)]
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

	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemAccessLog : NSCopying {
		
		[Export ("events")]
		AVPlayerItemAccessLogEvent [] Events { get; }

		[Export ("extendedLogDataStringEncoding")]
		NSStringEncoding ExtendedLogDataStringEncoding { get; }

		[Export ("extendedLogData"), NullAllowed]
		NSData ExtendedLogData { get; }
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemErrorLog : NSCopying {
		[Export ("events")]
		AVPlayerItemErrorLogEvent [] Events { get; }

		[Export ("extendedLogDataStringEncoding")]
		NSStringEncoding ExtendedLogDataStringEncoding { get; }

		[Export ("extendedLogData"), NullAllowed]
		NSData ExtendedLogData { get; }
	}
	
	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemAccessLogEvent : NSCopying {
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'NumberOfMediaRequests' instead.")]
		[Mac (10, 7)]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use 'NumberOfMediaRequests' instead.")]
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

		[iOS (8,0), TV (9,0), NoWatch, Mac (10,10)]
		[Export ("indicatedBitrate")]
		double IndicatedBitrate { get; }

		[iOS (10, 0), TV (10,0), NoWatch, Mac (10, 12)]
		[Export ("indicatedAverageBitrate")]
		double IndicatedAverageBitrate { get; }

		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Export ("averageVideoBitrate")]
		double AverageVideoBitrate { get; }

		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Export ("averageAudioBitrate")]
		double AverageAudioBitrate { get; }

		[Export ("numberOfDroppedVideoFrames")]
		nint DroppedVideoFrameCount { get; }

		[iOS (6,0), Mac (10, 9)]
		[Export ("numberOfMediaRequests")]
		nint NumberOfMediaRequests { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("startupTime")]
		double StartupTime { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("downloadOverdue")]
		nint DownloadOverdue { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Export ("observedMaxBitrate")]
		double ObservedMaxBitrate { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Export ("observedMinBitrate")]
		double ObservedMinBitrate { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Export ("observedBitrateStandardDeviation")]
		double ObservedBitrateStandardDeviation { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Export ("playbackType", ArgumentSemantic.Copy), NullAllowed]
		string PlaybackType { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Export ("mediaRequestsWWAN")]
		nint MediaRequestsWWAN { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Export ("switchBitrate")]
		double SwitchBitrate { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("transferDuration")]
		double TransferDuration { get; }

	}

	[NoWatch]
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

	interface IAVPlayerItemMetadataCollectorPushDelegate {}

	[NoWatch]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface AVPlayerItemMetadataCollectorPushDelegate
	{
		[Abstract]
		[Export ("metadataCollector:didCollectDateRangeMetadataGroups:indexesOfNewGroups:indexesOfModifiedGroups:")]
		void DidCollectDateRange (AVPlayerItemMetadataCollector metadataCollector, AVDateRangeMetadataGroup[] metadataGroups, NSIndexSet indexesOfNewGroups, NSIndexSet indexesOfModifiedGroups);
	}

	[NoWatch]
	[iOS (9,3), Mac (10,11,3)]
	[TV (9,2)]
	[BaseType (typeof(AVPlayerItemMediaDataCollector))]
#if MONOMAC || XAMCORE_3_0 // Avoid breaking change in iOS
	[DisableDefaultCtor]
#endif
	interface AVPlayerItemMetadataCollector
	{
		[Export ("initWithIdentifiers:classifyingLabels:")]
		IntPtr Constructor ([NullAllowed] string[] identifiers, [NullAllowed] string[] classifyingLabels);

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
	[BaseType (typeof (CALayer))]
	interface AVPlayerLayer {
		[NullAllowed] // by default this property is null
		[Export ("player", ArgumentSemantic.Retain)]
		AVPlayer Player { get; set;  }

		[Static, Export ("playerLayerWithPlayer:")]
		AVPlayerLayer FromPlayer ([NullAllowed] AVPlayer player);

#if !XAMCORE_2_0
		[Advice ("Use 'LayerVideoGravity' property instead.")]
		[Export ("videoGravity", ArgumentSemantic.Copy)][Sealed]
		string VideoGravity { get; set; }
#endif

		[Export ("videoGravity", ArgumentSemantic.Copy)][Protected]
		NSString WeakVideoGravity { get; set; }

		[Field ("AVLayerVideoGravityResizeAspect")]
		NSString GravityResizeAspect { get; }

		[Field ("AVLayerVideoGravityResizeAspectFill")]
		NSString GravityResizeAspectFill { get; }

		[Field ("AVLayerVideoGravityResize")]
		NSString GravityResize { get; }

		[Export ("isReadyForDisplay")]
		bool ReadyForDisplay { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("videoRect")]
		CGRect VideoRect { get;  }

		[iOS (9,0), Mac (10,11)]
		[Export ("pixelBufferAttributes", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary WeakPixelBufferAttributes { get; set; }
	}

	[NoWatch]
	[iOS (10,0), TV (10,0), Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVPlayerLooper
	{
		[Static]
		[Export ("playerLooperWithPlayer:templateItem:timeRange:")]
		AVPlayerLooper FromPlayer (AVQueuePlayer player, AVPlayerItem itemToLoop, CMTimeRange loopRange);

		[Static]
		[Export ("playerLooperWithPlayer:templateItem:")]
		AVPlayerLooper FromPlayer (AVQueuePlayer player, AVPlayerItem itemToLoop);

		[Export ("initWithPlayer:templateItem:timeRange:")]
		[DesignatedInitializer]
		IntPtr Constructor (AVQueuePlayer player, AVPlayerItem itemToLoop, CMTimeRange loopRange);

#if !XAMCORE_4_0 // This API got introduced in Xcode 8.0 binding but is not currently present nor in Xcode 8.3 or Xcode 9.0 needs research
		[PostSnippet ("loopingEnabled = false;")]
#endif
		[Export ("disableLooping")]
		void DisableLooping ();

		[Export ("loopCount")]
		nint LoopCount { get; }

		[Export ("loopingPlayerItems")]
		AVPlayerItem[] LoopingPlayerItems { get; }

		[Export ("status")]
		AVPlayerLooperStatus Status { get; }

		[NullAllowed, Export ("error")]
		NSError Error { get; }
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemTrack {
		[Export ("enabled", ArgumentSemantic.Assign)]
		bool Enabled { [Bind ("isEnabled")] get; set;  }

		[Export ("assetTrack")]
		AVAssetTrack AssetTrack { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("currentVideoFrameRate")]
		float CurrentVideoFrameRate { get;  } // defined as 'float'

#if MONOMAC
		[Mac (10,10)]
		[Field ("AVPlayerItemTrackVideoFieldModeDeinterlaceFields")]
		NSString VideoFieldModeDeinterlaceFields { get; }

		[Mac (10,10)]
		[Export ("videoFieldMode"), NullAllowed]
		string VideoFieldMode { get; set; }
#endif
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVAsynchronousKeyValueLoading {
		[Abstract]
		[Export ("statusOfValueForKey:error:")]
#if XAMCORE_4_0
		AVKeyValueStatus StatusOfValueForKeyerror (string key, out NSError error);
#else
		AVKeyValueStatus StatusOfValueForKeyerror (string key, [NullAllowed] IntPtr outError);
#endif
		[Abstract]
		[Export ("loadValuesAsynchronouslyForKeys:completionHandler:")]
		void LoadValuesAsynchronously (string [] keys, [NullAllowed] Action handler);
	}

	[NoWatch]
	[BaseType (typeof (AVPlayer))]
	interface AVQueuePlayer {
		
		[Static, Export ("queuePlayerWithItems:")]
		AVQueuePlayer FromItems (AVPlayerItem [] items);

		[Export ("initWithItems:")]
		IntPtr Constructor (AVPlayerItem [] items);

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

	[Watch (3,0)]
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

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Field ("AVAudioFileTypeKey")]
		NSString FileTypeKey { get; }
		
		[Field ("AVEncoderAudioQualityKey")]
		NSString AVEncoderAudioQualityKey { get; }
		
		[Field ("AVEncoderBitRateKey")]
		NSString AVEncoderBitRateKey { get; }
		
		[Field ("AVEncoderBitRatePerChannelKey")]
		NSString AVEncoderBitRatePerChannelKey { get; }

		[iOS (7,0), Mac (10, 9)]
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

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVAudioBitRateStrategy_Constant"), Internal]
		NSString _Constant { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Field ("AVAudioBitRateStrategy_LongTermAverage"), Internal]
		NSString _LongTermAverage { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Field ("AVAudioBitRateStrategy_VariableConstrained"), Internal]
		NSString _VariableConstrained { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[Field ("AVAudioBitRateStrategy_Variable"), Internal]
		NSString _Variable { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVSampleRateConverterAlgorithm_Normal"), Internal]
		NSString AVSampleRateConverterAlgorithm_Normal { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVSampleRateConverterAlgorithm_Mastering"), Internal]
		NSString AVSampleRateConverterAlgorithm_Mastering { get; }
		
		[iOS (10, 0), TV (10,0), Watch (3,0), Mac (10,12)]
		[Field ("AVSampleRateConverterAlgorithm_MinimumPhase")]
		NSString AVSampleRateConverterAlgorithm_MinimumPhase { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("AVEncoderAudioQualityForVBRKey"), Internal]
		NSString AVEncoderAudioQualityForVBRKey { get; }
	}

	[NoWatch]
	[TV (10,2), iOS (8,0), Mac (10,10)]
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

#if !XAMCORE_4_0
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

#if !XAMCORE_4_0
		[Wrap ("RequestMediaData (queue, enqueuer)", IsVirtual = true)]
		[Obsolete ("Use the 'RequestMediaData' method instead.")]
		void RequestMediaDataWhenReadyOnQueue (DispatchQueue queue, Action enqueuer);
#endif

		[Export ("stopRequestingMediaData")]
		void StopRequestingMediaData ();

		// TODO: Remove (alongside others) when https://github.com/xamarin/xamarin-macios/issues/3213 is fixed and conformance to 'AVQueuedSampleBufferRendering' is restored.
		[TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("timebase", ArgumentSemantic.Retain)]
		CMTimebase Timebase { get; }

		[iOS (8, 0), Mac (10,10)]
		[Field ("AVSampleBufferDisplayLayerFailedToDecodeNotification")]
		[Notification]
		NSString FailedToDecodeNotification { get; }

		[iOS (8, 0), Mac (10,10)]
		[Field ("AVSampleBufferDisplayLayerFailedToDecodeNotificationErrorKey")]
		NSString FailedToDecodeNotificationErrorKey { get; }
	}

	[NoWatch]
	[BaseType (typeof (CALayer))]
	interface AVSynchronizedLayer {
		[Static, Export ("synchronizedLayerWithPlayerItem:")]
		AVSynchronizedLayer FromPlayerItem (AVPlayerItem playerItem);
		
		[NullAllowed] // by default this property is null
		[Export ("playerItem", ArgumentSemantic.Retain)]
		AVPlayerItem PlayerItem { get; set; }
	}

#if !MONOMAC
	[Watch (3,0)]
	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	interface AVSpeechSynthesisVoice : NSSecureCoding {

		[Static, Export ("speechVoices")]
		AVSpeechSynthesisVoice [] GetSpeechVoices ();

		[Static, Export ("currentLanguageCode")]
		string CurrentLanguageCode { get; }

		[return: NullAllowed]
		[Static, Export ("voiceWithLanguage:")]
		AVSpeechSynthesisVoice FromLanguage ([NullAllowed] string language);

		[iOS (9,0)]
		[return: NullAllowed]
		[Static, Export ("voiceWithIdentifier:")]
		AVSpeechSynthesisVoice FromIdentifier ([NullAllowed] string identifier);

		[Export ("language", ArgumentSemantic.Copy)]
		string Language { get; }

		[iOS (9,0)]
		[Export ("identifier")]
		string Identifier { get; }

		[iOS (9,0)]
		[Export ("name")]
		string Name { get; }

		[iOS (9,0)]
		[Export ("quality")]
		AVSpeechSynthesisVoiceQuality Quality { get; }

		[iOS (9,0)]
		[Field ("AVSpeechSynthesisVoiceIdentifierAlex")]
		NSString IdentifierAlex { get; }

		[iOS (10, 0), TV (10,0), Watch (3,0), NoMac]
		[Field ("AVSpeechSynthesisIPANotationAttribute")]
		NSString IpaNotationAttribute { get; }
	}

	[Watch (3,0)]
	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	interface AVSpeechUtterance : NSCopying, NSSecureCoding {

		[Static, Export ("speechUtteranceWithString:")]
		AVSpeechUtterance FromString (string speechString);
		
		[iOS (10,0)]
		[TV (10,0)]
		[Static]
		[Export ("speechUtteranceWithAttributedString:")]
		AVSpeechUtterance FromString (NSAttributedString speechString);

		[Export ("initWithString:")]
		IntPtr Constructor (string speechString);
		
		[iOS (10,0)]
		[TV (10,0)]
		[Export ("initWithAttributedString:")]
		IntPtr Constructor (NSAttributedString speechString);

		[NullAllowed] // by default this property is null
		[Export ("voice", ArgumentSemantic.Retain)]
		AVSpeechSynthesisVoice Voice { get; set; }

		[Export ("speechString", ArgumentSemantic.Copy)]
		string SpeechString { get; }
		
		[iOS (10, 0)]
		[TV (10,0)]
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
	}

	[Watch (3,0)]
	[iOS (7,0)]
	[BaseType (typeof (NSObject), Delegates=new string [] {"WeakDelegate"}, Events=new Type [] { typeof (AVSpeechSynthesizerDelegate)})]
	interface AVSpeechSynthesizer {

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		AVSpeechSynthesizerDelegate Delegate { get; set; }

		[Export ("speaking")]
		bool Speaking { [Bind ("isSpeaking")] get; }

		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; }

		[Export ("speakUtterance:")]
		void SpeakUtterance (AVSpeechUtterance utterance);

		[Export ("stopSpeakingAtBoundary:")]
		bool StopSpeaking (AVSpeechBoundary boundary);

		[Export ("pauseSpeakingAtBoundary:")]
		bool PauseSpeaking (AVSpeechBoundary boundary);

		[Export ("continueSpeaking")]
		bool ContinueSpeaking ();
		
		[iOS (10, 0)]
		[TV (10,0)]
		[NullAllowed, Export ("outputChannels", ArgumentSemantic.Retain)]
		AVAudioSessionChannelDescription[] OutputChannels { get; set; }
	}

	[Watch (3,0)]
	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
	interface AVSpeechSynthesizerDelegate {
		[Export ("speechSynthesizer:didStartSpeechUtterance:")][EventArgs ("AVSpeechSynthesizerUterance")]
		void DidStartSpeechUtterance (AVSpeechSynthesizer synthesizer, AVSpeechUtterance utterance);

		[Export ("speechSynthesizer:didFinishSpeechUtterance:")][EventArgs ("AVSpeechSynthesizerUterance")]
		void DidFinishSpeechUtterance (AVSpeechSynthesizer synthesizer, AVSpeechUtterance utterance);

		[Export ("speechSynthesizer:didPauseSpeechUtterance:")][EventArgs ("AVSpeechSynthesizerUterance")]
		void DidPauseSpeechUtterance (AVSpeechSynthesizer synthesizer, AVSpeechUtterance utterance);

		[Export ("speechSynthesizer:didContinueSpeechUtterance:")][EventArgs ("AVSpeechSynthesizerUterance")]
		void DidContinueSpeechUtterance (AVSpeechSynthesizer synthesizer, AVSpeechUtterance utterance);

		[Export ("speechSynthesizer:didCancelSpeechUtterance:")][EventArgs ("AVSpeechSynthesizerUterance")]
		void DidCancelSpeechUtterance (AVSpeechSynthesizer synthesizer, AVSpeechUtterance utterance);

		[Export ("speechSynthesizer:willSpeakRangeOfSpeechString:utterance:")][EventArgs ("AVSpeechSynthesizerWillSpeak")]
		void WillSpeakRangeOfSpeechString (AVSpeechSynthesizer synthesizer, NSRange characterRange, AVSpeechUtterance utterance);
	}

	[NoWatch, NoTV, NoMac, iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVAssetDownloadStorageManager
	{
		[Static]
		[Export ("sharedDownloadStorageManager")]
		AVAssetDownloadStorageManager SharedDownloadStorageManager { get; }

		[Export ("setStorageManagementPolicy:forURL:")]
		void SetStorageManagementPolicy (AVAssetDownloadStorageManagementPolicy storageManagementPolicy, NSUrl downloadStorageUrl);

		[Export ("storageManagementPolicyForURL:")]
		[return: NullAllowed]
		AVAssetDownloadStorageManagementPolicy GetStorageManagementPolicy (NSUrl downloadStorageUrl);
	}

	[NoWatch, NoTV, NoMac, iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVAssetDownloadStorageManagementPolicy : NSCopying, NSMutableCopying
	{
		[Internal]
		[Export ("priority")]
		NSString _Priority { get; [NotImplemented] set; }

		[Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; [NotImplemented] set; }
	}

	[NoWatch, NoTV, NoMac, iOS (11,0)]
	[BaseType (typeof(AVAssetDownloadStorageManagementPolicy))]
	[DisableDefaultCtor]
	interface AVMutableAssetDownloadStorageManagementPolicy
	{
		[Internal]
		[Export ("priority")]
		NSString _Priority { get; set; }

		[Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; set; }
	}

	[NoWatch]
	[NoTV]
	[iOS (9,0)]
	[NoMac]
	[BaseType (typeof (NSUrlSessionTask))]
	[DisableDefaultCtor] // not meant to be user createable
	interface AVAssetDownloadTask {
		
		[Export ("URLAsset")]
		AVUrlAsset UrlAsset { get; }

		[Availability (Introduced = Platform.iOS_9_0, Deprecated = Platform.iOS_10_0)]
		[Export ("destinationURL")]
		NSUrl DestinationUrl { get; }

		[NullAllowed, Export ("options")]
		NSDictionary<NSString, NSObject> Options { get; }

		[Export ("loadedTimeRanges")]
		NSValue[] LoadedTimeRanges { get; }

	}

	[NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(NSUrlSessionTask))]
	[DisableDefaultCtor]
	interface AVAggregateAssetDownloadTask
	{
		[Export ("URLAsset")]
		AVUrlAsset UrlAsset { get; }
	}

	[NoWatch]
	[Obsoleted (PlatformName.TvOS, 12, 0)]
	[Static, Internal]
	interface AVAssetDownloadTaskKeys {
		[iOS (9,0)]
		[Field ("AVAssetDownloadTaskMinimumRequiredMediaBitrateKey")]
		NSString MinimumRequiredMediaBitrateKey { get; }

		[iOS (9,0)]
		[Field ("AVAssetDownloadTaskMediaSelectionKey")]
		NSString MediaSelectionKey { get; }
	}

	[NoWatch]
	[Obsoleted (PlatformName.TvOS, 12, 0)]
	[StrongDictionary ("AVAssetDownloadTaskKeys")]
	interface AVAssetDownloadOptions {
		NSNumber MinimumRequiredMediaBitrate { get; set; }
		AVMediaSelection MediaSelection { get; set; }
	}

	[NoTV]
	[NoWatch]
	[iOS (9,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSUrlSession), Name = "AVAssetDownloadURLSession")]
	interface AVAssetDownloadUrlSession {
		[Static]
		[return: ForcedType]
		[Export ("sessionWithConfiguration:assetDownloadDelegate:delegateQueue:")]
		AVAssetDownloadUrlSession CreateSession (NSUrlSessionConfiguration configuration, [NullAllowed] IAVAssetDownloadDelegate @delegate, [NullAllowed] NSOperationQueue delegateQueue);

		[Availability (Introduced = Platform.iOS_9_0, Deprecated = Platform.iOS_10_0, Message="Please use 'GetAssetDownloadTask (AVUrlAsset, string, NSData, NSDictionary<NSString, NSObject>)'.")]
		[Export ("assetDownloadTaskWithURLAsset:destinationURL:options:")]
		[return: NullAllowed]
		AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, NSUrl destinationUrl, [NullAllowed] NSDictionary options);

		[Wrap ("GetAssetDownloadTask (urlAsset, destinationUrl, options != null ? options.Dictionary : null)")]
		AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, NSUrl destinationUrl, AVAssetDownloadOptions options);

		[iOS (10,0)]
		[Export ("assetDownloadTaskWithURLAsset:assetTitle:assetArtworkData:options:")]
		[return: NullAllowed]
		AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, string title, [NullAllowed] NSData artworkData, [NullAllowed] NSDictionary options);

		[iOS (10,0)]
		[Wrap ("GetAssetDownloadTask (urlAsset, title, artworkData, options != null ? options.Dictionary : null)")]
		AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, string title, [NullAllowed] NSData artworkData, AVAssetDownloadOptions options);

		[NoMac, NoTV, NoWatch, iOS (11,0)]
		[Export ("aggregateAssetDownloadTaskWithURLAsset:mediaSelections:assetTitle:assetArtworkData:options:")]
		[return: NullAllowed]
		AVAggregateAssetDownloadTask GetAssetDownloadTask (AVUrlAsset URLAsset, AVMediaSelection[] mediaSelections, string title, [NullAllowed] NSData artworkData, [NullAllowed] NSDictionary<NSString, NSObject> options);

	}

	interface IAVAssetDownloadDelegate {}

	[NoTV]
	[NoMac]
	[NoWatch]
	[iOS (9,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVAssetDownloadDelegate : NSUrlSessionTaskDelegate {
		[Export ("URLSession:assetDownloadTask:didLoadTimeRange:totalTimeRangesLoaded:timeRangeExpectedToLoad:")]
		void DidLoadTimeRange (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, CMTimeRange timeRange, NSValue[] loadedTimeRanges, CMTimeRange timeRangeExpectedToLoad);

		[Export ("URLSession:assetDownloadTask:didResolveMediaSelection:")]
		void DidResolveMediaSelection (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, AVMediaSelection resolvedMediaSelection);

		[iOS (10,0)]
		[Export ("URLSession:assetDownloadTask:didFinishDownloadingToURL:")]
		void DidFinishDownloadingToUrl (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, NSUrl location);

		[iOS (11,0)]
		[Export ("URLSession:aggregateAssetDownloadTask:willDownloadToURL:")]
		void WillDownloadToUrl (NSUrlSession session, AVAggregateAssetDownloadTask aggregateAssetDownloadTask, NSUrl location);

		[iOS (11,0)]
		[Export ("URLSession:aggregateAssetDownloadTask:didCompleteForMediaSelection:")]
		void DidCompleteForMediaSelection (NSUrlSession session, AVAggregateAssetDownloadTask aggregateAssetDownloadTask, AVMediaSelection mediaSelection);

		[iOS (11,0)]
		[Export ("URLSession:aggregateAssetDownloadTask:didLoadTimeRange:totalTimeRangesLoaded:timeRangeExpectedToLoad:forMediaSelection:")]
		void DidLoadTimeRange (NSUrlSession session, AVAggregateAssetDownloadTask aggregateAssetDownloadTask, CMTimeRange timeRange, NSValue[] loadedTimeRanges, CMTimeRange timeRangeExpectedToLoad, AVMediaSelection mediaSelection);
	}

#endif

	[NoWatch]
	[iOS (7,0)][Mac (10,9)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVOutputSettingsAssistant {
		[Mac (10, 10)]
		[Static, Export ("availableOutputSettingsPresets")]
		string [] AvailableOutputSettingsPresets { get; }

		[return: NullAllowed]
		[Static, Export ("outputSettingsAssistantWithPreset:")]
		AVOutputSettingsAssistant FromPreset (string presetIdentifier);

		[Export ("audioSettings", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary WeakAudioSettings { get; }

		[Wrap ("WeakAudioSettings")]
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

		[Mac (10, 10)]
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

		[iOS (9,0), Mac (10,10)]
		[Internal, Field ("AVOutputSettingsPreset3840x2160")]
		NSString _Preset3840x2160 { get; }

		[iOS (11, 0), Mac (10, 13)]
		[TV (11, 0)]
		[Internal, Field ("AVOutputSettingsPresetHEVC1920x1080")]
		NSString _PresetHevc1920x1080 { get; }

		[iOS (11, 0), Mac (10, 13)]
		[TV (11, 0)]
		[Internal, Field ("AVOutputSettingsPresetHEVC3840x2160")]
		NSString _PresetHevc3840x2160 { get; }
	}

	[NoWatch]
	[iOS (9,0)][Mac (10,11)]
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

	[NoWatch]
	[iOS (9,0), Mac (10,11)]
	[BaseType (typeof (AVMediaSelection))]
	interface AVMutableMediaSelection {

		[Export ("selectMediaOption:inMediaSelectionGroup:")]
		void SelectMediaOption ([NullAllowed] AVMediaSelectionOption mediaSelectionOption, AVMediaSelectionGroup mediaSelectionGroup);
	}

	[NoWatch, iOS (9,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	interface AVAudioSequencer {

		[Export ("initWithAudioEngine:")]
		IntPtr Constructor (AVAudioEngine engine);

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
		AVMusicTrack[] Tracks { get; }

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

	[NoWatch, iOS (9,0)][Mac (10,11)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Docs/headers do not state that init is disallowed but if 
	// you get an instance that way and try to use it, it will inmediatelly crash also tested in ObjC app same result
	interface AVMusicTrack {
		
		[NullAllowed, Export ("destinationAudioUnit", ArgumentSemantic.Retain)]
		AVAudioUnit DestinationAudioUnit { get; set; }

		[NoTV]
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

	[Obsoleted (PlatformName.TvOS, 12,0, message: "All fields will return 'null'.")]
	[Watch (3,0)]
	[iOS (9,0)][Mac (10,11)]
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

	[NoWatch, iOS (9,0)][Mac (10,10)]
	[BaseType (typeof(NSObject))]
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

#if MONOMAC
		[Export ("availableArchitectures")]
		NSNumber[] AvailableArchitectures { get; }

		[Export ("userTagNames", ArgumentSemantic.Copy)]
		string[] UserTagNames { get; set; }

		[NullAllowed, Export ("iconURL")]
		NSUrl IconUrl { get; }

		[Mac (10,11)]
		[NullAllowed, Export ("icon")]
		global::AppKit.NSImage Icon { get; }

		[Export ("passesAUVal")]
		bool PassesAUVal { get; }

		[Export ("hasCustomView")]
		bool HasCustomView { get; }

		[Export ("configurationDictionary")]
		NSDictionary WeakConfigurationDictionary { get; }

		[Export ("supportsNumberInputChannels:outputChannels:")]
		bool SupportsNumberInputChannels (nint numInputChannels, nint numOutputChannels);
#endif

		[Export ("allTagNames")]
		string[] AllTagNames { get; }

#if XAMCORE_2_0
		[Export ("audioComponentDescription")]
		AudioComponentDescription AudioComponentDescription { get; }
#endif
		[iOS (9,0), Mac (10,10)]
		[Field ("AVAudioUnitComponentTagsDidChangeNotification")]
		[Notification]
		NSString TagsDidChangeNotification { get; }
	}

	delegate bool AVAudioUnitComponentFilter (AVAudioUnitComponent comp, ref bool stop);

	[NoWatch, iOS (9,0), Mac (10,10)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // for binary compatibility this is added in AVCompat.cs w/[Obsolete]
	interface AVAudioUnitComponentManager {
		
		[Export ("tagNames")]
		string[] TagNames { get; }

		[Export ("standardLocalizedTagNames")]
		string[] StandardLocalizedTagNames { get; }

		[Static]
		[Export ("sharedAudioUnitComponentManager")]
		AVAudioUnitComponentManager SharedInstance { get; }

		[Export ("componentsMatchingPredicate:")]
		AVAudioUnitComponent[] GetComponents (NSPredicate predicate);

		[Export ("componentsPassingTest:")]
		AVAudioUnitComponent[] GetComponents (AVAudioUnitComponentFilter testHandler);

#if XAMCORE_2_0
		[Export ("componentsMatchingDescription:")]
		AVAudioUnitComponent[] GetComponents (AudioComponentDescription desc);
#endif
	}

	[Watch (3,0)]
	[Static]
	interface AVAudioUnitManufacturerName {
		
		[Obsoleted (PlatformName.TvOS, 12,0, message: "Field will return 'null'.")]
		[Field ("AVAudioUnitManufacturerNameApple")]
		[Mac (10,10), iOS (9,0)]
		NSString Apple { get; }
	}

#if !MONOMAC && XAMCORE_2_0 // FIXME: Unsure about if CMMetadataFormatDescription will be an INativeObject and will need manual binding for Classic
	[NoWatch]
	[NoTV]
	[iOS (9,0)]
	[BaseType (typeof(AVCaptureInput))]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: Format description is required.
	interface AVCaptureMetadataInput {
		
		[Internal]
		[Static]
		[Export ("metadataInputWithFormatDescription:clock:")] // FIXME: Add CMMetadataFormatDescription
		AVCaptureMetadataInput MetadataInputWithFormatDescription (IntPtr /*CMMetadataFormatDescription*/ desc, CMClock clock);

		[Internal]
		[Export ("initWithFormatDescription:clock:")] // FIXME: Add CMMetadataFormatDescription
		IntPtr Constructor (IntPtr /*CMMetadataFormatDescription*/ desc, CMClock clock);

		[Export ("appendTimedMetadataGroup:error:")]
		bool AppendTimedMetadataGroup (AVTimedMetadataGroup metadata, out NSError outError);
	}
#endif

	[NoWatch]
	[iOS (9,0), Mac (10,11)]
	[BaseType (typeof(NSObject))]
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
	
#if !MONOMAC
	[NoiOS, TV (10,0), NoWatch, NoMac]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVContentProposal : NSCopying
	{
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
		AVMetadataItem[] Metadata { get; set; }

		[Export ("initWithContentTimeForTransition:title:previewImage:")]
		[DesignatedInitializer]
		IntPtr Constructor (CMTime contentTimeForTransition, string title, [NullAllowed] UIImage previewImage);
	}
#endif

	partial interface IAVContentKeySessionDelegate {}

	[TV (10,2), Mac (10,12,4), iOS (10,3), NoWatch]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVContentKeySessionDelegate
	{
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

		[NoWatch, NoTV, NoMac, iOS (11,0)]
		[Export ("contentKeySession:didUpdatePersistableContentKey:forContentKeyIdentifier:")]
		void DidUpdate (AVContentKeySession session, NSData persistableContentKey, NSObject keyIdentifier);

		[TV (12,0), NoWatch, Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Export ("contentKeySession:contentKeyRequestDidSucceed:")]
		void DidSucceed (AVContentKeySession session, AVContentKeyRequest keyRequest);

		[TV (12,0), NoWatch, Mac (10,14, onlyOn64: true), iOS (12,0)]
		[Export ("contentKeySessionDidGenerateExpiredSessionReport:")]
		void DidGenerateExpiredSessionReport (AVContentKeySession session);
	}

	partial interface IAVContentKeyRecipient {}

	[TV (10,2), Mac (10,12,4), iOS (10,3), NoWatch]
	[Protocol]
	interface AVContentKeyRecipient {
		[Abstract]
		[Export ("mayRequireContentKeysForMediaDataProcessing")]
		bool MayRequireContentKeysForMediaDataProcessing { get; }
	}

	[TV (10,2), Mac (10,12,4), iOS (10,3), NoWatch]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface AVContentKeySession {

		[TV (11,0), NoWatch, Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("contentKeySessionWithKeySystem:")]
		AVContentKeySession Create (string keySystem);

		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("contentKeySessionWithKeySystem:storageDirectoryAtURL:")]
		AVContentKeySession Create (NSString keySystem, [NullAllowed] NSUrl storageUrl);

		[Static]
		[Wrap ("Create (keySystem.GetConstant (), storageUrl)")]
		AVContentKeySession Create (AVContentKeySystem keySystem, [NullAllowed] NSUrl storageUrl);

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
		[NoWatch, NoTV, NoMac, iOS (11,0)]
		[Export ("makeSecureTokenForExpirationDateOfPersistableContentKey:completionHandler:")]
		void MakeSecureToken (NSData persistableContentKeyData, Action<NSData, NSError> handler);

		#region AVContentKeySession_AVContentKeySessionPendingExpiredSessionReports

		// binded because they are static and from a category.
		[Static]
		[Export ("pendingExpiredSessionReportsWithAppIdentifier:storageDirectoryAtURL:")]
		NSDictionary[] GetPendingExpiredSessionReports (NSData appIdentifier, NSUrl storageUrl);

		[Static]
		[Export ("removePendingExpiredSessionReports:withAppIdentifier:storageDirectoryAtURL:")]
		void RemovePendingExpiredSessionReports (NSDictionary[] expiredSessionReports, NSData appIdentifier, NSUrl storageUrl);

		#endregion
	}

	[TV (10,2), Mac (10,12,4), iOS (10,3), NoWatch]
	[Category]
	[BaseType (typeof(AVContentKeySession))]
	interface AVContentKeySession_AVContentKeyRecipients
	{
		[Export ("addContentKeyRecipient:")]
		void Add (IAVContentKeyRecipient recipient);

		[Export ("removeContentKeyRecipient:")]
		void Remove (IAVContentKeyRecipient recipient);

		[Export ("contentKeyRecipients")]
		IAVContentKeyRecipient[] GetContentKeyRecipients ();
	}

	[TV (10,2), Mac (10,12,4), iOS (10,3), NoWatch]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface AVContentKeyRequest {
		[Field ("AVContentKeyRequestProtocolVersionsKey")]
		NSString ProtocolVersions { get; }

		[Export ("status")]
		AVContentKeyRequestStatus Status { get; }

		[NullAllowed, Export ("error")]
		NSError Error { get; }

		[NullAllowed, Export ("identifier")]
		NSObject Identifier { get; }

		[NullAllowed, Export ("initializationData")]
		NSData InitializationData { get; }

		[Export ("canProvidePersistableContentKey")]
		bool CanProvidePersistableContentKey { get; }

		[Async]
		[Export ("makeStreamingContentKeyRequestDataForApp:contentIdentifier:options:completionHandler:")]
		void MakeStreamingContentKeyRequestData (NSData appIdentifier, [NullAllowed] NSData contentIdentifier, [NullAllowed] NSDictionary<NSString, NSObject> options, Action<NSData, NSError> handler);

		[Export ("processContentKeyResponse:")]
		void Process (AVContentKeyResponse keyResponse);

		[Export ("processContentKeyResponseError:")]
		void Process (NSError error);

		[Deprecated (PlatformName.iOS, 11, 2, message: "Use the 'NSError' overload instead.")]
		[Export ("respondByRequestingPersistableContentKeyRequest"), NoWatch, NoTV, NoMac]
		void RespondByRequestingPersistableContentKeyRequest ();

		[NoWatch, NoTV, NoMac, iOS (11,2)]
		[Export ("respondByRequestingPersistableContentKeyRequestAndReturnError:")]
		bool RespondByRequestingPersistableContentKeyRequest ([NullAllowed] out NSError error);
	}

	[Category]
	[Mac (10, 12, 4), iOS (10,3), TV (10, 2), NoWatch]
	[BaseType (typeof(AVContentKeyRequest))]
	interface AVContentKeyRequest_AVContentKeyRequestRenewal
	{
		[Export ("renewsExpiringResponseData")]
		bool GetRenewsExpiringResponseData ();
	}

	[TV (10,2), Mac (10,12,4), iOS (10,3), NoWatch]
	[DisableDefaultCtor]
	[BaseType (typeof (AVContentKeyRequest))]
	interface AVPersistableContentKeyRequest {
		[Export ("persistableContentKeyFromKeyVendorResponse:options:error:")]
		NSData GetPersistableContentKey (NSData keyVendorResponse, [NullAllowed] NSDictionary<NSString, NSObject> options, out NSError outError);

	}

	[TV (10,2), Mac (10,12,4), iOS (10,3), NoWatch]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface AVContentKeyResponse {
		[Static]
		[Export ("contentKeyResponseWithFairPlayStreamingKeyResponseData:")]
		AVContentKeyResponse Create (NSData fairPlayStreamingKeyResponseData);

		[TV (11,0), NoWatch, Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("contentKeyResponseWithClearKeyData:initializationVector:")]
		AVContentKeyResponse Create (NSData keyData, [NullAllowed] NSData initializationVector);
	}

	[TV (11,0), NoWatch, Mac (10,13), iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof(NSObject))]
	interface AVRouteDetector {
		[Notification]
		[TV (11, 0), NoWatch, Mac (10, 13), iOS (11, 0)]
		[Field ("AVRouteDetectorMultipleRoutesDetectedDidChangeNotification")]
		NSString MultipleRoutesDetectedDidChange { get; }

		[Export ("routeDetectionEnabled")]
		bool RouteDetectionEnabled { [Bind ("isRouteDetectionEnabled")] get; set; }

		[Export ("multipleRoutesDetected")]
		bool MultipleRoutesDetected { get; }
	}

	interface IAVCapturePhotoFileDataRepresentationCustomizer {}
	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Protocol]
	interface AVCapturePhotoFileDataRepresentationCustomizer
	{
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
	}

	[NoTV, iOS (11,0), NoWatch, NoMac]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVCapturePhoto
	{
		[Export ("timestamp")]
		CMTime Timestamp { get; }

		[Export ("rawPhoto")]
		bool RawPhoto { [Bind ("isRawPhoto")] get; }

		[NullAllowed, Export ("pixelBuffer")]
		CVPixelBuffer PixelBuffer { get; }

		[NullAllowed, Export ("previewPixelBuffer")]
		CVPixelBuffer PreviewPixelBuffer { get; }

		[NullAllowed, Export ("embeddedThumbnailPhotoFormat")]
		NSDictionary WeakEmbeddedThumbnailPhotoFormat { get; }

		[Wrap ("WeakEmbeddedThumbnailPhotoFormat")]
		AVVideoSettingsCompressed EmbeddedThumbnailPhotoFormat { get; }

		[NullAllowed, Export ("depthData")]
		AVDepthData DepthData { get; }

		[Export ("metadata")]
		NSDictionary WeakMetadata { get; }

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

		[Wrap ("AVCaptureDeviceTypeExtensions.GetValue (WeakSourceDeviceType)")]
		AVCaptureDeviceType SourceDeviceType { get; }

		// From @interface AVCapturePhotoBracketedCapture (AVCapturePhoto)

#if !MONOMAC
		[iOS (11, 0)]
		[NullAllowed, Export ("bracketSettings")]
		AVCaptureBracketedStillImageSettings BracketSettings { get; }
#endif

		[iOS (11, 0), NoMac]
		[Export ("lensStabilizationStatus")]
		AVCaptureLensStabilizationStatus LensStabilizationStatus { get; }

		[iOS (11, 0), NoMac]
		[Export ("sequenceCount")]
		nint SequenceCount { get; }

		// @interface AVCapturePhotoConversions (AVCapturePhoto)
		[iOS (11, 0)]
		[NullAllowed, Export ("fileDataRepresentation")]
		NSData FileDataRepresentation { get; }

		[iOS (11,0)]
		[Export ("fileDataRepresentationWithReplacementMetadata:replacementEmbeddedThumbnailPhotoFormat:replacementEmbeddedThumbnailPixelBuffer:replacementDepthData:")]
		[return: NullAllowed]
		NSData GetFileDataRepresentation ([NullAllowed] NSDictionary<NSString, NSObject> replacementMetadata, [NullAllowed] NSDictionary<NSString, NSObject> replacementEmbeddedThumbnailPhotoFormat, [NullAllowed] CVPixelBuffer replacementEmbeddedThumbnailPixelBuffer, [NullAllowed] AVDepthData replacementDepthData);


		[iOS (11, 0)]
		[NullAllowed, Export ("CGImageRepresentation")]
		CGImage CGImageRepresentation { get; }

		[iOS (11, 0)]
		[NullAllowed, Export ("previewCGImageRepresentation")]
		CGImage PreviewCGImageRepresentation { get; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[NullAllowed, Export ("portraitEffectsMatte")]
		AVPortraitEffectsMatte PortraitEffectsMatte { get; }

		[NoWatch, NoTV, NoMac, iOS (12,0)]
		[Export ("fileDataRepresentationWithCustomizer:")]
		[return: NullAllowed]
		NSData GetFileDataRepresentation (IAVCapturePhotoFileDataRepresentationCustomizer customizer);
	}

	[NoWatch, TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVPortraitEffectsMatte
	{
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

	[NoWatch, TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVAssetResourceLoadingRequestor
	{
		[Export ("providesExpiredSessionReports")]
		bool ProvidesExpiredSessionReports { get; }
	}
}
