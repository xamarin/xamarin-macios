#if !NET

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using AVKit;
using CoreGraphics;
using Foundation;
using MediaPlayer;
using ObjCRuntime;
using UIKit;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace iAd {

	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public class ADBannerView : UIView {

		public class ADBannerViewAppearance : UIViewAppearance {

			protected internal ADBannerViewAppearance (IntPtr handle) : base (handle)
			{
			}
		}

		protected internal ADBannerView (IntPtr handle)
		{
		}

		public unsafe override NativeHandle ClassHandle {
			get { return default (NativeHandle); }
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

		public new static ADBannerViewAppearance? GetAppearance<T> () where T : ADBannerView
		{
			return default (ADBannerViewAppearance);
		}

		public new static ADBannerViewAppearance? AppearanceWhenContainedIn (params Type [] containers)
		{
			return default (ADBannerViewAppearance);
		}

		public new static ADBannerViewAppearance? GetAppearance (UITraitCollection traits)
		{
			return default (ADBannerViewAppearance);
		}

		public new static ADBannerViewAppearance? GetAppearance (UITraitCollection traits, params Type [] containers)
		{
			return default (ADBannerViewAppearance);
		}

		public new static ADBannerViewAppearance? GetAppearance<T> (UITraitCollection traits) where T : ADBannerView
		{
			return default (ADBannerViewAppearance);
		}

		public new static ADBannerViewAppearance? GetAppearance<T> (UITraitCollection traits, params Type [] containers) where T : ADBannerView
		{
			return default (ADBannerViewAppearance);
		}
	}

	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public class AdErrorEventArgs : EventArgs {
		public NSError? Error {
			get { return default (NSError); }
			set { }
		}

		public AdErrorEventArgs (NSError error)
		{
		}
	}

	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public interface IADBannerViewDelegate : INativeObject, IDisposable {
	}

	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
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

	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public class ADBannerViewDelegate : NSObject, IADBannerViewDelegate, INativeObject, IDisposable {

		public ADBannerViewDelegate ()
		{
		}

		protected ADBannerViewDelegate (NSObjectFlag t)
		{
		}

		protected internal ADBannerViewDelegate (NativeHandle handle)
			: base (handle)
		{
		}

		public virtual void ActionFinished (ADBannerView banner)
		{
		}

		public virtual bool ActionShouldBegin (ADBannerView banner, bool willLeaveApplication)
		{
			return default (bool);
		}

		public virtual void AdLoaded (ADBannerView banner)
		{
		}

		public virtual void FailedToReceiveAd (ADBannerView banner, NSError error)
		{
		}

		public virtual void WillLoad (ADBannerView bannerView)
		{
		}
	}

	// some of this API is still provided
	public partial class ADClient : NSObject {

		[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
		public static NSString? ErrorDomain {
			get { return default (NSString); }
		}

		[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
		public virtual void DetermineAppInstallationAttribution (AttributedToiAdCompletionHandler completionHandler)
		{
		}

		[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
		public virtual void LookupAdConversionDetails (ADConversionDetails onCompleted)
		{
		}

		[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
		public virtual Task<ADClientConversionDetailsResult>? LookupAdConversionDetailsAsync ()
		{
			return default (Task<ADClientConversionDetailsResult>);
		}
	}

	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
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

	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public class ADInterstitialAd : NSObject {

		protected internal ADInterstitialAd (IntPtr handle)
			: base (handle)
		{
		}

		public unsafe override NativeHandle ClassHandle {
			get { return default (IntPtr); }
		}

		public virtual bool ActionInProgress {
			get { return default (bool); }
		}

		public IADInterstitialAdDelegate? Delegate {
			get { return default (IADInterstitialAdDelegate); }
			set { }
		}

		public virtual bool Loaded {
			get { return default (bool); }
		}

		public virtual NSObject? WeakDelegate {
			get { return default (NSObject); }
			set { }
		}

		public ADPredicate? ActionShouldBegin {
			get { return default (ADPredicate); }
			set { }
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

	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public class ADErrorEventArgs : EventArgs {
		public NSError? Error {
			get { return default (NSError); }
			set { }
		}

		public ADErrorEventArgs (NSError error)
		{
		}
	}

	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public interface IADInterstitialAdDelegate : INativeObject, IDisposable {

		void AdUnloaded (ADInterstitialAd interstitialAd);

		void FailedToReceiveAd (ADInterstitialAd interstitialAd, NSError error);

		void AdLoaded (ADInterstitialAd interstitialAd);

		bool ActionShouldBegin (ADInterstitialAd interstitialAd, bool willLeaveApplication);

		void ActionFinished (ADInterstitialAd interstitialAd);
	}

	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public static class ADInterstitialAdDelegate_Extensions {
		public static void WillLoad (this IADInterstitialAdDelegate This, ADInterstitialAd interstitialAd)
		{
		}
	}

	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public abstract class ADInterstitialAdDelegate : NSObject, IADInterstitialAdDelegate, INativeObject, IDisposable {

		protected ADInterstitialAdDelegate ()
		{
		}

		protected ADInterstitialAdDelegate (NSObjectFlag t)
		{
		}

		protected internal ADInterstitialAdDelegate (NativeHandle handle)
			: base (handle)
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

	[Deprecated (PlatformName.iOS, 13, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public class ADInterstitialAdPresentationViewController : UIViewController {

		public unsafe override NativeHandle ClassHandle {
			get { return default (NativeHandle); }
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

	[Deprecated (PlatformName.iOS, 10, 0, PlatformArchitecture.None, null)]
	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
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

	[Deprecated (PlatformName.iOS, 9, 0, PlatformArchitecture.None, "Use 'iAdPreroll_AVPlayerViewController' instead.")]
	[Obsoleted (PlatformName.iOS, 12, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public static class IAdPreroll {

		public static void CancelPreroll (this MPMoviePlayerController This)
		{
		}

		public static void PlayPrerollAd (this MPMoviePlayerController This, Action<NSError> completionHandler)
		{
		}
	}

	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public static class iAdPreroll_AVPlayerViewController {

		public static void CancelPreroll (this AVPlayerViewController This)
		{
		}

		public static void PlayPrerollAd (this AVPlayerViewController This, Action<NSError> completionHandler)
		{
		}
	}

	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public static class ADErrorExtensions {

		public static NSString? GetDomain (this ADError self)
		{
			return default (NSString?);
		}
	}

	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public delegate void ADConversionDetails (NSDate? appPurchaseDate, NSDate? iAdImpressionDate);

	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public delegate bool ADPredicate (ADInterstitialAd interstitialAd, bool willLeaveApplication);

	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public delegate bool AdAction (ADBannerView banner, bool willLeaveApplication);

	[Obsoleted (PlatformName.iOS, 15, 0, PlatformArchitecture.None, Constants.iAdRemoved)]
	public delegate void AttributedToiAdCompletionHandler (bool attributedToiAd);
}

#endif // !NET
