//
// GCController.cs: extensions to GCController iOS API
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013,2015 Xamarin Inc.

using System;
using System.Runtime.Versioning;

using ObjCRuntime;
using Foundation;

namespace GameController {

#if NET
	[SupportedOSPlatform ("ios7.0")]
	[SupportedOSPlatform ("macos10.9")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class GCController {

#if !NET
		// In an undefined enum (GCController.h).
		// the API will use the new enum in XAMCORE_3_0 so the constant is not helpful anymore
		public const int PlayerIndexUnset = -1;
#endif
	}
}
