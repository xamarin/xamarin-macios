//
// NSUrlConnection.cs:
// Author:
//   Miguel de Icaza
// Copyright 2011, 2012 Xamarin Inc
//

using System;
using System.ComponentModel;
using System.Collections;
using System.Runtime.InteropServices;

using ObjCRuntime;

#nullable enable

namespace Foundation {
#if !XAMCORE_5_0 && __WATCHOS__
	public partial class NSUrlConnection {
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This API is not available on this platform.")]
		public static NSData? SendSynchronousRequest (NSUrlRequest request, out NSUrlResponse? response, out NSError? error)
		{
			// This method was added to watchOS by mistake, the corresponding native API does not exist on watchOS.
			throw new PlatformNotSupportedException ();
		}
	}
#endif // !XAMCORE_5_0 && __WATCHOS__
}
