// Copyright 2014 Xamarin Inc. All rights reserved.

using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;
using System;

namespace XamCore.CoreWlan {
	public unsafe partial class CWConfiguration {
		public CWNetworkProfile [] NetworkProfiles {
			get {
				NSOrderedSet profiles = _NetworkProfiles;
				if (profiles != null)
					return profiles.ToArray<CWNetworkProfile> ();
				return null;
			}
		}
	}
}
