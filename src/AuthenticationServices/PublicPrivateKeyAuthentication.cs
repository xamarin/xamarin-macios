//
// PublicPrivateKeyAuthentication.cs
//
// Authors:
//	TJ Lambert <antlambe@microsoft.com>
//
// Copyright 2021 Microsoft Corporation
//

#if !TVOS && !WATCH

using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace AuthenticationServices {

    [NoWatch, NoTV, Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
	public static class PublicPrivateKeyAuthentication {
        [DllImport (Constants.AuthenticationServicesLibrary)]
        static extern /* NSString[] */ IntPtr ASAuthorizationAllSupportedPublicKeyCredentialDescriptorTransports ();

        // TODO - looks like this type should be NSArrary[ASAuthorizationSecurityKeyPublicKeyCredentialDescriptorTransport]
        public static NSString[] GetAllSupportedPublicKeyCredentialDescriptorTransports () {
            return NSArray.ArrayFromHandle<NSString> (ASAuthorizationAllSupportedPublicKeyCredentialDescriptorTransports ());
        }
	}
}

#endif
