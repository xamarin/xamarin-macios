// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MyWebViewApp
{
	[Register ("HybridViewController")]
	partial class HybridViewController
	{
		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		UIKit.UIWebView webView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (webView is not null) {
				webView.Dispose ();
				webView = null;
			}
		}
	}
}
