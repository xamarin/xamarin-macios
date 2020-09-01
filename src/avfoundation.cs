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
using MediaToolbox;
#else
// hack: ease compilation without extra defines
using CIBarcodeDescriptor = Foundation.NSObject;
#endif
using AudioToolbox;
using CoreMedia;
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
	[Watch (6,0)]
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

		[NoTV, NoWatch]
		[Obsoleted (PlatformName.iOS, 6,0)]
		[Deprecated (PlatformName.iOS, 12,0, message: "Always 'null'.")]
		[Obsoleted (PlatformName.MacOSX, 10,8)]
		[Deprecated (PlatformName.MacOSX, 10,14, message: "Always 'null'.")]
		[Field ("AVMediaTypeTimedMetadata")] // last header where I can find this: iOS 5.1 SDK, 10.7 only on Mac
		TimedMetadata = 6,

		[Field ("AVMediaTypeMuxed")]
		Muxed = 7,

		[iOS (9,0)][NoMac][NoWatch]
		[Field ("AVMediaTypeMetadataObject")]
		MetadataObject = 8,

		[Field ("AVMediaTypeMetadata")]
		Metadata = 9,

		[iOS (11, 0), Mac (10, 13), NoWatch]
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

		[Field ("AVMediaTypeMetadata")]
		NSString Metadata { get; }
	}
#endif

	[Watch (6,0)]
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

	[iOS (9,0), Mac (10,11), Watch (6,0)]
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
	[Watch (6,0)]
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

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Field ("AVMediaCharacteristicIsOriginalContent")]
		IsOriginalContent = 15,

	}

#if !XAMCORE_4_0
	[NoWatch]
	[Obsolete ("Use AVMediaCharacteristics enum values")]
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
		NSString DescribesVideoForAccessibility { get;  }
#if !MONOMAC
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

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Field ("AVMediaCharacteristicIsOriginalContent")]
		NSString IsOriginalContent { get; }

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Field ("AVMediaCharacteristicContainsAlphaChannel")]
		NSString ContainsAlphaChannel { get; }
	}
#endif

	[Watch (6,0)]
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

	[Watch (6,0)]
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

	[iOS (9,0), Mac (10,11), Watch (6,0)]
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

		[TV (9, 0)]
		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'AVVideoCodecType' enum instead.")]
		[Field ("AVVideoCodecH264")]
		NSString CodecH264 { get; }
		
		[TV (9, 0)]
		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'AVVideoCodecType' enum instead.")]
		[Field ("AVVideoCodecJPEG")]
		NSString CodecJPEG { get; }
		
		[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'AVVideoCodecType' enum instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'AVVideoCodecType' enum instead.")]
		[NoiOS, NoTV]
		[Field ("AVVideoCodecAppleProRes4444")]
		NSString AppleProRes4444 { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'AVVideoCodecType' enum instead.")]
		[NoiOS, NoTV]
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

		[Mac (10, 9)]
		[Field ("AVVideoProfileLevelH264High40")]
		NSString ProfileLevelH264High40 { get; }

		[Mac (10, 9)]
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

	[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
	delegate AVAudioEngineManualRenderingStatus AVAudioEngineManualRenderingBlock (/* AVAudioFrameCount = uint */ uint numberOfFrames, AudioBuffers outBuffer, [NullAllowed] /* OSStatus */ ref int outError);

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

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("manualRenderingBlock")]
		AVAudioEngineManualRenderingBlock ManualRenderingBlock { get; }

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
		[TV (12,0), Mac (10,14), iOS (12,0), NoWatch]
		[Export ("connectMIDI:to:format:block:")]
		void ConnectMidi (AVAudioNode sourceNode, AVAudioNode destinationNode, [NullAllowed] AVAudioFormat format, [NullAllowed] AUMidiOutputEventBlock tapHandler);

		[TV (12,0), Mac (10,14), iOS (12,0), NoWatch]
		[Export ("connectMIDI:toNodes:format:block:")]
		void ConnectMidi (AVAudioNode sourceNode, AVAudioNode[] destinationNodes, [NullAllowed] AVAudioFormat format, [NullAllowed] AUMidiOutputEventBlock tapHandler);
#endif

		[TV (12,0), Mac (10,14), iOS (12,0), NoWatch]
		[Export ("disconnectMIDI:from:")]
		void DisconnectMidi (AVAudioNode sourceNode, AVAudioNode destinationNode);

		[TV (12,0), Mac (10,14), iOS (12,0), NoWatch]
		[Export ("disconnectMIDI:fromNodes:")]
		void DisconnectMidi (AVAudioNode sourceNode, AVAudioNode[] destinationNodes);

		[TV (12,0), Mac (10,14), iOS (12,0), NoWatch]
		[Export ("disconnectMIDIInput:")]
		void DisconnectMidiInput (AVAudioNode node);

		[TV (12,0), Mac (10,14), iOS (12,0), NoWatch]
		[Export ("disconnectMIDIOutput:")]
		void DisconnectMidiOutput (AVAudioNode node);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("attachedNodes", ArgumentSemantic.Copy)]
		NSSet<AVAudioNode> AttachedNodes { get; }
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

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("outputType", ArgumentSemantic.Assign)]
		AVAudioEnvironmentOutputType OutputType { get; set; }
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

		[Wrap ("this (fileUrl, settings.GetDictionary ()!, out outError)")]
		IntPtr Constructor (NSUrl fileUrl, AudioSettings settings, out NSError outError);

		[Export ("initForWriting:settings:commonFormat:interleaved:error:"), Internal]
		IntPtr Constructor (NSUrl fileUrl, NSDictionary settings, AVAudioCommonFormat format, bool interleaved, out NSError outError);
		
		[Wrap ("this (fileUrl, settings.GetDictionary ()!, format, interleaved, out outError)")]
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

		[Wrap ("this (settings.GetDictionary ()!)")]
		IntPtr Constructor (AudioSettings settings);

		[iOS (9,0)][Mac (10,11)][Watch (6,0)]
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

		[iOS (9,0)][Mac (10,11)][Watch (6,0)]
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

#if XAMCORE_4_0
		[Abstract]
		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("sourceMode", ArgumentSemantic.Assign)]
		AVAudio3DMixingSourceMode SourceMode { get; set; }

		[Abstract]
		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("pointSourceInHeadMode", ArgumentSemantic.Assign)]
		AVAudio3DMixingPointSourceInHeadMode PointSourceInHeadMode { get; set; }
	
#else
		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("sourceMode", ArgumentSemantic.Assign)]
		AVAudio3DMixingSourceMode GetSourceMode ();

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("setSourceMode:")]
		void SetSourceMode (AVAudio3DMixingSourceMode sourceMode);

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("pointSourceInHeadMode", ArgumentSemantic.Assign)]
		AVAudio3DMixingPointSourceInHeadMode GetPointSourceInHeadMode ();

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("setPointSourceInHeadMode:")]
		void SetPointSourceInHeadMode (AVAudio3DMixingPointSourceInHeadMode pointSourceInHeadMode);
#endif
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
		[return: NullAllowed]
		string GetNameForInputBus (nuint bus);

		[Export ("nameForOutputBus:")]
		[return: NullAllowed]
		string GetNameForOutputBus (nuint bus);

		[Export ("installTapOnBus:bufferSize:format:block:")]
		void InstallTapOnBus (nuint bus, uint /* AVAudioFrameCount = uint32_t */ bufferSize, [NullAllowed] AVAudioFormat format, AVAudioNodeTapBlock tapBlock);

		[Export ("removeTapOnBus:")]
		void RemoveTapOnBus (nuint bus);

		[NoWatch, TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("AUAudioUnit")]
		AUAudioUnit AUAudioUnit { get; }

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

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("voiceProcessingEnabled")]
		bool VoiceProcessingEnabled { [Bind ("isVoiceProcessingEnabled")] get; }

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("setVoiceProcessingEnabled:error:")]
		bool SetVoiceProcessingEnabled (bool enabled, out NSError outError);
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

	[Watch (4,0), TV (11,0), Mac (10,10), iOS (8,0)]
	delegate AudioBuffers AVAudioIONodeInputBlock (uint frameCount);

 	[Watch (4,0)]
 	[iOS (8,0)][Mac (10,10)][TV (11,0)]
 	[BaseType (typeof (AVAudioIONode))]
	[DisableDefaultCtor] // returned Handle is nil
	// note: sample source (header) suggest it comes from AVAudioEngine properties
	interface AVAudioInputNode : AVAudioMixing {

		[Mac (10,13), iOS (11,0), Watch (6,0)]
		[Export ("setManualRenderingInputPCMFormat:inputBlock:")]
		bool SetManualRenderingInputPcmFormat (AVAudioFormat format, AVAudioIONodeInputBlock block);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("voiceProcessingBypassed")]
		bool VoiceProcessingBypassed { [Bind ("isVoiceProcessingBypassed")] get; set; }

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("voiceProcessingAGCEnabled")]
		bool VoiceProcessingAgcEnabled { [Bind ("isVoiceProcessingAGCEnabled")] get; set; }

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("voiceProcessingInputMuted")]
		bool VoiceProcessingInputMuted { [Bind ("isVoiceProcessingInputMuted")] get; set; }

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

		[Export ("settings")]
		NSDictionary WeakSettings { get;  }

		[Wrap ("WeakSettings")]
		AudioSettings Settings { get; }

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
		[Export ("channelAssignments", ArgumentSemantic.Copy), NullAllowed]
		AVAudioSessionChannelDescription [] ChannelAssignments { get; set; }
#endif
		[Mac (10,14)]
		[Export ("recordAtTime:")]
		bool RecordAt (double time);

		[Mac (10,14)]
		[Export ("recordAtTime:forDuration:")]
		bool RecordAt (double time, double duration);

		[Mac (10,14)]
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
		//[Export ("audioRecorderEndInterruption:withOptions:")]
		//void EndInterruption (AVAudioRecorder recorder, AVAudioSessionInterruptionFlags flags);
#endif
	}

	[NoMac]
	interface AVAudioSessionSecondaryAudioHintEventArgs {
		[Export ("AVAudioSessionSilenceSecondaryAudioHintNotification")]
		AVAudioSessionSilenceSecondaryAudioHintType Hint { get; }

		[Export ("AVAudioSessionSilenceSecondaryAudioHintTypeKey")]
		AVAudioSessionRouteDescription HintType { get; }
	}

	delegate void AVPermissionGranted (bool granted);

	[NoMac]
	[Watch (3,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // for binary compatibility this is added in AVAudioSession.cs w/[Obsolete]
	interface AVAudioSession {
		
		[Export ("sharedInstance"), Static]
		AVAudioSession SharedInstance ();
	
		[NoWatch]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'AVAudioSession.Notification.Observe*' methods instead.")]
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		[NoTV]
		NSObject WeakDelegate { get; set;  }

		[NoWatch]
		[Wrap ("WeakDelegate")]
		[Protocolize]
		[NullAllowed]
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

		[Export ("setActive:withOptions:error:")]
		bool SetActive (bool active, AVAudioSessionSetActiveOptions options, out NSError outError);

		[iOS (9,0)]
		[Export ("availableCategories")]
		string [] AvailableCategories { get; }

		[Export ("setCategory:withOptions:error:")]
		bool SetCategory (string category, AVAudioSessionCategoryOptions options, out NSError outError);
		
		[iOS (10,0), TV (10,0), Watch (3,0)]
		[Export ("setCategory:mode:options:error:")]
		bool SetCategory (string category, string mode, AVAudioSessionCategoryOptions options, out NSError outError);

		[Export ("categoryOptions")]
		AVAudioSessionCategoryOptions CategoryOptions { get;  }

		[iOS (9,0)]
		[Export ("availableModes")]
		string [] AvailableModes { get; }

		[Export ("overrideOutputAudioPort:error:")]
		bool OverrideOutputAudioPort (AVAudioSessionPortOverride portOverride, out NSError outError);

		[Export ("otherAudioPlaying")]
		bool OtherAudioPlaying { [Bind ("isOtherAudioPlaying")] get;  }

		[Export ("currentRoute")]
		AVAudioSessionRouteDescription CurrentRoute { get;  }

		[NoWatch]
		[Export ("setPreferredSampleRate:error:")]
		bool SetPreferredSampleRate (double sampleRate, out NSError error);
		
		[NoWatch]
		[Export ("preferredSampleRate")]
		double PreferredSampleRate { get;  }

		[NoWatch]
		[Export ("inputGain")]
		float InputGain { get;  } // defined as 'float'

		[NoWatch]
		[Export ("inputGainSettable")]
		bool InputGainSettable { [Bind ("isInputGainSettable")] get;  }

		[Export ("inputAvailable")]
		bool InputAvailable { [Bind ("isInputAvailable")] get;  }

		[Export ("sampleRate")]
		double SampleRate { get;  }

		[Export ("inputNumberOfChannels")]
		nint InputNumberOfChannels { get;  }

		[Export ("outputNumberOfChannels")]
		nint OutputNumberOfChannels { get;  }

		[Export ("outputVolume")]
		float OutputVolume { get;  } // defined as 'float'

		[Export ("inputLatency")]
		double InputLatency { get;  }

		[Export ("outputLatency")]
		double OutputLatency { get;  }

		[Export ("IOBufferDuration")]
		double IOBufferDuration { get;  }

		[NoWatch]
		[Export ("setInputGain:error:")]
		bool SetInputGain (float /* defined as 'float' */ gain, out NSError outError);

		[Field ("AVAudioSessionInterruptionNotification")]
		[Notification (typeof (AVAudioSessionInterruptionEventArgs))]
		NSString InterruptionNotification { get; }

		[Field ("AVAudioSessionRouteChangeNotification")]
		[Notification (typeof (AVAudioSessionRouteChangeEventArgs))]
		NSString RouteChangeNotification { get; }

		[Field ("AVAudioSessionMediaServicesWereResetNotification")]
		[Notification]
		NSString MediaServicesWereResetNotification { get; }

		[iOS (7,0), Notification, Field ("AVAudioSessionMediaServicesWereLostNotification")]
		NSString MediaServicesWereLostNotification { get; }
		
		[Field ("AVAudioSessionCategoryMultiRoute")]
		NSString CategoryMultiRoute { get; }
		
		[Field ("AVAudioSessionModeMoviePlayback")]
		NSString ModeMoviePlayback { get; }

		[iOS (7,0)]
		[Field ("AVAudioSessionModeVideoChat")]
		NSString ModeVideoChat { get; }

		[iOS (9,0)]
		[Field ("AVAudioSessionModeSpokenAudio")]
		NSString ModeSpokenAudio { get; }
		
		[Field ("AVAudioSessionPortLineIn")]
		NSString PortLineIn { get; }
		
		[Field ("AVAudioSessionPortBuiltInMic")]
		NSString PortBuiltInMic { get; }
		
		[Field ("AVAudioSessionPortHeadsetMic")]
		NSString PortHeadsetMic { get; }
		
		[Field ("AVAudioSessionPortLineOut")]
		NSString PortLineOut { get; }
		
		[Field ("AVAudioSessionPortHeadphones")]
		NSString PortHeadphones { get; }
		
		[Field ("AVAudioSessionPortBluetoothA2DP")]
		NSString PortBluetoothA2DP { get; }
		
		[Field ("AVAudioSessionPortBuiltInReceiver")]
		NSString PortBuiltInReceiver { get; }
		
		[Field ("AVAudioSessionPortBuiltInSpeaker")]
		NSString PortBuiltInSpeaker { get; }
		
		[Field ("AVAudioSessionPortHDMI")]
		NSString PortHdmi { get; }
		
		[Field ("AVAudioSessionPortAirPlay")]
		NSString PortAirPlay { get; }
		
		[Field ("AVAudioSessionPortBluetoothHFP")]
		NSString PortBluetoothHfp { get; }
		
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
		
		[Export ("inputDataSources"), NullAllowed]
		AVAudioSessionDataSourceDescription [] InputDataSources { get;  }

		[Export ("inputDataSource"), NullAllowed]
		AVAudioSessionDataSourceDescription InputDataSource { get;  }

		[Export ("outputDataSources"), NullAllowed]
		AVAudioSessionDataSourceDescription [] OutputDataSources { get;  }

		[Export ("outputDataSource"), NullAllowed]
		AVAudioSessionDataSourceDescription OutputDataSource { get;  }
		
		[NoWatch]
		[Export ("setInputDataSource:error:")]
		[PostGet ("InputDataSource")]
		bool SetInputDataSource ([NullAllowed] AVAudioSessionDataSourceDescription dataSource, out NSError outError);

		[NoWatch]
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
		[NullAllowed, Export ("availableInputs")]
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

		[Watch (5, 2), TV (12, 2), NoMac, iOS (12, 2)]
		[Export ("promptStyle")]
		AVAudioSessionPromptStyle PromptStyle { get; }

		[Watch (6,0), TV (13,0), NoMac, iOS (13,0)]
		[Export ("setAllowHapticsAndSystemSoundsDuringRecording:error:")]
		bool SetAllowHapticsAndSystemSoundsDuringRecording (bool inValue, [NullAllowed] out NSError outError);

		[Watch (6,0), TV (13,0), NoMac, iOS (13,0)]
		[Export ("allowHapticsAndSystemSoundsDuringRecording")]
		bool AllowHapticsAndSystemSoundsDuringRecording { get; }
	}
	
	[NoMac]
	[BaseType (typeof (NSObject))]
	interface AVAudioSessionDataSourceDescription {
		[Export ("dataSourceID")]
		NSNumber DataSourceID { get;  }

		[Export ("dataSourceName")]
		string DataSourceName { get;  }

		[iOS (7,0)]
		[Export ("location", ArgumentSemantic.Copy), NullAllowed]
		[Internal]
		NSString Location_ { get; }
	
		[iOS (7,0)]
		[Export ("orientation", ArgumentSemantic.Copy), NullAllowed]
		[Internal]
		NSString Orientation_ { get; }

		[NoWatch]
		[iOS (7,0)]
		[UnifiedInternal, Export ("supportedPolarPatterns"), NullAllowed]
		NSString [] SupportedPolarPatterns { get; }
	
		[NoWatch]
		[iOS (7,0)]
		[UnifiedInternal, Export ("selectedPolarPattern", ArgumentSemantic.Copy), NullAllowed]
		NSString SelectedPolarPattern { get; }
	
		[NoWatch]
		[iOS (7,0)]
		[UnifiedInternal, Export ("preferredPolarPattern", ArgumentSemantic.Copy), NullAllowed]
		NSString PreferredPolarPattern { get; }
	
		[NoWatch]
		[iOS (7,0)]
		[UnifiedInternal, Export ("setPreferredPolarPattern:error:")]
		bool SetPreferredPolarPattern ([NullAllowed] NSString pattern, out NSError outError);
		
	}

	[NoMac]
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

	[NoMac]
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

	[NoMac]
	[Watch (3,0)]
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

	[NoMac]
	[Watch (3,0)]
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

	[NoMac]
	[Watch (3,0)]
	[BaseType (typeof (NSObject))]
	interface AVAudioSessionRouteDescription {
		[Export ("inputs")]
		AVAudioSessionPortDescription [] Inputs { get;  }

		[Export ("outputs")]
		AVAudioSessionPortDescription [] Outputs { get;  }

	}

	[NoWatch, iOS (8,0)]
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

		[iOS (9,0), Mac (10,11)]
		[Static]
		[Export ("instantiateWithComponentDescription:options:completionHandler:")]
		[Async]
		void FromComponentDescription (AudioComponentDescription audioComponentDescription, AudioComponentInstantiationOptions options, Action<AVAudioUnit, NSError> completionHandler);

		[NoWatch, iOS (9,0), Mac (10,11)]
		[Export ("AUAudioUnit")]
		AUAudioUnit AUAudioUnit { get; }
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
		IntPtr Constructor (AudioComponentDescription audioComponentDescription);

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
		IntPtr Constructor (AudioComponentDescription audioComponentDescription);

		[Export ("bypass")]
		bool Bypass { get; set; }
	}

	[NoWatch, iOS (8,0)]
	[BaseType (typeof (AVAudioUnit), Name="AVAudioUnitMIDIInstrument")]
	[DisableDefaultCtor] // returns a nil handle
	interface AVAudioUnitMidiInstrument : AVAudioMixing { 
		[Export ("initWithAudioComponentDescription:")]
		IntPtr Constructor (AudioComponentDescription audioComponentDescription);

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
		IntPtr Constructor (AudioComponentDescription audioComponentDescription);

		[Export ("bypass")]
		bool Bypass { get; set; }
	}
	
	[NoWatch, iOS (8,0)]
	[BaseType (typeof (AVAudioUnitTimeEffect))]
	interface AVAudioUnitTimePitch {
		[Export ("initWithAudioComponentDescription:")]
		IntPtr Constructor (AudioComponentDescription audioComponentDescription);


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
		IntPtr Constructor (AudioComponentDescription audioComponentDescription);

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
		[return: NullAllowed]
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

	[Watch (6,0)]
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

#if !MONOMAC
		[Export ("compatibleWithSavedPhotosAlbum")]
		bool CompatibleWithSavedPhotosAlbum  { [Bind ("isCompatibleWithSavedPhotosAlbum")] get; }
#endif

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

		[iOS (11, 0), TV (11, 0), Mac (10, 13), Watch (6,0)]
		[Export ("allMediaSelections")]
		AVMediaSelection[] AllMediaSelections { get; }

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("minimumTimeOffsetFromLive")]
		CMTime MinimumTimeOffsetFromLive { get; }
	}

	interface IAVFragmentMinding {}

	[Protocol]
	[Mac (10,11)]
	[iOS (12, 0), TV (12,0), Watch (6,0)]
	interface AVFragmentMinding {

#if !MONOMAC || XAMCORE_4_0
		[Abstract] // not kept in Mac OS because is a breaking change, in other paltforms we are ok
#endif
		[Export ("isAssociatedWithFragmentMinder")]
		bool IsAssociatedWithFragmentMinder ();
	}

	[Mac (10,11)]
	[iOS (12, 0), TV (12,0), Watch (6,0)]
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
	[iOS (12, 0), TV (12,0), Watch (6,0)]
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
	[iOS (12,0), TV (12,0), Watch (6,0)]
	[BaseType (typeof(NSObject))]
	interface AVFragmentedAssetMinder {

		[Static]
		[Export ("fragmentedAssetMinderWithAsset:mindingInterval:")]
		AVFragmentedAssetMinder FromAsset (AVAsset asset, double mindingInterval);

		[Mac (10,14)]
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
	[iOS (12,0), TV (12,0), Watch (6,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (AVAssetTrack))]
	interface AVFragmentedAssetTrack {
	}

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
	[DisableDefaultCtor]
	interface AVCaptureSynchronizedDepthData
	{
		[Export ("depthData")]
		AVDepthData DepthData { get; }

		[Export ("depthDataWasDropped")]
		bool DepthDataWasDropped { get; }

		[Export ("droppedReason")]
		AVCaptureOutputDataDroppedReason DroppedReason { get; }
	}

	[TV (11,0), Watch (6,0), Mac (10,13), iOS (11,0)]
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

	[TV (11,0), Watch (6,0), Mac (10,13), iOS (11,0)]
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

		[TV (11,0), Mac (10,13), iOS (11,0)]
		[Notification (typeof (AudioRendererWasFlushedAutomaticallyEventArgs))]
		[Field ("AVSampleBufferAudioRendererWasFlushedAutomaticallyNotification")]
		NSString AudioRendererWasFlushedAutomaticallyNotification { get; }

	}

	[TV (11,0), Watch (6,0), Mac (10,13), iOS (11,0)]
	interface AudioRendererWasFlushedAutomaticallyEventArgs {
		[Internal]
		[Export ("AVSampleBufferAudioRendererFlushTimeKey")]
		NSValue _AudioRendererFlushTime { get; set; }
	}

	interface IAVQueuedSampleBufferRendering {}

	[TV (11,0), Watch (6,0), Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	interface AVSampleBufferRenderSynchronizer
	{
		[TV (12,0), Mac (10,14), iOS (12,0)]
		[Field ("AVSampleBufferRenderSynchronizerRateDidChangeNotification")]
		[Notification]
		NSString RateDidChangeNotification { get; }

		[TV (12,0), Mac (10,14), iOS (12,0)]
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
		[return: NullAllowed]
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
		[return: NullAllowed]
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

		[Mac (10,9)]
		[Export ("asset")]
		AVAsset Asset { get; }

		[NoWatch]
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
		IntPtr Constructor (AVAssetTrack track, [NullAllowed] NSDictionary outputSettings);

		[Wrap ("this (track, settings.GetDictionary ())")]		
		IntPtr Constructor (AVAssetTrack track, [NullAllowed] AudioSettings settings);

		[Wrap ("this (track, settings.GetDictionary ())")]		
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

		[Internal]
		[Advice ("Use 'Create' method.")]
		[Static, Export ("assetReaderAudioMixOutputWithAudioTracks:audioSettings:")]
		AVAssetReaderAudioMixOutput FromTracks (AVAssetTrack [] audioTracks, [NullAllowed] NSDictionary audioSettings);

		[Wrap ("FromTracks (audioTracks, settings.GetDictionary ())")]
		AVAssetReaderAudioMixOutput Create (AVAssetTrack [] audioTracks, [NullAllowed] AudioSettings settings);

		[DesignatedInitializer]
		[Export ("initWithAudioTracks:audioSettings:")]
		IntPtr Constructor (AVAssetTrack [] audioTracks, [NullAllowed] NSDictionary audioSettings);

		[Wrap ("this (audioTracks, settings.GetDictionary ())")]
		IntPtr Constructor (AVAssetTrack [] audioTracks, [NullAllowed] AudioSettings settings);

		[Internal]
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
		IntPtr Constructor (AVAssetTrack [] videoTracks, [NullAllowed] NSDictionary videoSettings);

		[Wrap ("this (videoTracks, settings.GetDictionary ())")]
		IntPtr Constructor (AVAssetTrack [] videoTracks, [NullAllowed] CVPixelBufferAttributes settings);		

		[Export ("videoSettings"), NullAllowed]
		NSDictionary WeakVideoSettings { get; }

		[Wrap ("WeakVideoSettings"), NullAllowed]
		CVPixelBufferAttributes UncompressedVideoSettings { get; }

		[NoWatch]
		[iOS (7,0), Mac (10, 9)]
		[Export ("customVideoCompositor", ArgumentSemantic.Copy), NullAllowed]
		[Protocolize]
		AVVideoCompositing CustomVideoCompositor { get; }
	}

	[Mac (10,9), NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no valid handle, docs now says "You do not create resource loader objects yourself."
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

	[Mac (10,9), Watch (6,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVAssetResourceLoaderDelegate {
#if !XAMCORE_4_0
		[Abstract]
#endif
		[NoWatch]
		[Export ("resourceLoader:shouldWaitForLoadingOfRequestedResource:")]
		bool ShouldWaitForLoadingOfRequestedResource (AVAssetResourceLoader resourceLoader, AVAssetResourceLoadingRequest loadingRequest);

		[NoWatch]
		[iOS (7,0), Mac (10, 9)]
		[Export ("resourceLoader:didCancelLoadingRequest:")]
		void DidCancelLoadingRequest (AVAssetResourceLoader resourceLoader, AVAssetResourceLoadingRequest loadingRequest);

		[NoWatch]
		[iOS (8,0), Mac (10, 10)]
		[Export ("resourceLoader:shouldWaitForResponseToAuthenticationChallenge:")]
		bool ShouldWaitForResponseToAuthenticationChallenge (AVAssetResourceLoader resourceLoader, NSUrlAuthenticationChallenge authenticationChallenge);

		[NoWatch]
		[iOS (8,0), Mac (10, 10)]
		[Export ("resourceLoader:didCancelAuthenticationChallenge:")]
		void DidCancelAuthenticationChallenge (AVAssetResourceLoader resourceLoader, NSUrlAuthenticationChallenge authenticationChallenge);

		[NoWatch]
		[iOS (8,0)]
		[Export ("resourceLoader:shouldWaitForRenewalOfRequestedResource:")]
		bool ShouldWaitForRenewalOfRequestedResource (AVAssetResourceLoader resourceLoader, AVAssetResourceRenewalRequest renewalRequest);		
	}

	[iOS (7,0), Mac (10, 9), NoWatch]
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
	
	[Mac (10, 9), NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // not meant be be user created (resource loader job, see documentation)
	interface AVAssetResourceLoadingRequest {
		[Export ("request")]
		NSUrlRequest Request { get;  }

		// note: we cannot use [Bind] here as it would break compatibility with iOS 6.x
		// `isFinished` was only added in iOS 7.0 SDK and cannot be called in earlier versions
		[Export ("isFinished")]
		[iOS (7,0)] // on iOS 6 it was `finished` but it's now rejected by Apple
		bool Finished { get; }

		[Export ("finishLoadingWithResponse:data:redirect:")]
		[Availability (Introduced = Platform.iOS_6_0, Deprecated = Platform.iOS_7_0, Message = "Use the 'Response', 'Redirect' properties and the 'AVAssetResourceLoadingDataRequest.Responds' and 'AVAssetResourceLoadingRequest.FinishLoading' methods instead.")]
		void FinishLoading ([NullAllowed] NSUrlResponse usingResponse, [NullAllowed] NSData data, [NullAllowed] NSUrlRequest redirect);

		[Export ("finishLoadingWithError:")]
		void FinishLoadingWithError ([NullAllowed]NSError error); // TODO: Should have been FinishLoading (NSerror);

		[return: NullAllowed]
		[Export ("streamingContentKeyRequestDataForApp:contentIdentifier:options:error:")]
		NSData GetStreamingContentKey (NSData appIdentifier, NSData contentIdentifier, [NullAllowed] NSDictionary options, out NSError error);

		[iOS (9,0), Mac (10,15)]
		[Export ("persistentContentKeyFromKeyVendorResponse:options:error:")]
		[return: NullAllowed]
		NSData GetPersistentContentKey (NSData keyVendorResponse, [NullAllowed] NSDictionary<NSString,NSObject> options, out NSError error);

		[iOS (9,0), Mac (10, 14), NoWatch]
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
		
		[TV (12, 0), Mac (10, 14), iOS (12, 0)]
		[Export ("requestor")]
		AVAssetResourceLoadingRequestor Requestor { get; }
	}

	[iOS (8,0), NoWatch]
	[DisableDefaultCtor] // not meant be be user created (resource loader job, see documentation) fix crash
	[BaseType (typeof (AVAssetResourceLoadingRequest))]
	interface AVAssetResourceRenewalRequest {
	}
	

	[iOS (7,0), Mac (10, 9), NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no valid handle, the instance is received (not created) -> see doc
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
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use the asynchronous 'FinishWriting (NSAction completionHandler)' instead.")]
		bool FinishWriting ();

		[Mac (10,9)]
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
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[AVAssetWriterInput initWithMediaType:outputSettings:] invalid parameter not satisfying: mediaType != ((void*)0)
	[DisableDefaultCtor]
	interface AVAssetWriterInput {
		[DesignatedInitializer]
		[Protected]
		[Export ("initWithMediaType:outputSettings:sourceFormatHint:")]
		IntPtr Constructor (string mediaType, [NullAllowed] NSDictionary outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[Wrap ("this (mediaType, outputSettings.GetDictionary (), sourceFormatHint)")]
		IntPtr Constructor (string mediaType, [NullAllowed] AudioSettings outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

		[Wrap ("this (mediaType, outputSettings.GetDictionary (), sourceFormatHint)")]
		IntPtr Constructor (string mediaType, [NullAllowed] AVVideoSettingsCompressed outputSettings, [NullAllowed] CMFormatDescription sourceFormatHint);

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
		IntPtr Constructor (string mediaType, [NullAllowed] NSDictionary outputSettings);

		[Wrap ("this (mediaType, outputSettings.GetDictionary ())")]		
		IntPtr Constructor (string mediaType, [NullAllowed] AudioSettings outputSettings);

		[Wrap ("this (mediaType, outputSettings.GetDictionary ())")]		
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

		[Advice ("Use 'Create' method.")]
		[Static, Export ("assetWriterInputPixelBufferAdaptorWithAssetWriterInput:sourcePixelBufferAttributes:")]
		AVAssetWriterInputPixelBufferAdaptor FromInput (AVAssetWriterInput input, [NullAllowed] NSDictionary sourcePixelBufferAttributes);

		[Static, Wrap ("FromInput (input, attributes.GetDictionary ())")]
		AVAssetWriterInputPixelBufferAdaptor Create (AVAssetWriterInput input, [NullAllowed] CVPixelBufferAttributes attributes);

		[DesignatedInitializer]
		[Export ("initWithAssetWriterInput:sourcePixelBufferAttributes:")]
		IntPtr Constructor (AVAssetWriterInput input, [NullAllowed] NSDictionary sourcePixelBufferAttributes);

		[Wrap ("this (input, attributes.GetDictionary ())")]
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

	[Watch (6,0)]
	[BaseType (typeof (AVAsset), Name="AVURLAsset")]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVUrlAsset 
#if !WATCH
		: AVContentKeyRecipient 
#endif
{

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get;  }

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
		IntPtr Constructor (NSUrl url, [NullAllowed] NSDictionary options);

		[Wrap ("this (url, options.GetDictionary ())")]
		IntPtr Constructor (NSUrl url, [NullAllowed] AVUrlAssetOptions options);

		[Wrap ("this (url, (NSDictionary) null!)")]
		IntPtr Constructor (NSUrl url);

		[return: NullAllowed]
		[Export ("compatibleTrackForCompositionTrack:")]
		AVAssetTrack CompatibleTrack (AVCompositionTrack forCompositionTrack);

		[Field ("AVURLAssetPreferPreciseDurationAndTimingKey")]
		NSString PreferPreciseDurationAndTimingKey { get; }

		[NoWatch]
		[Field ("AVURLAssetReferenceRestrictionsKey")]
		NSString ReferenceRestrictionsKey { get; }

		[Static, Export ("audiovisualMIMETypes")]
		string [] AudiovisualMimeTypes { get; }

		[Static, Export ("audiovisualTypes")]
		string [] AudiovisualTypes { get; }

		[Static, Export ("isPlayableExtendedMIMEType:")]
		bool IsPlayable (string extendedMimeType);

		[Mac (10, 9), NoWatch]
		[Export ("resourceLoader")]
		AVAssetResourceLoader ResourceLoader { get;  }

		[iOS (8,0), Mac (10,15)]
		[Field ("AVURLAssetHTTPCookiesKey")]
		NSString HttpCookiesKey { get; }

		[iOS (10,0), TV (10,0), Mac (10,12), NoWatch]
		[NullAllowed, Export ("assetCache")]
		AVAssetCache Cache { get; }

		[iOS (10, 0), TV (10, 0), Mac (10,15)]
		[Field ("AVURLAssetAllowsCellularAccessKey")]
		NSString AllowsCellularAccessKey { get; }	

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Field ("AVURLAssetAllowsExpensiveNetworkAccessKey")]
		NSString AllowsExpensiveNetworkAccessKey { get; }

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[Field ("AVURLAssetAllowsConstrainedNetworkAccessKey")]
		NSString AllowsConstrainedNetworkAccessKey { get; }
	}

	[Watch (6,0)]
	[BaseType (typeof (NSObject))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVAssetTrack : NSCopying {
		[Export ("trackID")]
		int /* CMPersistentTrackID = int32_t */ TrackID { get;  }

		[NullAllowed, Export ("asset", ArgumentSemantic.Weak)]
		AVAsset Asset { get; }

		[Export ("mediaType")]
		string MediaType { get;  }

		[iOS (11, 0), Mac (10, 13), TV (11, 0)]
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

		[NullAllowed, Export ("languageCode")]
		string LanguageCode { get;  }

		[NullAllowed, Export ("extendedLanguageTag")]
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

		[iOS (13,0), TV (13,0), Mac (10,15)]
		[Export ("hasAudioSampleDependencies")]
		bool HasAudioSampleDependencies { get; }
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

#if MONOMAC
		[NoTV, NoWatch, NoiOS, Mac (10,15)]
		[Export ("currentSampleAudioDependencyInfo")]
		AVSampleCursorAudioDependencyInfo CurrentSampleAudioDependencyInfo { get; }
#endif
	}

	[iOS (7,0), Mac (10, 9), Watch (6,0)]
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

	[iOS (7,0), Mac (10, 9), Watch (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVAssetTrackGroup : NSCopying {
		[Export ("trackIDs", ArgumentSemantic.Copy)]
		NSNumber [] TrackIDs { get; }
	}

	[Watch (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVMediaSelectionGroup : NSCopying {
		[Export ("options")]
		AVMediaSelectionOption [] Options { get;  }
		
		[Export ("allowsEmptySelection")]
		bool AllowsEmptySelection { get;  }

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
		AVMediaSelectionOption[] MediaSelectionOptionsFilteredAndSorted (AVMediaSelectionOption[] mediaSelectionOptions, string[] preferredLanguages);

		[iOS (8,0)][Mac (10,10)]
		[Export ("defaultOption"), NullAllowed]
		AVMediaSelectionOption DefaultOption { get; }
	}

	[Watch (6,0)]
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

	[Watch (6,0)]
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

	[Watch (6,0)]
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
		[iOS (8,0)][Mac (10,10)] [Watch (6,0)]
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

		[iOS (8,0)][Mac (10,10)][Watch (6,0)]
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

		[Watch (6,0)]
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

		[Watch (6,0)]
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

		[iOS (8,0)][Mac (10,10)][Watch (6,0)]
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

			[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataSpatialOverCaptureQualityScoringVersion")]
			NSString SpatialOverCaptureQualityScoringVersion { get; }

			[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataSpatialOverCaptureQualityScore")]
			NSString SpatialOverCaptureQualityScore { get; }

			[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataLivePhotoVitalityScoringVersion")]
			NSString LivePhotoVitalityScoringVersion { get; }

			[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataLivePhotoVitalityScore")]
			NSString LivePhotoVitalityScore { get; }

			[NoWatch, NoTV, NoMac, iOS (13, 0)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataDetectedSalientObject")]
			NSString DetectedSalientObject { get; }

			[NoWatch, NoTV, NoMac, iOS (13, 0)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataDetectedHumanBody")]
			NSString DetectedHumanBody { get; }

			[NoWatch, NoTV, NoMac, iOS (13, 0)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataDetectedDogBody")]
			NSString DetectedDogBody { get; }

			[NoWatch, NoTV, NoMac, iOS (13, 0)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataDetectedCatBody")]
			NSString DetectedCatBody { get; }

			[Watch (6, 0), TV (13, 0), NoMac, iOS (13, 0)]
			[Field ("AVMetadataIdentifierQuickTimeMetadataAutoLivePhoto")]
			NSString AutoLivePhoto { get; }
		}

		[iOS (8,0)][Mac (10,10)][Watch (6,0)]
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

		[iOS (8,0)][Mac (10,10)] [Watch (6,0)]
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

		[iOS (8,0)][Mac (10,10)][Watch (6,0)]
		[Static]
		interface IcyMetadata {
			[Field ("AVMetadataIdentifierIcyMetadataStreamTitle")]
			NSString StreamTitle { get; }
			
			[Field ("AVMetadataIdentifierIcyMetadataStreamURL")]
			NSString StreamUrl { get; }
		}
	}

	[Watch (6,0)]
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
		AVMetadataItem [] FilterWithKey (AVMetadataItem [] metadataItems, [NullAllowed] NSObject key, [NullAllowed] string keySpace);

		[iOS (7,0), Mac (10,9), NoWatch] // headers say it is the watch, but the AVMetadataItemFilter is not
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

	[Watch (6,0)]
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

	[iOS (7,0), Mac (10, 9), NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // for binary compatibility this is added in AVMetadataItemFilter.cs w/[Obsolete]
	interface AVMetadataItemFilter {
		[Static, Export ("metadataItemFilterForSharing")]
		AVMetadataItemFilter ForSharing { get; }
	}

	[NoWatch]
	[NoTV]
	[Mac (10,10)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSGenericException Reason: Cannot instantiate AVMetadataObject because it is an abstract superclass.
	[DisableDefaultCtor]
	interface AVMetadataObject {
		[Export ("duration")]
		CMTime Duration { get;  }

		[Export ("bounds")]
		CGRect Bounds { get;  }

		[Export ("type")]
		NSString WeakType { get;  }

		[Export ("time")]
		CMTime Time{ get;}

#if !XAMCORE_4_0
		[Field ("AVMetadataObjectTypeFace"), Mac (10,10)]
		NSString TypeFace { get; }

		[NoTV, iOS (7,0), Mac (10,15)]
		[Field ("AVMetadataObjectTypeAztecCode")]
		NSString TypeAztecCode { get; }
		
		[NoTV, iOS (7,0), Mac (10,15)]
		[Field ("AVMetadataObjectTypeCode128Code")]
		NSString TypeCode128Code { get; }
		
		[NoTV, iOS (7,0), Mac (10,15)]
		[Field ("AVMetadataObjectTypeCode39Code")]
		NSString TypeCode39Code { get; }
		
		[NoTV, iOS (7,0), Mac (10,15)]
		[Field ("AVMetadataObjectTypeCode39Mod43Code")]
		NSString TypeCode39Mod43Code { get; }
		
		[NoTV, iOS (7,0), Mac (10,15)]
		[Field ("AVMetadataObjectTypeCode93Code")]
		NSString TypeCode93Code { get; }
		
		[NoTV, iOS (7,0), Mac (10,15)]
		[Field ("AVMetadataObjectTypeEAN13Code")]
		NSString TypeEAN13Code { get; }
		
		[NoTV, iOS (7,0), Mac (10,15)]
		[Field ("AVMetadataObjectTypeEAN8Code")]
		NSString TypeEAN8Code { get; }
		
		[Field ("AVMetadataObjectTypePDF417Code")]
		[NoTV, iOS (7,0), Mac (10,15)]
		NSString TypePDF417Code { get; }
		
		[NoTV, iOS (7,0), Mac (10,15)]
		[Field ("AVMetadataObjectTypeQRCode")]
		NSString TypeQRCode { get; }
		
		[NoTV, iOS (7,0), Mac (10,15)]
		[Field ("AVMetadataObjectTypeUPCECode")]
		NSString TypeUPCECode { get; }

		[NoTV, iOS (8,0), Mac (10,15)]
		[Field ("AVMetadataObjectTypeInterleaved2of5Code")]
		NSString TypeInterleaved2of5Code { get; }
		
		[NoTV, iOS (8,0), Mac (10,15)]
		[Field ("AVMetadataObjectTypeITF14Code")]
		NSString TypeITF14Code { get; }
		
		[NoTV, iOS (8,0), Mac (10,15)]
		[Field ("AVMetadataObjectTypeDataMatrixCode")]
		NSString TypeDataMatrixCode { get; }

		[NoWatch, NoTV, iOS (13, 0), Mac (10, 15)]
		[Field ("AVMetadataObjectTypeCatBody")]
		NSString TypeCatBody { get; }

		[NoWatch, NoTV, iOS (13, 0), Mac (10, 15)]
		[Field ("AVMetadataObjectTypeDogBody")]
		NSString TypeDogBody { get; }

		[NoWatch, NoTV, iOS (13, 0), Mac (10, 15)]
		[Field ("AVMetadataObjectTypeHumanBody")]
		NSString TypeHumanBody { get; }

		[NoWatch, NoTV, iOS (13, 0), Mac (10, 15)]
		[Field ("AVMetadataObjectTypeSalientObject")]
		NSString TypeSalientObject { get; }
#endif
	}

#if XAMCORE_4_0
	[NoWatch]
	[NoTV]
#endif
	[Mac (10,10)]
	[Flags]
	enum AVMetadataObjectType : ulong {
		[Field (null)]
		None = 0,

		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeFace")]
		Face = 1 << 0,

		[iOS (7,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeAztecCode")]
		AztecCode = 1 << 1,

		[iOS (7,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeCode128Code")]
		Code128Code = 1 << 2,

		[iOS (7,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeCode39Code")]
		Code39Code = 1 << 3,

		[iOS (7,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeCode39Mod43Code")]
		Code39Mod43Code = 1 << 4,

		[iOS (7,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeCode93Code")]
		Code93Code = 1 << 5,

		[iOS (7,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeEAN13Code")]
		EAN13Code = 1 << 6,

		[iOS (7,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeEAN8Code")]
		EAN8Code = 1 << 7,

		[iOS (7,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypePDF417Code")]
		PDF417Code = 1 << 8,

		[iOS (7,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeQRCode")]
		QRCode = 1 << 9,

		[iOS (7,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeUPCECode")]
		UPCECode = 1 << 10,

		[iOS (8,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeInterleaved2of5Code")]
		Interleaved2of5Code = 1 << 11,

		[iOS (8,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeITF14Code")]
		ITF14Code = 1 << 12,

		[iOS (8,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeDataMatrixCode")]
		DataMatrixCode = 1 << 13,

		[iOS (13,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeCatBody")]
		CatBody = 1 << 14,

		[iOS (13,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeDogBody")]
		DogBody = 1 << 15,

		[iOS (13,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeHumanBody")]
		HumanBody = 1 << 16,

		[iOS (13,0), Mac (10,15)]
		[NoTV][NoWatch]
		[Field ("AVMetadataObjectTypeSalientObject")]
		SalientObject = 1 << 17,
	}

	[NoWatch]
	[NoTV]
	[Mac (10,10)]
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
	[iOS (7,0), Mac (10,15)]
	[BaseType (typeof (AVMetadataObject))]
	interface AVMetadataMachineReadableCodeObject {
		[Export ("corners", ArgumentSemantic.Copy)]
		NSDictionary [] WeakCorners { get; }

		[NullAllowed, Export ("stringValue", ArgumentSemantic.Copy)]
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

	[Watch (6,0), NoTV, Mac (10,10), iOS (13,0)]
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

	[Watch (6,0), iOS (13,0), NoTV]
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

	[Watch (6,0), iOS (13,0), NoTV]
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

	[Watch (6,0), NoTV, Mac (10,11), iOS (13,0)]
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

		[NullAllowed, Export ("defaultMediaDataStorage", ArgumentSemantic.Copy)]
		AVMediaDataStorage DefaultMediaDataStorage { get; set; }

		[Export ("interleavingPeriod", ArgumentSemantic.Assign)]
		CMTime InterleavingPeriod { get; set; }
	}

	[Watch (6,0), NoTV, iOS (13,0)]
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

	[Watch (6,0), NoTV, iOS (13,0)]
	[Category]
	[BaseType (typeof(AVMutableMovie))]
	interface AVMutableMovie_AVMutableMovieTrackLevelEditing
	{
		[Export ("mutableTrackCompatibleWithTrack:")]
		[return: NullAllowed]
		AVMutableMovieTrack GetMutableTrack (AVAssetTrack track);

		[Export ("addMutableTrackWithMediaType:copySettingsFromTrack:options:")]
		[return: NullAllowed]
		AVMutableMovieTrack AddMutableTrack (string mediaType, [NullAllowed] AVAssetTrack track, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("addMutableTracksCopyingSettingsFromTracks:options:")]
		AVMutableMovieTrack[] AddMutableTracks (AVAssetTrack[] existingTracks, [NullAllowed] NSDictionary<NSString, NSObject> options);

		[Export ("removeTrack:")]
		void RemoveTrack (AVMovieTrack track);
	}

	[Watch (6,0), NoTV, iOS (13,0)]
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

	[Mac (10,11), Watch (6,0), iOS (13,0), NoTV]
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

	[Mac (10,10), Watch (6,0), iOS (13,0), NoTV]
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

	[Watch (6,0), NoTV, iOS (13,0)]
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

	[Mac (10,10), Watch (6,0), iOS (13,0), NoTV]
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

	[Mac (10,10), Watch (6,0), iOS (13,0), NoTV]
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

	[Mac (10,11), Watch (6,0), iOS (13,0), NoTV]
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

		[Watch (6,0), NoTV, iOS (13,0), Mac (10,13)]
		[Export ("replaceFormatDescription:withFormatDescription:")]
		void ReplaceFormatDescription (CMFormatDescription formatDescription, CMFormatDescription newFormatDescription);
	}

	[Watch (6,0), NoTV, iOS (13,0)]
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

	[Watch (6,0), iOS (13,0), NoTV]
	[Category]
	[BaseType (typeof(AVMutableMovieTrack))]
	interface AVMutableMovieTrack_AVMutableMovieTrackTrackAssociations
	{
		[Export ("addTrackAssociationToTrack:type:")]
		void AddTrackAssociation (AVMovieTrack movieTrack, string trackAssociationType);

		[Export ("removeTrackAssociationToTrack:type:")]
		void RemoveTrackAssociation (AVMovieTrack movieTrack, string trackAssociationType);
	}

	[Mac (10,10), NoTV, Watch (6,0), iOS (13,0)]
	[BaseType (typeof(AVMovieTrack))]
	[DisableDefaultCtor]
	interface AVFragmentedMovieTrack
	{
#if !XAMCORE_4_0
		[Mac (10, 10), NoiOS, NoWatch]
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

		[Mac (10, 10), NoiOS, NoWatch]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use either 'AVFragmentedMovieTrackTimeRangeDidChangeNotification' or 'AVFragmentedMovieTrackSegmentsDidChangeNotification' instead. In either case, you can assume that the sender's 'TotalSampleDataLength' has changed.")]
		[Field ("AVFragmentedMovieTrackTotalSampleDataLengthDidChangeNotification")]
		NSString TotalSampleDataLengthDidChangeNotification { get; }
	}

	[Watch (6,0)]
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

	[Watch (6,0)]
	[BaseType (typeof (AVAssetTrack))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface AVCompositionTrack {
		[Export ("segments", ArgumentSemantic.Copy)]
		AVCompositionTrackSegment [] Segments { get; }

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("formatDescriptionReplacements")]
		AVCompositionTrackFormatDescriptionReplacement[] FormatDescriptionReplacements { get; }
	}

	[Watch (6,0)]
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

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[Export ("addTrackAssociationToTrack:type:")]
		void AddTrackAssociation (AVCompositionTrack compositionTrack, string trackAssociationType);

		[TV (12,0), Mac (10,14), iOS (12,0)]
		[Export ("removeTrackAssociationToTrack:type:")]
		void RemoveTrackAssociation (AVCompositionTrack compositionTrack, string trackAssociationType);

		[TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("replaceFormatDescription:withFormatDescription:")]
		void ReplaceFormatDescription (CMFormatDescription originalFormatDescription, [NullAllowed] CMFormatDescription replacementFormatDescription);

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }
	}

	[Watch (6,0)]
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
		[Field ("AVErrorDiscontinuityFlagsKey")]
		NSString DiscontinuityFlags { get; }
	}
	
	[Watch (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVAssetTrackSegment {
		[Export ("empty")]
		bool Empty { [Bind ("isEmpty")] get;  }

		[Export ("timeMapping")]
		CMTimeMapping TimeMapping { get; }

	}

	[Watch (6,0)]
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

	[iOS (9,0), Mac (10,11), Watch (6,0)]
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

	[Watch (6,0)]
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
	}

	[iOS (9,0), Mac (10,11), Watch (6,0)]
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

	[Watch (6,0)]
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

		[NoWatch]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'EstimateMaximumDuration' instead.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'EstimateMaximumDuration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'EstimateMaximumDuration' instead.")]
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
		void ExportAsynchronously (Action handler);

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

		[NoWatch, NoTV, NoiOS, Mac (10,15)]
		[Field ("AVAssetExportPresetAppleProRes4444LPCM")]
		NSString PresetAppleProRes4444Lpcm { get; }

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Field ("AVAssetExportPresetHEVC1920x1080WithAlpha")]
		NSString PresetHevc1920x1080WithAlpha { get; }

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Field ("AVAssetExportPresetHEVC3840x2160WithAlpha")]
		NSString PresetHevc3840x2160WithAlpha { get; }

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Field ("AVAssetExportPresetHEVCHighestQualityWithAlpha")]
		NSString PresetHevcHighestQualityWithAlpha { get; }

		// 5.0 APIs
		[Export ("asset", ArgumentSemantic.Retain)]
		AVAsset Asset { get; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'EstimateOutputFileLength' for more precise results.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use 'EstimateOutputFileLength' for more precise results.")]
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'EstimateOutputFileLength' for more precise results.")]
		[Export ("estimatedOutputFileLength")]
		long EstimatedOutputFileLength { get; }

		[Mac (10, 9)]
		[Static, Export ("determineCompatibilityOfExportPreset:withAsset:outputFileType:completionHandler:")]
		[Async]
		void DetermineCompatibilityOfExportPreset (string presetName, AVAsset asset, [NullAllowed] string outputFileType, Action<bool> isCompatibleResult);

		[Async]
		[Wrap ("DetermineCompatibilityOfExportPreset (presetName, asset, outputFileType.GetConstant (), isCompatibleResult)")]
		void DetermineCompatibilityOfExportPreset (string presetName, AVAsset asset, [NullAllowed] AVFileTypes outputFileType, Action<bool> isCompatibleResult);

		[Mac (10, 9)]
		[Export ("determineCompatibleFileTypesWithCompletionHandler:")]
		[Async]
		void DetermineCompatibleFileTypes (Action<string []> compatibleFileTypesHandler);

		[iOS (7,0), Mac (10,9)]
		[Export ("metadataItemFilter", ArgumentSemantic.Retain), NullAllowed]
		AVMetadataItemFilter MetadataItemFilter { get; set; }

		[NoWatch]
		[Mac (10,9)]
		[iOS (7,0)]
		[NullAllowed, Export ("customVideoCompositor", ArgumentSemantic.Copy)]
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

		[Async]
		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("estimateMaximumDurationWithCompletionHandler:")]
		void EstimateMaximumDuration (Action<CMTime, NSError> handler);

		[Async]
		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("estimateOutputFileLengthWithCompletionHandler:")]
		void EstimateOutputFileLength (Action<long, NSError> handler);
	}

	[iOS (7,0), Watch (6,0)]
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

	[Watch (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVAudioMix : NSMutableCopying {
		[Export ("inputParameters", ArgumentSemantic.Copy)]
		AVAudioMixInputParameters [] InputParameters { get;  }
	}

	[Watch (6,0)]
	[BaseType (typeof (AVAudioMix))]
	interface AVMutableAudioMix {
		[Export ("inputParameters", ArgumentSemantic.Copy)]
		AVAudioMixInputParameters [] InputParameters { get; set;  }

		[Static, Export ("audioMix")]
		AVMutableAudioMix Create ();
	}

	[Watch (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVAudioMixInputParameters : NSMutableCopying {
		[Export ("trackID")]
		int TrackID { get;  } // defined as 'CMPersistentTrackID' = int32_t

		[Export ("getVolumeRampForTime:startVolume:endVolume:timeRange:")]
		bool GetVolumeRamp (CMTime forTime, ref float /* defined as 'float*' */ startVolume, ref float /* defined as 'float*' */ endVolume, ref CMTimeRange timeRange);

		[Mac (10,9), NoWatch]
		[NullAllowed]
		[Export ("audioTapProcessor", ArgumentSemantic.Retain)]
		MTAudioProcessingTap AudioTapProcessor { get; [NotImplemented] set;}

		[iOS (7,0), Mac (10,11)]
		[NullAllowed] // by default this property is null
		[Export ("audioTimePitchAlgorithm", ArgumentSemantic.Copy)]
		NSString AudioTimePitchAlgorithm { get; [NotImplemented] set; }
	}

	[Watch (6,0)]
	[BaseType (typeof (AVAudioMixInputParameters))]
	interface AVMutableAudioMixInputParameters {
		[Export ("trackID")]
		int TrackID { get; set;  } // defined as 'CMPersistentTrackID'

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

		[Mac (10,9), NoWatch]
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

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("anticipateRenderingUsingHint:")]
		void AnticipateRendering (AVVideoCompositionRenderHint renderHint);

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("prerollForRenderingUsingHint:")]
		void PrerollForRendering (AVVideoCompositionRenderHint renderHint);
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

		[NoWatch]
		[Export ("animationTool", ArgumentSemantic.Retain), NullAllowed]
		AVVideoCompositionCoreAnimationTool AnimationTool { get;  }

		[Mac (10,14)]
		[Export ("renderScale")]
		float RenderScale { get; [NotImplemented] set; } // defined as 'float'

		[NoWatch]
		[Export ("isValidForAsset:timeRange:validationDelegate:")]
		bool IsValidForAsset ([NullAllowed] AVAsset asset, CMTimeRange timeRange, [Protocolize] [NullAllowed] AVVideoCompositionValidationHandling validationDelegate);

		[Mac (10, 9)]
		[Static, Export ("videoCompositionWithPropertiesOfAsset:")]
		AVVideoComposition FromAssetProperties (AVAsset asset);

		[iOS (7,0), Mac (10, 9)]
		[Export ("customVideoCompositorClass", ArgumentSemantic.Copy), NullAllowed]
		Class CustomVideoCompositorClass { get; [NotImplemented] set; }

		[NoWatch]
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
	
	[Watch (6,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[DisableDefaultCtor]
	interface AVVideoCompositionValidationHandling {
		[NoWatch]
		[Export ("videoComposition:shouldContinueValidatingAfterFindingInvalidValueForKey:")]
		bool ShouldContinueValidatingAfterFindingInvalidValueForKey (AVVideoComposition videoComposition, string key);

		[NoWatch]
		[Export ("videoComposition:shouldContinueValidatingAfterFindingEmptyTimeRange:")]
		bool ShouldContinueValidatingAfterFindingEmptyTimeRange (AVVideoComposition videoComposition, CMTimeRange timeRange);

		[NoWatch]
		[Export ("videoComposition:shouldContinueValidatingAfterFindingInvalidTimeRangeInInstruction:")]
		bool ShouldContinueValidatingAfterFindingInvalidTimeRangeInInstruction (AVVideoComposition videoComposition, AVVideoCompositionInstruction videoCompositionInstruction);

		[NoWatch]
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

		[Export ("instructions", ArgumentSemantic.Copy)]
		AVVideoCompositionInstruction [] Instructions { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("animationTool", ArgumentSemantic.Retain)]
		AVVideoCompositionCoreAnimationTool AnimationTool { get; set;  }

		[Mac (10,14)]
		[Export ("renderScale")]
		float RenderScale { get; set; } // defined as 'float'

		[Static, Export ("videoComposition")]
		AVMutableVideoComposition Create ();

		// in 7.0 they declared this was available in 6.0
		[Mac (10, 9)]
		[Static, Export ("videoCompositionWithPropertiesOfAsset:")]
		AVMutableVideoComposition Create (AVAsset asset);

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Static]
		[Export ("videoCompositionWithPropertiesOfAsset:prototypeInstruction:")]
		AVMutableVideoComposition Create (AVAsset asset, AVVideoCompositionInstruction prototypeInstruction);

		[NullAllowed]
		[iOS (7,0), Mac (10, 9)]
		[Export ("customVideoCompositorClass", ArgumentSemantic.Retain)]
		[Override]
		Class CustomVideoCompositorClass { get; set; }

		[NoWatch]
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
	[BaseType (typeof (NSObject))]
	interface AVVideoCompositionInstruction : NSSecureCoding, NSMutableCopying {
		[Export ("timeRange")]
		CMTimeRange TimeRange { get;  [NotImplemented ("Not available on AVVideoCompositionInstruction, only available on AVMutableVideoCompositionInstruction")]set; }

		[NullAllowed]
		[Export ("backgroundColor", ArgumentSemantic.Retain)]
		CGColor BackgroundColor { get;
			[NotImplemented] set;
		}

		[NoWatch]
		[Export ("layerInstructions", ArgumentSemantic.Copy)]
		AVVideoCompositionLayerInstruction [] LayerInstructions { get; [NotImplemented ("Not available on AVVideoCompositionInstruction, only available on AVMutableVideoCompositionInstruction")]set; }

		[Export ("enablePostProcessing")]
		bool EnablePostProcessing { get; [NotImplemented ("Not available on AVVideoCompositionInstruction, only available on AVMutableVideoCompositionInstruction")]set; }

		// These are there because it adopts the protocol *of the same name*

		[iOS (7,0), Mac (10, 9)]
		[Export ("containsTweening")]
		bool ContainsTweening { get; }

		[iOS (7,0), Mac (10, 9)]
		[NullAllowed, Export ("requiredSourceTrackIDs")]
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

		[Export ("layerInstructions", ArgumentSemantic.Copy)]
		[Override]
		AVVideoCompositionLayerInstruction [] LayerInstructions { get; set;  }

		[Static, Export ("videoCompositionInstruction")]
		AVVideoCompositionInstruction Create (); 		
	}

	[NoWatch]
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

		[Mac (10,15)]
		[Field ("AVCaptureSessionPreset1920x1080")]
		NSString Preset1920x1080 { get; }

		[Mac (10,15)]
		[iOS (9,0)]
		[Field ("AVCaptureSessionPreset3840x2160")]
		NSString Preset3840x2160 { get; }

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
		[Field ("AVCaptureSessionPreset320x240")]
		NSString Preset320x240 { get; }

		[NoiOS]
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

		[Mac (10, 14)]
		[Field ("AVCaptureSessionInterruptionEndedNotification")]
		[Notification]
		NSString InterruptionEndedNotification { get; }

		[Mac (10, 14)]
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
		[Export ("masterClock"), NullAllowed]
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

		[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
		[Export ("connections")]
		AVCaptureConnection[] Connections { get; }
	}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface AVCaptureConnection {
		
		[iOS (8,0)]
		[Static]
		[Export ("connectionWithInputPorts:output:")]
		AVCaptureConnection FromInputPorts (AVCaptureInputPort [] ports, AVCaptureOutput output);
		
		[iOS (8,0)]
		[Static]
		[Export ("connectionWithInputPort:videoPreviewLayer:")]
		AVCaptureConnection FromInputPort (AVCaptureInputPort port, AVCaptureVideoPreviewLayer layer);
		
		[iOS (8,0)]
		[Export ("initWithInputPorts:output:")]
		IntPtr Constructor (AVCaptureInputPort [] inputPorts, AVCaptureOutput output);

		[iOS (8,0)]
		[Export ("initWithInputPort:videoPreviewLayer:")]
		IntPtr Constructor (AVCaptureInputPort inputPort, AVCaptureVideoPreviewLayer layer);

		[NullAllowed]
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
		[Export ("videoMinFrameDuration")]
		CMTime VideoMinFrameDuration { get; set;  }
#if !MONOMAC
		[Export ("supportsVideoMaxFrameDuration"), Internal]
		bool _SupportsVideoMaxFrameDuration { [Bind ("isVideoMaxFrameDurationSupported")] get;  }

		[Export ("videoMaxFrameDuration")]
		[Deprecated (PlatformName.iOS, 7, 0 /* Only deprecated on iOS */)] 
		CMTime VideoMaxFrameDuration { get; set;  }

		[Export ("videoMaxScaleAndCropFactor")]
		nfloat VideoMaxScaleAndCropFactor { get;  }

		[Export ("videoScaleAndCropFactor")]
		nfloat VideoScaleAndCropFactor { get; set;  }
#endif
		[NullAllowed]
		[Export ("videoPreviewLayer")]
		AVCaptureVideoPreviewLayer VideoPreviewLayer { get;  }

		[Export ("automaticallyAdjustsVideoMirroring")]
		bool AutomaticallyAdjustsVideoMirroring { get; set;  }
#if !MONOMAC
		[Export ("supportsVideoStabilization")]
		bool SupportsVideoStabilization { [Bind ("isVideoStabilizationSupported")] get;  }

		[Export ("videoStabilizationEnabled")]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'ActiveVideoStabilizationMode' instead.")]
		bool VideoStabilizationEnabled { [Bind ("isVideoStabilizationEnabled")] get;  }

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

		[NullAllowed, Export ("formatDescription")]
		CMFormatDescription FormatDescription { get;  }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set;  }

		[Export ("input")]
		AVCaptureInput Input  { get; }

		[iOS (7,0), Mac (10, 9), Export ("clock", ArgumentSemantic.Copy), NullAllowed]
		CMClock Clock { get; }

		[BindAs (typeof (AVCaptureDeviceType))]
		[NoMac, iOS (13,0)]
		[NullAllowed, Export ("sourceDeviceType")]
		NSString SourceDeviceType { get; }

		[NoMac, iOS (13,0)]
		[Export ("sourceDevicePosition")]
		AVCaptureDevicePosition SourceDevicePosition { get; }
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
		void SetDelegate ([NullAllowed] IAVCaptureDepthDataOutputDelegate del,[NullAllowed] DispatchQueue callbackQueue);

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

	[NoWatch]
	[NoTV]
	[BaseType (typeof (AVCaptureInput))]
	// crash application if 'init' is called
	[DisableDefaultCtor]
	interface AVCaptureDeviceInput {
		[Export ("device")]
		AVCaptureDevice Device { get;  }

		[Static, Export ("deviceInputWithDevice:error:")]
		[return: NullAllowed]
		AVCaptureDeviceInput FromDevice (AVCaptureDevice device, out NSError error);

		[Export ("initWithDevice:error:")]
		IntPtr Constructor (AVCaptureDevice device, out NSError error);

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("unifiedAutoExposureDefaultsEnabled")]
		bool UnifiedAutoExposureDefaultsEnabled { get; set; }

		[NoMac, iOS (13,0)]
		[Export ("portsWithMediaType:sourceDeviceType:sourceDevicePosition:")]
		AVCaptureInputPort[] GetPorts ([BindAs (typeof (AVMediaTypes))] [NullAllowed] NSString mediaType, [BindAs (typeof (AVCaptureDeviceType))][NullAllowed] NSString sourceDeviceType, AVCaptureDevicePosition sourceDevicePosition);

		[NoMac, iOS (13,0)]
		[Export ("videoMinFrameDurationOverride", ArgumentSemantic.Assign)]
		CMTime VideoMinFrameDurationOverride { get; set; }
	}

#if MONOMAC
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	interface AVCaptureDeviceInputSource {
		[Export ("inputSourceID")]
		string InputSourceID { get; }

		[Export ("localizedName")]
		string LocalizedName { get; }
	}

	[NoWatch]
	[BaseType (typeof (AVCaptureFileOutput))]
	[NoTV]
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

	[NoWatch]
	[NoTV]
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
		[return: NullAllowed]
		AVCaptureConnection ConnectionFromMediaType (NSString avMediaType);

		[iOS (7,0), Mac (10,15)]
		[Export ("metadataOutputRectOfInterestForRect:")]
		CGRect GetMetadataOutputRectOfInterestForRect (CGRect rectInOutputCoordinates);

		[iOS (7,0), Mac (10,15)]
		[Export ("rectForMetadataOutputRectOfInterest:")]
		CGRect GetRectForMetadataOutputRectOfInterest (CGRect rectInMetadataOutputCoordinates);

		[Mac (10,15)]
		[Export ("transformedMetadataObjectForMetadataObject:connection:")]
		[return: NullAllowed]
		AVMetadataObject GetTransformedMetadataObject (AVMetadataObject metadataObject, AVCaptureConnection connection);
	}

#if MONOMAC
	[NoWatch]
	[NoTV]
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

		[Export ("capturesCursor")]
		bool CapturesCursor { get; set; }

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

		[iOS (8,0)]
		[Export ("setSessionWithNoConnection:")]
		void SetSessionWithNoConnection (AVCaptureSession session);

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

		[iOS (8,0)]
		[Internal]
		[Export ("initWithSessionWithNoConnection:")]
		IntPtr InitWithNoConnection (AVCaptureSession session);

		[NullAllowed, Export ("connection")]
		AVCaptureConnection Connection { get; }

		[Mac (10,15)]
		[Export ("captureDevicePointOfInterestForPoint:")]
		CGPoint CaptureDevicePointOfInterestForPoint (CGPoint pointInLayer);

		[Mac (10,15)]
		[Export ("pointForCaptureDevicePointOfInterest:")]
		CGPoint PointForCaptureDevicePointOfInterest (CGPoint captureDevicePointOfInterest);

		[Mac (10,15)]
		[Export ("transformedMetadataObjectForMetadataObject:")]
		[return: NullAllowed]
		AVMetadataObject GetTransformedMetadataObject (AVMetadataObject metadataObject);

		[iOS (7,0), Mac (10,15)]
		[Export ("metadataOutputRectOfInterestForRect:")]
		CGRect MapToMetadataOutputCoordinates (CGRect rectInLayerCoordinates);
		
		[iOS (7,0), Mac (10,15)]
		[Export ("rectForMetadataOutputRectOfInterest:")]
		CGRect MapToLayerCoordinates (CGRect rectInMetadataOutputCoordinates);

		[iOS (8,0), Mac (10,15)]
		[Static]
		[Export ("layerWithSessionWithNoConnection:")]
		AVCaptureVideoPreviewLayer CreateWithNoConnection (AVCaptureSession session);

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("previewing")]
		bool Previewing { [Bind ("isPreviewing")] get; }
	}

	[NoTV]
	[NoWatch]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureVideoDataOutput {
		[NullAllowed, Export ("sampleBufferDelegate")]
		[Protocolize]
		AVCaptureVideoDataOutputSampleBufferDelegate SampleBufferDelegate { get; }

		[NullAllowed, Export ("sampleBufferCallbackQueue")]
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

		[iOS (7,0), Mac (10,15)]
		[Export ("recommendedVideoSettingsForAssetWriterWithOutputFileType:")]
		[return: NullAllowed]
		NSDictionary GetRecommendedVideoSettingsForAssetWriter (string outputFileType);

		[iOS (11,0), Mac (10,15)]
		[Export ("availableVideoCodecTypesForAssetWriterWithOutputFileType:")]
		string[] GetAvailableVideoCodecTypes (string outputFileType);

		[Internal]
		[iOS (11,0), Mac (10,15)]
		[Export ("recommendedVideoSettingsForVideoCodecType:assetWriterOutputFileType:")]
		[return: NullAllowed]
		NSDictionary GetWeakRecommendedVideoSettings (string videoCodecType, string outputFileType);

		[iOS (11,0), Mac (10,15)]
		[Wrap ("new AVPlayerItemVideoOutputSettings (GetWeakRecommendedVideoSettings (videoCodecType, outputFileType)!)")]
		[return: NullAllowed]
		AVPlayerItemVideoOutputSettings GetRecommendedVideoSettings (string videoCodecType, string outputFileType);

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("automaticallyConfiguresOutputBufferDimensions")]
		bool AutomaticallyConfiguresOutputBufferDimensions { get; set; }

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("deliversPreviewSizedOutputBuffers")]
		bool DeliversPreviewSizedOutputBuffers { get; set; }
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

		[Export ("captureOutput:didDropSampleBuffer:fromConnection:")]
		void DidDropSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection);
	}

	interface IAVCaptureVideoDataOutputSampleBufferDelegate {}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureAudioDataOutput {
		[NullAllowed, Export ("sampleBufferDelegate")]
		[Protocolize]
		AVCaptureAudioDataOutputSampleBufferDelegate SampleBufferDelegate { get;  }

		[NullAllowed, Export ("sampleBufferCallbackQueue")]
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
		void SetSampleBufferDelegateQueue ([NullAllowed] AVCaptureAudioDataOutputSampleBufferDelegate sampleBufferDelegate, [NullAllowed] DispatchQueue sampleBufferCallbackDispatchQueue);
#endif

		[iOS (7,0), Mac (10,15)]
		[Export ("recommendedAudioSettingsForAssetWriterWithOutputFileType:")]
		[return: NullAllowed]
		NSDictionary GetRecommendedAudioSettingsForAssetWriter (string outputFileType);

		[NoiOS]
		[Export ("audioSettings", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDictionary WeakAudioSettings { get; set; }

		[NoiOS]
		[Wrap ("WeakAudioSettings")]
		[NullAllowed]
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

	[NoMac]
	[NoWatch]
	[NoTV]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	[Abstract]
	interface AVCaptureBracketedStillImageSettings {
		// Abstract class in obJC
	}

	[NoMac]
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

	[NoWatch, NoMac]
	[NoTV]
	[iOS (8,0)]
	[BaseType (typeof (AVCaptureBracketedStillImageSettings))]
	interface AVCaptureAutoExposureBracketedStillImageSettings {
		[Export ("exposureTargetBias")]
		float ExposureTargetBias { get; } /* float, not CGFloat */

		[Static, Export ("autoExposureSettingsWithExposureTargetBias:")]
		AVCaptureAutoExposureBracketedStillImageSettings Create (float /* float, not CGFloat */ exposureTargetBias);
	}
	
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

		[NullAllowed, Export ("outputFileURL")]
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
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		IAVCaptureFileOutputDelegate Delegate { get; set; }

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

		[Abstract]
		[Export ("captureOutput:didFinishRecordingToOutputFileAtURL:fromConnections:error:"), CheckDisposed]
		void FinishedRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject [] connections, [NullAllowed] NSError error);

#if MONOMAC
		[Export ("captureOutput:didPauseRecordingToOutputFileAtURL:fromConnections:")]
		void DidPauseRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, AVCaptureConnection [] connections);

		[Export ("captureOutput:didResumeRecordingToOutputFileAtURL:fromConnections:")]
		void DidResumeRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, AVCaptureConnection [] connections);

		[Export ("captureOutput:willFinishRecordingToOutputFileAtURL:fromConnections:error:")]
		void WillFinishRecording (AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, AVCaptureConnection [] connections, [NullAllowed] NSError error);
#endif
	}

#if !MONOMAC
	[NoWatch]
	[NoTV]
	[BaseType (typeof (AVCaptureOutput))]
	interface AVCaptureMetadataOutput {
		[NullAllowed, Export ("metadataObjectsDelegate")]
		[Protocolize]
		AVCaptureMetadataOutputObjectsDelegate Delegate { get;  }

		[NullAllowed, Export ("metadataObjectsCallbackQueue")]
		DispatchQueue CallbackQueue { get;  }

		[Export ("availableMetadataObjectTypes")]
		NSString [] WeakAvailableMetadataObjectTypes { get;  }

		[NullAllowed]
		[Export ("metadataObjectTypes", ArgumentSemantic.Copy)]
		NSString [] WeakMetadataObjectTypes { get; set;  }

		[Export ("setMetadataObjectsDelegate:queue:")]
		void SetDelegate ([NullAllowed][Protocolize] AVCaptureMetadataOutputObjectsDelegate objectsDelegate, [NullAllowed] DispatchQueue objectsCallbackQueue);

		[iOS (7,0)]
		[Export ("rectOfInterest", ArgumentSemantic.Copy)]
		CGRect RectOfInterest { get; set; }
		
	}

	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVCaptureMetadataOutputObjectsDelegate {
		[Export ("captureOutput:didOutputMetadataObjects:fromConnection:")]
		void DidOutputMetadataObjects (AVCaptureMetadataOutput captureOutput, AVMetadataObject [] metadataObjects, AVCaptureConnection connection);
	}
#endif

	[NoTV, Mac (10,15), NoWatch, iOS (12,0)]
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

	
	[NoTV, Mac (10,15), NoWatch, iOS (12,0)]
	[StrongDictionary ("AVCapturePhotoSettingsThumbnailFormatKeys")]
	interface AVCapturePhotoSettingsThumbnailFormat {
		NSString Codec { get; set; }
		NSNumber Width { get; set; }
		NSNumber Height { get; set; }
	}

	[NoWatch]
	[NoTV, Mac (10,15), iOS (10,0)]
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

		[NoMac]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'PhotoQualityPrioritization' instead.")]
		[Export ("autoStillImageStabilizationEnabled")]
		bool IsAutoStillImageStabilizationEnabled { [Bind ("isAutoStillImageStabilizationEnabled")] get; set; }

		[Export ("highResolutionPhotoEnabled")]
		bool IsHighResolutionPhotoEnabled { [Bind ("isHighResolutionPhotoEnabled")] get; set; }

		[NullAllowed, Export ("livePhotoMovieFileURL", ArgumentSemantic.Copy)]
		NSUrl LivePhotoMovieFileUrl { get; set; }

		[NullAllowed, Export ("livePhotoMovieMetadata", ArgumentSemantic.Copy)]
		AVMetadataItem[] LivePhotoMovieMetadata { get; set; }

		[Export ("availablePreviewPhotoPixelFormatTypes")]
		NSNumber[] AvailablePreviewPhotoPixelFormatTypes { get; }

		[NullAllowed, Export ("previewPhotoFormat", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> PreviewPhotoFormat { get; set; }

		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'AutoVirtualDeviceFusionEnabled' instead.")]
		[iOS (10, 2)]
		[Export ("autoDualCameraFusionEnabled")]
		bool AutoDualCameraFusionEnabled { [Bind ("isAutoDualCameraFusionEnabled")] get; set; }

		[iOS (11, 0)]
		[NullAllowed, Export ("processedFileType")]
		string ProcessedFileType { get; }

		[iOS (11, 0)]
		[NullAllowed, Export ("rawFileType")]
		string RawFileType { get; }

		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'VirtualDeviceConstituentPhotoDeliveryEnabled' instead.")]
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

		[NoMac]
		[Wrap ("WeakRawEmbeddedThumbnailPhotoFormat")]
		AVCapturePhotoSettingsThumbnailFormat RawEmbeddedThumbnailPhotoFormat { get; set; }

		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("autoRedEyeReductionEnabled")]
		bool AutoRedEyeReductionEnabled { [Bind ("isAutoRedEyeReductionEnabled")] get; set; }

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("photoQualityPrioritization", ArgumentSemantic.Assign)]
		AVCapturePhotoQualityPrioritization PhotoQualityPrioritization { get; set; }

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("embedsSemanticSegmentationMattesInPhoto")]
		bool EmbedsSemanticSegmentationMattesInPhoto { get; set; }

		[BindAs (typeof (AVSemanticSegmentationMatteType[]))]
		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Export ("enabledSemanticSegmentationMatteTypes", ArgumentSemantic.Assign)]
		NSString[] EnabledSemanticSegmentationMatteTypes { get; set; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Export ("virtualDeviceConstituentPhotoDeliveryEnabledDevices", ArgumentSemantic.Copy)]
		AVCaptureDevice[] VirtualDeviceConstituentPhotoDeliveryEnabledDevices { get; set; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Export ("autoVirtualDeviceFusionEnabled")]
		bool AutoVirtualDeviceFusionEnabled { [Bind ("isAutoVirtualDeviceFusionEnabled")] get; set; }
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
	[NoTV, Mac (10,15), iOS (10,0)]
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

		[NoMac]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'AVCaptureResolvedPhotoSettings.PhotoProcessingTimeRange' instead.")]
		[Export ("stillImageStabilizationEnabled")]
		bool IsStillImageStabilizationEnabled { [Bind ("isStillImageStabilizationEnabled")] get; }

		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'VirtualDeviceFusionEnabled' instead.")]
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

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("dimensionsForSemanticSegmentationMatteOfType:")]
		CMVideoDimensions GetDimensions ([BindAs (typeof (AVSemanticSegmentationMatteType))]NSString semanticSegmentationMatteType);

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("photoProcessingTimeRange")]
		CMTimeRange PhotoProcessingTimeRange { get; }

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("virtualDeviceFusionEnabled")]
		bool VirtualDeviceFusionEnabled { [Bind ("isVirtualDeviceFusionEnabled")] get; }
	}


	interface IAVCapturePhotoCaptureDelegate {}

	[NoWatch]
	[NoTV, Mac (10,15), iOS (10,0)]
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

		[NoMac]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use the 'DidFinishProcessingPhoto' overload accepting a 'AVCapturePhoto' instead.")]
		[Export ("captureOutput:didFinishProcessingPhotoSampleBuffer:previewPhotoSampleBuffer:resolvedSettings:bracketSettings:error:")]
		void DidFinishProcessingPhoto (AVCapturePhotoOutput captureOutput, [NullAllowed] CMSampleBuffer photoSampleBuffer, [NullAllowed] CMSampleBuffer previewPhotoSampleBuffer, AVCaptureResolvedPhotoSettings resolvedSettings, [NullAllowed] AVCaptureBracketedStillImageSettings bracketSettings, [NullAllowed] NSError error);

		[NoMac]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use the 'DidFinishProcessingPhoto' overload accepting a 'AVCapturePhoto' instead.")]
		[Export ("captureOutput:didFinishProcessingRawPhotoSampleBuffer:previewPhotoSampleBuffer:resolvedSettings:bracketSettings:error:")]
		void DidFinishProcessingRawPhoto (AVCapturePhotoOutput captureOutput, [NullAllowed] CMSampleBuffer rawSampleBuffer, [NullAllowed] CMSampleBuffer previewPhotoSampleBuffer, AVCaptureResolvedPhotoSettings resolvedSettings, [NullAllowed] AVCaptureBracketedStillImageSettings bracketSettings, [NullAllowed] NSError error);

		[iOS (11,0)]
		[Export ("captureOutput:didFinishProcessingPhoto:error:")]
		void DidFinishProcessingPhoto (AVCapturePhotoOutput output, AVCapturePhoto photo, [NullAllowed] NSError error);

		[NoMac]
		[Export ("captureOutput:didFinishRecordingLivePhotoMovieForEventualFileAtURL:resolvedSettings:")]
		void DidFinishRecordingLivePhotoMovie (AVCapturePhotoOutput captureOutput, NSUrl outputFileUrl, AVCaptureResolvedPhotoSettings resolvedSettings);

		[NoMac]
		[Export ("captureOutput:didFinishProcessingLivePhotoToMovieFileAtURL:duration:photoDisplayTime:resolvedSettings:error:")]
		void DidFinishProcessingLivePhotoMovie (AVCapturePhotoOutput captureOutput, NSUrl outputFileUrl, CMTime duration, CMTime photoDisplayTime, AVCaptureResolvedPhotoSettings resolvedSettings, [NullAllowed] NSError error);

		[Export ("captureOutput:didFinishCaptureForResolvedSettings:error:")]
		void DidFinishCapture (AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings, [NullAllowed] NSError error);
	}

	[NoWatch]
	[NoTV, Mac (10,15), iOS (10,0)]
	[BaseType (typeof(AVCaptureOutput))]
	interface AVCapturePhotoOutput
	{
		[Export ("capturePhotoWithSettings:delegate:")]
		void CapturePhoto (AVCapturePhotoSettings settings, IAVCapturePhotoCaptureDelegate cb);

		[Export ("availablePhotoPixelFormatTypes")]
		NSNumber [] AvailablePhotoPixelFormatTypes { get; }

		[Export ("availablePhotoCodecTypes")]
		string [] AvailablePhotoCodecTypes { get; }

		[NoMac]
		[Export ("availableRawPhotoPixelFormatTypes")]
		NSNumber [] AvailableRawPhotoPixelFormatTypes { get; }

		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'MaxPhotoQualityPrioritization' instead.")]
		[NoMac]
		[Export ("stillImageStabilizationSupported")]
		bool IsStillImageStabilizationSupported { [Bind ("isStillImageStabilizationSupported")] get; }

		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'MaxPhotoQualityPrioritization' instead.")]
		[NoMac]
		[Export ("isStillImageStabilizationScene")]
		bool IsStillImageStabilizationScene { get; }

		[NoMac]
#if XAMCORE_4_0
		[BindAs (typeof (AVCaptureFlashMode []))]
#endif		
		[Export ("supportedFlashModes")]
		NSNumber[] SupportedFlashModes { get; }

		[NoMac]
		[Export ("isFlashScene")]
		bool IsFlashScene { get; }

		[NoMac]
		[NullAllowed,Export ("photoSettingsForSceneMonitoring", ArgumentSemantic.Copy)]
		AVCapturePhotoSettings PhotoSettingsForSceneMonitoring { get; set; }

		[NoMac]
		[Export ("highResolutionCaptureEnabled")]
		bool IsHighResolutionCaptureEnabled { [Bind ("isHighResolutionCaptureEnabled")] get; set; }

		[NoMac]
		[Export ("maxBracketedCapturePhotoCount")]
		nuint MaxBracketedCapturePhotoCount { get; }

		[NoMac]
		[Export ("lensStabilizationDuringBracketedCaptureSupported")]
		bool IsLensStabilizationDuringBracketedCaptureSupported { [Bind ("isLensStabilizationDuringBracketedCaptureSupported")] get; }

		[NoMac]
		[Export ("livePhotoCaptureSupported")]
		bool IsLivePhotoCaptureSupported { [Bind ("isLivePhotoCaptureSupported")] get; }

		[NoMac]
		[Export ("livePhotoCaptureEnabled")]
		bool IsLivePhotoCaptureEnabled { [Bind ("isLivePhotoCaptureEnabled")] get; set; }

		[NoMac]
		[Export ("livePhotoCaptureSuspended")]
		bool IsLivePhotoCaptureSuspended { [Bind ("isLivePhotoCaptureSuspended")] get; set; }

		[NoMac]
		[Export ("livePhotoAutoTrimmingEnabled")]
		bool IsLivePhotoAutoTrimmingEnabled { [Bind ("isLivePhotoAutoTrimmingEnabled")] get; set; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'AVCapturePhoto.FileDataRepresentation' instead.")]
		[Static]
		[Export ("JPEGPhotoDataRepresentationForJPEGSampleBuffer:previewPhotoSampleBuffer:")]
		[return: NullAllowed]
		NSData GetJpegPhotoDataRepresentation (CMSampleBuffer JPEGSampleBuffer, [NullAllowed] CMSampleBuffer previewPhotoSampleBuffer);

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'AVCapturePhoto.FileDataRepresentation' instead.")]
		[Static]
		[Export ("DNGPhotoDataRepresentationForRawSampleBuffer:previewPhotoSampleBuffer:")]
		[return: NullAllowed]
		NSData GetDngPhotoDataRepresentation (CMSampleBuffer rawSampleBuffer, [NullAllowed] CMSampleBuffer previewPhotoSampleBuffer);

		[NoMac]
		[Export ("preparedPhotoSettingsArray")]
		AVCapturePhotoSettings[] PreparedPhotoSettings { get; }

		[NoMac]
		[Export ("setPreparedPhotoSettingsArray:completionHandler:")]
		[Async]
		void SetPreparedPhotoSettings (AVCapturePhotoSettings[] preparedPhotoSettingsArray, [NullAllowed] Action<bool, NSError> completionHandler);

		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'VirtualDeviceFusionSupported' instead.")]
		[iOS (10, 2), NoMac]
		[Export ("dualCameraFusionSupported")]
		bool DualCameraFusionSupported { [Bind ("isDualCameraFusionSupported")] get; }

		// From AVCapturePhotoOutput (AVCapturePhotoOutputDepthDataDeliverySupport) Category

		[iOS (11,0), NoMac]
		[Export ("depthDataDeliverySupported")]
		bool DepthDataDeliverySupported { [Bind ("isDepthDataDeliverySupported")] get; }

		[iOS (11,0), NoMac]
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
		[iOS (11, 0), NoMac]
		[Export ("availableRawPhotoFileTypes")]
		NSString[] _GetAvailableRawPhotoFileTypes { get; }

		[iOS (11, 0), NoMac]
		[Wrap ("Array.ConvertAll (_GetAvailableRawPhotoFileTypes, s => AVFileTypesExtensions.GetValue (s))")]
		AVFileTypes[] GetAvailableRawPhotoFileTypes { get; }

		[iOS (11,0)]
		[Export ("supportedPhotoPixelFormatTypesForFileType:")]
		NSNumber[] GetSupportedPhotoPixelFormatTypesForFileType (string fileType);

		[Internal]
		[iOS (11,0), Mac (10,15)]
		[Export ("supportedPhotoCodecTypesForFileType:")]
		NSString[] _GetSupportedPhotoCodecTypesForFileType (string fileType);

		[iOS (11,0), NoMac]
		[Wrap ("Array.ConvertAll (_GetSupportedPhotoCodecTypesForFileType (fileType), s => AVVideoCodecTypeExtensions.GetValue (s))")]
		AVVideoCodecType[] GetSupportedPhotoCodecTypesForFileType (string fileType);

		[iOS (11,0), NoMac]
		[Export ("supportedRawPhotoPixelFormatTypesForFileType:")]
		NSNumber[] GetSupportedRawPhotoPixelFormatTypesForFileType (string fileType);

		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'VirtualDeviceConstituentPhotoDeliverySupported' instead.")]
		[iOS (11, 0), NoMac]
		[Export ("dualCameraDualPhotoDeliverySupported")]
		bool DualCameraDualPhotoDeliverySupported { [Bind ("isDualCameraDualPhotoDeliverySupported")] get; }

		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'VirtualDeviceConstituentPhotoDeliveryEnabledDevices' instead.")]
		[iOS (11, 0), NoMac]
		[Export ("dualCameraDualPhotoDeliveryEnabled")]
		bool DualCameraDualPhotoDeliveryEnabled { [Bind ("isDualCameraDualPhotoDeliveryEnabled")] get; set; }

		[iOS (11, 0), NoMac]
		[Export ("availableLivePhotoVideoCodecTypes")]
		string[] AvailableLivePhotoVideoCodecTypes { [return: BindAs (typeof (AVVideoCodecType []))] get; }

		[iOS (11, 0), NoMac]
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

		[BindAs (typeof (AVSemanticSegmentationMatteType []))]
		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("availableSemanticSegmentationMatteTypes")]
		NSString[] AvailableSemanticSegmentationMatteTypes { get; }

		[BindAs (typeof (AVSemanticSegmentationMatteType []))]
		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("enabledSemanticSegmentationMatteTypes", ArgumentSemantic.Assign)]
		NSString[] EnabledSemanticSegmentationMatteTypes { get; set; }

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("maxPhotoQualityPrioritization", ArgumentSemantic.Assign)]
		AVCapturePhotoQualityPrioritization MaxPhotoQualityPrioritization { get; set; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Export ("virtualDeviceFusionSupported")]
		bool VirtualDeviceFusionSupported { [Bind ("isVirtualDeviceFusionSupported")] get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Export ("virtualDeviceConstituentPhotoDeliverySupported")]
		bool VirtualDeviceConstituentPhotoDeliverySupported { [Bind ("isVirtualDeviceConstituentPhotoDeliverySupported")] get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Export ("virtualDeviceConstituentPhotoDeliveryEnabled")]
		bool VirtualDeviceConstituentPhotoDeliveryEnabled { [Bind ("isVirtualDeviceConstituentPhotoDeliveryEnabled")] get; set; }
	}
	
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

		[iOS (10,0)]
		[Export ("outputSettingsForConnection:")]
		NSDictionary GetOutputSettings (AVCaptureConnection connection);

		[iOS (10,0)]
		[Export ("setOutputSettings:forConnection:")]
		void SetOutputSettings ([NullAllowed] NSDictionary outputSettings, AVCaptureConnection connection);

		[NoWatch, NoTV, NoMac, iOS (12,0)]
		[Export ("supportedOutputSettingsKeysForConnection:")]
		string[] GetSupportedOutputSettingsKeys (AVCaptureConnection connection);
	}

	[NoTV]
	[NoWatch]
	[Deprecated (PlatformName.iOS, 10,0, message: "Use 'AVCapturePhotoOutput' instead.")]
	[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'AVCapturePhotoOutput' instead.")]
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
		[return: NullAllowed]
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

	[NoTV, iOS (10,0), Mac (10,15), NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // init NS_UNAVAILABLE
	interface AVCaptureDeviceDiscoverySession {

		[Internal]
		[Static]
		[Export ("discoverySessionWithDeviceTypes:mediaType:position:")]
		AVCaptureDeviceDiscoverySession _Create (NSArray deviceTypes, [NullAllowed] string mediaType, AVCaptureDevicePosition position);

		[Export ("devices")]
		AVCaptureDevice [] Devices { get; }

		[NoMac, iOS (13,0)]
		[Export ("supportedMultiCamDeviceSets")]
		NSSet<AVCaptureDevice>[] SupportedMultiCamDeviceSets { get; }
	}

	[NoTV, iOS (10,0), Mac (10,15), NoWatch]
	enum AVCaptureDeviceType {

		[Field ("AVCaptureDeviceTypeBuiltInMicrophone")]
		BuiltInMicrophone,

		[Field ("AVCaptureDeviceTypeBuiltInWideAngleCamera")]
		BuiltInWideAngleCamera,

		[NoMac]
		[Field ("AVCaptureDeviceTypeBuiltInTelephotoCamera")]
		BuiltInTelephotoCamera,

		[iOS (10, 0), NoMac]
		[Deprecated (PlatformName.iOS, 10, 2, message: "Use 'BuiltInDualCamera' instead.")]
		[Field ("AVCaptureDeviceTypeBuiltInDuoCamera")]
		BuiltInDuoCamera,

		[iOS (10, 2), NoMac]
		[Field ("AVCaptureDeviceTypeBuiltInDualCamera")]
		BuiltInDualCamera,

		[iOS (11, 1), NoMac]
		[Field ("AVCaptureDeviceTypeBuiltInTrueDepthCamera")]
		BuiltInTrueDepthCamera,

		[NoMac, iOS (13, 0)]
		[Field ("AVCaptureDeviceTypeBuiltInUltraWideCamera")]
		BuiltInUltraWideCamera,

		[NoMac, iOS (13, 0)]
		[Field ("AVCaptureDeviceTypeBuiltInTripleCamera")]
		BuiltInTripleCamera,

		[NoMac, iOS (13, 0)]
		[Field ("AVCaptureDeviceTypeBuiltInDualWideCamera")]
		BuiltInDualWideCamera,

		[NoWatch, NoTV, NoiOS]
		[Field ("AVCaptureDeviceTypeExternalUnknown")]
		ExternalUnknown,
	}

	[NoTV, iOS (7,0), Mac (10,14), NoWatch] // matches API that uses it.
	enum AVAuthorizationMediaType {
		Video,
		Audio,
	}

#if WATCH
	[Static]
#endif
	[NoTV, Watch (6,0)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: Cannot instantiate a AVCaptureDevice directly.
	[DisableDefaultCtor]
	interface AVCaptureDevice {
		[NoWatch]
		[Export ("uniqueID")]
		string UniqueID { get;  }

		[NoWatch]
		[Export ("modelID")]
		string ModelID { get;  }

		[NoWatch]
		[Export ("localizedName")]
		string LocalizedName { get;  }

		[NoWatch]
		[Export ("connected")]
		bool Connected { [Bind ("isConnected")] get;  }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'AVCaptureDeviceDiscoverySession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'AVCaptureDeviceDiscoverySession' instead.")]
		[Static, Export ("devices")]
		AVCaptureDevice [] Devices { get;  }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'AVCaptureDeviceDiscoverySession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'AVCaptureDeviceDiscoverySession' instead.")]
		[Static]
		[Export ("devicesWithMediaType:")]
		AVCaptureDevice [] DevicesWithMediaType (string mediaType);

		[NoWatch]
		[Static]
		[Export ("defaultDeviceWithMediaType:")]
		[return: NullAllowed]
		AVCaptureDevice GetDefaultDevice (NSString mediaType);

		[NoWatch]
		[Static]
		[Wrap ("GetDefaultDevice (mediaType.GetConstant ()!)")]
		[return: NullAllowed]
		AVCaptureDevice GetDefaultDevice (AVMediaTypes mediaType);

#if !XAMCORE_4_0
		[NoWatch]
		[Obsolete ("Use 'GetDefaultDevice (AVMediaTypes)'.")]
		[Static]
		[Wrap ("GetDefaultDevice ((NSString) mediaType)")]
		[return: NullAllowed]
		AVCaptureDevice DefaultDeviceWithMediaType (string mediaType);
#endif

		[NoWatch]
		[Static]
		[Export ("deviceWithUniqueID:")]
		[return: NullAllowed]
		AVCaptureDevice DeviceWithUniqueID (string deviceUniqueID);

		[NoWatch]
		[Export ("hasMediaType:")]
		bool HasMediaType (string mediaType);

		[NoWatch]
		[Wrap ("HasMediaType ((string) mediaType.GetConstant ())")]
		bool HasMediaType (AVMediaTypes mediaType);

		[NoWatch]
		[Export ("lockForConfiguration:")]
		bool LockForConfiguration (out NSError error);

		[NoWatch]
		[Export ("unlockForConfiguration")]
		void UnlockForConfiguration ();

		[NoWatch]
		[Export ("supportsAVCaptureSessionPreset:")]
		bool SupportsAVCaptureSessionPreset (string preset);

		[NoWatch]
		[Availability (Deprecated = Platform.iOS_10_0, Message="Use 'AVCapturePhotoSettings.FlashMode' instead.")]
		[Export ("flashMode")]
		AVCaptureFlashMode FlashMode { get; set;  }

		[NoWatch]
		[Availability (Deprecated = Platform.iOS_10_0, Message="Use 'AVCapturePhotoOutput.SupportedFlashModes' instead.")]
		[Export ("isFlashModeSupported:")]
		bool IsFlashModeSupported (AVCaptureFlashMode flashMode);

		[NoWatch]
		[Export ("torchMode", ArgumentSemantic.Assign)] 
		AVCaptureTorchMode TorchMode { get; set;  }

		[NoWatch]
		[Export ("isTorchModeSupported:")]
		bool IsTorchModeSupported (AVCaptureTorchMode torchMode);

		[NoWatch]
		[Export ("isFocusModeSupported:")]
		bool IsFocusModeSupported (AVCaptureFocusMode focusMode);

		[NoWatch]
		[Export ("focusMode", ArgumentSemantic.Assign)]
		AVCaptureFocusMode FocusMode { get; set;  }

		[NoWatch]
		[Export ("focusPointOfInterestSupported")]
		bool FocusPointOfInterestSupported { [Bind ("isFocusPointOfInterestSupported")] get;  }

		[NoWatch]
		[Export ("focusPointOfInterest", ArgumentSemantic.Assign)]
		CGPoint FocusPointOfInterest { get; set;  }

		[NoWatch]
		[Export ("adjustingFocus")]
		bool AdjustingFocus { [Bind ("isAdjustingFocus")] get;  }

		[NoWatch]
		[Export ("exposureMode", ArgumentSemantic.Assign)]
		AVCaptureExposureMode ExposureMode { get; set;  }

		[NoWatch]
		[Export ("isExposureModeSupported:")]
		bool IsExposureModeSupported (AVCaptureExposureMode exposureMode);

		[NoWatch]
		[Export ("exposurePointOfInterestSupported")]
		bool ExposurePointOfInterestSupported { [Bind ("isExposurePointOfInterestSupported")] get;  }

		[NoWatch]
		[Export ("exposurePointOfInterest")]
		CGPoint ExposurePointOfInterest { get; set;  }

		[NoWatch]
		[Export ("adjustingExposure")]
		bool AdjustingExposure { [Bind ("isAdjustingExposure")] get;  }

		[NoWatch]
		[Export ("isWhiteBalanceModeSupported:")]
		bool IsWhiteBalanceModeSupported (AVCaptureWhiteBalanceMode whiteBalanceMode);
		
		[NoWatch]
		[Export ("whiteBalanceMode", ArgumentSemantic.Assign)]
		AVCaptureWhiteBalanceMode WhiteBalanceMode { get; set;  }

		[NoWatch]
		[Export ("adjustingWhiteBalance")]
		bool AdjustingWhiteBalance { [Bind ("isAdjustingWhiteBalance")] get;  }

		[NoWatch]
		[Export ("position")]
		AVCaptureDevicePosition Position { get; }

		[NoWatch]
		[Field ("AVCaptureDeviceWasConnectedNotification")]
		[Notification]
		NSString WasConnectedNotification { get; }

		[NoWatch]
		[Field ("AVCaptureDeviceWasDisconnectedNotification")]
		[Notification]
		NSString WasDisconnectedNotification { get; }

		[Mac (10,15), NoWatch]
		[Field ("AVCaptureMaxAvailableTorchLevel")]
		float MaxAvailableTorchLevel { get; } // defined as 'float'

		[NoWatch, NoMac]
		[Field ("AVCaptureDeviceSubjectAreaDidChangeNotification")]
		[Notification]
		NSString SubjectAreaDidChangeNotification { get; }

		[NoWatch, NoMac]
		[Export ("subjectAreaChangeMonitoringEnabled")]
		bool SubjectAreaChangeMonitoringEnabled { [Bind ("isSubjectAreaChangeMonitoringEnabled")] get; set; }
		
		[NoWatch, Mac (10,15)]
		[Export ("isFlashAvailable")]
		bool FlashAvailable { get;  }

		[NoWatch, NoMac]
		[Availability (Deprecated = Platform.iOS_10_0, Message="Use 'AVCapturePhotoOutput.IsFlashScene' instead.")]
		[Export ("isFlashActive")]
		bool FlashActive { get; }

		[NoWatch, Mac (10,15)]
		[Export ("isTorchAvailable")]
		bool TorchAvailable { get; }

		[NoWatch, Mac (10,15)]
		[Export ("torchLevel")]
		float TorchLevel { get; } // defined as 'float'

		// 6.0
		[NoWatch, Mac (10,15)]
		[Export ("torchActive")]
		bool TorchActive { [Bind ("isTorchActive")] get;  }

		[NoWatch, Mac (10,15)]
		[Export ("setTorchModeOnWithLevel:error:")]
		bool SetTorchModeLevel (float /* defined as 'float' */ torchLevel, out NSError outError);

		[NoWatch, NoMac]
		[Export ("lowLightBoostSupported")]
		bool LowLightBoostSupported { [Bind ("isLowLightBoostSupported")] get; }

		[NoWatch, NoMac]
		[Export ("lowLightBoostEnabled")]
		bool LowLightBoostEnabled { [Bind ("isLowLightBoostEnabled")] get; }

		[NoWatch, NoMac]
		[Export ("automaticallyEnablesLowLightBoostWhenAvailable")]
		bool AutomaticallyEnablesLowLightBoostWhenAvailable { get; set; }

		[iOS (7,0), NoWatch, NoMac]
		[Export ("videoZoomFactor")]
		nfloat VideoZoomFactor { get; set; }
	
		[iOS (7,0), NoWatch, NoMac]
		[Export ("rampToVideoZoomFactor:withRate:")]
		void RampToVideoZoom (nfloat factor, float /* float, not CGFloat */ rate);
	
		[iOS (7,0), NoWatch, NoMac]
		[Export ("rampingVideoZoom")]
		bool RampingVideoZoom { [Bind ("isRampingVideoZoom")] get; }
	
		[iOS (7,0), NoWatch, NoMac]
		[Export ("cancelVideoZoomRamp")]
		void CancelVideoZoomRamp ();
			
		[iOS (7,0), NoWatch, NoMac]
		[Export ("autoFocusRangeRestrictionSupported")]
		bool AutoFocusRangeRestrictionSupported { [Bind ("isAutoFocusRangeRestrictionSupported")] get; }
	
		[iOS (7,0), NoWatch, NoMac]
		[Export ("autoFocusRangeRestriction")]
		AVCaptureAutoFocusRangeRestriction AutoFocusRangeRestriction { get; set; }
	
		[iOS (7,0), NoWatch, NoMac]
		[Export ("smoothAutoFocusSupported")]
		bool SmoothAutoFocusSupported { [Bind ("isSmoothAutoFocusSupported")] get; }
	
		[iOS (7,0), NoWatch, NoMac]
		[Export ("smoothAutoFocusEnabled")]
		bool SmoothAutoFocusEnabled { [Bind ("isSmoothAutoFocusEnabled")] get; set; }

		// Either AVMediaTypeVideo or AVMediaTypeAudio.
		[iOS (7,0), NoWatch]
		[Static]
		[Wrap ("RequestAccessForMediaType (mediaType == AVAuthorizationMediaType.Video ? AVMediaTypes.Video.GetConstant ()! : AVMediaTypes.Audio.GetConstant ()!, completion)")]
		[Async]
		void RequestAccessForMediaType (AVAuthorizationMediaType mediaType, AVRequestAccessStatus completion);

		[NoWatch]
		[iOS (7,0)]
		[Mac (10,14)]
		[Static, Export ("requestAccessForMediaType:completionHandler:")]
		[Async]
		void RequestAccessForMediaType (NSString avMediaTypeToken, AVRequestAccessStatus completion);

		// Calling this method with any media type other than AVMediaTypeVideo or AVMediaTypeAudio raises an exception.
		[NoWatch]
		[iOS (7,0)]
		[Mac (10,14)]
		[Static]
		[Wrap ("GetAuthorizationStatus (mediaType == AVAuthorizationMediaType.Video ? AVMediaTypes.Video.GetConstant ()! : AVMediaTypes.Audio.GetConstant ()!)")]
		AVAuthorizationStatus GetAuthorizationStatus (AVAuthorizationMediaType mediaType);

		[NoWatch]
		[iOS (7,0)]
		[Mac (10,14)]
		[Static, Export ("authorizationStatusForMediaType:")]
		AVAuthorizationStatus GetAuthorizationStatus (NSString avMediaTypeToken);

		[iOS (7,0), NoWatch]
		[Export ("activeFormat", ArgumentSemantic.Retain)]
		AVCaptureDeviceFormat ActiveFormat { get; set; }

		[iOS (7,0), NoWatch]
		[Export ("formats")]
		AVCaptureDeviceFormat [] Formats { get; }

		[NoWatch]
		[Export ("hasFlash")]
		bool HasFlash { get; }

		[NoWatch]
		[Export ("hasTorch")]
		bool HasTorch { get; }

		[NoiOS, NoWatch]
		[Export ("inUseByAnotherApplication")]
		bool InUseByAnotherApplication { [Bind ("isInUseByAnotherApplication")] get; }

		[NoiOS, NoWatch]
		[Export ("suspended")]
		bool Suspended { [Bind ("isSuspended")] get; }

		[NoiOS, NoWatch]
		[Export ("linkedDevices")]
		AVCaptureDevice [] LinkedDevices { get; }

		[Mac (10,9), NoiOS, NoWatch]
		[Export ("manufacturer")]
		string Manufacturer { get; }

		[NoiOS, NoWatch]
		[Export ("transportControlsSpeed")]
		float TransportControlsSpeed { get; } // float intended

		[NoiOS, NoWatch]
		[Export ("transportControlsSupported")]
		bool TransportControlsSupported { get; }

		[NoWatch]
		[NoiOS] // TODO: We can provide a better binding once IOKit is bound kIOAudioDeviceTransportType*
		[Export ("transportType")]
		int WeakTransportType { get; } // int intended

#if MONOMAC // Can't use [NoiOS] since types are also inside a block
		[NullAllowed, Export ("activeInputSource", ArgumentSemantic.Retain)]
		AVCaptureDeviceInputSource ActiveInputSource { get; set; }

		[Export ("inputSources")]
		AVCaptureDeviceInputSource [] InputSources { get; }

		[Export ("setTransportControlsPlaybackMode:speed:")]
		void SetTransportControlsPlaybackMode (AVCaptureDeviceTransportControlsPlaybackMode mode, float speed); // Float intended

		[Export ("transportControlsPlaybackMode")]
		AVCaptureDeviceTransportControlsPlaybackMode TransportControlsPlaybackMode { get; }
#endif

		[NoWatch]
		[iOS (7,0)]
		[Mac (10,9)]
		[Export ("activeVideoMinFrameDuration", ArgumentSemantic.Copy)]
		CMTime ActiveVideoMinFrameDuration { get; set; }

		[NoWatch]
		[iOS (7,0)]
		[Mac (10,9)]
		[Export ("activeVideoMaxFrameDuration", ArgumentSemantic.Copy)]
		CMTime ActiveVideoMaxFrameDuration { get; set; }

		[iOS (10,0), NoMac, NoWatch]
		[Export ("lockingFocusWithCustomLensPositionSupported")]
		bool LockingFocusWithCustomLensPositionSupported { [Bind ("isLockingFocusWithCustomLensPositionSupported")] get; }

		[iOS (10,0), NoMac, NoWatch]
		[Export ("lockingWhiteBalanceWithCustomDeviceGainsSupported")]
		bool LockingWhiteBalanceWithCustomDeviceGainsSupported { [Bind ("isLockingWhiteBalanceWithCustomDeviceGainsSupported")] get; }

		// From AVCaptureDevice (AVCaptureDeviceType) Category
		[Internal]
		[iOS (10,0), Mac (10,15), NoWatch]
		[Export ("deviceType")]
		NSString _DeviceType { get; }

		[iOS (10, 0), Mac (10,15), NoWatch]
		[Wrap ("AVCaptureDeviceTypeExtensions.GetValue (_DeviceType)")]
		AVCaptureDeviceType DeviceType { get; }

		[Internal]
		[iOS (10,0), Mac (10,15), NoWatch]
		[Static]
		[Export ("defaultDeviceWithDeviceType:mediaType:position:")]
		AVCaptureDevice _DefaultDeviceWithDeviceType (NSString deviceType, string mediaType, AVCaptureDevicePosition position);

		[iOS (10,0), Mac (10,15), NoWatch]
		[Static]
		[Wrap ("AVCaptureDevice._DefaultDeviceWithDeviceType (deviceType.GetConstant ()!, mediaType, position)")]
		AVCaptureDevice GetDefaultDevice (AVCaptureDeviceType deviceType, string mediaType, AVCaptureDevicePosition position);

		//
		// iOS 8
		//
		[iOS (8,0), NoWatch, NoMac]
		[Field ("AVCaptureLensPositionCurrent")]
		float FocusModeLensPositionCurrent { get; } /* float, not CGFloat */
		
		[iOS (8,0), NoWatch, NoMac]
		[Export ("lensAperture")]
		float LensAperture { get; } /* float, not CGFloat */

		[iOS (8,0), NoWatch, NoMac]
		[Export ("exposureDuration")]
		CMTime ExposureDuration { get; }

		[iOS (8,0), NoWatch, NoMac]
		[Export ("ISO")]
		float ISO { get; } /* float, not CGFloat */

		[iOS (8,0), NoWatch, NoMac]
		[Export ("exposureTargetOffset")]
		float ExposureTargetOffset { get; } /* float, not CGFloat */

		[iOS (8,0), NoWatch, NoMac]
		[Export ("exposureTargetBias")]
		float ExposureTargetBias { get; } /* float, not CGFloat */

		[iOS (8,0), NoWatch, NoMac]
		[Export ("minExposureTargetBias")]
		float MinExposureTargetBias { get; } /* float, not CGFloat */

		[iOS (8,0), NoWatch, NoMac]
		[Export ("maxExposureTargetBias")]
		float MaxExposureTargetBias { get; } /* float, not CGFloat */

		[iOS (8,0), NoWatch, NoMac]
		[Export ("setExposureModeCustomWithDuration:ISO:completionHandler:")]
		[Async]
		void LockExposure (CMTime duration, float /* float, not CGFloat */ ISO, [NullAllowed] Action<CMTime> completionHandler);

		[iOS (8,0), NoWatch, NoMac]
		[Export ("setExposureTargetBias:completionHandler:")]
		[Async]
		void SetExposureTargetBias (float /* float, not CGFloat */ bias, [NullAllowed] Action<CMTime> completionHandler);

		[iOS (8,0), NoWatch, NoMac]
		[Export ("lensPosition")]
		float LensPosition { get; } /* float, not CGFloat */

		[iOS (8,0), NoWatch, NoMac]
		[Export ("setFocusModeLockedWithLensPosition:completionHandler:")]
		[Async]
		void SetFocusModeLocked (float /* float, not CGFloat */ lensPosition, [NullAllowed] Action<CMTime> completionHandler);		

		[iOS (8,0), NoWatch, NoMac]
		[Export ("deviceWhiteBalanceGains")]
		AVCaptureWhiteBalanceGains DeviceWhiteBalanceGains { get; }

		[iOS (8,0), NoWatch, NoMac]
		[Export ("grayWorldDeviceWhiteBalanceGains")]
		AVCaptureWhiteBalanceGains GrayWorldDeviceWhiteBalanceGains { get; }

		[iOS (8,0), NoWatch, NoMac]
		[Export ("maxWhiteBalanceGain")]
		float MaxWhiteBalanceGain { get; } /* float, not CGFloat */

		[iOS (8,0), NoWatch, NoMac]
		[Export ("setWhiteBalanceModeLockedWithDeviceWhiteBalanceGains:completionHandler:")]
		[Async]
		void SetWhiteBalanceModeLockedWithDeviceWhiteBalanceGains (AVCaptureWhiteBalanceGains whiteBalanceGains, [NullAllowed] Action<CMTime> completionHandler);

		[iOS (8,0), NoMac, NoWatch]
		[Export ("chromaticityValuesForDeviceWhiteBalanceGains:")]
		AVCaptureWhiteBalanceChromaticityValues GetChromaticityValues (AVCaptureWhiteBalanceGains whiteBalanceGains);

		[iOS (8,0), NoMac, NoWatch]
		[Export ("deviceWhiteBalanceGainsForChromaticityValues:")]
		AVCaptureWhiteBalanceGains GetDeviceWhiteBalanceGains (AVCaptureWhiteBalanceChromaticityValues chromaticityValues);

		[iOS (8,0), NoWatch, NoMac]
		[Export ("temperatureAndTintValuesForDeviceWhiteBalanceGains:")]
		AVCaptureWhiteBalanceTemperatureAndTintValues GetTemperatureAndTintValues (AVCaptureWhiteBalanceGains whiteBalanceGains);

		[iOS (8,0), NoWatch, NoMac]
		[Export ("deviceWhiteBalanceGainsForTemperatureAndTintValues:")]
		AVCaptureWhiteBalanceGains GetDeviceWhiteBalanceGains (AVCaptureWhiteBalanceTemperatureAndTintValues tempAndTintValues);

		[iOS (8,0), NoWatch, NoMac]
		[Field ("AVCaptureExposureDurationCurrent")]
		CMTime ExposureDurationCurrent { get; }

		[iOS (8,0), NoWatch, NoMac]
		[Field ("AVCaptureExposureTargetBiasCurrent")]
		float ExposureTargetBiasCurrent { get; } /* float, not CGFloat */

		[iOS (8,0), NoWatch, NoMac]
		[Field ("AVCaptureISOCurrent")]
		float ISOCurrent { get; } /* float, not CGFloat */

		[iOS (8,0), Watch (6,0), NoMac]
		[Field ("AVCaptureLensPositionCurrent")]
		float LensPositionCurrent { get; } /* float, not CGFloat */

		[iOS (8,0), NoWatch, NoMac]
		[Field ("AVCaptureWhiteBalanceGainsCurrent")]
		AVCaptureWhiteBalanceGains WhiteBalanceGainsCurrent { get; }

		[iOS (8,0), NoWatch, NoMac]
		[Export ("automaticallyAdjustsVideoHDREnabled")]
		bool AutomaticallyAdjustsVideoHdrEnabled { get; set; }

		[iOS (8,0), NoWatch, NoMac]
		[Export ("videoHDREnabled")]
		bool VideoHdrEnabled { [Bind ("isVideoHDREnabled")] get; set; }

		[iOS (10, 0), NoWatch, Mac (10,15)]
		[Export ("activeColorSpace", ArgumentSemantic.Assign)]
		AVCaptureColorSpace ActiveColorSpace { get; set; }

		// From AVCaptureDevice (AVCaptureDeviceDepthSupport) Category

		[iOS (11,0), NoWatch, NoMac]
		[Export ("activeDepthDataFormat", ArgumentSemantic.Retain), NullAllowed]
		AVCaptureDeviceFormat ActiveDepthDataFormat { get; set; }

		[iOS (11,0), NoWatch, NoMac]
		[Export ("minAvailableVideoZoomFactor")]
		nfloat MinAvailableVideoZoomFactor { get; }

		[iOS (11,0), NoWatch, NoMac]
		[Export ("maxAvailableVideoZoomFactor")]
		nfloat MaxAvailableVideoZoomFactor { get; }

		// From  AVCaptureDevice (AVCaptureDeviceSystemPressure) Category
		[NoWatch, NoTV, NoMac, iOS (11, 1)]
		[Export ("systemPressureState")]
		AVCaptureSystemPressureState SystemPressureState { get; }

		[Deprecated (PlatformName.iOS, 13,0, message: "Use 'VirtualDeviceSwitchOverVideoZoomFactors' instead.")]
		[iOS (11, 0), NoWatch, NoMac]
		[Export ("dualCameraSwitchOverVideoZoomFactor")]
		nfloat DualCameraSwitchOverVideoZoomFactor { get; }

		// From @interface AVCaptureDeviceDepthSupport (AVCaptureDevice)

		[NoMac, iOS (12, 0), NoWatch]
		[Export ("activeDepthDataMinFrameDuration", ArgumentSemantic.Assign)]
		CMTime ActiveDepthDataMinFrameDuration { get; set; }

		// From @interface AVCaptureDeviceExposure (AVCaptureDevice)
		[NoWatch, NoTV, NoMac, iOS (12, 0)]
		[Export ("activeMaxExposureDuration", ArgumentSemantic.Assign)]
		CMTime ActiveMaxExposureDuration { get; set; }

		[Mac (10,15), iOS (10,0), NoTV, NoWatch]
		[Static]
		[Export ("defaultDeviceWithDeviceType:mediaType:position:")]
		[return: NullAllowed]
		AVCaptureDevice GetDefaultDevice ([BindAs (typeof (AVCaptureDeviceType))]NSString deviceType, [NullAllowed] [BindAs (typeof (AVMediaTypes))] NSString mediaType, AVCaptureDevicePosition position);

		// From AVCaptureDevice_AVCaptureDeviceVirtual
		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Export ("virtualDevice")]
		bool VirtualDevice { [Bind ("isVirtualDevice")] get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Export ("constituentDevices")]
		AVCaptureDevice[] ConstituentDevices { get; }

		// from AVCaptureDevice_AVCaptureDeviceCalibration
		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Static]
		[Export ("extrinsicMatrixFromDevice:toDevice:")]
		[return: NullAllowed]
		NSData GetExtrinsicMatrix (AVCaptureDevice fromDevice, AVCaptureDevice toDevice);

		[Unavailable (PlatformName.MacCatalyst)]
		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("globalToneMappingEnabled")]
		bool GlobalToneMappingEnabled { [Bind ("isGlobalToneMappingEnabled")] get; set; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Export ("virtualDeviceSwitchOverVideoZoomFactors")]
		NSNumber[] VirtualDeviceSwitchOverVideoZoomFactors { get; }

		// from AVCaptureDevice_AVCaptureDeviceGeometricDistortionCorrection

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Export ("geometricDistortionCorrectionSupported")]
		bool GeometricDistortionCorrectionSupported { [Bind ("isGeometricDistortionCorrectionSupported")] get; }

		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Export ("geometricDistortionCorrectionEnabled")]
		bool GeometricDistortionCorrectionEnabled { [Bind ("isGeometricDistortionCorrectionEnabled")] get; set; }
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
	[iOS (7,0)]
	[DisableDefaultCtor] // crash -> immutable, it can be set but it should be selected from tha available formats (not a custom one)
	[BaseType (typeof (NSObject))]
	interface AVCaptureDeviceFormat {
		[Export ("mediaType", ArgumentSemantic.Copy)]
		NSString MediaType { get; }
	
		[Export ("formatDescription", ArgumentSemantic.Copy)]
		CMFormatDescription FormatDescription { get; }
	
		[Export ("videoSupportedFrameRateRanges", ArgumentSemantic.Copy)]
		AVFrameRateRange [] VideoSupportedFrameRateRanges { get; }

		[iOS (10,0), Mac (10,15)]
		[Export ("supportedColorSpaces")]
#if XAMCORE_4_0
		[BindAs (typeof (CGColorSpace []))]
#endif
		NSNumber[] SupportedColorSpaces { get; }

		[iOS (8,0), Mac (10,15)]
		[Export ("autoFocusSystem")]
		AVCaptureAutoFocusSystem AutoFocusSystem { get; }

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

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("highestPhotoQualitySupported")]
		bool HighestPhotoQualitySupported { [Bind ("isHighestPhotoQualitySupported")] get; }

		// from AVCaptureDeviceFormat_AVCaptureDeviceFormatMultiCamAdditions 
		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("multiCamSupported")]
		bool MultiCamSupported { [Bind ("isMultiCamSupported")] get; }

		[Unavailable (PlatformName.MacCatalyst)]
		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("globalToneMappingSupported")]
		bool GlobalToneMappingSupported { [Bind ("isGlobalToneMappingSupported")] get; }

		// from AVCaptureDeviceFormat_AVCaptureDeviceFormatGeometricDistortionCorrection 
		[NoWatch, NoTV, NoMac, iOS (13, 0)]
		[Export ("geometricDistortionCorrectedVideoFieldOfView")]
		float GeometricDistortionCorrectedVideoFieldOfView { get; }
#endif
	}

	delegate void AVCaptureCompletionHandler (CMSampleBuffer imageDataSampleBuffer, NSError error);

	[Watch (6,0)]
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

		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Deprecated (PlatformName.iOS, 11, 0)]
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

		[Mac (10,9)]
		[Export ("seekToDate:")]
		void Seek (NSDate date);

		[Mac (10,9)] // Header says 10.7, but it's a lie
		[Export ("seekToDate:completionHandler:")]
		[Async]
		void Seek (NSDate date, AVCompletion onComplete);

		[iOS (10, 0), TV (10,0), Mac (10,12)]
		[Export ("automaticallyWaitsToMinimizeStalling")]
		bool AutomaticallyWaitsToMinimizeStalling { get; set; }
		
		[Export ("setRate:time:atHostTime:")]
		void SetRate (float /* defined as 'float' */ rate, CMTime itemTime, CMTime hostClockTime);

		[Export ("prerollAtRate:completionHandler:")]
		[Async]
		void Preroll (float /* defined as 'float' */ rate, AVCompletion onComplete);

		[Export ("cancelPendingPrerolls")]
		void CancelPendingPrerolls ();

		[Mac (10,12)]
		[Export ("outputObscuredDueToInsufficientExternalProtection")]
		bool OutputObscuredDueToInsufficientExternalProtection { get; }

		[Export ("masterClock"), NullAllowed]
		CMClock MasterClock { get; set; }

		[Mac (10,11)]
		[Export ("allowsExternalPlayback")]
		bool AllowsExternalPlayback { get; set;  }

		[Mac (10,11)]
		[Export ("externalPlaybackActive")]
		bool ExternalPlaybackActive { [Bind ("isExternalPlaybackActive")] get; }

#if !MONOMAC
		[Export ("usesExternalPlaybackWhileExternalScreenIsActive")]
		bool UsesExternalPlaybackWhileExternalScreenIsActive { get; set;  }
#endif

		[Mac (10, 9)]
		[Protected]
		[Export ("externalPlaybackVideoGravity", ArgumentSemantic.Copy)]
		NSString WeakExternalPlaybackVideoGravity { get; set; }

		[iOS (7,0)]
		[Export ("volume")]
		float Volume { get; set; } // defined as 'float'

		[iOS (7,0)]
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

		[TV (12, 0), NoWatch, Mac (10, 14), iOS (12, 0)]
		[Export ("preventsDisplaySleepDuringVideoPlayback")]
		bool PreventsDisplaySleepDuringVideoPlayback { get; set; }

		[TV (13,4), NoWatch, Mac (10,15), iOS (13,4)]
		[Static]
		[Export ("eligibleForHDRPlayback")]
		bool EligibleForHdrPlayback { get; }

		[Notification]
		[TV (13,4), NoWatch, Mac (10,15), iOS (13,4)]
		[Field ("AVPlayerEligibleForHDRPlaybackDidChangeNotification")]
		NSString EligibleForHdrPlaybackDidChangeNotification { get; }
	}

	[iOS (7,0), Mac (10, 9), Watch (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerMediaSelectionCriteria {
		[Export ("preferredLanguages"), NullAllowed]
		string [] PreferredLanguages { get; }

		[Export ("preferredMediaCharacteristics"), NullAllowed]
		NSString [] PreferredMediaCharacteristics { get; }

		[Export ("initWithPreferredLanguages:preferredMediaCharacteristics:")]
		IntPtr Constructor ([NullAllowed] string [] preferredLanguages, [NullAllowed] NSString [] preferredMediaCharacteristics);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("initWithPrincipalMediaCharacteristics:preferredLanguages:preferredMediaCharacteristics:")]
		IntPtr Constructor ([NullAllowed] [BindAs (typeof (AVMediaCharacteristics []))]NSString[] principalMediaCharacteristics, [NullAllowed] [BindAs (typeof (AVMediaCharacteristics []))] NSString[] preferredLanguages, [NullAllowed] string[] preferredMediaCharacteristics);

		[BindAs (typeof (AVMediaCharacteristics []))]
		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[NullAllowed, Export ("principalMediaCharacteristics")]
		NSString[] PrincipalMediaCharacteristics { get; }
	}

	[NoWatch]
	[Mac (10, 9)]
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
		[Wrap ("FromTextMarkupAttributes (textMarkupAttributes.GetDictionary ()!)")]
		AVTextStyleRule FromTextMarkupAttributes (CMTextMarkupAttributes textMarkupAttributes);

		[return: NullAllowed]
		[Static][Internal]
		[Export ("textStyleRuleWithTextMarkupAttributes:textSelector:")]
		AVTextStyleRule FromTextMarkupAttributes (NSDictionary textMarkupAttributes, [NullAllowed] string textSelector);

		[return: NullAllowed]
		[Static]
		[Wrap ("FromTextMarkupAttributes (textMarkupAttributes.GetDictionary ()!, textSelector)")]
		AVTextStyleRule FromTextMarkupAttributes (CMTextMarkupAttributes textMarkupAttributes, [NullAllowed] string textSelector);

		[Export ("initWithTextMarkupAttributes:")]
		[Protected]
		IntPtr Constructor (NSDictionary textMarkupAttributes);

		[Wrap ("this (attributes.GetDictionary ()!)")]
		IntPtr Constructor (CMTextMarkupAttributes attributes);

		[DesignatedInitializer]
		[Export ("initWithTextMarkupAttributes:textSelector:")]
		[Protected]
		IntPtr Constructor (NSDictionary textMarkupAttributes, [NullAllowed] string textSelector);
	
		[Wrap ("this (attributes.GetDictionary ()!, textSelector)")]
		IntPtr Constructor (CMTextMarkupAttributes attributes, string textSelector);
	}

	[iOS (9,0)][Mac (10,11)][Watch (6,0)]
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

	[Watch (6,0)]
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

	[Watch (6,0)]
	[BaseType (typeof (AVTimedMetadataGroup))]
	interface AVMutableTimedMetadataGroup {

		[Export ("items", ArgumentSemantic.Copy)]
		[Override]
		AVMetadataItem [] Items { get; set;  }

		[Export ("timeRange")]
		[Override]
		CMTimeRange TimeRange { get; set; }
	}

	interface AVPlayerItemErrorEventArgs {
		[Export ("AVPlayerItemFailedToPlayToEndTimeErrorKey")]
		NSError Error { get; }
	}
		
	[Watch (6,0)]
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

		[NoWatch]
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

		[Export ("seekableTimeRanges")]
		NSValue [] SeekableTimeRanges { get;  }

		[Export ("loadedTimeRanges")]
		NSValue [] LoadedTimeRanges { get;  }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 13,0, message: "Use the class 'AVPlayerItemMetadataOutput' instead to get the time metadata info.")]
		[Deprecated (PlatformName.TvOS, 13,0, message: "Use the class 'AVPlayerItemMetadataOutput' instead to get the time metadata info.")]
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use the class 'AVPlayerItemMetadataOutput' instead to get the time metadata info.")]
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

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'Seek (NSDate, AVCompletion)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Seek (NSDate, AVCompletion)' instead.")]
		[Export ("seekToDate:")]
		bool Seek (NSDate date);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'Seek (CMTime, AVCompletion)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Seek (CMTime, AVCompletion)' instead.")]
		[Export ("seekToTime:")]
		void Seek (CMTime time);
		
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'Seek (CMTime, CMTime, CMTime, AVCompletion)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'Seek (CMTime, CMTime, CMTime, AVCompletion)' instead.")]
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

		[Export ("selectMediaOption:inMediaSelectionGroup:")]
		void SelectMediaOption ([NullAllowed] AVMediaSelectionOption mediaSelectionOption, AVMediaSelectionGroup mediaSelectionGroup);

		[return: NullAllowed]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CurrentMediaSelection' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CurrentMediaSelection' instead.")]
		[Export ("selectedMediaOptionInMediaSelectionGroup:")]
		AVMediaSelectionOption SelectedMediaOption (AVMediaSelectionGroup inMediaSelectionGroup);

		[iOS (9,0), MacAttribute (10,11)]
		[Export ("currentMediaSelection")]
		AVMediaSelection CurrentMediaSelection { get; }

		[Export ("canPlaySlowForward")]
		bool CanPlaySlowForward { get;  }

		[Export ("canPlayReverse")]
		bool CanPlayReverse { get;  }

		[Export ("canPlaySlowReverse")]
		bool CanPlaySlowReverse { get;  }

		[Export ("canStepForward")]
		bool CanStepForward { get;  }

		[Export ("canStepBackward")]
		bool CanStepBackward { get;  }
		
		[Export ("outputs")]
		AVPlayerItemOutput [] Outputs { get;  }

		[Export ("addOutput:")]
		[PostGet ("Outputs")]
		void AddOutput (AVPlayerItemOutput output);

		[Export ("removeOutput:")]
		[PostGet ("Outputs")]
		void RemoveOutput (AVPlayerItemOutput output);

		[Export ("timebase"), NullAllowed]
		CMTimebase Timebase { get;  }

		[Mac (10, 9)]
		[Export ("seekToDate:completionHandler:")]
		[Async]
		bool Seek (NSDate date, AVCompletion completion);

		[Mac (10, 9)]
		[Export ("seekingWaitsForVideoCompositionRendering")]
		bool SeekingWaitsForVideoCompositionRendering { get; set;  }

		[Mac (10, 9), NoWatch]
		[Export ("textStyleRules", ArgumentSemantic.Copy), NullAllowed]
		AVTextStyleRule [] TextStyleRules { get; set;  }

		[Mac (10, 9)]
		[Field ("AVPlayerItemPlaybackStalledNotification")]
		[Notification]
		NSString PlaybackStalledNotification { get; }
		
		[Mac (10, 9)]
		[Field ("AVPlayerItemNewAccessLogEntryNotification")]
		[Notification]
		NSString NewAccessLogEntryNotification { get; }
		
		[Mac (10, 9)]
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

		[iOS (7,0), Mac (10, 9), NoWatch]
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
		[NoiOS][NoMac][NoWatch]
		[TV (9,0)]
		[Export ("navigationMarkerGroups", ArgumentSemantic.Copy)]
		AVNavigationMarkersGroup[] NavigationMarkerGroups { get; set; }

		[NoMac][NoWatch]
		[TV (9,0)]
		[iOS (13,0)]
		[Export ("externalMetadata", ArgumentSemantic.Copy)]
		AVMetadataItem[] ExternalMetadata { get; set; }

		[NoiOS][NoMac][NoWatch]
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

		[Notification]
		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Field ("AVPlayerItemRecommendedTimeOffsetFromLiveDidChangeNotification")]
		NSString RecommendedTimeOffsetFromLiveDidChangeNotification { get; }

		[Notification]
		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Field ("AVPlayerItemMediaSelectionDidChangeNotification")]
		NSString MediaSelectionDidChangeNotification { get; }

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("configuredTimeOffsetFromLive", ArgumentSemantic.Assign)]
		CMTime ConfiguredTimeOffsetFromLive { get; set; }

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("recommendedTimeOffsetFromLive")]
		CMTime RecommendedTimeOffsetFromLive { get; }

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("automaticallyPreservesTimeOffsetFromLive")]
		bool AutomaticallyPreservesTimeOffsetFromLive { get; set; }

		[NoWatch, NoTV, Mac (10, 15), iOS (13, 0)]
		[Export ("audioSpatializationAllowed")]
		bool AudioSpatializationAllowed { [Bind ("isAudioSpatializationAllowed")] get; set; }
	}

	[NoiOS][NoTV][NoWatch]
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

	[NoWatch, NoMac, NoiOS]
	[TV (13,0)]
	[Category]
	[BaseType (typeof (AVPlayerItem))]
	interface AVPlayerItem_AVPlaybackRestrictions {
		[Async]
		[Export ("requestPlaybackRestrictionsAuthorization:")]
		void RequestPlaybackRestrictionsAuthorization (Action<bool, NSError> completion);
		
		[Export ("cancelPlaybackRestrictionsAuthorizationRequest")]
		void CancelPlaybackRestrictionsAuthorizationRequest ();
	}
	
	[Watch (6,0)]
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

		[NoiOS][NoTV][NoWatch]
		[Export ("itemTimeForCVTimeStamp:")]
		CMTime GetItemTime (CVTimeStamp timestamp);
	}

	[Watch (6,0)]
	[iOS (9,3)]
	[TV (9,2)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // initialization method -init cannot be sent to an abstract object of class AVPlayerItemMediaDataCollector: Create a concrete instance!
	[Abstract]
	interface AVPlayerItemMediaDataCollector
	{
	}

	[iOS (8,0)][Mac (10,10)][Watch (6,0)]
	[BaseType (typeof (AVPlayerItemOutput))]
	interface AVPlayerItemMetadataOutput {

		[DesignatedInitializer]
		[Export ("initWithIdentifiers:")]
		IntPtr Constructor ([NullAllowed] NSString [] metadataIdentifiers);

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
	[iOS (8,0)][Mac (10,10)][Watch (6,0)]
	[Protocol, Model]
	interface AVPlayerItemMetadataOutputPushDelegate : AVPlayerItemOutputPushDelegate {

		[iOS (8,0)]
		[Export ("metadataOutput:didOutputTimedMetadataGroups:fromPlayerItemTrack:")]
		void DidOutputTimedMetadataGroups (AVPlayerItemMetadataOutput output, AVTimedMetadataGroup [] groups, [NullAllowed] AVPlayerItemTrack track);
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
	[BaseType (typeof (AVPlayerItemOutput))]
	interface AVPlayerItemVideoOutput {
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get;  }
		
		[Wrap ("WeakDelegate")]
		[NullAllowed]
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
		[Wrap ("this (attributes.GetDictionary (), AVPlayerItemVideoOutput.InitMode.PixelAttributes)")]
		IntPtr Constructor (CVPixelBufferAttributes attributes);
		
		[DesignatedInitializer]
		[iOS (10,0), TV (10,0), Mac (10,12)]
		[Wrap ("this (settings.GetDictionary (), AVPlayerItemVideoOutput.InitMode.OutputSettings)")]
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

	[Watch (6,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVPlayerItemOutputPullDelegate {
		[Export ("outputMediaDataWillChange:")]
		void OutputMediaDataWillChange (AVPlayerItemOutput sender);

		[Export ("outputSequenceWasFlushed:")]
		void OutputSequenceWasFlushed (AVPlayerItemOutput output);
	}

	[Watch (6,0)]
	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface AVPlayerItemOutputPushDelegate {
		[Export ("outputSequenceWasFlushed:")]
		void OutputSequenceWasFlushed (AVPlayerItemOutput output);
	}

	[iOS (7,0), Watch (6,0)]
	[BaseType (typeof (AVPlayerItemOutputPushDelegate))]
	[Model]
	[Protocol]
	interface AVPlayerItemLegibleOutputPushDelegate  {
		[NoWatch]
		[Export ("legibleOutput:didOutputAttributedStrings:nativeSampleBuffers:forItemTime:")]
		void DidOutputAttributedStrings (AVPlayerItemLegibleOutput output, NSAttributedString [] strings, CMSampleBuffer [] nativeSamples, CMTime itemTime);		
	}

	[iOS (7,0), Mac (10, 9), NoWatch]
	[BaseType (typeof (AVPlayerItemOutput))]
	interface AVPlayerItemLegibleOutput {
		[Export ("initWithMediaSubtypesForNativeRepresentation:")]
		IntPtr Constructor (NSNumber [] subtypesFourCCcodes);
		
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

	[Watch (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemAccessLog : NSCopying {
		
		[Export ("events")]
		AVPlayerItemAccessLogEvent [] Events { get; }

		[Export ("extendedLogDataStringEncoding")]
		NSStringEncoding ExtendedLogDataStringEncoding { get; }

		[Export ("extendedLogData"), NullAllowed]
		NSData ExtendedLogData { get; }
	}

	[Watch (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemErrorLog : NSCopying {
		[Export ("events")]
		AVPlayerItemErrorLogEvent [] Events { get; }

		[Export ("extendedLogDataStringEncoding")]
		NSStringEncoding ExtendedLogDataStringEncoding { get; }

		[Export ("extendedLogData"), NullAllowed]
		NSData ExtendedLogData { get; }
	}
	
	[Watch (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemAccessLogEvent : NSCopying {
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'NumberOfMediaRequests' instead.")]
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

		[iOS (8,0), TV (9,0), Watch (6,0), Mac (10,10)]
		[Export ("indicatedBitrate")]
		double IndicatedBitrate { get; }

		[iOS (10, 0), TV (10,0), Watch (6,0), Mac (10, 12)]
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

		[Mac (10, 9)]
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

	[Watch (6,0)]
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

	[Watch (6,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface AVPlayerItemMetadataCollectorPushDelegate
	{
		[Abstract]
		[Export ("metadataCollector:didCollectDateRangeMetadataGroups:indexesOfNewGroups:indexesOfModifiedGroups:")]
		void DidCollectDateRange (AVPlayerItemMetadataCollector metadataCollector, AVDateRangeMetadataGroup[] metadataGroups, NSIndexSet indexesOfNewGroups, NSIndexSet indexesOfModifiedGroups);
	}

	[Watch (6,0)]
	[iOS (9,3), Mac (10,12)]
	[TV (9,2)]
	[BaseType (typeof(AVPlayerItemMediaDataCollector))]
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

	[Watch (6,0)]
	[BaseType (typeof (NSObject))]
	interface AVPlayerItemTrack {
		[Export ("enabled", ArgumentSemantic.Assign)]
		bool Enabled { [Bind ("isEnabled")] get; set;  }

		[NullAllowed, Export ("assetTrack")]
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

	[Watch (6,0)]
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

	[Watch (6,0)]
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

		// AVSampleBufferDisplayLayerImageProtection

		[TV (12,2), NoWatch, Mac (10,14,4), iOS (12,2)]
		[Export ("preventsCapture")]
		bool PreventsCapture { get; set; }

		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Export ("preventsDisplaySleepDuringVideoPlayback")]
		bool PreventsDisplaySleepDuringVideoPlayback { get; set; }
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

	[Mac (10,15)]
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
		AVSpeechSynthesisVoice FromIdentifier (string identifier);

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

		[iOS (10, 0), TV (10,0), Watch (3,0), Mac (10,15)]
		[Field ("AVSpeechSynthesisIPANotationAttribute")]
		NSString IpaNotationAttribute { get; }

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("gender")]
		AVSpeechSynthesisVoiceGender Gender { get; }

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Export ("audioFileSettings")]
		NSDictionary<NSString, NSObject> AudioFileSettings { get; }
	}

	[Mac (10,15)]
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

	[Mac (10,15)]
	[Watch (3,0)]
	[iOS (7,0)]
	[BaseType (typeof (NSObject), Delegates=new string [] {"WeakDelegate"}, Events=new Type [] { typeof (AVSpeechSynthesizerDelegate)})]
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

		[Unavailable (PlatformName.MacCatalyst)]
		[Watch (6,0), TV (13,0), NoMac, iOS (13,0)]
		[Advice ("This API is not available when using UIKit on macOS.")]
		[Export ("usesApplicationAudioSession")]
		bool UsesApplicationAudioSession { get; set; }

		[Watch (6,0), NoTV, NoMac, iOS (13,0)]
		[Export ("mixToTelephonyUplink")]
		bool MixToTelephonyUplink { get; set; }

		[Export ("speakUtterance:")]
		void SpeakUtterance (AVSpeechUtterance utterance);

		[Async]
		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Export ("writeUtterance:toBufferCallback:")]
		void WriteUtterance (AVSpeechUtterance utterance, Action<AVAudioBuffer> bufferCallback);

		[Export ("stopSpeakingAtBoundary:")]
		bool StopSpeaking (AVSpeechBoundary boundary);

		[Export ("pauseSpeakingAtBoundary:")]
		bool PauseSpeaking (AVSpeechBoundary boundary);

		[Export ("continueSpeaking")]
		bool ContinueSpeaking ();

		[NoMac]
		[iOS (10, 0)]
		[TV (10,0)]
		[NullAllowed, Export ("outputChannels", ArgumentSemantic.Retain)]
		AVAudioSessionChannelDescription[] OutputChannels { get; set; }
	}

	[Mac (10,15)]
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

	[NoMac, NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(NSUrlSessionTask))]
	[DisableDefaultCtor]
	interface AVAggregateAssetDownloadTask
	{
		[Export ("URLAsset")]
		AVUrlAsset UrlAsset { get; }
	}

	[NoWatch, NoMac]
	[Obsoleted (PlatformName.TvOS, 12, 0)]
	[Static, Internal]
	interface AVAssetDownloadTaskKeys {
		[iOS (9,0)]
		[Field ("AVAssetDownloadTaskMinimumRequiredMediaBitrateKey")]
		NSString MinimumRequiredMediaBitrateKey { get; }

		[iOS (9,0)]
		[Field ("AVAssetDownloadTaskMediaSelectionKey")]
		NSString MediaSelectionKey { get; }

		[NoWatch, NoTV, iOS (13,0)]
		[Field ("AVAssetDownloadTaskMediaSelectionPrefersMultichannelKey")]
		NSString MediaSelectionPrefersMultichannelKey { get; }
	}

	[NoMac]
	[NoWatch]
	[Obsoleted (PlatformName.TvOS, 12, 0)]
	[StrongDictionary ("AVAssetDownloadTaskKeys")]
	interface AVAssetDownloadOptions {
		NSNumber MinimumRequiredMediaBitrate { get; set; }
		AVMediaSelection MediaSelection { get; set; }
		[NoWatch, NoTV, iOS (13,0)]
		bool MediaSelectionPrefersMultichannel  { get; set;}
	}

	[NoMac]
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

		[Wrap ("GetAssetDownloadTask (urlAsset, destinationUrl, options.GetDictionary ())")]
		[return: NullAllowed]
		AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, NSUrl destinationUrl, AVAssetDownloadOptions options);

		[iOS (10,0)]
		[Export ("assetDownloadTaskWithURLAsset:assetTitle:assetArtworkData:options:")]
		[return: NullAllowed]
		AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, string title, [NullAllowed] NSData artworkData, [NullAllowed] NSDictionary options);

		[iOS (10,0)]
		[Wrap ("GetAssetDownloadTask (urlAsset, title, artworkData, options.GetDictionary ())")]
		[return: NullAllowed]
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

	[iOS (9,0)][Mac (10,11)][Watch (6,0)]
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

	[iOS (9,0), Mac (10,11)][Watch (6,0)]
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

		[Export ("audioComponentDescription")]
		AudioComponentDescription AudioComponentDescription { get; }
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

		[Export ("componentsMatchingDescription:")]
		AVAudioUnitComponent[] GetComponents (AudioComponentDescription desc);

		[Notification]
		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Field ("AVAudioUnitComponentManagerRegistrationsChangedNotification")]
		NSString RegistrationsChangedNotification { get; }
	}

	[Watch (3,0)]
	[Static]
	interface AVAudioUnitManufacturerName {
		
		[Obsoleted (PlatformName.TvOS, 12,0, message: "Field will return 'null'.")]
		[Field ("AVAudioUnitManufacturerNameApple")]
		[Mac (10,10), iOS (9,0)]
		NSString Apple { get; }
	}

#if !MONOMAC // FIXME: Unsure about if CMMetadataFormatDescription will be an INativeObject and will need manual binding for Classic
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

		[NoWatch, NoTV, Mac (10,15), iOS (11,0)]
		[Export ("contentKeySession:didUpdatePersistableContentKey:forContentKeyIdentifier:")]
		void DidUpdate (AVContentKeySession session, NSData persistableContentKey, NSObject keyIdentifier);

		[TV (12,0), NoWatch, Mac (10,14), iOS (12,0)]
		[Export ("contentKeySession:contentKeyRequestDidSucceed:")]
		void DidSucceed (AVContentKeySession session, AVContentKeyRequest keyRequest);

		[TV (12,0), NoWatch, Mac (10,14), iOS (12,0)]
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
		[NoWatch, NoTV, Mac (10,15), iOS (11,0)]
		[Export ("makeSecureTokenForExpirationDateOfPersistableContentKey:completionHandler:")]
		void MakeSecureToken (NSData persistableContentKeyData, Action<NSData, NSError> handler);

		[Async]
		[NoTV, Mac (10,15), iOS (12,2)]
		[Export ("invalidatePersistableContentKey:options:completionHandler:")]
		void InvalidatePersistableContentKey (NSData persistableContentKeyData, [NullAllowed] NSDictionary options, Action<NSData, NSError> handler);

		[Async]
		[NoTV, NoMac, iOS (12, 2)]
		[Wrap ("InvalidatePersistableContentKey (persistableContentKeyData, options.GetDictionary (), handler)")]
		void InvalidatePersistableContentKey (NSData persistableContentKeyData, [NullAllowed] AVContentKeySessionServerPlaybackContextOptions options, Action<NSData, NSError> handler);

		[Async]
		[NoTV, Mac (10,15), iOS (12,2)]
		[Export ("invalidateAllPersistableContentKeysForApp:options:completionHandler:")]
		void InvalidateAllPersistableContentKeys (NSData appIdentifier, [NullAllowed] NSDictionary options, Action<NSData, NSError> handler);

		[Async]
		[NoTV, NoMac, iOS (12, 2)]
		[Wrap ("InvalidateAllPersistableContentKeys (appIdentifier, options.GetDictionary (), handler)")]
		void InvalidateAllPersistableContentKeys (NSData appIdentifier, [NullAllowed] AVContentKeySessionServerPlaybackContextOptions options, Action<NSData, NSError> handler);

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

	[Static][Internal]
	[NoWatch, NoTV, Mac (10,15), iOS (12,2)]
	interface AVContentKeySessionServerPlaybackContextOptionKeys {
		[Field ("AVContentKeySessionServerPlaybackContextOptionProtocolVersions")]
		NSString ProtocolVersionsKey { get; }

		[Field ("AVContentKeySessionServerPlaybackContextOptionServerChallenge")]
		NSString ServerChallengeKey { get; }
	}

	[StrongDictionary ("AVContentKeySessionServerPlaybackContextOptionKeys")]
	[NoWatch, NoTV, NoMac, iOS (12,2)]
	interface AVContentKeySessionServerPlaybackContextOptions {
		NSNumber[] ProtocolVersions { get; }

		NSData ServerChallenge { get; }
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

#if WATCH
	[Static]
#endif
	[TV (10,2), Mac (10,12,4), iOS (10,3), Watch (6,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface AVContentKeyRequest {
		[NoWatch]
		[Field ("AVContentKeyRequestProtocolVersionsKey")]
		NSString ProtocolVersions { get; }

		[NoWatch]
		[Export ("status")]
		AVContentKeyRequestStatus Status { get; }

		[NoWatch]
		[NullAllowed, Export ("error")]
		NSError Error { get; }

		[NoWatch]
		[NullAllowed, Export ("identifier")]
		NSObject Identifier { get; }

		[NoWatch]
		[NullAllowed, Export ("initializationData")]
		NSData InitializationData { get; }

		[NoWatch]
		[Export ("canProvidePersistableContentKey")]
		bool CanProvidePersistableContentKey { get; }

		[NoWatch]
		[TV (12,2), Mac (10,14,4), iOS (12,2)]
		[Export ("options", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Options { get; }

		[NoWatch]
		[Async]
		[Export ("makeStreamingContentKeyRequestDataForApp:contentIdentifier:options:completionHandler:")]
		void MakeStreamingContentKeyRequestData (NSData appIdentifier, [NullAllowed] NSData contentIdentifier, [NullAllowed] NSDictionary<NSString, NSObject> options, Action<NSData, NSError> handler);

		[NoWatch]
		[Export ("processContentKeyResponse:")]
		void Process (AVContentKeyResponse keyResponse);

		[NoWatch]
		[Export ("processContentKeyResponseError:")]
		void Process (NSError error);

		[NoWatch]
		[Deprecated (PlatformName.iOS, 11, 2, message: "Use the 'NSError' overload instead.")]
		[Export ("respondByRequestingPersistableContentKeyRequest"), NoWatch, NoTV, NoMac]
		void RespondByRequestingPersistableContentKeyRequest ();

		[NoWatch, NoTV, Mac (10,15), iOS (11,2)]
		[Export ("respondByRequestingPersistableContentKeyRequestAndReturnError:")]
		bool RespondByRequestingPersistableContentKeyRequest ([NullAllowed] out NSError error);

		[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
		[Field ("AVContentKeyRequestRequiresValidationDataInSecureTokenKey")]
		NSString RequiresValidationDataInSecureTokenKey { get; }
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
		[return: NullAllowed]
		NSData GetPersistableContentKey (NSData keyVendorResponse, [NullAllowed] NSDictionary<NSString, NSObject> options, out NSError outError);

	}

	[TV (10,2), Mac (10,12,4), iOS (10,3), NoWatch]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface AVContentKeyResponse {
		[Internal]
		[Static]
		[Export ("contentKeyResponseWithFairPlayStreamingKeyResponseData:")]
		AVContentKeyResponse _InitWithFairPlayStreamingKeyResponseData (NSData fairPlayStreamingKeyResponseData);

		[TV (11,0), NoWatch, Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("contentKeyResponseWithClearKeyData:initializationVector:")]
		AVContentKeyResponse Create (NSData keyData, [NullAllowed] NSData initializationVector);

		[Internal]
		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		[Static]
		[Export ("contentKeyResponseWithAuthorizationTokenData:")]
		AVContentKeyResponse _InitWithAuthorizationToken (NSData authorizationTokenData);
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

		[iOS (13,0)]
		[Export ("replacementSemanticSegmentationMatteOfType:forPhoto:")]
		[return: NullAllowed]
		AVSemanticSegmentationMatte GetReplacementSemanticSegmentationMatte (NSString semanticSegmentationMatteType, AVCapturePhoto photo);
	}

	[NoTV, iOS (11,0), NoWatch, Mac (10,15)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVCapturePhoto
	{
		[Export ("timestamp")]
		CMTime Timestamp { get; }

		[NoMac]
		[Export ("rawPhoto")]
		bool RawPhoto { [Bind ("isRawPhoto")] get; }

		[NullAllowed, Export ("pixelBuffer")]
		CVPixelBuffer PixelBuffer { get; }

		[NoMac]
		[NullAllowed, Export ("previewPixelBuffer")]
		CVPixelBuffer PreviewPixelBuffer { get; }

		[NoMac]
		[NullAllowed, Export ("embeddedThumbnailPhotoFormat")]
		NSDictionary WeakEmbeddedThumbnailPhotoFormat { get; }

		[NoMac]
		[Wrap ("WeakEmbeddedThumbnailPhotoFormat")]
		AVVideoSettingsCompressed EmbeddedThumbnailPhotoFormat { get; }

		[NoMac]
		[NullAllowed, Export ("depthData")]
		AVDepthData DepthData { get; }

		[NoMac]
		[Export ("metadata")]
		NSDictionary WeakMetadata { get; }

		[NoMac]
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

		[iOS (11,0), NoMac]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetFileDataRepresentation' instead.")]
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

		[NoWatch, NoTV, NoMac, iOS (13,0)]
		[Export ("semanticSegmentationMatteForType:")]
		[return: NullAllowed]
		AVSemanticSegmentationMatte GetSemanticSegmentationMatte ([BindAs (typeof (AVSemanticSegmentationMatteType))] NSString semanticSegmentationMatteType);
	}

	[Watch (6,0), TV (12,0), Mac (10,14), iOS (12,0)]
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

	[NoWatch, TV (12,0), Mac (10,14), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVAssetResourceLoadingRequestor
	{
		[Export ("providesExpiredSessionReports")]
		bool ProvidesExpiredSessionReports { get; }
	}

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
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
	} 

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
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

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface AVCompositionTrackFormatDescriptionReplacement : NSSecureCoding {
		[Export ("originalFormatDescription")]
		CMFormatDescription OriginalFormatDescription { get; }

		[Export ("replacementFormatDescription")]
		CMFormatDescription ReplacementFormatDescription { get; }
	}

	delegate /* OSStatus */ int AVAudioSourceNodeRenderHandler (bool isSilence, AudioTimeStamp timestamp, uint frameCount, ref AudioBuffers outputData);

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof(AVAudioNode))]
	[DisableDefaultCtor]
	interface AVAudioSourceNode : AVAudioMixing {
		[Export ("initWithRenderBlock:")]
		[DesignatedInitializer]
		IntPtr Constructor (AVAudioSourceNodeRenderHandler renderHandler);

		[Export ("initWithFormat:renderBlock:")]
		[DesignatedInitializer]
		IntPtr Constructor (AVAudioFormat format, AVAudioSourceNodeRenderHandler renderHandler);
	}

	delegate int AVAudioSinkNodeReceiverHandler (AudioTimeStamp timestamp, uint frameCount, ref AudioBuffers inputData);

	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
	[BaseType (typeof(AVAudioNode))]
	[DisableDefaultCtor]
	interface AVAudioSinkNode {
		[Export ("initWithReceiverBlock:")]
		[DesignatedInitializer]
		IntPtr Constructor (AVAudioSinkNodeReceiverHandler receiverHandler);
	}

	[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(NSObject))]
	interface AVVideoCompositionRenderHint {

		[Export ("startCompositionTime")]
		CMTime StartCompositionTime { get; }

		[Export ("endCompositionTime")]
		CMTime EndCompositionTime { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[BaseType (typeof(AVCaptureSession))]
	interface AVCaptureMultiCamSession {
		[Static]
		[Export ("multiCamSupported")]
		bool MultiCamSupported { [Bind ("isMultiCamSupported")] get; }

		[Export ("hardwareCost")]
		float HardwareCost { get; }

		[Export ("systemPressureCost")]
		float SystemPressureCost { get; }
	}

	[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(AVMetadataObject))]
	[DisableDefaultCtor]
	interface AVMetadataBodyObject : NSCopying {
		[Export ("bodyID")]
		nint BodyId { get; }
	}

	[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(AVMetadataBodyObject))]
	[DisableDefaultCtor]
	interface AVMetadataCatBodyObject : NSCopying {
	}

	[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(AVMetadataBodyObject))]
	[DisableDefaultCtor]
	interface AVMetadataDogBodyObject : NSCopying {
	}

	[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(AVMetadataBodyObject))]
	[DisableDefaultCtor]
	interface AVMetadataHumanBodyObject : NSCopying {
	}

	[NoWatch, NoTV, Mac (10,15), iOS (13,0)]
	[BaseType (typeof(AVMetadataObject))]
	[DisableDefaultCtor]
	interface AVMetadataSalientObject : NSCopying {
		[Export ("objectID")]
		nint ObjectId { get; }
	}

}
