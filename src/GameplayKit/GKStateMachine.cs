//
// GKStateMachine.cs: Implements some nicer methods for GKStateMachine
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#nullable enable

using System;

using Foundation;

using ObjCRuntime;

namespace GameplayKit {
	public partial class GKStateMachine : NSObject {

		public GKState? GetState (Type stateType)
		{
			return GetState (GKState.GetClass (stateType, "stateType"));
		}

		public GKState? GetState (GKState state)
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
