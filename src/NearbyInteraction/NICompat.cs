//
// NICompat.cs
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Foundation;
using CoreFoundation;
using ObjCRuntime;

#if __MACCATALYST__ || !IOS
using ARSession = Foundation.NSObject;
#else
using ARKit;
#endif

#if NET
using Vector3 = global::System.Numerics.Vector3;
using MatrixFloat4x4 = global::CoreGraphics.NMatrix4;
#else
using NativeHandle = System.IntPtr;
using Vector3 = global::OpenTK.Vector3;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
#endif

#nullable enable
namespace NearbyInteraction {

#if WATCH
	public partial class NISession {

#if !NET
		[Obsolete ("This method was removed and will always throw a InvalidOperationException.")]
#endif
		public virtual void SetARSession (ARSession session) => throw new InvalidOperationException (Constants.ApiRemovedGeneral);

#if !NET
		[Obsolete ("This method was removed and will always throw a InvalidOperationException.")]
#endif
		public virtual MatrixFloat4x4 GetWorldTransform (NINearbyObject @object) => throw new InvalidOperationException (Constants.ApiRemovedGeneral);

	}
#endif
}
