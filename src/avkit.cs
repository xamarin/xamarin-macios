//
// avkit.cs: Definitions for AVKit.cs
//
// Copyright 2014-2015 Xamarin Inc
using System;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreVideo;
using AVFoundation;
#if HAS_OPENGLES
using OpenGLES;
#endif
#if !MONOMAC
using AVPlayerViewControlsStyle = Foundation.NSObject;
using NSColor = UIKit.UIColor;
using NSMenu = Foundation.NSObject;
using NSView = UIKit.UIView;
using UIKit;
#else
using AppKit;
using AVAudioSession = Foundation.NSObject;
using AVContentProposal = Foundation.NSObject;
using AVPlayerViewController = Foundation.NSObject;
using IUIViewControllerTransitionCoordinator = Foundation.NSObject;
using UIColor = AppKit.NSColor;
using UIImage = AppKit.NSImage;
using UILayoutGuide = Foundation.NSObject;
using UITraitCollection = Foundation.NSObject;
using UIView = AppKit.NSView;
using UIViewController = Foundation.NSObject;
using UIWindow = Foundation.NSObject;
using UIAction = Foundation.NSObject;
using UIMenuElement = Foundation.NSObject;
#endif // !MONOMAC

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AVKit {
	[TV (14, 0)]
	[iOS (9,0)]
	[Mac (10,15)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
#if NET
	[Sealed] // Apple docs: Do not subclass AVPictureInPictureController. Overriding this class’s methods is unsupported and results in undefined behavior.
#endif
	interface AVPictureInPictureController
	{
		[Static]
		[Export ("isPictureInPictureSupported")]
		bool IsPictureInPictureSupported { get; }
	
		[Export ("initWithPlayerLayer:")]
		NativeHandle Constructor (AVPlayerLayer playerLayer);

		[TV (15,0), NoWatch, Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
		[Export ("initWithContentSource:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVPictureInPictureControllerContentSource contentSource);
	
		[Export ("playerLayer")]
		AVPlayerLayer PlayerLayer { get; }
	
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IAVPictureInPictureControllerDelegate Delegate { get; set; }
	
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	
		[Export ("startPictureInPicture")]
		void StartPictureInPicture ();
	
		[Export ("stopPictureInPicture")]
		void StopPictureInPicture ();
	
		[Export ("pictureInPicturePossible")]
		bool PictureInPicturePossible { [Bind ("isPictureInPicturePossible")] get; }
	
		[Export ("pictureInPictureActive")]
		bool PictureInPictureActive { [Bind ("isPictureInPictureActive")] get; }
	
		[Export ("pictureInPictureSuspended")]
		bool PictureInPictureSuspended { [Bind ("isPictureInPictureSuspended")] get; }

		[iOS (13,0)]
		[Static]
		[Export ("pictureInPictureButtonStartImage")]
		UIImage PictureInPictureButtonStartImage { get; }
		
		[iOS (13,0)]
		[Static]
		[Export ("pictureInPictureButtonStopImage")]
		UIImage PictureInPictureButtonStopImage { get; }
		
		[NoMac]
		[Static]
		[Export ("pictureInPictureButtonStartImageCompatibleWithTraitCollection:")]
		UIImage CreateStartButton ([NullAllowed] UITraitCollection traitCollection);

		[NoMac]
		[Static]
		[Export ("pictureInPictureButtonStopImageCompatibleWithTraitCollection:")]
		UIImage CreateStopButton ([NullAllowed] UITraitCollection traitCollection);

		[NoWatch, Mac (11, 0), iOS (14, 0)]
		[Export ("requiresLinearPlayback")]
		bool RequiresLinearPlayback { get; set; }

		[NoWatch, NoMac, NoiOS, MacCatalyst (15,0)]
		[Export ("canStopPictureInPicture")]
		bool CanStopPictureInPicture { get; }

		[iOS (14,2)]
		[NoWatch, NoTV, NoMac, MacCatalyst (15,0)]
		[Export ("canStartPictureInPictureAutomaticallyFromInline")]
		bool CanStartPictureInPictureAutomaticallyFromInline { get; set; }

		[TV (15,0), NoWatch, Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
		[Export ("invalidatePlaybackState")]
		void InvalidatePlaybackState ();

		[NullAllowed]
		[TV (15,0), NoWatch, Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
		[Export ("contentSource", ArgumentSemantic.Strong)]
		AVPictureInPictureControllerContentSource ContentSource { get; set; }
	}
	
	interface IAVPictureInPictureControllerDelegate {}

	[iOS (9,0), Mac (10,15), TV (14,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface AVPictureInPictureControllerDelegate
	{
		[Export ("pictureInPictureControllerWillStartPictureInPicture:")]
		void WillStartPictureInPicture (AVPictureInPictureController pictureInPictureController);
	
		[Export ("pictureInPictureControllerDidStartPictureInPicture:")]
		void DidStartPictureInPicture (AVPictureInPictureController pictureInPictureController);
	
		[Export ("pictureInPictureController:failedToStartPictureInPictureWithError:")]
		void FailedToStartPictureInPicture (AVPictureInPictureController pictureInPictureController, NSError error);
	
		[Export ("pictureInPictureControllerWillStopPictureInPicture:")]
		void WillStopPictureInPicture (AVPictureInPictureController pictureInPictureController);
	
		[Export ("pictureInPictureControllerDidStopPictureInPicture:")]
		void DidStopPictureInPicture (AVPictureInPictureController pictureInPictureController);
	
		[Export ("pictureInPictureController:restoreUserInterfaceForPictureInPictureStopWithCompletionHandler:")]
		void RestoreUserInterfaceForPictureInPicture (AVPictureInPictureController pictureInPictureController, Action<bool> completionHandler);
	}

	[NoMac]
	[iOS (8,0)]
	[BaseType (typeof (UIViewController))]
	interface AVPlayerViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[NullAllowed] // by default this property is null
		[Export ("player", ArgumentSemantic.Retain)]
		AVPlayer Player { get; set; }

		[Export ("showsPlaybackControls")]
		bool ShowsPlaybackControls { get; set; }

		[Export ("videoGravity")]
		NSString WeakVideoGravity { get; set; }

		[Export ("readyForDisplay")]
		bool ReadyForDisplay { [Bind ("isReadyForDisplay")] get; }

		[NoTV]
		[Export ("videoBounds")]
		CGRect VideoBounds { get; }

		[NullAllowed]
		[Export ("contentOverlayView")]
		UIView ContentOverlayView { get; }

		[TV (11,0)]
		[NoiOS]
		[Export ("unobscuredContentGuide")]
		UILayoutGuide UnobscuredContentGuide { get; }

		[TV (14, 0)]
		[iOS (9,0)]
		[Export ("allowsPictureInPicturePlayback")]
		bool AllowsPictureInPicturePlayback { get; set; }

		[NoTV]
		[iOS (10,0)]
		[Export ("updatesNowPlayingInfoCenter")]
		bool UpdatesNowPlayingInfoCenter { get; set; }

		[iOS (11,0)]
		[NoTV]
		[Export ("entersFullScreenWhenPlaybackBegins")]
		bool EntersFullScreenWhenPlaybackBegins { get; set; }

		[iOS (11,0)]
		[NoTV]
		[Export ("exitsFullScreenWhenPlaybackEnds")]
		bool ExitsFullScreenWhenPlaybackEnds { get; set; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		[Protocolize]
		AVPlayerViewControllerDelegate Delegate { get; set; }

		[iOS (9,0)]
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NoMac]
		[TV (9,0), iOS (14, 0)]
		[Export ("requiresLinearPlayback")]
		bool RequiresLinearPlayback { get; set; }

#region AVPlayerViewControllerSubtitleOptions
		[NoiOS][NoMac]
		[TV (9,0)]
		[NullAllowed, Export ("allowedSubtitleOptionLanguages", ArgumentSemantic.Copy)]
		string[] AllowedSubtitleOptionLanguages { get; set; }

		[NoiOS][NoMac]
		[TV (9,0)]
		[Export ("requiresFullSubtitles")]
		bool RequiresFullSubtitles { get; set; }
#endregion

		[NullAllowed]
		[NoiOS, TV (10, 0), NoWatch, NoMac]
		[Export ("contentProposalViewController", ArgumentSemantic.Assign)]
		AVContentProposalViewController ContentProposalViewController { get; set; }

		[NoiOS, TV (10, 0), NoWatch, NoMac]
		[Export ("skippingBehavior", ArgumentSemantic.Assign)]
		AVPlayerViewControllerSkippingBehavior SkippingBehavior { get; set; }

		[NoiOS, TV (10, 0), NoWatch, NoMac]
		[Export ("skipForwardEnabled")]
		bool SkipForwardEnabled { [Bind ("isSkipForwardEnabled")] get; set; }

		[NoiOS, TV (10, 0), NoWatch, NoMac]
		[Export ("skipBackwardEnabled")]
		bool SkipBackwardEnabled { [Bind ("isSkipBackwardEnabled")] get; set; }

		// From AVPlayerViewControllerControls category

		[NoiOS, TV (11, 0), NoWatch, NoMac]
		[Export ("playbackControlsIncludeTransportBar")]
		bool PlaybackControlsIncludeTransportBar { get; set; }

		[NoiOS, TV (11, 0), NoWatch, NoMac]
		[Export ("playbackControlsIncludeInfoViews")]
		bool PlaybackControlsIncludeInfoViews { get; set; }

		[NullAllowed]
		[NoiOS, TV (11, 0), NoWatch, NoMac]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CustomInfoViewControllers' instead.")]
		[Export ("customInfoViewController", ArgumentSemantic.Assign)]
		UIViewController CustomInfoViewController { get; set; }

		[NoiOS, TV (11,2), NoMac, NoWatch]
		[Export ("appliesPreferredDisplayCriteriaAutomatically")]
		bool AppliesPreferredDisplayCriteriaAutomatically { get; set; }
		
		[iOS (9,0), TV (13,0), NoWatch]
		[NullAllowed, Export ("pixelBufferAttributes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> PixelBufferAttributes { get; set; }
		
		[NoiOS, TV (13,0), NoWatch]
		[NullAllowed, Export ("customOverlayViewController", ArgumentSemantic.Strong)]
		UIViewController CustomOverlayViewController { get; set; }

		[iOS (14, 0), NoTV]
		[Export ("showsTimecodes")]
		bool ShowsTimecodes { get; set; }

		[iOS (14,2)]
		[NoWatch, NoTV, MacCatalyst (15,0)]
		[Export ("canStartPictureInPictureAutomaticallyFromInline")]
		bool CanStartPictureInPictureAutomaticallyFromInline { get; set; }

		[TV (15,0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("contextualActions", ArgumentSemantic.Copy)]
		UIAction[] ContextualActions { get; set; }

		[TV (15,0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("infoViewActions", ArgumentSemantic.Copy)]
		UIAction[] InfoViewActions { get; set; }

		[TV (15,0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("customInfoViewControllers", ArgumentSemantic.Copy)]
		UIViewController[] CustomInfoViewControllers { get; set; }

		[TV (15,0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("transportBarCustomMenuItems", ArgumentSemantic.Copy)]
		UIMenuElement[] TransportBarCustomMenuItems { get; set; }

		[TV (15,0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("transportBarIncludesTitleView")]
		bool TransportBarIncludesTitleView { get; set; }
	}

	[NoMac]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface AVPlayerViewControllerDelegate
	{
		[TV (14, 0)]
		[Export ("playerViewControllerWillStartPictureInPicture:")]
		void WillStartPictureInPicture (AVPlayerViewController playerViewController);
	
		[TV (14, 0)]
		[Export ("playerViewControllerDidStartPictureInPicture:")]
		void DidStartPictureInPicture (AVPlayerViewController playerViewController);
	
		[TV (14, 0)]
		[Export ("playerViewController:failedToStartPictureInPictureWithError:")]
		void FailedToStartPictureInPicture (AVPlayerViewController playerViewController, NSError error);
	
		[TV (14, 0)]
		[Export ("playerViewControllerWillStopPictureInPicture:")]
		void WillStopPictureInPicture (AVPlayerViewController playerViewController);
	
		[TV (14, 0)]
		[Export ("playerViewControllerDidStopPictureInPicture:")]
		void DidStopPictureInPicture (AVPlayerViewController playerViewController);
	
		[TV (14, 0)]
		[Export ("playerViewControllerShouldAutomaticallyDismissAtPictureInPictureStart:")]
		bool ShouldAutomaticallyDismissAtPictureInPictureStart (AVPlayerViewController playerViewController);
	
		[TV (14, 0)]
		[Export ("playerViewController:restoreUserInterfaceForPictureInPictureStopWithCompletionHandler:")]
		void RestoreUserInterfaceForPictureInPicture (AVPlayerViewController playerViewController, Action<bool> completionHandler);

		[NoiOS][NoMac]
		[TV (9,0)]
		[Export ("playerViewController:didPresentInterstitialTimeRange:")]
		void DidPresentInterstitialTimeRange (AVPlayerViewController playerViewController, AVInterstitialTimeRange interstitial);

		[NoiOS][NoMac]
		[TV (11,0)]
		[Export ("playerViewControllerShouldDismiss:")]
		bool ShouldDismiss (AVPlayerViewController playerViewController);

		[NoiOS][NoMac]
		[TV (11,0)]
		[Export ("playerViewControllerWillBeginDismissalTransition:")]
		void WillBeginDismissalTransition (AVPlayerViewController playerViewController);

		[NoiOS][NoMac]
		[TV (11,0)]
		[Export ("playerViewControllerDidEndDismissalTransition:")]
		void DidEndDismissalTransition (AVPlayerViewController playerViewController);

		[NoiOS][NoMac]
		[TV (9,0)]
		[Export ("playerViewController:willPresentInterstitialTimeRange:")]
		void WillPresentInterstitialTimeRange (AVPlayerViewController playerViewController, AVInterstitialTimeRange interstitial);

		[NoiOS][NoMac]
		[TV (9,0)]
		[Export ("playerViewController:willResumePlaybackAfterUserNavigatedFromTime:toTime:")]
		void WillResumePlaybackAfterUserNavigatedFromTime (AVPlayerViewController playerViewController, CMTime oldTime, CMTime targetTime);

		[NoiOS][NoMac]
		[TV (9,0)]
		[Export ("playerViewController:didSelectMediaSelectionOption:inMediaSelectionGroup:")]
		void DidSelectMediaSelectionOption (AVPlayerViewController playerViewController, [NullAllowed] AVMediaSelectionOption mediaSelectionOption, AVMediaSelectionGroup mediaSelectionGroup);

		[NoiOS][NoMac]
		[TV (9,0)]
		[Export ("playerViewController:didSelectExternalSubtitleOptionLanguage:")]
		void DidSelectExternalSubtitleOptionLanguage (AVPlayerViewController playerViewController, string language);

		[NoiOS, TV (10,0), NoWatch, NoMac]
		[Export ("playerViewController:timeToSeekAfterUserNavigatedFromTime:toTime:")]
		CMTime GetTimeToSeekAfterUserNavigated (AVPlayerViewController playerViewController, CMTime oldTime, CMTime targetTime);

		[NoiOS, TV (10,0), NoWatch, NoMac]
		[Export ("skipToNextItemForPlayerViewController:")]
		void SkipToNextItem (AVPlayerViewController playerViewController);

		[NoiOS, TV (10,0), NoWatch, NoMac]
		[Export ("skipToPreviousItemForPlayerViewController:")]
		void SkipToPreviousItem (AVPlayerViewController playerViewController);

		[NoiOS, TV (10,0), NoWatch, NoMac]
		[Export ("playerViewController:shouldPresentContentProposal:")]
		bool ShouldPresentContentProposal (AVPlayerViewController playerViewController, AVContentProposal proposal);

		[NoiOS, TV (10,0), NoWatch, NoMac]
		[Export ("playerViewController:didAcceptContentProposal:")]
		void DidAcceptContentProposal (AVPlayerViewController playerViewController, AVContentProposal proposal);

		[NoiOS, TV (10,0), NoWatch, NoMac]
		[Export ("playerViewController:didRejectContentProposal:")]
		void DidRejectContentProposal (AVPlayerViewController playerViewController, AVContentProposal proposal);

		[NoiOS, TV (11,0), NoWatch, NoMac]
		[Export ("playerViewController:willTransitionToVisibilityOfTransportBar:withAnimationCoordinator:")]
		void WillTransitionToVisibilityOfTransportBar (AVPlayerViewController playerViewController, bool visible, IAVPlayerViewControllerAnimationCoordinator coordinator);
		
		[iOS (13,0), NoTV, NoWatch, NoMac]
		[Export ("playerViewController:willBeginFullScreenPresentationWithAnimationCoordinator:"), EventArgs ("AVPlayerViewFullScreenPresentationWillBegin")]
		void WillBeginFullScreenPresentation (AVPlayerViewController playerViewController, IUIViewControllerTransitionCoordinator coordinator);
		
		[iOS (13,0), NoTV, NoWatch, NoMac]
		[Export ("playerViewController:willEndFullScreenPresentationWithAnimationCoordinator:"), EventArgs ("AVPlayerViewFullScreenPresentationWillEnd")]
		void WillEndFullScreenPresentation (AVPlayerViewController playerViewController, IUIViewControllerTransitionCoordinator coordinator);
		
		[TV (13,0), NoiOS, NoWatch, NoMac]
		[Export ("nextChannelInterstitialViewControllerForPlayerViewController:")]
		UIViewController GetNextChannelInterstitialViewController (AVPlayerViewController playerViewController);
		
		[TV (13,0), NoiOS, NoWatch, NoMac]
		[Export ("playerViewController:skipToNextChannel:"), EventArgs ("AVPlayerViewSkipToNextChannel")]
		void SkipToNextChannel (AVPlayerViewController playerViewController, Action<bool> completion);
		
		[TV (13,0), NoiOS, NoWatch, NoMac]
		[Export ("playerViewController:skipToPreviousChannel:"), EventArgs ("AVPlayerViewSkipToPreviousChannel")]
		void SkipToPreviousChannel (AVPlayerViewController playerViewController, Action<bool> completion);
		
		[TV (13,0), NoiOS, NoWatch, NoMac]
		[Export ("previousChannelInterstitialViewControllerForPlayerViewController:")]
		UIViewController GetPreviousChannelInterstitialViewController (AVPlayerViewController playerViewController);

		[iOS (15,0), NoTV, NoMac, NoWatch, MacCatalyst (15,0)]
		[Export ("playerViewController:restoreUserInterfaceForFullScreenExitWithCompletionHandler:")]
		void RestoreUserInterfaceForFullScreenExit (AVPlayerViewController playerViewController, Action<bool> completionHandler);
	}

	[NoWatch, NoTV, NoMac, iOS (13,0)]
	[Category]
	[BaseType (typeof(AVAudioSession))]
	interface AVAudioSession_AVPlaybackRouteSelecting {

		[Async (ResultTypeName="PreparingRouteSelectionForPlayback")]
		[Export ("prepareRouteSelectionForPlaybackWithCompletionHandler:")]
		void PrepareRouteSelectionForPlayback (Action<bool, AVAudioSessionRouteSelection> completionHandler);
	}

	interface IAVPlayerViewControllerAnimationCoordinator { }

	[NoiOS, TV (11,0), NoWatch, NoMac]
	[Protocol]
	interface AVPlayerViewControllerAnimationCoordinator {

		[Abstract]
		[Export ("addCoordinatedAnimations:completion:")]
		void AddCoordinatedAnimations (Action animations, Action<bool> completion);
	}

	[NoiOS, NoWatch, NoTV]
	[Mac (10,9)]
	[BaseType (typeof (NSView))]
	interface AVPlayerView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frameRect);

		[NullAllowed]
		[Export ("player")]
		AVPlayer Player { get; set; }

		[Export ("controlsStyle")]
		AVPlayerViewControlsStyle ControlsStyle { get; set; }

		[Mac (10,10)]
		[Export ("videoGravity")]
		string VideoGravity { get; set; }

		[Mac (10,10)]
		[Export ("readyForDisplay")]
		bool ReadyForDisplay { [Bind ("isReadyForDisplay")] get; }

		[Mac (10,10)]
		[Export ("videoBounds")]
		CGRect VideoBounds { get; }

		[NullAllowed]
		[Mac (10,10)]
		[Export ("contentOverlayView")]
		NSView ContentOverlayView { get; }

		[Mac (10,13)]
		[Export ("updatesNowPlayingInfoCenter")]
		bool UpdatesNowPlayingInfoCenter { get; set; }

		[NullAllowed]
		[Export ("actionPopUpButtonMenu")]
		NSMenu ActionPopUpButtonMenu { get; set; }

		// No async
		[Export ("beginTrimmingWithCompletionHandler:")]
		void BeginTrimming ([NullAllowed] Action<AVPlayerViewTrimResult> handler);

		[Export ("canBeginTrimming")]
		bool CanBeginTrimming { get; }

		[Export ("flashChapterNumber:chapterTitle:")]
		void FlashChapter (nuint chapterNumber, [NullAllowed] string chapterTitle);

		[Export ("showsFrameSteppingButtons")]
		bool ShowsFrameSteppingButtons { get; set; }

		[Export ("showsFullScreenToggleButton")]
		bool ShowsFullScreenToggleButton { get; set; }

		[Export ("showsSharingServiceButton")]
		bool ShowsSharingServiceButton { get; set; }
		
		[Mac (10,15)]
		[Export ("allowsPictureInPicturePlayback")]
		bool AllowsPictureInPicturePlayback { get; set; }
		
		[Mac (10,15)]
		[Wrap ("WeakPictureInPictureDelegate")]
		[NullAllowed]
		IAVPlayerViewPictureInPictureDelegate PictureInPictureDelegate { get; set; }
		
		[Mac (10,15)]
		[NullAllowed, Export ("pictureInPictureDelegate", ArgumentSemantic.Weak)]
		NSObject WeakPictureInPictureDelegate { get; set; }
		
		[Mac (10,15)]
		[Export ("showsTimecodes")]
		bool ShowsTimecodes { get; set; }

		[Mac (12,0)]
		[Wrap ("WeakDelegate")]
		[Protocolize]
		AVPlayerViewDelegate Delegate { get; set; }

		[Mac (12,0)]
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

	interface IAVPlayerViewPictureInPictureDelegate {}

	[NoiOS, NoWatch, NoTV]
	[Mac (10,15)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof(NSObject))]
	interface AVPlayerViewPictureInPictureDelegate {

		[Export ("playerViewWillStartPictureInPicture:")]
		void WillStart (AVPlayerView playerView);

		[Export ("playerViewDidStartPictureInPicture:")]
		void DidStart (AVPlayerView playerView);

		[Export ("playerView:failedToStartPictureInPictureWithError:"), EventArgs ("AVPlayerViewFailedToStart")]
		void FailedToStart (AVPlayerView playerView, NSError error);

		[Export ("playerViewWillStopPictureInPicture:")]
		void WillStop (AVPlayerView playerView);

		[Export ("playerViewDidStopPictureInPicture:")]
		void DidStop (AVPlayerView playerView);

		[Export ("playerView:restoreUserInterfaceForPictureInPictureStopWithCompletionHandler:"), EventArgs ("AVPlayerViewRestoreUserInterface")]
		void RestoreUserInterface (AVPlayerView playerView, Action<bool> completionHandler);

		[Export ("playerViewShouldAutomaticallyDismissAtPictureInPictureStart:")]
		bool ShouldAutomaticallyDismiss (AVPlayerView playerView);
	}

	[NoiOS, NoWatch, NoTV]
	[Mac (10,10)]
	[BaseType (typeof (NSView))]
	interface AVCaptureView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frameRect);

		[Export ("session"), NullAllowed]
		AVCaptureSession Session { get; }

		[Export ("setSession:showVideoPreview:showAudioPreview:")]
		void SetSession ([NullAllowed] AVCaptureSession session, bool showVideoPreview, bool showAudioPreview);

		[Export ("fileOutput"), NullAllowed]
		AVCaptureFileOutput FileOutput { get; }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		IAVCaptureViewDelegate Delegate { get; set; }

		[Export ("controlsStyle")]
		AVCaptureViewControlsStyle ControlsStyle { get; set; }

		// TODO: Create an enum version of this property
		[Export ("videoGravity", ArgumentSemantic.Copy)]
		NSString WeakVideoGravity { get; set; }
	}

	interface IAVCaptureViewDelegate { }

	[NoiOS, NoWatch, NoTV]
	[Protocol, Model]
	[Mac (10,10)]
	[BaseType (typeof (NSObject))]
	interface AVCaptureViewDelegate {
		[Abstract]
		[Export ("captureView:startRecordingToFileOutput:")]
		void StartRecording (AVCaptureView captureView, AVCaptureFileOutput fileOutput);
	}

	[NoiOS][NoMac]
	[TV (9,0)]
	[BaseType (typeof (NSObject))]
	interface AVInterstitialTimeRange : NSCopying, NSSecureCoding {
		[Export ("initWithTimeRange:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CMTimeRange timeRange);

		[Export ("timeRange")]
		CMTimeRange TimeRange { get; }
	}

	[NoiOS][NoMac]
	[TV (9,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface AVNavigationMarkersGroup {
		[Export ("initWithTitle:timedNavigationMarkers:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string title, AVTimedMetadataGroup[] navigationMarkers);

		[Export ("initWithTitle:dateRangeNavigationMarkers:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string title, AVDateRangeMetadataGroup[] navigationMarkers);

		[NullAllowed, Export ("title")]
		string Title { get; }

		[NullAllowed, Export ("timedNavigationMarkers")]
		AVTimedMetadataGroup[] TimedNavigationMarkers { get; }

		[NullAllowed, Export ("dateRangeNavigationMarkers")]
		AVDateRangeMetadataGroup[] DateRangeNavigationMarkers { get; }
	}
	
	[NoMac]
	[NoiOS, TV (10,0), NoWatch]
	[BaseType (typeof(UIViewController))]
	interface AVContentProposalViewController
	{
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);
		
		[NullAllowed, Export ("contentProposal")]
		AVContentProposal ContentProposal { get; }

		[NullAllowed, Export ("playerViewController", ArgumentSemantic.Weak)]
		AVPlayerViewController PlayerViewController { get; }

		[Export ("preferredPlayerViewFrame")]
		CGRect PreferredPlayerViewFrame { get; }

		[Export ("playerLayoutGuide")]
		UILayoutGuide PlayerLayoutGuide { get; }

		[NullAllowed, Export ("dateOfAutomaticAcceptance", ArgumentSemantic.Assign)]
		NSDate DateOfAutomaticAcceptance { get; set; }

		[Export ("dismissContentProposalForAction:animated:completion:")]
		void DismissContentProposal (AVContentProposalAction action, bool animated, [NullAllowed] Action block);
	}

	[Static]
	[NoMac]
	[NoiOS, TV (10,1), NoWatch]
	interface AVKitMetadataIdentifier {

		[Field ("AVKitMetadataIdentifierExternalContentIdentifier")]
		NSString ExternalContentIdentifier { get; }
		[Field ("AVKitMetadataIdentifierExternalUserProfileIdentifier")]
		NSString ExternalUserProfileIdentifier { get; }
		[Field ("AVKitMetadataIdentifierPlaybackProgress")]
		NSString PlaybackProgress { get; }

		[TV (11,0)]
		[Field ("AVKitMetadataIdentifierExactStartDate")]
		NSString ExactStartDate { get; }

		[TV (11,0)]
		[Field ("AVKitMetadataIdentifierApproximateStartDate")]
		NSString ApproximateStartDate { get; }

		[TV (11,0)]
		[Field ("AVKitMetadataIdentifierExactEndDate")]
		NSString ExactEndDate { get; }

		[TV (11,0)]
		[Field ("AVKitMetadataIdentifierApproximateEndDate")]
		NSString ApproximateEndDate { get; }

		[TV (11,0)]
		[Field ("AVKitMetadataIdentifierServiceIdentifier")]
		NSString ServiceIdentifier { get; }
	}

	[Mac (10,15)]
	[TV (11,0), iOS (11,0)]
	[BaseType (typeof (UIView))]
	interface AVRoutePickerView {

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Wrap ("WeakDelegate", IsVirtual = true)]
		[NullAllowed]
		IAVRoutePickerViewDelegate Delegate { get; set; }
		
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NoMac]
		[Export ("activeTintColor", ArgumentSemantic.Assign), NullAllowed]
		UIColor ActiveTintColor { get; set; }

		[NoiOS, NoMac, NoWatch, NoMacCatalyst]
		[TV (11,0)]
		[Export ("routePickerButtonStyle", ArgumentSemantic.Assign)]
		AVRoutePickerViewButtonStyle RoutePickerButtonStyle { get; set; }

		[NoMac]
		[TV (13,0), iOS (13,0)]
		[Export ("prioritizesVideoDevices")]
		bool PrioritizesVideoDevices { get; set; }

		[NoiOS, NoTV, NoWatch]
		[Export ("routePickerButtonColorForState:")]
		NSColor GetRoutePickerButtonColor (AVRoutePickerViewButtonState state);

		[NoiOS, NoTV, NoWatch]
		[Export ("setRoutePickerButtonColor:forState:")]
		void SetRoutePickerButtonColor ([NullAllowed] NSColor color, AVRoutePickerViewButtonState state);

		[NoiOS, NoTV, NoWatch]
		[Export ("routePickerButtonBordered")]
		bool RoutePickerButtonBordered { [Bind ("isRoutePickerButtonBordered")] get; set; }

		[NoiOS, NoTV, NoWatch]
		[NullAllowed, Export ("player", ArgumentSemantic.Assign)]
		AVPlayer Player { get; set; }
	}

	[NoiOS, NoMac, NoWatch, NoMacCatalyst]
	[TV (11,0)]
	[Native]
	public enum AVRoutePickerViewButtonStyle : long {
		System,
		Plain,
		Custom,
	}

	interface IAVRoutePickerViewDelegate { }

	[TV (11,0), iOS (11,0)]
	[Mac (10,15)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVRoutePickerViewDelegate {

		[Export ("routePickerViewWillBeginPresentingRoutes:")]
		void WillBeginPresentingRoutes (AVRoutePickerView routePickerView);

		[Export ("routePickerViewDidEndPresentingRoutes:")]
		void DidEndPresentingRoutes (AVRoutePickerView routePickerView);
	}

	[TV (11,2), NoiOS, NoMac, NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVDisplayManager {

		[TV (11,3)]
		[Field ("AVDisplayManagerModeSwitchStartNotification")]
		[Notification]
		NSString ModeSwitchStartNotification { get; }

		[TV (11,3)]
		[Field ("AVDisplayManagerModeSwitchEndNotification")]
		[Notification]
		NSString ModeSwitchEndNotification { get; }

		[TV (11,3)]
		[Field ("AVDisplayManagerModeSwitchSettingsChangedNotification")]
		[Notification]
		NSString ModeSwitchSettingsChangedNotification { get; }

		[NullAllowed, Export ("preferredDisplayCriteria", ArgumentSemantic.Copy)]
		AVDisplayCriteria PreferredDisplayCriteria { get; set; }

		[Export ("displayModeSwitchInProgress")]
		bool DisplayModeSwitchInProgress { [Bind ("isDisplayModeSwitchInProgress")] get; }

		[TV (11,3)]
		[Export ("displayCriteriaMatchingEnabled")]
		bool DisplayCriteriaMatchingEnabled { [Bind ("isDisplayCriteriaMatchingEnabled")] get; }
	}

	[TV (11,2), NoiOS, NoMac, NoWatch]
	[Category]
	[BaseType (typeof (UIWindow))]
	interface UIWindow_AVAdditions {

		[Export ("avDisplayManager")]
		AVDisplayManager GetAVDisplayManager ();
	}

	[NoTV, NoWatch, NoMac, iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (UIViewController))]
	interface AVPictureInPictureVideoCallViewController {
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);
	}

	[TV (15,0), NoWatch, Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPictureInPictureControllerContentSource
	{
		[Export ("initWithPlayerLayer:")]
		NativeHandle Constructor (AVPlayerLayer playerLayer);

		[NullAllowed, Export ("playerLayer")]
		AVPlayerLayer PlayerLayer { get; }

		// interface AVPictureInPictureControllerContentSource_VideoCallSupport
		[NoWatch, NoTV, NoMac]
		[NoMacCatalyst] // doc as available, intro fails on macOS 12 beta 6
		[Export ("initWithActiveVideoCallSourceView:contentViewController:")]
		NativeHandle Constructor (UIView sourceView, AVPictureInPictureVideoCallViewController contentViewController);

		[NullAllowed]
		[NoWatch, NoTV, NoMac]
		[NoMacCatalyst] // doc as available, intro fails on macOS 12 beta 6
		[Export ("activeVideoCallSourceView", ArgumentSemantic.Weak)]
		UIView ActiveVideoCallSourceView { get; }

		[NoWatch, NoTV, NoMac]
		[NoMacCatalyst] // doc as available, intro fails on macOS 12 beta 6
		[Export ("activeVideoCallContentViewController")]
		AVPictureInPictureVideoCallViewController ActiveVideoCallContentViewController { get; }

		// interface AVPictureInPictureControllerContentSource_AVSampleBufferDisplayLayerSupport
		[Export ("initWithSampleBufferDisplayLayer:playbackDelegate:")]
		NativeHandle Constructor (AVSampleBufferDisplayLayer sampleBufferDisplayLayer, IAVPictureInPictureSampleBufferPlaybackDelegate playbackDelegate);

		[NullAllowed, Export ("sampleBufferDisplayLayer")]
		AVSampleBufferDisplayLayer SampleBufferDisplayLayer { get; }

		[Wrap ("WeakSampleBufferPlaybackDelegate")]
		[NullAllowed]
		IAVPictureInPictureSampleBufferPlaybackDelegate SampleBufferPlaybackDelegate { get; }

		[NullAllowed, Export ("sampleBufferPlaybackDelegate", ArgumentSemantic.Weak)]
		NSObject WeakSampleBufferPlaybackDelegate { get; }
	}

	interface IAVPictureInPictureSampleBufferPlaybackDelegate {}

	[TV (15,0), NoWatch, Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof(NSObject))]
	interface AVPictureInPictureSampleBufferPlaybackDelegate
	{
		[Abstract]
		[Export ("pictureInPictureController:setPlaying:")]
		void SetPlaying (AVPictureInPictureController pictureInPictureController, bool playing);

		[Abstract]
		[Export ("pictureInPictureControllerTimeRangeForPlayback:")]
		CMTimeRange GetTimeRange (AVPictureInPictureController pictureInPictureController);

		[Abstract]
		[Export ("pictureInPictureControllerIsPlaybackPaused:")]
		bool IsPlaybackPaused (AVPictureInPictureController pictureInPictureController);

		[Abstract]
		[Export ("pictureInPictureController:didTransitionToRenderSize:")]
		void DidTransitionToRenderSize (AVPictureInPictureController pictureInPictureController, CMVideoDimensions newRenderSize);

		[Abstract]
		[Export ("pictureInPictureController:skipByInterval:completionHandler:")]
		void SkipByInterval (AVPictureInPictureController pictureInPictureController, CMTime skipInterval, Action completionHandler);

		[Export ("pictureInPictureControllerShouldProhibitBackgroundAudioPlayback:")]
		bool ShouldProhibitBackgroundAudioPlayback (AVPictureInPictureController pictureInPictureController);
	}

	[Mac (12,0), NoiOS, NoTV, NoMacCatalyst]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof(NSObject))]
	interface AVPlayerViewDelegate
	{
		[Export ("playerViewWillEnterFullScreen:")]
		void WillEnterFullScreen (AVPlayerView playerView);

		[Export ("playerViewDidEnterFullScreen:")]
		void DidEnterFullScreen (AVPlayerView playerView);

		[Export ("playerViewWillExitFullScreen:")]
		void WillExitFullScreen (AVPlayerView playerView);

		[Export ("playerViewDidExitFullScreen:")]
		void DidExitFullScreen (AVPlayerView playerView);

		[Export ("playerView:restoreUserInterfaceForFullScreenExitWithCompletionHandler:")]
		void RestoreUserInterfaceForFullScreenExit (AVPlayerView playerView, Action<bool> completionHandler);
	}

	[Mac (10,10)]
	[NoiOS][NoTV][NoWatch][NoMacCatalyst]
	[Native]
	public enum AVCaptureViewControlsStyle : long {
		Inline,
		Floating,
		InlineDeviceSelection,
		Default = Inline,
	}

	[Mac (10,9)]
	[NoiOS][NoTV][NoWatch][NoMacCatalyst]
	[Native]
	public enum AVPlayerViewTrimResult : long {
		OKButton,
		CancelButton,
	}

}
