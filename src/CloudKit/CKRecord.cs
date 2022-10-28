#nullable enable

using ObjCRuntime;
using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CloudKit {
	public partial class CKRecord {
		public NSObject? this [string key] {
			get { return _ObjectForKey (key); }
			set { _SetObject (value.GetHandle (), key); }
		}
	}
}
