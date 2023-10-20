#nullable enable

#if MONOMAC

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using ppd_file_s = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace PrintCore {

	public partial class PDEPlugInCallbackProtocol {

#if NET
		[SupportedOSPlatform ("macos13.0")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#endif
		public PMPrintSession? PrintSession {
			get => GetPtrToStruct<PMPrintSession> (_GetPrintSession ());
		}

#if NET
		[SupportedOSPlatform ("macos13.0")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#endif
		public PMPrintSession? PrintSettings {
			get => GetPtrToStruct<PMPrintSession> (_GetPrintSettings ());
		}

#if NET
		[SupportedOSPlatform ("macos13.0")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#endif
		public PMPageFormat? PageFormat {
			get => GetPtrToStruct<PMPageFormat> (_GetPageFormat ());
		}

#if NET
		[SupportedOSPlatform ("macos13.0")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#endif
		public PMPrinter? PMPrinter {
			get => GetPtrToStruct<PMPrinter> (_GetPMPrinter ());
		}

#if NET
		[SupportedOSPlatform ("macos13.0")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
#endif
		public ppd_file_s? PpdFile {
			get => _GetPpdFile ();
		}

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
		internal static T? GetPtrToStruct<T> (IntPtr intPtr) where T : struct
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
		internal static void SetPtrToStruct<T> (T? value, Action<IntPtr> setAction) where T : struct
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
