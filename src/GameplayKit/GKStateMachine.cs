//
// GKStateMachine.cs: Implements some nicer methods for GKStateMachine
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//
#if XAMCORE_2_0 || !MONOMAC
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.GameplayKit {
	public partial class GKStateMachine : NSObject {
		
		public GKState GetState (Type stateType)
		{
			return GetState (GKState.GetClass (stateType, "stateType"));
		}

		public GKState GetState (GKState state)
		{
			return GetState (GKState.GetClass (state, "state"));
		}

		public bool CanEnterState (Type stateType)
		{
			return CanEnterState (GKState.GetClass (stateType, "stateType"));
		}

		public bool CanEnterState (GKState state)
		{
			return CanEnterState (GKState.GetClass (state, "state"));
		}

		public virtual bool EnterState (Type stateType)
		{
			return EnterState (GKState.GetClass (stateType, "stateType"));
		}

		public virtual bool EnterState (GKState state)
		{
			return EnterState (GKState.GetClass (state, "state"));
		}
	}
}
#endif