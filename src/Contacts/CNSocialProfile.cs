//
// CNSocialProfile.cs: Implements some nicer methods for CNSocialProfile
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
	public enum CNSocialProfileOption
	{
		UrlString,
		Username,
		UserIdentifier,
		Service
	}

	// Strong typed Keys to enum
	public enum CNSocialProfileServiceOption
	{
		Facebook,
		Flickr,
		LinkedIn,
		MySpace,
		SinaWeibo,
		TencentWeibo,
		Twitter,
		Yelp,
		GameCenter
	}

	public partial class CNSocialProfile {

		public static string LocalizeProperty (CNSocialProfileOption option)
		{
			switch (option) {
			case CNSocialProfileOption.UrlString:
				return LocalizeProperty (CNSocialProfileKey.UrlString);
			case CNSocialProfileOption.Username:
				return LocalizeProperty (CNSocialProfileKey.Username);
			case CNSocialProfileOption.UserIdentifier:
				return LocalizeProperty (CNSocialProfileKey.UserIdentifier);
			case CNSocialProfileOption.Service:
				return LocalizeProperty (CNSocialProfileKey.Service);
			default:
				throw new ArgumentOutOfRangeException ("option");
			}
		}

		public static string LocalizeService (CNSocialProfileServiceOption serviceOption)
		{
			var srvc = ServiceOptionsToNSString (serviceOption);
			return LocalizeService (srvc);
		}

		static NSString ServiceOptionsToNSString (CNSocialProfileServiceOption serviceOption)
		{
			switch (serviceOption) {
			case CNSocialProfileServiceOption.Facebook:
				return CNSocialProfileServiceKey.Facebook;
			case CNSocialProfileServiceOption.Flickr:
				return CNSocialProfileServiceKey.Flickr;
			case CNSocialProfileServiceOption.LinkedIn:
				return CNSocialProfileServiceKey.LinkedIn;
			case CNSocialProfileServiceOption.MySpace:
				return CNSocialProfileServiceKey.MySpace;
			case CNSocialProfileServiceOption.SinaWeibo:
				return CNSocialProfileServiceKey.SinaWeibo;
			case CNSocialProfileServiceOption.TencentWeibo:
				return CNSocialProfileServiceKey.TencentWeibo;
			case CNSocialProfileServiceOption.Twitter:
				return CNSocialProfileServiceKey.Twitter;
			case CNSocialProfileServiceOption.Yelp:
				return CNSocialProfileServiceKey.Yelp;
			case CNSocialProfileServiceOption.GameCenter:
				return CNSocialProfileServiceKey.GameCenter;
			default:
				throw new ArgumentOutOfRangeException ("serviceOption");
			}
		}
	}
#endif // XAMCORE_2_0
}

