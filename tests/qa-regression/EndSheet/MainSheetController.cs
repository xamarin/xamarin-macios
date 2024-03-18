using System;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace EndSheet {
	public partial class MainSheetController : NSWindowController {
		public MainSheetController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainSheetController (NSCoder coder) : base (coder)
		{
		}

		public MainSheetController () : base ("MainSheet")
		{
		}

		public override void AwakeFromNib ()
		{
			dismissSheetButton.Activated += (sender, e) => {
				NSApplication.SharedApplication.EndSheet (Window);
				Window.OrderOut (this);
			};
		}
	}
}
