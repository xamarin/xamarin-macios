//
// AppTrackingTransparency.cs
//
// Authors:
//   Dorothy Tam (dottam@gmail.com)
//

using System;
using Foundation;
using ObjCRuntime;

namespace AppTrackingTransparency {

	[iOS (14, 0), TV (14, 0), Mac (11, 0), NoWatch]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Native]
	public enum ATTrackingManagerAuthorizationStatus : ulong {
		NotDetermined = 0,
		Restricted = 1,
		Denied = 2,
		Authorized = 3,
	}

	[iOS (14, 0), TV (14, 0), Mac (11, 0), NoWatch]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ATTrackingManager {
		[Static, Export ("requestTrackingAuthorizationWithCompletionHandler:")]
		[Async]
		void RequestTrackingAuthorization (Action<ATTrackingManagerAuthorizationStatus> completion);

		[Static, Export ("trackingAuthorizationStatus", ArgumentSemantic.Assign)]
		ATTrackingManagerAuthorizationStatus TrackingAuthorizationStatus { get; }
	}
}
