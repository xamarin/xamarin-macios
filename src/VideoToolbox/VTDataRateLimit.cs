//
// VideoToolbox core types
//
// Authors: 
// 		Miguel de Icaza (miguel@xamarin.com)
//		Alex Soto (alex.soto@xamarin.com)
//
// Copyright 2014 Xamarin Inc
//

#nullable enable

using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace VideoToolbox {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct VTDataRateLimit {
		public uint NumberOfBytes { get; set; }
		public double Seconds { get; set; }

		public VTDataRateLimit (uint numberOfBytes, double seconds) : this ()
		{
			NumberOfBytes = numberOfBytes;
			Seconds = seconds;
		}
	}
}
