#if !__MACCATALYST__
using System;
using System.Runtime.Versioning;

namespace AppKit {
#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("maccatalyst")]
#endif
	public partial class NSPopUpButtonCell {
		public NSMenuItem this [nint idx] {
			get {
				return ItemAt (idx);
			}
		}

		public NSMenuItem this [string title]{
			get {
				return ItemWithTitle (title); 
			}
		}
	}
}
#endif // !__MACCATALYST__
