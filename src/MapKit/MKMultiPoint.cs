#if !WATCH
#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Runtime.InteropServices;
using XamCore.Foundation;
using XamCore.CoreLocation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.MapKit {

	public partial class MKMultiPoint {
		public unsafe MKMapPoint [] Points {
			get {
				var source = (MKMapPoint *) _Points;
				nint n = PointCount;
				var result = new MKMapPoint [n];
				for (int i = 0; i < n; i++)
					result [i] = source [i];

				return result;
			}
		}

		public unsafe CLLocationCoordinate2D [] GetCoordinates (int first, int count)
		{
			var range = new NSRange (first, count);
			var target = new CLLocationCoordinate2D [count];
			fixed (CLLocationCoordinate2D *firstE = &target [0]){
				GetCoords ((IntPtr) firstE, range);
			}
			return target;
		}
	}
}
#endif
#endif // !WATCH
