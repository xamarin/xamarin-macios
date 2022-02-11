// Copyright 2015 Xamarin Inc. All rights reserved.

// note: Contacts is not part of classic as several API requires generics

#if !XAMCORE_3_0

using System;
using Foundation;
using ObjCRuntime;

namespace Contacts {

#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public static partial class CNGroup_PredicatesExtension {

#if !MONOMAC
		[Obsolete ("This API is only available on macOS 10.11+.")]
		public static Foundation.NSPredicate GetPredicateForSubgroupsInGroup (CNGroup This, string parentGroupIdentifier)
		{
			return null;
		}
#endif
	}
}

#endif
