// Copyright 2014 Xamarin Inc. All rights reserved.

using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

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
