//
// SKAdNetwork.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Microsoft Corporation.
//

using System;
using Foundation;
using ObjCRuntime;

#if TVOS && !XAMCORE_4_0
namespace StoreKit {
	[Obsolete ("Not usable from tvOS and will be removed in the future.")]
	[Unavailable (PlatformName.TvOS)]
	public class SKAdNetwork : NSObject {

		[Obsolete ("Throws a 'NotSupportedException'.")]
		[Unavailable (PlatformName.TvOS)]
		public static void RegisterAppForAdNetworkAttribution () => throw new NotSupportedException ();
	}
}
#endif
