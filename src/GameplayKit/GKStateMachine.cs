//
// GKStateMachine.cs: Implements some nicer methods for GKStateMachine
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
