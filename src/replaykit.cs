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

	/// <summary>A view controller for previewing and editing a ReplayKit recording.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/ReplayKit/RPPreviewViewController">Apple documentation for <c>RPPreviewViewController</c></related>
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

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:ReplayKit.RPPreviewViewControllerDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:ReplayKit.RPPreviewViewControllerDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:ReplayKit.RPPreviewViewControllerDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:ReplayKit.RPPreviewViewControllerDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IRPPreviewViewControllerDelegate { }

	/// <summary>The view controller protocol for previewing and editing a ReplayKit recording.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/ReplayKit/Reference/RPPreviewViewControllerDelegate_Ref/index.html">Apple documentation for <c>RPPreviewViewControllerDelegate</c></related>
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

	/// <summary>Enables the user to record visual and audio output of applications, with simultaneous recorded audio (screencasts).</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/ReplayKit/Reference/RPScreenRecorder_Ref/index.html">Apple documentation for <c>RPScreenRecorder</c></related>
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
		[TV (15, 4), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("exportClipToURL:duration:completionHandler:")]
		void ExportClip (NSUrl url, double duration, [NullAllowed] Action<NSError> completionHandler);

		[Async]
		[TV (15, 4), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("startClipBufferingWithCompletionHandler:")]
		void StartClipBuffering ([NullAllowed] Action<NSError> completionHandler);

		[Async]
		[TV (15, 4), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("stopClipBufferingWithCompletionHandler:")]
		void StopClipBuffering ([NullAllowed] Action<NSError> completionHandler);
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:ReplayKit.RPScreenRecorderDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:ReplayKit.RPScreenRecorderDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:ReplayKit.RPScreenRecorderDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:ReplayKit.RPScreenRecorderDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IRPScreenRecorderDelegate { }

	/// <summary>Protocol for enabling the user to record visual and audio output of applications, with simultaneous recorded audio (screencasts)..</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/ReplayKit/Reference/RPScreenRecorderDelegate_Ref/index.html">Apple documentation for <c>RPScreenRecorderDelegate</c></related>
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

	/// <summary>Presents a user interface for choosing third-party broadcast services.</summary>
	///     <remarks>The view controller displays the currently installed broadcast services.</remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/ReplayKit/RPBroadcastActivityViewController">Apple documentation for <c>RPBroadcastActivityViewController</c></related>
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

	/// <include file="../docs/api/ReplayKit/IRPBroadcastActivityViewControllerDelegate.xml" path="/Documentation/Docs[@DocId='T:ReplayKit.IRPBroadcastActivityViewControllerDelegate']/*" />
	interface IRPBroadcastActivityViewControllerDelegate { }

	/// <summary>Responds to changes in the UI that is presented by a <see cref="T:ReplayKit.RPBroadcastActivityViewController" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/ReplayKit/RPBroadcastActivityViewControllerDelegate">Apple documentation for <c>RPBroadcastActivityViewControllerDelegate</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface RPBroadcastActivityViewControllerDelegate {
		[Abstract]
		[Export ("broadcastActivityViewController:didFinishWithBroadcastController:error:")]
		void DidFinish (RPBroadcastActivityViewController broadcastActivityViewController, [NullAllowed] RPBroadcastController broadcastController, [NullAllowed] NSError error);
	}

	/// <summary>Manages an audio or video broadcast stream.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/ReplayKit/RPBroadcastController">Apple documentation for <c>RPBroadcastController</c></related>
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

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:ReplayKit.RPBroadcastControllerDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:ReplayKit.RPBroadcastControllerDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:ReplayKit.RPBroadcastControllerDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:ReplayKit.RPBroadcastControllerDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface IRPBroadcastControllerDelegate { }

	/// <summary>Delegate object that responds to changes in a broadcast.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/ReplayKit/RPBroadcastControllerDelegate">Apple documentation for <c>RPBroadcastControllerDelegate</c></related>
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

	/// <related type="externalDocumentation" href="https://developer.apple.com/reference/ReplayKit/RPBroadcastConfiguration">Apple documentation for <c>RPBroadcastConfiguration</c></related>
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

	/// <param name="bundleID">The bundle ID of the newly loaded broadcasting service.</param>
	///     <param name="displayName">The display name of the newly loaded broadcasting service.</param>
	///     <param name="appIcon">The application icon of the newly loaded broadcasting service.</param>
	///     <summary>Delegate that specifies the signature of the completion handler in calls to the <see cref="M:ReplayKit.NSExtensionContext_RPBroadcastExtension.LoadBroadcastingApplicationInfo(Foundation.NSExtensionContext,ReplayKit.LoadBroadcastingHandler)" /> method.</summary>
	delegate void LoadBroadcastingHandler (string bundleID, string displayName, UIImage appIcon);

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

	/// <summary>Base class for managing Replay Kit broadcasts.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/ReplayKit/RPBroadcastHandler">Apple documentation for <c>RPBroadcastHandler</c></related>
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

	/// <related type="externalDocumentation" href="https://developer.apple.com/reference/ReplayKit/RPBroadcastMP4ClipHandler">Apple documentation for <c>RPBroadcastMP4ClipHandler</c></related>
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

	/// <summary>Processes ReplayKit buffer obects as they arrive.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/ReplayKit/RPBroadcastSampleHandler">Apple documentation for <c>RPBroadcastSampleHandler</c></related>
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
