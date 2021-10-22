//
// This file contains a generic version of NSOrderedCollectionDifference
//
#nullable enable
using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;

namespace Foundation {

#if !NET
	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
#else
	[SupportedOSPlatform ("ios13.0"), SupportedOSPlatform ("tvos13.0"), SupportedOSPlatform ("macos10.15")]
#endif
	[Register (SkipRegistration = true)]
	public sealed partial class NSOrderedCollectionDifference<TKey> : NSOrderedCollectionDifference
		where TKey : class, INativeObject {

		internal NSOrderedCollectionDifference (IntPtr handle)
			: base (handle) { }

		public NSOrderedCollectionDifference (NSOrderedCollectionChange<TKey>[] changes)
			: base (changes) {}

		public NSOrderedCollectionDifference (NSIndexSet inserts, TKey[]? insertedObjects, NSIndexSet removes, TKey[]? removedObjects, NSOrderedCollectionChange<TKey>[] changes)
			: base (inserts, NSArray.FromNSObjects (insertedObjects), removes, NSArray.FromNSObjects (removedObjects), changes) {}

		public NSOrderedCollectionDifference (NSIndexSet inserts, TKey[]? insertedObjects, NSIndexSet removes, TKey[]? removedObjects)
			: base (inserts, NSArray.FromNSObjects (insertedObjects), removes, NSArray.FromNSObjects (removedObjects)) {}

		TKey[] Insertions => NSArray.ArrayFromHandle<TKey> (_Insertions);

		TKey[] Removals => NSArray.ArrayFromHandle<TKey> (_Removals);

	}
}
