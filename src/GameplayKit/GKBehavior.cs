//
// GKBehavior.cs: Implements some nicer methods for GKBehavior
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using System.Runtime.Versioning;

namespace GameplayKit {
#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
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
