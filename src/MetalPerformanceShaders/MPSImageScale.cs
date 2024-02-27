// Copyright 2015 Xamarin Inc. All rights reserved.

#nullable enable

using System;
using System.Runtime.InteropServices;
using Foundation;
using Metal;
using ObjCRuntime;

namespace MetalPerformanceShaders {

	public partial class MPSImageScale {
		static int size_of_scale_transform = Marshal.SizeOf<MPSScaleTransform> ();

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
					} finally {
						Marshal.FreeHGlobal (ptr);
					}
				} else {
					_SetScaleTransform (IntPtr.Zero);
				}
			}
		}
	}
}
