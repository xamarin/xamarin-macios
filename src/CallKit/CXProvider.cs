//
// CXProvider extensions and syntax sugar
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !MONOMAC
using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace CallKit {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	public partial class CXProvider {

		public CXCallAction [] GetPendingCallActions<T> (NSUuid callUuid)
		{
			return GetPendingCallActions (new Class (typeof (T)), callUuid);
		}
	}
}
#endif // !MONOMAC
