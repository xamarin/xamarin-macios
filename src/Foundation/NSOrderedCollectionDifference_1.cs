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
#if false // https://github.com/xamarin/xamarin-macios/issues/15577
#if !NET
	[Watch (6,0), TV (13,0), iOS (13,0)]
#else
	[SupportedOSPlatform ("ios13.0"), SupportedOSPlatform ("tvos13.0"), SupportedOSPlatform ("macos")]
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

		public TKey[] Insertions => NSArray.ArrayFromHandle<TKey> (_Insertions);

		public TKey[] Removals => NSArray.ArrayFromHandle<TKey> (_Removals);

		static readonly NSOrderedCollectionDifferenceGetDifferenceHandlerProxy static_ChangeAllback = GetDiffHandlerTemplate;

		[MonoPInvokeCallback (typeof (NSOrderedCollectionDifferenceGetDifferenceHandlerProxy))]
		static NSOrderedCollectionDifference<NSObject>? GetDiffHandlerTemplate (IntPtr block, IntPtr change)
		{
			var callback = BlockLiteral.GetTarget<NSOrderedCollectionDifferenceGetDifferenceHandler> (block);
			if (callback is not null) {
				var nsChange = Runtime.GetNSObject<NSOrderedCollectionChange<NSObject>> (change, false);
				return callback (nsChange);
			}
			return null;
		}

		public NSOrderedCollectionDifference<TKey>? GetDifference (NSOrderedCollectionDifferenceGetDifferenceHandler callback)
		{
			if (callback is null)
				throw new ArgumentNullException (nameof (callback));

			var block = new BlockLiteral ();
			block.SetupBlock (static_ChangeAllback, callback);
			try {
				return Runtime.GetNSObject<NSOrderedCollectionDifference<TKey>> (_GetDifference (ref block));
			} finally {
				block.CleanupBlock ();
			}
		}

	}
#endif
}
