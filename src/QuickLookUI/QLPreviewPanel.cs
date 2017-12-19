using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreGraphics;
using XamCore.AppKit;

using System;
using System.ComponentModel;

namespace XamCore.QuickLookUI {
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
