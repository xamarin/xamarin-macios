using System;

namespace AppKit {
	public partial class NSPopUpButtonCell {
		public NSMenuItem this [nint idx] {
			get {
				return ItemAt (idx);
			}
		}

		public NSMenuItem this [string title]{
			get {
				return ItemWithTitle (title); 
			}
		}
	}
}