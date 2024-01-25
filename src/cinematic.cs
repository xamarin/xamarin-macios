using AVFoundation;

using CoreFoundation;

using CoreGraphics;

using CoreMedia;

using CoreVideo;

using Foundation;

using Metal;

using ObjCRuntime;

using System;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Cinematic {

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[ErrorDomain ("CNCinematicErrorDomain")]
	[Native]
	public enum CNCinematicErrorCode : long {
		Unknown = 1,
		Unreadable = 2,
		Incomplete = 3,
		Malformed = 4,
		Unsupported = 5,
		Incompatible = 6,
		Cancelled = 7,
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[Native]
	public enum CNRenderingQuality : long {
		Thumbnail,
		Preview,
		Export,
		ExportHigh,
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0)]
	[Native]
	public enum CNDetectionType : long {
		Unknown = 0,
		HumanFace = 1,
		HumanHead = 2,
		HumanTorso = 3,
		CatBody = 4,
		DogBody = 5,
		CatHead = 9,
		DogHead = 10,
		SportsBall = 11,
		AutoFocus = 100,
		FixedFocus = 101,
		Custom = 102,
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNAssetInfo {
		[Async]
		[Static]
		[Export ("checkIfCinematic:completionHandler:")]
		void CheckIfCinematic (AVAsset asset, Action<bool> completionHandler);

		[Async]
		[Static]
		[Export ("loadFromAsset:completionHandler:")]
		void LoadFromAsset (AVAsset asset, Action<CNAssetInfo, NSError> completionHandler);

		[Export ("asset", ArgumentSemantic.Strong)]
		AVAsset Asset { get; }

		[Export ("allCinematicTracks", ArgumentSemantic.Strong)]
		AVAssetTrack [] AllCinematicTracks { get; }

		[Export ("cinematicVideoTrack", ArgumentSemantic.Strong)]
		AVAssetTrack CinematicVideoTrack { get; }

		[Export ("cinematicDisparityTrack", ArgumentSemantic.Strong)]
		AVAssetTrack CinematicDisparityTrack { get; }

		[Export ("cinematicMetadataTrack", ArgumentSemantic.Strong)]
		AVAssetTrack CinematicMetadataTrack { get; }

		[Export ("timeRange")]
		CMTimeRange TimeRange { get; }

		[Export ("naturalSize")]
		CGSize NaturalSize { get; }

		[Export ("preferredSize")]
		CGSize PreferredSize { get; }

		[Export ("preferredTransform")]
		CGAffineTransform PreferredTransform { get; }

		// from @interface AbstractTracks (CNAssetInfo)

		[Export ("frameTimingTrack", ArgumentSemantic.Strong)]
		AVAssetTrack FrameTimingTrack { get; }

		[Export ("videoCompositionTracks", ArgumentSemantic.Strong)]
		AVAssetTrack [] VideoCompositionTracks { get; }

		[Export ("videoCompositionTrackIDs", ArgumentSemantic.Strong)]
		NSNumber [] VideoCompositionTrackIds { get; }

		[Export ("sampleDataTrackIDs", ArgumentSemantic.Strong)]
		NSNumber [] SampleDataTrackIds { get; }
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (CNAssetInfo))]
	interface CNCompositionInfo {
		[Export ("insertTimeRange:ofCinematicAssetInfo:atTime:error:")]
		bool InsertTimeRange (CMTimeRange timeRange, CNAssetInfo assetInfo, CMTime startTime, [NullAllowed] out NSError outError);
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNRenderingSessionAttributes {
		[Async]
		[Static]
		[Export ("loadFromAsset:completionHandler:")]
		void Load (AVAsset asset, Action<CNRenderingSessionAttributes, NSError> completionHandler);

		[Export ("renderingVersion")]
		nint RenderingVersion { get; }
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNRenderingSessionFrameAttributes : NSCopying, NSMutableCopying {
		[Export ("initWithSampleBuffer:sessionAttributes:")]
		NativeHandle Constructor (CMSampleBuffer sampleBuffer, CNRenderingSessionAttributes sessionAttributes);

		[Export ("initWithTimedMetadataGroup:sessionAttributes:")]
		NativeHandle Constructor (AVTimedMetadataGroup metadataGroup, CNRenderingSessionAttributes sessionAttributes);

		[Export ("focusDisparity")]
		float FocusDisparity { get; set; }

		[Export ("fNumber")]
		float FNumber { get; set; }
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNRenderingSession {
		[Export ("initWithCommandQueue:sessionAttributes:preferredTransform:quality:")]
		NativeHandle Constructor (IMTLCommandQueue commandQueue, CNRenderingSessionAttributes sessionAttributes, CGAffineTransform preferredTransform, CNRenderingQuality quality);

		[Export ("commandQueue", ArgumentSemantic.Strong)]
		IMTLCommandQueue CommandQueue { get; }

		[Export ("sessionAttributes", ArgumentSemantic.Strong)]
		CNRenderingSessionAttributes SessionAttributes { get; }

		[Export ("preferredTransform")]
		CGAffineTransform PreferredTransform { get; }

		[Export ("quality")]
		CNRenderingQuality Quality { get; }

		[Export ("encodeRenderToCommandBuffer:frameAttributes:sourceImage:sourceDisparity:destinationImage:")]
		bool EncodeRender (IMTLCommandBuffer commandBuffer, CNRenderingSessionFrameAttributes frameAttributes, CVPixelBuffer sourceImage, CVPixelBuffer sourceDisparity, CVPixelBuffer destinationImage);

		[Export ("encodeRenderToCommandBuffer:frameAttributes:sourceImage:sourceDisparity:destinationRGBA:")]
		bool EncodeRender (IMTLCommandBuffer commandBuffer, CNRenderingSessionFrameAttributes frameAttributes, CVPixelBuffer sourceImage, CVPixelBuffer sourceDisparity, IMTLTexture destinationRgba);

		[Export ("encodeRenderToCommandBuffer:frameAttributes:sourceImage:sourceDisparity:destinationLuma:destinationChroma:")]
		bool EncodeRender (IMTLCommandBuffer commandBuffer, CNRenderingSessionFrameAttributes frameAttributes, CVPixelBuffer sourceImage, CVPixelBuffer sourceDisparity, IMTLTexture destinationLuma, IMTLTexture destinationChroma);

		[Static]
		[Export ("sourcePixelFormatTypes", ArgumentSemantic.Strong)]
		NSNumber [] SourcePixelFormatTypes { get; }

		[Static]
		[Export ("destinationPixelFormatTypes", ArgumentSemantic.Strong)]
		NSNumber [] DestinationPixelFormatTypes { get; }
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNDetection : NSCopying {
		[Export ("initWithTime:detectionType:normalizedRect:focusDisparity:")]
		NativeHandle Constructor (CMTime time, CNDetectionType detectionType, CGRect normalizedRect, float focusDisparity);

		[Export ("time")]
		CMTime Time { get; }

		[Export ("detectionType")]
		CNDetectionType DetectionType { get; }

		[Export ("normalizedRect")]
		CGRect NormalizedRect { get; }

		[Export ("focusDisparity")]
		float FocusDisparity { get; }

		[Export ("detectionID")]
		long DetectionId { get; }

		[Export ("detectionGroupID")]
		long DetectionGroupId { get; }

		[Static]
		[Export ("isValidDetectionID:")]
		bool IsValidDetectionId (long detectionId);

		[Static]
		[Export ("isValidDetectionGroupID:")]
		bool IsValidDetectionGroupId (long detectionGroupId);

		[Static]
		[Export ("accessibilityLabelForDetectionType:")]
		string AccessibilityLabelForDetectionType (CNDetectionType detectionType);

		[Static]
		[Export ("disparityInNormalizedRect:sourceDisparity:detectionType:priorDisparity:")]
		float DisparityInNormalizedRect (CGRect normalizedRect, CVPixelBuffer sourceDisparity, CNDetectionType detectionType, float priorDisparity);
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNDecision : NSCopying {
		[Internal]
		[Export ("initWithTime:detectionID:strong:")]
		NativeHandle _InitWithSingleIdentifier (CMTime time, long detectionId, bool isStrong);

		[Internal]
		[Export ("initWithTime:detectionGroupID:strong:")]
		NativeHandle _InitWithGroupIdentifier (CMTime time, long detectionGroupId, bool isStrong);

		[Export ("time")]
		CMTime Time { get; }

		[Export ("detectionID")]
		long DetectionId { get; }

		[Export ("detectionGroupID")]
		long DetectionGroupId { get; }

		[Export ("userDecision")]
		bool UserDecision { [Bind ("isUserDecision")] get; }

		[Export ("groupDecision")]
		bool GroupDecision { [Bind ("isGroupDecision")] get; }

		[Export ("strongDecision")]
		bool StrongDecision { [Bind ("isStrongDecision")] get; }
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNDetectionTrack : NSCopying {
		[Export ("detectionType")]
		CNDetectionType DetectionType { get; }

		[Export ("detectionID")]
		long DetectionId { get; }

		[Export ("detectionGroupID")]
		long DetectionGroupId { get; }

		[Export ("userCreated")]
		bool UserCreated { [Bind ("isUserCreated")] get; }

		[Export ("discrete")]
		bool Discrete { [Bind ("isDiscrete")] get; }

		[Export ("detectionAtOrBeforeTime:")]
		[return: NullAllowed]
		CNDetection GetDetectionAtOrBeforeTime (CMTime time);

		[Export ("detectionNearestTime:")]
		[return: NullAllowed]
		CNDetection GetDetectionNearestTime (CMTime time);

		[Export ("detectionsInTimeRange:")]
		CNDetection [] GetDetectionsInTimeRange (CMTimeRange timeRange);
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (CNDetectionTrack))]
	interface CNFixedDetectionTrack {
		[Export ("initWithFocusDisparity:")]
		NativeHandle Constructor (float focusDisparity);

		[Export ("initWithOriginalDetection:")]
		NativeHandle Constructor (CNDetection originalDetection);

		[Export ("focusDisparity")]
		float FocusDisparity { get; }

		[NullAllowed, Export ("originalDetection", ArgumentSemantic.Strong)]
		CNDetection OriginalDetection { get; }
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (CNDetectionTrack))]
	interface CNCustomDetectionTrack {
		[Export ("initWithDetections:smooth:")]
		NativeHandle Constructor (CNDetection [] detections, bool applySmoothing);

		[Export ("allDetections", ArgumentSemantic.Strong)]
		CNDetection [] AllDetections { get; }
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNScript {
		[Async]
		[Static]
		[Export ("loadFromAsset:changes:progress:completionHandler:")]
		void Load (AVAsset asset, [NullAllowed] CNScriptChanges changes, [NullAllowed] NSProgress progress, Action<CNScript, NSError> completionHandler);

		[Export ("reloadWithChanges:")]
		void Reload ([NullAllowed] CNScriptChanges changes);

		[Export ("changes")]
		CNScriptChanges Changes { get; }

		[Export ("changesTrimmedByTimeRange:")]
		CNScriptChanges GetChangesTrimmed (CMTimeRange timeRange);

		[Export ("timeRange")]
		CMTimeRange TimeRange { get; }

		[Export ("frameAtTime:tolerance:")]
		[return: NullAllowed]
		CNScriptFrame GetFrame (CMTime time, CMTime tolerance);

		[Export ("framesInTimeRange:")]
		CNScriptFrame [] GetFrames (CMTimeRange timeRange);

		[Export ("decisionAtTime:tolerance:")]
		[return: NullAllowed]
		CNDecision GetDecision (CMTime time, CMTime tolerance);

		[Export ("decisionsInTimeRange:")]
		CNDecision [] GetDecisions (CMTimeRange timeRange);

		[Export ("decisionAfterTime:")]
		[return: NullAllowed]
		CNDecision GetDecisionAfterTime (CMTime time);

		[Export ("decisionBeforeTime:")]
		[return: NullAllowed]
		CNDecision GetDecisionBeforeTime (CMTime time);

		[Export ("primaryDecisionAtTime:")]
		[return: NullAllowed]
		CNDecision GetPrimaryDecision (CMTime time);

		[Export ("secondaryDecisionAtTime:")]
		[return: NullAllowed]
		CNDecision GetSecondaryDecision (CMTime time);

		[Export ("timeRangeOfTransitionAfterDecision:")]
		CMTimeRange GetTimeRangeOfTransitionAfterDecision (CNDecision decision);

		[Export ("timeRangeOfTransitionBeforeDecision:")]
		CMTimeRange GetTimeRangeOfTransitionBeforeDecision (CNDecision decision);

		[Export ("userDecisionsInTimeRange:")]
		CNDecision [] GetUserDecisions (CMTimeRange timeRange);

		[Export ("baseDecisionsInTimeRange:")]
		CNDecision [] GetBaseDecisions (CMTimeRange timeRange);

		[Export ("detectionTrackForID:")]
		[return: NullAllowed]
		CNDetectionTrack GetDetectionTrackForId (long detectionId);

		[Export ("detectionTrackForDecision:")]
		[return: NullAllowed]
		CNDetectionTrack GetDetectionTrack (CNDecision decision);

		[Export ("fNumber")]
		float FNumber { get; set; }

		[Export ("addUserDecision:")]
		bool AddUserDecision (CNDecision decision);

		[Export ("removeUserDecision:")]
		bool RemoveUserDecision (CNDecision decision);

		[Export ("removeAllUserDecisions")]
		void RemoveAllUserDecisions ();

		[Export ("addDetectionTrack:")]
		long AddDetectionTrack (CNDetectionTrack detectionTrack);

		[Export ("removeDetectionTrack:")]
		bool RemoveDetectionTrack (CNDetectionTrack detectionTrack);

		[Export ("addedDetectionTracks", ArgumentSemantic.Strong)]
		CNDetectionTrack [] AddedDetectionTracks { get; }
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNScriptChanges {
		[Export ("initWithDataRepresentation:")]
		NativeHandle Constructor (NSData dataRepresentation);

		[Export ("dataRepresentation")]
		NSData DataRepresentation { get; }

		[Export ("fNumber")]
		float FNumber { get; }

		[Export ("userDecisions")]
		CNDecision [] UserDecisions { get; }

		[Export ("addedDetectionTracks")]
		CNDetectionTrack [] AddedDetectionTracks { get; }
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNScriptFrame : NSCopying {
		[Export ("time")]
		CMTime Time { get; }

		[Export ("focusDisparity")]
		float FocusDisparity { get; }

		[Export ("focusDetection", ArgumentSemantic.Strong)]
		CNDetection FocusDetection { get; }

		[Export ("allDetections", ArgumentSemantic.Strong)]
		CNDetection [] AllDetections { get; }

		[Export ("detectionForID:")]
		[return: NullAllowed]
		CNDetection GetDetectionForId (long detectionId);

		[Export ("bestDetectionForGroupID:")]
		[return: NullAllowed]
		CNDetection GetBestDetectionForGroupId (long detectionGroupId);
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface CNBoundsPrediction : NSCopying, NSMutableCopying {
		[Export ("normalizedBounds", ArgumentSemantic.Assign)]
		CGRect NormalizedBounds { get; set; }

		[Export ("confidence")]
		float Confidence { get; set; }
	}

	[TV (17, 0), NoWatch, Mac (14, 0), iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CNObjectTracker {
		[Static]
		[Export ("isSupported")]
		bool IsSupported { get; }

		[Export ("initWithCommandQueue:")]
		NativeHandle Constructor (IMTLCommandQueue commandQueue);

		[Export ("findObjectAtPoint:sourceImage:")]
		[return: NullAllowed]
		CNBoundsPrediction FindObject (CGPoint point, CVPixelBuffer sourceImage);

		[Export ("startTrackingAt:within:sourceImage:sourceDisparity:")]
		bool StartTracking (CMTime atTime, CGRect normalizedBounds, CVPixelBuffer sourceImage, CVPixelBuffer sourceDisparity);

		[Export ("continueTrackingAt:sourceImage:sourceDisparity:")]
		[return: NullAllowed]
		CNBoundsPrediction ContinueTracking (CMTime atTime, CVPixelBuffer sourceImage, CVPixelBuffer sourceDisparity);

		[Export ("finishDetectionTrack")]
		CNDetectionTrack FinishDetectionTrack { get; }

		[Export ("resetDetectionTrack")]
		void ResetDetectionTrack ();
	}

}
