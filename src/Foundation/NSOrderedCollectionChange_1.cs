using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;

#nullable enable

namespace Foundation {

#if !NET
	[Watch (6,0), TV (13,0), Mac (10,15), iOS (13,0)]
#else
	[SupportedOSPlatform ("ios13.0"), SupportedOSPlatform ("tvos13.0"), SupportedOSPlatform ("macos10.15")]
#endif
	[Register (SkipRegistration = true)]
	public sealed partial class NSOrderedCollectionChange<TKey> : NSOrderedCollectionChange
		where TKey : class, INativeObject {

		public static NSOrderedCollectionChange<TKey> ChangeWithObject (TKey? anObject, NSCollectionChangeType type, nuint index)
			=> new NSOrderedCollectionChange<TKey> (NSOrderedCollectionChange._ChangeWithObject (anObject!.Handle, type, index));

		public static NSOrderedCollectionChange<TKey> ChangeWithObject (TKey? anObject, NSCollectionChangeType type, nuint index, nuint associatedIndex)
			=> new NSOrderedCollectionChange<TKey> (NSOrderedCollectionChange._ChangeWithObject (anObject!.Handle, type, index, associatedIndex));

		internal NSOrderedCollectionChange (IntPtr handle) : base (handle) { }

		public NSOrderedCollectionChange (TKey? anObject, NSCollectionChangeType type, nuint index) 
			: base (anObject!.Handle, type, index) {}

		public NSOrderedCollectionChange (TKey? anObject, NSCollectionChangeType type, nuint index, nuint associatedIndex)
			: base (anObject!.Handle, type, index, associatedIndex) {}

		TKey? Object => Runtime.GetINativeObject<TKey> (_Object, true);
	}
}
