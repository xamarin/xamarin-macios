using System;
using System.ComponentModel;
using CoreLocation;
using ObjCRuntime;
using Foundation;

namespace AppClip {

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[ErrorDomain ("APActivationPayloadErrorDomain")]
	[Native]
	public enum APActivationPayloadErrorCode : long {
		Disallowed = 1,
		DoesNotMatch = 2,
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface APActivationPayload : NSSecureCoding, NSCopying {

		[NullAllowed, Export ("URL", ArgumentSemantic.Strong)]
		NSUrl Url { get; }

		[Async]
		[Export ("confirmAcquiredInRegion:completionHandler:")]
		void ConfirmAcquired (CLRegion region, Action<bool, NSError> completionHandler);
	}
}
