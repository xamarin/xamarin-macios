//
// 
//

#if !WATCH

using System;
using System.Runtime.InteropServices;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.MapKit;

namespace XamCore.MapKit {
	public partial class MKOverlayView {

		[TV (9,2)]
		[Mac (10,9, onlyOn64 : true)]
		[DllImport (Constants.MapKitLibrary)]
		public static extern nfloat MKRoadWidthAtZoomScale (/* MKZoomScale */ nfloat zoomScale);
	}
}

#endif