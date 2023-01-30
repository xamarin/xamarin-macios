//
// NICompat.cs
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//

#if !XAMCORE_5_0

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

#if !__MACOS__ && !__TVOS__
	public partial class NISession {

#if WATCH
		[Obsolete ("This method was removed and will always throw a InvalidOperationException.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public virtual void SetARSession (ARSession session) => throw new InvalidOperationException (Constants.ApiRemovedGeneral);

		[Obsolete ("This method was removed and will always throw a InvalidOperationException.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public virtual MatrixFloat4x4 GetWorldTransform (NINearbyObject @object) => throw new InvalidOperationException (Constants.ApiRemovedGeneral);
#endif // WATCH

#if __MACCATALYST__
		[Obsolete ("This method was removed and will always throw a InvalidOperationException.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public virtual void SetARSession (NSObject session) => throw new InvalidOperationException (Constants.ApiRemovedGeneral);
#endif
	}
#endif // !__MACOS__ && !__TVOS__
}

#endif // !XAMCORE_5_0
