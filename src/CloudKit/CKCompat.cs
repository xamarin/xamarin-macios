// Copyright 2016, Xamarin Inc. All rights reserved.

#if !COREBUILD

using CloudKit;
using ObjCRuntime;
using Foundation;
using System;

namespace CloudKit {

#if !XAMCORE_4_0 && !WATCH
	public partial class CKOperation {

		[Obsoleted (PlatformName.iOS, 9,3, message: "Do not use; this API was removed and will always return 0.")]
		public virtual ulong ActivityStart ()
		{
			return 0;
		}
	}

	public partial class CKNotificationID {

		[Obsolete ("This type is not meant to be created by user code.")]
		public CKNotificationID ()
		{
		}
	}
#endif

#if WATCH

	public partial class CKModifyBadgeOperation {

		// `init` does not work on watchOS but we can keep compatibility with a different init
		public CKModifyBadgeOperation () : this (0)
		{
		}
	}

	public partial class CKModifyRecordZonesOperation {

		// `init` does not work on watchOS but we can keep compatibility with a different init
		public CKModifyRecordZonesOperation () : this (null, null)
		{
		}
	}

	public partial class CKModifyRecordsOperation {

		// `init` does not work on watchOS but we can keep compatibility with a different init
		public CKModifyRecordsOperation () : this (null, null)
		{
		}
	}
#endif
}

#endif
