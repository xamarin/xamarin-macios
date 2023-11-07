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

	[Watch (8, 0), NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[ErrorDomain ("NIErrorDomain")]
	[Native]
	public enum NIErrorCode : long {
		UnsupportedPlatform = -5889,
		InvalidConfiguration = -5888,
		SessionFailed = -5887,
		ResourceUsageTimeout = -5886,
		ActiveSessionsLimitExceeded = -5885,
		UserDidNotAllow = -5884,
		AccessoryPeerDeviceUnavailable = -5882,
		InvalidARConfiguration = -5883,
		IncompatiblePeerDevice = -5881,
		ActiveExtendedDistanceSessionsLimitExceeded = -5880,
	}

	[Watch (8, 0), NoTV, NoMac, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum NINearbyObjectRemovalReason : long {
		Timeout,
		PeerEnded,
	}

	[iOS (16, 0), NoMac, Watch (9, 0), NoTV, MacCatalyst (16, 0)]
	[Native]
	public enum NIAlgorithmConvergenceStatus : long {
		Unknown,
		NotConverged,
		Converged,
	}

	[iOS (16, 0), NoMac, Watch (9, 0), NoTV, MacCatalyst (16, 0)]
	[Native]
	public enum NINearbyObjectVerticalDirectionEstimate : long {
		Unknown = 0,
		Same = 1,
		Above = 2,
		Below = 3,
		AboveOrBelow = 4,
	}
}
