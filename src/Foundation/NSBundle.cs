using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

using ObjCRuntime;

#nullable enable

namespace Foundation {
	public partial class NSBundle : NSObject {
		public NSString GetLocalizedString (string key, string? value = null, string? table = null)
		{
			return GetLocalizedString ((NSString) key, (NSString?) value, (NSString?) table);
		}

		public string [] PathsForResources (string fileExtension)
		{
			return PathsForResources (fileExtension, null);
		}

#if !MONOMAC && !XAMCORE_5_0
		[Obsolete ("Do not use this constructor, it does not work as expected.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public NSBundle ()
			: base (NSObjectFlag.Empty)
		{
		}
#endif // !MONOMAC && !XAMCORE_5_0
	}
}
