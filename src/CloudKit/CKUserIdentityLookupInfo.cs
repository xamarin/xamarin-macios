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
		enum CKUserIdentityLookupInfoParamType {
			EmailAddress,
			PhoneNumber
		};

		CKUserIdentityLookupInfo (string data, CKUserIdentityLookupInfoParamType type)
		{
			switch (type) {
			case CKUserIdentityLookupInfoParamType.EmailAddress:
				Handle = _FromEmail (data);
				break;
			case CKUserIdentityLookupInfoParamType.PhoneNumber:
				Handle = _FromPhoneNumber (data);
				break;
			default:
				throw new ArgumentException ("type");
			}
		}

		public static CKUserIdentityLookupInfo FromEmail (string email)
		{
			if (string.IsNullOrEmpty (email))
				throw new ArgumentNullException (nameof (email));
			return new CKUserIdentityLookupInfo (email, CKUserIdentityLookupInfoParamType.EmailAddress);
		}

		public static CKUserIdentityLookupInfo FromPhoneNumber (string phoneNumber)
		{
			if (string.IsNullOrEmpty (phoneNumber))
				throw new ArgumentNullException (nameof (phoneNumber));
			return new CKUserIdentityLookupInfo (phoneNumber, CKUserIdentityLookupInfoParamType.PhoneNumber);
		}
	}
#endif
}
