using System;

using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreGraphics;

namespace XamCore.CoreAnimation {

	partial class CAScrollLayer {

		public CAScroll Scroll {
			get { return CAScrollExtensions.GetValue (ScrollMode); }
			set { ScrollMode = value.GetConstant (); }
		}
	}
}
