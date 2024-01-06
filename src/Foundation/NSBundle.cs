using System;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {
	public partial class NSBundle : NSObject {

#if !NET
		[Obsolete ("Use 'GetLocalizedString' instead.")]
		public virtual string LocalizedString (string key, string value, string table)
		{
			return (string) GetLocalizedString ((NSString) key, (NSString) value, (NSString) table);
		}

		[Obsolete ("Use 'GetLocalizedString' instead.")]
		public string LocalizedString (string key, string comment)
		{
			return LocalizedString (key, "", "");
		}

		[Obsolete ("Use 'GetLocalizedString' instead.")]
		public string LocalizedString (string key, string val, string table, string comment)
		{
			return LocalizedString (key, val, table);
		}
#endif
		public NSString GetLocalizedString (string key, string value = null, string table = null)
		{
			return GetLocalizedString ((NSString) key, (NSString) value, (NSString) table);
		}

		public string [] PathsForResources (string fileExtension)
		{
			return PathsForResources (fileExtension, null);
		}
	}
}
