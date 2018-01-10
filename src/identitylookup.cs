//
// IdentityLookup C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0
using System;
using Foundation;
using ObjCRuntime;

namespace IdentityLookup {

	[iOS (11,0)]
	[Native]
	public enum ILMessageFilterAction : long {
		None = 0,
		Allow = 1,
		Filter = 2,
	}

	[iOS (11,0)]
	[ErrorDomain ("ILMessageFilterErrorDomain")]
	[Native]
	public enum ILMessageFilterError : long {
		System = 1,
		InvalidNetworkUrl = 2,
		NetworkUrlUnauthorized = 3,
		NetworkRequestFailed = 4,
		RedundantNetworkDeferral = 5,
	}

	[iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface ILMessageFilterExtension {
	}

	[iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSExtensionContext))]
	interface ILMessageFilterExtensionContext {

		[Export ("deferQueryRequestToNetworkWithCompletion:")]
		[Async]
		void DeferQueryRequestToNetwork (Action<ILNetworkResponse, NSError> completion);
	}

	interface IILMessageFilterQueryHandling { }

	[iOS (11,0)]
	[Protocol]
	interface ILMessageFilterQueryHandling {

		[Abstract]
		[Export ("handleQueryRequest:context:completion:")]
		void HandleQueryRequest (ILMessageFilterQueryRequest queryRequest, ILMessageFilterExtensionContext context, Action<ILMessageFilterQueryResponse> completion);
	}

	[iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ILMessageFilterQueryRequest : NSSecureCoding {

		[NullAllowed, Export ("sender")]
		string Sender { get; }

		[NullAllowed, Export ("messageBody")]
		string MessageBody { get; }
	}

	[iOS (11,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface ILMessageFilterQueryResponse : NSSecureCoding {

		[Export ("action", ArgumentSemantic.Assign)]
		ILMessageFilterAction Action { get; set; }
	}

	[iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ILNetworkResponse : NSSecureCoding {

		[Export ("urlResponse")]
		NSHttpUrlResponse UrlResponse { get; }

		[Export ("data")]
		NSData Data { get; }
	}
}
#endif
