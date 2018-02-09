using System;

using Foundation;
using ObjCRuntime;
using CoreGraphics;

namespace CoreAnimation {

	partial class CAScrollLayer {

		public CAScroll Scroll {
			get { return CAScrollExtensions.GetValue (ScrollMode); }
			set { ScrollMode = value.GetConstant (); }
		}
	}
}
