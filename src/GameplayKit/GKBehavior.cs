//
// GKBehavior.cs: Implements some nicer methods for GKBehavior
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//
#if XAMCORE_2_0 || !MONOMAC
using System;
using Foundation;

namespace GameplayKit {
	public partial class GKBehavior {

		public GKGoal this [nuint index] {
			get { return ObjectAtIndexedSubscript (index); }
		}

		public NSNumber this [GKGoal goal] {
			get { return ObjectForKeyedSubscript (goal); }
			set { SetObject (value, goal); }
		}
	}
}
#endif
