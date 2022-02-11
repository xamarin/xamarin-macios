#if !WATCH
using System;
using System.Runtime.InteropServices;
using Foundation;
using CoreLocation;
using ObjCRuntime;
using System.Runtime.Versioning;

#nullable enable

namespace MapKit {

#if NET
	[SupportedOSPlatform ("tvos9.2")]
	[SupportedOSPlatform ("macos10.9")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	public partial class MKPolygon {

		public static unsafe MKPolygon FromPoints (MKMapPoint [] points)
		{
			if (points == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));
			if (points.Length == 0)
				return _FromPoints (IntPtr.Zero, 0);
			
			fixed (MKMapPoint *first = &points [0]){
				return _FromPoints ((IntPtr) first, points.Length);
			}
		}

		public static unsafe MKPolygon FromPoints (MKMapPoint [] points, MKPolygon [] interiorPolygons)
		{
			if (points == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));
			if (points.Length == 0)
				return _FromPoints (IntPtr.Zero, 0);
			
			fixed (MKMapPoint *first = &points [0]){
				return _FromPoints ((IntPtr) first, points.Length, interiorPolygons);
			}
		}

		public static unsafe MKPolygon FromCoordinates (CLLocationCoordinate2D [] coords)
		{
			if (coords == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (coords));
			if (coords.Length == 0)
				return _FromCoordinates (IntPtr.Zero, 0);

			fixed (CLLocationCoordinate2D *first = &coords [0]){
				return _FromCoordinates ((IntPtr) first, coords.Length);
			}
		}

		public static unsafe MKPolygon FromCoordinates (CLLocationCoordinate2D [] coords, MKPolygon [] interiorPolygons)
		{
			if (coords == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (coords));
			if (coords.Length == 0)
				return _FromCoordinates (IntPtr.Zero, 0);
			
			fixed (CLLocationCoordinate2D *first = &coords [0]){
				return _FromCoordinates ((IntPtr) first, coords.Length, interiorPolygons);
			}
		}

	}
}
#endif // !WATCH
