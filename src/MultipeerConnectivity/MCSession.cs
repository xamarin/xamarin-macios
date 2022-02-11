//
// MCSession Helpers
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014 Xamarin Inc.
//

using System;
using System.Collections.Generic;

using Foundation;
using Security;
using System.Runtime.Versioning;

namespace MultipeerConnectivity {

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class MCSession {

		public MCSession (MCPeerID myPeerID, SecIdentity identity, MCEncryptionPreference encryptionPreference)
			: base (NSObjectFlag.Empty)
		{
			if (identity == null) {
				Handle = Init (myPeerID, null, encryptionPreference);
			} else {
				using (var a = NSArray.FromNSObjects (identity))
					Handle = Init (myPeerID, a, encryptionPreference);
			}
		}

		public MCSession (MCPeerID myPeerID, SecIdentity identity, SecCertificate[] certificates, MCEncryptionPreference encryptionPreference)
			: base (NSObjectFlag.Empty)
		{
			if (identity == null) {
				if (certificates == null)
					Handle = Init (myPeerID, null, encryptionPreference);
				else
					throw new ArgumentNullException ("identity");
			} else {
				using (var certs = NSArray.FromNativeObjects (certificates))
					Handle = Init (myPeerID, certs, encryptionPreference);
			}
		}
	}
}
