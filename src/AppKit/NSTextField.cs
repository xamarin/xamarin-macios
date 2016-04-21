//
// NSTextField.cs: Support for the NSTextField class
//
using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.AppKit {

	public partial class NSTextField {
		public new NSTextFieldCell Cell {
			get { return (NSTextFieldCell)base.Cell; }
			set { base.Cell = value; }
		}
	}
}
