//
// CoreSpotlight enums
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015, 2016 Xamarin Inc. All rights reserved.
//

#if IOS

using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.CoreSpotlight {
#if !MONOMAC // TODO: Verify this is available in future OS X El Capitan betas, it was not included in beta 1, also do not forget foundation.cs(3801,3)
	// NSInteger -> CNContact.h
	[NoTV] // CS_TVOS_UNAVAILABLE
	[iOS (9,0), Mac (10,11)]
	[Native]
	[ErrorDomain ("CSIndexErrorDomain")]
	public enum CSIndexErrorCode : nint {
		UnknownError =	-1,
		IndexUnavailableError = -1000,
		InvalidItemError = -1001,
		InvalidClientStateError = -1002,
		RemoteConnectionError = -1003,
		QuotaExceeded = -1004,
		IndexingUnsupported = -1005,
	}
#endif
}

#endif
