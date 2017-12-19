#if !XAMCORE_4_0

using System;

using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreGraphics;

namespace XamCore.CoreAnimation {

	partial class CAScrollLayer {

		[Obsolete ("Use CAScroll enum")]
		public static NSString ScrollBoth {
			get { return CAScroll.Both.GetConstant (); }
		}

		[Obsolete ("Use CAScroll enum")]
		public static NSString ScrollHorizontally {
			get { return CAScroll.Horizontally.GetConstant (); }
		}

		[Obsolete ("Use CAScroll enum")]
		public static NSString ScrollNone {
			get { return CAScroll.None.GetConstant (); }
		}

		[Obsolete ("Use CAScroll enum")]
		public static NSString ScrollVertically {
			get { return CAScroll.Vertically.GetConstant (); }
		}
	}
}

#endif
