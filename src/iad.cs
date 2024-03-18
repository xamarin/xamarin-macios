//
// AdLib bindings.
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2010, Novell, Inc.
// Copyright 2011-2014 Xamarin Inc. All rights reserved.
//
#if !NET
using ObjCRuntime;
using Foundation;
using System;

namespace iAd {

	// Xcode 13 (beta1) removed most of the API
	// AppStore also started to reject apps using those API

	[Deprecated (PlatformName.iOS, 14, 5, message: "Use 'AAAttribution' instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ADClient {
		[Static]
		[Export ("sharedClient")]
		ADClient SharedClient { get; }

		[Deprecated (PlatformName.iOS, 13, 0)]
		[Export ("addClientToSegments:replaceExisting:")]
		void AddClientToSegments (string [] segmentIdentifiers, bool replaceExisting);

		[Export ("requestAttributionDetailsWithBlock:")]
		[Async]
		void RequestAttributionDetails (Action<NSDictionary, NSError> completionHandler);
	}
}
#endif // !NET
