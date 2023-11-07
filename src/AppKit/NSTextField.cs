//
// NSTextField.cs: Support for the NSTextField class
//

#if !__MACCATALYST__

using System;
using ObjCRuntime;
using Foundation;

#nullable enable

namespace AppKit {

	public partial class NSTextField {
		public new NSTextFieldCell Cell {
			get { return (NSTextFieldCell) base.Cell; }
			set { base.Cell = value; }
		}
	}
}
#endif // !__MACCATALYST__
