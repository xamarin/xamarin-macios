// 
// ABPerson.cs: Implements the managed ABPerson
//
// Authors: Mono Team
//          Marek Safar (marek.safar@gmail.com)
//     
// Copyright (C) 2009 Novell, Inc
// Copyright (C) 2012-2013 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//

#nullable enable

#if !MONOMAC

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AddressBook {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst14.0")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
#endif
	static class ABPersonPropertyId {

		public static int Address { get; private set; }
		public static int Birthday { get; private set; }
		public static int CreationDate { get; private set; }
		public static int Date { get; private set; }
		public static int Department { get; private set; }
		public static int Email { get; private set; }
		public static int FirstName { get; private set; }
		public static int FirstNamePhonetic { get; private set; }
		public static int InstantMessage { get; private set; }
		public static int JobTitle { get; private set; }
		public static int Kind { get; private set; }
		public static int LastName { get; private set; }
		public static int LastNamePhonetic { get; private set; }
		public static int MiddleName { get; private set; }
		public static int MiddleNamePhonetic { get; private set; }
		public static int ModificationDate { get; private set; }
		public static int Nickname { get; private set; }
		public static int Note { get; private set; }
		public static int Organization { get; private set; }
		public static int Phone { get; private set; }
		public static int Prefix { get; private set; }
		public static int RelatedNames { get; private set; }
		public static int Suffix { get; private set; }
		public static int Url { get; private set; }
		public static int SocialProfile { get; private set; }

		static ABPersonPropertyId ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			var handle = Libraries.AddressBook.Handle;
			Address = Dlfcn.GetInt32 (handle, "kABPersonAddressProperty");
			Birthday = Dlfcn.GetInt32 (handle, "kABPersonBirthdayProperty");
			CreationDate = Dlfcn.GetInt32 (handle, "kABPersonCreationDateProperty");
			Date = Dlfcn.GetInt32 (handle, "kABPersonDateProperty");
			Department = Dlfcn.GetInt32 (handle, "kABPersonDepartmentProperty");
			Email = Dlfcn.GetInt32 (handle, "kABPersonEmailProperty");
			FirstName = Dlfcn.GetInt32 (handle, "kABPersonFirstNameProperty");
			FirstNamePhonetic = Dlfcn.GetInt32 (handle, "kABPersonFirstNamePhoneticProperty");
			InstantMessage = Dlfcn.GetInt32 (handle, "kABPersonInstantMessageProperty");
			JobTitle = Dlfcn.GetInt32 (handle, "kABPersonJobTitleProperty");
			Kind = Dlfcn.GetInt32 (handle, "kABPersonKindProperty");
			LastName = Dlfcn.GetInt32 (handle, "kABPersonLastNameProperty");
			LastNamePhonetic = Dlfcn.GetInt32 (handle, "kABPersonLastNamePhoneticProperty");
			MiddleName = Dlfcn.GetInt32 (handle, "kABPersonMiddleNameProperty");
			MiddleNamePhonetic = Dlfcn.GetInt32 (handle, "kABPersonMiddleNamePhoneticProperty");
			ModificationDate = Dlfcn.GetInt32 (handle, "kABPersonModificationDateProperty");
			Nickname = Dlfcn.GetInt32 (handle, "kABPersonNicknameProperty");
			Note = Dlfcn.GetInt32 (handle, "kABPersonNoteProperty");
			Organization = Dlfcn.GetInt32 (handle, "kABPersonOrganizationProperty");
			Phone = Dlfcn.GetInt32 (handle, "kABPersonPhoneProperty");
			Prefix = Dlfcn.GetInt32 (handle, "kABPersonPrefixProperty");
			RelatedNames = Dlfcn.GetInt32 (handle, "kABPersonRelatedNamesProperty");
			Suffix = Dlfcn.GetInt32 (handle, "kABPersonSuffixProperty");
			Url = Dlfcn.GetInt32 (handle, "kABPersonURLProperty");
			SocialProfile = Dlfcn.GetInt32 (handle, "kABPersonSocialProfileProperty");
		}

		public static int ToId (ABPersonProperty property)
		{
			switch (property) {
			case ABPersonProperty.Address: return Address;
			case ABPersonProperty.Birthday: return Birthday;
			case ABPersonProperty.CreationDate: return CreationDate;
			case ABPersonProperty.Date: return Date;
			case ABPersonProperty.Department: return Department;
			case ABPersonProperty.Email: return Email;
			case ABPersonProperty.FirstName: return FirstName;
			case ABPersonProperty.FirstNamePhonetic: return FirstNamePhonetic;
			case ABPersonProperty.InstantMessage: return InstantMessage;
			case ABPersonProperty.JobTitle: return JobTitle;
			case ABPersonProperty.Kind: return Kind;
			case ABPersonProperty.LastName: return LastName;
			case ABPersonProperty.LastNamePhonetic: return LastNamePhonetic;
			case ABPersonProperty.MiddleName: return MiddleName;
			case ABPersonProperty.MiddleNamePhonetic: return MiddleNamePhonetic;
			case ABPersonProperty.ModificationDate: return ModificationDate;
			case ABPersonProperty.Nickname: return Nickname;
			case ABPersonProperty.Note: return Note;
			case ABPersonProperty.Organization: return Organization;
			case ABPersonProperty.Phone: return Phone;
			case ABPersonProperty.Prefix: return Prefix;
			case ABPersonProperty.RelatedNames: return RelatedNames;
			case ABPersonProperty.Suffix: return Suffix;
			case ABPersonProperty.Url: return Url;
			case ABPersonProperty.SocialProfile: return SocialProfile;
			}
			throw new NotSupportedException ("Invalid ABPersonProperty value: " + property);
		}

		public static ABPersonProperty ToPersonProperty (int id)
		{
			if (id == Address) return ABPersonProperty.Address;
			if (id == Birthday) return ABPersonProperty.Birthday;
			if (id == CreationDate) return ABPersonProperty.CreationDate;
			if (id == Date) return ABPersonProperty.Date;
			if (id == Department) return ABPersonProperty.Department;
			if (id == Email) return ABPersonProperty.Email;
			if (id == FirstName) return ABPersonProperty.FirstName;
			if (id == FirstNamePhonetic) return ABPersonProperty.FirstNamePhonetic;
			if (id == InstantMessage) return ABPersonProperty.InstantMessage;
			if (id == JobTitle) return ABPersonProperty.JobTitle;
			if (id == Kind) return ABPersonProperty.Kind;
			if (id == LastName) return ABPersonProperty.LastName;
			if (id == LastNamePhonetic) return ABPersonProperty.LastNamePhonetic;
			if (id == MiddleName) return ABPersonProperty.MiddleName;
			if (id == MiddleNamePhonetic) return ABPersonProperty.MiddleNamePhonetic;
			if (id == ModificationDate) return ABPersonProperty.ModificationDate;
			if (id == Nickname) return ABPersonProperty.Nickname;
			if (id == Note) return ABPersonProperty.Note;
			if (id == Organization) return ABPersonProperty.Organization;
			if (id == Phone) return ABPersonProperty.Phone;
			if (id == Prefix) return ABPersonProperty.Prefix;
			if (id == RelatedNames) return ABPersonProperty.RelatedNames;
			if (id == Suffix) return ABPersonProperty.Suffix;
			if (id == Url) return ABPersonProperty.Url;
			if (id == SocialProfile) return ABPersonProperty.SocialProfile;
			throw new NotSupportedException ("Invalid ABPersonPropertyId value: " + id);
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public static class ABPersonAddressKey {

		public static NSString? City { get; private set; }
		public static NSString? Country { get; private set; }
		public static NSString? CountryCode { get; private set; }
		public static NSString? State { get; private set; }
		public static NSString? Street { get; private set; }
		public static NSString? Zip { get; private set; }

		static ABPersonAddressKey ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			var handle = Libraries.AddressBook.Handle;
			City = Dlfcn.GetStringConstant (handle, "kABPersonAddressCityKey");
			Country = Dlfcn.GetStringConstant (handle, "kABPersonAddressCountryKey");
			CountryCode = Dlfcn.GetStringConstant (handle, "kABPersonAddressCountryCodeKey");
			State = Dlfcn.GetStringConstant (handle, "kABPersonAddressStateKey");
			Street = Dlfcn.GetStringConstant (handle, "kABPersonAddressStreetKey");
			Zip = Dlfcn.GetStringConstant (handle, "kABPersonAddressZIPKey");
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public static class ABPersonDateLabel {
		public static NSString? Anniversary { get; private set; }

		static ABPersonDateLabel ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			Anniversary = Dlfcn.GetStringConstant (Libraries.AddressBook.Handle, "kABPersonAnniversaryLabel");
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst14.0")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
#endif
	static class ABPersonKindId {
		public static NSNumber? Organization { get; private set; }
		public static NSNumber? Person { get; private set; }

		static ABPersonKindId ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			var handle = Libraries.AddressBook.Handle;
			Organization = Dlfcn.GetNSNumber (handle, "kABPersonKindOrganization");
			Person = Dlfcn.GetNSNumber (handle, "kABPersonKindPerson");
		}

		public static ABPersonKind ToPersonKind (NSNumber value)
		{
			if (object.ReferenceEquals (Organization, value))
				return ABPersonKind.Organization;
			if (object.ReferenceEquals (Person, value))
				return ABPersonKind.Person;
			return ABPersonKind.None;
		}

		public static NSNumber? FromPersonKind (ABPersonKind value)
		{
			switch (value) {
			case ABPersonKind.Organization: return Organization;
			case ABPersonKind.Person: return Person;
			}
			return null;
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	static class ABPersonSocialProfile {
		public static readonly NSString? URLKey;
		public static readonly NSString? ServiceKey;
		public static readonly NSString? UsernameKey;
		public static readonly NSString? UserIdentifierKey;

		static ABPersonSocialProfile ()
		{
			var handle = Libraries.AddressBook.Handle;
			URLKey = Dlfcn.GetStringConstant (handle, "kABPersonSocialProfileURLKey");
			ServiceKey = Dlfcn.GetStringConstant (handle, "kABPersonSocialProfileServiceKey");
			UsernameKey = Dlfcn.GetStringConstant (handle, "kABPersonSocialProfileUsernameKey");
			UserIdentifierKey = Dlfcn.GetStringConstant (handle, "kABPersonSocialProfileUserIdentifierKey");
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public static class ABPersonSocialProfileService {
		public static readonly NSString? Twitter;
		public static readonly NSString? GameCenter;
		public static readonly NSString? Facebook;
		public static readonly NSString? Myspace;
		public static readonly NSString? LinkedIn;
		public static readonly NSString? Flickr;
		public static readonly NSString? SinaWeibo;

		static ABPersonSocialProfileService ()
		{
			var handle = Libraries.AddressBook.Handle;
			Twitter = Dlfcn.GetStringConstant (handle, "kABPersonSocialProfileServiceTwitter");
			GameCenter = Dlfcn.GetStringConstant (handle, "kABPersonSocialProfileServiceGameCenter");
			Facebook = Dlfcn.GetStringConstant (handle, "kABPersonSocialProfileServiceFacebook");
			Myspace = Dlfcn.GetStringConstant (handle, "kABPersonSocialProfileServiceMyspace");
			LinkedIn = Dlfcn.GetStringConstant (handle, "kABPersonSocialProfileServiceLinkedIn");
			Flickr = Dlfcn.GetStringConstant (handle, "kABPersonSocialProfileServiceFlickr");
			SinaWeibo = Dlfcn.GetStringConstant (handle, "kABPersonSocialProfileServiceSinaWeibo");
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public static class ABPersonPhoneLabel {
		public static NSString? HomeFax { get; private set; }
		public static NSString? iPhone { get; private set; }
		public static NSString? Main { get; private set; }
		public static NSString? Mobile { get; private set; }
		public static NSString? Pager { get; private set; }
		public static NSString? WorkFax { get; private set; }
		public static NSString? OtherFax { get; private set; }

		static ABPersonPhoneLabel ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			var handle = Libraries.AddressBook.Handle;
			HomeFax = Dlfcn.GetStringConstant (handle, "kABPersonPhoneHomeFAXLabel");
			iPhone = Dlfcn.GetStringConstant (handle, "kABPersonPhoneIPhoneLabel");
			Main = Dlfcn.GetStringConstant (handle, "kABPersonPhoneMainLabel");
			Mobile = Dlfcn.GetStringConstant (handle, "kABPersonPhoneMobileLabel");
			Pager = Dlfcn.GetStringConstant (handle, "kABPersonPhonePagerLabel");
			WorkFax = Dlfcn.GetStringConstant (handle, "kABPersonPhoneWorkFAXLabel");
			OtherFax = Dlfcn.GetStringConstant (handle, "kABPersonPhoneOtherFAXLabel");
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public static class ABPersonInstantMessageService {
		public static NSString? Aim { get; private set; }
		public static NSString? Icq { get; private set; }
		public static NSString? Jabber { get; private set; }
		public static NSString? Msn { get; private set; }
		public static NSString? Yahoo { get; private set; }
		public static NSString? QQ { get; private set; }
		public static NSString? GoogleTalk { get; private set; }
		public static NSString? Skype { get; private set; }
		public static NSString? Facebook { get; private set; }
		public static NSString? GaduGadu { get; private set; }

		static ABPersonInstantMessageService ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			var handle = Libraries.AddressBook.Handle;
			Aim = Dlfcn.GetStringConstant (handle, "kABPersonInstantMessageServiceAIM");
			Icq = Dlfcn.GetStringConstant (handle, "kABPersonInstantMessageServiceICQ");
			Jabber = Dlfcn.GetStringConstant (handle, "kABPersonInstantMessageServiceJabber");
			Msn = Dlfcn.GetStringConstant (handle, "kABPersonInstantMessageServiceMSN");
			Yahoo = Dlfcn.GetStringConstant (handle, "kABPersonInstantMessageServiceYahoo");
			QQ = Dlfcn.GetStringConstant (handle, "kABPersonInstantMessageServiceQQ");
			GoogleTalk = Dlfcn.GetStringConstant (handle, "kABPersonInstantMessageServiceGoogleTalk");
			Skype = Dlfcn.GetStringConstant (handle, "kABPersonInstantMessageServiceSkype");
			Facebook = Dlfcn.GetStringConstant (handle, "kABPersonInstantMessageServiceFacebook");
			GaduGadu = Dlfcn.GetStringConstant (handle, "kABPersonInstantMessageServiceGaduGadu");
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public static class ABPersonInstantMessageKey {
		public static NSString? Service { get; private set; }
		public static NSString? Username { get; private set; }

		static ABPersonInstantMessageKey ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			var handle = Libraries.AddressBook.Handle;
			Service = Dlfcn.GetStringConstant (handle, "kABPersonInstantMessageServiceKey");
			Username = Dlfcn.GetStringConstant (handle, "kABPersonInstantMessageUsernameKey");
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public static class ABPersonUrlLabel {
		public static NSString? HomePage { get; private set; }

		static ABPersonUrlLabel ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			HomePage = Dlfcn.GetStringConstant (Libraries.AddressBook.Handle, "kABPersonHomePageLabel");
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public static class ABPersonRelatedNamesLabel {
		public static NSString? Assistant { get; private set; }
		public static NSString? Brother { get; private set; }
		public static NSString? Child { get; private set; }
		public static NSString? Father { get; private set; }
		public static NSString? Friend { get; private set; }
		public static NSString? Manager { get; private set; }
		public static NSString? Mother { get; private set; }
		public static NSString? Parent { get; private set; }
		public static NSString? Partner { get; private set; }
		public static NSString? Sister { get; private set; }
		public static NSString? Spouse { get; private set; }

		static ABPersonRelatedNamesLabel ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			var handle = Libraries.AddressBook.Handle;
			Assistant = Dlfcn.GetStringConstant (handle, "kABPersonAssistantLabel");
			Brother = Dlfcn.GetStringConstant (handle, "kABPersonBrotherLabel");
			Child = Dlfcn.GetStringConstant (handle, "kABPersonChildLabel");
			Father = Dlfcn.GetStringConstant (handle, "kABPersonFatherLabel");
			Friend = Dlfcn.GetStringConstant (handle, "kABPersonFriendLabel");
			Manager = Dlfcn.GetStringConstant (handle, "kABPersonManagerLabel");
			Mother = Dlfcn.GetStringConstant (handle, "kABPersonMotherLabel");
			Parent = Dlfcn.GetStringConstant (handle, "kABPersonParentLabel");
			Partner = Dlfcn.GetStringConstant (handle, "kABPersonPartnerLabel");
			Sister = Dlfcn.GetStringConstant (handle, "kABPersonSisterLabel");
			Spouse = Dlfcn.GetStringConstant (handle, "kABPersonSpouseLabel");
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public static class ABLabel {
		public static NSString? Home { get; private set; }
		public static NSString? Other { get; private set; }
		public static NSString? Work { get; private set; }

		static ABLabel ()
		{
			InitConstants.Init ();
		}

		internal static void Init ()
		{
			var handle = Libraries.AddressBook.Handle;
			Home = Dlfcn.GetStringConstant (handle, "kABHomeLabel");
			Other = Dlfcn.GetStringConstant (handle, "kABOtherLabel");
			Work = Dlfcn.GetStringConstant (handle, "kABWorkLabel");
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class ABPerson : ABRecord, IComparable, IComparable<ABPerson> {
		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCreate ();

		public ABPerson ()
			: base (ABPersonCreate (), true)
		{
			InitConstants.Init ();
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCreateInSource (IntPtr source);

		public ABPerson (ABRecord source)
			: base (ABPersonCreateInSource (ObjCRuntime.Runtime.ThrowOnNull (source, nameof (source)).Handle), true)
		{
		}

		[Preserve (Conditional = true)]
		internal ABPerson (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		internal ABPerson (NativeHandle handle, ABAddressBook? addressbook)
			: base (handle, false)
		{
			AddressBook = addressbook;
		}

		int IComparable.CompareTo (object? o)
		{
			var other = o as ABPerson;
			if (other is null)
				throw new ArgumentException ("Can only compare to other ABPerson instances.", nameof (o));
			return CompareTo (other);
		}

		public int CompareTo (ABPerson? other)
		{
			return CompareTo (other!, ABPersonSortBy.LastName);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static int ABPersonComparePeopleByName (IntPtr person1, IntPtr person2, ABPersonSortBy ordering);
		public int CompareTo (ABPerson other, ABPersonSortBy ordering)
		{
			if (other is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (other));
			if (ordering != ABPersonSortBy.FirstName && ordering != ABPersonSortBy.LastName)
				throw new ArgumentException ("Invalid ordering value: " + ordering, "ordering");
			return ABPersonComparePeopleByName (Handle, other.Handle, ordering);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCopyLocalizedPropertyName (int /* ABPropertyID = int32_t */ propertyId);
		public static string? LocalizedPropertyName (ABPersonProperty property)
		{
			return CFString.FromHandle (ABPersonCopyLocalizedPropertyName (ABPersonPropertyId.ToId (property)));
		}

		public static string? LocalizedPropertyName (int propertyId)
		{
			return CFString.FromHandle (ABPersonCopyLocalizedPropertyName (propertyId));
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static ABPropertyType ABPersonGetTypeOfProperty (int /* ABPropertyID = int32_t */ propertyId);
		public static ABPropertyType GetPropertyType (ABPersonProperty property)
		{
			return ABPersonGetTypeOfProperty (ABPersonPropertyId.ToId (property));
		}

		public static ABPropertyType GetPropertyType (int propertyId)
		{
			return ABPersonGetTypeOfProperty (propertyId);
		}

		[DllImport (Constants.AddressBookLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool ABPersonSetImageData (IntPtr person, IntPtr imageData, out IntPtr error);
		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCopyImageData (IntPtr person);

		public NSData? Image {
			get { return Runtime.GetNSObject<NSData> (ABPersonCopyImageData (Handle)); }
			set {
				if (!ABPersonSetImageData (Handle, value.GetHandle (), out var error))
					throw CFException.FromCFError (error);
			}
		}

		[DllImport (Constants.AddressBookLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool ABPersonHasImageData (IntPtr person);
		public bool HasImage {
			get { return ABPersonHasImageData (Handle); }
		}

		[DllImport (Constants.AddressBookLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool ABPersonRemoveImageData (IntPtr person, out IntPtr error);
		public void RemoveImage ()
		{
			if (!ABPersonRemoveImageData (Handle, out var error))
				throw CFException.FromCFError (error);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static ABPersonCompositeNameFormat ABPersonGetCompositeNameFormat ();

#if NET
		[SupportedOSPlatform ("maccatalyst14.0")]
		[SupportedOSPlatform ("ios")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
		[ObsoletedOSPlatform ("ios7.0", "Use 'GetCompositeNameFormat (null)' instead.")]
#else
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'GetCompositeNameFormat (null)' instead.")]
#endif
		public static ABPersonCompositeNameFormat CompositeNameFormat {
			get { return ABPersonGetCompositeNameFormat (); }
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#endif
		[DllImport (Constants.AddressBookLibrary)]
		extern static ABPersonCompositeNameFormat ABPersonGetCompositeNameFormatForRecord (IntPtr record);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#endif
		public static ABPersonCompositeNameFormat GetCompositeNameFormat (ABRecord? record)
		{
			return ABPersonGetCompositeNameFormatForRecord (record.GetHandle ());
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#endif
		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCopyCompositeNameDelimiterForRecord (IntPtr record);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst14.0")]
		[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
		[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#endif
		public static string? GetCompositeNameDelimiter (ABRecord? record)
		{
			var handle = ABPersonCopyCompositeNameDelimiterForRecord (record.GetHandle ());
			return CFString.FromHandle (handle, true);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static ABPersonSortBy ABPersonGetSortOrdering ();
		public static ABPersonSortBy SortOrdering {
			get { return ABPersonGetSortOrdering (); }
		}

		public string? FirstName {
			get { return PropertyToString (ABPersonPropertyId.FirstName); }
			set { SetValue (ABPersonPropertyId.FirstName, value); }
		}

		public string? FirstNamePhonetic {
			get { return PropertyToString (ABPersonPropertyId.FirstNamePhonetic); }
			set { SetValue (ABPersonPropertyId.FirstNamePhonetic, value); }
		}

		public string? LastName {
			get { return PropertyToString (ABPersonPropertyId.LastName); }
			set { SetValue (ABPersonPropertyId.LastName, value); }
		}

		public string? LastNamePhonetic {
			get { return PropertyToString (ABPersonPropertyId.LastNamePhonetic); }
			set { SetValue (ABPersonPropertyId.LastNamePhonetic, value); }
		}

		public string? MiddleName {
			get { return PropertyToString (ABPersonPropertyId.MiddleName); }
			set { SetValue (ABPersonPropertyId.MiddleName, value); }
		}

		public string? MiddleNamePhonetic {
			get { return PropertyToString (ABPersonPropertyId.MiddleNamePhonetic); }
			set { SetValue (ABPersonPropertyId.MiddleNamePhonetic, value); }
		}

		public string? Prefix {
			get { return PropertyToString (ABPersonPropertyId.Prefix); }
			set { SetValue (ABPersonPropertyId.Prefix, value); }
		}

		public string? Suffix {
			get { return PropertyToString (ABPersonPropertyId.Suffix); }
			set { SetValue (ABPersonPropertyId.Suffix, value); }
		}

		public string? Nickname {
			get { return PropertyToString (ABPersonPropertyId.Nickname); }
			set { SetValue (ABPersonPropertyId.Nickname, value); }
		}

		public string? Organization {
			get { return PropertyToString (ABPersonPropertyId.Organization); }
			set { SetValue (ABPersonPropertyId.Organization, value); }
		}

		public string? JobTitle {
			get { return PropertyToString (ABPersonPropertyId.JobTitle); }
			set { SetValue (ABPersonPropertyId.JobTitle, value); }
		}

		public string? Department {
			get { return PropertyToString (ABPersonPropertyId.Department); }
			set { SetValue (ABPersonPropertyId.Department, value); }
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCopySource (IntPtr group);

		public ABRecord? Source {
			get {
				var h = ABPersonCopySource (Handle);
				if (h == IntPtr.Zero)
					return null;

				return FromHandle (h, null);
			}
		}

		internal static string ToString (NativeHandle value)
		{
			return CFString.FromHandle (value)!;
		}

		public ABMultiValue<string>? GetEmails ()
		{
			return CreateStringMultiValue (CopyValue (ABPersonPropertyId.Email));
		}

		static ABMultiValue<string>? CreateStringMultiValue (NativeHandle handle)
		{
			if (handle == NativeHandle.Zero)
				return null;
			return new ABMultiValue<string> (handle, ABPerson.ToString, CFString.CreateNative, true);
		}

		public void SetEmails (ABMultiValue<string>? value)
		{
			SetValue (ABPersonPropertyId.Email, value.GetHandle ());
		}

		public NSDate? Birthday {
			get { return PropertyTo<NSDate> (ABPersonPropertyId.Birthday); }
			set { SetValue (ABPersonPropertyId.Birthday, value); }
		}

		public string? Note {
			get { return PropertyToString (ABPersonPropertyId.Note); }
			set { SetValue (ABPersonPropertyId.Note, value); }
		}

		public NSDate? CreationDate {
			get { return PropertyTo<NSDate> (ABPersonPropertyId.CreationDate); }
			set { SetValue (ABPersonPropertyId.CreationDate, value); }
		}

		public NSDate? ModificationDate {
			get { return PropertyTo<NSDate> (ABPersonPropertyId.ModificationDate); }
			set { SetValue (ABPersonPropertyId.ModificationDate, value); }
		}

		public ABMultiValue<PersonAddress>? GetAllAddresses ()
		{
			return CreateDictionaryMultiValue<PersonAddress> (CopyValue (ABPersonPropertyId.Address), l => new PersonAddress (l));
		}

		// Obsolete
		public void SetAddresses (ABMultiValue<NSDictionary>? value)
		{
			SetValue (ABPersonPropertyId.Address, value.GetHandle ());
		}

		public void SetAddresses (ABMultiValue<PersonAddress>? addresses)
		{
			SetValue (ABPersonPropertyId.Address, addresses.GetHandle ());
		}

		// Obsolete
		static ABMultiValue<NSDictionary>? CreateDictionaryMultiValue (NativeHandle handle)
		{
			if (handle == NativeHandle.Zero)
				return null;
			return new ABMultiValue<NSDictionary> (handle, true);
		}

		static ABMultiValue<T>? CreateDictionaryMultiValue<T> (NativeHandle handle, Func<NSDictionary, T> factory) where T : DictionaryContainer
		{
			if (handle == NativeHandle.Zero)
				return null;

			return new ABMultiValue<T> (handle,
				l => factory ((NSDictionary) (object) Runtime.GetNSObject (l)!),
				l => l.Dictionary.Handle,
				false);
		}

		public ABMultiValue<NSDate>? GetDates ()
		{
			return CreateDateMultiValue (CopyValue (ABPersonPropertyId.Date));
		}

		static ABMultiValue<NSDate>? CreateDateMultiValue (NativeHandle handle)
		{
			if (handle == NativeHandle.Zero)
				return null;
			return new ABMultiValue<NSDate> (handle, true);
		}

		public void SetDates (ABMultiValue<NSDate>? value)
		{
			SetValue (ABPersonPropertyId.Date, value.GetHandle ());
		}

		public ABPersonKind PersonKind {
			get { return ABPersonKindId.ToPersonKind (PropertyTo<NSNumber> (ABPersonPropertyId.Kind!)!); }
			set { SetValue (ABPersonPropertyId.Kind!, ABPersonKindId.FromPersonKind (value)); }
		}

		public ABMultiValue<string>? GetPhones ()
		{
			return CreateStringMultiValue (CopyValue (ABPersonPropertyId.Phone));
		}

		public void SetPhones (ABMultiValue<string>? value)
		{
			SetValue (ABPersonPropertyId.Phone, value.GetHandle ());
		}

		[Advice ("Use GetInstantMessageServices.")]
		ABMultiValue<NSDictionary>? GetInstantMessages ()
		{
			return CreateDictionaryMultiValue (CopyValue (ABPersonPropertyId.InstantMessage));
		}

		public ABMultiValue<InstantMessageService>? GetInstantMessageServices ()
		{
			return CreateDictionaryMultiValue<InstantMessageService> (CopyValue (ABPersonPropertyId.InstantMessage), l => new InstantMessageService (l));
		}

		// Obsolete
		public void SetInstantMessages (ABMultiValue<NSDictionary>? value)
		{
			SetValue (ABPersonPropertyId.InstantMessage, value.GetHandle ());
		}

		public void SetInstantMessages (ABMultiValue<InstantMessageService>? services)
		{
			SetValue (ABPersonPropertyId.InstantMessage, services.GetHandle ());
		}

		[Advice ("Use GetSocialProfiles.")]
		ABMultiValue<NSDictionary>? GetSocialProfile ()
		{
			return CreateDictionaryMultiValue (CopyValue (ABPersonPropertyId.SocialProfile));
		}

		public ABMultiValue<SocialProfile>? GetSocialProfiles ()
		{
			return CreateDictionaryMultiValue<SocialProfile> (CopyValue (ABPersonPropertyId.SocialProfile), l => new SocialProfile (l));
		}

		// Obsolete
		public void SetSocialProfile (ABMultiValue<NSDictionary>? value)
		{
			SetValue (ABPersonPropertyId.SocialProfile, value.GetHandle ());
		}

		public void SetSocialProfile (ABMultiValue<SocialProfile>? profiles)
		{
			SetValue (ABPersonPropertyId.SocialProfile, profiles.GetHandle ());
		}

		public ABMultiValue<string>? GetUrls ()
		{
			return CreateStringMultiValue (CopyValue (ABPersonPropertyId.Url));
		}

		public void SetUrls (ABMultiValue<string>? value)
		{
			SetValue (ABPersonPropertyId.Url, value.GetHandle ());
		}

		public ABMultiValue<string>? GetRelatedNames ()
		{
			return CreateStringMultiValue (CopyValue (ABPersonPropertyId.RelatedNames));
		}

		public void SetRelatedNames (ABMultiValue<string>? value)
		{
			SetValue (ABPersonPropertyId.RelatedNames, value.GetHandle ());
		}

		public object? GetProperty (ABPersonProperty property)
		{
			switch (property) {
			case ABPersonProperty.Address: return GetAllAddresses ();
			case ABPersonProperty.Birthday: return Birthday;
			case ABPersonProperty.CreationDate: return CreationDate;
			case ABPersonProperty.Date: return GetDates ();
			case ABPersonProperty.Department: return Department;
			case ABPersonProperty.Email: return GetEmails ();
			case ABPersonProperty.FirstName: return FirstName;
			case ABPersonProperty.FirstNamePhonetic: return FirstNamePhonetic;
			case ABPersonProperty.InstantMessage: return GetInstantMessages ();
			case ABPersonProperty.JobTitle: return JobTitle;
			case ABPersonProperty.Kind: return PersonKind;
			case ABPersonProperty.LastName: return LastName;
			case ABPersonProperty.LastNamePhonetic: return LastNamePhonetic;
			case ABPersonProperty.MiddleName: return MiddleName;
			case ABPersonProperty.MiddleNamePhonetic: return MiddleNamePhonetic;
			case ABPersonProperty.ModificationDate: return ModificationDate;
			case ABPersonProperty.Nickname: return Nickname;
			case ABPersonProperty.Note: return Note;
			case ABPersonProperty.Organization: return Organization;
			case ABPersonProperty.Phone: return GetPhones ();
			case ABPersonProperty.Prefix: return Prefix;
			case ABPersonProperty.RelatedNames: return GetRelatedNames ();
			case ABPersonProperty.Suffix: return Suffix;
			case ABPersonProperty.Url: return GetUrls ();
			case ABPersonProperty.SocialProfile: return GetSocialProfile ();
			}
			throw new ArgumentException ("Invalid property value: " + property);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCopyArrayOfAllLinkedPeople (IntPtr person);

		public ABPerson? []? GetLinkedPeople ()
		{
			var linked = ABPersonCopyArrayOfAllLinkedPeople (Handle);
			return NSArray.ArrayFromHandle (linked, l => new ABPerson (l, null));
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCopyImageDataWithFormat (IntPtr handle, nint format);

		public NSData? GetImage (ABPersonImageFormat format)
		{
#if ARCH_32
			return Runtime.GetNSObject<NSData> (ABPersonCopyImageDataWithFormat (Handle, (nint)(int)format));
#else
			return Runtime.GetNSObject<NSData> (ABPersonCopyImageDataWithFormat (Handle, (nint) (long) format));
#endif
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCreateVCardRepresentationWithPeople (IntPtr people);

		public static NSData? GetVCards (params ABPerson [] people)
		{
			if (people is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (people));

			var ptrs = new NativeHandle [people.Length];
			for (int i = 0; i < people.Length; ++i) {
				ptrs [i] = people [i].Handle;
			}

			var ptr = ABPersonCreateVCardRepresentationWithPeople (CFArray.Create (ptrs));
			return Runtime.GetNSObject<NSData> (ptr, true);
		}

		[DllImport (Constants.AddressBookLibrary)]
		extern static IntPtr ABPersonCreatePeopleInSourceWithVCardRepresentation (IntPtr source, IntPtr vCardData);

		public static ABPerson? []? CreateFromVCard (ABRecord? source, NSData vCardData)
		{
			if (vCardData is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (vCardData));

			// TODO: SIGSEGV when source is not null
			var res = ABPersonCreatePeopleInSourceWithVCardRepresentation (source.GetHandle (), vCardData.Handle);

			return NSArray.ArrayFromHandle (res, l => new ABPerson (l, null));
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class SocialProfile : DictionaryContainer {
		public SocialProfile ()
		{
		}

		public SocialProfile (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public string? ServiceName {
			get {
				return GetStringValue (ABPersonSocialProfile.ServiceKey!);
			}
			set {
				SetStringValue (ABPersonSocialProfile.ServiceKey!, value);
			}
		}

		public string? Username {
			get {
				return GetStringValue (ABPersonSocialProfile.UsernameKey!);
			}
			set {
				SetStringValue (ABPersonSocialProfile.UsernameKey!, value);
			}
		}

		public string? UserIdentifier {
			get {
				return GetStringValue (ABPersonSocialProfile.UserIdentifierKey!);
			}
			set {
				SetStringValue (ABPersonSocialProfile.UserIdentifierKey!, value);
			}
		}

		public string? Url {
			get {
				return GetStringValue (ABPersonSocialProfile.URLKey!);
			}
			set {
				SetStringValue (ABPersonSocialProfile.URLKey!, value);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class InstantMessageService : DictionaryContainer {
		public InstantMessageService ()
		{
		}

		public InstantMessageService (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public string? ServiceName {
			get {
				// TODO: It does not return ABPersonInstantMessageService value. Underlying
				// value is custom string, it coould be MT bug because this makes
				// ABPersonInstantMessageService constants useless
				return GetStringValue (ABPersonInstantMessageKey.Service!);
			}
			set {
				SetStringValue (ABPersonInstantMessageKey.Service!, value);
			}
		}

		public string? Username {
			get {
				return GetStringValue (ABPersonInstantMessageKey.Username!);
			}
			set {
				SetStringValue (ABPersonInstantMessageKey.Username!, value);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[SupportedOSPlatform ("ios")]
	[ObsoletedOSPlatform ("maccatalyst14.0", "Use the 'Contacts' API instead.")]
	[ObsoletedOSPlatform ("ios9.0", "Use the 'Contacts' API instead.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use the 'Contacts' API instead.")]
#endif
	public class PersonAddress : DictionaryContainer {
		public PersonAddress ()
		{
		}

		public PersonAddress (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		public string? City {
			get {
				return GetStringValue (ABPersonAddressKey.City!);
			}
			set {
				SetStringValue (ABPersonAddressKey.City!, value);
			}
		}

		public string? Country {
			get {
				return GetStringValue (ABPersonAddressKey.Country!);
			}
			set {
				SetStringValue (ABPersonAddressKey.Country!, value);
			}
		}

		public string? CountryCode {
			get {
				return GetStringValue (ABPersonAddressKey.CountryCode!);
			}
			set {
				SetStringValue (ABPersonAddressKey.CountryCode!, value);
			}
		}

		public string? State {
			get {
				return GetStringValue (ABPersonAddressKey.State!);
			}
			set {
				SetStringValue (ABPersonAddressKey.State!, value);
			}
		}

		public string? Street {
			get {
				return GetStringValue (ABPersonAddressKey.Street!);
			}
			set {
				SetStringValue (ABPersonAddressKey.Street!, value);
			}
		}

		public string? Zip {
			get {
				return GetStringValue (ABPersonAddressKey.Zip!);
			}
			set {
				SetStringValue (ABPersonAddressKey.Zip!, value);
			}
		}
	}
}

#endif // !MONOMAC
