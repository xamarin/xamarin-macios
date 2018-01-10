//
// CNContactStore.cs:
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;

namespace Contacts {
#if XAMCORE_2_0 // The Contacts framework uses generics heavily, which is only supported in Unified (for now at least)
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
#endif // XAMCORE_2_0
}
