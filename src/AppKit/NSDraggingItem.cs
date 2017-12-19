using System;
using System.Runtime.InteropServices;

using XamCore.CoreGraphics;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.AppKit {
	public partial class NSDraggingItem {
		public NSDraggingItem (NSPasteboardWriting pasteboardWriter) : this ((INSPasteboardWriting)pasteboardWriter) {
		}
	}
}