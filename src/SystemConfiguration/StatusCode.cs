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
	[ErrorDomain ("kCFErrorDomainSystemConfiguration")]
	public enum StatusCode
	{
   		OK = 0,
   		Failed = 1001,
   		InvalidArgument = 1002,
   		AccessError = 1003,
   		NoKey = 1004,
   		KeyExists = 1005,
   		Locked = 1006,
   		NeedLock = 1007,
   		NoStoreSession = 2001,
   		NoStoreServer = 2002,
   		NotifierActive = 2003,
   		NoPrefsSession = 3001,
   		PrefsBusy = 3002,
   		NoConfigFile = 3003,
   		NoLink = 3004,
   		Stale = 3005,
   		MaxLink = 3006,
   		ReachabilityUnknown = 4001,
   		ConnectionNoService = 5001,
   		ConnectionIgnore = 5002
	}
}
