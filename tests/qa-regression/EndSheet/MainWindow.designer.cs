// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace EndSheet
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSButton runSheetButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField dismissCountLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (runSheetButton is not null) {
				runSheetButton.Dispose ();
				runSheetButton = null;
			}

			if (dismissCountLabel is not null) {
				dismissCountLabel.Dispose ();
				dismissCountLabel = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
