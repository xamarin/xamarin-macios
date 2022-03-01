// Copyright 2014-2015 Xamarin Inc. All rights reserved.

using System;

#nullable enable

namespace CoreData {
#if !XAMCORE_3_0
	public partial class NSMergeConflict {

		[Obsolete ("Default constructor is not available")]
		public NSMergeConflict ()
		{
		}
	}

	public partial class NSMergePolicy {

		[Obsolete ("Default constructor is not available")]
		public NSMergePolicy ()
		{
		}
	}

	public partial class NSPersistentStore {

		[Obsolete ("Default constructor is not available")]
		public NSPersistentStore ()
		{
		}
	}
#endif
#if !NET
	public partial class NSCoreDataCoreSpotlightDelegate {

		[Obsolete ("Default constructor is not available")]
		public NSCoreDataCoreSpotlightDelegate ()
		{
		}
	}
#endif
}
