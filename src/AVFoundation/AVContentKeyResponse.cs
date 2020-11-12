
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

namespace AVFoundation {

#if !WATCH
	[TV (10,2), Mac (10,12,4), iOS (10,3), NoWatch]
	public enum AVContentKeyResponseDataType {
		FairPlayStreamingKeyResponseData,
		[TV (13,0), NoWatch, Mac (10,15), iOS (13,0)]
		AuthorizationTokenData,
	} 

	public partial class AVContentKeyResponse {

		public static AVContentKeyResponse Create (NSData fairPlayStreamingKeyResponseData) => Create (fairPlayStreamingKeyResponseData, AVContentKeyResponseDataType.FairPlayStreamingKeyResponseData);
			
		[NoWatch]
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