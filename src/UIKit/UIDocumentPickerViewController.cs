#if !XAMCORE_3_0

using System;

namespace XamCore.UIKit {

	public partial class UIDocumentPickerViewController {
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public UIDocumentPickerViewController ()
		{
		}
	}
}

#endif
