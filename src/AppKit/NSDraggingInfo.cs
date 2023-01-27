#if !__MACCATALYST__
using System;
using System.Runtime.InteropServices;

using CoreGraphics;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace AppKit {
#if NET
	public static partial class INSDraggingInfo_Extensions {
		public static void EnumerateDraggingItems (this INSDraggingInfo self, NSDraggingItemEnumerationOptions enumOpts, NSView view, INSPasteboardReading [] classArray, NSDictionary searchOptions, NSDraggingEnumerator enumerator)
		{
			using var nsa_classArray = NSArray.FromNSObjects (classArray);
			self.EnumerateDraggingItems (enumOpts, view, nsa_classArray.Handle, searchOptions, enumerator);
		}

		public static void EnumerateDraggingItems (this INSDraggingInfo self, NSDraggingItemEnumerationOptions enumOpts, NSView view, NSArray classArray, NSDictionary searchOptions, NSDraggingEnumerator enumerator)
		{
			self.EnumerateDraggingItems (enumOpts, view, classArray.Handle, searchOptions, enumerator);
		}
	}

#else
	public partial class NSDraggingInfo {
		public void EnumerateDraggingItems (NSDraggingItemEnumerationOptions enumOpts, NSView view, NSPasteboardReading [] classArray, NSDictionary searchOptions, NSDraggingEnumerator enumerator)
		{
			var nsa_classArray = NSArray.FromNSObjects (classArray);
			EnumerateDraggingItems (enumOpts, view, nsa_classArray.Handle, searchOptions, enumerator);
			nsa_classArray.Dispose ();
		}

		public void EnumerateDraggingItems (NSDraggingItemEnumerationOptions enumOpts, NSView view, NSArray classArray, NSDictionary searchOptions, NSDraggingEnumerator enumerator)
		{
			EnumerateDraggingItems (enumOpts, view, classArray.Handle, searchOptions, enumerator);
		}
	}
#endif
}
#endif // !__MACCATALYST__
