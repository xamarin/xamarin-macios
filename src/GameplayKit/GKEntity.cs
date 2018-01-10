//
// GKEntity.cs: Implements some nicer methods for GKEntity
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//
#if XAMCORE_2_0 || !MONOMAC
using System;
using Foundation;
using ObjCRuntime;

namespace GameplayKit {
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
#endif
