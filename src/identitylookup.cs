//
// IdentityLookup C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
// Copyright 2019 Microsoft Corporation.
//

using System;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace IdentityLookup {

	/// <summary>Enumerates actions that can be taken in response to a message.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum ILMessageFilterAction : long {
		None = 0,
		Allow = 1,
		Junk = 2,
#if !NET
		[Obsolete ("Use 'Junk' instead.")]
		Filter = Junk,
#endif
		[iOS (14, 0)]
		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		Promotion = 3,
		[iOS (14, 0)]
		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		Transaction = 4,
	}

	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[ErrorDomain ("ILMessageFilterErrorDomain")]
	[Native]
	public enum ILMessageFilterError : long {
		System = 1,
		InvalidNetworkUrl = 2,
		NetworkUrlUnauthorized = 3,
		NetworkRequestFailed = 4,
		RedundantNetworkDeferral = 5,
	}

	/// <summary>Enumerates message classification actions.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	enum ILClassificationAction : long {
		None = 0,
		ReportNotJunk = 1,
		ReportJunk = 2,
		ReportJunkAndBlockSender = 3,
	}

	[NoWatch, NoTV, NoMac, MacCatalyst (16, 0), iOS (16, 0)]
	[Native]
	public enum ILMessageFilterSubAction : long {
		None = 0,
		TransactionalOthers = 10000,
		TransactionalFinance = 10001,
		TransactionalOrders = 10002,
		TransactionalReminders = 10003,
		TransactionalHealth = 10004,
		TransactionalWeather = 10005,
		TransactionalCarrier = 10006,
		TransactionalRewards = 10007,
		TransactionalPublicServices = 10008,
		PromotionalOthers = 20000,
		PromotionalOffers = 20001,
		PromotionalCoupons = 20002,
	}

	/// <summary>Base class for message filter extensions.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface ILMessageFilterExtension {
	}

	/// <summary>Represents the extension context for a message filter.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSExtensionContext))]
	interface ILMessageFilterExtensionContext {

		[Export ("deferQueryRequestToNetworkWithCompletion:")]
		[Async]
		void DeferQueryRequestToNetwork (Action<ILNetworkResponse, NSError> completion);
	}

	interface IILMessageFilterQueryHandling { }

	/// <summary>Interface that is used by a message filter extension to respond to queries.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface ILMessageFilterQueryHandling {

		[Abstract]
		[Export ("handleQueryRequest:context:completion:")]
		void HandleQueryRequest (ILMessageFilterQueryRequest queryRequest, ILMessageFilterExtensionContext context, Action<ILMessageFilterQueryResponse> completion);
	}

	/// <summary>Represents a request to a message filter to examine a message from an unknown sender for filtering.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ILMessageFilterQueryRequest : NSSecureCoding {

		[NullAllowed, Export ("sender")]
		string Sender { get; }

		[NullAllowed, Export ("messageBody")]
		string MessageBody { get; }

		[NoWatch, NoTV, NoMac, MacCatalyst (16, 0), iOS (16, 0)]
		[NullAllowed, Export ("receiverISOCountryCode")]
		string ReceiverIsoCountryCode { get; }
	}

	/// <summary>Represents a message query response.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface ILMessageFilterQueryResponse : NSSecureCoding {

		[Export ("action", ArgumentSemantic.Assign)]
		ILMessageFilterAction Action { get; set; }

		[NoWatch, NoTV, NoMac, MacCatalyst (16, 0), iOS (16, 0)]
		[Export ("subAction", ArgumentSemantic.Assign)]
		ILMessageFilterSubAction SubAction { get; set; }
	}

	/// <summary>Represents a response to a network request by the filter extension.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ILNetworkResponse : NSSecureCoding {

		[Export ("urlResponse")]
		NSHttpUrlResponse UrlResponse { get; }

		[Export ("data")]
		NSData Data { get; }
	}

	/// <summary>A classification request.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (ILClassificationRequest))]
	[DisableDefaultCtor]
	interface ILCallClassificationRequest : NSSecureCoding {

		[Export ("callCommunications", ArgumentSemantic.Copy)]
		ILCallCommunication [] CallCommunications { get; }
	}

	/// <summary>An incoming call.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (ILCommunication))]
	[DisableDefaultCtor]
	interface ILCallCommunication {

		[Export ("isEqualToCallCommunication:")]
		bool IsEqualTo (ILCallCommunication communication);
	}

	/// <summary>Base class for classes that support users in reporting unwanted communications.</summary>
	[Abstract]
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ILClassificationRequest : NSSecureCoding {

	}

	/// <summary>Handles the classification of messages to mark them as junk, mark them as not junk, and/or to block the sender.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ILClassificationResponse : NSSecureCoding {

		[Export ("action", ArgumentSemantic.Assign)]
		ILClassificationAction Action { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("userString")]
		string UserString { get; set; }

		// Objects can be NSString, NSNumber, NSArray, NSDictionary, or NSNull
		[NullAllowed, Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> UserInfo { get; set; }

		[Export ("initWithClassificationAction:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ILClassificationAction action);
	}

	/// <summary>Base class for incoming calls and messages.</summary>
	[Abstract]
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ILCommunication : NSSecureCoding {

		[NullAllowed, Export ("sender")]
		string Sender { get; }

		[Export ("dateReceived", ArgumentSemantic.Copy)]
		NSDate DateReceived { get; }

		[Export ("isEqualToCommunication:")]
		bool IsEqualTo (ILCommunication communication);
	}

	/// <summary>A request to report a message.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (ILClassificationRequest))]
	[DisableDefaultCtor]
	interface ILMessageClassificationRequest : NSSecureCoding {

		[Export ("messageCommunications", ArgumentSemantic.Copy)]
		ILMessageCommunication [] MessageCommunications { get; }
	}

	/// <summary>An incoming SMS message.</summary>
	[NoMac]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (ILCommunication))]
	[DisableDefaultCtor]
	interface ILMessageCommunication {

		[NullAllowed, Export ("messageBody")]
		string MessageBody { get; }

		[Export ("isEqualToMessageCommunication:")]
		bool IsEqualTo (ILMessageCommunication communication);
	}

	[NoWatch, NoTV, NoMac, MacCatalyst (16, 0), iOS (16, 0)]
	[Protocol]
	interface ILMessageFilterCapabilitiesQueryHandling {
		[Abstract]
		[Export ("handleCapabilitiesQueryRequest:context:completion:")]
		void HandleQueryRequest (ILMessageFilterCapabilitiesQueryRequest capabilitiesQueryRequest, ILMessageFilterExtensionContext context, Action<ILMessageFilterCapabilitiesQueryResponse> completion);
	}

	[NoWatch, NoTV, NoMac, MacCatalyst (16, 0), iOS (16, 0)]
	[BaseType (typeof (NSObject))]
	interface ILMessageFilterCapabilitiesQueryResponse : NSSecureCoding {
		[BindAs (typeof (ILMessageFilterSubAction []))]
		[Export ("transactionalSubActions", ArgumentSemantic.Copy)]
		NSNumber [] TransactionalSubActions { get; set; }

		[BindAs (typeof (ILMessageFilterSubAction []))]
		[Export ("promotionalSubActions", ArgumentSemantic.Copy)]
		NSNumber [] PromotionalSubActions { get; set; }
	}

	[NoWatch, NoTV, NoMac, MacCatalyst (16, 0), iOS (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ILMessageFilterCapabilitiesQueryRequest : NSSecureCoding {
	}
}
