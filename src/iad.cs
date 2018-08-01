//
// AdLib bindings.
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2010, Novell, Inc.
// Copyright 2011-2014 Xamarin Inc. All rights reserved.
//
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using UIKit;
using MediaPlayer;
using System;
using System.ComponentModel;
using AVKit;

namespace iAd {

	[Deprecated (PlatformName.iOS, 10, 0)]
	[BaseType (typeof (UIView), Delegates=new string [] {"WeakDelegate"}, Events=new Type [] {typeof (ADBannerViewDelegate)})]
	interface ADBannerView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);
		
		[iOS (6,0)]
		[Export ("adType")]
		ADAdType AdType { get;  }

		[iOS (6,0)]
		[Export ("initWithAdType:")]
		IntPtr Constructor (ADAdType type);

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set;  }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		ADBannerViewDelegate Delegate { get; set; }
		
		[Export ("bannerLoaded")]
		bool BannerLoaded { [Bind ("isBannerLoaded")] get;  }

		[NullAllowed] // by default this property is null
		[Export ("advertisingSection", ArgumentSemantic.Copy)]
		string AdvertisingSection { get; set;  }

		[Export ("bannerViewActionInProgress")]
		bool BannerViewActionInProgress { [Bind ("isBannerViewActionInProgress")] get;  }

		[Availability (Deprecated = Platform.iOS_6_0)]
		[NullAllowed] // by default this property is null
		[Export ("requiredContentSizeIdentifiers", ArgumentSemantic.Copy)]
		NSSet RequiredContentSizeIdentifiers { get; set;  }

		[Export ("cancelBannerViewAction")]
		void CancelBannerViewAction ();

		[Availability (Deprecated = Platform.iOS_6_0)]
		[NullAllowed] // by default this property is null
		[Export ("currentContentSizeIdentifier", ArgumentSemantic.Copy)]
		string CurrentContentSizeIdentifier { get; set; }

		[Availability (Deprecated = Platform.iOS_6_0)]
		[Static, Export ("sizeFromBannerContentSizeIdentifier:")]
		CGSize SizeFromContentSizeIdentifier (string sizeIdentifier);

#if !XAMCORE_3_0
		[Availability (Deprecated = Platform.iOS_4_2)]
		[Field ("ADBannerContentSizeIdentifier320x50")]
		NSString SizeIdentifier320x50 { get; }

		[Availability (Deprecated = Platform.iOS_4_2)]
		[Field ("ADBannerContentSizeIdentifier480x32")]
		NSString SizeIdentifier480x32 { get; }
#endif

		[Availability (Deprecated = Platform.iOS_6_0)]
		[Field ("ADBannerContentSizeIdentifierLandscape")]
		NSString SizeIdentifierLandscape { get; }

		[Availability (Deprecated = Platform.iOS_6_0)]
		[Field ("ADBannerContentSizeIdentifierPortrait")]
		NSString SizeIdentifierPortrait { get; }
	}

	[Deprecated (PlatformName.iOS, 10, 0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface ADBannerViewDelegate {
		[Export ("bannerViewDidLoadAd:")]
		void AdLoaded (ADBannerView banner);

		[Export ("bannerView:didFailToReceiveAdWithError:"), EventArgs ("AdError")]
		void FailedToReceiveAd (ADBannerView banner, NSError error);

		[Export ("bannerViewActionShouldBegin:willLeaveApplication:"), DelegateName ("AdAction"), DefaultValue (true)]
		bool ActionShouldBegin (ADBannerView banner, bool willLeaveApplication);

		[Export ("bannerViewActionDidFinish:")]
		void ActionFinished (ADBannerView banner);

		[Export ("bannerViewWillLoadAd:"), EventArgs ("EventArgs", true, true)]
		void WillLoad (ADBannerView bannerView);
	}

	[Deprecated (PlatformName.iOS, 10, 0)]
	[BaseType (typeof (NSObject), Delegates=new string [] {"WeakDelegate"}, Events=new Type [] {typeof (ADInterstitialAdDelegate)})]
	interface ADInterstitialAd {
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		ADInterstitialAdDelegate Delegate { get; set;  }

		[Export ("loaded")]
		bool Loaded { [Bind ("isLoaded")] get;  }

		[Export ("actionInProgress")]
		bool ActionInProgress { [Bind ("isActionInProgress")] get;  }

		[Export ("cancelAction")]
		void CancelAction ();

		[Export ("presentInView:")]
		bool PresentInView (UIView containerView);

		[Export ("presentFromViewController:")]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use extension method 'UIViewController.RequestInterstitialAdPresentation' instead.")]
		void PresentFromViewController (UIViewController viewController);
	}

	[Deprecated (PlatformName.iOS, 10, 0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface ADInterstitialAdDelegate {
		[Abstract]
		[Export ("interstitialAdDidUnload:")]
		void AdUnloaded (ADInterstitialAd interstitialAd);

		[Abstract]
		[Export ("interstitialAd:didFailWithError:"), EventArgs ("ADError")]
		void FailedToReceiveAd (ADInterstitialAd interstitialAd, NSError error);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("interstitialAdDidLoad:")]
		void AdLoaded (ADInterstitialAd interstitialAd);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("interstitialAdActionShouldBegin:willLeaveApplication:"), DelegateName ("ADPredicate"), DefaultValue (true)]
		bool ActionShouldBegin (ADInterstitialAd interstitialAd, bool willLeaveApplication);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("interstitialAdActionDidFinish:")]
		void ActionFinished (ADInterstitialAd interstitialAd);

		[Export ("interstitialAdWillLoad:"), EventArgs ("EventArgs", true, true)]
		void WillLoad (ADInterstitialAd interstitialAd);
	}

	[Category (allowStaticMembers: true)] // Classic isn't internal so we need this
	[BaseType (typeof (MPMoviePlayerController))]
	[Deprecated (PlatformName.iOS, 9,0, message: "Use 'iAdPreroll_AVPlayerViewController' instead.")]
	[Obsoleted (PlatformName.iOS, 12,0)] // header removed in xcode10 beta5
	partial interface IAdPreroll {

#if XAMCORE_2_0
		[Internal]
#else
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
#endif
		[iOS (7,0), Static, Export ("preparePrerollAds")]
		void PreparePrerollAds ();

		[iOS (7,0), Export ("playPrerollAdWithCompletionHandler:")]
#if XAMCORE_2_0
		void PlayPrerollAd (Action<NSError> completionHandler);
#else
		void PlayPrerollAd (PlayPrerollAdCompletionHandler completionHandler);
#endif

		[iOS (8,0), Export ("cancelPreroll")]
		void CancelPreroll ();
	}

#if !XAMCORE_2_0
	delegate void PlayPrerollAdCompletionHandler (NSError error);
#endif

	[Deprecated (PlatformName.iOS, 10, 0)]
	[Category (allowStaticMembers: true)] // Classic isn't internal so we need this
	[BaseType (typeof (UIViewController))]
	partial interface IAdAdditions {

#if XAMCORE_2_0
		[Internal]
#else
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
#endif
		[iOS (7,0), Static, Export ("prepareInterstitialAds")]
		void PrepareInterstitialAds ();

		[iOS (7,0), Export ("interstitialPresentationPolicy")]
		ADInterstitialPresentationPolicy GetInterstitialPresentationPolicy ();
		
		[iOS (7,0), Export ("setInterstitialPresentationPolicy:")]
		void SetInterstitialPresentationPolicy (ADInterstitialPresentationPolicy policy);

		[iOS (7,0), Export ("canDisplayBannerAds")]
		bool GetCanDisplayBannerAds ();

		[iOS (7,0), Export ("setCanDisplayBannerAds:")]
		void SetCanDisplayBannerAds (bool value);

		[iOS (7,0), Export ("originalContentView")]
		[NullAllowed]
		UIView GetOriginalContentView ();

		[iOS (7,0), Export ("isPresentingFullScreenAd")]
		bool PresentingFullScreenAd ();

		[iOS (7,0), Export ("isDisplayingBannerAd")]
		bool DisplayingBannerAd ();

		[iOS (7,0), Export ("requestInterstitialAdPresentation")]
		bool RequestInterstitialAdPresentation ();

		[iOS (7,0), Export ("shouldPresentInterstitialAd")]
		bool ShouldPresentInterstitialAd ();
	}

	delegate void ADConversionDetails ([NullAllowed] NSDate appPurchaseDate, [NullAllowed] NSDate iAdImpressionDate);
	
	[iOS (7,1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ADClient {
		[Static]
		[Export ("sharedClient")]
		ADClient SharedClient { get; }

		[Availability (Introduced = Platform.iOS_7_1, Deprecated = Platform.iOS_9_0, Message = "Replaced by 'RequestAttributionDetails'.")]
		[Export ("determineAppInstallationAttributionWithCompletionHandler:")]
		void DetermineAppInstallationAttribution (AttributedToiAdCompletionHandler completionHandler);

		[Availability (Introduced = Platform.iOS_8_0, Deprecated = Platform.iOS_9_0, Message = "Replaced by 'RequestAttributionDetails'.")]
		[Export ("lookupAdConversionDetails:")]
		[Async (ResultTypeName="ADClientConversionDetailsResult")]
		void LookupAdConversionDetails (ADConversionDetails onCompleted);

		[iOS (8,0)]
		[Export ("addClientToSegments:replaceExisting:")]
		void AddClientToSegments ([NullAllowed] string [] segmentIdentifiers, bool replaceExisting);

		[iOS (9,0)]
		[Export ("requestAttributionDetailsWithBlock:")]
		[Async]
		void RequestAttributionDetails (Action<NSDictionary, NSError> completionHandler);

#if !XAMCORE_4_0
		[iOS (9,0)]
		[Field ("ADClientErrorDomain")]
		NSString ErrorDomain { get; }
#endif
	}

	delegate void AttributedToiAdCompletionHandler (bool attributedToiAd);

	[Category (allowStaticMembers: true)] // Classic isn't internal so we need this
	[BaseType (typeof (AVPlayerViewController))]
	interface iAdPreroll_AVPlayerViewController {
		[iOS (8,0)]
		[Static, Export ("preparePrerollAds")]
#if XAMCORE_2_0
		[Internal]
#else
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
#endif
		void PreparePrerollAds ();

		[iOS (8,0)]
		[Export ("playPrerollAdWithCompletionHandler:")]
		// [Async] - bug in generator
		void PlayPrerollAd (Action<NSError> completionHandler);

		[iOS (8,0)]
		[Export ("cancelPreroll")]
		void CancelPreroll ();
	}

	[iOS (12,0)]
	[NoWatch]
	[DisableDefaultCtor]
	[BaseType (typeof (UIViewController))]
	interface ADInterstitialAdPresentationViewController {
		// inlined ctor
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("initForInterstitialAd:")]
		IntPtr Constructor (ADInterstitialAd interstitialAd);

		[Export ("shouldTestVisibilityAtPoint:")]
		bool ShouldTestVisibility (CGPoint point);
	}
}
