#nullable enable

using ObjCRuntime;
using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CloudKit {
	public partial class CKUserIdentityLookupInfo {
		// extra parameter to get a unique signature for a string argument
		CKUserIdentityLookupInfo (string id, int type)
		{
			var h = type == 0 ? _FromEmail (id) : _FromPhoneNumber (id);
			InitializeHandle (h);
		}

		public static CKUserIdentityLookupInfo FromEmail (string email)
		{
			if (string.IsNullOrEmpty (email))
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (email));
			return new CKUserIdentityLookupInfo (email, 0);
		}

		public static CKUserIdentityLookupInfo FromPhoneNumber (string phoneNumber)
		{
			if (string.IsNullOrEmpty (phoneNumber))
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (phoneNumber));
			return new CKUserIdentityLookupInfo (phoneNumber, 1);
		}
	}
}
