using System;

namespace CloudKit {

	public partial class CKRecordID {
#if !XAMCORE_3_0
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public CKRecordID ()
		{
		}
#endif
	}
}