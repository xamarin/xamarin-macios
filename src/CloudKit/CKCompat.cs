// Copyright 2016, Xamarin Inc. All rights reserved.

#if !XAMCORE_4_0

using XamCore.ObjCRuntime;
using XamCore.Foundation;
using System;

namespace XamCore.CloudKit {
	
	public partial class CKOperation {

		[Obsoleted (PlatformName.iOS, 9,3, message: "Do not use; this API was removed in iOS 9.3 and will always return 0")]
		public virtual ulong ActivityStart ()
		{
			return 0;
		}
	}
}

#endif
