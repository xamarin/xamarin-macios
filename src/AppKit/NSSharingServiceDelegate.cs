using System;
using System.Runtime.InteropServices;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace AppKit {
	public partial class NSSharingServiceDelegate {
		CGRect SourceFrameOnScreenForShareItem (NSSharingService sharingService, NSPasteboardWriting item)
		{
			return SourceFrameOnScreenForShareItem (sharingService, (INSPasteboardWriting)item);
		}

		NSImage TransitionImageForShareItem (NSSharingService sharingService, NSPasteboardWriting item, CGRect contentRect)
		{
			return TransitionImageForShareItem (sharingService, (INSPasteboardWriting)item, contentRect);
		}
	}
}