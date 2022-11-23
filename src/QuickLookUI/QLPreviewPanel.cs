#nullable enable

using ObjCRuntime;
using Foundation;
using CoreGraphics;
using AppKit;

using System;
using System.ComponentModel;

namespace QuickLookUI {
	public partial class QLPreviewPanel {
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
