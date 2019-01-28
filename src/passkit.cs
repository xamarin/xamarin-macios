//
// PassKit bindings
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012, 2015-2016 Xamarin Inc. All rights reserved.
//

using System;
using System.ComponentModel;
using Contacts;
using ObjCRuntime;
using Foundation;
using UIKit;
#if !WATCH
using AddressBook;
#else
interface ABRecord {}
#endif

namespace PassKit {

	[Watch (3,0)]
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface PKContact : NSSecureCoding
	{
		[NullAllowed, Export ("name", ArgumentSemantic.Strong)]
		NSPersonNameComponents Name { get; set; }
	
#if XAMCORE_2_0 // The Contacts framework (CNPostalAddress) uses generics heavily, which is only supported in Unified (for now at least)
		[NullAllowed, Export ("postalAddress", ArgumentSemantic.Retain)]
		CNPostalAddress PostalAddress { get; set; }
#endif // XAMCORE_2_0
	
		[NullAllowed, Export ("emailAddress", ArgumentSemantic.Strong)]
		string EmailAddress { get; set; }
	
#if XAMCORE_2_0 // The Contacts framework (CNPhoneNumber) uses generics heavily, which is only supported in Unified (for now at least)
		[NullAllowed, Export ("phoneNumber", ArgumentSemantic.Strong)]
		CNPhoneNumber PhoneNumber { get; set; }
#endif // XAMCORE_2_0

		[iOS (9,2)]
		[Deprecated (PlatformName.iOS, 10,3, message:"Use 'SubLocality' and 'SubAdministrativeArea' on 'PostalAddress' instead.")]
		[Deprecated (PlatformName.WatchOS, 3,2, message:"Use 'SubLocality' and 'SubAdministrativeArea' on 'PostalAddress' instead.")]
		[NullAllowed, Export ("supplementarySubLocality", ArgumentSemantic.Strong)]
		string SupplementarySubLocality { get; set; }
	}
	
	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	interface PKPassLibrary {
		[Static][Export ("isPassLibraryAvailable")]
		bool IsAvailable { get; }

		[Export ("containsPass:")]
		bool Contains (PKPass pass);

		[Export ("passes")]
		PKPass[] GetPasses ();

		[Export ("passWithPassTypeIdentifier:serialNumber:")]
		PKPass GetPass (string identifier, string serialNumber);

		[iOS (8,0)]
		[Export ("passesOfType:")]
		PKPass [] GetPasses (PKPassType passType);

		[Export ("removePass:")]
		void Remove (PKPass pass);

		[Export ("replacePassWithPass:")]
		bool Replace (PKPass pass);

		[iOS (7,0)]
		[Export ("addPasses:withCompletionHandler:")]
		[Async]
		void AddPasses (PKPass[] passes, [NullAllowed] Action<PKPassLibraryAddPassesStatus> completion);

		[Field ("PKPassLibraryDidChangeNotification")]
		[Notification]
		NSString DidChangeNotification { get; }

		[iOS (9,0)]
		[Field ("PKPassLibraryRemotePaymentPassesDidChangeNotification")]
		[Notification]
		NSString RemotePaymentPassesDidChangeNotification { get; }

		[iOS (8,0)]
		[Static,Export ("isPaymentPassActivationAvailable")]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use the library's instance 'IsLibraryPaymentPassActivationAvailable' property instead.")]
		bool IsPaymentPassActivationAvailable { get; }

		[iOS (9,0)]
		[Export ("isPaymentPassActivationAvailable")]
		bool IsLibraryPaymentPassActivationAvailable { get; }

		[NoWatch]
		[iOS (8,0)]
		[Async]
		[Export ("activatePaymentPass:withActivationData:completion:")]
		void ActivatePaymentPass (PKPaymentPass paymentPass, NSData activationData, [NullAllowed] Action<bool, NSError> completion);

		[NoWatch]
		[iOS (8,0)]
		[Async]
		[Export ("activatePaymentPass:withActivationCode:completion:")]
		void ActivatePaymentPass (PKPaymentPass paymentPass, string activationCode, [NullAllowed] Action<bool, NSError> completion);

		[NoWatch]
		[iOS (8,3)]
		[Export ("openPaymentSetup")]
		void OpenPaymentSetup ();

		[iOS (9,0)]
		[Export ("canAddPaymentPassWithPrimaryAccountIdentifier:")]
		bool CanAddPaymentPass (string primaryAccountIdentifier);

		[iOS (10,1)]
		[Watch (3,1)]
		[Export ("canAddFelicaPass")]
		bool CanAddFelicaPass { get; }

		[NoWatch]
		[iOS(9,0)]
		[Static]
		[Export ("endAutomaticPassPresentationSuppressionWithRequestToken:")]
		void EndAutomaticPassPresentationSuppression (nuint requestToken);

		[NoWatch]
		[iOS(9,0)]
		[Static]
		[Export ("isSuppressingAutomaticPassPresentation")]
		bool IsSuppressingAutomaticPassPresentation { get; }

		[iOS (9,0)]
		[Export ("remotePaymentPasses")]
		PKPaymentPass[] RemotePaymentPasses { get; }

#if !WATCH
		[NoWatch]
		[iOS(9,0)]
		[Static]
		[Export ("requestAutomaticPassPresentationSuppressionWithResponseHandler:")]
		nuint RequestAutomaticPassPresentationSuppression (Action<PKAutomaticPassPresentationSuppressionResult> responseHandler);
#endif
		[NoWatch][iOS (10,0)]
		[Export ("presentPaymentPass:")]
		void PresentPaymentPass (PKPaymentPass pass);
	}

	[iOS (6,0)]
	[Static]
	interface PKPassLibraryUserInfoKey
	{
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
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface PKPayment {
		[Export ("token", ArgumentSemantic.Strong)]
		PKPaymentToken Token { get; }

		[NoWatch]
		[Export ("billingAddress", ArgumentSemantic.Assign)]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'BillingContact' instead.")]
		ABRecord BillingAddress { get; }

		[NoWatch]
		[Export ("shippingAddress", ArgumentSemantic.Assign)]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'ShippingContact' instead.")]
		ABRecord ShippingAddress { get; }

		[Export ("shippingMethod", ArgumentSemantic.Strong)]
		PKShippingMethod ShippingMethod { get; }

		
		[iOS (9,0)]
		[NullAllowed, Export ("shippingContact", ArgumentSemantic.Strong)]
		PKContact ShippingContact { get; }

		[iOS (9,0)]
		[NullAllowed, Export ("billingContact", ArgumentSemantic.Strong)]
		PKContact BillingContact { get; }
	}

#if !WATCH
	delegate void PKPaymentShippingAddressSelected (PKPaymentAuthorizationStatus status, PKShippingMethod [] shippingMethods, PKPaymentSummaryItem [] summaryItems);
	delegate void PKPaymentShippingMethodSelected (PKPaymentAuthorizationStatus status, PKPaymentSummaryItem[] summaryItems);

#if !XAMCORE_2_0
	delegate void PKPaymentAuthorizationHandler (PKPaymentAuthorizationStatus status);
#endif

	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface PKPaymentAuthorizationViewControllerDelegate {
		
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'DidAuthorizePayment2' instead.")]
		[Export ("paymentAuthorizationViewController:didAuthorizePayment:completion:")]
		[EventArgs ("PKPaymentAuthorization")]
#if !XAMCORE_4_0
		[Abstract]
#endif
		void DidAuthorizePayment (PKPaymentAuthorizationViewController controller, PKPayment payment, 
#if XAMCORE_2_0
			Action<PKPaymentAuthorizationStatus> completion);
#else
			PKPaymentAuthorizationHandler completion);
#endif

		[iOS (11,0)]
		[Export ("paymentAuthorizationViewController:didAuthorizePayment:handler:")]
		[EventArgs ("PKPaymentAuthorizationResult")]
		void DidAuthorizePayment2 (PKPaymentAuthorizationViewController controller, PKPayment payment, Action<PKPaymentAuthorizationResult> completion);

		[Export ("paymentAuthorizationViewControllerDidFinish:")]
		[Abstract]
		void PaymentAuthorizationViewControllerDidFinish (PKPaymentAuthorizationViewController controller);

		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'DidSelectShippingMethod2' instead.")]
		[Export ("paymentAuthorizationViewController:didSelectShippingMethod:completion:")]
		[EventArgs ("PKPaymentShippingMethodSelected")]
		void DidSelectShippingMethod (PKPaymentAuthorizationViewController controller, PKShippingMethod shippingMethod, PKPaymentShippingMethodSelected completion);

		[iOS (11,0)]
		[Export ("paymentAuthorizationViewController:didSelectShippingMethod:handler:")]
		[EventArgs ("PKPaymentRequestShippingMethodUpdate")]
		void DidSelectShippingMethod2 (PKPaymentAuthorizationViewController controller, PKShippingMethod shippingMethod, Action<PKPaymentRequestShippingMethodUpdate> completion);

		[Export ("paymentAuthorizationViewController:didSelectShippingAddress:completion:")]
		[EventArgs ("PKPaymentShippingAddressSelected")]
		void DidSelectShippingAddress (PKPaymentAuthorizationViewController controller, ABRecord address, PKPaymentShippingAddressSelected completion);

		[iOS (8,3)]
		[Export ("paymentAuthorizationViewControllerWillAuthorizePayment:")]
#if !XAMCORE_4_0
		[Abstract]
#endif
		void WillAuthorizePayment (PKPaymentAuthorizationViewController controller);

		[iOS (9,0)]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'DidSelectShippingContact' instead.")]
		[Export ("paymentAuthorizationViewController:didSelectShippingContact:completion:")]
		[EventArgs ("PKPaymentSelectedContact")]
		void DidSelectShippingContact (PKPaymentAuthorizationViewController controller, PKContact contact, PKPaymentShippingAddressSelected completion);

		[iOS (11,0)]
		[Export ("paymentAuthorizationViewController:didSelectShippingContact:handler:")]
		[EventArgs ("PKPaymentRequestShippingContactUpdate")]
		void DidSelectShippingContact2 (PKPaymentAuthorizationViewController controller, PKContact contact, Action<PKPaymentRequestShippingContactUpdate> completion);

		[iOS (9,0)]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'DidSelectPaymentMethod2' instead.")]
		[Export ("paymentAuthorizationViewController:didSelectPaymentMethod:completion:")]
		[EventArgs ("PKPaymentMethodSelected")]
		void DidSelectPaymentMethod (PKPaymentAuthorizationViewController controller, PKPaymentMethod paymentMethod, Action<PKPaymentSummaryItem[]> completion);

		[iOS (11,0)]
		[Export ("paymentAuthorizationViewController:didSelectPaymentMethod:handler:")]
		[EventArgs ("PKPaymentRequestPaymentMethodUpdate")]
		void DidSelectPaymentMethod2 (PKPaymentAuthorizationViewController controller, PKPaymentMethod paymentMethod, Action<PKPaymentRequestPaymentMethodUpdate> completion);
	}

	[iOS (8,0)]
	[BaseType (typeof (UIViewController), Delegates=new string []{"Delegate"}, Events=new Type [] {typeof(PKPaymentAuthorizationViewControllerDelegate)})]
	interface PKPaymentAuthorizationViewController {
		[DesignatedInitializer]
		[Export ("initWithPaymentRequest:")]
		IntPtr Constructor (PKPaymentRequest request);

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

		[iOS (9,0)]
		[Static]
		[Export ("canMakePaymentsUsingNetworks:capabilities:")]
		bool CanMakePaymentsUsingNetworks (string[] supportedNetworks, PKMerchantCapability capabilties);
	}
#endif

	[Watch (3,0)]
	[iOS (8,0)]
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

		[iOS (9,0)]
		[Export ("type", ArgumentSemantic.Assign)]
		PKPaymentSummaryItemType Type { get; set; }

		[iOS (9,0)]
		[Static]
		[Export ("summaryItemWithLabel:amount:type:")]
		PKPaymentSummaryItem Create (string label, NSDecimalNumber amount, PKPaymentSummaryItemType type);
	}

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (PKPaymentSummaryItem))]
	interface PKShippingMethod {
		[NullAllowed] // by default this property is null
		[Export ("identifier")]
		string Identifier { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("detail")]
		string Detail { get; set; }
	}

	[Watch (3,0)]
	[iOS (8,0)]
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

		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'RequiredBillingContactFields' instead.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'RequiredBillingContactFields' instead.")]
		[Export ("requiredBillingAddressFields", ArgumentSemantic.UnsafeUnretained)]
		PKAddressField RequiredBillingAddressFields { get; set; }

		[NoWatch]
		[NullAllowed] // by default this property is null
		[Export ("billingAddress", ArgumentSemantic.Assign)]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'BillingContact' instead.")]
		ABRecord BillingAddress { get; set; }

		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'RequiredShippingContactFields' instead.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'RequiredShippingContactFields' instead.")]
		[Export ("requiredShippingAddressFields", ArgumentSemantic.UnsafeUnretained)]
		PKAddressField RequiredShippingAddressFields { get; set; }

		[NoWatch]
		[NullAllowed] // by default this property is null
		[Export ("shippingAddress", ArgumentSemantic.Assign)]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'ShippingContact' instead.")]
		ABRecord ShippingAddress { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("shippingMethods", ArgumentSemantic.Copy)]
		PKShippingMethod [] ShippingMethods { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("applicationData", ArgumentSemantic.Copy)]
		NSData ApplicationData { get; set; }

		[iOS (8,3)]
		[Export ("shippingType", ArgumentSemantic.Assign)]
		PKShippingType ShippingType { get; set; }

		[iOS (9,0)]
		[NullAllowed, Export ("shippingContact", ArgumentSemantic.Strong)]
		PKContact ShippingContact { get; set; }

		[iOS (9,0)]
		[NullAllowed, Export ("billingContact", ArgumentSemantic.Strong)]
		PKContact BillingContact { get; set; }

		[Watch (3,0)][iOS (10,0)]
		[Static]
		[Export ("availableNetworks")]
		NSString[] AvailableNetworks { get; }

		[Watch (4,0)][iOS (11,0)]
		[Export ("requiredBillingContactFields", ArgumentSemantic.Strong)]
		NSSet WeakRequiredBillingContactFields { get; set; }

		[Watch (4,0)][iOS (11,0)]
		[Export ("requiredShippingContactFields", ArgumentSemantic.Strong)]
		NSSet WeakRequiredShippingContactFields { get; set; }

		[Watch (4,0)][iOS (11,0)]
		[NullAllowed, Export ("supportedCountries", ArgumentSemantic.Copy)]
		NSSet<NSString> SupportedCountries { get; set; }

		[Watch (4,0)][iOS (11,0)]
		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("paymentContactInvalidErrorWithContactField:localizedDescription:")]
		NSError CreatePaymentContactInvalidError (NSString field, [NullAllowed] string localizedDescription);

		[Watch (4,0)][iOS (11,0)]
		[Static]
		[Wrap ("CreatePaymentContactInvalidError (contactField.GetConstant (), localizedDescription)")]
		NSError CreatePaymentContactInvalidError (PKContactFields contactField, [NullAllowed] string localizedDescription);

		[Watch (4,0)][iOS (11,0)]
		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("paymentShippingAddressInvalidErrorWithKey:localizedDescription:")]
		NSError CreatePaymentShippingAddressInvalidError (NSString postalAddressKey, [NullAllowed] string localizedDescription);

#if XAMCORE_2_0
		[Watch (4,0)][iOS (11,0)]
		[Static]
		[Wrap ("CreatePaymentShippingAddressInvalidError (postalAddress.GetConstant (), localizedDescription)")]
		NSError CreatePaymentShippingAddressInvalidError (CNPostalAddressKeyOption postalAddress, [NullAllowed] string localizedDescription);
#endif

		[Watch (4,0)][iOS (11,0)]
		[Static]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("paymentBillingAddressInvalidErrorWithKey:localizedDescription:")]
		NSError CreatePaymentBillingAddressInvalidError (NSString postalAddressKey, [NullAllowed] string localizedDescription);

#if XAMCORE_2_0
		[Watch (4,0)][iOS (11,0)]
		[Static]
		[Wrap ("CreatePaymentBillingAddressInvalidError (postalAddress.GetConstant (), localizedDescription)")]
		NSError CreatePaymentBillingAddressInvalidError (CNPostalAddressKeyOption postalAddress, [NullAllowed] string localizedDescription);
#endif

		[Watch (4,0)][iOS (11,0)]
		[Static]
		[Export ("paymentShippingAddressUnserviceableErrorWithLocalizedDescription:")]
		NSError CreatePaymentShippingAddressUnserviceableError ([NullAllowed] string localizedDescription);
	}

	[Watch (4,0)][iOS (11,0)]
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

	[Watch (3,0)]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	interface PKPaymentToken {

		[NoWatch]
		[Export ("paymentInstrumentName", ArgumentSemantic.Copy)]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'PaymentMethod' instead.")]
		string PaymentInstrumentName { get; }

		[NoWatch]
		[Export ("paymentNetwork")]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'PaymentMethod' instead.")]
		string PaymentNetwork { get; }

		[Export ("transactionIdentifier")]
		string TransactionIdentifier { get; }

		[Export ("paymentData", ArgumentSemantic.Copy)]
		NSData PaymentData { get; }

		[iOS (9,0)]
		[Export ("paymentMethod", ArgumentSemantic.Strong)]
		PKPaymentMethod PaymentMethod { get; }		
	}

#if !WATCH
	[iOS (6,0)]
	[BaseType (typeof (UIViewController), Delegates = new string [] {"WeakDelegate"}, Events = new Type [] { typeof (PKAddPassesViewControllerDelegate) })]
	// invalid null handle for default 'init'
	[DisableDefaultCtor]
	interface PKAddPassesViewController {

		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("initWithPass:")]
		IntPtr Constructor (PKPass pass);

		[iOS (7,0)]
		[Export ("initWithPasses:")]
		IntPtr Constructor (PKPass[] pass);

		[iOS (8,0)]
		[Static]
		[Export ("canAddPasses")]
		bool CanAddPasses { get;}
			
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		PKAddPassesViewControllerDelegate Delegate { get; set;  }
	}

	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface PKAddPassesViewControllerDelegate {
		[Export ("addPassesViewControllerDidFinish:")]
		void Finished (PKAddPassesViewController controller);
	}

	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // designated
	interface PKAddPaymentPassRequest : NSSecureCoding
	{
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[NullAllowed, Export ("encryptedPassData", ArgumentSemantic.Copy)]
		NSData EncryptedPassData { get; set; }
	
		[NullAllowed, Export ("activationData", ArgumentSemantic.Copy)]
		NSData ActivationData { get; set; }
	
		[NullAllowed, Export ("ephemeralPublicKey", ArgumentSemantic.Copy)]
		NSData EphemeralPublicKey { get; set; }
	
		[NullAllowed, Export ("wrappedKey", ArgumentSemantic.Copy)]
		NSData WrappedKey { get; set; }
	}

	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface PKAddPaymentPassRequestConfiguration : NSSecureCoding
	{
		[DesignatedInitializer]
		[Export ("initWithEncryptionScheme:")]
		IntPtr Constructor (NSString encryptionScheme);
	
		[Export ("encryptionScheme")]
		NSString EncryptionScheme { get; }
	
		[NullAllowed, Export ("cardholderName")]
		string CardholderName { get; set; }
	
		[NullAllowed, Export ("primaryAccountSuffix")]
		string PrimaryAccountSuffix { get; set; }

		[iOS (10,1)]
		[NoWatch] // Radar: https://trello.com/c/MvaHEZlc
		[Export ("cardDetails", ArgumentSemantic.Copy)]
		PKLabeledValue[] CardDetails { get; set; }
	
		[NullAllowed, Export ("localizedDescription")]
		string LocalizedDescription { get; set; }
	
		[NullAllowed, Export ("primaryAccountIdentifier")]
		string PrimaryAccountIdentifier { get; set; }
	
		[NullAllowed, Export ("paymentNetwork")]
		string PaymentNetwork { get; set; }

		[iOS (10,1)]
		[NoWatch] // Radar: https://trello.com/c/MvaHEZlc
		[Export ("requiresFelicaSecureElement")]
		bool RequiresFelicaSecureElement { get; set; }

		[iOS (12, 0)]
		[Export ("style", ArgumentSemantic.Assign)]
		PKAddPaymentPassStyle Style { get; set; }
	}

	[iOS (9,0)]
	[BaseType (typeof(UIViewController))]
	[DisableDefaultCtor]
	interface PKAddPaymentPassViewController
	{
		[Static]
		[Export ("canAddPaymentPass")]
		bool CanAddPaymentPass { get; }
	
		[DesignatedInitializer]
		[Export ("initWithRequestConfiguration:delegate:")]
		IntPtr Constructor (PKAddPaymentPassRequestConfiguration configuration, [NullAllowed] IPKAddPaymentPassViewControllerDelegate viewControllerDelegate);

#if !XAMCORE_4_0
		[Obsolete ("Use the overload accepting a IPKAddPaymentPassViewControllerDelegate")]
		[Wrap ("this (configuration, (IPKAddPaymentPassViewControllerDelegate) viewControllerDelegate)")]
		IntPtr Constructor (PKAddPaymentPassRequestConfiguration configuration, PKAddPaymentPassViewControllerDelegate viewControllerDelegate);
#endif

		[Wrap ("WeakDelegate")]
		[NullAllowed, Protocolize]
		PKAddPaymentPassViewControllerDelegate Delegate { get; set; }
	
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

	interface IPKAddPaymentPassViewControllerDelegate {}
	
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface PKAddPaymentPassViewControllerDelegate
	{
		[Abstract]
		[Export ("addPaymentPassViewController:generateRequestWithCertificateChain:nonce:nonceSignature:completionHandler:")]
		void GenerateRequestWithCertificateChain (PKAddPaymentPassViewController controller, NSData[] certificates, NSData nonce, NSData nonceSignature, Action<PKAddPaymentPassRequest> handler);
	
		[Abstract]
		[Export ("addPaymentPassViewController:didFinishAddingPaymentPass:error:")]
		void DidFinishAddingPaymentPass (PKAddPaymentPassViewController controller, [NullAllowed] PKPaymentPass pass, [NullAllowed] NSError error);
	}
#endif // !WATCH
		
	[iOS (6,0)]
	[BaseType (typeof (PKObject))]
	interface PKPass : NSSecureCoding, NSCopying {
		[Export ("initWithData:error:")]
		IntPtr Constructor (NSData data, out NSError error);

		[Export ("authenticationToken", ArgumentSemantic.Copy)]
		string AuthenticationToken { get; }

		[NoWatch]
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

		[Export ("relevantDate", ArgumentSemantic.Copy)]
		NSDate RelevantDate { get; }

		[Export ("serialNumber", ArgumentSemantic.Copy)]
		string SerialNumber { get; }

		[Export ("webServiceURL", ArgumentSemantic.Copy)]
		NSUrl WebServiceUrl { get; }

		[Export ("localizedValueForFieldKey:")]
		NSObject GetLocalizedValue (NSString key); // TODO: Should be enum for PKPassLibraryUserInfoKey

#if !XAMCORE_4_0
		[Field ("PKPassKitErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		[iOS (7,0)]
		[Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary UserInfo { get; }

		[iOS (8,0)]
		[Export ("passType")]
		PKPassType PassType { get; }

		[iOS (8,0)]
		[Export ("paymentPass")]
		PKPaymentPass PaymentPass { get; }

		[iOS (9,0)]
		[Export ("remotePass")]
		bool RemotePass { [Bind ("isRemotePass")] get; }

		[iOS (9,0)]
		[Export ("deviceName")]
		string DeviceName { get; }		
	}

	[Watch (3,0)]
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface PKPaymentMethod : NSSecureCoding
	{
		[NullAllowed, Export ("displayName")]
		string DisplayName { get; }
	
		[NullAllowed, Export ("network")]
		string Network { get; }
	
		[Export ("type")]
		PKPaymentMethodType Type { get; }

		[NullAllowed, Export ("paymentPass", ArgumentSemantic.Copy)]
		PKPaymentPass PaymentPass { get; }
	}

	[iOS (8,0)]
	[BaseType (typeof (PKPass))]
	interface PKPaymentPass {

		[Export ("primaryAccountIdentifier")]
		string PrimaryAccountIdentifier { get; }

		[Export ("primaryAccountNumberSuffix")]
		string PrimaryAccountNumberSuffix { get; }

		[Export ("deviceAccountIdentifier")]
		string DeviceAccountIdentifier { get; }

		[Export ("deviceAccountNumberSuffix")]
		string DeviceAccountNumberSuffix { get; }

		[Export ("activationState")]
		PKPaymentPassActivationState ActivationState { get; }
	}
	
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	partial interface PKObject : NSCoding, NSSecureCoding, NSCopying {
		//Empty class in header file
	}

	[Static]
	[iOS (8,0)]
	[Watch (3,0)]
	interface PKPaymentNetwork {
		[Field ("PKPaymentNetworkAmex")]
		NSString Amex { get; }

		[iOS (10,3), Watch (3,2)]
		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'CartesBancaires' instead.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'CartesBancaires' instead.")]
		[Field ("PKPaymentNetworkCarteBancaire")]
		NSString CarteBancaire { get; }

		[iOS (11,0)][Watch (4,0)]
		[Deprecated (PlatformName.WatchOS, 4,2, message: "Use 'CartesBancaires' instead.")]
		[Deprecated (PlatformName.iOS, 11,2, message: "Use 'CartesBancaires' instead.")]
		[Field ("PKPaymentNetworkCarteBancaires")]
		NSString CarteBancaires { get; }

		[iOS (11,2)][Watch (4,2)]
		[Field ("PKPaymentNetworkCartesBancaires")]
		NSString CartesBancaires { get; }

		[iOS (9,2)]
		[Watch (2,2)]
		[Field ("PKPaymentNetworkChinaUnionPay")]
		NSString ChinaUnionPay { get; }

		[iOS (9,2)]
		[Watch (2,2)]
		[Field ("PKPaymentNetworkInterac")]
		NSString Interac { get; }

		[Field ("PKPaymentNetworkMasterCard")]
		NSString MasterCard { get; }

		[Field ("PKPaymentNetworkVisa")]
		NSString Visa { get; }

		[iOS (9,0)]
		[Field ("PKPaymentNetworkDiscover")]
		NSString Discover { get; }

		[iOS (9,0)]
		[Field ("PKPaymentNetworkPrivateLabel")]
		NSString PrivateLabel { get; }

		[Watch (3,1), iOS (10,1)]
		[Field ("PKPaymentNetworkJCB")]
		NSString Jcb { get; }

		[Watch (3,1), iOS (10,1)]
		[Field ("PKPaymentNetworkSuica")]
		NSString Suica { get; }

		[iOS (10,3), Watch (3,2)]
		[Field ("PKPaymentNetworkQuicPay")]
		NSString QuicPay { get; }

		[iOS (10,3), Watch (3,2)]
		[Field ("PKPaymentNetworkIDCredit")]
		NSString IDCredit { get; }

		[iOS (12,0), Watch (5,0)]
		[Field ("PKPaymentNetworkElectron")]
		NSString Electron { get; }

		[iOS (12,0), Watch (5,0)]
		[Field ("PKPaymentNetworkMaestro")]
		NSString Maestro { get; }

		[iOS (12,0), Watch (5,0)]
		[Field ("PKPaymentNetworkVPay")]
		NSString VPay { get; }

		[iOS (12,0), Watch (5,0)]
		[Field ("PKPaymentNetworkEftpos")]
		NSString Eftpos { get; }

		[Watch (5,1,2)][iOS (12,1,1)]
		[Field ("PKPaymentNetworkElo")]
		NSString Elo { get; }

		[Watch (5,1,2)][iOS (12,1,1)]
		[Field ("PKPaymentNetworkMada")]
		NSString Mada { get; }
	}

#if !WATCH
	[iOS (8,3)]
	[BaseType (typeof (UIButton))]
	[DisableDefaultCtor]
	interface PKPaymentButton {

		[Static]
		[Export ("buttonWithType:style:")]
		// note: named like UIButton method
		PKPaymentButton FromType (PKPaymentButtonType buttonType, PKPaymentButtonStyle buttonStyle);

		[iOS (9,0)]
		[Export ("initWithPaymentButtonType:paymentButtonStyle:")]
		[DesignatedInitializer]
		IntPtr Constructor (PKPaymentButtonType type, PKPaymentButtonStyle style);

		[iOS (12, 0)]
		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }
	}

	[iOS (9,0)]
	[BaseType (typeof (UIButton))]
	[DisableDefaultCtor]
	interface PKAddPassButton {
		[Static]
		[Export ("addPassButtonWithStyle:")]
		PKAddPassButton Create (PKAddPassButtonStyle addPassButtonStyle);

		[Export ("initWithAddPassButtonStyle:")]
		[DesignatedInitializer]
		IntPtr Constructor (PKAddPassButtonStyle style);

		[Appearance]
		[Export ("addPassButtonStyle", ArgumentSemantic.Assign)]
		PKAddPassButtonStyle Style { get; set; }
	}
#endif // !WATCH

	[iOS(9,0)]
	[Static]
	interface PKEncryptionScheme {
		[Field ("PKEncryptionSchemeECC_V2")]
		NSString Ecc_V2 { get; }

		[iOS (10,0)]
		[Watch (3,0)]
		[Field ("PKEncryptionSchemeRSA_V2")]
		NSString Rsa_V2 { get; }
	}

	[Watch (3,0)][iOS (10,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // providing DesignatedInitializer
	interface PKPaymentAuthorizationController {

		[Static]
		[Export ("canMakePayments")]
		bool CanMakePayments { get; }

		[Static]
		[Export ("canMakePaymentsUsingNetworks:")]
		bool CanMakePaymentsUsingNetworks (string[] supportedNetworks);

		[Static]
		[Export ("canMakePaymentsUsingNetworks:capabilities:")]
		bool CanMakePaymentsUsingNetworks (string[] supportedNetworks, PKMerchantCapability capabilties);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IPKPaymentAuthorizationControllerDelegate Delegate { get; set; }

		[Export ("initWithPaymentRequest:")]
		[DesignatedInitializer]
		IntPtr Constructor (PKPaymentRequest request);

		[Async]
		[Export ("presentWithCompletion:")]
		void Present ([NullAllowed] Action<bool> completion);

		[Async]
		[Export ("dismissWithCompletion:")]
		void Dismiss ([NullAllowed] Action completion);
	}

	interface IPKPaymentAuthorizationControllerDelegate {}

	[Watch (3,0)][iOS (10,0)]
	[Protocol][Model]
	[BaseType (typeof (NSObject))]
	interface PKPaymentAuthorizationControllerDelegate {

		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'DidAuthorizePayment' overload with the 'Action<PKPaymentAuthorizationResult>' parameter instead.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'DidAuthorizePayment' overload with the 'Action<PKPaymentAuthorizationResult>' parameter instead.")]
		[Abstract]
		[Export ("paymentAuthorizationController:didAuthorizePayment:completion:")]
		void DidAuthorizePayment (PKPaymentAuthorizationController controller, PKPayment payment, Action<PKPaymentAuthorizationStatus> completion);

		[Watch (4,0)][iOS (11,0)]
		[Export ("paymentAuthorizationController:didAuthorizePayment:handler:")]
		void DidAuthorizePayment (PKPaymentAuthorizationController controller, PKPayment payment, Action<PKPaymentAuthorizationResult> completion);

		[Abstract]
		[Export ("paymentAuthorizationControllerDidFinish:")]
		void DidFinish (PKPaymentAuthorizationController controller);

		[Export ("paymentAuthorizationControllerWillAuthorizePayment:")]
		void WillAuthorizePayment (PKPaymentAuthorizationController controller);

		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'DidSelectShippingMethod' overload with the 'Action<PKPaymentRequestPaymentMethodUpdate>' parameter instead.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'DidSelectShippingMethod' overload with the 'Action<PKPaymentRequestPaymentMethodUpdate>' parameter instead.")]
		[Export ("paymentAuthorizationController:didSelectShippingMethod:completion:")]
		void DidSelectShippingMethod (PKPaymentAuthorizationController controller, PKShippingMethod shippingMethod, Action<PKPaymentAuthorizationStatus, PKPaymentSummaryItem[]> completion);

		[Watch (4,0)][iOS (11,0)]
		[Export ("paymentAuthorizationController:didSelectShippingMethod:handler:")]
		void DidSelectShippingMethod (PKPaymentAuthorizationController controller, PKPaymentMethod paymentMethod, Action<PKPaymentRequestPaymentMethodUpdate> completion);

		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'DidSelectShippingContact' overload with the 'Action<PKPaymentRequestShippingContactUpdate>' parameter instead.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'DidSelectShippingContact' overload with the 'Action<PKPaymentRequestShippingContactUpdate>' parameter instead.")]
		[Export ("paymentAuthorizationController:didSelectShippingContact:completion:")]
		void DidSelectShippingContact (PKPaymentAuthorizationController controller, PKContact contact, Action<PKPaymentAuthorizationStatus, PKShippingMethod[], PKPaymentSummaryItem[]> completion);

		[Watch (4,0)][iOS (11,0)]
		[Export ("paymentAuthorizationController:didSelectShippingContact:handler:")]
		void DidSelectShippingContact (PKPaymentAuthorizationController controller, PKContact contact, Action<PKPaymentRequestShippingContactUpdate> completion);

		[Deprecated (PlatformName.WatchOS, 4,0, message: "Use 'DidSelectPaymentMethod' overload with the 'Action<PKPaymentRequestPaymentMethodUpdate>' parameter instead.")]
		[Deprecated (PlatformName.iOS, 11,0, message: "Use 'DidSelectPaymentMethod' overload with the 'Action<PKPaymentRequestPaymentMethodUpdate>' parameter instead.")]
		[Export ("paymentAuthorizationController:didSelectPaymentMethod:completion:")]
		void DidSelectPaymentMethod (PKPaymentAuthorizationController controller, PKPaymentMethod paymentMethod, Action<PKPaymentSummaryItem[]> completion);

		[Watch (4,0)][iOS (11,0)]
		[Export ("paymentAuthorizationController:didSelectPaymentMethod:handler:")]
		void DidSelectPaymentMethod (PKPaymentAuthorizationController controller, PKPaymentMethod paymentMethod, Action<PKPaymentRequestPaymentMethodUpdate> completion);
	}

	[iOS (10,1)]
	[NoWatch] // Radar: https://trello.com/c/MvaHEZlc
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // there's a designated initializer and it does not accept null
	interface PKLabeledValue
	{
		[Export ("initWithLabel:value:")]
		[DesignatedInitializer]
		IntPtr Constructor (string label, string value);

		[Export ("label")]
		string Label { get; }

		[Export ("value")]
		string Value { get; }
	}

	[Watch (4,3), iOS (11,3)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKTransitPassProperties {

		[Static]
		[Export ("passPropertiesForPass:")]
		[return: NullAllowed]
		PKTransitPassProperties GetPassProperties (PKPass pass);

		[Export ("transitBalance", ArgumentSemantic.Copy)]
		NSDecimalNumber TransitBalance { get; }

		[Export ("transitBalanceCurrencyCode")]
		string TransitBalanceCurrencyCode { get; }

		[Export ("inStation")]
		bool InStation { [Bind ("isInStation")] get; }

		[Export ("blacklisted")]
		bool Blacklisted { [Bind ("isBlacklisted")] get; }

		[NullAllowed, Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; }
	}

	[Watch (3,1), iOS (10,1)]
#if XAMCORE_4_0
	[DisableDefaultCtor] // hint: getter only props and a factory method.
#endif
	[BaseType (typeof (PKTransitPassProperties))]
	interface PKSuicaPassProperties
	{
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

		[Watch (4,3), iOS (11,3)]
		[Export ("balanceAllowedForCommute")]
		bool BalanceAllowedForCommute { [Bind ("isBalanceAllowedForCommute")] get; }

		[Watch (4,3), iOS (11,3)]
		[Export ("lowBalanceGateNotificationEnabled")]
		bool LowBalanceGateNotificationEnabled { [Bind ("isLowBalanceGateNotificationEnabled")] get; }

		[Export ("greenCarTicketUsed")]
		bool GreenCarTicketUsed { [Bind ("isGreenCarTicketUsed")] get; }

		[Export ("blacklisted")]
		bool Blacklisted { [Bind ("isBlacklisted")] get; }
	}

	[Watch (4,0)][iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKPaymentAuthorizationResult {
		[Export ("initWithStatus:errors:")]
		[DesignatedInitializer]
		IntPtr Constructor (PKPaymentAuthorizationStatus status, [NullAllowed] NSError[] errors);

		[Export ("status", ArgumentSemantic.Assign)]
		PKPaymentAuthorizationStatus Status { get; set; }

		[Export ("errors", ArgumentSemantic.Copy)]
		NSError[] Errors { get; set; }
	}

	[Watch (4,0)][iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface PKPaymentRequestUpdate {

		[Export ("initWithPaymentSummaryItems:")]
		[DesignatedInitializer]
		IntPtr Constructor (PKPaymentSummaryItem[] paymentSummaryItems);

		[Export ("status", ArgumentSemantic.Assign)]
		PKPaymentAuthorizationStatus Status { get; set; }

		[Export ("paymentSummaryItems", ArgumentSemantic.Copy)]
		PKPaymentSummaryItem[] PaymentSummaryItems { get; set; }
	}

	[Watch (4,0)][iOS (11,0)]
	[BaseType (typeof (PKPaymentRequestUpdate))]
	[DisableDefaultCtor]
	interface PKPaymentRequestShippingContactUpdate {

		[Export ("initWithErrors:paymentSummaryItems:shippingMethods:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSError[] errors, PKPaymentSummaryItem[] paymentSummaryItems, PKShippingMethod[] shippingMethods);

		[Export ("shippingMethods", ArgumentSemantic.Copy)]
		PKShippingMethod[] ShippingMethods { get; set; }

		[Export ("errors", ArgumentSemantic.Copy)]
		NSError[] Errors { get; set; }
	}

	[Watch (4,0)][iOS (11,0)]
	[BaseType (typeof (PKPaymentRequestUpdate))]
	[DisableDefaultCtor]
	interface PKPaymentRequestShippingMethodUpdate {

		// inlined
		[Export ("initWithPaymentSummaryItems:")]
		[DesignatedInitializer]
		IntPtr Constructor (PKPaymentSummaryItem[] paymentSummaryItems);
	}

	[Watch (4,0)][iOS (11,0)]
	[BaseType (typeof (PKPaymentRequestUpdate))]
	[DisableDefaultCtor]
	interface PKPaymentRequestPaymentMethodUpdate {

		// inlined
		[Export ("initWithPaymentSummaryItems:")]
		[DesignatedInitializer]
		IntPtr Constructor (PKPaymentSummaryItem[] paymentSummaryItems);
	}

	[Static] // not to enum'ify - exposed as NSString inside NSError
	interface PKPaymentErrorKeys {

		[Watch (4,0)][iOS (11,0)]
		[Field ("PKPaymentErrorContactFieldUserInfoKey")]
		NSString ContactFieldUserInfoKey { get; }

		[Watch (4,0)][iOS (11,0)]
		[Field ("PKPaymentErrorPostalAddressUserInfoKey")]
		NSString PostalAddressUserInfoKey { get; }
	}

	interface IPKDisbursementAuthorizationControllerDelegate { }

	[NoWatch]
	[iOS (12,2)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface PKDisbursementAuthorizationControllerDelegate {
#if false // missing PKDisbursementVoucher.h header inn xcode 10.2 beta 1
		[Abstract]
		[Export ("disbursementAuthorizationController:didAuthorizeWithDisbursementVoucher:")]
		void DidAuthorize (PKDisbursementAuthorizationController controller, PKDisbursementVoucher disbursementVoucher);
#endif
		[Abstract]
		[Export ("disbursementAuthorizationControllerDidFinish:")]
		void DidFinish (PKDisbursementAuthorizationController controller);
	}

	[NoWatch]
	[iOS (12,2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface PKDisbursementAuthorizationController {

		[Export ("initWithDisbursementRequest:delegate:")]
		IntPtr Constructor (PKDisbursementRequest disbursementRequest, IPKDisbursementAuthorizationControllerDelegate @delegate);

		[Wrap ("WeakDelegate")]
		IPKDisbursementAuthorizationControllerDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Assign)]
		NSObject WeakDelegate { get; }

		[Async]
		[Export ("authorizeDisbursementWithCompletion:")]
		void AuthorizeDisbursement (Action<bool, NSError> completion);

		[Static]
		[Export ("supportsDisbursements")]
		bool SupportsDisbursements { get; }
	}

	[NoWatch]
	[iOS (12, 2)]
	[Native]
	public enum PKDisbursementRequestSchedule : long {
		OneTime,
		Future,
	}

	[NoWatch]
	[iOS (12, 2)]
	[BaseType (typeof (NSObject))]
	interface PKDisbursementRequest {
		[NullAllowed, Export ("amount", ArgumentSemantic.Copy)]
		NSDecimalNumber Amount { get; set; }

		[NullAllowed, Export ("currencyCode")]
		string CurrencyCode { get; set; }

		[Export ("countryCode")]
		string CountryCode { get; set; }

		[Export ("requestSchedule", ArgumentSemantic.Assign)]
		PKDisbursementRequestSchedule RequestSchedule { get; set; }

		[NullAllowed, Export ("summaryItems", ArgumentSemantic.Copy)]
		PKPaymentSummaryItem [] SummaryItems { get; set; }
	}
}
