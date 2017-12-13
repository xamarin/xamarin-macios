//
// CXProvider extensions and syntax sugar
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


#if XAMCORE_2_0
namespace XamCore.CallKit {
	public partial class CXProvider {

		public CXCallAction [] GetPendingCallActions<T> (NSUuid callUuid)
		{
			return GetPendingCallActions (new Class (typeof (T)), callUuid);
		}
	}
}
#endif // XAMCORE_2_0
