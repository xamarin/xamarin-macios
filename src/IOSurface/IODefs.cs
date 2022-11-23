//
// IOSurface
//
// Authors: 
//    Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2017 Microsoft
//

#nullable enable

using System;
using ObjCRuntime;

namespace IOSurface {

	public enum IOSurfaceLockOptions : uint {
		ReadOnly = 1,
		AvoidSync = 2,
	}

	public enum IOSurfacePurgeabilityState : uint {
		NonVolatile = 0,
		Volatile = 1,
		Empty = 2,
		KeepCurrent = 3,
	}

	// To be used with kIOSurfaceCacheMode or IOSurfacePropertyKeyCacheMode
	public enum IOSurfaceMemoryMap {
		DefaultCache = 0,
		InhibitCache = 1 << 8,
		WriteThruCache = 2 << 8,
		CopybackCache = 3 << 8,
		WriteCombineCache = 4 << 8,
		CopybackInnerCache = 5 << 8,
	};


}
