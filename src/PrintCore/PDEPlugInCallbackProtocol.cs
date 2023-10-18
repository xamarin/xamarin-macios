#nullable enable

#if MONOMAC

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace PrintCore {

	public partial class PDEPlugInCallbackProtocol {

		// public virtual MPSScaleTransform? ScaleTransform {
		// 	get {
		// 		var ptr = _GetScaleTransform ();
		// 		if (ptr == IntPtr.Zero)
		// 			return null;
		// 		return Marshal.PtrToStructure<MPSScaleTransform> (ptr);
		// 	}
		// 	set {
		// 		if (value.HasValue) {
		// 			IntPtr ptr = Marshal.AllocHGlobal (size_of_scale_transform);
		// 			try {
		// 				Marshal.StructureToPtr<MPSScaleTransform> (value.Value, ptr, false);
		// 				_SetScaleTransform (ptr);
		// 			} finally {
		// 				Marshal.FreeHGlobal (ptr);
		// 			}
		// 		} else {
		// 			_SetScaleTransform (IntPtr.Zero);
		// 		}
		// 	}
		// }

		public PMPrintSession? PrintSession {
			get => GetPtrToStruct<PMPrintSession> (_GetPrintSession ());
		}

		public PMPrintSession? PrintSession {
			get => GetPtrToStruct<PMPrintSession> (_GetPrintSettings ());
		}
		
		public PMPageFormat? PageFormat {
			get => GetPtrToStruct<PMPageFormat> (_GetPageFormat ());
		}
		
		public PMPrinter? PMPrinter {
			get => GetPtrToStruct<PMPrinter> (_GetPMPrinter ());
		}
		
		public ppd_file_s? PpdFile {
			get => GetPtrToStruct<ppd_file_s> (_GetPpdFile ());
		}

		internal T? GetPtrToStruct<T> (IntPtr intPtr) where T : struct
		{
			if (intPtr == IntPtr.Zero)
				return null;
			return Marshal.PtrToStructure<T> (intPtr);
		}

		internal void SetPtrToStruct<T> (object? value, Action<IntPtr> setAction) where T : struct
		{
			if (value.HasValue) {
				var size_of_scale_transform = Marshal.SizeOf<T> ();
				IntPtr ptr = Marshal.AllocHGlobal (size_of_scale_transform);
				try {
					Marshal.StructureToPtr<T> (value.Value, ptr, false);
					setAction (ptr);
				} finally {
					Marshal.FreeHGlobal (ptr);
				}
			} else {
				setAction (IntPtr.Zero);
			}
		}
	}
}

#endif
