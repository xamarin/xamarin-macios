using ObjCRuntime;
using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace CloudKit
{
#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class CKRecord
	{
		public NSObject this[string key] {
			get { return _ObjectForKey (key); }
			set { _SetObject (value.Handle, key); }
		}
	}
}
