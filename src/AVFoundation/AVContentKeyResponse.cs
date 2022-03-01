
//
// Copyright 2019 Microsoft Corp
//
// Authors:
//   Manuel de la Pena mandel@microsoft.com 
//
using Foundation;
using ObjCRuntime;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace AVFoundation {

#if !WATCH
#if NET
	[SupportedOSPlatform ("tvos10.2")]
	[SupportedOSPlatform ("macos10.12.4")]
	[SupportedOSPlatform ("ios10.3")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class AVContentKeyResponse {

		public static AVContentKeyResponse Create (NSData fairPlayStreamingKeyResponseData) => Create (fairPlayStreamingKeyResponseData, AVContentKeyResponseDataType.FairPlayStreamingKeyResponseData);
			
#if !NET
		[NoWatch]
#endif
		public static AVContentKeyResponse Create (NSData data, AVContentKeyResponseDataType dataType = AVContentKeyResponseDataType.FairPlayStreamingKeyResponseData) {
			switch (dataType) {
			case AVContentKeyResponseDataType.AuthorizationTokenData:
				return AVContentKeyResponse._InitWithAuthorizationToken (data);
			default:
				return AVContentKeyResponse._InitWithFairPlayStreamingKeyResponseData (data);
			}
		}
	}
#endif
}
