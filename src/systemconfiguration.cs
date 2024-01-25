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

	[Static]
	interface CaptiveNetwork {

		[NoTV]
		[NoWatch]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kCNNetworkInfoKeyBSSID")]
		NSString NetworkInfoKeyBSSID { get; }

		[NoTV]
		[NoWatch]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kCNNetworkInfoKeySSID")]
		NSString NetworkInfoKeySSID { get; }

		[NoTV]
		[NoWatch]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kCNNetworkInfoKeySSIDData")]
		NSString NetworkInfoKeySSIDData { get; }
	}
}
