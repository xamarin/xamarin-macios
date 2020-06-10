#if IOS

using System;

using Foundation; 
using CoreGraphics;
using ObjCRuntime;

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
