//
// CoreMotion's struct and enum definitions used by the API file
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

namespace CoreMotion {

	// CMAccelerometer.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct CMAcceleration {
		public double X;
		public double Y;
		public double Z;

		public CMAcceleration (double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public override string ToString ()
		{
			return String.Format ("a=({0},{1},{2})", X, Y, Z);
		}
	}

	// CMAttitude.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct CMRotationMatrix {
		public double m11;
		public double m12;
		public double m13;
		public double m21;
		public double m22;
		public double m23;
		public double m31;
		public double m32;
		public double m33;
	}

	// CMAttitude.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct CMQuaternion {
		public double x;
		public double y;
		public double z;
		public double w;

		public CMQuaternion (double x, double y, double z, double w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public override string ToString ()
		{
			return String.Format ("quaternion=({0},{1},{2},{3})", x, y, z, w);
		}
	}

	// CMGyro.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct CMRotationRate {
		public double x;
		public double y;
		public double z;

		public CMRotationRate (double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override string ToString ()
		{
			return String.Format ("rotationRate=({0},{1},{2}", x, y, z);
		}
	}

	// untyped enum -> CMDeviceMotion.h
	public enum CMMagneticFieldCalibrationAccuracy {
		Uncalibrated = -1,
		Low,
		Medium,
		High,
	}
}
