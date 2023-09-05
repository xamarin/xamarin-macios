//
// StoreKit.cs: This file describes the API that the generator will
// produce for StoreKit
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2012 Xamarin Inc.
// Copyright 2020 Microsoft Corp.
//
using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;
using StoreKit;
#if MONOMAC
using AppKit;
using UIViewController = AppKit.NSViewController;
using UIWindowScene = Foundation.NSObject;
#else
using UIKit;
#endif
#if WATCH
using UIViewController = Foundation.NSObject;
#endif
using System;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace StoreKit {

	[ErrorDomain ("SKANErrorDomain")]
	[NoWatch, NoTV, NoMac, iOS (15, 4), NoMacCatalyst]
	[Native]
	public enum SKANError : long {
		ImpressionMissingRequiredValue = 0,
		Unsupported = 1,
		AdNetworkIdMissing = 2,
		MismatchedSourceAppId = 3,
		ImpressionNotFound = 4,
		InvalidCampaignId = 5,
		InvalidConversionValue = 6,
		InvalidSourceAppId = 7,
		InvalidAdvertisedAppId = 8,
		InvalidVersion = 9,
		Unknown = 10,
		ImpressionTooShort = 11,
	}

	[Deprecated (PlatformName.iOS, 16, 0)]
	[Deprecated (PlatformName.MacOSX, 13, 0)]
	[Deprecated (PlatformName.TvOS, 16, 0)]
	[Deprecated (PlatformName.WatchOS, 9, 0)]
	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 16, 0)]
	[BaseType (typeof (NSObject))]
	partial interface SKDownload {

		[iOS (12, 0)]
		[TV (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("state")]
		SKDownloadState State { get; }
#if MONOMAC
		[NoiOS][NoTV][NoWatch][NoMacCatalyst]
		[Obsolete ("Use 'State' instead.")]
		[Wrap ("State", IsVirtual = true)]
		SKDownloadState DownloadState { get;  }

		[NoiOS][NoTV][NoWatch][NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'ExpectedContentLength' instead.")]
		[Export ("contentLength", ArgumentSemantic.Copy)]
		NSNumber ContentLength { get; }
#else
		[NoMac]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'State' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'State' instead.")]
		[Export ("downloadState")]
		SKDownloadState DownloadState { get; }

		[NoMac]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ExpectedContentLength' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ExpectedContentLength' instead.")]
		[Export ("contentLength")]
		long ContentLength { get; }
#endif

		[TV (13, 0), Mac (10, 15), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("expectedContentLength")]
		long ExpectedContentLength { get; }

		[Export ("contentIdentifier")]
		string ContentIdentifier { get; }

		[NullAllowed]
		[Export ("contentURL", ArgumentSemantic.Copy)]
		NSUrl ContentUrl { get; }

		[Export ("contentVersion", ArgumentSemantic.Copy)]
		string ContentVersion { get; }

		[NullAllowed]
		[Export ("error", ArgumentSemantic.Copy)]
		NSError Error { get; }

		[Export ("progress")]
		float Progress { get; } /* float, not CGFloat */

		[Export ("timeRemaining")]
		double TimeRemaining { get; }

		[NoWatch]
		[NoTV]
		[NoiOS]
		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Export ("contentURLForProductID:")]
		[Static]
		NSUrl GetContentUrlForProduct (string productId);

		[NoWatch]
		[NoTV]
		[NoiOS]
		[MacCatalyst (13, 1)]
		[Export ("deleteContentForProductID:")]
		[Static]
		void DeleteContentForProduct (string productId);

		[Mac (10, 14)]
		[MacCatalyst (13, 1)]
		[Field ("SKDownloadTimeRemainingUnknown")]
		double TimeRemainingUnknown { get; }

		[Mac (10, 11)]
		[MacCatalyst (13, 1)]
		[Export ("transaction")]
		SKPaymentTransaction Transaction { get; }
	}

	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
#if NET
	[DisableDefaultCtor]
#endif
	partial interface SKPayment : NSMutableCopying {
		[Static]
		[Export ("paymentWithProduct:")]
		SKPayment CreateFrom (SKProduct product);

		[NoMac]
		[NoWatch]
		[Static]
		[Export ("paymentWithProductIdentifier:")]
		[Deprecated (PlatformName.iOS, 5, 0, message: "Use 'FromProduct (SKProduct)'' after fetching the list of available products from 'SKProductRequest' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'FromProduct (SKProduct)'' after fetching the list of available products from 'SKProductRequest' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'FromProduct (SKProduct)'' after fetching the list of available products from 'SKProductRequest' instead.")]
		SKPayment CreateFrom (string identifier);

		[Export ("productIdentifier", ArgumentSemantic.Copy)]
		string ProductIdentifier { get; }

		[Export ("requestData", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSData RequestData { get; [NotImplemented ("Not available on SKPayment, only available on SKMutablePayment")] set; }

		[Export ("quantity")]
		nint Quantity { get; }

		[Mac (10, 9)]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("applicationUsername", ArgumentSemantic.Copy)]
		string ApplicationUsername { get; }

		[iOS (8, 3), Mac (10, 14)]
		[MacCatalyst (13, 1)]
		[Export ("simulatesAskToBuyInSandbox")]
		bool SimulatesAskToBuyInSandbox { get; [NotImplemented ("Not available on SKPayment, only available on SKMutablePayment")] set; }

		[iOS (12, 2)]
		[TV (12, 2)]
		[Mac (10, 14, 4)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("paymentDiscount", ArgumentSemantic.Copy)]
		SKPaymentDiscount PaymentDiscount { get; [NotImplemented ("Not available on SKPayment, only available on SKMutablePayment")] set; }
	}

	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SKPayment))]
#if NET
	[DisableDefaultCtor]
#endif
	interface SKMutablePayment {
		[Static]
		[Export ("paymentWithProduct:")]
		SKMutablePayment PaymentWithProduct (SKProduct product);

		[NoWatch]
		[Static]
		[Export ("paymentWithProductIdentifier:")]
		[Deprecated (PlatformName.iOS, 5, 0, message: "Use 'PaymentWithProduct (SKProduct)' after fetching the list of available products from 'SKProductRequest' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PaymentWithProduct (SKProduct)' after fetching the list of available products from 'SKProductRequest' instead.")]
		SKMutablePayment PaymentWithProduct (string identifier);

		[Export ("productIdentifier", ArgumentSemantic.Copy)]
		[New]
		string ProductIdentifier { get; set; }

		[Export ("quantity")]
		[New]
		nint Quantity { get; set; }

		[NullAllowed]
		[Export ("requestData", ArgumentSemantic.Copy)]
		[Override]
		NSData RequestData { get; set; }

		[Mac (10, 9)]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("applicationUsername", ArgumentSemantic.Copy)]
		[New]
		string ApplicationUsername { get; set; }

		[iOS (8, 3), Mac (10, 14)]
		[MacCatalyst (13, 1)]
		[Export ("simulatesAskToBuyInSandbox")]
		bool SimulatesAskToBuyInSandbox { get; set; }

		[iOS (12, 2)]
		[TV (12, 2)]
		[Mac (10, 14, 4)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("paymentDiscount", ArgumentSemantic.Copy)]
		SKPaymentDiscount PaymentDiscount { get; set; }
	}

	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SKPaymentQueue {
		[Export ("defaultQueue")]
		[Static]
		SKPaymentQueue DefaultQueue { get; }

		[Export ("canMakePayments")]
		[Static]
		bool CanMakePayments { get; }

		[Export ("addPayment:")]
		void AddPayment (SKPayment payment);

		[Export ("restoreCompletedTransactions")]
		void RestoreCompletedTransactions ();

		[Mac (10, 9)]
		[MacCatalyst (13, 1)]
		[Export ("restoreCompletedTransactionsWithApplicationUsername:")]
		void RestoreCompletedTransactions ([NullAllowed] string username);

		[Export ("finishTransaction:")]
		void FinishTransaction (SKPaymentTransaction transaction);

		[Export ("addTransactionObserver:")]
		void AddTransactionObserver ([Protocolize] SKPaymentTransactionObserver observer);

		[Export ("removeTransactionObserver:")]
		void RemoveTransactionObserver ([Protocolize] SKPaymentTransactionObserver observer);

		[Export ("transactions")]
		SKPaymentTransaction [] Transactions { get; }

		//
		// iOS 6.0
		//
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.WatchOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("startDownloads:")]
		void StartDownloads (SKDownload [] downloads);

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.WatchOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("pauseDownloads:")]
		void PauseDownloads (SKDownload [] downloads);

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.WatchOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("resumeDownloads:")]
		void ResumeDownloads (SKDownload [] downloads);

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.WatchOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("cancelDownloads:")]
		void CancelDownloads (SKDownload [] downloads);

		[Mac (10, 15), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ISKPaymentQueueDelegate Delegate { get; set; }

		[Mac (10, 15), iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Mac (10, 15), iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("storefront")]
		SKStorefront Storefront { get; }

		[NoWatch, NoTV, NoMac, iOS (13, 4)]
		[NoMacCatalyst]
		[Export ("showPriceConsentIfNeeded")]
		void ShowPriceConsentIfNeeded ();

		[NoWatch, NoTV, NoMac, iOS (14, 0)]
		[NoMacCatalyst]
		[Export ("presentCodeRedemptionSheet")]
		void PresentCodeRedemptionSheet ();

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("transactionObservers")]
		ISKPaymentTransactionObserver [] TransactionObservers { get; }
	}

	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
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

#if MONOMAC
		[NoWatch][NoiOS][NoTV][NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10,15, message: "Use 'IsDownloadable' instead.")]
		[Export ("downloadable")]
		bool Downloadable { get; }
#elif !NET
		[NoMac]
		[Obsolete ("Use 'IsDownloadable' instead.")]
		bool Downloadable {
			[Wrap ("IsDownloadable")]
			get;
		}
#endif

		[Mac (10, 15)]
		[MacCatalyst (13, 1)]
		[Export ("isDownloadable")]
		bool IsDownloadable { get; }

		[NoiOS]
		[NoWatch]
#if NET
		[NoTV]
#else
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'DownloadContentLengths' instead.")]
#endif
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'DownloadContentLengths' instead.")]
		[NoMacCatalyst]
		[Export ("contentLengths")]
		NSNumber [] ContentLengths { get; }

		[Mac (10, 14)]
		[MacCatalyst (13, 1)]
		[Export ("downloadContentLengths")]
		NSNumber [] DownloadContentLengths { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'DownloadContentVersion' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("contentVersion")]
		string ContentVersion { get; }

		[Mac (10, 14)]
		[MacCatalyst (13, 1)]
		[Export ("downloadContentVersion")]
		string DownloadContentVersion { get; }

		[iOS (11, 2), TV (11, 2), Mac (10, 13, 2)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("subscriptionPeriod")]
		SKProductSubscriptionPeriod SubscriptionPeriod { get; }

		[iOS (11, 2), TV (11, 2), Mac (10, 13, 2)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("introductoryPrice")]
		SKProductDiscount IntroductoryPrice { get; }

		[iOS (12, 0), TV (12, 0), Mac (10, 14)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("subscriptionGroupIdentifier")]
		string SubscriptionGroupIdentifier { get; }

		[iOS (12, 2)]
		[TV (12, 2)]
		[Mac (10, 14, 4)]
		[MacCatalyst (13, 1)]
		[Export ("discounts")]
		SKProductDiscount [] Discounts { get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("isFamilyShareable")]
		bool IsFamilyShareable { get; }
	}

	interface ISKPaymentTransactionObserver { }

	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface SKPaymentTransactionObserver {

		[Export ("paymentQueue:updatedTransactions:")]
		[Abstract]
		void UpdatedTransactions (SKPaymentQueue queue, SKPaymentTransaction [] transactions);

		[Export ("paymentQueue:removedTransactions:")]
		void RemovedTransactions (SKPaymentQueue queue, SKPaymentTransaction [] transactions);

		[Export ("paymentQueue:restoreCompletedTransactionsFailedWithError:")]
		void RestoreCompletedTransactionsFailedWithError (SKPaymentQueue queue, NSError error);

		[Export ("paymentQueueRestoreCompletedTransactionsFinished:")]
		void RestoreCompletedTransactionsFinished (SKPaymentQueue queue);

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.WatchOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("paymentQueue:updatedDownloads:")]
		void UpdatedDownloads (SKPaymentQueue queue, SKDownload [] downloads);

		[iOS (11, 0)]
		[TV (11, 0)]
		[Mac (11, 0)]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("paymentQueue:shouldAddStorePayment:forProduct:")]
		bool ShouldAddStorePayment (SKPaymentQueue queue, SKPayment payment, SKProduct product);

		[Mac (10, 15)]
		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("paymentQueueDidChangeStorefront:")]
		void DidChangeStorefront (SKPaymentQueue queue);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("paymentQueue:didRevokeEntitlementsForProductIdentifiers:")]
		void DidRevokeEntitlements (SKPaymentQueue queue, string [] productIdentifiers);
	}

	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SKPaymentTransaction {
		[NullAllowed]
		[Export ("error")]
		NSError Error { get; }

		[NullAllowed]
		[Export ("originalTransaction")]
		SKPaymentTransaction OriginalTransaction { get; }

		[Export ("payment")]
		SKPayment Payment { get; }

		[NullAllowed]
		[Export ("transactionDate")]
		NSDate TransactionDate { get; }

		[NullAllowed]
		[Export ("transactionIdentifier")]
		string TransactionIdentifier { get; }

		[NoMac]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSBundle.AppStoreReceiptUrl' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSBundle.AppStoreReceiptUrl' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSBundle.AppStoreReceiptUrl' instead.")]
		[NullAllowed]
		[Export ("transactionReceipt")]
		NSData TransactionReceipt { get; }

		[Export ("transactionState")]
		SKPaymentTransactionState TransactionState { get; }

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.WatchOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("downloads")]
		SKDownload [] Downloads { get; }
	}

	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (SKRequestDelegate) })]
	interface SKRequest {
		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		SKRequestDelegate Delegate { get; set; }

		[Export ("cancel")]
		void Cancel ();

		[Export ("start")]
		void Start ();
	}

	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface SKRequestDelegate {
		[Export ("requestDidFinish:")]
		void RequestFinished (SKRequest request);

		[Export ("request:didFailWithError:"), EventArgs ("SKRequestError")]
		void RequestFailed (SKRequest request, NSError error);
	}

	[Watch (6, 2)]
	[Mac (10, 9)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SKRequest))]
	interface SKReceiptRefreshRequest {
		[Export ("initWithReceiptProperties:")]
		NativeHandle Constructor ([NullAllowed] NSDictionary properties);

		[Wrap ("this (receiptProperties.GetDictionary ())")]
		NativeHandle Constructor ([NullAllowed] SKReceiptProperties receiptProperties);

		[NullAllowed]
		[Export ("receiptProperties")]
		NSDictionary WeakReceiptProperties { get; }

		[NullAllowed]
		[Wrap ("WeakReceiptProperties")]
		SKReceiptProperties ReceiptProperties { get; }
	}

	[Mac (10, 9)]
	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[Static, Internal]
	interface _SKReceiptProperty {
		[Field ("SKReceiptPropertyIsExpired"), Internal]
		NSString IsExpired { get; }

		[Field ("SKReceiptPropertyIsRevoked"), Internal]
		NSString IsRevoked { get; }

		[Field ("SKReceiptPropertyIsVolumePurchase"), Internal]
		NSString IsVolumePurchase { get; }
	}

	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SKRequest), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (SKProductsRequestDelegate) })]
	interface SKProductsRequest {
		[Export ("initWithProductIdentifiers:")]
		NativeHandle Constructor (NSSet productIdentifiersStringSet);

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		[New]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[New]
		[Protocolize]
		SKProductsRequestDelegate Delegate { get; set; }
	}

	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SKProductsResponse {
		[Export ("products")]
		SKProduct [] Products { get; }

		[Export ("invalidProductIdentifiers")]
		string [] InvalidProducts { get; }
	}

	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SKRequestDelegate))]
	[Model]
	[Protocol]
	interface SKProductsRequestDelegate {
		[Export ("productsRequest:didReceiveResponse:")]
		[Abstract]
		[EventArgs ("SKProductsRequestResponse")]
		void ReceivedResponse (SKProductsRequest request, SKProductsResponse response);
	}

	[Mac (11, 0), NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController),
		   Delegates = new string [] { "WeakDelegate" },
		   Events = new Type [] { typeof (SKStoreProductViewControllerDelegate) })]
	interface SKStoreProductViewController {
#if !NET
		// SKStoreProductViewController is an OS View Controller which can't be customized
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);
#endif

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		SKStoreProductViewControllerDelegate Delegate { get; set; }

		[Export ("loadProductWithParameters:completionBlock:")]
		[Internal]
		[Async]
		void LoadProduct (NSDictionary parameters, [NullAllowed] Action<bool, NSError> callback);

		[Wrap ("LoadProduct (parameters.GetDictionary ()!, callback)")]
		[Async]
		void LoadProduct (StoreProductParameters parameters, [NullAllowed] Action<bool, NSError> callback);

		[Async]
		[NoMac, iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("loadProductWithParameters:impression:completionBlock:")]
		void LoadProduct (NSDictionary parameters, SKAdImpression impression, [NullAllowed] Action<bool, NSError> callback);

		[Async]
		[NoMac, iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Wrap ("LoadProduct (parameters.GetDictionary ()!, impression, callback)")]
		void LoadProduct (StoreProductParameters parameters, SKAdImpression impression, [NullAllowed] Action<bool, NSError> callback);
	}

	[Mac (11, 0), NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface SKStoreProductViewControllerDelegate {
		[Export ("productViewControllerDidFinish:"), EventArgs ("SKStoreProductViewController")]
		void Finished (SKStoreProductViewController controller);
	}

	[NoWatch]
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("SKStoreProductParameterKey")]
	interface StoreProductParameters {

		[iOS (11, 0)]
		[TV (11, 0)]
		[MacCatalyst (13, 1)]
		[Export ("ProductIdentifier")]
		string ProductIdentifier { get; set; }

		[iOS (8, 3)]
		[MacCatalyst (13, 1)]
		[Export ("ProviderToken")]
		string ProviderToken { get; set; }

		[iOS (11, 3), TV (11, 3), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("AdNetworkAttributionSignature")]
		string AdNetworkAttributionSignature { get; set; }

		[iOS (11, 3), TV (11, 3), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("AdNetworkCampaignIdentifier")]
		uint AdNetworkCampaignIdentifier { get; set; }

		[iOS (11, 3), TV (11, 3), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("AdNetworkIdentifier")]
		string AdNetworkIdentifier { get; set; }

		[iOS (11, 3), TV (11, 3), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("AdNetworkNonce")]
		NSUuid AdNetworkNonce { get; set; }

		[iOS (11, 3), TV (11, 3), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("AdNetworkTimestamp")]
		uint AdNetworkTimestamp { get; set; }

		[NoWatch, NoMac, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("AdNetworkSourceAppStoreIdentifier")]
		string AdNetworkSourceAppStoreIdentifier { get; set; }

		[NoWatch, NoMac, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("AdNetworkVersion")]
		string AdNetworkVersion { get; set; }
	}

	[NoWatch]
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Static]
	interface SKStoreProductParameterKey {
		[Field ("SKStoreProductParameterITunesItemIdentifier")]
		NSString ITunesItemIdentifier { get; }

		[iOS (11, 0)]
		[TV (11, 0)]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterProductIdentifier")]
		NSString ProductIdentifier { get; }

		[iOS (8, 0)]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAffiliateToken")]
		NSString AffiliateToken { get; }

		[iOS (8, 0)]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterCampaignToken")]
		NSString CampaignToken { get; }

		[iOS (8, 3)]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterProviderToken")]
		NSString ProviderToken { get; }

		[iOS (9, 3)]
		[TV (9, 2)]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAdvertisingPartnerToken")]
		NSString AdvertisingPartnerToken { get; }

		[iOS (11, 3), TV (11, 3), NoMac]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAdNetworkAttributionSignature")]
		NSString AdNetworkAttributionSignature { get; }

		[iOS (11, 3), TV (11, 3), NoMac]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAdNetworkCampaignIdentifier")]
		NSString AdNetworkCampaignIdentifier { get; }

		[NoMac, iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Field ("SKStoreProductParameterAdNetworkSourceIdentifier")]
		NSString AdNetworkSourceIdentifier { get; }

		[iOS (11, 3), TV (11, 3), NoMac]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAdNetworkIdentifier")]
		NSString AdNetworkIdentifier { get; }

		[iOS (11, 3), TV (11, 3), NoMac]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAdNetworkNonce")]
		NSString AdNetworkNonce { get; }

		[iOS (11, 3), TV (11, 3), NoMac]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAdNetworkTimestamp")]
		NSString AdNetworkTimestamp { get; }

		[NoWatch, NoMac, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("SKStoreProductParameterAdNetworkSourceAppStoreIdentifier")]
		NSString AdNetworkSourceAppStoreIdentifier { get; }

		[NoWatch, NoMac, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("SKStoreProductParameterAdNetworkVersion")]
		NSString AdNetworkVersion { get; }

		[Mac (12, 0), iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("SKStoreProductParameterCustomProductPageIdentifier")]
		NSString CustomProductPageIdentifier { get; }
	}

	[iOS (10, 1)]
	[NoMac]
	[NoWatch]
	[NoTV] // __TVOS_PROHIBITED
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface SKCloudServiceSetupViewController {
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		ISKCloudServiceSetupViewControllerDelegate Delegate { get; set; }

		[Async]
		[Export ("loadWithOptions:completionHandler:")]
		void Load (NSDictionary options, [NullAllowed] Action<bool, NSError> completionHandler);

		[Async]
		[Wrap ("Load (options.GetDictionary ()!, completionHandler)")]
		void Load (SKCloudServiceSetupOptions options, Action<bool, NSError> completionHandler);
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	interface ISKCloudServiceSetupViewControllerDelegate { }

	[iOS (10, 1)]
	[NoMac]
	[NoWatch]
	[NoTV] // __TVOS_PROHIBITED on the only member + SKCloudServiceSetupViewController is not in tvOS
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SKCloudServiceSetupViewControllerDelegate {
		[Export ("cloudServiceSetupViewControllerDidDismiss:")]
		void DidDismiss (SKCloudServiceSetupViewController cloudServiceSetupViewController);
	}

	[NoMac]
	[NoWatch, NoTV, iOS (10, 1)]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("SKCloudServiceSetupOptionsKeys")]
	interface SKCloudServiceSetupOptions {
		// Headers comment: Action for setup entry point (of type SKCloudServiceSetupAction).
		// FIXME: Once https://bugzilla.xamarin.com/show_bug.cgi?id=57870 is fixed we should have a wrapper on a new property
		// `SKCloudServiceSetupAction Action { get; set; }` and avoid manual code.
		[Internal]
		[Export ("ActionKey")]
		NSString _Action { get; set; }

		// Headers comment: Identifier of the iTunes Store item the user is trying to access which requires cloud service setup (NSNumber).
		nint ITunesItemIdentifier { get; set; }

		[iOS (10, 3)]
		[MacCatalyst (13, 1)]
		string AffiliateToken { get; set; }

		[iOS (10, 3)]
		[MacCatalyst (13, 1)]
		string CampaignToken { get; set; }

		[iOS (11, 0)]
		[MacCatalyst (13, 1)]
		string MessageIdentifier { get; set; }
	}

	[NoMac]
	[NoWatch, NoTV, iOS (10, 1)]
	[MacCatalyst (13, 1)]
	[Internal, Static]
	interface SKCloudServiceSetupOptionsKeys {
		[Field ("SKCloudServiceSetupOptionsActionKey")]
		NSString ActionKey { get; }

		[Field ("SKCloudServiceSetupOptionsITunesItemIdentifierKey")]
		NSString ITunesItemIdentifierKey { get; }

		[iOS (10, 3)]
		[MacCatalyst (13, 1)]
		[Field ("SKCloudServiceSetupOptionsAffiliateTokenKey")]
		NSString AffiliateTokenKey { get; }

		[iOS (10, 3)]
		[MacCatalyst (13, 1)]
		[Field ("SKCloudServiceSetupOptionsCampaignTokenKey")]
		NSString CampaignTokenKey { get; }

		[iOS (11, 0)]
		[MacCatalyst (13, 1)]
		[Field ("SKCloudServiceSetupOptionsMessageIdentifierKey")]
		NSString MessageIdentifierKey { get; }
	}

	[NoMac]
	[NoWatch, NoTV, iOS (10, 1)]
	[MacCatalyst (13, 1)]
	enum SKCloudServiceSetupAction {
		[Field ("SKCloudServiceSetupActionSubscribe")]
		Subscribe,
	}

	[NoMac]
	[NoWatch, iOS (11, 0), TV (11, 0)]
	[MacCatalyst (13, 1)]
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

	[Mac (11, 0), Watch (7, 0), iOS (9, 3), TV (9, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
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

		[iOS (11, 0)]
		[TV (11, 0)]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("requestStorefrontCountryCodeWithCompletionHandler:")]
		void RequestStorefrontCountryCode (Action<NSString, NSError> completionHandler);

		[Async]
		[Export ("requestCapabilitiesWithCompletionHandler:")]
		void RequestCapabilities (Action<SKCloudServiceCapability, NSError> completionHandler);

		[iOS (10, 3), TV (10, 2)]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'RequestUserToken' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'RequestUserToken' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RequestUserToken' instead.")]
		[Async]
		[Export ("requestPersonalizationTokenForClientToken:withCompletionHandler:")]
		void RequestPersonalizationToken (string clientToken, Action<NSString, NSError> completionHandler);

		[iOS (11, 0)]
		[TV (11, 0)]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("requestUserTokenForDeveloperToken:completionHandler:")]
		void RequestUserToken (string developerToken, Action<NSString, NSError> completionHandler);

		[Notification]
		[Field ("SKStorefrontIdentifierDidChangeNotification")]
		NSString StorefrontIdentifierDidChangeNotification { get; }

		[Notification]
		[Field ("SKCloudServiceCapabilitiesDidChangeNotification")]
		NSString CloudServiceCapabilitiesDidChangeNotification { get; }

		[iOS (11, 0)]
		[TV (11, 0)]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("SKStorefrontCountryCodeDidChangeNotification")]
		NSString StorefrontCountryCodeDidChangeNotification { get; }
	}


	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, iOS (11, 0), TV (11, 0), Mac (11, 0)]
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
		void Update (SKProduct [] storePromotionOrder, [NullAllowed] Action<NSError> completionHandler);
	}

	[iOS (10, 3), Mac (10, 14)]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Not specified but very likely
	interface SKStoreReviewController {

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use the 'RequestReview (UIWindowScene windowScene)' API instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'RequestReview (UIWindowScene windowScene)' API instead.")]
		[Static]
		[Export ("requestReview")]
		void RequestReview ();

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[NoWatch, NoTV, iOS (14, 0), NoMac]
		[Static]
		[Export ("requestReviewInScene:")]
		void RequestReview (UIWindowScene windowScene);
	}

	[Watch (6, 2), iOS (11, 2), TV (11, 2), Mac (10, 13, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SKProductSubscriptionPeriod {

		[Export ("numberOfUnits")]
		nuint NumberOfUnits { get; }

		[Export ("unit")]
		SKProductPeriodUnit Unit { get; }
	}

	[Watch (6, 2), iOS (11, 2), TV (11, 2), Mac (10, 13, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SKProductDiscount {

		[Export ("price")]
		NSDecimalNumber Price { get; }

		[Export ("priceLocale")]
		NSLocale PriceLocale { get; }

		[iOS (12, 2)]
		[TV (12, 2)]
		[Mac (10, 14, 4)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("identifier")]
		string Identifier { get; }

		[Export ("subscriptionPeriod")]
		SKProductSubscriptionPeriod SubscriptionPeriod { get; }

		[Export ("numberOfPeriods")]
		nuint NumberOfPeriods { get; }

		[Export ("paymentMode")]
		SKProductDiscountPaymentMode PaymentMode { get; }

		[iOS (12, 2)]
		[TV (12, 2)]
		[Mac (10, 14, 4)]
		[MacCatalyst (13, 1)]
		[Export ("type")]
		SKProductDiscountType Type { get; }
	}

	[iOS (11, 3), NoTV, NoMac, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SKAdNetwork {

		[Deprecated (PlatformName.iOS, 15, 4, message: "Use 'UpdatePostback' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 4, message: "Use 'UpdatePostback' instead.")]
		[Static]
		[Export ("registerAppForAdNetworkAttribution")]
		void RegisterAppForAdNetworkAttribution ();

		[Deprecated (PlatformName.iOS, 15, 4, message: "Use 'UpdatePostback' instead.")]
		[NoWatch, NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 15, 4, message: "Use 'UpdatePostback' instead.")]
		[Static]
		[Export ("updateConversionValue:")]
		void UpdateConversionValue (nint conversionValue);

		[NoWatch, NoTV, NoMac]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Static]
		[Async]
		[Export ("startImpression:completionHandler:")]
		void StartImpression (SKAdImpression impression, [NullAllowed] Action<NSError?> completion);

		[NoWatch, NoTV, NoMac]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Static]
		[Async]
		[Export ("endImpression:completionHandler:")]
		void EndImpression (SKAdImpression impression, [NullAllowed] Action<NSError?> completion);

		[NoWatch, NoTV, NoMac, iOS (15, 4), MacCatalyst (15, 4)]
		[Static]
		[Async]
		[Export ("updatePostbackConversionValue:completionHandler:")]
		void UpdatePostback (nint conversionValue, [NullAllowed] Action<NSError?> completion);

		[NoMac, iOS (16, 1), MacCatalyst (16, 1), NoWatch, NoTV]
		[Static]
		[Async]
		[Export ("updatePostbackConversionValue:coarseValue:completionHandler:")]
		void UpdatePostback (nint conversionValue, [BindAs (typeof (SKAdNetworkCoarseConversionValue))] NSString coarseValue, [NullAllowed] Action<NSError?> completion);

		[NoMac, iOS (16, 1), MacCatalyst (16, 1), NoWatch, NoTV]
		[Static]
		[Async]
		[Export ("updatePostbackConversionValue:coarseValue:lockWindow:completionHandler:")]
		void UpdatePostback (nint conversionValue, [BindAs (typeof (SKAdNetworkCoarseConversionValue))] NSString coarseValue, bool lockWindow, [NullAllowed] Action<NSError?> completion);
	}

	[iOS (12, 2)]
	[TV (12, 2)]
	[Mac (10, 14, 4)]
	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SKPaymentDiscount {
		[Export ("initWithIdentifier:keyIdentifier:nonce:signature:timestamp:")]
		NativeHandle Constructor (string identifier, string keyIdentifier, NSUuid nonce, string signature, NSNumber timestamp);

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("keyIdentifier")]
		string KeyIdentifier { get; }

		[Export ("nonce", ArgumentSemantic.Copy)]
		NSUuid Nonce { get; }

		[Export ("signature")]
		string Signature { get; }

		[Export ("timestamp", ArgumentSemantic.Copy)]
		NSNumber Timestamp { get; }
	}

	[Watch (6, 2)]
	[iOS (12, 2)]
	[TV (12, 2)]
	[Mac (10, 14, 4)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKProductDiscountType : long {
		Introductory,
		Subscription,
	}

	[Mac (10, 15)]
	[iOS (13, 0)]
	[TV (13, 0)]
	[Watch (6, 2)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // no `init` but non-null properties
	interface SKStorefront {

		[Export ("countryCode")]
		string CountryCode { get; }

		[Export ("identifier")]
		string Identifier { get; }
	}

	interface ISKPaymentQueueDelegate { }

	[Watch (6, 2), Mac (10, 15), iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol]
	[Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface SKPaymentQueueDelegate {
		[Export ("paymentQueue:shouldContinueTransaction:inStorefront:")]
		bool ShouldContinueTransaction (SKPaymentQueue paymentQueue, SKPaymentTransaction transaction, SKStorefront newStorefront);

		[NoWatch, NoMac, NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("paymentQueueShouldShowPriceConsent:")]
		bool ShouldShowPriceConsent (SKPaymentQueue paymentQueue);
	}

	// SKArcade.h has not been part of the StoreKit.h umbrella header since it was added
	// in Xcode 11 GM is was added - but only for macOS ?!?
	// https://feedbackassistant.apple.com/feedback/7017660 - https://github.com/xamarin/maccore/issues/1913

	[NoWatch]
	[NoiOS]
	[NoTV]
	[Mac (10, 15)]
	[NoMacCatalyst]
	delegate void SKArcadeServiceRegisterHandler (NSData randomFromFP, uint /* uint32_t */ randomFromFPLength, NSData cmacOfAppPid, uint /* uint32_t */ cmacOfAppPidLength, NSError error);

	[NoWatch]
	[NoiOS]
	[NoTV]
	[Mac (10, 15)]
	[NoMacCatalyst]
	delegate void SKArcadeServiceSubscriptionHandler (NSData subscriptionStatus, uint /* uint32_t */ subscriptionStatusLength, NSData cmacOfNonce, uint /* uint32_t */ cmacOfNonceLength, NSError error);

	[Mac (10, 15)]
	[iOS (13, 0)]
	[TV (13, 0)]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // all static members so far
	interface SKArcadeService {

		[Static]
		// [Async] it'ts not a `completionHandler` and there's not documentation (e.g. number of calls)
		[Export ("registerArcadeAppWithRandomFromLib:randomFromLibLength:resultHandler:")]
		void Register (NSData randomFromLib, uint randomFromLibLength, SKArcadeServiceRegisterHandler resultHandler);

		[Static]
		// [Async] it'ts not a `completionHandler` and there's not documentation (e.g. number of calls)
		[Export ("arcadeSubscriptionStatusWithNonce:resultHandler:")]
		void GetSubscriptionStatus (ulong nonce, SKArcadeServiceSubscriptionHandler resultHandler);

		[Static]
		[Export ("repairArcadeApp")]
		void Repair ();
	}


	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SKOverlayTransitionContext {
		[Export ("addAnimationBlock:")]
		void AddAnimationBlock (Action block);

		[Export ("startFrame")]
		CGRect StartFrame { get; }

		[Export ("endFrame")]
		CGRect EndFrame { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface SKOverlayConfiguration { }

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (SKOverlayConfiguration))]
	[DisableDefaultCtor]
	interface SKOverlayAppConfiguration {
		[Export ("initWithAppIdentifier:position:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string appIdentifier, SKOverlayPosition position);

		[Export ("appIdentifier", ArgumentSemantic.Retain)]
		string AppIdentifier { get; set; }

		[NullAllowed, Export ("campaignToken", ArgumentSemantic.Retain)]
		string CampaignToken { get; set; }

		[NullAllowed, Export ("providerToken", ArgumentSemantic.Retain)]
		string ProviderToken { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed]
		[Export ("customProductPageIdentifier", ArgumentSemantic.Retain)]
		string CustomProductPageIdentifier { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed]
		[Export ("latestReleaseID", ArgumentSemantic.Retain)]
		string LatestReleaseId { get; set; }

		[Export ("position", ArgumentSemantic.Assign)]
		SKOverlayPosition Position { get; set; }

		[Export ("userDismissible")]
		bool UserDismissible { get; set; }

		[Export ("setAdditionalValue:forKey:")]
		void SetAdditionalValue ([NullAllowed] NSObject value, string key);

		[Export ("additionalValueForKey:")]
		[return: NullAllowed]
		NSObject GetAdditionalValue (string key);

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("setAdImpression:")]
		void SetAdImpression (SKAdImpression impression);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (SKOverlayConfiguration))]
	[DisableDefaultCtor]
	interface SKOverlayAppClipConfiguration {
		[Export ("initWithPosition:")]
		[DesignatedInitializer]
		NativeHandle Constructor (SKOverlayPosition position);

		[NullAllowed, Export ("campaignToken", ArgumentSemantic.Retain)]
		string CampaignToken { get; set; }

		[NullAllowed, Export ("providerToken", ArgumentSemantic.Retain)]
		string ProviderToken { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed]
		[Export ("customProductPageIdentifier", ArgumentSemantic.Retain)]
		string CustomProductPageIdentifier { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed]
		[Export ("latestReleaseID", ArgumentSemantic.Retain)]
		string LatestReleaseId { get; set; }

		[Export ("position", ArgumentSemantic.Assign)]
		SKOverlayPosition Position { get; set; }

		[Export ("setAdditionalValue:forKey:")]
		void SetAdditionalValue ([NullAllowed] NSObject value, string key);

		[Export ("additionalValueForKey:")]
		[return: NullAllowed]
		NSObject GetAdditionalValue (string key);
	}

	interface ISKOverlayDelegate { }

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface SKOverlayDelegate {
		[Export ("storeOverlay:didFailToLoadWithError:")]
		void DidFailToLoad (SKOverlay overlay, NSError error);

		[Export ("storeOverlay:willStartPresentation:")]
		void WillStartPresentation (SKOverlay overlay, SKOverlayTransitionContext transitionContext);

		[Export ("storeOverlay:didFinishPresentation:")]
		void DidFinishPresentation (SKOverlay overlay, SKOverlayTransitionContext transitionContext);

		[Export ("storeOverlay:willStartDismissal:")]
		void WillStartDismissal (SKOverlay overlay, SKOverlayTransitionContext transitionContext);

		[Export ("storeOverlay:didFinishDismissal:")]
		void DidFinishDismissal (SKOverlay overlay, SKOverlayTransitionContext transitionContext);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SKOverlay {
		[Export ("initWithConfiguration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (SKOverlayConfiguration configuration);

		[Export ("presentInScene:")]
		void PresentInScene (UIWindowScene scene);

		[Static]
		[Export ("dismissOverlayInScene:")]
		void DismissOverlayInScene (UIWindowScene scene);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ISKOverlayDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("configuration", ArgumentSemantic.Copy)]
		SKOverlayConfiguration Configuration { get; }
	}

	[NoWatch, NoTV, NoMac]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (NSObject))]
	interface SKAdImpression {

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("initWithSourceAppStoreItemIdentifier:advertisedAppStoreItemIdentifier:adNetworkIdentifier:adCampaignIdentifier:adImpressionIdentifier:timestamp:signature:version:")]
		NativeHandle Constructor (NSNumber sourceAppStoreItemIdentifier, NSNumber advertisedAppStoreItemIdentifier, string adNetworkIdentifier, NSNumber adCampaignIdentifier, string adImpressionIdentifier, NSNumber timestamp, string signature, string version);

		[Export ("sourceAppStoreItemIdentifier", ArgumentSemantic.Strong)]
		NSNumber SourceAppStoreItemIdentifier { get; set; }

		[Export ("advertisedAppStoreItemIdentifier", ArgumentSemantic.Strong)]
		NSNumber AdvertisedAppStoreItemIdentifier { get; set; }

		[Export ("adNetworkIdentifier", ArgumentSemantic.Strong)]
		string AdNetworkIdentifier { get; set; }

		[Export ("adCampaignIdentifier", ArgumentSemantic.Strong)]
		NSNumber AdCampaignIdentifier { get; set; }

		[NoMac, iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Export ("sourceIdentifier", ArgumentSemantic.Strong)]
		NSNumber SourceIdentifier { get; set; }

		[Export ("adImpressionIdentifier", ArgumentSemantic.Strong)]
		string AdImpressionIdentifier { get; set; }

		[NullAllowed, Export ("adType", ArgumentSemantic.Strong)]
		string AdType { get; set; }

		[NullAllowed, Export ("adDescription", ArgumentSemantic.Strong)]
		string AdDescription { get; set; }

		[NullAllowed, Export ("adPurchaserName", ArgumentSemantic.Strong)]
		string AdPurchaserName { get; set; }

		[Export ("timestamp", ArgumentSemantic.Strong)]
		NSNumber Timestamp { get; set; }

		[Export ("signature", ArgumentSemantic.Strong)]
		string Signature { get; set; }

		[Export ("version", ArgumentSemantic.Strong)]
		string Version { get; set; }

	}
}
