// 
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
// Copyright 2011-2013, 2016 Xamarin Inc.
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

#if !MONOMAC

using System;
using System.Runtime.Versioning;

using Foundation;
using CoreFoundation;
using ObjCRuntime;

namespace AddressBook {
#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[UnsupportedOSPlatform ("maccatalyst14.0")]
	[UnsupportedOSPlatform ("ios9.0")]
#if __MACCATALYST__
	[Obsolete ("Starting with maccatalyst14.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios9.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message : "Use the 'Contacts' API instead.")]
#endif
	[Native]
	[ErrorDomain ("ABAddressBookErrorDomain")]
	public enum ABAddressBookError : long {
		OperationNotPermittedByStore = 0,
		OperationNotPermittedByUserError
	}

#if NET
	[SupportedOSPlatform ("maccatalyst14.0")]
	[UnsupportedOSPlatform ("maccatalyst14.0")]
	[UnsupportedOSPlatform ("ios9.0")]
#if __MACCATALYST__
	[Obsolete ("Starting with maccatalyst14.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
	[Obsolete ("Starting with ios9.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message : "Use the 'Contacts' API instead.")]
#endif
	[Native]
	public enum ABAuthorizationStatus : long {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized
	}

#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
#endif
	public enum ABPersonSortBy : uint /* uint32_t */ {
		FirstName = 0,
		LastName  = 1,
	}

#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
#endif
	public enum ABPersonCompositeNameFormat : uint /* uint32_t */ {
		FirstNameFirst  = 0,
		LastNameFirst   = 1,
	}

#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
#endif
	public enum ABPersonProperty {
		Address,
		Birthday,
		CreationDate,
		Date,
		Department,
		Email,
		FirstName,
		FirstNamePhonetic,
		InstantMessage,
		JobTitle,
		Kind,
		LastName,
		LastNamePhonetic,
		MiddleName,
		MiddleNamePhonetic,
		ModificationDate,
		Nickname,
		Note,
		Organization,
		Phone,
		Prefix,
		RelatedNames,
		Suffix,
		Url,
		SocialProfile,
	}

#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
#endif
	[Native]
	public enum ABPersonImageFormat : long {
		Thumbnail = 0,
		OriginalSize = 2,
	}

#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
#endif
	public enum ABPersonKind {
		None,
		Organization,
		Person,
	}

#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
#endif
	public enum ABRecordType : uint /* uint32_t */ {
		Person = 0,
		Group = 1,
		Source = 2,
	}

#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
#endif
	public enum ABPropertyType : uint /* uint32_t */ {
		Invalid         = 0,
		String          = 0x1,
		Integer         = 0x2,
		Real            = 0x3,
		DateTime        = 0x4,
		Dictionary      = 0x5,
		MultiString     = MultiMask | String,
		MultiInteger    = MultiMask | Integer,
		MultiReal       = MultiMask | Real,
		MultiDateTime   = MultiMask | DateTime,
		MultiDictionary = MultiMask | Dictionary,

		MultiMask       = (1 << 8),
	}

	// note: not a true flag
#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
#endif
	public enum ABSourceType : int /* typedef int */ {
		Local		= 0x0,
		Exchange	= 0x1,
		ExchangeGAL	= Exchange | SearchableMask,
		MobileMe	= 0x2,
		LDAP		= 0x3 | SearchableMask,
		CardDAV		= 0x4,
		DAVSearch	= CardDAV | SearchableMask,

		SearchableMask = 0x01000000,
	};

#if NET
	[UnsupportedOSPlatform ("ios9.0")]
#if IOS
	[Obsolete ("Starting with ios9.0 use the 'Contacts' API instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Contacts' API instead.")]
#endif
	public enum ABSourceProperty {
		Name,
		Type,
	}

}

#endif
