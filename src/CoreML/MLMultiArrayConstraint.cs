//
// MLMultiArrayConstraint.cs
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2017 Microsoft Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using Foundation;
using ObjCRuntime;

namespace CoreML {
	public partial class MLMultiArrayConstraint {
		public nint[] Shape {
			get {
				return MLMultiArray.ConvertArray (_Shape);
			}
		}
	}
}
#endif
