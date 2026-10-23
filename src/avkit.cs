//
// avkit.cs: Definitions for AVKit.cs
//
// Copyright 2014-2015 Xamarin Inc
using System.Collections.Generic;

using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreVideo;
using AVFoundation;
using UniformTypeIdentifiers;
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
using UIInteraction = Foundation.NSObjectProtocol;
using UILayoutGuide = Foundation.NSObject;
using UITraitCollection = Foundation.NSObject;
using UIView = AppKit.NSView;
using UIViewController = Foundation.NSObject;
using UIWindow = Foundation.NSObject;
using UIAction = Foundation.NSObject;
using UIMenu = Foundation.NSObject;
using UIMenuElement = Foundation.NSObject;
#endif // !MONOMAC

#if TVOS
using AVCustomRoutingController = Foundation.NSObject;
using AVCustomRoutingEvent = Foundation.NSObject;
using AVCustomRoutingActionItem = Foundation.NSObject;
using AVLegibleMediaOptionsMenuState = Foundation.NSObject;
#else
using AVRouting;
#endif

namespace AVKit {
	[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	enum AVVideoFrameAnalysisType : ulong {
		AVVideoFrameAnalysisTypeNone = 0,
		AVVideoFrameAnalysisTypeDefault = 1 << 0,
		AVVideoFrameAnalysisTypeText = 1 << 1,
		AVVideoFrameAnalysisTypeSubject = 1 << 2,
		AVVideoFrameAnalysisTypeVisualSearch = 1 << 3,
		[NoMac, NoMacCatalyst]
		AVVideoFrameAnalysisTypeMachineReadableCode = 1 << 4,
	}

	/// <summary>Provides video playback in a floating, resizable window on larger devices.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/prerelease/ios/documentation/AVKit/Reference/AVPictureInPictureControllerDelegate_Protocol/index.html#//apple_ref/doc/uid/TP40016161">Apple documentation for <c>AVPictureInPictureController</c></related>
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	[Sealed] // Apple docs: Do not subclass AVPictureInPictureController. Overriding this class’s methods is unsupported and results in undefined behavior.
	interface AVPictureInPictureController {
		[Static]
		[Export ("isPictureInPictureSupported")]
		bool IsPictureInPictureSupported { get; }

		[Export ("initWithPlayerLayer:")]
		NativeHandle Constructor (AVPlayerLayer playerLayer);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithContentSource:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVPictureInPictureControllerContentSource contentSource);

		[Export ("playerLayer")]
		AVPlayerLayer PlayerLayer { get; }

		/// <summary>An instance of the AVKit.IAVPictureInPictureControllerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the AVKit.IAVPictureInPictureControllerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IAVPictureInPictureControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("startPictureInPicture")]
		void StartPictureInPicture ();

		[Export ("stopPictureInPicture")]
		void StopPictureInPicture ();

		/// <summary>Gets a value that tells whether Picture in Picture is currently possible on the device.</summary>
		///         <value>
		///           <see langword="false" /> if another app is running Picture in Picture.</value>
		///         <remarks>To be added.</remarks>
		[Export ("pictureInPicturePossible")]
		bool PictureInPicturePossible { [Bind ("isPictureInPicturePossible")] get; }

		/// <summary>Gets a value that tells whether Picture in Picture is currently displayed.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("pictureInPictureActive")]
		bool PictureInPictureActive { [Bind ("isPictureInPictureActive")] get; }

		/// <summary>Gets a value that tells whether Picture in Picture playback has been paused and moved off the screen by another application.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("pictureInPictureSuspended")]
		bool PictureInPictureSuspended { [Bind ("isPictureInPictureSuspended")] get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("pictureInPictureButtonStartImage")]
		UIImage PictureInPictureButtonStartImage { get; }

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

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("requiresLinearPlayback")]
		bool RequiresLinearPlayback { get; set; }

		[NoMac, NoiOS, MacCatalyst (15, 0)]
		[Export ("canStopPictureInPicture")]
		bool CanStopPictureInPicture { get; }

		[iOS (14, 2)]
		[NoTV, NoMac, MacCatalyst (15, 0)]
		[Export ("canStartPictureInPictureAutomaticallyFromInline")]
		bool CanStartPictureInPictureAutomaticallyFromInline { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("invalidatePlaybackState")]
		void InvalidatePlaybackState ();

		[NullAllowed]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("contentSource", ArgumentSemantic.Strong)]
		AVPictureInPictureControllerContentSource ContentSource { get; set; }
	}

	interface IAVPictureInPictureControllerDelegate { }

	/// <summary>Delegate object providing methods for the application's <see cref="AVPictureInPictureController" /> at the start and stop of picture-in-picture video playback.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AVKit/Reference/AVPictureInPictureControllerDelegate_Protocol/index.html">Apple documentation for <c>AVPictureInPictureControllerDelegate</c></related>
	[TV (14, 0)]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVPictureInPictureControllerDelegate {
		/// <param name="pictureInPictureController">To be added.</param>
		/// <summary>Picture In Picture playback is about to start.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("pictureInPictureControllerWillStartPictureInPicture:")]
		void WillStartPictureInPicture (AVPictureInPictureController pictureInPictureController);

		/// <param name="pictureInPictureController">To be added.</param>
		/// <summary>Picture In Picture playback has started.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("pictureInPictureControllerDidStartPictureInPicture:")]
		void DidStartPictureInPicture (AVPictureInPictureController pictureInPictureController);

		/// <param name="pictureInPictureController">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>Picture In Picture playback failed to start.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("pictureInPictureController:failedToStartPictureInPictureWithError:")]
		void FailedToStartPictureInPicture (AVPictureInPictureController pictureInPictureController, NSError error);

		/// <param name="pictureInPictureController">To be added.</param>
		/// <summary>Picture In Picture playback is about to stop.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("pictureInPictureControllerWillStopPictureInPicture:")]
		void WillStopPictureInPicture (AVPictureInPictureController pictureInPictureController);

		/// <param name="pictureInPictureController">To be added.</param>
		/// <summary>Picture In Picture playback has stopped.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("pictureInPictureControllerDidStopPictureInPicture:")]
		void DidStopPictureInPicture (AVPictureInPictureController pictureInPictureController);

		/// <param name="pictureInPictureController">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>Picture In Picture playback is about to stop. Called to give the app the opportunity to provide a playback user interface by passing <see langword="true" /> to <paramref name="completionHandler" />.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("pictureInPictureController:restoreUserInterfaceForPictureInPictureStopWithCompletionHandler:")]
		void RestoreUserInterfaceForPictureInPicture (AVPictureInPictureController pictureInPictureController, Action<bool> completionHandler);
	}

	/// <include file="../docs/api/AVKit/AVPlayerViewController.xml" path="/Documentation/Docs[@DocId='T:AVKit.AVPlayerViewController']/*" />
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface AVPlayerViewController {
		/// <param name="nibName">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="bundle">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Creates a new <see cref="AVKit.AVPlayerViewController" /> for the specified NIB name and bundle.</summary>
		/// <remarks>To be added.</remarks>
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

		/// <summary>Gets a value that tells whether the first frame of vido is ready to display.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		/// <summary>An instance of the AVKit.IAVPlayerViewControllerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the AVKit.IAVPlayerViewControllerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
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
		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("contentProposalViewController", ArgumentSemantic.Assign)]
		AVContentProposalViewController ContentProposalViewController { get; set; }

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("skippingBehavior", ArgumentSemantic.Assign)]
		AVPlayerViewControllerSkippingBehavior SkippingBehavior { get; set; }

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("skipForwardEnabled")]
		bool SkipForwardEnabled { [Bind ("isSkipForwardEnabled")] get; set; }

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("skipBackwardEnabled")]
		bool SkipBackwardEnabled { [Bind ("isSkipBackwardEnabled")] get; set; }

		// From AVPlayerViewControllerControls category

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("playbackControlsIncludeTransportBar")]
		bool PlaybackControlsIncludeTransportBar { get; set; }

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("playbackControlsIncludeInfoViews")]
		bool PlaybackControlsIncludeInfoViews { get; set; }

		[NullAllowed]
		[NoiOS, NoMac]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'CustomInfoViewControllers' instead.")]
		[NoMacCatalyst]
		[Export ("customInfoViewController", ArgumentSemantic.Assign)]
		UIViewController CustomInfoViewController { get; set; }

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("appliesPreferredDisplayCriteriaAutomatically")]
		bool AppliesPreferredDisplayCriteriaAutomatically { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("pixelBufferAttributes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> PixelBufferAttributes { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[NullAllowed, Export ("customOverlayViewController", ArgumentSemantic.Strong)]
		UIViewController CustomOverlayViewController { get; set; }

		[iOS (14, 0), NoTV]
		[MacCatalyst (14, 0)]
		[Export ("showsTimecodes")]
		bool ShowsTimecodes { get; set; }

		[iOS (14, 2)]
		[NoTV, MacCatalyst (15, 0)]
		[Export ("canStartPictureInPictureAutomaticallyFromInline")]
		bool CanStartPictureInPictureAutomaticallyFromInline { get; set; }

		[TV (15, 0), NoMac, NoiOS, NoMacCatalyst]
		[Export ("contextualActions", ArgumentSemantic.Copy)]
		UIAction [] ContextualActions { get; set; }

		[TV (15, 0), NoMac, NoiOS, NoMacCatalyst]
		[Export ("infoViewActions", ArgumentSemantic.Copy)]
		[NullAllowed]
		UIAction [] InfoViewActions { get; set; }

		[TV (15, 0), NoMac, NoiOS, NoMacCatalyst]
		[Export ("customInfoViewControllers", ArgumentSemantic.Copy)]
		UIViewController [] CustomInfoViewControllers { get; set; }

		[TV (15, 0), NoMac, NoiOS, NoMacCatalyst]
		[Export ("transportBarCustomMenuItems", ArgumentSemantic.Copy)]
		UIMenuElement [] TransportBarCustomMenuItems { get; set; }

		[TV (15, 0), NoMac, NoiOS, NoMacCatalyst]
		[Export ("transportBarIncludesTitleView")]
		bool TransportBarIncludesTitleView { get; set; }

		[NoTV, MacCatalyst (16, 0), NoMac, iOS (16, 0)]
		[Export ("allowsVideoFrameAnalysis")]
		bool AllowsVideoFrameAnalysis { get; set; }

		[iOS (16, 0), MacCatalyst (16, 0), NoMac, TV (16, 0)]
		[Export ("speeds", ArgumentSemantic.Copy)]
		AVPlaybackSpeed [] Speeds { get; set; }

		[iOS (16, 0), MacCatalyst (16, 0), NoMac, TV (16, 0)]
		[NullAllowed, Export ("selectedSpeed")]
		AVPlaybackSpeed SelectedSpeed { get; }

		[iOS (16, 0), MacCatalyst (16, 0), NoMac, TV (16, 0)]
		[Export ("selectSpeed:")]
		void SelectSpeed (AVPlaybackSpeed speed);

		[iOS (17, 0), MacCatalyst (18, 0), NoTV, NoMac]
		[Export ("videoFrameAnalysisTypes")]
		AVVideoFrameAnalysisType VideoFrameAnalysisTypes { get; set; }

		[iOS (17, 0), MacCatalyst (18, 0), NoTV, NoMac]
		[Export ("toggleLookupAction")]
		UIAction ToggleLookupAction { get; }

		[MacCatalyst (26, 0), NoMac, NoTV, iOS (26, 0)]
		[Export ("preferredDisplayDynamicRange", ArgumentSemantic.Assign)]
		AVDisplayDynamicRange PreferredDisplayDynamicRange { get; set; }


		[TV (26, 0), MacCatalyst (26, 0), NoMac, iOS (26, 0)]
		[Static]
		[Export ("mediaCharacteristicsForSupportedCustomMediaSelectionSchemes")]
		string [] MediaCharacteristicsForSupportedCustomMediaSelectionSchemes { get; }
	}

	interface IAVPlayerViewControllerDelegate { }

	/// <summary>Delegate object for the picture-in-picture controller. When overridden, the methods allow the developer to respond to events relating to p-in-p playback.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AVKit/Reference/AVPlayerViewControllerDelegate_Protocol/index.html">Apple documentation for <c>AVPlayerViewControllerDelegate</c></related>
	[NoMac]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface AVPlayerViewControllerDelegate {
		/// <param name="playerViewController">To be added.</param>
		/// <summary>Picture In Picture playback is about to start.</summary>
		/// <remarks>To be added.</remarks>
		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewControllerWillStartPictureInPicture:")]
		void WillStartPictureInPicture (AVPlayerViewController playerViewController);

		/// <param name="playerViewController">To be added.</param>
		/// <summary>Picture In Picture playback has started.</summary>
		/// <remarks>To be added.</remarks>
		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewControllerDidStartPictureInPicture:")]
		void DidStartPictureInPicture (AVPlayerViewController playerViewController);

		/// <param name="playerViewController">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>Picture In Picture playback failed to start.</summary>
		/// <remarks>To be added.</remarks>
		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewController:failedToStartPictureInPictureWithError:")]
		void FailedToStartPictureInPicture (AVPlayerViewController playerViewController, NSError error);

		/// <param name="playerViewController">To be added.</param>
		/// <summary>Picture In Picture playback is about to stop.</summary>
		/// <remarks>To be added.</remarks>
		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewControllerWillStopPictureInPicture:")]
		void WillStopPictureInPicture (AVPlayerViewController playerViewController);

		/// <param name="playerViewController">To be added.</param>
		/// <summary>Picture In Picture playback has stopped.</summary>
		/// <remarks>To be added.</remarks>
		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewControllerDidStopPictureInPicture:")]
		void DidStopPictureInPicture (AVPlayerViewController playerViewController);

		/// <param name="playerViewController">To be added.</param>
		/// <summary>App developers should return <see langword="true" /> to indicate that the player viewer should dismiss when Picture In Picture playback starts, or <see langword="false" /> to prevent this.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewControllerShouldAutomaticallyDismissAtPictureInPictureStart:")]
		bool ShouldAutomaticallyDismissAtPictureInPictureStart (AVPlayerViewController playerViewController);

		/// <param name="playerViewController">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>Picture In Picture playback is about to stop. Called to give the app the opportunity to provide a playback user interface by passing <see langword="true" /> to <paramref name="completionHandler" />.</summary>
		/// <remarks>To be added.</remarks>
		[TV (14, 0)]
		[MacCatalyst (13, 1)]
		[Export ("playerViewController:restoreUserInterfaceForPictureInPictureStopWithCompletionHandler:")]
		void RestoreUserInterfaceForPictureInPicture (AVPlayerViewController playerViewController, Action<bool> completionHandler);

		[iOS (16, 0)]
		[NoMac]
		[NoMacCatalyst]
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

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:timeToSeekAfterUserNavigatedFromTime:toTime:")]
		CMTime GetTimeToSeekAfterUserNavigated (AVPlayerViewController playerViewController, CMTime oldTime, CMTime targetTime);

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("skipToNextItemForPlayerViewController:")]
		void SkipToNextItem (AVPlayerViewController playerViewController);

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("skipToPreviousItemForPlayerViewController:")]
		void SkipToPreviousItem (AVPlayerViewController playerViewController);

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:shouldPresentContentProposal:")]
		bool ShouldPresentContentProposal (AVPlayerViewController playerViewController, AVContentProposal proposal);

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:didAcceptContentProposal:")]
		void DidAcceptContentProposal (AVPlayerViewController playerViewController, AVContentProposal proposal);

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:didRejectContentProposal:")]
		void DidRejectContentProposal (AVPlayerViewController playerViewController, AVContentProposal proposal);

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:willTransitionToVisibilityOfTransportBar:withAnimationCoordinator:")]
		void WillTransitionToVisibilityOfTransportBar (AVPlayerViewController playerViewController, bool visible, IAVPlayerViewControllerAnimationCoordinator coordinator);

		[NoTV, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("playerViewController:willBeginFullScreenPresentationWithAnimationCoordinator:"), EventArgs ("AVPlayerViewFullScreenPresentationWillBegin")]
		void WillBeginFullScreenPresentation (AVPlayerViewController playerViewController, IUIViewControllerTransitionCoordinator coordinator);

		[NoTV, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("playerViewController:willEndFullScreenPresentationWithAnimationCoordinator:"), EventArgs ("AVPlayerViewFullScreenPresentationWillEnd")]
		void WillEndFullScreenPresentation (AVPlayerViewController playerViewController, IUIViewControllerTransitionCoordinator coordinator);

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("nextChannelInterstitialViewControllerForPlayerViewController:")]
		UIViewController GetNextChannelInterstitialViewController (AVPlayerViewController playerViewController);

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:skipToNextChannel:"), EventArgs ("AVPlayerViewSkipToNextChannel")]
		void SkipToNextChannel (AVPlayerViewController playerViewController, Action<bool> completion);

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("playerViewController:skipToPreviousChannel:"), EventArgs ("AVPlayerViewSkipToPreviousChannel")]
		void SkipToPreviousChannel (AVPlayerViewController playerViewController, Action<bool> completion);

		[NoiOS, NoMac]
		[NoMacCatalyst]
		[Export ("previousChannelInterstitialViewControllerForPlayerViewController:")]
		UIViewController GetPreviousChannelInterstitialViewController (AVPlayerViewController playerViewController);

		[iOS (15, 0), NoTV, NoMac, MacCatalyst (15, 0)]
		[Export ("playerViewController:restoreUserInterfaceForFullScreenExitWithCompletionHandler:")]
		void RestoreUserInterfaceForFullScreenExit (AVPlayerViewController playerViewController, Action<bool> completionHandler);
	}

	[NoTV, NoMac]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (AVAudioSession))]
	interface AVAudioSession_AVPlaybackRouteSelecting {

		[Async (ResultTypeName = "PreparingRouteSelectionForPlayback")]
		[Export ("prepareRouteSelectionForPlaybackWithCompletionHandler:")]
		void PrepareRouteSelectionForPlayback (Action<bool, AVAudioSessionRouteSelection> completionHandler);
	}

	interface IAVPlayerViewControllerAnimationCoordinator { }

	[NoiOS, NoMac]
	[NoMacCatalyst]
	[Protocol]
	interface AVPlayerViewControllerAnimationCoordinator {

		[Abstract]
		[Export ("addCoordinatedAnimations:completion:")]
		void AddCoordinatedAnimations (Action animations, Action<bool> completion);
	}

	[NoiOS, NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSView))]
	interface AVPlayerView {
		/// <param name="frameRect">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		/// <param name="chapterNumber">To be added.</param>
		/// <param name="chapterTitle">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
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

		[NoMacCatalyst]
		[Wrap ("WeakDelegate")]
		IAVPlayerViewDelegate Delegate { get; set; }

		[NoMacCatalyst]
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NoiOS, NoMacCatalyst, NoTV]
		[Export ("speeds", ArgumentSemantic.Copy)]
		AVPlaybackSpeed [] Speeds { get; set; }

		[NoiOS, NoMacCatalyst, NoTV]
		[NullAllowed, Export ("selectedSpeed")]
		AVPlaybackSpeed SelectedSpeed { get; }

		[NoiOS, NoMacCatalyst, NoTV]
		[Export ("selectSpeed:")]
		void SelectSpeed (AVPlaybackSpeed speed);

		[NoTV, NoMacCatalyst, NoiOS]
		[Export ("allowsVideoFrameAnalysis")]
		bool AllowsVideoFrameAnalysis { get; set; }

		[NoiOS, NoMacCatalyst, NoTV]
		[Export ("allowsMagnification")]
		bool AllowsMagnification { get; set; }

		[NoiOS, NoMacCatalyst, NoTV]
		[Export ("magnification")]
		nfloat Magnification { get; set; }

		[NoiOS, NoMacCatalyst, NoTV]
		[Export ("setMagnification:centeredAtPoint:")]
		void SetMagnification (nfloat magnification, CGPoint centeredAtPoint);

		[Export ("videoFrameAnalysisTypes")]
		AVVideoFrameAnalysisType VideoFrameAnalysisTypes { get; set; }

		[Mac (26, 0)]
		[Export ("preferredDisplayDynamicRange", ArgumentSemantic.Assign)]
		AVDisplayDynamicRange PreferredDisplayDynamicRange { get; set; }
	}

	interface IAVPlayerViewPictureInPictureDelegate { }

	[NoiOS, NoTV]
	[NoMacCatalyst]
	[Protocol, Model]
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

	[NoiOS, NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSView))]
	interface AVCaptureView {
		/// <param name="frameRect">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
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

	[NoiOS, NoTV]
	[Protocol, Model]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface AVCaptureViewDelegate {
		/// <param name="captureView">To be added.</param>
		/// <param name="fileOutput">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("captureView:startRecordingToFileOutput:")]
		void StartRecording (AVCaptureView captureView, AVCaptureFileOutput fileOutput);
	}

	[iOS (16, 0)]
	[NoMac]
	[NoMacCatalyst]
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
	[NoiOS]
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
	[NoiOS]
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

		/// <param name="frame">Frame used by the view, expressed in iOS points.</param>
		/// <summary>Initializes the AVRoutePickerView with the specified frame.</summary>
		/// <remarks>
		///           <para>This constructor is used to programmatically create a new instance of AVRoutePickerView with the specified dimension in the frame.   The object will only be displayed once it has been added to a view hierarchy by calling AddSubview in a containing view.</para>
		///           <para>This constructor is not invoked when deserializing objects from storyboards or XIB files; instead, the constructor that takes an NSCoder parameter is invoked.</para>
		///         </remarks>
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		/// <summary>An instance of the AVKit.IAVRoutePickerViewDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the AVKit.IAVRoutePickerViewDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate", IsVirtual = true)]
		[NullAllowed]
		IAVRoutePickerViewDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("activeTintColor", ArgumentSemantic.Assign), NullAllowed]
		UIColor ActiveTintColor { get; set; }

		[NoiOS, NoMac, NoMacCatalyst]
		[Export ("routePickerButtonStyle", ArgumentSemantic.Assign)]
		AVRoutePickerViewButtonStyle RoutePickerButtonStyle { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("prioritizesVideoDevices")]
		bool PrioritizesVideoDevices { get; set; }

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("routePickerButtonColorForState:")]
		NSColor GetRoutePickerButtonColor (AVRoutePickerViewButtonState state);

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("setRoutePickerButtonColor:forState:")]
		void SetRoutePickerButtonColor ([NullAllowed] NSColor color, AVRoutePickerViewButtonState state);

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Export ("routePickerButtonBordered")]
		bool RoutePickerButtonBordered { [Bind ("isRoutePickerButtonBordered")] get; set; }

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[NullAllowed, Export ("player", ArgumentSemantic.Assign)]
		AVPlayer Player { get; set; }

		[NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
		[NullAllowed, Export ("customRoutingController", ArgumentSemantic.Assign)]
		AVCustomRoutingController CustomRoutingController { get; set; }
	}

	[NoiOS, NoMac, NoMacCatalyst]
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

		/// <param name="routePickerView">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("routePickerViewWillBeginPresentingRoutes:")]
		void WillBeginPresentingRoutes (AVRoutePickerView routePickerView);

		/// <param name="routePickerView">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("routePickerViewDidEndPresentingRoutes:")]
		void DidEndPresentingRoutes (AVRoutePickerView routePickerView);
	}

	[NoiOS, NoMac]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVDisplayManager {

		[NoMacCatalyst]
		[Field ("AVDisplayManagerModeSwitchStartNotification")]
		[Notification]
		NSString ModeSwitchStartNotification { get; }

		[NoMacCatalyst]
		[Field ("AVDisplayManagerModeSwitchEndNotification")]
		[Notification]
		NSString ModeSwitchEndNotification { get; }

		[NoMacCatalyst]
		[Field ("AVDisplayManagerModeSwitchSettingsChangedNotification")]
		[Notification]
		NSString ModeSwitchSettingsChangedNotification { get; }

		[NullAllowed, Export ("preferredDisplayCriteria", ArgumentSemantic.Copy)]
		AVDisplayCriteria PreferredDisplayCriteria { get; set; }

		[Export ("displayModeSwitchInProgress")]
		bool DisplayModeSwitchInProgress { [Bind ("isDisplayModeSwitchInProgress")] get; }

		[NoMacCatalyst]
		[Export ("displayCriteriaMatchingEnabled")]
		bool DisplayCriteriaMatchingEnabled { [Bind ("isDisplayCriteriaMatchingEnabled")] get; }
	}

	[NoiOS, NoMac]
	[NoMacCatalyst]
	[Category]
	[BaseType (typeof (UIWindow))]
	interface UIWindow_AVAdditions {

		[Export ("avDisplayManager")]
		AVDisplayManager GetAVDisplayManager ();
	}

	[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (UIViewController))]
	interface AVPictureInPictureVideoCallViewController {
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPictureInPictureControllerContentSource {
		[Export ("initWithPlayerLayer:")]
		NativeHandle Constructor (AVPlayerLayer playerLayer);

		[NullAllowed, Export ("playerLayer")]
		AVPlayerLayer PlayerLayer { get; }

		// interface AVPictureInPictureControllerContentSource_VideoCallSupport
		[NoTV, NoMac]
		[Export ("initWithActiveVideoCallSourceView:contentViewController:")]
		NativeHandle Constructor (UIView sourceView, AVPictureInPictureVideoCallViewController contentViewController);

		[NullAllowed]
		[NoTV, NoMac]
		[Export ("activeVideoCallSourceView", ArgumentSemantic.Weak)]
		UIView ActiveVideoCallSourceView { get; }

		[NoTV, NoMac]
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

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol, Model]
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

	[NoiOS, NoTV, NoMacCatalyst]
	[Protocol, Model]
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
	[NoMacCatalyst]
	[Native]
	public enum AVCaptureViewControlsStyle : long {
		/// <summary>To be added.</summary>
		Inline,
		/// <summary>To be added.</summary>
		Floating,
		/// <summary>To be added.</summary>
		InlineDeviceSelection,
		/// <summary>To be added.</summary>
		Default = Inline,
	}

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	[Native]
	public enum AVPlayerViewTrimResult : long {
		/// <summary>To be added.</summary>
		OKButton,
		/// <summary>To be added.</summary>
		CancelButton,
	}

	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
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

	[iOS (17, 2), NoMac, MacCatalyst (17, 2), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureEvent {
		[Export ("phase")]
		AVCaptureEventPhase Phase { get; }

		[NoTV, NoMacCatalyst, NoMac, iOS (26, 0)]
		[Export ("playSound:")]
		bool PlaySound (AVCaptureEventSound sound);

		[NoTV, NoMacCatalyst, NoMac, iOS (26, 0)]
		[Export ("shouldPlaySound")]
		bool ShouldPlaySound { get; }
	}

	[iOS (17, 2), NoMac, MacCatalyst (17, 2), NoTV]
	[Native]
	public enum AVCaptureEventPhase : ulong {
		Began,
		Ended,
		Cancelled,
	}

	[iOS (17, 2), NoMac, MacCatalyst (17, 2), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureEventInteraction : UIInteraction {
		[Export ("initWithEventHandler:")]
		NativeHandle Constructor (Action<AVCaptureEvent> handler);

		[Export ("initWithPrimaryEventHandler:secondaryEventHandler:")]
		NativeHandle Constructor (Action<AVCaptureEvent> primaryHandler, Action<AVCaptureEvent> secondaryHandler);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NoTV, NoMacCatalyst, NoMac, iOS (26, 0)]
		[Static]
		[Export ("defaultCaptureSoundDisabled")]
		bool DefaultCaptureSoundDisabled { get; set; }
	}

	[TV (17, 0), NoMac, NoiOS, NoMacCatalyst]
	[BaseType (typeof (UIViewController))]
	interface AVContinuityDevicePickerViewController {
		[Static]
		[Export ("supported")]
		bool Supported { [Bind ("isSupported")] get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IAVContinuityDevicePickerViewControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

	[TV (17, 0), NoMac, NoiOS, NoMacCatalyst]
	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	[BaseType (typeof (NSObject))]
	interface AVContinuityDevicePickerViewControllerDelegate {
		[Export ("continuityDevicePickerWillBeginPresenting:")]
		void WillBeginPresenting (AVContinuityDevicePickerViewController pickerViewController);

		[Export ("continuityDevicePicker:didConnectDevice:")]
		void DidConnectDevice (AVContinuityDevicePickerViewController pickerViewController, AVContinuityDevice device);

		[Export ("continuityDevicePickerDidCancel:")]
		void DidCancel (AVContinuityDevicePickerViewController pickerViewController);

		[Export ("continuityDevicePickerDidEndPresenting:")]
		void DidEndPresenting (AVContinuityDevicePickerViewController pickerViewController);
	}
	interface IAVContinuityDevicePickerViewControllerDelegate { }

	[NoTV, NoMacCatalyst, NoMac, iOS (26, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureEventSound {
		/// <summary>Create a new <see cref="AVCaptureEventSound" /> instance for the specified <paramref name="url" />.</summary>
		/// <param name="url">The url for a local file from within the app bundle.</param>
		/// <param name="error">The error in case of failure.</param>
		/// <returns>A new <see cref="AVCaptureEventSound" /> instance if successful, otherwise <paramref name="error" /> will be an error for the failure.</returns>
		[Export ("initWithURL:error:")]
		[FactoryMethod]
		[return: NullAllowed]
		NativeHandle Constructor (NSUrl url, [NullAllowed] out NSError error);

		[Static]
		[Export ("cameraShutterSound")]
		AVCaptureEventSound CameraShutterSound { get; }

		[Static]
		[Export ("beginVideoRecordingSound")]
		AVCaptureEventSound BeginVideoRecordingSound { get; }

		[Static]
		[Export ("endVideoRecordingSound")]
		AVCaptureEventSound EndVideoRecordingSound { get; }
	}

	interface IAVInputPickerInteractionDelegate { }

	[NoTV, NoMacCatalyst, NoMac, iOS (26, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	[BaseType (typeof (NSObject))]
	interface AVInputPickerInteractionDelegate {
		[Export ("inputPickerInteractionWillBeginPresenting:")]
		void WillBeginPresenting (AVInputPickerInteraction inputPickerInteraction);

		[Export ("inputPickerInteractionDidEndPresenting:")]
		void DidEndPresenting (AVInputPickerInteraction inputPickerInteraction);

		[Export ("inputPickerInteractionWillBeginDismissing:")]
		void WillBeginDismissing (AVInputPickerInteraction inputPickerInteraction);

		[Export ("inputPickerInteractionDidEndDismissing:")]
		void DidEndDismissing (AVInputPickerInteraction inputPickerInteraction);
	}

	[NoTV, NoMacCatalyst, NoMac, iOS (26, 0)]
	[BaseType (typeof (NSObject))]
	interface AVInputPickerInteraction : UIInteraction {
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IAVInputPickerInteractionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("presented")]
		bool Presented { [Bind ("isPresented")] get; }

		[Export ("audioSession", ArgumentSemantic.Strong)]
		AVAudioSession AudioSession { get; set; }

		[Export ("present")]
		void Present ();

		[Export ("dismiss")]
		void Dismiss ();

		[Export ("initWithAudioSession:")]
		NativeHandle Constructor ([NullAllowed] AVAudioSession audioSession);
	}

	[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum AVDisplayDynamicRange : long {
		Automatic = 0,
		Standard = 1,
		ConstrainedHigh = 2,
		High = 3,
	}

	interface IAVLegibleMediaOptionsMenuControllerDelegate { }

	[NoTV, Mac (26, 4), iOS (26, 4), MacCatalyst (26, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVLegibleMediaOptionsMenuController {
		[Export ("initWithPlayer:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AVPlayer player);

		[Export ("menuWithContents:")]
		[return: NullAllowed]
		UIMenu BuildMenu (AVLegibleMediaOptionsMenuContents contents);

		[Export ("player", ArgumentSemantic.Assign)]
		AVPlayer Player { get; set; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IAVLegibleMediaOptionsMenuControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("menuState")]
		AVLegibleMediaOptionsMenuState MenuState { get; }
	}

	[NoTV, Mac (26, 4), iOS (26, 4), MacCatalyst (26, 4)]
	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	[BaseType (typeof (NSObject))]
	interface AVLegibleMediaOptionsMenuControllerDelegate {
		[Export ("legibleMenuController:didChangeMenuState:")]
		void DidChangeMenuState (AVLegibleMediaOptionsMenuController menuController, AVLegibleMediaOptionsMenuState state);

		[Export ("legibleMenuController:didRequestCaptionPreviewForProfileID:")]
		void DidRequestCaptionPreviewForProfileId (AVLegibleMediaOptionsMenuController menuController, string profileId);

		[Export ("legibleMenuControllerDidRequestStoppingSubtitleCaptionPreview:")]
		void DidRequestStoppingSubtitleCaptionPreview (AVLegibleMediaOptionsMenuController menuController);
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPlaybackUserInterfaceTimelineSegment : NSCopying, NSSecureCoding {
		[Export ("timeRange")]
		CMTimeRange TimeRange { get; }

		[Export ("segmentType")]
		AVPlaybackUserInterfaceTimelineSegmentType SegmentType { get; }

		[Export ("marked")]
		bool Marked { [Bind ("isMarked")] get; }

		[Export ("requiresLinearPlayback")]
		bool RequiresLinearPlayback { get; }

		[NullAllowed, Export ("identifier")]
		string Identifier { get; }

		[Export ("initWithTimeRange:segmentType:marked:requiresLinearPlayback:identifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CMTimeRange timeRange, AVPlaybackUserInterfaceTimelineSegmentType segmentType, bool marked, bool requiresLinearPlayback, [NullAllowed] string identifier);
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPlaybackUserInterfaceMediaSelectionOption : NSCopying, NSSecureCoding {
		[Export ("displayName")]
		string DisplayName { get; }

		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("extendedLanguageTag")]
		string ExtendedLanguageTag { get; }

		[BindAs (typeof (AVMediaCharacteristics []))]
		[Export ("mediaCharacteristics", ArgumentSemantic.Copy)]
		NSString [] MediaCharacteristics { get; }

		[Export ("initWithDisplayName:identifier:extendedLanguageTag:mediaCharacteristics:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string displayName, string identifier, [NullAllowed] string extendedLanguageTag, [BindAs (typeof (AVMediaCharacteristics []))] NSString [] mediaCharacteristics);
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPlaybackUserInterfaceContentArtwork : NSCopying, NSSecureCoding {
		[Export ("size")]
		CGSize Size { get; }

		[Static]
		[Export ("artworkWithURL:contentType:size:")]
		AVPlaybackUserInterfaceContentUrlArtwork FromUrl (NSUrl url, UTType contentType, CGSize size);
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (AVPlaybackUserInterfaceContentArtwork), Name = "AVPlaybackUserInterfaceContentURLArtwork")]
	[DisableDefaultCtor]
	interface AVPlaybackUserInterfaceContentUrlArtwork {
		[Export ("url", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("contentType", ArgumentSemantic.Copy)]
		UTType ContentType { get; }
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPlaybackUserInterfaceContentVideoProperties : NSCopying, NSSecureCoding {
		[Export ("presentationSize")]
		CGSize PresentationSize { get; }

		[Export ("initWithPresentationSize:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGSize presentationSize);
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (NSObject))]
	interface AVPlaybackUserInterfaceContentMetadataTemplate : NSCopying, NSSecureCoding {
		[NullAllowed, Export ("videoProperties", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceContentVideoProperties VideoProperties { get; set; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; set; }

		[Export ("artworkRepresentations", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceContentArtwork [] ArtworkRepresentations { get; set; }
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPlaybackUserInterfaceContentMetadata : NSCopying, NSSecureCoding {
		[NullAllowed, Export ("videoProperties", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceContentVideoProperties VideoProperties { get; }

		[NullAllowed, Export ("title")]
		string Title { get; }

		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; }

		[Export ("artworkRepresentations", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceContentArtwork [] ArtworkRepresentations { get; }

		[Export ("initWithVideoProperties:title:subtitle:artworkRepresentations:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] AVPlaybackUserInterfaceContentVideoProperties videoProperties, [NullAllowed] string title, [NullAllowed] string subtitle, AVPlaybackUserInterfaceContentArtwork [] artworkRepresentations);

		[Export ("initWithTemplate:")]
		NativeHandle Constructor ([NullAllowed] AVPlaybackUserInterfaceContentMetadataTemplate metadataTemplate);
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVPlaybackUserInterfacePlaybackPosition : NSCopying, NSSecureCoding {
		[Export ("position")]
		CMTime Position { get; }

		[Export ("hostTime")]
		CMTime HostTime { get; }

		[Export ("rate")]
		float Rate { get; }

		[Export ("initWithPosition:hostTime:rate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CMTime position, CMTime hostTime, float rate);
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface AVPlaybackUserInterfacePlaybackControllable {
		[Abstract]
		[Export ("ready")]
		bool Ready { [Bind ("isReady")] get; }

		[Abstract]
		[Export ("playing")]
		bool Playing { [Bind ("isPlaying")] get; set; }

		[Abstract]
		[Export ("buffering")]
		bool Buffering { [Bind ("isBuffering")] get; }

		[Abstract]
		[Export ("playbackSpeed")]
		float PlaybackSpeed { get; set; }

		[Abstract]
		[Export ("scanSpeed")]
		float ScanSpeed { get; set; }

		[Abstract]
		[Export ("state", ArgumentSemantic.Assign)]
		AVPlaybackUserInterfacePlaybackState State { get; set; }

		[Abstract]
		[Export ("supportedSeekCapabilities")]
		AVPlaybackUserInterfaceSeekCapabilities SupportedSeekCapabilities { get; }

		[Abstract]
		[Export ("containsLiveStreamingContent")]
		bool ContainsLiveStreamingContent { get; }

		[Abstract]
		[NullAllowed, Export ("error", ArgumentSemantic.Strong)]
		NSError Error { get; }
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface AVPlaybackUserInterfaceTimeControllable {
		[Abstract]
		[Export ("timeRange")]
		CMTimeRange TimeRange { get; }

		[Abstract]
		[Export ("playbackPosition", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfacePlaybackPosition PlaybackPosition { get; }

		[Abstract]
		[Export ("seekToPosition:tolerance:")]
		void Seek (CMTime position, CMTime tolerance);

		[Abstract]
		[Export ("segments", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceTimelineSegment [] Segments { get; }

		[Abstract]
		[Export ("currentSegment", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceTimelineSegment CurrentSegment { get; }

		[Abstract]
		[NullAllowed, Export ("seekableTimeRanges", ArgumentSemantic.Copy)]
		NSValue [] SeekableTimeRanges { get; }
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface AVPlaybackUserInterfaceMediaSelectionControllable {
		[Abstract]
		[NullAllowed, Export ("currentAudioOption", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceMediaSelectionOption CurrentAudioOption { get; set; }

		[Abstract]
		[NullAllowed, Export ("currentAudioDescriptionOption", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceMediaSelectionOption CurrentAudioDescriptionOption { get; set; }

		[Abstract]
		[NullAllowed, Export ("currentLegibleOption", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceMediaSelectionOption CurrentLegibleOption { get; set; }

		[Abstract]
		[Export ("audioOptions", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceMediaSelectionOption [] AudioOptions { get; }

		[Abstract]
		[Export ("audioDescriptionOptions", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceMediaSelectionOption [] AudioDescriptionOptions { get; }

		[Abstract]
		[Export ("legibleOptions", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceMediaSelectionOption [] LegibleOptions { get; }
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface AVPlaybackUserInterfaceVolumeControllable {
		[Abstract]
		[Export ("hasAudio")]
		bool HasAudio { get; }

		[Abstract]
		[Export ("muted")]
		bool Muted { [Bind ("isMuted")] get; set; }

		[Abstract]
		[Export ("volume")]
		float Volume { get; set; }
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface AVPlaybackUserInterfaceMetadataProviding {
		[Abstract]
		[Export ("metadata", ArgumentSemantic.Copy)]
		AVPlaybackUserInterfaceContentMetadata Metadata { get; }
	}

	[NoTV, NoMac, iOS (27, 0), MacCatalyst (27, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface AVPlaybackUserInterfaceControllable :
		AVPlaybackUserInterfaceTimeControllable,
		AVPlaybackUserInterfacePlaybackControllable,
		AVPlaybackUserInterfaceMediaSelectionControllable,
		AVPlaybackUserInterfaceVolumeControllable,
		AVPlaybackUserInterfaceMetadataProviding {
	}

	[NoTV, NoMac, NoMacCatalyst, iOS (27, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureDeviceDescriptor {
		[BindAs (typeof (AVCaptureDeviceType))]
		[Export ("deviceType")]
		NSString DeviceType { get; }

		[Export ("mediaTypes", ArgumentSemantic.Copy)]
		NSSet<NSString> WeakMediaTypes { get; }

		[Wrap ("WeakMediaTypes.ToHashSet (v => AVMediaTypesExtensions.GetValue (v))")]
		HashSet<AVMediaTypes> MediaTypes { get; }

		[Export ("position")]
		AVCaptureDevicePosition Position { get; }

		[Export ("uniqueID")]
		string UniqueId { get; }

		[Export ("localizedName")]
		string LocalizedName { get; }
	}

	[NoTV, NoMac, NoMacCatalyst, iOS (27, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureDeviceDirectionMap {
		[Export ("forwardFacingDeviceDescriptors", ArgumentSemantic.Copy)]
		AVCaptureDeviceDescriptor [] ForwardFacingDeviceDescriptors { get; }

		[Export ("backwardFacingDeviceDescriptors", ArgumentSemantic.Copy)]
		AVCaptureDeviceDescriptor [] BackwardFacingDeviceDescriptors { get; }
	}

	[NoTV, NoMac, NoMacCatalyst, iOS (27, 1)]
	delegate void AVCaptureDeviceDirectionCoordinatorChangeHandler (AVCaptureDeviceDirectionMap deviceDirections);

	[NoTV, NoMac, NoMacCatalyst, iOS (27, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AVCaptureDeviceDirectionCoordinator {
		/// <summary>Creates a coordinator that tracks device directions relative to a view.</summary>
		/// <param name="view">The view used to determine camera direction.</param>
		/// <param name="deviceTypes">The device types to observe.</param>
		/// <param name="changeHandler">An optional callback for direction changes, delivered on the main queue.</param>
		/// <remarks>Create this coordinator on the main thread.</remarks>
		[Export ("initWithView:deviceTypes:changeHandler:")]
		NativeHandle Constructor (UIView view, [BindAs (typeof (AVCaptureDeviceType []))] NSString [] deviceTypes, [NullAllowed] AVCaptureDeviceDirectionCoordinatorChangeHandler changeHandler);

		[Export ("deviceDirections")]
		AVCaptureDeviceDirectionMap DeviceDirections { get; }
	}
}
