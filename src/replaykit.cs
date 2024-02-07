//
// ReplayKit bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using AVFoundation;
using CoreMedia;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
#if MONOMAC
using AppKit;
using UIImage = AppKit.NSImage;
using UIViewController = AppKit.NSViewController;
using UIView = AppKit.NSView;
#else
using UIKit;
using NSWindow = Foundation.NSObject;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ReplayKit {

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface RPPreviewViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("previewControllerDelegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		IRPPreviewViewControllerDelegate PreviewControllerDelegate { get; set; }

		[NoiOS]
		[NoMac]
		[NoMacCatalyst]
		[Export ("mode", ArgumentSemantic.Assign)]
		RPPreviewViewControllerMode Mode { get; set; }
	}

	interface IRPPreviewViewControllerDelegate { }

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface RPPreviewViewControllerDelegate {

		[Export ("previewControllerDidFinish:")]
		void DidFinish (RPPreviewViewController previewController);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("previewController:didFinishWithActivityTypes:")]
		void DidFinish (RPPreviewViewController previewController, NSSet<NSString> activityTypes);
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
#if NET || MONOMAC
	[Sealed]
#endif
	interface RPScreenRecorder {

		[Static]
		[Export ("sharedRecorder")]
		RPScreenRecorder SharedRecorder { get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'StartRecording (Action<NSError>)' instead.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'StartRecording (Action<NSError>)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'StartRecording (Action<NSError>)' instead.")]
		[Async]
		[Export ("startRecordingWithMicrophoneEnabled:handler:")]
		void StartRecording (bool microphoneEnabled, [NullAllowed] Action<NSError> handler);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("startRecordingWithHandler:")]
		void StartRecording ([NullAllowed] Action<NSError> handler);

		[Async]
		[Export ("stopRecordingWithHandler:")]
		void StopRecording ([NullAllowed] Action<RPPreviewViewController, NSError> handler);

		[Async]
		[Export ("discardRecordingWithHandler:")]
		void DiscardRecording (Action handler);

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		IRPScreenRecorderDelegate Delegate { get; set; }

		[Export ("recording", ArgumentSemantic.Assign)]
		bool Recording { [Bind ("isRecording")] get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("microphoneEnabled", ArgumentSemantic.Assign)]
		bool MicrophoneEnabled {
			[Bind ("isMicrophoneEnabled")]
			get;
			[MacCatalyst (13, 1)]
			set;
		}

		[Export ("available", ArgumentSemantic.Assign)]
		bool Available { [Bind ("isAvailable")] get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("cameraEnabled")]
		bool CameraEnabled { [Bind ("isCameraEnabled")] get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("cameraPreviewView")]
		UIView CameraPreviewView { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("cameraPosition", ArgumentSemantic.Assign)]
		RPCameraPosition CameraPosition { get; set; }

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("startCaptureWithHandler:completionHandler:")]
		void StartCapture ([NullAllowed] Action<CMSampleBuffer, RPSampleBufferType, NSError> captureHandler, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("stopCaptureWithHandler:")]
		void StopCapture ([NullAllowed] Action<NSError> handler);

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[TV (14, 0), iOS (14, 0)]
		[Async]
		[Export ("stopRecordingWithOutputURL:completionHandler:")]
		void StopRecording (NSUrl url, [NullAllowed] Action<NSError> completionHandler);

		[Async]
		[TV (15, 4), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("exportClipToURL:duration:completionHandler:")]
		void ExportClip (NSUrl url, double duration, [NullAllowed] Action<NSError> completionHandler);

		[Async]
		[TV (15, 4), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("startClipBufferingWithCompletionHandler:")]
		void StartClipBuffering ([NullAllowed] Action<NSError> completionHandler);

		[Async]
		[TV (15, 4), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("stopClipBufferingWithCompletionHandler:")]
		void StopClipBuffering ([NullAllowed] Action<NSError> completionHandler);
	}

	interface IRPScreenRecorderDelegate { }

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface RPScreenRecorderDelegate {

		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'DidStopRecording(RPScreenRecorder,RPPreviewViewController,NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'DidStopRecording(RPScreenRecorder,RPPreviewViewController,NSError)' instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidStopRecording(RPScreenRecorder,RPPreviewViewController,NSError)' instead.")]
		[Export ("screenRecorder:didStopRecordingWithError:previewViewController:")]
		void DidStopRecording (RPScreenRecorder screenRecorder, NSError error, [NullAllowed] RPPreviewViewController previewViewController);

		[MacCatalyst (13, 1)]
		[Export ("screenRecorder:didStopRecordingWithPreviewViewController:error:")]
		void DidStopRecording (RPScreenRecorder screenRecorder, [NullAllowed] RPPreviewViewController previewViewController, [NullAllowed] NSError error);

		[Export ("screenRecorderDidChangeAvailability:")]
		void DidChangeAvailability (RPScreenRecorder screenRecorder);
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface RPBroadcastActivityViewController {
		// inlined
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Static]
		[Async]
		[Export ("loadBroadcastActivityViewControllerWithHandler:")]
		void LoadBroadcastActivityViewController (Action<RPBroadcastActivityViewController, NSError> handler);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IRPBroadcastActivityViewControllerDelegate Delegate { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Async]
		[Export ("loadBroadcastActivityViewControllerWithPreferredExtension:handler:")]
		void LoadBroadcastActivityViewController ([NullAllowed] string preferredExtension, Action<RPBroadcastActivityViewController, NSError> handler);
	}

	interface IRPBroadcastActivityViewControllerDelegate { }

	[NoMac]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface RPBroadcastActivityViewControllerDelegate {
		[Abstract]
		[Export ("broadcastActivityViewController:didFinishWithBroadcastController:error:")]
		void DidFinish (RPBroadcastActivityViewController broadcastActivityViewController, [NullAllowed] RPBroadcastController broadcastController, [NullAllowed] NSError error);
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
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

		[Deprecated (PlatformName.TvOS, 11, 0)]
		[Deprecated (PlatformName.iOS, 11, 0)]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("broadcastExtensionBundleID")]
		[NullAllowed]
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

	interface IRPBroadcastControllerDelegate { }

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface RPBroadcastControllerDelegate {
		[Export ("broadcastController:didFinishWithError:")]
		void DidFinish (RPBroadcastController broadcastController, [NullAllowed] NSError error);

		[Export ("broadcastController:didUpdateServiceInfo:")]
		void DidUpdateServiceInfo (RPBroadcastController broadcastController, NSDictionary<NSString, INSCoding> serviceInfo);

		[MacCatalyst (13, 1)]
		[Export ("broadcastController:didUpdateBroadcastURL:")]
		void DidUpdateBroadcastUrl (RPBroadcastController broadcastController, NSUrl broadcastUrl);
	}

	[Deprecated (PlatformName.TvOS, 11, 0)]
	[Deprecated (PlatformName.iOS, 11, 0)]
	[NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[BaseType (typeof (NSObject))]
	interface RPBroadcastConfiguration : NSCoding, NSSecureCoding {
		[Export ("clipDuration")]
		double ClipDuration { get; set; }

		[NullAllowed, Export ("videoCompressionProperties", ArgumentSemantic.Strong)]
		NSDictionary<NSString, INSSecureCoding> WeakVideoCompressionProperties { get; set; }
	}

	delegate void LoadBroadcastingHandler (string bundleID, string displayName, UIImage appIcon);

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (NSExtensionContext))]
	interface NSExtensionContext_RPBroadcastExtension {
		[Export ("loadBroadcastingApplicationInfoWithCompletion:")]
		void LoadBroadcastingApplicationInfo (LoadBroadcastingHandler handler);

		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CompleteRequest(NSUrl,NSDictionary<NSString,INSCoding>)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CompleteRequest(NSUrl,NSDictionary<NSString,INSCoding>)' instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CompleteRequest(NSUrl,NSDictionary<NSString,INSCoding>)' instead.")]
		[Export ("completeRequestWithBroadcastURL:broadcastConfiguration:setupInfo:")]
		void CompleteRequest (NSUrl broadcastURL, RPBroadcastConfiguration broadcastConfiguration, [NullAllowed] NSDictionary<NSString, INSCoding> setupInfo);

		[MacCatalyst (13, 1)]
		[Export ("completeRequestWithBroadcastURL:setupInfo:")]
		void CompleteRequest (NSUrl broadcastURL, [NullAllowed] NSDictionary<NSString, INSCoding> setupInfo);
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface RPBroadcastHandler : NSExtensionRequestHandling {
		[Export ("updateServiceInfo:")]
		void UpdateServiceInfo (NSDictionary<NSString, INSCoding> serviceInfo);

		// NSInvalidArgumentException -[RPBroadcastHandler updateBroadcastURL:]: unrecognized selector sent to instance 0x608001a4b160
		//	https://trello.com/c/eA440suj/91-33875315-rpbroadcasthandler-updatebroadcasturl-unrecognized-selector
		//
		//[Export ("updateBroadcastURL:")]
		//void UpdateBroadcastUrl (NSUrl broadcastUrl);
	}

	[NoMac]
	[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'RPBroadcastSampleHandler' instead.")]
	[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'RPBroadcastSampleHandler' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RPBroadcastSampleHandler' instead.")]
	[BaseType (typeof (RPBroadcastHandler))]
	interface RPBroadcastMP4ClipHandler {
		[Export ("processMP4ClipWithURL:setupInfo:finished:")]
		void ProcessMP4Clip ([NullAllowed] NSUrl mp4ClipURL, [NullAllowed] NSDictionary<NSString, NSObject> setupInfo, bool finished);

		[Export ("finishedProcessingMP4ClipWithUpdatedBroadcastConfiguration:error:")]
		void FinishedProcessingMP4Clip ([NullAllowed] RPBroadcastConfiguration broadcastConfiguration, [NullAllowed] NSError error);
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (RPBroadcastHandler))]
	interface RPBroadcastSampleHandler {

		[MacCatalyst (13, 1)]
		[Field ("RPVideoSampleOrientationKey")]
		NSString VideoSampleOrientationKey { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("RPApplicationInfoBundleIdentifierKey")]
		NSString ApplicationInfoBundleIdentifierKey { get; }

		[Export ("broadcastStartedWithSetupInfo:")]
		void BroadcastStarted ([NullAllowed] NSDictionary<NSString, NSObject> setupInfo);

		[Export ("broadcastPaused")]
		void BroadcastPaused ();

		[Export ("broadcastResumed")]
		void BroadcastResumed ();

		[Export ("broadcastFinished")]
		void BroadcastFinished ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("broadcastAnnotatedWithApplicationInfo:")]
		void BroadcastAnnotated (NSDictionary applicationInfo);

		[Export ("processSampleBuffer:withType:")]
		void ProcessSampleBuffer (CMSampleBuffer sampleBuffer, RPSampleBufferType sampleBufferType);

		[MacCatalyst (13, 1)]
		[Export ("finishBroadcastWithError:")]
		void FinishBroadcast (NSError error);
	}

	[NoTV]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface RPSystemBroadcastPickerView : NSCoding {

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[NullAllowed, Export ("preferredExtension")]
		string PreferredExtension { get; set; }

		[Export ("showsMicrophoneButton")]
		bool ShowsMicrophoneButton { get; set; }
	}

	[Mac (11, 0)]
	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface RPBroadcastActivityController {

		[Static]
		[Export ("showBroadcastPickerAtPoint:fromWindow:preferredExtensionIdentifier:completionHandler:")]
		void ShowBroadcastPicker (CGPoint point, [NullAllowed] NSWindow window, [NullAllowed] string preferredExtension, Action<RPBroadcastActivityController, NSError> handler);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IRPBroadcastActivityControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

	interface IRPBroadcastActivityControllerDelegate { }

	[Mac (11, 0)]
	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface RPBroadcastActivityControllerDelegate {

		[Abstract]
		[Export ("broadcastActivityController:didFinishWithBroadcastController:error:")]
		void DidFinish (RPBroadcastActivityController broadcastActivityController, [NullAllowed] RPBroadcastController broadcastController, [NullAllowed] NSError error);
	}
}
