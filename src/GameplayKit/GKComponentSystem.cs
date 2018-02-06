//
// GKComponentSystem.cs: Implements some nicer methods for GKComponentSystem
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//
#if XAMCORE_2_0 // GKComponentSystem is a generic type, which we only support in Unified (for now at least)
using System;
using Foundation;
using ObjCRuntime;

namespace GameplayKit {
	public partial class GKComponentSystem<TComponent> {
		
		public GKComponentSystem ()
			: this (GKState.GetClass (typeof (TComponent), "componentType"))
		{
		}

		public Type ComponentType { 
			get { return Class.Lookup (ComponentClass); }
		}

		public TComponent this [nuint index] {
			get { return ObjectAtIndexedSubscript (index); }
		}
	}
}
#endif
