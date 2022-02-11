//
// GKEntity.cs: Implements some nicer methods for GKEntity
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace GameplayKit {
#if NET
	[SupportedOSPlatform ("ios9.0")]
	[SupportedOSPlatform ("macos10.11")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class GKEntity {

		public void RemoveComponent (Type componentType)
		{
			RemoveComponent (GKState.GetClass (componentType, "componentType"));
		}

		public GKComponent GetComponent (Type componentType)
		{
			return GetComponent (GKState.GetClass (componentType, "componentType"));
		}
	}
}
