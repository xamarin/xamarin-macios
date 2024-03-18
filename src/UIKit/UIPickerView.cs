#if IOS

using System;

using Foundation; 
using CoreGraphics;
using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace UIKit {
	public partial class UIPickerView : UIView, IUITableViewDataSource {
		private UIPickerViewModel model;

		public UIPickerViewModel Model {
			get {
				return model;
			}
			set {
				model = value;
				WeakDelegate = value;
				DataSource = value;
				MarkDirty ();
			}
		}
	}
}

#endif // IOS
