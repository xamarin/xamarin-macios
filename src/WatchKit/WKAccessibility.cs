using System;
using System.Runtime.InteropServices;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

#if WATCH

namespace XamCore.WatchKit {

	public partial class WKAccessibility {

		[DllImport (Constants.WatchKitLibrary)]
		static extern bool WKAccessibilityIsVoiceOverRunning ();

		static public bool IsVoiceOverRunning {
			get { return WKAccessibilityIsVoiceOverRunning (); }
		}
	}
}

#endif