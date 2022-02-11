#if !WATCH // doesn't show up in watch headers
#if !MONOMAC
using System;
using ObjCRuntime;
using System.Runtime.Versioning;

#nullable enable

namespace MapKit {

	public enum MKMapCameraZoomRangeType {
		Min,
		Max,
	}

#if NET
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("macos10.15")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
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
