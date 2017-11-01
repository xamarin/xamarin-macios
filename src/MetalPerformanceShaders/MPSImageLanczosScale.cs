// Copyright 2015 Xamarin Inc. All rights reserved.

#if IOS

using System;
using System.Runtime.InteropServices;
using XamCore.Foundation;
using XamCore.Metal;
using XamCore.ObjCRuntime;

namespace XamCore.MetalPerformanceShaders {

	public partial class MPSImageLanczosScale {

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

#endif
