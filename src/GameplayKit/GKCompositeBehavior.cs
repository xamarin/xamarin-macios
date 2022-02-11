//
// GKCompositeBehavior.cs: Implements some nicer methods for GKCompositeBehavior
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using System.Runtime.Versioning;

namespace GameplayKit {
#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("macos10.12")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class GKCompositeBehavior {

		public GKBehavior this [nuint index] {
			get { return ObjectAtIndexedSubscript (index); }
		}

		public NSNumber this [GKBehavior behavior] {
			get { return ObjectForKeyedSubscript (behavior); }
			set { SetObject (value, behavior); }
		}
	}
}
