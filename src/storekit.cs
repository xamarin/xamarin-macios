//
// StoreKit.cs: This file describes the API that the generator will
// produce for StoreKit
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2012 Xamarin Inc.
//
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using StoreKit;
#if !MONOMAC
using UIKit;
#endif
using System;

namespace StoreKit {

	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	partial interface SKDownload {

		[iOS (12,0)]
		[Export ("state")]
		SKDownloadState State { get; }
#if MONOMAC
		[Obsolete ("Use 'State' instead.")]
		[Wrap ("State", IsVirtual = true)]
		SKDownloadState DownloadState { get;  }

		[Export ("contentLength", ArgumentSemantic.Copy)]
		NSNumber ContentLength { get; }
#else
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'State' instead.")]
		[Export ("downloadState")]
		SKDownloadState DownloadState { get;  }
		
		[Export ("contentLength")]
		long ContentLength { get;  }
#endif

		[Export ("contentIdentifier")]
		string ContentIdentifier { get;  }

		[Export ("contentURL", ArgumentSemantic.Copy)]
		NSUrl ContentUrl { get;  }

		[Export ("contentVersion", ArgumentSemantic.Copy)]
		string ContentVersion { get;  }

		[Export ("error", ArgumentSemantic.Copy)]
		NSError Error { get;  }

		[Export ("progress")]
		float Progress { get;  } /* float, not CGFloat */

		[Export ("timeRemaining")]
		double TimeRemaining { get;  }

#if MONOMAC
		[Export ("contentURLForProductID:")]
		[Static]
		NSUrl GetContentUrlForProduct (string productId);

		[Export ("deleteContentForProductID:")]
		[Static]
		void DeleteContentForProduct (string productId);
#endif

		[Mac (10,14, onlyOn64: true)]
		[Field ("SKDownloadTimeRemainingUnknown")]
		double TimeRemainingUnknown { get; }

		[Mac (10,11)]
		[Export ("transaction")]
		SKPaymentTransaction Transaction { get;  }
	}

	[BaseType (typeof (NSObject))]
	partial interface SKPayment : NSMutableCopying {
		[Static]
		[Export("paymentWithProduct:")]
		SKPayment CreateFrom (SKProduct product);
#if !MONOMAC
		[Static]
		[Export ("paymentWithProductIdentifier:")]
		[Availability (Deprecated = Platform.iOS_5_0, Message = "Use 'FromProduct (SKProduct)'' after fetching the list of available products from 'SKProductRequest' instead.")]
		SKPayment CreateFrom (string identifier);
#endif

		[Export ("productIdentifier", ArgumentSemantic.Copy)]
		string ProductIdentifier { get; }

		[Export ("requestData", ArgumentSemantic.Copy)]
		NSData RequestData { get; [NotImplemented ("Not available on SKPayment, only available on SKMutablePayment")] set;  }

		[Export ("quantity")]
		nint Quantity { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("applicationUsername", ArgumentSemantic.Copy)]
		string ApplicationUsername { get; }

		[iOS (8,3), Mac (10,14, onlyOn64: true)]
		[Export ("simulatesAskToBuyInSandbox")]
		bool SimulatesAskToBuyInSandbox { get; [NotImplemented ("Not available on SKPayment, only available on SKMutablePayment")] set; }
	}

	[BaseType (typeof (SKPayment))]
	interface SKMutablePayment {
		[Static]
		[Export("paymentWithProduct:")]
		SKMutablePayment PaymentWithProduct (SKProduct product);

		[Static]
		[Export ("paymentWithProductIdentifier:")]
		[Availability (Deprecated = Platform.iOS_5_0, Message = "Use 'PaymentWithProduct (SKProduct)' after fetching the list of available products from 'SKProductRequest' instead.")]
		SKMutablePayment PaymentWithProduct (string identifier);

		[NullAllowed] // by default this property is null
		[Export ("productIdentifier", ArgumentSemantic.Copy)][New]
		string ProductIdentifier { get; set; }

		[Export ("quantity")][New]
		nint Quantity { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("requestData", ArgumentSemantic.Copy)]
		[Override]
		NSData RequestData { get; set; }

		[iOS (7,0), Mac (10, 9)]
		[NullAllowed] // by default this property is null
		[Export ("applicationUsername", ArgumentSemantic.Copy)][New]
		string ApplicationUsername { get; set; }

		[iOS (8,3), Mac (10,14, onlyOn64: true)]
		[Export ("simulatesAskToBuyInSandbox")]
		bool SimulatesAskToBuyInSandbox { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface SKPaymentQueue {
		[Export ("defaultQueue")][Static]
		SKPaymentQueue DefaultQueue { get; }

		[Export ("canMakePayments")][Static]
		bool CanMakePayments { get; }

		[Export ("addPayment:")]
		void AddPayment (SKPayment payment);

		[Export ("restoreCompletedTransactions")]
		void RestoreCompletedTransactions ();

		[iOS (7,0), Mac (10, 9)]
		[Export ("restoreCompletedTransactionsWithApplicationUsername:")]
		void RestoreCompletedTransactions ([NullAllowed] string username);

		[Export ("finishTransaction:")]
		void FinishTransaction (SKPaymentTransaction transaction);

		[Export ("addTransactionObserver:")]
		void AddTransactionObserver ([Protocolize]SKPaymentTransactionObserver observer);

		[Export ("removeTransactionObserver:")]
		void RemoveTransactionObserver ([Protocolize]SKPaymentTransactionObserver observer);

		[Export ("transactions")]
		SKPaymentTransaction [] Transactions { get; }

		//
		// iOS 6.0
		//
		[iOS (6,0)]
		[Export ("startDownloads:")]
		void StartDownloads (SKDownload [] downloads);

		[iOS (6,0)]
		[Export ("pauseDownloads:")]
		void PauseDownloads (SKDownload [] downloads);

		[iOS (6,0)]
		[Export ("resumeDownloads:")]
		void ResumeDownloads (SKDownload [] downloads);

		[iOS (6,0)]
		[Export ("cancelDownloads:")]
		void CancelDownloads (SKDownload [] downloads);


	}
	
	[BaseType (typeof (NSObject))]
	interface SKProduct {
		[Export ("localizedDescription")]
		string LocalizedDescription { get; }

		[Export ("localizedTitle")]
		string LocalizedTitle { get; }

		[Export ("price")]
		NSDecimalNumber Price { get; }

		[Export ("priceLocale")]
		NSLocale PriceLocale { get; }

		[Export ("productIdentifier")]
		string ProductIdentifier { get; }

		[iOS (6,0)]
		[Export ("downloadable")]
		bool Downloadable { [Bind ("isDownloadable")] get; }

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'DownloadContentLengths' instead.")]
		[Export ("contentLengths")]
		NSNumber [] ContentLengths { get; }

		[iOS (6,0), Mac (10,14, onlyOn64: true)]
		[Export ("downloadContentLengths")]
		NSNumber [] DownloadContentLengths { get;  }

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'DownloadContentVersion' instead.")]
		[Export ("contentVersion")]
		string ContentVersion { get; }

		[iOS (6,0), Mac (10,14, onlyOn64: true)]
		[Export ("downloadContentVersion")]
		string DownloadContentVersion { get;  }

		[iOS (11,2), TV (11,2), Mac (10,13,2)]
		[NullAllowed, Export ("subscriptionPeriod")]
		SKProductSubscriptionPeriod SubscriptionPeriod { get; }

		[iOS (11,2), TV (11,2), Mac (10,13,2)]
		[NullAllowed, Export ("introductoryPrice")]
		SKProductDiscount IntroductoryPrice { get; }

		[iOS (12,0), Mac (10,14, onlyOn64: true)]
		[NullAllowed, Export ("subscriptionGroupIdentifier")]
		string SubscriptionGroupIdentifier { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface SKPaymentTransactionObserver {

		[Export ("paymentQueue:updatedTransactions:")][Abstract]
		void UpdatedTransactions (SKPaymentQueue queue, SKPaymentTransaction [] transactions);

		[Export ("paymentQueue:removedTransactions:")]
		void RemovedTransactions (SKPaymentQueue queue, SKPaymentTransaction [] transactions);

		[Export ("paymentQueue:restoreCompletedTransactionsFailedWithError:")]
		void RestoreCompletedTransactionsFailedWithError (SKPaymentQueue queue, NSError error);

		[Export ("paymentQueueRestoreCompletedTransactionsFinished:")]
		void RestoreCompletedTransactionsFinished (SKPaymentQueue queue);

		[iOS (6,0)]
		[Export ("paymentQueue:updatedDownloads:")]
		void UpdatedDownloads (SKPaymentQueue queue, SKDownload [] downloads);

		[iOS (11,0)][TV (11,0)][NoMac]
		[Export ("paymentQueue:shouldAddStorePayment:forProduct:")]
		bool ShouldAddStorePayment (SKPaymentQueue queue, SKPayment payment, SKProduct product);
	}

	[BaseType (typeof (NSObject))]
	interface SKPaymentTransaction {
		[Export ("error")]
		NSError Error { get; }

		[Export ("originalTransaction")]
		SKPaymentTransaction OriginalTransaction { get; }

		[Export ("payment")]
		SKPayment Payment { get; } 

		[Export ("transactionDate")]
		NSDate TransactionDate { get; }

		[Export ("transactionIdentifier")]
		string TransactionIdentifier { get; }

#if !MONOMAC
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'NSBundle.AppStoreReceiptUrl' instead.")]
		[Export ("transactionReceipt")]
		NSData TransactionReceipt { get; }
#endif

		[Export ("transactionState")]
		SKPaymentTransactionState TransactionState { get; }

		[iOS (6,0)]
		[Export ("downloads")]
		SKDownload [] Downloads { get;  }
	}

	[BaseType (typeof (NSObject), Delegates=new string [] {"WeakDelegate"}, Events=new Type [] {typeof (SKRequestDelegate)})]
	interface SKRequest {
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; [NullAllowed] set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		SKRequestDelegate Delegate { get; [NullAllowed] set; }

		[Export ("cancel")]
		void Cancel ();

		[Export ("start")]
		void Start ();
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface SKRequestDelegate {
		[Export ("requestDidFinish:")]
		void RequestFinished (SKRequest request);
		
		[Export ("request:didFailWithError:"), EventArgs ("SKRequestError")]
		void RequestFailed (SKRequest request, NSError error);
	}
		
	[iOS (7,0)]
	[Mac (10,9)]
	[BaseType (typeof (SKRequest))]
	interface SKReceiptRefreshRequest {
		[Export ("initWithReceiptProperties:")]
		IntPtr Constructor ([NullAllowed] NSDictionary properties);

		[Wrap ("this (receiptProperties == null ? null : receiptProperties.Dictionary)")]
		IntPtr Constructor ([NullAllowed] SKReceiptProperties receiptProperties);

		[Export ("receiptProperties")]
		NSDictionary WeakReceiptProperties { get; }

		[Wrap ("WeakReceiptProperties")]
		SKReceiptProperties ReceiptProperties { get; }
	}

	[iOS (7,0)]
	[Mac (10,9)]
	[Static, Internal]
	interface _SKReceiptProperty {
		[Field ("SKReceiptPropertyIsExpired"), Internal]
		NSString IsExpired { get; }

		[Field ("SKReceiptPropertyIsRevoked"), Internal]
		NSString IsRevoked { get; }

		[Field ("SKReceiptPropertyIsVolumePurchase"), Internal]
		NSString IsVolumePurchase { get; }
	}

	[BaseType (typeof (SKRequest), Delegates=new string [] {"WeakDelegate"}, Events=new Type [] {typeof (SKProductsRequestDelegate)})]
	interface SKProductsRequest {
		[Export ("initWithProductIdentifiers:")]
		IntPtr Constructor (NSSet productIdentifiersStringSet);
		
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed][New]
		NSObject WeakDelegate { get; [NullAllowed] set; }

		[Wrap ("WeakDelegate")][New]
		[Protocolize]
		SKProductsRequestDelegate Delegate { get; [NullAllowed] set; }
	}
	
	[BaseType (typeof (NSObject))]
	interface SKProductsResponse {
		[Export ("products")]
		SKProduct [] Products { get; }

		[Export ("invalidProductIdentifiers")]
		string [] InvalidProducts { get; }
	}

	[BaseType (typeof (SKRequestDelegate))]
	[Model]
	[Protocol]
	interface SKProductsRequestDelegate {
		[Export ("productsRequest:didReceiveResponse:")][Abstract][EventArgs ("SKProductsRequestResponse")]
		void ReceivedResponse (SKProductsRequest request, SKProductsResponse response);
	}

#if !MONOMAC
	[NoTV]
	[iOS (6,0)]
	[BaseType (typeof (UIViewController),
		   Delegates=new string [] { "WeakDelegate" },
		   Events   =new Type   [] { typeof (SKStoreProductViewControllerDelegate) })]
	interface SKStoreProductViewController {
#if !XAMCORE_4_0
		// SKStoreProductViewController is an OS View Controller which can't be customized
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);
#endif

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set;  }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		SKStoreProductViewControllerDelegate Delegate { get; set; }

		[Export ("loadProductWithParameters:completionBlock:")][Internal]
		[Async]
		void LoadProduct (NSDictionary parameters, [NullAllowed] Action<bool,NSError> callback);

		[Wrap ("LoadProduct (parameters == null ? null : parameters.Dictionary, callback)")]
		[Async]
		void LoadProduct (StoreProductParameters parameters, [NullAllowed] Action<bool,NSError> callback);
	}

	[iOS (6,0), NoMac]
	[StrongDictionary ("SKStoreProductParameterKey")]
	interface StoreProductParameters {

		[iOS (11,3), TV (11,3)]
		[Export ("AdNetworkAttributionSignature")]
		string AdNetworkAttributionSignature { get; set; }

		[iOS (11,3), TV (11,3)]
		[Export ("AdNetworkCampaignIdentifier")]
		uint AdNetworkCampaignIdentifier { get; set; }

		[iOS (11,3), TV (11,3)]
		[Export ("AdNetworkIdentifier")]
		string AdNetworkIdentifier { get; set; }

		[iOS (11,3), TV (11,3)]
		[Export ("AdNetworkNonce")]
		NSUuid AdNetworkNonce { get; set; }

		[iOS (11,3), TV (11,3)]
		[Export ("AdNetworkTimestamp")]
		uint AdNetworkTimestamp { get; set; }
	}

	[Since (6,0)]
	[Static]
	interface SKStoreProductParameterKey
	{
		[Field ("SKStoreProductParameterITunesItemIdentifier")]
		NSString ITunesItemIdentifier { get; }

		[iOS (11,0)][TV (11,0)]
		[Field ("SKStoreProductParameterProductIdentifier")]
		NSString ProductIdentifier { get; }

		[iOS (8,0)]
		[Field ("SKStoreProductParameterAffiliateToken")]
		NSString AffiliateToken { get; }

		[iOS (8,0)]
		[Field ("SKStoreProductParameterCampaignToken")]
		NSString CampaignToken { get; }

		[iOS (8,3)]
		[Field ("SKStoreProductParameterProviderToken")]
		NSString ProviderToken { get; }

		[iOS (9,3)]
		[TV (9,2)]
		[Field ("SKStoreProductParameterAdvertisingPartnerToken")]
		NSString AdvertisingPartnerToken { get; }

		[iOS (11,3), TV (11,3), NoMac]
		[Field ("SKStoreProductParameterAdNetworkAttributionSignature")]
		NSString AdNetworkAttributionSignature { get; }

		[iOS (11,3), TV (11,3), NoMac]
		[Field ("SKStoreProductParameterAdNetworkCampaignIdentifier")]
		NSString AdNetworkCampaignIdentifier { get; }

		[iOS (11,3), TV (11,3), NoMac]
		[Field ("SKStoreProductParameterAdNetworkIdentifier")]
		NSString AdNetworkIdentifier { get; }

		[iOS (11,3), TV (11,3), NoMac]
		[Field ("SKStoreProductParameterAdNetworkNonce")]
		NSString AdNetworkNonce { get; }

		[iOS (11,3), TV (11,3), NoMac]
		[Field ("SKStoreProductParameterAdNetworkTimestamp")]
		NSString AdNetworkTimestamp { get; }
	}

	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface SKStoreProductViewControllerDelegate {
		[Export ("productViewControllerDidFinish:"), EventArgs ("SKStoreProductViewController")]
		void Finished (SKStoreProductViewController controller);
	}

	[iOS (9,3)]
	[TV (9,2)]
	[BaseType (typeof (NSObject))]
#if XAMCORE_3_0 // Avoid breaking change in iOS
	[DisableDefaultCtor]
#endif
	interface SKCloudServiceController {
		[Static]
		[Export ("authorizationStatus")]
		SKCloudServiceAuthorizationStatus AuthorizationStatus { get; }

		[Static]
		[Async]
		[Export ("requestAuthorization:")]
		void RequestAuthorization (Action<SKCloudServiceAuthorizationStatus> handler);

		[Async]
		[Export ("requestStorefrontIdentifierWithCompletionHandler:")]
		void RequestStorefrontIdentifier (Action<NSString, NSError> completionHandler);

		[iOS (11,0)][TV (11,0)]
		[Async]
		[Export ("requestStorefrontCountryCodeWithCompletionHandler:")]
		void RequestStorefrontCountryCode (Action<NSString, NSError> completionHandler);

		[Async]
		[Export ("requestCapabilitiesWithCompletionHandler:")]
		void RequestCapabilities (Action<SKCloudServiceCapability, NSError> completionHandler);

		[iOS (10,3), TV (10,2)]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'RequestUserToken' instead.")]
		[Deprecated (PlatformName.TvOS, 11,0, message: "Use 'RequestUserToken' instead.")]
		[Async]
		[Export ("requestPersonalizationTokenForClientToken:withCompletionHandler:")]
		void RequestPersonalizationToken (string clientToken, Action<NSString, NSError> completionHandler);

		[iOS (11,0)][TV (11,0)]
		[Async]
		[Export ("requestUserTokenForDeveloperToken:completionHandler:")]
		void RequestUserToken (string developerToken, Action<NSString, NSError> completionHandler);

		[Notification]
		[Field ("SKStorefrontIdentifierDidChangeNotification")]
		NSString StorefrontIdentifierDidChangeNotification { get; }

		[Notification]
		[Field ("SKCloudServiceCapabilitiesDidChangeNotification")]
		NSString CloudServiceCapabilitiesDidChangeNotification { get; }

		[iOS (11,0)][TV (11,0)]
		[Notification]
		[Field ("SKStorefrontCountryCodeDidChangeNotification")]
		NSString StorefrontCountryCodeDidChangeNotification { get; }
	}

	[iOS (10,1)]
	[NoTV] // __TVOS_PROHIBITED
	[BaseType (typeof(UIViewController))]
	interface SKCloudServiceSetupViewController
	{
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		ISKCloudServiceSetupViewControllerDelegate Delegate { get; set; }

		[Async]
		[Export ("loadWithOptions:completionHandler:")]
		void Load (NSDictionary options, [NullAllowed] Action<bool, NSError> completionHandler);

		[Async]
		[Wrap ("Load (options == null ? null : options.Dictionary, completionHandler)")]
		void Load (SKCloudServiceSetupOptions options, Action<bool, NSError> completionHandler);
	}

	interface ISKCloudServiceSetupViewControllerDelegate {}

	[iOS (10,1)]
	[NoTV] // __TVOS_PROHIBITED on the only member + SKCloudServiceSetupViewController is not in tvOS
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface SKCloudServiceSetupViewControllerDelegate
	{
		[Export ("cloudServiceSetupViewControllerDidDismiss:")]
		void DidDismiss (SKCloudServiceSetupViewController cloudServiceSetupViewController);
	}

	[NoTV, iOS (10,1)]
	[StrongDictionary ("SKCloudServiceSetupOptionsKeys")]
	interface SKCloudServiceSetupOptions
	{
		// Headers comment: Action for setup entry point (of type SKCloudServiceSetupAction).
		// FIXME: Once https://bugzilla.xamarin.com/show_bug.cgi?id=57870 is fixed we should have a wrapper on a new property
		// `SKCloudServiceSetupAction Action { get; set; }` and avoid manual code.
		[Internal]
		[Export ("ActionKey")]
		NSString _Action { get; set; }

		// Headers comment: Identifier of the iTunes Store item the user is trying to access which requires cloud service setup (NSNumber).
		nint ITunesItemIdentifier { get; set; }

		[iOS (10,3)]
		string AffiliateToken { get; set; }

		[iOS (10,3)]
		string CampaignToken { get; set; }

		[iOS (11,0)]
		string MessageIdentifier { get; set; }
	}

	[NoTV, iOS (10,1)]
	[Internal, Static]
	interface SKCloudServiceSetupOptionsKeys
	{
		[Field ("SKCloudServiceSetupOptionsActionKey")]
		NSString ActionKey { get; }

		[Field ("SKCloudServiceSetupOptionsITunesItemIdentifierKey")]
		NSString ITunesItemIdentifierKey { get; }

		[iOS (10,3)]
		[Field ("SKCloudServiceSetupOptionsAffiliateTokenKey")]
		NSString AffiliateTokenKey { get; }

		[iOS (10,3)]
		[Field ("SKCloudServiceSetupOptionsCampaignTokenKey")]
		NSString CampaignTokenKey { get; }

		[iOS (11,0)]
		[Field ("SKCloudServiceSetupOptionsMessageIdentifierKey")]
		NSString MessageIdentifierKey { get; }
	}

	[NoTV, iOS (10,1)]
	enum SKCloudServiceSetupAction
	{
		[Field ("SKCloudServiceSetupActionSubscribe")]
		Subscribe,
	}

	[iOS (11,0), TV (11,0)]
	enum SKCloudServiceSetupMessageIdentifier {
		[Field ("SKCloudServiceSetupMessageIdentifierJoin")]
		Join,
		[Field ("SKCloudServiceSetupMessageIdentifierConnect")]
		Connect,
		[Field ("SKCloudServiceSetupMessageIdentifierAddMusic")]
		AddMusic,
		[Field ("SKCloudServiceSetupMessageIdentifierPlayMusic")]
		PlayMusic,
	}

	[iOS (11,0), TV (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // static Default property is the only documented way to get the controller
	interface SKProductStorePromotionController {
		[Static]
		[Export ("defaultController")]
		SKProductStorePromotionController Default { get; }

		[Async]
		[Export ("fetchStorePromotionVisibilityForProduct:completionHandler:")]
		void FetchStorePromotionVisibility (SKProduct product, [NullAllowed] Action<SKProductStorePromotionVisibility, NSError> completionHandler);

		[Async]
		[Export ("updateStorePromotionVisibility:forProduct:completionHandler:")]
		void Update (SKProductStorePromotionVisibility promotionVisibility, SKProduct product, [NullAllowed] Action<NSError> completionHandler);

		[Async]
		[Export ("fetchStorePromotionOrderWithCompletionHandler:")]
		void FetchStorePromotionOrder ([NullAllowed] Action<SKProduct [], NSError> completionHandler);

		[Async]
		[Export ("updateStorePromotionOrder:completionHandler:")]
		void Update (SKProduct[] storePromotionOrder, [NullAllowed] Action<NSError> completionHandler);
	}
#endif

	[iOS (10,3), Mac (10,14, onlyOn64: true)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Not specified but very likely
	interface SKStoreReviewController {

		[Static]
		[Export ("requestReview")]
		void RequestReview ();
	}

	[iOS (11,2), TV (11,2), Mac (10,13,2)]
	[BaseType (typeof (NSObject))]
	interface SKProductSubscriptionPeriod {

		[Export ("numberOfUnits")]
		nuint NumberOfUnits { get; }

		[Export ("unit")]
		SKProductPeriodUnit Unit { get; }
	}

	[iOS (11,2), TV (11,2), Mac (10,13,2)]
	[BaseType (typeof (NSObject))]
	interface SKProductDiscount {

		[Export ("price")]
		NSDecimalNumber Price { get; }

		[Export ("priceLocale")]
		NSLocale PriceLocale { get; }

		[Export ("subscriptionPeriod")]
		SKProductSubscriptionPeriod SubscriptionPeriod { get; }

		[Export ("numberOfPeriods")]
		nuint NumberOfPeriods { get; }

		[Export ("paymentMode")]
		SKProductDiscountPaymentMode PaymentMode { get; }
	}

	[iOS (11,3), NoTV, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SKAdNetwork {

		[Static]
		[Export ("registerAppForAdNetworkAttribution")]
		void RegisterAppForAdNetworkAttribution ();
	}
}
