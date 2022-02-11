#if !NET
using System;

using Foundation;
using ObjCRuntime;
using CoreGraphics;

#nullable enable

namespace CoreAnimation {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	partial class CAScrollLayer {

		public CAScroll Scroll {
			get { return CAScrollExtensions.GetValue (ScrollMode); }
			set { ScrollMode = value.GetConstant ()!; }
		}
	}
}
#endif
