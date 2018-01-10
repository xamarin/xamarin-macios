//
// Authors:
//   Rolf Bjarne Kvinge (rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if !WATCH
#if XAMCORE_2_0 || !MONOMAC
using System;
using System.Threading.Tasks;
using System.Threading;

using Foundation;
using ObjCRuntime;
using CoreLocation;

namespace MapKit {
	public partial class MKGeodesicPolyline {
		public static unsafe MKGeodesicPolyline FromPoints (MKMapPoint [] points)
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			if (points.Length == 0)
				return PolylineWithPoints (IntPtr.Zero, 0);

			fixed (MKMapPoint *first = &points [0]){
				return PolylineWithPoints ((IntPtr) first, points.Length);
			}
		}

		public static unsafe MKGeodesicPolyline FromCoordinates (CLLocationCoordinate2D [] coords)
		{
			if (coords == null)
				throw new ArgumentNullException ("coords");
			if (coords.Length == 0)
				return PolylineWithCoordinates (IntPtr.Zero, 0);

			fixed (CLLocationCoordinate2D *first = &coords [0]){
				return PolylineWithCoordinates ((IntPtr) first, coords.Length);
			}
		}
	}
}
#endif
#endif // !WATCH
