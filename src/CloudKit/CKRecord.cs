using XamCore.ObjCRuntime;
using XamCore.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;

namespace XamCore.CloudKit
{
	public partial class CKRecord
	{
#if XAMCORE_2_0 || !MONOMAC
		public NSObject this[string key] {
			get { return _ObjectForKey (key); }
			set { _SetObject (value.Handle, key); }
		}
#endif

#if !XAMCORE_2_0 && !MONOMAC
		public CKRecordID Id {
			get {
				return RecordId;
			}
		}
#endif
	}
}

