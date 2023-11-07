using System;
using System.Drawing;

using Foundation;
using UIKit;

namespace MyDocumentPickerExtension {
	public partial class DocumentPickerViewController : UIDocumentPickerExtensionViewController {
		public DocumentPickerViewController (IntPtr handle) : base (handle)
		{
		}

		public override void PrepareForPresentation (UIDocumentPickerMode mode)
		{
			base.PrepareForPresentation (mode);
		}

		partial void OpenDocument (NSObject sender)
		{
			var url = DocumentStorageUrl.Append ("Untitled.txt", false);
			DismissGrantingAccess (url);
		}
	}
}
