#nullable enable

using System;
using System.Runtime.InteropServices;

using Foundation;

using ObjCRuntime;

#if WATCH

namespace WatchKit {

	public partial class WKAccessibility {

		[DllImport (Constants.WatchKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool WKAccessibilityIsVoiceOverRunning ();

		static public bool IsVoiceOverRunning {
			get { return WKAccessibilityIsVoiceOverRunning (); }
		}

		[DllImport (Constants.WatchKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool WKAccessibilityIsReduceMotionEnabled ();

		static public bool IsReduceMotionEnabled {
			get { return WKAccessibilityIsReduceMotionEnabled (); }
		}
	}
}

#endif
