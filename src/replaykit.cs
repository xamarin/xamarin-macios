//
// ReplayKit bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.UIKit;

namespace XamCore.ReplayKit {

	[iOS (9,0)]
	[BaseType (typeof (UIViewController))]
	interface RPPreviewViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("previewControllerDelegate", ArgumentSemantic.Weak)][NullAllowed]
		IRPPreviewViewControllerDelegate PreviewControllerDelegate { get; set; }
	}

	interface IRPPreviewViewControllerDelegate { }

	[iOS (9,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface RPPreviewViewControllerDelegate {

		[Export ("previewControllerDidFinish:")]
		void DidFinish (RPPreviewViewController previewController);

		[Export ("previewController:didFinishWithActivityTypes:")]
		void DidFinish (RPPreviewViewController previewController, NSSet<NSString> activityTypes);
	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface RPScreenRecorder {

		[Static]
		[Export ("sharedRecorder")]
		RPScreenRecorder SharedRecorder { get; }

		[Async]
		[Export ("startRecordingWithMicrophoneEnabled:handler:")]
		void StartRecording (bool microphoneEnabled, [NullAllowed] Action<NSError> handler);

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

		[Export ("microphoneEnabled", ArgumentSemantic.Assign)]
		bool MicrophoneEnabled { [Bind ("isMicrophoneEnabled")] get; }

		[Export ("available", ArgumentSemantic.Assign)]
		bool Available { [Bind ("isAvailable")] get; }
	}

	interface IRPScreenRecorderDelegate { }

	[iOS (9,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface RPScreenRecorderDelegate {

		[Export ("screenRecorder:didStopRecordingWithError:previewViewController:")]
		void DidStopRecording (RPScreenRecorder screenRecorder, NSError error, [NullAllowed] RPPreviewViewController previewViewController);

		[Export ("screenRecorderDidChangeAvailability:")]
		void DidChangeAvailability (RPScreenRecorder screenRecorder);
	}
}

