// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Runtime.InteropServices;
using Foundation;
using Metal;
using ObjCRuntime;
using System.Runtime.Versioning;

namespace MetalPerformanceShaders {

#if NET
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("macos10.13")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class MPSImageScale {
		static int size_of_scale_transform = Marshal.SizeOf (typeof(MPSScaleTransform));

		public virtual MPSScaleTransform? ScaleTransform {
			get {
				var ptr = _GetScaleTransform ();
				if (ptr == IntPtr.Zero)
					return null;
				return Marshal.PtrToStructure<MPSScaleTransform> (ptr);
			}
			set {
				if (value.HasValue) {
					IntPtr ptr = Marshal.AllocHGlobal (size_of_scale_transform);
					try {
						Marshal.StructureToPtr<MPSScaleTransform> (value.Value, ptr, false);
						_SetScaleTransform (ptr);
					}
					finally {
						Marshal.FreeHGlobal (ptr);
					}
				} else {
					_SetScaleTransform (IntPtr.Zero);
				}
			}
		}
	}
}
