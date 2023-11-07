using ObjCRuntime;
using Foundation;
using System;

namespace AdServices {

	[Mac (11, 1), iOS (14, 3)]
	[MacCatalyst (14, 3)]
	[NoTV]
	[NoWatch]
	[Native]
	[ErrorDomain ("AAAttributionErrorDomain")]
	enum AAAttributionErrorCode : long {
		NetworkError = 1,
		InternalError = 2,
		PlatformNotSupported = 3,
	}

	[Mac (11, 1), iOS (14, 3)]
	[MacCatalyst (14, 3)]
	[NoTV]
	[NoWatch]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AAAttribution {

		[Static]
		[Export ("attributionTokenWithError:")]
		[return: NullAllowed]
		string GetAttributionToken ([NullAllowed] out NSError error);
	}
}
