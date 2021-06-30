//
// PublicPrivateKeyAuthentication.cs
//
// Authors:
//	TJ Lambert <antlambe@microsoft.com>
//
// Copyright 2021 Microsoft Corporation
//

#if !TVOS && !WATCHOS

using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace AuthenticationServices {

    // authenticationservices.cs contain the following attributes for this class
    // [NoWatch, NoTV, Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
	public static partial class PublicPrivateKeyAuthentication {
        [DllImport ("__Internal")]
        static extern /* NSString[] */ IntPtr AllSupportedPublicKeyCredentialDescriptorTransports ();

        public static NSString[] GetAllSupportedPublicKeyCredentialDescriptorTransports () {
            return NSArray.ArrayFromHandle<NSString> (AllSupportedPublicKeyCredentialDescriptorTransports ());
        }
	}
}

#endif
