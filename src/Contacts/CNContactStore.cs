//
// CNContactStore.cs:
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace Contacts {
#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class CNContactStore {

		public CNContact GetUnifiedContact<T> (string identifier, T [] keys, out NSError error)
			where T : INSObjectProtocol, INSSecureCoding, INSCopying
		{
			using (var array = NSArray.From<T> (keys))
				return GetUnifiedContact (identifier, array, out error);
		}

		public CNContact[] GetUnifiedContacts<T> (NSPredicate predicate, T [] keys, out NSError error)
			where T : INSObjectProtocol, INSSecureCoding, INSCopying
		{
			using (var array = NSArray.From<T> (keys))
				return GetUnifiedContacts (predicate, array, out error);
		}

#if MONOMAC
		public NSObject GetUnifiedMeContact<T> (T [] keys, out NSError error)
			where T : INSObjectProtocol, INSSecureCoding, INSCopying
		{
			using (var array = NSArray.From<T> (keys))
				return GetUnifiedMeContact (array, out error);
		}
#endif
		
	}
}
