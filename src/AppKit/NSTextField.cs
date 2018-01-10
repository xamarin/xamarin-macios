//
// NSTextField.cs: Support for the NSTextField class
//
using System;
using ObjCRuntime;
using Foundation;

namespace AppKit {

	public partial class NSTextField {
		public new NSTextFieldCell Cell {
			get { return (NSTextFieldCell)base.Cell; }
			set { base.Cell = value; }
		}
	}
}
