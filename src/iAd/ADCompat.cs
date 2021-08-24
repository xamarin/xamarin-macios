#if !XAMCORE_4_0

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using AVKit;
using CoreGraphics;
using Foundation;
using MediaPlayer;
using ObjCRuntime;
using UIKit;

#nullable enable

namespace iAd {

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public class ADBannerView : UIView {

		public class ADBannerViewAppearance : UIViewAppearance {

			protected internal ADBannerViewAppearance (IntPtr handle) : base (handle)
			{
			}
		}

		protected internal ADBannerView (IntPtr handle)
		{
		}

		public unsafe override IntPtr ClassHandle {
			get { return default (IntPtr); }
		}

		public virtual ADAdType AdType {
			get { return default (ADAdType); }
		}

		public virtual string? AdvertisingSection {
			get { return default (string); }
			set { }
		}

		public virtual bool BannerLoaded {
			get { return default (bool); }
		}

		public virtual bool BannerViewActionInProgress {
			get { return default (bool); }
		}

		public virtual string? CurrentContentSizeIdentifier {
			get { return default (string); }
			set { }
		}

		public IADBannerViewDelegate? Delegate {
			get { return default (IADBannerViewDelegate); }
			set { }
		}

		public virtual NSSet? RequiredContentSizeIdentifiers {
			get { return default (NSSet); }
			set { }
		}

		public virtual NSObject? WeakDelegate {
			get { return default (NSObject); }
			set { }
		}

		public static NSString? SizeIdentifier320x50 {
			get { return default (NSString); }
		}

		public static NSString? SizeIdentifier480x32 {
			get { return default (NSString); }
		}

		public static NSString? SizeIdentifierLandscape {
			get { return default (NSString); }
		}

		public static NSString? SizeIdentifierPortrait {
			get { return default (NSString); }
		}

		public AdAction? ActionShouldBegin {
			get { return default (AdAction); }
			set { }
		}

		public new static ADBannerViewAppearance? Appearance {
			get { return default (ADBannerViewAppearance); }
		}

		public event EventHandler ActionFinished {
			add { }
			remove { }
		}

		public event EventHandler AdLoaded {
			add { }
			remove { }
		}

		public event EventHandler<AdErrorEventArgs> FailedToReceiveAd {
			add { }
			remove { }
		}

		public event EventHandler WillLoad {
			add { }
			remove { }
		}

		public static CGSize GetClampedBannerSize (CGSize size)
		{
			return default (CGSize);
		}

		public ADBannerView ()
		{
		}

		public ADBannerView (NSCoder coder)
		{
		}

		protected ADBannerView (NSObjectFlag t)
		{
		}

		public ADBannerView (CGRect frame)
		{
		}

		public ADBannerView (ADAdType type)
		{
		}

		public virtual void CancelBannerViewAction ()
		{
		}

		public static CGSize SizeFromContentSizeIdentifier (string sizeIdentifier)
		{
			return default (CGSize);
		}

		protected override void Dispose (bool disposing)
		{
		}

		public new static ADBannerViewAppearance? GetAppearance<T>() where T : ADBannerView
		{
			return default (ADBannerViewAppearance);
		}

		public new static ADBannerViewAppearance? AppearanceWhenContainedIn(params Type[] containers)
		{
			return default (ADBannerViewAppearance);
		}

		public new static ADBannerViewAppearance? GetAppearance(UITraitCollection traits)
		{
			return default (ADBannerViewAppearance);
		}

		public new static ADBannerViewAppearance? GetAppearance(UITraitCollection traits, params Type[] containers)
		{
			return default (ADBannerViewAppearance);
		}

		public new static ADBannerViewAppearance? GetAppearance<T>(UITraitCollection traits) where T : ADBannerView
		{
			return default (ADBannerViewAppearance);
		}

		public new static ADBannerViewAppearance? GetAppearance<T>(UITraitCollection traits, params Type[] containers) where T : ADBannerView
		{
			return default (ADBannerViewAppearance);
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public class AdErrorEventArgs : EventArgs {
		public NSError? Error {
			get { return default (NSError); }
			set { }
		}

		public AdErrorEventArgs (NSError error)
		{
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public interface IADBannerViewDelegate : INativeObject, IDisposable {
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public static class ADBannerViewDelegate_Extensions {

		public static void AdLoaded (this IADBannerViewDelegate This, ADBannerView banner)
		{
		}

		public static void FailedToReceiveAd (this IADBannerViewDelegate This, ADBannerView banner, NSError error)
		{
		}

		public static bool ActionShouldBegin (this IADBannerViewDelegate This, ADBannerView banner, bool willLeaveApplication)
		{
			return default (bool);
		}

		public static void ActionFinished (this IADBannerViewDelegate This, ADBannerView banner)
		{
		}

		public static void WillLoad (this IADBannerViewDelegate This, ADBannerView bannerView)
		{
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public class ADBannerViewDelegate : NSObject, IADBannerViewDelegate, INativeObject, IDisposable {

		public ADBannerViewDelegate ()
		{
		}

		protected ADBannerViewDelegate (NSObjectFlag t)
		{
		}

		protected internal ADBannerViewDelegate (IntPtr handle)
		{
		}

		public virtual void ActionFinished (ADBannerView banner)
		{
		}

		public virtual bool ActionShouldBegin (ADBannerView banner, bool willLeaveApplication)
		{
			return default (bool);
		}

		public virtual void AdLoaded(ADBannerView banner)
		{
		}

		public virtual void FailedToReceiveAd(ADBannerView banner, NSError error)
		{
		}

		public virtual void WillLoad(ADBannerView bannerView)
		{
		}
	}

	// some of this API is still provided
	public partial class ADClient : NSObject {

#if !NET
		[Deprecated (PlatformName.iOS, 15, 0, message: "The iAd framework has been removed from iOS")]
#else
		[UnsupportedOSPlatform ("ios15.0")]
#if IOS
		[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
#endif
		public static NSString? ErrorDomain {
			get { return default (NSString); }
		}

#if !NET
		[Deprecated (PlatformName.iOS, 15, 0, message: "The iAd framework has been removed from iOS")]
#else
		[UnsupportedOSPlatform ("ios15.0")]
#if IOS
		[Obsolete ("Starting with ios15.0 The iAd framework has been removed from iOS", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public virtual void DetermineAppInstallationAttribution (AttributedToiAdCompletionHandler completionHandler)
		{
		}

#if !NET
		[Deprecated (PlatformName.iOS, 15, 0, message: "The iAd framework has been removed from iOS")]
#else
		[UnsupportedOSPlatform ("ios15.0")]
#if IOS
		[Obsolete ("Starting with ios15.0 The iAd framework has been removed from iOS", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public virtual void LookupAdConversionDetails (ADConversionDetails onCompleted)
		{
		}

#if !NET
		[Deprecated (PlatformName.iOS, 15, 0, message: "The iAd framework has been removed from iOS")]
#else
		[UnsupportedOSPlatform ("ios15.0")]
#if IOS
		[Obsolete ("Starting with ios15.0 The iAd framework has been removed from iOS", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
		public virtual Task<ADClientConversionDetailsResult>? LookupAdConversionDetailsAsync()
		{
			return default (Task<ADClientConversionDetailsResult>);
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public class ADClientConversionDetailsResult {
		public NSDate? AppPurchaseDate {
			get { return default (NSDate); }
			set { }
		}

		public NSDate? IAdImpressionDate {
			get { return default (NSDate); }
			set { }
		}

		public ADClientConversionDetailsResult (NSDate appPurchaseDate, NSDate iAdImpressionDate)
		{
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public class ADInterstitialAd : NSObject {

		protected internal ADInterstitialAd (IntPtr handle)
		{
		}

		public unsafe override IntPtr ClassHandle {
			get { return default (IntPtr); }
		}

		public virtual bool ActionInProgress {
			get { return default (bool); }
		}

		public IADInterstitialAdDelegate? Delegate {
			get { return default (IADInterstitialAdDelegate); }
			set	{ }
		}

		public virtual bool Loaded {
			get { return default (bool); }
		}

		public virtual NSObject? WeakDelegate {
			get { return default (NSObject); }
			set	{ }
		}

		public ADPredicate? ActionShouldBegin {
			get { return default (ADPredicate); }
			set	{ }
		}

		public event EventHandler ActionFinished {
			add { }
			remove { }
		}

		public event EventHandler AdLoaded {
			add { }
			remove { }
		}

		public event EventHandler AdUnloaded {
			add { }
			remove { }
		}

		public event EventHandler<ADErrorEventArgs> FailedToReceiveAd {
			add { }
			remove { }
		}

		public event EventHandler WillLoad {
			add { }
			remove { }
		}

		public ADInterstitialAd ()
		{
		}

		protected ADInterstitialAd (NSObjectFlag t)
		{
		}

		public virtual void CancelAction ()
		{
		}

		public virtual void PresentFromViewController (UIViewController viewController)
		{
		}

		public virtual bool PresentInView (UIView containerView)
		{
			return default (bool);
		}

		protected override void Dispose (bool disposing)
		{
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public class ADErrorEventArgs : EventArgs {
		public NSError? Error {
			get { return default (NSError); }
			set	{ }
		}

		public ADErrorEventArgs (NSError error)
		{
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public interface IADInterstitialAdDelegate : INativeObject, IDisposable {

		void AdUnloaded (ADInterstitialAd interstitialAd);

		void FailedToReceiveAd (ADInterstitialAd interstitialAd, NSError error);

		void AdLoaded (ADInterstitialAd interstitialAd);

		bool ActionShouldBegin (ADInterstitialAd interstitialAd, bool willLeaveApplication);

		void ActionFinished (ADInterstitialAd interstitialAd);
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public static class ADInterstitialAdDelegate_Extensions {
		public static void WillLoad (this IADInterstitialAdDelegate This, ADInterstitialAd interstitialAd)
		{
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public abstract class ADInterstitialAdDelegate : NSObject, IADInterstitialAdDelegate, INativeObject, IDisposable {

		protected ADInterstitialAdDelegate ()
		{
		}

		protected ADInterstitialAdDelegate (NSObjectFlag t)
		{
		}

		protected internal ADInterstitialAdDelegate (IntPtr handle)
		{
		}

		public abstract void ActionFinished (ADInterstitialAd interstitialAd);

		public abstract bool ActionShouldBegin (ADInterstitialAd interstitialAd, bool willLeaveApplication);

		public abstract void AdLoaded (ADInterstitialAd interstitialAd);

		public abstract void AdUnloaded (ADInterstitialAd interstitialAd);

		public abstract void FailedToReceiveAd (ADInterstitialAd interstitialAd, NSError error);

		public virtual void WillLoad (ADInterstitialAd interstitialAd)
		{
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Deprecated (PlatformName.iOS, 13, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public class ADInterstitialAdPresentationViewController : UIViewController {

		public unsafe override IntPtr ClassHandle {
			get { return default (IntPtr); }
		}

		public ADInterstitialAdPresentationViewController (NSCoder coder)
		{
		}

		protected ADInterstitialAdPresentationViewController (NSObjectFlag t)
		{
		}

		protected internal ADInterstitialAdPresentationViewController (IntPtr handle)
		{
		}

		public ADInterstitialAdPresentationViewController (string? nibName, NSBundle? bundle)
		{
		}

		public ADInterstitialAdPresentationViewController (ADInterstitialAd interstitialAd)
		{
		}

		public virtual bool ShouldTestVisibility (CGPoint point)
		{
			return default (bool);
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public static class IAdAdditions {

		public static bool DisplayingBannerAd (this UIViewController This)
		{
			return default (bool);
		}

		public static bool GetCanDisplayBannerAds (this UIViewController This)
		{
			return default (bool);
		}

		public static ADInterstitialPresentationPolicy GetInterstitialPresentationPolicy (this UIViewController This)
		{
			return default (ADInterstitialPresentationPolicy);
		}

		public static UIView? GetOriginalContentView (this UIViewController This)
		{
			return default (UIView);
		}

		public static bool PresentingFullScreenAd (this UIViewController This)
		{
			return default (bool);
		}

		public static bool RequestInterstitialAdPresentation (this UIViewController This)
		{
			return default (bool);
		}

		public static void SetCanDisplayBannerAds (this UIViewController This, bool value)
		{
		}

		public static void SetInterstitialPresentationPolicy (this UIViewController This, ADInterstitialPresentationPolicy policy)
		{
		}

		public static bool ShouldPresentInterstitialAd (this UIViewController This)
		{
			return default (bool);
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, PlatformArchitecture.None, "Use 'iAdPreroll_AVPlayerViewController' instead.")]
	[Obsoleted (PlatformName.iOS, 12, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public static class IAdPreroll {

#if !NET
		[iOS (8,0)]
#else
		[SupportedOSPlatform ("ios8.0")]
#endif
		public static void CancelPreroll (this MPMoviePlayerController This)
		{
		}

		public static void PlayPrerollAd (this MPMoviePlayerController This, Action<NSError> completionHandler)
		{
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public static class iAdPreroll_AVPlayerViewController {

		public static void CancelPreroll (this AVPlayerViewController This)
		{
		}

		public static void PlayPrerollAd (this AVPlayerViewController This, Action<NSError> completionHandler)
		{
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios")]
#else
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public static class ADErrorExtensions {

		public static NSString? GetDomain( this ADError self)
		{
			return default (NSString?);
		}
	}

#if !NET
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public delegate void ADConversionDetails (NSDate? appPurchaseDate, NSDate? iAdImpressionDate);

#if !NET
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public delegate bool ADPredicate (ADInterstitialAd interstitialAd, bool willLeaveApplication);

#if !NET
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public delegate bool AdAction (ADBannerView banner, bool willLeaveApplication);

#if !NET
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
#endif
	public delegate void AttributedToiAdCompletionHandler (bool attributedToiAd);
}

#endif // !XAMCORE_4_0
