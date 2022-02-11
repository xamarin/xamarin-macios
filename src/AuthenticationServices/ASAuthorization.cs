//
// ASAuthorization.cs
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright 2019 Microsoft Corporation
//

#nullable enable

using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace AuthenticationServices {

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class ASAuthorization {
		public T? GetProvider<T> () where T : NSObject, IASAuthorizationProvider => Runtime.GetINativeObject<T> (_Provider, false);

		public T? GetCredential<T> () where T : NSObject, IASAuthorizationCredential => Runtime.GetINativeObject<T> (_Credential, false);
	}
}
