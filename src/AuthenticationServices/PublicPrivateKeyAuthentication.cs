//
// PublicPrivateKeyAuthentication.cs
//
// Authors:
//	TJ Lambert <antlambe@microsoft.com>
//
// Copyright 2021 Microsoft Corporation
//

#if !TVOS && !WATCHOS

using Foundation;
using ObjCRuntime;

#nullable enable

namespace AuthenticationServices {

	public static partial class PublicPrivateKeyAuthentication {
		[NoWatch, NoTV, Mac (12,0), iOS (15,0), MacCatalyst (15,0)]
        [DllImport ("__Internal")]
        static extern /* NSString[] */ IntPtr AllSupportedPublicKeyCredentialDescriptorTransports ();

        public static NSString[] GetAllSupportedPublicKeyCredentialDescriptorTransports () {
            return NSArray.ArrayFromHandle<NSString> (AllSupportedPublicKeyCredentialDescriptorTransports ());
        }
	}
}

#endif
