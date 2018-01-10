//
// CNContact.cs: Implements some nicer methods for CNContact
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;

namespace Contacts {
#if XAMCORE_2_0 // The Contacts framework uses generics heavily, which is only supported in Unified (for now at least)
	[iOS (9,0)][Mac (10,11)]
	[Flags]
	public enum CNContactOptions : long {
		None						= 0,
		Nickname					= 1 << 0,
		PhoneticGivenName			= 1 << 1,
		PhoneticMiddleName			= 1 << 2,
		PhoneticFamilyName			= 1 << 3,
		OrganizationName			= 1 << 4,
		DepartmentName				= 1 << 5,
		JobTitle					= 1 << 6,
		Birthday					= 1 << 7,
		NonGregorianBirthday		= 1 << 8,
		Note						= 1 << 9,
#if !MONOMAC
		ImageData					= 1 << 10,
#endif
		ThumbnailImageData			= 1 << 11,
#if !MONOMAC
		ImageDataAvailable			= 1 << 12,
#endif
		Type						= 1 << 13,
		PhoneNumbers				= 1 << 14,
		EmailAddresses				= 1 << 15,
		PostalAddresses				= 1 << 16,
		Dates						= 1 << 17,
		UrlAddresses				= 1 << 18,
		Relations					= 1 << 19,
		SocialProfiles				= 1 << 20,
		InstantMessageAddresses		= 1 << 21,
	}

	public partial class CNContact {
		
		public virtual bool IsKeyAvailable (CNContactOptions options)
		{
			var key = ContactOptionsToNSString (options);
			return IsKeyAvailable (key);
		}

		public static string LocalizeProperty (CNContactOptions options)
		{
			var key = ContactOptionsToNSString (options);
			return LocalizeProperty (key);
		}

		static NSString ContactOptionsToNSString (CNContactOptions options)
		{
			switch (options) {
			case CNContactOptions.Nickname:
				return CNContactKey.Nickname;
			case CNContactOptions.PhoneticGivenName:
				return CNContactKey.PhoneticGivenName;
			case CNContactOptions.PhoneticMiddleName:
				return CNContactKey.PhoneticMiddleName;
			case CNContactOptions.PhoneticFamilyName:
				return CNContactKey.PhoneticFamilyName;
			case CNContactOptions.OrganizationName:
				return CNContactKey.OrganizationName;
			case CNContactOptions.DepartmentName:
				return CNContactKey.DepartmentName;
			case CNContactOptions.JobTitle:
				return CNContactKey.JobTitle;
			case CNContactOptions.Birthday:
				return CNContactKey.Birthday;
			case CNContactOptions.NonGregorianBirthday:
				return CNContactKey.NonGregorianBirthday;
			case CNContactOptions.Note:
				return CNContactKey.Note;
#if !MONOMAC
			case CNContactOptions.ImageData:
				return CNContactKey.ImageData;
			case CNContactOptions.ImageDataAvailable:
				return CNContactKey.ImageDataAvailable;
#endif
			case CNContactOptions.ThumbnailImageData:
				return CNContactKey.ThumbnailImageData;
			case CNContactOptions.Type:
				return CNContactKey.Type;
			case CNContactOptions.PhoneNumbers:
				return CNContactKey.PhoneNumbers;
			case CNContactOptions.EmailAddresses:
				return CNContactKey.EmailAddresses;
			case CNContactOptions.PostalAddresses:
				return CNContactKey.PostalAddresses;
			case CNContactOptions.Dates:
				return CNContactKey.Dates;
			case CNContactOptions.UrlAddresses:
				return CNContactKey.UrlAddresses;
			case CNContactOptions.Relations:
				return CNContactKey.Relations;
			case CNContactOptions.SocialProfiles:
				return CNContactKey.SocialProfiles;
			case CNContactOptions.InstantMessageAddresses:
				return CNContactKey.InstantMessageAddresses;
			default:
				throw new ArgumentOutOfRangeException ("contactOption");
			}
		}

		public bool AreKeysAvailable<T> (T [] keyDescriptors)
			where T : INSObjectProtocol, INSSecureCoding, INSCopying
		{
			using (var array = NSArray.From<T> (keyDescriptors))
				return AreKeysAvailable (array);
		}

		public bool AreKeysAvailable (CNContactOptions options)
		{
			using (var array = new NSMutableArray ()) {
				foreach (CNContactOptions value in Enum.GetValues (typeof (CNContactOptions))) {
					if ((options & value) != CNContactOptions.None)
						array.Add (ContactOptionsToNSString (value));
				}
				return AreKeysAvailable (array);
			}
		}
	}
#endif // XAMCORE_2_0
}

