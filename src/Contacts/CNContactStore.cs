//
// CNContactStore.cs:
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#nullable enable

using System;

using Foundation;

using ObjCRuntime;

namespace Contacts {
	public partial class CNContactStore {

		public CNContact? GetUnifiedContact<T> (string identifier, T [] keys, out NSError? error)
			where T : INSObjectProtocol, INSSecureCoding, INSCopying
		{
			using (var array = NSArray.From<T> (keys))
				return GetUnifiedContact (identifier, array, out error);
		}

		public CNContact []? GetUnifiedContacts<T> (NSPredicate predicate, T [] keys, out NSError? error)
			where T : INSObjectProtocol, INSSecureCoding, INSCopying
		{
			using (var array = NSArray.From<T> (keys))
				return GetUnifiedContacts (predicate, array, out error);
		}

#if MONOMAC
		public NSObject? GetUnifiedMeContact<T> (T [] keys, out NSError? error)
			where T : INSObjectProtocol, INSSecureCoding, INSCopying
		{
			using (var array = NSArray.From<T> (keys))
				return GetUnifiedMeContact (array, out error);
		}
#endif

	}
}
