#if !__MACCATALYST__
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace AppKit {
#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("maccatalyst")]
#endif
	public partial class NSPasteboard {
		public bool WriteObjects (INSPasteboardWriting [] objects)
		{
			var nsa_pasteboardReading = NSArray.FromNSObjects (objects);
			bool result = WriteObjects (nsa_pasteboardReading.Handle);
			nsa_pasteboardReading.Dispose ();
			return result;
		}
		
#if !NET
		public bool WriteObjects (NSPasteboardWriting [] objects)
		{
			var nsa_pasteboardReading = NSArray.FromNSObjects (objects);
			bool result = WriteObjects (nsa_pasteboardReading.Handle);
			nsa_pasteboardReading.Dispose ();
			return result;
		}
#endif
	}
}
#endif // !__MACCATALYST__
