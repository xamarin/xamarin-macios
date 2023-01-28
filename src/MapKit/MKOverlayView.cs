//
// 
//

#if !XAMCORE_5_0
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
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[ObsoletedOSPlatform ("ios", "Use 'MKOverlayRenderer.MKRoadWidthAtZoomScale' instead.")]
		[ObsoletedOSPlatform ("macos", "Use 'MKOverlayRenderer.MKRoadWidthAtZoomScale' instead.")]
		[ObsoletedOSPlatform ("tvos", "Use 'MKOverlayRenderer.MKRoadWidthAtZoomScale' instead.")]
		[ObsoletedOSPlatform ("maccatalyst", "Use 'MKOverlayRenderer.MKRoadWidthAtZoomScale' instead.")]
#else
		[Obsolete ("Use 'MKOverlayRenderer.MKRoadWidthAtZoomScale' instead.")]
		[TV (9, 2)]
		[Mac (10, 9)]
#endif
		[DllImport (Constants.MapKitLibrary)]
		public static extern nfloat MKRoadWidthAtZoomScale (/* MKZoomScale */ nfloat zoomScale);
	}
}

#endif
#endif // !XAMCORE_5_0
