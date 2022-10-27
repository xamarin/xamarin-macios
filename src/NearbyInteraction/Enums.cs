//
// NearbyInteraction enums
//
// Authors:
//	Whitney Schmidt  <whschm@microsoft.com>
//
// Copyright 2020 Microsoft Inc.
//

using ObjCRuntime;
using Foundation;
using System;

namespace NearbyInteraction {

	[Watch (8,0), NoTV, NoMac, iOS (14,0)]
	[ErrorDomain ("NIErrorDomain")]
	[Native]
	public enum NIErrorCode : long
	{
		UnsupportedPlatform = -5889,
		InvalidConfiguration = -5888,
		SessionFailed = -5887,
		ResourceUsageTimeout = -5886,
		ActiveSessionsLimitExceeded = -5885,
		UserDidNotAllow = -5884,
	}

	[Watch (8,0), NoTV, NoMac, iOS (14,0)]
	[Native]
	public enum NINearbyObjectRemovalReason : long
	{
		Timeout,
		PeerEnded,
	}
}
