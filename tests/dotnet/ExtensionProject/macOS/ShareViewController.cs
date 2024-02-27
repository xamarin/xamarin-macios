using System;
using System.Drawing;

using NotificationCenter;
using Foundation;
using Social;
using AppKit;
using System.Linq;

namespace ShareExtensionTest {
	public partial class ShareViewController : NSViewController {
		public ShareViewController (IntPtr handle) : base (handle)
		{
		}

		public override void LoadView ()
		{
			base.LoadView ();

			NSExtensionItem item = ExtensionContext.InputItems.First ();
			Console.WriteLine ("Attachments {0}", item);
		}

		partial void Cancel (Foundation.NSObject sender)
		{
			NSExtensionItem outputItem = new NSExtensionItem ();
			var outputItems = new [] { outputItem };
			ExtensionContext.CompleteRequest (outputItems, null);
		}

		partial void Send (Foundation.NSObject sender)
		{
			NSError cancelError = NSError.FromDomain (NSError.CocoaErrorDomain, 3072, null);
			ExtensionContext.CancelRequest (cancelError);
		}
	}
}
