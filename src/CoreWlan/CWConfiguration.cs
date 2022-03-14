// Copyright 2014 Xamarin Inc. All rights reserved.
#if !__MACCATALYST__

#nullable enable

using Foundation;
using CoreFoundation;
using ObjCRuntime;
using System;

namespace CoreWlan {
	public unsafe partial class CWConfiguration {
		public CWNetworkProfile []? NetworkProfiles {
			get {
				NSOrderedSet profiles = _NetworkProfiles;
				return profiles?.ToArray<CWNetworkProfile> ();
			}
		}
	}
}
#endif
