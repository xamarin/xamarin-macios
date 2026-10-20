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

using System.ComponentModel;

using BackgroundAssets;
using CoreGraphics;
using StoreKit;

#if MONOMAC
using AppKit;
using UIViewController = AppKit.NSViewController;
using UIWindowScene = Foundation.NSObject;
#else
using UIKit;
#endif

#nullable enable

namespace StoreKit {

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Internal]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface XamarinSwiftFunctions {
		[Export ("requestReview:")]
		[Static]
		[iOS (16, 0), MacCatalyst (16, 0), NoTV]
#if MONOMAC
		void RequestReview (NSViewController viewController);
#else
		void RequestReview (UIWindowScene windowScene);
#endif
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Internal]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	[iOS (14, 0), MacCatalyst (14, 0), Mac (11, 0), NoTV]
	interface XamarinWidgetKitSwiftFunctions {
		[Export ("reloadTimelinesOfKind:")]
		[Static]
		void ReloadTimelines (string kind);

		[Export ("reloadAllTimelines")]
		[Static]
		void ReloadAllTimelines ();
	}

	[ErrorDomain ("SKANErrorDomain")]
	[NoTV, NoMac, iOS (15, 4), MacCatalyst (17, 0)]
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
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 16, 0)]
	[BaseType (typeof (NSObject))]
	partial interface SKDownload {

		[MacCatalyst (13, 1)]
		[Export ("state")]
		SKDownloadState State { get; }
#if MONOMAC
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Obsolete ("Use 'State' instead.")]
		[Wrap ("State", IsVirtual = true)]
		SKDownloadState DownloadState { get; }

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'ExpectedContentLength' instead.")]
		[Export ("contentLength", ArgumentSemantic.Copy)]
		NSNumber ContentLength { get; }
#else
		[NoMac]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'State' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'State' instead.")]
		[Export ("downloadState")]
		SKDownloadState DownloadState { get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ExpectedContentLength' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ExpectedContentLength' instead.")]
		[Export ("contentLength")]
		long ContentLength { get; }
#endif

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

		[NoTV]
		[NoiOS]
		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Export ("contentURLForProductID:")]
		[Static]
		NSUrl GetContentUrlForProduct (string productId);

		[NoTV]
		[NoiOS]
		[MacCatalyst (13, 1)]
		[Export ("deleteContentForProductID:")]
		[Static]
		void DeleteContentForProduct (string productId);

		/// <summary>Represents the value associated with the constant SKDownloadTimeRemainingUnknown</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("SKDownloadTimeRemainingUnknown")]
		double TimeRemainingUnknown { get; }

		[MacCatalyst (13, 1)]
		[Export ("transaction")]
		SKPaymentTransaction Transaction { get; }
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	partial interface SKPayment : NSMutableCopying {
		[Static]
		[Export ("paymentWithProduct:")]
		SKPayment CreateFrom (SKProduct product);

		[NoMac]
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

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("applicationUsername", ArgumentSemantic.Copy)]
		string ApplicationUsername { get; }

		[MacCatalyst (13, 1)]
		[Export ("simulatesAskToBuyInSandbox")]
		bool SimulatesAskToBuyInSandbox { get; [NotImplemented ("Not available on SKPayment, only available on SKMutablePayment")] set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("paymentDiscount", ArgumentSemantic.Copy)]
		SKPaymentDiscount PaymentDiscount { get; [NotImplemented ("Not available on SKPayment, only available on SKMutablePayment")] set; }
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SKPayment))]
	[DisableDefaultCtor]
	interface SKMutablePayment {
		[Static]
		[Export ("paymentWithProduct:")]
		SKMutablePayment PaymentWithProduct (SKProduct product);

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

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("applicationUsername", ArgumentSemantic.Copy)]
		[New]
		string ApplicationUsername { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("simulatesAskToBuyInSandbox")]
		bool SimulatesAskToBuyInSandbox { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("paymentDiscount", ArgumentSemantic.Copy)]
		SKPaymentDiscount PaymentDiscount { get; set; }
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
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

		[MacCatalyst (13, 1)]
		[Export ("restoreCompletedTransactionsWithApplicationUsername:")]
		void RestoreCompletedTransactions ([NullAllowed] string username);

		[Export ("finishTransaction:")]
		void FinishTransaction (SKPaymentTransaction transaction);

		[Export ("addTransactionObserver:")]
		void AddTransactionObserver (ISKPaymentTransactionObserver observer);

		[Export ("removeTransactionObserver:")]
		void RemoveTransactionObserver (ISKPaymentTransactionObserver observer);

		[Export ("transactions")]
		SKPaymentTransaction [] Transactions { get; }

		//
		// iOS 6.0
		//
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("startDownloads:")]
		void StartDownloads (SKDownload [] downloads);

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("pauseDownloads:")]
		void PauseDownloads (SKDownload [] downloads);

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("resumeDownloads:")]
		void ResumeDownloads (SKDownload [] downloads);

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("cancelDownloads:")]
		void CancelDownloads (SKDownload [] downloads);

		[MacCatalyst (13, 1)]
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ISKPaymentQueueDelegate Delegate { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("storefront")]
		SKStorefront Storefront { get; }

		[NoTV, NoMac, iOS (13, 4)]
		[NoMacCatalyst]
		[Export ("showPriceConsentIfNeeded")]
		void ShowPriceConsentIfNeeded ();

		[NoTV, NoMac, iOS (14, 0)]
		[NoMacCatalyst]
		[Export ("presentCodeRedemptionSheet")]
		void PresentCodeRedemptionSheet ();

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("transactionObservers")]
		ISKPaymentTransactionObserver [] TransactionObservers { get; }
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
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
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'IsDownloadable' instead.")]
		[Export ("downloadable")]
		bool Downloadable { get; }
#endif

		[MacCatalyst (13, 1)]
		[Export ("isDownloadable")]
		bool IsDownloadable { get; }

		[NoiOS]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'DownloadContentLengths' instead.")]
		[NoMacCatalyst]
		[Export ("contentLengths")]
		NSNumber [] ContentLengths { get; }

		[MacCatalyst (13, 1)]
		[Export ("downloadContentLengths")]
		NSNumber [] DownloadContentLengths { get; }

		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'DownloadContentVersion' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("contentVersion")]
		string ContentVersion { get; }

		[MacCatalyst (13, 1)]
		[Export ("downloadContentVersion")]
		string DownloadContentVersion { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("subscriptionPeriod")]
		SKProductSubscriptionPeriod SubscriptionPeriod { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("introductoryPrice")]
		SKProductDiscount IntroductoryPrice { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("subscriptionGroupIdentifier")]
		string SubscriptionGroupIdentifier { get; }

		[MacCatalyst (13, 1)]
		[Export ("discounts")]
		SKProductDiscount [] Discounts { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("isFamilyShareable")]
		bool IsFamilyShareable { get; }
	}

	interface ISKPaymentTransactionObserver { }

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	interface SKPaymentTransactionObserver {

		/// <param name="queue">To be added.</param>
		/// <param name="transactions">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("paymentQueue:updatedTransactions:")]
		[Abstract]
		void UpdatedTransactions (SKPaymentQueue queue, SKPaymentTransaction [] transactions);

		/// <param name="queue">To be added.</param>
		/// <param name="transactions">To be added.</param>
		/// <summary>Method that is called after transactions have been removed from the queue.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("paymentQueue:removedTransactions:")]
		void RemovedTransactions (SKPaymentQueue queue, SKPaymentTransaction [] transactions);

		/// <param name="queue">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>Method that is called when an error occurs while restoring transactions.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("paymentQueue:restoreCompletedTransactionsFailedWithError:")]
		void RestoreCompletedTransactionsFailedWithError (SKPaymentQueue queue, NSError error);

		/// <param name="queue">To be added.</param>
		/// <summary>Method that is called after transactions have been restored.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("paymentQueueRestoreCompletedTransactionsFinished:")]
		void RestoreCompletedTransactionsFinished (SKPaymentQueue queue);

		/// <param name="queue">To be added.</param>
		/// <param name="downloads">To be added.</param>
		/// <summary>Method that is called when one or more downloads has been updated by the queue.</summary>
		/// <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		[Deprecated (PlatformName.TvOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("paymentQueue:updatedDownloads:")]
		void UpdatedDownloads (SKPaymentQueue queue, SKDownload [] downloads);

		/// <param name="queue">The payment queue on which the payment was made.</param>
		/// <param name="payment">The payment.</param>
		/// <param name="product">The product that was paid for.</param>
		/// <summary>Called to indicate that the user has started an in-app App Store purchase.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("paymentQueue:shouldAddStorePayment:forProduct:")]
		bool ShouldAddStorePayment (SKPaymentQueue queue, SKPayment payment, SKProduct product);

		[MacCatalyst (13, 1)]
		[Export ("paymentQueueDidChangeStorefront:")]
		void DidChangeStorefront (SKPaymentQueue queue);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("paymentQueue:didRevokeEntitlementsForProductIdentifiers:")]
		void DidRevokeEntitlements (SKPaymentQueue queue, string [] productIdentifiers);
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
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
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("downloads")]
		SKDownload [] Downloads { get; }
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (SKRequestDelegate) })]
	interface SKRequest {
		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the StoreKit.ISKRequestDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the StoreKit.ISKRequestDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		ISKRequestDelegate Delegate { get; set; }

		[Export ("cancel")]
		void Cancel ();

		[Export ("start")]
		void Start ();
	}

	interface ISKRequestDelegate { }

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface SKRequestDelegate {
		/// <param name="request">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("requestDidFinish:")]
		void RequestFinished (SKRequest request);

		/// <param name="request">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("request:didFailWithError:"), EventArgs ("SKRequestError", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void RequestFailed (SKRequest request, NSError error);
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SKRequest))]
	interface SKReceiptRefreshRequest {
		[Export ("initWithReceiptProperties:")]
		NativeHandle Constructor ([NullAllowed] NSDictionary properties);

		/// <param name="receiptProperties">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Wrap ("this (receiptProperties.GetDictionary ())")]
		NativeHandle Constructor ([NullAllowed] SKReceiptProperties receiptProperties);

		[NullAllowed]
		[Export ("receiptProperties")]
		NSDictionary WeakReceiptProperties { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Wrap ("WeakReceiptProperties")]
		SKReceiptProperties ReceiptProperties { get; }
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (SKRequest), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (SKProductsRequestDelegate) })]
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	interface SKProductsRequest {
		[Export ("initWithProductIdentifiers:")]
		NativeHandle Constructor (NSSet productIdentifiersStringSet);

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		[New]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the StoreKit.ISKProductsRequestDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the StoreKit.ISKProductsRequestDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		[New]
		ISKProductsRequestDelegate Delegate { get; set; }
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SKProductsResponse {
		[Export ("products")]
		SKProduct [] Products { get; }

		[Export ("invalidProductIdentifiers")]
		string [] InvalidProducts { get; }
	}

	interface ISKProductsRequestDelegate { }

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (SKRequestDelegate))]
	[Model]
	[Protocol]
	interface SKProductsRequestDelegate {
		/// <param name="request">To be added.</param>
		/// <param name="response">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("productsRequest:didReceiveResponse:")]
		[Abstract]
		[EventArgs ("SKProductsRequestResponse", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void ReceivedResponse (SKProductsRequest request, SKProductsResponse response);
	}

	/// <summary>A subclass of <see cref="UIViewController" /> that presents a store allowing the application user to purchase an item from the App Store.</summary>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/StoreKit/Reference/SKITunesProductViewController_Ref/index.html">Apple documentation for <c>SKStoreProductViewController</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController),
		   Delegates = new string [] { "WeakDelegate" },
		   Events = new Type [] { typeof (SKStoreProductViewControllerDelegate) })]
	interface SKStoreProductViewController {
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the StoreKit.ISKStoreProductViewControllerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the StoreKit.ISKStoreProductViewControllerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		ISKStoreProductViewControllerDelegate Delegate { get; set; }

		[Export ("loadProductWithParameters:completionBlock:")]
		[Internal]
		[Async]
		void LoadProduct (NSDictionary parameters, [NullAllowed] Action<bool, NSError> callback);

		/// <param name="parameters">To be added.</param>
		///         <param name="callback">To be added.</param>
		///         <summary>Loads the product that is specified by the specified product <paramref name="parameters" /> and runs the provided <paramref name="callback" /> when the operation completes.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("LoadProduct (parameters.GetDictionary ()!, callback)")]
		[Async (XmlDocs = """
			<param name="parameters">To be added.</param>
			<summary>Returns a task that loads the product that is specified by the specified product <paramref name="parameters" />.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
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

	interface ISKStoreProductViewControllerDelegate { }

	/// <summary>A delegate object that allows the application developer to customize the behavior of a <see cref="StoreKit.SKStoreProductViewController" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/StoreKit/Reference/SKITunesProductViewControllerDelegate_ProtocolRef/index.html">Apple documentation for <c>SKStoreProductViewControllerDelegate</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface SKStoreProductViewControllerDelegate {
		/// <param name="controller">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("productViewControllerDidFinish:"), EventArgs ("SKStoreProductViewController", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void Finished (SKStoreProductViewController controller);
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("SKStoreProductParameterKey")]
	interface StoreProductParameters {

		[MacCatalyst (13, 1)]
		[Export ("ProductIdentifier")]
		string ProductIdentifier { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("ProviderToken")]
		string ProviderToken { get; set; }

		/// <summary>Gets or sets the ad network's cryptographic signature. Used for attribution.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("AdNetworkAttributionSignature")]
		string AdNetworkAttributionSignature { get; set; }

		/// <summary>Gets or sets the ad network campaign.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("AdNetworkCampaignIdentifier")]
		uint AdNetworkCampaignIdentifier { get; set; }

		/// <summary>Gets or sets the ad network's unique ID.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("AdNetworkIdentifier")]
		string AdNetworkIdentifier { get; set; }

		/// <summary>Gets or sets a cryptographic nonce value.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("AdNetworkNonce")]
		NSUuid AdNetworkNonce { get; set; }

		/// <summary>Gets or sets a key for the time of the ad impression.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("AdNetworkTimestamp")]
		uint AdNetworkTimestamp { get; set; }

		[NoMac, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("AdNetworkSourceAppStoreIdentifier")]
		string AdNetworkSourceAppStoreIdentifier { get; set; }

		[NoMac, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("AdNetworkVersion")]
		string AdNetworkVersion { get; set; }
	}

	/// <summary>Encapsulates the iTunes identifier for the item that the store should display when the application is displaying a <see cref="StoreKit.SKStoreProductViewController" />.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Static]
	interface SKStoreProductParameterKey {
		/// <summary>Represents the value associated with the constant SKStoreProductParameterITunesItemIdentifier</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("SKStoreProductParameterITunesItemIdentifier")]
		NSString ITunesItemIdentifier { get; }

		/// <summary>Represents the value associated with the constant SKStoreProductParameterProductIdentifier.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterProductIdentifier")]
		NSString ProductIdentifier { get; }

		/// <summary>Represents the value associated with the constant SKStoreProductParameterAffiliateToken</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAffiliateToken")]
		NSString AffiliateToken { get; }

		/// <summary>Represents the value associated with the constant SKStoreProductParameterCampaignToken</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterCampaignToken")]
		NSString CampaignToken { get; }

		/// <summary>TRepresents the value associated with the constant SKStoreProductParameterProviderToken.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterProviderToken")]
		NSString ProviderToken { get; }

		/// <summary>Represents the value associated with the constant SKStoreProductParameterAdvertisingPartnerToken.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAdvertisingPartnerToken")]
		NSString AdvertisingPartnerToken { get; }

		/// <summary>Represents the value associated with the constant SKStoreProductParameterAdNetworkAttributionSignature.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAdNetworkAttributionSignature")]
		NSString AdNetworkAttributionSignature { get; }

		/// <summary>Represents the value associated with the constant SKStoreProductParameterAdNetworkCampaignIdentifier.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAdNetworkCampaignIdentifier")]
		NSString AdNetworkCampaignIdentifier { get; }

		[NoMac, iOS (16, 1), MacCatalyst (16, 1), TV (16, 1)]
		[Field ("SKStoreProductParameterAdNetworkSourceIdentifier")]
		NSString AdNetworkSourceIdentifier { get; }

		/// <summary>Represents the value associated with the constant SKStoreProductParameterAdNetworkIdentifier.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAdNetworkIdentifier")]
		NSString AdNetworkIdentifier { get; }

		/// <summary>Represents the value associated with the constant SKStoreProductParameterAdNetworkNonce.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAdNetworkNonce")]
		NSString AdNetworkNonce { get; }

		/// <summary>Represents the value associated with the constant SKStoreProductParameterAdNetworkTimestamp.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("SKStoreProductParameterAdNetworkTimestamp")]
		NSString AdNetworkTimestamp { get; }

		[NoMac, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("SKStoreProductParameterAdNetworkSourceAppStoreIdentifier")]
		NSString AdNetworkSourceAppStoreIdentifier { get; }

		[NoMac, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("SKStoreProductParameterAdNetworkVersion")]
		NSString AdNetworkVersion { get; }

		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("SKStoreProductParameterCustomProductPageIdentifier")]
		NSString CustomProductPageIdentifier { get; }
	}

	/// <summary>A view controller for setting up cloud services.</summary>
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires SwiftUI */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires SwiftUI */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires SwiftUI */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires SwiftUI */ )]
	[NoMac]
	[NoTV] // __TVOS_PROHIBITED
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface SKCloudServiceSetupViewController {
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		ISKCloudServiceSetupViewControllerDelegate Delegate { get; set; }

		[Async (XmlDocs = """
			<param name="options">A dictionary of setup options.</param>
			<summary>Loads a setup view with the specified <paramref name="options" /> and runs a handler when the view is loaded.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous Load operation.  The value of the TResult parameter is of type System.Action&lt;System.Boolean,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The LoadAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		[Export ("loadWithOptions:completionHandler:")]
		void Load (NSDictionary options, [NullAllowed] Action<bool, NSError> completionHandler);

		/// <param name="options">Setup options object.</param>
		///         <param name="completionHandler">
		///           <para>A handler to run after the load operation completes</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Loads a setup view with the specified <paramref name="options" /> and runs a handler when the view is loaded.</summary>
		///         <remarks>To be added.</remarks>
		[Async (XmlDocs = """
			<param name="options">Setup options object.</param>
			<summary>Asynchronously loads a setup view with the specified <paramref name="options" />, returning a task that indicates success or failure and includes an error, if one occurred.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		[Wrap ("Load (options.GetDictionary ()!, completionHandler)")]
		void Load (SKCloudServiceSetupOptions options, Action<bool, NSError> completionHandler);
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	interface ISKCloudServiceSetupViewControllerDelegate { }

	/// <summary>Contains methods to report the status of a cloud service setup view.</summary>
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires SwiftUI */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires SwiftUI */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires SwiftUI */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires SwiftUI */ )]
	[NoMac]
	[NoTV] // __TVOS_PROHIBITED on the only member + SKCloudServiceSetupViewController is not in tvOS
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SKCloudServiceSetupViewControllerDelegate {
		/// <param name="cloudServiceSetupViewController">The view controller that was dismissed.</param>
		/// <summary>Method that is called after the setup view has been dismissed.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("cloudServiceSetupViewControllerDidDismiss:")]
		void DidDismiss (SKCloudServiceSetupViewController cloudServiceSetupViewController);
	}

	/// <summary>Contains cloud service setup options.</summary>
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[NoMac]
	[NoTV]
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
		/// <summary>Gets or sets the identifier for the item for which access is being requested.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		nint ITunesItemIdentifier { get; set; }

		/// <summary>Gets or sets the affiliate token.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		string AffiliateToken { get; set; }

		/// <summary>Gets or sets the campaign token.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		string CampaignToken { get; set; }

		/// <summary>Gets or sets the setup message identifier.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		string MessageIdentifier { get; set; }
	}

	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Internal, Static]
	interface SKCloudServiceSetupOptionsKeys {
		[Field ("SKCloudServiceSetupOptionsActionKey")]
		NSString ActionKey { get; }

		[Field ("SKCloudServiceSetupOptionsITunesItemIdentifierKey")]
		NSString ITunesItemIdentifierKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("SKCloudServiceSetupOptionsAffiliateTokenKey")]
		NSString AffiliateTokenKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("SKCloudServiceSetupOptionsCampaignTokenKey")]
		NSString CampaignTokenKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("SKCloudServiceSetupOptionsMessageIdentifierKey")]
		NSString MessageIdentifierKey { get; }
	}

	/// <summary>Enumerates Cloud Service setup actions.</summary>
	[NoMac]
	[NoTV]
	[MacCatalyst (13, 1)]
	enum SKCloudServiceSetupAction {
		/// <summary>Indicates a subscription action in a setup view.</summary>
		[Field ("SKCloudServiceSetupActionSubscribe")]
		Subscribe,
	}

	/// <summary>Enumerates cloud service setup message identifiers.</summary>
	/// <remarks>To be added.</remarks>
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[NoMac]
	[MacCatalyst (13, 1)]
	enum SKCloudServiceSetupMessageIdentifier {
		/// <summary>Indicates a message for joining.</summary>
		[Field ("SKCloudServiceSetupMessageIdentifierJoin")]
		Join,
		/// <summary>Indicates a message for connecting.</summary>
		[Field ("SKCloudServiceSetupMessageIdentifierConnect")]
		Connect,
		/// <summary>Indicates a message for adding music</summary>
		[Field ("SKCloudServiceSetupMessageIdentifierAddMusic")]
		AddMusic,
		/// <summary>Indicates a message for playing.</summary>
		[Field ("SKCloudServiceSetupMessageIdentifierPlayMusic")]
		PlayMusic,
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SKCloudServiceController {
		[Static]
		[Export ("authorizationStatus")]
		SKCloudServiceAuthorizationStatus AuthorizationStatus { get; }

		[Static]
		[Async (XmlDocs = """
			<summary>Requests permission from the user to access the device's music library.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous RequestAuthorization operation.  The value of the TResult parameter is of type System.Action&lt;StoreKit.SKCloudServiceAuthorizationStatus&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("requestAuthorization:")]
		void RequestAuthorization (Action<SKCloudServiceAuthorizationStatus> handler);

		[Async (XmlDocs = """
			<summary>Requests the storefront identifier for the device.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous RequestStorefrontIdentifier operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSString,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("requestStorefrontIdentifierWithCompletionHandler:")]
		void RequestStorefrontIdentifier (Action<NSString, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<summary>Requests the country code for the user's iTunes account and passes the code and an error, if present, to the provided handler.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous RequestStorefrontCountryCode operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSString,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("requestStorefrontCountryCodeWithCompletionHandler:")]
		void RequestStorefrontCountryCode (Action<NSString, NSError> completionHandler);

		[Async (XmlDocs = """
			<summary>Requests the current capabilities of the music library on the device.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous RequestCapabilities operation.  The value of the TResult parameter is of type System.Action&lt;StoreKit.SKCloudServiceCapability,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("requestCapabilitiesWithCompletionHandler:")]
		void RequestCapabilities (Action<SKCloudServiceCapability, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<param name="clientToken">To be added.</param>
			<summary>Developers should not use this deprecated method. Developers should use 'RequestUserToken' instead.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous RequestPersonalizationToken operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSString,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("requestPersonalizationTokenForClientToken:withCompletionHandler:")]
		void RequestPersonalizationToken (string clientToken, Action<NSString, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<param name="developerToken">The JWT token to authenticate the developer.</param>
			<summary>Requests the user code for accessing personalized music content, passing the code and an error, if present, to the provided handler.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous RequestUserToken operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSString,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The RequestUserTokenAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		[Export ("requestUserTokenForDeveloperToken:completionHandler:")]
		void RequestUserToken (string developerToken, Action<NSString, NSError> completionHandler);

		[Notification]
		[Field ("SKStorefrontIdentifierDidChangeNotification")]
		NSString StorefrontIdentifierDidChangeNotification { get; }

		[Notification]
		[Field ("SKCloudServiceCapabilitiesDidChangeNotification")]
		NSString CloudServiceCapabilitiesDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("SKStorefrontCountryCodeDidChangeNotification")]
		NSString StorefrontCountryCodeDidChangeNotification { get; }
	}


	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // static Default property is the only documented way to get the controller
	interface SKProductStorePromotionController {
		[Static]
		[Export ("defaultController")]
		SKProductStorePromotionController Default { get; }

		[Async (XmlDocs = """
			<param name="product">The product whose visibility to fetch.</param>
			<summary>Fetches the visibility of the specified product on the device.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous FetchStorePromotionVisibility operation.  The value of the TResult parameter is of type System.Action&lt;StoreKit.SKProductStorePromotionVisibility,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("fetchStorePromotionVisibilityForProduct:completionHandler:")]
		void FetchStorePromotionVisibility (SKProduct product, [NullAllowed] Action<SKProductStorePromotionVisibility, NSError> completionHandler);

		[Async (XmlDocs = """
			<param name="promotionVisibility">The new visibility.</param>
			<param name="product">The product whose visibility to update.</param>
			<summary>Updates the visibility of the specified product on the device.</summary>
			<returns>A task that represents the asynchronous Update operation</returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("updateStorePromotionVisibility:forProduct:completionHandler:")]
		void Update (SKProductStorePromotionVisibility promotionVisibility, SKProduct product, [NullAllowed] Action<NSError> completionHandler);

		[Async (XmlDocs = """
			<summary>Fetches the override that controls the product order on the device.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous FetchStorePromotionOrder operation.  The value of the TResult parameter is of type System.Action&lt;StoreKit.SKProduct[],Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("fetchStorePromotionOrderWithCompletionHandler:")]
		void FetchStorePromotionOrder ([NullAllowed] Action<SKProduct [], NSError> completionHandler);

		[Async (XmlDocs = """
			<param name="storePromotionOrder">An array of products in the desired order.</param>
			<summary>Updates the product order on the device.</summary>
			<returns>A task that represents the asynchronous Update operation</returns>
			<remarks>
			          <para copied="true">The UpdateAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		[Export ("updateStorePromotionOrder:completionHandler:")]
		void Update (SKProduct [] storePromotionOrder, [NullAllowed] Action<NSError> completionHandler);
	}

	/// <summary>Contains a method that requests a review.</summary>
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Not specified but very likely
	interface SKStoreReviewController {

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use the 'RequestReview (UIWindowScene windowScene)' API instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'RequestReview (UIWindowScene windowScene)' API instead.")]
		[Static]
		[Export ("requestReview")]
		void RequestReview ();

		[Deprecated (PlatformName.iOS, 18, 0, message: "Use the 'AppStore.RequestReview (UIWindowScene)' API instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use the 'AppStore.RequestReview (UIWindowScene)' API instead.")]
		[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use the 'AppStore.RequestReview (NSViewController)' API instead.")]
		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[NoTV, iOS (14, 0), NoMac]
		[Static]
		[Export ("requestReviewInScene:")]
		void RequestReview (UIWindowScene windowScene);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	interface SKProductSubscriptionPeriod {

		[Export ("numberOfUnits")]
		nuint NumberOfUnits { get; }

		[Export ("unit")]
		SKProductPeriodUnit Unit { get; }
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SKProductDiscount {

		[Export ("price")]
		NSDecimalNumber Price { get; }

		[Export ("priceLocale")]
		NSLocale PriceLocale { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("identifier")]
		string Identifier { get; }

		[Export ("subscriptionPeriod")]
		SKProductSubscriptionPeriod SubscriptionPeriod { get; }

		[Export ("numberOfPeriods")]
		nuint NumberOfPeriods { get; }

		[Export ("paymentMode")]
		SKProductDiscountPaymentMode PaymentMode { get; }

		[MacCatalyst (13, 1)]
		[Export ("type")]
		SKProductDiscountType Type { get; }
	}

	[NoTV, NoMac]
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
		[NoTV, NoMac, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 15, 4, message: "Use 'UpdatePostback' instead.")]
		[Static]
		[Export ("updateConversionValue:")]
		void UpdateConversionValue (nint conversionValue);

		[NoTV, NoMac]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Static]
		[Async]
		[Export ("startImpression:completionHandler:")]
		void StartImpression (SKAdImpression impression, [NullAllowed] Action<NSError?> completion);

		[NoTV, NoMac]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Static]
		[Async]
		[Export ("endImpression:completionHandler:")]
		void EndImpression (SKAdImpression impression, [NullAllowed] Action<NSError?> completion);

		[NoTV, NoMac, iOS (15, 4), MacCatalyst (15, 4)]
		[Static]
		[Async]
		[Export ("updatePostbackConversionValue:completionHandler:")]
		void UpdatePostback (nint conversionValue, [NullAllowed] Action<NSError?> completion);

		[NoMac, iOS (16, 1), MacCatalyst (16, 1), NoTV]
		[Static]
		[Async]
		[Export ("updatePostbackConversionValue:coarseValue:completionHandler:")]
		void UpdatePostback (nint conversionValue, [BindAs (typeof (SKAdNetworkCoarseConversionValue))] NSString coarseValue, [NullAllowed] Action<NSError?> completion);

		[NoMac, iOS (16, 1), MacCatalyst (16, 1), NoTV]
		[Static]
		[Async]
		[Export ("updatePostbackConversionValue:coarseValue:lockWindow:completionHandler:")]
		void UpdatePostback (nint conversionValue, [BindAs (typeof (SKAdNetworkCoarseConversionValue))] NSString coarseValue, bool lockWindow, [NullAllowed] Action<NSError?> completion);
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
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

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[Native]
	public enum SKProductDiscountType : long {
		/// <summary>To be added.</summary>
		Introductory,
		/// <summary>To be added.</summary>
		Subscription,
	}

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
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

	[Deprecated (PlatformName.iOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacCatalyst, 18, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.MacOSX, 15, 0 /* Apple's replacement requires Swift */ )]
	[Deprecated (PlatformName.TvOS, 18, 0 /* Apple's replacement requires Swift */ )]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SKPaymentQueueDelegate {
		[Export ("paymentQueue:shouldContinueTransaction:inStorefront:")]
		bool ShouldContinueTransaction (SKPaymentQueue paymentQueue, SKPaymentTransaction transaction, SKStorefront newStorefront);

		[NoMac, NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("paymentQueueShouldShowPriceConsent:")]
		bool ShouldShowPriceConsent (SKPaymentQueue paymentQueue);
	}

	// SKArcade.h has not been part of the StoreKit.h umbrella header since it was added
	// in Xcode 11 GM is was added - but only for macOS ?!?
	// https://feedbackassistant.apple.com/feedback/7017660 - https://github.com/xamarin/maccore/issues/1913

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	delegate void SKArcadeServiceRegisterHandler ([NullAllowed] NSData randomFromFP, uint /* uint32_t */ randomFromFPLength, [NullAllowed] NSData cmacOfAppPid, uint /* uint32_t */ cmacOfAppPidLength, [NullAllowed] NSError error);

	[NoiOS]
	[NoTV]
	[NoMacCatalyst]
	delegate void SKArcadeServiceSubscriptionHandler ([NullAllowed] NSData subscriptionStatus, uint /* uint32_t */ subscriptionStatusLength, [NullAllowed] NSData cmacOfNonce, uint /* uint32_t */ cmacOfNonceLength, [NullAllowed] NSError error);

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


	[NoTV, NoMac, iOS (14, 0)]
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

	[NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface SKOverlayConfiguration { }

	[NoTV, NoMac, iOS (14, 0)]
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

	[NoTV, NoMac, iOS (14, 0)]
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

	[NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol, Model]
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

	[NoTV, NoMac, iOS (14, 0)]
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

	[NoTV, NoMac]
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

		[NoMac, iOS (16, 1), MacCatalyst (16, 1), NoTV]
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

	[iOS (26, 0), Mac (26, 0), TV (26, 0), MacCatalyst (26, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface SKDownloaderExtension : BAManagedDownloaderExtension {
	}
}
