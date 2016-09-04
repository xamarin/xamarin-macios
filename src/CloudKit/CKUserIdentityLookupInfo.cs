using XamCore.ObjCRuntime;
using XamCore.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;

namespace XamCore.CloudKit
{
#if XAMCORE_2_0

	public partial class CKUserIdentityLookupInfo
	{
		// extra, unused, parameter to get a unique signature
		CKUserIdentityLookupInfo (IntPtr handle, int unused)
		{
			InitializeHandle (handle);
		}

		public static CKUserIdentityLookupInfo FromEmail (string email)
		{
			if (string.IsNullOrEmpty (email))
				throw new ArgumentNullException (nameof (email));
			return new CKUserIdentityLookupInfo (_FromEmail (email), 0);
		}

		public static CKUserIdentityLookupInfo FromPhoneNumber (string phoneNumber)
		{
			if (string.IsNullOrEmpty (phoneNumber))
				throw new ArgumentNullException (nameof (phoneNumber));
			return new CKUserIdentityLookupInfo (_FromPhoneNumber (phoneNumber), 0);
		}
	}
#endif
}
