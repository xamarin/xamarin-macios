#nullable enable

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace CarPlay {

	[SupportedOSPlatform ("ios26.4")]
	[UnsupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("tvos")]
	[UnsupportedOSPlatform ("macos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct CPLocationCoordinate3D {
		double latitude;
		double longitude;
		double altitude;

#if !COREBUILD
		public double Latitude { get => latitude; set => latitude = value; }
		public double Longitude { get => longitude; set => longitude = value; }
		public double Altitude { get => altitude; set => altitude = value; }
#endif
	}
}
