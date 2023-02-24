#nullable enable
using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using ObjCRuntime;

namespace Foundation {
#if false // https://github.com/xamarin/xamarin-macios/issues/15577
	public delegate NSOrderedCollectionDifference<NSObject>? NSOrderedCollectionDifferenceGetDifferenceHandler (NSOrderedCollectionChange<NSObject>? collectionChange);

#if !NET
	[Watch (6,0), TV (13,0), iOS (13,0)]
#else
	[SupportedOSPlatform ("ios13.0"), SupportedOSPlatform ("tvos13.0"), SupportedOSPlatform ("macos")]
#endif
	public partial class NSOrderedCollectionDifference
	{

		public NSObject[] Insertions => NSArray.ArrayFromHandle<NSObject> (_Insertions);

		public NSObject[] Removals => NSArray.ArrayFromHandle<NSObject> (_Removals);

		internal delegate NSOrderedCollectionDifference<NSObject>? NSOrderedCollectionDifferenceGetDifferenceHandlerProxy (IntPtr blockLiteral, /* NSOrderedCollectionChange */ IntPtr change);
		static readonly NSOrderedCollectionDifferenceGetDifferenceHandlerProxy static_ChangeAllback = GetDiffHandler;

		[MonoPInvokeCallback (typeof (NSOrderedCollectionDifferenceGetDifferenceHandlerProxy))]
		static NSOrderedCollectionDifference<NSObject>? GetDiffHandler (IntPtr block, IntPtr change)
		{
			var callback = BlockLiteral.GetTarget<NSOrderedCollectionDifferenceGetDifferenceHandler> (block);
			if (callback is not null) {
				var nsChange = Runtime.GetNSObject<NSOrderedCollectionChange<NSObject>> (change, false);
				return callback (nsChange);
			}
			return null;
		}

		public NSOrderedCollectionDifference? GetDifference (NSOrderedCollectionDifferenceGetDifferenceHandler callback)
		{
			if (callback is null)
				throw new ArgumentNullException (nameof (callback));

			var block = new BlockLiteral ();
			block.SetupBlock (static_ChangeAllback, callback);
			try {
				return Runtime.GetNSObject<NSOrderedCollectionDifference> (_GetDifference (ref block));
			} finally {
				block.CleanupBlock ();
			}
		}
	
	}
#endif
}
