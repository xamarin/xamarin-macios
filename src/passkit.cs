//
// PassKit bindings
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012, 2015-2016 Xamarin Inc. All rights reserved.
// Copyright 2020 Microsoft Corp.
//

using System;
using System.ComponentModel;
using Contacts;
using CoreGraphics;
using ObjCRuntime;
using Foundation;
#if MONOMAC
using AppKit;
using ABRecord = Foundation.NSObject;
using UIButton = AppKit.NSButton;
using UIImage = AppKit.NSImage;
using UIViewController = AppKit.NSViewController;
using UIWindow = AppKit.NSWindow;
using UIControl = AppKit.NSControl;
using UIView = AppKit.NSView;
#else
using UIKit;
#if IOS
using AddressBook;
#else
using ABRecord = Foundation.NSObject;
using UIViewController = Foundation.NSObject;
using UIWindow = Foundation.NSObject;
using UIControl = Foundation.NSObject;
#endif // IOS
#endif // MONOMAC

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace PassKit {

	[Mac (11, 0)] // mention 10.12 but the framework was not available on macOS at that time
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PKContact : NSSecureCoding {
		[NullAllowed, Export ("name", ArgumentSemantic.Strong)]
		NSPersonNameComponents Name { get; set; }

		[NullAllowed, Export ("postalAddress", ArgumentSemantic.Retain)]
		CNPostalAddress PostalAddress { get; set; }

		[NullAllowed, Export ("emailAddress", ArgumentSemantic.Strong)]
		string EmailAddress { get; set; }

		[NullAllowed, Export ("phoneNumber", ArgumentSemantic.Strong)]
		CNPhoneNumber PhoneNumber { get; set; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 10, 3, message: "Use 'SubLocality' and 'SubAdministrativeArea' on 'PostalAddress' instead.")]
		[Deprecated (PlatformName.WatchOS, 3, 2, message: "Use 'SubLocality' and 'SubAdministrativeArea' on 'PostalAddress' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SubLocality' and 'SubAdministrativeArea' on 'PostalAddress' instead.")]
		[NullAllowed, Export ("supplementarySubLocality", ArgumentSemantic.Strong)]
		string SupplementarySubLocality { get; set; }
	}

	[Mac (11, 0)]
	[Watch (6, 2), iOS (13, 4)]
	[MacCatalyst (13, 1)]
	delegate void PKPassLibrarySignDataCompletionHandler (NSData signedData, NSData signature, NSError error);

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PKPassLibrary {
		[Static]
		[Export ("isPassLibraryAvailable")]
		bool IsAvailable { get; }

		[Export ("containsPass:")]
		bool Contains (PKPass pass);

		[Export ("passes")]
		PKPass [] GetPasses ();

		[Export ("passWithPassTypeIdentifier:serialNumber:")]
		[return: NullAllowed]
		PKPass GetPass (string identifier, string serialNumber);

		[MacCatalyst (13, 1)]
		[Export ("passesOfType:")]
		PKPass [] GetPasses (PKPassType passType);

		[Export ("removePass:")]
		void Remove (PKPass pass);

		[Export ("replacePassWithPass:")]
		bool Replace (PKPass pass);

		[Export ("addPasses:withCompletionHandler:")]
		[Async]
		void AddPasses (PKPass [] passes, [NullAllowed] Action<PKPassLibraryAddPassesStatus> completion);

		[Field ("PKPassLibraryDidChangeNotification")]
		[Notification]
		NSString DidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Field ("PKPassLibraryRemotePaymentPassesDidChangeNotification")]
		[Notification]
		NSString RemotePaymentPassesDidChangeNotification { get; }

		[NoMac]
		[Static, Export ("isPaymentPassActivationAvailable")]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use the library's instance 'IsLibraryPaymentPassActivationAvailable' property instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the library's instance 'IsLibraryPaymentPassActivationAvailable' property instead.")]
		bool IsPaymentPassActivationAvailable { get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'SecureElementPassActivationAvailable' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 2, message: "Use 'SecureElementPassActivationAvailable' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SecureElementPassActivationAvailable' instead.")]
		[Export ("isPaymentPassActivationAvailable")]
		bool IsLibraryPaymentPassActivationAvailable { get; }

		[Watch (6, 2), iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("secureElementPassActivationAvailable")]
		bool SecureElementPassActivationAvailable { [Bind ("isSecureElementPassActivationAvailable")] get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'ActivateSecureElementPass' instead.")]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ActivateSecureElementPass' instead.")]
		[Async]
		[Export ("activatePaymentPass:withActivationData:completion:")]
		void ActivatePaymentPass (PKPaymentPass paymentPass, NSData activationData, [NullAllowed] Action<bool, NSError> completion);

		[Async]
		[NoWatch, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("activateSecureElementPass:withActivationData:completion:")]
		void ActivateSecureElementPass (PKSecureElementPass secureElementPass, NSData activationData, [NullAllowed] Action<bool, NSError> completion);

		[NoMac]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'ActivatePaymentPass (PKPaymentPass, NSData, Action<bool, NSError> completion)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ActivatePaymentPass (PKPaymentPass, NSData, Action<bool, NSError> completion)' instead.")]
		[Async]
		[Export ("activatePaymentPass:withActivationCode:completion:")]
		void ActivatePaymentPass (PKPaymentPass paymentPass, string activationCode, [NullAllowed] Action<bool, NSError> completion);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("openPaymentSetup")]
		void OpenPaymentSetup ();

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'CanAddSecureElementPass' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 2, message: "Use 'CanAddSecureElementPass' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CanAddSecureElementPass' instead.")]
		[Export ("canAddPaymentPassWithPrimaryAccountIdentifier:")]
		bool CanAddPaymentPass (string primaryAccountIdentifier);

		[Watch (6, 2), iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("canAddSecureElementPassWithPrimaryAccountIdentifier:")]
		bool CanAddSecureElementPass (string primaryAccountIdentifier);

		[MacCatalyst (13, 1)]
		[Export ("canAddFelicaPass")]
		bool CanAddFelicaPass { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("endAutomaticPassPresentationSuppressionWithRequestToken:")]
		void EndAutomaticPassPresentationSuppression (nuint requestToken);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("isSuppressingAutomaticPassPresentation")]
		bool IsSuppressingAutomaticPassPresentation { get; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'RemoteSecureElementPasses' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 2, message: "Use 'RemoteSecureElementPasses' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RemoteSecureElementPasses' instead.")]
		[Export ("remotePaymentPasses")]
		PKPaymentPass [] RemotePaymentPasses { get; }

		[Watch (6, 2), iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("remoteSecureElementPasses", ArgumentSemantic.Copy)]
		PKSecureElementPass [] RemoteSecureElementPasses { get; }

#if !WATCH
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("requestAutomaticPassPresentationSuppressionWithResponseHandler:")]
		nuint RequestAutomaticPassPresentationSuppression (Action<PKAutomaticPassPresentationSuppressionResult> responseHandler);
#endif

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'PresentSecureElementPass' instead.")]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PresentSecureElementPass' instead.")]
		[Export ("presentPaymentPass:")]
		void PresentPaymentPass (PKPaymentPass pass);

		[NoWatch, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("presentSecureElementPass:")]
		void PresentSecureElementPass (PKSecureElementPass pass);

		[Async (ResultTypeName = "PKSignDataCompletionResult")]
		[Watch (6, 2), iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("signData:withSecureElementPass:completion:")]
		void SignData (NSData signData, PKSecureElementPass secureElementPass, PKPassLibrarySignDataCompletionHandler completion);

		[Async (ResultTypeName = "PKServiceProviderDataCompletionResult")]
		[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		[Export ("serviceProviderDataForSecureElementPass:completion:")]
		void GetServiceProviderData (PKSecureElementPass secureElementPass, Action<NSData, NSError> completion);

		[Async]
		[Watch (9, 0), iOS (16, 0), MacCatalyst (16, 0), Mac (13, 0), NoTV]
		[Export ("encryptedServiceProviderDataForSecureElementPass:completion:")]
		void GetEncryptedServiceProviderData (PKSecureElementPass secureElementPass, Action<NSDictionary, NSError> completion);
	}

	[Static]
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	interface PKPassLibraryUserInfoKey {
		[Field ("PKPassLibraryAddedPassesUserInfoKey")]
		NSString AddedPasses { get; }

		[Field ("PKPassLibraryReplacementPassesUserInfoKey")]
		NSString ReplacementPasses { get; }

		[Field ("PKPassLibraryRemovedPassInfosUserInfoKey")]
		NSString RemovedPassInfos { get; }

		[Field ("PKPassLibraryPassTypeIdentifierUserInfoKey")]
		NSString PassTypeIdentifier { get; }

		[Field ("PKPassLibrarySerialNumberUserInfoKey")]
		NSString SerialNumber { get; }

		[Watch (8, 3), iOS (15, 2), Mac (12, 1), MacCatalyst (15, 2)]
		[Field ("PKPassLibraryRecoveredPassesUserInfoKey")]
		NSString RecoveredPasses { get; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PKPayment {
		[Export ("token", ArgumentSemantic.Strong)]
		PKPaymentToken Token { get; }

		[NoMac]
		[NoMacCatalyst]
		[NoWatch]
		[Export ("billingAddress", ArgumentSemantic.Assign)]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'BillingContact' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'BillingContact' instead.")]
		ABRecord BillingAddress { get; }

		[NoMac]
		[NoMacCatalyst]
		[NoWatch]
		[Export ("shippingAddress", ArgumentSemantic.Assign)]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'ShippingContact' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ShippingContact' instead.")]
		ABRecord ShippingAddress { get; }

		[NullAllowed, Export ("shippingMethod", ArgumentSemantic.Strong)]
		PKShippingMethod ShippingMethod { get; }


		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("shippingContact", ArgumentSemantic.Strong)]
		PKContact ShippingContact { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("billingContact", ArgumentSemantic.Strong)]
		PKContact BillingContact { get; }
	}

#if !WATCH
	delegate void PKPaymentShippingAddressSelected (PKPaymentAuthorizationStatus status, PKShippingMethod [] shippingMethods, PKPaymentSummaryItem [] summaryItems);
	delegate void PKPaymentShippingMethodSelected (PKPaymentAuthorizationStatus status, PKPaymentSummaryItem [] summaryItems);

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface PKPaymentAuthorizationViewControllerDelegate {

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'DidAuthorizePayment2' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidAuthorizePayment2' instead.")]
		[Export ("paymentAuthorizationViewController:didAuthorizePayment:completion:")]
		[EventArgs ("PKPaymentAuthorization")]
#if !NET
		[Abstract]
#endif
		void DidAuthorizePayment (PKPaymentAuthorizationViewController controller, PKPayment payment, Action<PKPaymentAuthorizationStatus> completion);

		[MacCatalyst (13, 1)]
		[Export ("paymentAuthorizationViewController:didAuthorizePayment:handler:")]
		[EventArgs ("PKPaymentAuthorizationResult")]
		void DidAuthorizePayment2 (PKPaymentAuthorizationViewController controller, PKPayment payment, Action<PKPaymentAuthorizationResult> completion);

		[Export ("paymentAuthorizationViewControllerDidFinish:")]
		[Abstract]
		void PaymentAuthorizationViewControllerDidFinish (PKPaymentAuthorizationViewController controller);

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'DidSelectShippingMethod2' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidSelectShippingMethod2' instead.")]
		[Export ("paymentAuthorizationViewController:didSelectShippingMethod:completion:")]
		[EventArgs ("PKPaymentShippingMethodSelected")]
		void DidSelectShippingMethod (PKPaymentAuthorizationViewController controller, PKShippingMethod shippingMethod, PKPaymentShippingMethodSelected completion);

		[MacCatalyst (13, 1)]
		[Export ("paymentAuthorizationViewController:didSelectShippingMethod:handler:")]
		[EventArgs ("PKPaymentRequestShippingMethodUpdate")]
		void DidSelectShippingMethod2 (PKPaymentAuthorizationViewController controller, PKShippingMethod shippingMethod, Action<PKPaymentRequestShippingMethodUpdate> completion);

		[NoMacCatalyst]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[NoMac]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("paymentAuthorizationViewController:didSelectShippingAddress:completion:")]
		[EventArgs ("PKPaymentShippingAddressSelected")]
		void DidSelectShippingAddress (PKPaymentAuthorizationViewController controller, ABRecord address, PKPaymentShippingAddressSelected completion);

		[MacCatalyst (13, 1)]
		[Export ("paymentAuthorizationViewControllerWillAuthorizePayment:")]
#if !NET
		[Abstract]
#endif
		void WillAuthorizePayment (PKPaymentAuthorizationViewController controller);

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'DidSelectShippingContact' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidSelectShippingContact' instead.")]
		[Export ("paymentAuthorizationViewController:didSelectShippingContact:completion:")]
		[EventArgs ("PKPaymentSelectedContact")]
		void DidSelectShippingContact (PKPaymentAuthorizationViewController controller, PKContact contact, PKPaymentShippingAddressSelected completion);

		[MacCatalyst (13, 1)]
		[Export ("paymentAuthorizationViewController:didSelectShippingContact:handler:")]
		[EventArgs ("PKPaymentRequestShippingContactUpdate")]
		void DidSelectShippingContact2 (PKPaymentAuthorizationViewController controller, PKContact contact, Action<PKPaymentRequestShippingContactUpdate> completion);

		[NoMac]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'DidSelectPaymentMethod2' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidSelectPaymentMethod2' instead.")]
		[Export ("paymentAuthorizationViewController:didSelectPaymentMethod:completion:")]
		[EventArgs ("PKPaymentMethodSelected")]
		void DidSelectPaymentMethod (PKPaymentAuthorizationViewController controller, PKPaymentMethod paymentMethod, Action<PKPaymentSummaryItem []> completion);

		[MacCatalyst (13, 1)]
		[Export ("paymentAuthorizationViewController:didSelectPaymentMethod:handler:")]
		[EventArgs ("PKPaymentRequestPaymentMethodUpdate")]
		void DidSelectPaymentMethod2 (PKPaymentAuthorizationViewController controller, PKPaymentMethod paymentMethod, Action<PKPaymentRequestPaymentMethodUpdate> completion);

		[Watch (7, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("paymentAuthorizationViewController:didRequestMerchantSessionUpdate:")]
		[EventArgs ("PKPaymentRequestMerchantSessionUpdate")]
		void DidRequestMerchantSessionUpdate (PKPaymentAuthorizationViewController controller, Action<PKPaymentRequestMerchantSessionUpdate> updateHandler);

		[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("paymentAuthorizationViewController:didChangeCouponCode:handler:")]
		[EventArgs ("PKPaymentRequestCouponCodeUpdate")]
		void DidChangeCouponCode (PKPaymentAuthorizationViewController controller, string couponCode, Action<PKPaymentRequestCouponCodeUpdate> completion);
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController), Delegates = new string [] { "Delegate" }, Events = new Type [] { typeof (PKPaymentAuthorizationViewControllerDelegate) })]
	[DisableDefaultCtor]
	interface PKPaymentAuthorizationViewController {
		[DesignatedInitializer]
		[Export ("initWithPaymentRequest:")]
		NativeHandle Constructor (PKPaymentRequest request);

		[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithDisbursementRequest:")]
		NativeHandle Constructor (PKDisbursementRequest request);

		[Export ("delegate", ArgumentSemantic.UnsafeUnretained)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		PKPaymentAuthorizationViewControllerDelegate Delegate { get; set; }

		[Static, Export ("canMakePayments")]
		bool CanMakePayments { get; }

		// These are the NSString constants
		[Static, Export ("canMakePaymentsUsingNetworks:")]
		bool CanMakePaymentsUsingNetworks (NSString [] paymentNetworks);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("canMakePaymentsUsingNetworks:capabilities:")]
		bool CanMakePaymentsUsingNetworks (string [] supportedNetworks, PKMerchantCapability capabilties);

		[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("supportsDisbursements")]
		bool SupportsDisbursements ();

		[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("supportsDisbursementsUsingNetworks:")]
		bool SupportsDisbursements (string [] supportedNetworks);

		[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("supportsDisbursementsUsingNetworks:capabilities:")]
		bool SupportsDisbursements (string [] supportedNetworks, PKMerchantCapability capabilities);
	}
#endif

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PKPaymentSummaryItem {
		[NullAllowed] // by default this property is null
		[Export ("label")]
		string Label { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("amount", ArgumentSemantic.Copy)]
		NSDecimalNumber Amount { get; set; }

		[Static, Export ("summaryItemWithLabel:amount:")]
		PKPaymentSummaryItem Create (string label, NSDecimalNumber amount);

		[MacCatalyst (13, 1)]
		[Export ("type", ArgumentSemantic.Assign)]
		PKPaymentSummaryItemType Type { get; set; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("summaryItemWithLabel:amount:type:")]
		PKPaymentSummaryItem Create (string label, NSDecimalNumber amount, PKPaymentSummaryItemType type);
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (PKPaymentSummaryItem))]
	interface PKShippingMethod {
		[NullAllowed] // by default this property is null
		[Export ("identifier")]
		string Identifier { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("detail")]
		string Detail { get; set; }

		[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		[NullAllowed]
		[Export ("dateComponentsRange", ArgumentSemantic.Copy)]
		PKDateComponentsRange DateComponentsRange { get; set; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PKPaymentRequest {
		[NullAllowed] // by default this property is null
		[Export ("merchantIdentifier")]
		string MerchantIdentifier { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("countryCode")]
		string CountryCode { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("supportedNetworks", ArgumentSemantic.Copy)]
		NSString [] SupportedNetworks { get; set; }

		[Export ("merchantCapabilities", ArgumentSemantic.UnsafeUnretained)]
		PKMerchantCapability MerchantCapabilities { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("paymentSummaryItems", ArgumentSemantic.Copy)]
		PKPaymentSummaryItem [] PaymentSummaryItems { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("currencyCode")]
		string CurrencyCode { get; set; }

		[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("supportsCouponCode")]
		bool SupportsCouponCode { get; set; }

		[NullAllowed]
		[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("couponCode")]
		string CouponCode { get; set; }

		[Watch (8, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("shippingContactEditingMode", ArgumentSemantic.Assign)]
		PKShippingContactEditingMode ShippingContactEditingMode { get; set; }

		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'RequiredBillingContactFields' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'RequiredBillingContactFields' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RequiredBillingContactFields' instead.")]
		[Export ("requiredBillingAddressFields", ArgumentSemantic.UnsafeUnretained)]
		PKAddressField RequiredBillingAddressFields { get; set; }

		[NoMac]
		[NoMacCatalyst]
		[NoWatch]
		[NullAllowed] // by default this property is null
		[Export ("billingAddress", ArgumentSemantic.Assign)]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'BillingContact' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'BillingContact' instead.")]
		ABRecord BillingAddress { get; set; }

		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'RequiredShippingContactFields' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'RequiredShippingContactFields' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RequiredShippingContactFields' instead.")]
		[Export ("requiredShippingAddressFields", ArgumentSemantic.UnsafeUnretained)]
		PKAddressField RequiredShippingAddressFields { get; set; }

		[NoMac]
		[NoMacCatalyst]
		[NoWatch]
		[NullAllowed] // by default this property is null
		[Export ("shippingAddress", ArgumentSemantic.Assign)]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'ShippingContact' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ShippingContact' instead.")]
		ABRecord ShippingAddress { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("shippingMethods", ArgumentSemantic.Copy)]
		PKShippingMethod [] ShippingMethods { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("applicationData", ArgumentSemantic.Copy)]
		NSData ApplicationData { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("shippingType", ArgumentSemantic.Assign)]
		PKShippingType ShippingType { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("shippingContact", ArgumentSemantic.Strong)]
		PKContact ShippingContact { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("billingContact", ArgumentSemantic.Strong)]
		PKContact BillingContact { get; set; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("availableNetworks")]
		NSString [] AvailableNetworks { get; }

		[MacCatalyst (13, 1)]
		[Export ("requiredBillingContactFields", ArgumentSemantic.Strong)]
		NSSet WeakRequiredBillingContactFields { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("requiredShippingContactFields", ArgumentSemantic.Strong)]
		NSSet WeakRequiredShippingContactFields { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("supportedCountries", ArgumentSemantic.Copy)]
		NSSet<NSString> SupportedCountries { get; set; }

		[MacCatalyst (13, 1)]
		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("paymentContactInvalidErrorWithContactField:localizedDescription:")]
		NSError CreatePaymentContactInvalidError (NSString field, [NullAllowed] string localizedDescription);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("CreatePaymentContactInvalidError (contactField.GetConstant ()!, localizedDescription)")]
		NSError CreatePaymentContactInvalidError (PKContactFields contactField, [NullAllowed] string localizedDescription);

		[MacCatalyst (13, 1)]
		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("paymentShippingAddressInvalidErrorWithKey:localizedDescription:")]
		NSError CreatePaymentShippingAddressInvalidError (NSString postalAddressKey, [NullAllowed] string localizedDescription);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("CreatePaymentShippingAddressInvalidError (postalAddress.GetConstant ()!, localizedDescription)")]
		NSError CreatePaymentShippingAddressInvalidError (CNPostalAddressKeyOption postalAddress, [NullAllowed] string localizedDescription);

		[MacCatalyst (13, 1)]
		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("paymentBillingAddressInvalidErrorWithKey:localizedDescription:")]
		NSError CreatePaymentBillingAddressInvalidError (NSString postalAddressKey, [NullAllowed] string localizedDescription);

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("CreatePaymentBillingAddressInvalidError (postalAddress.GetConstant ()!, localizedDescription)")]
		NSError CreatePaymentBillingAddressInvalidError (CNPostalAddressKeyOption postalAddress, [NullAllowed] string localizedDescription);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("paymentShippingAddressUnserviceableErrorWithLocalizedDescription:")]
		NSError CreatePaymentShippingAddressUnserviceableError ([NullAllowed] string localizedDescription);

		[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("paymentCouponCodeInvalidErrorWithLocalizedDescription:")]
		NSError GetCouponCodeInvalidError ([NullAllowed] string localizedDescription);

		[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("paymentCouponCodeExpiredErrorWithLocalizedDescription:")]
		NSError GetCouponCodeExpiredError ([NullAllowed] string localizedDescription);

		[NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[Export ("multiTokenContexts", ArgumentSemantic.Copy)]
		PKPaymentTokenContext [] MultiTokenContexts { get; set; }

		[NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[NullAllowed, Export ("recurringPaymentRequest", ArgumentSemantic.Strong)]
		PKRecurringPaymentRequest RecurringPaymentRequest { get; set; }

		[NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[NullAllowed, Export ("automaticReloadPaymentRequest", ArgumentSemantic.Strong)]
		PKAutomaticReloadPaymentRequest AutomaticReloadPaymentRequest { get; set; }

		[NullAllowed]
		[NoWatch, Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4), NoTV]
		[Export ("deferredPaymentRequest", ArgumentSemantic.Strong)]
		PKDeferredPaymentRequest DeferredPaymentRequest { get; set; }

		[iOS (17, 0), Mac (14, 0), Watch (10, 0), NoTV, MacCatalyst (17, 0)]
		[Export ("applePayLaterAvailability", ArgumentSemantic.Assign)]
		PKApplePayLaterAvailability ApplePayLaterAvailability { get; set; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Flags]
	enum PKContactFields {
		None = 0,

		[Field ("PKContactFieldPostalAddress")]
		PostalAddress = 1 << 0,

		[Field ("PKContactFieldEmailAddress")]
		EmailAddress = 1 << 1,

		[Field ("PKContactFieldPhoneNumber")]
		PhoneNumber = 1 << 2,

		[Field ("PKContactFieldName")]
		Name = 1 << 3,

		[Field ("PKContactFieldPhoneticName")]
		PhoneticName = 1 << 4,
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PKPaymentToken {

		[NoMac]
		[NoWatch]
		[Export ("paymentInstrumentName", ArgumentSemantic.Copy)]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'PaymentMethod' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PaymentMethod' instead.")]
		string PaymentInstrumentName { get; }

		[NoMac]
		[NoWatch]
		[Export ("paymentNetwork")]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'PaymentMethod' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PaymentMethod' instead.")]
		string PaymentNetwork { get; }

		[Export ("transactionIdentifier")]
		string TransactionIdentifier { get; }

		[Export ("paymentData", ArgumentSemantic.Copy)]
		NSData PaymentData { get; }

		[MacCatalyst (13, 1)]
		[Export ("paymentMethod", ArgumentSemantic.Strong)]
		PKPaymentMethod PaymentMethod { get; }
	}

	[NoMac] // under `TARGET_OS_IPHONE`
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (PKAddPassesViewControllerDelegate) })]
	// invalid null handle for default 'init'
	[DisableDefaultCtor]
	interface PKAddPassesViewController {

		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("initWithPass:")]
		NativeHandle Constructor (PKPass pass);

		[Export ("initWithPasses:")]
		NativeHandle Constructor (PKPass [] pass);

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("initWithIssuerData:signature:error:")]
		NativeHandle Constructor (NSData issuerData, NSData signature, [NullAllowed] out NSError error);

		[iOS (8, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("canAddPasses")]
		bool CanAddPasses { get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		PKAddPassesViewControllerDelegate Delegate { get; set; }
	}

	[NoMac] // under `TARGET_OS_IPHONE`
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface PKAddPassesViewControllerDelegate {
		[Export ("addPassesViewControllerDidFinish:")]
		void Finished (PKAddPassesViewController controller);
	}

	[NoWatch]
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface PKAddPaymentPassRequest : NSSecureCoding {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[NullAllowed, Export ("encryptedPassData", ArgumentSemantic.Copy)]
		NSData EncryptedPassData { get; set; }

		[NullAllowed, Export ("activationData", ArgumentSemantic.Copy)]
		NSData ActivationData { get; set; }

		[NullAllowed, Export ("ephemeralPublicKey", ArgumentSemantic.Copy)]
		NSData EphemeralPublicKey { get; set; }

		[NullAllowed, Export ("wrappedKey", ArgumentSemantic.Copy)]
		NSData WrappedKey { get; set; }
	}

	[Mac (11, 0)] // not explict (no availability macro) but part of macOS headers
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKAddPaymentPassRequestConfiguration : NSSecureCoding {
		[DesignatedInitializer]
		[Export ("initWithEncryptionScheme:")]
		NativeHandle Constructor (NSString encryptionScheme);

		[Export ("encryptionScheme")]
		NSString EncryptionScheme { get; }

		[NullAllowed, Export ("cardholderName")]
		string CardholderName { get; set; }

		[NullAllowed, Export ("primaryAccountSuffix")]
		string PrimaryAccountSuffix { get; set; }

		[NoWatch] // Radar: https://trello.com/c/MvaHEZlc
		[MacCatalyst (13, 1)]
		[Export ("cardDetails", ArgumentSemantic.Copy)]
		PKLabeledValue [] CardDetails { get; set; }

		[NullAllowed, Export ("localizedDescription")]
		string LocalizedDescription { get; set; }

		[NullAllowed, Export ("primaryAccountIdentifier")]
		string PrimaryAccountIdentifier { get; set; }

		[NullAllowed, Export ("paymentNetwork")]
		string PaymentNetwork { get; set; }

		[NoWatch] // Radar: https://trello.com/c/MvaHEZlc
		[MacCatalyst (13, 1)]
		[Export ("requiresFelicaSecureElement")]
		bool RequiresFelicaSecureElement { get; set; }

		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("style", ArgumentSemantic.Assign)]
		PKAddPaymentPassStyle Style { get; set; }

		[NoWatch] // https://feedbackassistant.apple.com/feedback/6301809 https://github.com/xamarin/maccore/issues/1819
		[iOS (12, 3)]
		[MacCatalyst (13, 1)]
		[Export ("productIdentifiers", ArgumentSemantic.Copy)]
		NSSet<NSString> ProductIdentifiers { get; set; }
	}

	[NoMac] // under `#if TARGET_OS_IPHONE`
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor]
	interface PKAddPaymentPassViewController {
		[Static]
		[Export ("canAddPaymentPass")]
		bool CanAddPaymentPass { get; }

		[DesignatedInitializer]
		[Export ("initWithRequestConfiguration:delegate:")]
		NativeHandle Constructor (PKAddPaymentPassRequestConfiguration configuration, [NullAllowed] IPKAddPaymentPassViewControllerDelegate viewControllerDelegate);

#if !NET
		[Obsolete ("Use the overload accepting a IPKAddPaymentPassViewControllerDelegate")]
		[Wrap ("this (configuration, (IPKAddPaymentPassViewControllerDelegate) viewControllerDelegate)")]
		NativeHandle Constructor (PKAddPaymentPassRequestConfiguration configuration, PKAddPaymentPassViewControllerDelegate viewControllerDelegate);
#endif

		[Wrap ("WeakDelegate")]
		[NullAllowed, Protocolize]
		PKAddPaymentPassViewControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

	interface IPKAddPaymentPassViewControllerDelegate { }

	[NoWatch]
	[NoMac] // under `#if TARGET_OS_IPHONE`
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface PKAddPaymentPassViewControllerDelegate {
		[Abstract]
		[Export ("addPaymentPassViewController:generateRequestWithCertificateChain:nonce:nonceSignature:completionHandler:")]
		void GenerateRequestWithCertificateChain (PKAddPaymentPassViewController controller, NSData [] certificates, NSData nonce, NSData nonceSignature, Action<PKAddPaymentPassRequest> handler);

		[Abstract]
		[Export ("addPaymentPassViewController:didFinishAddingPaymentPass:error:")]
		void DidFinishAddingPaymentPass (PKAddPaymentPassViewController controller, [NullAllowed] PKPaymentPass pass, [NullAllowed] NSError error);
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (PKObject))]
	interface PKPass : NSSecureCoding, NSCopying {
		[Export ("initWithData:error:")]
		NativeHandle Constructor (NSData data, out NSError error);

		[NullAllowed, Export ("authenticationToken", ArgumentSemantic.Copy)]
		string AuthenticationToken { get; }

		[NoWatch]
		[NoMac]
		[NoMacCatalyst]
		[Export ("icon", ArgumentSemantic.Copy)]
		UIImage Icon { get; }

		[Export ("localizedDescription", ArgumentSemantic.Copy)]
		string LocalizedDescription { get; }

		[Export ("localizedName", ArgumentSemantic.Copy)]
		string LocalizedName { get; }

		[Export ("organizationName", ArgumentSemantic.Copy)]
		string OrganizationName { get; }

		[Export ("passTypeIdentifier", ArgumentSemantic.Copy)]
		string PassTypeIdentifier { get; }

		[NullAllowed]
		[Export ("passURL", ArgumentSemantic.Copy)]
		NSUrl PassUrl { get; }

		[NullAllowed, Export ("relevantDate", ArgumentSemantic.Copy)]
		NSDate RelevantDate { get; }

		[Export ("serialNumber", ArgumentSemantic.Copy)]
		string SerialNumber { get; }

		[NullAllowed, Export ("webServiceURL", ArgumentSemantic.Copy)]
		NSUrl WebServiceUrl { get; }

		[Export ("localizedValueForFieldKey:")]
		[return: NullAllowed]
		NSObject GetLocalizedValue (NSString key); // TODO: Should be enum for PKPassLibraryUserInfoKey

#if !NET
		[NoMac]
		[Field ("PKPassKitErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary UserInfo { get; }

		[MacCatalyst (13, 1)]
		[Export ("passType")]
		PKPassType PassType { get; }

		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'SecureElementPass' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 2, message: "Use 'SecureElementPass' instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SecureElementPass' instead.")]
		[Export ("paymentPass")]
		PKPaymentPass PaymentPass { get; }

		[Watch (6, 2), iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("secureElementPass")]
		PKSecureElementPass SecureElementPass { get; }

		[MacCatalyst (13, 1)]
		[Export ("remotePass")]
		bool RemotePass { [Bind ("isRemotePass")] get; }

		[MacCatalyst (13, 1)]
		[Export ("deviceName")]
		string DeviceName { get; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PKPaymentMethod : NSSecureCoding {
		[NullAllowed, Export ("displayName")]
		string DisplayName { get; }

		[NullAllowed, Export ("network")]
		string Network { get; }

		[Export ("type")]
		PKPaymentMethodType Type { get; }

		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'SecureElementPass' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 2, message: "Use 'SecureElementPass' instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SecureElementPass' instead.")]
		[NullAllowed, Export ("paymentPass", ArgumentSemantic.Copy)]
		PKPaymentPass PaymentPass { get; }

		[Watch (6, 2), iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("secureElementPass", ArgumentSemantic.Copy)]
		PKSecureElementPass SecureElementPass { get; }

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("billingAddress", ArgumentSemantic.Copy)]
		CNContact BillingAddress { get; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (PKSecureElementPass))]
	interface PKPaymentPass {

		[Deprecated (PlatformName.iOS, 13, 4, message: "Use 'PKSecureElementPass.PassActivationState' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 2, message: "Use 'PKSecureElementPass.PassActivationState' instead.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PKSecureElementPass.PassActivationState' instead.")]
		[Export ("activationState")]
		PKPaymentPassActivationState ActivationState { get; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface PKObject : NSCoding, NSSecureCoding, NSCopying {
		//Empty class in header file
	}

	[Static]
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	interface PKPaymentNetwork {
		[Field ("PKPaymentNetworkAmex")]
		NSString Amex { get; }

		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CartesBancaires' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CartesBancaires' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CartesBancaires' instead.")]
		[Field ("PKPaymentNetworkCarteBancaire")]
		NSString CarteBancaire { get; }

		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4, 2, message: "Use 'CartesBancaires' instead.")]
		[Deprecated (PlatformName.iOS, 11, 2, message: "Use 'CartesBancaires' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CartesBancaires' instead.")]
		[Field ("PKPaymentNetworkCarteBancaires")]
		NSString CarteBancaires { get; }

		[iOS (11, 2)]
		[Watch (4, 2)]
		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkCartesBancaires")]
		NSString CartesBancaires { get; }

		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkChinaUnionPay")]
		NSString ChinaUnionPay { get; }

		[Watch (8, 5), Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Field ("PKPaymentNetworkDankort")]
		NSString Dankort { get; }

		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkInterac")]
		NSString Interac { get; }

		[Field ("PKPaymentNetworkMasterCard")]
		NSString MasterCard { get; }

		[Field ("PKPaymentNetworkVisa")]
		NSString Visa { get; }

		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkDiscover")]
		NSString Discover { get; }

		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkPrivateLabel")]
		NSString PrivateLabel { get; }

		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkJCB")]
		NSString Jcb { get; }

		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkSuica")]
		NSString Suica { get; }

		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkQuicPay")]
		NSString QuicPay { get; }

		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkIDCredit")]
		NSString IDCredit { get; }

		[iOS (12, 0), Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkElectron")]
		NSString Electron { get; }

		[iOS (12, 0), Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkMaestro")]
		NSString Maestro { get; }

		[iOS (12, 0), Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkVPay")]
		NSString VPay { get; }

		[iOS (12, 0), Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkEftpos")]
		NSString Eftpos { get; }

		[Watch (5, 1, 2)]
		[iOS (12, 1, 1)]
		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkElo")]
		NSString Elo { get; }

		[Watch (5, 1, 2)]
		[iOS (12, 1, 1)]
		[MacCatalyst (13, 1)]
		[Field ("PKPaymentNetworkMada")]
		NSString Mada { get; }

		[Watch (7, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("PKPaymentNetworkBarcode")]
		NSString Barcode { get; }

		[Watch (7, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("PKPaymentNetworkGirocard")]
		NSString Girocard { get; }

		[Watch (7, 4)]
		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("PKPaymentNetworkMir")]
		NSString Mir { get; }

		[Watch (9, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("PKPaymentNetworkNanaco")]
		NSString Nanaco { get; }

		[Watch (9, 4), Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4)]
		[Field ("PKPaymentNetworkPostFinance")]
		NSString PKPaymentNetworkPostFinance { get; }

		[Watch (9, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("PKPaymentNetworkWaon")]
		NSString Waon { get; }

		[iOS (16, 0), Mac (13, 0), Watch (9, 0), NoTV, MacCatalyst (16, 0)]
		[Field ("PKPaymentNetworkBancomat")]
		NSString Bancomat { get; }

		[iOS (16, 0), Mac (13, 0), Watch (9, 0), NoTV, MacCatalyst (16, 0)]
		[Field ("PKPaymentNetworkBancontact")]
		NSString Bancontact { get; }

		[iOS (17, 0), Mac (14, 0), Watch (10, 0), NoTV, MacCatalyst (17, 0)]
		[Field ("PKPaymentNetworkPagoBancomat")]
		NSString PagoBancomat { get; }

		[iOS (17, 0), Mac (14, 0), Watch (10, 0), NoTV, MacCatalyst (17, 0)]
		[Field ("PKPaymentNetworkTmoney")]
		NSString Tmoney { get; }
	}

#if !WATCH
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIButton))]
	[DisableDefaultCtor]
	interface PKPaymentButton {

		[Static]
		[Export ("buttonWithType:style:")]
		// note: named like UIButton method
		PKPaymentButton FromType (PKPaymentButtonType buttonType, PKPaymentButtonStyle buttonStyle);

		[MacCatalyst (13, 1)]
		[Export ("initWithPaymentButtonType:paymentButtonStyle:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PKPaymentButtonType type, PKPaymentButtonStyle style);

		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }
	}

	[NoMac] // under `#if TARGET_OS_IOS`
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIButton))]
	[DisableDefaultCtor]
	interface PKAddPassButton {
		[Static]
		[Export ("addPassButtonWithStyle:")]
		PKAddPassButton Create (PKAddPassButtonStyle addPassButtonStyle);

		[Export ("initWithAddPassButtonStyle:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PKAddPassButtonStyle style);

		[Appearance]
		[Export ("addPassButtonStyle", ArgumentSemantic.Assign)]
		PKAddPassButtonStyle Style { get; set; }
	}
#endif // !WATCH

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Static]
	interface PKEncryptionScheme {
		[Field ("PKEncryptionSchemeECC_V2")]
		NSString Ecc_V2 { get; }

		[MacCatalyst (13, 1)]
		[Field ("PKEncryptionSchemeRSA_V2")]
		NSString Rsa_V2 { get; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // providing DesignatedInitializer
	interface PKPaymentAuthorizationController {

		[Static]
		[Export ("canMakePayments")]
		bool CanMakePayments { get; }

		[Static]
		[Export ("canMakePaymentsUsingNetworks:")]
		bool CanMakePaymentsUsingNetworks (string [] supportedNetworks);

		[Static]
		[Export ("canMakePaymentsUsingNetworks:capabilities:")]
		bool CanMakePaymentsUsingNetworks (string [] supportedNetworks, PKMerchantCapability capabilties);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IPKPaymentAuthorizationControllerDelegate Delegate { get; set; }

		[Export ("initWithPaymentRequest:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PKPaymentRequest request);

		[Async]
		[Export ("presentWithCompletion:")]
		void Present ([NullAllowed] Action<bool> completion);

		[Async]
		[Export ("dismissWithCompletion:")]
		void Dismiss ([NullAllowed] Action completion);

		[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("supportsDisbursements")]
		bool SupportsDisbursements ();

		[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("supportsDisbursementsUsingNetworks:")]
		bool SupportsDisbursements (string [] supportedNetworks);

		[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("supportsDisbursementsUsingNetworks:capabilities:")]
		bool SupportsDisbursements (string [] supportedNetworks, PKMerchantCapability capabilities);

		[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithDisbursementRequest:")]
		NativeHandle Constructor (PKDisbursementRequest request);
	}

	interface IPKPaymentAuthorizationControllerDelegate { }

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface PKPaymentAuthorizationControllerDelegate {

		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'DidAuthorizePayment' overload with the 'Action<PKPaymentAuthorizationResult>' parameter instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'DidAuthorizePayment' overload with the 'Action<PKPaymentAuthorizationResult>' parameter instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidAuthorizePayment' overload with the 'Action<PKPaymentAuthorizationResult>' parameter instead.")]
#if !NET
		[Abstract]
#endif
		[Export ("paymentAuthorizationController:didAuthorizePayment:completion:")]
		void DidAuthorizePayment (PKPaymentAuthorizationController controller, PKPayment payment, Action<PKPaymentAuthorizationStatus> completion);

		[MacCatalyst (13, 1)]
		[Export ("paymentAuthorizationController:didAuthorizePayment:handler:")]
		void DidAuthorizePayment (PKPaymentAuthorizationController controller, PKPayment payment, Action<PKPaymentAuthorizationResult> completion);

		[Abstract]
		[Export ("paymentAuthorizationControllerDidFinish:")]
		void DidFinish (PKPaymentAuthorizationController controller);

		[Export ("paymentAuthorizationControllerWillAuthorizePayment:")]
		void WillAuthorizePayment (PKPaymentAuthorizationController controller);

		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'DidSelectShippingMethod' overload with the 'Action<PKPaymentRequestPaymentMethodUpdate>' parameter instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'DidSelectShippingMethod' overload with the 'Action<PKPaymentRequestPaymentMethodUpdate>' parameter instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidSelectShippingMethod' overload with the 'Action<PKPaymentRequestPaymentMethodUpdate>' parameter instead.")]
		[Export ("paymentAuthorizationController:didSelectShippingMethod:completion:")]
		void DidSelectShippingMethod (PKPaymentAuthorizationController controller, PKShippingMethod shippingMethod, Action<PKPaymentAuthorizationStatus, PKPaymentSummaryItem []> completion);

		[MacCatalyst (13, 1)]
		[Export ("paymentAuthorizationController:didSelectShippingMethod:handler:")]
		void DidSelectShippingMethod (PKPaymentAuthorizationController controller, PKPaymentMethod paymentMethod, Action<PKPaymentRequestPaymentMethodUpdate> completion);

		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'DidSelectShippingContact' overload with the 'Action<PKPaymentRequestShippingContactUpdate>' parameter instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'DidSelectShippingContact' overload with the 'Action<PKPaymentRequestShippingContactUpdate>' parameter instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidSelectShippingContact' overload with the 'Action<PKPaymentRequestShippingContactUpdate>' parameter instead.")]
		[Export ("paymentAuthorizationController:didSelectShippingContact:completion:")]
		void DidSelectShippingContact (PKPaymentAuthorizationController controller, PKContact contact, Action<PKPaymentAuthorizationStatus, PKShippingMethod [], PKPaymentSummaryItem []> completion);

		[MacCatalyst (13, 1)]
		[Export ("paymentAuthorizationController:didSelectShippingContact:handler:")]
		void DidSelectShippingContact (PKPaymentAuthorizationController controller, PKContact contact, Action<PKPaymentRequestShippingContactUpdate> completion);

		[NoMac]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'DidSelectPaymentMethod' overload with the 'Action<PKPaymentRequestPaymentMethodUpdate>' parameter instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'DidSelectPaymentMethod' overload with the 'Action<PKPaymentRequestPaymentMethodUpdate>' parameter instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidSelectPaymentMethod' overload with the 'Action<PKPaymentRequestPaymentMethodUpdate>' parameter instead.")]
		[Export ("paymentAuthorizationController:didSelectPaymentMethod:completion:")]
		void DidSelectPaymentMethod (PKPaymentAuthorizationController controller, PKPaymentMethod paymentMethod, Action<PKPaymentSummaryItem []> completion);

		[MacCatalyst (13, 1)]
		[Export ("paymentAuthorizationController:didSelectPaymentMethod:handler:")]
		void DidSelectPaymentMethod (PKPaymentAuthorizationController controller, PKPaymentMethod paymentMethod, Action<PKPaymentRequestPaymentMethodUpdate> completion);

		[Watch (7, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("paymentAuthorizationController:didRequestMerchantSessionUpdate:")]
		void DidRequestMerchantSessionUpdate (PKPaymentAuthorizationController controller, Action<PKPaymentRequestMerchantSessionUpdate> handler);

		[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("paymentAuthorizationController:didChangeCouponCode:handler:")]
		void DidChangeCouponCode (PKPaymentAuthorizationController controller, string couponCode, Action<PKPaymentRequestCouponCodeUpdate> completion);

		[Watch (7, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("presentationWindowForPaymentAuthorizationController:")]
		[return: NullAllowed]
#if MONOMAC || __MACCATALYST__
		[Abstract]
#endif
		UIWindow GetPresentationWindow (PKPaymentAuthorizationController controller);
	}

	[Mac (11, 0)]
	[NoWatch] // Radar: https://trello.com/c/MvaHEZlc
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // there's a designated initializer and it does not accept null
	interface PKLabeledValue {
		[Export ("initWithLabel:value:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string label, string value);

		[Export ("label")]
		string Label { get; }

		[Export ("value")]
		string Value { get; }
	}

	[Mac (11, 0)]
	[Watch (4, 3), iOS (11, 3)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (PKStoredValuePassProperties))]
	[DisableDefaultCtor]
	interface PKTransitPassProperties {

		[Static]
		[Export ("passPropertiesForPass:")]
		[return: NullAllowed]
		PKTransitPassProperties GetPassProperties (PKPass pass);

		[Deprecated (PlatformName.iOS, 15, 0)]
		[Deprecated (PlatformName.WatchOS, 8, 0)]
		[Deprecated (PlatformName.MacOSX, 12, 0)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0)]
		[Export ("transitBalance", ArgumentSemantic.Copy)]
		NSDecimalNumber TransitBalance { get; }

		[Deprecated (PlatformName.iOS, 15, 0)]
		[Deprecated (PlatformName.WatchOS, 8, 0)]
		[Deprecated (PlatformName.MacOSX, 12, 0)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0)]
		[Export ("transitBalanceCurrencyCode")]
		string TransitBalanceCurrencyCode { get; }

		[Export ("inStation")]
		bool InStation { [Bind ("isInStation")] get; }

		[Deprecated (PlatformName.iOS, 14, 5, message: "Use 'Blocked' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 5, message: "Use 'Blocked' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 4, message: "Use 'Blocked' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 3, message: "Use 'Blocked' instead.")]
		[Export ("blacklisted")]
		bool Blacklisted { [Bind ("isBlacklisted")] get; }

		[iOS (14, 5)]
		[Watch (7, 4)]
		[Mac (11, 3)]
		[MacCatalyst (14, 5)]
		[Export ("blocked")]
		bool Blocked { [Bind ("isBlocked")] get; }

		[NullAllowed, Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
#if NET || MONOMAC
	[DisableDefaultCtor] // hint: getter only props and a factory method.
#endif
	[BaseType (typeof (PKTransitPassProperties))]
	interface PKSuicaPassProperties {
		[Static]
		[Export ("passPropertiesForPass:")]
		[return: NullAllowed]
		PKSuicaPassProperties GetPassProperties (PKPass pass);

		[Export ("transitBalance", ArgumentSemantic.Copy)]
		NSDecimalNumber TransitBalance { get; }

		[Export ("transitBalanceCurrencyCode")]
		string TransitBalanceCurrencyCode { get; }

		[Export ("inStation")]
		bool InStation { [Bind ("isInStation")] get; }

		[Export ("inShinkansenStation")]
		bool InShinkansenStation { [Bind ("isInShinkansenStation")] get; }

		[Watch (4, 3), iOS (11, 3)]
		[MacCatalyst (13, 1)]
		[Export ("balanceAllowedForCommute")]
		bool BalanceAllowedForCommute { [Bind ("isBalanceAllowedForCommute")] get; }

		[Watch (4, 3), iOS (11, 3)]
		[MacCatalyst (13, 1)]
		[Export ("lowBalanceGateNotificationEnabled")]
		bool LowBalanceGateNotificationEnabled { [Bind ("isLowBalanceGateNotificationEnabled")] get; }

		[Export ("greenCarTicketUsed")]
		bool GreenCarTicketUsed { [Bind ("isGreenCarTicketUsed")] get; }

		[Export ("blacklisted")]
		[Deprecated (PlatformName.iOS, 14, 5, message: "Use 'Blocked' instead.")] // exists in base class
		[Deprecated (PlatformName.WatchOS, 7, 4, message: "Use 'Blocked' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 3, message: "Use 'Blocked' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 5, message: "Use 'Blocked' instead.")]
		bool Blacklisted { [Bind ("isBlacklisted")] get; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPaymentAuthorizationResult {
		[Export ("initWithStatus:errors:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PKPaymentAuthorizationStatus status, [NullAllowed] NSError [] errors);

		[Export ("status", ArgumentSemantic.Assign)]
		PKPaymentAuthorizationStatus Status { get; set; }

		[NullAllowed, Export ("errors", ArgumentSemantic.Copy)]
		NSError [] Errors { get; set; }

		[NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[NullAllowed, Export ("orderDetails", ArgumentSemantic.Strong)]
		PKPaymentOrderDetails OrderDetails { get; set; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPaymentRequestUpdate {

		[Export ("initWithPaymentSummaryItems:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PKPaymentSummaryItem [] paymentSummaryItems);

		[Export ("status", ArgumentSemantic.Assign)]
		PKPaymentAuthorizationStatus Status { get; set; }

		[Export ("paymentSummaryItems", ArgumentSemantic.Copy)]
		PKPaymentSummaryItem [] PaymentSummaryItems { get; set; }

		[Watch (8, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("shippingMethods", ArgumentSemantic.Copy)]
		PKShippingMethod [] ShippingMethods { get; set; }

		[NoWatch, Mac (13, 0), iOS (16, 0), NoTV, MacCatalyst (16, 0)]
		[NullAllowed, Export ("multiTokenContexts", ArgumentSemantic.Copy)]
		PKPaymentTokenContext [] MultiTokenContexts { get; set; }

		[NoWatch, Mac (13, 0), iOS (16, 0), NoTV, MacCatalyst (16, 0)]
		[NullAllowed, Export ("recurringPaymentRequest", ArgumentSemantic.Strong)]
		PKRecurringPaymentRequest RecurringPaymentRequest { get; set; }

		[NoWatch, Mac (13, 0), iOS (16, 0), NoTV, MacCatalyst (16, 0)]
		[NullAllowed, Export ("automaticReloadPaymentRequest", ArgumentSemantic.Strong)]
		PKAutomaticReloadPaymentRequest AutomaticReloadPaymentRequest { get; set; }

		[NullAllowed]
		[NoWatch, Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4), NoTV]
		[Export ("deferredPaymentRequest", ArgumentSemantic.Strong)]
		PKDeferredPaymentRequest DeferredPaymentRequest { get; set; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (PKPaymentRequestUpdate))]
	[DisableDefaultCtor]
	interface PKPaymentRequestShippingContactUpdate {

		[Export ("initWithErrors:paymentSummaryItems:shippingMethods:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSError [] errors, PKPaymentSummaryItem [] paymentSummaryItems, PKShippingMethod [] shippingMethods);

		[Export ("shippingMethods", ArgumentSemantic.Copy)]
		PKShippingMethod [] ShippingMethods { get; set; }

		[NullAllowed, Export ("errors", ArgumentSemantic.Copy)]
		NSError [] Errors { get; set; }
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (PKPaymentRequestUpdate))]
	[DisableDefaultCtor]
	interface PKPaymentRequestShippingMethodUpdate {

		// inlined
		[Export ("initWithPaymentSummaryItems:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PKPaymentSummaryItem [] paymentSummaryItems);
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (PKPaymentRequestUpdate))]
	[DisableDefaultCtor]
	interface PKPaymentRequestPaymentMethodUpdate {

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithErrors:paymentSummaryItems:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSError [] errors, PKPaymentSummaryItem [] paymentSummaryItems);

		[Watch (6, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("errors", ArgumentSemantic.Copy)]
		NSError [] Errors { get; set; }

		// inlined
		[Export ("initWithPaymentSummaryItems:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PKPaymentSummaryItem [] paymentSummaryItems);
	}

	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[Static] // not to enum'ify - exposed as NSString inside NSError
	interface PKPaymentErrorKeys {

		[MacCatalyst (13, 1)]
		[Field ("PKPaymentErrorContactFieldUserInfoKey")]
		NSString ContactFieldUserInfoKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("PKPaymentErrorPostalAddressUserInfoKey")]
		NSString PostalAddressUserInfoKey { get; }
	}

	interface IPKDisbursementAuthorizationControllerDelegate { }

#if !XAMCORE_5_0
	[NoMac] // only used in non-macOS API
	[NoWatch]
	[iOS (12, 2)]
	[MacCatalyst (13, 1)]
	[Obsoleted (PlatformName.iOS, 17, 0, message: "No longer used.")]
	[Obsoleted (PlatformName.MacCatalyst, 17, 0, message: "No longer used.")]
	[Native]
	public enum PKDisbursementRequestSchedule : long {
		OneTime,
		Future,
	}
#endif

	[NoWatch]
	[iOS (12, 2)]
	[NoMac] // all members are decorated as such, but not the type itself
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface PKDisbursementRequest {

#if XAMCORE_5_0
		[Export ("currencyCode")]
#else
		[NullAllowed, Export ("currencyCode")]
#endif
		string CurrencyCode { get; set; }

#if XAMCORE_5_0
		[Export ("summaryItems", ArgumentSemantic.Copy)]
#else
		[NullAllowed, Export ("summaryItems", ArgumentSemantic.Copy)]
#endif
		PKPaymentSummaryItem [] SummaryItems { get; set; }

		[iOS (17, 0), NoMac, NoWatch, NoTV, NoMacCatalyst]
		[Export ("merchantIdentifier")]
		string MerchantIdentifier { get; set; }

		[iOS (17, 0), NoMac, NoWatch, NoTV, NoMacCatalyst]
		[Export ("regionCode")]
		string RegionCode { get; set; }

		[iOS (17, 0), NoMac, NoWatch, NoTV, NoMacCatalyst]
		[Export ("supportedNetworks", ArgumentSemantic.Copy)]
		string [] SupportedNetworks { get; set; }

		[iOS (17, 0), NoMac, NoWatch, NoTV, NoMacCatalyst]
		[Export ("merchantCapabilities", ArgumentSemantic.Assign)]
		PKMerchantCapability MerchantCapabilities { get; set; }

		[iOS (17, 0), NoMac, NoWatch, NoTV, NoMacCatalyst]
		[Export ("requiredRecipientContactFields", ArgumentSemantic.Strong)]
		string [] RequiredRecipientContactFields { get; set; }

		[iOS (17, 0), NoMac, NoWatch, NoTV, NoMacCatalyst]
		[NullAllowed, Export ("recipientContact", ArgumentSemantic.Strong)]
		PKContact RecipientContact { get; set; }

		[iOS (17, 0), NoMac, NoWatch, NoTV, NoMacCatalyst]
		[NullAllowed, Export ("supportedRegions", ArgumentSemantic.Copy)]
		string [] SupportedRegions { get; set; }

		[iOS (17, 0), NoMac, NoWatch, NoTV, NoMacCatalyst]
		[NullAllowed, Export ("applicationData", ArgumentSemantic.Copy)]
		NSData ApplicationData { get; set; }

		[iOS (17, 0), NoMac, NoWatch, NoTV, NoMacCatalyst]
		[Export ("initWithMerchantIdentifier:currencyCode:regionCode:supportedNetworks:merchantCapabilities:summaryItems:")]
		NativeHandle Constructor (string merchantIdentifier, string currencyCode, string regionCode, string [] supportedNetworks, PKMerchantCapability merchantCapabilities, PKPaymentSummaryItem [] summaryItems);

		[iOS (17, 0), NoMac, NoWatch, NoTV, NoMacCatalyst]
		[Static]
		[Export ("disbursementContactInvalidErrorWithContactField:localizedDescription:")]
		NSError GetDisbursementContactInvalidError (string field, [NullAllowed] string localizedDescription);

		[iOS (17, 0), NoMac, NoWatch, NoTV, NoMacCatalyst]
		[Static]
		[Export ("disbursementCardUnsupportedError")]
		NSError DisbursementCardUnsupportedError { get; }
	}

	[Mac (11, 0)]
	[Watch (6, 2), iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (PKPass))]
	[DisableDefaultCtor]
	interface PKSecureElementPass {

		[Export ("primaryAccountIdentifier")]
		string PrimaryAccountIdentifier { get; }

		[Export ("primaryAccountNumberSuffix")]
		string PrimaryAccountNumberSuffix { get; }

		[Export ("deviceAccountIdentifier", ArgumentSemantic.Strong)]
		string DeviceAccountIdentifier { get; }

		[Export ("deviceAccountNumberSuffix", ArgumentSemantic.Strong)]
		string DeviceAccountNumberSuffix { get; }

		[Export ("passActivationState")]
		PKSecureElementPassActivationState PassActivationState { get; }

		[NullAllowed, Export ("devicePassIdentifier")]
		string DevicePassIdentifier { get; }

		[NullAllowed, Export ("pairedTerminalIdentifier")]
		string PairedTerminalIdentifier { get; }
	}

	[Mac (11, 0)]
	[NoWatch, NoTV]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum PKAddShareablePassConfigurationPrimaryAction : ulong {
		Add,
		Share,
	}

	[Mac (11, 0)]
	[Watch (7, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[NoTV]
	[Native]
	public enum PKBarcodeEventConfigurationDataType : long {
		Unknown,
		SigningKeyMaterial,
		SigningCertificate,
	}

	[NoWatch, NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum PKIssuerProvisioningExtensionAuthorizationResult : long {
		Canceled,
		Authorized,
	}

	[NoWatch, NoTV]
	[iOS (13, 4)]
	[Mac (11, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKAddSecureElementPassConfiguration {

		[NullAllowed, Export ("issuerIdentifier")]
		string IssuerIdentifier { get; set; }

		[NullAllowed, Export ("localizedDescription")]
		string LocalizedDescription { get; set; }
	}

	[NoWatch, NoTV]
	[Mac (11, 0)] // not explicit (no attribute) but headers are present
	[iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (PKAddSecureElementPassConfiguration))]
	// note: `init` is present in headers
	interface PKAddCarKeyPassConfiguration {

		[Export ("password")]
		string Password { get; set; }

		// headers say [Watch (7,3)] but PKAddSecureElementPassConfiguration is not supported for watch
		[iOS (14, 5)]
		[Mac (11, 3)]
		[MacCatalyst (14, 5)]
		[Export ("supportedRadioTechnologies", ArgumentSemantic.Assign)]
		PKRadioTechnology SupportedRadioTechnologies { get; set; }

		// headers say [Watch (9,0)] but PKAddSecureElementPassConfiguration is not supported for watch
		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), NoTV, NoWatch]
		[Export ("manufacturerIdentifier")]
		string ManufacturerIdentifier { get; set; }

		// headers say [Watch (9,0)] but PKAddSecureElementPassConfiguration is not supported for watch
		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), NoTV, NoWatch]
		[NullAllowed, Export ("provisioningTemplateIdentifier", ArgumentSemantic.Strong)]
		string ProvisioningTemplateIdentifier { get; set; }
	}

	interface IPKAddSecureElementPassViewControllerDelegate { }

	[NoWatch, NoTV, NoMac] // under `#if TARGET_OS_IOS`
	[iOS (13, 4)]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface PKAddSecureElementPassViewControllerDelegate {

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'DidFinishAddingSecureElementPasses' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'DidFinishAddingSecureElementPasses' instead.")]
#if !XAMCORE_5_0
		[Abstract]
#endif
		[Export ("addSecureElementPassViewController:didFinishAddingSecureElementPass:error:")]
		void DidFinishAddingSecureElementPass (PKAddSecureElementPassViewController controller, [NullAllowed] PKSecureElementPass pass, [NullAllowed] NSError error);

		[Abstract]
		[Export ("addSecureElementPassViewController:didFinishAddingSecureElementPasses:error:")]
		void DidFinishAddingSecureElementPasses (PKAddSecureElementPassViewController controller, [NullAllowed] PKSecureElementPass [] passes, [NullAllowed] NSError error);
	}

	[NoWatch, NoTV, NoMac] // under `#if TARGET_OS_IOS`
	[iOS (13, 4)]
	[MacCatalyst (14, 0)] // doc mention 13.4 but we can't load the class
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor]
	interface PKAddSecureElementPassViewController {

		[Static]
		[Export ("canAddSecureElementPassWithConfiguration:")]
		bool CanAddSecureElementPass (PKAddSecureElementPassConfiguration configuration);

		[Export ("initWithConfiguration:delegate:")]
		NativeHandle Constructor (PKAddSecureElementPassConfiguration configuration, [NullAllowed] IPKAddSecureElementPassViewControllerDelegate @delegate);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IPKAddSecureElementPassViewControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

	[NoWatch, NoTV]
	[iOS (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKShareablePassMetadata {

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("initWithProvisioningCredentialIdentifier:cardConfigurationIdentifier:sharingInstanceIdentifier:passThumbnailImage:ownerDisplayName:localizedDescription:")]
		NativeHandle Constructor (string credentialIdentifier, string cardConfigurationIdentifier, string sharingInstanceIdentifier, CGImage passThumbnailImage, string ownerDisplayName, string localizedDescription);

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		[Export ("initWithProvisioningCredentialIdentifier:sharingInstanceIdentifier:passThumbnailImage:ownerDisplayName:localizedDescription:accountHash:templateIdentifier:relyingPartyIdentifier:requiresUnifiedAccessCapableDevice:")]
		NativeHandle Constructor (string credentialIdentifier, string sharingInstanceIdentifier, CGImage passThumbnailImage, string ownerDisplayName, string localizedDescription, string accountHash, string templateIdentifier, string relyingPartyIdentifier, bool requiresUnifiedAccessCapableDevice);

		[Internal]
		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Export ("initWithProvisioningCredentialIdentifier:sharingInstanceIdentifier:cardTemplateIdentifier:preview:")]
		NativeHandle InitWithCardTemplate (string credentialIdentifier, string sharingInstanceIdentifier, string templateIdentifier, PKShareablePassMetadataPreview preview);

		[Internal]
		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Export ("initWithProvisioningCredentialIdentifier:sharingInstanceIdentifier:cardConfigurationIdentifier:preview:")]
		NativeHandle InitWithCardConfiguration (string credentialIdentifier, string sharingInstanceIdentifier, string templateIdentifier, PKShareablePassMetadataPreview preview);

		[Export ("credentialIdentifier", ArgumentSemantic.Strong)]
		string CredentialIdentifier { get; }

		[Export ("cardConfigurationIdentifier", ArgumentSemantic.Strong)]
		string CardConfigurationIdentifier { get; }

		[Export ("sharingInstanceIdentifier", ArgumentSemantic.Strong)]
		string SharingInstanceIdentifier { get; }

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("passThumbnailImage")]
		CGImage PassThumbnailImage { get; }

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("localizedDescription", ArgumentSemantic.Strong)]
		string LocalizedDescription { get; }

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("ownerDisplayName", ArgumentSemantic.Strong)]
		string OwnerDisplayName { get; }

		[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		[Export ("accountHash", ArgumentSemantic.Strong)]
		string AccountHash { get; [iOS (16, 0), Mac (13, 0), Watch (9, 0), NoTV, MacCatalyst (16, 0)] set; }

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		[Export ("templateIdentifier", ArgumentSemantic.Strong)]
		string TemplateIdentifier { get; }

		[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		[Export ("relyingPartyIdentifier", ArgumentSemantic.Strong)]
		string RelyingPartyIdentifier { get; [iOS (16, 0), Mac (13, 0), Watch (9, 0), NoTV, MacCatalyst (16, 0)] set; }

		[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		[Export ("requiresUnifiedAccessCapableDevice")]
		bool RequiresUnifiedAccessCapableDevice { get; [iOS (16, 0), Mac (13, 0), Watch (9, 0), NoTV, MacCatalyst (16, 0)] set; }

		[NoWatch, NoTV, iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("cardTemplateIdentifier", ArgumentSemantic.Strong)]
		string CardTemplateIdentifier { get; }

		[NoWatch, NoTV, iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("preview", ArgumentSemantic.Strong)]
		PKShareablePassMetadataPreview Preview { get; }

		[NoWatch, NoTV, iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("serverEnvironmentIdentifier", ArgumentSemantic.Strong)]
		string ServerEnvironmentIdentifier { get; set; }
	}

	[NoWatch, NoTV]
	[iOS (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (PKAddSecureElementPassConfiguration))]
	[DisableDefaultCtor]
	interface PKAddShareablePassConfiguration {

		[Async]
		[Static]
		[Export ("configurationForPassMetadata:provisioningPolicyIdentifier:primaryAction:completion:")]
		void GetConfiguration (PKShareablePassMetadata [] passMetadata, string provisioningPolicyIdentifier, PKAddShareablePassConfigurationPrimaryAction action, Action<PKAddShareablePassConfiguration, NSError> completion);

		[Export ("primaryAction")]
		PKAddShareablePassConfigurationPrimaryAction PrimaryAction { get; }

		[Export ("credentialsMetadata", ArgumentSemantic.Strong)]
		PKShareablePassMetadata [] CredentialsMetadata { get; }

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("provisioningPolicyIdentifier", ArgumentSemantic.Strong)]
		string ProvisioningPolicyIdentifier { get; }

		[NoWatch, NoTV, iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Static, Async]
		[Export ("configurationForPassMetadata:primaryAction:completion:")]
		void GetConfiguration (PKShareablePassMetadata [] passMetadata, PKAddShareablePassConfigurationPrimaryAction action, Action<PKAddShareablePassConfiguration, NSError> completion);
	}

	[Mac (11, 0)]
	[Watch (7, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKBarcodeEventConfigurationRequest {

		[Export ("deviceAccountIdentifier")]
		string DeviceAccountIdentifier { get; }

		[Export ("configurationData")]
		NSData ConfigurationData { get; }

		[Export ("configurationDataType")]
		PKBarcodeEventConfigurationDataType ConfigurationDataType { get; }
	}

	[Mac (11, 0)]
	[Watch (7, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKBarcodeEventMetadataRequest {

		[Export ("deviceAccountIdentifier")]
		string DeviceAccountIdentifier { get; }

		[Export ("lastUsedBarcodeIdentifier")]
		string LastUsedBarcodeIdentifier { get; }
	}

	[Mac (11, 0)]
	[Watch (7, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKBarcodeEventMetadataResponse {

		[Export ("initWithPaymentInformation:")]
		NativeHandle Constructor (NSData paymentInformation);

		[Export ("paymentInformation", ArgumentSemantic.Copy)]
		NSData PaymentInformation { get; set; }
	}

	[Mac (11, 0)]
	[Watch (7, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKBarcodeEventSignatureRequest {

		[Export ("deviceAccountIdentifier")]
		string DeviceAccountIdentifier { get; }

		[Export ("transactionIdentifier")]
		string TransactionIdentifier { get; }

		[Export ("barcodeIdentifier")]
		string BarcodeIdentifier { get; }

		[Export ("rawMerchantName")]
		string RawMerchantName { get; }

		[Export ("merchantName")]
		string MerchantName { get; }

		[Export ("transactionDate", ArgumentSemantic.Strong)]
		NSDate TransactionDate { get; }

		[Export ("currencyCode")]
		string CurrencyCode { get; }

		// NSDecimalNumber is used elsewhere (but it's a subclass for NSNumber and can't be used here)
		[Export ("amount", ArgumentSemantic.Strong)]
		NSNumber Amount { get; }

		[Export ("transactionStatus")]
		string TransactionStatus { get; }

		[Export ("partialSignature", ArgumentSemantic.Copy)]
		NSData PartialSignature { get; }
	}

	[Mac (11, 0)]
	[Watch (7, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKBarcodeEventSignatureResponse {

		[Export ("initWithSignedData:")]
		NativeHandle Constructor (NSData signedData);

		[Export ("signedData", ArgumentSemantic.Copy)]
		NSData SignedData { get; set; }
	}

	[NoWatch, NoTV]
	[iOS (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface PKIssuerProvisioningExtensionStatus {

		[Export ("requiresAuthentication")]
		bool RequiresAuthentication { get; set; }

		[Export ("passEntriesAvailable")]
		bool PassEntriesAvailable { get; set; }

		[Export ("remotePassEntriesAvailable")]
		bool RemotePassEntriesAvailable { get; set; }
	}

	[NoWatch, NoTV]
	[iOS (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKIssuerProvisioningExtensionPassEntry {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("title")]
		string Title { get; }

		[Export ("art")]
		CGImage Art { get; }
	}

	[NoWatch, NoTV, NoMac]
	[iOS (14, 0)]
	[NoMacCatalyst] // type cannot be loaded, lack of documentation about usage
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKIssuerProvisioningExtensionHandler {

		[Async]
		[Export ("statusWithCompletion:")]
		void GetStatus (Action<PKIssuerProvisioningExtensionStatus> completion);

		[Async]
		[Export ("passEntriesWithCompletion:")]
		void PassEntries (Action<PKIssuerProvisioningExtensionPassEntry []> completion);

		[Async]
		[Export ("remotePassEntriesWithCompletion:")]
		void RemotePassEntries (Action<PKIssuerProvisioningExtensionPassEntry []> completion);

		[Async]
		[Export ("generateAddPaymentPassRequestForPassEntryWithIdentifier:configuration:certificateChain:nonce:nonceSignature:completionHandler:")]
		void GenerateAddPaymentPassRequest (string identifier, PKAddPaymentPassRequestConfiguration configuration, NSData [] certificates, NSData nonce, NSData nonceSignature, Action<PKAddPaymentPassRequest> completion);
	}

	[NoWatch, NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface PKIssuerProvisioningExtensionAuthorizationProviding {

		[Abstract]
		[NullAllowed, Export ("completionHandler", ArgumentSemantic.Copy)]
		Action<PKIssuerProvisioningExtensionAuthorizationResult> CompletionHandler { get; set; }
	}

	[NoWatch, NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	delegate void PKInformationRequestCompletionBlock (PKBarcodeEventMetadataResponse response);

	[NoWatch, NoTV, NoMac]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	delegate void PKSignatureRequestCompletionBlock (PKBarcodeEventSignatureResponse response);

	[NoTV]
	[Mac (11, 0)]
	[Watch (7, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface PKPaymentInformationRequestHandling {

		[Abstract]
		[Export ("handleInformationRequest:completion:")]
		void HandleInformationRequest (PKBarcodeEventMetadataRequest infoRequest, PKInformationRequestCompletionBlock completion);

		[Abstract]
		[Export ("handleSignatureRequest:completion:")]
		void HandleSignatureRequest (PKBarcodeEventSignatureRequest signatureRequest, PKSignatureRequestCompletionBlock completion);

		[Abstract]
		[Export ("handleConfigurationRequest:completion:")]
		void HandleConfigurationRequest (PKBarcodeEventConfigurationRequest configurationRequest, Action completion);
	}

	[NoWatch, NoTV]
	[iOS (14, 0)]
	[Mac (11, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (PKIssuerProvisioningExtensionPassEntry))]
	[DisableDefaultCtor]
	interface PKIssuerProvisioningExtensionPaymentPassEntry {

		[Export ("initWithIdentifier:title:art:addRequestConfiguration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, string title, CGImage art, PKAddPaymentPassRequestConfiguration configuration);

		[Export ("addRequestConfiguration")]
		PKAddPaymentPassRequestConfiguration AddRequestConfiguration { get; }
	}

	[NoTV]
	[Watch (7, 0)]
	[Mac (11, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPaymentMerchantSession {

		[Export ("initWithDictionary:")]
		NativeHandle Constructor (NSDictionary dictionary);
	}

	[NoTV]
	[Watch (7, 0)]
	[Mac (11, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface PKPaymentRequestMerchantSessionUpdate {

		[Export ("initWithStatus:merchantSession:")]
		NativeHandle Constructor (PKPaymentAuthorizationStatus status, [NullAllowed] PKPaymentMerchantSession session);

		[Export ("status", ArgumentSemantic.Assign)]
		PKPaymentAuthorizationStatus Status { get; set; }

		[NullAllowed, Export ("session", ArgumentSemantic.Strong)]
		PKPaymentMerchantSession Session { get; set; }
	}

	[NoWatch, Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PKPaymentRequestUpdate))]
	[DisableDefaultCtor]
	interface PKPaymentRequestCouponCodeUpdate {
		[Export ("initWithPaymentSummaryItems:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PKPaymentSummaryItem [] paymentSummaryItems);

		[Export ("initWithErrors:paymentSummaryItems:shippingMethods:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSError [] errors, PKPaymentSummaryItem [] paymentSummaryItems, PKShippingMethod [] shippingMethods);

		[NullAllowed, Export ("errors", ArgumentSemantic.Copy)]
		NSError [] Errors { get; set; }

		[NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[Export ("multiTokenContexts", ArgumentSemantic.Copy)]
		PKPaymentTokenContext [] MultiTokenContexts { get; set; }

		[NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[Export ("recurringPaymentRequest", ArgumentSemantic.Strong)]
		PKRecurringPaymentRequest RecurringPaymentRequest { get; set; }

		[NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[Export ("automaticReloadPaymentRequest", ArgumentSemantic.Strong)]
		PKAutomaticReloadPaymentRequest AutomaticReloadPaymentRequest { get; set; }
	}

	[Watch (7, 0)]
	[Mac (11, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPaymentInformationEventExtension {
	}

	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Flags]
	[Native]
	enum PKRadioTechnology : ulong {
		None = 0,
		Nfc = 1 << 0,
		Bluetooth = 1 << 1,
	}

	[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKDateComponentsRange : NSCopying, NSSecureCoding {
		[Export ("initWithStartDateComponents:endDateComponents:")]
		[return: NullAllowed]
		NativeHandle Constructor (NSDateComponents startDateComponents, NSDateComponents endDateComponents);

		[Export ("startDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents StartDateComponents { get; }

		[Export ("endDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents EndDateComponents { get; }
	}

	[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PKPaymentSummaryItem))]
	[DisableDefaultCtor]
	interface PKDeferredPaymentSummaryItem {
		[Export ("deferredDate", ArgumentSemantic.Copy)]
		NSDate DeferredDate { get; set; }
	}

	[Watch (8, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum PKShippingContactEditingMode : ulong {
		Enabled = 1,
		StorePickup,
	}

	[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (PKPaymentSummaryItem))]
	[DisableDefaultCtor]
	interface PKRecurringPaymentSummaryItem {
		[NullAllowed, Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; set; }

		[Export ("intervalUnit", ArgumentSemantic.Assign)]
		NSCalendarUnit IntervalUnit { get; set; }

		[Export ("intervalCount")]
		nint IntervalCount { get; set; }

		[NullAllowed, Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; set; }
	}

	[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
	public enum PKStoredValuePassBalanceType {
		[Field ("PKStoredValuePassBalanceTypeCash")]
		Cash,
		[Field ("PKStoredValuePassBalanceTypeLoyaltyPoints")]
		LoyaltyPoints,
	}

	[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKStoredValuePassBalance {
		[Export ("amount", ArgumentSemantic.Strong)]
		NSDecimalNumber Amount { get; }

		[NullAllowed, Export ("currencyCode")]
		string CurrencyCode { get; }

		[Export ("balanceType")]
		string BalanceType { get; }

		[NullAllowed, Export ("expiryDate", ArgumentSemantic.Strong)]
		NSDate ExpiryDate { get; }

		[Export ("isEqualToBalance:")]
		bool IsEqual (PKStoredValuePassBalance balance);
	}

	[Watch (8, 0), iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKStoredValuePassProperties {
		[Static]
		[Export ("passPropertiesForPass:")]
		[return: NullAllowed]
		PKStoredValuePassProperties GetPassProperties (PKPass pass);

		[Export ("blocked")]
		bool Blocked { [Bind ("isBlocked")] get; }

		[NullAllowed, Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; }

		[Export ("balances", ArgumentSemantic.Copy)]
		PKStoredValuePassBalance [] Balances { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	interface IPKIdentityDocumentDescriptor { }

	[NoWatch, NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface PKIdentityDocumentDescriptor {
		[Abstract]
		[Export ("elements")]
		PKIdentityElement [] Elements { get; }

		[Abstract]
		[Export ("intentToStoreForElement:")]
		[return: NullAllowed]
		PKIdentityIntentToStore GetIntentToStore (PKIdentityElement element);

		[Abstract]
		[Export ("addElements:withIntentToStore:")]
		void AddElements (PKIdentityElement [] elements, PKIdentityIntentToStore intentToStore);
	}

	interface IPKShareSecureElementPassViewControllerDelegate { };

	[iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV, NoMac]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface PKShareSecureElementPassViewControllerDelegate {
		[Abstract]
		[Export ("shareSecureElementPassViewController:didFinishWithResult:")]
		void DidFinish (PKShareSecureElementPassViewController controller, PKShareSecureElementPassResult result);

		[Export ("shareSecureElementPassViewController:didCreateShareURL:activationCode:")]
		void DidCreateShareUrl (PKShareSecureElementPassViewController controller, [NullAllowed] NSUrl universalShareUrl, [NullAllowed] string activationCode);
	}

	interface IPKVehicleConnectionDelegate { }

	[iOS (16, 0), Mac (13, 0), Watch (9, 0), NoTV, MacCatalyst (16, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface PKVehicleConnectionDelegate {
		[Abstract]
		[Export ("sessionDidChangeConnectionState:")]
		void SessionDidChangeConnectionState (PKVehicleConnectionSessionConnectionState newState);

		[Abstract]
		[Export ("sessionDidReceiveData:")]
		void SessionDidReceiveData (NSData data);
	}

	[NoWatch, iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKAutomaticReloadPaymentRequest // : NSCoding, NSCopying, NSSecureCoding // https://feedbackassistant.apple.com/feedback/11018799
	{
		[Export ("paymentDescription")]
		string PaymentDescription { get; set; }

		[Export ("automaticReloadBilling", ArgumentSemantic.Strong)]
		PKAutomaticReloadPaymentSummaryItem AutomaticReloadBilling { get; set; }

		[NullAllowed, Export ("billingAgreement")]
		string BillingAgreement { get; set; }

		[Export ("managementURL", ArgumentSemantic.Strong)]
		NSUrl ManagementUrl { get; set; }

		[NullAllowed, Export ("tokenNotificationURL", ArgumentSemantic.Strong)]
		NSUrl TokenNotificationUrl { get; set; }

		[Export ("initWithPaymentDescription:automaticReloadBilling:managementURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string paymentDescription, PKAutomaticReloadPaymentSummaryItem automaticReloadBilling, NSUrl managementUrl);
	}

	[NoWatch, iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), NoTV]
	[BaseType (typeof (PKPaymentSummaryItem))]
	interface PKAutomaticReloadPaymentSummaryItem // : NSCoding, NSCopying, NSSecureCoding // https://feedbackassistant.apple.com/feedback/11018799
	{
		[Export ("thresholdAmount", ArgumentSemantic.Strong)]
		NSDecimalNumber ThresholdAmount { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface PKIdentityAuthorizationController {
		[Async]
		[Export ("checkCanRequestDocument:completion:")]
		void CheckCanRequestDocument (IPKIdentityDocumentDescriptor descriptor, Action<bool> completion);

		[Async]
		[Export ("requestDocument:completion:")]
		void RequestDocument (PKIdentityRequest request, Action<PKIdentityDocument, NSError> completion);

		[Export ("cancelRequest")]
		void CancelRequest ();
	}

	[NoWatch, NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (UIControl))]
	interface PKIdentityButton {
		[Export ("initWithLabel:style:")]
		[DesignatedInitializer]
		NativeHandle Constructor (PKIdentityButtonLabel label, PKIdentityButtonStyle style);

		[Static]
		[Export ("buttonWithLabel:style:")]
		PKIdentityButton Create (PKIdentityButtonLabel label, PKIdentityButtonStyle style);

		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKIdentityDocument {
		[Export ("encryptedData")]
		NSData EncryptedData { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKIdentityDriversLicenseDescriptor : PKIdentityDocumentDescriptor {
	}

	[NoWatch, NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKIdentityElement : NSCopying {
		[Static]
		[Export ("givenNameElement")]
		PKIdentityElement GivenNameElement { get; }

		[Static]
		[Export ("familyNameElement")]
		PKIdentityElement FamilyNameElement { get; }

		[Static]
		[Export ("portraitElement")]
		PKIdentityElement PortraitElement { get; }

		[Static]
		[Export ("addressElement")]
		PKIdentityElement AddressElement { get; }

		[Static]
		[Export ("issuingAuthorityElement")]
		PKIdentityElement IssuingAuthorityElement { get; }

		[Static]
		[Export ("documentIssueDateElement")]
		PKIdentityElement DocumentIssueDateElement { get; }

		[Static]
		[Export ("documentExpirationDateElement")]
		PKIdentityElement DocumentExpirationDateElement { get; }

		[Static]
		[Export ("documentNumberElement")]
		PKIdentityElement DocumentNumberElement { get; }

		[Static]
		[Export ("drivingPrivilegesElement")]
		PKIdentityElement DrivingPrivilegesElement { get; }

		[Static]
		[Export ("ageElement")]
		PKIdentityElement AgeElement { get; }

		[Static]
		[Export ("dateOfBirthElement")]
		PKIdentityElement DateOfBirthElement { get; }

		[Static]
		[Export ("ageThresholdElementWithAge:")]
		PKIdentityElement AgeThresholdElementWithAge (nint age);
	}

	[NoWatch, NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKIdentityIntentToStore : NSCopying {
		[Static]
		[Export ("willNotStoreIntent")]
		PKIdentityIntentToStore WillNotStoreIntent { get; }

		[Static]
		[Export ("mayStoreIntent")]
		PKIdentityIntentToStore MayStoreIntent { get; }

		[Static]
		[Export ("mayStoreIntentForDays:")]
		PKIdentityIntentToStore MayStoreIntentForDays (nint days);
	}

	[NoWatch, NoTV, NoMac, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface PKIdentityRequest {
		[NullAllowed, Export ("descriptor", ArgumentSemantic.Assign)]
		IPKIdentityDocumentDescriptor Descriptor { get; set; }

		[NullAllowed, Export ("nonce", ArgumentSemantic.Copy)]
		NSData Nonce { get; set; }

		[NullAllowed, Export ("merchantIdentifier")]
		string MerchantIdentifier { get; set; }
	}

	[NoWatch, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPaymentOrderDetails // : NSCopying, NSSecureCoding // https://feedbackassistant.apple.com/feedback/11018799
	{
		[Export ("initWithOrderTypeIdentifier:orderIdentifier:webServiceURL:authenticationToken:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string orderTypeIdentifier, string orderIdentifier, NSUrl webServiceUrl, string authenticationToken);

		[Export ("orderTypeIdentifier")]
		string OrderTypeIdentifier { get; set; }

		[Export ("orderIdentifier")]
		string OrderIdentifier { get; set; }

		[Export ("webServiceURL", ArgumentSemantic.Copy)]
		NSUrl WebServiceUrl { get; set; }

		[Export ("authenticationToken")]
		string AuthenticationToken { get; set; }
	}

	[NoWatch, iOS (16, 0), MacCatalyst (16, 0), Mac (13, 0), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPaymentTokenContext // : NSCoding, NSCopying, NSSecureCoding // https://feedbackassistant.apple.com/feedback/11018799
	{
		[Export ("initWithMerchantIdentifier:externalIdentifier:merchantName:merchantDomain:amount:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string merchantIdentifier, string externalIdentifier, string merchantName, [NullAllowed] string merchantDomain, NSDecimalNumber amount);

		[Export ("merchantIdentifier")]
		string MerchantIdentifier { get; set; }

		[Export ("externalIdentifier")]
		string ExternalIdentifier { get; set; }

		[Export ("merchantName")]
		string MerchantName { get; set; }

		[NullAllowed, Export ("merchantDomain")]
		string MerchantDomain { get; set; }

		[Export ("amount", ArgumentSemantic.Copy)]
		NSDecimalNumber Amount { get; set; }
	}

	[NoWatch, iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKRecurringPaymentRequest // : NSCoding, NSCopying, NSSecureCoding // https://feedbackassistant.apple.com/feedback/11018799
	{
		[Export ("initWithPaymentDescription:regularBilling:managementURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string paymentDescription, PKRecurringPaymentSummaryItem regularBilling, NSUrl managementUrl);

		[Export ("paymentDescription")]
		string PaymentDescription { get; set; }

		[Export ("regularBilling", ArgumentSemantic.Strong)]
		PKRecurringPaymentSummaryItem RegularBilling { get; set; }

		[NullAllowed, Export ("trialBilling", ArgumentSemantic.Strong)]
		PKRecurringPaymentSummaryItem TrialBilling { get; set; }

		[NullAllowed, Export ("billingAgreement")]
		string BillingAgreement { get; set; }

		[Export ("managementURL", ArgumentSemantic.Strong)]
		NSUrl ManagementUrl { get; set; }

		[NullAllowed, Export ("tokenNotificationURL", ArgumentSemantic.Strong)]
		NSUrl TokenNotificationUrl { get; set; }
	}

	[NoWatch, NoTV, iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKShareablePassMetadataPreview // : NSCoding, NSCopying, NSSecureCoding // https://feedbackassistant.apple.com/feedback/11018799
	{
		[Export ("initWithPassThumbnail:localizedDescription:")]
		NativeHandle Constructor (CGImage passThumbnail, string description);

		[Export ("initWithTemplateIdentifier:")]
		NativeHandle Constructor (string templateIdentifier);

		[Static]
		[Export ("previewWithPassThumbnail:localizedDescription:")]
		PKShareablePassMetadataPreview PreviewWithPassThumbnail (CGImage passThumbnail, string description);

		[Static]
		[Export ("previewWithTemplateIdentifier:")]
		PKShareablePassMetadataPreview PreviewWithTemplateIdentifier (string templateIdentifier);

		[NullAllowed, Export ("passThumbnailImage", ArgumentSemantic.Assign)]
		CGImage PassThumbnailImage { get; }

		[NullAllowed, Export ("localizedDescription", ArgumentSemantic.Strong)]
		string LocalizedDescription { get; }

		[NullAllowed, Export ("ownerDisplayName", ArgumentSemantic.Strong)]
		string OwnerDisplayName { get; set; }

		[NullAllowed, Export ("provisioningTemplateIdentifier", ArgumentSemantic.Strong)]
		string ProvisioningTemplateIdentifier { get; }
	}

	[iOS (16, 0), MacCatalyst (16, 0), NoTV, NoWatch, NoMac]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor]
	interface PKShareSecureElementPassViewController {
		// from UIViewController
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("initWithSecureElementPass:delegate:")]
		NativeHandle Constructor (PKSecureElementPass pass, [NullAllowed] IPKShareSecureElementPassViewControllerDelegate @delegate);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IPKShareSecureElementPassViewControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("promptToShareURL")]
		bool PromptToShareUrl { get; set; }
	}

	[iOS (16, 0), Mac (13, 0), Watch (9, 0), NoTV, MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKVehicleConnectionSession {
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IPKVehicleConnectionDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[Export ("connectionStatus", ArgumentSemantic.Assign)]
		PKVehicleConnectionSessionConnectionState ConnectionStatus { get; }

		[Async]
		[Static]
		[Export ("sessionForPass:delegate:completion:")]
		void GetSession (PKSecureElementPass pass, IPKVehicleConnectionDelegate @delegate, Action<PKVehicleConnectionSession, NSError> completion);

		[Export ("sendData:error:")]
		bool SendData (NSData message, [NullAllowed] out NSError error);

		[Export ("invalidate")]
		void Invalidate ();
	}

	[NoWatch, NoTV, Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKDeferredPaymentRequest {

		[Export ("paymentDescription")]
		string PaymentDescription { get; set; }

		[Export ("deferredBilling", ArgumentSemantic.Strong)]
		PKDeferredPaymentSummaryItem DeferredBilling { get; set; }

		[NullAllowed, Export ("billingAgreement")]
		string BillingAgreement { get; set; }

		[Export ("managementURL", ArgumentSemantic.Strong)]
		NSUrl ManagementUrl { get; set; }

		[NullAllowed, Export ("tokenNotificationURL", ArgumentSemantic.Strong)]
		NSUrl TokenNotificationUrl { get; set; }

		[NullAllowed, Export ("freeCancellationDate", ArgumentSemantic.Strong)]
		NSDate FreeCancellationDate { get; set; }

		[NullAllowed, Export ("freeCancellationDateTimeZone", ArgumentSemantic.Strong)]
		NSTimeZone FreeCancellationDateTimeZone { get; set; }

		[Export ("initWithPaymentDescription:deferredBilling:managementURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string paymentDescription, PKDeferredPaymentSummaryItem deferredBilling, NSUrl managementUrl);
	}

	[NoWatch, NoTV, NoMac, iOS (17, 0), NoMacCatalyst]
	[BaseType (typeof (UIView))]
	[DisableDefaultCtor]
	interface PKPayLaterView {

		[Export ("initWithAmount:currencyCode:")]
		NativeHandle Constructor (NSDecimalNumber amount, string currencyCode);

		[Export ("delegate", ArgumentSemantic.Assign)]
		IPKPayLaterViewDelegate Delegate { get; set; }

		[Export ("amount", ArgumentSemantic.Copy)]
		NSDecimalNumber Amount { get; set; }

		[Export ("currencyCode")]
		string CurrencyCode { get; set; }

		[Export ("displayStyle", ArgumentSemantic.Assign)]
		PKPayLaterDisplayStyle DisplayStyle { get; set; }

		[Export ("action", ArgumentSemantic.Assign)]
		PKPayLaterAction Action { get; set; }
	}

	interface IPKPayLaterViewDelegate { }

	[NoWatch, NoTV, NoMac, iOS (17, 0), NoMacCatalyst]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface PKPayLaterViewDelegate {
		[Abstract]
		[Export ("payLaterViewDidUpdateHeight:")]
		void PayLaterViewDidUpdateHeight (PKPayLaterView view);
	}

	[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[Static]
	interface PKDirbursementError {
		[Field ("PKDisbursementErrorContactFieldUserInfoKey")]
		NSString ContactFieldUserInfoKey { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (PKPaymentSummaryItem))]
	[DisableDefaultCtor]
	interface PKInstantFundsOutFeeSummaryItem : NSCoding, NSCopying, NSSecureCoding {
	}

	[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (PKPaymentSummaryItem))]
	[DisableDefaultCtor]
	interface PKDisbursementSummaryItem : NSCoding, NSCopying, NSSecureCoding {
	}
}
