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
#if IOS
		[Obsolete ("Starting with ios7.0 use 'MKOverlayRenderer' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[TV (9,2)]
		[Mac (10,9)]
#endif
		[DllImport (Constants.MapKitLibrary)]
		public static extern nfloat MKRoadWidthAtZoomScale (/* MKZoomScale */ nfloat zoomScale);
	}
}

#endif
