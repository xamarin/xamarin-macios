//
// MCSession Helpers
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc.
//

#nullable enable

using System;
using System.Collections.Generic;

using Foundation;

using Security;

namespace MultipeerConnectivity {

	public partial class MCSession {

		public MCSession (MCPeerID myPeerID, SecIdentity identity, MCEncryptionPreference encryptionPreference)
			: base (NSObjectFlag.Empty)
		{
			if (identity is null) {
				Handle = Init (myPeerID, null, encryptionPreference);
			} else {
				using (var a = NSArray.FromNSObjects (identity))
					Handle = Init (myPeerID, a, encryptionPreference);
			}
		}

		public MCSession (MCPeerID myPeerID, SecIdentity identity, SecCertificate [] certificates, MCEncryptionPreference encryptionPreference)
			: base (NSObjectFlag.Empty)
		{
			if (identity is null) {
				if (certificates is null)
					Handle = Init (myPeerID, null, encryptionPreference);
				else
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (identity));
			} else {
				using (var certs = NSArray.FromNativeObjects (certificates))
					Handle = Init (myPeerID, certs, encryptionPreference);
			}
		}
	}
}
