//
// NSLevelIndicator: Support for the NSLevelIndicator class
//
// Author:
//   Pavel Sich (pavel.sich@me.com)
//

#if !__MACCATALYST__

using System;
using ObjCRuntime;
using Foundation;
using System.Runtime.Versioning;

namespace AppKit {

#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("maccatalyst")]
#endif
	public partial class NSLevelIndicator {
		public new NSLevelIndicatorCell Cell {
			get { return (NSLevelIndicatorCell)base.Cell; }
			set { base.Cell = value; }
		}
	}
}
#endif // !__MACCATALYST__
