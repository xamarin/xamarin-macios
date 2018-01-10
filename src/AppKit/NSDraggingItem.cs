using System;
using System.Runtime.InteropServices;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace AppKit {
	public partial class NSDraggingItem {
		public NSDraggingItem (NSPasteboardWriting pasteboardWriter) : this ((INSPasteboardWriting)pasteboardWriter) {
		}
	}
}