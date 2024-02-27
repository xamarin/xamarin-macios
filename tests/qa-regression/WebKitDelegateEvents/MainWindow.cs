using System;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace WebKitDelegateEvents {
	public partial class MainWindow : NSWindow {
		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}
	}
}
