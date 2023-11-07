//
// GCController.cs: extensions to GCController iOS API
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013,2015 Xamarin Inc.

#nullable enable

using System;

using ObjCRuntime;
using Foundation;

namespace GameController {

	public partial class GCController {

#if !NET
		// In an undefined enum (GCController.h).
		// the API will use the new enum in XAMCORE_3_0 so the constant is not helpful anymore
		public const int PlayerIndexUnset = -1;
#endif
	}
}
