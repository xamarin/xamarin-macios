// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace EndSheet
{
	[Register ("MainSheetController")]
	partial class MainSheetController
	{
		[Outlet]
		MonoMac.AppKit.NSButton dismissSheetButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (dismissSheetButton is not null) {
				dismissSheetButton.Dispose ();
				dismissSheetButton = null;
			}
		}
	}

	[Register ("MainSheet")]
	partial class MainSheet
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
