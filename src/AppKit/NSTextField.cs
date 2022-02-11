//
// NSTextField.cs: Support for the NSTextField class
//

#if !__MACCATALYST__

using System;
using System.Runtime.Versioning;
using ObjCRuntime;
using Foundation;

namespace AppKit {

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("maccatalyst")]
#endif
	public partial class NSTextField {
		public new NSTextFieldCell Cell {
			get { return (NSTextFieldCell)base.Cell; }
			set { base.Cell = value; }
		}
	}
}
#endif // !__MACCATALYST__
