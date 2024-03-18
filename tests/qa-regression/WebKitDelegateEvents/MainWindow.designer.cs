// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace WebKitDelegateEvents
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField locationTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSegmentedControl navigationButtons { get; set; }

		[Outlet]
		MonoMac.AppKit.NSOutlineView outlineView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton populateOutlineViewButton { get; set; }

		[Outlet]
		MonoMac.WebKit.WebView webView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (locationTextField is not null) {
				locationTextField.Dispose ();
				locationTextField = null;
			}

			if (navigationButtons is not null) {
				navigationButtons.Dispose ();
				navigationButtons = null;
			}

			if (webView is not null) {
				webView.Dispose ();
				webView = null;
			}

			if (outlineView is not null) {
				outlineView.Dispose ();
				outlineView = null;
			}

			if (populateOutlineViewButton is not null) {
				populateOutlineViewButton.Dispose ();
				populateOutlineViewButton = null;
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
