#nullable enable

namespace BindingHelpers
{
	internal static class BindingHelpers {
		// this method is helpful for the getter for something like:
		// unsafe PMPrinter* PMPrinter { get; }
		//
		// we can change this to:
		// [Internal]
		// [Export ("PMPrinter")]
		// IntPtr _GetPMPrinter ();
		//
		// and then add this to the manual file:
		// public PMPrinter? PMPrinter {
		//    get => BindingHelpers.GetPtrToStruct<PMPrinter> (_GetPMPrinter ());
		// }
		public static T? GetPtrToStruct<T> (IntPtr intPtr) where T : struct
		{
			if (intPtr == IntPtr.Zero)
				return null;
			return Marshal.PtrToStructure<T> (intPtr);
		}

		// this method is helpful for the setter for something like:
		// unsafe MPSScaleTransform* ScaleTransform { get; set; }
		//
		// we can change this to:
		// [Export ("setScaleTransform:")]
		// [Internal]
		// void _SetScaleTransform (IntPtr value);
		//
		// and then add this to the manual file:
		// public MPSScaleTransform? ScaleTransform {
		//    ...
		//    set => BindingHelpers.SetPtrToStruct<MPSScaleTransform> (value, (ptr) => _SetScaleTransform (ptr));
		// }
		public static void SetPtrToStruct<T> (object? value, Action<IntPtr> setAction) where T : struct
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