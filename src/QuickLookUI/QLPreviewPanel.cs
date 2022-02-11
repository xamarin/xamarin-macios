using ObjCRuntime;
using System.Runtime.Versioning;
using Foundation;
using CoreGraphics;
using AppKit;

using System;
using System.ComponentModel;

namespace QuickLookUI {
#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public partial class QLPreviewPanel
	{
		public bool EnterFullScreenMode ()
		{
			return EnterFullScreenMode (null, null);
		}

		public void ExitFullScreenModeWithOptions ()
		{
			ExitFullScreenModeWithOptions (null);
		}
	}
}
