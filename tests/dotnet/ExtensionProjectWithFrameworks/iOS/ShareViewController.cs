using System;
using System.Drawing;

using Foundation;

using Social;

using UIKit;

namespace MyShareExtension {
	public partial class ShareViewController : SLComposeServiceViewController {
		public ShareViewController (IntPtr handle) : base (handle)
		{
		}

		public override bool IsContentValid ()
		{
			// Do validation of contentText and/or NSExtensionContext attachments here
			return true;
		}

		public override void DidSelectPost ()
		{
			// This is called after the user selects Post. Do the upload of contentText and/or NSExtensionContext attachments.

			// Inform the host that we're done, so it un-blocks its UI. Note: Alternatively you could call super's -didSelectPost, which will similarly complete the extension context.
			ExtensionContext.CompleteRequest (null, null);
		}

		public override SLComposeSheetConfigurationItem [] GetConfigurationItems ()
		{
			// To add configuration options via table cells at the bottom of the sheet, return an array of SLComposeSheetConfigurationItem here.
			return new SLComposeSheetConfigurationItem [0];
		}
	}
}
