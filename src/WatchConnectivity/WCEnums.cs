//
// WatchConnectivity enums
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using ObjCRuntime;
using Foundation;

namespace WatchConnectivity {

	// NSInteger -> WCError.h
	[iOS (9,0)]
	[Native]
	public enum WCErrorCode : long {
		GenericError = 7001,
		SessionNotSupported = 7002,
		SessionMissingDelegate = 7003,
		SessionNotActivated = 7004,
		DeviceNotPaired = 7005,
		WatchAppNotInstalled = 7006,
		NotReachable = 7007,
		InvalidParameter = 7008,
		PayloadTooLarge = 7009,
		PayloadUnsupportedTypes = 7010,
		MessageReplyFailed = 7011,
		MessageReplyTimedOut = 7012,
		FileAccessDenied = 7013,
		DeliveryFailed = 7014,
		InsufficientSpace = 7015,
		// iOS 9.3 / watchOS 2.2
		SessionInactive = 7016,
		TransferTimedOut = 7017,
	}

	[Watch (2,2)][iOS (9,3)]
	[Native]
	public enum WCSessionActivationState : long {
		NotActivated = 0,
		Inactive = 1,
		Activated = 2
	}
}
