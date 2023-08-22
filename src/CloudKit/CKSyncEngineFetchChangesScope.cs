using System;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace CloudKit {
		public partial class CKSyncEngineFetchChangesScope {
			public CKSyncEngineFetchChangesScope (NSSet<CKRecordZoneID>? zoneIds, bool excluded) {
				if (excluded) {
					InitializeHandle (_InitWithExcludedZoneIds (zoneIds!), "initWithZoneIDs:");
				} else {
					InitializeHandle (_InitWithZoneIds (zoneIds!), "initWithExcludedZoneIDs:");
				}
			}
		}
}

