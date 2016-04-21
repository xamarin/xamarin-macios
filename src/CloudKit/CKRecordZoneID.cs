using System;

namespace XamCore.CloudKit {

	public partial class CKRecordZoneID {
#if !XAMCORE_3_0
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public CKRecordZoneID ()
		{
		}
#endif
	}
}