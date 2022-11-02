//
// GKCompositeBehavior.cs: Implements some nicer methods for GKCompositeBehavior
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#nullable enable

using System;
using Foundation;

namespace GameplayKit {
	public partial class GKCompositeBehavior {

		public new GKBehavior this [nuint index] {
			get { return ObjectAtIndexedSubscript (index); }
		}

		public NSNumber this [GKBehavior behavior] {
			get { return ObjectForKeyedSubscript (behavior); }
			set { SetObject (value, behavior); }
		}
	}
}
