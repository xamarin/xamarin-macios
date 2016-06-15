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

#if !MONOMAC || XAMCORE_2_0
namespace XamCore.CallKit {
	public partial class CXProvider {

		public CXCallAction [] GetPendingCallActions<T> (NSUuid callUuid)
		{
			return GetPendingCallActions (new Class (typeof (T)), callUuid);
		}
	}
}
#endif // !MONOMAC || XAMCORE_2_0
