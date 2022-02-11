#if !__MACCATALYST__
using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace AppKit
{
#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("maccatalyst")]
#endif
	public partial class NSSharingService
	{
		public static NSSharingService GetSharingService (NSSharingServiceName service)
		{
			var constant = service.GetConstant ();
			if (constant == null)
				return null;
			return NSSharingService.GetSharingService (constant);
		}
	}
}
#endif // !__MACCATALYST__
