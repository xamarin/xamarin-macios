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

namespace AuthenticationServices {

	public partial class ASAuthorization {
		public T? GetProvider<T> () where T : NSObject, IASAuthorizationProvider => Runtime.GetINativeObject<T> (_Provider, false);

		public T? GetCredential<T> () where T : NSObject, IASAuthorizationCredential => Runtime.GetINativeObject<T> (_Credential, false);
	}
}
