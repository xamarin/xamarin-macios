#if !__MACCATALYST__
using System;
using System.Runtime.InteropServices;

using CoreGraphics;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace AppKit {
#if !NET
	public partial class NSDraggingItem {
		public NSDraggingItem (NSPasteboardWriting pasteboardWriter) : this ((INSPasteboardWriting) pasteboardWriter)
		{
		}
	}
#endif
}
#endif // !__MACCATALYST__
