#if !WATCH
#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Runtime.InteropServices;
using Foundation;
using CoreLocation;
using ObjCRuntime;

namespace MapKit {

	public partial class MKPolygon {

		public static unsafe MKPolygon FromPoints (MKMapPoint [] points)
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			if (points.Length == 0)
				return _FromPoints (IntPtr.Zero, 0);
			
			fixed (MKMapPoint *first = &points [0]){
				return _FromPoints ((IntPtr) first, points.Length);
			}
		}

		public static unsafe MKPolygon FromPoints (MKMapPoint [] points, MKPolygon [] interiorPolygons)
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			if (points.Length == 0)
				return _FromPoints (IntPtr.Zero, 0);
			
			fixed (MKMapPoint *first = &points [0]){
				return _FromPoints ((IntPtr) first, points.Length, interiorPolygons);
			}
		}

		public static unsafe MKPolygon FromCoordinates (CLLocationCoordinate2D [] coords)
		{
			if (coords == null)
				throw new ArgumentNullException ("coords");
			if (coords.Length == 0)
				return _FromCoordinates (IntPtr.Zero, 0);

			fixed (CLLocationCoordinate2D *first = &coords [0]){
				return _FromCoordinates ((IntPtr) first, coords.Length);
			}
		}

		public static unsafe MKPolygon FromCoordinates (CLLocationCoordinate2D [] coords, MKPolygon [] interiorPolygons)
		{
			if (coords == null)
				throw new ArgumentNullException ("coords");
			if (coords.Length == 0)
				return _FromCoordinates (IntPtr.Zero, 0);
			
			fixed (CLLocationCoordinate2D *first = &coords [0]){
				return _FromCoordinates ((IntPtr) first, coords.Length, interiorPolygons);
			}
		}

	}
}
#endif
#endif // !WATCH
