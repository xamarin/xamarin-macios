using System;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace EndSheet {
	public partial class MainWindowController : NSWindowController {
		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		public MainWindowController () : base ("MainWindow")
		{
		}

		public override void AwakeFromNib ()
		{
			runSheetButton.Activated += (sender, e) =>
				NSApplication.SharedApplication.BeginSheet (
					new MainSheetController ().Window, Window, () =>
						dismissCountLabel.IntValue++);
		}
	}
}
