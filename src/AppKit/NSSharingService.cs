using System;
using Foundation;
using ObjCRuntime;

namespace AppKit
{
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

