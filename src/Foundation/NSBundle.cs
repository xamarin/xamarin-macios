using System;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

using XamCore.ObjCRuntime;

namespace XamCore.Foundation {
	public partial class NSBundle : NSObject {
		public string LocalizedString (string key, string comment) {
			return LocalizedString (key, "", "");
		}

		public string LocalizedString (string key, string val, string table, string comment) {
			return LocalizedString (key, val, table);
		}

		public string [] PathsForResources (string fileExtension) {
			return PathsForResources (fileExtension, null);
		}
	}
}
