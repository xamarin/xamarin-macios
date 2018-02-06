//
// systemconfiguration.cs: Definitions for SystemConfiguration
//
// Authors: 
//  Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc.
//

using System;
using Foundation;
using ObjCRuntime;

namespace SystemConfiguration {

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