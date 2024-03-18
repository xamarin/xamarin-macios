using System;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace CloudKit {
	public partial class CKSyncEngineFetchChangesScope {
		public CKSyncEngineFetchChangesScope (NSSet<CKRecordZoneID>? zoneIds, bool excluded)
		{
			if (excluded) {
				// needs to be converted to an empty set
				zoneIds ??= new NSSet<CKRecordZoneID> ();
				InitializeHandle (_InitWithExcludedZoneIds (zoneIds!), "initWithZoneIDs:");
			} else {
				// supports a null parameter
				InitializeHandle (_InitWithZoneIds (zoneIds!), "initWithExcludedZoneIDs:");
			}
		}
	}
}

