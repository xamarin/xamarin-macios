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

	/// <summary>Deprecated: Use <see cref="T:NetworkExtension.NEHotspotHelper" /> instead. A class that encapsulates the Captive Network system component, which is responsible for detecting networks that require user interaction prior to providing Internet access.</summary>
	[Static]
	interface CaptiveNetwork {

		[NoTV]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kCNNetworkInfoKeyBSSID")]
		NSString NetworkInfoKeyBSSID { get; }

		[NoTV]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kCNNetworkInfoKeySSID")]
		NSString NetworkInfoKeySSID { get; }

		[NoTV]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Field ("kCNNetworkInfoKeySSIDData")]
		NSString NetworkInfoKeySSIDData { get; }
	}
}
