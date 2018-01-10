using System;
using System.Runtime.InteropServices;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace AppKit {
	public partial class NSPasteboard {
#if !XAMCORE_2_0
		// This is a stable API we can't break, even if it is all kinds of wrong
		public bool WriteObjects (NSPasteboardReading [] objects)
		{
			var nsa_pasteboardReading = NSArray.FromNSObjects (objects);
			bool result = WriteObjects (nsa_pasteboardReading.Handle);
			nsa_pasteboardReading.Dispose ();
			return result;
		}
#endif

		public bool WriteObjects (INSPasteboardWriting [] objects)
		{
			var nsa_pasteboardReading = NSArray.FromNSObjects (objects);
			bool result = WriteObjects (nsa_pasteboardReading.Handle);
			nsa_pasteboardReading.Dispose ();
			return result;
		}
		
		public bool WriteObjects (NSPasteboardWriting [] objects)
		{
			var nsa_pasteboardReading = NSArray.FromNSObjects (objects);
			bool result = WriteObjects (nsa_pasteboardReading.Handle);
			nsa_pasteboardReading.Dispose ();
			return result;
		}
	}
}
