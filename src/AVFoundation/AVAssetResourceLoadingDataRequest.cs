//
// AVAssetResourceLoadingDataRequest.cs
//
// Author:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012, 2014 Xamarin Inc
//

#if !WATCH

using System;
using Foundation;
using System.Runtime.Versioning;

namespace AVFoundation {
#if NET
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos10.9")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class AVAssetResourceLoadingDataRequest {
		public override string ToString ()
		{
			return "AVAssetResourceLoadingDataRequest";
		}
	}
}

#endif
