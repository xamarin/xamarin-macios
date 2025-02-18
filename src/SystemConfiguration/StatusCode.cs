//
// StatusCode.cs: SystemConfiguration error handling
//
// Authors:
//    Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012, 2016 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace SystemConfiguration {

	// untyped enum -> SystemConfiguration.framework/Headers/SystemConfiguration.h
	/// <summary>An enumeration whose values specify various statuses relating to network reachability.</summary>
	[ErrorDomain ("kCFErrorDomainSystemConfiguration")]
	public enum StatusCode {
		/// <summary>The call succeeded.</summary>
		OK = 0,
		/// <summary>General error.</summary>
		Failed = 1001,
		/// <summary>A bad argument was passed.</summary>
		InvalidArgument = 1002,
		/// <summary>Permission was denied.</summary>
		AccessError = 1003,
		/// <summary>The key does not exist.</summary>
		NoKey = 1004,
		/// <summary>The key is already defined.</summary>
		KeyExists = 1005,
		/// <summary>A lock is already defined.</summary>
		Locked = 1006,
		/// <summary>The operation requires a lock.</summary>
		NeedLock = 1007,
		/// <summary>The configuration daemon session is not active.</summary>
		NoStoreSession = 2001,
		/// <summary>The configuration daemon is not available.</summary>
		NoStoreServer = 2002,
		/// <summary>Notifier is active.</summary>
		NotifierActive = 2003,
		/// <summary>The preferences session is not active.</summary>
		NoPrefsSession = 3001,
		/// <summary>A preferences update is currently active.</summary>
		PrefsBusy = 3002,
		/// <summary>The config file cannot be found.</summary>
		NoConfigFile = 3003,
		/// <summary>The link does not exist.</summary>
		NoLink = 3004,
		/// <summary>A write was attempted on a stale version of the object.</summary>
		Stale = 3005,
		/// <summary>The max link count has been exceeded.</summary>
		MaxLink = 3006,
		/// <summary>The reachability of the target cannot be determined.</summary>
		ReachabilityUnknown = 4001,
		/// <summary>Network service for connection is unavailable.</summary>
		ConnectionNoService = 5001,
		/// <summary>To be added.</summary>
		ConnectionIgnore = 5002
	}
}
