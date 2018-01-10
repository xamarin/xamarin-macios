//
// GCController.cs: extensions to GCController iOS API
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013,2015 Xamarin Inc.

using System;

using ObjCRuntime;
using Foundation;

namespace GameController {

	[iOS (9,0)][Mac (10,11)]
	[Native]
	public enum GCControllerPlayerIndex : long {
		Unset = -1,
		Index1 = 0,
		Index2,
		Index3,
		Index4
	}

	public partial class GCController {

#if !XAMCORE_4_0
		// In an undefined enum (GCController.h).
		// the API will use the new enum in XAMCORE_3_0 so the constant is not helpful anymore
		public const int PlayerIndexUnset = -1;
#endif
	}
}
