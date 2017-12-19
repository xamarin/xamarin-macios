//
// systemconfiguration.cs: Definitions for SystemConfiguration
//
// Authors: 
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.SystemConfiguration {

#if !MONOMAC
	[Static]
	interface CaptiveNetwork {

		[Field ("kCNNetworkInfoKeyBSSID")]
		NSString NetworkInfoKeyBSSID { get; }

		[Field ("kCNNetworkInfoKeySSID")]
		NSString NetworkInfoKeySSID { get; }

		[Field ("kCNNetworkInfoKeySSIDData")]
		NSString NetworkInfoKeySSIDData { get; }
	}
#endif
}