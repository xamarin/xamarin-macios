//
// GKState.cs: Implements some nicer methods for GKState
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
	public partial class GKState {

		internal static Class GetClass (Type type, string parameterName)
		{
			if (type is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameterName));

			var klass = new Class (type);
			// most API do not accept null so we throw in managed code instead of crashing the app
			if (klass.Handle == IntPtr.Zero)
				throw new ArgumentException ("Not an type exposed to ObjC", parameterName);

			return klass;
		}

		internal static Class GetClass (NSObject instance, string parameterName)
		{
			if (instance is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (parameterName));

			var klass = instance.Class;
			if ((klass is null) || (klass.Handle == IntPtr.Zero))
				throw new ArgumentException ("Not an type exposed to ObjC", parameterName);

			return klass;
		}

		// helper - cannot be virtual as it would not be called from GameplayKit/ObjC
		public bool IsValidNextState (Type stateType)
		{
			return IsValidNextState (GetClass (stateType, "stateType"));
		}

		// helper [#32844] - cannot be virtual as it would not be called from GameplayKit/ObjC
		public bool IsValidNextState (GKState state)
		{
			return IsValidNextState (GetClass (state, "state"));
		}
	}
}
