using ObjCRuntime;
using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace CloudKit
{
#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class CKUserIdentityLookupInfo
	{
		// extra parameter to get a unique signature for a string argument
		CKUserIdentityLookupInfo (string id, int type)
		{
			var h = type == 0 ? _FromEmail (id) : _FromPhoneNumber (id);
			InitializeHandle (h);
		}

		public static CKUserIdentityLookupInfo FromEmail (string email)
		{
			if (string.IsNullOrEmpty (email))
				throw new ArgumentNullException (nameof (email));
			return new CKUserIdentityLookupInfo (email, 0);
		}

		public static CKUserIdentityLookupInfo FromPhoneNumber (string phoneNumber)
		{
			if (string.IsNullOrEmpty (phoneNumber))
				throw new ArgumentNullException (nameof (phoneNumber));
			return new CKUserIdentityLookupInfo (phoneNumber, 1);
		}
	}
}
