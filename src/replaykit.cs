//
// ReplayKit bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.CoreMedia;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.UIKit;

namespace XamCore.ReplayKit {

	[iOS (9,0)]
	[TV (10,0)]
	[BaseType (typeof (UIViewController))]
	interface RPPreviewViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("previewControllerDelegate", ArgumentSemantic.Weak)][NullAllowed]
		IRPPreviewViewControllerDelegate PreviewControllerDelegate { get; set; }

		[TV (10, 0), NoiOS]
		[Export ("mode", ArgumentSemantic.Assign)]
		RPPreviewViewControllerMode Mode { get; set; }
	}

	interface IRPPreviewViewControllerDelegate { }

	[iOS (9,0)]
	[TV (10,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface RPPreviewViewControllerDelegate {

		[Export ("previewControllerDidFinish:")]
		void DidFinish (RPPreviewViewController previewController);

		[NoTV]
		[Export ("previewController:didFinishWithActivityTypes:")]
		void DidFinish (RPPreviewViewController previewController, NSSet<NSString> activityTypes);
	}

	[iOS (9,0)]
	[TV (10,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface RPScreenRecorder {

		[Static]
		[Export ("sharedRecorder")]
		RPScreenRecorder SharedRecorder { get; }

		[Availability (Deprecated = Platform.iOS_10_0, Message = "Use StartRecording(Action<NSError>)")]
		[Async]
		[Export ("startRecordingWithMicrophoneEnabled:handler:")]
		void StartRecording (bool microphoneEnabled, [NullAllowed] Action<NSError> handler);

		[iOS (10,0)]
		[Async]
		[Export ("startRecordingWithHandler:")]
		void StartRecording ([NullAllowed] Action<NSError> handler);

		[Async]
		[Export ("stopRecordingWithHandler:")]
		void StopRecording ([NullAllowed] Action<RPPreviewViewController, NSError> handler);

		[Async]
		[Export ("discardRecordingWithHandler:")]
		void DiscardRecording ([NullAllowed] Action handler);

		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		IRPScreenRecorderDelegate Delegate { get; set; }

		[Export ("recording", ArgumentSemantic.Assign)]
		bool Recording { [Bind ("isRecording")] get; }

		[NoTV]
		[Export ("microphoneEnabled", ArgumentSemantic.Assign)]
		bool MicrophoneEnabled {
			[Bind ("isMicrophoneEnabled")] get;
			[iOS (10,0)] set;
		}

		[Export ("available", ArgumentSemantic.Assign)]
		bool Available { [Bind ("isAvailable")] get; }

		[NoTV, iOS (10,0)]
		[Export ("cameraEnabled")]
		bool CameraEnabled { [Bind ("isCameraEnabled")] get; set; }

		[NoTV, iOS (10,0)]
		[NullAllowed, Export ("cameraPreviewView")]
		UIView CameraPreviewView { get; }
	}

	interface IRPScreenRecorderDelegate { }

	[iOS (9,0)]
	[TV (10,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface RPScreenRecorderDelegate {

		[Export ("screenRecorder:didStopRecordingWithError:previewViewController:")]
		void DidStopRecording (RPScreenRecorder screenRecorder, NSError error, [NullAllowed] RPPreviewViewController previewViewController);

		[Export ("screenRecorderDidChangeAvailability:")]
		void DidChangeAvailability (RPScreenRecorder screenRecorder);
	}

	[iOS (10,0)]
	[TV (10,0)]
	[BaseType (typeof (UIViewController))]
	interface RPBroadcastActivityViewController {
		// inlined
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Static]
		[Async]
		[Export ("loadBroadcastActivityViewControllerWithHandler:")]
		void LoadBroadcastActivityViewController (Action<RPBroadcastActivityViewController, NSError> handler);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IRPBroadcastActivityViewControllerDelegate Delegate { get; set; }
	}

	interface IRPBroadcastActivityViewControllerDelegate {}

	[iOS (10,0)]
	[TV (10,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface RPBroadcastActivityViewControllerDelegate {
		[Abstract]
		[Export ("broadcastActivityViewController:didFinishWithBroadcastController:error:")]
		void DidFinish (RPBroadcastActivityViewController broadcastActivityViewController, [NullAllowed] RPBroadcastController broadcastController, [NullAllowed] NSError error);
	}

	[iOS (10,0)]
	[TV (10,0)]
	[BaseType (typeof (NSObject))]
	interface RPBroadcastController {
		[Export ("broadcasting")]
		bool Broadcasting { [Bind ("isBroadcasting")] get; }

		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; }

		[Export ("broadcastURL")]
		NSUrl BroadcastUrl { get; }

		[NullAllowed, Export ("serviceInfo")]
		NSDictionary<NSString, INSCoding> ServiceInfo { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IRPBroadcastControllerDelegate Delegate { get; set; }

		[Export ("broadcastExtensionBundleID")]
		string BroadcastExtensionBundleID { get; }

		[Async]
		[Export ("startBroadcastWithHandler:")]
		void StartBroadcast (Action<NSError> handler);

		[Export ("pauseBroadcast")]
		void PauseBroadcast ();

		[Export ("resumeBroadcast")]
		void ResumeBroadcast ();

		[Async]
		[Export ("finishBroadcastWithHandler:")]
		void FinishBroadcast (Action<NSError> handler);
	}

	interface IRPBroadcastControllerDelegate {}

	[iOS (10,0)]
	[TV (10,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface RPBroadcastControllerDelegate {
		[Export ("broadcastController:didFinishWithError:")]
		void DidFinish (RPBroadcastController broadcastController, [NullAllowed] NSError error);

		[Export ("broadcastController:didUpdateServiceInfo:")]
		void DidUpdateServiceInfo (RPBroadcastController broadcastController, NSDictionary<NSString, INSCoding> serviceInfo);
	}

	[iOS (10,0)]
	[TV (10,0)]
	[BaseType (typeof (NSObject))]
	interface RPBroadcastConfiguration : NSCoding, NSSecureCoding {
		[Export ("clipDuration")]
		double ClipDuration { get; set; }

		// TODO review, once API documentation is available, if we can provide an easier to use strongdictionary
		[NullAllowed, Export ("videoCompressionProperties", ArgumentSemantic.Strong)]
		NSDictionary<NSNumber, INSSecureCoding> VideoCompressionProperties { get; set; }
	}

	delegate void LoadBroadcastingHandler (string bundleID, string displayName, UIImage appIcon);

	[iOS (10,0)]
	[TV (10,0)]
	[Category]
	[BaseType (typeof (NSExtensionContext))]
	interface NSExtensionContext_RPBroadcastExtension {
		[Export ("loadBroadcastingApplicationInfoWithCompletion:")]
		void LoadBroadcastingApplicationInfo (LoadBroadcastingHandler handler);

		[Export ("completeRequestWithBroadcastURL:broadcastConfiguration:setupInfo:")]
		void CompleteRequest (NSUrl broadcastURL, RPBroadcastConfiguration broadcastConfiguration, [NullAllowed] NSDictionary<NSString, INSCoding> setupInfo);
	}

	[iOS (10,0)]
	[TV (10,0)]
	[BaseType (typeof (NSObject))]
	interface RPBroadcastHandler : NSExtensionRequestHandling {
		[Export ("updateServiceInfo:")]
		void UpdateServiceInfo (NSDictionary<NSString, INSCoding> serviceInfo);
	}

	[iOS (10,0)]
	[TV (10,0)]
	[BaseType (typeof (RPBroadcastHandler))]
	interface RPBroadcastMP4ClipHandler {
		[Export ("processMP4ClipWithURL:setupInfo:finished:")]
		void ProcessMP4Clip ([NullAllowed] NSUrl mp4ClipURL, [NullAllowed] NSDictionary<NSString, NSObject> setupInfo, bool finished);

		[Export ("finishedProcessingMP4ClipWithUpdatedBroadcastConfiguration:error:")]
		void FinishedProcessingMP4Clip ([NullAllowed] RPBroadcastConfiguration broadcastConfiguration, [NullAllowed] NSError error);
	}

	[iOS (10,0)]
	[TV (10,0)]
	[BaseType (typeof (RPBroadcastHandler))]
	interface RPBroadcastSampleHandler {
		[Export ("broadcastStartedWithSetupInfo:")]
		void BroadcastStarted ([NullAllowed] NSDictionary<NSString, NSObject> setupInfo);

		[Export ("broadcastPaused")]
		void BroadcastPaused ();

		[Export ("broadcastResumed")]
		void BroadcastResumed ();

		[Export ("broadcastFinished")]
		void BroadcastFinished ();

		[Export ("processSampleBuffer:withType:")]
		void ProcessSampleBuffer (CMSampleBuffer sampleBuffer, RPSampleBufferType sampleBufferType);
	}
}
