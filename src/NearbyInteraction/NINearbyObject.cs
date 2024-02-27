//
// NearbyInteraction manual bindings
//
// Authors:
//	Whitney Schmidt  <whschm@microsoft.com>
//
// Copyright 2020 Microsoft Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;
#if NET
using Vector3 = global::System.Numerics.Vector3;
using MatrixFloat4x4 = global::CoreGraphics.NMatrix4;
#else
using Vector3 = global::OpenTK.Vector3;
using MatrixFloat4x4 = global::OpenTK.NMatrix4;
#endif

#if __IOS__ || WATCH
namespace NearbyInteraction {
	partial class NINearbyObject
	{
		static Vector3? _DirectionNotAvailable;

		// TODO: https://github.com/xamarin/maccore/issues/2274
		// We do not have generator support to trampoline Vector3 -> vector_float3 for Fields
		[Field ("NINearbyObjectDirectionNotAvailable",  "NearbyInteraction")]
		public static Vector3 DirectionNotAvailable {
			get {
				if (_DirectionNotAvailable is null) {
					unsafe {
						Vector3 *pointer = (Vector3 *) Dlfcn.GetIndirect (Libraries.NearbyInteraction.Handle, "NINearbyObjectDirectionNotAvailable");
						_DirectionNotAvailable = *pointer;
					}
				}
				return (Vector3)_DirectionNotAvailable;
			}
		}

		static MatrixFloat4x4? _WorldTransformNotAvailable;

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#else
		[iOS (16,0), Watch (9,0), MacCatalyst (16,0)]
#endif // NET
		// Following similar strategy found here: https://github.com/xamarin/maccore/issues/2274
		[Field ("NINearbyObjectWorldTransformNotAvailable",  "NearbyInteraction")]
		public static MatrixFloat4x4 WorldTransformNotAvailable {
			get {
				if (_WorldTransformNotAvailable is null) {
					unsafe {
						MatrixFloat4x4 *pointer = (MatrixFloat4x4 *) Dlfcn.GetIndirect (Libraries.NearbyInteraction.Handle, "NINearbyObjectWorldTransformNotAvailable");
						if (pointer is null)
							throw new PlatformNotSupportedException ("This property is not supported on this version of the OS");
						_WorldTransformNotAvailable = *pointer;
					}
				}
				return (MatrixFloat4x4)_WorldTransformNotAvailable;
			}
		}
	}
}
#endif
