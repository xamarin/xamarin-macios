using System;
using System.Runtime.Versioning;

namespace CloudKit {

#if NET
	[SupportedOSPlatform ("ios8.0")]
	[SupportedOSPlatform ("macos10.10")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class CKRecordZoneID {
#if !XAMCORE_3_0
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public CKRecordZoneID ()
		{
		}
#endif
	}
}
