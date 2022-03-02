#if !WATCH // doesn't show up in watch headers
#if !MONOMAC
using System;
using ObjCRuntime;

#nullable enable

namespace MapKit {

	public enum MKMapCameraZoomRangeType {
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
				InitializeHandle (InitWithMinCenterCoordinateDistance (distance));
				break;
			case MKMapCameraZoomRangeType.Max:
				InitializeHandle (InitWithMaxCenterCoordinateDistance (distance));
				break;
			default:
				throw new ArgumentException (nameof (type));
			}
		}
	}
}
#endif
#endif // !WATCH
