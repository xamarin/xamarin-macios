#if !__MACCATALYST__
using System;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace AppKit {
	public partial class NSSharingService {
		public static NSSharingService? GetSharingService (NSSharingServiceName service)
		{
			var constant = service.GetConstant ();
			if (constant is null)
				return null;
			return NSSharingService.GetSharingService (constant);
		}
	}
}
#endif // !__MACCATALYST__
