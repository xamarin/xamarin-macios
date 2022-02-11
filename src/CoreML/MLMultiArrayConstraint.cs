//
// MLMultiArrayConstraint.cs
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2017 Microsoft Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace CoreML {
#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class MLMultiArrayConstraint {
		public nint[] Shape {
			get {
				return MLMultiArray.ConvertArray (_Shape);
			}
		}
	}
}
