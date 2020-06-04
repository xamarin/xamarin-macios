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

namespace IdentityLookup {

	[iOS (11,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[Native]
	public enum ILMessageFilterAction : long {
		None = 0,
		Allow = 1,
		Filter = 2,
	}

	[iOS (11,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[ErrorDomain ("ILMessageFilterErrorDomain")]
	[Native]
	public enum ILMessageFilterError : long {
		System = 1,
		InvalidNetworkUrl = 2,
		NetworkUrlUnauthorized = 3,
		NetworkRequestFailed = 4,
		RedundantNetworkDeferral = 5,
	}

	[iOS (12, 0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[Native]
	enum ILClassificationAction : long {
		None = 0,
		ReportNotJunk = 1,
		ReportJunk = 2,
		ReportJunkAndBlockSender = 3,
	}

	[iOS (11,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface ILMessageFilterExtension {
	}

	[iOS (11,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[DisableDefaultCtor]
	[BaseType (typeof (NSExtensionContext))]
	interface ILMessageFilterExtensionContext {

		[Export ("deferQueryRequestToNetworkWithCompletion:")]
		[Async]
		void DeferQueryRequestToNetwork (Action<ILNetworkResponse, NSError> completion);
	}

	interface IILMessageFilterQueryHandling { }

	[iOS (11,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[Protocol]
	interface ILMessageFilterQueryHandling {

		[Abstract]
		[Export ("handleQueryRequest:context:completion:")]
		void HandleQueryRequest (ILMessageFilterQueryRequest queryRequest, ILMessageFilterExtensionContext context, Action<ILMessageFilterQueryResponse> completion);
	}

	[iOS (11,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ILMessageFilterQueryRequest : NSSecureCoding {

		[NullAllowed, Export ("sender")]
		string Sender { get; }

		[NullAllowed, Export ("messageBody")]
		string MessageBody { get; }
	}

	[iOS (11,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface ILMessageFilterQueryResponse : NSSecureCoding {

		[Export ("action", ArgumentSemantic.Assign)]
		ILMessageFilterAction Action { get; set; }
	}

	[iOS (11,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ILNetworkResponse : NSSecureCoding {

		[Export ("urlResponse")]
		NSHttpUrlResponse UrlResponse { get; }

		[Export ("data")]
		NSData Data { get; }
	}

	[iOS (12,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[BaseType (typeof (ILClassificationRequest))]
	[DisableDefaultCtor]
	interface ILCallClassificationRequest : NSSecureCoding {

		[Export ("callCommunications", ArgumentSemantic.Copy)]
		ILCallCommunication [] CallCommunications { get; }
	}

	[iOS (12,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[BaseType (typeof (ILCommunication))]
	[DisableDefaultCtor]
	interface ILCallCommunication {

		[Export ("isEqualToCallCommunication:")]
		bool IsEqualTo (ILCallCommunication communication);
	}

	[Abstract]
	[iOS (12,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ILClassificationRequest : NSSecureCoding {

	}

	[iOS (12,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ILClassificationResponse : NSSecureCoding {

		[Export ("action", ArgumentSemantic.Assign)]
		ILClassificationAction Action { get; }

		[iOS (12,1)]
		[NullAllowed, Export ("userString")]
		string UserString { get; set; }

		// Objects can be NSString, NSNumber, NSArray, NSDictionary, or NSNull
		[NullAllowed, Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> UserInfo { get; set; }

		[Export ("initWithClassificationAction:")]
		[DesignatedInitializer]
		IntPtr Constructor (ILClassificationAction action);
	}

	[Abstract]
	[iOS (12,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
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

	[iOS (12,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[BaseType (typeof (ILClassificationRequest))]
	[DisableDefaultCtor]
	interface ILMessageClassificationRequest : NSSecureCoding {

		[Export ("messageCommunications", ArgumentSemantic.Copy)]
		ILMessageCommunication [] MessageCommunications { get; }
	}

	[iOS (12,0)]
	[Introduced (PlatformName.UIKitForMac, 13,0)]
	[NoMac][NoWatch][NoTV]
	[BaseType (typeof (ILCommunication))]
	[DisableDefaultCtor]
	interface ILMessageCommunication {

		[NullAllowed, Export ("messageBody")]
		string MessageBody { get; }

		[Export ("isEqualToMessageCommunication:")]
		bool IsEqualTo (ILMessageCommunication communication);
	}
}
