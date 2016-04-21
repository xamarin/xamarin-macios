//
// CNPostalAddress.cs: Implements some nicer methods for CNPostalAddress
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.Foundation;

namespace XamCore.Contacts {
#if XAMCORE_2_0 // The Contacts framework uses generics heavily, which is only supported in Unified (for now at least)
	// Strong typed Keys to enum
	public enum CNPostalAddressKeyOption {
		Street,
		City,
		State,
		PostalCode,
		Country,
		IsoCountryCode
	}

	public partial class CNPostalAddress {
		
		public static string LocalizeProperty (CNPostalAddressKeyOption option)
		{
			var srvc = LocalizeOptionsToNSString (option);
			return LocalizeProperty (srvc);
		}

		static NSString LocalizeOptionsToNSString (CNPostalAddressKeyOption option)
		{
			switch (option) {
			case CNPostalAddressKeyOption.Street:
				return CNPostalAddressKey.Street;
			case CNPostalAddressKeyOption.City:
				return CNPostalAddressKey.City;
			case CNPostalAddressKeyOption.State:
				return CNPostalAddressKey.State;
			case CNPostalAddressKeyOption.PostalCode:
				return CNPostalAddressKey.PostalCode;
			case CNPostalAddressKeyOption.Country:
				return CNPostalAddressKey.Country;
			case CNPostalAddressKeyOption.IsoCountryCode:
				return CNPostalAddressKey.IsoCountryCode;
			default:
				throw new ArgumentOutOfRangeException ("option");
			}
		}
	}
#endif // XAMCORE_2_0
}

