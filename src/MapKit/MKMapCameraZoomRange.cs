#if !WATCH // doesn't show up in watch headers
#if XAMCORE_2_0 || !MONOMAC
using System;
using ObjCRuntime;

namespace MapKit {

	public enum  MKMapCameraZoomRange {
		Min,
        Max,
	}

	public partial class MKMapCameraZoomRange {
		public MKMapCameraZoomRange (double distance) : this (distance, MKMapCameraZoomRangeType.Min)
		{
		}

		public MKMapCameraZoomRange (double distance, MKMapCameraZoomRangeType type)
		{
			// two different `init*` would share the same C# signature
			switch (type) {
			case MKMapCameraZoomRangeType.Min:
				Handle = InitWithMinCenterCoordinateDistance (distance);
				break;
			case MKMapCameraZoomRangeType.Max:
				Handle = InitWithMaxCenterCoordinateDistance (distance);
				break;
			default:
				throw new ArgumentException ("type");
			}
		}
	}
}
#endif
#endif // !WATCH
