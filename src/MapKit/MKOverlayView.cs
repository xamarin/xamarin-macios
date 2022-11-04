//
// 
//

#if !WATCH

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using MapKit;

#nullable enable

namespace MapKit {
	public partial class MKOverlayView {

#if NET
		[SupportedOSPlatform ("tvos9.2")]
		[SupportedOSPlatform ("macos10.9")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios7.0")]
		[ObsoletedOSPlatform ("ios7.0", "Use 'MKOverlayRenderer' instead.")]
#else
		[TV (9,2)]
		[Mac (10,9)]
#endif
		[DllImport (Constants.MapKitLibrary)]
		public static extern nfloat MKRoadWidthAtZoomScale (/* MKZoomScale */ nfloat zoomScale);
	}
}

#endif
