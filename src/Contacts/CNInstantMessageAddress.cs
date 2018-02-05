//
// CNInstantMessageAddress.cs: Implements some nicer methods for CNInstantMessageAddress
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;

namespace Contacts {
#if XAMCORE_2_0 // The Contacts framework uses generics heavily, which is only supported in Unified (for now at least)
	// Strong typed Keys to enum
	public enum CNInstantMessageAddressOption {
		Username,
		Service
	}

	// Strong typed Keys to enum
	public enum CNInstantMessageServiceOption {
		Aim,
		Facebook,
		GaduGadu,
		GoogleTalk,
		Icq,
		Jabber,
		Msn,
		QQ,
		Skype,
		Yahoo
	}

	public partial class CNInstantMessageAddress {

		public static string LocalizeProperty (CNInstantMessageAddressOption property)
		{
			switch (property) {
			case CNInstantMessageAddressOption.Username:
				return LocalizeProperty (CNInstantMessageAddressKey.Username);
			case CNInstantMessageAddressOption.Service:
				return LocalizeProperty (CNInstantMessageAddressKey.Service);
			default:
				throw new ArgumentOutOfRangeException ("serviceOption");
			}
		}

		public static string LocalizeService (CNInstantMessageServiceOption serviceOption)
		{
			var srvc = ServiceOptionsToNSString (serviceOption);
			return LocalizeService (srvc);
		}

		static NSString ServiceOptionsToNSString (CNInstantMessageServiceOption serviceOption)
		{
			switch (serviceOption) {
			case CNInstantMessageServiceOption.Aim:
				return CNInstantMessageServiceKey.Aim;
			case CNInstantMessageServiceOption.Facebook:
				return CNInstantMessageServiceKey.Facebook;
			case CNInstantMessageServiceOption.GaduGadu:
				return CNInstantMessageServiceKey.GaduGadu;
			case CNInstantMessageServiceOption.GoogleTalk:
				return CNInstantMessageServiceKey.GoogleTalk;
			case CNInstantMessageServiceOption.Icq:
				return CNInstantMessageServiceKey.Icq;
			case CNInstantMessageServiceOption.Jabber:
				return CNInstantMessageServiceKey.Jabber;
			case CNInstantMessageServiceOption.Msn:
				return CNInstantMessageServiceKey.Msn;
			case CNInstantMessageServiceOption.QQ:
				return CNInstantMessageServiceKey.QQ;
			case CNInstantMessageServiceOption.Skype:
				return CNInstantMessageServiceKey.Skype;
			case CNInstantMessageServiceOption.Yahoo:
				return CNInstantMessageServiceKey.Yahoo;
			default:
				throw new ArgumentOutOfRangeException ("serviceOption");
			}
		}
	}
#endif // XAMCORE_2_0
}

