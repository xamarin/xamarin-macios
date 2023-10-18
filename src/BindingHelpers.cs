#nullable enable

namespace BindingHelpers
{
	internal static class BindingHelpers {
		public static T? GetPtrToStruct<T> (IntPtr intPtr) where T : struct
		{
			if (intPtr == IntPtr.Zero)
				return null;
			return Marshal.PtrToStructure<T> (intPtr);
		}

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