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

#if TVOS || WATCH
using AVCustomRoutingController = Foundation.NSObject;
using AVCustomRoutingEvent = Foundation.NSObject;
using AVCustomRoutingActionItem = Foundation.NSObject;
#else
using AVRouting;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AVKit {
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
#if NET
	[Sealed] // Apple docs: Do not subclass AVPictureInPictureController. Overriding this class’s methods is unsupported and results in undefined behavior.
#endif
	interface AVPictureInPictureController {
		[Static]
		[Export ("isPictureInPictureSupported")]
		bool IsPictureInPictureSupported { get; }

		[Export ("initWithPlayerLayer:")]
		NativeHandle Constructor (AVPlayerLayer playerLayer);

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("pictureInPictureButtonStartImage")]
		UIImage PictureInPictureButtonStartImage { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("pictureInPictureButtonStopImage")]
		UIImage PictureInPictureButtonStopImage { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("pictureInPictureButtonStartImageCompatibleWithTraitCollection:")]
		UIImage CreateStartButton ([NullAllowed] UITraitCollection traitCollection);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("pictureInPictureButtonStopImageCompatibleWithTraitCollection:")]
		UIImage CreateStopButton ([NullAllowed] UITraitCollection traitCollection);

		[NoWatch, Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("requiresLinearPlayback")]
		bool RequiresLinearPlayback { get; set; }

		[NoWatch, NoMac, NoiOS, MacCatalyst (15, 0)]
		[Export ("canStopPictureInPicture")]
		bool CanStopPictureInPicture { get; }

		[iOS (14, 2)]
		[NoWatch, NoTV, NoMac, MacCatalyst (15, 0)]
		[Export ("canStartPictureInPictureAutomaticallyFromInline")]
		bool CanStartPictureInPictureAutomaticallyFromInline { get; set; }

		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("invalidatePlaybackState")]
		void InvalidatePlaybackState ();

		[NullAllowed]
		[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("contentSource", ArgumentSemantic.Strong)]
		AVPictureInPictureControllerContentSource ContentSource { get; set; }
	}

	interface IAVPictureInPictureControllerDelegate { }

	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVPictureInPictureControllerDelegate {
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
	[MacCatalyst (13, 1)]
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
		[MacCatalyst (13, 1)]
		[Export ("videoBounds")]
		CGRect VideoBounds { get; }

		[NullAllowed]
		[Export ("contentOverlayView")]
		UIView ContentOverlayView { get; }

		[NoiOS]
		[NoMacCatalyst]
		[Export ("unobscuredContentGuide")]
		UILayoutGuide UnobscuredContentGuide { get; }

		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("allowsPictureInPicturePlayback")]
		bool AllowsPictureInPicturePlayback { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("updatesNowPlayingInfoCenter")]
		bool UpdatesNowPlayingInfoCenter { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("entersFullScreenWhenPlaybackBegins")]
		bool EntersFullScreenWhenPlaybackBegins { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("exitsFullScreenWhenPlaybackEnds")]
		bool ExitsFullScreenWhenPlaybackEnds { get; set; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IAVPlayerViewControllerDelegate Delegate { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NoMac]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("requiresLinearPlayback")]
		bool RequiresLinearPlayback { get; set; }

		#region AVPlayerViewControllerSubtitleOptions
		[NoiOS]
		[NoMac]
		[NoMacCatalyst]
		[NullAllowed, Export ("allowedSubtitleOptionLanguages", ArgumentSemantic.Copy)]
		string [] AllowedSubtitleOptionLanguages { get; set; }

		[NoiOS]
		[NoMac]
		[NoMacCatalyst]
		[Export ("requiresFullSubtitles")]
		bool RequiresFullSubtitles { get; set; }
		#endregion

		[NullAllowed]
		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("contentProposalViewController", ArgumentSemantic.Assign)]
		AVContentProposalViewController ContentProposalViewController { get; set; }

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("skippingBehavior", ArgumentSemantic.Assign)]
		AVPlayerViewControllerSkippingBehavior SkippingBehavior { get; set; }

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("skipForwardEnabled")]
		bool SkipForwardEnabled { [Bind ("isSkipForwardEnabled")] get; set; }

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("skipBackwardEnabled")]
		bool SkipBackwardEnabled { [Bind ("isSkipBackwardEnabled")] get; set; }

		// From AVPlayerViewControllerControls category

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("playbackControlsIncludeTransportBar")]
		bool PlaybackControlsIncludeTransportBar { get; set; }

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("playbackControlsIncludeInfoViews")]
		bool PlaybackControlsIncludeInfoViews { get; set; }

		[NullAllowed]
		[NoiOS, NoWatch, NoMac]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CustomInfoViewControllers' instead.")]
		[NoMacCatalyst]
		[Export ("customInfoViewController", ArgumentSemantic.Assign)]
		UIViewController CustomInfoViewController { get; set; }

		[NoiOS, TV (11, 2), NoMac, NoWatch]
		[NoMacCatalyst]
		[Export ("appliesPreferredDisplayCriteriaAutomatically")]
		bool AppliesPreferredDisplayCriteriaAutomatically { get; set; }

		[TV (13, 0), NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("pixelBufferAttributes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> PixelBufferAttributes { get; set; }

		[NoiOS, TV (13, 0), NoWatch]
		[NoMacCatalyst]
		[NullAllowed, Export ("customOverlayViewController", ArgumentSemantic.Strong)]
		UIViewController CustomOverlayViewController { get; set; }

		[iOS (14, 0), NoTV]
		[MacCatalyst (14, 0)]
		[Export ("showsTimecodes")]
		bool ShowsTimecodes { get; set; }

		[iOS (14, 2)]
		[NoWatch, NoTV, MacCatalyst (15, 0)]
		[Export ("canStartPictureInPictureAutomaticallyFromInline")]
		bool CanStartPictureInPictureAutomaticallyFromInline { get; set; }

		[TV (15, 0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("contextualActions", ArgumentSemantic.Copy)]
		UIAction [] ContextualActions { get; set; }

		[TV (15, 0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("infoViewActions", ArgumentSemantic.Copy)]
		UIAction [] InfoViewActions { get; set; }

		[TV (15, 0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("customInfoViewControllers", ArgumentSemantic.Copy)]
		UIViewController [] CustomInfoViewControllers { get; set; }

		[TV (15, 0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("transportBarCustomMenuItems", ArgumentSemantic.Copy)]
		UIMenuElement [] TransportBarCustomMenuItems { get; set; }

		[TV (15, 0), NoWatch, NoMac, NoiOS, NoMacCatalyst]
		[Export ("transportBarIncludesTitleView")]
		bool TransportBarIncludesTitleView { get; set; }

		[NoWatch, NoTV, MacCatalyst (16, 0), NoMac, iOS (16, 0)]
		[Export ("allowsVideoFrameAnalysis")]
		bool AllowsVideoFrameAnalysis { get; set; }

		[iOS (16, 0), MacCatalyst (16, 0), NoMac, NoWatch, TV (16, 0)]
		[Export ("speeds", ArgumentSemantic.Copy)]
		AVPlaybackSpeed [] Speeds { get; set; }

		[iOS (16, 0), MacCatalyst (16, 0), NoMac, NoWatch, TV (16, 0)]
		[NullAllowed, Export ("selectedSpeed")]
		AVPlaybackSpeed SelectedSpeed { get; }

		[iOS (16, 0), MacCatalyst (16, 0), NoMac, NoWatch, TV (16, 0)]
		[Export ("selectSpeed:")]
		void SelectSpeed (AVPlaybackSpeed speed);
	}

	interface IAVPlayerViewControllerDelegate { }

	[NoMac]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVPlayerViewControllerDelegate {
		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewControllerWillStartPictureInPicture:")]
		void WillStartPictureInPicture (AVPlayerViewController playerViewController);

		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewControllerDidStartPictureInPicture:")]
		void DidStartPictureInPicture (AVPlayerViewController playerViewController);

		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewController:failedToStartPictureInPictureWithError:")]
		void FailedToStartPictureInPicture (AVPlayerViewController playerViewController, NSError error);

		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewControllerWillStopPictureInPicture:")]
		void WillStopPictureInPicture (AVPlayerViewController playerViewController);

		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewControllerDidStopPictureInPicture:")]
		void DidStopPictureInPicture (AVPlayerViewController playerViewController);

		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewControllerShouldAutomaticallyDismissAtPictureInPictureStart:")]
		bool ShouldAutomaticallyDismissAtPictureInPictureStart (AVPlayerViewController playerViewController);

		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewController:restoreUserInterfaceForPictureInPictureStopWithCompletionHandler:")]
		void RestoreUserInterfaceForPictureInPicture (AVPlayerViewController playerViewController, Action<bool> completionHandler);

		[iOS (16, 0)]
		[NoMac]
		[NoMacCatalyst]
		[NoWatch]
		[Export ("playerViewController:didPresentInterstitialTimeRange:")]
		void DidPresentInterstitialTimeRange (AVPlayerViewController playerViewController, AVInterstitialTimeRange interstitial);

		[NoiOS]
		[NoMac]
		[NoMacCatalyst]
		[Export ("playerViewControllerShouldDismiss:")]
		bool ShouldDismiss (AVPlayerViewController playerViewController);

		[NoiOS]
		[NoMac]
		[NoMacCatalyst]
		[Export ("playerViewControllerWillBeginDismissalTransition:")]
		void WillBeginDismissalTransition (AVPlayerViewController playerViewController);

		[NoiOS]
		[NoMac]
		[NoMacCatalyst]
		[Export ("playerViewControllerDidEndDismissalTransition:")]
		void DidEndDismissalTransition (AVPlayerViewController playerViewController);

		[iOS (16, 0)]
		[NoMac]
		[NoWatch]
		[NoMacCatalyst]
		[Export ("playerViewController:willPresentInterstitialTimeRange:")]
		void WillPresentInterstitialTimeRange (AVPlayerViewController playerViewController, AVInterstitialTimeRange interstitial);

		[NoiOS]
		[NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:willResumePlaybackAfterUserNavigatedFromTime:toTime:")]
		void WillResumePlaybackAfterUserNavigatedFromTime (AVPlayerViewController playerViewController, CMTime oldTime, CMTime targetTime);

		[NoiOS]
		[NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:didSelectMediaSelectionOption:inMediaSelectionGroup:")]
		void DidSelectMediaSelectionOption (AVPlayerViewController playerViewController, [NullAllowed] AVMediaSelectionOption mediaSelectionOption, AVMediaSelectionGroup mediaSelectionGroup);

		[NoiOS]
		[NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:didSelectExternalSubtitleOptionLanguage:")]
		void DidSelectExternalSubtitleOptionLanguage (AVPlayerViewController playerViewController, string language);

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:timeToSeekAfterUserNavigatedFromTime:toTime:")]
		CMTime GetTimeToSeekAfterUserNavigated (AVPlayerViewController playerViewController, CMTime oldTime, CMTime targetTime);

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("skipToNextItemForPlayerViewController:")]
		void SkipToNextItem (AVPlayerViewController playerViewController);

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("skipToPreviousItemForPlayerViewController:")]
		void SkipToPreviousItem (AVPlayerViewController playerViewController);

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:shouldPresentContentProposal:")]
		bool ShouldPresentContentProposal (AVPlayerViewController playerViewController, AVContentProposal proposal);

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:didAcceptContentProposal:")]
		void DidAcceptContentProposal (AVPlayerViewController playerViewController, AVContentProposal proposal);

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:didRejectContentProposal:")]
		void DidRejectContentProposal (AVPlayerViewController playerViewController, AVContentProposal proposal);

		[NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:willTransitionToVisibilityOfTransportBar:withAnimationCoordinator:")]
		void WillTransitionToVisibilityOfTransportBar (AVPlayerViewController playerViewController, bool visible, IAVPlayerViewControllerAnimationCoordinator coordinator);

		[iOS (13, 0), NoTV, NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("playerViewController:willBeginFullScreenPresentationWithAnimationCoordinator:"), EventArgs ("AVPlayerViewFullScreenPresentationWillBegin")]
		void WillBeginFullScreenPresentation (AVPlayerViewController playerViewController, IUIViewControllerTransitionCoordinator coordinator);

		[iOS (13, 0), NoTV, NoWatch, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("playerViewController:willEndFullScreenPresentationWithAnimationCoordinator:"), EventArgs ("AVPlayerViewFullScreenPresentationWillEnd")]
		void WillEndFullScreenPresentation (AVPlayerViewController playerViewController, IUIViewControllerTransitionCoordinator coordinator);

		[TV (13, 0), NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("nextChannelInterstitialViewControllerForPlayerViewController:")]
		UIViewController GetNextChannelInterstitialViewController (AVPlayerViewController playerViewController);

		[TV (13, 0), NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:skipToNextChannel:"), EventArgs ("AVPlayerViewSkipToNextChannel")]
		void SkipToNextChannel (AVPlayerViewController playerViewController, Action<bool> completion);

		[TV (13, 0), NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:skipToPreviousChannel:"), EventArgs ("AVPlayerViewSkipToPreviousChannel")]
		void SkipToPreviousChannel (AVPlayerViewController playerViewController, Action<bool> completion);

		[TV (13, 0), NoiOS, NoWatch, NoMac]
		[NoMacCatalyst]
		[Export ("previousChannelInterstitialViewControllerForPlayerViewController:")]
		UIViewController GetPreviousChannelInterstitialViewController (AVPlayerViewController playerViewController);

		[iOS (15, 0), NoTV, NoMac, NoWatch, MacCatalyst (15, 0)]
		[Export ("playerViewController:restoreUserInterfaceForFullScreenExitWithCompletionHandler:")]
		void RestoreUserInterfaceForFullScreenExit (AVPlayerViewController playerViewController, Action<bool> completionHandler);
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVAudioSession))]
	interface AVAudioSession_AVPlaybackRouteSelecting {

		[Async (ResultTypeName = "PreparingRouteSelectionForPlayback")]
		[Export ("prepareRouteSelectionForPlaybackWithCompletionHandler:")]
		void PrepareRouteSelectionForPlayback (Action<bool, AVAudioSessionRouteSelection> completionHandler);
	}

	interface IAVPlayerViewControllerAnimationCoordinator { }

	[NoiOS, NoWatch, NoMac]
	[NoMacCatalyst]
	[Protocol]
	interface AVPlayerViewControllerAnimationCoordinator {

		[Abstract]
		[Export ("addCoordinatedAnimations:completion:")]
		void AddCoordinatedAnimations (Action animations, Action<bool> completion);
	}

	[NoiOS, NoWatch, NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSView))]
	interface AVPlayerView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frameRect);

		[NullAllowed]
		[Export ("player")]
		AVPlayer Player { get; set; }

		[Export ("controlsStyle")]
		AVPlayerViewControlsStyle ControlsStyle { get; set; }

		[NoMacCatalyst]
		[Export ("videoGravity")]
		string VideoGravity { get; set; }

		[NoMacCatalyst]
		[Export ("readyForDisplay")]
		bool ReadyForDisplay { [Bind ("isReadyForDisplay")] get; }

		[NoMacCatalyst]
		[Export ("videoBounds")]
		CGRect VideoBounds { get; }

		[NullAllowed]
		[NoMacCatalyst]
		[Export ("contentOverlayView")]
		NSView ContentOverlayView { get; }

		[NoMacCatalyst]
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

		[NoMacCatalyst]
		[Export ("allowsPictureInPicturePlayback")]
		bool AllowsPictureInPicturePlayback { get; set; }

		[NoMacCatalyst]
		[Wrap ("WeakPictureInPictureDelegate")]
		[NullAllowed]
		IAVPlayerViewPictureInPictureDelegate PictureInPictureDelegate { get; set; }

		[NoMacCatalyst]
		[NullAllowed, Export ("pictureInPictureDelegate", ArgumentSemantic.Weak)]
		NSObject WeakPictureInPictureDelegate { get; set; }

		[NoMacCatalyst]
		[Export ("showsTimecodes")]
		bool ShowsTimecodes { get; set; }

		[Mac (12, 0)]
		[NoMacCatalyst]
		[Wrap ("WeakDelegate")]
		IAVPlayerViewDelegate Delegate { get; set; }

		[Mac (12, 0)]
		[NoMacCatalyst]
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Mac (13, 0), NoWatch, NoiOS, NoMacCatalyst, NoTV]
		[Export ("speeds", ArgumentSemantic.Copy)]
		AVPlaybackSpeed [] Speeds { get; set; }

		[Mac (13, 0), NoWatch, NoiOS, NoMacCatalyst, NoTV]
		[NullAllowed, Export ("selectedSpeed")]
		AVPlaybackSpeed SelectedSpeed { get; }

		[Mac (13, 0), NoWatch, NoiOS, NoMacCatalyst, NoTV]
		[Export ("selectSpeed:")]
		void SelectSpeed (AVPlaybackSpeed speed);

		[NoWatch, NoTV, NoMacCatalyst, NoiOS, Mac (13, 0)]
		[Export ("allowsVideoFrameAnalysis")]
		bool AllowsVideoFrameAnalysis { get; set; }

		[Mac (13, 0), NoWatch, NoiOS, NoMacCatalyst, NoTV]
		[Export ("allowsMagnification")]
		bool AllowsMagnification { get; set; }

		[Mac (13, 0), NoWatch, NoiOS, NoMacCatalyst, NoTV]
		[Export ("magnification")]
		nfloat Magnification { get; set; }

		[Mac (13, 0), NoWatch, NoiOS, NoMacCatalyst, NoTV]
		[Export ("setMagnification:centeredAtPoint:")]
		void SetMagnification (nfloat magnification, CGPoint centeredAtPoint);
	}

	interface IAVPlayerViewPictureInPictureDelegate { }

	[NoiOS, NoWatch, NoTV]
	[NoMacCatalyst]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
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
	[NoMacCatalyst]
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
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface AVCaptureViewDelegate {
		[Abstract]
		[Export ("captureView:startRecordingToFileOutput:")]
		void StartRecording (AVCaptureView captureView, AVCaptureFileOutput fileOutput);
	}

	[iOS (16, 0)]
	[NoMac]
	[NoMacCatalyst]
	[NoWatch]
	[BaseType (typeof (NSObject))]
	interface AVInterstitialTimeRange : NSCopying, NSSecureCoding {
		[Export ("initWithTimeRange:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CMTimeRange timeRange);

		[Export ("timeRange")]
		CMTimeRange TimeRange { get; }
	}

	[NoiOS]
	[NoMac]
	[NoMacCatalyst]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface AVNavigationMarkersGroup {
		[Export ("initWithTitle:timedNavigationMarkers:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string title, AVTimedMetadataGroup [] navigationMarkers);

		[Export ("initWithTitle:dateRangeNavigationMarkers:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string title, AVDateRangeMetadataGroup [] navigationMarkers);

		[NullAllowed, Export ("title")]
		string Title { get; }

		[NullAllowed, Export ("timedNavigationMarkers")]
		AVTimedMetadataGroup [] TimedNavigationMarkers { get; }

		[NullAllowed, Export ("dateRangeNavigationMarkers")]
		AVDateRangeMetadataGroup [] DateRangeNavigationMarkers { get; }
	}

	[NoMac]
	[NoiOS, NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (UIViewController))]
	interface AVContentProposalViewController {
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
	[NoiOS, NoWatch]
	[NoMacCatalyst]
	interface AVKitMetadataIdentifier {

		[Field ("AVKitMetadataIdentifierExternalContentIdentifier")]
		NSString ExternalContentIdentifier { get; }
		[Field ("AVKitMetadataIdentifierExternalUserProfileIdentifier")]
		NSString ExternalUserProfileIdentifier { get; }
		[Field ("AVKitMetadataIdentifierPlaybackProgress")]
		NSString PlaybackProgress { get; }

		[NoMacCatalyst]
		[Field ("AVKitMetadataIdentifierExactStartDate")]
		NSString ExactStartDate { get; }

		[NoMacCatalyst]
		[Field ("AVKitMetadataIdentifierApproximateStartDate")]
		NSString ApproximateStartDate { get; }

		[NoMacCatalyst]
		[Field ("AVKitMetadataIdentifierExactEndDate")]
		NSString ExactEndDate { get; }

		[NoMacCatalyst]
		[Field ("AVKitMetadataIdentifierApproximateEndDate")]
		NSString ApproximateEndDate { get; }

		[NoMacCatalyst]
		[Field ("AVKitMetadataIdentifierServiceIdentifier")]
		NSString ServiceIdentifier { get; }
	}

	[MacCatalyst (13, 1)]
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
		[MacCatalyst (13, 1)]
		[Export ("activeTintColor", ArgumentSemantic.Assign), NullAllowed]
		UIColor ActiveTintColor { get; set; }

		[NoiOS, NoMac, NoWatch, NoMacCatalyst]
		[Export ("routePickerButtonStyle", ArgumentSemantic.Assign)]
		AVRoutePickerViewButtonStyle RoutePickerButtonStyle { get; set; }

		[NoMac]
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("prioritizesVideoDevices")]
		bool PrioritizesVideoDevices { get; set; }

		[NoiOS, NoTV, NoWatch]
		[NoMacCatalyst]
		[Export ("routePickerButtonColorForState:")]
		NSColor GetRoutePickerButtonColor (AVRoutePickerViewButtonState state);

		[NoiOS, NoTV, NoWatch]
		[NoMacCatalyst]
		[Export ("setRoutePickerButtonColor:forState:")]
		void SetRoutePickerButtonColor ([NullAllowed] NSColor color, AVRoutePickerViewButtonState state);

		[NoiOS, NoTV, NoWatch]
		[NoMacCatalyst]
		[Export ("routePickerButtonBordered")]
		bool RoutePickerButtonBordered { [Bind ("isRoutePickerButtonBordered")] get; set; }

		[NoiOS, NoTV, NoWatch]
		[NoMacCatalyst]
		[NullAllowed, Export ("player", ArgumentSemantic.Assign)]
		AVPlayer Player { get; set; }

		[NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0), NoWatch]
		[NullAllowed, Export ("customRoutingController", ArgumentSemantic.Assign)]
		AVCustomRoutingController CustomRoutingController { get; set; }
	}

	[NoiOS, NoMac, NoWatch, NoMacCatalyst]
	[Native]
	public enum AVRoutePickerViewButtonStyle : long {
		System,
		Plain,
		Custom,
	}

	interface IAVRoutePickerViewDelegate { }

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVRoutePickerViewDelegate {

		[Export ("routePickerViewWillBeginPresentingRoutes:")]
		void WillBeginPresentingRoutes (AVRoutePickerView routePickerView);

		[Export ("routePickerViewDidEndPresentingRoutes:")]
		void DidEndPresentingRoutes (AVRoutePickerView routePickerView);
	}

	[TV (11, 2), NoiOS, NoMac, NoWatch]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVDisplayManager {

		[TV (11, 3)]
		[NoMacCatalyst]
		[Field ("AVDisplayManagerModeSwitchStartNotification")]
		[Notification]
		NSString ModeSwitchStartNotification { get; }

		[TV (11, 3)]
		[NoMacCatalyst]
		[Field ("AVDisplayManagerModeSwitchEndNotification")]
		[Notification]
		NSString ModeSwitchEndNotification { get; }

		[TV (11, 3)]
		[NoMacCatalyst]
		[Field ("AVDisplayManagerModeSwitchSettingsChangedNotification")]
		[Notification]
		NSString ModeSwitchSettingsChangedNotification { get; }

		[NullAllowed, Export ("preferredDisplayCriteria", ArgumentSemantic.Copy)]
		AVDisplayCriteria PreferredDisplayCriteria { get; set; }

		[Export ("displayModeSwitchInProgress")]
		bool DisplayModeSwitchInProgress { [Bind ("isDisplayModeSwitchInProgress")] get; }

		[TV (11, 3)]
		[NoMacCatalyst]
		[Export ("displayCriteriaMatchingEnabled")]
		bool DisplayCriteriaMatchingEnabled { [Bind ("isDisplayCriteriaMatchingEnabled")] get; }
	}

	[TV (11, 2), NoiOS, NoMac, NoWatch]
	[NoMacCatalyst]
	[Category]
	[BaseType (typeof (UIWindow))]
	interface UIWindow_AVAdditions {

		[Export ("avDisplayManager")]
		AVDisplayManager GetAVDisplayManager ();
	}

	[NoTV, NoWatch, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (UIViewController))]
	interface AVPictureInPictureVideoCallViewController {
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);
	}

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPictureInPictureControllerContentSource {
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

	interface IAVPictureInPictureSampleBufferPlaybackDelegate { }

	[TV (15, 0), NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface AVPictureInPictureSampleBufferPlaybackDelegate {
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

	interface IAVPlayerViewDelegate { }

	[Mac (12, 0), NoiOS, NoTV, NoMacCatalyst]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface AVPlayerViewDelegate {
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

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[Native]
	public enum AVCaptureViewControlsStyle : long {
		Inline,
		Floating,
		InlineDeviceSelection,
		Default = Inline,
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[NoMacCatalyst]
	[Native]
	public enum AVPlayerViewTrimResult : long {
		OKButton,
		CancelButton,
	}

	[TV (16, 0), NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPlaybackSpeed {
		[Static]
		[Export ("systemDefaultSpeeds")]
		AVPlaybackSpeed [] SystemDefaultSpeeds { get; }

		[Export ("initWithRate:localizedName:")]
		NativeHandle Constructor (float rate, string localizedName);

		[Export ("rate")]
		float Rate { get; }

		[Export ("localizedName")]
		string LocalizedName { get; }

		[Export ("localizedNumericName")]
		string LocalizedNumericName { get; }
	}
}
